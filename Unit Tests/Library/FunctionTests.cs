using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the global Function object.
    /// </summary>
    [TestClass]
    public class FunctionTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Constructor
            Assert.AreEqual(Undefined.Value, Evaluate("f = new Function('', ''); f()"));
            Assert.AreEqual(4, Evaluate("f = new Function('a', 'b', 'return a+b'); f(1, 3)"));
            Assert.AreEqual(4, Evaluate("f = new Function('a,b', 'return a+b'); f(1, 3)"));
            Assert.AreEqual(3, Evaluate("f = new Function('a=1,b = 2', 'return a+b'); f()"));
            Assert.AreEqual(4, Evaluate("f = new Function('a', 'b=3', 'return a+b'); f(1)"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("f = new Function('a, ,b', 'return a+b')"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("f = new Function('a,15,b', 'return a+b')"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("f = new Function('a,this,b', 'return a+b')"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("f = new Function('a,c d,b', 'return a+b')"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("f = new Function('a,c d,b', 'return a+b }')"));

            // Call
            Assert.AreEqual(4, Evaluate("f = Function('a', 'b', 'return a+b'); f(1, 3)"));

            // toString
            Assert.AreEqual("function Function() { [native code] }", Evaluate("Function.toString()"));

            // length
            Assert.AreEqual(1, Evaluate("Function.length"));

            // no enumerable properties
            Assert.AreEqual("", Evaluate("y = ''; for (var x in Function) { y += x } y"));
            Assert.AreEqual("", Evaluate("y = ''; for (var x in new Function()) { y += x } y"));
        }

        [TestMethod]
        public void prototype()
        {
            // prototype of built-in object.
            Assert.AreEqual(PropertyAttributes.Sealed, EvaluateAccessibility("Function", "prototype"));

            // prototype of new function.
            Assert.AreEqual(PropertyAttributes.Writable, EvaluateAccessibility("new Function()", "prototype"));

            // prototype of empty function.
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(Function).prototype === undefined"));

            // [[Prototype]]
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(new Function()) === Function.prototype"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(Function.prototype) === Object.prototype"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(new Function().prototype) === Object.prototype"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf((function() {}).prototype) === Object.prototype"));

            // prototype of built-in functions should be undefined.
            Assert.AreEqual(Undefined.Value, Evaluate("Math.sin.prototype"));

            // prototype of new functions should be a new object.
            Assert.AreEqual(true, Evaluate("Function('a+b').prototype !== Object.prototype"));
            Assert.AreEqual("object", Evaluate("typeof(Function('a+b').prototype)"));

            // The prototype property becomes the prototype of new objects (as long as the prototype property is an object).
            Assert.AreEqual(true, Evaluate("f = function() { }; x = {}; f.prototype = x; Object.getPrototypeOf(new f()) === x"));
            Assert.AreEqual(true, Evaluate("f = function() { }; x = 5; f.prototype = x; Object.getPrototypeOf(new f()) === Object.prototype"));
        }

        [TestMethod]
        public void constructor()
        {
            Assert.AreEqual(true, Evaluate("new Function().constructor === Function"));
            Assert.AreEqual(true, Evaluate("f = Function('a+b'); f.prototype.constructor === f"));

            // constructor is non-enumerable, writable and configurable.
            Assert.AreEqual(PropertyAttributes.NonEnumerable, EvaluateAccessibility("Function.prototype", "constructor"));
        }

        [TestMethod]
        public void length()
        {
            Assert.AreEqual(0, Evaluate("Function.prototype.length"));
            Assert.AreEqual(1, Evaluate("new Function('a', 'return this / a').length"));
            Assert.AreEqual(2, Evaluate("new Function('a,b', 'return a / b').length"));
            Assert.AreEqual(2, Evaluate("(function(a, b) {}).length"));
            
            // length is non-enumerable, non-writable and configurable.
            Assert.AreEqual(PropertyAttributes.Configurable, EvaluateAccessibility("Function", "length"));
            Assert.AreEqual(PropertyAttributes.Configurable, EvaluateAccessibility("new Function", "length"));
        }

        [TestMethod]
        public void name()
        {
            Assert.AreEqual("f", Evaluate("function f() { } f.name"));
            Assert.AreEqual("g", Evaluate("f = function g() { }; f.name"));
            Assert.AreEqual("", Evaluate("f = function() { }; f.name"));
            Assert.AreEqual("f", Evaluate("x = { y: function f() { } }; x.y.name"));
            Assert.AreEqual("", Evaluate("x = { y: function() { } }; x.y.name"));
            Assert.AreEqual("get f", Evaluate("x = { get f() { } }; Object.getOwnPropertyDescriptor(x, 'f').get.name"));
            Assert.AreEqual("set f", Evaluate("x = { set f(val) { } }; Object.getOwnPropertyDescriptor(x, 'f').set.name"));
            Assert.AreEqual("anonymous", Evaluate("new Function('').name"));
        }

        [TestMethod]
        public void displayName()
        {
            Assert.AreEqual(Undefined.Value, Evaluate("function f() { } f.displayName"));
            Assert.AreEqual(Undefined.Value, Evaluate("f = function g() { }; f.displayName"));
            Assert.AreEqual("f", Evaluate("f = function() { }; f.displayName"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = { y: function f() { } }; x.y.displayName"));
            Assert.AreEqual("y", Evaluate("x = { y: function() { } }; x.y.displayName"));
            Assert.AreEqual("get f", Evaluate("x = { get f() { } }; Object.getOwnPropertyDescriptor(x, 'f').get.displayName"));
            Assert.AreEqual("set f", Evaluate("x = { set f(value) { } }; Object.getOwnPropertyDescriptor(x, 'f').set.displayName"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Function('').displayName"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("function anonymous(a, b) {\nreturn a + b\n}", Evaluate("new Function('a, b', 'return a + b').toString()"));
            Assert.AreEqual("function atan2() { [native code] }", Evaluate("Math.atan2.toString()"));
            Assert.AreEqual("function (a, b) {\n return a + b \n}", Evaluate("(function(a, b) { return a + b }).toString()"));
            Assert.AreEqual("function (a, b) {\n return a + 51 \n}", Evaluate("(function(a, b) { return a + 51 }).toString()"));
            Assert.AreEqual("function (a, b) {\n function inner() { return a + b } return inner() \n}",
                Evaluate("(function(a, b) { function inner() { return a + b } return inner() }).toString()"));
        }

        [TestMethod]
        public void apply()
        {
            Assert.AreEqual("[object Math]", Evaluate("({}.toString.apply(Math, []))"));
            Assert.AreEqual("onetwo", Evaluate("(String.prototype.concat).apply('one', ['two'])"));
            Assert.AreEqual(2, Evaluate("new Function('a', 'return this / a').apply(10, [5])"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Function('a', 'return a').apply(10, null)"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Function('a', 'return a').apply(10, undefined)"));
            Assert.AreEqual(5, Evaluate("new Function('a', 'return a').apply(10, {length: 1, 0: 5})"));

            // length
            Assert.AreEqual(2, Evaluate("Function.prototype.apply.length"));

            // Errors
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Function('a', 'return a').apply(10, 2)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Function('a', 'return a').apply(10, {})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Function('a', 'return a').apply(10, {length: null})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Function('a', 'return a').apply(10, {length: undefined})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Function('a', 'return a').apply(10, {length: -1})"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Function('a', 'return a').apply(10, {length: 4294967296})"));
        }

        [TestMethod]
        public void call()
        {
            Assert.AreEqual("[object Math]", Evaluate("({}.toString.call(Math))"));
            Assert.AreEqual(2, Evaluate("new Function('a', 'return this / a').call(10, 5)"));
            Assert.AreEqual(true, Evaluate("new Function('return this').call() === this"));
            Assert.AreEqual("[object Undefined]", Evaluate("toString.call()"));

            // length
            Assert.AreEqual(1, Evaluate("Function.prototype.call.length"));
        }

        [TestMethod]
        public void bind()
        {
            Assert.AreEqual("[object Math]", Evaluate("var f = {}.toString.bind(Math); f();"));
            Assert.AreEqual(32, Evaluate("var f = Math.pow.bind(undefined, 2); f(5);"));
            Assert.AreEqual(5, Evaluate("new Function('a,b', 'return a / b').bind(undefined, 10)(2)"));
            Assert.AreEqual(15, Evaluate("new Function('a,b', 'return a + b').bind(undefined, 10, 5)(2)"));

            // length of bound functions is the number of arguments remaining.
            Assert.AreEqual(2, Evaluate("Math.pow.length"));
            Assert.AreEqual(2, Evaluate("var f = Math.pow.bind(undefined); f.length"));
            Assert.AreEqual(1, Evaluate("var f = Math.pow.bind(undefined, 2); f.length"));
            Assert.AreEqual(0, Evaluate("var f = Math.pow.bind(undefined, 2, 5); f.length"));
            Assert.AreEqual(0, Evaluate("var f = Math.pow.bind(undefined, 2, 5, 7); f.length"));

            // length
            Assert.AreEqual(1, Evaluate("Function.prototype.bind.length"));

            // Caller and arguments throw a TypeError exception.
            Assert.AreEqual("TypeError", EvaluateExceptionType(@"
                function foo() { return bar.arguments; }
                var bar = foo.bind({});
                function baz() { return bar(); }
                baz();"));
            Assert.AreEqual("TypeError", EvaluateExceptionType(@"
                function foo() { return bar.caller; }
                var bar = foo.bind({});
                function baz() { return bar(); }
                baz();"));
        }

        [TestMethod]
        public void FunctionDeclarationWithinLoop()
        {
            // This shouldn't throw.
            Execute(@"
'use strict';
for (var i = 0; i < 3; i++) {
    (function () {
        function test() {
        }
        test();
    }());
}

");
        }
    }
}
