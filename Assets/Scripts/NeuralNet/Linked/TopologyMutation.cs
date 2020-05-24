using System;

namespace Assets.Scripts.NeuralNet
{
    public enum TopologyMutationType { SPLIT_LINK, ADD_LINK }

    public class TopologyMutation : IEquatable<TopologyMutation>
    {
        private int innovationNumber;
        private LinkGene linkInvolved;
        private TopologyMutationType type;

        public TopologyMutation(TopologyMutationType type, LinkGene linkInvolved, int innovationNumber) : this(type, linkInvolved)
        {
            this.innovationNumber = innovationNumber;
        }

        public TopologyMutation(TopologyMutationType type, LinkGene linkInvolved)
        {
            this.type = type;
            this.linkInvolved = linkInvolved;
        }

        public bool Equals(TopologyMutation other)
        {
            return type == other.type && linkInvolved.Equals(other.linkInvolved);
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