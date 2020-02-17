using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNet : MonoBehaviour
{
    public int inputsCount = 5;
    public int outputsCount = 3;
	public int hiddenLayerCount = 2;
	public int neuronsPerHiddenLayer = 5;

	[SerializeField] public int directionsCount;

	float[] inputs;
    float[] outputs;

    float[][] i_h0Weights;
    float[][] h0_h1Weights;
    float[][] h1_oWeights;


	void Awake()
    {
		directionsCount = inputsCount - 1;
		inputs = new float[inputsCount];
        outputs = new float[outputsCount];

		i_h0Weights = new float[inputsCount][];
		for(int i = 0; i < inputsCount; i++)
		{
			i_h0Weights[i] = new float[neuronsPerHiddenLayer];
			for(int j = 0; j < neuronsPerHiddenLayer; j++)
			{
				i_h0Weights[i][j] = Random.Range(0F, 1F);
			}
		}

		h0_h1Weights = new float[neuronsPerHiddenLayer][];
		for (int i = 0; i < neuronsPerHiddenLayer; i++)
		{
			h0_h1Weights[i] = new float[neuronsPerHiddenLayer];
			for (int j = 0; j < neuronsPerHiddenLayer; j++)
			{
				h0_h1Weights[i][j] = Random.Range(0F, 1F);
			}
		}

		h1_oWeights = new float[neuronsPerHiddenLayer][];
		for (int i = 0; i < neuronsPerHiddenLayer; i++)
		{
			h1_oWeights[i] = new float[outputsCount];
			for (int j = 0; j < outputsCount; j++)
			{
				h1_oWeights[i][j] = Random.Range(0F, 1F);
			}
		}
	}

	private float[][] MatricesMultiplication(float[][] a, int ra, int ca, float[][] b, int rb, int cb)
	{
		if (ra != cb)
		{
			return null;
		}

		float[][] matrix = new float[ra][];
		for (int i = 0; i < ra; i++)
		{
			matrix[i] = new float[cb];
		}

		float sum = 0;
		for (int i = 0; i < ra; i++)
		{
			for (int j = 0; j < cb; j++)
			{
				for (int k = 0; k < rb; k++)
				{
					sum = sum + a[i][k] * b[k][j];
				}

				matrix[i][j] = sum;
				sum = 0;
			}
		}
		return matrix;
	}
}
