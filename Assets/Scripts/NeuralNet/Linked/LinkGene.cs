using System;

namespace Assets.Scripts.NeuralNet
{
    public class LinkGene : IEquatable<LinkGene>
    {
        private NodeGene from;
        private int innovationNumber;
        private NodeGene to;
        private double weight;

        public LinkGene(NodeGene from, NodeGene to, double weight) : this(from, to, weight, 0)
        {
        }

        public LinkGene(NodeGene from, NodeGene to) : this(from, to, 0D)
        {
        }

        public LinkGene(NodeGene from, NodeGene to, double weight, int innovationNumber)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
            this.innovationNumber = innovationNumber;
        }

        public LinkGene Copy()
        {
            return new LinkGene(from, to, weight, innovationNumber);
        }

        public bool Equals(LinkGene other)
        {
            return this.from.id == other.from.id && this.to.id == other.to.id;
        }

        public NodeGene From()
        {
            return from;
        }

        public int GetInnovationNumber()
        {
            return innovationNumber;
        }

        public double GetWeight()
        {
            return weight;
        }

        public void SetFrom(NodeGene node)
        {
            from = node;
        }

        public void SetInnovationNumber(int innovationNumber)
        {
            this.innovationNumber = innovationNumber;
        }

        public void SetTo(NodeGene node)
        {
            to = node;
        }

        public void SetWeight(double weight)
        {
            this.weight = weight;
        }

        public NodeGene To()
        {
            return to;
        }

        public override string ToString()
        {
            return from.id + " -> " + to.id + ", W: " + weight + ", I: " + innovationNumber;
        }
    }
}