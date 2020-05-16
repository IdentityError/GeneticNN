using System.Collections.Generic;

namespace Assets.Scripts.TWEANN
{
    public class Biocenosis
    {
        private List<Species> speciesList;

        public Biocenosis()
        {
            speciesList = new List<Species>();
        }

        public Biocenosis(List<Species> speciesList)
        {
            this.speciesList = speciesList;
        }

        public void AddToSpeciesOrCreate(IIndividual individual)
        {
            foreach (Species species in speciesList)
            {
                if (species.Belongs(individual))
                {
                    species.AddToSpecies(individual);
                    return;
                }
            }
            Species newSpecies = new Species();
            newSpecies.AddToSpecies(individual);
            AddSpecies(newSpecies);
        }

        public void AddSpecies(Species species)
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
    }
}