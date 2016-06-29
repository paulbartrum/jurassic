using System;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in Math class that has mathematical constants and functions.
    /// </summary>
    public partial class MathObject : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Math object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal MathObject(ObjectInstance prototype)
            : base(prototype)
        {
            var properties = GetDeclarativeProperties(Engine);
            properties.Add(new PropertyNameAndValue(Engine.Symbol.ToStringTag, "Math", PropertyAttributes.Configurable));
            FastSetProperties(properties);
        }



        //     JAVASCRIPT FIELDS
        //_________________________________________________________________________________________

        /// <summary>
        /// The mathematical constant E, approximately 2.718.
        /// </summary>
        [JSField]
        public const double E       = 2.7182818284590452;

        /// <summary>
        /// The natural logarithm of 2, approximately 0.693.
        /// </summary>
        [JSField]
        public const double LN2     = 0.6931471805599453;

        /// <summary>
        /// The natural logarithm of 10, approximately 2.303.
        /// </summary>
        [JSField]
        public const double LN10    = 2.3025850929940456;

        /// <summary>
        /// The base 2 logarithm of E, approximately 1.442.
        /// </summary>
        [JSField]
        public const double LOG2E   = 1.4426950408889634;

        /// <summary>
        /// The base 10 logarithm of E, approximately 0.434.
        /// </summary>
        [JSField]
        public const double LOG10E  = 0.4342944819032518;

        /// <summary>
        /// The ratio of the circumference of a circle to its diameter, approximately 3.14159.
        /// </summary>
        [JSField]
        public const double PI      = 3.1415926535897932;

        /// <summary>
        /// The square root of 0.5, approximately 0.707.
        /// </summary>
        [JSField]
        public const double SQRT1_2 = 0.7071067811865475;

        /// <summary>
        /// The square root of 2, approximately 1.414.
        /// </summary>
        [JSField]
        public const double SQRT2   = 1.4142135623730950;



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns the absolute value of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The absolute value of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "abs")]
        public static double Abs(double number)
        {
            return Math.Abs(number);
        }

        /// <summary>
        /// Returns the arccosine of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The arccosine of the <paramref name="number"/> parameter.  If
        /// <paramref name="number"/> is less than -1 or greater than 1, then <c>NaN</c> is
        /// returned. </returns>
        [JSInternalFunction(Name = "acos")]
        public static double Acos(double number)
        {
            return Math.Acos(number);
        }

        /// <summary>
        /// Returns the arcsine of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The arcsine of the <paramref name="number"/> parameter. If
        /// <paramref name="number"/> is less than -1 or greater than 1, then <c>NaN</c> is
        /// returned. </returns>
        [JSInternalFunction(Name = "asin")]
        public static double Asin(double number)
        {
            return Math.Asin(number);
        }

        /// <summary>
        /// Returns the arctangent of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The arctangent of the <paramref name="number"/> parameter. If
        /// <paramref name="number"/> is less than -1 or greater than 1, then <c>NaN</c> is
        /// returned. </returns>
        [JSInternalFunction(Name = "atan")]
        public static double Atan(double number)
        {
            return Math.Atan(number);
        }

        /// <summary>
        /// Returns the counter-clockwise angle (in radians) from the X axis to the point (x,y).
        /// </summary>
        /// <param name="x"> A numeric expression representing the cartesian x-coordinate. </param>
        /// <param name="y"> A numeric expression representing the cartesian y-coordinate. </param>
        /// <returns> The angle (in radians) from the X axis to a point (x,y) (between -pi and pi). </returns>
        [JSInternalFunction(Name = "atan2")]
        public static double Atan2(double y, double x)
        {
            if (double.IsInfinity(y) || double.IsInfinity(x))
            {
                if (double.IsPositiveInfinity(y) && double.IsPositiveInfinity(x))
                    return PI / 4.0;
                if (double.IsPositiveInfinity(y) && double.IsNegativeInfinity(x))
                    return 3.0 * PI / 4.0;
                if (double.IsNegativeInfinity(y) && double.IsPositiveInfinity(x))
                    return -PI / 4.0;
                if (double.IsNegativeInfinity(y) && double.IsNegativeInfinity(x))
                    return -3.0 * PI / 4.0;
            }
            return Math.Atan2(y, x);
        }

        /// <summary>
        /// Returns the smallest integer greater than or equal to a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The smallest integer greater than or equal to the <paramref name="number"/>
        /// parameter. </returns>
        [JSInternalFunction(Name = "ceil")]
        public static double Ceil(double number)
        {
            return Math.Ceiling(number);
        }

        /// <summary>
        /// Returns the cosine of an angle.
        /// </summary>
        /// <param name="angle"> The angle to operate on. </param>
        /// <returns> The cosine of the <paramref name="angle"/> parameter (between -1 and 1). </returns>
        [JSInternalFunction(Name = "cos")]
        public static double Cos(double angle)
        {
            return Math.Cos(angle);
        }

        /// <summary>
        /// Returns e (the base of natural logarithms) raised to the specified power.
        /// </summary>
        /// <param name="number"> The exponent. </param>
        /// <returns> E (the base of natural logarithms) raised to the specified power. </returns>
        [JSInternalFunction(Name = "exp")]
        public static double Exp(double number)
        {
            return Math.Exp(number);
        }

        /// <summary>
        /// Returns the greatest integer less than or equal to a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The greatest integer less than or equal to the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "floor")]
        public static double Floor(double number)
        {
            return Math.Floor(number);
        }

        /// <summary>
        /// Returns the natural logarithm of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The natural logarithm of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "log")]
        public static double Log(double number)
        {
            return Math.Log(number);
        }

        /// <summary>
        /// Returns the largest of zero or more numbers.
        /// </summary>
        /// <param name="numbers"> The numbers to operate on. </param>
        /// <returns> The largest of zero or more numbers.  If no arguments are provided, the
        /// return value is equal to NEGATIVE_INFINITY.  If any of the arguments cannot be
        /// converted to a number, the return value is NaN. </returns>
        [JSInternalFunction(Name = "max", Length = 2)]
        public static double Max(params double[] numbers)
        {
            double result = double.NegativeInfinity;
            foreach (double number in numbers)
                if (number > result || double.IsNaN(number))
                    result = number;
            return result;
        }

        /// <summary>
        /// Returns the smallest of zero or more numbers.
        /// </summary>
        /// <param name="numbers"> The numbers to operate on. </param>
        /// <returns> The smallest of zero or more numbers.  If no arguments are provided, the
        /// return value is equal to NEGATIVE_INFINITY.  If any of the arguments cannot be
        /// converted to a number, the return value is NaN. </returns>
        [JSInternalFunction(Name = "min", Length = 2)]
        public static double Min(params double[] numbers)
        {
            double result = double.PositiveInfinity;
            foreach (double number in numbers)
                if (number < result || double.IsNaN(number))
                    result = number;
            return result;
        }

        /// <summary>
        /// Returns the value of a base expression taken to a specified power.
        /// </summary>
        /// <param name="base"> The base value of the expression. </param>
        /// <param name="exponent"> The exponent value of the expression. </param>
        /// <returns> The value of the base expression taken to the specified power. </returns>
        [JSInternalFunction(Name = "pow")]
        public static double Pow(double @base, double exponent)
        {
            if (@base == 1.0 && double.IsInfinity(exponent))
                return double.NaN;
            if (double.IsNaN(@base) && exponent == 0.0)
                return 1.0;
            return Math.Pow(@base, exponent);
        }

        private static object randomNumberGeneratorLock = new object();
        private static Random randomNumberGenerator;

        /// <summary>
        /// Returns a pseudorandom number between 0 and 1.
        /// </summary>
        /// <returns> A pseudorandom number between 0 and 1.  The pseudorandom number generated is
        /// from 0 (inclusive) to 1 (exclusive), that is, the returned number can be zero, but it
        /// will always be less than one. The random number generator is seeded automatically.
        /// </returns>
        [JSInternalFunction(Name = "random")]
        public static double Random()
        {
            lock (randomNumberGeneratorLock)
            {
                if (randomNumberGenerator == null)
                    randomNumberGenerator = new Random();
                return randomNumberGenerator.NextDouble();
            }
        }

        /// <summary>
        /// Returns the value of a number rounded to the nearest integer.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The required number argument is the value to be rounded to the nearest
        /// integer.  For positive numbers, if the decimal portion of number is 0.5 or greater,
        /// the return value is equal to the smallest integer greater than number. If the decimal
        /// portion is less than 0.5, the return value is the largest integer less than or equal to
        /// number.  For negative numbers, if the decimal portion is exactly -0.5, the return value
        /// is the smallest integer that is greater than the number.  For example, Math.round(8.5)
        /// returns 9, but Math.round(-8.5) returns -8. </returns>
        [JSInternalFunction(Name = "round")]
        public static double Round(double number)
        {
            if (number > 0.0)
                return Math.Floor(number + 0.5);
            if (number >= -0.5)
            {
                // BitConverter is used to distinguish positive and negative zero.
                if (BitConverter.DoubleToInt64Bits(number) == 0L)
                    return 0.0;
                return -0.0;
            }
            return Math.Floor(number + 0.5);
        }

        /// <summary>
        /// Returns the sine of an angle.
        /// </summary>
        /// <param name="angle"> The angle, in radians. </param>
        /// <returns> The sine of the <paramref name="angle"/> parameter (between -1 and 1). </returns>
        [JSInternalFunction(Name = "sin")]
        public static double Sin(double angle)
        {
            return Math.Sin(angle);
        }

        /// <summary>
        /// Returns the square root of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The square root of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "sqrt")]
        public static double Sqrt(double number)
        {
            return Math.Sqrt(number);
        }

        /// <summary>
        /// Returns the tangent of an angle.
        /// </summary>
        /// <param name="angle"> The angle, in radians. </param>
        /// <returns> The tangent of the <paramref name="angle"/> parameter (between -1 and 1). </returns>
        [JSInternalFunction(Name = "tan")]
        public static double Tan(double angle)
        {
            return Math.Tan(angle);
        }

        /// <summary>
        /// Returns the base 10 logarithm of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The base 10 logarithm of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "log10")]
        public static double Log10(double number)
        {
            return Math.Log10(number);
        }

        /// <summary>
        /// Returns the base 2 logarithm of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The base 2 logarithm of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "log2")]
        public static double Log2(double number)
        {
            return Math.Log(number) / LN2;
        }

        /// <summary>
        /// Returns the natural logarithm (base E) of one plus a number.  The result is calculated
        /// in such a way that the result is accurate even if the number is close to zero.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The natural logarithm (base E) of one plus the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "log1p")]
        public static double Log1p(double number)
        {
            if (Math.Abs(number) < 0.01)
            {
                // For small numbers, use a taylor series approximation.
                return number * (1.0 + number * (-1.0 / 2.0 + number * (1.0 / 3.0 + number *
                    (-1.0 / 4.0 + number * (1.0 / 5.0 + number * (-1.0 / 6.0 + number * (1.0 / 7.0)))))));
            }
            else
            {
                // Otherwise just use the normal log function.
                return Math.Log(1.0 + number);
            }
        }

        /// <summary>
        /// Returns E to the power of a number minus 1.  The result is calculated in such a way
        /// that the result is accurate even if the number is close to zero.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> E to the power of the <paramref name="number"/> parameter minus 1. </returns>
        [JSInternalFunction(Name = "expm1")]
        public static double Expm1(double number)
        {
            if (Math.Abs(number) < 0.01)
            {
                // For small numbers, use a taylor series approximation.
                return number * (1.0 + number * (1.0 / 2.0 + number * (1.0 / 6.0 + number *
                    (1.0 / 24.0 + number * (1.0 / 120.0 + number * (1.0 / 720.0 + number * (1.0 / 5040.0))))))); ;
            }
            else
            {
                // Otherwise just use the normal exp function.
                return Math.Exp(number) - 1.0;
            }
        }

        /// <summary>
        /// Returns the hyperbolic cosine of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The hyperbolic cosine of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "cosh")]
        public static double Cosh(double number)
        {
            return Math.Cosh(number);
        }

        /// <summary>
        /// Returns the hyperbolic sine of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The hyperbolic sine of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "sinh")]
        public static double Sinh(double number)
        {
            return Math.Sinh(number);
        }

        /// <summary>
        /// Returns the hyperbolic tangent of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The hyperbolic tangent of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "tanh")]
        public static double Tanh(double number)
        {
            return Math.Tanh(number);
        }

        /// <summary>
        /// Returns the inverse hyperbolic cosine of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The inverse hyperbolic cosine of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "acosh")]
        public static double Acosh(double number)
        {
            return Math.Log(number + Math.Sqrt((number * number) - 1.0));
        }

        /// <summary>
        /// Returns the inverse hyperbolic sine of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The inverse hyperbolic sine of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "asinh")]
        public static double Asinh(double number)
        {
            if (number == 0.0 || double.IsNegativeInfinity(number))
                return number;
            return Math.Log(number + Math.Sqrt(number * number + 1.0));
        }

        /// <summary>
        /// Returns the inverse hyperbolic tangent of a number.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The inverse hyperbolic tangent of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "atanh")]
        public static double Atanh(double number)
        {
            if (number == 0.0)
                return number;
            return Math.Log((1.0 + number) / (1.0 - number)) / 2.0;
        }


        /// <summary>
        /// Returns the square root of the sum of squares of the provided numbers.
        /// </summary>
        /// <param name="numbers"> The numbers to operate on. </param>
        /// <returns> The square root of the sum of squares of <paramref name="numbers"/>. </returns>
        [JSInternalFunction(Name = "hypot", Length = 2)]
        public static double Hypot(params double[] numbers)
        {
            if (numbers.Length == 0)
                return 0;
            if (numbers.Length == 1)
                return numbers[0];
            if (numbers.Length == 2)
                return Hypot(numbers[0], numbers[1]);

            double result = Hypot(numbers[0], numbers[1]);
            for (int i = 2; i < numbers.Length; i++)
                result = Hypot(result, numbers[i]);
            return result;
        }

        /// <summary>
        /// Returns the square root of the sum of squares of the provided numbers.
        /// </summary>
        /// <param name="number1"> The first number to operate on. </param>
        /// <param name="number2"> The second number to operate on. </param>
        /// <returns> The square root of the sum of squares of <paramref name="number1"/> and
        /// <paramref name="number2"/>. </returns>
        public static double Hypot(double number1, double number2)
        {
            double abs1 = Math.Abs(number1);
            double abs2 = Math.Abs(number2);
            double min = Math.Min(abs1, abs2);
            double max = Math.Max(abs1, abs2);
            double u = min / max;
            if (min == 0)
                return max;
            return max * Math.Sqrt(1 + u * u);
        }

        /// <summary>
        /// Returns the integral part of a number, removing any fractional digits.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The integral part of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "trunc")]
        public static double Trunc(double number)
        {
            return Math.Truncate(number);
        }

        /// <summary>
        /// Returns the sign of the x, indicating whether x is positive, negative or zero.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The sign of the <paramref name="number"/> parameter. </returns>
        [JSInternalFunction(Name = "sign")]
        public static double Sign(double number)
        {
            // If the input is negative zero, we should return negative zero.
            // If the input is NaN, we should return NaN.
            if (number == -0.0 || double.IsNaN(number))
                return number;
            return Math.Sign(number);
        }

        /// <summary>
        /// Returns the result of the 32-bit multiplication of the two parameters.
        /// </summary>
        /// <param name="number1"> The first value to multiply. </param>
        /// <param name="number2"> The second value to multiply. </param>
        /// <returns> The result of multiplying the two numbers as if they were 32-bit integers. </returns>
        [JSInternalFunction(Name = "imul")]
        public static int IMul(double number1, double number2)
        {
            return (int)(TypeConverter.ToUint32(number1) * TypeConverter.ToUint32(number2));
        }

        /// <summary>
        /// Returns the result of converting the double precision number to the nearest single
        /// precision equivalent.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The result of converting the double precision number to the nearest single
        /// precision equivalent. </returns>
        [JSInternalFunction(Name = "fround")]
        public static double Fround(double number)
        {
            return (double)(float)number;
        }

        private static readonly int[] clz32Table = new int[] {
            32, 31,  0, 16,  0, 30,  3,  0, 15,  0,  0,  0, 29, 10,  2,  0,
             0,  0, 12, 14, 21,  0, 19,  0,  0, 28,  0, 25,  0,  9,  1,  0,
            17,  0,  4,  0,  0,  0, 11,  0, 13, 22, 20,  0, 26,  0,  0, 18,
             5,  0,  0, 23,  0, 27,  0,  6,  0, 24,  7,  0,  8,  0,  0,  0
        };

        /// <summary>
        /// Converts the input value to an unsigned 32-bit integer, then returns the number of
        /// leading zero bits.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> The number of leading zero bits, treating the input like an unsigned 32-bit
        /// integer. </returns>
        [JSInternalFunction(Name = "clz32")]
        public static double Clz32(double number)
        {
            var x = TypeConverter.ToUint32(number);
            x = x | (x >> 1);       // Propagate leftmost
            x = x | (x >> 2);       // 1-bit to the right.
            x = x | (x >> 4);
            x = x | (x >> 8);
            x = x | (x >> 16);
            x = x * 0x06EB14F9;     // Multiplier is 7*255**3.
            return clz32Table[x >> 26];
        }

        /// <summary>
        /// Returns an approximation to the cube root of the input value.
        /// </summary>
        /// <param name="number"> The number to operate on. </param>
        /// <returns> An approximation to the cube root of the input value. </returns>
        [JSInternalFunction(Name = "cbrt")]
        public static double Cbrt(double number)
        {
            if (number == 0)
                return number;  // Handles zero and negative zero.
            var absResult = Math.Pow(Math.Abs(number), 1.0 / 3.0);
            return number < 0 ? -absResult : absResult;
        }
    }
}
