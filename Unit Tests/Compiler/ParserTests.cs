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
        public void AutomaticSemicolonInsertion()
        {
            Assert.AreEqual(6, Evaluate("x = 1 + \r\n 5"));
            Assert.AreEqual("11,2", Evaluate("x = 1 + \r\n [1, 2]"));

            // Var statement.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var y = 6; var x = y ++ 5"));
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
            Assert.AreEqual("TypeError", EvaluateExceptionType(@"
                var a = 1, b = 2, c = 3, d = 4, e = 5;
                a = b + c
                (d + e).toString()"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType(@"
                var i,s
                s='here is a string'
                i=0
                /[a-z]/g.exec(s)"));
            
        }

        [TestMethod]
        public void Operators()
        {
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("++"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("*"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("5 *"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("!"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("! * 5"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("("));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("(1"));
        }

        [TestMethod]
        public void TemplateLiterals()
        {
            Assert.AreEqual("nine", Evaluate("`nine`"));

            // New lines are allowed and included in the resulting string.
            Assert.AreEqual("ni\r\nne", Evaluate("`ni\r\nne`"));
            Assert.AreEqual("line 1  \r\n  line 2", Evaluate("`line 1  \r\n  line 2`"));

            // Escape sequences
            Assert.AreEqual(" \x08 \x09 \x0a \x0b \x0c \x0d \x22 \x27 \x5c \x00 ", Evaluate(@"` \b \t \n \v \f \r \"" \' \\ \0 `"));
            Assert.AreEqual(@" $ $$ $\ ", Evaluate(@"` $ $$ $\\ `"));

            // Substitutions
            Assert.AreEqual("1 + 1 = 2", Evaluate("`1 + 1 = ${1 + 1}`"));
            Assert.AreEqual("Fifteen is 15 and not 20.", Evaluate("var a = 5; var b = 10; `Fifteen is ${a + b} and not ${2 * a + b}.`"));
            Assert.AreEqual("Object [object Object]", Evaluate("`Object ${{}}`"));

            // Nested substitutions
            Assert.AreEqual("1 + 1 = Hello 3 world", Evaluate("`1 + 1 = ${`Hello ${3} world`}`"));

            // Tagged templates
            Assert.AreEqual("Hello ,  world ,  | 15 | 50 | undefined", Evaluate(@"
                function tag(strings, value1, value2, value3) {
                    return strings.join(', ') + ' | ' + value1 + ' | ' + value2 + ' | ' + value3;
                }

                var a = 5, b = 10;
                tag`Hello ${a + b} world ${a * b}`;"));
            Assert.AreEqual("3 | ,, | 11 | 2 | undefined", Evaluate(@"
                function tag(strings, value1, value2, value3) {
                    return strings.length + ' | ' + strings.join(',') + ' | ' + value1 + ' | ' + value2 + ' | ' + value3;
                }
                tag `${11}${2}`;"));

            // Raw strings.
            Assert.AreEqual("2 | one\r\n,\r\nthree | two | undefined | undefined", Evaluate(@"
                function tag(strings, value1, value2, value3) {
                    return strings.raw.length + ' | ' + strings.raw.join(',') + ' | ' + value1 + ' | ' + value2 + ' | ' + value3;
                }
                tag `one\r\n${ 'two'}\r\nthree`;"));

            // Syntax errors
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`unterminated"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`unterminated\r\n"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"`sd\xfgf`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"`te\ufffg`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`test\""));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`test'"));

            // Octal escape sequences are not supported in templates.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\05`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\05a`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\011`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\0377`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\0400`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\09`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\0444`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\44`"));


            Assert.AreEqual("nine", Evaluate("`nine`"));
            Assert.AreEqual("nine", Evaluate("`nine`"));
        }
    }
}
