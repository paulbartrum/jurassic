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
            Assert.AreEqual("[object Global]", TestUtils.Evaluate("this.toString()"));

            // valueOf()
            Assert.AreEqual(true, TestUtils.Evaluate("this.valueOf() === this"));
        }

        [TestMethod]
        public void GlobalProperties()
        {
            // Global constants: Infinity, NaN and undefined.
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("Infinity"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("NaN"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("undefined"));

            // In ECMAScript 5 these properties are not enumerable, not configurable and not writable.
            Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("this", "Infinity"));
            Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("this", "NaN"));
            Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("this", "undefined"));

            // In ECMAScript 5 these properties are not enumerable, not configurable and writable.
            TestUtils.CompatibilityMode = CompatibilityMode.ECMAScript3;
            try
            {
                Assert.AreEqual(PropertyAttributes.Writable, TestUtils.EvaluateAccessibility("this", "Infinity"));
                Assert.AreEqual(PropertyAttributes.Writable, TestUtils.EvaluateAccessibility("this", "NaN"));
                Assert.AreEqual(PropertyAttributes.Writable, TestUtils.EvaluateAccessibility("this", "undefined"));
            }
            finally
            {
                TestUtils.CompatibilityMode = CompatibilityMode.Latest;
            }

            // Built-in objects should be writable and configurable but not enumerable.
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "Array"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "Boolean"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "Date"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "Error"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "EvalError"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "Function"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "JSON"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "Math"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "Number"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "Object"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "RangeError"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "ReferenceError"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "RegExp"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "String"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "SyntaxError"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "TypeError"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "URIError"));

            // Functions are writable and configurable.
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("this", "decodeURI"));

            // length is sealed.
            Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("this.decodeURI", "length"));
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
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("eval()"));
            Assert.AreEqual(5, TestUtils.Evaluate("eval(5)"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; eval(x) === x;"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; e = eval; e(x) === x;"));
            Assert.AreEqual(1, TestUtils.Evaluate("eval('Math.abs(-1)')"));

            // The lexical environment does not change inside an eval if it is a direct call.
            Assert.AreEqual(5, TestUtils.Evaluate("(function() { var a = 5; return eval('a'); })()"));
            Assert.AreEqual(6, TestUtils.Evaluate("(function() { var a = 5; eval('a = 6'); return a; })()"));

            // Variables should not be reinitialized.
            Assert.AreEqual(0, TestUtils.Evaluate("var x = 0; eval('var x'); x"));

            // Eval() can introduce new variables into a function scope.
            Assert.AreEqual(5, TestUtils.Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); return b; })()"));
            Assert.AreEqual(1, TestUtils.Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); b = 4; })(); b;"));
            Assert.AreEqual(8, TestUtils.Evaluate("(function() { eval('var b = 8'); return eval('b'); })();"));

            // The global lexical environment is used for a non-direct call.
            Assert.AreEqual(5, TestUtils.Evaluate("e = eval; (function() { var a = 5; e('a = 6'); return a; })()"));
            Assert.AreEqual(1, TestUtils.Evaluate("e = eval; a = 1; (function() { var a = 5; return e('a'); })()"));
            Assert.AreEqual(3, TestUtils.Evaluate("e = eval; a = 3; b = 2; (function() { var a = 5; e('var b = a'); return b; })()"));
            Assert.AreEqual(3, TestUtils.Evaluate("e = eval; a = 3; b = 2; (function() { var a = 5; e('var b = a'); })(); b"));

            // Strict mode: eval has it's own scope.
            TestUtils.Evaluate("delete a");
            Assert.AreEqual("undefined", TestUtils.Evaluate("'use strict'; eval('var a = false'); typeof a"));
            Assert.AreEqual("undefined", TestUtils.Evaluate(@"eval(""'use strict'; var a = false""); typeof a"));

            // Return is not allowed.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("e = eval; (function() { var a = 5; e('return a'); })()"));
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
            Assert.AreEqual(0.1, TestUtils.Evaluate("parseFloat('.1')"));
            Assert.AreEqual(0.01, TestUtils.Evaluate("parseFloat('.01')"));
            Assert.AreEqual(0.07, TestUtils.Evaluate("parseFloat('.7e-1')"));
            Assert.AreEqual(-0.5, TestUtils.Evaluate("parseFloat('-.5')"));
            Assert.AreEqual(5, TestUtils.Evaluate("parseFloat('5e')"));
            Assert.AreEqual(5, TestUtils.Evaluate("parseFloat('5.e')"));
            Assert.AreEqual(5, TestUtils.Evaluate("parseFloat('5e.5')"));
            Assert.AreEqual(12, TestUtils.Evaluate("parseFloat('12x3')"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("parseFloat('')"));
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("parseFloat('Infinity')"));
            Assert.AreEqual(double.NegativeInfinity, TestUtils.Evaluate("parseFloat('-Infinity')"));
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("parseFloat(' Infinity')"));
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("parseFloat('InfinityZ')"));
            Assert.AreEqual(0, TestUtils.Evaluate("parseFloat('0xff')"));
            Assert.AreEqual(0, TestUtils.Evaluate("parseFloat('0x')"));
            Assert.AreEqual(0, TestUtils.Evaluate("parseFloat('0zff')"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("parseFloat('infinity')"));
            Assert.AreEqual(-1.1, TestUtils.Evaluate("parseFloat('\u205F -1.1')"));

            // Very large numbers.
            Assert.AreEqual(18446744073709551616d, TestUtils.Evaluate("parseFloat('18446744073709551616')"));
            Assert.AreEqual(295147905179352825856d, TestUtils.Evaluate("parseFloat('295147905179352825856')"));
            Assert.AreEqual(4722366482869645213696d, TestUtils.Evaluate("parseFloat('4722366482869645213696')"));
            Assert.AreEqual(75557863725914323419136d, TestUtils.Evaluate("parseFloat('75557863725914323419136')"));
        }

        [TestMethod]
        public void parseInt()
        {
            Assert.AreEqual(1, TestUtils.Evaluate("parseInt('1')"));
            Assert.AreEqual(123, TestUtils.Evaluate("parseInt('123')"));
            Assert.AreEqual(65, TestUtils.Evaluate("parseInt('65')"));
            Assert.AreEqual(987654, TestUtils.Evaluate("parseInt('987654')"));
            Assert.AreEqual(10000000, TestUtils.Evaluate("parseInt('10000000')"));
            Assert.AreEqual(10000001, TestUtils.Evaluate("parseInt('10000001')"));
            Assert.AreEqual(987654321, TestUtils.Evaluate("parseInt('987654321')"));
            Assert.AreEqual(9876543212d, TestUtils.Evaluate("parseInt('9876543212')"));
            Assert.AreEqual(987654321234d, TestUtils.Evaluate("parseInt('987654321234')"));
            Assert.AreEqual(-987654321234d, TestUtils.Evaluate("parseInt('-987654321234')"));
            Assert.AreEqual(9876543212345d, TestUtils.Evaluate("parseInt('9876543212345')"));
            Assert.AreEqual(98765432123456d, TestUtils.Evaluate("parseInt('98765432123456')"));
            Assert.AreEqual(987654321234567d, TestUtils.Evaluate("parseInt('987654321234567')"));
            Assert.AreEqual(9876543212345678d, TestUtils.Evaluate("parseInt('9876543212345678')"));
            Assert.AreEqual(98765432123456789d, TestUtils.Evaluate("parseInt('98765432123456789')"));
            Assert.AreEqual(-98765432123456789d, TestUtils.Evaluate("parseInt('-98765432123456789')"));
            Assert.AreEqual(18446744073709551616d, TestUtils.Evaluate("parseInt('18446744073709551616')"));
            Assert.AreEqual(295147905179352825856d, TestUtils.Evaluate("parseInt('295147905179352825856')"));
            Assert.AreEqual(4722366482869645213696d, TestUtils.Evaluate("parseInt('4722366482869645213696')"));
            Assert.AreEqual(75557863725914323419136d, TestUtils.Evaluate("parseInt('75557863725914323419136')"));

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
            Assert.AreEqual(1.512366075204171e+36, TestUtils.Evaluate("parseInt('0x123456789abcdef0123456789abcdef')"));
            
            // Bases.
            Assert.AreEqual(17, TestUtils.Evaluate("parseInt('0x11', 16)"));
            Assert.AreEqual(90, TestUtils.Evaluate("parseInt('0X5a', 16)"));
            Assert.AreEqual(17, TestUtils.Evaluate("parseInt('11', 16)"));
            Assert.AreEqual(2748, TestUtils.Evaluate("parseInt('abc', 16)"));
            Assert.AreEqual(3, TestUtils.Evaluate("parseInt('11', 2)"));
            Assert.AreEqual(16, TestUtils.Evaluate("parseInt('0x10')"));
            Assert.AreEqual(4096, TestUtils.Evaluate("parseInt('0x1000')"));
            Assert.AreEqual(1048576, TestUtils.Evaluate("parseInt('0x100000')"));
            Assert.AreEqual(268435456, TestUtils.Evaluate("parseInt('0x10000000')"));
            Assert.AreEqual(68719476736d, TestUtils.Evaluate("parseInt('0x1000000000')"));
            Assert.AreEqual(17592186044416d, TestUtils.Evaluate("parseInt('0x100000000000')"));
            Assert.AreEqual(4503599627370496d, TestUtils.Evaluate("parseInt('0x10000000000000')"));
            Assert.AreEqual(1152921504606847000d, TestUtils.Evaluate("parseInt('0x1000000000000000')"));
            Assert.AreEqual(295147905179352830000d, TestUtils.Evaluate("parseInt('0x100000000000000000')"));
            Assert.AreEqual(7.555786372591432e+22, TestUtils.Evaluate("parseInt('0x10000000000000000000')"));
            Assert.AreEqual(1.9342813113834067e+25, TestUtils.Evaluate("parseInt('0x1000000000000000000000')"));

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

            // Radix uses ToInt32() so has weird wrapping issues.
            Assert.AreEqual(19, TestUtils.Evaluate("parseInt('23', 4294967304)"));

            // Octal parsing (only works in compatibility mode).
            TestUtils.CompatibilityMode = CompatibilityMode.ECMAScript3;
            try
            {
                Assert.AreEqual(9, TestUtils.Evaluate("parseInt('011')"));
                Assert.AreEqual(0, TestUtils.Evaluate("parseInt('08')"));
                Assert.AreEqual(1, TestUtils.Evaluate("parseInt('018')"));
                Assert.AreEqual(11, TestUtils.Evaluate("parseInt('011', 10)"));
            }
            finally
            {
                TestUtils.CompatibilityMode = CompatibilityMode.Latest;
            }

            // Octal parsing was removed from ES5.
            if (TestUtils.Engine != JSEngine.JScript)
            {
                Assert.AreEqual(11, TestUtils.Evaluate("parseInt('011')"));
                Assert.AreEqual(8, TestUtils.Evaluate("parseInt('08')"));
                Assert.AreEqual(18, TestUtils.Evaluate("parseInt('018')"));
                Assert.AreEqual(11, TestUtils.Evaluate("parseInt('011', 10)"));
            }

            // Large numbers.
            Assert.AreEqual(9214843084008499.0, TestUtils.Evaluate("parseInt('9214843084008499')"));
            Assert.AreEqual(18014398509481993.0, TestUtils.Evaluate("parseInt('18014398509481993')"));
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
