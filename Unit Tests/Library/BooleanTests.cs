using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the global Boolean object.
    /// </summary>
    [TestClass]
    public class BooleanTests
    {
        [TestMethod]
        public void Constructor()
        {
            // Construct
            Assert.AreEqual(false, TestUtils.Evaluate("new Boolean().valueOf()"));
            Assert.AreEqual(false, TestUtils.Evaluate("new Boolean(false).valueOf()"));
            Assert.AreEqual(true, TestUtils.Evaluate("new Boolean(true).valueOf()"));

            // Call
            Assert.AreEqual(false, TestUtils.Evaluate("Boolean()"));
            Assert.AreEqual(false, TestUtils.Evaluate("Boolean(false)"));
            Assert.AreEqual(true, TestUtils.Evaluate("Boolean(true)"));
            Assert.AreEqual(false, TestUtils.Evaluate("Boolean(0)"));
            Assert.AreEqual(true, TestUtils.Evaluate("Boolean(1)"));
            Assert.AreEqual(true, TestUtils.Evaluate("Boolean(2)"));

            // toString and valueOf.
            Assert.AreEqual("function Boolean() { [native code] }", TestUtils.Evaluate("Boolean.toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("Boolean.valueOf() === Boolean"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Boolean.length"));
        }

        [TestMethod]
        public void Properties()
        {
            // Constructor and __proto__
            Assert.AreEqual(true, TestUtils.Evaluate("new Boolean().constructor === Boolean"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new Boolean()) === Boolean.prototype"));

            // No initial enumerable properties.
            Assert.AreEqual("", TestUtils.Evaluate("y = ''; for (var x in Boolean) { y += x } y"));
            Assert.AreEqual("", TestUtils.Evaluate("y = ''; for (var x in new Boolean(5)) { y += x } y"));
            Assert.AreEqual("", TestUtils.Evaluate("y = ''; for (var x in false) { y += x } y"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("false", TestUtils.Evaluate("false.toString()"));
            Assert.AreEqual("true", TestUtils.Evaluate("true.toString()"));
            Assert.AreEqual("false", TestUtils.Evaluate("new Boolean(false).toString()"));
            Assert.AreEqual("true", TestUtils.Evaluate("new Boolean(true).toString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("Boolean.prototype.toString.length"));
        }
    }
}
