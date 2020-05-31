using UnityEngine;

namespace Assets.Scripts.Stores
{
    [System.Serializable]
    public class BreedingParameters
    {
        [Tooltip("Probability of adding a new non existing link")]
        [Range(0, 1)]
        public float addLinkProb;
        [Tooltip("Probability of splitting a link into two links and inserting a new node")]
        [Range(0, 1)]
        public float splitLinkProb;
        [Tooltip("Probability for each link to be randomly mutated")]
        [Range(0, 1)]
        public float weightChangeProb;
        [Tooltip("Probability of executing a crossover for each individual")]
        [Range(0, 1)]
        public float crossoverProbability;
    }
}