using Assets.Scripts.Descriptors;
using Assets.Scripts.NeuralNet;
using Assets.Scripts.TUtils.Interfaces;
using Assets.Scripts.TUtils.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    [System.Serializable]
    public class TrainerNEAT : PopulationTrainer
    {
        [Header("Crossover Operators")]
        [SerializeField] private bool unifom;
        [SerializeField] private bool singlePoint;
        [SerializeField] private bool KPoint;
        [SerializeField] private bool average;
        [Space(5)]
        private List<CrossoverOperator> crossoverOperators;

        [Header("Mutation Operators")]
        [SerializeField] private bool swap;

        public override void Initialize()
        {
            int crossoverOperatorsN = 0;
            crossoverOperatorsN += unifom ? 1 : 0;
            crossoverOperatorsN += singlePoint ? 1 : 0;
            crossoverOperatorsN += KPoint ? 1 : 0;
            crossoverOperatorsN += average ? 1 : 0;
            crossoverOperators = new List<CrossoverOperator>();
            if (unifom) crossoverOperators.Add(new UniformCrossoverOperator());
            if (singlePoint) crossoverOperators.Add(new SinglePointCrossover());
            if (KPoint) crossoverOperators.Add(new KPointsCrossoverOperator());
            if (average) crossoverOperators.Add(new AverageCrossoverOperator());
            //Debug.Log(crossoverOperators.Count);
            foreach (CrossoverOperator @operator in crossoverOperators)
            {
                @operator.SetSelectProbability(1F / crossoverOperators.Count);
                //Debug.Log(@operator.ProvideSelectProbability());
            }
        }

        public override DescriptorsWrapper.OffspringDescriptor[] Train(Biocenosis biocenosis)
        {
            GlobalParams.ResetGenerationMutations();

            int expectedIndividualCount = biocenosis.GetExpectedIndividualNumber();
            DescriptorsWrapper.OffspringDescriptor[] pop = new DescriptorsWrapper.OffspringDescriptor[expectedIndividualCount];
            CrossoverOperator operatorToApply = null;
            int currentIndex = 0;
            foreach (Species current in biocenosis.GetSpeciesList())
            {
                for (int i = 0; i < current.GetExpectedIndividualCount(); i++)
                {
                    (IIndividual, IIndividual) parents = SelectionFromSpecies(current);
                    Genotype childGen = null;
                    if (Random.Range(0F, 1F) <= current.breedingParameters.crossoverProbability)
                    {
                        operatorToApply = GetRandomOperator();
                        childGen = operatorToApply.Apply(parents.Item1, parents.Item2);
                    }
                    else
                    {
                        if (Random.Range(0F, 1F) < 0.5F)
                        {
                            childGen = parents.Item1.ProvideNeuralNet().GetGenotype();
                        }
                        else
                        {
                            childGen = parents.Item2.ProvideNeuralNet().GetGenotype();
                        }
                    }

                    childGen.Mutate(current.breedingParameters);
                    pop[currentIndex] = new DescriptorsWrapper.OffspringDescriptor(parents.Item1, parents.Item2, childGen, operatorToApply);
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
        private (IIndividual, IIndividual) PickFittestTwoInSpecies(Species species)
        {
            IIndividual first = null;
            double firstFitness = -1F;
            foreach (IIndividual car in species.GetIndividuals())
            {
                if (car.ProvideAdjustedFitness() > firstFitness)
                {
                    first = car;
                    firstFitness = car.ProvideAdjustedFitness();
                }
            }
            double secondFitness = -1F;
            IIndividual second = null;
            foreach (IIndividual car in species.GetIndividuals())
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
        private (IIndividual, IIndividual) SelectionFromSpecies(Species species)
        {
            if (species.GetIndividualCount() == 1)
            {
                IIndividual champ = species.GetChamp();
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

        private CrossoverOperator GetRandomOperator()
        {
            List<IProbSelectable> sel = new List<IProbSelectable>(crossoverOperators);
            CrossoverOperator op = TUtilsProvider.SelectWithProbability<CrossoverOperator>(sel);
            return op;
        }

        public void UpdateCrossoverOperatorsProgressions(List<DescriptorsWrapper.CrossoverOperationDescriptor> descriptors)
        {
            double singlePointSum = 0;
            int singlePointCount = 0;

            double kPointSum = 0;
            int kPointCount = 0;

            double avgSum = 0;
            int avgCount = 0;

            double uniformSum = 0;
            int uniformCount = 0;

            foreach (DescriptorsWrapper.CrossoverOperationDescriptor descriptor in descriptors)
            {
                //Debug.Log("P(" + ((MonoBehaviour)descriptor.parent).name + "): " + descriptor.parent.ProvideRawFitness() + ", P1(" + ((MonoBehaviour)descriptor.parent1).name + "): " + descriptor.parent1.ProvideRawFitness() + ", C(" + ((MonoBehaviour)descriptor.child).name + "): " + descriptor.child.ProvideRawFitness() + "OP: " + descriptor.operatorUsed.ToString());
                double value = descriptor.child.ProvideRawFitness() - ((descriptor.parent.ProvideRawFitness() + descriptor.parent1.ProvideRawFitness()) / 2F);

                if (descriptor.operatorUsed is UniformCrossoverOperator)
                {
                    uniformSum += value;
                    uniformCount++;
                    //Debug.Log("Uniform: " + value);
                }
                else if (descriptor.operatorUsed is SinglePointCrossover)
                {
                    singlePointSum += value;
                    singlePointCount++;
                    //Debug.Log("Single: " + value);
                }
                else if (descriptor.operatorUsed is KPointsCrossoverOperator)
                {
                    kPointSum += value;
                    kPointCount++;
                }
                else if (descriptor.operatorUsed is AverageCrossoverOperator)
                {
                    avgSum += value;
                    avgCount++;
                }
            }

            singlePointSum = singlePointCount > 0 ? singlePointSum / singlePointCount : 0;
            kPointSum = kPointCount > 0 ? kPointSum / kPointCount : 0;
            uniformSum = uniformCount > 0 ? uniformSum / uniformCount : 0;
            avgSum = avgCount > 0 ? avgSum / avgCount : 0;

            //Debug.Log("SUM: " + totalSum);
            //Debug.Log("SPC: " + singlePointCount + "UC: " + uniformCount + "KPC:" + kPointCount + "AVGC: " + avgCount);

            UniformCrossoverOperator uniformCrossoverOp = (UniformCrossoverOperator)crossoverOperators.Find((x) => x is UniformCrossoverOperator);
            SinglePointCrossover singleCrossoverOp = (SinglePointCrossover)crossoverOperators.Find((x) => x is SinglePointCrossover);
            KPointsCrossoverOperator kPointsCrossoverOp = (KPointsCrossoverOperator)crossoverOperators.Find((x) => x is KPointsCrossoverOperator);
            AverageCrossoverOperator averageCrossoverOp = (AverageCrossoverOperator)crossoverOperators.Find((x) => x is AverageCrossoverOperator);

            uniformCrossoverOp.SetCurrentProgression((float)(uniformSum));
            singleCrossoverOp.SetCurrentProgression((float)(singlePointSum));
            kPointsCrossoverOp.SetCurrentProgression((float)(kPointSum));
            averageCrossoverOp.SetCurrentProgression((float)(avgSum));

            Debug.Log("SP: " + singleCrossoverOp.currentProgression + "U: " + uniformSum + "KP:" + kPointSum + "AVG: " + avgSum);
            crossoverOperators = crossoverOperators.OrderByDescending(x => x.currentProgression).ToList();
            string rank = "Operators ranking: \n";
            foreach (CrossoverOperator @operator in crossoverOperators)
            {
                rank += @operator.ToString() + "\n";
            }
            Debug.Log(rank);

            //Debug.Log("Ordered crossovers operators: " + crossoverOperators);

            int influence = crossoverOperators.Count;
            float normalization = 0F;
            for (int i = 0; i < crossoverOperators.Count; i++)
            {
                normalization += influence-- * crossoverOperators[i].ProvideSelectProbability();
            }
            //Debug.Log("Normalization: " + normalization);
            influence = crossoverOperators.Count;
            for (int i = 0; i < crossoverOperators.Count; i++)
            {
                crossoverOperators[i].SetSelectProbability((influence-- * crossoverOperators[i].ProvideSelectProbability()) / normalization);
            }

            rank = "Current crossover probabilities: \n";
            foreach (CrossoverOperator @operator in crossoverOperators)
            {
                rank += @operator.ToString() + ", " + @operator.ProvideSelectProbability() + "\n";
            }
            Debug.Log(rank);
        }
    }
}