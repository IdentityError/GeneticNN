public class NeuralNet
{
    public DNA dna;

    public NeuralNet(DNA dna)
    {
        this.dna = dna;
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
            output[i] = TMath.Tanh(sum);
        }
        return output;
    }

    /// <summary>
    ///   Execute a process through the NeuralNet
    ///   <para> Return: the output vector (outputCount) </para>
    /// </summary>
    public float[] FeedForward(float[] inputs)
    {
        //Calculate the fist activation vector I_H0
        float[] firstLayer = ActivationVector_Matrix(inputs, dna.weights.i_h0Weights, DNA.INPUT_COUNT, dna.topology.neuronsAtLayer[0]);

        //Calculate the second activation vector H0_H1
        float[] current = ActivationVector_Matrix(firstLayer, dna.weights.intraNetWeights[0], dna.topology.neuronsAtLayer[0], dna.topology.neuronsAtLayer[1]);

        //Iterate through all of the remaining NeuralNet hidden layers Hi-1_Hi
        for (int i = 1; i < dna.topology.hiddenLayerCount - 1; i++)
        {
            current = ActivationVector_Matrix(current, dna.weights.intraNetWeights[i], dna.topology.neuronsAtLayer[i], dna.topology.neuronsAtLayer[i + 1]);
        }

        //Calculate the output vector Hn_O
        float[] output = ActivationVector_Matrix(current, dna.weights.hn_oWeights, dna.topology.neuronsAtLayer[dna.topology.hiddenLayerCount - 1], DNA.OUTPUT_COUNT);

        return output;
    }
}