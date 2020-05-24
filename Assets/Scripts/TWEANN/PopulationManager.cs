using Assets.Scripts.CustomBehaviour;
using Assets.Scripts.NeuralNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    public class PopulationManager : MonoBehaviour
    {
        public List<ISimulatingIndividual> populationList;
        private float averageThrottle = 1;
        private Biocenosis biocenosis;
        private int currentSimulating;
        private Genotype fittestGenotype;
        private int generationCount = 0;
        private double generationMaxFitness = 0;
        [Header("References")]
        [SerializeField] private GameObject individualPrefab;
        [Header("Parameters")]
        [SerializeField] private int initialPopulationNumber;
        [SerializeField] private float sharingThreshold;
        private IEnumerator ShouldRestart_C;
        private bool simulating = false;
        private float simulationTime = 0;
        [SerializeField] private float timeScale = 1F;
        [SerializeField] private Track track;
        [Header("Training")]
        [SerializeField] private PopulationTrainerProvider trainerProvider;
        private UIManager uiManager;

        private void Start()
        {
            uiManager = FindObjectOfType<UIManager>();
            //uiManager?.DrawNetUI(trainerProvider.ProvideTrainer().GetPredefinedTopologyDescriptor());

            ShouldRestart_C = CheckAverageThrottle();
            populationList = new List<ISimulatingIndividual>();
            biocenosis = new Biocenosis(sharingThreshold);
            InitializeAncestors();
        }

        private void Update()
        {
            Time.timeScale = this.timeScale;
            //if (simulating)
            //{
            //    simulationTime += Time.deltaTime;
            //    if (simulationTime > 2.5F && averageThrottle < 0.016)
            //    {
            //        Debug.LogError("SHOULD");
            //        ForceSimulationEnd();
            //    }
            //}
        }

        /// <summary>
        ///   Initialize the first population, used only one time at the start
        /// </summary>
        private void InitializeAncestors()
        {
            for (int i = 0; i < initialPopulationNumber; i++)
            {
                populationList.Add(InstantiateIndividual(new NeuralNetwork(new Genotype(trainerProvider.ProvideTrainer().GetPredefinedTopologyDescriptor())), i.ToString()));
            }
            generationCount++;
            currentSimulating = initialPopulationNumber;
            GlobalParams.InitializeGlobalInnovationNumber(trainerProvider.ProvideTrainer().GetPredefinedTopologyDescriptor());
            //foreach (IIndividual ind in population)
            //{
            //    Debug.Log(ind.ProvideNeuralNet().GetGenotype().ToString());
            //}
            //StartCoroutine(ShouldRestart_C);
            simulating = true;
        }

        /// <summary>
        ///   Advance to the next generation with the following steps: <br> </br>
        ///   1. Speciation <br> </br>
        ///   2. Training <br> </br>
        ///   3. Instantiate new population <br> </br>
        /// </summary>
        private void AdvanceGeneration()
        {
            //! Speciation
            biocenosis.Speciate(populationList.ToArray());
            uiManager.AppendToLog("Current biocenosis: \n" + biocenosis.ToString());
            // Retrieve a new trained Network population
            NeuralNetwork[] pop = trainerProvider.ProvideTrainer().Train(biocenosis);

            // Destroy all the objects
            foreach (ISimulatingIndividual individual in populationList)
            {
                PoolManager.GetInstance().DeactivateObject(((MonoBehaviour)individual).gameObject);
            }
            populationList.Clear();

            for (int i = 0; i < pop.Length; i++)
            {
                populationList.Add(InstantiateIndividual(pop[i], i.ToString()));
                //Debug.Log(((MonoBehaviour)populationList[i]).gameObject.name + populationList[i].ProvideNeuralNet().GetGenotype().ToString());
            }

            generationCount++;
            currentSimulating = pop.Length;
            simulating = true;
            averageThrottle = 1;
            uiManager.UpdateGenerationCount(generationCount);
        }

        /// <summary>
        ///   Force the end of the simulation
        /// </summary>
        public void ForceSimulationEnd()
        {
            foreach (ISimulatingIndividual individual in populationList)
            {
                if (individual.IsSimulating())
                {
                    individual.StopSimulating();
                }
            }
            currentSimulating = 0;
            SimulationEnded();
        }

        /// <summary>
        ///   Called when the simulation has ended
        /// </summary>
        private void SimulationEnded()
        {
            simulating = false;
            simulationTime = 0;
            generationMaxFitness = 0;
            foreach (IIndividual individual in populationList)
            {
                individual.SetFitness(individual.EvaluateFitnessFunction());
            }
            AdvanceGeneration();
        }

        /// <summary>
        ///   Inform the manager that an individual has ended the simulation <br> </br><b> Important: </b> do not call this method on the
        ///   implementation of the function StopSimulating() from the interface ISimulationPerformer/ISimulationIndividual, since when
        ///   force ending simulation, the manager calls that function and it takes care of restarting the simulation
        /// </summary>
        /// <param name="subject"> </param>
        public void IndividualEndedSimulation(ISimulatingIndividual subject)
        {
            //subject.SetFitness(FitnessFunction(subject.ProvideSimulationStats()));
            currentSimulating--;
            if (currentSimulating <= 0 && simulating)
            {
                SimulationEnded();
            }
        }

        /// <summary>
        ///   Instantiate a new individual
        /// </summary>
        /// <param name="neuralNet"> </param>
        /// <param name="name"> </param>
        /// <returns> </returns>
        private ISimulatingIndividual InstantiateIndividual(NeuralNetwork neuralNet, string name)
        {
            GameObject obj = PoolManager.GetInstance().Spawn("Individual", "prefab", track.GetStartPoint().localPosition, track.GetStartPoint().rotation);
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

        /// <summary>
        ///   Get the current track of the simulation
        /// </summary>
        /// <returns> </returns>
        public Track GetTrack()
        {
            return track;
        }

        private IEnumerator CheckAverageThrottle()
        {
            //TODO fix the problem that sometimes restarts too quickly
            yield return new WaitForSeconds(1F);
            while (true)
            {
                averageThrottle = 0;
                float sum = 0;
                foreach (ISimulatingIndividual individual in populationList)
                {
                    if (individual.IsSimulating())
                    {
                        sum += individual.ProvideSimulationStats().averageThrottle;
                    }
                }

                averageThrottle = sum / initialPopulationNumber;
                //Debug.Log(averageThrottle);
                yield return new WaitForSeconds(2F);
            }
        }
    }
}