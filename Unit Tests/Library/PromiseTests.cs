using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global Promise object.
    /// </summary>
    [TestClass]
    public class PromiseTests
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Promise()"));

            // Construct
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Promise()"));
            Assert.AreEqual("", TestUtils.Evaluate("new Promise(function(resolve, reject) { }).toString()"));

            // toString and valueOf.
            Assert.AreEqual("function Promise() { [native code] }", TestUtils.Evaluate("Promise.toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("Promise.valueOf() === Promise"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Promise.length"));
        }

        // TODO: Promise.all, Promise.race etc.
    }
}
