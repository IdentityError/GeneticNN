using Assets.Scripts.Interfaces;
using Assets.Scripts.NeuralNet;
using Assets.Scripts.Stores;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    [System.Serializable]
    public class TrainerNEAT : PopulationTrainer
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

        public override void Train(IIndividual[] population)
        {
            base.Train(population);
            for (int i = 0; i < populationNumber; i++)
            {
                (IIndividual, IIndividual) parents = Selection();
                //Debug.Log("Parent1: " + ((MonoBehaviour)parents.Item1).name + ", parent2: " + ((MonoBehaviour)parents.Item2).name);
                Crossover(parents.Item1, parents.Item2, population[i]);
                //Mutation(population[i]);
            }
        }

        private (IIndividual, IIndividual) Selection()
        {
            NormalizePopulationPickProbability();
            //Pick first parent and temporarily set its fitness to 0 and renormalize the probabilities so it won't be picked as second parent
            IIndividual first = PickRandom();
            double firstFitness = first.ProvideFitness();
            first.SetFitness(0);
            NormalizePopulationPickProbability();

            //Picks second parent and reset the first parent fitness so it can be picked on the next iteration
            IIndividual second = PickRandom();
            first.SetFitness(firstFitness);

            return (first, second);
        }

        private void Crossover(IIndividual parent, IIndividual parent1, IIndividual child)
        {
            IIndividual fittest = parent.ProvideFitness() > parent1.ProvideFitness() ? parent : parent1;
            IIndividual partner = fittest.Equals(parent) ? parent1 : parent;
            Debug.Log("Fittest: " + fittest.ProvideNeuralNet().GetGenotype().ToString() + ", partner: " + partner.ProvideNeuralNet().GetGenotype().ToString());
            Genotype newGen = fittest.ProvideNeuralNet().GetGenotype().Crossover(partner.ProvideNeuralNet().GetGenotype());
            Debug.Log("Child: " + newGen.ToString());
            child.SetNeuralNet(new NeuralNetwork(newGen));
        }

        private void Mutation(IIndividual individual)
        {
            Mutation mutation = individual.ProvideNeuralNet().GetGenotype().Mutate(mutationRate, this);
            if (mutation != null)
            {
                AddGenerationMutation(mutation);
            }
        }

        private IIndividual PickRandom()
        {
            double seed = UnityEngine.Random.Range(0F, 1F);
            int index = -1;
            while (seed >= 0)
            {
                seed -= newPopulation[++index].ProvidePickProbability();
            }
            return newPopulation[index];
        }

        private void NormalizePopulationPickProbability()
        {
            double fitnessSum = 0F;
            foreach (IIndividual individual in newPopulation)
            {
                fitnessSum += individual.ProvideFitness();
            }
            foreach (IIndividual individual in newPopulation)
            {
                individual.SetPickProbability(individual.ProvideFitness() / fitnessSum);
            }
        }
    }
}