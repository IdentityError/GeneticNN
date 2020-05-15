namespace Assets.Scripts.NeuralNet
{
    public enum TopologyMutationType { SPLIT_LINK, ADD_LINK }

    public class TopologyMutation
    {
        private TopologyMutationType type;
        private int innovationNumber;

        private LinkGene linkInvolved;

        public TopologyMutation(TopologyMutationType type, LinkGene linkInvolved, int innovationNumber) : this(type, linkInvolved)
        {
            this.innovationNumber = innovationNumber;
        }

        public TopologyMutation(TopologyMutationType type, LinkGene linkInvolved)
        {
            this.type = type;
            this.linkInvolved = linkInvolved;
        }

        public override bool Equals(object obj)
        {
            if (obj is TopologyMutation)
            {
                TopologyMutation other = (TopologyMutation)obj;
                return type == other.type && linkInvolved.Equals(other.linkInvolved);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public int GetInnovationNumber()
        {
            return innovationNumber;
        }

        public void SetInnovationNumber(int innovationNumber)
        {
            this.innovationNumber = innovationNumber;
        }
    }
}