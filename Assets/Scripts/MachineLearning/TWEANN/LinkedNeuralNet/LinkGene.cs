using System;

public class LinkGene : IEquatable<LinkGene>
{
    public int InnovationNumber { get; set; }

    public NodeGene Source { get; set; }

    public NodeGene Destination { get; set; }

    public double Weight { get; set; }

    public LinkGene(NodeGene source, NodeGene destination, double weight) : this(source, destination, weight, 0)
    {
    }

    public LinkGene(NodeGene source, NodeGene destination) : this(source, destination, 0D)
    {
    }

    public LinkGene(NodeGene source, NodeGene destination, double weight, int innovationNumber)
    {
        this.Source = source;
        this.Destination = destination;
        this.Weight = weight;
        this.InnovationNumber = innovationNumber;
    }

    public LinkGene Copy()
    {
        return new LinkGene(Source, Destination, Weight, InnovationNumber);
    }

    public bool Equals(LinkGene other)
    {
        return this.Source.Id == other.Source.Id && this.Destination.Id == other.Destination.Id;
    }

    public override string ToString()
    {
        return Source.Id + " -> " + Destination.Id + ", W: " + Weight + ", I: " + InnovationNumber;
    }
}