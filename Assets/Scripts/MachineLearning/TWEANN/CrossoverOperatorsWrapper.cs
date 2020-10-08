using Assets.Scripts.TUtils.Utils;
using System.Collections.Generic;

namespace Assets.Scripts.MachineLearning.TWEANN
{
    [System.Serializable]
    public class CrossoverOperatorsWrapper
    {
        public List<CrossoverOperator> crossoverOperators;

        public int OperatorsCount
        {
            get => crossoverOperators.Count;
        }

        public CrossoverOperatorsWrapper(List<CrossoverOperator> operators)
        {
            crossoverOperators = new List<CrossoverOperator>();
            for (int i = 0; i < operators.Count; i++)
            {
                crossoverOperators.Add(operators[i]);
                crossoverOperators[i].SetSelectProbability(1F / operators.Count);
                crossoverOperators[i].SetCurrentProgression(0F);
            }
        }

        public CrossoverOperatorsWrapper Copy()
        {
            return new CrossoverOperatorsWrapper(this.crossoverOperators);
        }

        public CrossoverOperator GetRandomOperator()
        {
            CrossoverOperator op = TUtilsProvider.SelectWithProbability(crossoverOperators);
            return op;
        }

        public T GetOperatorOfType<T>() where T : CrossoverOperator
        {
            return (T)crossoverOperators.Find((x) => x is T);
        }

        public override string ToString()
        {
            string ret = "";
            foreach (CrossoverOperator op in crossoverOperators)
            {
                ret += op.ToString() + "\n";
            }
            return ret;
        }
    }
}