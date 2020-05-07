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

        private void Start()
        {
            population = new IIndividual[populationNumber];
            InitializeAncestors();
        }

        private void Update()
        {
            Time.timeScale = this.timeScale;
        }

        public void AdvanceGeneration()
        {
            DNA[] newDNAPopulation = trainerProvider.ProvideTrainer().Train(BuildDNAPopulation());
            EvolveGeneration(newDNAPopulation);
            generationCount++;
        }

        private void EvolveGeneration(DNA[] newDNAPopulation)
        {
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
        }

        private void InitializeAncestors()
        {
            for (int i = 0; i < populationNumber; i++)
            {
                population[i] = InstantiateIndividual(new DNA(trainerProvider.ProvideTrainer().predefinedTopology), i.ToString());
            }
            generationCount++;
        }

        public int GetGenerationCount()
        {
            return generationCount;
        }

        private DNA[] BuildDNAPopulation()
        {
            DNA[] dnaPopulation = new DNA[populationNumber];
            for (int i = 0; i < populationNumber; i++)
            {
                dnaPopulation[i] = population[i].GetDNA();
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

        private void CalculatePopulationFitness()
        {
        }
    }
}