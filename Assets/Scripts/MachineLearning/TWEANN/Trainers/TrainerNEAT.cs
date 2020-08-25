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
        private float maxMutationRate;
        private float minCrossoverRatio;

        public Breeding breeding;

        public TrainerNEAT(Breeding breeding, float maxMutationRate, float minCrossoverRatio)
        {
            this.breeding = breeding;
            this.maxMutationRate = maxMutationRate;
            this.minCrossoverRatio = minCrossoverRatio;
        }

        public override Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[] Train(Biocenosis biocenosis)
        {
            GlobalParams.ResetGenerationMutations();

            double maxFitness = biocenosis.GetCurrentFittest().ProvideRawFitness();
            double averageFitness = biocenosis.GetAverageFitness();
            breeding.rates.mutationRate = (float)(maxMutationRate * ((averageFitness / maxFitness))); //Could be a power here
            Debug.Log("AVG: " + averageFitness + ", MAX: " + maxFitness);
            Debug.Log("M: " + breeding.rates.mutationRate + ", C: " + breeding.rates.crossoverRate);

            biocenosis.AdjustSpecies();

            int expectedIndividualCount = biocenosis.GetExpectedIndividualNumber();
            Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[] pop = new Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>[expectedIndividualCount];
            CrossoverOperator operatorToApply = null;
            int currentIndex = 0;

            foreach (Species current in biocenosis.GetSpeciesList())
            {
                (IOrganism, IOrganism) parents = SelectionFromSpecies(current);
                for (int i = 0; i < current.GetExpectedOffpringsCount(); i++)
                {
                    Genotype childGen = null;
                    if (UnityEngine.Random.Range(0F, 1F) <= breeding.rates.crossoverRate)
                    {
                        operatorToApply = breeding.GetRandomOperator();
                        childGen = operatorToApply.Apply(parents.Item1, parents.Item2);
                    }
                    else
                    {
                        if (parents.Item1.ProvideRawFitness() > parents.Item2.ProvideRawFitness())
                        {
                            childGen = parents.Item1.ProvideNeuralNet().GetGenotype();
                        }
                        else
                        {
                            childGen = parents.Item2.ProvideNeuralNet().GetGenotype();
                        }
                    }

                    childGen.Mutate(breeding.rates.mutationRate);

                    pop[currentIndex] = new Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, Genotype>(
                        new DescriptorsWrapper.CrossoverOperationDescriptor(parents.Item1.ProvideRawFitness(), parents.Item2.ProvideRawFitness(), operatorToApply),
                        childGen);
                    //Debug.Log("Child: " + childGen.ToString());
                    currentIndex++;
                }
            }

            return pop;
        }

        /// <summary>
        ///   Pick the 2 fittest individuals in the species, if the species has only 1 member, it returns the member (duplicate)
        /// </summary>
        /// <param name="species"> </param>
        /// <returns> </returns>
        private (IOrganism, IOrganism) PickFittestTwoInSpecies(Species species)
        {
            IOrganism first = null;
            double firstFitness = -1F;
            foreach (IOrganism car in species.GetIndividuals())
            {
                if (car.ProvideAdjustedFitness() > firstFitness)
                {
                    first = car;
                    firstFitness = car.ProvideAdjustedFitness();
                }
            }
            double secondFitness = -1F;
            IOrganism second = null;
            foreach (IOrganism car in species.GetIndividuals())
            {
                if (car.ProvideAdjustedFitness() > secondFitness)
                {
                    if (species.GetIndividualCount() > 1)
                    {
                        if (car != first)
                        {
                            second = car;
                            secondFitness = car.ProvideAdjustedFitness();
                        }
                    }
                    else
                    {
                        second = car;
                        secondFitness = car.ProvideAdjustedFitness();
                    }
                }
            }
            return (first, second);
        }

        /// <summary>
        ///   Perform selection from a specified species
        /// </summary>
        /// <param name="species"> </param>
        /// <returns> </returns>
        private (IOrganism, IOrganism) SelectionFromSpecies(Species species)
        {
            if (species.GetIndividualCount() == 1)
            {
                IOrganism champ = species.GetChamp();
                return (champ, champ);
            }
            else if (species.GetIndividualCount() > 1)
            {
                //Debug.Log("Returning 2 ");
                return PickFittestTwoInSpecies(species);
            }
            else
            {
                throw new System.Exception("Unable to select parents from a species");
            }
        }

        public void UpdateCrossoverOperatorsProgressions(List<Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism>> descriptors)
        {
            double singlePointSum = 0;
            int singlePointCount = 0;

            double kPointSum = 0;
            int kPointCount = 0;

            double avgSum = 0;
            int avgCount = 0;

            double uniformSum = 0;
            int uniformCount = 0;

            foreach (Tuple<DescriptorsWrapper.CrossoverOperationDescriptor, IOrganism> descriptor in descriptors)
            {
                double max = TMath.Max(descriptor.Item2.ProvideRawFitness(), descriptor.Item1.parentFitness, descriptor.Item1.parent1Fitness);
                double value = max - ((descriptor.Item1.parentFitness + descriptor.Item1.parent1Fitness) / 2F);
                //Debug.Log(descriptor.parentFitness + ", " + descriptor.parent1Fitness + ", C: " + descriptor.child.ProvideRawFitness() + "OP: " + descriptor.operatorUsed.ToString() + "V: " + value);
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
            //Debug.Log("SUM: " + total);
            //Debug.Log("SPC: " + singlePointSum + "UC: " + uniformSum + "KPC:" + kPointSum + "AVGC: " + avgSum);

            UniformCrossoverOperator uniformCrossoverOp = breeding.GetOperatorOfType<UniformCrossoverOperator>();
            SinglePointCrossover singleCrossoverOp = breeding.GetOperatorOfType<SinglePointCrossover>();
            KPointsCrossoverOperator kPointsCrossoverOp = breeding.GetOperatorOfType<KPointsCrossoverOperator>();
            AverageCrossoverOperator averageCrossoverOp = breeding.GetOperatorOfType<AverageCrossoverOperator>();

            uniformCrossoverOp?.SetCurrentProgression((float)uniformSum);
            singleCrossoverOp?.SetCurrentProgression((float)singlePointSum);
            kPointsCrossoverOp?.SetCurrentProgression((float)kPointSum);
            averageCrossoverOp?.SetCurrentProgression((float)avgSum);

            uniformCrossoverOp?.SetSelectProbability((float)(uniformSum / total * (1 - breeding.OperatorsCount * minCrossoverRatio) + minCrossoverRatio));
            singleCrossoverOp?.SetSelectProbability((float)(singlePointSum / total * (1 - breeding.OperatorsCount * minCrossoverRatio) + minCrossoverRatio));
            kPointsCrossoverOp?.SetSelectProbability((float)(kPointSum / total * (1 - breeding.OperatorsCount * minCrossoverRatio) + minCrossoverRatio));
            averageCrossoverOp?.SetSelectProbability((float)(avgSum / total * (1 - breeding.OperatorsCount * minCrossoverRatio) + minCrossoverRatio));

            //Debug.Log("SP: " + uniformSum + "U: " + uniformSum + "KP:" + kPointSum + "AVG: " + avgSum);
            breeding.crossoverOperators = breeding.crossoverOperators.OrderByDescending(x => x.GetCurrentProgression()).ToList();
            string rank = "Operators ranking\n";
            foreach (CrossoverOperator @operator in breeding.crossoverOperators)
            {
                rank += @operator.ToString() + "\n";
            }
            //Debug.Log(rank);

            rank = "Operators probabilities\n";
            foreach (CrossoverOperator @operator in breeding.crossoverOperators)
            {
                rank += @operator.ToString() + ", " + @operator.ProvideSelectProbability() + "\n";
            }
            //Debug.Log(rank);
        }
    }
}