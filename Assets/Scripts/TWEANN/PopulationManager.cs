using Assets.Scripts.CustomBehaviour;
using Assets.Scripts.NeuralNet;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    public class PopulationManager : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private int populationNumber;
        [SerializeField] private float timeScale = 1F;
        [Header("References")]
        [SerializeField] private GameObject individualPrefab;
        [SerializeField] private Track track;
        [Header("Training")]
        [SerializeField] private PopulationTrainerProvider trainerProvider;

        private ISimulatingIndividual[] population;
        private int generationCount = 0;
        private int currentSimulating;
        private UIManager uiManager;

        private Genotype fittestGenotype;
        private double generationMaxFitness = 0;

        private Biocenosis biocenosis;
        private List<Species> topologicalSpecies;

        private void Start()
        {
            uiManager = FindObjectOfType<UIManager>();
            //uiManager?.DrawNetUI(trainerProvider.ProvideTrainer().GetPredefinedTopologyDescriptor());

            population = new ISimulatingIndividual[populationNumber];
            biocenosis = new Biocenosis();
            InitializeAncestors();
        }

        private void Update()
        {
            Time.timeScale = this.timeScale;
        }

        public int GetGenerationCount()
        {
            return generationCount;
        }

        private void AdvanceGeneration()
        {
            trainerProvider.ProvideTrainer().Train(population, ref biocenosis);

            //foreach (IIndividual ind in population)
            //{
            //    Debug.Log(ind.ProvideNeuralNet().GetGenotype().ToString());
            //}
            ResetPopulation();
            generationCount++;
            currentSimulating = populationNumber;
            uiManager.UpdateGenerationCount(generationCount);
            //uiManager?.DrawNetUI(population[0].ProvideNeuralNet().topology);
        }

        private void InitializeAncestors()
        {
            for (int i = 0; i < populationNumber; i++)
            {
                population[i] = InstantiateIndividual(new NeuralNetwork(new Genotype(trainerProvider.ProvideTrainer().GetPredefinedTopologyDescriptor())), i.ToString());
                biocenosis.AddToSpeciesOrCreate(population[i]);
            }
            generationCount++;
            currentSimulating = populationNumber;
            GlobalParams.InitializeGlobalInnovationNumber(trainerProvider.ProvideTrainer().GetPredefinedTopologyDescriptor());
        }

        private ISimulatingIndividual InstantiateIndividual(NeuralNetwork neuralNet, string name)
        {
            GameObject obj = Instantiate(individualPrefab, track.GetStartPoint().localPosition, track.GetStartPoint().rotation);
            obj.name = name;
            ISimulatingIndividual individual = obj.GetComponent<ISimulatingIndividual>();
            if (individual == null)
            {
                throw new System.Exception("The individual prefab is not implementing the ISimulatingIndividual interface");
            }
            individual.SetPopulationManager(this);
            individual.SetNeuralNet(neuralNet);
            return individual;
        }

        private float FitnessFunction(SimulationStats stats)
        {
            return (2F * stats.averageThrottle + stats.distance) * stats.time;
        }

        public void IndividualEndedSimulation(ISimulatingIndividual subject)
        {
            //subject.SetFitness(FitnessFunction(subject.ProvideSimulationStats()));
            currentSimulating--;
            if (currentSimulating <= 0)
            {
                SimulationEnded();
            }
        }

        public void ForceSimulationEnd()
        {
            foreach (ISimulatingIndividual individual in population)
            {
                if (individual.IsSimulating())
                {
                    individual.StopSimulating();
                    IndividualEndedSimulation(individual);
                }
            }
        }

        private void SimulationEnded()
        {
            generationMaxFitness = 0;
            foreach (IIndividual individual in population)
            {
                if (individual.ProvideFitness() > generationMaxFitness)
                {
                    fittestGenotype = individual.ProvideNeuralNet().GetGenotype();
                    generationMaxFitness = individual.ProvideFitness();
                }
            }
            Debug.Log("Fitness: " + generationMaxFitness + "\n" + fittestGenotype.ToString());
            AdvanceGeneration();
        }

        public Track GetTrack()
        {
            return track;
        }

        private void ResetPopulation()
        {
            for (int i = 0; i < populationNumber; i++)
            {
                population[i].SetPickProbability(0);
                population[i].SetSimulationStats(new SimulationStats(0, 0, 0, track.GetId()));
                population[i].ResetStatus();
            }
        }
    }
}