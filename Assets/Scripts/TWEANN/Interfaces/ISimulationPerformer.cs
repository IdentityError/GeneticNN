using Assets.Scripts.TWEANN;

namespace Assets.Scripts.Interfaces
{
    /// <summary>
    ///   A simulation performer
    /// </summary>
    public interface ISimulationPerformer
    {
        /// <summary>
        ///   Set the Population manager of this performer
        /// </summary>
        /// <param name="populationManager"> </param>
        void SetPopulationManager(PopulationManager populationManager);

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