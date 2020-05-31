using Assets.Scripts.NeuralNet;

namespace Assets.Scripts.Interfaces
{
    public interface INeuralNetHolder
    {
        /// <summary>
        ///   Retrieve the Neural Net
        /// </summary>
        /// <returns> </returns>
        NeuralNetwork ProvideNeuralNet();

        /// <summary>
        ///   Set the Neural Net
        /// </summary>
        /// <param name="net"> </param>
        void SetNeuralNet(NeuralNetwork net);
    }
}