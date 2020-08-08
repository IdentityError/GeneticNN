using System;

namespace Assets.Scripts.TWEANN
{
    public static class TweannMath
    {
        public static double MutationRate(double maxFitness, double avgFitness, double ratio)
        {
            return Math.Pow((maxFitness - avgFitness) / maxFitness, ratio);
        }
    }
}