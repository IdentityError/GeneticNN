using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.NeuralNet
{
    public class NeuralNetwork
    {
        private Genotype genotype;

        public NeuralNetwork(Genotype genotype)
        {
            this.genotype = genotype;
        }

        /// <summary>
        ///   Process the inputs and get the outputs
        /// </summary>
        /// <param name="inputs"> </param>
        /// <returns> </returns>
        public List<double> FeedForward(List<double> inputs)
        {
            //Set the inputs
            foreach (var item in inputs.Zip(genotype.inputs, Tuple.Create))
            {
                item.Item2.SetInputValue(item.Item1);
            }

            // Propagate the activation. For each node of the outputs and for each of its incoming
            // links, calculate the activation. The GetActivation function of the node recursively
            // call Activate on each node that is not activated
            List<double> outputs = new List<double>();
            foreach (NodeGene node in genotype.outputs)
            {
                outputs.Add(node.GetActivation());
            }
            return outputs;
        }

        /// <summary>
        ///   Process the inputs and get the outputs
        /// </summary>
        /// <param name="inputs"> </param>
        /// <returns> </returns>
        public double[] FeedForward(double[] inputs)
        {
            List<double> inputsList = inputs.ToList();
            List<double> outputsList = FeedForward(inputsList);
            return outputsList.ToArray();
        }

        public Genotype GetGenotype()
        {
            return genotype;
        }
    }
}