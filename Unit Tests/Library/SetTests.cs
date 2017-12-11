using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global Set object.
    /// </summary>
    [TestClass]
    public class SetTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", EvaluateExceptionType("Set()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Set(2)"));

            // Construct

            // new Set();
            Assert.AreEqual(0, Evaluate("new Set().size"));

            Assert.AreEqual(Undefined.Value, Evaluate("new Set([,1])[0]"));

            // new Set(iterable);
            Assert.AreEqual(4, Evaluate("new Set([2, 1, 1, 3, 5, 3, 2]).size"));
            Assert.AreEqual("2,1,3,5", Evaluate("Int8Array.from(new Set([2, 1, 1, 3, 5, 3, 2])).toString()"));

            // Set.prototype.constructor
            Assert.AreEqual(true, Evaluate("Set.prototype.constructor === Set"));

            // toString and valueOf.
            Assert.AreEqual("function Set() { [native code] }", Evaluate("Set.toString()"));
            Assert.AreEqual(true, Evaluate("Set.valueOf() === Set"));

            // species
            Assert.AreEqual(true, Evaluate("Set[Symbol.species] === Set"));

            // length
            Assert.AreEqual(0, Evaluate("Set.length"));
        }

        [TestMethod]
        public void add()
        {
            // Returns the set.
            Assert.AreEqual(true, Evaluate("var s = new Set(); s.add(1) === s"));

            // Values are unique.
            Assert.AreEqual(3, Evaluate("new Set().add(1).add(2).add(2).add(3).size"));
            Assert.AreEqual("1,2,3", Evaluate("Int8Array.from(new Set().add(1).add(2).add(2).add(3)).toString()"));

            // length
            Assert.AreEqual(1, Evaluate("Set.prototype.add.length"));
        }

        [TestMethod]
        public void clear()
        {
            Assert.AreEqual(0, Evaluate("var s = new Set(); s.add(1).clear(); s.size"));
            Assert.AreEqual("", Evaluate("var s = new Set(); s.add(1).clear(); Int8Array.from(s).toString()"));

            // length
            Assert.AreEqual(0, Evaluate("Set.prototype.clear.length"));
        }

        [TestMethod]
        public void delete()
        {
            Assert.AreEqual(true, Evaluate("var s = new Set(); s.add(1).delete(1)"));
            Assert.AreEqual(false, Evaluate("var s = new Set(); s.add(1).delete(2)"));
            Assert.AreEqual(false, Evaluate("var s = new Set([3, 1, 4, 1, 5]); s.delete(1); s.has(1)"));

            // length
            Assert.AreEqual(1, Evaluate("Set.prototype.delete.length"));
        }

        [TestMethod]
        public void entries()
        {
            Execute("var i = new Set([11, 7]).entries()");

            // Item #1
            Execute("var result = i.next();");
            Assert.AreEqual("11,11", Evaluate("result.value.toString()"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // Item #2
            Execute("var result = i.next();");
            Assert.AreEqual("7,7", Evaluate("result.value.toString()"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // No more items.
            Execute("var result = i.next();");
            Assert.AreEqual(Undefined.Value, Evaluate("result.value"));
            Assert.AreEqual(true, Evaluate("result.done"));

            // toString
            Assert.AreEqual("[object Set Iterator]", Evaluate("new Set([11, 7]).entries().toString()"));

            // length
            Assert.AreEqual(0, Evaluate("Set.prototype.entries.length"));
        }

        [TestMethod]
        public void forEach()
        {
            Assert.AreEqual("42", Evaluate("var result = []; new Set().add(42).forEach(function (e1, e2, S) { result.push(e1) }); result.toString()"));
            Assert.AreEqual("42", Evaluate("var result = []; new Set().add(42).forEach(function (e1, e2, S) { result.push(e2) }); result.toString()"));
            Assert.AreEqual("[object Set]", Evaluate("var result = []; new Set().add(42).forEach(function (e1, e2, S) { result.push(S.toString()) }); result.toString()"));
            Assert.AreEqual("99", Evaluate("var result = []; new Set().add(42).forEach(function (e1, e2, S) { result.push(this) }, 99); result.toString()"));
            Assert.AreEqual("42,9,13,7,32", Evaluate("var result = []; new Set().add(42).add(9).add(13).add(7).add(32).forEach(function (e1, e2, S) { result.push(e1) }); result.toString()"));

            // Items can be added or deleted in the callback.
            Assert.AreEqual("1,2,3", Evaluate("var result = []; new Set([1, 2, 3]).forEach(function (e1, e2, S) { if (e1 === 1) { S.delete(1); } result.push(e1); }); result.toString()"));
            Assert.AreEqual("1,3", Evaluate("var result = []; new Set([1, 2, 3]).forEach(function (e1, e2, S) { if (e1 === 1) { S.delete(2); } result.push(e1); }); result.toString()"));
            Assert.AreEqual("1,2,3,4", Evaluate("var result = []; new Set([1, 2, 3]).forEach(function (e1, e2, S) { if (e1 === 1) { S.add(4); } result.push(e1) }); result.toString()"));
            Assert.AreEqual("1,2,3,4", Evaluate("var result = []; new Set([1, 2, 3]).forEach(function (e1, e2, S) { if (e1 === 3) { S.add(4); } result.push(e1) }); result.toString()"));

            // -0 is converted to +0.
            Assert.AreEqual(double.PositiveInfinity, Evaluate(@"var k; new Set([-0]).forEach(function(value) { k = 1 / value; }); k"));

            // length
            Assert.AreEqual(1, Evaluate("Set.prototype.forEach.length"));
        }

        [TestMethod]
        public void has()
        {
            Assert.AreEqual(true, Evaluate("new Set().add(1).has(1)"));
            Assert.AreEqual(false, Evaluate("new Set().add(1).has(2)"));
            Assert.AreEqual(true, Evaluate("var x = {}; new Set().add(x).has(x)"));
            Assert.AreEqual(false, Evaluate("var x = {}; new Set().add(x).has({})"));
            Assert.AreEqual(true, Evaluate("new Set().add('episodic').has('episodic')"));
            Assert.AreEqual(false, Evaluate("new Set().add('episodic').has('dozens')"));
            Assert.AreEqual(false, Evaluate("new Set().add('').has(0)"));
            Assert.AreEqual(true, Evaluate("new Set().add(0).has(0)"));
            Assert.AreEqual(true, Evaluate("new Set().add(0).has(-0)"));
            Assert.AreEqual(true, Evaluate("new Set().add(-0).has(0)"));
            Assert.AreEqual(true, Evaluate("new Set().add(-0).has(-0)"));

            // length
            Assert.AreEqual(1, Evaluate("Set.prototype.has.length"));
        }

        [TestMethod]
        public void keys()
        {
            Execute("var i = new Set([11, 7]).keys()");

            // Item #1
            Execute("var result = i.next();");
            Assert.AreEqual(11, Evaluate("result.value"));
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
            Assert.AreEqual("[object Set Iterator]", Evaluate("new Set([1, 2, 3]).keys().toString()"));

            // length
            Assert.AreEqual(0, Evaluate("Set.prototype.keys.length"));
        }

        [TestMethod]
        public void values()
        {
            Execute("var i = new Set([11, 7]).values()");

            // Item #1
            Execute("var result = i.next();");
            Assert.AreEqual(11, Evaluate("result.value"));
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
            Assert.AreEqual("[object Set Iterator]", Evaluate("new Set([1, 2, 3]).values().toString()"));

            // length
            Assert.AreEqual(0, Evaluate("Set.prototype.values.length"));
        }

        [TestMethod]
        public void Symbol_iterator()
        {
            // The Symbol.iterator value is just equal to the values function.
            Assert.AreEqual(true, Evaluate("new Set()[Symbol.iterator] === new Set().values"));
        }

        [TestMethod]
        public void Symbol_toStringTag()
        {
            Assert.AreEqual("Set", Evaluate("new Set()[Symbol.toStringTag]"));
        }
    }
}
