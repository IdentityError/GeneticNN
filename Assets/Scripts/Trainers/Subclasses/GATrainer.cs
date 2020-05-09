using Assets.Scripts.Stores;
using UnityEngine;

namespace Assets.Scripts.Trainers
{
    [System.Serializable]
    public class GATrainer : Trainer
    {
        [Header("Paradigms")]
        [Tooltip("Defines the NeuralNet mutation paradigm:" +
        "\n1. WEIGHTS: only weights will be affected" +
        "\n2. TOPOLOGY: only the topology will be affected" +
        "\n3. HYBRID: both topology and weights will be affected")]
        [SerializeField] private Paradigms.MutationParadigm mutationParadigm;
        [Tooltip("Defines the weights fill paradigm (Crossover and Mutation):" +
        "\n1. COPY: weights will be copied from another existing neuron" +
        "\n2. RANDOM: weights will be initialized as random" +
        "\n3. HYBRID: weights will be a linear combination of the weights of another neuron and random ones")]
        [SerializeField] private Paradigms.MissingWeightsFillParadigm weightsFillParadigm;
        [Space(5)]
        [Header("Parameters")]
        [Range(0, 1)]
        [SerializeField] private float mutationRate;

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
            newDnaPopulation.CopyTo(dnaPopulation, 0);
        }

        private (DNA, DNA) Selection()
        {
            NormalizePopulationPickProbability();
            //Pick first parent and temporarily set its fitness to 0 and renormalize the probabilities so it won't be picked as second parent
            DNA first = PickRandom();
            float firstFitness = first.fitness;
            first.fitness = 0;
            NormalizePopulationPickProbability();

            //Picks second parent and reset the first parent fitness so it can be picked on the next iteration
            DNA second = PickRandom();
            first.fitness = firstFitness;

            return (first, second);
        }

        private DNA Crossover(DNA parent, DNA parent1)
        {
            return parent.Crossover(parent1, weightsFillParadigm, 2);
        }

        private void Mutation(DNA childDNA)
        {
            childDNA.Mutate(mutationParadigm, weightsFillParadigm, mutationRate);
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

        private void NormalizePopulationPickProbability()
        {
            float fitnessSum = 0F;
            foreach (DNA individual in dnaPopulation)
            {
                fitnessSum += individual.fitness;
            }
            foreach (DNA individual in dnaPopulation)
            {
                individual.pickProbability = individual.fitness / fitnessSum;
            }
        }
    }
}