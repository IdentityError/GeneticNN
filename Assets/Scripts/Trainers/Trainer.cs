using UnityEngine;

namespace Assets.Scripts.Trainers
{
    public abstract class Trainer
    {
        protected DNA[] dnaPopulation;
        protected int populationNumber;

        [SerializeField] private DNA.DnaTopology predefinedTopology = null;

        public DNA.DnaTopology GetPredefinedTopology()
        {
            if (predefinedTopology == null)
            {
                predefinedTopology = new DNA.DnaTopology(1, new int[] { 1 });
            }
            return predefinedTopology;
        }

        /// <summary>
        ///   Train and return a new population in place
        /// </summary>
        /// <returns> </returns>
        public DNA[] Train(DNA[] dnaPopulation)
        {
            populationNumber = dnaPopulation.Length;
            this.dnaPopulation = new DNA[populationNumber];
            dnaPopulation.CopyTo(this.dnaPopulation, 0);
            this.dnaPopulation = dnaPopulation;
            this.Train();
            return this.dnaPopulation;
        }

        protected abstract void Train();
    }
}