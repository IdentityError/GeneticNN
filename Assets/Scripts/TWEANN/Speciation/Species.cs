using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    [System.Serializable]
    public class Species
    {
        [SerializeField] private int individualCount;
        private int expectedIndividualCount;
        private List<IIndividual> individuals;
        private IIndividual representative = null;
        private bool initializated;

        private float mutationRate;
        private float crossoverRate;

        public Species(float mutationRate, float crossoverRate) : this()
        {
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
        }

        public Species()
        {
            individuals = new List<IIndividual>();
            individualCount = 0;
            initializated = false;
        }

        public bool Belongs(IIndividual genotype, float sharingThreshold)
        {
            if (individualCount == 0)
            {
                return true;
            }
            if (!initializated)
            {
                return representative.ProvideNeuralNet().GetGenotype().GetTopologicalDistance(genotype.ProvideNeuralNet().GetGenotype()) < sharingThreshold;
            }
            else
            {
                return representative.ProvideNeuralNet().GetGenotype().GetTopologicalDistance(genotype.ProvideNeuralNet().GetGenotype()) < sharingThreshold && individualCount < expectedIndividualCount;
            }
        }

        public void AddToSpecies(IIndividual genotype)
        {
            if (representative == null)
            {
                representative = genotype;
            }
            individuals.Add(genotype);
            individualCount++;
        }

        public int GetIndividualCount()
        {
            return individualCount;
        }

        public List<IIndividual> GetIndividuals()
        {
            return individuals;
        }

        public void SetNewExpectedIndividualCount(int expectedIndividualCount)
        {
            this.expectedIndividualCount = expectedIndividualCount;
            initializated = true;
        }

        public double GetAdjustedFitnessSum()
        {
            double sum = 0;
            foreach (IIndividual individual in individuals)
            {
                sum += individual.ProvideAdjustedFitness();
            }
            return sum;
        }

        public double GetRawFitnessSum()
        {
            double sum = 0;
            foreach (IIndividual individual in individuals)
            {
                sum += individual.ProvideRawFitness();
            }
            return sum;
        }

        public void Reset()
        {
            individualCount = 0;
            individuals.Clear();
        }

        public int GetExpectedIndividualCount()
        {
            return expectedIndividualCount;
        }

        public IIndividual GetChamp()
        {
            double maxFitness = 0;
            IIndividual best = null;
            foreach (IIndividual individual in individuals)
            {
                if (maxFitness <= individual.ProvideAdjustedFitness())
                {
                    maxFitness = individual.ProvideAdjustedFitness();
                    best = individual;
                }
            }
            return best;
        }

        public void SetMutationRate(float mutationRate)
        {
            this.mutationRate = mutationRate;
        }

        public float GetMutationRate()
        {
            return mutationRate;
        }

        public void SetCrossoverRate(float crossoverRate)
        {
            this.crossoverRate = crossoverRate;
        }

        public float GetCrossoverRate()
        {
            return crossoverRate;
        }
    }
}