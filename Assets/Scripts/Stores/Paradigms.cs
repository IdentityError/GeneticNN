namespace Assets.Scripts.Stores
{
    public class Paradigms
    {
        public enum TrainingParadigm { GENETIC, BPA }

        public enum MutationParadigm { TOPOLOGY, WEIGHTS, HYBRID }

        public enum MissingWeightsFillParadigm { COPY, RANDOM, HYBRID }

        public enum TopologyMutationType { HIDDEN_LAYER_ADD, NEURONS_NUMBER_CHANGE, HIDDEN_LAYER_REMOVE }
    }
}