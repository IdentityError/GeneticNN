using System;
using System.Collections.Generic;

namespace Assets.Scripts.NeuralNet
{
    public class NodeGene : IEquatable<NodeGene>
    {
        public Func<double, double> activationFunction;
        public int id;
        private bool activated;
        private double activation, activationSum;
        private double bias;
        private List<LinkGene> incoming;
        private List<LinkGene> outgoing;
        private NodeType type;

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
            this.type = type;
        }

        public NodeGene(int id, Func<double, double> activationFunction)
        {
            this.bias = UnityEngine.Random.Range(-0.25F, 0.25F);
            this.activated = false;
            this.id = id;
            this.activationFunction = activationFunction;
            this.incoming = new List<LinkGene>();
            this.outgoing = new List<LinkGene>();
        }

        #endregion Constructors

        /// <summary>
        ///   Add an incoming link to this node
        /// </summary>
        /// <param name="link"> The link is directly added, no copied </param>
        public void AddIncomingLink(LinkGene link)
        {
            incoming.Add(link);
        }

        public void AddOutgoingLink(LinkGene link)
        {
            this.outgoing.Add(link);
        }

        /// <summary>
        ///   Get a copy of this node, omitting the incoming links
        /// </summary>
        /// <returns> The node copy </returns>
        public NodeGene CopyNoLinks()
        {
            return new NodeGene(id, activationFunction, GetType());
        }

        /// <summary>
        ///   Override of the Equals function
        /// </summary>
        /// <param name="other"> </param>
        /// <returns> </returns>
        public bool Equals(NodeGene other)
        {
            return this.id == other.id && this.type == other.type;
        }

        public void AddActivationContribute(double contribute)
        {
            activationSum += contribute;
        }

        /// <summary>
        ///   Get the current activation value of the node
        /// </summary>
        /// <returns> </returns>
        public double GetActivation()
        {
            if (!activated)
            {
                Activate();
            }
            return activation;
        }

        /// <summary>
        ///   Retrieve all the incoming links
        /// </summary>
        /// <returns> The incoming links list </returns>
        public List<LinkGene> GetIncomingLinks()
        {
            return incoming;
        }

        public List<LinkGene> GetOutgoingLinks()
        {
            return outgoing;
        }

        /// <summary>
        ///   Get the type of this node
        /// </summary>
        /// <returns> </returns>
        public new NodeType GetType()
        {
            return type;
        }

        /// <summary>
        ///   Reset the activation status of this node
        /// </summary>
        public void ResetActivation()
        {
            activated = false;
            activationSum = 0;
            activation = 0;
        }

        /// <summary>
        ///   Set the node activation manually. If this node is not an input node, nothing is done
        /// </summary>
        /// <param name="value"> </param>
        public void SetInputValue(double value)
        {
            if (type == NodeType.INPUT)
            {
                this.activation = value;
                activated = true;
            }
            else
            {
                throw new Exception("Trying to set the activation of a non input node");
            }
        }

        /// <summary>
        ///   Set a new type for this node
        /// </summary>
        /// <param name="position"> </param>
        public void SetType(NodeType position)
        {
            this.type = position;
        }

        /// <summary>
        ///   Override of the ToString function
        /// </summary>
        /// <returns> </returns>
        public override string ToString()
        {
            return "ID: " + id + ", Type: " + type;
        }

        private void Activate()
        {
            if (!activated)
            {
                activated = true;
                double sum = 0;
                foreach (LinkGene link in incoming)
                {
                    sum += link.From().GetActivation() * link.GetWeight();
                }
                activationSum = sum;
                activation = activationFunction(activationSum/* + bias*/);
            }
        }
    }

    public enum NodeType { INPUT, HIDDEN, OUTPUT }
}