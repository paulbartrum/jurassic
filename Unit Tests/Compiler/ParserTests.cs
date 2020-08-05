using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the Parser object.
    /// </summary>
    [TestClass]
    public class ParserTests : TestBase
    {
        [TestMethod]
        [Ignore]
        public void AutomaticSemicolonInsertion()
        {
            Assert.AreEqual(6, Evaluate("x = 1 + \r\n 5"));
            Assert.AreEqual("11,2", Evaluate("x = 1 + \r\n [1, 2]"));

            // Var statement.
            Assert.AreEqual("SyntaxError: Expected operator but found '5'", EvaluateExceptionMessage("var y = 6; var x = y ++ 5"));
            Assert.AreEqual(5, Evaluate("var y = 6; var x = y ++ \r\n 5"));
            Evaluate("var y = 6; var x = y ++ \r\n function f() {}");
            Evaluate("var x = \r\nfunction f() { }");
            Evaluate("var x = 5\r\nfunction f() { }");
            Evaluate("x = { a: 1 }; y = x.a \r\n var b;");
            Evaluate("var a \n , b");

            // Check post-increment operator.
            Assert.AreEqual(0, Evaluate("var x = 0, y = 0; x ++ \n y"));
            Assert.AreEqual(1, Evaluate("x"));
            Assert.AreEqual(0, Evaluate("y"));

            // Check pre-increment operator.
            Assert.AreEqual(1, Evaluate("var x = 0, y = 0; x \n ++ y"));
            Assert.AreEqual(0, Evaluate("x"));
            Assert.AreEqual(1, Evaluate("y"));
            Assert.AreEqual(1, Evaluate("var x = 0, y = 0; x \n ++ \n y"));
            Assert.AreEqual(0, Evaluate("x"));
            Assert.AreEqual(1, Evaluate("y"));

            // Do while.
            Assert.AreEqual(true, Evaluate("do { } while (false) true"));
            Assert.AreEqual(true, Evaluate("do; while \r\n (false) true"));

            // Ambiguity.
            Assert.AreEqual("TypeError: 'c' is not a function", EvaluateExceptionMessage(@"
                var a = 1, b = 2, c = 3, d = 4, e = 5;
                a = b + c
                (d + e).toString()"));
            Assert.AreEqual("ReferenceError: z is not defined.", EvaluateExceptionMessage(@"
                var i,s
                s='here is a string'
                i=0
                /[a-z]/g.exec(s)"));
            
        }

        [TestMethod]
        public void Operators()
        {
            Assert.AreEqual("SyntaxError: Wrong number of operands", EvaluateExceptionMessage("++"));
            Assert.AreEqual("SyntaxError: Unexpected token '*' in expression.", EvaluateExceptionMessage("*"));
            Assert.AreEqual("SyntaxError: Wrong number of operands", EvaluateExceptionMessage("5 *"));
            Assert.AreEqual("SyntaxError: Wrong number of operands", EvaluateExceptionMessage("!"));
            Assert.AreEqual("SyntaxError: Unexpected token '*' in expression.", EvaluateExceptionMessage("! * 5"));
            Assert.AreEqual("SyntaxError: Wrong number of operands", EvaluateExceptionMessage("("));
            Assert.AreEqual("SyntaxError: Missing closing token ')'", EvaluateExceptionMessage("(1"));

            // Two operators in a row
            Execute("var op = { test: 5, '1': 3 };");
            Assert.AreEqual(6, Evaluate("++op.test"));
            Assert.AreEqual(4, Evaluate("++op[1]"));
            Assert.AreEqual(true, Evaluate("isNaN((op++).test)"));
            Assert.AreEqual(true, Evaluate("isNaN((op++)[0])"));
            Assert.AreEqual("SyntaxError: Unexpected token '.' in expression", EvaluateExceptionMessage("op++.test"));
            Assert.AreEqual("SyntaxError: Unexpected token '[' in expression", EvaluateExceptionMessage("op++[1]"));
            Assert.AreEqual("SyntaxError: Unexpected token '(' in expression", EvaluateExceptionMessage("op++(1)"));
            Assert.AreEqual("SyntaxError: Unexpected token '++' in expression", EvaluateExceptionMessage("new ++op"));
            Assert.AreEqual("SyntaxError: Unexpected token '.' in expression.", EvaluateExceptionMessage("op+.test"));
        }
    }
}
