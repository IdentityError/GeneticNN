using Assets.Scripts.TUtils.Utils;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    [System.Serializable]
    /// <summary>
    ///   The Uniform Crossover operator selects each matching gene with an equal probability between the two parents. Disjoint genes are
    ///   all imported from the fittest parent
    /// </summary>
    public class UniformCrossoverOperator : CrossoverOperator
    {
        public override Genotype Apply(IOrganism first, IOrganism second)
        {
            Genotype childGen = new Genotype();

            Genotype firstGen = first.ProvideNeuralNet().GetGenotype();
            Genotype secondGen = second.ProvideNeuralNet().GetGenotype();

            List<LinkGene> remaining = new List<LinkGene>(firstGen.links);
            List<LinkGene> partnerRemaining = new List<LinkGene>(secondGen.links);
            // Zip togheter the links that have the same innovation number
            List<Tuple<LinkGene, LinkGene>> zippedLinks = TUtilsProvider.ZipWithPredicate(firstGen.links, secondGen.links, (item1, item2) => item1.GetInnovationNumber().Equals(item2.GetInnovationNumber()));

            //Add to che child all the matching genes(links)
            foreach (Tuple<LinkGene, LinkGene> gene in zippedLinks)
            {
                LinkGene copy;
                if (/*thisFitness > partnerFitness*/UnityEngine.Random.Range(0F, 1F) < 0.5F)
                {
                    copy = gene.Item1;
                }
                else
                {
                    copy = gene.Item2;
                }

                childGen.AddLinkAndNodes(copy);
                remaining.RemoveAll(item => item.GetInnovationNumber().Equals(copy.GetInnovationNumber()));
                partnerRemaining.RemoveAll(item => item.GetInnovationNumber().Equals(copy.GetInnovationNumber()));
            }

            // At this point all common genes are added we add all the disjoint genes from the fittest
            if (first.ProvideAdjustedFitness() > second.ProvideAdjustedFitness())
            {
                foreach (LinkGene gene in remaining)
                {
                    childGen.AddLinkAndNodes(gene);
                }
            }
            else
            {
                foreach (LinkGene gene in partnerRemaining)
                {
                    childGen.AddLinkAndNodes(gene);
                }
            }

            return childGen;
        }
    }
}