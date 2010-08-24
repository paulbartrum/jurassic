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
            TestUtils.Evaluate("var num=77.1234");
            Assert.AreEqual("7.71234e+1", TestUtils.Evaluate("num.toExponential()"));
            Assert.AreEqual("7.7123e+1", TestUtils.Evaluate("num.toExponential(4)"));
            Assert.AreEqual("7.71e+1", TestUtils.Evaluate("num.toExponential(2)"));
            Assert.AreEqual("8e+1", TestUtils.Evaluate("num.toExponential(0)"));
            Assert.AreEqual("7.71234e+1", TestUtils.Evaluate("77.1234.toExponential()"));
            Assert.AreEqual("8e+1", TestUtils.Evaluate("77.1234.toExponential(0)"));
            Assert.AreEqual("7.7e+1", TestUtils.Evaluate("77 .toExponential()"));

            // Negative tests.
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("num.toExponential(-1)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("num.toExponential(21)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("NaN.toExponential.length"));
        }

        [TestMethod]
        public void toFixed()
        {
            TestUtils.Evaluate("var num=77.1274");
            Assert.AreEqual("77", TestUtils.Evaluate("num.toFixed()"));
            Assert.AreEqual("77.1274", TestUtils.Evaluate("num.toFixed(4)"));
            Assert.AreEqual("77.13", TestUtils.Evaluate("num.toFixed(2)"));
            Assert.AreEqual("77", TestUtils.Evaluate("num.toFixed(0)"));
            Assert.AreEqual("77", TestUtils.Evaluate("77.1234.toFixed()"));
            Assert.AreEqual("77.13", TestUtils.Evaluate("77.1274.toFixed(2)"));
            Assert.AreEqual("77.00", TestUtils.Evaluate("77 .toFixed(2)"));
            Assert.AreEqual("0.1", TestUtils.Evaluate("(0.09).toFixed(1)"));
            Assert.AreEqual("-1", TestUtils.Evaluate("(-0.7).toFixed()"));

            // Negative tests.
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("num.toFixed(-1)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("num.toFixed(21)"));

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
            Assert.AreEqual("77.1274", TestUtils.Evaluate("77.1274.toPrecision()"));
            Assert.AreEqual("8e+1", TestUtils.Evaluate("77.1274.toPrecision(1)"));
            Assert.AreEqual("77", TestUtils.Evaluate("77.1274.toPrecision(2)"));
            Assert.AreEqual("77.13", TestUtils.Evaluate("77.1274.toPrecision(4)"));
            Assert.AreEqual("77.127", TestUtils.Evaluate("77.1274.toPrecision(5)"));
            Assert.AreEqual("77.1274", TestUtils.Evaluate("77.1274.toPrecision(6)"));
            Assert.AreEqual("77.12740", TestUtils.Evaluate("77.1274.toPrecision(7)"));
            Assert.AreEqual("77.1274000000000", TestUtils.Evaluate("77.1274.toPrecision(15)"));
            Assert.AreEqual("77.12740000000000", TestUtils.Evaluate("77.1274.toPrecision(16)"));
            Assert.AreEqual("77.127400000000000", TestUtils.Evaluate("77.1274.toPrecision(17)"));
            Assert.AreEqual("77.1274000000000000", TestUtils.Evaluate("77.1274.toPrecision(18)"));
            Assert.AreEqual("77.12740000000000000", TestUtils.Evaluate("77.1274.toPrecision(19)"));
            Assert.AreEqual("77.127400000000000000", TestUtils.Evaluate("77.1274.toPrecision(20)"));
            Assert.AreEqual("77.1274000000000000000", TestUtils.Evaluate("77.1274.toPrecision(21)"));
            Assert.AreEqual("77.1274", TestUtils.Evaluate("77.1274.toPrecision(undefined)"));
            Assert.AreEqual("0.0000012", TestUtils.Evaluate("0.00000123.toPrecision(2)"));
            Assert.AreEqual("1.2e-7", TestUtils.Evaluate("0.000000123.toPrecision(2)"));
            Assert.AreEqual("0.00000123000000000000000000", TestUtils.Evaluate("0.00000123.toPrecision(21)"));
            Assert.AreEqual("1.23e+5", TestUtils.Evaluate("123456 .toPrecision(3)"));
            Assert.AreEqual("1.80e+308", TestUtils.Evaluate("Number.MAX_VALUE.toPrecision(3)"));
            Assert.AreEqual("123456.000000000", TestUtils.Evaluate("123456 .toPrecision(15)"));
            Assert.AreEqual("0.0000", TestUtils.Evaluate("0 .toPrecision(5)"));
            Assert.AreEqual("0.000001000000000", TestUtils.Evaluate("0.000001.toPrecision(10)"));   // Why more than 10 digits?!?
            Assert.AreEqual("1.000000000e-7", TestUtils.Evaluate("0.0000001.toPrecision(10)"));
            Assert.AreEqual("1e-11", TestUtils.Evaluate("0.00000000001.toPrecision(1)"));
            Assert.AreEqual("1.0e-11", TestUtils.Evaluate("0.00000000001.toPrecision(2)"));
            Assert.AreEqual("55", TestUtils.Evaluate("(55).toPrecision(2)"));
            Assert.AreEqual("6e+1", TestUtils.Evaluate("(55).toPrecision(1)"));
            Assert.AreEqual("-55", TestUtils.Evaluate("(-55).toPrecision(2)"));
            Assert.AreEqual("-6e+1", TestUtils.Evaluate("(-55).toPrecision(1)"));
            Assert.AreEqual("-6e+20", TestUtils.Evaluate("(-555555555555555555555).toPrecision(1)"));
            Assert.AreEqual("-6e+21", TestUtils.Evaluate("(-5555555555555555555555).toPrecision(1)"));

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
            Assert.AreEqual("1234567890123456800", TestUtils.Evaluate("1234567890123456789 .toString()"));
            Assert.AreEqual("2234567890123456800", TestUtils.Evaluate("2234567890123456789 .toString()"));
            Assert.AreEqual("3234567890123457000", TestUtils.Evaluate("3234567890123456789 .toString()"));
            Assert.AreEqual("4234567890123457000", TestUtils.Evaluate("4234567890123456789 .toString()"));
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

            // Radix out of range.
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "RangeError", TestUtils.EvaluateExceptionType("254 .toString(37)"));
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "RangeError", TestUtils.EvaluateExceptionType("254 .toString(1)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("NaN.toString.length"));
        }
    }
}
