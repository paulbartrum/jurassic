using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global Map object.
    /// </summary>
    [TestClass]
    public class MapTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", EvaluateExceptionType("Map()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Map(2)"));

            // Construct

            // new Map();
            Assert.AreEqual(0, Evaluate("new Map().size"));

            // new Map(iterable);
            Assert.AreEqual(3, Evaluate("new Map([[2, 1], [1, 3], [5, 3]]).size"));
            Assert.AreEqual(3, Evaluate("new Map([[2, 1], [1, 3], [5, 3]]).get(1)"));
            Assert.AreEqual(1, Evaluate("new Map([[2, 1], [1, 3], [5, 3]]).get(2)"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Map([[2, 1], [1, 3], [5, 3]]).get(3)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Map(5)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Map([1, 2])"));

            // Map.prototype.constructor
            Assert.AreEqual(true, Evaluate("Map.prototype.constructor === Map"));

            // toString and valueOf.
            Assert.AreEqual("function Map() { [native code] }", Evaluate("Map.toString()"));
            Assert.AreEqual(true, Evaluate("Map.valueOf() === Map"));

            // species
            Assert.AreEqual(true, Evaluate("Map[Symbol.species] === Map"));

            // length
            Assert.AreEqual(0, Evaluate("Map.length"));
        }

        [TestMethod]
        public void clear()
        {
            Assert.AreEqual(0, Evaluate("var s = new Map(); s.set(1, 2).clear(); s.size"));

            // length
            Assert.AreEqual(0, Evaluate("Map.prototype.clear.length"));
        }

        [TestMethod]
        public void delete()
        {
            Assert.AreEqual(true, Evaluate("var s = new Map(); s.set(1, 2).delete(1)"));
            Assert.AreEqual(false, Evaluate("var s = new Map(); s.set(1, 2).delete(2)"));
            Assert.AreEqual(false, Evaluate("var s = new Map([[3, 1], [4, 1]]); s.delete(4); s.has(4)"));

            // length
            Assert.AreEqual(1, Evaluate("Map.prototype.delete.length"));
        }

        [TestMethod]
        public void entries()
        {
            Execute("var i = new Map([[13, 1], [11, 7]]).entries()");

            // Item #1
            Execute("var result = i.next();");
            Assert.AreEqual("13,1", Evaluate("result.value.toString()"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // Item #2
            Execute("var result = i.next();");
            Assert.AreEqual("11,7", Evaluate("result.value.toString()"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // No more items.
            Execute("var result = i.next();");
            Assert.AreEqual(Undefined.Value, Evaluate("result.value"));
            Assert.AreEqual(true, Evaluate("result.done"));

            // toString
            Assert.AreEqual("[object Map Iterator]", Evaluate("new Map().entries().toString()"));

            // length
            Assert.AreEqual(0, Evaluate("Map.prototype.entries.length"));
        }

        [TestMethod]
        public void forEach()
        {
            Assert.AreEqual("9", Evaluate("var result = []; new Map().set(9, 3).forEach(function (e1, e2, S) { result.push(e1) }); result.toString()"));
            Assert.AreEqual("3", Evaluate("var result = []; new Map().set(9, 3).forEach(function (e1, e2, S) { result.push(e2) }); result.toString()"));
            Assert.AreEqual("[object Map]", Evaluate("var result = []; new Map().set(42, 5).forEach(function (e1, e2, S) { result.push(S.toString()) }); result.toString()"));
            Assert.AreEqual("99", Evaluate("var result = []; new Map().set(42, 14).forEach(function (e1, e2, S) { result.push(this) }, 99); result.toString()"));
            Assert.AreEqual("42,2,9,5,13,3", Evaluate("var result = []; new Map().set(42, 2).set(9, 5).set(13, 3).forEach(function (e1, e2, S) { result.push(e1); result.push(e2) }); result.toString()"));

            // Items can be added or deleted in the callback.
            Assert.AreEqual("1,2,3", Evaluate("var result = []; new Map([[1, 0], [2, 0], [3, 0]]).forEach(function (e1, e2, S) { if (e1 === 1) { S.delete(1); } result.push(e1); }); result.toString()"));
            Assert.AreEqual("1,3", Evaluate("var result = []; new Map([[1, 0], [2, 0], [3, 0]]).forEach(function (e1, e2, S) { if (e1 === 1) { S.delete(2); } result.push(e1); }); result.toString()"));
            Assert.AreEqual("1,2,3,4", Evaluate("var result = []; new Map([[1, 0], [2, 0], [3, 0]]).forEach(function (e1, e2, S) { if (e1 === 1) { S.set(4, 0); } result.push(e1) }); result.toString()"));
            Assert.AreEqual("1,2,3,4", Evaluate("var result = []; new Map([[1, 0], [2, 0], [3, 0]]).forEach(function (e1, e2, S) { if (e1 === 3) { S.set(4, 0); } result.push(e1) }); result.toString()"));

            // -0 is converted to +0.
            Assert.AreEqual(double.PositiveInfinity, Evaluate(@"var k; new Map([[-0, 0]]).forEach(function(value) { k = 1 / value; }); k"));

            // length
            Assert.AreEqual(1, Evaluate("Map.prototype.forEach.length"));
        }

        [TestMethod]
        public void get()
        {
            Assert.AreEqual(2, Evaluate("new Map().set(1, 2).get(1)"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Map().set(1, 2).get(2)"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Map().set(0, 2).get('')"));
            Assert.AreEqual(2, Evaluate("new Map([[1, 2]]).get(1)"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Map([[1, 2]]).get(2)"));
            Assert.AreEqual(3.1, Evaluate("new Map([['a', 'b'], [1, 2], [false, true], [5.5, 3.1]]).get(5.5)"));

            // length
            Assert.AreEqual(1, Evaluate("Map.prototype.get.length"));
        }

        [TestMethod]
        public void has()
        {
            Assert.AreEqual(true, Evaluate("new Map().set(1, 2).has(1)"));
            Assert.AreEqual(false, Evaluate("new Map().set(1, 2).has(2)"));
            Assert.AreEqual(true, Evaluate("var x = {}; new Map().set(x, x).has(x)"));
            Assert.AreEqual(false, Evaluate("var x = {}; new Map().set(x, x).has({})"));
            Assert.AreEqual(true, Evaluate("new Map().set('episodic', 0).has('episodic')"));
            Assert.AreEqual(false, Evaluate("new Map().set('episodic', 0).has('dozens')"));
            Assert.AreEqual(false, Evaluate("new Map().set('', 0).has(0)"));
            Assert.AreEqual(true, Evaluate("new Map().set(0, 'abc').has(0)"));
            Assert.AreEqual(true, Evaluate("new Map().set(0, 'abc').has(-0)"));
            Assert.AreEqual(true, Evaluate("new Map().set(-0, 'abc').has(0)"));
            Assert.AreEqual(true, Evaluate("new Map().set(-0, 'abc').has(-0)"));

            // length
            Assert.AreEqual(1, Evaluate("Map.prototype.has.length"));
        }

        [TestMethod]
        public void keys()
        {
            Execute("var i = new Map([[13, 1], [11, 7]]).keys()");

            // Item #1
            Execute("var result = i.next();");
            Assert.AreEqual(13, Evaluate("result.value"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // Item #2
            Execute("var result = i.next();");
            Assert.AreEqual(11, Evaluate("result.value"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // No more items.
            Execute("var result = i.next();");
            Assert.AreEqual(Undefined.Value, Evaluate("result.value"));
            Assert.AreEqual(true, Evaluate("result.done"));

            // toString
            Assert.AreEqual("[object Map Iterator]", Evaluate("new Map().keys().toString()"));

            // length
            Assert.AreEqual(0, Evaluate("Map.prototype.keys.length"));
        }

        [TestMethod]
        public void set()
        {
            // Returns the set.
            Assert.AreEqual(true, Evaluate("var s = new Map(); s.set(1, 2) === s"));

            // Values are unique.
            Assert.AreEqual(3, Evaluate("new Map().set(1, 2).set(2, 2).set(2, 2).set(3, 2).size"));

            // length
            Assert.AreEqual(2, Evaluate("Map.prototype.set.length"));
        }

        [TestMethod]
        public void values()
        {
            Execute("var i = new Map([[13, 1], [11, 7]]).values()");

            // Item #1
            Execute("var result = i.next();");
            Assert.AreEqual(1, Evaluate("result.value"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // Item #2
            Execute("var result = i.next();");
            Assert.AreEqual(7, Evaluate("result.value"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // No more items.
            Execute("var result = i.next();");
            Assert.AreEqual(Undefined.Value, Evaluate("result.value"));
            Assert.AreEqual(true, Evaluate("result.done"));

            // toString
            Assert.AreEqual("[object Map Iterator]", Evaluate("new Map().values().toString()"));

            // length
            Assert.AreEqual(0, Evaluate("Map.prototype.values.length"));
        }

        [TestMethod]
        public void Symbol_iterator()
        {
            // The Symbol.iterator value is just equal to the entries function.
            Assert.AreEqual(true, Evaluate("new Map()[Symbol.iterator] === new Map().entries"));
        }

        [TestMethod]
        public void Symbol_toStringTag()
        {
            Assert.AreEqual("Map", Evaluate("new Map()[Symbol.toStringTag]"));
        }
    }
}
