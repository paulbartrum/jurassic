using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global Symbol object.
    /// </summary>
    [TestClass]
    public class SymbolTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("Symbol()", Evaluate("Symbol().toString()"));
            Assert.AreEqual("Symbol(one)", Evaluate("Symbol('one').toString()"));

            // Construct
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Symbol()"));

            // toString and valueOf.
            Assert.AreEqual("function Symbol() { [native code] }", Evaluate("Symbol.toString()"));
            Assert.AreEqual(true, Evaluate("Symbol.valueOf() === Symbol"));
            Assert.AreEqual("Symbol", Evaluate("Symbol.prototype[Symbol.toStringTag]"));

            // Symbols can be explicitly converted to strings.
            Assert.AreEqual("Symbol(foo)", Evaluate("String(Symbol('foo'))"));

            // ...but not implicitly
            Assert.AreEqual("TypeError: Cannot convert a Symbol value to a string.", EvaluateExceptionMessage("Symbol('foo') + ''"));

            // typeof
            Assert.AreEqual("symbol", Evaluate("typeof Symbol()"));

            // length
            Assert.AreEqual(0, Evaluate("Symbol.length"));
        }

        [TestMethod]
        public void toPrimitive()
        {
            Assert.AreEqual("1default2", Evaluate("var x = { a: 1 }; x[Symbol.toPrimitive] = function (hint) { return this.a + hint; }; x + 2"));
            Assert.AreEqual("1defaulttest", Evaluate("var x = { a: 1 }; x[Symbol.toPrimitive] = function (hint) { return this.a + hint; }; x + 'test'"));
        }

        // TODO: Symbol.iterator, Symbol.for, Symbol.keyFor, etc.
    }
}
