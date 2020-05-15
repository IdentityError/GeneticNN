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
                AddLink(link, -linkN++, UnityEngine.Random.Range(-1F, 1F));
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
            //foreach (LinkGene link in links)
            //{
            //    childGen.AddLink(new LinkDescriptor(link.From().id, link.To().id), link.GetInnovationNumber()).SetWeight(0);
            //}
            ////? maybe only links are added ?
            //// Add all nodes from the fittest
            //foreach (NodeGene gene in all)
            //{
            //    childGen.AddNode(gene.CopyNoLinks());
            //}
            List<LinkGene> remaining = new List<LinkGene>(links);

            //// Order the two links list by innovation number
            ////links.OrderBy(item => item.GetInnovationNumber());
            ////partner.links.OrderBy(item => item.GetInnovationNumber());

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
                    NodeGene newNode = new NodeGene(GetCurrentMaxId() + 1, TMath.Tanh, NodeType.HIDDEN);
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
                    List<NodeGene> temp = new List<NodeGene>(all);
                    int index = UnityEngine.Random.Range(0, temp.Count);
                    NodeGene fromRandom = temp.ElementAt(index);
                    temp.RemoveAt(index);
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
            for (int i = 0; i < LinkCount / 6; i++)
            {
                if (UnityEngine.Random.Range(0F, 1F) < mutationRate)
                {
                    LinkGene random = links.ElementAt(UnityEngine.Random.Range(0, links.Count));
                    random.SetWeight(UnityEngine.Random.Range(-1F, 1F));
                    //Debug.Log("WEIGHT MUTATION HAPPENED: " + random.ToString());
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

        private int GetCurrentMaxId()
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
    }
}