using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    [System.Serializable]
    public class Species
    {
        [SerializeField] private int individualCount;
        private int expectedOffspringsCount;
        [HideInInspector] public List<IOrganism> individuals;
        private Genotype representative;
        public int atRiskGenerations;

        public Species()
        {
            individuals = new List<IOrganism>();
            individualCount = 0;
            atRiskGenerations = 0;
        }

        public bool Belongs(IOrganism genotype, float sharingThreshold)
        {
            if (representative == null)
            {
                representative = genotype.ProvideNeuralNet().GetGenotype();
                return true;
            }
            else
            {
                return representative.GetTopologicalDistance(genotype.ProvideNeuralNet().GetGenotype()) < sharingThreshold;
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
            double maxFitness = double.MinValue;
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