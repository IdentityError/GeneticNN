using Assets.Scripts.CustomBehaviour;
using Assets.Scripts.TUtils.ObjectPooling;
using Assets.Scripts.TUtils.SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.MachineLearning.TWEANN.DescriptorsWrapper;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    public class PopulationManager : MonoBehaviour
    {
        [System.Serializable]
        public struct OptionsWrapper
        {
            [Header("Parameters")]
            [Tooltip("Number of the population")]
            public int populationNumber;
            public TrainerPreferences preferences;
        }

        public List<ISimulatingOrganism> populationList;
        private int currentSimulating;
        private int generationCount = 0;

        [SerializeField] private Biocenosis biocenosis;
        [SerializeField] private OptionsWrapper options;
        [Space(15)]
        [SerializeField] private float timeScale = 1F;

        [Header("References")]
        [SerializeField] private GameObject individualPrefab;
        [SerializeField] private Track track;

        [Header("Parameters")]
        private IEnumerator CheckSimStat_C;
        private bool simulating = false;
        private float simulationTime = 0;
        private bool shouldRestart = false;
        private int completedSimulationCount = 0;
        private bool completedTrack = false;

        private int simulationCount = 0;
        private TrainerNEAT trainerNEAT;

        private UIManager uiManager;
        private List<Tuple<CrossoverOperationDescriptor, IOrganism>> operationsDescriptors = null;

        private event Action ForceEnded;

        public void SubscribeToForceEnded(Action func)
        {
            ForceEnded += func;
        }

        public void UnsubscribeToForceEnded(Action func)
        {
            ForceEnded -= func;
        }

        /// <summary>
        ///   Force the end of the simulation
        /// </summary>
        public void ForceSimulationEnd()
        {
            ForceEnded();
            //foreach (ISimulatingOrganism individual in populationList)
            //{
            //    if (individual.IsSimulating())
            //    {
            //        individual.StopSimulating();
            //    }
            //    individual.SetRawFitness(individual.EvaluateFitnessFunction());
            //}
            currentSimulating = 0;
            SimulationEnded();
            shouldRestart = false;
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
        public void IndividualEndedSimulation(ISimulatingOrganism subject, bool completedTrack)
        {
            if (completedTrack)
            {
                completedSimulationCount++;
            }
            currentSimulating--;
            if (currentSimulating <= 0 && simulating)
            {
                SimulationEnded();
            }
        }

        /// <summary>
        ///   Advance to the next generation with the following steps: <br> </br>
        ///   1. Speciation <br> </br>
        ///   2. Training <br> </br>
        ///   3. Substitute population <br> </br>
        /// </summary>
        private void AdvanceGeneration()
        {
            if (generationCount > 50)
            {
                for (int i = 0; i < populationList.Count; i++)
                {
                    PoolManager.GetInstance().DeactivateObject(((MonoBehaviour)populationList.ElementAt(i)).gameObject);
                }
                simulationCount++;
                Debug.Log("restarting");
                InitializeAncestors();
                return;
            }
            operationsDescriptors.Clear();
            if (generationCount > 1)
            {
                trainerNEAT.UpdateCrossoverOperatorsProgressions(biocenosis);
            }

            #region Convergence

            IOrganism fittest = biocenosis.GetCurrentFittest();
            //Debug.Log("Generation: " + generationCount + "\nHighest fitness: " + ((MonoBehaviour)fittest).gameObject.name + ", " + fittest.ProvideRawFitness() + "\n" + "Average fitness: " + biocenosis.GetAverageFitness() + "\nCT: " + completedSimulationCount);

            TSaveManager.SerializeToFile("data/averageFitness" + simulationCount + ".csv", biocenosis.GetAverageFitness().ToString(), true);
            TSaveManager.SerializeToFile("data/completedTrack" + simulationCount + ".csv", completedSimulationCount.ToString(), true);

            uiManager.UpdateTextBox1(completedSimulationCount.ToString());
            populationList = populationList.OrderByDescending(x => x.ProvideRawFitness()).ToList();
            List<ISimulatingOrganism> subList = populationList.GetRange(0, options.populationNumber / 2);
            bool converged = true;
            foreach (IOrganism organism in subList)
            {
                if (organism.ProvideRawFitness() < 1900)
                {
                    converged = false;
                    break;
                }
            }

            if (converged)
            {
                uiManager.AppendToLog("50% of the population has converged to 90% of the max achievable fitness in " + generationCount + " generations");
            }

            if (completedSimulationCount >= (int)(options.populationNumber * 0.9) && !completedTrack)
            {
                completedTrack = true;
                uiManager.AppendToLog("90% of the population has completed the track in " + generationCount + " generations");
            }

            #endregion Convergence

            //! Training
            Tuple<CrossoverOperationDescriptor, Genotype>[] pop = trainerNEAT.Train(biocenosis);

            //! Substitute population
            for (int i = 0; i < populationList.Count; i++)
            {
                PoolManager.GetInstance().DeactivateObject(((MonoBehaviour)populationList.ElementAt(i)).gameObject);
                ISimulatingOrganism childInd = InstantiateIndividual(new NeuralNetwork(pop[i].Item2), i.ToString());
                operationsDescriptors.Add(new Tuple<CrossoverOperationDescriptor, IOrganism>(pop[i].Item1, childInd));
                populationList[i] = childInd;
            }
            biocenosis.Speciate(operationsDescriptors.ToArray());

            generationCount++;
            currentSimulating = pop.Length;
            simulating = true;
            completedSimulationCount = 0;
            uiManager.UpdateGenerationCount(generationCount);
        }

        /// <summary>
        ///   Initialize the first population, used only one time at the start
        /// </summary>
        private void InitializeAncestors()
        {
            populationList.Clear();
            generationCount = 1;
            operationsDescriptors = new List<Tuple<CrossoverOperationDescriptor, IOrganism>>();
            for (int i = 0; i < options.populationNumber; i++)
            {
                ISimulatingOrganism org = InstantiateIndividual(new NeuralNetwork(trainerNEAT.GetPredefinedGenotype()), i.ToString());
                populationList.Add(org);
                operationsDescriptors.Add(new Tuple<CrossoverOperationDescriptor, IOrganism>(default, org));
            }
            uiManager.UpdateGenerationCount(generationCount);
            currentSimulating = options.populationNumber;
            GlobalParams.InitializeGlobalInnovationNumber(trainerNEAT.GetPredefinedGenotype());
            GlobalParams.ResetGenerationMutations();
            simulating = true;
            biocenosis.Speciate(operationsDescriptors.ToArray());
        }

        /// <summary>
        ///   Instantiate a new individual
        /// </summary>
        /// <param name="neuralNet"> </param>
        /// <param name="name"> </param>
        /// <returns> </returns>
        private ISimulatingOrganism InstantiateIndividual(NeuralNetwork neuralNet, string name)
        {
            GameObject obj = PoolManager.GetInstance().Spawn("Individual", "prefab", track.GetStartPoint().localPosition, track.GetStartPoint().rotation);
            obj.name = name;
            ISimulatingOrganism individual = obj.GetComponent<ISimulatingOrganism>();
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
            AdvanceGeneration();
        }

        private void Start()
        {
            //string c = "";
            //for (int i = 0; i < 1000; i++)
            //{
            //    c += TMath.RandomGen.NextGaussian(0, 0.25) + "\n";
            //}
            //Debug.Log(c);

            List<CrossoverOperator> crossoverOperators = new List<CrossoverOperator>();
            if (options.preferences.uniformEnabled) crossoverOperators.Add(new UniformCrossoverOperator());
            if (options.preferences.singlePointEnabled) crossoverOperators.Add(new SinglePointCrossover());
            if (options.preferences.kPointEnabled) crossoverOperators.Add(new KPointsCrossoverOperator());
            if (options.preferences.averageEnabled) crossoverOperators.Add(new AverageCrossoverOperator());

            if (crossoverOperators.Count < 1)
            {
                throw new System.Exception("There is no Crossover operator!");
            }
            options.preferences.maxAchievableFitness = 6F * track.Length();
            trainerNEAT = new TrainerNEAT(options.preferences);

            uiManager = FindObjectOfType<UIManager>();
            uiManager?.UpdateTrackLength(track.Length());
            //uiManager?.DrawNetUI(trainerProvider.ProvideTrainer().GetPredefinedTopologyDescriptor());
            CheckSimStat_C = CheckSimulationState();
            populationList = new List<ISimulatingOrganism>();
            biocenosis = new Biocenosis(options.preferences.sharingThreshold, new CrossoverOperatorsWrapper(crossoverOperators));
            InitializeAncestors();
            StartCoroutine(CheckSimStat_C);

            //TSaveManager.DeleteObjectData("data/averageFitness.csv");
            //TSaveManager.DeleteObjectData("data/completedTrack.csv");
        }

        private void Update()
        {
            Time.timeScale = this.timeScale;
            if (simulating)
            {
                simulationTime += Time.deltaTime;
                if (simulationTime > 2.5F && shouldRestart)
                {
                    ForceSimulationEnd();
                }
            }
        }

        private IEnumerator CheckSimulationState()
        {
            yield return new WaitForSeconds(2.5F);
            while (true)
            {
                bool shouldRestartSim = true;
                foreach (ISimulatingOrganism individual in populationList)
                {
                    if (individual.IsSimulating())
                    {
                        if (individual.ProvideSimulationStats().lastThrottle > 0.085D)
                        {
                            shouldRestartSim = false;
                        }
                    }
                }
                shouldRestart = shouldRestartSim;
                yield return new WaitForSeconds(2.5F);
            }
        }
    }
}