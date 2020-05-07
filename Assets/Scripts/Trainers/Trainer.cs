using UnityEngine;

namespace Assets.Scripts.Trainers
{
    public abstract class Trainer
    {
        protected DNA[] dnaPopulation;

        [SerializeField] private DNA.DnaTopology predefinedTopology = null;

        public DNA.DnaTopology GetPredefinedTopology()
        {
            if(predefinedTopology == null)
            {
                predefinedTopology = new DNA.DnaTopology(1, 1);
            }
            return predefinedTopology;
        }

        /// <summary>
        ///   Train and return a new population in place
        /// </summary>
        /// <param name="nnPopulation"> </param>
        /// <returns> </returns>
        public DNA[] Train(DNA[] nnPopulation)
        {
            this.dnaPopulation = nnPopulation;
            this.Train();
            return nnPopulation;
        }

        protected abstract void Train();
    }
}