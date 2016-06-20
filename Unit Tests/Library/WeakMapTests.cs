using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global WeakMap object.
    /// </summary>
    [TestClass]
    public class WeakMapTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", EvaluateExceptionType("WeakMap()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("WeakMap(2)"));

            // Construct

            // new WeakMap();
            Assert.AreEqual("[object WeakMap]", Evaluate("new WeakMap().toString()"));

            // new WeakMap(iterable);
            Assert.AreEqual("[object WeakMap]", Evaluate("new WeakMap([[{ a: 1 }, 1]]).toString()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new WeakMap(5)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new WeakMap([1, 2])"));

            // WeakMap.prototype.constructor
            Assert.AreEqual(true, Evaluate("WeakMap.prototype.constructor === WeakMap"));

            // toString and valueOf.
            Assert.AreEqual("function WeakMap() { [native code] }", Evaluate("WeakMap.toString()"));
            Assert.AreEqual(true, Evaluate("WeakMap.valueOf() === WeakMap"));

            // length
            Assert.AreEqual(0, Evaluate("WeakMap.length"));
        }

        [TestMethod]
        public void delete()
        {
            Assert.AreEqual(true, Evaluate("var x = {}, y = {}; new WeakMap().set(x, y).delete(x)"));
            Assert.AreEqual(false, Evaluate("var x = {}, y = {}; new WeakMap().set(x, y).delete(y)"));
            Assert.AreEqual(true, Evaluate("var x = {}, y = {}; var m = new WeakMap([[x, 1], [y, 2]]); m.delete(x); m.has(y)"));
            Assert.AreEqual(false, Evaluate("var x = {}, y = {}; var m = new WeakMap([[x, 1], [y, 2]]); m.delete(x); m.has(x)"));

            // length
            Assert.AreEqual(1, Evaluate("WeakMap.prototype.delete.length"));
        }

        [TestMethod]
        public void get()
        {
            Assert.AreEqual(true, Evaluate("var x = {}, y = {}; new WeakMap().set(x, y).get(x) === y"));
            Assert.AreEqual(Undefined.Value, Evaluate("var x = {}, y = {}; new WeakMap().set(x, y).get(y)"));
            Assert.AreEqual(true, Evaluate("var x = {}, y = {}; new WeakMap([[x, y]]).get(x) === y"));
            Assert.AreEqual(Undefined.Value, Evaluate("var x = {}, y = {}; new WeakMap([[x, y]]).get(y)"));

            // Returns undefined for non-objects.
            Assert.AreEqual(Undefined.Value, Evaluate("var x = {}, y = {}; new WeakMap().set(x, y).get(5)"));

            // length
            Assert.AreEqual(1, Evaluate("WeakMap.prototype.get.length"));
        }

        [TestMethod]
        public void has()
        {
            Assert.AreEqual(true, Evaluate("var x = {}, y = {}; new WeakMap().set(x, y).has(x)"));
            Assert.AreEqual(false, Evaluate("var x = {}, y = {}; new WeakMap().set(x, y).has(y)"));

            // Returns false for non-objects.
            Assert.AreEqual(false, Evaluate("var x = {}, y = {}; new WeakMap().set(x, y).has(5)"));

            // length
            Assert.AreEqual(1, Evaluate("WeakMap.prototype.has.length"));
        }
        
        [TestMethod]
        public void set()
        {
            // Returns the set.
            Assert.AreEqual(true, Evaluate("var s = new WeakMap(); s.set({}, {}) === s"));
            Assert.AreEqual(true, Evaluate("var s = new WeakMap(); s.set({}, 1) === s"));

            // Attempting to use a key which is not an object fails.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new WeakMap().set(1, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new WeakMap().set('abc', {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new WeakMap().set(true, {})"));

            // length
            Assert.AreEqual(2, Evaluate("WeakMap.prototype.set.length"));
        }

        [TestMethod]
        public void Symbol_toStringTag()
        {
            Assert.AreEqual("WeakMap", Evaluate("new WeakMap()[Symbol.toStringTag]"));
        }
    }
}
