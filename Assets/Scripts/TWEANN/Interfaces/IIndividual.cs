using Assets.Scripts.Interfaces;

namespace Assets.Scripts.TWEANN
{
    /// <summary>
    ///   Individual holding a Neural Network and a fitness
    /// </summary>
    public interface IIndividual : INeuralNetHolder
    {
        double ProvideFitness();

        void SetFitness(double fitness);

        double EvaluateFitnessFunction();
    }
}