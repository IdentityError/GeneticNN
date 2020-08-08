using UnityEngine;

namespace Assets.Scripts.Stores
{
    [System.Serializable]
    public class BreedingParameters
    {
        public BreedingParameters(float mutationProbability, float crossoverProbability)
        {
            this.mutationProbability = mutationProbability;
            this.crossoverProbability = crossoverProbability;
        }

        [Tooltip("Probability of mutating the gene")]
        [Range(0, 1)]
        public float mutationProbability;
        [Tooltip("Probability of executing a crossover for each individual")]
        [Range(0, 1)]
        public float crossoverProbability;
    }
}