using Assets.Scripts.Stores;

namespace Assets.Scripts.TWEANN
{
    [System.Serializable]
    public class PopulationTrainerProvider
    {
        public Paradigms.TrainingParadigm trainingParadigm;

        public TrainerNEAT gaTrainer = new TrainerNEAT();

        public PopulationTrainer ProvideTrainer()
        {
            switch (trainingParadigm)
            {
                case Paradigms.TrainingParadigm.GENETIC:
                    return gaTrainer;

                default:
                    return null;
            }
        }
    }
}