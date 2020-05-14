using Assets.Scripts.Stores;
using UnityEngine;

namespace Assets.Scripts.MachineLearning
{
    [System.Serializable]
    public class DNA
    {
        public float fitness = 0F;
        public float pickProbability = 0F;

        [System.Serializable]
        public class DnaTopology
        {
            [HideInInspector] public int layerCount;
            [HideInInspector] public int[] neuronsAtLayer;

            private int linkNumber;

            /// <summary>
            ///   Get the total link number in this DNA
            /// </summary>
            public int GetLinkNumber()
            {
                return linkNumber;
            }

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

            public void CalculateLinkNumber()
            {
                linkNumber = 0;
                for (int i = 0; i < layerCount - 1; i++)
                {
                    linkNumber += neuronsAtLayer[i] * neuronsAtLayer[i + 1];
                }
            }
        }

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
        ///   Each element of each weights matrix will mutate with a mutationRate probability
        /// </summary>
        public void Mutate(Paradigms.MutationParadigm mutationParadigm, Paradigms.MissingWeightsFillParadigm missingWeightsFillParadigm, float mutationRate)
        {
            switch (mutationParadigm)
            {
                case Paradigms.MutationParadigm.WEIGHTS:
                    MutateWeights(mutationRate);
                    break;

                case Paradigms.MutationParadigm.TOPOLOGY:
                    MutateTopology(missingWeightsFillParadigm, mutationRate);
                    break;

                case Paradigms.MutationParadigm.HYBRID:
                    MutateTopology(missingWeightsFillParadigm, mutationRate / 2);
                    MutateWeights(mutationRate / 2);
                    break;
            }
        }

