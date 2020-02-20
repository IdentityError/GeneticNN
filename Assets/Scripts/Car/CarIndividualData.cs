[System.Serializable]
public class CarIndividualData
{
    private DNA dna;
    private float fitness;

    public CarIndividualData(DNA dna, float fitness)
    {
        this.dna = dna;
        this.fitness = fitness;
    }

    public DNA GetDNA()
    {
        return this.dna;
    }

    public float GetFitness()
    {
        return this.fitness;
    }
}