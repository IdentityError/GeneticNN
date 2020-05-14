using Assets.Scripts.Interfaces;
using Assets.Scripts.NeuralNet;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    public abstract class PopulationTrainer
    {
        protected IIndividual[] population;
        protected IIndividual[] newPopulation;
        protected int populationNumber;

        private List<Mutation> generationMutations = new List<Mutation>();
        private int globalInnovationNumber = 1;

        [Space(5)]
        [Header("Predefined Topology")]
        [SerializeField] private TopologyDescriptor descriptor;

        public TopologyDescriptor GetPredefinedTopologyDescriptor()
        {
            return descriptor;
        }

        /// <summary>
        ///   Train a new population in place
        /// </summary>
        /// <returns> </returns>
        public virtual void Train(IIndividual[] population)
        {
            populationNumber = population.Length;
            newPopulation = new IIndividual[populationNumber];
            population.CopyTo(newPopulation, 0);
            generationMutations.Clear();
        }

        public int GetGlobalInnovationNumber()
        {
            return globalInnovationNumber++;
        }

        protected void AddGenerationMutation(Mutation mutation)
        {
            generationMutations.Add(mutation);
        }
    }
}