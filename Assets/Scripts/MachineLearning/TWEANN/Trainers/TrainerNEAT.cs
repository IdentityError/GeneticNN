using Assets.Scripts.TUtils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    [System.Serializable]
    public class TrainerNEAT : PopulationTrainer
    {
        private DescriptorsWrapper.TrainerPreferences preferences;

        /// <summary>
        ///   If dynamic mutation rate is not enabled, use a static mutation rate indicated by the max mutation rate
        /// </summary>
        /// <param name="wrapper"> </param>
        /// <param name="minCrossoverRatio"> </param>
        /// <param name="maxAchievableFitness"> </param>
        /// <param name="dynamicMutationRate"> </param>
        /// <param name="maxMutationRate"> </param>
        public TrainerNEAT(DescriptorsWrapper.TrainerPreferences preferences)
        {
            //Debug.Log("TrainerNEAT initialized:\nDynamic mutation rate: " + dynamicMutationRate + "\nMax mutation rate: " + maxMutationRate + "\nMin crossover ratio: " + minCrossoverRatio + "\nMax achievable fitness: " + maxAchievableFitness);
            this.preferences = preferences;
        }

        public override Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[] Train(Biocenosis biocenosis)
        {
            // Reset the generation mutations list
            GlobalParams.ResetGenerationMutations();

            // Adjust species offprings' numbers
            biocenosis.AdjustSpecies();

            int expectedIndividualCount = biocenosis.GetExpectedIndividualNumber();
            // Array containing each new genotype and the crossover operation used for each
            Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[] pop = new Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[expectedIndividualCount];

            CrossoverOperator operatorToApply = null;
            int currentIndex = 0;

            foreach (Specie current in biocenosis.GetSpeciesList())
            {
                double maxFitness = current.GetChamp().ProvideRawFitness();
                double averageFitness = current.GetRawFitnessSum() / current.GetIndividualCount();
                if (preferences.dynamicRates)
                {
                    // Dynamically set the mutation rate for the current species
                    current.rates.weightMutationRate = (float)(preferences.maxWeightMutationRate * Math.Pow((averageFitness / maxFitness), 2.5D));
                    current.rates.weightMutationRate *= (1F - (float)(averageFitness / preferences.maxAchievableFitness));
                    current.rates.splitLinkRate = (float)(preferences.maxSplitLinkRate * Math.Pow((averageFitness / maxFitness), 2D));
                    current.rates.splitLinkRate *= (1F - (float)(averageFitness / preferences.maxAchievableFitness));
                    current.rates.addLinkRate = (float)(preferences.maxAddLinkRate * Math.Pow((averageFitness / maxFitness), 2D));
                    current.rates.addLinkRate *= (1F - (float)(averageFitness / preferences.maxAchievableFitness));
                    //rates.crossoverRatio = (1F - Mathf.Pow(((float)(averageFitness / preferences.maxAchievableFitness)), 3.75F));
                    //rates.crossoverRatio *= (1F - (float)(averageFitness / preferences.maxAchievableFitness));
                    current.rates.crossoverRatio = 1F - TMath.Avg(current.rates.weightMutationRate, current.rates.splitLinkRate, current.rates.addLinkRate);
                }
                else
                {
                    current.rates.weightMutationRate = preferences.maxWeightMutationRate;
                    current.rates.splitLinkRate = preferences.maxSplitLinkRate;
                    current.rates.addLinkRate = preferences.maxAddLinkRate;
                    current.rates.crossoverRatio = 1F;
                }
                //Debug.Log("WM: " + rates.weightMutationRate);
                // Order the individuals based on their fitnesses
                current.individuals = current.individuals.OrderByDescending(x => x.ProvideRawFitness()).ToList();
                List<IOrganism> top = GetMatingSubPopulationInSpecies(current, 0.2F);
                // Select the top two organisms
                (IOrganism, IOrganism) parents = SelectParents(top);

                //Debug.Log("CR: " + rates.crossoverRatio);
                // Create a number of offsprings as expected by the specie
                for (int i = 0; i < current.GetExpectedOffpringsCount(); i++)
                {
                    Genotype childGen = null;
                    operatorToApply = current.operatorsWrapper.GetRandomOperator();
                    if (operatorToApply == null)
                    {
                        // If not applying crossover, copy the genotype of the fittest parent (reinserting the same individual in the new population)
                        if (parents.Item1.ProvideRawFitness() > parents.Item2.ProvideRawFitness())
                        {
                            childGen = parents.Item1.ProvideNeuralNet().GetGenotype().Copy();
                        }
                        else
                        {
                            childGen = parents.Item2.ProvideNeuralNet().GetGenotype().Copy();
                        }
                    }
                    else
                    {
                        // Apply crossover operator
                        childGen = operatorToApply.Apply(parents.Item1, parents.Item2);
                    }

                    // Mutate the child genotype based on the dynamic mutation rate
                    childGen.Mutate(current.rates);
                    //Debug.Log("species: " + current.GetIndividualCount() + ", exp: " + current.GetExpectedOffpringsCount() + ": " + currentIndex);
                    // Add the currently created genotype and its crossover operation descriptor to the array
                    pop[currentIndex++] = new Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>(
                        new DescriptorsWrapper.CrossoverOperationDescriptor(parents.Item1.ProvideRawFitness(), parents.Item2.ProvideRawFitness(), operatorToApply),
                        childGen);
                }
            }

            return pop;
        }

        public List<IOrganism> GetMatingSubPopulationInSpecies(Specie species, float percentage)
        {
            int number = Mathf.CeilToInt(percentage * species.GetIndividualCount());
            return species.individuals.GetRange(0, number);
        }

        /// <summary>
        ///   Perform selection from a specified species
        /// </summary>
        /// <param name="species"> </param>
        /// <returns> </returns>
        private (IOrganism, IOrganism) SelectParents(List<IOrganism> organisms)
        {
            if (organisms.Count == 1)
            {
                IOrganism org = organisms[0];
                return (org, org);
            }
            else if (!preferences.enhancedSelectionOperator)
            {
                TUtilsProvider.NormalizeProbabilities(organisms, (o) => (float)o.ProvideRawFitness());
                IOrganism first = TUtilsProvider.SelectWithProbability(organisms);
                organisms.Remove(first);
                TUtilsProvider.NormalizeProbabilities(organisms, (o) => (float)o.ProvideRawFitness());
                IOrganism second = TUtilsProvider.SelectWithProbability(organisms);
                return (first, second);
            }
            else
            {
                if (organisms.Count > 1)
                {
                    return (organisms[0], organisms[1]);
                }
                else
                {
                    throw new System.Exception("Unable to select parents from a species");
                }
            }
        }

        public void UpdateCrossoverOperatorsProgressions(Biocenosis biocenosis)
        {
            foreach (Specie specie in biocenosis.GetSpeciesList())
            {
                // If there is only one operator, skip this passage
                if (specie.operatorsWrapper.crossoverOperators.Count < 2) return;
                double singlePointSum = 0;
                int singlePointCount = 0;

                double kPointSum = 0;
                int kPointCount = 0;

                double avgSum = 0;
                int avgCount = 0;

                double uniformSum = 0;
                int uniformCount = 0;

                foreach (Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism> descriptor in specie.lastGenDescriptor)
                {
                    List<double> values = new List<double> { descriptor.Item2.ProvideRawFitness(), descriptor.Item1.parentFitness, descriptor.Item1.parent1Fitness };
                    values = values.OrderByDescending(x => x).ToList();
                    double S = values[0] + values[1];
                    double value = S - (descriptor.Item1.parentFitness + descriptor.Item1.parent1Fitness);
                    if (descriptor.Item1.operatorUsed is null)
                    {
                        continue;
                    }
                    if (descriptor.Item1.operatorUsed is UniformCrossoverOperator)
                    {
                        uniformSum += value;
                        uniformCount++;
                    }
                    else if (descriptor.Item1.operatorUsed is SinglePointCrossover)
                    {
                        singlePointSum += value;
                        singlePointCount++;
                    }
                    else if (descriptor.Item1.operatorUsed is KPointsCrossoverOperator)
                    {
                        kPointSum += value;
                        kPointCount++;
                    }
                    else if (descriptor.Item1.operatorUsed is AverageCrossoverOperator)
                    {
                        avgSum += value;
                        avgCount++;
                    }
                }

                singlePointSum = singlePointCount > 0 ? singlePointSum / singlePointCount : 0;
                kPointSum = kPointCount > 0 ? kPointSum / kPointCount : 0;
                uniformSum = uniformCount > 0 ? uniformSum / uniformCount : 0;
                avgSum = avgCount > 0 ? avgSum / avgCount : 0;

                double total = singlePointSum + kPointSum + uniformSum + avgSum;
                if (total == 0)
                {
                    return;
                }
                //Debug.Log("SUM: " + total);
                //Debug.Log("SPC: " + singlePointSum + "UC: " + uniformSum + "KPC:" + kPointSum + "AVGC: " + avgSum);

                UniformCrossoverOperator uniformCrossoverOp = specie.operatorsWrapper.GetOperatorOfType<UniformCrossoverOperator>();
                SinglePointCrossover singleCrossoverOp = specie.operatorsWrapper.GetOperatorOfType<SinglePointCrossover>();
                KPointsCrossoverOperator kPointsCrossoverOp = specie.operatorsWrapper.GetOperatorOfType<KPointsCrossoverOperator>();
                AverageCrossoverOperator averageCrossoverOp = specie.operatorsWrapper.GetOperatorOfType<AverageCrossoverOperator>();

                uniformCrossoverOp?.SetCurrentProgression((float)uniformSum);
                singleCrossoverOp?.SetCurrentProgression((float)singlePointSum);
                kPointsCrossoverOp?.SetCurrentProgression((float)kPointSum);
                averageCrossoverOp?.SetCurrentProgression((float)avgSum);

                uniformCrossoverOp?.SetSelectProbability((float)(uniformSum / total * (specie.rates.crossoverRatio - specie.operatorsWrapper.OperatorsCount * preferences.minCrossoverRatio) + preferences.minCrossoverRatio));
                singleCrossoverOp?.SetSelectProbability((float)(singlePointSum / total * (specie.rates.crossoverRatio - specie.operatorsWrapper.OperatorsCount * preferences.minCrossoverRatio) + preferences.minCrossoverRatio));
                kPointsCrossoverOp?.SetSelectProbability((float)(kPointSum / total * (specie.rates.crossoverRatio - specie.operatorsWrapper.OperatorsCount * preferences.minCrossoverRatio) + preferences.minCrossoverRatio));
                averageCrossoverOp?.SetSelectProbability((float)(avgSum / total * (specie.rates.crossoverRatio - specie.operatorsWrapper.OperatorsCount * preferences.minCrossoverRatio) + preferences.minCrossoverRatio));
                //Debug.Log(specie.operatorsWrapper.ToString());
            }

            //operatorsWrapper.crossoverOperators = operatorsWrapper.crossoverOperators.OrderByDescending(x => x.GetCurrentProgression()).ToList();
            //string rank = "Operators ranking\n";
            //foreach (CrossoverOperator @operator in operatorsWrapper.crossoverOperators)
            //{
            //    rank += @operator.ToString() + "\n";
            //}
            //Debug.Log(rank);

            //rank = "Operators probabilities\n";
            //foreach (CrossoverOperator @operator in operatorsWrapper.crossoverOperators)
            //{
            //    rank += @operator.ToString() + ", " + @operator.ProvideSelectProbability() + "\n";
            //}
            ////Debug.Log(rank);
        }
    }
}