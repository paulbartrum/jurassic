﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            // length
            Assert.AreEqual(3, Evaluate("Reflect.apply.length"));
        }

        [TestMethod]
        public void construct()
        {
            Assert.AreEqual(5, Evaluate("Reflect.construct(Number, [5]).valueOf()"));
            Assert.AreEqual(5, Evaluate("Reflect.construct(Number, [5], Number).valueOf()"));
            Assert.AreEqual(true, Evaluate("Reflect.construct(function () { return new.target; }, [], Number) === Number"));
            
            // The first parameter must be a constructor.
            Assert.AreEqual("TypeError: undefined cannot be converted to an object", EvaluateExceptionMessage("Reflect.construct(Math)"));
            
            // The second parameter must be an object.
            Assert.AreEqual("TypeError: undefined cannot be converted to an object", EvaluateExceptionMessage("Reflect.construct(Number)"));
            
            // The third parameter must be undefined or a constructor.
            Assert.AreEqual("TypeError: Incorrect argument type.", EvaluateExceptionMessage("Reflect.construct(Number, [5], Math)"));
            
            // length
            Assert.AreEqual(2, Evaluate("Reflect.construct.length"));
        }

        [TestMethod]
        public void defineProperty()
        {
            Assert.AreEqual(5, Evaluate("var x = {}; Reflect.defineProperty(x, 'test', { value: 5 }); x.test"));

            // The descriptor must be an object.
            Assert.AreEqual("TypeError: Invalid property descriptor '5'.", EvaluateExceptionMessage("Reflect.defineProperty({}, 'test', 5)"));

            // length
            Assert.AreEqual(3, Evaluate("Reflect.defineProperty.length"));
        }

        [TestMethod]
        public void deleteProperty()
        {
            Assert.AreEqual(true, Evaluate("var x = { a: 1 }; Reflect.deleteProperty(x, 'a')"));
            Assert.AreEqual(false, Evaluate("var x = { a: 1 }; Reflect.deleteProperty(x, 'a'); 'a' in x"));

            // length
            Assert.AreEqual(2, Evaluate("Reflect.deleteProperty.length"));
        }

        [TestMethod]
        public void get()
        {
            Assert.AreEqual(10, Evaluate("Reflect.get({ a: 10 }, 'a')"));

            // length
            Assert.AreEqual(2, Evaluate("Reflect.get.length"));
        }

        [TestMethod]
        public void getOwnPropertyDescriptor()
        {
            // length
            Assert.AreEqual(2, Evaluate("Reflect.getOwnPropertyDescriptor.length"));
        }

        [TestMethod]
        public void getPrototypeOf()
        {
            // length
            Assert.AreEqual(1, Evaluate("Reflect.getPrototypeOf.length"));
        }

        [TestMethod]
        public void has()
        {
            // length
            Assert.AreEqual(2, Evaluate("Reflect.has.length"));
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
        public void ownKeys()
        {
            Assert.AreEqual("z,y,x", Evaluate("Reflect.ownKeys({z: 3, y: 2, x: 1}).toString()"));
            Assert.AreEqual("length", Evaluate("Reflect.ownKeys([]).toString()"));
        
            Execute(@"
                var sym = Symbol('comet')
                var sym2 = Symbol('meteor')
                var obj = {[sym]: 0, 'str': 0, '773': 0, '0': 0,
                           [sym2]: 0, '-1': 0, '8': 0, 'second str': 0}");
            Assert.AreEqual(8, Evaluate("Reflect.ownKeys(obj).length"));
            Assert.AreEqual("0", Evaluate("Reflect.ownKeys(obj)[0]"));
            Assert.AreEqual("8", Evaluate("Reflect.ownKeys(obj)[1]"));
            Assert.AreEqual("773", Evaluate("Reflect.ownKeys(obj)[2]"));
            Assert.AreEqual("str", Evaluate("Reflect.ownKeys(obj)[3]"));
            Assert.AreEqual("-1", Evaluate("Reflect.ownKeys(obj)[4]"));
            Assert.AreEqual("second str", Evaluate("Reflect.ownKeys(obj)[5]"));
            Assert.AreEqual(true, Evaluate("Reflect.ownKeys(obj)[6] === sym"));
            Assert.AreEqual(true, Evaluate("Reflect.ownKeys(obj)[7] === sym2"));

            // length
            Assert.AreEqual(1, Evaluate("Reflect.ownKeys.length"));
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

            // length
            Assert.AreEqual(3, Evaluate("Reflect.set.length"));
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
            Assert.AreEqual("TypeError: Object prototype may only be an Object or null.", EvaluateExceptionMessage("Reflect.setPrototypeOf({}, undefined)"));

            // Object must be extensible.
            Assert.AreEqual(false, Evaluate("Reflect.setPrototypeOf(Object.preventExtensions({}), {})"));

            // No cyclic references.
            Assert.AreEqual(false, Evaluate("var a = {}; Reflect.setPrototypeOf(a, a)"));
        }
    }
}
