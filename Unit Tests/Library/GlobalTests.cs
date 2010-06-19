using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the Global object.
    /// </summary>
    [TestClass]
    public class GlobalTests
    {
        [TestMethod]
        public void Constructor()
        {
            // toString
            Assert.AreEqual("[object Global]", TestUtils.Evaluate("toString()"));

            // valueOf()
            Assert.AreEqual(true, TestUtils.Evaluate("valueOf() === this"));
        }

        [TestMethod]
        public void GlobalProperties()
        {
            // Global constants: Infinity, NaN and undefined.
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("Infinity"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("NaN"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("undefined"));

            // These properties are not enumerable, not configurable and not writable (they were writable in ECMAScript 3 however).
            Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("this", "Infinity"));
            Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("this", "NaN"));
            Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("this", "undefined"));
        }

        [TestMethod]
        public void decodeURI()
        {
            Assert.AreEqual("abc", TestUtils.Evaluate("decodeURI('abc')"));
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789;/?:@&=+$,-_.!~*'()#",
                TestUtils.Evaluate("decodeURI('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789;/?:@&=+$,-_.!~*\\'()#')"));
            Assert.AreEqual("%3F", TestUtils.Evaluate("decodeURI('%3F')"));
            Assert.AreEqual("%2f", TestUtils.Evaluate("decodeURI('%2f')"));
            Assert.AreEqual("%23", TestUtils.Evaluate("decodeURI('%23')"));
            Assert.AreEqual("!", TestUtils.Evaluate("decodeURI('%21')"));
            Assert.AreEqual("^", TestUtils.Evaluate("decodeURI('%5e')"));
            Assert.AreEqual("^", TestUtils.Evaluate("decodeURI('^')"));
            Assert.AreEqual("Ҧ", TestUtils.Evaluate("decodeURI('Ҧ')"));
            Assert.AreEqual("Ҧ", TestUtils.Evaluate("decodeURI('%D2%A6')"));
            Assert.AreEqual("Ҧa", TestUtils.Evaluate("decodeURI('%D2%A6a')"));
            Assert.AreEqual("ᵝ", TestUtils.Evaluate("decodeURI('%E1%B5%9D')"));
            Assert.AreEqual("\U0001D11E", TestUtils.Evaluate("decodeURI('%F0%9D%84%9E')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURI('%B')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURI('%H5')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURI('%B5')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURI('%F8')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURI('%E1')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURI('%E1%B5')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURI('%E1%E1%9D')"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURI('%C0%A6')"));    // Test is buggy in JScript.
        }

        [TestMethod]
        public void decodeURIComponent()
        {
            Assert.AreEqual("abc", TestUtils.Evaluate("decodeURIComponent('abc')"));
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.!~*'()",
                TestUtils.Evaluate("decodeURIComponent('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.!~*\\'()')"));
            Assert.AreEqual("?", TestUtils.Evaluate("decodeURIComponent('%3F')"));
            Assert.AreEqual("/", TestUtils.Evaluate("decodeURIComponent('%2f')"));
            Assert.AreEqual("#", TestUtils.Evaluate("decodeURIComponent('%23')"));
            Assert.AreEqual("!", TestUtils.Evaluate("decodeURIComponent('%21')"));
            Assert.AreEqual("^", TestUtils.Evaluate("decodeURIComponent('%5e')"));
            Assert.AreEqual("^", TestUtils.Evaluate("decodeURIComponent('^')"));
            Assert.AreEqual("Ҧ", TestUtils.Evaluate("decodeURIComponent('Ҧ')"));
            Assert.AreEqual("Ҧ", TestUtils.Evaluate("decodeURIComponent('%D2%A6')"));
            Assert.AreEqual("Ҧa", TestUtils.Evaluate("decodeURIComponent('%D2%A6a')"));
            Assert.AreEqual("ᵝ", TestUtils.Evaluate("decodeURIComponent('%E1%B5%9D')"));
            Assert.AreEqual("\U0001D11E", TestUtils.Evaluate("decodeURIComponent('%F0%9D%84%9E')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURIComponent('%B')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURIComponent('%H5')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURIComponent('%B5')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURIComponent('%F8')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURIComponent('%E1')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURIComponent('%E1%B5')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURIComponent('%E1%E1%9D')"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("decodeURIComponent('%C0%A6')"));    // Test is buggy in JScript.
        }

        [TestMethod]
        public void encodeURI()
        {
            Assert.AreEqual("abc", TestUtils.Evaluate("encodeURI('abc')"));
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789;/?:@&=+$,-_.!~*'()#",
                TestUtils.Evaluate("encodeURI('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789;/?:@&=+$,-_.!~*\\'()#')"));
            Assert.AreEqual("%5E", TestUtils.Evaluate("encodeURI('^')"));
            Assert.AreEqual("%D2%A6", TestUtils.Evaluate("encodeURI('Ҧ')"));
            Assert.AreEqual("%E1%B5%9D", TestUtils.Evaluate("encodeURI('ᵝ')"));
            Assert.AreEqual("%F0%9D%84%9E", TestUtils.Evaluate("encodeURI('\U0001D11E')"));
            Assert.AreEqual("%F0%9D%84%9Ee", TestUtils.Evaluate("encodeURI('\U0001D11Ee')"));
            Assert.AreEqual("k%CC%81u%CC%ADo%CC%84%CC%81n", TestUtils.Evaluate("encodeURI('\u006B\u0301\u0075\u032D\u006F\u0304\u0301\u006E')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("encodeURI('\uD834')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("encodeURI('\uDD1E')"));
        }

        [TestMethod]
        public void encodeURIComponent()
        {
            Assert.AreEqual("abc", TestUtils.Evaluate("encodeURIComponent('abc')"));
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.!~*'()",
                TestUtils.Evaluate("encodeURIComponent('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.!~*\\'()')"));
            Assert.AreEqual("%3F", TestUtils.Evaluate("encodeURIComponent('?')"));
            Assert.AreEqual("%2F", TestUtils.Evaluate("encodeURIComponent('/')"));
            Assert.AreEqual("%23", TestUtils.Evaluate("encodeURIComponent('#')"));
            Assert.AreEqual("%5E", TestUtils.Evaluate("encodeURIComponent('^')"));
            Assert.AreEqual("%D2%A6", TestUtils.Evaluate("encodeURIComponent('Ҧ')"));
            Assert.AreEqual("%E1%B5%9D", TestUtils.Evaluate("encodeURIComponent('ᵝ')"));
            Assert.AreEqual("%F0%9D%84%9E", TestUtils.Evaluate("encodeURIComponent('\U0001D11E')"));
            Assert.AreEqual("%F0%9D%84%9Ee", TestUtils.Evaluate("encodeURIComponent('\U0001D11Ee')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("encodeURIComponent('\uD834')"));
            Assert.AreEqual("URIError", TestUtils.EvaluateExceptionType("encodeURIComponent('\uDD1E')"));
        }

        [TestMethod]
        public void escape()
        {
            // Note: escape is deprecated.
            Assert.AreEqual("abc", TestUtils.Evaluate("escape('abc')"));
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@*_+-./",
                TestUtils.Evaluate("escape('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@*_+-./')"));
            Assert.AreEqual("%23%24%3F", TestUtils.Evaluate("escape('#$?')"));
            Assert.AreEqual("%u04A6", TestUtils.Evaluate("escape('Ҧ')"));
        }

        [TestMethod]
        public void eval()
        {
            Assert.AreEqual(1, TestUtils.Evaluate("eval('Math.abs(-1)')"));

            // The lexical environment does not change inside an eval if it is a direct call.
            Assert.AreEqual(6, TestUtils.Evaluate("(function() { a = 5; eval('a = 6'); return a; })()"));

            // The global lexical environment is used for a non-direct call.
            Assert.AreEqual(5, TestUtils.Evaluate("e = eval; (function() { a = 5; e('a = 6'); return a; })()"));
        }

        [TestMethod]
        public void isFinite()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("isFinite(0)"));
            Assert.AreEqual(true, TestUtils.Evaluate("isFinite(1)"));
            Assert.AreEqual(true, TestUtils.Evaluate("isFinite(-1)"));
            Assert.AreEqual(true, TestUtils.Evaluate("isFinite(Number.MAX_VALUE)"));
            Assert.AreEqual(true, TestUtils.Evaluate("isFinite(Number.MIN_VALUE)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isFinite(NaN)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isFinite(Number.POSITIVE_INFINITY)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isFinite(Number.NEGATIVE_INFINITY)"));
            Assert.AreEqual(true, TestUtils.Evaluate("isFinite('12')"));
            Assert.AreEqual(false, TestUtils.Evaluate("isFinite('string')"));
            Assert.AreEqual(true, TestUtils.Evaluate("isFinite('')"));
            Assert.AreEqual(true, TestUtils.Evaluate(@"isFinite('  \n \t ')"));
            Assert.AreEqual(true, TestUtils.Evaluate(@"isFinite(null)"));
            
        }

        [TestMethod]
        public void isNaN()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("isNaN(0)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isNaN(1)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isNaN(-1)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isNaN(Number.MAX_VALUE)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isNaN(Number.MIN_VALUE)"));
            Assert.AreEqual(true, TestUtils.Evaluate("isNaN(NaN)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isNaN(Number.POSITIVE_INFINITY)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isNaN(Number.NEGATIVE_INFINITY)"));
            Assert.AreEqual(false, TestUtils.Evaluate("isNaN('12')"));
            Assert.AreEqual(true, TestUtils.Evaluate("isNaN('string')"));
            Assert.AreEqual(false, TestUtils.Evaluate("isNaN('')"));
            Assert.AreEqual(false, TestUtils.Evaluate(@"isNaN('  \n \t ')"));
            Assert.AreEqual(false, TestUtils.Evaluate(@"isNaN(null)"));
        }

        [TestMethod]
        public void parseFloat()
        {
            Assert.AreEqual(34, TestUtils.Evaluate("parseFloat('34')"));
            Assert.AreEqual(34.5, TestUtils.Evaluate("parseFloat('34.5')"));
            Assert.AreEqual(3400, TestUtils.Evaluate("parseFloat('34e2')"));
            Assert.AreEqual(3.45, TestUtils.Evaluate("parseFloat('34.5e-1')"));
            Assert.AreEqual(0.345, TestUtils.Evaluate("parseFloat('34.5E-2')"));
            Assert.AreEqual(-34, TestUtils.Evaluate("parseFloat('-34')"));
            Assert.AreEqual(34, TestUtils.Evaluate("parseFloat('+34')"));
            Assert.AreEqual(11, TestUtils.Evaluate("parseFloat('011')"));
            Assert.AreEqual(11, TestUtils.Evaluate("parseFloat(' 11')"));
            Assert.AreEqual(0.5, TestUtils.Evaluate("parseFloat('.5')"));
            Assert.AreEqual(-0.5, TestUtils.Evaluate("parseFloat('-.5')"));
            Assert.AreEqual(5, TestUtils.Evaluate("parseFloat('5e')"));
            Assert.AreEqual(5, TestUtils.Evaluate("parseFloat('5.e')"));
            Assert.AreEqual(5, TestUtils.Evaluate("parseFloat('5e.5')"));
            Assert.AreEqual(12, TestUtils.Evaluate("parseFloat('12x3')"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("parseFloat('')"));
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("parseFloat('Infinity')"));
            Assert.AreEqual(double.NegativeInfinity, TestUtils.Evaluate("parseFloat('-Infinity')"));
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("parseFloat(' Infinity')"));
            Assert.AreEqual(0, TestUtils.Evaluate("parseFloat('0xff')"));
            Assert.AreEqual(0, TestUtils.Evaluate("parseFloat('0zff')"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("parseFloat('infinity')"));
        }

        [TestMethod]
        public void parseInt()
        {
            Assert.AreEqual(1, TestUtils.Evaluate("parseInt('1')"));
            Assert.AreEqual(123, TestUtils.Evaluate("parseInt('123')"));
            Assert.AreEqual(65, TestUtils.Evaluate("parseInt('65')"));

            // Sign.
            Assert.AreEqual(-123, TestUtils.Evaluate("parseInt('-123')"));
            Assert.AreEqual(123, TestUtils.Evaluate("parseInt('+123')"));

            // Empty string should produce NaN.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("parseInt('')"));

            // Leading whitespace should be skipped.
            Assert.AreEqual(1, TestUtils.Evaluate("parseInt('  1')"));
            Assert.AreEqual(1, TestUtils.Evaluate("parseInt('  1.5')"));
            Assert.AreEqual(35, TestUtils.Evaluate("parseInt('\t35')"));

            // Hex prefix should be respected.
            Assert.AreEqual(17, TestUtils.Evaluate("parseInt('0x11')"));
            
            // Bases.
            Assert.AreEqual(17, TestUtils.Evaluate("parseInt('0x11', 16)"));
            Assert.AreEqual(90, TestUtils.Evaluate("parseInt('0X5a', 16)"));
            Assert.AreEqual(17, TestUtils.Evaluate("parseInt('11', 16)"));
            Assert.AreEqual(2748, TestUtils.Evaluate("parseInt('abc', 16)"));
            Assert.AreEqual(3, TestUtils.Evaluate("parseInt('11', 2)"));

            // Base out of range.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("parseInt('11', 1)"));
            Assert.AreEqual(11, TestUtils.Evaluate("parseInt('11', 0)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("parseInt('11', -1)"));

            // Hex prefix should not be respected if base is specified explicitly.
            Assert.AreEqual(0, TestUtils.Evaluate("parseInt('0x11', 10)"));

            // Junk characters and out of range characters should stop parsing
            Assert.AreEqual(123, TestUtils.Evaluate("parseInt('123x456')"));
            Assert.AreEqual(1, TestUtils.Evaluate("parseInt('1a')"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("parseInt('a')"));
            Assert.AreEqual(1, TestUtils.Evaluate("parseInt('19', 8)"));

            // Invalid prefix.
            Assert.AreEqual(0, TestUtils.Evaluate("parseInt('0z11', 10)"));
            Assert.AreEqual(0, TestUtils.Evaluate("parseInt('0z11')"));

            // Octal parsing was removed from ES5.
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? 9 : 11, TestUtils.Evaluate("parseInt('011')"));
            Assert.AreEqual(11, TestUtils.Evaluate("parseInt('011', 10)"));

            // Radix uses ToInt32() so has weird wrapping issues.
            Assert.AreEqual(19, TestUtils.Evaluate("parseInt('23', 4294967304)"));
        }

        [TestMethod]
        public void unescape()
        {
            // Note: unescape is deprecated.
            Assert.AreEqual("abc", TestUtils.Evaluate("unescape('abc')"));
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@*_+-./",
                TestUtils.Evaluate("unescape('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@*_+-./')"));
            Assert.AreEqual("#$?", TestUtils.Evaluate("unescape('%23%24%3F')"));
            Assert.AreEqual("Ҧ", TestUtils.Evaluate("unescape('%u04A6')"));
            Assert.AreEqual("Ҧ", TestUtils.Evaluate("unescape('Ҧ')"));
            Assert.AreEqual("%u04", TestUtils.Evaluate("unescape('%u04')"));
            Assert.AreEqual("%u04c", TestUtils.Evaluate("unescape('%u04c')"));
            Assert.AreEqual("%u04xc", TestUtils.Evaluate("unescape('%u04xc')"));
            Assert.AreEqual("%a", TestUtils.Evaluate("unescape('%a')"));
            Assert.AreEqual("%ag", TestUtils.Evaluate("unescape('%ag')"));
        }
    }
}
