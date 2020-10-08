using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.MachineLearning.TWEANN.DescriptorsWrapper;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    [Serializable]
    public class Specie
    {
        [SerializeField] private int individualCount;
        public int atRiskGenerations;
        public RatesDescriptor rates;

        private Genotype representative;
        private int expectedOffspringsCount;
        [HideInInspector] public List<IOrganism> individuals;
        [HideInInspector] public CrossoverOperatorsWrapper operatorsWrapper = null;
        [HideInInspector] public List<Tuple<CrossoverOperationDescriptor, IOrganism>> lastGenDescriptor = new List<Tuple<CrossoverOperationDescriptor, IOrganism>>();

        public Specie(CrossoverOperatorsWrapper wrapper)
        {
            this.operatorsWrapper = wrapper;
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
            lastGenDescriptor.Clear();
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