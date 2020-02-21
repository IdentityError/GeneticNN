using UnityEngine;

public class TrainingManager : Manager
{
    [Header("Training")]
    public int populationNumber;
    [Range(0F, 1F)]
    public float mutationRate;

    protected CarIndividual.SimulationStats bestStats;
    protected int generationCount = 0;
    protected CarIndividual[] population = null;
    protected int currentAlive;
    protected float currentGenerationMaxFitness = 0F;
    protected float maxFitness = 0F;

    public virtual void CalculateFitness(CarIndividual individual)
    {
        return;
    }

    public virtual void HasEndedSimulation(CarIndividual individual)
    {
        return;
    }

    protected override CarIndividual InstantiateAndInitializeIndividual(DNA dna, string name)
    {
        CarIndividual newCar = base.InstantiateAndInitializeIndividual(dna, name);
        newCar.manager = this;
        return newCar;
    }
}