using System;
using UnityEngine;
using UnityEngine.UI;

public class GeneticTrackTrainingManager : TrainingManager
{
    [Header("Parameters")]
    public bool saveFittest;
    public bool trainFittest;

    [Header("UI")]
    public Text generationTxt = null;
    public Text savedMaxFitnessTxt = null;
    public Text currentBestStatsTxt = null;
    public Text currentMaxFitnessTxt = null;

    private void Start()
    {
        population = new CarIndividual[populationNumber];
        SaveObject saveObj = SaveManager.GetInstance().LoadPersistentData(SaveManager.FITTEST_DATA);
        if (saveObj != null)
        {
            CarIndividualData seedData = saveObj.GetData<CarIndividualData>();
            savedMaxFitnessTxt.text = "Saved max fitness: " + seedData.GetFitness();
            if (trainFittest)
            {
                StartNewSimulation(CreatePopulationFromSeedDNA(seedData.GetDNA()));
            }
            else
            {
                StartNewSimulation(null);
            }
        }
        else
        {
            Debug.Log("Unable to load fittest, starting clear simulation");
            savedMaxFitnessTxt.text = "0";
            StartNewSimulation(null);
        }
    }

    private void Update()
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
        currentMaxFitnessTxt.text = "Current generation max fitness: " + currentGenerationMaxFitness;
    }

    private CarIndividual[] CrossoverPopulation()
    {
        //NormalizePopulationBreedingProbabilities();
        CarIndividual[] newPopulation = new CarIndividual[populationNumber];

        CarIndividualData[] parents = PickFittestTwo();

        for (int i = 0; i < populationNumber; i++)
        {
            DNA childDna = parents[0].GetDNA().Crossover(parents[1].GetDNA());
            childDna.Mutate(mutationRate);

            CarIndividual child = InstantiateAndInitializeIndividual(childDna, "Car" + i);
            child.gameObject.SetActive(false);
            newPopulation[i] = child;
        }
        return newPopulation;
    }

    public override void HasEndedSimulation(CarIndividual individual)
    {
        if (individual != null && individual.fitness > maxFitness)
        {
            maxFitness = individual.fitness;
            bestStats = individual.stats;
            currentBestStatsTxt.text = bestStats.ToString() + "\nFitness: " + maxFitness;

            if (saveFittest)
            {
                SaveObject saveObj = SaveManager.GetInstance().LoadPersistentData(SaveManager.FITTEST_DATA);
                if (saveObj != null)
                {
                    CarIndividualData currentFittest = saveObj.GetData<CarIndividualData>();
                    SaveManager.GetInstance().SavePersistentData<CarIndividualData>(new CarIndividualData(individual.neuralNet.dna, individual.fitness), SaveManager.FITTEST_DATA);
                    Debug.Log("Overridden the fittest data, fitness: " + individual.fitness);
                    savedMaxFitnessTxt.text = "Saved max fitness: " + individual.fitness;
                }
                else
                {
                    SaveManager.GetInstance().SavePersistentData<CarIndividualData>(new CarIndividualData(individual.neuralNet.dna, individual.fitness), SaveManager.FITTEST_DATA);
                }
            }
        }

        if (currentAlive > 0)
        {
            currentAlive--;
        }
        if (currentAlive == 0)
        {
            EndCurrentSimulation();
        }
    }

    private void EndCurrentSimulation()
    {
        currentAlive = 0;
        currentGenerationMaxFitness = 0;
        CarIndividual[] newPopulation = CrossoverPopulation();
        StartNewSimulation(newPopulation);
    }

    private void StartNewSimulation(CarIndividual[] newPopulation)
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
        generationTxt.text = generationCount + "° Generation";

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

    private CarIndividualData[] PickFittestTwo()
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
            if (car.fitness > secondFitness)
            {
                if (car != firstCar)
                {
                    secondCar = car;
                    secondFitness = car.fitness;
                }
            }
        }

        CarIndividualData[] returnOut = new CarIndividualData[2];
        returnOut[0] = new CarIndividualData(firstCar.neuralNet.dna, firstFitness);
        returnOut[1] = new CarIndividualData(secondCar.neuralNet.dna, secondFitness);
        return returnOut;
    }

    private void NormalizePopulationBreedingProbabilities()
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

    private CarIndividual PickRandom()
    {
        float seed = UnityEngine.Random.Range(0F, 1F);
        int index = -1;
        while (seed >= 0)
        {
            seed -= population[++index].breedingProbability;
        }
        return population[index];
    }

    private CarIndividual[] CreatePopulationFromSeedDNA(DNA seedDna)
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

    public override void CalculateFitness(CarIndividual individual)
    {
        individual.fitness = (6F * (float)Math.Exp(3F * (individual.stats.averageThrottle - 1F)) * (0.2F * individual.stats.distance)) * Time.deltaTime;
    }
}