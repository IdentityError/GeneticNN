using System;
using System.Collections.Generic;
using System.Linq;
using static Assets.Scripts.TWEANN.TrainerNEAT;

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
                NodeGene current = new NodeGene(-i - 1, TMath.Tanh);
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
                AddLink(new LinkDescriptor(-link.fromId, -link.toId), linkN++, UnityEngine.Random.Range(-1F, 1F));
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

        /// <summary>
        ///   Perform a crossover between the two genotypes and get the result
        /// </summary>
        /// <param name="partner"> </param>
        /// <param name="thisFitness"> </param>
        /// <param name="partnerFitness"> </param>
        /// <returns> </returns>
        public Genotype Crossover(Genotype partner, double thisFitness, double partnerFitness)
        {
            Genotype childGen = new Genotype();
            //TODO IMPROVE

            List<LinkGene> remaining = new List<LinkGene>(links);
            List<LinkGene> partnerRemaining = new List<LinkGene>(partner.links);
            // Zip togheter the links that have the same innovation number
            List<Tuple<LinkGene, LinkGene>> zippedLinks = TUtilsProvider.ZipWithPredicate(links, partner.links, (item1, item2) => item1.GetInnovationNumber().Equals(item2.GetInnovationNumber()));

            //Add to che child all the matching genes(links)
            foreach (Tuple<LinkGene, LinkGene> gene in zippedLinks)
            {
                LinkGene copy;
                if (/*thisFitness > partnerFitness*/UnityEngine.Random.Range(0F, 1F) < 0.5F)
                {
                    copy = gene.Item1;
                }
                else
                {
                    copy = gene.Item2;
                }

                childGen.AddLinkAndNodes(copy);
                remaining.RemoveAll(item => item.GetInnovationNumber().Equals(copy.GetInnovationNumber()));
                partnerRemaining.RemoveAll(item => item.GetInnovationNumber().Equals(copy.GetInnovationNumber()));
            }

            // At this point all common genes are added we add all the disjoint genes from the fittest
            if (thisFitness > partnerFitness)
            {
                foreach (LinkGene gene in remaining)
                {
                    childGen.AddLinkAndNodes(gene);
                }
            }
            else
            {
                foreach (LinkGene gene in partnerRemaining)
                {
                    childGen.AddLinkAndNodes(gene);
                }
            }

            return childGen;
        }

        /// <summary>
        ///   Add a link and the nodes involved, if not already present
        /// </summary>
        /// <param name="newLink"> </param>
        public void AddLinkAndNodes(LinkGene newLink)
        {
            if (links.Contains(newLink))
            {
                return;
            }
            NodeGene from = null;
            NodeGene to = null;
            foreach (NodeGene node in all)
            {
                if (node.Equals(newLink.From()))
                {
                    from = node;
                }
                if (node.Equals(newLink.To()))
                {
                    to = node;
                }
                if (from != null && to != null) break;
            }
            if (to == null)
            {
                to = newLink.To().CopyNoLinks();
            }
            if (from == null)
            {
                from = newLink.From().CopyNoLinks();
            }
            LinkGene currentNew = new LinkGene(from, to, newLink.GetWeight(), newLink.GetInnovationNumber());
            to.AddIncomingLink(currentNew);
            AddNode(to);
            AddNode(from);
            links.Add(currentNew);
        }

        /// <summary>
        ///   Mutate this genotype based on the specified probabilities
        /// </summary>
        /// <param name="mutation"> </param>
        public void Mutate(MutationProbabilities mutation)
        {
            if (UnityEngine.Random.Range(0F, 1F) < mutation.splitLinkProb)
            {
                // Select a random link to be mutated
                LinkGene random = links.ElementAt(UnityEngine.Random.Range(0, links.Count));
                // Create a node with a random id
                NodeGene newNode = new NodeGene(UnityEngine.Random.Range(0, int.MaxValue), TMath.Tanh, NodeType.HIDDEN);
                // Create a topological mutation object that will represent this mutation
                TopologyMutation topologyMutation = new TopologyMutation(TopologyMutationType.SPLIT_LINK, random);
                // Retrieve the innovation number from the global params
                topologyMutation.SetInnovationNumber(GlobalParams.GetGenerationInnovationNumber(topologyMutation));
                // Create a new link going from the old link From node to the new node
                LinkGene newLink = new LinkGene(random.From(), newNode, 1F, topologyMutation.GetInnovationNumber());
                // Add to the node incoming link the newLink
                newNode.AddIncomingLink(newLink);
                // Set the initial randomly selected link From node to the new node
                random.SetFrom(newNode);
                // Add the new node and the link
                AddNode(newNode);
                AddLinkAndNodes(newLink);
            }
            if (UnityEngine.Random.Range(0F, 1F) < mutation.addLinkProb)
            {
                // Create a list of all nodes except output nodes to select the random From node
                List<NodeGene> temp = new List<NodeGene>(inputs);
                temp.AddRange(hidden);
                // Select a random From node
                NodeGene fromRandom = temp.ElementAt(UnityEngine.Random.Range(0, temp.Count));

                // Create a list of all nodes except input nodes to select the random To node
                temp = new List<NodeGene>(outputs);
                temp.AddRange(hidden);
                // Remove the already selected node
                temp.Remove(fromRandom);

                // Remove all the nodes that are "behind" the From node
                List<NodeGene> newList = new List<NodeGene>(temp);
                foreach (NodeGene gene in newList)
                {
                    if (fromRandom.id > gene.id && gene.GetType() != NodeType.OUTPUT)
                    {
                        temp.Remove(gene);
                    }
                }
                // If there still is at least one node
                if (temp.Count > 0)
                {
                    // Select a random To node
                    NodeGene toRandom = temp.ElementAt(UnityEngine.Random.Range(0, temp.Count));
                    // Create the new link
                    LinkGene newLink = new LinkGene(fromRandom, toRandom, 1F);
                    // If the link is not already present add it
                    if (!links.Contains(newLink))
                    {
                        // Create a topological mutation object that will represent this mutation
                        TopologyMutation topologyMutation = new TopologyMutation(TopologyMutationType.ADD_LINK, newLink);
                        // Retrieve the innovation number from the global params
                        topologyMutation.SetInnovationNumber(GlobalParams.GetGenerationInnovationNumber(topologyMutation));
                        newLink.SetInnovationNumber(topologyMutation.GetInnovationNumber());
                        // Add the new link to the links list and to the incoming links of the To node
                        AddLinkAndNodes(newLink);
                        toRandom.AddIncomingLink(newLink);
                    }
                }
            }
            // For each link of this genotype try to mutate it
            foreach (LinkGene gene in links)
            {
                if (UnityEngine.Random.Range(0F, 1F) < mutation.weightChangeProb)
                {
                    int random = UnityEngine.Random.Range(0, LinkCount);
                    links[random].SetWeight(UnityEngine.Random.Range(-1F, 1F));
                }
            }
        }

        /// <summary>
        ///   Add a link described by a link descriptor only if the two nodes involved are present
        /// </summary>
        /// <param name="link"> </param>
        /// <param name="innovationNumber"> </param>
        /// <param name="weight"> </param>
        /// <returns> </returns>
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

        /// <summary>
        ///   Add a nodes only if not already present
        /// </summary>
        /// <param name="nodeCopy"> </param>
        /// <returns> </returns>
        public NodeGene AddNode(NodeGene nodeCopy)
        {
            if (all.Contains(nodeCopy)) return nodeCopy;

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
            return nodeCopy;
        }

        /// <summary>
        ///   Get the string representation of this genotype
        /// </summary>
        /// <returns> </returns>
        public override string ToString()
        {
            string output = "Inputs: " + InputCount + ", Outputs: " + OutputCount + ", Hidden: " + HiddenCount + ", Total: " + NodeCount + ", Link count: " + LinkCount + "\n";
            foreach (NodeGene node in all)
            {
                if (node.GetIncomingLinks().Count > 0)
                    output += "To node " + node.id + ":\n";
                foreach (LinkGene gene in node.GetIncomingLinks())
                {
                    output += gene.ToString() + "\n";
                }
            }
            return output;
        }

        /// <summary>
        ///   Get the genotype distance between this genotype and another one
        /// </summary>
        /// <param name="to"> </param>
        /// <returns> </returns>
        public float GetTopologicalDistance(Genotype to)
        {
            int maxGenomes = this.LinkCount > to.LinkCount ? this.LinkCount : to.LinkCount;
            int genesDifference = this.LinkCount - to.LinkCount;
            List<Tuple<LinkGene, LinkGene>> zippedLinks = TUtilsProvider.ZipWithPredicate(links, to.links, (item1, item2) => item1.GetInnovationNumber().Equals(item2.GetInnovationNumber()));
            float differenceSum = 0;
            foreach (Tuple<LinkGene, LinkGene> current in zippedLinks)
            {
                differenceSum += TMath.Abs((float)(current.Item1.GetWeight() - current.Item2.GetWeight()));
            }
            float averageDiff = differenceSum / zippedLinks.Count;
            float c = 1F, c2 = 0.3F;
            //Debug.Log("current diff: " + (c * genesDifference) / maxGenomes + (c2 * averageDiff));
            return (c * genesDifference) / maxGenomes + (c2 * averageDiff);
        }
    }
}