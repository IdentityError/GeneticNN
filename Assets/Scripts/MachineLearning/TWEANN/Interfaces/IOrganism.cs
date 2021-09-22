using GibFrame.Utils;

public interface IOrganism : INeuralNetHolder, IProbSelectable
{
    double ProvideAdjustedFitness();

    double ProvideRawFitness();

    void SetRawFitness(double rawFitness);

    void AdjustFitness(double fitness);

    double EvaluateFitnessFunction();
}