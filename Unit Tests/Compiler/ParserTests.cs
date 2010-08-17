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
            // Automatic semi-colon insertion.
            Assert.AreEqual(6, TestUtils.Evaluate("x = 1 + \r\n 5"));
            Assert.AreEqual("11,2", TestUtils.Evaluate("x = 1 + \r\n [1, 2]"));

            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("var y = 6; var x = y ++ 5"));
            Assert.AreEqual(5, TestUtils.Evaluate("var y = 6; var x = y ++ \r\n 5"));
            TestUtils.Evaluate("var y = 6; var x = y ++ \r\n function f() {}");

            TestUtils.Evaluate("var x = \r\nfunction f() { }");
            TestUtils.Evaluate("var x = 5\r\nfunction f() { }");

            TestUtils.Evaluate("x = { a: 1 }; y = x.a \r\n var b;");
        }

    }
}
