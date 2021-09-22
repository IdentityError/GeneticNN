using System;
using UnityEngine;

[Serializable]
[Obsolete("Needs further implementations")]
public class DNA
{
    public DnaTopology topology;

    public float[][][] weights;

    /// <summary>
    ///  Initialize a DNA with a certain topology and a random set of weights
    ///</summary>
    public DNA(DnaTopology topology) : this(topology, null)
    {
    }

    /// <summary>
    ///   Initialize a specified DNA
    /// </summary>
    public DNA(DnaTopology topology, float[][][] weights)
    {
        this.topology = new DnaTopology(topology.neuronsAtLayer);
        InitializeWeights(weights);
    }

    /// <summary>
    ///   Implements the actual mixing of the DNA of two individuals. The equality parameters determines how the DNA of the child is built
    ///   from the the parents DNA
    ///   <para>
    ///     E.g setting the equality to 3 means that the partner is going to be responsible for 1 piece of DNA each 3, so its 1/3
    ///     responsible for trasmitting the DNA
    ///   </para>
    ///   <para> Return: the crossovered DNA </para>
    /// </summary>
    public DNA Crossover(DNA partnerDna, int equality)
    {
        //Copy the DNA from the fitness with higher fitness
        DNA childDna = new DNA(this.topology, this.weights);

        //Debug.Log(childDna.topology.ToString());

        for (int i = 0; i < childDna.topology.layerCount - 2; i++)
        {
            for (int j = 0; j < childDna.topology.neuronsAtLayer[i]; j++)
            {
                for (int k = 0; k < childDna.topology.neuronsAtLayer[i + 1]; k++)
                {
                    if ((j + k) % equality == 0)
                    {
                        childDna.weights[i][j][k] = partnerDna.weights[i][j][k];
                    }
                }
            }
        }

        return childDna;
    }

    /// <summary>
    ///   Each element of each weights matrix will mutate with a mutationRate probability
    /// </summary>
    public void Mutate(float mutationRate)
    {
        MutateWeights(mutationRate / 2);
    }

    private void InitializeWeights(float[][][] weights)
    {
        this.weights = new float[topology.layerCount - 1][][];

        for (int i = 0; i < topology.layerCount - 1; i++)
        {
            this.weights[i] = new float[topology.neuronsAtLayer[i]][];
            for (int j = 0; j < topology.neuronsAtLayer[i]; j++)
            {
                this.weights[i][j] = new float[topology.neuronsAtLayer[i + 1]];
                for (int k = 0; k < topology.neuronsAtLayer[i + 1]; k++)
                {
                    if (weights == null)
                    {
                        this.weights[i][j][k] = UnityEngine.Random.Range(-1F, 1F);
                    }
                    else
                    {
                        this.weights[i][j][k] = weights[i][j][k];
                    }
                }
            }
        }
    }

    private void MutateWeights(float mutationRate)
    {
        int mutationsNumber = UnityEngine.Random.Range(1, topology.layerCount);

        for (int i = 0; i < mutationsNumber; i++)
        {
            if (UnityEngine.Random.Range(0F, 1F) < mutationRate)
            {
                int layer = UnityEngine.Random.Range(0, this.topology.layerCount - 1);
                int row, col;
                //Selected the input weight matrix
                if (layer == 0)
                {
                    Debug.Log("Mutating the input matrix");
                    row = UnityEngine.Random.Range(0, topology.InputCount);
                    col = UnityEngine.Random.Range(0, this.topology.neuronsAtLayer[1]);
                }
                //Selected the output weight matrix
                else if (layer == (this.topology.layerCount - 2))
                {
                    Debug.Log("Mutating the output matrix");
                    row = UnityEngine.Random.Range(0, this.topology.neuronsAtLayer[layer]);
                    col = UnityEngine.Random.Range(0, topology.OutputCount);
                }
                //Selected a hidden weight matrix
                else
                {
                    Debug.Log("Mutating the matrix");
                    row = UnityEngine.Random.Range(0, this.topology.neuronsAtLayer[layer]);
                    col = UnityEngine.Random.Range(0, this.topology.neuronsAtLayer[layer + 1]);
                }

                this.weights[layer][row][col] = UnityEngine.Random.Range(0F, 1F);
            }
        }
    }

    [System.Serializable]
    public class DnaTopology
    {
        [HideInInspector] public int layerCount;
        [HideInInspector] public int[] neuronsAtLayer;

        private int linkNumber;

        public int InputCount
        {
            get
            {
                return neuronsAtLayer[0];
            }
        }

        public int OutputCount
        {
            get
            {
                return neuronsAtLayer[layerCount - 1];
            }
        }

        public DnaTopology(int[] neuronsAtLayer)
        {
            layerCount = neuronsAtLayer.Length;

            this.neuronsAtLayer = new int[layerCount];
            neuronsAtLayer.CopyTo(this.neuronsAtLayer, 0);
            CalculateLinkNumber();
        }

        public void CalculateLinkNumber()
        {
            linkNumber = 0;
            for (int i = 0; i < layerCount - 1; i++)
            {
                linkNumber += neuronsAtLayer[i] * neuronsAtLayer[i + 1];
            }
        }

        /// <summary>
        ///   Get the total link number in this DNA
        /// </summary>
        public int GetLinkNumber()
        {
            return linkNumber;
        }

        public bool IsValid()
        {
            if (layerCount < 3) return false;
            if (neuronsAtLayer.Length == 0) return false;
            for (int i = 1; i < layerCount - 1; i++)
            {
                if (neuronsAtLayer[i] == 0) return false;
            }
            return true;
        }

        public override string ToString()
        {
            string code = "Links Number: " + linkNumber + "\n";
            for (int i = 0; i < layerCount; i++)
            {
                code += "(" + neuronsAtLayer[i] + ")";
                if (i < layerCount - 1)
                {
                    code += "->";
                }
            }
            return code;
        }
    }
}