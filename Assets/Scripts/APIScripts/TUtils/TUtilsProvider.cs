using System;
using System.Collections.Generic;

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

    public static List<Tuple<Type1, Type2>> ZipWithPredicate<Type1, Type2>(List<Type1> first, List<Type2> second, Func<Type1, Type2, bool> predicate)
    {
        List<Tuple<Type1, Type2>> res = new List<Tuple<Type1, Type2>>();
        IEnumerator<Type2> num2 = second.GetEnumerator();
        num2.MoveNext();
        foreach (Type1 elem in first)
        {
            if (predicate(elem, num2.Current))
            {
                res.Add(Tuple.Create(elem, num2.Current));
                if (!num2.MoveNext())
                {
                    break;
                }
            }
        }
        return res;
    }
}