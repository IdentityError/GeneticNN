using System;
using UnityEngine;
using UnityEngine.UI;

public class TrainingManager : Manager
{
    public enum TrainingAmbient { TRACK, OPEN_AREA }

    [Header("Training")]
    public TrainingAmbient ambient;
    public int populationNumber;
    [Range(0F, 1F)]
    public float mutationRate;
    public bool saveFittest;
    public bool trainFittest;
    [Header("UI")]
    public Text generationTxt = null;
    public Text savedMaxFitnessTxt = null;
    public Text currentBestStatsTxt = null;
    public Text currentMaxFitnessTxt = null;

    protected CarIndividual.SimulationStats bestStats;
    protected int generationCount = 0;
    protected CarIndividual[] population = null;
    protected int currentAlive;
    protected float currentGenerationMaxFitness = 0F;
    protected float maxFitness = 0F;

    private UINetBuilder uiBuilder;
    private DNA.DnaTopology simulationTopology;

    protected void Start()
    {
        population = new CarIndividual[populationNumber];
        uiBuilder = FindObjectOfType<UINetBuilder>();
        SaveObject saveObj = TSaveManager.GetInstance().LoadPersistentData(TSaveManager.FITTEST_DATA);
        if (saveObj != null)
        {
            CarIndividualData seedData = saveObj.GetData<CarIndividualData>();
            savedMaxFitnessTxt.text = "Saved max fitness: " + seedData.GetFitness();
            if (trainFittest)
            {
                StartNewSimulation(CreatePopulationFromSeedDNA(seedData.GetDNA()));
                simulationTopology = seedData.GetDNA().topology;
                uiBuilder.DraweNetUI(seedData.GetDNA().topology);
            }
            else
            {
                simulationTopology = carIndividualPrefab.GetComponent<CarIndividual>().predefinedTopology;
                StartNewSimulation(null);
            }
        }
        else
        {
            simulationTopology = carIndividualPrefab.GetComponent<CarIndividual>().predefinedTopology;
            Debug.Log("Unable to load fittest, starting clear simulation");
            savedMaxFitnessTxt.text = "0";
            StartNewSimulation(null);
        }
        uiBuilder.DraweNetUI(simulationTopology);
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
        foreach (CarIndividual car in population)
        {
            if (car.fitness > currentGenerationMaxFitness)
            {
                currentGenerationMaxFitness = car.fitness;
            }
        }
        currentMaxFitnessTxt.text = "Current generation max <i>raw</i> fitness: " + currentGenerationMaxFitness;
    }

    public void CalculateFitness(CarIndividual individual)
    {
        switch (ambient)
        {
            case TrainingAmbient.TRACK:
                individual.fitness = 6F * (float)Math.Exp(1.5F * (individual.stats.averageThrottle - 1F)) * (1F * individual.stats.distance) * Time.deltaTime;
                break;

            case TrainingAmbient.OPEN_AREA:
                individual.fitness = 6F * 2F * individual.stats.time * (1F * individual.stats.distance) * Time.deltaTime;
                break;
        }
    }

    public void HasEndedSimulation(CarIndividual individual)
    {
        if (individual != null && individual.fitness > maxFitness)
        {
            maxFitness = individual.fitness;
            bestStats = individual.stats;
            currentBestStatsTxt.text = bestStats.ToString() + "\nFitness: " + maxFitness;

            if (saveFittest)
            {
                SaveObject saveObj = TSaveManager.GetInstance().LoadPersistentData(TSaveManager.FITTEST_DATA);
                if (saveObj != null)
                {
                    CarIndividualData currentFittest = saveObj.GetData<CarIndividualData>();
                    if (currentFittest.GetFitness() < individual.fitness)
                    {
                        TSaveManager.GetInstance().SavePersistentData<CarIndividualData>(new CarIndividualData(individual.neuralNet.dna, individual.fitness), TSaveManager.FITTEST_DATA);
                        Debug.Log("Overridden the fittest data, fitness: " + individual.fitness);
                        savedMaxFitnessTxt.text = "Saved max fitness: " + individual.fitness;
                    }
                }
                else
                {
                    TSaveManager.GetInstance().SavePersistentData<CarIndividualData>(new CarIndividualData(individual.neuralNet.dna, individual.fitness), TSaveManager.FITTEST_DATA);
                    Debug.Log("Overridden the fittest data, fitness: " + individual.fitness);
                    savedMaxFitnessTxt.text = "Saved max fitness: " + individual.fitness;
                }
            }
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

        generationTxt.text = generationCount + "° generation";

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
        currentGenerationMaxFitness = 0;
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

        float secondFitness = 0F;

        CarIndividual secondCar = null;
        foreach (CarIndividual car in population)
        {
            if (car != firstCar && car.fitness > secondFitness)
            {
                secondCar = car;
                secondFitness = car.fitness;
            }
        }

        CarIndividualData[] returnOut = new CarIndividualData[2];
        returnOut[0] = new CarIndividualData(firstCar.neuralNet.dna, firstFitness);
        returnOut[1] = new CarIndividualData(secondCar.neuralNet.dna, secondFitness);
        return returnOut;
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