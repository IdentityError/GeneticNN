namespace Assets.Scripts.NeuralNet
{
    public enum MutationType { SPLIT_LINK, ADD_LINK }

    public class Mutation
    {
        private MutationType type;
        private int innovationNumber;

        private LinkGene linkInvolved;

        public override bool Equals(object obj)
        {
            if (obj is Mutation)
            {
                Mutation other = (Mutation)obj;
                return type == other.type && innovationNumber == other.innovationNumber && linkInvolved.Equals(other.linkInvolved);
            }
            else
            {
                return base.Equals(obj);
            }
        }
    }
}