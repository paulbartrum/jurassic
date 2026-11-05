using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test ECMAScript 6 arrow functions.
    /// </summary>
    [TestClass]
    public class ArrowFunctionTests : TestBase
    {
        [TestMethod]
        public void SingleParameter()
        {
            Assert.AreEqual(4, Evaluate("var f = x => x * 2; f(2)"));
            Assert.AreEqual(4, Evaluate("var f = (x) => x * 2; f(2)"));
            Assert.AreEqual(3, Evaluate("var f = x => { return x + 1; }; f(2)"));
        }

        [TestMethod]
        public void MultipleParameters()
        {
            Assert.AreEqual(5, Evaluate("var f = (a, b) => a + b; f(2, 3)"));
            Assert.AreEqual(5, Evaluate("var f = (a, b) => { return a + b; }; f(2, 3)"));
            Assert.AreEqual(Jurassic.Undefined.Value, Evaluate("var f = (a, b) => { }; f(2, 3)"));
        }

        [TestMethod]
        public void NoParameters()
        {
            Assert.AreEqual(7, Evaluate("var f = () => 7; f()"));
            Assert.AreEqual(7, Evaluate("var f = () => { return 7; }; f()"));
        }

        [TestMethod]
        public void DefaultParameters()
        {
            Assert.AreEqual(11, Evaluate("var f = (a, b = 10) => a + b; f(1)"));
            Assert.AreEqual(3, Evaluate("var f = (a, b = 10) => a + b; f(1, 2)"));
        }

        [TestMethod]
        public void DefaultParametersReferToEarlierParameters()
        {
            Assert.AreEqual(2, Evaluate("var f = (a, b = a) => a + b; f(1)"));
            Assert.AreEqual(3, Evaluate("var f = (a, b = a + 1) => a + b; f(1, 2)"));
            Assert.AreEqual(5, Evaluate("var a = 100; var f = (a, b = [a + 1][0]) => b; f(4)"));
            Assert.AreEqual(5, Evaluate("var a = 100; var f = (a, b = { v: a + 1 }) => b.v; f(4)"));
            Assert.AreEqual(5, Evaluate("var a = 100; var f = (a, b = `${a + 1}`) => b; f(4)") is string x ? int.Parse(x) : -1);
            Assert.AreEqual(5, Evaluate("var a = 100; var f = (a, b = (() => a + 1)()) => b; f(4)"));
            Assert.AreEqual(5, Evaluate("function g(x) { return x + 1; } var f = (a, b = g(a)) => b; f(4)"));
            // Defaults can still see outer variables.
            Assert.AreEqual(101, Evaluate("var o = 100; var f = (a, b = o + a) => b; f(1)"));
        }

        [TestMethod]
        public void ThisInDefaultParameters()
        {
            Assert.AreEqual(5, Evaluate("var o = { v: 5, f: function() { return ((a = this.v) => a)(); } }; o.f()"));
            Assert.AreEqual(5, Evaluate("var o = { v: 5, f: function() { return ((a = this.v) => a).call({ v: 6 }); } }; o.f()"));
            Assert.AreEqual(5, Evaluate("var o = { v: 5, f: function() { return (() => (a = this.v) => a)()(); } }; o.f()"));
        }

        [TestMethod]
        public void AsArgument()
        {
            Assert.AreEqual("2,4,6", Evaluate("[1, 2, 3].map(x => x * 2).join(',')"));
            Assert.AreEqual("2,3", Evaluate("[1, 2, 3].filter((x, i) => i > 0).join(',')"));
            Assert.AreEqual(6, Evaluate("[1, 2, 3].reduce((a, b) => a + b, 0)"));
            Assert.AreEqual(5, Evaluate("function f(a, b) { return a(1) + b; } f(x => x + 1, 3)"));
        }

        [TestMethod]
        public void InExpressions()
        {
            Assert.AreEqual(2, Evaluate("var f; f = x => x + 1, f(1)"));
            Assert.AreEqual(3, Evaluate("var c = true; var f = c ? x => x + 1 : x => x - 1; f(2)"));
            Assert.AreEqual(1, Evaluate("var c = false; var f = c ? x => x + 1 : x => x - 1; f(2)"));
            Assert.AreEqual(3, Evaluate("(x => x + 1)(2)"));
            Assert.AreEqual(3, Evaluate("((x, y) => x + y)(1, 2)"));
            Assert.AreEqual(5, Evaluate("var o = { f: x => x + 1 }; o.f(4)"));
            Assert.AreEqual(5, Evaluate("var a = [x => x + 1]; a[0](4)"));
        }

        [TestMethod]
        public void Nested()
        {
            Assert.AreEqual(6, Evaluate("var f = a => b => c => a + b + c; f(1)(2)(3)"));
            Assert.AreEqual(3, Evaluate("var f = (a) => (b) => a + b; f(1)(2)"));
        }

        [TestMethod]
        public void Closures()
        {
            Assert.AreEqual(3, Evaluate("function make() { var n = 0; return () => ++n; } var c = make(); c(); c(); c()"));
        }

        [TestMethod]
        public void LexicalThis()
        {
            Assert.AreEqual(5, Evaluate("var o = { v: 5, f: function() { return (() => this.v)(); } }; o.f()"));
            Assert.AreEqual(5, Evaluate("var o = { v: 5, f: function() { return [1].map(() => this.v)[0]; } }; o.f()"));
            Assert.AreEqual(5, Evaluate("var o = { v: 5, f: function() { return (() => () => this.v)()(); } }; o.f()"));
            // Calling with a different "this" has no effect.
            Assert.AreEqual(5, Evaluate("var o = { v: 5, f: function() { return (() => this.v).call({ v: 6 }); } }; o.f()"));
            Assert.AreEqual(5, Evaluate("function C() { this.v = 5; this.f = () => this.v; } var c = new C(); var f = c.f; f()"));
        }

        [TestMethod]
        public void LexicalArguments()
        {
            Assert.AreEqual(3, Evaluate("function f() { return (() => arguments.length)(); } f(1, 2, 3)"));
            Assert.AreEqual(2, Evaluate("function f() { return (() => arguments[0])(); } f(2)"));
        }

        [TestMethod]
        public void NotAConstructor()
        {
            Assert.AreEqual("TypeError", EvaluateExceptionType("var f = () => 1; new f()"));
            Assert.AreEqual(Jurassic.Undefined.Value, Evaluate("var f = () => 1; f.prototype"));
        }

        [TestMethod]
        public void ErrorCases()
        {
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = (a, 1) => a;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = (a, a) => a;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = a\n=> a;"));
        }

        [TestMethod]
        public void TrailingComma()
        {
            Assert.AreEqual(3, Evaluate("var f = (a, b,) => a + b; f(1, 2)"));
            Assert.AreEqual(1, Evaluate("var f = (a,) => a; f(1)"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = (,) => 1;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = (a,,) => 1;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("(1, 2,)"));
        }

        [TestMethod]
        public void Length()
        {
            Assert.AreEqual(0, Evaluate("(() => 1).length"));
            Assert.AreEqual(2, Evaluate("((a, b) => 1).length"));
            Assert.AreEqual(1, Evaluate("((a, b = 1) => 1).length"));
            Assert.AreEqual(0, Evaluate("((a = 1, b) => 1).length"));
            Assert.AreEqual(1, Evaluate("(function(a, b = 1) {}).length"));
        }

        [TestMethod]
        public void UseStrictWithNonSimpleParameters()
        {
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = (a = 1) => { 'use strict'; };"));
            Assert.AreEqual(1, Evaluate("var f = (a) => { 'use strict'; return a; }; f(1)"));
        }

        [TestMethod]
        public void RestrictedProperties()
        {
            Assert.AreEqual(false, Evaluate("var f = () => 1; f.hasOwnProperty('caller') || f.hasOwnProperty('arguments')"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("var f = () => 1; f.caller"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("var f = () => 1; f.arguments"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("var f = () => 1; f.caller = {}"));
            // Ordinary functions are unaffected.
            Assert.AreEqual(Jurassic.Undefined.Value, Evaluate("function g() {} g.caller"));
        }

        [TestMethod]
        public void ThisIsLexicalAtTopLevel()
        {
            Assert.AreEqual(true, Evaluate("var usurper = {}; var seen; [1].forEach(v => { seen = this; }, usurper); seen !== usurper"));
            Assert.AreEqual(true, Evaluate("var f = () => this; f.call({}) === this"));
        }

        [TestMethod]
        public void PrecedenceChecks()
        {
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("1 + x => x"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("1 + (x) => x"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("1 + () => 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("!x => x"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("typeof x => x"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("a || x => x"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("a.b => 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = x => {} + 1"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = x => {}.y"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = x => {}(1)"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = x => {} ? 1 : 2"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var f = x => {} = 1"));
            // Valid uses.
            Assert.AreEqual(1, Evaluate("var f = x => {}\n+1; 1"));
            Assert.AreEqual(2, Evaluate("var f; f = x => x, 2"));
            Assert.AreEqual(3, Evaluate("var f = true ? x => 3 : 4; f()"));
            Assert.AreEqual(4, Evaluate("var f = false ? 3 : x => 4; f()"));
            Assert.AreEqual(7, Evaluate("var f = [x => 7][0]; f()"));
            Assert.AreEqual(7, Evaluate("var o = {}; o.f = x => 7; o.f()"));
            Assert.AreEqual(8, Evaluate("var f; f += 1, f = x => 8; f()"));
            Assert.AreEqual(9, Evaluate("var a = [1]; a[(x => 0)()] = 9; a[0]"));
            Assert.AreEqual(1, Evaluate("var f = x => {};\n1"));
            Assert.AreEqual(1, Evaluate("function g(a, b) { return b; } g((x => {}), 1)"));
            Assert.AreEqual(1, Evaluate("function g(a, b) { return b; } g(x => {}, 1)"));
        }

        [TestMethod]
        public void GroupingStillWorks()
        {
            Assert.AreEqual(6, Evaluate("var a = 1, b = 2, c = 3; (a + b + c)"));
            Assert.AreEqual(3, Evaluate("var a = 1, b = 2; (a, b, a + b)"));
            Assert.AreEqual(5, Evaluate("var a = 1; (a) = 5; a"));
        }

        [TestMethod]
        public void IsFunction()
        {
            Assert.AreEqual("function", Evaluate("typeof (() => 1)"));
            Assert.AreEqual(2, Evaluate("var f = (a, b) => a; f.length"));
        }
    }
}
