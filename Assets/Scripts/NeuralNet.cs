using UnityEngine;

public class NeuralNet : MonoBehaviour
{
    [HideInInspector] public int inputsCount = 5;
    [HideInInspector] public int outputsCount = 2;

    [Header("Topology")]
    [Tooltip("Number of hidden layers of the Neural Network")]
    public int hiddenLayerCount = 2;

    [Tooltip("Number of neurons for each hidden layers of the Neural Network")]
    public int neuronsPerHiddenLayer = 5;

    private float[][] i_h0Weights;

    private float[][][] intraNetWeights;

    private float[][] h1_oWeights;

    public void InitializeNet()
    {
        //TODO Make the weights be serialized to a json (?)

        //Weights matrix for the first layer I_H0
        i_h0Weights = new float[inputsCount][];
        for (int i = 0; i < inputsCount; i++)
        {
            i_h0Weights[i] = new float[neuronsPerHiddenLayer];
            for (int j = 0; j < neuronsPerHiddenLayer; j++)
            {
                i_h0Weights[i][j] = Random.Range(-1F, 1F);
            }
        }

        //Weights matrices for all the hidden layers, 3D array required: intraNetWeights[layerNumber][neuron][neuron]
        intraNetWeights = new float[hiddenLayerCount][][];
        for (int i = 0; i < hiddenLayerCount; i++)
        {
            intraNetWeights[i] = new float[neuronsPerHiddenLayer][];
            for (int j = 0; j < neuronsPerHiddenLayer; j++)
            {
                intraNetWeights[i][j] = new float[neuronsPerHiddenLayer];
                for (int k = 0; k < neuronsPerHiddenLayer; k++)
                {
                    intraNetWeights[i][j][k] = Random.Range(-1F, 1F);
                }
            }
        }

        //Weights matrix for last layer and output Hn_O
        h1_oWeights = new float[neuronsPerHiddenLayer][];
        for (int i = 0; i < neuronsPerHiddenLayer; i++)
        {
            h1_oWeights[i] = new float[outputsCount];
            for (int j = 0; j < outputsCount; j++)
            {
                h1_oWeights[i][j] = Random.Range(-1F, 1F);
            }
        }
    }

    /// <summary>
    /// Multiplies a vector with a matrix
    /// <para> Return: the result vector </para>
    /// </summary>
    private float[] Vector_Matrix(float[] vector, float[][] matrix, int r, int c)
    {
        float[] output = new float[c];

        for (int i = 0; i < c; i++)
        {
            float sum = 0;
            for (int j = 0; j < r; j++)
            {
                sum += vector[j] * matrix[j][i];
            }
            output[i] = Tanh(sum);
        }

        return output;
    }

    /// <summary>
    /// Execute a process through the NeuralNet
    /// <para> Return: the output vector (outputsCount) </para>
    /// </summary>
    public float[] Process(float[] inputs)
    {
        //Calculate the fist activation vector I_H0
        float[] firstLayer = Vector_Matrix(inputs, i_h0Weights, inputsCount, neuronsPerHiddenLayer);

        //Calculate the second activation vector H0_H1
        float[] current = Vector_Matrix(firstLayer, intraNetWeights[0], neuronsPerHiddenLayer, neuronsPerHiddenLayer);

        //Iterate through all of the remaining NeuralNet hidden layers Hi-1_Hi
        for (int i = 1; i < hiddenLayerCount - 1; i++)
        {
            current = Vector_Matrix(current, intraNetWeights[i], neuronsPerHiddenLayer, neuronsPerHiddenLayer);
        }

        //Calculate the output vector Hn_O
        float[] output = Vector_Matrix(current, h1_oWeights, neuronsPerHiddenLayer, outputsCount);

        return output;
    }

    private float Tanh(float x)
    {
        float exp = Mathf.Exp(2 * x);
        return (exp - 1) / (exp + 1);
    }
}