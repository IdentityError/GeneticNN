using GibFrame.Extensions;
using GibFrame.Utils.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static NodeGene;

[Serializable]
public class Genotype
{
    public List<NodeGene> all;
    public List<NodeGene> hidden;
    public List<NodeGene> inputs;
    public List<NodeGene> outputs;
    public List<LinkGene> links;

    public int HiddenCount => hidden.Count;

    public int InputCount => inputs.Count;

    public int LinkCount => links.Count;

    public int OutputCount => outputs.Count;

    public int NodeCount { get; private set; }

    public Genotype()
    {
        inputs = new List<NodeGene>();
        outputs = new List<NodeGene>();
        hidden = new List<NodeGene>();
        all = new List<NodeGene>();
        links = new List<LinkGene>();
    }

    public void AddLinkAndNodes(LinkGene newLink)
    {
        if (links.Contains(newLink))
        {
            return;
        }
        NodeGene source = null;
        NodeGene destination = null;
        foreach (NodeGene node in all)
        {
            if (node.Equals(newLink.Source))
            {
                source = node;
            }
            if (node.Equals(newLink.Destination))
            {
                destination = node;
            }
            if (source != null && destination != null) break;
        }
        if (destination == null)
        {
            destination = newLink.Destination.CopyNoLinks();
        }
        if (source == null)
        {
            source = newLink.Source.CopyNoLinks();
        }
        LinkGene currentNew = new LinkGene(source, destination, newLink.Weight, newLink.InnovationNumber);
        destination.AddIncomingLink(currentNew);
        source.AddOutgoingLink(currentNew);
        AddNode(destination);
        AddNode(source);
        links.Add(currentNew);
    }

    public NodeGene AddNode(NodeGene nodeCopy)
    {
        if (all.Contains(nodeCopy)) return nodeCopy;

        if (nodeCopy.Type.Equals(NodeType.INPUT))
        {
            inputs.Add(nodeCopy);
        }
        else if (nodeCopy.Type.Equals(NodeType.OUTPUT))
        {
            outputs.Add(nodeCopy);
        }
        else if (nodeCopy.Type.Equals(NodeType.HIDDEN))
        {
            hidden.Add(nodeCopy);
        }

        all.Add(nodeCopy);
        NodeCount++;
        return nodeCopy;
    }

    public float GetTopologicalDistance(Genotype from)
    {
        int maxGenomes = this.LinkCount > from.LinkCount ? this.LinkCount : from.LinkCount;
        int genesDifference = GMath.Abs(this.LinkCount - from.LinkCount);
        List<Tuple<LinkGene, LinkGene>> zippedLinks = links.ZipWithFirstPredicateMatching(from.links, (item1, item2) => item1.InnovationNumber.Equals(item2.InnovationNumber));
        float differenceSum = 0;
        foreach (Tuple<LinkGene, LinkGene> current in zippedLinks)
        {
            differenceSum += GMath.Abs((float)(current.Item1.Weight - current.Item2.Weight));
        }
        float averageDiff = differenceSum / zippedLinks.Count;
        float c = 1.75F, c2 = 0.3F;
        return (c * genesDifference) / maxGenomes + (c2 * averageDiff);
    }

