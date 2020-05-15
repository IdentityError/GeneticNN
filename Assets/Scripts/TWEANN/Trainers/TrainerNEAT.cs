using Assets.Scripts.NeuralNet;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    [System.Serializable]
    public class TrainerNEAT : PopulationTrainer
    {
        //[Header("Paradigms")]
        //[Tooltip("Defines the NeuralNet mutation paradigm:" +
        //"\n1. WEIGHTS: only weights will be affected" +
        //"\n2. TOPOLOGY: only the topology will be affected" +
        //"\n3. HYBRID: both topology and weights will be affected")]
        //[SerializeField] private Paradigms.MutationParadigm mutationParadigm;
        //[Tooltip("Defines the weights fill paradigm (Crossover and Mutation):" +
        //"\n1. COPY: weights will be copied from another existing neuron" +
        //"\n2. RANDOM: weights will be initialized as random" +
        //"\n3. HYBRID: weights will be a linear combination of the weights of another neuron and random ones")]
        //[SerializeField] private Paradigms.MissingWeightsFillParadigm weightsFillParadigm;
        [Space(5)]
        [Header("Parameters")]
        [Range(0, 1)]
        [SerializeField] private float mutationRate;

        public override void Train(IIndividual[] population)
        {
            base.Train(population);
            for (int i = 0; i < populationNumber; i++)
            {
                (IndividualIdentifier, IndividualIdentifier) parents = PickFittestTwo();

                //Debug.Log("Parent1: " + parents.Item1.genotype.ToString() + ", parent2: " + parents.Item2.genotype.ToString());
                Genotype childGen = Crossover(parents.Item1, parents.Item2);
                //Debug.Log("child before mutation: " + childGen.ToString());
                childGen.Mutate(mutationRate);
                population[i].SetNeuralNet(new NeuralNetwork(childGen));
                //Debug.Log("CHILD: " + population[i].ProvideNeuralNet().GetGenotype().ToString());
            }
        }

        private (IndividualIdentifier, IndividualIdentifier) Selection()
        {
            NormalizePopulationPickProbability();
            //Pick first parent and temporarily set its fitness to 0 and renormalize the probabilities so it won't be picked as second parent
            IndividualIdentifier first = PickRandom();
            double firstFitness = first.fitness;
            first.fitness = 0;
            NormalizePopulationPickProbability();

            //Picks second parent and reset the first parent fitness so it can be picked on the next iteration
            IndividualIdentifier second = PickRandom();
            first.fitness = firstFitness;
            return (first, second);
        }

        private Genotype Crossover(IndividualIdentifier parent, IndividualIdentifier parent1)
        {
            IndividualIdentifier fittest = parent.fitness > parent1.fitness ? parent : parent1;
            IndividualIdentifier partner = fittest.Equals(parent) ? parent1 : parent;
            Genotype newGen = fittest.genotype.Crossover(partner.genotype);
            return newGen;
        }
    }
}