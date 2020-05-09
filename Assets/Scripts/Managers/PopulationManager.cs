using Assets.Scripts.CustomBehaviour;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Providers;
using UnityEngine;

namespace Assets.Scripts.Managers
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
        [SerializeField] private TrainerProvider trainerProvider;

        private IIndividual[] population;
        private int generationCount = 0;
        private int currentSimulating;
        private UIManager uiManager;

        private void Start()
        {
            if (!trainerProvider.ProvideTrainer().GetPredefinedTopology().IsValid())
            {
                throw new System.Exception("Predefined Topology wrongly set!");
            }

            uiManager = FindObjectOfType<UIManager>();
            uiManager?.DrawNetUI(trainerProvider.ProvideTrainer().GetPredefinedTopology());

            population = new IIndividual[populationNumber];
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
            DNA[] newDNAPopulation = trainerProvider.ProvideTrainer().Train(BuildDNAPopulation());
            for (int i = 0; i < populationNumber; i++)
            {
                if (population[i] != null)
                {
                    Destroy(((MonoBehaviour)population[i]).gameObject);
                }
            }

            for (int i = 0; i < populationNumber; i++)
            {
                population[i] = InstantiateIndividual(newDNAPopulation[i], i.ToString());
            }
            generationCount++;
        }

        private void InitializeAncestors()
        {
            for (int i = 0; i < populationNumber; i++)
            {
                population[i] = InstantiateIndividual(new DNA(trainerProvider.ProvideTrainer().GetPredefinedTopology()), i.ToString());
            }
            generationCount++;
            currentSimulating = populationNumber;
        }

        private DNA[] BuildDNAPopulation()
        {
            DNA[] dnaPopulation = new DNA[populationNumber];
            for (int i = 0; i < populationNumber; i++)
            {
                dnaPopulation[i] = population[i].ProvideDNA();
            }
            return dnaPopulation;
        }

        private IIndividual InstantiateIndividual(DNA dna, string name)
        {
            GameObject obj = Instantiate(individualPrefab, track.GetStartPoint().localPosition, track.GetStartPoint().rotation);
            obj.name = name;
            IIndividual individual = obj.GetComponent<IIndividual>();
            if (individual == null)
            {
                throw new System.Exception("The individual prefab is not implementing the IIndividual interface");
            }
            individual.SetPopulationManager(this);
            individual.SetDNA(dna);
            return individual;
        }

        private float CalculateIndividualFitness(SimulationStats stats)
        {
            //TODO implement
            return Random.Range(2F, 20F);
        }

        public void IndividualEndedSimulation(IIndividual subject)
        {
            subject.ProvideDNA().fitness = CalculateIndividualFitness(subject.ProvideStats());
            Debug.Log("provided:" + subject.ProvideDNA().fitness);
            currentSimulating--;
            if (currentSimulating <= 0)
            {
                SimulationEnded();
            }
        }

        private void SimulationEnded()
        {
            AdvanceGeneration();
        }

        public Track GetTrack()
        {
            return track;
        }
    }
}