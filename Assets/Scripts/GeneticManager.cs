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
        //SaveObject fittestDataObject = SaveManager.GetInstance().LoadPersistentData(SaveManager.BEST_FITNESS_DATA);
        //if (fittestDataObject != null)
        //{
        //    Debug.Log("Retrieved fittest, fitness: " + fittestDataObject.GetData<float>());
        //}

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
        NormalizePopulationBreedingProbabilities();
        Car[] newPopulation = new Car[populationNumber];
        for (int i = 0; i < populationNumber; i++)
        {
            Car parent1 = PickRandom();
            Car parent2 = PickRandom();
            Debug.Log("P1: " + parent1.gameObject.name + ", P2: " + parent2.gameObject.name);

            DNA childDna = parent1.neuralNet.dna.Crossover(parent2.neuralNet.dna);
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
        Car fittest = null;
        float maxFitness = 0F;
        foreach (Car car in population)
        {
            if (car.fitness > maxFitness)
            {
                fittest = car;
                maxFitness = car.fitness;
            }
        }
        //SaveManager.GetInstance().SavePersistentData<DNA>(fittest.neuralNet.dna, SaveManager.FITTEST_DNA_DATA);
        //SaveManager.GetInstance().SavePersistentData<float>(maxFitness, SaveManager.BEST_FITNESS_DATA);

        Debug.Log("Max fitness: " + maxFitness);
        Car[] newPopulation = CrossoverPopulation();
        StartNewSimulation(newPopulation);
    }

    private void StartNewSimulation(Car[] newPopulation)
    {
        if (population == null)
        {
            population = newPopulation;
        }

        for (int i = 0; i < populationNumber; i++)
        {
            Destroy(population[i].gameObject);
            newPopulation[i].gameObject.SetActive(true);
            population[i] = newPopulation[i];
        }
        currentAlive = populationNumber;
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