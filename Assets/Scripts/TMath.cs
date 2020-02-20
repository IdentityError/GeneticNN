using System;

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
    public static float Tanh(float x)
    {
        float exp = (float)Math.Exp(2 * x);
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
    public static float ArctanCatetis(float x, float y)
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
        return -a * (sub * sub) / (2 * c * c);
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
}