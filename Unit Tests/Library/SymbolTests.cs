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

            // typeof
            Assert.AreEqual("symbol", Evaluate("typeof Symbol()"));

            // length
            Assert.AreEqual(0, Evaluate("Symbol.length"));
        }

        // TODO: Symbol.iterator, Symbol.for, Symbol.keyFor, etc.
    }
}
