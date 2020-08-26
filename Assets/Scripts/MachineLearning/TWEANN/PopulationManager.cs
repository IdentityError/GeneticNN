using Assets.Scripts.CustomBehaviour;
using Assets.Scripts.TUtils.ObjectPooling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MachineLearning.TWEANN
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

        public List<ISimulatingOrganism> populationList;
        private int currentSimulating;
        private int generationCount = 0;

        [SerializeField] private Biocenosis biocenosis;
        [SerializeField] private EnabledOperators enabledOperators;
        [Space(15)]
        [SerializeField] private float timeScale = 1F;

        [Header("References")]
        [SerializeField] private GameObject individualPrefab;
        [SerializeField] private Track track;

        [Header("Parameters")]
        [Tooltip("Number of the population")]
        [SerializeField] private int populationNumber;
        [Tooltip("Max value for the mutation rate")]
        [SerializeField] private float maxMutationRate;
        [Tooltip("Minumum crossover ratio for a crossover operator")]
        [SerializeField] private float minCrossoverRatio;
        [Tooltip("Top percentage of population in each species selected for breeding")]
        [SerializeField] private float matingRatio;
        [Tooltip("Topological distance threshold for speciation")]
        [SerializeField] private float sharingThreshold;

        private IEnumerator CheckSimStat_C;
        private bool simulating = false;
        private float simulationTime = 0;
        private bool shouldRestart = false;

        private TrainerNEAT trainerNEAT;

        private UIManager uiManager;
        private List<Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism>> operationsDescriptors = null;

        /// <summary>
        ///   Force the end of the simulation
        /// </summary>
        public void ForceSimulationEnd()
        {
            foreach (ISimulatingOrganism individual in populationList)
            {
                if (individual.IsSimulating())
                {
                    individual.StopSimulating();
                }
                individual.SetRawFitness(individual.EvaluateFitnessFunction());
            }
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
        public void IndividualEndedSimulation(ISimulatingOrganism subject)
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
        ///   3. Substitute population <br> </br>
        /// </summary>
        private void AdvanceGeneration()
        {
            if (operationsDescriptors == null)
            {
                operationsDescriptors = new List<Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism>>();
            }
            else
            {
                trainerNEAT.UpdateCrossoverOperatorsProgressions(operationsDescriptors);
            }
            operationsDescriptors.Clear();

            IOrganism fittest = biocenosis.GetCurrentFittest();
            uiManager.UpdateTextBox1("Generation: " + generationCount + "\nHighest fitness: " + ((MonoBehaviour)fittest).gameObject.name + ", " + fittest.ProvideRawFitness() + "\n" + "Average fitness: " + biocenosis.GetAverageFitness());

            populationList = populationList.OrderByDescending(x => x.ProvideRawFitness()).ToList();
            List<ISimulatingOrganism> subList = populationList.GetRange(0, populationNumber / 2);
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
                uiManager.AppendToLog("50% of the population has converged to 95% of the max achievable fitness in " + generationCount + " generations");
            }
            //! Training
            Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[] pop = trainerNEAT.Train(biocenosis);

            //! Substitute population
            foreach (ISimulatingOrganism individual in populationList)
            {
                PoolManager.GetInstance().DeactivateObject(((MonoBehaviour)individual).gameObject);
            }
            populationList.Clear();

            for (int i = 0; i < pop.Length; i++)
            {
                ISimulatingOrganism childInd = InstantiateIndividual(new NeuralNetwork(pop[i].Item2), i.ToString());
                populationList.Add(childInd);
                operationsDescriptors.Add(new Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism>(pop[i].Item1, childInd));
            }
            biocenosis.Speciate(populationList.ToArray());

            generationCount++;
            currentSimulating = pop.Length;
            simulating = true;
            uiManager.UpdateGenerationCount(generationCount);
        }

        /// <summary>
        ///   Initialize the first population, used only one time at the start
        /// </summary>
        private void InitializeAncestors()
        {
            for (int i = 0; i < populationNumber; i++)
            {
                populationList.Add(InstantiateIndividual(new NeuralNetwork(trainerNEAT.GetPredefinedGenotype()), i.ToString()));
            }
            generationCount++;
            currentSimulating = populationNumber;
            GlobalParams.InitializeGlobalInnovationNumber(trainerNEAT.GetPredefinedGenotype());
            simulating = true;
            biocenosis.Speciate(populationList.ToArray());
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
            List<CrossoverOperator> crossoverOperators = new List<CrossoverOperator>();
            if (enabledOperators.uniformEnabled) crossoverOperators.Add(new UniformCrossoverOperator());
            if (enabledOperators.singlePointEnabled) crossoverOperators.Add(new SinglePointCrossover());
            if (enabledOperators.kPointEnabled) crossoverOperators.Add(new KPointsCrossoverOperator());
            if (enabledOperators.averageEnabled) crossoverOperators.Add(new AverageCrossoverOperator());

            if (crossoverOperators.Count < 1)
            {
                throw new System.Exception("There is no Crossover operator!");
            }

            trainerNEAT = new TrainerNEAT(new CrossoverOperatorsWrapper(crossoverOperators), maxMutationRate, minCrossoverRatio, matingRatio, 6F * track.Length());

            uiManager = FindObjectOfType<UIManager>();
            uiManager?.UpdateTrackLength(track.Length());
            //uiManager?.DrawNetUI(trainerProvider.ProvideTrainer().GetPredefinedTopologyDescriptor());
            CheckSimStat_C = CheckSimulationState();
            populationList = new List<ISimulatingOrganism>();
            biocenosis = new Biocenosis(sharingThreshold);
            InitializeAncestors();
            StartCoroutine(CheckSimStat_C);
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