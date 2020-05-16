using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.TWEANN
{
    public class Species
    {
        private int individualCount;
        private List<IIndividual> individuals;

        public Species()
        {
            individuals = new List<IIndividual>();
            individualCount = 0;
        }

        public bool Belongs(IIndividual genotype)
        {
            if (individualCount == 0)
            {
                return true;
            }
            //TODO set threshold
            return individuals.ElementAt(UnityEngine.Random.Range(0, individuals.Count)).ProvideNeuralNet().GetGenotype().GetTopologicalDistance(genotype.ProvideNeuralNet().GetGenotype()) < 10;
        }

        public void AddToSpecies(IIndividual genotype)
        {
            if (Belongs(genotype))
            {
                individuals.Add(genotype);
                individualCount++;
            }
        }

        public int GetIndividualCount()
        {
            return individualCount;
        }

        public List<IIndividual> GetIndividuals()
        {
            return individuals;
        }
    }
}