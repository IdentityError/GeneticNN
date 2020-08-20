using Assets.Scripts.CustomBehaviour;
using Assets.Scripts.Descriptors;
using Assets.Scripts.NeuralNet;
using Assets.Scripts.Stores;
using Assets.Scripts.TUtils.ObjectPooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    public class PopulationManager : MonoBehaviour
    {
        [System.Serializable]
        public struct EnabledOperators
        {
            public bool uniformEnabled;
            public bool singlePointEnabled;
            public bool kPointEnabled;
            public bool averageEnabled;

            public EnabledOperators(bool uniformEnabled, bool singlePointEnabled, bool kPointEnabled, bool averageEnabled)
            {
                this.uniformEnabled = uniformEnabled;
                this.singlePointEnabled = singlePointEnabled;
                this.kPointEnabled = kPointEnabled;
                this.averageEnabled = averageEnabled;
            }
        }

        public List<ISimulatingIndividual> populationList;
        private float averageThrottle = 1;
        [SerializeField] private Biocenosis biocenosis;
        [SerializeField] private EnabledOperators enabledOperators;
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

        private TrainerNEAT trainerNEAT;

        private UIManager uiManager;
        private List<Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IIndividual>> operationsDescriptors = null;

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
                individual.SetRawFitness(individual.EvaluateFitnessFunction());
            }
            currentSimulating = 0;
            SimulationEnded();
        }

        /// <summary>
        ///   Get the generation count
        /// </summary>
        /// <returns> </returns>
        public int GetGenerationCount()
        {
            return generationCount;
        }

        /// <summary>
        ///   Get the current track of the simulation
        /// </summary>
        /// <returns> </returns>
        public Track GetTrack()
        {
            return track;
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
            subject.SetRawFitness(subject.EvaluateFitnessFunction());
            if (currentSimulating <= 0 && simulating)
            {
                SimulationEnded();
            }
        }

        /// <summary>
        ///   Advance to the next generation with the following steps: <br> </br>
        ///   1. Speciation <br> </br>
        ///   2. Training <br> </br>
        ///   3. Instantiate new population <br> </br>
        /// </summary>
        private void AdvanceGeneration()
        {
            if (operationsDescriptors == null)
            {
                operationsDescriptors = new List<Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IIndividual>>();
            }
            else
            {
                trainerNEAT.UpdateCrossoverOperatorsProgressions(operationsDescriptors);
            }
            operationsDescriptors.Clear();

            //! Speciation
            biocenosis.Speciate(populationList.ToArray());
            //uiManager.AppendToLog("Current biocenosis: \n" + biocenosis.ToString());

            foreach (Species species in biocenosis.GetSpeciesList())
            {
                IIndividual champ = species.GetChamp();
                double maxFitness = champ.ProvideAdjustedFitness();
                double averageFitness = species.GetAdjustedFitnessSum() / species.GetIndividualCount();
                double value = 1D - ((maxFitness - averageFitness) / maxFitness);
                species.SetMutationRate((float)(value));
                //species.breedingParameters.crossoverProbability = 1 - species.breedingParameters.mutationProbability;
            }

            ISimulatingIndividual fittest = GetFittest();
            uiManager.UpdateTextBox1("Highest fitness: " + fittest.ProvideRawFitness() + "\n" + "Average fitness: " + biocenosis.GetAverageFitness());
            //Debug.Log(fittest.ToString() + "\n" + fittest.ProvideNeuralNet().GetGenotype().ToString());
            //uiManager.AppendToLog("Biocenosis:" + biocenosis.ToString());
            //UpdateFittest(fittest.ProvideNeuralNet().GetGenotype(), fittest.ProvideFitness());
            //breedingParameters.crossoverProbability = parameter;

            // Retrieve a new trained Network population
            Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[] pop = trainerNEAT.Train(biocenosis);

            // Destroy all the objects
            foreach (ISimulatingIndividual individual in populationList)
            {
                PoolManager.GetInstance().DeactivateObject(((MonoBehaviour)individual).gameObject);
            }
            populationList.Clear();

            // Instantiate new population
            for (int i = 0; i < pop.Length; i++)
            {
                ISimulatingIndividual childInd = InstantiateIndividual(new NeuralNetwork(pop[i].Item2), i.ToString());
                populationList.Add(childInd);
                operationsDescriptors.Add(new Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IIndividual>(pop[i].Item1, childInd));
            }

            generationCount++;
            currentSimulating = pop.Length;
            simulating = true;
            averageThrottle = 1;
            uiManager.UpdateGenerationCount(generationCount);
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

        private ISimulatingIndividual GetFittest()
        {
            double max = 0;
            ISimulatingIndividual best = null;
            foreach (ISimulatingIndividual individual in populationList)
            {
                if (individual.ProvideRawFitness() > max)
                {
                    best = individual;
                    max = individual.ProvideRawFitness();
                }
            }
            return best;
        }

        /// <summary>
        ///   Initialize the first population, used only one time at the start
        /// </summary>
        private void InitializeAncestors()
        {
            for (int i = 0; i < initialPopulationNumber; i++)
            {
                populationList.Add(InstantiateIndividual(new NeuralNetwork(trainerNEAT.GetPredefinedGenotype()), i.ToString()));
            }
            generationCount++;
            currentSimulating = initialPopulationNumber;
            GlobalParams.InitializeGlobalInnovationNumber(trainerNEAT.GetPredefinedGenotype());
            //foreach (IIndividual ind in population)
            //{
            //    Debug.Log(ind.ProvideNeuralNet().GetGenotype().ToString());
            //}
            //StartCoroutine(ShouldRestart_C);
            simulating = true;
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
        ///   Called when the simulation has ended
        /// </summary>
        private void SimulationEnded()
        {
            simulating = false;
            simulationTime = 0;
            generationMaxFitness = 0;
            AdvanceGeneration();
        }

        private void Start()
        {
            List<CrossoverOperator> crossoverOperators = new List<CrossoverOperator>();
            if (enabledOperators.uniformEnabled) crossoverOperators.Add(new UniformCrossoverOperator());
            if (enabledOperators.singlePointEnabled) crossoverOperators.Add(new SinglePointCrossover());
            if (enabledOperators.kPointEnabled) crossoverOperators.Add(new KPointsCrossoverOperator());
            if (enabledOperators.averageEnabled) crossoverOperators.Add(new AverageCrossoverOperator());

            if (crossoverOperators.Count < 1)
            {
                throw new System.Exception("There is no Crossover operator!");
            }

            trainerNEAT = new TrainerNEAT(new Breeding(0.5F, 1F, crossoverOperators));

            Debug.Log(trainerNEAT.breeding.crossoverOperators.Count);

            uiManager = FindObjectOfType<UIManager>();
            uiManager?.UpdateTrackLength(track.Length());
            //uiManager?.DrawNetUI(trainerProvider.ProvideTrainer().GetPredefinedTopologyDescriptor());
            ShouldRestart_C = CheckAverageThrottle();
            populationList = new List<ISimulatingIndividual>();
            biocenosis = new Biocenosis(sharingThreshold, trainerNEAT.breeding.rates.crossoverRate, trainerNEAT.breeding.rates.mutationRate);
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

        private void UpdateFittest(Genotype candidate, double fitness)
        {
            //IndividualData newBest = null;
            //SaveObject obj = TSaveManager.LoadPersistentData(TSaveManager.BEST_INDIVIDUAL);
            //if (obj != null)
            //{
            //    IndividualData other = obj.GetData<IndividualData>();
            //    if (other.GetFitness() < fitness)
            //    {
            //        uiManager?.AppendToLog("Updating current best");
            //        newBest = new IndividualData(candidate, fitness);
            //    }
            //}
            //else
            //{
            //    uiManager?.AppendToLog("Initializing first best individual");
            //    newBest = new IndividualData(candidate, fitness);
            //}
            //if (newBest != null)
            //{
            //    TSaveManager.SavePersistentData(newBest, TSaveManager.BEST_INDIVIDUAL);
            //}
        }
    }
}