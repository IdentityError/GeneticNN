using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    [System.Serializable]
    public class Species
    {
        [SerializeField] private int individualCount;
        private int expectedOffspringsCount;
        private List<IOrganism> individuals;

        public Species()
        {
            individuals = new List<IOrganism>();
            individualCount = 0;
        }

        public bool Belongs(IOrganism genotype, float sharingThreshold)
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

        public void AddToSpecies(IOrganism genotype)
        {
            individuals.Add(genotype);
            individualCount++;
        }

        public int GetIndividualCount()
        {
            return individualCount;
        }

        public List<IOrganism> GetIndividuals()
        {
            return individuals;
        }

        public double GetAdjustedFitnessSum()
        {
            double sum = 0;
            foreach (IOrganism individual in individuals)
            {
                sum += individual.ProvideAdjustedFitness();
            }
            return sum;
        }

        public double GetRawFitnessSum()
        {
            double sum = 0;
            foreach (IOrganism individual in individuals)
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

        public IOrganism GetChamp()
        {
            double maxFitness = 0;
            IOrganism best = null;
            foreach (IOrganism individual in individuals)
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