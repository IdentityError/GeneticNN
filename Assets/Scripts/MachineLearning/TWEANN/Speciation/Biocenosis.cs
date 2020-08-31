using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MachineLearning.TWEANN
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
        public void Speciate(IOrganism[] population)
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
        ///   Normalize the fitness within the species, adjust the species breeding parameters and set the expected species individual count
        /// </summary>
        public void AdjustSpecies()
        {
            foreach (Species species in speciesList)
            {
                foreach (IOrganism individual in species.GetIndividuals())
                {
                    individual.AdjustFitness(Mathf.Round((float)individual.ProvideRawFitness() / species.GetIndividualCount()));
                }
            }
            double sum = 0;
            foreach (Species species in speciesList)
            {
                sum += species.GetAdjustedFitnessSum();
            }
            Debug.Log("sum: " + sum);
            int count = 0;
            foreach (Species species in speciesList)
            {
                Debug.Log("spec sum: " + species.GetAdjustedFitnessSum());
                float val = (float)(species.GetAdjustedFitnessSum() * GetTotalIndividualNumber() / sum);
                species.SetExpectedOffspringsCount(Mathf.RoundToInt(Mathf.Floor(val)));
                count += Mathf.RoundToInt(Mathf.Floor(val));
            }
            int rest = GetTotalIndividualNumber() - count;
            Species spec = null;
            if (rest > 0)
            {
                spec = GetFittestSpecies();
            }
            else
            {
                spec = GetLeastFitSpecies();
            }
            spec.SetExpectedOffspringsCount(spec.GetExpectedOffpringsCount() + rest);
            Purge();
            int c = 0;
            foreach (Species species1 in speciesList)
            {
                for (int i = 0; i < species1.GetExpectedOffpringsCount(); i++)
                {
                    c++;
                }
            }
            Debug.Log(c);
            Debug.Log("EXPadj: " + GetExpectedIndividualNumber());
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

        public Species GetLeastFitSpecies()
        {
            Species worse = null;
            double val = double.MaxValue;
            foreach (Species current in speciesList)
            {
                double sum = current.GetAdjustedFitnessSum();
                if (sum < val)
                {
                    worse = current;
                    val = sum;
                }
            }
            return worse;
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
        ///   Remove all the extinct species
        /// </summary>
        private void Purge()
        {
            speciesList.RemoveAll((current) => current.GetIndividuals().Count <= 0 && current.GetExpectedOffpringsCount() <= 0);
        }

        public override string ToString()
        {
            return "Sharing threshold: " + speciesSharingThreshold + ", Number of species: " + speciesList.Count + ", Tot: " + GetExpectedIndividualNumber();
        }
    }
}