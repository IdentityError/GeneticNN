using UnityEngine;

namespace Assets.Scripts.Trainers
{
    public abstract class Trainer
    {
        protected DNA[] nnPopulation;

        public DNA.DnaTopology predefinedTopology
        {
            get => predefinedTopology;
        }

        /// <summary>
        ///   Train and return a new population in place
        /// </summary>
        /// <param name="nnPopulation"> </param>
        /// <returns> </returns>
        public DNA[] Train(DNA[] nnPopulation)
        {
            this.nnPopulation = nnPopulation;
            this.Train();
            return nnPopulation;
        }

        protected abstract void Train();
    }
}