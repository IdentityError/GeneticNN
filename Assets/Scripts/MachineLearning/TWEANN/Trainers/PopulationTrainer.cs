using GibFrame.Utils.Mathematics;
using System;
using static NodeGene;

public abstract class PopulationTrainer
{
    private Genotype predefinedGenotype;

    public Genotype GetPredefinedGenotype()
    {
        predefinedGenotype = new Genotype();
        for (int i = 0; i < 7; i++)
        {
            NodeGene newNode = new NodeGene(-i - 1, GMath.Tanh);
            if (i < 5)
            {
                newNode.Type = NodeType.INPUT;
            }
            else if (i > 7 - 2 - 1)
            {
                newNode.Type = NodeType.OUTPUT;
            }
            predefinedGenotype.AddNode(newNode);
        }

        int inn = 0;
        foreach (NodeGene input in predefinedGenotype.inputs)
        {
            foreach (NodeGene output in predefinedGenotype.outputs)
            {
                predefinedGenotype.AddLinkAndNodes(new LinkGene(input, output, UnityEngine.Random.Range(-1F, 1F), inn++));
            }
        }

        return predefinedGenotype;
    }

    public abstract Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[] Train(Biocenosis biocenosis);
}