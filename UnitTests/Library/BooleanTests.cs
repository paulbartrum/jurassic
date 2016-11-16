using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the global Boolean object.
    /// </summary>
    [TestClass]
    public class BooleanTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Construct
            Assert.AreEqual(false, Evaluate("new Boolean().valueOf()"));
            Assert.AreEqual(false, Evaluate("new Boolean(false).valueOf()"));
            Assert.AreEqual(true, Evaluate("new Boolean(true).valueOf()"));

            // Call
            Assert.AreEqual(false, Evaluate("Boolean()"));
            Assert.AreEqual(false, Evaluate("Boolean(false)"));
            Assert.AreEqual(true, Evaluate("Boolean(true)"));
            Assert.AreEqual(false, Evaluate("Boolean(0)"));
            Assert.AreEqual(true, Evaluate("Boolean(1)"));
            Assert.AreEqual(true, Evaluate("Boolean(2)"));

            // toString and valueOf.
            Assert.AreEqual("function Boolean() { [native code] }", Evaluate("Boolean.toString()"));
            Assert.AreEqual(true, Evaluate("Boolean.valueOf() === Boolean"));

            // length
            Assert.AreEqual(1, Evaluate("Boolean.length"));
        }

        [TestMethod]
        public void Properties()
        {
            // Constructor and __proto__
            Assert.AreEqual(true, Evaluate("new Boolean().constructor === Boolean"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(new Boolean()) === Boolean.prototype"));

            // No initial enumerable properties.
            Assert.AreEqual("", Evaluate("y = ''; for (var x in Boolean) { y += x } y"));
            Assert.AreEqual("", Evaluate("y = ''; for (var x in new Boolean(5)) { y += x } y"));
            Assert.AreEqual("", Evaluate("y = ''; for (var x in false) { y += x } y"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("false", Evaluate("false.toString()"));
            Assert.AreEqual("true", Evaluate("true.toString()"));
            Assert.AreEqual("false", Evaluate("new Boolean(false).toString()"));
            Assert.AreEqual("true", Evaluate("new Boolean(true).toString()"));
            Assert.AreEqual(0, Evaluate("Boolean.prototype.toString.length"));
        }
    }
}
