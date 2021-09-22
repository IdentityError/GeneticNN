public interface ISimulationPerformer
{
    SimulationStats ProvideSimulationStats();

    bool IsSimulating();
}