using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the Lexer object.
    /// </summary>
    [TestClass]
    public class LexerTests : TestBase
    {
        [TestMethod]
        public void Comments()
        {
            // Single-line comment.
            Assert.AreEqual(Undefined.Value, Evaluate("// testing"));
            Assert.AreEqual(Undefined.Value, Evaluate("// testing\r\n"));

            // Multi-line comment.
            Assert.AreEqual(Undefined.Value, Evaluate("/* testing */"));
            Assert.AreEqual(Undefined.Value, Evaluate("/* test\r\ning */"));
            Assert.AreEqual(Undefined.Value, Evaluate("/*\r\n test\r\ning */"));
            Assert.AreEqual(Undefined.Value, Evaluate("/* testing \r\n*/"));
            Assert.AreEqual(Undefined.Value, Evaluate("/* test\ning */"));
            Assert.AreEqual(Undefined.Value, Evaluate("/* test\ring */"));
            Assert.AreEqual(Undefined.Value, Evaluate("/* te*sting**/"));
        }

        [TestMethod]
        public void Identifiers()
        {
            Assert.AreEqual(5, Evaluate("delete $; $ = 5; $"));
            Assert.AreEqual(6, Evaluate("delete dung; d\\u0075ng = 6; dung"));
            Assert.AreEqual(7, Evaluate("delete another; \\u0061nother = 7; another"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("ident\\u0020ifier"));
            Assert.AreEqual(12, Evaluate(@"delete \u{20BB7}; \u{20BB7} = 12; \u{20BB7}"));
            Assert.AreEqual(13, Evaluate(@"delete Te\u{20BB7}st; Te\u{20BB7}st = 13; Te\u{20BB7}st"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"ident\u{20}ifier"));
        }

        [TestMethod]
        public void Keywords()
        {
            Assert.AreEqual(false, Evaluate("false"));
            Assert.AreEqual(true, Evaluate("true"));
            Assert.AreEqual(Null.Value, Evaluate("null"));

            // Reserved words.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("class = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("enum = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("extends = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("super = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("const = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("export = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("import = 1"));

            // Future reserved words.
            Assert.AreEqual(1, Evaluate("implements = 1"));
            Assert.AreEqual(1, Evaluate("let = 1"));
            Assert.AreEqual(1, Evaluate("private = 1"));
            Assert.AreEqual(1, Evaluate("public = 1"));
            Assert.AreEqual(1, Evaluate("yield = 1"));
            Assert.AreEqual(1, Evaluate("interface = 1"));
            Assert.AreEqual(1, Evaluate("package = 1"));
            Assert.AreEqual(1, Evaluate("protected = 1"));
            Assert.AreEqual(1, Evaluate("static = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; implements = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; let = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; private = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; public = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; yield = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; interface = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; package = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; protected = 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; static = 1"));

            Assert.AreEqual(Null.Value, Evaluate("null"));
            Assert.AreEqual(Null.Value, Evaluate("null"));
        }

        [TestMethod]
        public void Number()
        {
            // Basics.
            Assert.AreEqual(0, Evaluate("0"));
            Assert.AreEqual(123, Evaluate("123"));
            Assert.AreEqual(123, Evaluate("+123"));
            Assert.AreEqual(-123, Evaluate("-123"));
            Assert.AreEqual(987654, Evaluate("987654"));
            Assert.AreEqual(10000000, Evaluate("10000000"));
            Assert.AreEqual(10000001, Evaluate("10000001"));
            Assert.AreEqual(987654321, Evaluate("987654321"));
            Assert.AreEqual(9876543212d, Evaluate("9876543212"));
            Assert.AreEqual(987654321234d, Evaluate("987654321234"));
            Assert.AreEqual(-987654321234d, Evaluate("-987654321234"));
            Assert.AreEqual(9876543212345d, Evaluate("9876543212345"));
            Assert.AreEqual(98765432123456d, Evaluate("98765432123456"));
            Assert.AreEqual(987654321234567d, Evaluate("987654321234567"));
            Assert.AreEqual(9876543212345678d, Evaluate("9876543212345678"));
            Assert.AreEqual(98765432123456789d, Evaluate("98765432123456789"));
            Assert.AreEqual(0.1, Evaluate("0.1"));
            Assert.AreEqual(0.123, Evaluate("0.123"));
            Assert.AreEqual(0.345, Evaluate("34.5E-2"));
            Assert.AreEqual(0.123, Evaluate("0.123"));
            Assert.AreEqual(0.0005, Evaluate("0.0005"));
            Assert.AreEqual(0.0005000, Evaluate("0.0005000"));
            Assert.AreEqual(0.00050001, Evaluate("0.00050001"));
            Assert.AreEqual(0.123456789, Evaluate("0.123456789"));
            Assert.AreEqual(0.123456789012, Evaluate("0.123456789012"));
            Assert.AreEqual(0.1234567890123456789, Evaluate("0.1234567890123456789"));
            Assert.AreEqual(1.23456, Evaluate("1.23456"));
            Assert.AreEqual(12.3456789, Evaluate("12.3456789"));
            Assert.AreEqual(12.001, Evaluate("12.001"));
            Assert.AreEqual(12.00100, Evaluate("12.00100"));
            Assert.AreEqual(1234567890.123, Evaluate("1234567890.123"));
            Assert.AreEqual(1234567890.123456789, Evaluate("1234567890.123456789"));
            Assert.AreEqual(500000, Evaluate("5e5"));
            Assert.AreEqual(500000, Evaluate("5e05"));
            Assert.AreEqual(123e95, Evaluate("123e95"));
            Assert.AreEqual(150, Evaluate("1.5e+2"));
            Assert.AreEqual(1.5e-2, Evaluate("1.5e-2"));
            Assert.AreEqual(1.5e-2, Evaluate("+1.5e-2"));
            Assert.AreEqual(-1.5e-2, Evaluate("-1.5e-2"));
            Assert.AreEqual(1.2e+308, Evaluate("1.2e+308"));
            Assert.AreEqual(5e-324, Evaluate("5e-324"));
            Assert.AreEqual(1.2347e-320, Evaluate("1.234567890123456789e-320"));
            Assert.AreEqual(0.5, Evaluate(".5"));
            Assert.AreEqual(50, Evaluate(".5e2"));
            Assert.AreEqual(5, Evaluate("5."));
            Assert.AreEqual(500, Evaluate("5.e2"));

            // Hex literals.
            Assert.AreEqual(255, Evaluate("0xff"));
            Assert.AreEqual(241, Evaluate("0xF1"));
            Assert.AreEqual(244837814094590.0, Evaluate("0xdeadbeefcafe"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("0xgg"));

            // ES6 binary literals.
            Assert.AreEqual(21, Evaluate("0b10101"));
            Assert.AreEqual(1, Evaluate("0b1"));
            Assert.AreEqual(-21, Evaluate("-0B10101"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("0b2"));

            // ES6 octal literals.
            Assert.AreEqual(61, Evaluate("0o75"));
            Assert.AreEqual(4095, Evaluate("0O7777"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("0o80"));

            // Overflow and underflow.
            Assert.AreEqual(double.PositiveInfinity, Evaluate("1.8e+308"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("1e+309"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("1.79876543210987654321e+308"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("1e+9999999999"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("-1.8e+308"));
            Assert.AreEqual(5e-324, Evaluate("3e-324"));
            Assert.AreEqual(0, Evaluate("2e-324"));
            Assert.AreEqual(0, Evaluate("9e-325"));
            Assert.AreEqual(0, Evaluate("2.1234567890123456789e-324"));
            Assert.AreEqual(0, Evaluate("1e-9999999999"));

            // Tricky cases (from http://www.exploringbinary.com/)
            Assert.AreEqual("1.0000000000000000000000000000000000000000000000000010 x 2^54", ToBinary(Evaluate("18014398509481993")));
            Assert.AreEqual("1.1101011011111101001000011111111100101110010010001111 x 2^-1", ToBinary(Evaluate("0.9199")));
            Assert.AreEqual("1.1110001111010111000010100011110101110000101000111101 x 2^0", ToBinary(Evaluate("1.89")));
            Assert.AreEqual("1.1101110011010000000010001001110000010011000101001110 x 2^218", ToBinary(Evaluate("7.8459735791271921e65")));
            Assert.AreEqual("1.0110001001100100010011000110000111010100000110101010 x 2^885", ToBinary(Evaluate("3.571e266")));
            Assert.AreEqual("1.0100000011011110010010000110011101100110010100111011 x 2^-105", ToBinary(Evaluate("3.08984926168550152811e-32")));
            Assert.AreEqual("1.0000010111100110110011101100010101110111011000011010 x 2^53", ToBinary(Evaluate("9214843084008499")));
            Assert.AreEqual("1.0000000000000000000000000000000000000000000000000010 x 2^-1", ToBinary(Evaluate("0.500000000000000166533453693773481063544750213623046875")));
            Assert.AreEqual("1.1001011110100011110001110010011100011011000000100001 x 2^74", ToBinary(Evaluate("30078505129381147446200")));
            Assert.AreEqual("1.1000000110000000110101011011101011010010111000111110 x 2^70", ToBinary(Evaluate("1777820000000000000001")));
            Assert.AreEqual("1.0000000000000000000000000000000000000000000000000010 x 2^-1", ToBinary(Evaluate("0.500000000000000166547006220929549868969843373633921146392822265625")));
            Assert.AreEqual("1.0000000000000000000000000000000000000000000000000010 x 2^-1", ToBinary(Evaluate("0.50000000000000016656055874808561867439493653364479541778564453125")));
            Assert.AreEqual("1.1001001010111011001101010010110001000110001000111010 x 2^-2", ToBinary(Evaluate("0.3932922657273")));
            Assert.AreEqual("1.0000000000000000000000000000000000000000000000000010 x 2^45", ToBinary(Evaluate("3.518437208883201171875e13")));
            Assert.AreEqual("1.1111010001001010101111010101101010100111110010100100 x 2^5", ToBinary(Evaluate("62.5364939768271845828")));
            Assert.AreEqual("1.1011110101011100101110101110111100001111110100001100 x 2^-31", ToBinary(Evaluate("8.10109172351e-10")));
            Assert.AreEqual("1.1000000000000000000000000000000000000000000000000000 x 2^0", ToBinary(Evaluate("1.50000000000000011102230246251565404236316680908203125")));
            Assert.AreEqual("1.1111111111111111111111111111111111111111111111111111 x 2^52", ToBinary(Evaluate("9007199254740991.4999999999999999999999999999999995")));
            Assert.AreEqual("1.1111111111111111111111111111111111111111111111111111 x 2^-1023", ToBinary(Evaluate("2.2250738585072011e-308")));

            Assert.AreEqual("SyntaxError", EvaluateExceptionType("5.e"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("5e"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("5e+"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("5e.5"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("0x"));
        }

        [TestMethod]
        public void String()
        {
            Assert.AreEqual("nine", Evaluate("'nine'"));
            Assert.AreEqual("eight", Evaluate("\"eight\""));
            Assert.AreEqual(" \x08 \x09 \x0a \x0b \x0c \x0d \x22 \x27 \x5c \x00 ", Evaluate(@"' \b \t \n \v \f \r \"" \' \\ \0 '"));
            Assert.AreEqual("ÿ", Evaluate(@"'\xfF'"));
            Assert.AreEqual("①ﬄ", Evaluate(@"'\u2460\ufB04'"));
            Assert.AreEqual("line-\r\ncon\rtin\nuation", Evaluate(@"'line-\r\ncon\rtin\nuation'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'unterminated"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'unterminated\r\n"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"'sd\xfgf'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"'te\ufffg'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'te\r\nst'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'test\""));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("\"test'"));
            Assert.AreEqual("test", Evaluate(@"'te\1st'"));

            // ECMAScript 6
            Assert.AreEqual("𠮷", Evaluate(@"'\u{20BB7}'"));
        }

        [TestMethod]
        public void RegExp()
        {
            Assert.AreEqual("abc", Evaluate("/abc/gi.source"));

            // A slash inside a character class does not terminate the regular expression.
            Assert.AreEqual("[/]", Evaluate("/[/]/g.source"));

            // An escaped slash also does not terminate the regular expression.
            Assert.AreEqual(@"\/", Evaluate(@"/\//i.source"));

            // The closing bracket can be escaped.
            Assert.AreEqual(@"[\]/]", Evaluate(@"/[\]/]/i.source"));

            // Escape sequences are allowed in the flags part.
            Assert.AreEqual("/abc/g", Evaluate("(/abc/\u0067).toString()"));

            // Line terminators are not allowed in regexps - even if the line terminator is escaped.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("/\u000A/"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("/\\\u000A/"));

            // Unexpected end of input.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("/"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("/\\"));
        }

        [TestMethod]
        public void OctalNumbers()
        {
            // Octal numbers and escape sequences are not supported in strict mode.
            Assert.AreEqual(0, Evaluate("'use strict'; 0"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; 05"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; 011"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; 0123456701234567"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; 09"));

            // Octal numbers and escape sequences are supported in ECMAScript 5 mode.
            Assert.AreEqual(0, Evaluate("0"));
            Assert.AreEqual(5, Evaluate("05"));
            Assert.AreEqual(9, Evaluate("011"));
            Assert.AreEqual(5744368105847.0, Evaluate("0123456701234567"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("09"));

            // And they are supported in compatibility mode.
            CompatibilityMode = CompatibilityMode.ECMAScript3;
            try
            {
                Assert.AreEqual(0, Evaluate("0"));
                Assert.AreEqual(5, Evaluate("05"));
                Assert.AreEqual(9, Evaluate("011"));
                Assert.AreEqual(5744368105847.0, Evaluate("0123456701234567"));
                Assert.AreEqual("SyntaxError", EvaluateExceptionType("09"));
            }
            finally
            {
                CompatibilityMode = CompatibilityMode.Latest;
            }
        }

        [TestMethod]
        public void OctalEscapeSequence()
        {
            // Octal escape sequences are not supported in strict mode.
            Assert.AreEqual("\0", Evaluate("'use strict'; '\\0'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; '\\05'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; '\\05a'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; '\\011'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; '\\0377'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; '\\0400'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; '\\09'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; '\\0444'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; '\\44'"));

            // Octal escape sequences are supported in ECMAScript 5 mode.
            Assert.AreEqual("\0", Evaluate("'\\0'"));
            Assert.AreEqual("\u0005", Evaluate("'\\05'"));
            Assert.AreEqual("\u0005Z", Evaluate("'\\05Z'"));
            Assert.AreEqual("\u0009", Evaluate("'\\011'"));
            Assert.AreEqual("\u001F7", Evaluate("'\\0377'"));
            Assert.AreEqual("\u00200", Evaluate("'\\0400'"));
            Assert.AreEqual("$4", Evaluate("'\\0444'"));
            Assert.AreEqual("$", Evaluate("'\\44'"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'\\09'"));

            // And they are supported in compatibility mode.
            CompatibilityMode = CompatibilityMode.ECMAScript3;
            try
            {
                Assert.AreEqual("\0", Evaluate("'\\0'"));
                Assert.AreEqual("\u0005", Evaluate("'\\05'"));
                Assert.AreEqual("\u0005Z", Evaluate("'\\05Z'"));
                Assert.AreEqual("\u0009", Evaluate("'\\011'"));
                Assert.AreEqual("\u001F7", Evaluate("'\\0377'"));
                Assert.AreEqual("\u00200", Evaluate("'\\0400'"));
                Assert.AreEqual("$4", Evaluate("'\\0444'"));
                Assert.AreEqual("$", Evaluate("'\\44'"));
                Assert.AreEqual("SyntaxError", EvaluateExceptionType("'\\09'"));
            }
            finally
            {
                CompatibilityMode = CompatibilityMode.Latest;
            }
        }

        [TestMethod]
        public void DivisionAmbiguity()
        {
            // Division and regular expressions are ambiguous in the lexical grammar.  The parser
            // is required to resolve the ambiguity.
            Assert.AreEqual("/abc/", Evaluate("/abc/.toString()"));
            Assert.AreEqual(3, Evaluate("abc = 2; 6/abc"));
            Assert.AreEqual("/abc/", Evaluate("if (true) /abc/.toString()"));
            Assert.AreEqual(1.5, Evaluate("g = 2; (5 + 1) /2/g"));
            Assert.AreEqual("/abc/", Evaluate("if (true) {} /abc/.toString()"));
            Assert.AreEqual(double.NaN, Evaluate("a = {} / 2"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("{} / 2"));
        }



        //     HELPER METHODS
        //_________________________________________________________________________________________

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
