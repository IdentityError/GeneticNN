using UnityEngine;

namespace Assets.Scripts.Trainers
{
    public abstract class Trainer
    {
        protected DNA[] dnaPopulation;
        protected int populationNumber;

        [Space(5)]
        [Header("Predefined Topology")]
        [SerializeField] private int inputCount;
        [SerializeField] private int outputCount;
        [SerializeField] private int[] hiddenLayers = null;

        public DNA.DnaTopology GetPredefinedTopology()
        {
            int[] temp = new int[hiddenLayers.Length + 2];
            hiddenLayers.CopyTo(temp, 1);
            temp[0] = inputCount;
            temp[temp.Length - 1] = outputCount;
            return new DNA.DnaTopology(temp);
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
            this.Train();
            return this.dnaPopulation;
        }

        protected abstract void Train();
    }
}