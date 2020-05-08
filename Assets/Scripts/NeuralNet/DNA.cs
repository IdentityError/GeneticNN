using Assets.Scripts.Stores;

[System.Serializable]
public class DNA
{
    public const int INPUT_COUNT = 5;
    public const int OUTPUT_COUNT = 2;

    public float fitness = 0F;
    public float pickProbability = 0F;

    [System.Serializable]
    public class DnaTopology
    {
        public int hiddenLayerCount;
        public int[] neuronsAtLayer;

        public DnaTopology(int hiddenLayerCount, int[] neuronsPerHiddenLayer)
        {
            if (neuronsPerHiddenLayer.Length != hiddenLayerCount)
            {
                throw new System.Exception("Hidden layers neuron vector has different length than the hidden layer count!");
            }
            this.hiddenLayerCount = hiddenLayerCount;
            this.neuronsAtLayer = new int[hiddenLayerCount];
            neuronsPerHiddenLayer.CopyTo(this.neuronsAtLayer, 0);
        }

        public bool IsValid()
        {
            if (hiddenLayerCount == 0) return true;
            if (neuronsAtLayer.Length == 0) return true;
            for (int i = 0; i < hiddenLayerCount; i++)
            {
                if (neuronsAtLayer[i] == 0) return true;
            }
            return false;
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
                this.i_h0Weights[i] = new float[topology.neuronsAtLayer[0]];
                for (int j = 0; j < topology.neuronsAtLayer[0]; j++)
                {
                    if (i_h0Weights == null)
                    {
                        this.i_h0Weights[i][j] = UnityEngine.Random.Range(-1F, 1F);
                    }
                    else
                    {
                        this.i_h0Weights[i][j] = i_h0Weights[i][j];
                    }
                }
            }

            //Weights matrices for all the hidden layers, 3D array required: intraNetWeights[layerNumber][neuron][neuron]
            this.intraNetWeights = new float[topology.hiddenLayerCount - 1][][];
            for (int i = 0; i < topology.hiddenLayerCount - 1; i++)
            {
                this.intraNetWeights[i] = new float[topology.neuronsAtLayer[i]][];
                for (int j = 0; j < topology.neuronsAtLayer[i]; j++)
                {
                    this.intraNetWeights[i][j] = new float[topology.neuronsAtLayer[i + 1]];
                    for (int k = 0; k < topology.neuronsAtLayer[i + 1]; k++)
                    {
                        if (intraNetWeights == null)
                        {
                            this.intraNetWeights[i][j][k] = UnityEngine.Random.Range(-1F, 1F);
                        }
                        else
                        {
                            this.intraNetWeights[i][j][k] = intraNetWeights[i][j][k];
                        }
                    }
                }
            }

            //Weights matrix for last layer and output Hn_O
            this.hn_oWeights = new float[topology.neuronsAtLayer[topology.hiddenLayerCount - 1]][];
            for (int i = 0; i < topology.neuronsAtLayer[topology.hiddenLayerCount - 1]; i++)
            {
                this.hn_oWeights[i] = new float[OUTPUT_COUNT];
                for (int j = 0; j < OUTPUT_COUNT; j++)
                {
                    if (hn_oWeights == null)
                    {
                        this.hn_oWeights[i][j] = UnityEngine.Random.Range(-1F, 1F);
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
        this.topology = new DnaTopology(topology.hiddenLayerCount, topology.neuronsAtLayer);
        this.weights = new DnaWeights(this.topology, null, null, null);
    }

    /// <summary>
    ///   Initialize a specified DNA
    /// </summary>
    public DNA(DnaTopology topology, DnaWeights weights)
    {
        this.topology = new DnaTopology(topology.hiddenLayerCount, topology.neuronsAtLayer);
        this.weights = new DnaWeights(this.topology, weights.i_h0Weights, weights.intraNetWeights, weights.hn_oWeights);
    }

    /// <summary>
    ///   Each element of each weights matrix will mutate with a mutationRate probability
    /// </summary>
    public void Mutate(Paradigms.MutationParadigm mutationParadigm, float mutationRate)
    {
        switch (mutationParadigm)
        {
            case Paradigms.MutationParadigm.WEIGHTS:
                MutateWeights(mutationRate);
                break;

            case Paradigms.MutationParadigm.TOPOLOGY:
                MutateTopology(mutationRate);
                break;

            case Paradigms.MutationParadigm.HYBRID:
                MutateWeights(mutationRate / 2);
                MutateTopology(mutationRate / 2);
                break;
        }
    }

    private void MutateTopology(float mutationRate)
    {
        //TODO implement
    }

    private void MutateWeights(float mutationRate)
    {
        for (int i = 0; i < INPUT_COUNT; i++)
        {
            for (int j = 0; j < topology.neuronsAtLayer[0]; j++)
            {
                if (UnityEngine.Random.Range(0F, 1F) < mutationRate)
                {
                    //Debug.Log("MUTATED");
                    this.weights.i_h0Weights[i][j] = UnityEngine.Random.Range(-1F, 1F);
                }
            }
        }

        for (int i = 0; i < topology.hiddenLayerCount - 1; i++)
        {
            for (int j = 0; j < topology.neuronsAtLayer[i]; j++)
            {
                for (int k = 0; k < topology.neuronsAtLayer[i + 1]; k++)
                {
                    if (UnityEngine.Random.Range(0F, 1F) < mutationRate)
                    {
                        this.weights.intraNetWeights[i][j][k] = UnityEngine.Random.Range(-1F, 1F);
                    }
                }
            }
        }

        for (int i = 0; i < topology.neuronsAtLayer[topology.hiddenLayerCount]; i++)
        {
            for (int j = 0; j < OUTPUT_COUNT; j++)
            {
                if (UnityEngine.Random.Range(0F, 1F) < mutationRate)
                {
                    this.weights.hn_oWeights[i][j] = UnityEngine.Random.Range(-1F, 1F);
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
            for (int j = 0; j < topology.neuronsAtLayer[0]; j++)
            {
                if ((i + j) % equality == 0)
                {
                    childDna.weights.i_h0Weights[i][j] = partnerDna.weights.i_h0Weights[i][j];
                }
            }
        }

        for (int i = 0; i < topology.hiddenLayerCount - 1; i++)
        {
            for (int j = 0; j < topology.neuronsAtLayer[i]; j++)
            {
                for (int k = 0; k < topology.neuronsAtLayer[i + 1]; k++)
                {
                    if ((j + k) % equality == 0)
                    {
                        childDna.weights.intraNetWeights[i][j][k] = partnerDna.weights.intraNetWeights[i][j][k];
                    }
                }
            }
        }

        for (int i = 0; i < topology.neuronsAtLayer[topology.hiddenLayerCount]; i++)
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