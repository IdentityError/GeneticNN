using Assets.Scripts.Stores;
using UnityEngine;

namespace Assets.Scripts.Trainers
{
    [System.Serializable]
    public class GATrainer : Trainer
    {
        [SerializeField] private Paradigms.MutationParadigm mutationParadigm;
        [Space(5)]
        [Header("Parameters")]
        [Range(0, 1)]
        [SerializeField] private float mutationPercentage;

        protected override void Train()
        {
            Selection();
            Crossover();
            Mutation();
        }

        private void Selection()
        {
        }

        private void Crossover()
        {
        }

        private void Mutation()
        {
        }
    }
}