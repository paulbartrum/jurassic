using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the global Number object.
    /// </summary>
    [TestClass]
    public class NumberTests
    {
        


        [TestMethod]
        public void Constructor()
        {
            // Construct
            Assert.AreEqual(0, TestUtils.Evaluate("new Number().valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Number(undefined).valueOf()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Number(null).valueOf()"));
            Assert.AreEqual(1, TestUtils.Evaluate("new Number(true).valueOf()"));
            Assert.AreEqual(5, TestUtils.Evaluate("new Number(5).valueOf()"));
            Assert.AreEqual(5.1, TestUtils.Evaluate("new Number(5.1).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Number(NaN).valueOf()"));
            Assert.AreEqual(123, TestUtils.Evaluate("new Number('123').valueOf()"));

            // Call
            Assert.AreEqual(0, TestUtils.Evaluate("Number()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Number(undefined)"));
            Assert.AreEqual(0, TestUtils.Evaluate("Number(null)"));
            Assert.AreEqual(1, TestUtils.Evaluate("Number(true)"));
            Assert.AreEqual(5, TestUtils.Evaluate("Number(5)"));
            Assert.AreEqual(5.1, TestUtils.Evaluate("Number(5.1)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Number(NaN)"));
            Assert.AreEqual(123, TestUtils.Evaluate("Number('123')"));
            Assert.AreEqual(0, TestUtils.Evaluate("Number('')"));
            Assert.AreEqual(0, TestUtils.Evaluate("Number(' ')"));
            Assert.AreEqual(0, TestUtils.Evaluate("Number('\u00A0')"));

            // toString and valueOf.
            Assert.AreEqual("function Number() { [native code] }", TestUtils.Evaluate("Number.toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("Number.valueOf() === Number"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Number.length"));
        }

        [TestMethod]
        public void Properties()
        {
            // Number constants.
            Assert.AreEqual(double.MaxValue, TestUtils.Evaluate("Number.MAX_VALUE"));
            Assert.AreEqual(double.Epsilon, TestUtils.Evaluate("Number.MIN_VALUE"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Number.NaN"));
            Assert.AreEqual(double.NegativeInfinity, TestUtils.Evaluate("Number.NEGATIVE_INFINITY"));
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("Number.POSITIVE_INFINITY"));

            // Constants are non-enumerable, non-configurable, non-writable.
            Assert.AreEqual(double.MaxValue, TestUtils.Evaluate("Number.MAX_VALUE = 5; Number.MAX_VALUE"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete Number.MAX_VALUE"));
            Assert.AreEqual(double.MaxValue, TestUtils.Evaluate("delete Number.MAX_VALUE; Number.MAX_VALUE"));

            // Constructor and __proto__
            Assert.AreEqual(true, TestUtils.Evaluate("new Number().constructor === Number"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new Number()) === Number.prototype"));

            // No initial enumerable properties.
            Assert.AreEqual("", TestUtils.Evaluate("y = ''; for (var x in Number) { y += x } y"));
            Assert.AreEqual("", TestUtils.Evaluate("y = ''; for (var x in new Number(5)) { y += x } y"));
            Assert.AreEqual("", TestUtils.Evaluate("y = ''; for (var x in 5) { y += x } y"));
            
        }

        [TestMethod]
        public void toExponential()
        {
            Assert.AreEqual("0e+0", TestUtils.Evaluate("0 .toExponential()"));
            Assert.AreEqual("0.00e+0", TestUtils.Evaluate("0 .toExponential(2)"));
            Assert.AreEqual("7.71234e+1", TestUtils.Evaluate("77.1234.toExponential()"));
            Assert.AreEqual("7.7123e+1", TestUtils.Evaluate("77.1234.toExponential(4)"));
            Assert.AreEqual("7.71e+1", TestUtils.Evaluate("77.1234.toExponential(2)"));
            Assert.AreEqual("8e+1", TestUtils.Evaluate("77.1234.toExponential(0)"));
            Assert.AreEqual("7.71234e+1", TestUtils.Evaluate("77.1234.toExponential()"));
            Assert.AreEqual("8e+1", TestUtils.Evaluate("77.1234.toExponential(0)"));
            Assert.AreEqual("7.7e+1", TestUtils.Evaluate("77 .toExponential()"));
            Assert.AreEqual("5e-16", TestUtils.Evaluate("5e-16.toExponential()"));
            Assert.AreEqual("5.000e-16", TestUtils.Evaluate("5e-16.toExponential(3)"));
            Assert.AreEqual("1e+1", TestUtils.Evaluate("9.9.toExponential(0)"));
            Assert.AreEqual("1.2345678901234568e+18", TestUtils.Evaluate("1234567890123456789 .toExponential()"));
            Assert.AreEqual("1.23456789012345676800e+18", TestUtils.Evaluate("1234567890123456789 .toExponential(20)"));
            Assert.AreEqual("5e-324", TestUtils.Evaluate("Number.MIN_VALUE.toExponential()"));
            Assert.AreEqual("4.94e-324", TestUtils.Evaluate("Number.MIN_VALUE.toExponential(2)"));
            Assert.AreEqual("1.80e+308", TestUtils.Evaluate("Number.MAX_VALUE.toExponential(2)"));
            Assert.AreEqual("Infinity", TestUtils.Evaluate("Number.POSITIVE_INFINITY.toExponential()"));
            Assert.AreEqual("-Infinity", TestUtils.Evaluate("Number.NEGATIVE_INFINITY.toExponential()"));
            Assert.AreEqual("NaN", TestUtils.Evaluate("NaN.toExponential()"));

            // Negative tests.
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("77.1234.toExponential(-1)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("77.1234.toExponential(21)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("NaN.toExponential.length"));
        }

        [TestMethod]
        public void toFixed()
        {
            Assert.AreEqual("0", TestUtils.Evaluate("0 .toFixed()"));
            Assert.AreEqual("0.00", TestUtils.Evaluate("0 .toFixed(2)"));
            Assert.AreEqual("77", TestUtils.Evaluate("77.1274.toFixed()"));
            Assert.AreEqual("77.1274", TestUtils.Evaluate("77.1274.toFixed(4)"));
            Assert.AreEqual("77.13", TestUtils.Evaluate("77.1274.toFixed(2)"));
            Assert.AreEqual("77", TestUtils.Evaluate("77.1274.toFixed(0)"));
            Assert.AreEqual("77", TestUtils.Evaluate("77.1234.toFixed()"));
            Assert.AreEqual("77.13", TestUtils.Evaluate("77.1274.toFixed(2)"));
            Assert.AreEqual("77.00", TestUtils.Evaluate("77 .toFixed(2)"));
            Assert.AreEqual("0.1", TestUtils.Evaluate("0.09.toFixed(1)"));
            Assert.AreEqual("0.2", TestUtils.Evaluate("0.19.toFixed(1)"));
            Assert.AreEqual("0.0", TestUtils.Evaluate("0.03.toFixed(1)"));
            Assert.AreEqual("-1", TestUtils.Evaluate("(-0.7).toFixed()"));
            Assert.AreEqual("1000000000000000", TestUtils.Evaluate("1e+15.toFixed()"));
            Assert.AreEqual("1e+21", TestUtils.Evaluate("1e21.toFixed()"));
            Assert.AreEqual("1e+21", TestUtils.Evaluate("1e21.toFixed(15)"));
            Assert.AreEqual("1000000000000000.00000000000000000000", TestUtils.Evaluate("1e+15.toFixed(20)"));
            Assert.AreEqual("0", TestUtils.Evaluate("1e-15.toFixed()"));
            Assert.AreEqual("0.00000000000000100000", TestUtils.Evaluate("1e-15.toFixed(20)"));
            Assert.AreEqual("1234567890123456768", TestUtils.Evaluate("1234567890123456789 .toFixed(0)"));
            Assert.AreEqual("77.12739999999999440661", TestUtils.Evaluate("77.1274.toFixed(20)"));
            Assert.AreEqual("Infinity", TestUtils.Evaluate("Number.POSITIVE_INFINITY.toFixed()"));
            Assert.AreEqual("-Infinity", TestUtils.Evaluate("Number.NEGATIVE_INFINITY.toFixed()"));
            Assert.AreEqual("NaN", TestUtils.Evaluate("NaN.toFixed()"));

            // Negative tests.
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("77.1274.toFixed(-1)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("77.1274.toFixed(21)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("NaN.toFixed.length"));
        }

        [TestMethod]
        public void toLocaleString()
        {
            if (TestUtils.Engine != JSEngine.JScript)
            {
                Assert.AreEqual("77", TestUtils.Evaluate("77 .toLocaleString()"));
                Assert.AreEqual("77.5", TestUtils.Evaluate("77.5.toLocaleString()"));

                // English.
                Assert.AreEqual("77.123", TestUtils.ChangeLocale("en-NZ", () => TestUtils.Evaluate("77.123.toLocaleString()")));
                Assert.AreEqual("7.7e+101", TestUtils.ChangeLocale("en-NZ", () => TestUtils.Evaluate("77e100 .toLocaleString()")));
                Assert.AreEqual("123456789", TestUtils.ChangeLocale("en-NZ", () => TestUtils.Evaluate("123456789 .toLocaleString()")));
                Assert.AreEqual("-500", TestUtils.ChangeLocale("en-NZ", () => TestUtils.Evaluate("(-500).toLocaleString()")));

                // Spanish.
                Assert.AreEqual("77,123", TestUtils.ChangeLocale("es-ES", () => TestUtils.Evaluate("77.123.toLocaleString()")));
                Assert.AreEqual("7,7e+101", TestUtils.ChangeLocale("es-ES", () => TestUtils.Evaluate("77e100 .toLocaleString()")));
                Assert.AreEqual("123456789", TestUtils.ChangeLocale("es-ES", () => TestUtils.Evaluate("123456789 .toLocaleString()")));
                Assert.AreEqual("-500", TestUtils.ChangeLocale("es-ES", () => TestUtils.Evaluate("(-500).toLocaleString()")));
            }
            Assert.AreEqual("Infinity", TestUtils.Evaluate("Infinity.toLocaleString()"));
            Assert.AreEqual("-Infinity", TestUtils.Evaluate("(-Infinity).toLocaleString()"));
            Assert.AreEqual("NaN", TestUtils.Evaluate("NaN.toLocaleString()"));

            // length
            Assert.AreEqual(0, TestUtils.Evaluate("NaN.toLocaleString.length"));
        }

        [TestMethod]
        public void toPrecision()
        {
            Assert.AreEqual("8e+1", TestUtils.Evaluate("77.1274.toPrecision(1)"));
            Assert.AreEqual("77", TestUtils.Evaluate("77.1274.toPrecision(2)"));
            Assert.AreEqual("77.13", TestUtils.Evaluate("77.1274.toPrecision(4)"));
            Assert.AreEqual("77.127", TestUtils.Evaluate("77.1274.toPrecision(5)"));
            Assert.AreEqual("77.1274", TestUtils.Evaluate("77.1274.toPrecision(6)"));
            Assert.AreEqual("77.12740", TestUtils.Evaluate("77.1274.toPrecision(7)"));
            Assert.AreEqual("77.1274000000000", TestUtils.Evaluate("77.1274.toPrecision(15)"));
            Assert.AreEqual("77.12739999999999", TestUtils.Evaluate("77.1274.toPrecision(16)"));
            Assert.AreEqual("77.127399999999994", TestUtils.Evaluate("77.1274.toPrecision(17)"));
            Assert.AreEqual("77.1273999999999944", TestUtils.Evaluate("77.1274.toPrecision(18)"));
            Assert.AreEqual("77.12739999999999441", TestUtils.Evaluate("77.1274.toPrecision(19)"));
            Assert.AreEqual("77.127399999999994407", TestUtils.Evaluate("77.1274.toPrecision(20)"));
            Assert.AreEqual("77.1273999999999944066", TestUtils.Evaluate("77.1274.toPrecision(21)"));
            Assert.AreEqual("0.0000012", TestUtils.Evaluate("0.00000123.toPrecision(2)"));
            Assert.AreEqual("1.2e-7", TestUtils.Evaluate("0.000000123.toPrecision(2)"));
            Assert.AreEqual("0.00000123000000000000008198", TestUtils.Evaluate("0.00000123.toPrecision(21)"));
            Assert.AreEqual("1.23e+5", TestUtils.Evaluate("123456 .toPrecision(3)"));
            Assert.AreEqual("1.80e+308", TestUtils.Evaluate("Number.MAX_VALUE.toPrecision(3)"));
            Assert.AreEqual("123456.000000000", TestUtils.Evaluate("123456 .toPrecision(15)"));
            Assert.AreEqual("0.0000", TestUtils.Evaluate("0 .toPrecision(5)"));
            Assert.AreEqual("0.000001", TestUtils.Evaluate("0.000001.toPrecision(1)"));
            Assert.AreEqual("0.000001000000000", TestUtils.Evaluate("0.000001.toPrecision(10)"));
            Assert.AreEqual("1.000000000e-7", TestUtils.Evaluate("0.0000001.toPrecision(10)"));
            Assert.AreEqual("1e-11", TestUtils.Evaluate("0.00000000001.toPrecision(1)"));
            Assert.AreEqual("1.0e-11", TestUtils.Evaluate("0.00000000001.toPrecision(2)"));
            Assert.AreEqual("55", TestUtils.Evaluate("(55).toPrecision(2)"));
            Assert.AreEqual("6e+1", TestUtils.Evaluate("(55).toPrecision(1)"));
            Assert.AreEqual("-55", TestUtils.Evaluate("(-55).toPrecision(2)"));
            Assert.AreEqual("-6e+1", TestUtils.Evaluate("(-55).toPrecision(1)"));
            Assert.AreEqual("1e+1", TestUtils.Evaluate("9.59.toPrecision(1)"));
            Assert.AreEqual("9.6", TestUtils.Evaluate("9.59.toPrecision(2)"));
            Assert.AreEqual("9.9", TestUtils.Evaluate("9.95.toPrecision(2)"));
            Assert.AreEqual("10", TestUtils.Evaluate("9.96.toPrecision(2)"));
            Assert.AreEqual("-6e+20", TestUtils.Evaluate("(-555555555555555555555).toPrecision(1)"));
            Assert.AreEqual("-6e+21", TestUtils.Evaluate("(-5555555555555555555555).toPrecision(1)"));
            Assert.AreEqual("18014398509482012.0000", TestUtils.Evaluate("18014398509482012 .toPrecision(21)"));
            Assert.AreEqual("180143985094820134912", TestUtils.Evaluate("180143985094820121234 .toPrecision(21)"));
            Assert.AreEqual("0.100000000000000005551", TestUtils.Evaluate("0.1.toPrecision(21)"));
            Assert.AreEqual("4.9e-324", TestUtils.Evaluate("Number.MIN_VALUE.toPrecision(2)"));
            Assert.AreEqual("4.94065645841246544177e-324", TestUtils.Evaluate("Number.MIN_VALUE.toPrecision(21)"));

            // If the precision argument is undefined, it is the same as toString().
            Assert.AreEqual("77.1274", TestUtils.Evaluate("77.1274.toPrecision()"));
            Assert.AreEqual("77.1274", TestUtils.Evaluate("77.1274.toPrecision(undefined)"));

            // NaN & infinity
            Assert.AreEqual("NaN", TestUtils.Evaluate("Number.NaN.toPrecision()"));
            Assert.AreEqual("NaN", TestUtils.Evaluate("Number.NaN.toPrecision(3)"));
            Assert.AreEqual("Infinity", TestUtils.Evaluate("Number.POSITIVE_INFINITY.toPrecision()"));
            Assert.AreEqual("Infinity", TestUtils.Evaluate("Number.POSITIVE_INFINITY.toPrecision(3)"));
            Assert.AreEqual("-Infinity", TestUtils.Evaluate("Number.NEGATIVE_INFINITY.toPrecision()"));
            Assert.AreEqual("-Infinity", TestUtils.Evaluate("Number.NEGATIVE_INFINITY.toPrecision(3)"));

            // Precision out of range.
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("77.1274.toPrecision(-1)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("77.1274.toPrecision(0)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("77.1274.toPrecision(22)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("NaN.toPrecision.length"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("12345", TestUtils.Evaluate("12345 .toString()"));
            Assert.AreEqual("18014398509481990", TestUtils.Evaluate("18014398509481992 .toString()"));
            Assert.AreEqual("18014398509482010", TestUtils.Evaluate("18014398509482008 .toString()"));
            Assert.AreEqual("18014398509482012", TestUtils.Evaluate("18014398509482012 .toString()"));
            Assert.AreEqual("18014398509481988", TestUtils.Evaluate("18014398509481988 .toString()"));
            Assert.AreEqual("1234567890123456800", TestUtils.Evaluate("1234567890123456789 .toString()"));
            Assert.AreEqual("2234567890123456800", TestUtils.Evaluate("2234567890123456789 .toString()"));
            Assert.AreEqual("3234567890123457000", TestUtils.Evaluate("3234567890123456789 .toString()"));
            Assert.AreEqual("4234567890123457000", TestUtils.Evaluate("4234567890123456789 .toString()"));
            Assert.AreEqual("72057594037927940", TestUtils.Evaluate("72057594037927944 .toString()"));
            Assert.AreEqual("72057594037927950", TestUtils.Evaluate("72057594037927945 .toString()"));
            Assert.AreEqual("72057594037927950", TestUtils.Evaluate("72057594037927959 .toString()"));
            Assert.AreEqual("9.999999999999998", TestUtils.Evaluate("9.999999999999999.toString()"));
            Assert.AreEqual("10", TestUtils.Evaluate("9.9999999999999999.toString()"));
            Assert.AreEqual("100000000000000000000", TestUtils.Evaluate("99999999999999999999 .toString()"));
            Assert.AreEqual("999999999999990000000", TestUtils.Evaluate("999999999999990000000 .toString()"));
            Assert.AreEqual("1e+21", TestUtils.Evaluate("999999999999999999999 .toString()"));
            Assert.AreEqual("100000000000000000000", TestUtils.Evaluate("99999999999999999999 .toString()"));
            Assert.AreEqual("-77", TestUtils.Evaluate("(-77).toString()"));
            Assert.AreEqual("77.1274", TestUtils.Evaluate("77.1274.toString()"));
            Assert.AreEqual("77.001", TestUtils.Evaluate("77.001.toString()"));
            Assert.AreEqual("77.12345678901235", TestUtils.Evaluate("77.1234567890123456789.toString()"));
            Assert.AreEqual("7.123456789012345", TestUtils.Evaluate("7.1234567890123456789.toString()"));
            Assert.AreEqual("0.000005", TestUtils.Evaluate("5e-6.toString()"));
            Assert.AreEqual("0.000001", TestUtils.Evaluate("1e-6.toString()"));
            Assert.AreEqual("5e-7", TestUtils.Evaluate("5e-7.toString()"));
            Assert.AreEqual("1e-7", TestUtils.Evaluate("1e-7.toString()"));
            Assert.AreEqual("1000000000000000", TestUtils.Evaluate("1e15.toString()"));
            Assert.AreEqual("10000000000000000", TestUtils.Evaluate("1e16.toString()"));
            Assert.AreEqual("100000000000000000", TestUtils.Evaluate("1e17.toString()"));
            Assert.AreEqual("1000000000000000000", TestUtils.Evaluate("1e18.toString()"));
            Assert.AreEqual("10000000000000000000", TestUtils.Evaluate("1e19.toString()"));
            Assert.AreEqual("100000000000000000000", TestUtils.Evaluate("1e20.toString()"));
            Assert.AreEqual("1e+21", TestUtils.Evaluate("1e21.toString()"));
            Assert.AreEqual("1e+21", TestUtils.Evaluate("999999999999999999999 .toString()"));
            Assert.AreEqual("100111122133144160", TestUtils.Evaluate("100111122133144155 .toString()"));
            Assert.AreEqual("Infinity", TestUtils.Evaluate("Infinity.toString()"));
            Assert.AreEqual("-Infinity", TestUtils.Evaluate("(-Infinity).toString()"));
            Assert.AreEqual("NaN", TestUtils.Evaluate("NaN.toString()"));
            Assert.AreEqual("1.7976931348623157e+308", TestUtils.Evaluate("Number.MAX_VALUE.toString()"));
            Assert.AreEqual("5e-324", TestUtils.Evaluate("Number.MIN_VALUE.toString()"));
            
            // Radix which is not 10.
            Assert.AreEqual("115", TestUtils.Evaluate("77 .toString(8)"));
            Assert.AreEqual("1001", TestUtils.Evaluate("9 .toString(2)"));
            Assert.AreEqual("fe", TestUtils.Evaluate("254 .toString(16)"));
            Assert.AreEqual("-115.4621320712601014", TestUtils.Evaluate("(-77.598).toString(8)"));
            Assert.AreEqual("0.00142233513615237575", TestUtils.Evaluate("0.003.toString(8)"));
            Assert.AreEqual("27524716460150203300000000000000000", TestUtils.Evaluate("15e30.toString(8)"));
            Assert.AreEqual("0.252525252525252525", TestUtils.Evaluate("(1/3).toString(8)"));

            // Decimal point should be '.' regardless of locale.
            Assert.AreEqual("77.1274", TestUtils.ChangeLocale("es-ES", () => TestUtils.Evaluate("77.1274.toString()")));

            // Radix out of range.
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "RangeError", TestUtils.EvaluateExceptionType("254 .toString(37)"));
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "RangeError", TestUtils.EvaluateExceptionType("254 .toString(1)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("NaN.toString.length"));
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
    }
}
