using Assets.Scripts.NeuralNet;
using Assets.Scripts.TUtils.Interfaces;

namespace Assets.Scripts.TWEANN
{
    /// <summary>
    ///   Represent the abstract class of a generic Crossover operator
    /// </summary>
    public abstract class CrossoverOperator : IProbSelectable
    {
        private float selectProbability;
        public float currentProgression;

        public CrossoverOperator()
        {
        }

        public abstract Genotype Apply(IIndividual first, IIndividual second);

        public void SetSelectProbability(float probability)
        {
            selectProbability = probability;
        }

        public void UpdateSelectProbability(float selectProbability)
        {
            this.selectProbability = selectProbability;
        }

        public float ProvideSelectProbability()
        {
            return selectProbability;
        }

        public void SetCurrentProgression(float currentProgression)
        {
            this.currentProgression = currentProgression;
        }
    }
}