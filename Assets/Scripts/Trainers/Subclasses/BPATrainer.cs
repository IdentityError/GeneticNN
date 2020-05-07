using System;
using UnityEngine;

namespace Assets.Scripts.Trainers
{
    [System.Serializable]
    public class BPATrainer : Trainer
    {
        [SerializeField] private float learningRate;

        protected override void Train()
        {
            throw new NotImplementedException();
        }
    }
}