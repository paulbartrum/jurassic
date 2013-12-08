using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global String object.
    /// </summary>
    [TestClass]
    public class StringTests
    {
        
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("", TestUtils.Evaluate("String()"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("String(undefined)"));
            Assert.AreEqual("null", TestUtils.Evaluate("String(null)"));
            Assert.AreEqual("5.1", TestUtils.Evaluate("String(5.1)"));
            Assert.AreEqual("510000", TestUtils.Evaluate("String(5.1e5)"));
            Assert.AreEqual("100000000000000000000", TestUtils.Evaluate("String(100000000000000000000)"));
            Assert.AreEqual("1e+21", TestUtils.Evaluate("String(1000000000000000000000)"));
            Assert.AreEqual("NaN", TestUtils.Evaluate("String(NaN)"));
            Assert.AreEqual("", TestUtils.Evaluate("String('')"));
            Assert.AreEqual("deadline", TestUtils.Evaluate("String('deadline')"));

            // Construct
            Assert.AreEqual("", TestUtils.Evaluate("new String().valueOf()"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("new String(undefined).valueOf()"));
            Assert.AreEqual("null", TestUtils.Evaluate("new String(null).valueOf()"));
            Assert.AreEqual("5.1", TestUtils.Evaluate("new String(5.1).valueOf()"));
            Assert.AreEqual("NaN", TestUtils.Evaluate("new String(NaN).valueOf()"));
            Assert.AreEqual("", TestUtils.Evaluate("new String('').valueOf()"));
            Assert.AreEqual("deadline", TestUtils.Evaluate("new String('deadline').valueOf()"));

            // toString and valueOf.
            Assert.AreEqual("function String() { [native code] }", TestUtils.Evaluate("String.toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("String.valueOf() === String"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("String.length"));
        }

        [TestMethod]
        public void Properties()
        {
            // Constructor and __proto__
            Assert.AreEqual(true, TestUtils.Evaluate("new String().constructor === String"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new String()) === String.prototype"));

            // length is enumerable, non-configurable, non-writable
            Assert.AreEqual(3, TestUtils.Evaluate("'abc'.length"));
            Assert.AreEqual(0, TestUtils.Evaluate("var x = ''; x.length = 5; x.length"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(false, TestUtils.Evaluate("var x = 'abc'; delete x.length"));
            Assert.AreEqual(3, TestUtils.Evaluate("var x = 'abc'; delete x.length; x.length"));

            // String can be indexed like arrays.
            Assert.AreEqual("a", TestUtils.Evaluate("'abc'[0]"));
            Assert.AreEqual("b", TestUtils.Evaluate("'abc'[1]"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("c", TestUtils.Evaluate("new String('abc')[2]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'abc'[-1]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'abc'[3]"));

            // The array indices are enumerable.
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("01234", TestUtils.Evaluate("y = ''; for (var x in 'hello') { y += x } y"));

            // Array elements cannot be modified within the range of the string, but can be modified otherwise.
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("a", TestUtils.Evaluate("var x = new String('abc'); x[0] = 'c'; x[0]"));
            Assert.AreEqual("c", TestUtils.Evaluate("var x = new String('abc'); x[100] = 'c'; x[100]"));
            Assert.AreEqual("c", TestUtils.Evaluate("var x = new String('abc'); x.a = 'c'; x.a"));
        }

        [TestMethod]
        public void fromCharCode()
        {
            Assert.AreEqual("", TestUtils.Evaluate("String.fromCharCode()"));
            Assert.AreEqual("ha", TestUtils.Evaluate("String.fromCharCode(104, 97)"));
            Assert.AreEqual("!", TestUtils.Evaluate("String.fromCharCode(5999951905)"));

            // length
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(1, TestUtils.Evaluate("String.fromCharCode.length"));

            // No this object is required.
            Assert.AreEqual("ha", TestUtils.Evaluate("var f = String.fromCharCode; f(104, 97)"));
        }

        [TestMethod]
        public void anchor()
        {
            Assert.AreEqual(@"<a name=""undefined"">haha</a>", (string)TestUtils.Evaluate("'haha'.anchor()"), true);
            Assert.AreEqual(@"<a name=""test"">haha</a>", (string)TestUtils.Evaluate("'haha'.anchor('test')"), true);
            Assert.AreEqual(1, TestUtils.Evaluate("''.anchor.length"));
        }

        [TestMethod]
        public void big()
        {
            Assert.AreEqual("<big>haha</big>", (string)TestUtils.Evaluate("'haha'.big()"), true);
            Assert.AreEqual(0, TestUtils.Evaluate("''.big.length"));
        }

        [TestMethod]
        public void blink()
        {
            Assert.AreEqual("<blink>haha</blink>", (string)TestUtils.Evaluate("'haha'.blink()"), true);
            Assert.AreEqual(0, TestUtils.Evaluate("''.blink.length"));
        }

        [TestMethod]
        public void bold()
        {
            Assert.AreEqual("<b>haha</b>", (string)TestUtils.Evaluate("'haha'.bold()"), true);
            Assert.AreEqual(0, TestUtils.Evaluate("''.bold.length"));
        }

        [TestMethod]
        public void charAt()
        {
            Assert.AreEqual("h", TestUtils.Evaluate("'haha'.charAt(0)"));
            Assert.AreEqual("a", TestUtils.Evaluate("'haha'.charAt(1)"));
            Assert.AreEqual("h", TestUtils.Evaluate("'haha'.charAt(2)"));
            Assert.AreEqual("a", TestUtils.Evaluate("'haha'.charAt(3)"));
            Assert.AreEqual("", TestUtils.Evaluate("'haha'.charAt(-1)"));
            Assert.AreEqual("", TestUtils.Evaluate("'haha'.charAt(4)"));

            // Index argument uses ToInteger().
            Assert.AreEqual("", TestUtils.Evaluate("'abc'.charAt(Infinity)"));
            Assert.AreEqual("", TestUtils.Evaluate("'abc'.charAt(-Infinity)"));
            Assert.AreEqual("a", TestUtils.Evaluate("'abc'.charAt(NaN)"));
            Assert.AreEqual("", TestUtils.Evaluate("'abc'.charAt(4294967298)"));
            Assert.AreEqual("a", TestUtils.Evaluate("'abc'.charAt(null)"));
            Assert.AreEqual("a", TestUtils.Evaluate("'abc'.charAt(undefined)"));
            Assert.AreEqual("b", TestUtils.Evaluate("'abc'.charAt(true)"));
            Assert.AreEqual("a", TestUtils.Evaluate("'abc'.charAt(false)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("''.charAt.length"));

            // charAt is generic.
            Assert.AreEqual("2", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.charAt; x.f(3)"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.charAt.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.charAt.call(null)"));
        }

        [TestMethod]
        public void charCodeAt()
        {
            Assert.AreEqual(104, TestUtils.Evaluate("'haha'.charCodeAt(0)"));
            Assert.AreEqual(97, TestUtils.Evaluate("'haha'.charCodeAt(1)"));
            Assert.AreEqual(104, TestUtils.Evaluate("'haha'.charCodeAt(2)"));
            Assert.AreEqual(97, TestUtils.Evaluate("'haha'.charCodeAt(3)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("'haha'.charCodeAt(-1)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("'haha'.charCodeAt(4)"));
            Assert.AreEqual(104, TestUtils.Evaluate("'haha'.charCodeAt()"));
            Assert.AreEqual(1, TestUtils.Evaluate("''.charCodeAt.length"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("''.charCodeAt.length"));

            // charCodeAt is generic.
            Assert.AreEqual(50, TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.charCodeAt; x.f(3)"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.charCodeAt.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.charCodeAt.call(null)"));
        }

        [TestMethod]
        public void concat()
        {
            Assert.AreEqual("one", TestUtils.Evaluate("'one'.concat()"));
            Assert.AreEqual("onetwo", TestUtils.Evaluate("'one'.concat('two')"));
            Assert.AreEqual("onetwothree", TestUtils.Evaluate("'one'.concat('two', 'three')"));
            Assert.AreEqual("oneundefined", TestUtils.Evaluate("'one'.concat(undefined)"));
            Assert.AreEqual("onenull", TestUtils.Evaluate("'one'.concat(null)"));

            // concat does not change the original string.
            Assert.AreEqual("onetwo", TestUtils.Evaluate("var x = 'one'; x.concat('two')"));
            Assert.AreEqual("one", TestUtils.Evaluate("var x = 'one'; x.concat('two'); x"));
            Assert.AreEqual("onetwo", TestUtils.Evaluate("var x = 'one'; x += 'two'; x.concat();"));
            Assert.AreEqual("onetwothree", TestUtils.Evaluate("var x = 'one'; x += 'two'; x.concat('three');"));
            Assert.AreEqual("onetwo", TestUtils.Evaluate("var x = 'one'; x += 'two'; x.concat('three'); x"));
            Assert.AreEqual("onetwothreefour", TestUtils.Evaluate("var x = 'one'; x += 'two'; x.concat('three', 'four');"));
            Assert.AreEqual("onetwo", TestUtils.Evaluate("var x = 'one'; x += 'two'; x.concat('three', 'four'); x"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("''.concat.length"));

            // concat is generic.
            Assert.AreEqual("6.1234300", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.concat; x.f(300)"));
            Assert.AreEqual("first", TestUtils.Evaluate(@"
                obj1 = { toString: function() { throw 'first' } };
                obj2 = { toString: function() { throw 'second' } };
                obj1.concat = String.prototype.concat;
                try {
                    obj1.concat(obj2);
                }
                catch (e) {
                    e;
                }"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.concat.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.concat.call(null)"));
        }

        [TestMethod]
        public void @fixed()
        {
            Assert.AreEqual("<tt>haha</tt>", (string)TestUtils.Evaluate("'haha'.fixed()"), true);
            Assert.AreEqual(0, TestUtils.Evaluate("''.fixed.length"));
        }

        [TestMethod]
        public void fontcolor()
        {
            Assert.AreEqual(@"<font color=""undefined"">haha</font>", (string)TestUtils.Evaluate("'haha'.fontcolor()"), true);
            Assert.AreEqual(@"<font color=""red"">haha</font>", (string)TestUtils.Evaluate("'haha'.fontcolor('red')"), true);
            Assert.AreEqual(1, TestUtils.Evaluate("''.fontcolor.length"));
        }

        [TestMethod]
        public void fontsize()
        {
            Assert.AreEqual(@"<font size=""5"">haha</font>", (string)TestUtils.Evaluate("'haha'.fontsize(5)"), true);
            Assert.AreEqual(@"<font size=""abc"">haha</font>", (string)TestUtils.Evaluate("'haha'.fontsize('abc')"), true);
            Assert.AreEqual(@"<font size=""undefined"">haha</font>", (string)TestUtils.Evaluate("'haha'.fontsize()"), true);
            Assert.AreEqual(1, TestUtils.Evaluate("''.fontsize.length"));
        }

        [TestMethod]
        public void indexOf()
        {
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.indexOf('two')"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.indexOf('t')"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.indexOf('two', 2)"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.indexOf('two', 3)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'onetwothree'.indexOf('two', 4)"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.indexOf('two', -400)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'onetwothree'.indexOf('two', 400)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'onetwothree'.indexOf('no')"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'onetwothree'.indexOf('e', 400)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("''.indexOf('no')"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.indexOf('')"));

            // length
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(1, TestUtils.Evaluate("''.indexOf.length"));

            // indexOf is generic.
            Assert.AreEqual(2, TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.indexOf; x.f('123')"));
            Assert.AreEqual(8, TestUtils.Evaluate("x = new Date(0); x.f = ''.indexOf; x.getTimezoneOffset() > 0 ? x.f('31') : x.f('01')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.indexOf.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.indexOf.call(null)"));
        }

        [TestMethod]
        public void italics()
        {
            Assert.AreEqual("<i>haha</i>", (string)TestUtils.Evaluate("'haha'.italics()"), true);
            Assert.AreEqual(0, TestUtils.Evaluate("'haha'.italics.length"));
        }

        [TestMethod]
        public void lastIndexOf()
        {
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.lastIndexOf('two')"));
            Assert.AreEqual(6, TestUtils.Evaluate("'onetwothree'.lastIndexOf('t')"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.lastIndexOf('t', 4)"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.lastIndexOf('t', 3)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'onetwothree'.lastIndexOf('two', 2)"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.lastIndexOf('two', 3)"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.lastIndexOf('two', 4)"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.lastIndexOf('two', 400)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'onetwothree'.lastIndexOf('two', -400)"));
            Assert.AreEqual(6, TestUtils.Evaluate("'onetwothree'.lastIndexOf('three')"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'onetwothree'.lastIndexOf('no')"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'onetwothree'.lastIndexOf('o', -400)"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onetwothree'.lastIndexOf('two', NaN)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("''.lastIndexOf('no')"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.lastIndexOf('')"));
            Assert.AreEqual(3, TestUtils.Evaluate("'onenullthree'.lastIndexOf(null)"));
            Assert.AreEqual(3, TestUtils.Evaluate("'oneundefinedthree'.lastIndexOf(undefined)"));
            

            // length
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(1, TestUtils.Evaluate("''.lastIndexOf.length"));

            // lastIndexOf is generic.
            Assert.AreEqual(2, TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.lastIndexOf; x.f('123')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.lastIndexOf.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.lastIndexOf.call(null)"));
        }

        [TestMethod]
        public void link()
        {
            Assert.AreEqual(@"<a href=""undefined"">haha</a>", (string)TestUtils.Evaluate("'haha'.link()"), true);
            Assert.AreEqual(@"<a href=""http://www.beer.com"">haha</a>", (string)TestUtils.Evaluate("'haha'.link('http://www.beer.com')"), true);
            Assert.AreEqual(1, TestUtils.Evaluate("''.link.length"));
        }

        [TestMethod]
        public void localeCompare()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("'haha'.localeCompare('haha')"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'haha'.localeCompare('HAHA')"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'a'.localeCompare('b')"));
            Assert.AreEqual(1, TestUtils.Evaluate("'b'.localeCompare('a')"));
            Assert.AreEqual(0, TestUtils.Evaluate("'undefined'.localeCompare(undefined)"));
            Assert.AreEqual(0, TestUtils.Evaluate("'null'.localeCompare(null)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("''.localeCompare.length"));

            // localeCompare is generic.
            Assert.AreEqual(0, TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.localeCompare; x.f('6.1234')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.localeCompare.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.localeCompare.call(null)"));
        }

        [TestMethod]
        public void match()
        {
            // Non-global, no match = null.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate("'A long string for testing'.match('nein')"));

            // Non-global, single match only (regexp created from string).
            TestUtils.Evaluate("var result = 'A long string for testing'.match('lo|st')");
            Assert.AreEqual("lo", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual(1, TestUtils.Evaluate("result.length"));
            Assert.AreEqual(2, TestUtils.Evaluate("result.index"));
            Assert.AreEqual("A long string for testing", TestUtils.Evaluate("result.input"));

            // Non-global, single match only (regexp created from string) with subgroups.
            TestUtils.Evaluate("var result = 'A long string for testing'.match('(l)o|s(t)')");
            Assert.AreEqual("lo", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("l", TestUtils.Evaluate("result[1]"));
            if (TestUtils.Engine != JSEngine.JScript)
            {
                Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("result[2]"));
                Assert.AreEqual(true, TestUtils.Evaluate("result.hasOwnProperty(2)"));
            }
            Assert.AreEqual(3, TestUtils.Evaluate("result.length"));
            Assert.AreEqual(2, TestUtils.Evaluate("result.index"));
            Assert.AreEqual("A long string for testing", TestUtils.Evaluate("result.input"));

            // Non-global, no match = null.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate("'A long string for testing'.match(/nein/)"));

            // Non-global, single match only.
            TestUtils.Evaluate("var result = 'A long string for testing'.match(/lo|st/)");
            Assert.AreEqual("lo", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual(1, TestUtils.Evaluate("result.length"));
            Assert.AreEqual(2, TestUtils.Evaluate("result.index"));
            Assert.AreEqual("A long string for testing", TestUtils.Evaluate("result.input"));

            // Non-global, single match only with subgroups.
            TestUtils.Evaluate("var result = 'A long string for testing'.match(/(l)o|s(t)/)");
            Assert.AreEqual("lo", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("l", TestUtils.Evaluate("result[1]"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(result[2])"));
            Assert.AreEqual(3, TestUtils.Evaluate("result.length"));
            Assert.AreEqual(2, TestUtils.Evaluate("result.index"));
            Assert.AreEqual("A long string for testing", TestUtils.Evaluate("result.input"));

            // Non-global, make sure lastIndex is not modified.
            TestUtils.Evaluate("var regex = /lo|st/");
            TestUtils.Evaluate("regex.lastIndex = 18");
            TestUtils.Evaluate("var result = 'A long string for testing'.match(regex)");
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(18, TestUtils.Evaluate("regex.lastIndex"));

            // Global, no match = null.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate("'A long string for testing'.match(/nein/g)"));

            // Global, three matches.
            TestUtils.Evaluate("var result = 'A long string for testing'.match(/lo|st/g)");
            Assert.AreEqual("lo", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("st", TestUtils.Evaluate("result[1]"));
            Assert.AreEqual("st", TestUtils.Evaluate("result[2]"));
            Assert.AreEqual(3, TestUtils.Evaluate("result.length"));
            if (TestUtils.Engine != JSEngine.JScript)
            {
                Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(result.index)"));
                Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(result.input)"));
            }

            // Global, three matches with subgroups (subgroups are not returned with global enabled).
            TestUtils.Evaluate("var result = 'A long string for testing'.match(/(l)o|s(t)/g)");
            Assert.AreEqual("lo", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("st", TestUtils.Evaluate("result[1]"));
            Assert.AreEqual("st", TestUtils.Evaluate("result[2]"));
            Assert.AreEqual(3, TestUtils.Evaluate("result.length"));
            if (TestUtils.Engine != JSEngine.JScript)
            {
                Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(result.index)"));
                Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(result.input)"));
            }

            // Global, make sure lastIndex is set to zero just after match() is called and just before it returns.
            TestUtils.Evaluate("var regex = /lo|st/g");
            TestUtils.Evaluate("regex.lastIndex = 18");
            TestUtils.Evaluate("var result = 'A long string for testing'.match(regex)");
            Assert.AreEqual("lo", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("st", TestUtils.Evaluate("result[1]"));
            Assert.AreEqual("st", TestUtils.Evaluate("result[2]"));
            Assert.AreEqual(3, TestUtils.Evaluate("result.length"));
            TestUtils.Evaluate("regex.lastIndex = 0");

            // Global zero length matches should not hang the browser.
            TestUtils.Evaluate(@"var result = 'A\nB\nC'.match(/^/gm)");
            Assert.AreEqual(3, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("", TestUtils.Evaluate("result[1]"));
            Assert.AreEqual("", TestUtils.Evaluate("result[2]"));

            // Passing undefined is equivalent to passing an empty string.
            Assert.AreEqual(1, TestUtils.Evaluate("''.match().length"));
            Assert.AreEqual("", TestUtils.Evaluate("''.match()[0]"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("''.match.length"));

            // match is generic.
            TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.match; var result = x.f('12')");
            Assert.AreEqual(1, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("12", TestUtils.Evaluate("result[0]"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.match.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.match.call(null)"));

            // Test the deprecated RegExp properties.
            TestUtils.Evaluate("'honey bunny'.match(/n(.)y/)");
            Assert.AreEqual("e", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("honey bunny", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("honey bunny", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("ney", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("ney", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("e", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("e", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("ho", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("ho", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual(" bunny", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual(" bunny", TestUtils.Evaluate("RegExp[\"$'\"]"));

            TestUtils.Evaluate("'honey bunny'.match(/n.?y/g)");
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("honey bunny", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("honey bunny", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("nny", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("nny", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("honey bu", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("honey bu", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp[\"$'\"]"));

            TestUtils.Evaluate("'honey bunny'.match(/(bu|ho)(..)y/g)");
            Assert.AreEqual("bu", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("nn", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$3"));
            Assert.AreEqual("honey bunny", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("honey bunny", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("bunny", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("bunny", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("nn", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("nn", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("honey ", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("honey ", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp[\"$'\"]"));
        }

        [TestMethod]
        public void quote()
        {
            Assert.AreEqual(@"""test""", TestUtils.Evaluate(@"'test'.quote()"));
            Assert.AreEqual(@"""te\""st""", TestUtils.Evaluate(@"'te""st'.quote()"));
            Assert.AreEqual(@"""te'st""", TestUtils.Evaluate(@"""te'st"".quote()"));
        }

        [TestMethod]
        public void replace()
        {
            // replace(string, string)
            Assert.AreEqual("A long string for testing", TestUtils.Evaluate("'A long string for testing'.replace('ew', 'ah!')"));
            Assert.AreEqual("A long ew!ring for testing", TestUtils.Evaluate("'A long string for testing'.replace('st', 'ew!')"));
            Assert.AreEqual("TA short string", TestUtils.Evaluate("'A short string'.replace('', 'T')"));
            
            // replace(regExp, string)
            Assert.AreEqual("A ew!ng string for testing", TestUtils.Evaluate("'A long string for testing'.replace(/lo|st/, 'ew!')"));
            Assert.AreEqual("A ew!ng ew!ring for teew!ing", TestUtils.Evaluate("'A long string for testing'.replace(/lo|st/g, 'ew!')"));
            Assert.AreEqual("[{ ]@ ], ]@ ] }]", TestUtils.Evaluate(@"
                '[{ \""tag\"": ""titillation"", \""popularity\"": 4294967296 }]'.
                replace(/""[^""\\\n\r]*""|true|false|null|-?\d+(?:\.\d*)?(:?[eE][+\-]?\d+)?/g, ']').
                replace(/:/g, '@')"));
            Assert.AreEqual("A $ng $ring for te$ing", TestUtils.Evaluate("'A long string for testing'.replace(/lo|st/g, '$$')"));
            Assert.AreEqual("A ${test}ng ${test}ring for te${test}ing", TestUtils.Evaluate("'A long string for testing'.replace(/lo|st/g, '${test}')"));
            Assert.AreEqual("A <lo>ng <st>ring for te<st>ing", TestUtils.Evaluate("'A long string for testing'.replace(/lo|st/g, '<$&>')"));
            Assert.AreEqual("A short <A short >ring", TestUtils.Evaluate("'A short string'.replace(/lo|st/g, '<$`>')"));
            Assert.AreEqual("A short <ring>ring", TestUtils.Evaluate(@"'A short string'.replace(/lo|st/g, '<$\'>')"));
            Assert.AreEqual("A l  $3 l0ng  t $3 0ring for te t $3 0ing", TestUtils.Evaluate("'A long string for testing'.replace(/(l)o|s(t)/g, '$1 $2 $3 $10')"));
            Assert.AreEqual("A long string g", TestUtils.Evaluate("'A long string for testing'.replace(/(f)(o)(r)( )(t)(e)(s)(t)(i)(n)(g)/g, '$11')"));
            Assert.AreEqual("$1-$11,$1-$22", TestUtils.Evaluate(@"'$1,$2'.replace(/(\$(\d))/g, '$$1-$1$2')"));
            Assert.AreEqual("$es$ing", TestUtils.Evaluate(@"'testing'.replace(/t/g, '$')"));

            // replace(regExp, function)
            Assert.AreEqual("blah def34", TestUtils.Evaluate("'abc12 def34'.replace(/([a-z]+)([0-9]+)/, function() { return 'blah' })"));
            Assert.AreEqual("12abc def34", TestUtils.Evaluate("'abc12 def34'.replace(/([a-z]+)([0-9]+)/, function() { return arguments[2] + arguments[1] })"));
            Assert.AreEqual("A aort aring", TestUtils.Evaluate("'A short string'.replace(/(s)h|s(t)/g, function() { return 'a'; })"));
            TestUtils.Execute(@"var parameterValues = []");
            TestUtils.Evaluate("'A short string'.replace(/(s)h|s(t)/g, function() { parameterValues.push(arguments); })");
            Assert.AreEqual(2, TestUtils.Evaluate("parameterValues.length"));
            Assert.AreEqual(5, TestUtils.Evaluate("parameterValues[0].length"));
            Assert.AreEqual("sh", TestUtils.Evaluate("parameterValues[0][0]"));
            Assert.AreEqual("s", TestUtils.Evaluate("parameterValues[0][1]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(parameterValues[0][2])"));
            Assert.AreEqual(true, TestUtils.Evaluate("parameterValues[0].hasOwnProperty(2)"));
            Assert.AreEqual(2, TestUtils.Evaluate("parameterValues[0][3]"));
            Assert.AreEqual("A short string", TestUtils.Evaluate("parameterValues[0][4]"));
            Assert.AreEqual(5, TestUtils.Evaluate("parameterValues[1].length"));
            Assert.AreEqual("st", TestUtils.Evaluate("parameterValues[1][0]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(parameterValues[1][1])"));
            Assert.AreEqual("t", TestUtils.Evaluate("parameterValues[1][2]"));
            Assert.AreEqual(8, TestUtils.Evaluate("parameterValues[1][3]"));
            Assert.AreEqual("A short string", TestUtils.Evaluate("parameterValues[1][4]"));

            Assert.AreEqual("undefined runny", TestUtils.Evaluate("arr = []; 'funny runny'.replace(/(..)nny/, function(a, b, c, d) { arr.push(a); arr.push(b); arr.push(c); arr.push(d) })"));
            Assert.AreEqual("funny,fu,0,funny runny", TestUtils.Evaluate("arr.toString()"));

            TestUtils.Execute(@"var parameterValues = []");
            TestUtils.Execute(@"'Type some [Markdown] text on the left side.'.replace(/(\[([^\[\]]+)\])()()()()()/g, function(wholeMatch,m1,m2,m3,m4,m5,m6,m7) {parameterValues.push(arguments)})");
            Assert.AreEqual(1, TestUtils.Evaluate("parameterValues.length"));
            Assert.AreEqual(10, TestUtils.Evaluate("parameterValues[0].length"));
            Assert.AreEqual("[Markdown]", TestUtils.Evaluate("parameterValues[0][0]"));
            Assert.AreEqual("[Markdown]", TestUtils.Evaluate("parameterValues[0][1]"));
            Assert.AreEqual("Markdown", TestUtils.Evaluate("parameterValues[0][2]"));
            Assert.AreEqual("", TestUtils.Evaluate("parameterValues[0][3]"));
            Assert.AreEqual("", TestUtils.Evaluate("parameterValues[0][4]"));
            Assert.AreEqual("", TestUtils.Evaluate("parameterValues[0][5]"));
            Assert.AreEqual("", TestUtils.Evaluate("parameterValues[0][6]"));
            Assert.AreEqual("", TestUtils.Evaluate("parameterValues[0][7]"));
            Assert.AreEqual(10, TestUtils.Evaluate("parameterValues[0][8]"));
            Assert.AreEqual("Type some [Markdown] text on the left side.", TestUtils.Evaluate("parameterValues[0][9]"));

            // replace(string, function)
            Assert.AreEqual("A short string", TestUtils.Evaluate("'A short string'.replace('test', function() { return 'a'; })"));
            Assert.AreEqual("A ahort string", TestUtils.Evaluate("'A short string'.replace('s', function() { return 'a'; })"));
            Assert.AreEqual("A long string", TestUtils.Evaluate("'A short string'.replace('short', function() { return 'long'; })"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("''.replace.length"));

            // replace is generic.
            Assert.AreEqual("6.4334", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.replace; x.f('12', '43')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.replace.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.replace.call(null)"));

            // Test the deprecated RegExp properties.
            Assert.AreEqual(" runny", TestUtils.Evaluate("'funny runny'.replace(/(..)nny/, '')"));
            Assert.AreEqual("fu", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("funny runny", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("funny runny", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("funny", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("funny", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("fu", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("fu", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual(" runny", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual(" runny", TestUtils.Evaluate("RegExp[\"$'\"]"));

            Assert.AreEqual(" ", TestUtils.Evaluate("'funny runny'.replace(/(..)nny/g, '')"));
            Assert.AreEqual("ru", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("funny runny", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("funny runny", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("runny", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("runny", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("ru", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("ru", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("funny ", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("funny ", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp[\"$'\"]"));

            Assert.AreEqual("funny runny", TestUtils.Evaluate("'funny runny'.replace(/boo/g, '')"));
            Assert.AreEqual("ru", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("funny runny", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("funny runny", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("runny", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("runny", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("ru", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("ru", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("funny ", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("funny ", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp[\"$'\"]"));

            Assert.AreEqual("fu,funny,fu,, runny", TestUtils.Evaluate(@"arr = []; 'funny runny'.replace(/(..)nny/, function() {
                    arr.push(RegExp.$1);
                    arr.push(RegExp.lastMatch);
                    arr.push(RegExp.lastParen);
                    arr.push(RegExp.leftContext);
                    arr.push(RegExp.rightContext);
                }); arr.toString()"));
            Assert.AreEqual("fu,funny,fu,, runny,ru,runny,ru,funny ,", TestUtils.Evaluate(@"arr = []; 'funny runny'.replace(/(..)nny/g, function() {
                    arr.push(RegExp.$1);
                    arr.push(RegExp.lastMatch);
                    arr.push(RegExp.lastParen);
                    arr.push(RegExp.leftContext);
                    arr.push(RegExp.rightContext);
                }); arr.toString()"));

            // "this" should be the global object in non-strict mode.
            Assert.AreEqual(true, TestUtils.Evaluate(@"
                var global = this;
                var success = false;
                'test'.replace('e', function (x, y) {
                    success = this === global;
                });
                success"));

            // "this" should be undefined in strict mode.
            Assert.AreEqual(true, TestUtils.Evaluate(@"
                'use strict';
                var success = false;
                'test'.replace('e', function (x, y) {
                    success = this === undefined;
                });
                success"));
        }

        [TestMethod]
        public void search()
        {
            Assert.AreEqual(-1, TestUtils.Evaluate("'A long string for testing'.search('nein')"));
            Assert.AreEqual(7, TestUtils.Evaluate("'A long string for testing'.search('st')"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'A long string for testing'.search(/nein/)"));
            Assert.AreEqual(7, TestUtils.Evaluate("'A long string for testing'.search(/st/)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("'A long string for testing'.search(/nein/g)"));
            Assert.AreEqual(7, TestUtils.Evaluate("'A long string for testing'.search(/st/g)"));

            // Make sure lastIndex is not modified.
            TestUtils.Evaluate("var regex = /lo|st/");
            TestUtils.Evaluate("regex.lastIndex = 15");
            Assert.AreEqual(2, TestUtils.Evaluate("'A long string for testing'.search(regex)"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(15, TestUtils.Evaluate("regex.lastIndex"));
            TestUtils.Evaluate("regex = /lo|st/g");
            TestUtils.Evaluate("regex.lastIndex = 15");
            Assert.AreEqual(2, TestUtils.Evaluate("'A long string for testing'.search(regex)"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(15, TestUtils.Evaluate("regex.lastIndex"));

            // Passing undefined to is equivalent to passing an empty string.
            Assert.AreEqual(0, TestUtils.Evaluate("''.search('')"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.search()"));
            Assert.AreEqual(0, TestUtils.Evaluate("'--undefined--'.search()"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.search(undefined)"));

            // length
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(1, TestUtils.Evaluate("''.search.length"));

            // search is generic.
            Assert.AreEqual(2, TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.search; x.f('12')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.search.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.search.call(null)"));

            // Test the deprecated RegExp properties.
            Assert.AreEqual(7, TestUtils.Evaluate("'lots of honey'.search(/(...)ney/)"));
            Assert.AreEqual(" ho", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("lots of honey", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("lots of honey", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual(" honey", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual(" honey", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual(" ho", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual(" ho", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("lots of", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("lots of", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp[\"$'\"]"));

            Assert.AreEqual(-1, TestUtils.Evaluate("'tons of honey'.search(/nomatch/)"));
            Assert.AreEqual(" ho", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("lots of honey", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("lots of honey", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual(" honey", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual(" honey", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual(" ho", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual(" ho", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("lots of", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("lots of", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp[\"$'\"]"));
        }

        [TestMethod]
        public void slice()
        {
            Assert.AreEqual("testing", TestUtils.Evaluate("'A long string for testing'.slice(18)"));
            Assert.AreEqual("ing", TestUtils.Evaluate("'A long string for testing'.slice(-3)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.slice(40)"));
            Assert.AreEqual("A long string for testing", TestUtils.Evaluate("'A long string for testing'.slice(-40)"));
            Assert.AreEqual("on", TestUtils.Evaluate("'A long string for testing'.slice(3, 5)"));
            Assert.AreEqual("te", TestUtils.Evaluate("'A long string for testing'.slice(-7, 20)"));
            Assert.AreEqual("te", TestUtils.Evaluate("'A long string for testing'.slice(18, -5)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.slice(19, -10)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.slice(19, -40)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.slice(12, 10)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.slice(40, 10)"));

            // Index arguments use ToInteger().
            Assert.AreEqual("abc", TestUtils.Evaluate("'abc'.slice(0, Infinity)"));
            Assert.AreEqual("abc", TestUtils.Evaluate("'abc'.slice(0, 10000000000)"));
            Assert.AreEqual("abc", TestUtils.Evaluate("'abc'.slice(-Infinity)"));
            Assert.AreEqual("abc", TestUtils.Evaluate("'abc'.slice(-10000000000)"));
            Assert.AreEqual("", TestUtils.Evaluate("'abc'.slice(Infinity)"));
            Assert.AreEqual("", TestUtils.Evaluate("'abc'.slice(0, -Infinity)"));

            // length
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(2, TestUtils.Evaluate("''.slice.length"));

            // slice is generic.
            Assert.AreEqual("23", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.slice; x.f(3, 5)"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.slice.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.slice.call(null)"));
        }

        [TestMethod]
        public void small()
        {
            Assert.AreEqual("<small>haha</small>", (string)TestUtils.Evaluate("'haha'.small()"), true);
            Assert.AreEqual(0, TestUtils.Evaluate("''.small.length"));
        }

        [TestMethod]
        public void split()
        {
            // String splits.
            TestUtils.Evaluate("var result = 'A short string'.split(' ')");
            Assert.AreEqual(3, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("A", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("short", TestUtils.Evaluate("result[1]"));
            Assert.AreEqual("string", TestUtils.Evaluate("result[2]"));

            TestUtils.Evaluate("var result = '5,,7'.split(',')");
            Assert.AreEqual(3, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("5", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("", TestUtils.Evaluate("result[1]"));
            Assert.AreEqual("7", TestUtils.Evaluate("result[2]"));

            // String split (with limit).
            TestUtils.Evaluate("var result = '5,,7'.split(',', 2)");
            Assert.AreEqual(2, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("5", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("", TestUtils.Evaluate("result[1]"));
            TestUtils.Evaluate("var result = '5,,7'.split(',', -1)");
            Assert.AreEqual(3, TestUtils.Evaluate("result.length"));

            // Regex splits.
            TestUtils.Evaluate(@"var result = 'A long string for testing'.split(/lo|st/)");
            Assert.AreEqual(4, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("A ", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("ng ", TestUtils.Evaluate("result[1]"));
            Assert.AreEqual("ring for te", TestUtils.Evaluate("result[2]"));
            Assert.AreEqual("ing", TestUtils.Evaluate("result[3]"));

            // Regex split (with limit).
            TestUtils.Evaluate(@"var result = 'A long string for testing'.split(/i/, 1)");
            Assert.AreEqual(1, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("A long str", TestUtils.Evaluate("result[0]"));

            // Regex splits with subgroups.
            if (TestUtils.Engine != JSEngine.JScript)
            {
                TestUtils.Evaluate(@"var result = 'A<B>bold</B>and<CODE>coded</CODE>'.split(/<(\/)?([^<>]+)>/)");
                Assert.AreEqual(13, TestUtils.Evaluate("result.length"));
                Assert.AreEqual("A", TestUtils.Evaluate("result[0]"));
                Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(result[1])"));
                Assert.AreEqual("B", TestUtils.Evaluate("result[2]"));
                Assert.AreEqual("bold", TestUtils.Evaluate("result[3]"));
                Assert.AreEqual("/", TestUtils.Evaluate("result[4]"));
                Assert.AreEqual("B", TestUtils.Evaluate("result[5]"));
                Assert.AreEqual("and", TestUtils.Evaluate("result[6]"));
                Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(result[7])"));
                Assert.AreEqual("CODE", TestUtils.Evaluate("result[8]"));
                Assert.AreEqual("coded", TestUtils.Evaluate("result[9]"));
                Assert.AreEqual("/", TestUtils.Evaluate("result[10]"));
                Assert.AreEqual("CODE", TestUtils.Evaluate("result[11]"));
                Assert.AreEqual("", TestUtils.Evaluate("result[12]"));

                // Do not match the empty substring at the start and end of the string, or at the
                // end of a previous match.
                Assert.AreEqual(@"[""o"",null,""n"",null,""e"",""t"","""",""w"",""o"",""t"",""h"",null,""r"",null,""e"",null,""e""]",
                    TestUtils.Evaluate("JSON.stringify('onetwothree'.split(/(t|w)?/))"));

                // Try a regex with multiple captures.
                Assert.AreEqual(@"[""one"",""w"",""o"",""t"",""hree""]",
                    TestUtils.Evaluate("JSON.stringify('onetwothree'.split(/(t|w)+/))"));
                Assert.AreEqual(@"["""",""e"",""""]",
                    TestUtils.Evaluate("JSON.stringify('onetwothree'.split(/([a-z])+/))"));

                // Test the limit argument.
                Assert.AreEqual(@"[""o"",null,null,""n"",""et""]",
                    TestUtils.Evaluate("JSON.stringify('onetwothree'.split(/(et)?(wo)?/, 5))"));
            }

            // Spec violation but de-facto standard: undefined is converted to 'undefined'.
            Assert.AreEqual(2, TestUtils.Evaluate("'teundefinedst'.split(undefined).length"));

            // Splitting by an empty string splits the string into individual characters.
            Assert.AreEqual("a,b,c", TestUtils.Evaluate("'abc'.split('').toString()"));
            Assert.AreEqual("a,b,c", TestUtils.Evaluate("'abc'.split(new RegExp()).toString()"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("''.split.length"));

            // split is generic.
            TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.split; var result = x.f('2')");
            Assert.AreEqual(2, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("6.1", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("34", TestUtils.Evaluate("result[1]"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.split.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.split.call(null)"));

            // Test the deprecated RegExp properties.
            TestUtils.Evaluate("'lots of money and honey'.split(/(..)ney/)");
            Assert.AreEqual("ho", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("lots of money and honey", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("lots of money and honey", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("honey", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("honey", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("ho", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("ho", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("lots of money and ", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("lots of money and ", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp[\"$'\"]"));

            TestUtils.Evaluate("'tons of money and honey'.split(/nomatch/)");
            Assert.AreEqual("ho", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("lots of money and honey", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("lots of money and honey", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("honey", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("honey", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("ho", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("ho", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("lots of money and ", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("lots of money and ", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp[\"$'\"]"));
        }

        [TestMethod]
        public void strike()
        {
            Assert.AreEqual("<strike>haha</strike>", (string)TestUtils.Evaluate("'haha'.strike()"), true);
            Assert.AreEqual(0, TestUtils.Evaluate("''.strike.length"));
        }

        [TestMethod]
        public void sub()
        {
            Assert.AreEqual("<sub>haha</sub>", (string)TestUtils.Evaluate("'haha'.sub()"), true);
            Assert.AreEqual(0, TestUtils.Evaluate("''.sub.length"));
        }

        [TestMethod]
        public void substr()
        {
            Assert.AreEqual("testing", TestUtils.Evaluate("'A long string for testing'.substr(18)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.substr(40)"));
            Assert.AreEqual("ong s", TestUtils.Evaluate("'A long string for testing'.substr(3, 5)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.substr(18, -5)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.substr(19, -10)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.substr(19, -40)"));
            Assert.AreEqual("g for test", TestUtils.Evaluate("'A long string for testing'.substr(12, 10)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.substr(40, 10)"));

            // IE disagrees with the spec w.r.t. the meaning of a negative start parameter.
            if (TestUtils.Engine != JSEngine.JScript)
            {
                Assert.AreEqual("ing", TestUtils.Evaluate("'A long string for testing'.substr(-3)"));
                Assert.AreEqual("A long string for testing", TestUtils.Evaluate("'A long string for testing'.substr(-40)"));
                Assert.AreEqual("testing", TestUtils.Evaluate("'A long string for testing'.substr(-7, 20)"));
            }

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("''.substr.length"));

            // substr is generic.
            Assert.AreEqual("23", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.substr; x.f(3, 2)"));
        }

        [TestMethod]
        public void substring()
        {
            Assert.AreEqual("testing", TestUtils.Evaluate("'A long string for testing'.substring(18)"));
            Assert.AreEqual("A long string for testing", TestUtils.Evaluate("'A long string for testing'.substring(-3)"));
            Assert.AreEqual("", TestUtils.Evaluate("'A long string for testing'.substring(40)"));
            Assert.AreEqual("A long string for testing", TestUtils.Evaluate("'A long string for testing'.substring(-40)"));
            Assert.AreEqual("on", TestUtils.Evaluate("'A long string for testing'.substring(3, 5)"));
            Assert.AreEqual("A long string for te", TestUtils.Evaluate("'A long string for testing'.substring(-7, 20)"));
            Assert.AreEqual("A long string for ", TestUtils.Evaluate("'A long string for testing'.substring(18, -5)"));
            Assert.AreEqual("A long string for t", TestUtils.Evaluate("'A long string for testing'.substring(19, -10)"));
            Assert.AreEqual("A long string for t", TestUtils.Evaluate("'A long string for testing'.substring(19, -40)"));
            Assert.AreEqual("in", TestUtils.Evaluate("'A long string for testing'.substring(12, 10)"));
            Assert.AreEqual("ing for testing", TestUtils.Evaluate("'A long string for testing'.substring(40, 10)"));
            Assert.AreEqual("foo", TestUtils.Evaluate("'foo'.substring(0, undefined)"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("''.substring.length"));

            // substring is generic.
            Assert.AreEqual("23", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.substring; x.f(5, 3)"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.substring.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.substring.call(null)"));
        }

        [TestMethod]
        public void sup()
        {
            Assert.AreEqual("<sup>haha</sup>", (string)TestUtils.Evaluate("'haha'.sup()"), true);
            Assert.AreEqual(0, TestUtils.Evaluate("''.sup.length"));
        }

        [TestMethod]
        public void trim()
        {
            if (TestUtils.Engine == JSEngine.JScript)
                Assert.Inconclusive("JScript does not support String.prototype.trim.");
            Assert.AreEqual("hello world", TestUtils.Evaluate("'  hello world  '.trim()"));
            Assert.AreEqual("6.1234", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.trim; x.f()"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.trim.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.trim.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.trim.call(null)"));
        }

        [TestMethod]
        public void toLocaleLowerCase()
        {
            Assert.AreEqual("hello world", TestUtils.Evaluate("'Hello World'.toLocaleLowerCase()"));
            Assert.AreEqual("6.1234", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.toLocaleLowerCase; x.f()"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.toLocaleLowerCase.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.toLocaleLowerCase.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.toLocaleLowerCase.call(null)"));
        }

        [TestMethod]
        public void toLocaleUpperCase()
        {
            Assert.AreEqual("HELLO WORLD", TestUtils.Evaluate("'Hello World'.toLocaleUpperCase()"));
            Assert.AreEqual("6.1234", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.toLocaleUpperCase; x.f()"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.toLocaleUpperCase.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.toLocaleUpperCase.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.toLocaleUpperCase.call(null)"));
        }

        [TestMethod]
        public void toLowerCase()
        {
            Assert.AreEqual("hello world", TestUtils.Evaluate("'Hello World'.toLowerCase()"));
            Assert.AreEqual("6.1234", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.toLowerCase; x.f()"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.toLowerCase.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.toLowerCase.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.toLowerCase.call(null)"));
        }

        [TestMethod]
        public void toUpperCase()
        {
            Assert.AreEqual("HELLO WORLD", TestUtils.Evaluate("'Hello World'.toUpperCase()"));
            Assert.AreEqual("6.1234", TestUtils.Evaluate("x = new Number(6.1234); x.f = ''.toUpperCase; x.f()"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.toUpperCase.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.toUpperCase.call(undefined)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.toUpperCase.call(null)"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("hello world", TestUtils.Evaluate("'hello world'.toString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("''.toString.length"));

            // toString is not generic.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.toString.call(5)"));
        }

        [TestMethod]
        public void valueOf()
        {
            Assert.AreEqual("hello world", TestUtils.Evaluate("'hello world'.valueOf()"));
            Assert.AreEqual(0, TestUtils.Evaluate("'hello world'.valueOf.length"));

            // valueOf is not generic.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("''.valueOf.call(5)"));
        }
    }
}
