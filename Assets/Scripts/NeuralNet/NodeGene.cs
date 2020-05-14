using System;
using System.Collections.Generic;

namespace Assets.Scripts.NeuralNet
{
    public enum NodeType { INPUT, HIDDEN, OUTPUT }

    public class NodeGene
    {
        public Func<double, double> activationFunction;
        public int id;
        private List<LinkGene> incoming;
        private bool activated = false;
        private double activation, activationSum;
        private NodeType position;

        public NodeGene(int id, Func<double, double> activationFunction, List<LinkGene> incoming, NodeType position) : this(id, activationFunction)
        {
            if (incoming != null)
            {
                this.incoming = new List<LinkGene>(incoming);
            }

            this.position = position;
        }

        public NodeGene(int id, Func<double, double> activationFunction, NodeType position) : this(id, activationFunction, null, position)
        {
        }

        public NodeGene(int id, Func<double, double> activationFunction)
        {
            this.id = id;
            this.activationFunction = activationFunction;
            this.incoming = new List<LinkGene>();
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
        ///   Activates the current node -&gt; calculate the activation value and activation sum
        /// </summary>
        private void Activate()
        {
            if (!activated)
            {
                double sum = 0;
                foreach (LinkGene link in incoming)
                {
                    sum += link.From().GetActivation() * link.weight;
                }
                activationSum = sum;
                activation = activationFunction(activationSum);
                activated = true;
            }
        }

        public bool IsActivated()
        {
            return activated;
        }

        /// <summary>
        ///   Sets the node activation manually. Can be set only on inputs type nodes
        /// </summary>
        /// <param name="activation"> </param>
        public void SetInputValue(double activation)
        {
            if (position == NodeType.INPUT)
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
            this.position = position;
        }

        public NodeType GetType()
        {
            return position;
        }

        public void AddIncomingLink(LinkGene link)
        {
            incoming.Add(link);
        }

        public List<LinkGene> GetIncomingLinks()
        {
            return incoming;
        }

        public NodeGene Copy()
        {
            return new NodeGene(id, activationFunction, new List<LinkGene>(GetIncomingLinks()), GetType());
        }
    }
}