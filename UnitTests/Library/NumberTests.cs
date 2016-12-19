using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace UnitTests
{
    /// <summary>
    /// Test the global Number object.
    /// </summary>
    [TestClass]
    public class NumberTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Construct
            Assert.AreEqual(0, Evaluate("new Number().valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Number(undefined).valueOf()"));
            Assert.AreEqual(0, Evaluate("new Number(null).valueOf()"));
            Assert.AreEqual(1, Evaluate("new Number(true).valueOf()"));
            Assert.AreEqual(5, Evaluate("new Number(5).valueOf()"));
            Assert.AreEqual(5.1, Evaluate("new Number(5.1).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Number(NaN).valueOf()"));
            Assert.AreEqual(123, Evaluate("new Number('123').valueOf()"));
            Assert.AreEqual(9, Evaluate("new Number('0o11').valueOf()"));
            Assert.AreEqual(3, Evaluate("new Number('0b11').valueOf()"));

            // Call
            Assert.AreEqual(0, Evaluate("Number()"));
            Assert.AreEqual(double.NaN, Evaluate("Number(undefined)"));
            Assert.AreEqual(0, Evaluate("Number(null)"));
            Assert.AreEqual(1, Evaluate("Number(true)"));
            Assert.AreEqual(5, Evaluate("Number(5)"));
            Assert.AreEqual(5.1, Evaluate("Number(5.1)"));
            Assert.AreEqual(double.NaN, Evaluate("Number(NaN)"));
            Assert.AreEqual(123, Evaluate("Number('123')"));
            Assert.AreEqual(0, Evaluate("Number('')"));
            Assert.AreEqual(0, Evaluate("Number(' ')"));
            Assert.AreEqual(0, Evaluate("Number('\u00A0')"));
            Assert.AreEqual(9, Evaluate("Number('0o11')"));
            Assert.AreEqual(3, Evaluate("Number('0b11')"));

            // toString and valueOf.
            Assert.AreEqual("function Number() { [native code] }", Evaluate("Number.toString()"));
            Assert.AreEqual(true, Evaluate("Number.valueOf() === Number"));

            // length
            Assert.AreEqual(1, Evaluate("Number.length"));
        }

        [TestMethod]
        public void Properties()
        {
            // Number constants.
            Assert.AreEqual(double.MaxValue, Evaluate("Number.MAX_VALUE"));
            Assert.AreEqual(double.Epsilon, Evaluate("Number.MIN_VALUE"));
            Assert.AreEqual(double.NaN, Evaluate("Number.NaN"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Number.NEGATIVE_INFINITY"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Number.POSITIVE_INFINITY"));
            Assert.AreEqual(2.2204460492503130808472633361816e-16, Evaluate("Number.EPSILON"));
            Assert.AreEqual(9007199254740991.0, Evaluate("Number.MAX_SAFE_INTEGER"));
            Assert.AreEqual(-9007199254740991.0, Evaluate("Number.MIN_SAFE_INTEGER"));

            // Constants are non-enumerable, non-configurable, non-writable.
            Assert.AreEqual(double.MaxValue, Evaluate("Number.MAX_VALUE = 5; Number.MAX_VALUE"));
            Assert.AreEqual(false, Evaluate("delete Number.MAX_VALUE"));
            Assert.AreEqual(double.MaxValue, Evaluate("delete Number.MAX_VALUE; Number.MAX_VALUE"));

            // Constructor and __proto__
            Assert.AreEqual(true, Evaluate("new Number().constructor === Number"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(new Number()) === Number.prototype"));

            // No initial enumerable properties.
            Assert.AreEqual("", Evaluate("y = ''; for (var x in Number) { y += x } y"));
            Assert.AreEqual("", Evaluate("y = ''; for (var x in new Number(5)) { y += x } y"));
            Assert.AreEqual("", Evaluate("y = ''; for (var x in 5) { y += x } y"));

        }

        [TestMethod]
        public void toExponential()
        {
            Assert.AreEqual("0e+0", Evaluate("0 .toExponential()"));
            Assert.AreEqual("0.00e+0", Evaluate("0 .toExponential(2)"));
            Assert.AreEqual("7.71234e+1", Evaluate("77.1234.toExponential()"));
            Assert.AreEqual("7.7123e+1", Evaluate("77.1234.toExponential(4)"));
            Assert.AreEqual("7.71e+1", Evaluate("77.1234.toExponential(2)"));
            Assert.AreEqual("8e+1", Evaluate("77.1234.toExponential(0)"));
            Assert.AreEqual("7.71234e+1", Evaluate("77.1234.toExponential()"));
            Assert.AreEqual("8e+1", Evaluate("77.1234.toExponential(0)"));
            Assert.AreEqual("7.7e+1", Evaluate("77 .toExponential()"));
            Assert.AreEqual("5e-16", Evaluate("5e-16.toExponential()"));
            Assert.AreEqual("5.000e-16", Evaluate("5e-16.toExponential(3)"));
            Assert.AreEqual("1e+1", Evaluate("9.9.toExponential(0)"));
            Assert.AreEqual("1.2345678901234568e+18", Evaluate("1234567890123456789 .toExponential()"));
            Assert.AreEqual("1.23456789012345676800e+18", Evaluate("1234567890123456789 .toExponential(20)"));
            Assert.AreEqual("5e-324", Evaluate("Number.MIN_VALUE.toExponential()"));
            Assert.AreEqual("4.94e-324", Evaluate("Number.MIN_VALUE.toExponential(2)"));
            Assert.AreEqual("1.80e+308", Evaluate("Number.MAX_VALUE.toExponential(2)"));
            Assert.AreEqual("Infinity", Evaluate("Number.POSITIVE_INFINITY.toExponential()"));
            Assert.AreEqual("-Infinity", Evaluate("Number.NEGATIVE_INFINITY.toExponential()"));
            Assert.AreEqual("NaN", Evaluate("NaN.toExponential()"));

            // Negative tests.
            Assert.AreEqual("RangeError", EvaluateExceptionType("77.1234.toExponential(-1)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("77.1234.toExponential(21)"));

            // length
            Assert.AreEqual(1, Evaluate("NaN.toExponential.length"));
        }

        [TestMethod]
        public void toFixed()
        {
            Assert.AreEqual("0", Evaluate("0 .toFixed()"));
            Assert.AreEqual("0.00", Evaluate("0 .toFixed(2)"));
            Assert.AreEqual("77", Evaluate("77.1274.toFixed()"));
            Assert.AreEqual("77.1274", Evaluate("77.1274.toFixed(4)"));
            Assert.AreEqual("77.13", Evaluate("77.1274.toFixed(2)"));
            Assert.AreEqual("77", Evaluate("77.1274.toFixed(0)"));
            Assert.AreEqual("77", Evaluate("77.1234.toFixed()"));
            Assert.AreEqual("77.13", Evaluate("77.1274.toFixed(2)"));
            Assert.AreEqual("77.00", Evaluate("77 .toFixed(2)"));
            Assert.AreEqual("0.1", Evaluate("0.09.toFixed(1)"));
            Assert.AreEqual("0.2", Evaluate("0.19.toFixed(1)"));
            Assert.AreEqual("0.0", Evaluate("0.03.toFixed(1)"));
            Assert.AreEqual("-1", Evaluate("(-0.7).toFixed()"));
            Assert.AreEqual("1000000000000000", Evaluate("1e+15.toFixed()"));
            Assert.AreEqual("1e+21", Evaluate("1e21.toFixed()"));
            Assert.AreEqual("1e+21", Evaluate("1e21.toFixed(15)"));
            Assert.AreEqual("1000000000000000.00000000000000000000", Evaluate("1e+15.toFixed(20)"));
            Assert.AreEqual("0", Evaluate("1e-15.toFixed()"));
            Assert.AreEqual("0.00000000000000100000", Evaluate("1e-15.toFixed(20)"));
            Assert.AreEqual("1234567890123456768", Evaluate("1234567890123456789 .toFixed(0)"));
            Assert.AreEqual("77.12739999999999440661", Evaluate("77.1274.toFixed(20)"));
            Assert.AreEqual("Infinity", Evaluate("Number.POSITIVE_INFINITY.toFixed()"));
            Assert.AreEqual("-Infinity", Evaluate("Number.NEGATIVE_INFINITY.toFixed()"));
            Assert.AreEqual("NaN", Evaluate("NaN.toFixed()"));

            // Negative tests.
            Assert.AreEqual("RangeError", EvaluateExceptionType("77.1274.toFixed(-1)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("77.1274.toFixed(21)"));

            // length
            Assert.AreEqual(1, Evaluate("NaN.toFixed.length"));
        }

        [TestMethod]
        public void toLocaleString()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                // Culture is en-NZ.
                CultureInfo.CurrentCulture = new CultureInfo("en-nz");
                Assert.AreEqual("77", Evaluate("77 .toLocaleString()"));
                Assert.AreEqual("77.5", Evaluate("77.5.toLocaleString()"));
                Assert.AreEqual("77.123", Evaluate("77.123.toLocaleString()"));
                Assert.AreEqual("7.7e+101", Evaluate("77e100 .toLocaleString()"));
                Assert.AreEqual("123456789", Evaluate("123456789 .toLocaleString()"));
                Assert.AreEqual("-500", Evaluate("(-500).toLocaleString()"));

                // Culture is es-ES (spanish).
                CultureInfo.CurrentCulture = new CultureInfo("es-ES");
                Assert.AreEqual("77,123", Evaluate("77.123.toLocaleString()"));
                Assert.AreEqual("7,7e+101", Evaluate("77e100 .toLocaleString()"));
                Assert.AreEqual("123456789", Evaluate("123456789 .toLocaleString()"));
                Assert.AreEqual("-500", Evaluate("(-500).toLocaleString()"));
                Assert.AreEqual(CultureInfo.CurrentCulture.NumberFormat.PositiveInfinitySymbol, Evaluate("Infinity.toLocaleString()"));
                Assert.AreEqual(CultureInfo.CurrentCulture.NumberFormat.NegativeInfinitySymbol, Evaluate("(-Infinity).toLocaleString()"));
                Assert.AreEqual(CultureInfo.CurrentCulture.NumberFormat.NaNSymbol, Evaluate("NaN.toLocaleString()"));
            }
            finally
            {
                // Revert the culture back to the original value.
                CultureInfo.CurrentCulture = originalCulture;
            }

            // length
            Assert.AreEqual(0, Evaluate("NaN.toLocaleString.length"));
        }

        [TestMethod]
        public void toPrecision()
        {
            Assert.AreEqual("8e+1", Evaluate("77.1274.toPrecision(1)"));
            Assert.AreEqual("77", Evaluate("77.1274.toPrecision(2)"));
            Assert.AreEqual("77.13", Evaluate("77.1274.toPrecision(4)"));
            Assert.AreEqual("77.127", Evaluate("77.1274.toPrecision(5)"));
            Assert.AreEqual("77.1274", Evaluate("77.1274.toPrecision(6)"));
            Assert.AreEqual("77.12740", Evaluate("77.1274.toPrecision(7)"));
            Assert.AreEqual("77.1274000000000", Evaluate("77.1274.toPrecision(15)"));
            Assert.AreEqual("77.12739999999999", Evaluate("77.1274.toPrecision(16)"));
            Assert.AreEqual("77.127399999999994", Evaluate("77.1274.toPrecision(17)"));
            Assert.AreEqual("77.1273999999999944", Evaluate("77.1274.toPrecision(18)"));
            Assert.AreEqual("77.12739999999999441", Evaluate("77.1274.toPrecision(19)"));
            Assert.AreEqual("77.127399999999994407", Evaluate("77.1274.toPrecision(20)"));
            Assert.AreEqual("77.1273999999999944066", Evaluate("77.1274.toPrecision(21)"));
            Assert.AreEqual("0.0000012", Evaluate("0.00000123.toPrecision(2)"));
            Assert.AreEqual("1.2e-7", Evaluate("0.000000123.toPrecision(2)"));
            Assert.AreEqual("0.00000123000000000000008198", Evaluate("0.00000123.toPrecision(21)"));
            Assert.AreEqual("1.23e+5", Evaluate("123456 .toPrecision(3)"));
            Assert.AreEqual("1.80e+308", Evaluate("Number.MAX_VALUE.toPrecision(3)"));
            Assert.AreEqual("123456.000000000", Evaluate("123456 .toPrecision(15)"));
            Assert.AreEqual("0.0000", Evaluate("0 .toPrecision(5)"));
            Assert.AreEqual("0.000001", Evaluate("0.000001.toPrecision(1)"));
            Assert.AreEqual("0.000001000000000", Evaluate("0.000001.toPrecision(10)"));
            Assert.AreEqual("1.000000000e-7", Evaluate("0.0000001.toPrecision(10)"));
            Assert.AreEqual("1e-11", Evaluate("0.00000000001.toPrecision(1)"));
            Assert.AreEqual("1.0e-11", Evaluate("0.00000000001.toPrecision(2)"));
            Assert.AreEqual("55", Evaluate("(55).toPrecision(2)"));
            Assert.AreEqual("6e+1", Evaluate("(55).toPrecision(1)"));
            Assert.AreEqual("-55", Evaluate("(-55).toPrecision(2)"));
            Assert.AreEqual("-6e+1", Evaluate("(-55).toPrecision(1)"));
            Assert.AreEqual("1e+1", Evaluate("9.59.toPrecision(1)"));
            Assert.AreEqual("9.6", Evaluate("9.59.toPrecision(2)"));
            Assert.AreEqual("9.9", Evaluate("9.95.toPrecision(2)"));
            Assert.AreEqual("10", Evaluate("9.96.toPrecision(2)"));
            Assert.AreEqual("-6e+20", Evaluate("(-555555555555555555555).toPrecision(1)"));
            Assert.AreEqual("-6e+21", Evaluate("(-5555555555555555555555).toPrecision(1)"));
            Assert.AreEqual("18014398509482012.0000", Evaluate("18014398509482012 .toPrecision(21)"));
            Assert.AreEqual("180143985094820134912", Evaluate("180143985094820121234 .toPrecision(21)"));
            Assert.AreEqual("0.100000000000000005551", Evaluate("0.1.toPrecision(21)"));
            Assert.AreEqual("4.9e-324", Evaluate("Number.MIN_VALUE.toPrecision(2)"));
            Assert.AreEqual("4.94065645841246544177e-324", Evaluate("Number.MIN_VALUE.toPrecision(21)"));

            // If the precision argument is undefined, it is the same as toString().
            Assert.AreEqual("77.1274", Evaluate("77.1274.toPrecision()"));
            Assert.AreEqual("77.1274", Evaluate("77.1274.toPrecision(undefined)"));

            // NaN & infinity
            Assert.AreEqual("NaN", Evaluate("Number.NaN.toPrecision()"));
            Assert.AreEqual("NaN", Evaluate("Number.NaN.toPrecision(3)"));
            Assert.AreEqual("Infinity", Evaluate("Number.POSITIVE_INFINITY.toPrecision()"));
            Assert.AreEqual("Infinity", Evaluate("Number.POSITIVE_INFINITY.toPrecision(3)"));
            Assert.AreEqual("-Infinity", Evaluate("Number.NEGATIVE_INFINITY.toPrecision()"));
            Assert.AreEqual("-Infinity", Evaluate("Number.NEGATIVE_INFINITY.toPrecision(3)"));

            // Precision out of range.
            Assert.AreEqual("RangeError", EvaluateExceptionType("77.1274.toPrecision(-1)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("77.1274.toPrecision(0)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("77.1274.toPrecision(22)"));

            // length
            Assert.AreEqual(1, Evaluate("NaN.toPrecision.length"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("12345", Evaluate("12345 .toString()"));
            Assert.AreEqual("18014398509481990", Evaluate("18014398509481992 .toString()"));
            Assert.AreEqual("18014398509482010", Evaluate("18014398509482008 .toString()"));
            Assert.AreEqual("18014398509482012", Evaluate("18014398509482012 .toString()"));
            Assert.AreEqual("18014398509481988", Evaluate("18014398509481988 .toString()"));
            Assert.AreEqual("1234567890123456800", Evaluate("1234567890123456789 .toString()"));
            Assert.AreEqual("2234567890123456800", Evaluate("2234567890123456789 .toString()"));
            Assert.AreEqual("3234567890123457000", Evaluate("3234567890123456789 .toString()"));
            Assert.AreEqual("4234567890123457000", Evaluate("4234567890123456789 .toString()"));
            Assert.AreEqual("72057594037927940", Evaluate("72057594037927944 .toString()"));
            Assert.AreEqual("72057594037927950", Evaluate("72057594037927945 .toString()"));
            Assert.AreEqual("72057594037927950", Evaluate("72057594037927959 .toString()"));
            Assert.AreEqual("9.999999999999998", Evaluate("9.999999999999999.toString()"));
            Assert.AreEqual("10", Evaluate("9.9999999999999999.toString()"));
            Assert.AreEqual("100000000000000000000", Evaluate("99999999999999999999 .toString()"));
            Assert.AreEqual("999999999999990000000", Evaluate("999999999999990000000 .toString()"));
            Assert.AreEqual("1e+21", Evaluate("999999999999999999999 .toString()"));
            Assert.AreEqual("100000000000000000000", Evaluate("99999999999999999999 .toString()"));
            Assert.AreEqual("-77", Evaluate("(-77).toString()"));
            Assert.AreEqual("77.1274", Evaluate("77.1274.toString()"));
            Assert.AreEqual("77.001", Evaluate("77.001.toString()"));
            Assert.AreEqual("77.12345678901235", Evaluate("77.1234567890123456789.toString()"));
            Assert.AreEqual("7.123456789012345", Evaluate("7.1234567890123456789.toString()"));
            Assert.AreEqual("0.000005", Evaluate("5e-6.toString()"));
            Assert.AreEqual("0.000001", Evaluate("1e-6.toString()"));
            Assert.AreEqual("5e-7", Evaluate("5e-7.toString()"));
            Assert.AreEqual("1e-7", Evaluate("1e-7.toString()"));
            Assert.AreEqual("1000000000000000", Evaluate("1e15.toString()"));
            Assert.AreEqual("10000000000000000", Evaluate("1e16.toString()"));
            Assert.AreEqual("100000000000000000", Evaluate("1e17.toString()"));
            Assert.AreEqual("1000000000000000000", Evaluate("1e18.toString()"));
            Assert.AreEqual("10000000000000000000", Evaluate("1e19.toString()"));
            Assert.AreEqual("100000000000000000000", Evaluate("1e20.toString()"));
            Assert.AreEqual("1e+21", Evaluate("1e21.toString()"));
            Assert.AreEqual("1e+21", Evaluate("999999999999999999999 .toString()"));
            Assert.AreEqual("100111122133144160", Evaluate("100111122133144155 .toString()"));
            Assert.AreEqual("Infinity", Evaluate("Infinity.toString()"));
            Assert.AreEqual("-Infinity", Evaluate("(-Infinity).toString()"));
            Assert.AreEqual("NaN", Evaluate("NaN.toString()"));
            Assert.AreEqual("1.7976931348623157e+308", Evaluate("Number.MAX_VALUE.toString()"));
            Assert.AreEqual("5e-324", Evaluate("Number.MIN_VALUE.toString()"));

            // Radix which is not 10.
            Assert.AreEqual("115", Evaluate("77 .toString(8)"));
            Assert.AreEqual("1001", Evaluate("9 .toString(2)"));
            Assert.AreEqual("fe", Evaluate("254 .toString(16)"));
            Assert.AreEqual("-115.4621320712601014", Evaluate("(-77.598).toString(8)"));
            Assert.AreEqual("0.00142233513615237575", Evaluate("0.003.toString(8)"));
            Assert.AreEqual("27524716460150203300000000000000000", Evaluate("15e30.toString(8)"));
            Assert.AreEqual("0.252525252525252525", Evaluate("(1/3).toString(8)"));

            // Decimal point should be '.' regardless of locale.
            Assert.AreEqual("77.1274", ChangeLocale("es-ES", () => Evaluate("77.1274.toString()")));

            // Radix out of range.
            Assert.AreEqual("RangeError", EvaluateExceptionType("254 .toString(37)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("254 .toString(1)"));

            // length
            Assert.AreEqual(1, Evaluate("NaN.toString.length"));
        }

        /// <summary>
        /// Converts a double precision floating point number to binary scientific notation.
        /// </summary>
        /// <param name="value"> The floating point number to convert. </param>
        /// <returns> A string containing the number in binary scientific notation. </returns>
        private static string ToBinary(object value)
        {
            double value2;
            if (value is int)
                value2 = (double)(int)value;
            else if (value is double)
                value2 = (double)value;
            else
                throw new ArgumentException("value must be a number.", "value");

            var output = new System.Text.StringBuilder();

            long bits = BitConverter.DoubleToInt64Bits(value2);

            // Sign bit.
            if (bits < 0)
                output.Append("-");

            // Exponent.
            var exponent = (int)((bits & 0x7FF0000000000000) >> 52);

            // Mantissa.
            long mantissa = bits & 0xFFFFFFFFFFFFF;

            // Infinity and NaN.
            if (exponent == 0x7ff)
            {
                if (mantissa == 0)
                    output.Append("Infinity");
                else
                    output.Append("NaN");
                return output.ToString();
            }

            exponent -= 1023;
            mantissa += 0x10000000000000;

            int log2 = 0;
            long temp = mantissa;
            while ((temp >>= 1) > 0)
            {
                log2++;
            }

            output.Append("1.");
            for (int i = 0; i < 52; i++)
            {
                output.Append((int)(mantissa >> (51 - i)) & 1);
            }

            output.Append(" x 2^");
            output.Append(exponent);

            return output.ToString();
        }

        [TestMethod]
        public void isFinite()
        {
            Assert.AreEqual(true, Evaluate("Number.isFinite(0)"));
            Assert.AreEqual(true, Evaluate("Number.isFinite(123.456)"));
            Assert.AreEqual(false, Evaluate("Number.isFinite(Infinity)"));
            Assert.AreEqual(false, Evaluate("Number.isFinite(-Infinity)"));
            Assert.AreEqual(false, Evaluate("Number.isFinite(NaN)"));
            Assert.AreEqual(false, Evaluate("Number.isFinite('0')"));
        }

        [TestMethod]
        public void isNaN()
        {
            Assert.AreEqual(false, Evaluate("Number.isNaN(0)"));
            Assert.AreEqual(false, Evaluate("Number.isNaN(123.456)"));
            Assert.AreEqual(false, Evaluate("Number.isNaN(Infinity)"));
            Assert.AreEqual(false, Evaluate("Number.isNaN(-Infinity)"));
            Assert.AreEqual(true, Evaluate("Number.isNaN(NaN)"));
            Assert.AreEqual(false, Evaluate("Number.isNaN('0')"));
        }

        [TestMethod]
        public void isInteger()
        {
            Assert.AreEqual(true, Evaluate("Number.isInteger(0)"));
            Assert.AreEqual(true, Evaluate("Number.isInteger(9007199254740994)"));
            Assert.AreEqual(false, Evaluate("Number.isInteger(123.456)"));
            Assert.AreEqual(false, Evaluate("Number.isInteger(Infinity)"));
            Assert.AreEqual(false, Evaluate("Number.isInteger(-Infinity)"));
            Assert.AreEqual(false, Evaluate("Number.isInteger(NaN)"));
            Assert.AreEqual(false, Evaluate("Number.isInteger('0')"));
        }

        [TestMethod]
        public void isSafeInteger()
        {
            Assert.AreEqual(true, Evaluate("Number.isSafeInteger(0)"));
            Assert.AreEqual(false, Evaluate("Number.isSafeInteger(9007199254740994)"));
            Assert.AreEqual(false, Evaluate("Number.isSafeInteger(9007199254740992)"));
            Assert.AreEqual(true, Evaluate("Number.isSafeInteger(9007199254740991)"));
            Assert.AreEqual(false, Evaluate("Number.isSafeInteger(-9007199254740994)"));
            Assert.AreEqual(true, Evaluate("Number.isSafeInteger(-9007199254740991)"));
            Assert.AreEqual(true, Evaluate("Number.isSafeInteger(12)"));
            Assert.AreEqual(true, Evaluate("Number.isSafeInteger(-12)"));
            Assert.AreEqual(false, Evaluate("Number.isSafeInteger(123.456)"));
            Assert.AreEqual(false, Evaluate("Number.isSafeInteger(Infinity)"));
            Assert.AreEqual(false, Evaluate("Number.isSafeInteger(-Infinity)"));
            Assert.AreEqual(false, Evaluate("Number.isSafeInteger(NaN)"));
            Assert.AreEqual(false, Evaluate("Number.isSafeInteger('0')"));
        }

        [TestMethod]
        public void parseFloat()
        {
            Assert.AreEqual(34, Evaluate("Number.parseFloat('34')"));
            Assert.AreEqual(34.5, Evaluate("Number.parseFloat('34.5')"));
            Assert.AreEqual(3400, Evaluate("Number.parseFloat('34e2')"));
            Assert.AreEqual(3.45, Evaluate("Number.parseFloat('34.5e-1')"));
            Assert.AreEqual(0.345, Evaluate("Number.parseFloat('34.5E-2')"));
            Assert.AreEqual(-34, Evaluate("Number.parseFloat('-34')"));
            Assert.AreEqual(34, Evaluate("Number.parseFloat('+34')"));
            Assert.AreEqual(11, Evaluate("Number.parseFloat('011')"));
            Assert.AreEqual(11, Evaluate("Number.parseFloat(' 11')"));
            Assert.AreEqual(0.5, Evaluate("Number.parseFloat('.5')"));
            Assert.AreEqual(0.1, Evaluate("Number.parseFloat('.1')"));
            Assert.AreEqual(0.01, Evaluate("Number.parseFloat('.01')"));
            Assert.AreEqual(0.07, Evaluate("Number.parseFloat('.7e-1')"));
            Assert.AreEqual(-0.5, Evaluate("Number.parseFloat('-.5')"));
            Assert.AreEqual(5, Evaluate("Number.parseFloat('5e')"));
            Assert.AreEqual(5, Evaluate("Number.parseFloat('5.e')"));
            Assert.AreEqual(5, Evaluate("Number.parseFloat('5e.5')"));
            Assert.AreEqual(12, Evaluate("Number.parseFloat('12x3')"));
            Assert.AreEqual(double.NaN, Evaluate("Number.parseFloat('')"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Number.parseFloat('Infinity')"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Number.parseFloat('-Infinity')"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Number.parseFloat(' Infinity')"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Number.parseFloat('InfinityZ')"));
            Assert.AreEqual(0, Evaluate("Number.parseFloat('0xff')"));
            Assert.AreEqual(0, Evaluate("Number.parseFloat('0x')"));
            Assert.AreEqual(0, Evaluate("Number.parseFloat('0zff')"));
            Assert.AreEqual(double.NaN, Evaluate("Number.parseFloat('infinity')"));
            Assert.AreEqual(-1.1, Evaluate("Number.parseFloat('\u205F -1.1')"));

            // Very large numbers.
            Assert.AreEqual(18446744073709551616d, Evaluate("Number.parseFloat('18446744073709551616')"));
            Assert.AreEqual(295147905179352825856d, Evaluate("Number.parseFloat('295147905179352825856')"));
            Assert.AreEqual(4722366482869645213696d, Evaluate("Number.parseFloat('4722366482869645213696')"));
            Assert.AreEqual(75557863725914323419136d, Evaluate("Number.parseFloat('75557863725914323419136')"));
        }

        [TestMethod]
        public void parseInt()
        {
            Assert.AreEqual(1, Evaluate("Number.parseInt('1')"));
            Assert.AreEqual(123, Evaluate("Number.parseInt('123')"));
            Assert.AreEqual(65, Evaluate("Number.parseInt('65')"));
            Assert.AreEqual(987654, Evaluate("Number.parseInt('987654')"));
            Assert.AreEqual(10000000, Evaluate("Number.parseInt('10000000')"));
            Assert.AreEqual(10000001, Evaluate("Number.parseInt('10000001')"));
            Assert.AreEqual(987654321, Evaluate("Number.parseInt('987654321')"));
            Assert.AreEqual(9876543212d, Evaluate("Number.parseInt('9876543212')"));
            Assert.AreEqual(987654321234d, Evaluate("Number.parseInt('987654321234')"));
            Assert.AreEqual(-987654321234d, Evaluate("Number.parseInt('-987654321234')"));
            Assert.AreEqual(9876543212345d, Evaluate("Number.parseInt('9876543212345')"));
            Assert.AreEqual(98765432123456d, Evaluate("Number.parseInt('98765432123456')"));
            Assert.AreEqual(987654321234567d, Evaluate("Number.parseInt('987654321234567')"));
            Assert.AreEqual(9876543212345678d, Evaluate("Number.parseInt('9876543212345678')"));
            Assert.AreEqual(98765432123456789d, Evaluate("Number.parseInt('98765432123456789')"));
            Assert.AreEqual(-98765432123456789d, Evaluate("Number.parseInt('-98765432123456789')"));
            Assert.AreEqual(18446744073709551616d, Evaluate("Number.parseInt('18446744073709551616')"));
            Assert.AreEqual(295147905179352825856d, Evaluate("Number.parseInt('295147905179352825856')"));
            Assert.AreEqual(4722366482869645213696d, Evaluate("Number.parseInt('4722366482869645213696')"));
            Assert.AreEqual(75557863725914323419136d, Evaluate("Number.parseInt('75557863725914323419136')"));

            // Sign.
            Assert.AreEqual(-123, Evaluate("Number.parseInt('-123')"));
            Assert.AreEqual(123, Evaluate("Number.parseInt('+123')"));

            // Empty string should produce NaN.
            Assert.AreEqual(double.NaN, Evaluate("Number.parseInt('')"));

            // Leading whitespace should be skipped.
            Assert.AreEqual(1, Evaluate("Number.parseInt('  1')"));
            Assert.AreEqual(1, Evaluate("Number.parseInt('  1.5')"));
            Assert.AreEqual(35, Evaluate("Number.parseInt('\t35')"));

            // Hex prefix should be respected.
            Assert.AreEqual(17, Evaluate("Number.parseInt('0x11')"));
            Assert.AreEqual(1.512366075204171e+36, Evaluate("Number.parseInt('0x123456789abcdef0123456789abcdef')"));

            // Bases.
            Assert.AreEqual(17, Evaluate("Number.parseInt('0x11', 16)"));
            Assert.AreEqual(90, Evaluate("Number.parseInt('0X5a', 16)"));
            Assert.AreEqual(17, Evaluate("Number.parseInt('11', 16)"));
            Assert.AreEqual(2748, Evaluate("Number.parseInt('abc', 16)"));
            Assert.AreEqual(3, Evaluate("Number.parseInt('11', 2)"));
            Assert.AreEqual(16, Evaluate("Number.parseInt('0x10')"));
            Assert.AreEqual(4096, Evaluate("Number.parseInt('0x1000')"));
            Assert.AreEqual(1048576, Evaluate("Number.parseInt('0x100000')"));
            Assert.AreEqual(268435456, Evaluate("Number.parseInt('0x10000000')"));
            Assert.AreEqual(68719476736d, Evaluate("Number.parseInt('0x1000000000')"));
            Assert.AreEqual(17592186044416d, Evaluate("Number.parseInt('0x100000000000')"));
            Assert.AreEqual(4503599627370496d, Evaluate("Number.parseInt('0x10000000000000')"));
            Assert.AreEqual(1152921504606847000d, Evaluate("Number.parseInt('0x1000000000000000')"));
            Assert.AreEqual(295147905179352830000d, Evaluate("Number.parseInt('0x100000000000000000')"));
            Assert.AreEqual(7.555786372591432e+22, Evaluate("Number.parseInt('0x10000000000000000000')"));
            Assert.AreEqual(1.9342813113834067e+25, Evaluate("Number.parseInt('0x1000000000000000000000')"));

            // Base out of range.
            Assert.AreEqual(double.NaN, Evaluate("Number.parseInt('11', 1)"));
            Assert.AreEqual(11, Evaluate("Number.parseInt('11', 0)"));
            Assert.AreEqual(double.NaN, Evaluate("Number.parseInt('11', -1)"));

            // Hex prefix should not be respected if base is specified explicitly.
            Assert.AreEqual(0, Evaluate("Number.parseInt('0x11', 10)"));

            // Junk characters and out of range characters should stop parsing
            Assert.AreEqual(123, Evaluate("Number.parseInt('123x456')"));
            Assert.AreEqual(1, Evaluate("Number.parseInt('1a')"));
            Assert.AreEqual(double.NaN, Evaluate("Number.parseInt('a')"));
            Assert.AreEqual(1, Evaluate("Number.parseInt('19', 8)"));

            // Invalid prefix.
            Assert.AreEqual(0, Evaluate("Number.parseInt('0z11', 10)"));
            Assert.AreEqual(0, Evaluate("Number.parseInt('0z11')"));

            // Radix uses ToInt32() so has weird wrapping issues.
            Assert.AreEqual(19, Evaluate("Number.parseInt('23', 4294967304)"));

            // Octal parsing (only works in compatibility mode).
            CompatibilityMode = Jurassic.CompatibilityMode.ECMAScript3;
            try
            {
                Assert.AreEqual(9, Evaluate("Number.parseInt('011')"));
                Assert.AreEqual(0, Evaluate("Number.parseInt('08')"));
                Assert.AreEqual(1, Evaluate("Number.parseInt('018')"));
                Assert.AreEqual(11, Evaluate("Number.parseInt('011', 10)"));
            }
            finally
            {
                CompatibilityMode = Jurassic.CompatibilityMode.Latest;
            }

            // Octal parsing was removed from ES5.
            Assert.AreEqual(11, Evaluate("Number.parseInt('011')"));
            Assert.AreEqual(8, Evaluate("Number.parseInt('08')"));
            Assert.AreEqual(18, Evaluate("Number.parseInt('018')"));
            Assert.AreEqual(11, Evaluate("Number.parseInt('011', 10)"));

            // Large numbers.
            Assert.AreEqual(9214843084008499.0, Evaluate("Number.parseInt('9214843084008499')"));
            Assert.AreEqual(18014398509481993.0, Evaluate("Number.parseInt('18014398509481993')"));
        }

        [TestMethod]
        public void clz()
        {
            Assert.AreEqual(32, Evaluate("0 .clz()"));
            Assert.AreEqual(31, Evaluate("1 .clz()"));
            Assert.AreEqual(28, Evaluate("12 .clz()"));
            Assert.AreEqual(0, Evaluate("4294967275 .clz()"));
            Assert.AreEqual(0, Evaluate("4294967295 .clz()"));
            Assert.AreEqual(16, Evaluate("60000 .clz()"));
            Assert.AreEqual(0, Evaluate("(-1).clz()"));
            Assert.AreEqual(31, Evaluate("1.9.clz()"));
        }
    }
}
