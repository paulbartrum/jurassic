using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Compiler;

namespace UnitTests
{
    /// <summary>
    /// Test the Lexer object.
    /// </summary>
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void Comments()
        {
            // Single-line comment.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("// testing"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("// testing\r\n"));

            // Multi-line comment.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("/* testing */"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("/* test\r\ning */"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("/*\r\n test\r\ning */"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("/* testing \r\n*/"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("/* test\ning */"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("/* test\ring */"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("/* te*sting**/"));
        }

        [TestMethod]
        public void Identifiers()
        {
            Assert.AreEqual(5, TestUtils.Evaluate("delete $; $ = 5; $"));
            Assert.AreEqual(6, TestUtils.Evaluate("delete dung; d\\u0075ng = 6; dung"));
            Assert.AreEqual(7, TestUtils.Evaluate("delete another; \\u0061nother = 7; another"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("ident\\u0020ifier"));
        }

        [TestMethod]
        public void Keywords()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("false"));
            Assert.AreEqual(true, TestUtils.Evaluate("true"));
            Assert.AreEqual(Null.Value, TestUtils.Evaluate("null"));

            // Reserved words.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("class = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("enum = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("extends = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("super = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("const = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("export = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("import = 1"));

            // Future reserved words.
            Assert.AreEqual(1, TestUtils.Evaluate("implements = 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("let = 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("private = 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("public = 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("yield = 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("interface = 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("package = 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("protected = 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("static = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; implements = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; let = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; private = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; public = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; yield = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; interface = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; package = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; protected = 1"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; static = 1"));

            Assert.AreEqual(Null.Value, TestUtils.Evaluate("null"));
            Assert.AreEqual(Null.Value, TestUtils.Evaluate("null"));
        }

        [TestMethod]
        public void Number()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("0"));
            Assert.AreEqual(34, TestUtils.Evaluate("34"));
            Assert.AreEqual(34.5, TestUtils.Evaluate("34.5"));
            Assert.AreEqual(3400, TestUtils.Evaluate("34e2"));
            Assert.AreEqual(3.45, TestUtils.Evaluate("34.5e-1"));
            Assert.AreEqual(0.345, TestUtils.Evaluate("34.5E-2"));
            Assert.AreEqual(11, TestUtils.Evaluate(" 11"));
            Assert.AreEqual(0.5, TestUtils.Evaluate("0.5"));
            Assert.AreEqual(0.005, TestUtils.Evaluate("0.005"));
            Assert.AreEqual(255, TestUtils.Evaluate("0xff"));
            Assert.AreEqual(241, TestUtils.Evaluate("0xF1"));
            Assert.AreEqual(0.5, TestUtils.Evaluate(".5"));
            Assert.AreEqual(50, TestUtils.Evaluate(".5e2"));
            Assert.AreEqual(5, TestUtils.Evaluate("5."));
            Assert.AreEqual(0.07, TestUtils.Evaluate(".7e-1"));
            Assert.AreEqual(0.11, TestUtils.Evaluate("1.1e-1"));
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("1e1000000000"));
            Assert.AreEqual(0, TestUtils.Evaluate("1e-1000000000"));
            Assert.AreEqual(244837814094590.0, TestUtils.Evaluate("0xdeadbeefcafe"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("5.e"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("5e"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("5e+"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("5e.5"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("0x"));
        }

        [TestMethod]
        public void String()
        {
            Assert.AreEqual("nine", TestUtils.Evaluate("'nine'"));
            Assert.AreEqual("eight", TestUtils.Evaluate("\"eight\""));
            Assert.AreEqual(" \x08 \x09 \x0a \x0b \x0c \x0d \x22 \x27 \x5c \x00 ", TestUtils.Evaluate(@"' \b \t \n \v \f \r \"" \' \\ \0 '"));
            Assert.AreEqual("ÿ", TestUtils.Evaluate(@"'\xfF'"));
            Assert.AreEqual("①ﬄ", TestUtils.Evaluate(@"'\u2460\ufB04'"));
            Assert.AreEqual("line-\r\ncon\rtin\nuation", TestUtils.Evaluate(@"'line-\r\ncon\rtin\nuation'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'unterminated"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'unterminated\r\n"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"'sd\xfgf'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"'te\ufffg'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'te\r\nst'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'test\""));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("\"test'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"'te\1st'"));
        }

        [TestMethod]
        public void RegExp()
        {
            Assert.AreEqual("abc", TestUtils.Evaluate("/abc/gi.source"));

            // A slash inside a character class does not terminate the regular expression.
            Assert.AreEqual("[/]", TestUtils.Evaluate("/[/]/g.source"));

            // An escaped slash also does not terminate the regular expression.
            Assert.AreEqual(@"\/", TestUtils.Evaluate(@"/\//i.source"));

            // The closing bracket can be escaped.
            Assert.AreEqual(@"[\]/]", TestUtils.Evaluate(@"/[\]/]/i.source"));

            // Escape sequences are allowed in the flags part.
            Assert.AreEqual("/abc/g", TestUtils.Evaluate("(/abc/\u0067).toString()"));

            // Line terminators are not allowed in regexps - even if the line terminator is escaped.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("/\u000A/"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("/\\\u000A/"));

            // Unexpected end of input.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("/"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("/\\"));
        }

        [TestMethod]
        public void OctalNumbers()
        {
            // Octal numbers and escape sequences are not supported in ECMAScript 5 mode.
            Assert.AreEqual(0, TestUtils.Evaluate("0"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("05"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("011"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("0123456701234567"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("09"));

            // But they are supported in compatibility mode.
            TestUtils.CompatibilityMode = CompatibilityMode.ECMAScript3;
            try
            {
                Assert.AreEqual(0, TestUtils.Evaluate("0"));
                Assert.AreEqual(5, TestUtils.Evaluate("05"));
                Assert.AreEqual(9, TestUtils.Evaluate("011"));
                Assert.AreEqual(5744368105847.0, TestUtils.Evaluate("0123456701234567"));
                Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("09"));
            }
            finally
            {
                TestUtils.CompatibilityMode = CompatibilityMode.Latest;
            }
        }

        [TestMethod]
        public void OctalEscapeSequence()
        {
            // Octal escape sequences are not supported in ECMAScript 5 mode.
            Assert.AreEqual("\0", TestUtils.Evaluate("'\\0'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'\\05'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'\\05a'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'\\011'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'\\0377'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'\\0400'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'\\09'"));

            // But they are supported in compatibility mode.
            TestUtils.CompatibilityMode = CompatibilityMode.ECMAScript3;
            try
            {
                Assert.AreEqual("\0", TestUtils.Evaluate("'\\0'"));
                Assert.AreEqual("\u0005", TestUtils.Evaluate("'\\05'"));
                Assert.AreEqual("\u0005Z", TestUtils.Evaluate("'\\05Z'"));
                Assert.AreEqual("\u0009", TestUtils.Evaluate("'\\011'"));
                Assert.AreEqual("\u00FF", TestUtils.Evaluate("'\\0377'"));
                Assert.AreEqual("\u00200", TestUtils.Evaluate("'\\0400'"));
                Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'\\09'"));
            }
            finally
            {
                TestUtils.CompatibilityMode = CompatibilityMode.Latest;
            }
        }

        [TestMethod]
        public void DivisionAmbiguity()
        {
            // Division and regular expressions are ambiguous in the lexical grammar.  The parser
            // is required to resolve the ambiguity.
            Assert.AreEqual("/abc/", TestUtils.Evaluate("/abc/.toString()"));
            Assert.AreEqual(3, TestUtils.Evaluate("abc = 2; 6/abc"));
            Assert.AreEqual("/abc/", TestUtils.Evaluate("if (true) /abc/.toString()"));
            Assert.AreEqual(1.5, TestUtils.Evaluate("g = 2; (5 + 1) /2/g"));
            Assert.AreEqual("/abc/", TestUtils.Evaluate("if (true) {} /abc/.toString()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("a = {} / 2"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("{} / 2"));
        }
    }
}
