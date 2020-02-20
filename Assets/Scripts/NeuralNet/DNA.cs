using UnityEngine;

[System.Serializable]
public class DNA
{
    [System.Serializable]
    public struct DnaTopology
    {
        public int inputCount;
        public int outputCount;
        public int hiddenLayerCount;
        public int neuronsPerHiddenLayer;

        public DnaTopology(int inputCount, int outputCount, int hiddenLayerCount, int neuronsPerHiddenLayer)
        {
            this.inputCount = inputCount;
            this.outputCount = outputCount;
            this.hiddenLayerCount = hiddenLayerCount;
            this.neuronsPerHiddenLayer = neuronsPerHiddenLayer;
        }
    }

    [System.Serializable]
    public struct DnaWeights
    {
        public float[][] i_h0Weights;
        public float[][][] intraNetWeights;
        public float[][] hn_oWeights;

        public DnaWeights(float[][] i_h0Weights, float[][][] intraNetWeights, float[][] hn_oWeights)
        {
            this.i_h0Weights = i_h0Weights;
            this.intraNetWeights = intraNetWeights;
            this.hn_oWeights = hn_oWeights;
        }
    }

    public DnaTopology topology;

    public DnaWeights weights;

    /// <summary>
    ///  Initialize a DNA with a random set of weights
    ///</summary>
    public DNA(DnaTopology parameters)
    {
        this.topology = parameters;
        InitializeRandomDNAWeights();
    }

    public DNA(DnaTopology parameters, DnaWeights weights)
    {
        this.topology = parameters;
        this.weights = weights;
    }

    private void InitializeRandomDNAWeights()
    {
        //Weights matrix for the first layer I_H0
        weights.i_h0Weights = new float[topology.inputCount][];
        for (int i = 0; i < topology.inputCount; i++)
        {
            weights.i_h0Weights[i] = new float[topology.neuronsPerHiddenLayer];
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                weights.i_h0Weights[i][j] = Random.Range(-1F, 1F);
            }
        }

        //Weights matrices for all the hidden layers, 3D array required: intraNetWeights[layerNumber][neuron][neuron]
        weights.intraNetWeights = new float[topology.hiddenLayerCount][][];
        for (int i = 0; i < topology.hiddenLayerCount; i++)
        {
            weights.intraNetWeights[i] = new float[topology.neuronsPerHiddenLayer][];
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                weights.intraNetWeights[i][j] = new float[topology.neuronsPerHiddenLayer];
                for (int k = 0; k < topology.neuronsPerHiddenLayer; k++)
                {
                    weights.intraNetWeights[i][j][k] = Random.Range(-1F, 1F);
                }
            }
        }

        //Weights matrix for last layer and output Hn_O
        weights.hn_oWeights = new float[topology.neuronsPerHiddenLayer][];
        for (int i = 0; i < topology.neuronsPerHiddenLayer; i++)
        {
            weights.hn_oWeights[i] = new float[topology.outputCount];
            for (int j = 0; j < topology.outputCount; j++)
            {
                weights.hn_oWeights[i][j] = Random.Range(-1F, 1F);
            }
        }
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < topology.inputCount; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                if (Random.Range(0F, 1F) < mutationRate)
                {
                    this.weights.i_h0Weights[i][j] = Random.Range(-1F, 1F);
                }
            }
        }

        for (int i = 0; i < topology.hiddenLayerCount; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                for (int k = 0; k < topology.neuronsPerHiddenLayer; k++)
                {
                    if (Random.Range(0F, 1F) < mutationRate)
                    {
                        this.weights.intraNetWeights[i][j][k] = Random.Range(-1F, 1F);
                    }
                }
            }
        }

        for (int i = 0; i < topology.neuronsPerHiddenLayer; i++)
        {
            for (int j = 0; j < topology.outputCount; j++)
            {
                if (Random.Range(0F, 1F) < mutationRate)
                {
                    this.weights.hn_oWeights[i][j] = Random.Range(-1F, 1F);
                }
            }
        }
    }

    public DNA Crossover(DNA partnerDna)
    {
        DNA childDna = new DNA(this.topology, this.weights);

        for (int i = 0; i < topology.inputCount; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    childDna.weights.i_h0Weights[i][j] = -partnerDna.weights.i_h0Weights[i][j];
                }
            }
        }

        for (int i = 0; i < topology.hiddenLayerCount; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                for (int k = 0; k < topology.neuronsPerHiddenLayer; k++)
                {
                    if ((i + j + k) % 2 == 0)
                    {
                        childDna.weights.intraNetWeights[i][j][k] = -partnerDna.weights.intraNetWeights[i][j][k];
                    }
                }
            }
        }

        for (int i = 0; i < topology.neuronsPerHiddenLayer; i++)
        {
            for (int j = 0; j < topology.outputCount; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    childDna.weights.hn_oWeights[i][j] = -partnerDna.weights.hn_oWeights[i][j];
                }
            }
        }

        return childDna;
    }
}