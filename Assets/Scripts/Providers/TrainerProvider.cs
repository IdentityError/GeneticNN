using Assets.Scripts.Stores;
using Assets.Scripts.Trainers;
using UnityEngine;

namespace Assets.Scripts.Providers
{
    [System.Serializable]
    public class TrainerProvider
    {
        public Paradigms.TrainingParadigm trainingParadigm;

        public GATrainer gaTrainer;
        public BPATrainer bpaTrainer;

        public Trainer ProvideTrainer()
        {
            switch (trainingParadigm)
            {
                case Paradigms.TrainingParadigm.BPA:
                    return bpaTrainer;

                case Paradigms.TrainingParadigm.GENETIC:
                    return gaTrainer;

                default:
                    return null;
            }
        }
    }
}