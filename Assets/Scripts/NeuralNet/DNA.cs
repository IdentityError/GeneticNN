using Assets.Scripts.Stores;
using UnityEngine;

[System.Serializable]
public class DNA
{
    public const int INPUT_COUNT = 5;
    public const int OUTPUT_COUNT = 2;

    public float fitness = 0F;

    [System.Serializable]
    public class DnaTopology
    {
        public int hiddenLayerCount;
        public int neuronsPerHiddenLayer;

        public DnaTopology(int hiddenLayerCount, int neuronsPerHiddenLayer)
        {
            this.hiddenLayerCount = hiddenLayerCount;
            this.neuronsPerHiddenLayer = neuronsPerHiddenLayer;
        }
    }

    [System.Serializable]
    public class DnaWeights
    {
        public float[][] i_h0Weights;
        public float[][][] intraNetWeights;
        public float[][] hn_oWeights;

        public DnaWeights(DnaTopology topology, float[][] i_h0Weights, float[][][] intraNetWeights, float[][] hn_oWeights)
        {
            //Weights matrix for the first layer I_H0
            this.i_h0Weights = new float[INPUT_COUNT][];
            for (int i = 0; i < INPUT_COUNT; i++)
            {
                this.i_h0Weights[i] = new float[topology.neuronsPerHiddenLayer];
                for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
                {
                    if (i_h0Weights == null)
                    {
                        this.i_h0Weights[i][j] = Random.Range(-1F, 1F);
                    }
                    else
                    {
                        this.i_h0Weights[i][j] = i_h0Weights[i][j];
                    }
                }
            }

            //Weights matrices for all the hidden layers, 3D array required: intraNetWeights[layerNumber][neuron][neuron]
            this.intraNetWeights = new float[topology.hiddenLayerCount][][];
            for (int i = 0; i < topology.hiddenLayerCount; i++)
            {
                this.intraNetWeights[i] = new float[topology.neuronsPerHiddenLayer][];
                for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
                {
                    this.intraNetWeights[i][j] = new float[topology.neuronsPerHiddenLayer];
                    for (int k = 0; k < topology.neuronsPerHiddenLayer; k++)
                    {
                        if (intraNetWeights == null)
                        {
                            this.intraNetWeights[i][j][k] = Random.Range(-1F, 1F);
                        }
                        else
                        {
                            this.intraNetWeights[i][j][k] = intraNetWeights[i][j][k];
                        }
                    }
                }
            }

            //Weights matrix for last layer and output Hn_O
            this.hn_oWeights = new float[topology.neuronsPerHiddenLayer][];
            for (int i = 0; i < topology.neuronsPerHiddenLayer; i++)
            {
                this.hn_oWeights[i] = new float[OUTPUT_COUNT];
                for (int j = 0; j < OUTPUT_COUNT; j++)
                {
                    if (hn_oWeights == null)
                    {
                        this.hn_oWeights[i][j] = Random.Range(-1F, 1F);
                    }
                    else
                    {
                        this.hn_oWeights[i][j] = hn_oWeights[i][j];
                    }
                }
            }
        }
    }

    public DnaTopology topology;

    public DnaWeights weights;

    /// <summary>
    ///  Initialize a DNA with a certain topology and a random set of weights
    ///</summary>
    public DNA(DnaTopology topology)
    {
        this.topology = new DnaTopology(topology.hiddenLayerCount, topology.neuronsPerHiddenLayer);
        this.weights = new DnaWeights(this.topology, null, null, null);
    }

    /// <summary>
    ///   Initialize a specified DNA
    /// </summary>
    public DNA(DnaTopology topology, DnaWeights weights)
    {
        this.topology = new DnaTopology(topology.hiddenLayerCount, topology.neuronsPerHiddenLayer);
        this.weights = new DnaWeights(this.topology, weights.i_h0Weights, weights.intraNetWeights, weights.hn_oWeights);
    }

    /// <summary>
    ///   Each element of each weights matrix will mutate with a mutationRate probability
    /// </summary>
    public void Mutate(Paradigms.MutationParadigm mutationParadigm, float mutationRate)
    {
        for (int i = 0; i < INPUT_COUNT; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                if (Random.Range(0F, 1F) < mutationRate)
                {
                    //Debug.Log("MUTATED");
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
            for (int j = 0; j < OUTPUT_COUNT; j++)
            {
                if (Random.Range(0F, 1F) < mutationRate)
                {
                    this.weights.hn_oWeights[i][j] = Random.Range(-1F, 1F);
                }
            }
        }
    }

    /// <summary>
    ///   Implements the actual mixing of the DNA of two individuals. The equality parameters
    ///   determines how the DNA of the child is built from the the parents DNA
    ///   <para>
    ///     E.g setting the equality to 3 means that the partner is going to be responsible for 1
    ///     piece of DNA each 3, so its 1/3 responsible for trasmitting the DNA
    ///   </para>
    ///   <para> Return: the crossovered DNA </para>
    /// </summary>
    public DNA Crossover(DNA partnerDna, int equality)
    {
        DNA childDna = new DNA(this.topology, this.weights);

        for (int i = 0; i < INPUT_COUNT; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                if ((i + j) % equality == 0)
                {
                    childDna.weights.i_h0Weights[i][j] = partnerDna.weights.i_h0Weights[i][j];
                }
            }
        }

        for (int i = 0; i < topology.hiddenLayerCount; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                for (int k = 0; k < topology.neuronsPerHiddenLayer; k++)
                {
                    if ((j + k) % equality == 0)
                    {
                        childDna.weights.intraNetWeights[i][j][k] = partnerDna.weights.intraNetWeights[i][j][k];
                    }
                }
            }
        }

        for (int i = 0; i < topology.neuronsPerHiddenLayer; i++)
        {
            for (int j = 0; j < OUTPUT_COUNT; j++)
            {
                if ((i + j) % equality == 0)
                {
                    childDna.weights.hn_oWeights[i][j] = partnerDna.weights.hn_oWeights[i][j];
                }
            }
        }
        return childDna;
    }
}