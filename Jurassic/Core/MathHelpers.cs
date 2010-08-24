using System;

namespace Jurassic
{

    /// <summary>
    /// Contains handy mathematical routines.
    /// </summary>
    internal static class MathHelpers
    {
        /// <summary>
        /// Converts the given number to the closest Int32.
        /// </summary>
        /// <param name="x"> The number to convert.  Can not be NaN. </param>
        /// <returns> The closest Int32 to the given number. </returns>
        /// <remarks>
        /// The number is assumed to be an integer already.
        /// </remarks>
        internal static int ClampToInt32(double x)
        {
            if (x > int.MaxValue)
                return int.MaxValue;
            return (int)x;
        }

        /// <summary>
        /// Multiplies the given number with a power of 10 and returns the result.
        /// </summary>
        /// <param name="x"> The number to multiply. </param>
        /// <param name="exponent"> The exponent. </param>
        /// <returns> The result of calculating <paramref name="x"/> * Math.Pow(10,
        /// <paramref name="exponent" />). </returns>
        /// <example>
        /// For example, FastMulPow10(5, 2) returns 500.
        /// </example>
        internal static double MulPow10(double x, int exponent)
        {
            if (exponent > 0)
                return x * System.Math.Pow(10, exponent);
            else if (exponent < 0)
                return x / System.Math.Pow(10, -exponent);
            return x;
        }

        /// <summary>
        /// Determines the floor of the base-10 logarithm of the given number (i.e. ).
        /// </summary>
        /// <param name="x"> The number to get the logarithm of. </param>
        /// <returns> The result of calculating calculating Math.Floor(Math.Log10(
        /// <paramref name="x"/>)). </returns>
        internal static int FloorLog10(double x)
        {
            return (int)Math.Floor(Math.Log10(x));
        }
    }

}
