using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the global WeakSet object.
    /// </summary>
    [TestClass]
    public class WeakSetTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", EvaluateExceptionType("WeakSet()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("WeakSet(2)"));

            // Construct

            // new WeakSet();
            Assert.AreEqual("[object WeakSet]", Evaluate("new WeakSet().toString()"));

            // new WeakSet(iterable);
            Assert.AreEqual("[object WeakSet]", Evaluate("var x = {}; y = {}; new WeakSet([x, y, y, x]).toString()"));

            // WeakSet.prototype.constructor
            Assert.AreEqual(true, Evaluate("WeakSet.prototype.constructor === WeakSet"));

            // toString and valueOf.
            Assert.AreEqual("function WeakSet() { [native code] }", Evaluate("WeakSet.toString()"));
            Assert.AreEqual(true, Evaluate("WeakSet.valueOf() === WeakSet"));

            // length
            Assert.AreEqual(0, Evaluate("WeakSet.length"));
        }

        [TestMethod]
        public void add()
        {
            // Returns the set.
            Assert.AreEqual(true, Evaluate("var s = new WeakSet(); s.add({}) === s"));

            // Attempting to add a value which is not an object fails.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new WeakSet().add(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new WeakSet().add('abc')"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new WeakSet().add(true)"));

            // length
            Assert.AreEqual(1, Evaluate("WeakSet.prototype.add.length"));
        }

        [TestMethod]
        public void delete()
        {
            Assert.AreEqual(true, Evaluate("var x = {}; var y = {}; var s = new WeakSet(); s.add(x).delete(x)"));
            Assert.AreEqual(false, Evaluate("var x = {}; var y = {}; var s = new WeakSet(); s.add(x).delete(y)"));
            Assert.AreEqual(false, Evaluate("var x = {}; var y = {}; var s = new WeakSet(); s.add(x).delete(x); s.has(x)"));

            // Returns false for non-objects.
            Assert.AreEqual(false, Evaluate("var x = {}; var y = {}; var s = new WeakSet(); s.add(x).delete(1)"));

            // length
            Assert.AreEqual(1, Evaluate("WeakSet.prototype.delete.length"));
        }
        
        [TestMethod]
        public void has()
        {
            Assert.AreEqual(true, Evaluate("x = {}; y = {}; new WeakSet().add(x).has(x)"));
            Assert.AreEqual(false, Evaluate("x = {}; y = {}; new WeakSet().add(x).has(y)"));
            Assert.AreEqual(true, Evaluate("x = {}; y = {}; new WeakSet([x]).has(x)"));
            Assert.AreEqual(false, Evaluate("x = {}; y = {}; new WeakSet([x]).has(y)"));

            // Returns false for non-objects.
            Assert.AreEqual(false, Evaluate("new WeakSet().has(1)"));

            // length
            Assert.AreEqual(1, Evaluate("WeakSet.prototype.has.length"));
        }

        [TestMethod]
        public void Symbol_toStringTag()
        {
            Assert.AreEqual("WeakSet", Evaluate("new WeakSet()[Symbol.toStringTag]"));
        }
    }
}
