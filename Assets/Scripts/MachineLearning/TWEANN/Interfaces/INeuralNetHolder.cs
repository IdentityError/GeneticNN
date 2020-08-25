namespace Assets.Scripts.MachineLearning.TWEANN
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