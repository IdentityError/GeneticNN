using Assets.Scripts.NeuralNet;
using Assets.Scripts.TUtils.Utils;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.TWEANN
{
    [System.Serializable]
    /// <summary>
    ///   The K point Crossover operator splits the matching genes in k sub strings and copied them alternately into the child dna. Disjoint
    ///   genes are all imported from the fittest parent
    /// </summary>
    public class KPointsCrossoverOperator : CrossoverOperator
    {
        public override Genotype Apply(IIndividual first, IIndividual second)
        {
            Genotype childGen = new Genotype();

            Genotype firstGen = first.ProvideNeuralNet().GetGenotype();
            Genotype secondGen = second.ProvideNeuralNet().GetGenotype();

            List<LinkGene> remaining = new List<LinkGene>(firstGen.links);
            List<LinkGene> partnerRemaining = new List<LinkGene>(secondGen.links);
            // Zip togheter the links that have the same innovation number
            List<Tuple<LinkGene, LinkGene>> zippedLinks = TUtilsProvider.ZipWithPredicate(firstGen.links, secondGen.links, (item1, item2) => item1.GetInnovationNumber().Equals(item2.GetInnovationNumber()));

            int stride = zippedLinks.Count / UnityEngine.Random.Range(1, zippedLinks.Count);
            //Add to che child all the matching genes(links)
            int i = 0;
            bool importFromFirst = true;
            foreach (Tuple<LinkGene, LinkGene> gene in zippedLinks)
            {
                LinkGene copy;
                if (++i % stride == 0)
                {
                    importFromFirst = !importFromFirst;
                }
                if (importFromFirst)
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