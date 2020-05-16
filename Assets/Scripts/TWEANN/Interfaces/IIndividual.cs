using Assets.Scripts.Interfaces;

namespace Assets.Scripts.TWEANN
{
    public interface IIndividual : INeuralNetHolder
    {
        double ProvideFitness();

        void SetFitness(double fitness);

        double ProvidePickProbability();

        void SetPickProbability(double pickProbability);
    }
}