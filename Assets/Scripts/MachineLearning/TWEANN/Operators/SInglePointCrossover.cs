using GibFrame.Extensions;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SinglePointCrossover : CrossoverOperator
{
    public override Genotype Apply(IOrganism first, IOrganism second)
    {
        Genotype childGen = new Genotype();

        Genotype firstGen = first.ProvideNeuralNet().GetGenotype();
        Genotype secondGen = second.ProvideNeuralNet().GetGenotype();

        List<LinkGene> remaining = new List<LinkGene>(firstGen.links);
        List<LinkGene> partnerRemaining = new List<LinkGene>(secondGen.links);
        // Zip togheter the links that have the same innovation number
        List<Tuple<LinkGene, LinkGene>> zippedLinks = firstGen.links.ZipWithFirstPredicateMatching(secondGen.links, (item1, item2) => item1.InnovationNumber.Equals(item2.InnovationNumber));

        int crossoverPoint = UnityEngine.Random.Range(0, zippedLinks.Count);
        //Add to che child all the matching genes(links)
        int i = 0;
        foreach (Tuple<LinkGene, LinkGene> gene in zippedLinks)
        {
            LinkGene copy;
            if (i++ < crossoverPoint)
            {
                copy = gene.Item1;
            }
            else
            {
                copy = gene.Item2;
            }

            childGen.AddLinkAndNodes(copy);
            remaining.RemoveAll(item => item.InnovationNumber.Equals(copy.InnovationNumber));
            partnerRemaining.RemoveAll(item => item.InnovationNumber.Equals(copy.InnovationNumber));
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