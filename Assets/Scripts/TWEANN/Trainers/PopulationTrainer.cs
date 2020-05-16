using Assets.Scripts.NeuralNet;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    public abstract class PopulationTrainer
    {
        protected class IndividualDescriptor
        {
            public Genotype genotype;
            public double fitness;
            public double pickProbability;

            public IndividualDescriptor(Genotype genotype, double fitness, double pickProbability)
            {
                this.genotype = genotype;
                this.fitness = fitness;
                this.pickProbability = pickProbability;
            }
        }

        protected IndividualDescriptor[] identifierPopulation;
        protected int populationNumber;
        protected Biocenosis newBiocenosis;

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
        ///   Train a new population in place, the Biocenosis gets updated automatically and the new population individuals are splitted
        ///   into the Biocenosis species based on their topologies. Even if the population is not passed by reference, <b> each element is
        ///   modified using the interface </b>, so the population passed will be overridden with the new population
        /// </summary>
        /// <returns> </returns>
        public virtual void Train(IIndividual[] population, ref Biocenosis genotypesSpecies)
        {
            newBiocenosis = new Biocenosis();
            populationNumber = population.Length;
            identifierPopulation = new IndividualDescriptor[populationNumber];
            for (int i = 0; i < populationNumber; i++)
            {
                identifierPopulation[i] = new IndividualDescriptor(population[i].ProvideNeuralNet().GetGenotype(), population[i].ProvideFitness(), 0D);
            }
            GlobalParams.ResetGenerationMutations();
        }

        protected IndividualDescriptor PickRandom()
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

        protected (IndividualDescriptor, IndividualDescriptor) PickFittestTwo()
        {
            IndividualDescriptor first = null;
            double firstFitness = -1F;
            foreach (IndividualDescriptor car in identifierPopulation)
            {
                if (car.fitness > firstFitness)
                {
                    first = car;
                    firstFitness = car.fitness;
                }
            }
            double secondFitness = -1F;
            IndividualDescriptor second = null;
            foreach (IndividualDescriptor car in identifierPopulation)
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

        protected (IndividualDescriptor, IndividualDescriptor) PickFittestTwoInSpecies(Species species)
        {
            IIndividual first = null;
            double firstFitness = -1F;
            foreach (IIndividual car in species.GetIndividuals())
            {
                if (car.ProvideFitness() > firstFitness)
                {
                    first = car;
                    firstFitness = car.ProvideFitness();
                }
            }
            double secondFitness = -1F;
            IIndividual second = null;
            foreach (IIndividual car in species.GetIndividuals())
            {
                if (car.ProvideFitness() > secondFitness)
                {
                    if (populationNumber > 1)
                    {
                        if (car != first)
                        {
                            second = car;
                            secondFitness = car.ProvideFitness();
                        }
                    }
                    else
                    {
                        second = car;
                        secondFitness = car.ProvideFitness();
                    }
                }
            }
            return (new IndividualDescriptor(first.ProvideNeuralNet().GetGenotype(), first.ProvideFitness(), first.ProvidePickProbability()),
                    new IndividualDescriptor(second.ProvideNeuralNet().GetGenotype(), second.ProvideFitness(), second.ProvidePickProbability()));
        }
    }
}