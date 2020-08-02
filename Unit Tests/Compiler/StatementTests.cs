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
            Assert.AreEqual("SyntaxError: Unexpected token 'else' in expression.", EvaluateExceptionMessage("if (true) 5 else 6"));
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
            Assert.AreEqual("SyntaxError: Expected an expression but found ')' instead", EvaluateExceptionMessage("x = 6; while () { x = x + 3 } x"));
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
            Assert.AreEqual("SyntaxError: Expected ',' but found '+'", EvaluateExceptionMessage("for (var x + 1; x < 5; x ++) { }"));
            Assert.AreEqual("SyntaxError: Invalid target of postfix operation.", EvaluateExceptionMessage("for (var x = 0; x < 1; 0 ++) { }"));

            // Strict mode.
            Assert.AreEqual(45, Evaluate("'use strict'; var y = 0; for (var x = 0; x < 10; x ++) { y += x; } y"));
            Execute("'use strict'; var y = 0; for (var x = 0; x < 10; x ++) { y += x; }");
            Assert.AreEqual(45, Evaluate("y"));

            // This shouldn't throw.
            Execute(@"
                'use strict';
                for (var i = 0; i < 3; i++) {
                    (function () {
                        function test() {
                        }
                        test();
                    }());
                }");
        }

        [TestMethod]
        public void ForIn()
        {
            // for (x in <expression>)
            Assert.AreEqual("ab", Evaluate("y = ''; for (x in {a: 1, b: 2}) { y += x } y"));
            Assert.AreEqual("1", Evaluate("y = 0; for (x in [7, 5]) { y = x } y"));
            Assert.AreEqual(0, Evaluate("x = 0; for (x in null) { x = 1 } x"));
            Assert.AreEqual(0, Evaluate("x = 0; for (x in undefined) { x = 1 } x"));
            Assert.AreEqual("SyntaxError: Invalid left-hand side in for loop.", EvaluateExceptionMessage("for (5 in [1, 2]) {}"));
            Assert.AreEqual("2", Evaluate("var x = { a: 1 }; for (x.a in [1, 2, 3]) { } x.a"));

            // for (var x in <expression>)
            Assert.AreEqual("1", Evaluate("y = 0; for (var x in [7, 5]) { y = x } y"));
            Assert.AreEqual("01234", Evaluate("y = ''; for (var x in 'hello') { y += x } y"));
            Assert.AreEqual("SyntaxError: Expected identifier but found '5'", EvaluateExceptionMessage("for (var 5 in [1, 2])"));
            Assert.AreEqual("SyntaxError: Expected ',' but found '.'", EvaluateExceptionMessage("var x = { a: 1 }; for (var x.a in [1, 2, 3]) { } x.a"));

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
            Assert.AreEqual("SyntaxError: The variable name cannot be 'eval' in strict mode.", EvaluateExceptionMessage("'use strict'; for (var eval in {a:1}) {}"));
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
            Assert.AreEqual(3, Evaluate("var x = { a: 1 }; for (x.a of [1, 2, 3]) { } x.a"));

            // for (var x of <expression>)
            Assert.AreEqual(206, Evaluate("y = 0; for (var x of [93, 113]) { y += x } y"));
            Assert.AreEqual("SyntaxError: Expected ',' but found '.'", EvaluateExceptionMessage("var x = { a: 1 }; for (var x.a of [1, 2, 3]) { } x.a"));

            // Iterate over a generator.
            Assert.AreEqual("2 3 4 5 ", Evaluate(@"
                var range = {
                  from: 2,
                  to: 5
                };
                // 1. call to for..of initially calls this
                range[Symbol.iterator] = function() {
                  // ...it returns the iterator object:
                  // 2. Onward, for..of works only with this iterator, asking it for next values
                  return {
                    current: this.from,
                    last: this.to,
                    // 3. next() is called on each iteration by the for..of loop
                    next() {
                      // 4. it should return the value as an object {done:.., value :...}
                      if (this.current <= this.last) {
                        return { done: false, value: this.current++ };
                      } else {
                        return { done: true };
                      }
                    }
                  };
                };
                y = ''; for (var x of range) { y += x + ' ' } y"));

            // Type errors.
            Assert.AreEqual("TypeError: 1 is not iterable.", EvaluateExceptionMessage("for (x of 1) {}"));
            Assert.AreEqual("TypeError: null is not iterable.", EvaluateExceptionMessage("for (x of null) {}"));
            Assert.AreEqual("TypeError: undefined is not iterable.", EvaluateExceptionMessage("for (x of undefined) {}"));
            Assert.AreEqual("TypeError: [object Object] is not iterable.", EvaluateExceptionMessage("for (x of {}) {}"));

            // Syntax errors.
            Assert.AreEqual("SyntaxError: Expected ')' but found ','", EvaluateExceptionMessage("for (x of [1, 2], [3, 4]) {}"));
            Assert.AreEqual("SyntaxError: Invalid left-hand side in for loop.", EvaluateExceptionMessage("for (5 of [1, 2]) {}"));
            Assert.AreEqual("SyntaxError: Expected identifier but found '5'", EvaluateExceptionMessage("for (var 5 of [1, 2]) {}"));
            Assert.AreEqual("SyntaxError: Invalid left-hand side in for loop; must have a single binding.", EvaluateExceptionMessage("for (var x, y of [1, 2]) {}"));
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
            Assert.AreEqual("SyntaxError: The statement with label 'test' is not a loop", EvaluateExceptionMessage("x = 1; test: continue test; x"));
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
            Assert.AreEqual("SyntaxError: Label 'label' has already been declared", EvaluateExceptionMessage("label: { label: { } }"));

            // The label must be an enclosing statement in the same function.
            Assert.AreEqual("SyntaxError: Undefined label 'test'", EvaluateExceptionMessage("x = 1; test: x ++; break test; x")); // Not an enclosing statement.
        }

        [TestMethod]
        public void Var()
        {
            Assert.AreEqual(Undefined.Value, Evaluate("var x"));
            Assert.AreEqual(Undefined.Value, Evaluate("var x; x"));
            Assert.AreEqual(Undefined.Value, Evaluate("var x, y"));
            Assert.AreEqual(Undefined.Value, Evaluate("_varDeclaration; var _varDeclaration"));
            Assert.AreEqual(Undefined.Value, Evaluate("(function() { var x; return x; })();"));
            Assert.AreEqual(Undefined.Value, Evaluate("(function() { var a = x; var x; return a; })();"));
            Assert.AreEqual(2, Evaluate("(function() { for (var i = 0; i < 2; i ++) { } return i; })();"));
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

            // Variables declared using 'var' in the global scope are stored in the global object.
            Assert.AreEqual(true, Evaluate(@"
                var inGlobal = 'globalVar1' in this;
                var globalVar1 = 7;
                inGlobal"));
            Assert.AreEqual(@"{""value"":7,""writable"":true,""enumerable"":true,""configurable"":false}", Evaluate(@"
                var globalVar2 = 7;
                JSON.stringify(Object.getOwnPropertyDescriptor(this, 'globalVar2'))"));
            Assert.AreEqual(Undefined.Value, Evaluate(@"
                var x = this.globalVar3;
                var globalVar3 = 7;
                x"));

            // Strict mode.
            Assert.AreEqual("ReferenceError: globalVar4 is not defined.",
                EvaluateExceptionMessage(@"'use strict'; var x = globalVar4; x"));
            Assert.AreEqual(Undefined.Value,
                Evaluate(@"'use strict'; var x = globalVar5; var globalVar5; x"));
            Assert.AreEqual("ReferenceError: globalVar6 is not defined.",
                EvaluateExceptionMessage(@"'use strict'; globalVar6 = 10"));

            // Strict mode: the name "eval" is not allowed in strict mode.
            Assert.AreEqual("SyntaxError: The variable name cannot be 'eval' in strict mode.",
                EvaluateExceptionMessage("'use strict'; var eval = 5"));
        }

        [TestMethod]
        public void Let()
        {
            // Basic declaration syntax checks.
            Assert.AreEqual(Undefined.Value, Evaluate("let x"));
            Assert.AreEqual(Undefined.Value, Evaluate("let x; x"));
            Assert.AreEqual(Undefined.Value, Evaluate("let x, y"));
            Assert.AreEqual(5, Evaluate("let x = 5; x"));
            Assert.AreEqual(6, Evaluate("let x, y = 6; y"));
            Assert.AreEqual(1, Evaluate("let x = 1, y = 2; x"));
            Assert.AreEqual(2, Evaluate("let x = 1, y = 2; y"));
            Assert.AreEqual(2, Evaluate("let x = Math.max(1, 2); x"));
            Assert.AreEqual(3, Evaluate("'use strict'; let x = 3; x"));

            // 'let' declarations are specific to the block they're in.
            Assert.AreEqual(15, Evaluate(@"
                let a = 15;
                {
                    let a = 3;
                }
                a"));

            // 'let' declarations cannot be accessed before they are initialized.
            Assert.AreEqual("ReferenceError: Cannot access 'a' before initialization.", EvaluateExceptionMessage(@"
                let a = 15, b = 16;
                {
                    b = a;
                    let a = 3;
                }
                b"));
            Assert.AreEqual("ReferenceError: Cannot access 'foo' before initialization.", EvaluateExceptionMessage(@"
                (function do_something() {
                    let x = foo;
                    let foo = 2;
                })()"));
            Assert.AreEqual("ReferenceError: Cannot access 'foo' before initialization.", EvaluateExceptionMessage(@"
                (function do_something() {
                    let x = typeof foo;
                    let foo = 2;
                })()"));


            // 'let' variables do not get stored in the global object.
            Assert.AreEqual(Undefined.Value, Evaluate(@"let notAGlobal = 5; this.notAGlobal"));

            Assert.AreEqual("ReferenceError: _letVar3 is not defined.", EvaluateExceptionMessage("(function() { for (let _letVar3 = 0; _letVar3 < 2; _letVar3 ++) { } return _letVar3; })();"));
            Assert.AreEqual("undefined", Evaluate("delete i; (function() { i = 5; var i = 3; })(); typeof(i);"));

            // Let is not a keyword in non-strict mode.
            Assert.AreEqual(5, Evaluate("var let = 5; let"));

            // Let is a keyword in strict mode.
            Assert.AreEqual("SyntaxError: Expected identifier but found 'let'", EvaluateExceptionMessage("'use strict'; var let = 5; let"));

            // Duplicate names are not allowed.
            Assert.AreEqual("SyntaxError: Identifier 'x' has already been declared.", EvaluateExceptionMessage("let x = 3, x = 5; x"));
            Assert.AreEqual("SyntaxError: Identifier 'x' has already been declared.", EvaluateExceptionMessage("let x = 3; let x = 5; x"));

            // 'let' is not a valid name in a let declaration.
            Assert.AreEqual("SyntaxError: 'let' is not allowed here.", EvaluateExceptionMessage("let let"));
            Assert.AreEqual("SyntaxError: 'let' is not allowed here.", EvaluateExceptionMessage("let let = 5"));

            // Each loop of a scope is a new one.
            Assert.AreEqual("ReferenceError: Cannot access 'a' before initialization.", EvaluateExceptionMessage(@"
                var b = 0;
                for (let i = 0; i < 2; i++) {
                    if (i == 1) {
                        b = a;
                    }
                    let a = 7;
                }"));

            // for (let i ...) { }
            Assert.AreEqual(13, Evaluate("var i = 10, g = i; for (let i = 0; i < 3; i ++) { g += i; } g;"));
            Assert.AreEqual("ReferenceError: _letVar1 is not defined.", EvaluateExceptionMessage("do { let _letVar1 = 5; } while (_letVar1 > 5);"));
            Assert.AreEqual("ReferenceError: _letVar2 is not defined.", EvaluateExceptionMessage("for (; _letVar2 < 2; _letVar2++) { let _letVar2; }"));
        }

        [TestMethod]
        public void Const()
        {
            // Basic declaration syntax checks.
            Assert.AreEqual(Undefined.Value, Evaluate("const x = 5"));
            Assert.AreEqual(5, Evaluate("const x = 5; x"));
            Assert.AreEqual(6, Evaluate("const x = 5, y = 6; y"));
            Assert.AreEqual(3, Evaluate("'use strict'; const x = 3; x"));

            // 'const' declaration require an initializer.
            Assert.AreEqual("SyntaxError: Missing initializer in const declaration.", EvaluateExceptionMessage("const x"));

            // 'const' declarations are specific to the block they're in.
            Assert.AreEqual(15, Evaluate(@"
                const a = 15;
                {
                    const a = 3;
                }
                a"));

            // 'const' declarations cannot be accessed before they are initialized.
            Assert.AreEqual("ReferenceError: Cannot access 'a' before initialization.", EvaluateExceptionMessage(@"
                let a = 15, b = 16;
                {
                    b = a;
                    const a = 3;
                }
                b"));
            Assert.AreEqual("ReferenceError: Cannot access 'foo' before initialization.", EvaluateExceptionMessage(@"
                (function do_something() {
                    let x = foo;
                    const foo = 2;
                })()"));
            Assert.AreEqual("ReferenceError: Cannot access 'foo' before initialization.", EvaluateExceptionMessage(@"
                (function do_something() {
                    let x = typeof foo;
                    const foo = 2;
                })()"));


            // 'const' variables do not get stored in the global object.
            Assert.AreEqual(Undefined.Value, Evaluate(@"const notAGlobal = 5; this.notAGlobal"));

            // Duplicate names are not allowed.
            Assert.AreEqual("SyntaxError: Identifier 'x' has already been declared.", EvaluateExceptionMessage("const x = 3, x = 5; x"));
            Assert.AreEqual("SyntaxError: Identifier 'x' has already been declared.", EvaluateExceptionMessage("const x = 3; const x = 5; x"));

            // 'const' variables are read-only.
            Assert.AreEqual("TypeError: Illegal assignment to constant variable 'x'.", EvaluateExceptionMessage("const x = 5; x = 6"));
        }

        [TestMethod]
        public void Return()
        {
            Assert.AreEqual(10, Evaluate("function f() { for (var i = 0; i < 1; i++) { return 10; } return 15; } f()"));
            Assert.AreEqual(5, Evaluate("function f() { return 5 } f()"));
            Assert.AreEqual(Undefined.Value, Evaluate("function f() { } f()"));
            Assert.AreEqual(Undefined.Value, Evaluate("function f() { return } f()"));
            Assert.AreEqual("SyntaxError: Return statements are only allowed inside functions", EvaluateExceptionMessage("return 5"));
            Assert.AreEqual("SyntaxError: Return statements are only allowed inside functions", EvaluateExceptionMessage("eval('return 5')"));
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
            Assert.AreEqual(43, Evaluate("delete a; x = { a: 43 }; with (x) { function y() { return a } } y()"));
            Execute("_f = undefined");
            Assert.AreEqual("undefined", Evaluate("result = typeof _f; with ({a: 2}) { function _f() { return 5 } } result"));
            Assert.AreEqual("function", Evaluate("typeof _f"));

            // With statements are syntax errors in strict mode.
            Assert.AreEqual("SyntaxError: The with statement is not supported in strict mode", EvaluateExceptionMessage("'use strict'; var x = {}; with (x) { }"));
            Assert.AreEqual("SyntaxError: The with statement is not supported in strict mode", EvaluateExceptionMessage(@"eval(""'use strict'; var o = {}; with (o) {}"")"));
            Assert.AreEqual("SyntaxError: The with statement is not supported in strict mode", EvaluateExceptionMessage(@"'use strict'; eval(""var o = {}; with (o) {}"")"));
            Assert.AreEqual("SyntaxError: The with statement is not supported in strict mode", EvaluateExceptionMessage(@"eval(""function f() { 'use strict'; var o = {}; with (o) {} }"")"));
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
            Assert.AreEqual("SyntaxError: Only one default clause is allowed.", EvaluateExceptionMessage("x = 5; switch (x) { default: 6; default: 7 }"));
        }

        [TestMethod]
        public void Throw()
        {
            Assert.AreEqual("Error: test", EvaluateExceptionMessage("throw new Error('test')"));
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
            Assert.AreEqual(5, Evaluate("var b = 2; try { throw 6; } catch { var b = 5; } b"));

            // Try without catch or finally is an error.
            Assert.AreEqual("SyntaxError: Missing catch or finally after try", EvaluateExceptionMessage("try { }"));

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

            // Catch binding is optional in ECMAScript 2019.
            Assert.AreEqual(6, Evaluate("(function() { try { throw 5 } catch { return 6 } })()"));

            // Errors.
            Assert.AreEqual("SyntaxError: Expected ')' but found ','", EvaluateExceptionMessage("(function() { try { throw 5 } catch (a, b) { return 6 } })()"));

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
                throw new JavaScriptException(ScriptEngine, ErrorType.Error, "This is a test.");
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
                throw new JavaScriptException(ScriptEngine, ErrorType.Error, "This is a test.");
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
        public void Function()
        {
            Assert.AreEqual(6, Evaluate("function f(a, b, c) { return a + b + c; } f(1, 2, 3)"));

            // No return means the function returns undefined.
            Assert.AreEqual(Undefined.Value, Evaluate("function f(a, b, c) { c = a + b; } f(1, 2, 3)"));
            Assert.AreEqual(Undefined.Value, Evaluate("function f(a, b, c) { if (c > 3) return 3; } f(1, 2, 3)"));

            // Multiple variable definitions.
            Assert.AreEqual(5, Evaluate("var a = 5; function a() { return 6 }; a"));
            Assert.AreEqual(5, Evaluate("function a() { return 6 }; var a = 5; a"));
            Assert.IsInstanceOfType(Evaluate("var a; function a() { return 6 }; a"), typeof(FunctionInstance));
            Assert.IsInstanceOfType(Evaluate("function a() { return 6 }; var a; a"), typeof(FunctionInstance));
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
            Assert.AreEqual("SyntaxError: The variable name cannot be 'eval' in strict mode.", EvaluateExceptionMessage("'use strict'; function eval(){}"));
            Assert.AreEqual("SyntaxError: The variable name cannot be 'eval' in strict mode.", EvaluateExceptionMessage("'use strict'; function test(eval){}"));
            Assert.AreEqual(true, Evaluate("'use strict'; var f = new Function('eval', 'return true'); f()"));
            Assert.AreEqual("SyntaxError: Functions cannot be named 'eval' in strict mode.", EvaluateExceptionMessage("function eval(){ 'use strict'; }"));
            Assert.AreEqual("SyntaxError: Arguments cannot be named 'eval' in strict mode.", EvaluateExceptionMessage("function test(eval){ 'use strict'; }"));
            Assert.AreEqual("SyntaxError: Arguments cannot be named 'eval' in strict mode.", EvaluateExceptionMessage(@"f = new Function('eval', ""'use strict'; return true""); f()"));

            // Strict mode: argument names cannot be identical.
            Assert.AreEqual("SyntaxError: Duplicate argument name 'arg' is not allowed in strict mode.", EvaluateExceptionMessage("'use strict'; (function(arg, arg) { })()"));
            Assert.AreEqual("SyntaxError: Duplicate argument name 'arg' is not allowed in strict mode.", EvaluateExceptionMessage("(function(arg, arg) { 'use strict' })()"));
            Assert.AreEqual("SyntaxError: Duplicate argument name 'arg' is not allowed in strict mode.", EvaluateExceptionMessage("'use strict'; function f(arg, arg) { }"));
            Assert.AreEqual("SyntaxError: Duplicate argument name 'arg' is not allowed in strict mode.", EvaluateExceptionMessage("function f(arg, arg) { 'use strict' }"));
            Assert.AreEqual("SyntaxError: Duplicate argument name 'arg' is not allowed in strict mode.", EvaluateExceptionMessage("f = new Function('arg', 'arg', \"'use strict'; return true\"); f()"));

            // Hoisting.
            Assert.AreEqual("function", Evaluate(@"
                var x = typeof double;
                var double = 22;
                function double(num) {
                  return (num*2);
                }
                x"));
            Assert.AreEqual("number", Evaluate(@"
                var double = 22;
                function double(num) {
                  return (num*2);
                }
                typeof double"));
            Assert.AreEqual("function", Evaluate(@"
                var double;
                function double(num) {
                  return (num*2);
                }
                typeof double"));

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

            // TODO
            //Assert.AreEqual("ReferenceError", EvaluateExceptionMessage("(function(a, b = c*2) { var c = 3; return b })(5)"));
            //Assert.AreEqual("ReferenceError", EvaluateExceptionMessage("(function(a = a) { return a; })()"));
        }

        [TestMethod]
        public void Class()
        {
            // Class with single function.
            Assert.AreEqual(17, Evaluate(@"
                class A {
                    b() { return 17; }
                }
                new A().b()"));

            // Class with getter.
            Assert.AreEqual(17, Evaluate(@"
                class A {
                    get b() { return 17; }
                }
                new A().b"));

            // Class with getter and setter.
            Assert.AreEqual(400, Evaluate(@"
                class A {
                    get b() { return this.c * 2; }
                    set b(value) { this.c = value * 2; }
                }
                var a = new A();
                a.b = 100;
                a.b"));

            // Class with constructor.
            Assert.AreEqual(15, Evaluate(@"
                class A {
                    constructor() {
                        this.b = 15;
                    }
                    getB() { return this.b; }
                }
                new A().getB()"));
            Assert.AreEqual(99, Evaluate(@"
                class A {
                    constructor(value) {
                        this.b = value;
                    }
                    getB() { return this.b; }
                }
                new A(99).getB()"));

            // Class with static member.
            Assert.AreEqual(12, Evaluate(@"
                class A {
                    static b() { return 12; }
                }
                A.b()"));

            // Class with static accessor properties.
            Assert.AreEqual(true, Evaluate(@"
                var baz = false;
                class C {
                   static get foo() { return 'foo'; }
                   static set bar(x) { baz = x; }
                }
                C.bar = true;
                C.foo === 'foo' && baz"));

            // Class expression.
            Assert.AreEqual(17, Evaluate(@"
                A = class {
                    b() { return 17; }
                }
                new A().b()"));

            // Semi-colons are allowed within class bodies.
            Assert.AreEqual(true, Evaluate(@"
                class C {
                    ;
                    method() { return 2; };
                    method2() { return 2; }
                    method3() { return 2; };
                }
                typeof C.prototype.method === 'function'
                    && typeof C.prototype.method2 === 'function'
                    && typeof C.prototype.method3 === 'function';"));

            // Multiple constructors are not allowed.
            Assert.AreEqual("SyntaxError: A class may only have one constructor.", EvaluateExceptionMessage(@"
                class A {
                    constructor() { }
                    constructor() { }
                }"));

            // A static member called 'prototype' is not allowed.
            Assert.AreEqual("SyntaxError: Classes may not have a static property named 'prototype'.", EvaluateExceptionMessage(@"
                class A {
                    static prototype() { }
                }"));

            // Class with extends.
            Assert.AreEqual("oof", Evaluate(@"
                class Animal {
                    speak1() { return 'oof'; }
                }
                class Dog extends Animal {
                    speak2() { return 'bark'; }
                }
                new Dog().speak1()"));

            // Property overrides.
            Assert.AreEqual("bark", Evaluate(@"
                class Animal {
                    speak() { return 'oof'; }
                }
                class Dog extends Animal {
                    speak() { return 'bark'; }
                }
                new Dog().speak()"));

            // Check the prototypes are correct when extending.
            Assert.AreEqual(true, Evaluate(@"
                class Animal {
                    speak() { return 'oof'; }
                }
                class Dog extends Animal {
                    speak() { return 'bark'; }
                }
                new Dog() instanceof Animal && Animal.isPrototypeOf(Dog)"));

            // Extend from null
            Assert.AreEqual(true, Evaluate(@"
                class C extends null {
                    speak() { return 'oof'; }
                }
                Function.prototype.isPrototypeOf(C) && Object.getPrototypeOf(C.prototype) === null"));

            // The class name should be valid inside of class functions.
            Assert.AreEqual(true, Evaluate(@"
                class C {
                    method() { return typeof C === ""function""; }
                }
                var M = C.prototype.method;
                C = void undefined;
                C === void undefined && M();"));

            // Classes are block-scoped.
            Assert.AreEqual(true, Evaluate(@"
                class C {}
                var c1 = C;
                {
                    class C {}
                    var c2 = C;
                }
                C === c1;"));

            // name property.
            Assert.AreEqual("C", Evaluate(@"var x = class C {}; x.name"));
            Assert.AreEqual("", Evaluate(@"var x = []; x[5] = class {}; x[5].name"));

            // length property.
            Assert.AreEqual(0, Evaluate(@"var x = class C {}; x.length"));
            Assert.AreEqual(1, Evaluate(@"var x = class C { constructor(a) { } }; x.length"));
        }
    }
}
