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
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Function).prototype === null"));
                Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("Object.getPrototypeOf(Function)", "prototype"));

                // [[Prototype]]
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new Function()) === Function.prototype"));
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Function.prototype) === Object.prototype"));
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new Function().prototype) === Object.prototype"));
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf((function() {}).prototype) === Object.prototype"));
            }

            // prototype of built-in function.
            Assert.AreEqual(true, TestUtils.Evaluate("Math.sin.prototype !== Math.cos.prototype"));
            Assert.AreEqual(true, TestUtils.Evaluate("Math.toString.prototype !== Object.prototype"));
            Assert.AreEqual(true, TestUtils.Evaluate("Function('a+b').prototype !== Object.prototype"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof(Function('a+b').prototype)"));
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
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("function anonymous(a, b) {\nreturn a + b\n}", TestUtils.Evaluate("new Function('a, b', 'return a + b').toString()"));
            Assert.AreEqual("function atan2() { [native code] }", TestUtils.Evaluate("Math.atan2.toString()"));
            Assert.AreEqual("function(a, b) { return a + b }", TestUtils.Evaluate("(function(a, b) { return a + b }).toString()"));
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
            Assert.AreEqual("[object undefined]", TestUtils.Evaluate("toString.call()"));

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
        }

        [TestMethod]
        public void Arguments()
        {
            // Arguments variable is writable but not configurable.
            Assert.AreEqual(5, TestUtils.Evaluate("(function(a, b, c) { arguments = 5; return arguments })(1, 2, 3)"));
            Assert.AreEqual(false, TestUtils.Evaluate("(function(a, b, c) { return delete arguments })(1, 2, 3)"));

            // Mapping between arguments array and function parameters.
            Assert.AreEqual(5, TestUtils.Evaluate("(function(a, b, c) { a = 5; return arguments[0] })(1, 2, 3)"));
            Assert.AreEqual(5, TestUtils.Evaluate("(function(a, b, c) { arguments[0] = 5; return a })(1, 2, 3)"));

            // Duplicate argument names are not mapped (since there is nothing to map to).
            Assert.AreEqual(2, TestUtils.Evaluate("(function(a, a) { return a; })(1, 2)"));
            Assert.AreEqual(1, TestUtils.Evaluate("(function(a, a) { return arguments[0]; })(1, 2)"));
            Assert.AreEqual(1, TestUtils.Evaluate("(function(a, a) { a = 5; return arguments[0]; })(1, 2)"));

            // If the array index is outside the number of parameters then it behaves as per a normal object.
            Assert.AreEqual(6, TestUtils.Evaluate("(function(a, b, c) { arguments[3] = 6; return arguments[3] })(1, 2, 3)"));
            Assert.AreEqual(6, TestUtils.Evaluate("(function(a, b, c) { arguments.test = 6; return arguments.test })(1, 2, 3)"));

            // The "length" property contains the number of arguments passed to the function.
            // Note: unlike an array, the length property does not update.
            Assert.AreEqual(3, TestUtils.Evaluate("(function(a, b, c) { return arguments.length })(1, 2, 3)"));
            Assert.AreEqual(2, TestUtils.Evaluate("(function(a, b, c) { return arguments.length })(1, 2)"));
            Assert.AreEqual(2, TestUtils.Evaluate("(function(a, b, c) { arguments[9] = 6; return arguments.length })(1, 2)"));

            // Mapping between arguments and parameters is broken after delete.
            Assert.AreEqual(1, TestUtils.Evaluate("(function(a, b, c) { delete arguments[0]; return a })(1, 2, 3)"));
            Assert.AreEqual(1, TestUtils.Evaluate("(function(a, b, c) { delete arguments[0]; arguments[0] = 9; return a })(1, 2, 3)"));

            // However, deleting the parameter doesn't break the mapping.
            //if (TestUtils.Engine != JSEngine.JScript)   // JScript bug?
            //{
                Assert.AreEqual(1, TestUtils.Evaluate("(function(a, b, c) { delete a; return arguments[0] })(1, 2, 3)"));
                Assert.AreEqual(1, TestUtils.Evaluate("a = 5; (function(a, b, c) { delete a; return arguments[0] })(1, 2, 3)"));
            //}

            // The callee property is the function that is associated with the arguments object.
            Assert.AreEqual(true, TestUtils.Evaluate("f = function(a, b, c) { return arguments.callee }; f() === f"));

            // If one of the parameters is "arguments" then the arguments object is never created.
            Assert.AreEqual(5, TestUtils.Evaluate("(function(arguments) { return arguments; })(5)"));

            // The argument array indices are enumerable.
            Assert.AreEqual("01", TestUtils.Evaluate("(function(a, b, c) { var str = ''; for (var key in arguments) str += key; return str; })(1, 2)"));
            Assert.AreEqual("012", TestUtils.Evaluate("(function(a, b, c) { arguments[1] = 3; arguments[2] = 4; var str = ''; for (var key in arguments) str += key; return str; })(1, 2)"));
        }

        [TestMethod]
        public void ArgumentsStrict()
        {
            if (TestUtils.Engine == JSEngine.JScript)
                Assert.Fail("JScript does not support strict mode.");

            // In strict mode arguments variable is not writable and not configurable.
            Assert.AreEqual(false, TestUtils.EvaluateExceptionType("(function(a, b, c) { arguments = 5; return arguments === 5 })(1, 2, 3)"));
            Assert.AreEqual(false, TestUtils.Evaluate("(function(a, b, c) { return delete arguments })(1, 2, 3)"));

            // In strict mode callee and caller throw TypeErrors on access.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; (function(a, b, c) { return arguments.callee })(1, 2, 3)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; (function(a, b, c) { arguments.callee = 5 })(1, 2, 3)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; (function(a, b, c) { return arguments.caller })(1, 2, 3)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; (function(a, b, c) { arguments.caller = 5 })(1, 2, 3)"));

            // Arguments cannot be redefined.
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("'use strict'; (function(arg) { arguments = 5; })()"));

            // Arguments and caller don't exist outside the function.
            Assert.AreEqual("undefined", TestUtils.EvaluateExceptionType("'use strict'; function test(){ function inner(){ return typeof(test.arguments); } return inner(); } test()"));
            Assert.AreEqual("undefined", TestUtils.EvaluateExceptionType("'use strict'; function test(){ function inner(){ return typeof(inner.caller); } return inner(); } test()"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("'use strict'; function test(){ function inner(){ test.arguments = 5; } return inner(); } test()"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("'use strict'; function test(){ function inner(){ inner.caller = 5; } return inner(); } test()"));
        }

        [TestMethod]
        public void Arguments_toString()
        {
            Assert.AreEqual("[object Arguments]", TestUtils.Evaluate("(function() { return arguments.toString(); })()"));
        }
    }
}
