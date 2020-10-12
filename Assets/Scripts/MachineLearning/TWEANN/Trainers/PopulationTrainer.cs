using Assets.Scripts.TUtils.Utils;
using System;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    public abstract class PopulationTrainer
    {
        private Genotype predefinedGenotype;

        /// <summary>
        ///   Get the predefined topology, in this case the input layer densely connected to the output layer
        /// </summary>
        /// <returns> </returns>
        public Genotype GetPredefinedGenotype()
        {
            predefinedGenotype = new Genotype();
            for (int i = 0; i < 7; i++)
            {
                NodeGene newNode = new NodeGene(-i - 1, TMath.Tanh);
                if (i < 5)
                {
                    newNode.SetType(NodeType.INPUT);
                }
                else if (i > 7 - 2 - 1)
                {
                    newNode.SetType(NodeType.OUTPUT);
                }
                predefinedGenotype.AddNode(newNode);
            }

            int inn = 0;
            foreach (NodeGene input in predefinedGenotype.inputs)
            {
                foreach (NodeGene output in predefinedGenotype.outputs)
                {
                    predefinedGenotype.AddLinkAndNodes(new LinkGene(input, output, UnityEngine.Random.Range(-1F, 1F), inn++));
                }
            }

            //for (int i = 1; i < 6; i++)
            //{
            //    NodeGene from = new NodeGene(-i, TMath.Tanh, NodeType.INPUT);
            //    for (int j = 6; j < 8; j++)
            //    {
            //        NodeGene to = new NodeGene(-j, TMath.Tanh, NodeType.OUTPUT);
            //        predefinedGenotype.AddLinkAndNodes(new LinkGene(from, to, UnityEngine.Random.Range(-1F, 1F)));
            //    }
            //}
            //for (int i = 1; i < descriptor.inputCount + 1; i++)
            //{
            //    for (int j = descriptor.inputCount + 1; j < descriptor.inputCount + 1 + descriptor.outputCount; j++)
            //    {
            //        descriptor.links.Add(new DescriptorsWrapper.LinkDescriptor(i, j));
            //    }
            //}
            //descriptor = new TopologyDescriptor(5, 16, 2);

            //for (int i = 1; i < 6; i++)
            //{
            //    for (int j = 6; j < 14; j++)
            //    {
            //        descriptor.links.Add(new LinkDescriptor(i, j));
            //    }
            //}
            //for (int i = 6; i < 14; i++)
            //{
            //    for (int j = 14; j < 22; j++)
            //    {
            //        descriptor.links.Add(new LinkDescriptor(i, j));
            //    }
            //}

            //for (int i = 14; i < 22; i++)
            //{
            //    for (int j = 22; j < 24; j++)
            //    {
            //        descriptor.links.Add(new LinkDescriptor(i, j));
            //    }
            //}
            return predefinedGenotype;
        }

        /// <summary>
        ///   <b> Train a new population </b>
        /// </summary>
        /// <param name="biocenosis"> The biocenosis of the population </param>
        /// <param name="breedingParameters"> The breeding parameters to use during crossover and mutation </param>
        /// <returns> Next generation of Genotypes </returns>
        public abstract Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[] Train(Biocenosis biocenosis);
    }
}