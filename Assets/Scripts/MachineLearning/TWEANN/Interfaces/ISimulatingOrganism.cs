using Assets.Scripts.MachineLearning.Interfaces;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    /// <summary>
    ///   An individual and a simulation performer
    /// </summary>
    public interface ISimulatingOrganism : ISimulationPerformer, IOrganism
    {
    }
}