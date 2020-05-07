using Assets.Scripts.Stores;
using Assets.Scripts.Trainers;

namespace Assets.Scripts.Providers
{
    [System.Serializable]
    public class TrainerProvider
    {
        public Paradigms.TrainingParadigm trainingParadigm;

        public GATrainer gaTrainer = new GATrainer();
        public BPATrainer bpaTrainer = new BPATrainer();

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