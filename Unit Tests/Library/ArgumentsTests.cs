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
    /// Test the Arguments object.
    /// </summary>
    [TestClass]
    public class ArgumentsTests
    {
        [TestMethod]
        public void Arguments_NonStrict()
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
        public void Arguments_StrictMode()
        {
            if (TestUtils.Engine == JSEngine.JScript)
                Assert.Fail("JScript does not support strict mode.");

            // In strict mode arguments variable is not writable and not configurable.
            Assert.AreEqual(false, TestUtils.EvaluateExceptionType("(function(a, b, c) { 'use strict'; arguments = 5; return arguments === 5 })(1, 2, 3)"));
            Assert.AreEqual(false, TestUtils.Evaluate("(function(a, b, c) { 'use strict'; return delete arguments })(1, 2, 3)"));

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
        public void Arguments_ToString()
        {
            Assert.AreEqual("[object Arguments]", TestUtils.Evaluate("(function() { return arguments.toString(); })()"));
        }
    }
}
