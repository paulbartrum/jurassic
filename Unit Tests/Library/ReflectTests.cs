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
    /// Test the global Reflect object.
    /// </summary>
    [TestClass]
    public class ReflectTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Reflect is not a function.
            Assert.AreEqual("TypeError: The new operator requires a function, found a 'object' instead", EvaluateExceptionMessage("new Reflect"));
            Assert.AreEqual("TypeError: 'Reflect' is not a function", EvaluateExceptionMessage("Reflect()"));
        }

        // TODO: Reflect.apply, Reflect.construct, etc.

        [TestMethod]
        public void apply()
        {
            Assert.AreEqual("", Evaluate("Reflect.apply()"));
        }

        [TestMethod]
        public void isExtensible()
        {
            Assert.AreEqual(true, Evaluate("Reflect.isExtensible({})"));
            Assert.AreEqual(true, Evaluate("Reflect.isExtensible({a: 1})"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(Object.seal({a: 1}))"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(Object.freeze({a: 1}))"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(Object.preventExtensions({}))"));
            Assert.AreEqual(true, Evaluate("Reflect.isExtensible(new Boolean(true))"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(true)"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(5)"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible('test')"));

            // length
            Assert.AreEqual(1, Evaluate("Reflect.isExtensible.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError: undefined cannot be converted to an object", EvaluateExceptionMessage("Reflect.isExtensible()"));
            Assert.AreEqual("TypeError: undefined cannot be converted to an object", EvaluateExceptionMessage("Reflect.isExtensible(undefined)"));
            Assert.AreEqual("TypeError: null cannot be converted to an object", EvaluateExceptionMessage("Reflect.isExtensible(null)"));
        }

        [TestMethod]
        public void preventExtensions()
        {
            // Simple object
            Assert.AreEqual(true, Evaluate("var x = {a: 1, c: 2}; Reflect.preventExtensions(x)"));
            Assert.AreEqual(2, Evaluate("x.a = 2; x.a"));
            Assert.AreEqual(true, Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.b = 6; x.b"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(x)"));
            Assert.AreEqual(PropertyAttributes.FullAccess, EvaluateAccessibility("x", "c"));
            Assert.AreEqual(true, Evaluate("Reflect.preventExtensions(true)"));
            Assert.AreEqual(true, Evaluate("Reflect.preventExtensions(5)"));
            Assert.AreEqual(true, Evaluate("Reflect.preventExtensions('test')"));

            // length
            Assert.AreEqual(1, Evaluate("Reflect.preventExtensions.length"));

            // If the argument is not an object, this method throws a TypeError.
            Assert.AreEqual("TypeError: Reflect.preventExtensions called on non-object", EvaluateExceptionMessage("Reflect.preventExtensions()"));
            Assert.AreEqual("TypeError: Reflect.preventExtensions called on non-object", EvaluateExceptionMessage("Reflect.preventExtensions(undefined)"));
            Assert.AreEqual("TypeError: Reflect.preventExtensions called on non-object", EvaluateExceptionMessage("Reflect.preventExtensions(null)"));
            Assert.AreEqual("TypeError: Reflect.preventExtensions called on non-object", EvaluateExceptionMessage("Reflect.preventExtensions(5)"));
        }

        [TestMethod]
        public void set()
        {
            Assert.AreEqual(2, Evaluate("var x = { a: 1 }; Reflect.set(x, 'a', 2); x.a"));
        }

        [TestMethod]
        public void setPrototypeOf()
        {
            // Returns true on success.
            Assert.AreEqual(true, Evaluate("var a = {}; Reflect.setPrototypeOf(a, Math)"));
            Assert.AreEqual(true, Evaluate("var a = {}; Reflect.setPrototypeOf(a, null)"));

            // Check that the prototype was changed.
            Assert.AreEqual(true, Evaluate("var a = {}; Reflect.setPrototypeOf(a, Math); Reflect.getPrototypeOf(a) === Math && Object.getPrototypeOf(a) === Math"));
            Assert.AreEqual(true, Evaluate("var a = {}; Reflect.setPrototypeOf(a, null); Reflect.getPrototypeOf(a) === null && Object.getPrototypeOf(a) === null"));
            Assert.AreEqual(5, Evaluate("var a = {}; Reflect.setPrototypeOf(a, Math); a.abs(-5)"));

            // length
            Assert.AreEqual(2, Evaluate("Reflect.setPrototypeOf.length"));

            // Argument must be an object or null.
            Assert.AreEqual("TypeError: Object prototype may only be an Object or null", EvaluateExceptionMessage("Reflect.setPrototypeOf({}, undefined)"));

            // Object must be extensible.
            Assert.AreEqual(false, Evaluate("Reflect.setPrototypeOf(Object.preventExtensions({}), {})"));

            // No cyclic references.
            Assert.AreEqual(false, Evaluate("var a = {}; Reflect.setPrototypeOf(a, a)"));
        }
    }
}
