using Assets.Scripts.NeuralNet;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    public abstract class PopulationTrainer
    {
        [Space(5)]
        [Header("Predefined Topology")]
        [SerializeField] private TopologyDescriptor descriptor;

        public TopologyDescriptor GetPredefinedTopologyDescriptor()
        {
            descriptor = new TopologyDescriptor(5, 16, 2);

            for (int i = 1; i < 6; i++)
            {
                for (int j = 6; j < 14; j++)
                {
                    descriptor.links.Add(new LinkDescriptor(i, j));
                }
            }
            for (int i = 6; i < 14; i++)
            {
                for (int j = 14; j < 22; j++)
                {
                    descriptor.links.Add(new LinkDescriptor(i, j));
                }
            }

            for (int i = 14; i < 22; i++)
            {
                for (int j = 22; j < 24; j++)
                {
                    descriptor.links.Add(new LinkDescriptor(i, j));
                }
            }
            return descriptor;
        }

        /// <summary>
        ///   <b> Train a new population </b><br> </br> The Biocenosis needs to be speciated since it will be used for the intra-species
        ///   selection, be sure to call
        ///   <code>Biocenosis.Speciate([]) </code>
        ///   to the passed Biocenosis
        /// </summary>
        /// <returns> </returns>
        public abstract NeuralNetwork[] Train(Biocenosis genotypesSpecies);
    }
}