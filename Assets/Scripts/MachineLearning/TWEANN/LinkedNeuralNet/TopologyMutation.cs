using System;

public enum TopologyMutationType { SPLIT_LINK, ADD_LINK }

public class TopologyMutation : IEquatable<TopologyMutation>
{
    private readonly LinkGene linkInvolved;
    private readonly TopologyMutationType type;

    public int InnovationNumber { get; set; }

    public TopologyMutation(TopologyMutationType type, LinkGene linkInvolved, int innovationNumber) : this(type, linkInvolved)
    {
        this.InnovationNumber = innovationNumber;
    }

    public TopologyMutation(TopologyMutationType type, LinkGene linkInvolved)
    {
        this.type = type;
        this.linkInvolved = linkInvolved;
    }

    public bool Equals(TopologyMutation other)
    {
        return type == other.type && linkInvolved.Equals(other.linkInvolved);
    }
}