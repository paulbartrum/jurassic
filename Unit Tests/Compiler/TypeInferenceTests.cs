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
    /// Type inference tests.
    /// </summary>
    [TestClass]
    public class TypeInferenceTests
    {
        
        [TestMethod]
        public void TypeInference1()
        {
            // Simple type inference: both "i" and "x" can be inferred to be integers.
            Assert.AreEqual(4, TestUtils.Evaluate(@"
                function f() {
                    var x = 5;
                    for (var i = 1; i < 10; i ++)
                        x = x ^ i;
                    return x;
                }
                f()"));

            // Simple type inference:
            // "i" can be inferred to be an integer.
            // "x" can be inferred to be a number.
            Assert.AreEqual(362880, TestUtils.Evaluate(@"
                function f() {
                    var x = 1;
                    for (var i = 1; i < 10; i ++)
                        x = x * i;
                    return x;
                }
                f()"));
            Assert.AreEqual(362880, TestUtils.Evaluate(@"
                function f(x) {
                    for (var i = 1; i < 10; i ++)
                        x = x * i;
                    return x;
                }
                f(1)"));

            // "x" can be inferred to be a number, but only by analysing the whole function and
            // not just the loop.
            Assert.AreEqual(19, TestUtils.Evaluate(@"
                function f() {
                    var x = 1;
                    for (var i = 1; i < 10; i ++)
                        x = x + 2;
                    return x;
                }
                f()"));

            // Now "x" cannot be inferred without whole-program analysis.
            Assert.AreEqual(19, TestUtils.Evaluate(@"
                function f(x) {
                    for (var i = 1; i < 10; i ++)
                        x = x + 2;
                    return x;
                }
                f(1)"));

            // "x" can be inferred to be a string.
            Assert.AreEqual("1*********", TestUtils.Evaluate(@"
                function f(x) {
                    for (var i = 1; i < 10; i ++)
                        x = x + '*';
                    return x;
                }
                f(1)"));
        }

        [TestMethod]
        public void TypeInference2()
        {
            // Here x alternates between a number and a string, so no stable type can be inferred.
            Assert.AreEqual("401044422751291", TestUtils.Evaluate(@"
                function f(x) {
                    for (var i = 1; i < 10; i ++) {
                        x = x * i;
                        x = x + '1';
                    }
                    return x;
                }
                f(1)"));

            // Here x can be inferred to be a number, since all assignments are compatible.
            Assert.AreEqual(185794560, TestUtils.Evaluate(@"
                function f(x) {
                    for (var i = 1; i < 10; i ++) {
                        x = x * i;
                        x = x * 2;
                    }
                    return x;
                }
                f(1)"));

            // Here x can be inferred to be a number (normally the addition would be ambiguous,
            // but in this case x must be a number at the point that the addition occurs.
            Assert.AreEqual(1609940, TestUtils.Evaluate(@"
                function f(x) {
                    for (var i = 1; i < 10; i ++) {
                        x = x * i;
                        x = x + 2;
                    }
                    return x;
                }
                f(1)"));
        }

        [TestMethod]
        public void TypeInference3()
        {
            // x can be inferred to be a number, but only if analysis can recognise that the
            // assignment must execute at least once.
            Assert.AreEqual(0, TestUtils.Evaluate(@"
                function f() {
                    var x = null;
                    for (var i = 1; i < 10; i ++)
                        if (i < 2)
                            x = x * i;
                    return x;
                }
                f()"));

            // In this case "x" is not a number because the assignment statement never executes.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate(@"
                function f() {
                    var x = null;
                    for (var i = 1; i < 10; i ++)
                        if (i < 1)
                            x = x * i;
                    return x;
                }
                f()"));

            // Again, "x" is not a number because the assignment statement never executes.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate(@"
                function f() {
                    var x = null;
                    for (var i = 1; i < 10; i ++)
                        i < 1 ? x = x * i : 0;
                    return x;
                }
                f()"));

            // Again, "x" is not a number because the assignment statement never executes.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate(@"
                function f() {
                    var x = null;
                    for (var i = 1; i < 10; i ++)
                        i < 1 && (x = 1);
                    return x;
                }
                f()"));


            // Again, "x" is not a number because the assignment statement never executes.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate(@"
                function f() {
                    var x = null;
                    for (var i = 1; i < 10; i ++) {
                        break;
                        x = x * i;
                    }
                    return x;
                }
                f()"));

            // Again, "x" is not a number because the assignment statement never executes.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate(@"
                function f() {
                    var x = null;
                    try {
                        for (var i = 1; i < 10; i ++) {
                            throw 1;
                            x = x * i;
                        }
                    }
                    catch (e) {
                    }
                    return x;
                }
                f()"));

            // Again, "x" is not a number because the assignment statement never executes.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate(@"
                function f() {
                    var x = null;
                    for (var i = 1; i < 10; i ++) {
                        try {
                            throw 1;
                            x = x * i;
                        }
                        catch (e) {
                        }
                    }
                    return x;
                }
                f()"));

            // Again, "x" is not a number because the assignment statement never executes.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate(@"
                function f() {
                    var x = null;
                    for (var i = 1; i < 10; i ++) {
                        continue;
                        x = x * i;
                    }
                    return x;
                }
                f()"));

            // Again, "x" is not a number because the assignment statement never executes.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate(@"
                function f() {
                    var x = null;
                    for (var i = 1; i < 10; i ++) {
                        if (i >= 1)
                            continue;
                        x = x * i;
                    }
                    return x;
                }
                f()"));
        }

    }
}
