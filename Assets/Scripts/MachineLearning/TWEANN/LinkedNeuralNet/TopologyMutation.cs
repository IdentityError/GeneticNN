using System;

namespace Assets.Scripts.MachineLearning.TWEANN
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

        /// <summary>
        ///   Set the innovation number of this mutation
        /// </summary>
        /// <param name="innovationNumber"> </param>
        public void SetInnovationNumber(int innovationNumber)
        {
            this.innovationNumber = innovationNumber;
        }

        /// <summary>
        ///   Retrieve the innovation number of this mutation
        /// </summary>
        /// <returns> </returns>
        public int GetInnovationNumber()
        {
            return innovationNumber;
        }

        /// <summary>
        ///   Override of the Equals function
        /// </summary>
        /// <param name="other"> </param>
        /// <returns> </returns>
        public bool Equals(TopologyMutation other)
        {
            return type == other.type && linkInvolved.Equals(other.linkInvolved);
        }
    }
}