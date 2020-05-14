namespace Assets.Scripts.NeuralNet
{
    public class LinkGene
    {
        private NodeGene from;
        private NodeGene to;
        public double weight;
        private int innovationNumber;

        public LinkGene(NodeGene from, NodeGene to, double weight) : this(from, to, weight, 0)
        {
        }

        public LinkGene(NodeGene from, NodeGene to) : this(from, to, 0D)
        {
        }

        public LinkGene(NodeGene from, NodeGene to, double weight, int innovationNumber)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
            this.innovationNumber = innovationNumber;
        }

        public NodeGene From()
        {
            return from;
        }

        public NodeGene To()
        {
            return to;
        }

        public LinkGene Copy()
        {
            return new LinkGene(from.Copy(), to.Copy(), weight, innovationNumber);
        }

        public int GetInnovationNumber()
        {
            return innovationNumber;
        }
    }
}