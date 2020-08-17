using Assets.Scripts.NeuralNet;
using Assets.Scripts.TWEANN;
using System.Collections.Generic;

namespace Assets.Scripts.Descriptors
{
    public class DescriptorsWrapper
    {
        public struct OffspringDescriptor
        {
            public IIndividual parent;
            public IIndividual parent1;
            public Genotype childGen;
            public CrossoverOperator operatorUsed;

            public OffspringDescriptor(IIndividual parent, IIndividual parent1, Genotype childGen, CrossoverOperator operatorUsed)
            {
                this.parent = parent;
                this.parent1 = parent1;
                this.childGen = childGen;
                this.operatorUsed = operatorUsed;
            }
        }

        [System.Serializable]
        public struct LinkDescriptor
        {
            public LinkDescriptor(int fromId, int toId)
            {
                this.fromId = fromId;
                this.toId = toId;
            }

            public int fromId;
            public int toId;
        }

        [System.Serializable]
        public class TopologyDescriptor
        {
            public int inputCount;
            public int outputCount;
            public int hiddenCount;
            public List<LinkDescriptor> links = new List<LinkDescriptor>();

            public int NodeCount
            {
                get => inputCount + hiddenCount + outputCount;
            }

            public TopologyDescriptor(int inputCount, int hiddenCount, int outputCount)
            {
                this.inputCount = inputCount;
                this.hiddenCount = hiddenCount;
                this.outputCount = outputCount;
            }
        }

        public struct CrossoverOperationDescriptor
        {
            public IIndividual parent;
            public IIndividual parent1;
            public IIndividual child;
            public CrossoverOperator operatorUsed;

            public CrossoverOperationDescriptor(IIndividual parent, IIndividual parent1, IIndividual child, CrossoverOperator operatorUsed)
            {
                this.parent = parent;
                this.parent1 = parent1;
                this.child = child;
                this.operatorUsed = operatorUsed;
            }
        }
    }
}