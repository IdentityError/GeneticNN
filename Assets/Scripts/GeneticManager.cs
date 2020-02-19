using UnityEngine;

public class GeneticManager : MonoBehaviour
{
    [Header("References")]
    public GameObject carIndividualPrefab;

    public int populationNumber;
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

    private Car[] CrossoverPopulation()
    {
        NormalizePopulationBreedingProbabilities();
        Car[] newPopulation = new Car[populationNumber];
        for (int i = 0; i < populationNumber; i++)
        {
            Car parent1 = PickRandom();
            Car parent2 = PickRandom();

            DNA childDna = parent1.neuralNet.dna.Crossover(parent2.neuralNet.dna);
            childDna.Mutate(mutationRate);
            Car child = Instantiate(carIndividualPrefab, new Vector3(0F, 0.75F, 0F), Quaternion.identity).GetComponent<Car>();
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
            GameObject individual = Instantiate(carIndividualPrefab, new Vector3(0F, 0.75F, 0F), Quaternion.identity);
            Car car = individual.GetComponent<Car>();
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
        Debug.Log("Crossovering population");
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
            car.breedingProbability = fitnessSum / fitnessSum;
        }
    }
}