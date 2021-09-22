using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biocenosis
{
    private readonly float speciesSharingThreshold;
    private readonly CrossoverOperatorsWrapper crossoverOperatorsWrapper;
    [SerializeField] private List<Specie> speciesList;

    public Biocenosis(float speciesSharingThreshold, CrossoverOperatorsWrapper wrapper)
    {
        this.crossoverOperatorsWrapper = wrapper;
        speciesList = new List<Specie>();
        this.speciesSharingThreshold = speciesSharingThreshold;
    }

    public void Speciate(Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism>[] population)
    {
        Reset();
        int lenght = population.Length;
        for (int i = 0; i < lenght; i++)
        {
            AddToSpeciesOrCreate(population[i]);
        }
        Purge(s => s.GetIndividualCount() <= 0);
    }

    public void AdjustSpecies()
    {
        int popSize = GetTotalIndividualNumber();
        int c = speciesList.Count;
        foreach (Specie species1 in speciesList)
        {
            if (species1.GetRawFitnessSum() / species1.GetIndividualCount() <= species1.LastGenAvgFitness)
            {
                species1.atRiskGenerations++;
            }
            else
            {
                species1.atRiskGenerations = 0;
            }
        }

        double sum = 0;
        foreach (Specie species in speciesList)
        {
            foreach (IOrganism individual in species.GetIndividuals())
            {
                individual.AdjustFitness((float)individual.ProvideRawFitness() / species.GetIndividualCount());
            }
            sum += species.GetAdjustedFitnessSum();
        }
        Purge(s => s.atRiskGenerations >= 3
        );

        int count = 0;
        foreach (Specie species in speciesList)
        {
            int val = Mathf.RoundToInt(Mathf.Floor((float)(species.GetAdjustedFitnessSum() * popSize / sum)));
            species.SetExpectedOffspringsCount(val);
            count += val;
        }
        int rest = popSize - count;
        Specie spec = GetFittestSpecies();
        spec.SetExpectedOffspringsCount(spec.GetExpectedOffpringsCount() + rest);
    }

    public Specie GetFittestSpecies()
    {
        Specie fittest = null;
        double val = 0;
        foreach (Specie current in speciesList)
        {
            double sum = current.GetAdjustedFitnessSum();
            if (sum > val)
            {
                fittest = current;
                val = sum;
            }
        }
        return fittest;
    }

    public int GetExpectedIndividualNumber()
    {
        int count = 0;
        foreach (Specie species in speciesList)
        {
            count += species.GetExpectedOffpringsCount();
        }
        return count;
    }

    public int GetTotalIndividualNumber()
    {
        int count = 0;
        foreach (Specie species in speciesList)
        {
            count += species.GetIndividualCount();
        }
        return count;
    }

    public List<Specie> GetSpeciesList()
    {
        return speciesList;
    }

    public IOrganism GetCurrentFittest()
    {
        double max = -1;
        IOrganism fittest = null;
        foreach (Specie species in speciesList)
        {
            IOrganism current = species.GetChamp();
            if (current != null && current.ProvideRawFitness() > max)
            {
                fittest = current;
                max = fittest.ProvideRawFitness();
            }
        }
        return fittest;
    }

    public double GetAverageFitness()
    {
        double avg = 0F;
        foreach (Specie species in speciesList)
        {
            avg += species.GetRawFitnessSum() / species.GetIndividualCount();
        }
        avg /= speciesList.Count;
        return avg;
    }

    public override string ToString()
    {
        return "Sharing threshold: " + speciesSharingThreshold + ", Number of species: " + speciesList.Count + ", Tot: " + GetExpectedIndividualNumber();
    }

    private void AddToSpeciesOrCreate(Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism> individual)
    {
        foreach (Specie species in speciesList)
        {
            if (species.Belongs(individual.Item2, speciesSharingThreshold))
            {
                species.AddToSpecies(individual.Item2);
                species.lastGenDescriptor.Add(individual);
                return;
            }
        }
        Specie newSpecies = new Specie(crossoverOperatorsWrapper);
        newSpecies.AddToSpecies(individual.Item2);
        newSpecies.lastGenDescriptor.Add(individual);
        AddSpecies(newSpecies);
    }

    private void AddSpecies(Specie species)
    {
        if (!speciesList.Contains(species))
        {
            speciesList.Add(species);
        }
    }

    private void Reset()
    {
        foreach (Specie species in speciesList)
        {
            species.Reset();
        }
    }

    private void Purge(Predicate<Specie> match)
    {
        speciesList.RemoveAll(match);
    }
}