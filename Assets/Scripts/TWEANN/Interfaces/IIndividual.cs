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
        double ProvideAdjustedFitness();

        /// <summary>
        ///   Retrieve the not adjusted fitness
        /// </summary>
        /// <returns> </returns>
        double ProvideRawFitness();

        /// <summary>
        ///   Set the raw value of the fitness
        /// </summary>
        /// <returns> </returns>
        void SetRawFitness(double rawFitness);

        /// <summary>
        ///   Set the adjusted fitness of this individual
        /// </summary>
        /// <param name="fitness"> </param>
        void AdjustFitness(double fitness);

        /// <summary>
        ///   Fitness function
        /// </summary>
        /// <returns> </returns>
        double EvaluateFitnessFunction();
    }
}