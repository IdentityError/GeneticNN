using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
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
        public void Speciate(IIndividual[] population)
        {
            Reset();
            int lenght = population.Length;
            for (int i = 0; i < lenght; i++)
            {
                AddToSpeciesOrCreate(population[i]);
            }
            //Debug.Log("Speciated: " + ToString());

            //foreach (IIndividual individual in population)
            //{
            //    UnityEngine.Debug.Log(((MonoBehaviour)individual).name + ", fitness adj: " + individual.ProvideFitness());
            //}
        }

        private void AddToSpeciesOrCreate(IIndividual individual)
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
        ///   Normalize the fitness within the species, adjust the species breeding parameters and set the expected species individual count
        /// </summary>
        public void AdjustSpecies()
        {
            foreach (Species species in speciesList)
            {
                foreach (IIndividual individual in species.GetIndividuals())
                {
                    individual.AdjustFitness(individual.ProvideRawFitness() / species.GetIndividualCount());
                }
            }
            double sum = 0;
            foreach (Species species in speciesList)
            {
                sum += species.GetAdjustedFitnessSum();
            }
            double averageFitness = sum / GetTotalIndividualNumber();
            foreach (Species species in speciesList)
            {
                species.SetExpectedOffspringsCount(Mathf.RoundToInt((float)(species.GetAdjustedFitnessSum() / averageFitness)));
                //Debug.Log("Species sum: " + species.GetAdjustedFitnessSum() + ", adj Sum: " + sum + ", avgFitness: " + averageFitness + ", Going form " + species.GetIndividualCount() + " to " + species.GetExpectedIndividualCount());
            }
            Purge();
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

        public float GetSharingTreshold()
        {
            return speciesSharingThreshold;
        }

        public List<Species> GetSpeciesList()
        {
            return speciesList;
        }

        public IIndividual GetCurrentFittest()
        {
            double max = 0;
            IIndividual fittest = null;
            foreach (Species species in speciesList)
            {
                IIndividual current = species.GetChamp();
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
        ///   Remove all the extinct species
        /// </summary>
        private void Purge()
        {
            speciesList.RemoveAll((current) => current.GetExpectedOffpringsCount() <= 0);
        }

        public override string ToString()
        {
            return "Sharing threshold: " + speciesSharingThreshold + ", Number of species: " + speciesList.Count + ", Tot: " + GetExpectedIndividualNumber();
        }
    }
}