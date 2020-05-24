using System;
using System.Collections.Generic;

namespace Assets.Scripts.NeuralNet
{
    public enum NodeType { INPUT, HIDDEN, OUTPUT }

    public class NodeGene : IEquatable<NodeGene>
    {
        public Func<double, double> activationFunction;
        public int id;
        private bool activated;
        private double activation, activationSum;
        private double bias;
        private List<LinkGene> incoming;
        private NodeType type;

        #region Constructors

        public NodeGene(int id, Func<double, double> activationFunction, List<LinkGene> incoming, NodeType type) : this(id, activationFunction, UnityEngine.Random.Range(-0.25F, 0.25F))
        {
        }

        public NodeGene(int id, Func<double, double> activationFunction, NodeType type) : this(id, activationFunction, null, type, UnityEngine.Random.Range(-0.25F, 0.25F))
        {
        }

        public NodeGene(int id, Func<double, double> activationFunction) : this(id, activationFunction, UnityEngine.Random.Range(-0.25F, 0.25F))
        {
        }

        public NodeGene(int id, Func<double, double> activationFunction, List<LinkGene> incoming, NodeType type, double bias) : this(id, activationFunction, bias)
        {
            if (incoming != null)
            {
                this.incoming = new List<LinkGene>(incoming);
            }

            this.type = type;
        }

        public NodeGene(int id, Func<double, double> activationFunction, NodeType type, double bias) : this(id, activationFunction, null, type, bias)
        {
        }

        public NodeGene(int id, Func<double, double> activationFunction, double bias)
        {
            this.bias = bias;
            this.activated = false;
            this.id = id;
            this.activationFunction = activationFunction;
            this.incoming = new List<LinkGene>();
        }

        #endregion Constructors

        /// <summary>
        ///   Activates the current node -&gt; calculate the activation value and activation sum
        /// </summary>
        public void Activate()
        {
            if (!activated)
            {
                double sum = 0;
                foreach (LinkGene link in incoming)
                {
                    sum += link.From().GetActivation() * link.GetWeight();
                }
                activationSum = sum;
                activation = activationFunction(activationSum/* + bias*/);
                activated = true;
            }
        }

        public void AddIncomingLink(LinkGene link)
        {
            incoming.Add(link);
        }

        public NodeGene Copy()
        {
            return new NodeGene(id, activationFunction, new List<LinkGene>(GetIncomingLinks()), GetType());
        }

        public NodeGene CopyNoLinks()
        {
            return new NodeGene(id, activationFunction, GetType());
        }

        public bool Equals(NodeGene other)
        {
            return this.id == other.id && this.type == other.type;
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

        public List<LinkGene> GetIncomingLinks()
        {
            return incoming;
        }

        public new NodeType GetType()
        {
            return type;
        }

        public bool IsActivated()
        {
            return activated;
        }

        public void ResetActivation()
        {
            activated = false;
            activationSum = 0;
            activation = 0;
        }

        /// <summary>
        ///   Sets the node activation manually. Can be set only on inputs type nodes
        /// </summary>
        /// <param name="activation"> </param>
        public void SetInputValue(double activation)
        {
            if (type == NodeType.INPUT)
            {
                this.activation = activation;
                activated = true;
            }
            else
            {
                throw new Exception("Trying to set the activation of a non input node");
            }
        }

        public void SetType(NodeType position)
        {
            this.type = position;
        }

        public override string ToString()
        {
            return "ID: " + id + ", Type: " + type;
        }
    }
}