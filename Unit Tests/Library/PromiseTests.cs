using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the global Promise object.
    /// </summary>
    [TestClass]
    public class PromiseTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", EvaluateExceptionType("Promise()"));

            // Construct
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Promise()"));
            Assert.AreEqual("[object Promise]", Evaluate("new Promise(function(resolve, reject) { }).toString()"));
            Assert.AreEqual("s", Evaluate("var f; var x = new Promise(function(resolve, reject) { f = resolve; }); f.name"));
            Assert.AreEqual("t", Evaluate("var f; var x = new Promise(function(resolve, reject) { f = reject; }); f.name"));

            // toString and valueOf.
            Assert.AreEqual("function Promise() { [native code] }", Evaluate("Promise.toString()"));
            Assert.AreEqual("Promise", Evaluate("Promise.prototype[Symbol.toStringTag]"));
            Assert.AreEqual(true, Evaluate("Promise.valueOf() === Promise"));

            // species
            Assert.AreEqual(true, Evaluate("Promise[Symbol.species] === Promise"));

            // length
            Assert.AreEqual(1, Evaluate("Promise.length"));
        }

        // TODO: Promise.all, Promise.race etc.
    }
}
