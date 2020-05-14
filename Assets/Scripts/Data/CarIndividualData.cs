using Assets.Scripts.NeuralNet;

[System.Serializable]
public class CarIndividualData
{
    private Genotype genotype;
    private double fitness;

    public CarIndividualData(Genotype genotype, double fitness)
    {
        this.genotype = genotype;
        this.fitness = fitness;
    }

    public Genotype GetGenotype()
    {
        return this.genotype;
    }

    public double GetFitness()
    {
        return this.fitness;
    }
}