using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingManager : Manager
{
    [Header("Training")]
    public int populationNumber;
    [Range(0F, 1F)]
    public float mutationRate;
    public bool saveFittest;
    public bool trainFittest;
    [Header("Track")]
    public GameObject track;

    protected int generationCount = 0;
    protected CarIndividual[] population = null;
    private List<CarIndividual> completedTrackIndividuals = new List<CarIndividual>();
    protected int currentAlive;

    private DNA.DnaTopology simulationTopology;

    private float currentTrackLength;
    private GameObject finishLine;

    private UIManager uiManager;
    private SimulationStats currentBestSimulation = null;

    protected void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        finishLine = TUtilsProvider.GetInstance().GetFirstGameObjectInChildrenWithTag(track, "FinishLine", false);
        population = new CarIndividual[populationNumber];

        if (trainFittest)
        {
            SaveObject saveObj = TSaveManager.GetInstance().LoadPersistentData(TSaveManager.TRACKS_STATS);
            if (saveObj != null)
            {
                TracksStats stats = saveObj.GetData<TracksStats>();
                TrackBest best = stats.Get(track.name);
                if (best != null)
                {
                    currentBestSimulation = best.stats;
                    DNA bestDna = best.individualData.GetDNA();
                    StartNewSimulation(CreatePopulationFromSeedDNA(bestDna));
                    simulationTopology = bestDna.topology;
                    uiManager.DrawNetUI(bestDna.topology);
                    uiManager.AppendToLog("Retrieved the best on this track, training it...");
                    uiManager.UpdateSavedBestStats(best);
                }
                else
                {
                    uiManager.AppendToLog("Unable to find the best on this track, starting a clear training session...");
                    simulationTopology = carIndividualPrefab.GetComponent<CarIndividual>().predefinedTopology;
                    StartNewSimulation(null);
                }
            }
            else
            {
                uiManager.AppendToLog("Unable to find Track Stats, starting a clear training session...");
                simulationTopology = carIndividualPrefab.GetComponent<CarIndividual>().predefinedTopology;
                StartNewSimulation(null);
            }
        }
        else
        {
            simulationTopology = carIndividualPrefab.GetComponent<CarIndividual>().predefinedTopology;
            StartNewSimulation(null);
        }
        uiManager.DrawNetUI(simulationTopology);
        uiManager.UpdateTrackLength(CalculateTrackLength());
    }

    private float CalculateTrackLength()
    {
        float length = 0;
        foreach (Transform child in track.transform)
        {
            if (child.tag.Equals("TrackPart"))
            {
                length += child.localScale.z * 30;
            }
        }
        length -= TMath.Abs((finishLine.transform.localPosition - startPoint.localPosition).magnitude);
        return length;
    }

    protected void Update()
    {
        Time.timeScale = timeScale;
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (CarIndividual car in population)
            {
                car.StopSimulating(false);
            }
        }
    }

    public void CalculateFitness(CarIndividual individual)
    {
        individual.fitness = 6F * (float)Math.Exp(1.5F * (individual.stats.averageThrottle - 1F)) * (1F * individual.stats.distance) * Time.deltaTime;
    }

    public void HasEndedSimulation(CarIndividual individual)
    {
        if (individual != null)
        {
            if (individual.stats.BetterThan(currentBestSimulation))
            {
                currentBestSimulation = individual.stats;
                TrackBest trackBest = new TrackBest(individual.stats, individual.GetIndividualData());
                if (saveFittest)
                {
                    uiManager.AppendToLog("New best on this track, overriding it...");
                    SaveObject saveObj = TSaveManager.GetInstance().LoadPersistentData(TSaveManager.TRACKS_STATS);
                    if (saveObj != null)
                    {
                        TracksStats tracksStats = saveObj.GetData<TracksStats>();
                        tracksStats.AddOrReplace(trackBest);
                        TSaveManager.GetInstance().SavePersistentData(tracksStats, TSaveManager.TRACKS_STATS);
                    }
                    else
                    {
                        TracksStats newStats = new TracksStats();
                        newStats.AddOrReplace(trackBest);
                        TSaveManager.GetInstance().SavePersistentData(newStats, TSaveManager.TRACKS_STATS);
                        uiManager.AppendToLog("TrackStats not found, creating new one and setting the best for this track...");
                    }
                }
                uiManager.UpdateSavedBestStats(trackBest);
            }
        }

        if (individual != null)
        {
            completedTrackIndividuals.Add(individual);
        }

        if (currentAlive > 0)
        {
            currentAlive--;
        }
        if (currentAlive == 0)
        {
            EndAndRestartNewSimulation();
        }
    }

    protected override CarIndividual InstantiateAndInitializeIndividual(DNA dna, string name)
    {
        CarIndividual newCar = base.InstantiateAndInitializeIndividual(dna, name);
        newCar.manager = this;
        return newCar;
    }

    protected void StartNewSimulation(CarIndividual[] newPopulation)
    {
        if (newPopulation == null)
        {
            for (int i = 0; i < populationNumber; i++)
            {
                population[i] = InstantiateAndInitializeIndividual(null, "Car" + i);
            }
            return;
        }

        currentAlive = populationNumber;
        generationCount++;

        uiManager.UpdateGenerationCount(generationCount);

        for (int i = 0; i < populationNumber; i++)
        {
            if (population[i] != null)
            {
                Destroy(population[i].gameObject);
            }
            newPopulation[i].gameObject.SetActive(true);
            population[i] = newPopulation[i];
        }
    }

    protected void EndAndRestartNewSimulation()
    {
        currentAlive = 0;
        CarIndividual[] newPopulation = CrossoverPopulation();
        StartNewSimulation(newPopulation);
    }

    protected CarIndividual[] CrossoverPopulation()
    {
        //NormalizePopulationBreedingProbabilities();
        CarIndividual[] newPopulation = new CarIndividual[populationNumber];

        CarIndividualData[] parents = PickFittestTwo();

        for (int i = 0; i < populationNumber; i++)
        {
            DNA childDna = parents[0].GetDNA().Crossover(parents[1].GetDNA(), 3);
            childDna.Mutate(mutationRate);

            CarIndividual child = InstantiateAndInitializeIndividual(childDna, "Car" + i);
            child.gameObject.SetActive(false);
            newPopulation[i] = child;
        }
        return newPopulation;
    }

    protected CarIndividualData[] PickFittestTwo()
    {
        CarIndividualData[] returnOut = new CarIndividualData[2];
        int inserted = 0;
        if (completedTrackIndividuals.Count > 0)
        {
            returnOut[0] = new CarIndividualData(completedTrackIndividuals[0].neuralNet.dna, completedTrackIndividuals[0].fitness);
            inserted = 1;
        }
        if (completedTrackIndividuals.Count > 1)
        {
            returnOut[1] = new CarIndividualData(completedTrackIndividuals[1].neuralNet.dna, completedTrackIndividuals[1].fitness);
            uiManager.AppendToLog("Parents selected as the two that completed the track");
            return returnOut;
        }

        CarIndividual firstCar = null;
        float firstFitness = 0F;
        foreach (CarIndividual car in population)
        {
            if (car.fitness > firstFitness)
            {
                firstCar = car;
                firstFitness = car.fitness;
            }
        }
        if (inserted > 0)
        {
            uiManager.AppendToLog("Parent one completed the track, the second selected as the one with highest fitness");
            returnOut[1] = new CarIndividualData(firstCar.neuralNet.dna, firstCar.fitness);
            return returnOut;
        }
        else
        {
            float secondFitness = 0F;
            CarIndividual secondCar = null;
            foreach (CarIndividual car in population)
            {
                if (car.fitness > secondFitness)
                {
                    if (populationNumber > 1)
                    {
                        if (car != firstCar)
                        {
                            secondCar = car;
                            secondFitness = car.fitness;
                        }
                    }
                    else
                    {
                        secondCar = car;
                        secondFitness = car.fitness;
                    }
                }
            }
            uiManager.AppendToLog("No parents completed the track, selected the best two");
            returnOut[0] = new CarIndividualData(firstCar.neuralNet.dna, firstFitness);
            returnOut[1] = new CarIndividualData(secondCar.neuralNet.dna, secondFitness);
            return returnOut;
        }
    }

    protected void NormalizePopulationBreedingProbabilities()
    {
        float fitnessSum = 0;
        foreach (CarIndividual car in population)
        {
            fitnessSum += car.fitness;
        }

        foreach (CarIndividual car in population)
        {
            car.breedingProbability = car.fitness / fitnessSum;
        }
    }

    protected CarIndividual PickRandom()
    {
        float seed = UnityEngine.Random.Range(0F, 1F);
        int index = -1;
        while (seed >= 0)
        {
            seed -= population[++index].breedingProbability;
        }
        return population[index];
    }

    protected CarIndividual[] CreatePopulationFromSeedDNA(DNA seedDna)
    {
        CarIndividual[] newPopulation = new CarIndividual[populationNumber];
        for (int i = 0; i < populationNumber; i++)
        {
            DNA individualDna = new DNA(seedDna.topology, seedDna.weights);
            individualDna.Mutate(mutationRate);
            newPopulation[i] = InstantiateAndInitializeIndividual(individualDna, "Car" + i);
        }
        return newPopulation;
    }
}