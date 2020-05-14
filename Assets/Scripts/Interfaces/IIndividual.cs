using Assets.Scripts.TWEANN;

namespace Assets.Scripts.Interfaces
{
    public interface IIndividual : INeuralNetHolder
    {
        double ProvideFitness();

        void SetFitness(double fitness);

        double ProvidePickProbability();

        void SetPickProbability(double pickProbability);

        Species ProvideSpecies();
    }
}