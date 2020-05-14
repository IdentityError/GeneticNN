using Assets.Scripts.TWEANN;
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
        public List<NodeGene> all;
        public List<LinkGene> links;

        #region Properties

        private int nodeNumber;

        public int NodeCount
        {
            get => nodeNumber;
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
            get => all.Count - inputs.Count - outputs.Count;
        }

        #endregion Properties

        private int lastNodeId = 0;

        public Genotype(TopologyDescriptor descriptor) : this()
        {
            nodeNumber = descriptor.inputCount + descriptor.hiddenCount + descriptor.outputCount;
            for (int i = 0; i < nodeNumber; i++)
            {
                NodeGene current = null;
                current = new NodeGene(i + 1, TMath.Tanh);
                if (i < descriptor.inputCount)
                {
                    current.SetType(NodeType.INPUT);
                    inputs.Add(current);
                }
                else if (i > nodeNumber - descriptor.outputCount - 1)
                {
                    current.SetType(NodeType.OUTPUT);
                    outputs.Add(current);
                }
                else
                {
                    current.SetType(NodeType.HIDDEN);
                }
                all.Add(current);
                lastNodeId = i;
            }

            int linkN = 0;
            foreach (LinkDescriptor link in descriptor.links)
            {
                AddLink(link, linkN++);
            }
            Debug.Log(ToString());
        }

        public Genotype()
        {
            inputs = new List<NodeGene>();
            outputs = new List<NodeGene>();
            all = new List<NodeGene>();
            links = new List<LinkGene>();
        }

        public Genotype Crossover(Genotype partner)
        {
            Genotype childGen = new Genotype();

            // Add all nodes from the fittest
            foreach (NodeGene gene in all)
            {
                childGen.AddNode(gene.Copy());
            }

            List<LinkGene> remaining = new List<LinkGene>(links);

            // Order the two links list by innovation number
            links.OrderBy(item => item.GetInnovationNumber());
            partner.links.OrderBy(item => item.GetInnovationNumber());

            // Zip togheter the links that have the same innovation number
            List<Tuple<LinkGene, LinkGene>> linksres = TUtilsProvider.ZipWithPredicate(links, partner.links, (item1, item2) => item1.GetInnovationNumber().Equals(item2.GetInnovationNumber()));

            // Add to che child all the matching genes (links)
            foreach (Tuple<LinkGene, LinkGene> gene in linksres)
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
                childGen.AddLink(copy);
                int count = remaining.RemoveAll(item => item.GetInnovationNumber().Equals(copy.GetInnovationNumber()));
            }

            // At this point all common genes are added we add all the disjoint genes from the fittest
            foreach (LinkGene gene in remaining)
            {
                childGen.AddLink(gene.Copy());
            }

            return childGen;
        }

        public Mutation Mutate(float mutationRate, PopulationTrainer trainer)
        {
            if (UnityEngine.Random.Range(0F, 1F) < mutationRate)
            {
                // Mutation happened
            }
            //TODO implement
            return null;
        }

        private void AddLink(LinkDescriptor link, int innovationNumber)
        {
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
                LinkGene newLink = new LinkGene(from, to, UnityEngine.Random.Range(-1F, 1F), innovationNumber);
                to.AddIncomingLink(newLink);
                links.Add(newLink);
            }
        }

        public void AddLink(LinkGene linkCopy)
        {
            if (links.Contains(linkCopy)) return;
            linkCopy.To().AddIncomingLink(linkCopy);
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

            all.Add(nodeCopy);
            nodeNumber++;
        }

        public override string ToString()
        {
            string output = "Inputs: " + inputs.Count + ", Outputs: " + outputs.Count + ", Total: " + NodeCount + ", Link count: " + LinkCount + "\n";
            foreach (LinkGene link in links)
            {
                output += "Link:" + link.From().id + " -> " + link.To().id + " IN: " + link.GetInnovationNumber() + "\n";
            }
            return output;
        }
    }
}