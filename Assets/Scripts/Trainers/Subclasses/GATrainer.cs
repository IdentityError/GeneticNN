using Assets.Scripts.Stores;
using UnityEngine;

namespace Assets.Scripts.Trainers
{
    [System.Serializable]
    public class GATrainer : Trainer
    {
        [SerializeField] private Paradigms.MutationParadigm mutationParadigm;
        [Space(5)]
        [Header("Parameters")]
        [Range(0, 1)]
        [SerializeField] private float mutationPercentage;

        protected override void Train()
        {
            DNA[] newDnaPopulation = new DNA[populationNumber];
            for (int i = 0; i < populationNumber; i++)
            {
                (DNA, DNA) parents = Selection();
                DNA childDNA = Crossover(parents.Item1, parents.Item2);
                Mutation(childDNA);
                newDnaPopulation[i] = childDNA;
            }
            dnaPopulation = newDnaPopulation;
        }

        private (DNA, DNA) Selection()
        {
            //TODO dont pick already selected parents?
            return (PickRandom(), PickRandom());
        }

        private DNA Crossover(DNA parent, DNA parent1)
        {
            return parent.Crossover(parent1, 2);
        }

        private void Mutation(DNA childDNA)
        {
            childDNA.Mutate(mutationParadigm, mutationPercentage);
        }

        private DNA PickRandom()
        {
            float seed = UnityEngine.Random.Range(0F, 1F);
            int index = -1;
            while (seed >= 0)
            {
                seed -= dnaPopulation[++index].pickProbability;
            }
            return dnaPopulation[index];
        }
    }
}