using Assets.Scripts.TUtils.Utils;
using System;

namespace Assets.Scripts.MachineLearning
{
    [Obsolete("Needs further implementations")]
    public class DenseNetwork
    {
        public DNA dna;

        public DenseNetwork(DNA dna)
        {
            this.dna = dna;
        }

        /// <summary>
        ///   Execute a process through the NeuralNet
        ///   <para> Return: the output vector (outputCount) </para>
        /// </summary>
        public float[] FeedForward(float[] inputs)
        {
            float[] current = inputs;

            //Feed forward thorugh all the layers
            for (int i = 0; i < dna.topology.layerCount - 1; i++)
            {
                current = ActivationVector_Matrix(current, dna.weights[i], dna.topology.neuronsAtLayer[i], dna.topology.neuronsAtLayer[i + 1]);
            }
            return current;
        }

        /// <summary>
        ///   Multiplies a vector with a matrix
        ///   <para> Return: the result vector </para>
        /// </summary>
        private float[] ActivationVector_Matrix(float[] vector, float[][] matrix, int vectorLength, int matrixColumns)
        {
            float[] output = new float[matrixColumns];

            for (int i = 0; i < matrixColumns; i++)
            {
                float sum = 0;
                for (int j = 0; j < vectorLength; j++)
                {
                    sum += vector[j] * matrix[j][i];
                }
                output[i] = (float)TMath.Tanh(sum);
            }
            return output;
        }
    }
}