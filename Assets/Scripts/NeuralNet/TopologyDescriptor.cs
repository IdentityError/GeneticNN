using System.Collections.Generic;

namespace Assets.Scripts.NeuralNet
{
    [System.Serializable]
    public struct LinkDescriptor
    {
        public LinkDescriptor(int fromId, int toId)
        {
            this.fromId = fromId;
            this.toId = toId;
        }

        public int fromId;
        public int toId;
    }

    [System.Serializable]
    public class TopologyDescriptor
    {
        public int inputCount;
        public int outputCount;
        public int hiddenCount;
        public List<LinkDescriptor> links = new List<LinkDescriptor>();

        public int NodeCount
        {
            get => inputCount + hiddenCount + outputCount;
        }
    }
}