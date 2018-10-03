using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Language statement tests.
    /// </summary>
    [TestClass]
    public class StatementTests : TestBase
    {
        [TestMethod]
        public void If()
        {
            Assert.AreEqual(5, Evaluate("if (true) 5"));
            Assert.AreEqual(Undefined.Value, Evaluate("if (false) 5"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("if (true) 5 else 6"));
            Assert.AreEqual(5, Evaluate("if (true) 5; else 6"));
            Assert.AreEqual(6, Evaluate("if (false) 5; else 6"));

            // Nested if statements.
            Assert.AreEqual(1, Evaluate("if (true) if (true) 1; else 2; else 3"));
            Assert.AreEqual(2, Evaluate("if (true) if (false) 1; else 2; else 3"));
            Assert.AreEqual(3, Evaluate("if (false) if (true) 1; else 2; else 3"));
            Assert.AreEqual(3, Evaluate("if (false) if (false) 1; else 2; else 3"));
        }

        [TestMethod]
        public void Empty()
        {
            Assert.AreEqual(Undefined.Value, Evaluate(""));
            Assert.AreEqual(Undefined.Value, Evaluate(";"));
        }

        [TestMethod]
        public void Do()
        {
            Assert.AreEqual(7, Evaluate("x = 1; do { x = x + 3 } while (x < 5); x"));
            Assert.AreEqual(9, Evaluate("x = 6; do { x = x + 3 } while (x < 5); x"));
            Assert.AreEqual(5, Evaluate("x = 1; do { x = x + 1 } while (x < 5)"));
            Assert.AreEqual(5, Evaluate("5; do { } while(false)"));
        }

        [TestMethod]
        public void While()
        {
            Assert.AreEqual(7, Evaluate("x = 1; while (x < 5) { x = x + 3 } x"));
            Assert.AreEqual(6, Evaluate("x = 6; while (x < 5) { x = x + 3 } x"));
            Assert.AreEqual(6, Evaluate("x = 6; while (x < 5) { x = x + 3; x = x + 1 } x"));
            Assert.AreEqual(6, Evaluate("x = 6; while (x < 5) { } x"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = 6; while () { x = x + 3 } x"));
            Assert.AreEqual(7, Evaluate("x = 1; while (x < 5) { x = x + 3 }"));
            Assert.AreEqual(",2,3,4,5,6,7,8", Evaluate(@"
                (function() {
                    var r = 8;
                    var count = Array(8);
                    while (r != 1) { count[r - 1] = r; r--; }
                    return count.toString();
                })();"));
            Assert.AreEqual(1, Evaluate(@"
                (function() {
                    var r = 8;
                    var count = Array(8);
                    while (r != 1) { count[r - 1] = r; r--; }
                    return r;
                })();"));
        }

        [TestMethod]
        public void For()
        {
            // for (<initial>; <test>; <increment>)
            Assert.AreEqual(11, Evaluate("y = 1; for (x = 1; x < 5; x ++) { y = y + x } y"));
            Assert.AreEqual(100, Evaluate("for (;;) { y = 100; break; } y"));

            // for (var x = <initial>; <test>; <increment>)
            Assert.AreEqual(0, Evaluate("x = 0; for (var x; x < 5; x ++) { }"));
            Assert.AreEqual(11, Evaluate("y = 1; for (var x = 1; x < 5; x ++) { y = y + x } y"));
            Assert.AreEqual(11, Evaluate("for (var x = 1, y = 1; x < 5; x ++) { y = y + x } y"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("for (var x + 1; x < 5; x ++) { }"));

            // Strict mode.
            Assert.AreEqual(45, Evaluate("'use strict'; var y = 0; for (var x = 0; x < 10; x ++) { y += x; } y"));
            Execute("'use strict'; var y = 0; for (var x = 0; x < 10; x ++) { y += x; }");
            Assert.AreEqual(45, Evaluate("y"));
        }

        [TestMethod]
        public void ForIn()
        {
            // for (x in <expression>)
            Assert.AreEqual("ab", Evaluate("y = ''; for (x in {a: 1, b: 2}) { y += x } y"));
            Assert.AreEqual("1", Evaluate("y = 0; for (x in [7, 5]) { y = x } y"));
            Assert.AreEqual(0, Evaluate("x = 0; for (x in null) { x = 1 } x"));
            Assert.AreEqual(0, Evaluate("x = 0; for (x in undefined) { x = 1 } x"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("for (5 in [1, 2]) {}"));

            // for (var x in <expression>)
            Assert.AreEqual("1", Evaluate("y = 0; for (var x in [7, 5]) { y = x } y"));
            Assert.AreEqual("01234", Evaluate("y = ''; for (var x in 'hello') { y += x } y"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("for (var 5 in [1, 2])"));

            // All properties in the prototype chain should be enumerated, but the same property
            // name is never enumerated twice.  Properties in the prototype chain with the same
            // name are shadowed and therefore are not enumerated.
            Assert.AreEqual("b,c", Evaluate("y = []; for (var x in Object.create({a:1, b:2, c:3}, {a:{value:4}, b:{value:5, enumerable:true}})) { y.push(x) } y.toString()"));

            // Properties are not enumerated if they have been deleted.
            Assert.AreEqual("a,b", Evaluate("y = []; z = {a:1, b:2, c:3}; for (var x in z) { y.push(x); delete z.c; } y.toString()"));

            // Adding a property while enumerating it should return the keys as they were originally.
            Assert.AreEqual("bc", Evaluate("var a = {b: 2, c: 3}; var keys = ''; for (var x in a) { a.d = 5; keys += x; }"));

            // Accessor properties should not be evaluated.
            Assert.AreEqual("a", Evaluate(@"
                var y = '';
                var x = { get a() { y += '!'; return '*'; } };
                for (var prop in x) {
                    y += prop
                }
                y;"));

            // Strict mode: the name "eval" is not allowed in strict mode.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; for (var eval in {a:1}) {}"));
        }

        [TestMethod]
        public void ForOf()
        {
            // for (x of <expression>)
            Assert.AreEqual(206, Evaluate("y = 0; for (x of [93, 113]) { y += x } y"));
            Assert.AreEqual("93, 113", Evaluate("y = ''; for (x of [93, 113]) { y += (y ? ', ' : '') + x } y"));
            Assert.AreEqual("b, o, o", Evaluate("y = ''; for (x of 'boo') { y += (y ? ', ' : '') + x } y"));
            Assert.AreEqual("0, 255", Evaluate("y = ''; for (x of new Uint8Array([0x00, 0xff])) { y += (y ? ', ' : '') + x } y"));
            Assert.AreEqual("a,1, b,2, c,3", Evaluate("y = ''; for (x of new Map([['a', 1], ['b', 2], ['c', 3]])) { y += (y ? ', ' : '') + x } y"));
            Assert.AreEqual("1, 2, 3", Evaluate("y = ''; for (x of new Set([1, 1, 2, 2, 3, 3])) { y += (y ? ', ' : '') + x } y"));

            // for (var x of <expression>)
            Assert.AreEqual(206, Evaluate("y = 0; for (var x of [93, 113]) { y += x } y"));

            // Type errors.
            Assert.AreEqual("TypeError", EvaluateExceptionType("for (x of 1) {}"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("for (x of null) {}"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("for (x of undefined) {}"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("for (x of {}) {}"));

            // Syntax errors.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("for (x of [1, 2], [3, 4]) {}"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("for (5 of [1, 2])"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("for (var 5 of [1, 2])"));

            // TODO: iterate over a generator.
        }

        [TestMethod]
        public void Continue()
        {
            // continue
            Assert.AreEqual(1, Evaluate("x = y = 1; while(x < 5) { x ++; continue; y ++ } y"));
            Assert.AreEqual(2, Evaluate("x = 1; do { x ++; continue; x += 10 } while(false); x"));
            Assert.AreEqual(5, Evaluate("for(x = 1; x < 5; x ++) { continue; x += 10 } x"));

            // continue [label]
            Assert.AreEqual(1, Evaluate("x = y = 1; test: while(x < 5) { x ++; continue test; y ++ } y"));
            Assert.AreEqual(2, Evaluate("x = 0; test: do { x ++; while(x < 5) { x ++; continue test; x += 10 } x += 20 } while(false); x"));

            // The label must be an enclosing *iteration* statement in the same function.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = 1; test: continue test; x"));
        }

        [TestMethod]
        public void Break()
        {
            // break
            Assert.AreEqual(1, Evaluate("x = 1; while(x < 5) { break; x ++ } x"));
            Assert.AreEqual(1, Evaluate("x = 1; do { break; x = 5 } while(false); x"));
            Assert.AreEqual(1, Evaluate("for(x = 1; x < 5; x ++) { break; } x"));

            // break [label]
            Assert.AreEqual(1, Evaluate("x = 1; test: do { break test; x = 5 } while(false); x"));
            Assert.AreEqual(3, Evaluate("x = 1; test: do { x = 2; do { x = 3; break test; x = 4 } while(false); x = 5 } while(false); x"));
            Assert.AreEqual(1, Evaluate("x = 1; test: break test; x"));
            Assert.AreEqual(1, Evaluate("x = 1; test: { break test; x = 5 } x"));
            Assert.AreEqual(6, Evaluate("x = 6; test: with (x) { break test; 5 }"));
            Assert.AreEqual(6, Evaluate("x = 6; test: with (x) { break test; 5 }"));
            Assert.AreEqual(1, Evaluate("var j = 0; lbl: for(var i in {p1: 1, p2: 1}){ j ++; break lbl; } j"));

            // Test in the presence of type inference.
            Assert.AreEqual(7, Evaluate(@"
                (function() {
                    var r = 8;
                    while (r != 1) { r--; break; }
                    return r;
                })();"));
            Assert.AreEqual(1, Evaluate(@"
                (function() {
                    var r = 8;
                    var count = Array(8);
                    while (true) {
                        while (r != 1) { count[r - 1] = r; r--; }
                        break;
                    }
                    return r;
                })();"));

            // Duplicate nested labels are not allowed.
            Assert.AreEqual(Undefined.Value, Evaluate("label: { } label: { }"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("label: { label: { } }"));

            // The label must be an enclosing statement in the same function.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = 1; test: x ++; break test; x")); // Not an enclosing statement.
        }

        [TestMethod]
        public void Var()
        {
            Assert.AreEqual(Undefined.Value, Evaluate("var x"));
            Assert.AreEqual(Undefined.Value, Evaluate("var x; x"));
            Assert.AreEqual(Undefined.Value, Evaluate("var x, y"));
            Assert.AreEqual(5, Evaluate("var x = 5; x"));
            Assert.AreEqual(6, Evaluate("var x, y = 6; y"));
            Assert.AreEqual(1, Evaluate("var x = 1, y = 2; x"));
            Assert.AreEqual(2, Evaluate("var x = 1, y = 2; y"));
            Assert.AreEqual(2, Evaluate("var x = Math.max(1, 2); x"));
            Assert.AreEqual(2, Evaluate("(function() { for (var i = 0; i < 2; i ++) { } return i; })();"));
            Assert.AreEqual("undefined", Evaluate("delete i; (function() { i = 5; var i = 3; })(); typeof(i);"));

            // Duplicate names are allowed.
            Assert.AreEqual(5, Evaluate("var x = 3, x = 5; x"));
            Assert.AreEqual(5, Evaluate("var x = 3; var x = 5; x"));
            Assert.AreEqual(5, Evaluate("'use strict'; var x = 3, x = 5; x"));

            // Strict mode: the name "eval" is not allowed in strict mode.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; var eval = 5"));
        }

        [TestMethod]
        [Ignore]    // not supported yet
        public void Let()
        {
            Assert.AreEqual(Undefined.Value, Evaluate("let x"));
            Assert.AreEqual(Undefined.Value, Evaluate("let x; x"));
            Assert.AreEqual(Undefined.Value, Evaluate("let x, y"));
            Assert.AreEqual(5, Evaluate("let x = 5; x"));
            Assert.AreEqual(6, Evaluate("let x, y = 6; y"));
            Assert.AreEqual(1, Evaluate("let x = 1, y = 2; x"));
            Assert.AreEqual(2, Evaluate("let x = 1, y = 2; y"));
            Assert.AreEqual(2, Evaluate("let x = Math.max(1, 2); x"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("(function() { for (let i = 0; i < 2; i ++) { } return i; })();"));
            Assert.AreEqual("undefined", Evaluate("delete i; (function() { i = 5; var i = 3; })(); typeof(i);"));

            // Duplicate names are not allowed.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("let x = 3, x = 5; x"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("let x = 3; let x = 5; x"));
        }

        [TestMethod]
        public void Return()
        {
            Assert.AreEqual(10, Evaluate("function f() { for (var i = 0; i < 1; i++) { return 10; } return 15; } f()"));
            Assert.AreEqual(5, Evaluate("function f() { return 5 } f()"));
            Assert.AreEqual(Undefined.Value, Evaluate("function f() { } f()"));
            Assert.AreEqual(Undefined.Value, Evaluate("function f() { return } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("return 5"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("eval('return 5')"));
        }

        [TestMethod]
        public void With()
        {
            Assert.AreEqual(234, Evaluate("x = { a: 234 }; with (x) { a }"));
            Assert.AreEqual(234, Evaluate("x = { a: 234 }; a = 5; with (x) { a }"));
            Assert.AreEqual(15, Evaluate("x = { a: 234 }; b = 15; with (x) { b }"));
            Assert.AreEqual(1, Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } a"));
            Assert.AreEqual(13, Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } b"));
            Assert.AreEqual(12, Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } x.a"));
            Assert.AreEqual(6, Evaluate("b = 5; with (x) { y = b; x.b = 6; b; }"));
            Assert.AreEqual(3, Evaluate("x = Object.create({b: 3}); b = 2; with (x) { b }"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } x.b"));
            Assert.AreEqual("number", Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, typeof(b) }"));

            // Implicit this.
            Assert.AreEqual(1970, Evaluate("x = new Date(86400000); x.f = x.getFullYear; with (x) { f() }"));
            Assert.AreEqual(true, Evaluate("x = { a: 1, b: 2 }; with (x) { (function() { return this })() === this }"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("x = new Date(5); f = x.getFullYear; with (x) { f() }"));
            Assert.AreEqual(1970, Evaluate("x = new Date(86400000); x.f = x.getFullYear; with (x) { (function b() { return f() })() }"));

            // With and var.
            Assert.AreEqual(5, Evaluate("x = { a: 234 }; with (x) { var a = 5; } x.a"));
            Assert.AreEqual(0, Evaluate("a = 0; x = { a: 234 }; with (x) { var a = 5; } a"));
            Assert.AreEqual(5, Evaluate("b = 0; x = { a: 234 }; with (x) { var b = 5; } b"));
            Assert.AreEqual(4, Evaluate("foo = {x: 4}; with (foo) { var x; x }"));
            Assert.AreEqual(1123, Evaluate("with ({}) { var with_unique_1 = 1123; } with_unique_1"));

            // With and prototype chains.
            Assert.AreEqual(10, Evaluate("x = Object.create({ b: 5 }); with (x) { b = 10 } x.b"));
            Assert.AreEqual(5, Evaluate("x = Object.create({ b: 5 }); with (x) { b = 10 } Object.getPrototypeOf(x).b"));

            // With inside a function.
            Assert.AreEqual(1, Evaluate("function foo() { with ({ a: 1 }) { return a; } } foo()"));
            Assert.AreEqual(1, Evaluate("x = { a: 1 }; function foo() { with (x) { return a; } } foo()"));
            Assert.AreEqual(2, Evaluate("x = { a: 1 }; function foo() { with (x) { a = 2; } } foo(); x.a"));
            Assert.AreEqual(2, Evaluate("x = { a: 1 }; var foo = function() { with (x) { a = 2; } }; foo(); x.a"));
            Assert.AreEqual(2, Evaluate("x = { a: 1 }; y = 2; function foo() { with (x) { return y; } } foo()"));
            Assert.AreEqual(1, Evaluate("x = { a: 1 }; y = 2; function foo() { with (x) { y = a; } } foo(); y"));
            Assert.AreEqual(1, Evaluate(@"var x = { eval: 1 }; var f = function(){ with(x){ return st_eval = eval; } }; f();"));

            // With and object literals.
            Assert.AreEqual(42, Evaluate("delete a; x = { a: 42 }; with (x) { y = { get z() { return a; }} } y.z"));

            // With and function declarations.
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("delete a; x = { a: 43 }; with (x) { function y() { return a } } y()"));
            Assert.AreEqual("function", Evaluate("result = typeof _f; with ({a: 2}) { function _f() { return 5 } } result"));
  
            // With statements are syntax errors in strict mode.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; var x = {}; with (x) { }"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"eval(""'use strict'; var o = {}; with (o) {}"")"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"'use strict'; eval(""var o = {}; with (o) {}"")"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"eval(""function f() { 'use strict'; var o = {}; with (o) {} }"")"));
        }

        [TestMethod]
        public void Switch()
        {
            Assert.AreEqual(5, Evaluate("x = 5; switch (x) { }"));
            Assert.AreEqual(6, Evaluate("x = 5; switch (x) { case 5: 6 }"));
            Assert.AreEqual(5, Evaluate("x = 5; switch (x) { case 4: 6 }"));
            Assert.AreEqual(6, Evaluate("x = 5; switch (x) { default: 6 }"));
            Assert.AreEqual(6, Evaluate("x = 5; switch (x) { case 4: case 5: 6 }"));
            Assert.AreEqual(7, Evaluate("x = 5; switch (x) { case 5: 6; case 6: 7; }"));
            Assert.AreEqual(6, Evaluate("x = 5; switch (x) { case 5: 6; break; 7 }"));
            Assert.AreEqual(6, Evaluate("x = 5; switch (x) { case 5: 6; break; case 6: 7 }"));
            Assert.AreEqual(8, Evaluate("x = 5; switch (x) { case 4: 6; case 5: 7; default: 8 }"));
            Assert.AreEqual(1, Evaluate("switch (5) { default: 3; case 4: 1 }"));
            Assert.AreEqual(5, Evaluate("(function(x) { switch (x) { case 8: return 4; case 9: return 5; default: return 7 } })(9)"));

            // If there identical clauses, pick the first that matches.
            Assert.AreEqual(1, Evaluate("x = 5; switch (x) { case 5: 1; break; case 5: 2; }"));

            // Switch expression is evaluated first, then all the clauses.
            Assert.AreEqual(6, Evaluate("x = 5; switch (x ++) { } x"));
            Assert.AreEqual(4, Evaluate("x = 0; switch (x = 1) { case x = 2: x = 3; break; case x = 4: x = 5; } x"));
            Assert.AreEqual(2, Evaluate("x = 0; switch (x = 1, 2) { case x = 2: break; case x = 4: x = 5; } x"));
            Assert.AreEqual(3, Evaluate("x = 0; switch (x = 1, 2) { case x = 2: x = 3; break; case x = 4: x = 5; } x"));

            // Multiple default clauses are not allowed.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = 5; switch (x) { default: 6; default: 7 }"));
        }

        [TestMethod]
        public void Throw()
        {
            Assert.AreEqual("Error", EvaluateExceptionType("throw new Error('test')"));
        }

        [TestMethod]
        public void TryCatchFinally()
        {
            // Try to catch various values.
            Assert.AreEqual(5, Evaluate("try { throw 5 } catch (e) { e }"));
            Assert.AreEqual("test", Evaluate("try { throw 'test' } catch (e) { e }"));
            Assert.AreEqual(Undefined.Value, Evaluate("try { throw undefined } catch (e) { e }"));
            Assert.AreEqual("test", Evaluate("try { throw new String('test') } catch (e) { e.valueOf() }"));

            // Test the finally block runs in all circumstances.
            Assert.AreEqual(6, Evaluate("try { } finally { 6 }"));
            Assert.AreEqual(6, Evaluate("try { } catch (e) { e } finally { 6 }"));
            Assert.AreEqual(6, Evaluate("try { throw 5 } catch (e) { e } finally { 6 }"));
            Assert.AreEqual(6, Evaluate("try { try { throw 5 } finally { 6 } } catch (e) { }"));
            Assert.AreEqual(6, Evaluate("var x = 0; try { try { throw 4; } catch (e) { throw 5; } finally { x = 6; } } catch (e) { } x"));

            // Return is valid within catch and finally blocks.
            Assert.AreEqual(6, Evaluate("function f() { try { throw 5 } catch (e) { return 6 } } f()"));
            Assert.AreEqual(6, Evaluate("function f() { try { throw 5 } finally { return 6 } } f()"));

            // Catch creates a new scope - but only for the exception variable.
            Assert.AreEqual(5, Evaluate("e = 5; try { throw 6; } catch (e) { } e"));
            Assert.AreEqual(5, Evaluate("e = 5; try { throw 6; } catch (e) { var e = 10; } e"));
            Assert.AreEqual(5, Evaluate("var b = 2; try { throw 6; } catch (e) { var b = 5; } b"));

            // Try without catch or finally is an error.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("try { }"));

            // Can declare a function inside a catch block.
            Assert.AreEqual(2, Evaluate("try { throw 6; } catch (e) { function foo() { return 2; } foo() }"));
            Assert.AreEqual(2, Evaluate("try { throw 6; } catch (e) { function foo() { return 2; } } foo()"));

            // Try using continue within try, catch and finally blocks.
            Assert.AreEqual(3, Evaluate("var j = 0; for (var i = 0; i < 3; i ++) { try { j ++; continue; } catch (e) { j = 0; } j = 0; } j"));
            Assert.AreEqual(3, Evaluate("var j = 0; for (var i = 0; i < 3; i ++) { try { throw 5; } catch (e) { j ++; continue; } j = 0; } j"));
            Assert.AreEqual(6, Evaluate("var j = 0; for (var i = 0; i < 3; i ++) { try { j ++; continue; } finally { j ++; } j = 0; } j"));
            Assert.AreEqual(6, Evaluate("var j = 0; for (var i = 0; i < 3; i ++) { try { j ++; } finally { j ++; continue; } j = 0; } j"));
            Assert.AreEqual(6, Evaluate("var j = 0; for (var i = 0; i < 3; i ++) { try { j ++; continue; } catch (e) { j = 0; } finally { j ++ } j = 0; } j"));
            Assert.AreEqual(6, Evaluate("var j = 0; for (var i = 0; i < 3; i ++) { try { j ++; } catch (e) { j = 0; } finally { j ++; continue; } j = 0; } j"));
            Assert.AreEqual(6, Evaluate("var j = 0; for (var i = 0; i < 3; i ++) { try { throw 5; } catch (e) { j ++; continue; } finally { j ++ } j = 0; } j"));
            Assert.AreEqual(6, Evaluate("var j = 0; for (var i = 0; i < 3; i ++) { try { throw 5; } catch (e) { j ++; } finally { j ++; continue; } j = 0; } j"));
            Assert.AreEqual(4, Evaluate("var j = 0; try { for (var i = 0; i < 3; i ++) { j ++; continue; j ++; } } finally { j ++; } j"));
            Assert.AreEqual(4, Evaluate("var j = 0; try { j ++ } finally { for (var i = 0; i < 3; i ++) { j ++; continue; j ++; } } j"));

            var scriptEngine = new ScriptEngine();

            // Catch should not catch exceptions other than JavaScriptException.
            // AND if the catch block is not run then the finally block shouldn't run either.
            scriptEngine.SetGlobalFunction("test", new Action(() =>
            {
                throw new ArgumentException("This is a test.");
            }));
            try
            {
                scriptEngine.Execute(@"
                    catchBlockRan = false;
                    finallyBlockRan = false;
                    try {
                        test();
                    } catch (ex) {
                        catchBlockRan = true;
                    } finally {
                        finallyBlockRan = true;
                    }");
                Assert.Fail("The exception should bubble out without being caught.");
            }
            catch (ArgumentException)
            {
                Assert.AreEqual(false, scriptEngine.GetGlobalValue<bool>("exceptionHandled"));
                Assert.AreEqual(false, scriptEngine.GetGlobalValue<bool>("finallyBlockRan"));
            }

            // Catch should not catch exceptions from other script engines.
            // AND if the catch block is not run then the finally block shouldn't run either.
            scriptEngine.SetGlobalFunction("test", new Action(() =>
            {
                throw new JavaScriptException(jurassicScriptEngine, ErrorType.Error, "This is a test.");
            }));
            try
            {
                scriptEngine.Execute(@"
                    catchBlockRan = false;
                    finallyBlockRan = false;
                    try {
                        test();
                    } catch (ex) {
                        catchBlockRan = true;
                    } finally {
                        finallyBlockRan = true;
                    }");
                Assert.Fail("The exception should bubble out without being caught.");
            }
            catch (JavaScriptException)
            {
                Assert.AreEqual(false, scriptEngine.GetGlobalValue<bool>("exceptionHandled"));
                Assert.AreEqual(false, scriptEngine.GetGlobalValue<bool>("finallyBlockRan"));
            }

            // The finally block shouldn't run for exceptions other than JavaScriptException.
            scriptEngine.SetGlobalFunction("test", new Action(() =>
            {
                throw new ArgumentException("This is a test.");
            }));
            try
            {
                scriptEngine.Execute(@"
                    finallyBlockRan = false;
                    try {
                        test();
                    } finally {
                        finallyBlockRan = true;
                    }");
                Assert.Fail("The exception should bubble out without being caught.");
            }
            catch (ArgumentException)
            {
                Assert.AreEqual(false, scriptEngine.GetGlobalValue<bool>("finallyBlockRan"));
            }

            // The finally block shouldn't run for exceptions from other script engines.
            scriptEngine.SetGlobalFunction("test", new Action(() =>
            {
                throw new JavaScriptException(jurassicScriptEngine, ErrorType.Error, "This is a test.");
            }));
            try
            {
                scriptEngine.Execute(@"
                    finallyBlockRan = false;
                    try {
                        test();
                    } finally {
                        finallyBlockRan = true;
                    }");
                Assert.Fail("The exception should bubble out without being caught.");
            }
            catch (JavaScriptException)
            {
                Assert.AreEqual(false, scriptEngine.GetGlobalValue<bool>("finallyBlockRan"));
            }
        }

        [TestMethod]
        [Ignore]    // default parameters are not supported yet.
        public void Function()
        {
            Assert.AreEqual(6, Evaluate("function f(a, b, c) { return a + b + c; } f(1, 2, 3)"));
            Assert.AreEqual(Undefined.Value, Evaluate("function f(a, b, c) { c = a + b; } f(1, 2, 3)"));

            // Multiple variable definitions.
            Assert.AreEqual(5, Evaluate("var a = 5; function a() { return 6 }; a"));
            Assert.AreEqual(5, Evaluate("function a() { return 6 }; var a = 5; a"));
            Assert.IsInstanceOfType(Evaluate("var a; function a() { return 6 }; a"), typeof(Jurassic.Library.FunctionInstance));
            Assert.IsInstanceOfType(Evaluate("function a() { return 6 }; var a; a"), typeof(Jurassic.Library.FunctionInstance));
            Assert.AreEqual(7, Evaluate("function a() { return 6 }; function a() { return 7 } a()"));
            Assert.AreEqual(4, Evaluate("a(); function a() { return 1 } function a() { return 2 } function a() { return 3 } function a() { return 4 }"));

            // Closures.
            Assert.AreEqual(7, Evaluate("function f(a, b, c) { return f2(a + 1); function f2(d) { return d + b + c; } } f(1, 2, 3)"));
            Assert.AreEqual(11, Evaluate("function makeAdder(a) { return function(b) { return a + b; } } x = makeAdder(5); x(6)"));
            Assert.AreEqual(27, Evaluate("function makeAdder(a) { return function(b) { return a + b; } } y = makeAdder(20); y(7)"));

            // Function expressions.
            Assert.AreEqual(3, Evaluate("var f = function () { return 3; }; f()"));

            // This
            Assert.AreEqual(1, Evaluate("var f = function () { return this.a; }; var x = {'a': 1, 'f': f}; x.f()"));

            // Function expressions are not visible in the local scope.
            Assert.AreEqual(1, Evaluate("function f() {return 1} x = function f() {return 2}; f()"));

            // The function name is defined in the function scope.
            Assert.AreEqual(24, Evaluate("var fact='test'; Math.factorial = function fact(n) {return n<=1?1:n*fact(n-1)}; Math.factorial(4)"));

            // Argument names override the function name and 'arguments'.
            Assert.AreEqual(5, Evaluate("(function test(test) { return test; })(5)"));
            Assert.AreEqual(Undefined.Value, Evaluate("(function test(test) { return test; })()"));
            Assert.AreEqual(5, Evaluate("function test(test) { return test; } test(5)"));
            Assert.AreEqual(5, Evaluate("(function f(arguments) { return arguments })(5)"));
            Assert.AreEqual(Undefined.Value, Evaluate("(function f(arguments) { return arguments })()"));
            Assert.AreEqual(5, Evaluate("(function f(a) { arguments = 5; return arguments })(5)"));
            Assert.AreEqual("[object Arguments]", Evaluate("function arguments() { return arguments } arguments().toString()"));

            // Duplicate argument names.
            Assert.AreEqual(Undefined.Value, Evaluate("function f1(x, a, b, x){ return x; } f1(1, 2)"));
            Assert.AreEqual(true, Evaluate("f = new Function('a', 'a', 'return true'); f()"));

            // Strict mode
            Assert.AreEqual("blah", Evaluate("'use strict'; function strict_test(){ return 'blah'; } strict_test()"));
            Assert.AreEqual("blah2", Evaluate("function strict_test2(){ 'use strict'; return 'blah2'; } strict_test2()"));

            // Strict mode: the name "eval" is not allowed in strict mode.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function eval(){}"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function test(eval){}"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function(eval){}"));
            Assert.AreEqual(true, Evaluate("'use strict'; var f = new Function('eval', 'return true'); f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function eval(){ 'use strict'; }"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function test(eval){ 'use strict'; }"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function(eval){ 'use strict'; }"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("f = new Function('eval', \"'use strict'; return true\"); f()"));

            // Strict mode: argument names cannot be identical.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; (function(arg, arg) { })()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("(function(arg, arg) { 'use strict' })()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f(arg, arg) { }"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f(arg, arg) { 'use strict' }"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("f = new Function('arg', 'arg', \"'use strict'; return true\"); f()"));

            // Default function parameters.
            Execute("function f(a = 1, b = 2, c = 3) { return a + b + c; }");
            Assert.AreEqual(6, Evaluate("f()"));
            Assert.AreEqual(9, Evaluate("f(4)"));
            Assert.AreEqual(3, Evaluate("f(1, 1, 1, 1)"));
            Assert.AreEqual(9, Evaluate("f(undefined, 5, undefined)"));
            Assert.AreEqual(5, Evaluate("f(null, 5, null)"));
            Assert.AreEqual(10, Evaluate("(function(a, b = a*2) { return b })(5)"));
            Assert.AreEqual("testFunc751", Evaluate("(function testFunc751(a = testFunc751) { return a; })().name"));
            Assert.AreEqual("test", Evaluate("(function(a = this) { return a; }).call('test').toString()"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("(function(a, b = c*2) { var c = 3; return b })(5)"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("(function(a = a) { return a; })()"));
        }
    }
}
