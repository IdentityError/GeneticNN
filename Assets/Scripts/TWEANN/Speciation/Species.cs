using System.Collections.Generic;

namespace Assets.Scripts.TWEANN
{
    public class Species
    {
        private int individualCount;
        private int expectedIndividualCount;
        private List<IIndividual> individuals;
        private IIndividual representative = null;
        private bool initializated;

        public Species()
        {
            individuals = new List<IIndividual>();
            individualCount = 0;
            initializated = false;
        }

        public bool Belongs(IIndividual genotype, float sharingThreshold)
        {
            if (individualCount == 0)
            {
                return true;
            }
            if (!initializated)
            {
                return representative.ProvideNeuralNet().GetGenotype().GetTopologicalDistance(genotype.ProvideNeuralNet().GetGenotype()) < sharingThreshold;
            }
            else
            {
                return representative.ProvideNeuralNet().GetGenotype().GetTopologicalDistance(genotype.ProvideNeuralNet().GetGenotype()) < sharingThreshold && individualCount < expectedIndividualCount;
            }
        }

        public void AddToSpecies(IIndividual genotype)
        {
            if (representative == null)
            {
                representative = genotype;
            }
            individuals.Add(genotype);
            individualCount++;
        }

        public int GetIndividualCount()
        {
            return individualCount;
        }

        public List<IIndividual> GetIndividuals()
        {
            return individuals;
        }

        public void SetNewExpectedIndividualCount(int expectedIndividualCount)
        {
            this.expectedIndividualCount = expectedIndividualCount;
            initializated = true;
        }

        public double GetFitnessSum()
        {
            double sum = 0;
            foreach (IIndividual individual in individuals)
            {
                sum += individual.ProvideFitness();
            }
            return sum;
        }

        public void Reset()
        {
            individualCount = 0;
            individuals.Clear();
        }

        public int GetExpectedIndividualCount()
        {
            return expectedIndividualCount;
        }

        public IIndividual GetChamp()
        {
            double maxFitness = 0;
            IIndividual best = null;
            foreach (IIndividual individual in individuals)
            {
                if (maxFitness < individual.ProvideFitness())
                {
                    maxFitness = individual.ProvideFitness();
                    best = individual;
                }
            }
            return best;
        }
    }
}