        private void MutateTopology(Paradigms.MissingWeightsFillParadigm missingWeightsFillParadigm, float mutationRate)
        {
            bool mutated = false;
            int mutationsNumber = UnityEngine.Random.Range(1, 8);
            mutationsNumber = 1;
            for (int i = 0; i < mutationsNumber; i++)
            {
                if (UnityEngine.Random.Range(0F, 1F) < mutationRate)
                {
                    mutated = true;
                    //Select a random mutation type
                    Paradigms.TopologyMutationType type = GetTopologyMutationType();

                    //Select a random hidden layer to mutate
                    int layer = UnityEngine.Random.Range(1, topology.layerCount - 1);

                    //Select a random number of neurons
                    int neuronsNumber = UnityEngine.Random.Range(2, 14);

                    int[] temp;
                    switch (type)
                    {
                        case Paradigms.TopologyMutationType.HIDDEN_LAYER_ADD:
                            //Adding a new layer at position <layer> of <neuronsNumber> neurons
                            topology.layerCount++;
                            temp = new int[topology.layerCount];
                            TUtilsProvider.CopyArrayWithHolesAt(topology.neuronsAtLayer, temp, new int[] { layer });
                            topology.neuronsAtLayer = new int[topology.layerCount];
                            temp.CopyTo(topology.neuronsAtLayer, 0);
                            topology.neuronsAtLayer[layer] = neuronsNumber;
                            break;

                        case Paradigms.TopologyMutationType.HIDDEN_LAYER_REMOVE:
                            //Removing a layer at position <layer>
                            this.topology.layerCount--;
                            temp = new int[this.topology.layerCount];
                            TUtilsProvider.CopyArrayWithExceptsAt(this.topology.neuronsAtLayer, temp, new int[] { layer });
                            this.topology.neuronsAtLayer = new int[this.topology.layerCount];
                            temp.CopyTo(this.topology.neuronsAtLayer, 0);
                            break;

                        case Paradigms.TopologyMutationType.NEURONS_NUMBER_CHANGE:
                            this.topology.neuronsAtLayer[layer] = neuronsNumber;
                            break;
                    }
                }
            }
            if (mutated)
            {
                this.topology.CalculateLinkNumber();
                //After the mutations, readapt the weights matrices dimensions
                ReadaptWeightsToTopology(missingWeightsFillParadigm);
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

        private void ReadaptWeightsToTopology(Paradigms.MissingWeightsFillParadigm missingWeightsFillParadigm)
        {
            //Build a new weights matrix array with correct dimensions
            float[][][] newWeights = new float[topology.layerCount - 1][][];

            for (int i = 0; i < topology.layerCount - 1; i++)
            {
                newWeights[i] = new float[topology.neuronsAtLayer[i]][];
                for (int j = 0; j < topology.neuronsAtLayer[i]; j++)
                {
                    newWeights[i][j] = new float[topology.neuronsAtLayer[i + 1]];
                    for (int k = 0; k < topology.neuronsAtLayer[i + 1]; k++)
                    {
                        //If the link was already present, copy it
                        if (HasLinkAt(i, j, k))
                        {
                            newWeights[i][j][k] = this.weights[i][j][k];
                        }
                        else
                        {
                            switch (missingWeightsFillParadigm)
                            {
                                case Paradigms.MissingWeightsFillParadigm.RANDOM:
                                    newWeights[i][j][k] = UnityEngine.Random.Range(-1F, 1F);
                                    break;

                                case Paradigms.MissingWeightsFillParadigm.COPY:
                                    break;

                                case Paradigms.MissingWeightsFillParadigm.HYBRID:
                                    break;
                            }
                        }
                    }
                }
            }

            this.weights = newWeights;
        }

        /// <summary>
        ///   Implements the actual mixing of the DNA of two individuals. The equality parameters
        ///   determines how the DNA of the child is built from the the parents DNA
        ///   <para>
        ///     E.g setting the equality to 3 means that the partner is going to be responsible for
        ///     1 piece of DNA each 3, so its 1/3 responsible for trasmitting the DNA
        ///   </para>
        ///   <para> Return: the crossovered DNA </para>
        /// </summary>
        public DNA Crossover(DNA partnerDna, Paradigms.MissingWeightsFillParadigm missingWeightsFillParadigm, int equality)
        {
            //Copy the DNA from the fitness with higher fitness
            DNA childDna = this.fitness > partnerDna.fitness ? new DNA(this.topology, this.weights) : new DNA(partnerDna.topology, partnerDna.weights);

            //Debug.Log(childDna.topology.ToString());

            for (int i = 0; i < childDna.topology.layerCount - 2; i++)
            {
                for (int j = 0; j < childDna.topology.neuronsAtLayer[i]; j++)
                {
                    for (int k = 0; k < childDna.topology.neuronsAtLayer[i + 1]; k++)
                    {
                        if ((j + k) % equality == 0)
                        {
                            if (partnerDna.HasLinkAt(i, j, k))
                            {
                                childDna.weights[i][j][k] = partnerDna.weights[i][j][k];
                            }
                            else
                            {
                                switch (missingWeightsFillParadigm)
                                {
                                    case Paradigms.MissingWeightsFillParadigm.RANDOM:
                                        childDna.weights[i][j][k] = UnityEngine.Random.Range(0F, 1F);
                                        break;

                                    case Paradigms.MissingWeightsFillParadigm.COPY:
                                        childDna.weights[i][j][k] = UnityEngine.Random.Range(0F, 1F);
                                        break;

                                    case Paradigms.MissingWeightsFillParadigm.HYBRID:
                                        childDna.weights[i][j][k] = UnityEngine.Random.Range(0F, 1F);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return childDna;
        }

        /// <summary>
        ///   Get the topological distance between two DNA
        /// </summary>
        public float TopologicalDistance(DNA from)
        {
            float distance = 0;
            int bound = from.topology.layerCount < this.topology.layerCount ? from.topology.layerCount : this.topology.layerCount;
            for (int i = 0; i < bound; i++)
            {
                distance += TMath.Abs(from.topology.neuronsAtLayer[i] - this.topology.neuronsAtLayer[i]);
            }
            for (int i = bound; i < from.topology.layerCount; i++)
            {
                distance += from.topology.neuronsAtLayer[i];
            }
            for (int i = bound; i < this.topology.layerCount; i++)
            {
                distance += this.topology.neuronsAtLayer[i];
            }
            return distance;
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

        private bool HasLinkAt(int layer, int row, int col)
        {
            return (layer >= 0 && layer < weights.Length) &&
                   (row >= 0 && row < weights[layer].Length) &&
                   (col >= 0 && col < weights[layer][row].Length);
        }

        private Paradigms.TopologyMutationType GetTopologyMutationType()
        {
            Paradigms.TopologyMutationType[] paradigms = new Paradigms.TopologyMutationType[3];
            paradigms[0] = Paradigms.TopologyMutationType.HIDDEN_LAYER_ADD;
            paradigms[1] = Paradigms.TopologyMutationType.NEURONS_NUMBER_CHANGE;
            if (this.topology.layerCount > 3)
            {
                paradigms[2] = Paradigms.TopologyMutationType.HIDDEN_LAYER_REMOVE;
            }
            return paradigms[UnityEngine.Random.Range(0, 3)];
        }
    }
}