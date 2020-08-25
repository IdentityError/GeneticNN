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
    }
}