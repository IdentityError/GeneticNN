using System;

namespace Assets.Scripts.NeuralNet
{
    public class LinkGene : IEquatable<LinkGene>
    {
        private NodeGene from;
        private NodeGene to;
        private double weight;
        private int innovationNumber;

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

        public NodeGene From()
        {
            return from;
        }

        public void SetFrom(NodeGene node)
        {
            from = node;
        }

        public NodeGene To()
        {
            return to;
        }

        public void SetTo(NodeGene node)
        {
            to = node;
        }

        public LinkGene Copy()
        {
            return new LinkGene(from.Copy(), to.Copy(), weight, innovationNumber);
        }

        public int GetInnovationNumber()
        {
            return innovationNumber;
        }

        public void SetInnovationNumber(int innovationNumber)
        {
            this.innovationNumber = innovationNumber;
        }

        public double GetWeight()
        {
            return weight;
        }

        public void SetWeight(double weight)
        {
            this.weight = weight;
        }

        public override string ToString()
        {
            return from.id + " -> " + to.id + ", W: " + weight + ", I: " + innovationNumber;
        }

        public bool Equals(LinkGene other)
        {
            return this.from.id == other.from.id && this.to.id == other.to.id;
        }
    }
}