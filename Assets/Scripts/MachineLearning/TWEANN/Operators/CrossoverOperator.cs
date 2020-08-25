using Assets.Scripts.TUtils.Interfaces;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    [System.Serializable]
    /// <summary>
    ///   Represent the abstract class of a generic Crossover operator
    /// </summary>
    public abstract class CrossoverOperator : IProbSelectable
    {
        private float selectProbability;
        private float currentProgression;

        public CrossoverOperator()
        {
        }

        public abstract Genotype Apply(IOrganism first, IOrganism second);

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

        public float GetCurrentProgression()
        {
            return currentProgression;
        }
    }
}