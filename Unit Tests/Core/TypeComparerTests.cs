using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the type comparison routines.
    /// </summary>
    [TestClass]
    public class TypeComparerTests
    {
        [TestMethod]
        public void SameValue()
        {
            // undefined
            Assert.AreEqual(true, TypeComparer.SameValue(Undefined.Value, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(Undefined.Value, Null.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(Undefined.Value, 0));
            Assert.AreEqual(true, TypeComparer.SameValue(null, null));
            Assert.AreEqual(true, TypeComparer.SameValue(null, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(null, Null.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(null, 0));

            // null
            Assert.AreEqual(true, TypeComparer.SameValue(Null.Value, Null.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(Null.Value, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(Null.Value, 0));

            // number
            Assert.AreEqual(true, TypeComparer.SameValue(+0.0, +0.0));
            Assert.AreEqual(true, TypeComparer.SameValue(-0.0, -0.0));
            Assert.AreEqual(false, TypeComparer.SameValue(+0.0, -0.0));
            Assert.AreEqual(false, TypeComparer.SameValue(-0.0, +0.0));
            Assert.AreEqual(true, TypeComparer.SameValue(1, 1));
            Assert.AreEqual(false, TypeComparer.SameValue(0, 1));
            Assert.AreEqual(true, TypeComparer.SameValue(5, 5.0));
            Assert.AreEqual(true, TypeComparer.SameValue(5.0, 5));
            Assert.AreEqual(true, TypeComparer.SameValue(5.0, 5.0));
            Assert.AreEqual(false, TypeComparer.SameValue(5.0, 6.0));
            Assert.AreEqual(true, TypeComparer.SameValue(double.NaN, double.NaN));
            Assert.AreEqual(false, TypeComparer.SameValue(double.NaN, 5));
            Assert.AreEqual(false, TypeComparer.SameValue(double.NaN, 5.0));
            Assert.AreEqual(false, TypeComparer.SameValue(0, "0"));

            // string
            Assert.AreEqual(true, TypeComparer.SameValue("", ""));
            Assert.AreEqual(true, TypeComparer.SameValue("a", "a"));
            Assert.AreEqual(false, TypeComparer.SameValue("a", "b"));
            Assert.AreEqual(false, TypeComparer.SameValue("0", 0));

            // bool
            Assert.AreEqual(true, TypeComparer.SameValue(false, false));
            Assert.AreEqual(true, TypeComparer.SameValue(true, true));
            Assert.AreEqual(false, TypeComparer.SameValue(true, false));
            Assert.AreEqual(false, TypeComparer.SameValue(false, 0));

            // object
            var engine = new ScriptEngine();
            var temp1 = engine.Object.Construct();
            var temp2 = engine.Object.Construct();
            var number1 = engine.Number.Construct(5.0);
            Assert.AreEqual(true, TypeComparer.SameValue(temp1, temp1));
            Assert.AreEqual(false, TypeComparer.SameValue(temp1, temp2));
            Assert.AreEqual(true, TypeComparer.SameValue(number1, number1));
            Assert.AreEqual(false, TypeComparer.SameValue(number1, 5.0));
        }

    }
}
