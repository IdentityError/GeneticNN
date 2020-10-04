using Assets.Scripts.TUtils.Interfaces;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    public interface IOrganism : INeuralNetHolder, IProbSelectable
    {
        /// <summary>
        ///   Set the Population manager of this performer
        /// </summary>
        /// <param name="populationManager"> </param>
        void SetPopulationManager(PopulationManager populationManager);

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