using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Compiler;

namespace UnitTests
{
    /// <summary>
    /// Language statement tests.
    /// </summary>
    [TestClass]
    public class StatementTests
    {
        
        [TestMethod]
        public void If()
        {
            Assert.AreEqual(5, TestUtils.Evaluate("if (true) 5"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("if (false) 5"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("if (true) 5 else 6"));
            Assert.AreEqual(5, TestUtils.Evaluate("if (true) 5; else 6"));
            Assert.AreEqual(6, TestUtils.Evaluate("if (false) 5; else 6"));

            // Nested if statements.
            Assert.AreEqual(1, TestUtils.Evaluate("if (true) if (true) 1; else 2; else 3"));
            Assert.AreEqual(2, TestUtils.Evaluate("if (true) if (false) 1; else 2; else 3"));
            Assert.AreEqual(3, TestUtils.Evaluate("if (false) if (true) 1; else 2; else 3"));
            Assert.AreEqual(3, TestUtils.Evaluate("if (false) if (false) 1; else 2; else 3"));
        }

        [TestMethod]
        public void Empty()
        {
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate(""));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate(";"));
        }

        [TestMethod]
        public void Do()
        {
            Assert.AreEqual(7, TestUtils.Evaluate("x = 1; do { x = x + 3 } while (x < 5); x"));
            Assert.AreEqual(9, TestUtils.Evaluate("x = 6; do { x = x + 3 } while (x < 5); x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 1; do { x = x + 1 } while (x < 5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("5; do { } while(false)"));
        }

        [TestMethod]
        public void While()
        {
            Assert.AreEqual(7, TestUtils.Evaluate("x = 1; while (x < 5) { x = x + 3 } x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 6; while (x < 5) { x = x + 3 } x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 6; while (x < 5) { x = x + 3; x = x + 1 } x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 6; while (x < 5) { } x"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = 6; while () { x = x + 3 } x"));
            Assert.AreEqual(7, TestUtils.Evaluate("x = 1; while (x < 5) { x = x + 3 }"));
        }

        [TestMethod]
        public void For()
        {
            // for (<initial>; <test>; <increment>)
            Assert.AreEqual(11, TestUtils.Evaluate("y = 1; for (x = 1; x < 5; x ++) { y = y + x } y"));
            Assert.AreEqual(100, TestUtils.Evaluate("for (;;) { y = 100; break; } y"));

            // for (var x = <initial>; <test>; <increment>)
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; for (var x; x < 5; x ++) { }"));
            Assert.AreEqual(11, TestUtils.Evaluate("y = 1; for (var x = 1; x < 5; x ++) { y = y + x } y"));
            Assert.AreEqual(11, TestUtils.Evaluate("for (var x = 1, y = 1; x < 5; x ++) { y = y + x } y"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("for (var x + 1; x < 5; x ++) { }"));

            // for (x in <expression>)
            Assert.AreEqual("ab", TestUtils.Evaluate("y = ''; for (x in {a: 1, b: 2}) { y += x } y"));
            Assert.AreEqual("1", TestUtils.Evaluate("y = 0; for (x in [7, 5]) { y = x } y"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; for (x in null) { x = 1 } x"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; for (x in undefined) { x = 1 } x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("for (5 in [1, 2]) {}"));
            Assert.AreEqual("atest", TestUtils.Evaluate("Object.prototype.test = 19; y = ''; for (x in {a: 5}) { y += x } delete Object.prototype.test; y"));

            // for (var x in <expression>)
            Assert.AreEqual("1", TestUtils.Evaluate("y = 0; for (var x in [7, 5]) { y = x } y"));
            Assert.AreEqual("01234", TestUtils.Evaluate("y = ''; for (var x in 'hello') { y += x } y"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("for (var 5 in [1, 2])"));

            // Modify an object while enumerating it should return the keys as they were originally.
            Assert.AreEqual("bc", TestUtils.Evaluate("var a = {b: 2, c: 3}; var keys = ''; for (var x in a) { a.d = 5; keys += x; }"));

            // Strict mode: the name "eval" is not allowed in strict mode.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; for (var eval in {a:1}) {}"));
        }

        [TestMethod]
        public void Continue()
        {
            // continue
            Assert.AreEqual(1, TestUtils.Evaluate("x = y = 1; while(x < 5) { x ++; continue; y ++ } y"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 1; do { x ++; continue; x += 10 } while(false); x"));
            Assert.AreEqual(5, TestUtils.Evaluate("for(x = 1; x < 5; x ++) { continue; x += 10 } x"));

            // continue [label]
            Assert.AreEqual(1, TestUtils.Evaluate("x = y = 1; test: while(x < 5) { x ++; continue test; y ++ } y"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 0; test: do { x ++; while(x < 5) { x ++; continue test; x += 10 } x += 20 } while(false); x"));

            // The label must be an enclosing *iteration* statement in the same function.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = 1; test: continue test; x"));
        }

        [TestMethod]
        public void Break()
        {
            // break
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; while(x < 5) { break; x ++ } x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; do { break; x = 5 } while(false); x"));
            Assert.AreEqual(1, TestUtils.Evaluate("for(x = 1; x < 5; x ++) { break; } x"));

            // break [label]
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; test: do { break test; x = 5 } while(false); x"));
            Assert.AreEqual(3, TestUtils.Evaluate("x = 1; test: do { x = 2; do { x = 3; break test; x = 4 } while(false); x = 5 } while(false); x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; test: break test; x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; test: { break test; x = 5 } x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 6; test: with (x) { break test; 5 }"));

            // Duplicate nested labels are not allowed.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("label: { } label: { }"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("label: { label: { } }"));

            // The label must be an enclosing statement in the same function.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = 1; test: x ++; break test; x")); // Not an enclosing statement.
        }

        [TestMethod]
        public void Var()
        {
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("var x"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("var x; x"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("var x, y"));
            Assert.AreEqual(5, TestUtils.Evaluate("var x = 5; x"));
            Assert.AreEqual(6, TestUtils.Evaluate("var x, y = 6; y"));
            Assert.AreEqual(1, TestUtils.Evaluate("var x = 1, y = 2; x"));
            Assert.AreEqual(2, TestUtils.Evaluate("var x = 1, y = 2; y"));
            Assert.AreEqual(2, TestUtils.Evaluate("var x = Math.max(1, 2); x"));

            // Strict mode: the name "eval" is not allowed in strict mode.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; var eval = 5"));
        }

        [TestMethod]
        public void Return()
        {
            Assert.AreEqual(5, TestUtils.Evaluate("function f() { return 5 } f()"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("function f() { } f()"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("function f() { return } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("return 5"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("eval('return 5')"));
        }

        [TestMethod]
        public void With()
        {
            Assert.AreEqual(234, TestUtils.Evaluate("x = { a: 234 }; with (x) { a }"));
            Assert.AreEqual(234, TestUtils.Evaluate("x = { a: 234 }; a = 5; with (x) { a }"));
            Assert.AreEqual(15, TestUtils.Evaluate("x = { a: 234 }; b = 15; with (x) { b }"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } a"));
            Assert.AreEqual(13, TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } b"));
            Assert.AreEqual(12, TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } x.a"));
            Assert.AreEqual(6, TestUtils.Evaluate("b = 5; with (x) { y = b; x.b = 6; b; }"));
            Assert.AreEqual(3, TestUtils.Evaluate("x = Object.create({b: 3}); b = 2; with (x) { b }"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } x.b"));
            Assert.AreEqual("number", TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, typeof(b) }"));

            // Implicit this.
            Assert.AreEqual(1970, TestUtils.Evaluate("x = new Date(5); x.f = x.getFullYear; with (x) { f() }"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = { a: 1, b: 2 }; with (x) { (function() { return this })() === this }"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("x = new Date(5); f = x.getFullYear; with (x) { f() }"));
            Assert.AreEqual(1970, TestUtils.Evaluate("x = new Date(5); x.f = x.getFullYear; with (x) { (function b() { return f() })() }"));

            // With and var.
            Assert.AreEqual(5, TestUtils.Evaluate("x = { a: 234 }; with (x) { var a = 5; } x.a"));
            Assert.AreEqual(0, TestUtils.Evaluate("a = 0; x = { a: 234 }; with (x) { var a = 5; } a"));
            Assert.AreEqual(5, TestUtils.Evaluate("b = 0; x = { a: 234 }; with (x) { var b = 5; } b"));
            Assert.AreEqual(4, TestUtils.Evaluate("foo = {x: 4}; with (foo) { var x; x }"));

            // With and prototype chains.
            Assert.AreEqual(10, TestUtils.Evaluate("x = Object.create({ b: 5 }); with (x) { b = 10 } x.b"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = Object.create({ b: 5 }); with (x) { b = 10 } Object.getPrototypeOf(x).b"));

            // With inside a function.
            Assert.AreEqual(1, TestUtils.Evaluate("function foo() { with ({ a: 1 }) { return a; } } foo()"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = { a: 1 }; function foo() { with (x) { return a; } } foo()"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = { a: 1 }; function foo() { with (x) { a = 2; } } foo(); x.a"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = { a: 1 }; var foo = function() { with (x) { a = 2; } }; foo(); x.a"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = { a: 1 }; y = 2; function foo() { with (x) { return y; } } foo()"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = { a: 1 }; y = 2; function foo() { with (x) { y = a; } } foo(); y"));
            Assert.AreEqual(1, TestUtils.Evaluate(@"var x = { eval: 1 }; var f = function(){ with(x){ return st_eval = eval; } }; f();"));
  
            // With statements are syntax errors in strict mode.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; with (x) { }"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"eval(""'use strict'; var o = {}; with (o) {}"")"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"'use strict'; eval(""var o = {}; with (o) {}"")"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"eval(""function f() { 'use strict'; var o = {}; with (o) {} }"")"));
        }

        [TestMethod]
        public void Switch()
        {
            Assert.AreEqual(5, TestUtils.Evaluate("x = 5; switch (x) { }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x ++) { } x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { case 5: 6 }"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 5; switch (x) { case 4: 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { default: 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { case 4: case 5: 6 }"));
            Assert.AreEqual(7, TestUtils.Evaluate("x = 5; switch (x) { case 5: 6; case 6: 7; }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { case 5: 6; break; 7 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { case 5: 6; break; case 6: 7 }"));
            Assert.AreEqual(8, TestUtils.Evaluate("x = 5; switch (x) { case 4: 6; case 5: 7; default: 8 }"));
            Assert.AreEqual(1, TestUtils.Evaluate("switch (5) { default: 3; case 4: 1 }"));
            Assert.AreEqual(5, TestUtils.Evaluate("(function(x) { switch (x) { case 8: return 4; case 9: return 5; default: return 7 } })(9)"));
        }

        [TestMethod]
        public void Throw()
        {
            Assert.AreEqual("Error", TestUtils.EvaluateExceptionType("throw new Error('test')"));
        }

        [TestMethod]
        public void TryCatchFinally()
        {
            // Try to catch various values.
            Assert.AreEqual(5, TestUtils.Evaluate("try { throw 5 } catch (e) { e }"));
            Assert.AreEqual("test", TestUtils.Evaluate("try { throw 'test' } catch (e) { e }"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("try { throw undefined } catch (e) { e }"));
            Assert.AreEqual("test", TestUtils.Evaluate("try { throw new String('test') } catch (e) { e.valueOf() }"));

            // Test the finally block runs in all circumstances.
            Assert.AreEqual(6, TestUtils.Evaluate("try { } finally { 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("try { } catch (e) { e } finally { 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("try { throw 5 } catch (e) { e } finally { 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("try { try { throw 5 } finally { 6 } } catch (e) { }"));

            // Exceptions can be swallowed using finally.
            Assert.AreEqual(6, TestUtils.Evaluate("function f() { try { throw 5 } finally { return 6 } } f()"));

            // Catch creates a new scope - but only for the exception variable.
            Assert.AreEqual(5, TestUtils.Evaluate("e = 5; try { throw 6; } catch (e) { } e"));
            Assert.AreEqual(5, TestUtils.Evaluate("e = 5; try { throw 6; } catch (e) { var e = 10; } e"));
            Assert.AreEqual(5, TestUtils.Evaluate("var b = 2; try { throw 6; } catch (e) { var b = 5; } b"));

            // Try without catch or finally is an error.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("try { }"));

            // Cannot declare a function inside a catch block.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("try { throw 6; } catch (e) { function foo() { return 2; } foo() }"));
        }

        [TestMethod]
        public void Function()
        {
            Assert.AreEqual(6, TestUtils.Evaluate("function f(a, b, c) { return a + b + c; } f(1, 2, 3)"));

            // Multiple variable definitions.
            Assert.AreEqual(5, TestUtils.Evaluate("var a = 5; function a() { return 6 }; a"));
            Assert.AreEqual(5, TestUtils.Evaluate("function a() { return 6 }; var a = 5; a"));
            Assert.IsInstanceOfType(TestUtils.Evaluate("var a; function a() { return 6 }; a"), typeof(Jurassic.Library.FunctionInstance));
            Assert.IsInstanceOfType(TestUtils.Evaluate("function a() { return 6 }; var a; a"), typeof(Jurassic.Library.FunctionInstance));
            Assert.AreEqual(7, TestUtils.Evaluate("function a() { return 6 }; function a() { return 7 } a()"));
            Assert.AreEqual(4, TestUtils.Evaluate("a(); function a() { return 1 } function a() { return 2 } function a() { return 3 } function a() { return 4 }"));

            // Closures.
            Assert.AreEqual(7, TestUtils.Evaluate("function f(a, b, c) { return f2(a + 1); function f2(d) { return d + b + c; } } f(1, 2, 3)"));
            Assert.AreEqual(11, TestUtils.Evaluate("function makeAdder(a) { return function(b) { return a + b; } } x = makeAdder(5); x(6)"));
            Assert.AreEqual(27, TestUtils.Evaluate("function makeAdder(a) { return function(b) { return a + b; } } y = makeAdder(20); y(7)"));

            // Function expressions.
            Assert.AreEqual(3, TestUtils.Evaluate("var f = function () { return 3; }; f()"));

            // This
            Assert.AreEqual(1, TestUtils.Evaluate("var f = function () { return this.a; }; var x = {'a': 1, 'f': f}; x.f()"));

            // Function expressions are not visible in the local scope.
            Assert.AreEqual(1, TestUtils.Evaluate("function f() {return 1} x = function f() {return 2}; f()"));

            // The function name is defined in the function scope.
            Assert.AreEqual(24, TestUtils.Evaluate("var fact='test'; Math.factorial = function fact(n) {return n<=1?1:n*fact(n-1)}; Math.factorial(4)"));

            // Argument names override the function name and 'arguments'.
            Assert.AreEqual(5, TestUtils.Evaluate("(function test(test) { return test; })(5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("function test(test) { return test; } test(5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("(function f(arguments) { return arguments })(5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("(function f(a) { arguments = 5; return arguments })(5)"));
            Assert.AreEqual("[object Arguments]", TestUtils.Evaluate("function arguments() { return arguments } arguments().toString()"));

            // Strict mode: the name "eval" is not allowed in strict mode.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function eval(){}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function test(eval){}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function(eval){}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; new Function('eval', '')"));

            // Strict mode: argument names cannot be identical.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; (function(arg, arg) { })()"));
        }

    }
}
