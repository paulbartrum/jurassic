using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the global Object object.
    /// </summary>
    [TestClass]
    public class ObjectTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Construct
            Assert.AreEqual("[object Object]", Evaluate("new Object().toString()"));
            Assert.AreEqual("[object Object]", Evaluate("new Object(undefined).toString()"));
            Assert.AreEqual("[object Object]", Evaluate("new Object(null).toString()"));
            Assert.AreEqual("hi", Evaluate("new Object('hi').valueOf()"));
            Assert.AreEqual(5501, Evaluate("new Object(5501).valueOf()"));
            Assert.AreEqual(true, Evaluate("x = new String('test'); new Object(x) === x"));
            Assert.AreEqual(true, Evaluate("x = new Number(55); new Object(x) === x"));

            // Call
            Assert.AreEqual("[object Object]", Evaluate("Object().toString()"));
            Assert.AreEqual("[object Object]", Evaluate("Object(undefined).toString()"));
            Assert.AreEqual("[object Object]", Evaluate("Object(null).toString()"));
            Assert.AreEqual("hi", Evaluate("Object('hi').valueOf()"));
            Assert.AreEqual(5501, Evaluate("Object(5501).valueOf()"));
            Assert.AreEqual(true, Evaluate("x = new String('test'); Object(x) === x"));
            Assert.AreEqual(true, Evaluate("x = new Number(55); Object(x) === x"));

            // length
            Assert.AreEqual(1, Evaluate("Object.length"));

            // toString
            Assert.AreEqual("function Object() { [native code] }", Evaluate("Object.toString()"));

            // valueOf
            Assert.AreEqual(true, Evaluate("Object.valueOf() === Object"));

            // prototype
            Assert.AreEqual(true, Evaluate("Object.prototype === Object.getPrototypeOf(new Object())"));
            Assert.AreEqual(PropertyAttributes.Sealed, EvaluateAccessibility("Object", "prototype"));

            // constructor
            Assert.AreEqual(true, Evaluate("Object.prototype.constructor === Object"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, EvaluateAccessibility("Object.prototype", "constructor"));

            // Functions are writable and configurable.
            Assert.AreEqual(PropertyAttributes.NonEnumerable, EvaluateAccessibility("Object", "keys"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, EvaluateAccessibility("Object.prototype", "hasOwnProperty"));
        }



        //     CONSTRUCTOR METHODS
        //_________________________________________________________________________________________

        [TestMethod]
        public void getPrototypeOf()
        {
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(new Number(55)) === Number.prototype"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(new String('test')) === String.prototype"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(true) == Boolean.prototype"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(5) == Number.prototype"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf('test') == String.prototype"));

            // length
            Assert.AreEqual(1, Evaluate("Object.getPrototypeOf.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.getPrototypeOf()"));
        }

        [TestMethod]
        public void getOwnPropertyDescriptor()
        {
            // Non-existant properties return "undefined".
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(Number, 'bunnyEars')"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(Number, 'toString')"));

            // Otherwise, returns an object containing various properties.
            Evaluate("var descriptor = Object.getOwnPropertyDescriptor(Number, 'MAX_VALUE')");
            Assert.AreEqual(false, Evaluate("descriptor.configurable"));
            Assert.AreEqual(false, Evaluate("descriptor.enumerable"));
            Assert.AreEqual(false, Evaluate("descriptor.writable"));
            Assert.AreEqual(1.7976931348623157e+308, Evaluate("descriptor.value"));

            Evaluate("var descriptor = Object.getOwnPropertyDescriptor({p: 15}, 'p')");
            Assert.AreEqual(true, Evaluate("descriptor.configurable"));
            Assert.AreEqual(true, Evaluate("descriptor.enumerable"));
            Assert.AreEqual(true, Evaluate("descriptor.writable"));
            Assert.AreEqual(15, Evaluate("descriptor.value"));
            Assert.AreEqual(Undefined.Value, Evaluate("descriptor.get"));
            Assert.AreEqual(Undefined.Value, Evaluate("descriptor.set"));

            // Check the right properties are defined.
            Assert.AreEqual(true, Evaluate("descriptor.hasOwnProperty('value')"));
            Assert.AreEqual(true, Evaluate("descriptor.hasOwnProperty('writable')"));
            Assert.AreEqual(false, Evaluate("descriptor.hasOwnProperty('get')"));
            Assert.AreEqual(false, Evaluate("descriptor.hasOwnProperty('set')"));

            // Add accessor property.
            Evaluate("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, enumerable: true, configurable: true}); descriptor = Object.getOwnPropertyDescriptor(x, 'a')");
            Assert.AreEqual(Undefined.Value, Evaluate("descriptor.value"));
            Assert.AreEqual(true, Evaluate("descriptor.enumerable"));
            Assert.AreEqual(Undefined.Value, Evaluate("descriptor.writable"));
            Assert.AreEqual(true, Evaluate("descriptor.configurable"));
            Assert.AreEqual("function", Evaluate("typeof(descriptor.get)"));
            Assert.AreEqual(Undefined.Value, Evaluate("descriptor.set"));

            // Check the right properties are defined.
            Assert.AreEqual(false, Evaluate("descriptor.hasOwnProperty('value')"));
            Assert.AreEqual(false, Evaluate("descriptor.hasOwnProperty('writable')"));
            Assert.AreEqual(true, Evaluate("descriptor.hasOwnProperty('get')"));
            Assert.AreEqual(true, Evaluate("descriptor.hasOwnProperty('set')"));

            Evaluate("var x = {}; Object.defineProperty(x, 'a', {}); descriptor = Object.getOwnPropertyDescriptor(x, 'a')");
            Assert.AreEqual(Undefined.Value, Evaluate("descriptor.value"));
            Assert.AreEqual(false, Evaluate("descriptor.enumerable"));
            Assert.AreEqual(false, Evaluate("descriptor.writable"));
            Assert.AreEqual(false, Evaluate("descriptor.configurable"));
            Assert.AreEqual(Undefined.Value, Evaluate("descriptor.get"));
            Assert.AreEqual(Undefined.Value, Evaluate("descriptor.set"));

            // Check the right properties are defined.
            Assert.AreEqual(true, Evaluate("descriptor.hasOwnProperty('value')"));
            Assert.AreEqual(true, Evaluate("descriptor.hasOwnProperty('writable')"));
            Assert.AreEqual(false, Evaluate("descriptor.hasOwnProperty('get')"));
            Assert.AreEqual(false, Evaluate("descriptor.hasOwnProperty('set')"));

            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(true, 'toString')"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(5, 'toString')"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor('test', 'toString')"));

            // length
            Assert.AreEqual(2, Evaluate("Object.getOwnPropertyDescriptor.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.getOwnPropertyDescriptor()"));
        }

        [TestMethod]
        public void getOwnPropertyNames()
        {
            Assert.AreEqual("", Evaluate("Object.getOwnPropertyNames({}).toString()"));
            Assert.AreEqual("a", Evaluate("Object.getOwnPropertyNames({a: 'hello'}).toString()"));
            Assert.AreEqual("0,1,length", Evaluate("Object.getOwnPropertyNames([15, 16]).toString()"));
            Assert.AreEqual("", Evaluate("Object.getOwnPropertyNames(true).toString()"));
            Assert.AreEqual("", Evaluate("Object.getOwnPropertyNames(5).toString()"));
            Assert.AreEqual("0,1,2,3,length", Evaluate("Object.getOwnPropertyNames('test').toString()"));

            // length
            Assert.AreEqual(1, Evaluate("Object.getOwnPropertyNames.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.getOwnPropertyNames()"));
        }

        [TestMethod]
        public void create()
        {
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(Object.create(null)) === null"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(Object.create(String.prototype)) === String.prototype"));

            // Initialize object properties
            Evaluate("var x = Object.create(null, {a: {value: 5}, b: {value: 10}})");
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(x) === null"));
            Assert.AreEqual(5, Evaluate("x.a"));
            Assert.AreEqual(10, Evaluate("x.b"));

            // length
            Assert.AreEqual(2, Evaluate("Object.create.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.create()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.create(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.create(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.create(5)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.create('test')"));
        }

        [TestMethod]
        public void defineProperty()
        {
            // Add simple property.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: true, configurable: true}) === x"));
            Assert.AreEqual(7, Evaluate("x.a"));
            Assert.AreEqual(PropertyAttributes.FullAccess, EvaluateAccessibility("x", "a"));
            Assert.AreEqual("a", Evaluate("var p = ''; for (var y in x) { p += y } p"));
            Assert.AreEqual(5, Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a"));

            // Add non-writable property.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}) === x"));
            Assert.AreEqual(7, Evaluate("x.a"));
            Assert.AreEqual(PropertyAttributes.Enumerable | PropertyAttributes.Configurable, EvaluateAccessibility("x", "a"));
            Assert.AreEqual("a", Evaluate("var p = ''; for (var y in x) { p += y } p"));
            Assert.AreEqual(7, Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a"));

            // Non-writable properties can have the value changed (as long as configurable = true).
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}) === x"));
            Assert.AreEqual(8, Evaluate("Object.defineProperty(x, 'a', {value: 8, enumerable: true, writable: false, configurable: true}); x.a"));
            Assert.AreEqual(9, Evaluate("Object.defineProperty(x, 'a', {value: 9, enumerable: true, writable: false, configurable: false}); x.a"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 10, enumerable: true, writable: false, configurable: false}); x.a"));

            // Non-writable properties can be switched to accessor properties (as long as configurable = true).
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}) === x"));
            Assert.AreEqual(1, Evaluate("Object.defineProperty(x, 'a', {get: function() { return 1 }, enumerable: true, configurable: true}); x.a"));

            // Non-writable properties can go back to writable.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}) === x"));
            Assert.AreEqual(8, Evaluate("Object.defineProperty(x, 'a', {value: 8, enumerable: true, writable: true, configurable: true}); x.a"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));

            // Add non-enumerable property.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: false, writable: true, configurable: true}) === x"));
            Assert.AreEqual(7, Evaluate("x.a"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, EvaluateAccessibility("x", "a"));
            Assert.AreEqual("", Evaluate("var p = ''; for (var y in x) { p += y } p"));
            Assert.AreEqual(5, Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a"));

            // Add non-configurable property.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: true, configurable: false}) === x"));
            Assert.AreEqual(7, Evaluate("x.a"));
            Assert.AreEqual(PropertyAttributes.Enumerable | PropertyAttributes.Writable, EvaluateAccessibility("x", "a"));
            Assert.AreEqual("a", Evaluate("var p = ''; for (var y in x) { p += y } p"));
            Assert.AreEqual(5, Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(false, Evaluate("delete x.a"));
            Assert.AreEqual(5, Evaluate("x.a"));

            // Add read-only accessor property.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, enumerable: true, configurable: true}) === x"));
            Assert.AreEqual(7, Evaluate("x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual("function", Evaluate("typeof(Object.getOwnPropertyDescriptor(x, 'a').get)"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').set"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));
            Assert.AreEqual(7, Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a"));

            // Add read-write accessor property.
            Assert.AreEqual(true, Evaluate("var x = {a2: 10}; Object.defineProperty(x, 'a', {get: function() { return this.a2 }, set: function(value) { this.a2 = value }, enumerable: true, configurable: true}) === x"));
            Assert.AreEqual(10, Evaluate("x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual("function", Evaluate("typeof(Object.getOwnPropertyDescriptor(x, 'a').get)"));
            Assert.AreEqual("function", Evaluate("typeof(Object.getOwnPropertyDescriptor(x, 'a').set)"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));
            Assert.AreEqual(5, Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(true, Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a"));

            // Add write-only accessor property.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {set: function(value) { this.a2 = value }, enumerable: true, configurable: true}) === x"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').get"));
            Assert.AreEqual("function", Evaluate("typeof(Object.getOwnPropertyDescriptor(x, 'a').set)"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a = 5; x.a"));
            Assert.AreEqual(5, Evaluate("x.a = 5; x.a2"));
            Assert.AreEqual(true, Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a"));

            // An empty property descriptor defines a default property (if the property doesn't already exist).
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {}) === x"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a"));
            Assert.AreEqual(true, Evaluate("x.hasOwnProperty('a')"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').get"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').set"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));

            // An empty property descriptor does nothing if the property already exists.
            Assert.AreEqual(true, Evaluate("var x = {a: 5}; Object.defineProperty(x, 'a', {}) === x"));
            Assert.AreEqual(5, Evaluate("x.a"));
            Assert.AreEqual(true, Evaluate("x.hasOwnProperty('a')"));
            Assert.AreEqual(5, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').get"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').set"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));

            // A property descriptor without a value does not change the existing value.
            Assert.AreEqual(true, Evaluate("var x = {a: 5}; Object.defineProperty(x, 'a', {writable: false}) === x"));
            Assert.AreEqual(5, Evaluate("x.a"));
            Assert.AreEqual(true, Evaluate("x.hasOwnProperty('a')"));
            Assert.AreEqual(5, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').value"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').get"));
            Assert.AreEqual(Undefined.Value, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').set"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').enumerable"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').writable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'a').configurable"));

            // get and set can be undefined.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 1; }, set: undefined, configurable: true}) === x"));
            Assert.AreEqual(1, Evaluate("x.a"));
            Assert.AreEqual(true, Evaluate("Object.defineProperty(x, 'a', {get: undefined, set: undefined}) === x"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.a"));

            // Non-extensible objects cannot have properties added.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.preventExtensions(x) === x"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: true, configurable: true})"));

            // Non-configurable properties can only have the value changed.
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: true, configurable: false}) === x"));
            Assert.AreEqual(10, Evaluate("Object.defineProperty(x, 'a', {value: 10, enumerable: true, writable: true, configurable: false}); x.a"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(x, 'a', {get: function() { return 7; }, enumerable: true, writable: true, configurable: false}); x.a"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 10, enumerable: false, writable: true, configurable: false})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 10, enumerable: true, writable: false, configurable: false})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(x, 'a', {value: 10, enumerable: true, writable: true, configurable: true})"));
            Assert.AreEqual(true, Evaluate("var x = {}; var f = function() { return 1; }; Object.defineProperty(x, 'a', {get: f}); Object.defineProperty(x, 'a', {get: f}) === x"));
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 1; }}); Object.defineProperty(x, 'a', {set: undefined}) === x"));

            // Value is not allowed when specifying an accessor property.
            Assert.AreEqual("TypeError", EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, enumerable: true, configurable: true, value: 5})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, set: function() { }, enumerable: true, configurable: true, value: 5})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {set: function() { }, enumerable: true, configurable: true, value: 5})"));

            // Writable is not allowed when specifying an accessor property.
            Assert.AreEqual("TypeError", EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, enumerable: true, writable: false, configurable: true})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: function() { return 7 }, set: function() { }, enumerable: true, writable: true, configurable: true})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {set: function() { }, enumerable: true, writable: true, configurable: true})"));

            // Get and set must be a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: 5, enumerable: true, configurable: true})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {get: 5, set: 5, enumerable: true, configurable: true})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("var x = {}; Object.defineProperty(x, 'a', {set: 5, enumerable: true, configurable: true})"));

            // length
            Assert.AreEqual(3, Evaluate("Object.defineProperty.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty({})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(undefined, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(null, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(true, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty(5, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty('test', {})"));

            // Property descriptors must be objects.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty({}, 'a', 5)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty({}, 'a', undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperty({}, 'a', null)"));
        }

        [TestMethod]
        public void defineProperties()
        {
            Assert.AreEqual(true, Evaluate("var x = {}; Object.defineProperties(x, {a: {value: 5}, b: {value: 10}}) === x"));
            Assert.AreEqual(5, Evaluate("x.a"));
            Assert.AreEqual(10, Evaluate("x.b"));

            // length
            Assert.AreEqual(2, Evaluate("Object.defineProperties.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties({})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties(undefined, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties(null, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties(true, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties(5, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties('test', {})"));

            // Property descriptors must be objects.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties({}, { a: 1 })"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties({}, { a: undefined })"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.defineProperties({}, { a: null })"));
        }

        [TestMethod]
        public void seal()
        {
            // Simple object
            Assert.AreEqual(true, Evaluate("var x = {a: 1}; Object.seal(x) === x"));
            Assert.AreEqual(false, Evaluate("delete x.a"));
            Assert.AreEqual(2, Evaluate("x.a = 2; x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.b = 6; x.b"));
            Assert.AreEqual(PropertyAttributes.Enumerable | PropertyAttributes.Writable, EvaluateAccessibility("x", "a"));
            Assert.AreEqual(true, Evaluate("Object.seal(true).valueOf()"));
            Assert.AreEqual(5, Evaluate("Object.seal(5).valueOf()"));
            Assert.AreEqual("test", Evaluate("Object.seal('test').valueOf()"));

            // length
            Assert.AreEqual(1, Evaluate("Object.seal.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.seal()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.seal(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.seal(null)"));
        }

        [TestMethod]
        public void freeze()
        {
            // Simple object
            Assert.AreEqual(true, Evaluate("var x = {a: 1}; Object.freeze(x) === x"));
            Assert.AreEqual(false, Evaluate("delete x.a"));
            Assert.AreEqual(1, Evaluate("x.a = 2; x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.b = 6; x.b"));
            Assert.AreEqual(PropertyAttributes.Enumerable, EvaluateAccessibility("x", "a"));
            Assert.AreEqual(true, Evaluate("Object.freeze(true).valueOf()"));
            Assert.AreEqual(5, Evaluate("Object.freeze(5).valueOf()"));
            Assert.AreEqual("test", Evaluate("Object.freeze('test').valueOf()"));

            // length
            Assert.AreEqual(1, Evaluate("Object.freeze.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.freeze()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.freeze(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.freeze(null)"));
        }

        [TestMethod]
        public void preventExtensions()
        {
            // Simple object
            Assert.AreEqual(true, Evaluate("var x = {a: 1, c: 2}; Object.preventExtensions(x) === x"));
            Assert.AreEqual(2, Evaluate("x.a = 2; x.a"));
            Assert.AreEqual(true, Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.b = 6; x.b"));
            Assert.AreEqual(PropertyAttributes.FullAccess, EvaluateAccessibility("x", "c"));
            Assert.AreEqual(true, Evaluate("Object.preventExtensions(true).valueOf()"));
            Assert.AreEqual(5, Evaluate("Object.preventExtensions(5).valueOf()"));
            Assert.AreEqual("test", Evaluate("Object.preventExtensions('test').valueOf()"));

            // length
            Assert.AreEqual(1, Evaluate("Object.preventExtensions.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.preventExtensions()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.preventExtensions(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.preventExtensions(null)"));
        }

        [TestMethod]
        public void isSealed()
        {
            Assert.AreEqual(false, Evaluate("Object.isSealed({})"));
            Assert.AreEqual(false, Evaluate("Object.isSealed({a: 1})"));
            Assert.AreEqual(true, Evaluate("Object.isSealed(Object.seal({a: 1}))"));
            Assert.AreEqual(true, Evaluate("Object.isSealed(Object.freeze({a: 1}))"));
            Assert.AreEqual(true, Evaluate("Object.isSealed(Object.preventExtensions({}))"));
            Assert.AreEqual(false, Evaluate("Object.isSealed(Object.preventExtensions({a: 1}))"));
            Assert.AreEqual(false, Evaluate("Object.isSealed(new Boolean(true))"));
            Assert.AreEqual(true, Evaluate("Object.isSealed(true)"));
            Assert.AreEqual(true, Evaluate("Object.isSealed(5)"));
            Assert.AreEqual(true, Evaluate("Object.isSealed('test')"));

            // length
            Assert.AreEqual(1, Evaluate("Object.isSealed.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isSealed()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isSealed(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isSealed(null)"));
        }

        [TestMethod]
        public void isFrozen()
        {
            Assert.AreEqual(false, Evaluate("Object.isFrozen({})"));
            Assert.AreEqual(false, Evaluate("Object.isFrozen({a: 1})"));
            Assert.AreEqual(false, Evaluate("Object.isFrozen(Object.seal({a: 1}))"));
            Assert.AreEqual(true, Evaluate("Object.isFrozen(Object.freeze({a: 1}))"));
            Assert.AreEqual(true, Evaluate("Object.isFrozen(Object.preventExtensions({}))"));
            Assert.AreEqual(false, Evaluate("Object.isFrozen(new Boolean(true))"));
            Assert.AreEqual(true, Evaluate("Object.isFrozen(true)"));
            Assert.AreEqual(true, Evaluate("Object.isFrozen(5)"));
            Assert.AreEqual(true, Evaluate("Object.isFrozen('test')"));

            // length
            Assert.AreEqual(1, Evaluate("Object.isFrozen.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isFrozen()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isFrozen(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isFrozen(null)"));
        }

        [TestMethod]
        public void isExtensible()
        {
            Assert.AreEqual(true, Evaluate("Object.isExtensible({})"));
            Assert.AreEqual(true, Evaluate("Object.isExtensible({a: 1})"));
            Assert.AreEqual(false, Evaluate("Object.isExtensible(Object.seal({a: 1}))"));
            Assert.AreEqual(false, Evaluate("Object.isExtensible(Object.freeze({a: 1}))"));
            Assert.AreEqual(false, Evaluate("Object.isExtensible(Object.preventExtensions({}))"));
            Assert.AreEqual(true, Evaluate("Object.isExtensible(new Boolean(true))"));
            Assert.AreEqual(false, Evaluate("Object.isExtensible(true)"));
            Assert.AreEqual(false, Evaluate("Object.isExtensible(5)"));
            Assert.AreEqual(false, Evaluate("Object.isExtensible('test')"));

            // length
            Assert.AreEqual(1, Evaluate("Object.isExtensible.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isExtensible()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isExtensible(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isExtensible(null)"));
            
        }

        [TestMethod]
        public void keys()
        {
            Assert.AreEqual("", Evaluate("Object.keys(Object.prototype).toString()"));
            Assert.AreEqual("a", Evaluate("Object.keys({a: 1}).toString()"));
            Assert.AreEqual("0,1,2", Evaluate("Object.keys([1, 2, 3]).toString()"));
            Assert.AreEqual("0,1,2", Evaluate("Object.keys(new String('sdf')).toString()"));
            Assert.AreEqual("0,1,2", Evaluate("function foo() { return Object.keys(arguments) } foo(1, 2, 3).toString()"));

            // length
            Assert.AreEqual(1, Evaluate("Object.keys.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.keys()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.keys(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.keys(null)"));
            Assert.AreEqual("", Evaluate("Object.keys(true).toString()"));
            Assert.AreEqual("", Evaluate("Object.keys(5).toString()"));
            Assert.AreEqual("0,1,2,3", Evaluate("Object.keys('test').toString()"));
        }



        //     PROTOTYPE METHODS
        //_________________________________________________________________________________________

        [TestMethod]
        public void hasOwnProperty()
        {
            Assert.AreEqual(true, Evaluate("String.prototype.hasOwnProperty('split')"));
            Assert.AreEqual(false, Evaluate("new String('test').hasOwnProperty('split')"));
            Evaluate("var x = new String('sdf');");
            Evaluate("x.testing = 5;");
            Assert.AreEqual(true, Evaluate("x.hasOwnProperty('testing')"));
            Assert.AreEqual(false, Evaluate("x.hasOwnProperty('Testing')"));
            Assert.AreEqual(false, Evaluate("x.hasOwnProperty('split')"));
            Assert.AreEqual(true, Evaluate("Math.hasOwnProperty('cos')"));
            Assert.AreEqual(true, Evaluate("Math.hasOwnProperty('E')"));
            Assert.AreEqual(true, Evaluate("x = function() { this.a = 5; }; x.prototype = { b: 2 }; y = new x(); y.hasOwnProperty('a')"));
            Assert.AreEqual(false, Evaluate("x = function() { this.a = 5; }; x.prototype = { b: 2 }; y = new x(); y.hasOwnProperty('b')"));
            Assert.AreEqual(true, Evaluate("Math.hasOwnProperty({toString: function() { return 'max' }})"));

            // length
            Assert.AreEqual(1, Evaluate("Math.hasOwnProperty.length"));

            // "this" object must be convertible to object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Math.hasOwnProperty.call(undefined, 'max')"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Math.hasOwnProperty.call(null, 'max')"));

            // First parameter must be convertible to string.
            Assert.AreEqual("Error", EvaluateExceptionType("Math.hasOwnProperty({toString: function() { throw new Error('test') }})"));
            Assert.AreEqual("Error", EvaluateExceptionType("Math.hasOwnProperty.call(undefined, {toString: function() { throw new Error('test') }})"));
        }

        [TestMethod]
        public void isPrototypeOf()
        {
            Assert.AreEqual(true, Evaluate("Object.prototype.isPrototypeOf({})"));
            Assert.AreEqual(false, Evaluate("({}).isPrototypeOf(Object.prototype)"));
            Assert.AreEqual(true, Evaluate("Object.prototype.isPrototypeOf(new String('test'))"));
            Assert.AreEqual(true, Evaluate("Error.prototype.isPrototypeOf(new RangeError('test'))"));
            Assert.AreEqual(false, Evaluate("RangeError.prototype.isPrototypeOf(new Error('test'))"));

            // If the parameter is not an object, return false.
            Assert.AreEqual(false, Evaluate("({}).isPrototypeOf('testing')"));
            Assert.AreEqual(false, Evaluate("({}).isPrototypeOf(false)"));
            Assert.AreEqual(false, Evaluate("Object.isPrototypeOf.call(undefined, false)"));

            // length
            Assert.AreEqual(1, Evaluate("Object.isPrototypeOf.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isPrototypeOf.call(undefined, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.isPrototypeOf.call(null, {})"));
        }

        [TestMethod]
        public void propertyIsEnumerable()
        {
            Assert.AreEqual(false, Evaluate("Math.propertyIsEnumerable('toString')"));
            Assert.AreEqual(false, Evaluate("Math.propertyIsEnumerable('min')"));
            Assert.AreEqual(true, Evaluate("({a:1}).propertyIsEnumerable('a')"));
            Assert.AreEqual(true, Evaluate("[15].propertyIsEnumerable('0')"));
            Assert.AreEqual(false, Evaluate("[15].propertyIsEnumerable('1')"));
            Assert.AreEqual(false, Evaluate("[15].propertyIsEnumerable('length')"));
            Assert.AreEqual(true, Evaluate("x = function() { this.a = 5; }; x.prototype = { b: 2 }; y = new x(); y.propertyIsEnumerable('a')"));
            Assert.AreEqual(false, Evaluate("x = function() { this.a = 5; }; x.prototype = { b: 2 }; y = new x(); y.propertyIsEnumerable('b')"));

            // length
            Assert.AreEqual(1, Evaluate("Object.propertyIsEnumerable.length"));

            // "this" object must be convertible to object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.propertyIsEnumerable.call(undefined, 'max')"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.propertyIsEnumerable.call(null, 'max')"));

            // First parameter must be convertible to string.
            Assert.AreEqual("Error", EvaluateExceptionType("Math.propertyIsEnumerable({toString: function() { throw new Error('test') }})"));
            Assert.AreEqual("Error", EvaluateExceptionType("Math.propertyIsEnumerable.call(undefined, {toString: function() { throw new Error('test') }})"));
        }

        [TestMethod]
        public void toLocaleString()
        {
            // The base implementation calls toString.
            Assert.AreEqual("[object Object]", Evaluate("({}).toLocaleString()"));
            Assert.AreEqual("test", Evaluate("({ toString: function() { return 'test' } }).toLocaleString()"));

            // length
            Assert.AreEqual(0, Evaluate("Object.toLocaleString.length"));

            // Error is thrown if there is no implementation of toString.
            Assert.AreEqual("TypeError", EvaluateExceptionType("var o = Object.create(null); o.toLocaleString = Object.toLocaleString; o.toLocaleString()"));

            // Error in toString will result in error.
            Assert.AreEqual("Error", EvaluateExceptionType("({ toString: function() { throw new Error('test') } }).toLocaleString()"));

            // "this" must be convertible to an object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.toLocaleString.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Object.toLocaleString.call(null)"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("[object Object]", Evaluate("({}).toString()"));
            Assert.AreEqual("test", Evaluate("({ toString: function() { return 'test' } }).toString()"));

            // length
            Assert.AreEqual(0, Evaluate("Object.toString.length"));

            // Error in toString will result in error.
            Assert.AreEqual("Error", EvaluateExceptionType("({ toString: function() { throw new Error('test') } }).toString()"));

            // Addendum 7-1-10: null and undefined return their own strings.
            Assert.AreEqual("[object Undefined]", Evaluate("({}).toString.call(undefined)"));
            Assert.AreEqual("[object Null]", Evaluate("({}).toString.call(null)"));
        }

        [TestMethod]
        public void valueOf()
        {
            Assert.AreEqual(true, Evaluate("Object.valueOf() === Object"));
            Assert.AreEqual(true, Evaluate("new String('test').valueOf() === 'test'"));
            Assert.AreEqual(true, Evaluate("new Number(5).valueOf() === 5"));

            // length
            Assert.AreEqual(0, Evaluate("Object.valueOf.length"));
        }
    }
}
