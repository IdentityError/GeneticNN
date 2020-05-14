using Assets.Scripts.NeuralNet;

namespace Assets.Scripts.TWEANN
{
    public class Individual
    {
        protected float fitness;
        protected Species species;
        protected NeuralNetwork neuralNet;
        protected PopulationManager populationManager;
    }
}