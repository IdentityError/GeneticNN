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

        /// <summary>
        ///   Get the From node reference
        /// </summary>
        /// <returns> </returns>
        public NodeGene From()
        {
            return from;
        }

        /// <summary>
        ///   Set a new reference of the From node
        /// </summary>
        /// <param name="node"> </param>
        public void SetFrom(NodeGene node)
        {
            from = node;
        }

        /// <summary>
        ///   Get the To node reference
        /// </summary>
        /// <returns> </returns>
        public NodeGene To()
        {
            return to;
        }

        /// <summary>
        ///   Set a new reference of the To node
        /// </summary>
        /// <param name="node"> </param>
        public void SetTo(NodeGene node)
        {
            to = node;
        }

        /// <summary>
        ///   Set the innovation number associated with this link
        /// </summary>
        /// <param name="innovationNumber"> </param>
        public void SetInnovationNumber(int innovationNumber)
        {
            this.innovationNumber = innovationNumber;
        }

        /// <summary>
        ///   Retrieve the innovation number associated with this link
        /// </summary>
        /// <returns> </returns>
        public int GetInnovationNumber()
        {
            return innovationNumber;
        }

        /// <summary>
        ///   Set the weight value associated with this link
        /// </summary>
        /// <param name="weight"> </param>
        public void SetWeight(double weight)
        {
            this.weight = weight;
        }

        /// <summary>
        ///   Retrieve the weight value associated with this link
        /// </summary>
        /// <returns> </returns>
        public double GetWeight()
        {
            return weight;
        }

        /// <summary>
        ///   Get a copy of this link instance
        /// </summary>
        /// <returns> </returns>
        public LinkGene Copy()
        {
            return new LinkGene(from, to, weight, innovationNumber);
        }

        /// <summary>
        ///   Override of the Equals function
        /// </summary>
        /// <param name="other"> </param>
        /// <returns> </returns>
        public bool Equals(LinkGene other)
        {
            return this.from.id == other.from.id && this.to.id == other.to.id;
        }

        /// <summary>
        ///   Override of the ToString function
        /// </summary>
        /// <returns> </returns>
        public override string ToString()
        {
            return from.id + " -> " + to.id + ", W: " + weight + ", I: " + innovationNumber;
        }
    }
}