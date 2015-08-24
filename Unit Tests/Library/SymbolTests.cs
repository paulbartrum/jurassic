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
    public class SymbolTests
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("Symbol()", TestUtils.Evaluate("Symbol().toString()"));
            Assert.AreEqual("Symbol(one)", TestUtils.Evaluate("Symbol('one').toString()"));

            // Construct
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Symbol()"));

            // toString and valueOf.
            Assert.AreEqual("function Symbol() { [native code] }", TestUtils.Evaluate("Symbol.toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("Symbol.valueOf() === Symbol"));

            // typeof
            Assert.AreEqual("symbol", TestUtils.Evaluate("typeof Symbol()"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Symbol.length"));
        }

        // TODO: Symbol.iterator, Symbol.for, Symbol.keyFor, etc.
    }
}
