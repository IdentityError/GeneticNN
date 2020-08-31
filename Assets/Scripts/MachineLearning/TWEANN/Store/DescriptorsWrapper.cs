using UnityEngine;

namespace Assets.Scripts.MachineLearning.TWEANN
{
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
        public struct MutationRatesDescriptor
        {
            public float maxWeightMutationRate;
            public float maxSplitLinkRate;
            public float maxAddLinkRate;
            [HideInInspector] public float weightMutationRate;
            [HideInInspector] public float splitLinkRate;
            [HideInInspector] public float addLinkRate;

            public override string ToString()
            {
                return "Weights\nCurrent: " + weightMutationRate + ", Max: " + maxWeightMutationRate + "\n" +
                    "Split Link\nCurrent: " + splitLinkRate + ", Max: " + maxSplitLinkRate + "\n" +
                    "Add Link\nCurrent: " + addLinkRate + ", Max: " + maxAddLinkRate + "\n";
            }
        }
    }
}