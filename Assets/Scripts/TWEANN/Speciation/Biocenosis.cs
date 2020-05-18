using System.Collections.Generic;

namespace Assets.Scripts.TWEANN
{
    public class Biocenosis
    {
        private List<Species> speciesList;
        private float speciesSharingThreshold;

        public Biocenosis(float speciesSharingThreshold)
        {
            speciesList = new List<Species>();
            this.speciesSharingThreshold = speciesSharingThreshold;
        }

        public Biocenosis(List<Species> speciesList, float speciesSharingThreshold) : this(speciesSharingThreshold)
        {
            this.speciesList = speciesList;
        }

        public void AddToSpeciesOrCreate(IIndividual individual)
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

        public List<Species> GetSpeciesList()
        {
            return speciesList;
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

        public int GetExpectedIndividualNumber()
        {
            int count = 0;
            foreach (Species species in speciesList)
            {
                count += species.GetExpectedIndividualCount();
            }
            return count;
        }

        private void Reset()
        {
            foreach (Species species in speciesList)
            {
                species.Reset();
            }
        }

        public void CalculateSpeciesExpectedNumber()
        {
            foreach (Species species in speciesList)
            {
                foreach (IIndividual individual in species.GetIndividuals())
                {
                    individual.SetFitness(individual.ProvideFitness() / species.GetIndividualCount());
                }
            }
            double sum = 0;
            int count = 0;
            foreach (Species species in speciesList)
            {
                foreach (IIndividual individual in species.GetIndividuals())
                {
                    count++;
                    sum += individual.ProvideFitness();
                }
            }
            double averageFitness = sum / count;
            //UnityEngine.Debug.Log("Sum: " + sum + ", count: " + count + ", avg: " + averageFitness);

            int summ = 0;
            foreach (Species species in speciesList)
            {
                species.SetNewExpectedIndividualCount((int)(species.GetFitnessSum() / averageFitness));
                //UnityEngine.Debug.Log("Set specie number: " + species.GetFitnessSum());
                summ += species.GetExpectedIndividualCount();
            }
        }

        public float GetSharingTreshold()
        {
            return speciesSharingThreshold;
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
            CalculateSpeciesExpectedNumber();
        }

        public override string ToString()
        {
            return "Sharing threshold: " + speciesSharingThreshold + "\n" + "Number of species: " + speciesList.Count;
        }
    }
}