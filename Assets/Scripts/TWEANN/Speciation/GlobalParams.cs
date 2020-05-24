using System.Collections.Generic;

namespace Assets.Scripts.NeuralNet
{
    public static class GlobalParams
    {
        private static List<TopologyMutation> generationMutations = new List<TopologyMutation>();
        private static int globalInnovationNumber = 1;

        public static int GetGenerationInnovationNumber(TopologyMutation mutation)
        {
            TopologyMutation res;
            if (mutation == null) return globalInnovationNumber++;
            if ((res = generationMutations.Find(mut => mut.Equals(mutation))) != null)
            {
                return res.GetInnovationNumber();
            }
            else
            {
                generationMutations.Add(mutation);
                return globalInnovationNumber++;
            }
        }

        public static void InitializeGlobalInnovationNumber(TopologyDescriptor descriptor)
        {
            globalInnovationNumber = descriptor.links.Count + 1;
        }

        public static void ResetGenerationMutations()
        {
            generationMutations.Clear();
        }
    }
}