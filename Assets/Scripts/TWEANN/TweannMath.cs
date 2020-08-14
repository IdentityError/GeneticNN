using System;
using System.Collections.Generic;

namespace Assets.Scripts.TWEANN
{
    public static class TweannMath
    {
        public static double StandardDeviation(List<IIndividual> population)
        {
            double avg = 0;
            foreach (IIndividual individual in population)
            {
                avg += individual.ProvideFitness();
            }
            avg /= population.Count;
            double sum = 0;
            foreach (IIndividual individual1 in population)
            {
                double diff = individual1.ProvideFitness() - avg;
                sum += (diff * diff);
            }
            sum /= population.Count;
            return Math.Sqrt(sum);
        }
    }
}