    public void Mutate(DescriptorsWrapper.RatesDescriptor rates)
    {
        if (UnityEngine.Random.Range(0F, 1F) < rates.splitLinkRate)
        {
            // 1. Select a random link to be mutated
            // 2. Create a new node
            // 3. Create a topology mutation based on the random link selected, set its innovation number and set the same number to the
            // random link
            LinkGene random = links.ElementAt(UnityEngine.Random.Range(0, links.Count));
            NodeGene newNode = new NodeGene(UnityEngine.Random.Range(random.Source.Id, random.Destination.Id), GMath.Tanh, NodeType.HIDDEN);
            TopologyMutation topologyMutation = new TopologyMutation(TopologyMutationType.SPLIT_LINK, random);
            topologyMutation.InnovationNumber = GlobalParams.GetGenerationInnovationNumber(topologyMutation);
            random.InnovationNumber = topologyMutation.InnovationNumber;

            // 1. Create a new link that will connect the new node
            // 2. Create a topology mutation based on the new link, set its innovation number and set the same number to the link
            // 3. Redirect the connections:
            // Before: A --random--> B
            // After: A --newLink--> C --random--> B
            LinkGene newLink = new LinkGene(random.Source, newNode, 1F);
            topologyMutation = new TopologyMutation(TopologyMutationType.SPLIT_LINK, newLink);
            topologyMutation.InnovationNumber = GlobalParams.GetGenerationInnovationNumber(topologyMutation);
            newLink.InnovationNumber = topologyMutation.InnovationNumber;
            random.Source = newNode;

            // Add the new node and the link
            AddNode(newNode);
            AddLinkAndNodes(newLink);
        }
        if (UnityEngine.Random.Range(0F, 1F) < rates.addLinkRate)
        {
            // Create a list of all nodes except the output nodes and select the random From node
            List<NodeGene> temp = new List<NodeGene>(inputs);
            temp.AddRange(hidden);
            NodeGene fromRandom = temp.ElementAt(UnityEngine.Random.Range(0, temp.Count));

            // 1. Create a list of all nodes except the input nodes
            // 2. Remove the already selected node if present to prevent recurrent link
            // 3. Remove all the nodes in the list that are topologically behind the randomly selected node, only if they are not output nodes
            // 4. If at least one node remains:
            // - Select a random To node
            // - Create a new link connecting the two nodes
            // - If the link is not already present, create a topology mutation, retrieve the innovation number and add the link
            temp = new List<NodeGene>(outputs);
            temp.AddRange(hidden);
            temp.Remove(fromRandom);
            List<NodeGene> newList = new List<NodeGene>(temp);
            newList.RemoveAll((node) => node.Id < fromRandom.Id && node.Type != NodeType.OUTPUT);

            if (temp.Count > 0)
            {
                NodeGene toRandom = temp.ElementAt(UnityEngine.Random.Range(0, temp.Count));
                LinkGene newLink = new LinkGene(fromRandom, toRandom, 1F);
                if (!links.Contains(newLink))
                {
                    TopologyMutation topologyMutation = new TopologyMutation(TopologyMutationType.ADD_LINK, newLink);
                    topologyMutation.InnovationNumber = GlobalParams.GetGenerationInnovationNumber(topologyMutation);
                    newLink.InnovationNumber = topologyMutation.InnovationNumber;
                    AddLinkAndNodes(newLink);
                }
            }
        }
        // For each link of this genotype try to mutate it
        foreach (LinkGene gene in links)
        {
            if (UnityEngine.Random.Range(0F, 1F) < (rates.weightMutationRate))
            {
                int random = UnityEngine.Random.Range(0, LinkCount);
                double value = links[random].Weight + GMath.RandomGen.NextGaussian(0F, 0.25F);
                value = Mathf.Clamp((float)value, -1F, 1F);
                links[random].Weight = value;
            }
        }
    }

    public Genotype Copy()
    {
        Genotype genotype = new Genotype();
        foreach (LinkGene link in links)
        {
            genotype.AddLinkAndNodes(link.Copy());
        }
        return genotype;
    }

    public override string ToString()
    {
        string output = "Inputs: " + InputCount + ", Outputs: " + OutputCount + ", Hidden: " + HiddenCount + ", Total: " + NodeCount + ", Link count: " + LinkCount + "\n";
        foreach (NodeGene node in all)
        {
            if (node.GetIncomingLinks().Count > 0)
                output += "To node " + node.Id + ":\n";
            foreach (LinkGene gene in node.GetIncomingLinks())
            {
                output += gene.ToString() + "\n";
            }
        }
        return output;
    }
}