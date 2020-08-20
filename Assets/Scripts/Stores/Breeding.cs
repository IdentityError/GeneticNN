using Assets.Scripts.TUtils.Interfaces;
using Assets.Scripts.TUtils.Utils;
using Assets.Scripts.TWEANN;
using System.Collections.Generic;

namespace Assets.Scripts.Stores
{
    [System.Serializable]
    public class Breeding
    {
        [System.Serializable]
        public struct BreedingRates
        {
            public float mutationRate;
            public float crossoverRate;

            public BreedingRates(float mutationProbability, float crossoverProbability)
            {
                this.mutationRate = mutationProbability;
                this.crossoverRate = crossoverProbability;
            }
        }

        public BreedingRates rates;
        public List<CrossoverOperator> crossoverOperators;

        public int OperatorsCount
        {
            get => crossoverOperators.Count;
        }

        public Breeding(float mutationRate, float crossoverRate, List<CrossoverOperator> operators)
        {
            rates = new BreedingRates(mutationRate, crossoverRate);
            crossoverOperators = new List<CrossoverOperator>();
            for (int i = 0; i < operators.Count; i++)
            {
                crossoverOperators.Add(operators[i]);
                crossoverOperators[i].SetSelectProbability(1F / operators.Count);
            }
        }

        public Breeding Copy()
        {
            return new Breeding(rates.mutationRate, rates.crossoverRate, this.crossoverOperators);
        }

        public CrossoverOperator GetRandomOperator()
        {
            List<IProbSelectable> sel = new List<IProbSelectable>(crossoverOperators);
            CrossoverOperator op = TUtilsProvider.SelectWithProbability<CrossoverOperator>(sel);
            return op;
        }

        public T GetOperatorOfType<T>() where T : CrossoverOperator
        {
            return (T)crossoverOperators.Find((x) => x is T);
        }
    }
}