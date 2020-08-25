using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.MachineLearning.TWEANN
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
            foreach (NodeGene node in genotype.all)
            {
                node.ResetActivation();
            }

            //Set the inputs
            foreach (var item in inputs.Zip(genotype.inputs, Tuple.Create))
            {
                item.Item2.SetInputValue(item.Item1);
            }

            // Propagate the activation. For each node of the outputs and for each of its incoming links, calculate the activation. The
            // GetActivation function of the node recursively call Activate on each node that is not activated
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
            return FeedForward(inputs.ToList()).ToArray();
        }

        /// <summary>
        ///   Get the genotype of this Network
        /// </summary>
        /// <returns> </returns>
        public Genotype GetGenotype()
        {
            return genotype;
        }
    }
}