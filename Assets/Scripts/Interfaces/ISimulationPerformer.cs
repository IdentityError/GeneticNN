using Assets.Scripts.TWEANN;

namespace Assets.Scripts.Interfaces
{
    /// <summary>
    ///   A simulation performer
    /// </summary>
    public interface ISimulationPerformer
    {
        void SetPopulationManager(PopulationManager populationManager);

        SimulationStats ProvideSimulationStats();

        void SetSimulationStats(SimulationStats stats);

        bool IsSimulating();

        void StopSimulating();
    }
}