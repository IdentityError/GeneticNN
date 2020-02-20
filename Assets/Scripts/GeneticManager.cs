using UnityEngine;

public class GeneticManager : MonoBehaviour
{
    [Header("References")]
    public GameObject carIndividualPrefab;
    public float timeScale = 1F;
    public int populationNumber;
    [Range(0F, 1F)]
    public float mutationRate;

    [Header("Neural Net")]
    public DNA.DnaTopology topology;

    private Car[] population = null;
    private int currentAlive;

    private void Start()
    {
        population = new Car[populationNumber];
        InitializeRandomPopulation();
    }

    private void Update()
    {
        Time.timeScale = timeScale;
        if (Input.GetKeyDown(KeyCode.R))
        {
            EndCurrentSimulation();
        }
    }

    private Car[] CrossoverPopulation()
    {
        //NormalizePopulationBreedingProbabilities();
        Car[] newPopulation = new Car[populationNumber];
        Car[] parents = PickFittestTwo();
        for (int i = 0; i < populationNumber; i++)
        {
            DNA childDna = parents[0].neuralNet.dna.Crossover(parents[1].neuralNet.dna);
            childDna.Mutate(mutationRate);

            Car child = Instantiate(carIndividualPrefab, new Vector3(0F, 0.75F, 0F), Quaternion.identity).GetComponent<Car>();
            child.gameObject.name = "Car" + i;
            child.InitializeNeuralNet(childDna);
            child.manager = this;

            child.gameObject.SetActive(false);
            newPopulation[i] = child;
        }
        return newPopulation;
    }

    private void InitializeRandomPopulation()
    {
        currentAlive = populationNumber;
        for (int i = 0; i < populationNumber; i++)
        {
            Car car = Instantiate(carIndividualPrefab, new Vector3(0F, 0.75F, 0F), Quaternion.identity).GetComponent<Car>();
            car.gameObject.name = "Car" + i;
            car.manager = this;
            DNA individualDna = new DNA(topology);
            car.InitializeNeuralNet(individualDna);
            population[i] = car;
        }
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
        Car[] newPopulation = CrossoverPopulation();
        StartNewSimulation(newPopulation);
    }

    private void StartNewSimulation(Car[] newPopulation)
    {
        currentAlive = populationNumber;

        for (int i = 0; i < populationNumber; i++)
        {
            Destroy(population[i].gameObject);
            newPopulation[i].gameObject.SetActive(true);
            population[i] = newPopulation[i];
        }
    }

    private Car PickRandom()
    {
        float seed = Random.Range(0F, 1F);
        int index = -1;
        while (seed >= 0)
        {
            seed -= population[++index].breedingProbability;
        }
        return population[index];
    }

    private Car[] PickFittestTwo()
    {
        float maxFitness = 0;

        Car firstCar = null;
        foreach (Car car in population)
        {
            if (car.fitness > maxFitness)
            {
                firstCar = car;
                maxFitness = car.fitness;
            }
        }

        maxFitness = 0F;

        Car secondCar = null;
        foreach (Car car in population)
        {
            if (car.fitness > maxFitness)
            {
                if (car != firstCar)
                {
                    secondCar = car;
                    maxFitness = car.fitness;
                }
            }
        }

        Car[] returnOut = new Car[2];
        returnOut[0] = firstCar;
        returnOut[1] = secondCar;
        return returnOut;
    }

    private void NormalizePopulationBreedingProbabilities()
    {
        float fitnessSum = 0;
        foreach (Car car in population)
        {
            fitnessSum += car.fitness;
        }

        foreach (Car car in population)
        {
            car.breedingProbability = car.fitness / fitnessSum;
        }
    }
}