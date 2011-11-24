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
    /// Test the global Object object.
    /// </summary>
    [TestClass]
    public class ObjectTests
    {
        [TestMethod]
        public void Constructor()
        {
            // Construct
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("new Object().toString()"));
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("new Object(undefined).toString()"));
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("new Object(null).toString()"));
            Assert.AreEqual("hi", TestUtils.Evaluate("new Object('hi').valueOf()"));
            Assert.AreEqual(5501, TestUtils.Evaluate("new Object(5501).valueOf()"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = new String('test'); new Object(x) === x"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = new Number(55); new Object(x) === x"));

            // Call
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("Object().toString()"));
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("Object(undefined).toString()"));
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("Object(null).toString()"));
            Assert.AreEqual("hi", TestUtils.Evaluate("Object('hi').valueOf()"));
            Assert.AreEqual(5501, TestUtils.Evaluate("Object(5501).valueOf()"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = new String('test'); Object(x) === x"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = new Number(55); Object(x) === x"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.length"));

            // toString
            Assert.AreEqual("function Object() { [native code] }", TestUtils.Evaluate("Object.toString()"));

            // valueOf
            Assert.AreEqual(true, TestUtils.Evaluate("Object.valueOf() === Object"));

            // prototype
            Assert.AreEqual(true, TestUtils.Evaluate("Object.prototype === Object.getPrototypeOf(new Object())"));
            Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("Object", "prototype"));

            // constructor
            Assert.AreEqual(true, TestUtils.Evaluate("Object.prototype.constructor === Object"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("Object.prototype", "constructor"));

            // Functions are writable and configurable.
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("Object", "keys"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("Object.prototype", "hasOwnProperty"));
        }



        //     CONSTRUCTOR METHODS
        //_________________________________________________________________________________________

        [TestMethod]
        public void getPrototypeOf()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new Number(55)) === Number.prototype"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new String('test')) === String.prototype"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.getPrototypeOf.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getPrototypeOf()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getPrototypeOf(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getPrototypeOf(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getPrototypeOf('test')"));
        }

        [TestMethod]
        public void getOwnPropertyDescriptor()
        {
            // Non-existant properties return "undefined".
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(Number, 'bunnyEars')"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(Number, 'toString')"));

            // Otherwise, returns an object containing various properties.
            TestUtils.Evaluate("var descriptor = Object.getOwnPropertyDescriptor(Number, 'MAX_VALUE')");
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.configurable"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.enumerable"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.writable"));
            Assert.AreEqual(1.7976931348623157e+308, TestUtils.Evaluate("descriptor.value"));

            TestUtils.Evaluate("var descriptor = Object.getOwnPropertyDescriptor({p: 15}, 'p')");
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.configurable"));
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.enumerable"));
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.writable"));
            Assert.AreEqual(15, TestUtils.Evaluate("descriptor.value"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("descriptor.get"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("descriptor.set"));

            // Check the right properties are defined.
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.hasOwnProperty('value')"));
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.hasOwnProperty('writable')"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.hasOwnProperty('get')"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.hasOwnProperty('set')"));

            // Add accessor property.
            TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, enumerable: true, configurable: true}); descriptor = Object.getOwnPropertyDescriptor(x, 'a')");
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("descriptor.value"));
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.enumerable"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("descriptor.writable"));
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.configurable"));
            Assert.AreEqual("function", TestUtils.Evaluate("typeof(descriptor.get)"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("descriptor.set"));

            // Check the right properties are defined.
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.hasOwnProperty('value')"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.hasOwnProperty('writable')"));
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.hasOwnProperty('get')"));
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.hasOwnProperty('set')"));

            TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {}); descriptor = Object.getOwnPropertyDescriptor(x, 'a')");
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("descriptor.value"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.enumerable"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.writable"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.configurable"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("descriptor.get"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("descriptor.set"));

            // Check the right properties are defined.
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.hasOwnProperty('value')"));
            Assert.AreEqual(true, TestUtils.Evaluate("descriptor.hasOwnProperty('writable')"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.hasOwnProperty('get')"));
            Assert.AreEqual(false, TestUtils.Evaluate("descriptor.hasOwnProperty('set')"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("Object.getOwnPropertyDescriptor.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getOwnPropertyDescriptor()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getOwnPropertyDescriptor(true, 'toString')"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getOwnPropertyDescriptor(5, 'toString')"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getOwnPropertyDescriptor('test', 'toString')"));
        }

        [TestMethod]
        public void getOwnPropertyNames()
        {
            Assert.AreEqual("", TestUtils.Evaluate("Object.getOwnPropertyNames({}).toString()"));
            Assert.AreEqual("a", TestUtils.Evaluate("Object.getOwnPropertyNames({a: 'hello'}).toString()"));
            Assert.AreEqual("0,1,length", TestUtils.Evaluate("Object.getOwnPropertyNames([15, 16]).toString()"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.getOwnPropertyNames.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getOwnPropertyNames()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getOwnPropertyNames(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getOwnPropertyNames(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.getOwnPropertyNames('test')"));
        }

        [TestMethod]
        public void create()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.create(null)) === null"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.create(String.prototype)) === String.prototype"));

            // Initialize object properties
            TestUtils.Evaluate("var x = Object.create(null, {a: {value: 5}, b: {value: 10}})");
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(x) === null"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(10, TestUtils.Evaluate("x.b"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("Object.create.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.create()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.create(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.create(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.create(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.create('test')"));
        }

        [TestMethod]
        public void defineProperty()
        {
            // Add simple property.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: true, configurable: true}) === x"));
            Assert.AreEqual(7, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(PropertyAttributes.FullAccess, TestUtils.EvaluateAccessibility("x", "a"));
            Assert.AreEqual("a", TestUtils.Evaluate("var p = ''; for (var y in x) { p += y } p"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a"));

            // Add non-writable property.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}) === x"));
            Assert.AreEqual(7, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(PropertyAttributes.Enumerable | PropertyAttributes.Configurable, TestUtils.EvaluateAccessibility("x", "a"));
            Assert.AreEqual("a", TestUtils.Evaluate("var p = ''; for (var y in x) { p += y } p"));
            Assert.AreEqual(7, TestUtils.Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a"));

            // Non-writable properties can have the value changed (as long as configurable = true).
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}) === x"));
            Assert.AreEqual(8, TestUtils.Evaluate("Object.defineProperty(x, 'a', {value: 8, enumerable: true, writable: false, configurable: true}); x.a"));
            Assert.AreEqual(9, TestUtils.Evaluate("Object.defineProperty(x, 'a', {value: 9, enumerable: true, writable: false, configurable: false}); x.a"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 10, enumerable: true, writable: false, configurable: false}); x.a"));

            // Non-writable properties can be switched to accessor properties (as long as configurable = true).
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}) === x"));
            Assert.AreEqual(1, TestUtils.Evaluate("Object.defineProperty(x, 'a', {get: function() { return 1 }, enumerable: true, configurable: true}); x.a"));

            // Non-writable properties can go back to writable.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}) === x"));
            Assert.AreEqual(8, TestUtils.Evaluate("Object.defineProperty(x, 'a', {value: 8, enumerable: true, writable: true, configurable: true}); x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));

            // Add non-enumerable property.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: false, writable: true, configurable: true}) === x"));
            Assert.AreEqual(7, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("x", "a"));
            Assert.AreEqual("", TestUtils.Evaluate("var p = ''; for (var y in x) { p += y } p"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a"));

            // Add non-configurable property.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: true, configurable: false}) === x"));
            Assert.AreEqual(7, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(PropertyAttributes.Enumerable | PropertyAttributes.Writable, TestUtils.EvaluateAccessibility("x", "a"));
            Assert.AreEqual("a", TestUtils.Evaluate("var p = ''; for (var y in x) { p += y } p"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a"));

            // Add read-only accessor property.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, enumerable: true, configurable: true}) === x"));
            Assert.AreEqual(7, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual("function", TestUtils.Evaluate("typeof(Object.getOwnPropertyDescriptor(x, 'a').get)"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').set"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));
            Assert.AreEqual(7, TestUtils.Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a"));

            // Add read-write accessor property.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {a2: 10}; Object.defineProperty(x, 'a', {get: function() { return this.a2 }, set: function(value) { this.a2 = value }, enumerable: true, configurable: true}) === x"));
            Assert.AreEqual(10, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual("function", TestUtils.Evaluate("typeof(Object.getOwnPropertyDescriptor(x, 'a').get)"));
            Assert.AreEqual("function", TestUtils.Evaluate("typeof(Object.getOwnPropertyDescriptor(x, 'a').set)"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a"));

            // Add write-only accessor property.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {set: function(value) { this.a2 = value }, enumerable: true, configurable: true}) === x"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').get"));
            Assert.AreEqual("function", TestUtils.Evaluate("typeof(Object.getOwnPropertyDescriptor(x, 'a').set)"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a = 5; x.a2"));
            Assert.AreEqual(true, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a"));

            // An empty property descriptor defines a default property (if the property doesn't already exist).
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {}) === x"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.hasOwnProperty('a')"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').get"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').set"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));

            // An empty property descriptor does nothing if the property already exists.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {a: 5}; Object.defineProperty(x, 'a', {}) === x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.hasOwnProperty('a')"));
            Assert.AreEqual(5, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').get"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').set"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));

            // A property descriptor without a value does not change the existing value.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {a: 5}; Object.defineProperty(x, 'a', {writable: false}) === x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.hasOwnProperty('a')"));
            Assert.AreEqual(5, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').get"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').set"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));

            // get and set can be undefined.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 1; }, set: undefined, configurable: true}) === x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.defineProperty(x, 'a', {get: undefined, set: undefined}) === x"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.a"));

            // Non-extensible objects cannot have properties added.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.preventExtensions(x) === x"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: true, configurable: true})"));

            // Non-configurable properties can only have the value changed.
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: true, configurable: false}) === x"));
            Assert.AreEqual(10, TestUtils.Evaluate("Object.defineProperty(x, 'a', {value: 10, enumerable: true, writable: true, configurable: false}); x.a"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(x, 'a', {get: function() { return 7; }, enumerable: true, writable: true, configurable: false}); x.a"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 10, enumerable: false, writable: true, configurable: false})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 10, enumerable: true, writable: false, configurable: false})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 10, enumerable: true, writable: true, configurable: true})"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; var f = function() { return 1; }; Object.defineProperty(x, 'a', {get: f}); Object.defineProperty(x, 'a', {get: f}) === x"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 1; }}); Object.defineProperty(x, 'a', {set: undefined}) === x"));

            // Value is not allowed when specifying an accessor property.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, enumerable: true, configurable: true, value: 5})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, set: function() { }, enumerable: true, configurable: true, value: 5})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {set: function() { }, enumerable: true, configurable: true, value: 5})"));

            // Writable is not allowed when specifying an accessor property.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, enumerable: true, writable: false, configurable: true})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, set: function() { }, enumerable: true, writable: true, configurable: true})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {set: function() { }, enumerable: true, writable: true, configurable: true})"));

            // Get and set must be a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: 5, enumerable: true, configurable: true})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: 5, set: 5, enumerable: true, configurable: true})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {set: 5, enumerable: true, configurable: true})"));

            // length
            Assert.AreEqual(3, TestUtils.Evaluate("Object.defineProperty.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty({})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(undefined, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(null, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(true, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty(5, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty('test', {})"));

            // Property descriptors must be objects.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty({}, 'a', 5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty({}, 'a', undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperty({}, 'a', null)"));
        }

        [TestMethod]
        public void defineProperties()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {}; Object.defineProperties(x, {a: {value: 5}, b: {value: 10}}) === x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x.a"));
            Assert.AreEqual(10, TestUtils.Evaluate("x.b"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("Object.defineProperties.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties({})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties(undefined, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties(null, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties(true, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties(5, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties('test', {})"));

            // Property descriptors must be objects.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties({}, { a: 1 })"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties({}, { a: undefined })"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.defineProperties({}, { a: null })"));
        }

        [TestMethod]
        public void seal()
        {
            // Simple object
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {a: 1}; Object.seal(x) === x"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.a = 2; x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.b = 6; x.b"));
            Assert.AreEqual(PropertyAttributes.Enumerable | PropertyAttributes.Writable, TestUtils.EvaluateAccessibility("x", "a"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.seal.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.seal()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.seal(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.seal(null)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.seal(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.seal(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.seal('test')"));
        }

        [TestMethod]
        public void freeze()
        {
            // Simple object
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {a: 1}; Object.freeze(x) === x"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.a = 2; x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.b = 6; x.b"));
            Assert.AreEqual(PropertyAttributes.Enumerable, TestUtils.EvaluateAccessibility("x", "a"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.freeze.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.freeze()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.freeze(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.freeze(null)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.freeze(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.freeze(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.freeze('test')"));
        }

        [TestMethod]
        public void preventExtensions()
        {
            // Simple object
            Assert.AreEqual(true, TestUtils.Evaluate("var x = {a: 1, c: 2}; Object.preventExtensions(x) === x"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.a = 2; x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x.b = 6; x.b"));
            Assert.AreEqual(PropertyAttributes.FullAccess, TestUtils.EvaluateAccessibility("x", "c"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.preventExtensions.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.preventExtensions()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.preventExtensions(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.preventExtensions(null)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.preventExtensions(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.preventExtensions(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.preventExtensions('test')"));
        }

        [TestMethod]
        public void isSealed()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isSealed({})"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isSealed({a: 1})"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.isSealed(Object.seal({a: 1}))"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.isSealed(Object.freeze({a: 1}))"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.isSealed(Object.preventExtensions({}))"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isSealed(Object.preventExtensions({a: 1}))"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.isSealed.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isSealed()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isSealed(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isSealed(null)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isSealed(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isSealed(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isSealed('test')"));
        }

        [TestMethod]
        public void isFrozen()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isFrozen({})"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isFrozen({a: 1})"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isFrozen(Object.seal({a: 1}))"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.isFrozen(Object.freeze({a: 1}))"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.isFrozen(Object.preventExtensions({}))"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.isFrozen.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isFrozen()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isFrozen(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isFrozen(null)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isFrozen(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isFrozen(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isFrozen('test')"));
        }

        [TestMethod]
        public void isExtensible()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("Object.isExtensible({})"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.isExtensible({a: 1})"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isExtensible(Object.seal({a: 1}))"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isExtensible(Object.freeze({a: 1}))"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isExtensible(Object.preventExtensions({}))"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.isExtensible.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isExtensible()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isExtensible(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isExtensible(null)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isExtensible(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isExtensible(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isExtensible('test')"));
        }

        [TestMethod]
        public void keys()
        {
            Assert.AreEqual("", TestUtils.Evaluate("Object.keys(Object.prototype).toString()"));
            Assert.AreEqual("a", TestUtils.Evaluate("Object.keys({a: 1}).toString()"));
            Assert.AreEqual("0,1,2", TestUtils.Evaluate("Object.keys([1, 2, 3]).toString()"));
            Assert.AreEqual("0,1,2", TestUtils.Evaluate("Object.keys(new String('sdf')).toString()"));
            Assert.AreEqual("0,1,2", TestUtils.Evaluate("function foo() { return Object.keys(arguments) } foo(1, 2, 3).toString()"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.keys.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.keys()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.keys(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.keys(null)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.keys(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.keys(5)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.keys('test')"));
        }



        //     PROTOTYPE METHODS
        //_________________________________________________________________________________________

        [TestMethod]
        public void hasOwnProperty()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("String.prototype.hasOwnProperty('split')"));
            Assert.AreEqual(false, TestUtils.Evaluate("new String('test').hasOwnProperty('split')"));
            TestUtils.Evaluate("var x = new String('sdf');");
            TestUtils.Evaluate("x.testing = 5;");
            Assert.AreEqual(true, TestUtils.Evaluate("x.hasOwnProperty('testing')"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.hasOwnProperty('Testing')"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.hasOwnProperty('split')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Math.hasOwnProperty('cos')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Math.hasOwnProperty('E')"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = function() { this.a = 5; }; x.prototype = { b: 2 }; y = new x(); y.hasOwnProperty('a')"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = function() { this.a = 5; }; x.prototype = { b: 2 }; y = new x(); y.hasOwnProperty('b')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Math.hasOwnProperty({toString: function() { return 'max' }})"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Math.hasOwnProperty.length"));

            // "this" object must be convertible to object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Math.hasOwnProperty.call(undefined, 'max')"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Math.hasOwnProperty.call(null, 'max')"));

            // First parameter must be convertible to string.
            Assert.AreEqual("Error", TestUtils.EvaluateExceptionType("Math.hasOwnProperty({toString: function() { throw new Error('test') }})"));
            Assert.AreEqual("Error", TestUtils.EvaluateExceptionType("Math.hasOwnProperty.call(undefined, {toString: function() { throw new Error('test') }})"));
        }

        [TestMethod]
        public void isPrototypeOf()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("Object.prototype.isPrototypeOf({})"));
            Assert.AreEqual(false, TestUtils.Evaluate("({}).isPrototypeOf(Object.prototype)"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.prototype.isPrototypeOf(new String('test'))"));
            Assert.AreEqual(true, TestUtils.Evaluate("Error.prototype.isPrototypeOf(new RangeError('test'))"));
            Assert.AreEqual(false, TestUtils.Evaluate("RangeError.prototype.isPrototypeOf(new Error('test'))"));

            // If the parameter is not an object, return false.
            Assert.AreEqual(false, TestUtils.Evaluate("({}).isPrototypeOf('testing')"));
            Assert.AreEqual(false, TestUtils.Evaluate("({}).isPrototypeOf(false)"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.isPrototypeOf.call(undefined, false)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.isPrototypeOf.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isPrototypeOf.call(undefined, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.isPrototypeOf.call(null, {})"));
        }

        [TestMethod]
        public void propertyIsEnumerable()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("Math.propertyIsEnumerable('toString')"));
            Assert.AreEqual(false, TestUtils.Evaluate("Math.propertyIsEnumerable('min')"));
            Assert.AreEqual(true, TestUtils.Evaluate("({a:1}).propertyIsEnumerable('a')"));
            Assert.AreEqual(true, TestUtils.Evaluate("[15].propertyIsEnumerable('0')"));
            Assert.AreEqual(false, TestUtils.Evaluate("[15].propertyIsEnumerable('1')"));
            Assert.AreEqual(false, TestUtils.Evaluate("[15].propertyIsEnumerable('length')"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = function() { this.a = 5; }; x.prototype = { b: 2 }; y = new x(); y.propertyIsEnumerable('a')"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = function() { this.a = 5; }; x.prototype = { b: 2 }; y = new x(); y.propertyIsEnumerable('b')"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Object.propertyIsEnumerable.length"));

            // "this" object must be convertible to object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.propertyIsEnumerable.call(undefined, 'max')"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.propertyIsEnumerable.call(null, 'max')"));

            // First parameter must be convertible to string.
            Assert.AreEqual("Error", TestUtils.EvaluateExceptionType("Math.propertyIsEnumerable({toString: function() { throw new Error('test') }})"));
            Assert.AreEqual("Error", TestUtils.EvaluateExceptionType("Math.propertyIsEnumerable.call(undefined, {toString: function() { throw new Error('test') }})"));
        }

        [TestMethod]
        public void toLocaleString()
        {
            // The base implementation calls toString.
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("({}).toLocaleString()"));
            Assert.AreEqual("test", TestUtils.Evaluate("({ toString: function() { return 'test' } }).toLocaleString()"));

            // length
            Assert.AreEqual(0, TestUtils.Evaluate("Object.toLocaleString.length"));

            // Error is thrown if there is no implementation of toString.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("var o = Object.create(null); o.toLocaleString = Object.toLocaleString; o.toLocaleString()"));

            // Error in toString will result in error.
            Assert.AreEqual("Error", TestUtils.EvaluateExceptionType("({ toString: function() { throw new Error('test') } }).toLocaleString()"));

            // "this" must be convertible to an object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.toLocaleString.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Object.toLocaleString.call(null)"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("({}).toString()"));
            Assert.AreEqual("test", TestUtils.Evaluate("({ toString: function() { return 'test' } }).toString()"));

            // length
            Assert.AreEqual(0, TestUtils.Evaluate("Object.toString.length"));

            // Error in toString will result in error.
            Assert.AreEqual("Error", TestUtils.EvaluateExceptionType("({ toString: function() { throw new Error('test') } }).toString()"));

            // Addendum 7-1-10: null and undefined return their own strings.
            Assert.AreEqual("[object Undefined]", TestUtils.Evaluate("({}).toString.call(undefined)"));
            Assert.AreEqual("[object Null]", TestUtils.Evaluate("({}).toString.call(null)"));
        }

        [TestMethod]
        public void valueOf()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("Object.valueOf() === Object"));
            Assert.AreEqual(true, TestUtils.Evaluate("new String('test').valueOf() === 'test'"));
            Assert.AreEqual(true, TestUtils.Evaluate("new Number(5).valueOf() === 5"));

            // length
            Assert.AreEqual(0, TestUtils.Evaluate("Object.valueOf.length"));
        }
    }
}
