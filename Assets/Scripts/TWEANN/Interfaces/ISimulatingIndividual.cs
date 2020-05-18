using Assets.Scripts.Interfaces;

namespace Assets.Scripts.TWEANN
{
    /// <summary>
    ///   An individual and a simulation performer
    /// </summary>
    public interface ISimulatingIndividual : IIndividual, ISimulationPerformer
    {
    }
}