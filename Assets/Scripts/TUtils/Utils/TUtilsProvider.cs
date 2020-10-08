// Copyright (c) 2020 Matteo Beltrame

using Assets.Scripts.TUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.TUtils.Utils
{
    public class TUtilsProvider
    {
        /// <summary>
        ///   Copy an array to another array ignoring the elements at indexes: <paramref name="indexesExcepts"/>
        /// </summary>
        public static void CopyArrayWithExceptsAt(Array from, Array to, int[] indexesExcepts)
        {
            Array.Sort(indexesExcepts);
            if (from.Length - to.Length != indexesExcepts.Length)
            {
                throw new System.Exception("Unable to copy arrays of wrong dimensions");
            }

            int exIndex = 0;
            int toIndex = 0;
            for (int i = 0; i < from.Length; i++)
            {
                if (i != indexesExcepts[exIndex])
                {
                    to.SetValue(from.GetValue(i), toIndex++);
                }
                else
                {
                    if (exIndex < indexesExcepts.Length - 1)
                        exIndex++;
                }
            }
        }

        /// <summary>
        ///   Copy an array to another array leaving holes at indexes: <paramref name="indexesExcepts"/>
        /// </summary>
        public static void CopyArrayWithHolesAt(Array from, Array to, int[] indexesExcepts)
        {
            Array.Sort(indexesExcepts);
            if (to.Length - from.Length != indexesExcepts.Length)
            {
                throw new System.Exception("Unable to copy arrays of wrong dimensions");
            }

            int elementToCopy = 0;
            int exIndex = 0;

            for (int i = 0; i < to.Length; i++)
            {
                if (exIndex > indexesExcepts.Length - 1)
                {
                    to.SetValue(from.GetValue(elementToCopy++), i);
                }
                else if (i != indexesExcepts[exIndex])
                {
                    to.SetValue(from.GetValue(elementToCopy++), i);
                }
                else
                {
                    exIndex++;
                }
            }
        }

        /// <summary>
        ///   For each element of the first list, zips the first predicate matching element from the second list
        /// </summary>
        /// <typeparam name="Type1"> </typeparam>
        /// <typeparam name="Type2"> </typeparam>
        /// <param name="first"> </param>
        /// <param name="second"> </param>
        /// <param name="predicate"> </param>
        /// <returns> The zipped list </returns>
        public static List<Tuple<Type1, Type2>> ZipWithFirstPredicateMatching<Type1, Type2>(List<Type1> first, List<Type2> second, Func<Type1, Type2, bool> predicate)
        {
            List<Tuple<Type1, Type2>> res = new List<Tuple<Type1, Type2>>();
            foreach (Type1 elem in first)
            {
                foreach (Type2 elem2 in second)
                {
                    if (predicate(elem, elem2))
                    {
                        res.Add(Tuple.Create(elem, elem2));
                        break;
                    }
                }
            }
            return res;
        }

        /// <summary>
        ///   Select an item based on its probability (interface)
        /// </summary>
        /// <param name="set"> </param>
        /// <returns> The selecte item </returns>
        public static T SelectWithProbability<T>(List<T> set) where T : IProbSelectable
        {
            int index = -1;
            float r = UnityEngine.Random.Range(0F, 1F);
            while (r > 0 && index + 1 < set.Count)
            {
                r -= set.ElementAt(++index).ProvideSelectProbability();
            }
            if (index < set.Count)
            {
                return (T)set.ElementAt(index);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        ///   Normalize the select probabilities using the <b> float value(T:IProbSelectable) </b> function
        /// </summary>
        /// <param name="set"> </param>
        /// <returns> </returns>
        public static void NormalizeProbabilities<T>(List<T> set, Func<T, float> value) where T : IProbSelectable
        {
            float sum = 0;
            foreach (T elem in set)
            {
                sum += value(elem);
            }
            foreach (T elem1 in set)
            {
                elem1.SetSelectProbability(value(elem1) / sum);
            }
        }
    }
}