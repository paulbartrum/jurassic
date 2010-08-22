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
    /// Test the Parser object.
    /// </summary>
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void AutomaticSemicolonInsertion()
        {
            Assert.AreEqual(6, TestUtils.Evaluate("x = 1 + \r\n 5"));
            Assert.AreEqual("11,2", TestUtils.Evaluate("x = 1 + \r\n [1, 2]"));

            // Var statement.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("var y = 6; var x = y ++ 5"));
            Assert.AreEqual(5, TestUtils.Evaluate("var y = 6; var x = y ++ \r\n 5"));
            TestUtils.Evaluate("var y = 6; var x = y ++ \r\n function f() {}");
            TestUtils.Evaluate("var x = \r\nfunction f() { }");
            TestUtils.Evaluate("var x = 5\r\nfunction f() { }");
            TestUtils.Evaluate("x = { a: 1 }; y = x.a \r\n var b;");
            TestUtils.Evaluate("var a \n , b");

            // Check post-increment operator.
            Assert.AreEqual(0, TestUtils.Evaluate("var x = 0, y = 0; x ++ \n y"));
            Assert.AreEqual(1, TestUtils.Evaluate("x"));
            Assert.AreEqual(0, TestUtils.Evaluate("y"));

            // Check pre-increment operator.
            Assert.AreEqual(1, TestUtils.Evaluate("var x = 0, y = 0; x \n ++ y"));
            Assert.AreEqual(0, TestUtils.Evaluate("x"));
            Assert.AreEqual(1, TestUtils.Evaluate("y"));
            Assert.AreEqual(1, TestUtils.Evaluate("var x = 0, y = 0; x \n ++ \n y"));
            Assert.AreEqual(0, TestUtils.Evaluate("x"));
            Assert.AreEqual(1, TestUtils.Evaluate("y"));

            // Ambiguity.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType(@"
                var a = 1, b = 2, c = 3, d = 4, e = 5;
                a = b + c
                (d + e).toString()"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType(@"
                var i,s
                s='here is a string'
                i=0
                /[a-z]/g.exec(s)"));
            
        }

        [TestMethod]
        public void Operators()
        {
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("++"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("*"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("5 *"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("!"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("! * 5"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("("));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("(1"));
        }

    }
}
