namespace Assets.Scripts.MachineLearning.Interfaces
{
    /// <summary>
    ///   A simulation performer
    /// </summary>
    public interface ISimulationPerformer
    {
        /// <summary>
        ///   Provide the Simulation stats of this performer
        /// </summary>
        /// <returns> </returns>
        SimulationStats ProvideSimulationStats();

        /// <summary>
        ///   True if the performer is still simulating
        /// </summary>
        /// <returns> </returns>
        bool IsSimulating();

        /// <summary>
        ///   Stop the simulation for this performer
        /// </summary>
        void StopSimulating();
    }
}