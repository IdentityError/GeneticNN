using GibFrame.Extensions;
using System;
using System.Collections.Generic;

[System.Serializable]
public class AverageCrossoverOperator : CrossoverOperator
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

        //Add to che child all the matching genes(links)
        foreach (Tuple<LinkGene, LinkGene> gene in zippedLinks)
        {
            LinkGene copy = new LinkGene(gene.Item1.Source, gene.Item1.Destination, (gene.Item1.Weight + gene.Item2.Weight) / 2F, gene.Item1.InnovationNumber);
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