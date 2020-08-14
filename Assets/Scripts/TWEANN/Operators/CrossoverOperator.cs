using Assets.Scripts.NeuralNet;
using Assets.Scripts.TWEANN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TWEANN
{
    /// <summary>
    /// Represent the abstract class of a generic Crossover operator
    /// </summary>
    public abstract class CrossoverOperator
    {
        protected float progression;

        public abstract Genotype Apply(IIndividual first, IIndividual second);
    }
}