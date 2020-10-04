// Copyright (c) 2020 Matteo Beltrame
using System;

namespace Assets.Scripts.TUtils.Utils
{
    /// <summary>
    ///   Trat Math
    /// </summary>
    public static class TMath
    {
        public const float RadToDeg = 57.29578F;
        public const float DegToRad = 0.0174533F;

        /// <summary>
        ///   Return: hyperbolic sine
        /// </summary>
        public static float Sinh(float x)
        {
            return (float)(Math.Exp(x) - Math.Exp(-x)) / 2F;
        }

        /// <summary>
        ///   Return: hyperbolic tangent
        /// </summary>
        public static double Tanh(double x)
        {
            double exp = (double)Math.Exp(2 * x);
            return (exp - 1) / (exp + 1);
        }

        /// <summary>
        ///   Return: hyperbolic cosine
        /// </summary>
        public static float Cosh(float x)
        {
            return (float)(Math.Exp(x) + Math.Exp(-x)) / 2F;
        }

        /// <summary>
        ///   Return: the arctan value given the 2 catetis
        /// </summary>
        public static float Arctan4(float x, float y)
        {
            if (x >= 0)
            {
                if (y >= 0)
                {
                    if (y == 0) return -90f;

                    return (float)-Math.Atan(x / y) * TMath.RadToDeg;
                }
                else
                {
                    return -180f + ((float)Math.Atan(x / -y) * TMath.RadToDeg);
                }
            }
            else
            {
                if (y >= 0)
                {
                    if (y == 0) return 90f;

                    return (float)Math.Atan(-x / y) * TMath.RadToDeg;
                }
                else
                {
                    return 180f - ((float)Math.Atan(x / y) * TMath.RadToDeg);
                }
            }
        }

        /// <summary>
        ///   Return: the Gaussian function with a, b and c costants
        /// </summary>
        public static float Gaussian(float x, float a, float b, float c)
        {
            if (c == 0)
            {
                throw new System.Exception("TMath Exception -> Costant C cannot be 0 in Gaussian function");
            }
            float sub = x - b;
            return (float)(a * Math.Exp(-(sub * sub) / (c * c)));
        }

        /// <summary>
        ///   Return: the e function raised to the b*x power and multiplied by a
        /// </summary>
        /// <param name="a"> </param>
        /// <param name="b"> </param>
        /// <param name="x"> </param>
        /// <returns> </returns>
        public static double AdjExp(float a, float b, float x)
        {
            return Math.Exp(b * x) * a;
        }

        /// <summary>
        ///   Return: the Kronecker Delta given the two indexes
        /// </summary>
        public static int KroneckerDelta(int i, int j)
        {
            return (i == j ? 1 : 0);
        }

        /// <summary>
        ///   Return: the Heaviside Step function
        /// </summary>
        public static int HeavisideStep(int x)
        {
            return (x > 0 ? 1 : 0);
        }

        /// <summary>
        ///   Reutrn: the Levi-Civita tensor value
        /// </summary>
        public static int LeviCivitaTensor(int i, int j, int k)
        {
            return Math.Sign(j - i) + Math.Sign(k - j) + Math.Sign(i - k);
        }

        /// <summary>
        ///   Reutrn: the absolute value of a floating point number
        /// </summary>
        public static float Abs(float value)
        {
            return value > 0 ? value : -value;
        }

        /// <summary>
        ///   Reutrn: the absolute value of an int number
        /// </summary>
        public static int Abs(int value)
        {
            return value > 0 ? value : -value;
        }

        /// <summary>
        ///   Return the max value between an arbitrary number of integers
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static int Max(params int[] data)
        {
            int max = int.MinValue;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > max)
                {
                    max = data[i];
                }
            }
            return max;
        }

        /// <summary>
        ///   Return the max value between an arbitrary number of floats
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static float Max(params float[] data)
        {
            float max = float.MinValue;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > max)
                {
                    max = data[i];
                }
            }
            return max;
        }

        /// <summary>
        ///   Return the max value between an arbitrary number of doubles
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static double Max(params double[] data)
        {
            double max = double.MinValue;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > max)
                {
                    max = data[i];
                }
            }
            return max;
        }

        /// <summary>
        ///   Return the min value between an arbitrary number of integers
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static int Min(params int[] data)
        {
            int min = int.MaxValue;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < min)
                {
                    min = data[i];
                }
            }
            return min;
        }

        /// <summary>
        ///   Return the min value between an arbitrary number of floats
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static float Min(params float[] data)
        {
            float min = float.MaxValue;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < min)
                {
                    min = data[i];
                }
            }
            return min;
        }

        /// <summary>
        ///   Return the min value between an arbitrary number of doubles
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static double Min(params double[] data)
        {
            double min = double.MaxValue;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < min)
                {
                    min = data[i];
                }
            }
            return min;
        }

        /// <summary>
        ///   Returns the average of an arbitrary number of floats
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static float Avg(params float[] data)
        {
            float sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }
            return sum / data.Length;
        }

        /// <summary>
        ///   Returns the average of an arbitrary number of doubles
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static double Avg(params double[] data)
        {
            double sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }
            return sum / data.Length;
        }

        /// <summary>
        ///   Returns the sum of an arbitrary number of doubles
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static double Sum(params double[] data)
        {
            double sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }
            return sum;
        }

        /// <summary>
        ///   Returns the sum of an arbitrary number of floats
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static float Sum(params float[] data)
        {
            float sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }
            return sum;
        }

        /// <summary>
        ///   Returns the sum of an arbitrary number of ints
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public static int Sum(params int[] data)
        {
            int sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }
            return sum;
        }

        public static class RandomGen
        {
            private static bool nextGaussianAvailable;
            private static double nextGaussian;

            public static double NextGaussian(double mean, double stdv)
            {
                double amplitude = 1F / Math.Sqrt(2F * Math.PI * stdv * stdv);
                if (nextGaussianAvailable)
                {
                    nextGaussianAvailable = false;
                    return nextGaussian;
                }
                else
                {
                    double u1 = UnityEngine.Random.Range(0F, 1F);
                    double u2 = UnityEngine.Random.Range(0F, 1F);
                    double x = 0;
                    double y = 0;
                    if (u1 != 0)
                    {
                        x = amplitude * (mean + stdv * (Math.Sqrt(-2F * Math.Log(u1))) * Math.Cos(2F * Math.PI * u2));
                    }
                    if (u2 != 0)
                    {
                        y = amplitude * (mean + stdv * (Math.Sqrt(-2F * Math.Log(u1))) * Math.Sin(2F * Math.PI * u2));
                    }
                    nextGaussianAvailable = true;
                    nextGaussian = y;
                    return x;
                }
            }
        }
    }
}