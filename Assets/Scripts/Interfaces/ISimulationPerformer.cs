using Assets.Scripts.TWEANN;

namespace Assets.Scripts.Interfaces
{
    public interface ISimulationPerformer
    {
        void SetPopulationManager(PopulationManager populationManager);

        SimulationStats ProvideSimulationStats();

        void SetSimulationStats(SimulationStats stats);
    }
}