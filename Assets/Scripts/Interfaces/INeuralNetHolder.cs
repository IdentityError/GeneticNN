using Assets.Scripts.NeuralNet;

namespace Assets.Scripts.Interfaces
{
    public interface INeuralNetHolder
    {
        NeuralNetwork ProvideNeuralNet();

        void SetNeuralNet(NeuralNetwork net);
    }
}