using Assets.Scripts.Managers;

namespace Assets.Scripts.Interfaces
{
    public interface IIndividual : IDNAHolder
    {
        void SetPopulationManager(PopulationManager populationManager);

        SimulationStats ProvideStats();
    }
}