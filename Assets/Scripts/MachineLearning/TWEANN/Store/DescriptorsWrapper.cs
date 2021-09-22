using UnityEngine;

public class DescriptorsWrapper
{
    public struct CrossoverOperationDescriptor
    {
        public double parentFitness;
        public double parent1Fitness;
        public CrossoverOperator operatorUsed;

        public CrossoverOperationDescriptor(double parentFitness, double parent1Fitness, CrossoverOperator operatorUsed)
        {
            this.parentFitness = parentFitness;
            this.parent1Fitness = parent1Fitness;
            this.operatorUsed = operatorUsed;
        }
    }

    [System.Serializable]
    public struct RatesDescriptor
    {
        public float crossoverRatio;
        public float weightMutationRate;
        public float splitLinkRate;
        public float addLinkRate;

        public override string ToString()
        {
            return "Weights\nCurrent: " + weightMutationRate + "\n" +
                "Split LinkGene\nCurrent: " + splitLinkRate + "\n" +
                "Add LinkGene\nCurrent: " + addLinkRate + "\n" +
                "Crossover rate: " + crossoverRatio + "\n";
        }
    }

    [System.Serializable]
    public struct TrainerPreferences
    {
        [Header("Mutations")]
        public float maxWeightMutationRate;
        public float maxSplitLinkRate;
        public float maxAddLinkRate;
        [Header("Crossover")]
        public float minCrossoverRatio;
        [Header("Speciation")]
        public float sharingThreshold;
        [Header("Operators")]
        public bool dynamicRates;
        public bool enhancedSelectionOperator;

        [HideInInspector] public float maxAchievableFitness;

        [Header("Crossover operators")]
        public bool uniformEnabled;
        public bool singlePointEnabled;
        public bool kPointEnabled;
        public bool averageEnabled;
    }

    [System.Serializable]
    public struct OptionsWrapper
    {
        [Header("Parameters")]
        [Tooltip("Number of the population")]
        public int populationNumber;
        public TrainerPreferences preferences;
    }
}