using UnityEngine;
using UnityEngine.UI;

public class GeneticManager : MonoBehaviour
{
    [Header("References")]
    public GameObject carIndividualPrefab;
    [Header("Parameters")]
    public float timeScale = 1F;
    public int populationNumber;
    [Range(0F, 1F)]
    public float mutationRate;
    public bool saveFittest;
    public bool trainFittest;

    [Header("Neural Net")]
    public DNA.DnaTopology topology;

    [Header("UI")]
    public Text generationTxt = null;
    public Text savedMaxFitnessTxt = null;
    public Text currentMaxFitnessTxt = null;

    private int generationCount = 0;
    private CarIndividual[] population = null;
    private int currentAlive;
    private float currentMaxFitness = 0F;

    private void Start()
    {
        population = new CarIndividual[populationNumber];
        CarIndividualData seedData = SaveManager.GetInstance().LoadPersistentData(SaveManager.FITTEST_DATA).GetData<CarIndividualData>();
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

    private void Update()
    {
        Time.timeScale = timeScale;
        if (Input.GetKeyDown(KeyCode.R))
        {
            EndCurrentSimulation();
        }
        foreach (CarIndividual car in population)
        {
            if (car.fitness > currentMaxFitness)
            {
                currentMaxFitness = car.fitness;
            }
        }
        currentMaxFitnessTxt.text = "Current max fitness: " + currentMaxFitness;
    }

    private CarIndividual[] CrossoverPopulation()
    {
        //NormalizePopulationBreedingProbabilities();
        CarIndividual[] newPopulation = new CarIndividual[populationNumber];

        CarIndividualData[] parents = PickFittestTwo();

        if (saveFittest)
        {
            CarIndividualData currentFittest = SaveManager.GetInstance().LoadPersistentData(SaveManager.FITTEST_DATA).GetData<CarIndividualData>();
            if (parents[0].GetFitness() > currentFittest.GetFitness())
            {
                SaveManager.GetInstance().SavePersistentData<CarIndividualData>(parents[0], SaveManager.FITTEST_DATA);
                Debug.Log("Overridden the fittest data, fitness: " + parents[0].GetFitness());
                savedMaxFitnessTxt.text = "Saved max fitness: " + parents[0].GetFitness();
            }
        }

        for (int i = 0; i < populationNumber; i++)
        {
            DNA childDna = parents[0].GetDNA().Crossover(parents[1].GetDNA());
            childDna.Mutate(mutationRate);

            CarIndividual child = Instantiate(carIndividualPrefab, new Vector3(0F, 0.75F, 0F), Quaternion.identity).GetComponent<CarIndividual>();
            child.gameObject.name = "Car" + i;
            child.InitializeNeuralNet(childDna);
            child.manager = this;

            child.gameObject.SetActive(false);
            newPopulation[i] = child;
        }
        return newPopulation;
    }

    public void DecrementPopulationAliveCount()
    {
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
        CarIndividual[] newPopulation = CrossoverPopulation();
        StartNewSimulation(newPopulation);
    }

    private void StartNewSimulation(CarIndividual[] newPopulation)
    {
        if (newPopulation == null)
        {
            for (int i = 0; i < populationNumber; i++)
            {
                CarIndividual car = Instantiate(carIndividualPrefab, new Vector3(0F, 0.75F, 0F), Quaternion.identity).GetComponent<CarIndividual>();
                car.gameObject.name = "Car" + i;
                car.manager = this;
                DNA individualDna = new DNA(topology);
                car.InitializeNeuralNet(individualDna);
                population[i] = car;
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
        float seed = Random.Range(0F, 1F);
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
            CarIndividual car = Instantiate(carIndividualPrefab, new Vector3(0F, 0.75F, 0F), Quaternion.identity).GetComponent<CarIndividual>();
            car.gameObject.name = "Car" + i;
            car.manager = this;
            DNA individualDna = new DNA(seedDna.topology, seedDna.weights);
            individualDna.Mutate(mutationRate);
            car.InitializeNeuralNet(individualDna);
            newPopulation[i] = car;
        }
        return newPopulation;
    }
}