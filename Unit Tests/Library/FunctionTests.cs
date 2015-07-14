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
    /// Test the global Function object.
    /// </summary>
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void Constructor()
        {
            // Constructor
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("f = new Function('', ''); f()"));
            Assert.AreEqual(4, TestUtils.Evaluate("f = new Function('a', 'b', 'return a+b'); f(1, 3)"));
            Assert.AreEqual(4, TestUtils.Evaluate("f = new Function('a,b', 'return a+b'); f(1, 3)"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("f = new Function('a, ,b', 'return a+b')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("f = new Function('a,15,b', 'return a+b')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("f = new Function('a,this,b', 'return a+b')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("f = new Function('a,c d,b', 'return a+b')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("f = new Function('a,c d,b', 'return a+b }')"));

            // Call
            Assert.AreEqual(4, TestUtils.Evaluate("f = Function('a', 'b', 'return a+b'); f(1, 3)"));

            // toString
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("function Function() { [native code] }", TestUtils.Evaluate("Function.toString()"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Function.length"));

            // no enumerable properties
            Assert.AreEqual("", TestUtils.Evaluate("y = ''; for (var x in Function) { y += x } y"));
            Assert.AreEqual("", TestUtils.Evaluate("y = ''; for (var x in new Function()) { y += x } y"));
        }

        [TestMethod]
        public void prototype()
        {
            // Prototype
            if (TestUtils.Engine != JSEngine.JScript)
            {
                // prototype of built-in object.
                Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("Function", "prototype"));

                // prototype of new function.
                Assert.AreEqual(PropertyAttributes.Writable, TestUtils.EvaluateAccessibility("new Function()", "prototype"));

                // prototype of empty function.
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Function).prototype === undefined"));

                // [[Prototype]]
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new Function()) === Function.prototype"));
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Function.prototype) === Object.prototype"));
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new Function().prototype) === Object.prototype"));
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf((function() {}).prototype) === Object.prototype"));
            }

            // prototype of built-in functions should be undefined.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("Math.sin.prototype"));

            // prototype of new functions should be a new object.
            Assert.AreEqual(true, TestUtils.Evaluate("Function('a+b').prototype !== Object.prototype"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof(Function('a+b').prototype)"));

            // The prototype property becomes the prototype of new objects (as long as the prototype property is an object).
            Assert.AreEqual(true, TestUtils.Evaluate("f = function() { }; x = {}; f.prototype = x; Object.getPrototypeOf(new f()) === x"));
            Assert.AreEqual(true, TestUtils.Evaluate("f = function() { }; x = 5; f.prototype = x; Object.getPrototypeOf(new f()) === Object.prototype"));
        }

        [TestMethod]
        public void constructor()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("new Function().constructor === Function"));
            Assert.AreEqual(true, TestUtils.Evaluate("f = Function('a+b'); f.prototype.constructor === f"));

            // constructor is non-enumerable, writable and configurable.
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("Function.prototype", "constructor"));
        }

        [TestMethod]
        public void length()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("Function.prototype.length"));
            Assert.AreEqual(1, TestUtils.Evaluate("new Function('a', 'return this / a').length"));
            Assert.AreEqual(2, TestUtils.Evaluate("new Function('a,b', 'return a / b').length"));
            Assert.AreEqual(2, TestUtils.Evaluate("(function(a, b) {}).length"));
            
            // length is non-enumerable, non-writable and non-configurable.
            if (TestUtils.Engine != JSEngine.JScript)
            {
                Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("Function", "length"));
                Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("new Function", "length"));
            }
        }

        [TestMethod]
        public void name()
        {
            Assert.AreEqual("f", TestUtils.Evaluate("function f() { } f.name"));
            Assert.AreEqual("g", TestUtils.Evaluate("f = function g() { }; f.name"));
            Assert.AreEqual("", TestUtils.Evaluate("f = function() { }; f.name"));
            Assert.AreEqual("f", TestUtils.Evaluate("x = { y: function f() { } }; x.y.name"));
            Assert.AreEqual("", TestUtils.Evaluate("x = { y: function() { } }; x.y.name"));
            Assert.AreEqual("f", TestUtils.Evaluate("x = { get f() { } }; Object.getOwnPropertyDescriptor(x, 'f').get.name"));
            Assert.AreEqual("anonymous", TestUtils.Evaluate("new Function('').name"));
        }

        [TestMethod]
        public void displayName()
        {
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("function f() { } f.displayName"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("f = function g() { }; f.displayName"));
            Assert.AreEqual("f", TestUtils.Evaluate("f = function() { }; f.displayName"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = { y: function f() { } }; x.y.displayName"));
            Assert.AreEqual("y", TestUtils.Evaluate("x = { y: function() { } }; x.y.displayName"));
            Assert.AreEqual("get f", TestUtils.Evaluate("x = { get f() { } }; Object.getOwnPropertyDescriptor(x, 'f').get.displayName"));
            Assert.AreEqual("set f", TestUtils.Evaluate("x = { set f(value) { } }; Object.getOwnPropertyDescriptor(x, 'f').set.displayName"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("new Function('').displayName"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("function anonymous(a, b) {\nreturn a + b\n}", TestUtils.Evaluate("new Function('a, b', 'return a + b').toString()"));
            Assert.AreEqual("function atan2() { [native code] }", TestUtils.Evaluate("Math.atan2.toString()"));
            Assert.AreEqual("function (a, b) {\n return a + b \n}", TestUtils.Evaluate("(function(a, b) { return a + b }).toString()"));
            Assert.AreEqual("function (a, b) {\n return a + 51 \n}", TestUtils.Evaluate("(function(a, b) { return a + 51 }).toString()"));
            Assert.AreEqual("function (a, b) {\n function inner() { return a + b } return inner() \n}",
                TestUtils.Evaluate("(function(a, b) { function inner() { return a + b } return inner() }).toString()"));
        }

        [TestMethod]
        public void apply()
        {
            Assert.AreEqual("[object Math]", TestUtils.Evaluate("({}.toString.apply(Math, []))"));
            Assert.AreEqual("onetwo", TestUtils.Evaluate("(String.prototype.concat).apply('one', ['two'])"));
            Assert.AreEqual(2, TestUtils.Evaluate("new Function('a', 'return this / a').apply(10, [5])"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("new Function('a', 'return a').apply(10, null)"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("new Function('a', 'return a').apply(10, undefined)"));
            Assert.AreEqual(5, TestUtils.Evaluate("new Function('a', 'return a').apply(10, {length: 1, 0: 5})"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("Function.prototype.apply.length"));

            // Errors
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Function('a', 'return a').apply(10, 2)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Function('a', 'return a').apply(10, {})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Function('a', 'return a').apply(10, {length: null})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Function('a', 'return a').apply(10, {length: undefined})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Function('a', 'return a').apply(10, {length: -1})"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Function('a', 'return a').apply(10, {length: 4294967296})"));
        }

        [TestMethod]
        public void call()
        {
            Assert.AreEqual("[object Math]", TestUtils.Evaluate("({}.toString.call(Math))"));
            Assert.AreEqual(2, TestUtils.Evaluate("new Function('a', 'return this / a').call(10, 5)"));
            Assert.AreEqual(true, TestUtils.Evaluate("new Function('return this').call() === this"));
            Assert.AreEqual("[object Undefined]", TestUtils.Evaluate("toString.call()"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Function.prototype.call.length"));
        }

        [TestMethod]
        public void bind()
        {
            Assert.AreEqual("[object Math]", TestUtils.Evaluate("var f = {}.toString.bind(Math); f();"));
            Assert.AreEqual(32, TestUtils.Evaluate("var f = Math.pow.bind(undefined, 2); f(5);"));
            Assert.AreEqual(5, TestUtils.Evaluate("new Function('a,b', 'return a / b').bind(undefined, 10)(2)"));
            Assert.AreEqual(15, TestUtils.Evaluate("new Function('a,b', 'return a + b').bind(undefined, 10, 5)(2)"));

            // length of bound functions is the number of arguments remaining.
            Assert.AreEqual(2, TestUtils.Evaluate("Math.pow.length"));
            Assert.AreEqual(2, TestUtils.Evaluate("var f = Math.pow.bind(undefined); f.length"));
            Assert.AreEqual(1, TestUtils.Evaluate("var f = Math.pow.bind(undefined, 2); f.length"));
            Assert.AreEqual(0, TestUtils.Evaluate("var f = Math.pow.bind(undefined, 2, 5); f.length"));
            Assert.AreEqual(0, TestUtils.Evaluate("var f = Math.pow.bind(undefined, 2, 5, 7); f.length"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Function.prototype.bind.length"));

            // Caller and arguments throw a TypeError exception.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType(@"
                function foo() { return bar.arguments; }
                var bar = foo.bind({});
                function baz() { return bar(); }
                baz();"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType(@"
                function foo() { return bar.caller; }
                var bar = foo.bind({});
                function baz() { return bar(); }
                baz();"));
        }
    }
}
