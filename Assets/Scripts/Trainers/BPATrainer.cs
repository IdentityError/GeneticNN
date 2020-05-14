using System;
using UnityEngine;

namespace Assets.Scripts.Trainers
{
    [System.Serializable]
    public class BPATrainer
    {
        [SerializeField] private float learningRate;

        protected void Train()
        {
            throw new NotImplementedException();
        }
    }
}