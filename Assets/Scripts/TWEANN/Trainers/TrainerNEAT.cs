using Assets.Scripts.NeuralNet;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    [System.Serializable]
    public class TrainerNEAT : PopulationTrainer
    {
        [System.Serializable]
        public struct MutationProbabilities
        {
            [Range(0, 1)]
            public float weightChangeProb;

            [Range(0, 1)]
            public float splitLinkProb;

            [Range(0, 1)]
            public float addLinkProb;
        }

        [Header("Mutation Parameters")]
        [SerializeField] private MutationProbabilities mutation;

        public override NeuralNetwork[] Train(Biocenosis biocenosis)
        {
            GlobalParams.ResetGenerationMutations();

            int expectedIndividualCount = biocenosis.GetExpectedIndividualNumber();
            NeuralNetwork[] pop = new NeuralNetwork[expectedIndividualCount];

            int currentIndex = 0;
            foreach (Species current in biocenosis.GetSpeciesList())
            {
                for (int i = 0; i < current.GetExpectedIndividualCount(); i++)
                {
                    (IIndividual, IIndividual) parents = SelectionFromSpecies(current);
                    Debug.Log(((MonoBehaviour)parents.Item1).name + ", " + ((MonoBehaviour)parents.Item2).name + "\n" + parents.Item1.ProvideNeuralNet().GetGenotype().ToString() + ", " + parents.Item2.ProvideNeuralNet().GetGenotype().ToString());
                    Genotype childGen = Crossover(parents.Item1, parents.Item2);
                    childGen.Mutate(mutation);
                    pop[currentIndex] = new NeuralNetwork(childGen);
                    //Debug.Log("Child: " + childGen.ToString());
                    currentIndex++;
                }
            }
            return pop;
        }

        /// <summary>
        ///   Perform selection from a specified species
        /// </summary>
        /// <param name="species"> </param>
        /// <returns> </returns>
        private (IIndividual, IIndividual) SelectionFromSpecies(Species species)
        {
            if (species.GetIndividualCount() == 1)
            {
                IIndividual champ = species.GetChamp();
                return (champ, champ);
            }
            else if (species.GetIndividualCount() > 1)
            {
                //Debug.Log("Returning 2 ");
                return PickFittestTwoInSpecies(species);
            }
            else
            {
                throw new System.Exception("Unable to select parents from a species");
            }
        }

        /// <summary>
        ///   Pick the 2 fittest individuals in the species, if the species has only 1 member, it returns the member (duplicate)
        /// </summary>
        /// <param name="species"> </param>
        /// <returns> </returns>
        private (IIndividual, IIndividual) PickFittestTwoInSpecies(Species species)
        {
            IIndividual first = null;
            double firstFitness = -1F;
            foreach (IIndividual car in species.GetIndividuals())
            {
                if (car.ProvideFitness() > firstFitness)
                {
                    first = car;
                    firstFitness = car.ProvideFitness();
                }
            }
            double secondFitness = -1F;
            IIndividual second = null;
            foreach (IIndividual car in species.GetIndividuals())
            {
                if (car.ProvideFitness() > secondFitness)
                {
                    if (species.GetIndividualCount() > 1)
                    {
                        if (car != first)
                        {
                            second = car;
                            secondFitness = car.ProvideFitness();
                        }
                    }
                    else
                    {
                        second = car;
                        secondFitness = car.ProvideFitness();
                    }
                }
            }
            return (first, second);
        }

        /// <summary>
        ///   Perform the crossover between the 2 parents
        /// </summary>
        /// <param name="parent"> </param>
        /// <param name="parent1"> </param>
        /// <returns> </returns>
        private Genotype Crossover(IIndividual parent, IIndividual parent1)
        {
            Genotype newGen = parent.ProvideNeuralNet().GetGenotype().Crossover(parent1.ProvideNeuralNet().GetGenotype(), parent.ProvideFitness(), parent1.ProvideFitness());
            return newGen;
        }
    }
}