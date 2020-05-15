using Assets.Scripts.Interfaces;
using Assets.Scripts.NeuralNet;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    public abstract class PopulationTrainer
    {
        protected class IndividualIdentifier
        {
            public Genotype genotype;
            public double fitness;
            public double pickProbability;

            public IndividualIdentifier(Genotype genotype, double fitness, double pickProbability)
            {
                this.genotype = genotype;
                this.fitness = fitness;
                this.pickProbability = pickProbability;
            }
        }

        protected IndividualIdentifier[] identifierPopulation;
        protected int populationNumber;

        [Space(5)]
        [Header("Predefined Topology")]
        [SerializeField] private TopologyDescriptor descriptor;

        public TopologyDescriptor GetPredefinedTopologyDescriptor()
        {
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
            return descriptor;
        }

        /// <summary>
        ///   Train a new population in place
        /// </summary>
        /// <returns> </returns>
        public virtual void Train(IIndividual[] population)
        {
            populationNumber = population.Length;

            identifierPopulation = new IndividualIdentifier[populationNumber];

            for (int i = 0; i < populationNumber; i++)
            {
                identifierPopulation[i] = new IndividualIdentifier(population[i].ProvideNeuralNet().GetGenotype(), population[i].ProvideFitness(), 0D);
            }
            GlobalParams.ResetGenerationMutations();
        }

        protected IndividualIdentifier PickRandom()
        {
            double seed = UnityEngine.Random.Range(0F, 1F);
            int index = -1;
            while (seed > 0)
            {
                seed -= identifierPopulation[++index].pickProbability;
            }
            return identifierPopulation[index];
        }

        protected void NormalizePopulationPickProbability()
        {
            double fitnessSum = 0F;
            for (int i = 0; i < populationNumber; i++)
            {
                fitnessSum += identifierPopulation[i].fitness;
            }
            for (int i = 0; i < populationNumber; i++)
            {
                identifierPopulation[i].pickProbability = identifierPopulation[i].fitness / fitnessSum;
            }
        }

        protected (IndividualIdentifier, IndividualIdentifier) PickFittestTwo()
        {
            IndividualIdentifier first = null;
            double firstFitness = -1F;
            foreach (IndividualIdentifier car in identifierPopulation)
            {
                if (car.fitness > firstFitness)
                {
                    first = car;
                    firstFitness = car.fitness;
                }
            }
            double secondFitness = -1F;
            IndividualIdentifier second = null;
            foreach (IndividualIdentifier car in identifierPopulation)
            {
                if (car.fitness > secondFitness)
                {
                    if (populationNumber > 1)
                    {
                        if (car != first)
                        {
                            second = car;
                            secondFitness = car.fitness;
                        }
                    }
                    else
                    {
                        second = car;
                        secondFitness = car.fitness;
                    }
                }
            }
            return (first, second);
        }
    }
}