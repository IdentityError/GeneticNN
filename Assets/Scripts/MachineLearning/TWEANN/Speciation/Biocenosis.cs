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
        [SerializeField] private List<Species> speciesList;
        private float speciesSharingThreshold;

        public Biocenosis(float speciesSharingThreshold)
        {
            speciesList = new List<Species>();
            this.speciesSharingThreshold = speciesSharingThreshold;
        }

        /// <summary>
        ///   <b> Speciates the population </b><br> </br>
        ///   1. Split oranism into species based on the Biocenosis sharing threshold <br> </br>
        ///   2. Calculate the next generation expected number per species and total number of individuals
        /// </summary>
        /// <param name="population"> </param>
        public void Speciate(IOrganism[] population)
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

        private void AddToSpeciesOrCreate(IOrganism individual)
        {
            foreach (Species species in speciesList)
            {
                if (species.Belongs(individual, speciesSharingThreshold))
                {
                    species.AddToSpecies(individual);
                    return;
                }
            }
            Species newSpecies = new Species();
            newSpecies.AddToSpecies(individual);
            AddSpecies(newSpecies);
        }

        private void AddSpecies(Species species)
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
            foreach (Species species1 in speciesList)
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
            foreach (Species species in speciesList)
            {
                foreach (IOrganism individual in species.GetIndividuals())
                {
                    individual.AdjustFitness(Mathf.Round((float)individual.ProvideRawFitness() / species.GetIndividualCount()));
                }
                sum += species.GetAdjustedFitnessSum();
            }
            Purge(s => s.atRiskGenerations >= 3);

            int count = 0;
            foreach (Species species in speciesList)
            {
                int val = Mathf.RoundToInt(Mathf.Floor((float)(species.GetAdjustedFitnessSum() * popSize / sum)));
                species.SetExpectedOffspringsCount(val);
                count += val;
            }
            int rest = popSize - count;
            Species spec = GetFittestSpecies();
            spec.SetExpectedOffspringsCount(spec.GetExpectedOffpringsCount() + rest);
            Debug.Log("EXP: " + GetExpectedIndividualNumber());
        }

        public Species GetFittestSpecies()
        {
            Species fittest = null;
            double val = 0;
            foreach (Species current in speciesList)
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
            foreach (Species species in speciesList)
            {
                count += species.GetExpectedOffpringsCount();
            }
            return count;
        }

        public int GetTotalIndividualNumber()
        {
            int count = 0;
            foreach (Species species in speciesList)
            {
                count += species.GetIndividualCount();
            }
            return count;
        }

        public List<Species> GetSpeciesList()
        {
            return speciesList;
        }

        public IOrganism GetCurrentFittest()
        {
            double max = 0;
            IOrganism fittest = null;
            foreach (Species species in speciesList)
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
            foreach (Species species in speciesList)
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
            foreach (Species species in speciesList)
            {
                species.Reset();
            }
        }

        /// <summary>
        ///   Remove all the species matching the predicate
        /// </summary>
        private void Purge(Predicate<Species> match)
        {
            speciesList.RemoveAll(match);
        }

        public override string ToString()
        {
            return "Sharing threshold: " + speciesSharingThreshold + ", Number of species: " + speciesList.Count + ", Tot: " + GetExpectedIndividualNumber();
        }
    }
}