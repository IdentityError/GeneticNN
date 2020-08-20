using Assets.Scripts.Descriptors;
using Assets.Scripts.NeuralNet;
using Assets.Scripts.TUtils.Utils;
using System;

namespace Assets.Scripts.TWEANN
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
            for (int i = 1; i < 6; i++)
            {
                for (int j = 1; j < 3; j++)
                {
                    predefinedGenotype.AddLinkAndNodes(new LinkGene(new NodeGene(i, TMath.Tanh), new NodeGene(j, TMath.Tanh)));
                }
            }
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
        ///   <b> Train a new population </b><br> </br> The Biocenosis needs to be speciated since it will be used for the intra-species
        ///   selection, be sure to call
        ///   <code>Biocenosis.Speciate([]) </code>
        ///   before passing it
        /// </summary>
        /// <param name="biocenosis">
        ///   The biocenosis of the population. Note that the biocenosis needs to be speciated before passing it to this function
        /// </param>
        /// <param name="breedingParameters"> The breeding parameters to use during crossover and mutation </param>
        /// <returns> Next generation of NeuralNetworks </returns>
        public abstract Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[] Train(Biocenosis biocenosis);
    }
}