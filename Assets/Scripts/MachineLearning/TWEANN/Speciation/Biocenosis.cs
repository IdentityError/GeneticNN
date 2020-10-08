using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    /// <summary>
    ///   Represents the Biocenosis of the population, that is the set of all species present in the population
    /// </summary>
    [System.Serializable]
    public class Biocenosis
    {
        [SerializeField] private List<Specie> speciesList;
        private CrossoverOperatorsWrapper crossoverOperatorsWrapper;
        private float speciesSharingThreshold;

        public Biocenosis(float speciesSharingThreshold, CrossoverOperatorsWrapper wrapper)
        {
            this.crossoverOperatorsWrapper = wrapper;
            speciesList = new List<Specie>();
            this.speciesSharingThreshold = speciesSharingThreshold;
        }

        /// <summary>
        ///   <b> Speciates the population </b><br> </br>
        ///   1. Split oranism into species based on the Biocenosis sharing threshold <br> </br>
        ///   2. Calculate the next generation expected number per species and total number of individuals
        /// </summary>
        /// <param name="population"> </param>
        public void Speciate(Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism>[] population)
        {
            Reset();
            int lenght = population.Length;
            for (int i = 0; i < lenght; i++)
            {
                AddToSpeciesOrCreate(population[i]);
            }
            Purge(s => s.GetIndividualCount() <= 0);
            //Debug.Log("Speciated: " + ToString());

            //foreach (IIndividual individual in population)
            //{
            //    UnityEngine.Debug.Log(((MonoBehaviour)individual).name + ", fitness adj: " + individual.ProvideFitness());
            //}
        }

        private void AddToSpeciesOrCreate(Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism> individual)
        {
            foreach (Specie species in speciesList)
            {
                if (species.Belongs(individual.Item2, speciesSharingThreshold))
                {
                    species.AddToSpecies(individual.Item2);
                    species.lastGenDescriptor.Add(individual);
                    return;
                }
            }
            Specie newSpecies = new Specie(crossoverOperatorsWrapper);
            newSpecies.AddToSpecies(individual.Item2);
            newSpecies.lastGenDescriptor.Add(individual);
            AddSpecies(newSpecies);
        }

        private void AddSpecies(Specie species)
        {
            if (!speciesList.Contains(species))
            {
                speciesList.Add(species);
            }
        }

        /// <summary>
        ///   Normalize the fitness within the species, adjust the species breeding parameters and set the expected species individual
        ///   count, extinct eventual bad performing species
        /// </summary>
        public void AdjustSpecies()
        {
            int popSize = GetTotalIndividualNumber();
            int c = speciesList.Count;
            foreach (Specie species1 in speciesList)
            {
                if (species1.GetIndividualCount() <= popSize * 0.1)
                {
                    species1.atRiskGenerations++;
                }
                else
                {
                    species1.atRiskGenerations = 0;
                }
            }

            double sum = 0;
            foreach (Specie species in speciesList)
            {
                foreach (IOrganism individual in species.GetIndividuals())
                {
                    individual.AdjustFitness((float)individual.ProvideRawFitness() / species.GetIndividualCount());
                }
                sum += species.GetAdjustedFitnessSum();
            }
            Purge(s => s.atRiskGenerations >= 3);

            int count = 0;
            foreach (Specie species in speciesList)
            {
                int val = Mathf.RoundToInt(Mathf.Floor((float)(species.GetAdjustedFitnessSum() * popSize / sum)));
                species.SetExpectedOffspringsCount(val);
                count += val;
            }
            int rest = popSize - count;
            Specie spec = GetFittestSpecies();
            spec.SetExpectedOffspringsCount(spec.GetExpectedOffpringsCount() + rest);
        }

        public Specie GetFittestSpecies()
        {
            Specie fittest = null;
            double val = 0;
            foreach (Specie current in speciesList)
            {
                double sum = current.GetAdjustedFitnessSum();
                if (sum > val)
                {
                    fittest = current;
                    val = sum;
                }
            }
            return fittest;
        }

        public int GetExpectedIndividualNumber()
        {
            int count = 0;
            foreach (Specie species in speciesList)
            {
                count += species.GetExpectedOffpringsCount();
            }
            return count;
        }

        public int GetTotalIndividualNumber()
        {
            int count = 0;
            foreach (Specie species in speciesList)
            {
                count += species.GetIndividualCount();
            }
            return count;
        }

        public List<Specie> GetSpeciesList()
        {
            return speciesList;
        }

        public IOrganism GetCurrentFittest()
        {
            double max = -1;
            IOrganism fittest = null;
            foreach (Specie species in speciesList)
            {
                IOrganism current = species.GetChamp();
                if (current != null && current.ProvideRawFitness() > max)
                {
                    fittest = current;
                    max = fittest.ProvideRawFitness();
                }
            }
            return fittest;
        }

        public double GetAverageFitness()
        {
            double avg = 0F;
            foreach (Specie species in speciesList)
            {
                avg += species.GetRawFitnessSum() / species.GetIndividualCount();
            }
            avg /= speciesList.Count;
            return avg;
        }

        /// <summary>
        ///   Clear all the species in the Biocenosis
        /// </summary>
        private void Reset()
        {
            foreach (Specie species in speciesList)
            {
                species.Reset();
            }
        }

        /// <summary>
        ///   Remove all the species matching the predicate
        /// </summary>
        private void Purge(Predicate<Specie> match)
        {
            speciesList.RemoveAll(match);
        }

        public override string ToString()
        {
            return "Sharing threshold: " + speciesSharingThreshold + ", Number of species: " + speciesList.Count + ", Tot: " + GetExpectedIndividualNumber();
        }
    }
}