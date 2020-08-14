using Assets.Scripts.Stores;

namespace Assets.Scripts.TWEANN
{
    [System.Serializable]
    public class PopulationTrainerProvider
    {
        public Paradigms.TrainingParadigm trainingParadigm;

        public TrainerNEAT gaTrainer;

        /// <summary>
        ///   Provide the current set Trainer
        /// </summary>
        /// <returns> </returns>
        public PopulationTrainer ProvideTrainer()
        {
            switch (trainingParadigm)
            {
                case Paradigms.TrainingParadigm.NEAT:
                    return gaTrainer;

                default:
                    return null;
            }
        }
    }
}