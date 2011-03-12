using System;
using System.Numerics;

namespace Jurassic
{

    /// <summary>
    /// Converts numbers into strings.
    /// </summary>
    internal static class NumberFormatter
    {
        /// <summary>
        /// Used to specify the type of number formatting that should be applied.
        /// </summary>
        internal enum Style
        {
            /// <summary>
            /// Specifies that the shortest number that accurately represents the number should be
            /// displayed.  Scientific notation is used if the exponent is less than -6 or greater
            /// than twenty.  The precision parameter has no semantic meaning.
            /// </summary>
            Regular,

            /// <summary>
            /// Specifies that a fixed number of significant figures should be displayed (specified
            /// by the precision parameter).  If the number cannot be displayed using the given
            /// number of digits, scientific notation is used.
            /// </summary>
            Precision,

            /// <summary>
            /// Specifies that a fixed number of digits should be displayed after the decimal place
            /// (specified by the precision parameter).  Scientific notation is used if the
            /// exponent is greater than twenty.
            /// </summary>
            Fixed,
            
            /// <summary>
            /// Specifies that numbers should always be displayed in scientific notation.  The
            /// precision parameter specifies the number of figures to display after the decimal
            /// point.
            /// </summary>
            Exponential,
        }

        /// <summary>
        /// Converts a number to a string.
        /// </summary>
        /// <param name="value"> The value to convert to a string. </param>
        /// <param name="radix"> The base of the number system to convert to. </param>
        /// <param name="style"> The type of formatting to apply. </param>
        /// <param name="precision">
        /// This value is dependent on the formatting style:
        /// Regular - this value has no meaning.
        /// Precision - the number of significant figures to display.
        /// Fixed - the number of figures to display after the decimal point.
        /// Exponential - the number of figures to display after the decimal point.
        /// </param>
        internal static string ToString(double value, int radix, Style style, int precision = 0)
        {
            // Handle NaN.
            if (double.IsNaN(value))
                return "NaN";

            // Handle zero.
            if (value == 0.0)
            {
                switch (style)
                {
                    case Style.Regular:
                        return "0";
                    case Style.Precision:
                        return "0." + new string('0', precision - 1);
                    case Style.Fixed:
                        if (precision == 0)
                            return "0";
                        return "0." + new string('0', precision);
                    case Style.Exponential:
                        if (precision <= 0)
                            return "0e+0";
                        return string.Format("0.{0}e+0", new string('0', precision));
                }
            }

            var result = new System.Text.StringBuilder(10);

            // Handle negative numbers.
            if (value < 0)
            {
                value = -value;
                result.Append('-');
            }

            // Handle infinity.
            if (double.IsPositiveInfinity(value))
            {
                result.Append("Infinity");
                return result.ToString();
            }

            // Calculate the logarithm of the number.
            int exponent;
            if (radix == 10)
                exponent = (int)Math.Floor(Math.Log10(value));
            else
                exponent = (int)Math.Floor(Math.Log(value, radix));

            // toFixed acts like toString() if the exponent is >= 21.
            if (style == Style.Fixed && exponent >= 21)
                style = Style.Regular;

            // Calculate the number of significant digits.
            // We add 5 so that there is enough precision to distinguish halfway numbers.
            int significantDigits = radix == 10 ? 22 : (int)Math.Floor(53 / Math.Log(radix, 2)) + 7;

            // Calculate the number of integral digits, or if negative, the number of zeros after
            // the decimal point.
            int integralDigits = exponent + 1;

            // Scale the value so it is a 22 digit (for base 10) integer.
            var integerValue = ConvertDoubleToScaledInteger(value, radix, significantDigits - integralDigits);

            // Calculate the error.
            BigInteger errorDelta;
            switch (style)
            {
                case Style.Regular:
                    errorDelta = ConvertDoubleToScaledInteger(CalculateError(value), radix, significantDigits - integralDigits) >> 1;
                    break;
                case Style.Precision:
                    errorDelta = BigInteger.Pow(radix, significantDigits - precision) >> 1;
                    break;
                case Style.Fixed:
                    errorDelta = BigInteger.Pow(radix, Math.Max(0, significantDigits - integralDigits - precision)) >> 1;
                    break;
                case Style.Exponential:
                    if (precision < 0)
                        errorDelta = ConvertDoubleToScaledInteger(CalculateError(value), radix, significantDigits - integralDigits) >> 1;
                    else
                        errorDelta = BigInteger.Pow(radix, significantDigits - precision - 1) >> 1;
                    break;
                default:
                    throw new ArgumentException("Unknown formatting style.", "style");
            }

            // Shrink the error in the case where ties are resolved towards the value with the 
            // least significant bit set to zero.
            if ((BitConverter.DoubleToInt64Bits(value) & 1) == 1)
                errorDelta--;

            // Calculate the exponent thresholds.
            int lowExponentThreshold = int.MinValue;
            if (radix == 10 && style != Style.Fixed)
                lowExponentThreshold = -7;
            if (style == Style.Exponential)
                lowExponentThreshold = -1;
            int highExponentThreshold = int.MaxValue;
            if (radix == 10 && style == Style.Regular)
                highExponentThreshold = 21;
            if (style == Style.Precision)
                highExponentThreshold = precision;
            if (style == Style.Exponential)
                highExponentThreshold = 0;

            // toFixed with a low precision causes rounding.
            if (style == Style.Fixed && precision <= -integralDigits)
            {
                int diff = (-integralDigits) - (precision - 1);
                significantDigits += diff;
                integralDigits += diff;
            }

            // Output any leading zeros.
            bool decimalPointOutput = false;
            if (integralDigits <= 0 && integralDigits > lowExponentThreshold + 1)
            {
                result.Append('0');
                if (integralDigits < 0)
                {
                    result.Append('.');
                    decimalPointOutput = true;
                    result.Append('0', -integralDigits);
                }
            }

            // Output the digits.
            int zeroCount = 0;
            BigInteger digits = BigInteger.Zero;
            int digitsOutput = 0;
            bool rounded = false, scientificNotation = false;
            for (; digitsOutput < significantDigits && rounded == false; digitsOutput++)
            {
                // Calculate the next digit.
                var scaleFactor = BigInteger.Pow(radix, significantDigits - 1 - digitsOutput);
                var digit = (int)(integerValue / scaleFactor);
                integerValue -= digit * scaleFactor;

                if (integerValue <= errorDelta && integerValue < (scaleFactor >> 1))
                {
                    // Round down.
                    rounded = true;
                }
                else if (scaleFactor - integerValue <= errorDelta)
                {
                    // Round up.
                    rounded = true;
                    digit++;
                    if (digit == radix)
                    {
                        digit = 1;
                        exponent++;
                    }
                }

                if (digit > 0 || decimalPointOutput == false)
                {
                    // Check if the decimal point should be output.
                    if (decimalPointOutput == false && (scientificNotation == true || digitsOutput == integralDigits))
                    {
                        result.Append('.');
                        decimalPointOutput = true;
                    }

                    // Output any pent-up zeros.
                    if (zeroCount > 0)
                    {
                        result.Append('0', zeroCount);
                        zeroCount = 0;
                    }

                    // Output the next digit.
                    if (digit < 10)
                        result.Append((char)(digit + '0'));
                    else
                        result.Append((char)(digit - 10 + 'a'));
                }
                else
                    zeroCount++;

                // Check whether the number should be displayed in scientific notation (we cannot
                // determine this up front for large exponents because the number might get rounded
                // up to cross the threshold).
                if (digitsOutput == 0 && (exponent <= lowExponentThreshold || exponent >= highExponentThreshold))
                    scientificNotation = true;
            }

            // Add any extra zeros on the end, if necessary.
            if (scientificNotation == false && integralDigits > digitsOutput)
            {
                result.Append('0', integralDigits - digitsOutput);
                digitsOutput = integralDigits;
            }

            // Most of the styles output redundent zeros.
            int redundentZeroCount = 0;
            switch (style)
            {
                case Style.Precision:
                    redundentZeroCount = zeroCount + precision - digitsOutput;
                    break;
                case Style.Fixed:
                    redundentZeroCount = precision - (digitsOutput - zeroCount - integralDigits);
                    break;
                case Style.Exponential:
                    redundentZeroCount = precision - (digitsOutput - zeroCount) + 1;
                    break;
            }
            if (redundentZeroCount > 0)
            {
                if (decimalPointOutput == false)
                    result.Append('.');
                result.Append('0', redundentZeroCount);
            }

            if (scientificNotation == true)
            {
                // Add the exponent on the end.
                result.Append('e');
                if (exponent > 0)
                    result.Append('+');
                result.Append(exponent);
            }

            return result.ToString();
        }



