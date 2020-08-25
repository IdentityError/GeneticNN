using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    [System.Serializable]
    public class Species
    {
        [SerializeField] private int individualCount;
        private int expectedOffspringsCount;
        private List<IIndividual> individuals;

        public Species()
        {
            individuals = new List<IIndividual>();
            individualCount = 0;
        }

        public bool Belongs(IIndividual genotype, float sharingThreshold)
        {
            if (individualCount == 0)
            {
                return true;
            }
            else
            {
                return individuals[UnityEngine.Random.Range(0, individualCount)].ProvideNeuralNet().GetGenotype().GetTopologicalDistance(genotype.ProvideNeuralNet().GetGenotype()) < sharingThreshold;
            }
        }

        public void AddToSpecies(IIndividual genotype)
        {
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
            expectedOffspringsCount = 0;
            individuals.Clear();
        }

        public void SetExpectedOffspringsCount(int expectedOffspringsCount)
        {
            this.expectedOffspringsCount = expectedOffspringsCount;
        }

        public int GetExpectedOffpringsCount()
        {
            return expectedOffspringsCount;
        }

        public IIndividual GetChamp()
        {
            double maxFitness = 0;
            IIndividual best = null;
            foreach (IIndividual individual in individuals)
            {
                if (maxFitness <= individual.ProvideRawFitness())
                {
                    maxFitness = individual.ProvideRawFitness();
                    best = individual;
                }
            }
            return best;
        }

        public override string ToString()
        {
            return "Count: " + individualCount + ", Expected count: " + expectedOffspringsCount;
        }
    }
}