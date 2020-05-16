using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.NeuralNet
{
    [System.Serializable]
    public class Genotype
    {
        public List<NodeGene> inputs;
        public List<NodeGene> outputs;
        public List<NodeGene> hidden;
        public List<NodeGene> all;
        public List<LinkGene> links;

        #region Properties

        public int NodeCount
        {
            get; private set;
        }

        public int LinkCount
        {
            get => links.Count;
        }

        public int InputCount
        {
            get => inputs.Count;
        }

        public int OutputCount
        {
            get => outputs.Count;
        }

        public int HiddenCount
        {
            get => hidden.Count;
        }

        #endregion Properties

        public Genotype(TopologyDescriptor descriptor) : this()
        {
            int lenght = descriptor.inputCount + descriptor.hiddenCount + descriptor.outputCount;
            for (int i = 0; i < lenght; i++)
            {
                NodeGene current = new NodeGene(i + 1, TMath.Tanh);
                if (i < descriptor.inputCount)
                {
                    current.SetType(NodeType.INPUT);
                }
                else if (i > lenght - descriptor.outputCount - 1)
                {
                    current.SetType(NodeType.OUTPUT);
                }
                else
                {
                    current.SetType(NodeType.HIDDEN);
                }
                AddNode(current);
            }

            int linkN = 1;
            foreach (LinkDescriptor link in descriptor.links)
            {
                AddLink(link, linkN++, UnityEngine.Random.Range(-1F, 1F));
            }
        }

        public Genotype()
        {
            inputs = new List<NodeGene>();
            outputs = new List<NodeGene>();
            hidden = new List<NodeGene>();
            all = new List<NodeGene>();
            links = new List<LinkGene>();
        }

        public Genotype Crossover(Genotype partner)
        {
            Genotype childGen = new Genotype();
            ////TODO IMPROVE
            foreach (NodeGene gene in inputs)
            {
                childGen.AddNode(gene.CopyNoLinks());
            }
            foreach (NodeGene gene in outputs)
            {
                childGen.AddNode(gene.CopyNoLinks());
            }
            List<LinkGene> remaining = new List<LinkGene>(links);
            // Zip togheter the links that have the same innovation number
            List<Tuple<LinkGene, LinkGene>> zippedLinks = TUtilsProvider.ZipWithPredicate(links, partner.links, (item1, item2) => item1.GetInnovationNumber().Equals(item2.GetInnovationNumber()));

            // Add to che child all the matching genes (links)
            foreach (Tuple<LinkGene, LinkGene> gene in zippedLinks)
            {
                LinkGene copy;
                if (UnityEngine.Random.Range(0F, 1F) < 0.5F)
                {
                    copy = gene.Item1.Copy();
                }
                else
                {
                    copy = gene.Item2.Copy();
                }
                if (!childGen.all.Contains(copy.From()))
                {
                    childGen.AddNode(copy.From().CopyNoLinks());
                }
                if (!childGen.all.Contains(copy.To()))
                {
                    childGen.AddNode(copy.To().CopyNoLinks());
                }
                childGen.AddLink(new LinkDescriptor(copy.From().id, copy.To().id), copy.GetInnovationNumber(), copy.GetWeight());
                remaining.RemoveAll(item => item.GetInnovationNumber().Equals(copy.GetInnovationNumber()));
            }
            //// At this point all common genes are added we add all the disjoint genes from the fittest
            foreach (LinkGene gene in remaining)
            {
                childGen.AddLink(new LinkDescriptor(gene.From().id, gene.To().id), gene.GetInnovationNumber(), gene.GetWeight());
            }

            return childGen;
        }

        public void Mutate(float mutationRate)
        {
            if (UnityEngine.Random.Range(0F, 1F) < mutationRate * 5)
            {
                if (UnityEngine.Random.Range(0F, 1F) < 0.5F)
                {
                    LinkGene random = links.ElementAt(UnityEngine.Random.Range(0, links.Count));
                    NodeGene newNode = new NodeGene(GetCurrentNodeMaxId() + 1, TMath.Tanh, NodeType.HIDDEN);
                    TopologyMutation mutation = new TopologyMutation(TopologyMutationType.SPLIT_LINK, random);
                    mutation.SetInnovationNumber(GlobalParams.GetGenerationInnovationNumber(mutation));
                    LinkGene newLink = new LinkGene(random.From(), newNode, 1F, mutation.GetInnovationNumber());
                    newNode.AddIncomingLink(newLink);
                    random.SetFrom(newNode);
                    AddNode(newNode);
                    AddLinkRaw(newLink);
                    Debug.Log("SPLIT LINK MUTATION HAPPENED");
                }
                else
                {
                    // Create a list of all nodes except output nodes to select the random From node
                    List<NodeGene> temp = new List<NodeGene>(inputs);
                    temp.AddRange(hidden);
                    // Select a random From node
                    int index = UnityEngine.Random.Range(0, temp.Count);
                    NodeGene fromRandom = temp.ElementAt(index);

                    // Create now the list of all nodes except input nodes to select the random To node, the node can only be a forward node
                    // in respect of the From node, the net is not recurrent
                    temp = new List<NodeGene>(outputs);
                    temp.Remove(fromRandom);
                    // Remove all the back nodes
                    foreach (NodeGene gene in temp)
                    {
                        if (gene.id < fromRandom.id)
                        {
                            temp.Remove(gene);
                        }
                    }
                    if (temp.Count > 0)
                    {
                        NodeGene toRandom = temp.ElementAt(UnityEngine.Random.Range(0, temp.Count));

                        LinkGene newLink = new LinkGene(fromRandom, toRandom, UnityEngine.Random.Range(-1F, 1F));
                        if (!links.Contains(newLink))
                        {
                            TopologyMutation mutation = new TopologyMutation(TopologyMutationType.ADD_LINK, newLink);
                            mutation.SetInnovationNumber(GlobalParams.GetGenerationInnovationNumber(mutation));
                            newLink.SetInnovationNumber(mutation.GetInnovationNumber());
                            AddLinkRaw(newLink);
                            Debug.Log("ADDING LINK MUTATION HAPPENED");
                        }
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (UnityEngine.Random.Range(0F, 1F) < mutationRate)
                {
                    LinkGene random = links.ElementAt(UnityEngine.Random.Range(0, links.Count));
                    random.SetWeight(UnityEngine.Random.Range(-1F, 1F));
                    Debug.Log("WEIGHT MUTATION HAPPENED: " + random.ToString());
                }
            }
        }

        private LinkGene AddLink(LinkDescriptor link, int innovationNumber, double weight)
        {
            LinkGene newLink = null;
            NodeGene from = null;
            NodeGene to = null;
            foreach (NodeGene node in all)
            {
                if (node.id.Equals(link.fromId))
                {
                    from = node;
                }
                if (node.id.Equals(link.toId))
                {
                    to = node;
                }
                if (from != null && to != null) break;
            }
            if (to != null && from != null)
            {
                newLink = new LinkGene(from, to, weight, innovationNumber);
                to.AddIncomingLink(newLink);
                links.Add(newLink);
            }
            return newLink;
        }

        private void AddLinkRaw(LinkGene linkCopy)
        {
            links.Add(linkCopy);
        }

        public void AddNode(NodeGene nodeCopy)
        {
            if (all.Contains(nodeCopy)) return;

            if (nodeCopy.GetType().Equals(NodeType.INPUT))
            {
                inputs.Add(nodeCopy);
            }
            else if (nodeCopy.GetType().Equals(NodeType.OUTPUT))
            {
                outputs.Add(nodeCopy);
            }
            else if (nodeCopy.GetType().Equals(NodeType.HIDDEN))
            {
                hidden.Add(nodeCopy);
            }

            all.Add(nodeCopy);
            NodeCount++;
        }

        public override string ToString()
        {
            string output = "Inputs: " + InputCount + ", Outputs: " + OutputCount + ", Hidden: " + HiddenCount + ", Total: " + NodeCount + ", Link count: " + LinkCount + "\n";
            foreach (LinkGene link in links)
            {
                output += link.ToString() + "\n";
            }
            return output;
        }

        private int GetCurrentNodeMaxId()
        {
            int max = 0;
            foreach (NodeGene node in all)
            {
                if (node.id > max)
                {
                    max = node.id;
                }
            }
            return max;
        }

        public float GetTopologicalDistance(Genotype to)
        {
            //TODO implement
            return 0;
        }
    }
}