        //     PRIVATE HELPER METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Calculates the minimum increment that creates a number distinct from the value that was
        /// provided.  The error for the number is plus or minus half the result of this method
        /// (note that the number returned may be so small that halfing it produces zero).
        /// </summary>
        /// <param name="value"> The number to calculate an error value for. </param>
        /// <returns> The minimum increment that creates a number distinct from the value that was
        /// provided. </returns>
        private static double CalculateError(double value)
        {
            long bits = BitConverter.DoubleToInt64Bits(value);

            // Extract the base-2 exponent.
            var base2Exponent = (int)((bits & 0x7FF0000000000000) >> 52);

            // Handle denormals.
            if (base2Exponent == 0)
                return double.Epsilon;

            // Handle very small numbers.
            if (base2Exponent < 53)
                return BitConverter.Int64BitsToDouble(1L << (base2Exponent - 1));

            // Subtract 52 from the exponent to get the error (52 is the number of bits in the mantissa).
            return BitConverter.Int64BitsToDouble((long)(base2Exponent - 52) << 52);
        }

        /// <summary>
        /// Scales the given double-precision number using the given scale factor.
        /// </summary>
        /// <param name="value"> The value to scale. </param>
        /// <param name="radix"> The base of the scale factor. </param>
        /// <param name="exponent"> The exponent of the scale factor. </param>
        /// <returns> A BigInteger containing the scaled integer. </returns>
        private static BigInteger ConvertDoubleToScaledInteger(double value, int radix, int exponent)
        {
            long bits = BitConverter.DoubleToInt64Bits(value);

            // Extract the base-2 exponent.
            var base2Exponent = (int)((bits & 0x7FF0000000000000) >> 52) - 1023;

            // Extract the mantissa.
            long mantissa = bits & 0xFFFFFFFFFFFFF;
            if (base2Exponent > -1023)
            {
                mantissa |= 0x10000000000000;
                base2Exponent -= 52;
            }
            else
            {
                // Denormals.
                base2Exponent -= 51;
            }

            // Extract the sign bit.
            if (bits < 0)
                mantissa = -mantissa;

            var result = new BigInteger(mantissa);

            // Scale the result using the given radix and exponent.
            if (exponent > 0)
                result *= BigInteger.Pow(radix, exponent);
            if (base2Exponent > 0)
                result *= BigInteger.Pow(2, base2Exponent);
            else if (base2Exponent < 0)
            {
                // Rounded divide.
                var divisor = BigInteger.Pow(2, -base2Exponent);
                result = (result + (divisor >> 1)) / divisor;
            }
            if (exponent < 0)
            {
                // Rounded divide.
                var divisor = BigInteger.Pow(radix, -exponent);
                result = (result + (divisor >> 1)) / divisor;
            }

            return result;
        }
    }

}
