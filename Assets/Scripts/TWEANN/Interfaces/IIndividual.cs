using Assets.Scripts.Interfaces;

namespace Assets.Scripts.TWEANN
{
    /// <summary>
    ///   Individual holding a Neural Network and a fitness
    /// </summary>
    public interface IIndividual : INeuralNetHolder
    {
        /// <summary>
        ///   Retrieve the fitness
        /// </summary>
        /// <returns> </returns>
        double ProvideFitness();

        /// <summary>
        ///   Set the fitness of this individual
        /// </summary>
        /// <param name="fitness"> </param>
        void SetFitness(double fitness);

        /// <summary>
        ///   Fitness function
        /// </summary>
        /// <returns> </returns>
        double EvaluateFitnessFunction();
    }
}