using System;
using System.Collections.Generic;

public class NodeGene : IEquatable<NodeGene>
{
    public enum NodeType { INPUT, HIDDEN, OUTPUT }

    private readonly List<LinkGene> incoming;
    private readonly List<LinkGene> outgoing;
    private readonly Func<double, double> ActivationFunction;
    private readonly double bias;
    private bool activated;
    private double activation, activationSum;

    public int Id { get; private set; }

    public NodeType Type { get; set; }

    #region Constructors

    public NodeGene(int id, Func<double, double> activationFunction, List<LinkGene> incoming, List<LinkGene> outgoing, double bias, NodeType type) : this(id, activationFunction, bias, type)
    {
        if (incoming != null)
        {
            this.incoming = new List<LinkGene>(incoming);
        }
        if (outgoing != null)
        {
            this.outgoing = new List<LinkGene>(outgoing);
        }
    }

    public NodeGene(int id, Func<double, double> activationFunction, double bias, NodeType type) : this(id, activationFunction, type)
    {
        this.bias = bias;
    }

    public NodeGene(int id, Func<double, double> activationFunction, NodeType type) : this(id, activationFunction)
    {
        this.Type = type;
    }

    public NodeGene(int id, Func<double, double> activationFunction)
    {
        this.bias = UnityEngine.Random.Range(-0.25F, 0.25F);
        this.activated = false;
        this.Id = id;
        this.ActivationFunction = activationFunction;
        this.incoming = new List<LinkGene>();
        this.outgoing = new List<LinkGene>();
    }

    #endregion Constructors

    public void AddIncomingLink(LinkGene link)
    {
        incoming.Add(link);
    }

    public void AddOutgoingLink(LinkGene link)
    {
        outgoing.Add(link);
    }

    public NodeGene CopyNoLinks()
    {
        return new NodeGene(Id, ActivationFunction, Type);
    }

    public bool Equals(NodeGene other)
    {
        return this.Id == other.Id && this.Type == other.Type;
    }

    public void AddActivationContribute(double contribute)
    {
        activationSum += contribute;
    }

    public double GetActivation()
    {
        if (!activated)
        {
            Activate();
        }
        return activation;
    }

    public List<LinkGene> GetIncomingLinks()
    {
        return incoming;
    }

    public List<LinkGene> GetOutgoingLinks()
    {
        return outgoing;
    }

    public void ResetActivation()
    {
        activated = false;
        activationSum = 0;
        activation = 0;
    }

    public void SetInputValue(double value)
    {
        if (Type == NodeType.INPUT)
        {
            this.activation = value;
            activated = true;
        }
        else
        {
            throw new Exception("Trying to set the activation of a non input node");
        }
    }

    public override string ToString()
    {
        return "ID: " + Id + ", Type: " + Type;
    }

    private void Activate()
    {
        if (!activated)
        {
            activated = true;
            double sum = 0;
            foreach (LinkGene link in incoming)
            {
                sum += link.Source.GetActivation() * link.Weight;
            }
            activationSum = sum;
            activation = ActivationFunction(activationSum/* + bias*/);
        }
    }
}