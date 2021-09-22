public interface INeuralNetHolder
{
    NeuralNetwork ProvideNeuralNet();

    void SetNeuralNet(NeuralNetwork net);
}