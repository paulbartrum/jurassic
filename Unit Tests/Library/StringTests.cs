using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global String object.
    /// </summary>
    [TestClass]
    public class StringTests : TestBase
    {
        
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("", Evaluate("String()"));
            Assert.AreEqual("undefined", Evaluate("String(undefined)"));
            Assert.AreEqual("null", Evaluate("String(null)"));
            Assert.AreEqual("5.1", Evaluate("String(5.1)"));
            Assert.AreEqual("510000", Evaluate("String(5.1e5)"));
            Assert.AreEqual("100000000000000000000", Evaluate("String(100000000000000000000)"));
            Assert.AreEqual("1e+21", Evaluate("String(1000000000000000000000)"));
            Assert.AreEqual("NaN", Evaluate("String(NaN)"));
            Assert.AreEqual("", Evaluate("String('')"));
            Assert.AreEqual("deadline", Evaluate("String('deadline')"));

            // Construct
            Assert.AreEqual("", Evaluate("new String().valueOf()"));
            Assert.AreEqual("undefined", Evaluate("new String(undefined).valueOf()"));
            Assert.AreEqual("null", Evaluate("new String(null).valueOf()"));
            Assert.AreEqual("5.1", Evaluate("new String(5.1).valueOf()"));
            Assert.AreEqual("NaN", Evaluate("new String(NaN).valueOf()"));
            Assert.AreEqual("", Evaluate("new String('').valueOf()"));
            Assert.AreEqual("deadline", Evaluate("new String('deadline').valueOf()"));

            // toString and valueOf.
            Assert.AreEqual("function String() { [native code] }", Evaluate("String.toString()"));
            Assert.AreEqual(true, Evaluate("String.valueOf() === String"));

            // length
            Assert.AreEqual(1, Evaluate("String.length"));
        }

        [TestMethod]
        public void Properties()
        {
            // Constructor and __proto__
            Assert.AreEqual(true, Evaluate("new String().constructor === String"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(new String()) === String.prototype"));

            // length is enumerable, non-configurable, non-writable
            Assert.AreEqual(3, Evaluate("'abc'.length"));
            Assert.AreEqual(0, Evaluate("var x = ''; x.length = 5; x.length"));
            Assert.AreEqual(false, Evaluate("var x = 'abc'; delete x.length"));
            Assert.AreEqual(3, Evaluate("var x = 'abc'; delete x.length; x.length"));

            // String can be indexed like arrays.
            Assert.AreEqual("a", Evaluate("'abc'[0]"));
            Assert.AreEqual("b", Evaluate("'abc'[1]"));
            Assert.AreEqual("c", Evaluate("new String('abc')[2]"));
            Assert.AreEqual(Undefined.Value, Evaluate("'abc'[-1]"));
            Assert.AreEqual(Undefined.Value, Evaluate("'abc'[3]"));

            // The array indices are enumerable.
            Assert.AreEqual("01234", Evaluate("y = ''; for (var x in 'hello') { y += x } y"));

            // Array elements cannot be modified within the range of the string, but can be modified otherwise.
            Assert.AreEqual("a", Evaluate("var x = new String('abc'); x[0] = 'c'; x[0]"));
            Assert.AreEqual("c", Evaluate("var x = new String('abc'); x[100] = 'c'; x[100]"));
            Assert.AreEqual("c", Evaluate("var x = new String('abc'); x.a = 'c'; x.a"));
        }

        [TestMethod]
        public void fromCharCode()
        {
            Assert.AreEqual("", Evaluate("String.fromCharCode()"));
            Assert.AreEqual("ha", Evaluate("String.fromCharCode(104, 97)"));
            Assert.AreEqual("!", Evaluate("String.fromCharCode(5999951905)"));

            // length
            Assert.AreEqual(1, Evaluate("String.fromCharCode.length"));

            // No this object is required.
            Assert.AreEqual("ha", Evaluate("var f = String.fromCharCode; f(104, 97)"));
        }

        [TestMethod]
        public void fromCodePoint()
        {
            Assert.AreEqual("", Evaluate("String.fromCodePoint()"));
            Assert.AreEqual("ha", Evaluate("String.fromCodePoint(104, 97)"));
            Assert.AreEqual("𠮷", Evaluate("String.fromCodePoint(134071)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("String.fromCodePoint(1.5)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("String.fromCodePoint(-5)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("String.fromCodePoint(5999951905)"));

            // length
            Assert.AreEqual(1, Evaluate("String.fromCodePoint.length"));

            // No this object is required.
            Assert.AreEqual("ha", Evaluate("var f = String.fromCodePoint; f(104, 97)"));
        }

        [TestMethod]
        public void anchor()
        {
            Assert.AreEqual(@"<a name=""undefined"">haha</a>", (string)Evaluate("'haha'.anchor()"), true);
            Assert.AreEqual(@"<a name=""test"">haha</a>", (string)Evaluate("'haha'.anchor('test')"), true);
            Assert.AreEqual(1, Evaluate("''.anchor.length"));
        }

        [TestMethod]
        public void big()
        {
            Assert.AreEqual("<big>haha</big>", (string)Evaluate("'haha'.big()"), true);
            Assert.AreEqual(0, Evaluate("''.big.length"));
        }

        [TestMethod]
        public void blink()
        {
            Assert.AreEqual("<blink>haha</blink>", (string)Evaluate("'haha'.blink()"), true);
            Assert.AreEqual(0, Evaluate("''.blink.length"));
        }

        [TestMethod]
        public void bold()
        {
            Assert.AreEqual("<b>haha</b>", (string)Evaluate("'haha'.bold()"), true);
            Assert.AreEqual(0, Evaluate("''.bold.length"));
        }

        [TestMethod]
        public void charAt()
        {
            Assert.AreEqual("h", Evaluate("'haha'.charAt(0)"));
            Assert.AreEqual("a", Evaluate("'haha'.charAt(1)"));
            Assert.AreEqual("h", Evaluate("'haha'.charAt(2)"));
            Assert.AreEqual("a", Evaluate("'haha'.charAt(3)"));
            Assert.AreEqual("", Evaluate("'haha'.charAt(-1)"));
            Assert.AreEqual("", Evaluate("'haha'.charAt(4)"));

            // Index argument uses ToInteger().
            Assert.AreEqual("", Evaluate("'abc'.charAt(Infinity)"));
            Assert.AreEqual("", Evaluate("'abc'.charAt(-Infinity)"));
            Assert.AreEqual("a", Evaluate("'abc'.charAt(NaN)"));
            Assert.AreEqual("", Evaluate("'abc'.charAt(4294967298)"));
            Assert.AreEqual("a", Evaluate("'abc'.charAt(null)"));
            Assert.AreEqual("a", Evaluate("'abc'.charAt(undefined)"));
            Assert.AreEqual("b", Evaluate("'abc'.charAt(true)"));
            Assert.AreEqual("a", Evaluate("'abc'.charAt(false)"));

            // length
            Assert.AreEqual(1, Evaluate("''.charAt.length"));

            // charAt is generic.
            Assert.AreEqual("2", Evaluate("x = new Number(6.1234); x.f = ''.charAt; x.f(3)"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.charAt.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.charAt.call(null)"));
        }

        [TestMethod]
        public void charCodeAt()
        {
            Assert.AreEqual(104, Evaluate("'haha'.charCodeAt(0)"));
            Assert.AreEqual(97, Evaluate("'haha'.charCodeAt(1)"));
            Assert.AreEqual(104, Evaluate("'haha'.charCodeAt(2)"));
            Assert.AreEqual(97, Evaluate("'haha'.charCodeAt(3)"));
            Assert.AreEqual(double.NaN, Evaluate("'haha'.charCodeAt(-1)"));
            Assert.AreEqual(double.NaN, Evaluate("'haha'.charCodeAt(4)"));
            Assert.AreEqual(104, Evaluate("'haha'.charCodeAt()"));
            Assert.AreEqual(1, Evaluate("''.charCodeAt.length"));

            // Strings must be UTF-16 in ECMAScript 6.
            Assert.AreEqual(55362, Evaluate("'𠮷a'.charCodeAt(0)"));
            Assert.AreEqual(57271, Evaluate("'𠮷a'.charCodeAt(1)"));
            Assert.AreEqual(97, Evaluate("'𠮷a'.charCodeAt(2)"));

            // length
            Assert.AreEqual(1, Evaluate("''.charCodeAt.length"));

            // charCodeAt is generic.
            Assert.AreEqual(50, Evaluate("x = new Number(6.1234); x.f = ''.charCodeAt; x.f(3)"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.charCodeAt.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.charCodeAt.call(null)"));
        }

        [TestMethod]
        public void codePointAt()
        {
            Assert.AreEqual(134071, Evaluate("'𠮷a'.codePointAt(0)"));
            Assert.AreEqual(57271, Evaluate("'𠮷a'.codePointAt(1)"));
            Assert.AreEqual(97, Evaluate("'𠮷a'.codePointAt(2)"));

            // length
            Assert.AreEqual(1, Evaluate("''.charCodeAt.length"));

            // charCodeAt is generic.
            Assert.AreEqual(50, Evaluate("x = new Number(6.1234); x.f = ''.charCodeAt; x.f(3)"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.charCodeAt.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.charCodeAt.call(null)"));
        }

        [TestMethod]
        public void concat()
        {
            Assert.AreEqual("one", Evaluate("'one'.concat()"));
            Assert.AreEqual("onetwo", Evaluate("'one'.concat('two')"));
            Assert.AreEqual("onetwothree", Evaluate("'one'.concat('two', 'three')"));
            Assert.AreEqual("oneundefined", Evaluate("'one'.concat(undefined)"));
            Assert.AreEqual("onenull", Evaluate("'one'.concat(null)"));

            // concat does not change the original string.
            Assert.AreEqual("onetwo", Evaluate("var x = 'one'; x.concat('two')"));
            Assert.AreEqual("one", Evaluate("var x = 'one'; x.concat('two'); x"));
            Assert.AreEqual("onetwo", Evaluate("var x = 'one'; x += 'two'; x.concat();"));
            Assert.AreEqual("onetwothree", Evaluate("var x = 'one'; x += 'two'; x.concat('three');"));
            Assert.AreEqual("onetwo", Evaluate("var x = 'one'; x += 'two'; x.concat('three'); x"));
            Assert.AreEqual("onetwothreefour", Evaluate("var x = 'one'; x += 'two'; x.concat('three', 'four');"));
            Assert.AreEqual("onetwo", Evaluate("var x = 'one'; x += 'two'; x.concat('three', 'four'); x"));

            // length
            Assert.AreEqual(1, Evaluate("''.concat.length"));

            // concat is generic.
            Assert.AreEqual("6.1234300", Evaluate("x = new Number(6.1234); x.f = ''.concat; x.f(300)"));
            Assert.AreEqual("first", Evaluate(@"
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
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.concat.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.concat.call(null)"));
        }

        [TestMethod]
        public void @fixed()
        {
            Assert.AreEqual("<tt>haha</tt>", (string)Evaluate("'haha'.fixed()"), true);
            Assert.AreEqual(0, Evaluate("''.fixed.length"));
        }

        [TestMethod]
        public void fontcolor()
        {
            Assert.AreEqual(@"<font color=""undefined"">haha</font>", (string)Evaluate("'haha'.fontcolor()"), true);
            Assert.AreEqual(@"<font color=""red"">haha</font>", (string)Evaluate("'haha'.fontcolor('red')"), true);
            Assert.AreEqual(1, Evaluate("''.fontcolor.length"));
        }

        [TestMethod]
        public void fontsize()
        {
            Assert.AreEqual(@"<font size=""5"">haha</font>", (string)Evaluate("'haha'.fontsize(5)"), true);
            Assert.AreEqual(@"<font size=""abc"">haha</font>", (string)Evaluate("'haha'.fontsize('abc')"), true);
            Assert.AreEqual(@"<font size=""undefined"">haha</font>", (string)Evaluate("'haha'.fontsize()"), true);
            Assert.AreEqual(1, Evaluate("''.fontsize.length"));
        }

        [TestMethod]
        public void includes()
        {
            Assert.AreEqual(true, Evaluate("'onetwothree'.includes('two')"));
            Assert.AreEqual(true, Evaluate("'onetwothree'.includes('t')"));
            Assert.AreEqual(true, Evaluate("'onetwothree'.includes('two', 2)"));
            Assert.AreEqual(true, Evaluate("'onetwothree'.includes('two', 3)"));
            Assert.AreEqual(false, Evaluate("'onetwothree'.includes('two', 4)"));
            Assert.AreEqual(true, Evaluate("'onetwothree'.includes('two', -400)"));
            Assert.AreEqual(false, Evaluate("'onetwothree'.includes('two', 400)"));
            Assert.AreEqual(false, Evaluate("'onetwothree'.includes('no')"));
            Assert.AreEqual(false, Evaluate("'onetwothree'.includes('e', 400)"));
            Assert.AreEqual(false, Evaluate("''.includes('no')"));
            Assert.AreEqual(true, Evaluate("''.includes('')"));

            // The substring cannot be a regular expression.
            Assert.AreEqual("TypeError", EvaluateExceptionType("'onetwothree'.includes(/two/)"));

            // length
            Assert.AreEqual(1, Evaluate("''.includes.length"));

            // includes is generic.
            Assert.AreEqual(true, Evaluate("x = new Number(6.1234); x.f = ''.includes; x.f('123')"));
            Assert.AreEqual(true, Evaluate("x = new Date(0); x.f = ''.includes; x.getTimezoneOffset() > 0 ? x.f('31') : x.f('01')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.includes.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.includes.call(null)"));
        }

        [TestMethod]
        public void indexOf()
        {
            Assert.AreEqual(3, Evaluate("'onetwothree'.indexOf('two')"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.indexOf('t')"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.indexOf('two', 2)"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.indexOf('two', 3)"));
            Assert.AreEqual(-1, Evaluate("'onetwothree'.indexOf('two', 4)"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.indexOf('two', -400)"));
            Assert.AreEqual(-1, Evaluate("'onetwothree'.indexOf('two', 400)"));
            Assert.AreEqual(-1, Evaluate("'onetwothree'.indexOf('no')"));
            Assert.AreEqual(-1, Evaluate("'onetwothree'.indexOf('e', 400)"));
            Assert.AreEqual(-1, Evaluate("''.indexOf('no')"));
            Assert.AreEqual(0, Evaluate("''.indexOf('')"));

            // length
            Assert.AreEqual(1, Evaluate("''.indexOf.length"));

            // indexOf is generic.
            Assert.AreEqual(2, Evaluate("x = new Number(6.1234); x.f = ''.indexOf; x.f('123')"));
            Assert.AreEqual(8, Evaluate("x = new Date(0); x.f = ''.indexOf; x.getTimezoneOffset() > 0 ? x.f('31') : x.f('01')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.indexOf.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.indexOf.call(null)"));
        }

        [TestMethod]
        public void italics()
        {
            Assert.AreEqual("<i>haha</i>", (string)Evaluate("'haha'.italics()"), true);
            Assert.AreEqual(0, Evaluate("'haha'.italics.length"));
        }

        [TestMethod]
        public void lastIndexOf()
        {
            Assert.AreEqual(3, Evaluate("'onetwothree'.lastIndexOf('two')"));
            Assert.AreEqual(6, Evaluate("'onetwothree'.lastIndexOf('t')"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.lastIndexOf('t', 4)"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.lastIndexOf('t', 3)"));
            Assert.AreEqual(-1, Evaluate("'onetwothree'.lastIndexOf('two', 2)"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.lastIndexOf('two', 3)"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.lastIndexOf('two', 4)"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.lastIndexOf('two', 400)"));
            Assert.AreEqual(-1, Evaluate("'onetwothree'.lastIndexOf('two', -400)"));
            Assert.AreEqual(6, Evaluate("'onetwothree'.lastIndexOf('three')"));
            Assert.AreEqual(-1, Evaluate("'onetwothree'.lastIndexOf('no')"));
            Assert.AreEqual(-1, Evaluate("'onetwothree'.lastIndexOf('o', -400)"));
            Assert.AreEqual(3, Evaluate("'onetwothree'.lastIndexOf('two', NaN)"));
            Assert.AreEqual(-1, Evaluate("''.lastIndexOf('no')"));
            Assert.AreEqual(0, Evaluate("''.lastIndexOf('')"));
            Assert.AreEqual(3, Evaluate("'onenullthree'.lastIndexOf(null)"));
            Assert.AreEqual(3, Evaluate("'oneundefinedthree'.lastIndexOf(undefined)"));
            

            // length
            Assert.AreEqual(1, Evaluate("''.lastIndexOf.length"));

            // lastIndexOf is generic.
            Assert.AreEqual(2, Evaluate("x = new Number(6.1234); x.f = ''.lastIndexOf; x.f('123')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.lastIndexOf.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.lastIndexOf.call(null)"));
        }

        [TestMethod]
        public void link()
        {
            Assert.AreEqual(@"<a href=""undefined"">haha</a>", (string)Evaluate("'haha'.link()"), true);
            Assert.AreEqual(@"<a href=""http://www.beer.com"">haha</a>", (string)Evaluate("'haha'.link('http://www.beer.com')"), true);
            Assert.AreEqual(1, Evaluate("''.link.length"));
        }

        [TestMethod]
        public void localeCompare()
        {
            Assert.AreEqual(0, Evaluate("'haha'.localeCompare('haha')"));
            Assert.AreEqual(-1, Evaluate("'haha'.localeCompare('HAHA')"));
            Assert.AreEqual(-1, Evaluate("'a'.localeCompare('b')"));
            Assert.AreEqual(1, Evaluate("'b'.localeCompare('a')"));
            Assert.AreEqual(0, Evaluate("'undefined'.localeCompare(undefined)"));
            Assert.AreEqual(0, Evaluate("'null'.localeCompare(null)"));

            // length
            Assert.AreEqual(1, Evaluate("''.localeCompare.length"));

            // localeCompare is generic.
            Assert.AreEqual(0, Evaluate("x = new Number(6.1234); x.f = ''.localeCompare; x.f('6.1234')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.localeCompare.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.localeCompare.call(null)"));
        }

        [TestMethod]
        public void match()
        {
            // Non-global, no match = null.
            Assert.AreEqual(Null.Value, Evaluate("'A long string for testing'.match('nein')"));

            // Non-global, single match only (regexp created from string).
            Evaluate("var result = 'A long string for testing'.match('lo|st')");
            Assert.AreEqual("lo", Evaluate("result[0]"));
            Assert.AreEqual(1, Evaluate("result.length"));
            Assert.AreEqual(2, Evaluate("result.index"));
            Assert.AreEqual("A long string for testing", Evaluate("result.input"));

            // Non-global, single match only (regexp created from string) with subgroups.
            Evaluate("var result = 'A long string for testing'.match('(l)o|s(t)')");
            Assert.AreEqual("lo", Evaluate("result[0]"));
            Assert.AreEqual("l", Evaluate("result[1]"));
            Assert.AreEqual(Undefined.Value, Evaluate("result[2]"));
            Assert.AreEqual(true, Evaluate("result.hasOwnProperty(2)"));
            Assert.AreEqual(3, Evaluate("result.length"));
            Assert.AreEqual(2, Evaluate("result.index"));
            Assert.AreEqual("A long string for testing", Evaluate("result.input"));

            // Non-global, no match = null.
            Assert.AreEqual(Null.Value, Evaluate("'A long string for testing'.match(/nein/)"));

            // Non-global, single match only.
            Evaluate("var result = 'A long string for testing'.match(/lo|st/)");
            Assert.AreEqual("lo", Evaluate("result[0]"));
            Assert.AreEqual(1, Evaluate("result.length"));
            Assert.AreEqual(2, Evaluate("result.index"));
            Assert.AreEqual("A long string for testing", Evaluate("result.input"));

            // Non-global, single match only with subgroups.
            Evaluate("var result = 'A long string for testing'.match(/(l)o|s(t)/)");
            Assert.AreEqual("lo", Evaluate("result[0]"));
            Assert.AreEqual("l", Evaluate("result[1]"));
            Assert.AreEqual("undefined", Evaluate("typeof(result[2])"));
            Assert.AreEqual(3, Evaluate("result.length"));
            Assert.AreEqual(2, Evaluate("result.index"));
            Assert.AreEqual("A long string for testing", Evaluate("result.input"));

            // Non-global, make sure lastIndex is not modified.
            Evaluate("var regex = /lo|st/");
            Evaluate("regex.lastIndex = 18");
            Evaluate("var result = 'A long string for testing'.match(regex)");
            Assert.AreEqual(18, Evaluate("regex.lastIndex"));

            // Global, no match = null.
            Assert.AreEqual(Null.Value, Evaluate("'A long string for testing'.match(/nein/g)"));

            // Global, three matches.
            Evaluate("var result = 'A long string for testing'.match(/lo|st/g)");
            Assert.AreEqual("lo", Evaluate("result[0]"));
            Assert.AreEqual("st", Evaluate("result[1]"));
            Assert.AreEqual("st", Evaluate("result[2]"));
            Assert.AreEqual(3, Evaluate("result.length"));
            Assert.AreEqual("undefined", Evaluate("typeof(result.index)"));
            Assert.AreEqual("undefined", Evaluate("typeof(result.input)"));

            // Global, three matches with subgroups (subgroups are not returned with global enabled).
            Evaluate("var result = 'A long string for testing'.match(/(l)o|s(t)/g)");
            Assert.AreEqual("lo", Evaluate("result[0]"));
            Assert.AreEqual("st", Evaluate("result[1]"));
            Assert.AreEqual("st", Evaluate("result[2]"));
            Assert.AreEqual(3, Evaluate("result.length"));
            Assert.AreEqual("undefined", Evaluate("typeof(result.index)"));
            Assert.AreEqual("undefined", Evaluate("typeof(result.input)"));

            // Global, make sure lastIndex is set to zero just after match() is called and just before it returns.
            Evaluate("var regex = /lo|st/g");
            Evaluate("regex.lastIndex = 18");
            Evaluate("var result = 'A long string for testing'.match(regex)");
            Assert.AreEqual("lo", Evaluate("result[0]"));
            Assert.AreEqual("st", Evaluate("result[1]"));
            Assert.AreEqual("st", Evaluate("result[2]"));
            Assert.AreEqual(3, Evaluate("result.length"));
            Evaluate("regex.lastIndex = 0");

            // Global zero length matches should not hang the browser.
            Evaluate(@"var result = 'A\nB\nC'.match(/^/gm)");
            Assert.AreEqual(3, Evaluate("result.length"));
            Assert.AreEqual("", Evaluate("result[0]"));
            Assert.AreEqual("", Evaluate("result[1]"));
            Assert.AreEqual("", Evaluate("result[2]"));

            // Passing undefined is equivalent to passing an empty string.
            Assert.AreEqual(1, Evaluate("''.match().length"));
            Assert.AreEqual("", Evaluate("''.match()[0]"));

            // length
            Assert.AreEqual(1, Evaluate("''.match.length"));

            // match is generic.
            Evaluate("x = new Number(6.1234); x.f = ''.match; var result = x.f('12')");
            Assert.AreEqual(1, Evaluate("result.length"));
            Assert.AreEqual("12", Evaluate("result[0]"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.match.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.match.call(null)"));

            // Test the deprecated RegExp properties.
            Evaluate("'honey bunny'.match(/n(.)y/)");
            Assert.AreEqual("e", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("honey bunny", Evaluate("RegExp.input"));
            Assert.AreEqual("honey bunny", Evaluate("RegExp.$_"));
            Assert.AreEqual("ney", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("ney", Evaluate("RegExp['$&']"));
            Assert.AreEqual("e", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("e", Evaluate("RegExp['$+']"));
            Assert.AreEqual("ho", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("ho", Evaluate("RegExp['$`']"));
            Assert.AreEqual(" bunny", Evaluate("RegExp.rightContext"));
            Assert.AreEqual(" bunny", Evaluate("RegExp[\"$'\"]"));

            Evaluate("'honey bunny'.match(/n.?y/g)");
            Assert.AreEqual("", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("honey bunny", Evaluate("RegExp.input"));
            Assert.AreEqual("honey bunny", Evaluate("RegExp.$_"));
            Assert.AreEqual("nny", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("nny", Evaluate("RegExp['$&']"));
            Assert.AreEqual("", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("", Evaluate("RegExp['$+']"));
            Assert.AreEqual("honey bu", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("honey bu", Evaluate("RegExp['$`']"));
            Assert.AreEqual("", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", Evaluate("RegExp[\"$'\"]"));

            Evaluate("'honey bunny'.match(/(bu|ho)(..)y/g)");
            Assert.AreEqual("bu", Evaluate("RegExp.$1"));
            Assert.AreEqual("nn", Evaluate("RegExp.$2"));
            Assert.AreEqual("", Evaluate("RegExp.$3"));
            Assert.AreEqual("honey bunny", Evaluate("RegExp.input"));
            Assert.AreEqual("honey bunny", Evaluate("RegExp.$_"));
            Assert.AreEqual("bunny", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("bunny", Evaluate("RegExp['$&']"));
            Assert.AreEqual("nn", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("nn", Evaluate("RegExp['$+']"));
            Assert.AreEqual("honey ", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("honey ", Evaluate("RegExp['$`']"));
            Assert.AreEqual("", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", Evaluate("RegExp[\"$'\"]"));
        }

        [TestMethod]
        public void normalize()
        {
            // Default is NFC
            Assert.AreEqual(7855, Evaluate(@"'\u1EAF'.normalize().charCodeAt(0)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u1EAF'.normalize().charCodeAt(1)"));
            Assert.AreEqual(7855, Evaluate(@"'\u0103\u0301'.normalize().charCodeAt(0)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0103\u0301'.normalize().charCodeAt(1)"));
            Assert.AreEqual(7855, Evaluate(@"'\u0061\u0306\u0301'.normalize().charCodeAt(0)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0061\u0306\u0301'.normalize().charCodeAt(1)"));
            Assert.AreEqual(50, Evaluate(@"'\u0032\u2075'.normalize().charCodeAt(0)"));
            Assert.AreEqual(8309, Evaluate(@"'\u0032\u2075'.normalize().charCodeAt(1)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0032\u2075'.normalize().charCodeAt(2)"));

            // NFC
            Assert.AreEqual(7855, Evaluate(@"'\u1EAF'.normalize('NFC').charCodeAt(0)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u1EAF'.normalize('NFC').charCodeAt(1)"));
            Assert.AreEqual(7855, Evaluate(@"'\u0103\u0301'.normalize('NFC').charCodeAt(0)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0103\u0301'.normalize('NFC').charCodeAt(1)"));
            Assert.AreEqual(7855, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFC').charCodeAt(0)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFC').charCodeAt(1)"));
            Assert.AreEqual(50, Evaluate(@"'\u0032\u2075'.normalize('NFC').charCodeAt(0)"));
            Assert.AreEqual(8309, Evaluate(@"'\u0032\u2075'.normalize('NFC').charCodeAt(1)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0032\u2075'.normalize('NFC').charCodeAt(2)"));

            // NFD
            Assert.AreEqual(97, Evaluate(@"'\u1EAF'.normalize('NFD').charCodeAt(0)"));
            Assert.AreEqual(774, Evaluate(@"'\u1EAF'.normalize('NFD').charCodeAt(1)"));
            Assert.AreEqual(769, Evaluate(@"'\u1EAF'.normalize('NFD').charCodeAt(2)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u1EAF'.normalize('NFD').charCodeAt(3)"));
            Assert.AreEqual(97, Evaluate(@"'\u0103\u0301'.normalize('NFD').charCodeAt(0)"));
            Assert.AreEqual(774, Evaluate(@"'\u0103\u0301'.normalize('NFD').charCodeAt(1)"));
            Assert.AreEqual(769, Evaluate(@"'\u0103\u0301'.normalize('NFD').charCodeAt(2)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0103\u0301'.normalize('NFD').charCodeAt(3)"));
            Assert.AreEqual(97, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFD').charCodeAt(0)"));
            Assert.AreEqual(774, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFD').charCodeAt(1)"));
            Assert.AreEqual(769, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFD').charCodeAt(2)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFD').charCodeAt(3)"));
            Assert.AreEqual(50, Evaluate(@"'\u0032\u2075'.normalize('NFD').charCodeAt(0)"));
            Assert.AreEqual(8309, Evaluate(@"'\u0032\u2075'.normalize('NFD').charCodeAt(1)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0032\u2075'.normalize('NFD').charCodeAt(2)"));

            // NFKC
            Assert.AreEqual(7855, Evaluate(@"'\u1EAF'.normalize('NFKC').charCodeAt(0)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u1EAF'.normalize('NFKC').charCodeAt(1)"));
            Assert.AreEqual(7855, Evaluate(@"'\u0103\u0301'.normalize('NFKC').charCodeAt(0)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0103\u0301'.normalize('NFKC').charCodeAt(1)"));
            Assert.AreEqual(7855, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFKC').charCodeAt(0)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFKC').charCodeAt(1)"));
            Assert.AreEqual(50, Evaluate(@"'\u0032\u2075'.normalize('NFKC').charCodeAt(0)"));
            Assert.AreEqual(53, Evaluate(@"'\u0032\u2075'.normalize('NFKC').charCodeAt(1)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0032\u2075'.normalize('NFKC').charCodeAt(2)"));

            // NFKD
            Assert.AreEqual(97, Evaluate(@"'\u1EAF'.normalize('NFKD').charCodeAt(0)"));
            Assert.AreEqual(774, Evaluate(@"'\u1EAF'.normalize('NFKD').charCodeAt(1)"));
            Assert.AreEqual(769, Evaluate(@"'\u1EAF'.normalize('NFKD').charCodeAt(2)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u1EAF'.normalize('NFKD').charCodeAt(3)"));
            Assert.AreEqual(97, Evaluate(@"'\u0103\u0301'.normalize('NFKD').charCodeAt(0)"));
            Assert.AreEqual(774, Evaluate(@"'\u0103\u0301'.normalize('NFKD').charCodeAt(1)"));
            Assert.AreEqual(769, Evaluate(@"'\u0103\u0301'.normalize('NFKD').charCodeAt(2)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0103\u0301'.normalize('NFKD').charCodeAt(3)"));
            Assert.AreEqual(97, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFKD').charCodeAt(0)"));
            Assert.AreEqual(774, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFKD').charCodeAt(1)"));
            Assert.AreEqual(769, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFKD').charCodeAt(2)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0061\u0306\u0301'.normalize('NFKD').charCodeAt(3)"));
            Assert.AreEqual(50, Evaluate(@"'\u0032\u2075'.normalize('NFKD').charCodeAt(0)"));
            Assert.AreEqual(53, Evaluate(@"'\u0032\u2075'.normalize('NFKD').charCodeAt(1)"));
            Assert.AreEqual(double.NaN, Evaluate(@"'\u0032\u2075'.normalize('NFKD').charCodeAt(2)"));

            // Valid parameter values are: NFC, NFD, NFKC, NFKD
            Assert.AreEqual("RangeError", EvaluateExceptionType(@"'\u1EAF'.normalize('abc')"));
            Assert.AreEqual("RangeError", EvaluateExceptionType(@"'\u1EAF'.normalize('nfc')"));

            // normalize is generic.
            Assert.AreEqual("6.1234", Evaluate("x = new Number(6.1234); x.f = ''.normalize; x.f()"));
        }

        [TestMethod]
        public void quote()
        {
            Assert.AreEqual(@"""test""", Evaluate(@"'test'.quote()"));
            Assert.AreEqual(@"""te\""st""", Evaluate(@"'te""st'.quote()"));
            Assert.AreEqual(@"""te'st""", Evaluate(@"""te'st"".quote()"));
        }

        [TestMethod]
        public void replace()
        {
            // replace(string, string)
            Assert.AreEqual("A long string for testing", Evaluate("'A long string for testing'.replace('ew', 'ah!')"));
            Assert.AreEqual("A long ew!ring for testing", Evaluate("'A long string for testing'.replace('st', 'ew!')"));
            Assert.AreEqual("TA short string", Evaluate("'A short string'.replace('', 'T')"));
            
            // replace(regExp, string)
            Assert.AreEqual("A ew!ng string for testing", Evaluate("'A long string for testing'.replace(/lo|st/, 'ew!')"));
            Assert.AreEqual("A ew!ng ew!ring for teew!ing", Evaluate("'A long string for testing'.replace(/lo|st/g, 'ew!')"));
            Assert.AreEqual("[{ ]@ ], ]@ ] }]", Evaluate(@"
                '[{ \""tag\"": ""titillation"", \""popularity\"": 4294967296 }]'.
                replace(/""[^""\\\n\r]*""|true|false|null|-?\d+(?:\.\d*)?(:?[eE][+\-]?\d+)?/g, ']').
                replace(/:/g, '@')"));
            Assert.AreEqual("A $ng $ring for te$ing", Evaluate("'A long string for testing'.replace(/lo|st/g, '$$')"));
            Assert.AreEqual("A ${test}ng ${test}ring for te${test}ing", Evaluate("'A long string for testing'.replace(/lo|st/g, '${test}')"));
            Assert.AreEqual("A <lo>ng <st>ring for te<st>ing", Evaluate("'A long string for testing'.replace(/lo|st/g, '<$&>')"));
            Assert.AreEqual("A short <A short >ring", Evaluate("'A short string'.replace(/lo|st/g, '<$`>')"));
            Assert.AreEqual("A short <ring>ring", Evaluate(@"'A short string'.replace(/lo|st/g, '<$\'>')"));
            Assert.AreEqual("A l  $3 l0ng  t $3 0ring for te t $3 0ing", Evaluate("'A long string for testing'.replace(/(l)o|s(t)/g, '$1 $2 $3 $10')"));
            Assert.AreEqual("A long string g", Evaluate("'A long string for testing'.replace(/(f)(o)(r)( )(t)(e)(s)(t)(i)(n)(g)/g, '$11')"));
            Assert.AreEqual("$1-$11,$1-$22", Evaluate(@"'$1,$2'.replace(/(\$(\d))/g, '$$1-$1$2')"));
            Assert.AreEqual("$es$ing", Evaluate(@"'testing'.replace(/t/g, '$')"));

            // replace(regExp, function)
            Assert.AreEqual("blah def34", Evaluate("'abc12 def34'.replace(/([a-z]+)([0-9]+)/, function() { return 'blah' })"));
            Assert.AreEqual("12abc def34", Evaluate("'abc12 def34'.replace(/([a-z]+)([0-9]+)/, function() { return arguments[2] + arguments[1] })"));
            Assert.AreEqual("A aort aring", Evaluate("'A short string'.replace(/(s)h|s(t)/g, function() { return 'a'; })"));
            Execute(@"var parameterValues = []");
            Evaluate("'A short string'.replace(/(s)h|s(t)/g, function() { parameterValues.push(arguments); })");
            Assert.AreEqual(2, Evaluate("parameterValues.length"));
            Assert.AreEqual(5, Evaluate("parameterValues[0].length"));
            Assert.AreEqual("sh", Evaluate("parameterValues[0][0]"));
            Assert.AreEqual("s", Evaluate("parameterValues[0][1]"));
            Assert.AreEqual("undefined", Evaluate("typeof(parameterValues[0][2])"));
            Assert.AreEqual(true, Evaluate("parameterValues[0].hasOwnProperty(2)"));
            Assert.AreEqual(2, Evaluate("parameterValues[0][3]"));
            Assert.AreEqual("A short string", Evaluate("parameterValues[0][4]"));
            Assert.AreEqual(5, Evaluate("parameterValues[1].length"));
            Assert.AreEqual("st", Evaluate("parameterValues[1][0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(parameterValues[1][1])"));
            Assert.AreEqual("t", Evaluate("parameterValues[1][2]"));
            Assert.AreEqual(8, Evaluate("parameterValues[1][3]"));
            Assert.AreEqual("A short string", Evaluate("parameterValues[1][4]"));

            Assert.AreEqual("undefined runny", Evaluate("arr = []; 'funny runny'.replace(/(..)nny/, function(a, b, c, d) { arr.push(a); arr.push(b); arr.push(c); arr.push(d) })"));
            Assert.AreEqual("funny,fu,0,funny runny", Evaluate("arr.toString()"));

            Execute(@"var parameterValues = []");
            Execute(@"'Type some [Markdown] text on the left side.'.replace(/(\[([^\[\]]+)\])()()()()()/g, function(wholeMatch,m1,m2,m3,m4,m5,m6,m7) {parameterValues.push(arguments)})");
            Assert.AreEqual(1, Evaluate("parameterValues.length"));
            Assert.AreEqual(10, Evaluate("parameterValues[0].length"));
            Assert.AreEqual("[Markdown]", Evaluate("parameterValues[0][0]"));
            Assert.AreEqual("[Markdown]", Evaluate("parameterValues[0][1]"));
            Assert.AreEqual("Markdown", Evaluate("parameterValues[0][2]"));
            Assert.AreEqual("", Evaluate("parameterValues[0][3]"));
            Assert.AreEqual("", Evaluate("parameterValues[0][4]"));
            Assert.AreEqual("", Evaluate("parameterValues[0][5]"));
            Assert.AreEqual("", Evaluate("parameterValues[0][6]"));
            Assert.AreEqual("", Evaluate("parameterValues[0][7]"));
            Assert.AreEqual(10, Evaluate("parameterValues[0][8]"));
            Assert.AreEqual("Type some [Markdown] text on the left side.", Evaluate("parameterValues[0][9]"));

            // replace(string, function)
            Assert.AreEqual("A short string", Evaluate("'A short string'.replace('test', function() { return 'a'; })"));
            Assert.AreEqual("A ahort string", Evaluate("'A short string'.replace('s', function() { return 'a'; })"));
            Assert.AreEqual("A long string", Evaluate("'A short string'.replace('short', function() { return 'long'; })"));

            // length
            Assert.AreEqual(2, Evaluate("''.replace.length"));

            // replace is generic.
            Assert.AreEqual("6.4334", Evaluate("x = new Number(6.1234); x.f = ''.replace; x.f('12', '43')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.replace.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.replace.call(null)"));

            // Test the deprecated RegExp properties.
            Assert.AreEqual(" runny", Evaluate("'funny runny'.replace(/(..)nny/, '')"));
            Assert.AreEqual("fu", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("funny runny", Evaluate("RegExp.input"));
            Assert.AreEqual("funny runny", Evaluate("RegExp.$_"));
            Assert.AreEqual("funny", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("funny", Evaluate("RegExp['$&']"));
            Assert.AreEqual("fu", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("fu", Evaluate("RegExp['$+']"));
            Assert.AreEqual("", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("", Evaluate("RegExp['$`']"));
            Assert.AreEqual(" runny", Evaluate("RegExp.rightContext"));
            Assert.AreEqual(" runny", Evaluate("RegExp[\"$'\"]"));

            Assert.AreEqual(" ", Evaluate("'funny runny'.replace(/(..)nny/g, '')"));
            Assert.AreEqual("ru", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("funny runny", Evaluate("RegExp.input"));
            Assert.AreEqual("funny runny", Evaluate("RegExp.$_"));
            Assert.AreEqual("runny", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("runny", Evaluate("RegExp['$&']"));
            Assert.AreEqual("ru", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("ru", Evaluate("RegExp['$+']"));
            Assert.AreEqual("funny ", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("funny ", Evaluate("RegExp['$`']"));
            Assert.AreEqual("", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", Evaluate("RegExp[\"$'\"]"));

            Assert.AreEqual("funny runny", Evaluate("'funny runny'.replace(/boo/g, '')"));
            Assert.AreEqual("ru", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("funny runny", Evaluate("RegExp.input"));
            Assert.AreEqual("funny runny", Evaluate("RegExp.$_"));
            Assert.AreEqual("runny", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("runny", Evaluate("RegExp['$&']"));
            Assert.AreEqual("ru", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("ru", Evaluate("RegExp['$+']"));
            Assert.AreEqual("funny ", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("funny ", Evaluate("RegExp['$`']"));
            Assert.AreEqual("", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", Evaluate("RegExp[\"$'\"]"));

            Assert.AreEqual("fu,funny,fu,, runny", Evaluate(@"arr = []; 'funny runny'.replace(/(..)nny/, function() {
                    arr.push(RegExp.$1);
                    arr.push(RegExp.lastMatch);
                    arr.push(RegExp.lastParen);
                    arr.push(RegExp.leftContext);
                    arr.push(RegExp.rightContext);
                }); arr.toString()"));
            Assert.AreEqual("fu,funny,fu,, runny,ru,runny,ru,funny ,", Evaluate(@"arr = []; 'funny runny'.replace(/(..)nny/g, function() {
                    arr.push(RegExp.$1);
                    arr.push(RegExp.lastMatch);
                    arr.push(RegExp.lastParen);
                    arr.push(RegExp.leftContext);
                    arr.push(RegExp.rightContext);
                }); arr.toString()"));

            // "this" should be the global object in non-strict mode.
            Assert.AreEqual(true, Evaluate(@"
                var global = this;
                var success = false;
                'test'.replace('e', function (x, y) {
                    success = this === global;
                });
                success"));

            // "this" should be undefined in strict mode.
            Assert.AreEqual(true, Evaluate(@"
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
            Assert.AreEqual(-1, Evaluate("'A long string for testing'.search('nein')"));
            Assert.AreEqual(7, Evaluate("'A long string for testing'.search('st')"));
            Assert.AreEqual(-1, Evaluate("'A long string for testing'.search(/nein/)"));
            Assert.AreEqual(7, Evaluate("'A long string for testing'.search(/st/)"));
            Assert.AreEqual(-1, Evaluate("'A long string for testing'.search(/nein/g)"));
            Assert.AreEqual(7, Evaluate("'A long string for testing'.search(/st/g)"));

            // Make sure lastIndex is not modified.
            Evaluate("var regex = /lo|st/");
            Evaluate("regex.lastIndex = 15");
            Assert.AreEqual(2, Evaluate("'A long string for testing'.search(regex)"));
            Assert.AreEqual(15, Evaluate("regex.lastIndex"));
            Evaluate("regex = /lo|st/g");
            Evaluate("regex.lastIndex = 15");
            Assert.AreEqual(2, Evaluate("'A long string for testing'.search(regex)"));
            Assert.AreEqual(15, Evaluate("regex.lastIndex"));

            // Passing undefined to is equivalent to passing an empty string.
            Assert.AreEqual(0, Evaluate("''.search('')"));
            Assert.AreEqual(0, Evaluate("''.search()"));
            Assert.AreEqual(0, Evaluate("'--undefined--'.search()"));
            Assert.AreEqual(0, Evaluate("''.search(undefined)"));

            // length
            Assert.AreEqual(1, Evaluate("''.search.length"));

            // search is generic.
            Assert.AreEqual(2, Evaluate("x = new Number(6.1234); x.f = ''.search; x.f('12')"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.search.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.search.call(null)"));

            // Test the deprecated RegExp properties.
            Assert.AreEqual(7, Evaluate("'lots of honey'.search(/(...)ney/)"));
            Assert.AreEqual(" ho", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("lots of honey", Evaluate("RegExp.input"));
            Assert.AreEqual("lots of honey", Evaluate("RegExp.$_"));
            Assert.AreEqual(" honey", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual(" honey", Evaluate("RegExp['$&']"));
            Assert.AreEqual(" ho", Evaluate("RegExp.lastParen"));
            Assert.AreEqual(" ho", Evaluate("RegExp['$+']"));
            Assert.AreEqual("lots of", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("lots of", Evaluate("RegExp['$`']"));
            Assert.AreEqual("", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", Evaluate("RegExp[\"$'\"]"));

            Assert.AreEqual(-1, Evaluate("'tons of honey'.search(/nomatch/)"));
            Assert.AreEqual(" ho", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("lots of honey", Evaluate("RegExp.input"));
            Assert.AreEqual("lots of honey", Evaluate("RegExp.$_"));
            Assert.AreEqual(" honey", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual(" honey", Evaluate("RegExp['$&']"));
            Assert.AreEqual(" ho", Evaluate("RegExp.lastParen"));
            Assert.AreEqual(" ho", Evaluate("RegExp['$+']"));
            Assert.AreEqual("lots of", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("lots of", Evaluate("RegExp['$`']"));
            Assert.AreEqual("", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", Evaluate("RegExp[\"$'\"]"));
        }

        [TestMethod]
        public void slice()
        {
            Assert.AreEqual("testing", Evaluate("'A long string for testing'.slice(18)"));
            Assert.AreEqual("ing", Evaluate("'A long string for testing'.slice(-3)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.slice(40)"));
            Assert.AreEqual("A long string for testing", Evaluate("'A long string for testing'.slice(-40)"));
            Assert.AreEqual("on", Evaluate("'A long string for testing'.slice(3, 5)"));
            Assert.AreEqual("te", Evaluate("'A long string for testing'.slice(-7, 20)"));
            Assert.AreEqual("te", Evaluate("'A long string for testing'.slice(18, -5)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.slice(19, -10)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.slice(19, -40)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.slice(12, 10)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.slice(40, 10)"));

            // Index arguments use ToInteger().
            Assert.AreEqual("abc", Evaluate("'abc'.slice(0, Infinity)"));
            Assert.AreEqual("abc", Evaluate("'abc'.slice(0, 10000000000)"));
            Assert.AreEqual("abc", Evaluate("'abc'.slice(-Infinity)"));
            Assert.AreEqual("abc", Evaluate("'abc'.slice(-10000000000)"));
            Assert.AreEqual("", Evaluate("'abc'.slice(Infinity)"));
            Assert.AreEqual("", Evaluate("'abc'.slice(0, -Infinity)"));

            // length
            Assert.AreEqual(2, Evaluate("''.slice.length"));

            // slice is generic.
            Assert.AreEqual("23", Evaluate("x = new Number(6.1234); x.f = ''.slice; x.f(3, 5)"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.slice.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.slice.call(null)"));
        }

        [TestMethod]
        public void small()
        {
            Assert.AreEqual("<small>haha</small>", (string)Evaluate("'haha'.small()"), true);
            Assert.AreEqual(0, Evaluate("''.small.length"));
        }

        [TestMethod]
        public void split()
        {
            // String splits.
            Evaluate("var result = 'A short string'.split(' ')");
            Assert.AreEqual(3, Evaluate("result.length"));
            Assert.AreEqual("A", Evaluate("result[0]"));
            Assert.AreEqual("short", Evaluate("result[1]"));
            Assert.AreEqual("string", Evaluate("result[2]"));

            Evaluate("var result = '5,,7'.split(',')");
            Assert.AreEqual(3, Evaluate("result.length"));
            Assert.AreEqual("5", Evaluate("result[0]"));
            Assert.AreEqual("", Evaluate("result[1]"));
            Assert.AreEqual("7", Evaluate("result[2]"));

            // String split (with limit).
            Evaluate("var result = '5,,7'.split(',', 2)");
            Assert.AreEqual(2, Evaluate("result.length"));
            Assert.AreEqual("5", Evaluate("result[0]"));
            Assert.AreEqual("", Evaluate("result[1]"));
            Evaluate("var result = '5,,7'.split(',', -1)");
            Assert.AreEqual(3, Evaluate("result.length"));

            // Regex splits.
            Evaluate(@"var result = 'A long string for testing'.split(/lo|st/)");
            Assert.AreEqual(4, Evaluate("result.length"));
            Assert.AreEqual("A ", Evaluate("result[0]"));
            Assert.AreEqual("ng ", Evaluate("result[1]"));
            Assert.AreEqual("ring for te", Evaluate("result[2]"));
            Assert.AreEqual("ing", Evaluate("result[3]"));

            // Regex split (with limit).
            Evaluate(@"var result = 'A long string for testing'.split(/i/, 1)");
            Assert.AreEqual(1, Evaluate("result.length"));
            Assert.AreEqual("A long str", Evaluate("result[0]"));

            // Regex splits with subgroups.
            Evaluate(@"var result = 'A<B>bold</B>and<CODE>coded</CODE>'.split(/<(\/)?([^<>]+)>/)");
            Assert.AreEqual(13, Evaluate("result.length"));
            Assert.AreEqual("A", Evaluate("result[0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(result[1])"));
            Assert.AreEqual("B", Evaluate("result[2]"));
            Assert.AreEqual("bold", Evaluate("result[3]"));
            Assert.AreEqual("/", Evaluate("result[4]"));
            Assert.AreEqual("B", Evaluate("result[5]"));
            Assert.AreEqual("and", Evaluate("result[6]"));
            Assert.AreEqual("undefined", Evaluate("typeof(result[7])"));
            Assert.AreEqual("CODE", Evaluate("result[8]"));
            Assert.AreEqual("coded", Evaluate("result[9]"));
            Assert.AreEqual("/", Evaluate("result[10]"));
            Assert.AreEqual("CODE", Evaluate("result[11]"));
            Assert.AreEqual("", Evaluate("result[12]"));

            // Do not match the empty substring at the start and end of the string, or at the
            // end of a previous match.
            Assert.AreEqual(@"[""o"",null,""n"",null,""e"",""t"","""",""w"",""o"",""t"",""h"",null,""r"",null,""e"",null,""e""]",
                Evaluate("JSON.stringify('onetwothree'.split(/(t|w)?/))"));

            // Try a regex with multiple captures.
            Assert.AreEqual(@"[""one"",""w"",""o"",""t"",""hree""]",
                Evaluate("JSON.stringify('onetwothree'.split(/(t|w)+/))"));
            Assert.AreEqual(@"["""",""e"",""""]",
                Evaluate("JSON.stringify('onetwothree'.split(/([a-z])+/))"));

            // Test the limit argument.
            Assert.AreEqual(@"[""o"",null,null,""n"",""et""]",
                Evaluate("JSON.stringify('onetwothree'.split(/(et)?(wo)?/, 5))"));

            // Spec violation but de-facto standard: undefined is converted to 'undefined'.
            Assert.AreEqual(2, Evaluate("'teundefinedst'.split(undefined).length"));

            // Splitting by an empty string splits the string into individual characters.
            Assert.AreEqual("a,b,c", Evaluate("'abc'.split('').toString()"));
            Assert.AreEqual("a,b,c", Evaluate("'abc'.split(new RegExp()).toString()"));

            // length
            Assert.AreEqual(2, Evaluate("''.split.length"));

            // split is generic.
            Evaluate("x = new Number(6.1234); x.f = ''.split; var result = x.f('2')");
            Assert.AreEqual(2, Evaluate("result.length"));
            Assert.AreEqual("6.1", Evaluate("result[0]"));
            Assert.AreEqual("34", Evaluate("result[1]"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.split.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.split.call(null)"));

            // Test the deprecated RegExp properties.
            Evaluate("'lots of money and honey'.split(/(..)ney/)");
            Assert.AreEqual("ho", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("lots of money and honey", Evaluate("RegExp.input"));
            Assert.AreEqual("lots of money and honey", Evaluate("RegExp.$_"));
            Assert.AreEqual("honey", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("honey", Evaluate("RegExp['$&']"));
            Assert.AreEqual("ho", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("ho", Evaluate("RegExp['$+']"));
            Assert.AreEqual("lots of money and ", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("lots of money and ", Evaluate("RegExp['$`']"));
            Assert.AreEqual("", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", Evaluate("RegExp[\"$'\"]"));

            Evaluate("'tons of money and honey'.split(/nomatch/)");
            Assert.AreEqual("ho", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("lots of money and honey", Evaluate("RegExp.input"));
            Assert.AreEqual("lots of money and honey", Evaluate("RegExp.$_"));
            Assert.AreEqual("honey", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("honey", Evaluate("RegExp['$&']"));
            Assert.AreEqual("ho", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("ho", Evaluate("RegExp['$+']"));
            Assert.AreEqual("lots of money and ", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("lots of money and ", Evaluate("RegExp['$`']"));
            Assert.AreEqual("", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("", Evaluate("RegExp[\"$'\"]"));
        }

        [TestMethod]
        public void strike()
        {
            Assert.AreEqual("<strike>haha</strike>", (string)Evaluate("'haha'.strike()"), true);
            Assert.AreEqual(0, Evaluate("''.strike.length"));
        }

        [TestMethod]
        public void sub()
        {
            Assert.AreEqual("<sub>haha</sub>", (string)Evaluate("'haha'.sub()"), true);
            Assert.AreEqual(0, Evaluate("''.sub.length"));
        }

        [TestMethod]
        public void substr()
        {
            Assert.AreEqual("testing", Evaluate("'A long string for testing'.substr(18)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.substr(40)"));
            Assert.AreEqual("ong s", Evaluate("'A long string for testing'.substr(3, 5)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.substr(18, -5)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.substr(19, -10)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.substr(19, -40)"));
            Assert.AreEqual("g for test", Evaluate("'A long string for testing'.substr(12, 10)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.substr(40, 10)"));

            // IE disagrees with the spec w.r.t. the meaning of a negative start parameter.
            Assert.AreEqual("ing", Evaluate("'A long string for testing'.substr(-3)"));
            Assert.AreEqual("A long string for testing", Evaluate("'A long string for testing'.substr(-40)"));
            Assert.AreEqual("testing", Evaluate("'A long string for testing'.substr(-7, 20)"));

            // length
            Assert.AreEqual(2, Evaluate("''.substr.length"));

            // substr is generic.
            Assert.AreEqual("23", Evaluate("x = new Number(6.1234); x.f = ''.substr; x.f(3, 2)"));
        }

        [TestMethod]
        public void substring()
        {
            Assert.AreEqual("testing", Evaluate("'A long string for testing'.substring(18)"));
            Assert.AreEqual("A long string for testing", Evaluate("'A long string for testing'.substring(-3)"));
            Assert.AreEqual("", Evaluate("'A long string for testing'.substring(40)"));
            Assert.AreEqual("A long string for testing", Evaluate("'A long string for testing'.substring(-40)"));
            Assert.AreEqual("on", Evaluate("'A long string for testing'.substring(3, 5)"));
            Assert.AreEqual("A long string for te", Evaluate("'A long string for testing'.substring(-7, 20)"));
            Assert.AreEqual("A long string for ", Evaluate("'A long string for testing'.substring(18, -5)"));
            Assert.AreEqual("A long string for t", Evaluate("'A long string for testing'.substring(19, -10)"));
            Assert.AreEqual("A long string for t", Evaluate("'A long string for testing'.substring(19, -40)"));
            Assert.AreEqual("in", Evaluate("'A long string for testing'.substring(12, 10)"));
            Assert.AreEqual("ing for testing", Evaluate("'A long string for testing'.substring(40, 10)"));
            Assert.AreEqual("foo", Evaluate("'foo'.substring(0, undefined)"));

            // length
            Assert.AreEqual(2, Evaluate("''.substring.length"));

            // substring is generic.
            Assert.AreEqual("23", Evaluate("x = new Number(6.1234); x.f = ''.substring; x.f(5, 3)"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.substring.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.substring.call(null)"));
        }

        [TestMethod]
        public void sup()
        {
            Assert.AreEqual("<sup>haha</sup>", (string)Evaluate("'haha'.sup()"), true);
            Assert.AreEqual(0, Evaluate("''.sup.length"));
        }

        [TestMethod]
        public void trim()
        {
            Assert.AreEqual("hello world", Evaluate("'  hello world  '.trim()"));
            Assert.AreEqual("6.1234", Evaluate("x = new Number(6.1234); x.f = ''.trim; x.f()"));
            Assert.AreEqual(0, Evaluate("''.trim.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.trim.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.trim.call(null)"));
        }

        [TestMethod]
        public void toLocaleLowerCase()
        {
            Assert.AreEqual("hello world", Evaluate("'Hello World'.toLocaleLowerCase()"));
            Assert.AreEqual("6.1234", Evaluate("x = new Number(6.1234); x.f = ''.toLocaleLowerCase; x.f()"));
            Assert.AreEqual(0, Evaluate("''.toLocaleLowerCase.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.toLocaleLowerCase.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.toLocaleLowerCase.call(null)"));
        }

        [TestMethod]
        public void toLocaleUpperCase()
        {
            Assert.AreEqual("HELLO WORLD", Evaluate("'Hello World'.toLocaleUpperCase()"));
            Assert.AreEqual("6.1234", Evaluate("x = new Number(6.1234); x.f = ''.toLocaleUpperCase; x.f()"));
            Assert.AreEqual(0, Evaluate("''.toLocaleUpperCase.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.toLocaleUpperCase.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.toLocaleUpperCase.call(null)"));
        }

        [TestMethod]
        public void toLowerCase()
        {
            Assert.AreEqual("hello world", Evaluate("'Hello World'.toLowerCase()"));
            Assert.AreEqual("6.1234", Evaluate("x = new Number(6.1234); x.f = ''.toLowerCase; x.f()"));
            Assert.AreEqual(0, Evaluate("''.toLowerCase.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.toLowerCase.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.toLowerCase.call(null)"));
        }

        [TestMethod]
        public void toUpperCase()
        {
            Assert.AreEqual("HELLO WORLD", Evaluate("'Hello World'.toUpperCase()"));
            Assert.AreEqual("6.1234", Evaluate("x = new Number(6.1234); x.f = ''.toUpperCase; x.f()"));
            Assert.AreEqual(0, Evaluate("''.toUpperCase.length"));

            // Undefined and null are not allowed as the "this" object.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.toUpperCase.call(undefined)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.toUpperCase.call(null)"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("hello world", Evaluate("'hello world'.toString()"));
            Assert.AreEqual(0, Evaluate("''.toString.length"));

            // toString is not generic.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.toString.call(5)"));
        }

        [TestMethod]
        public void valueOf()
        {
            Assert.AreEqual("hello world", Evaluate("'hello world'.valueOf()"));
            Assert.AreEqual(0, Evaluate("'hello world'.valueOf.length"));

            // valueOf is not generic.
            Assert.AreEqual("TypeError", EvaluateExceptionType("''.valueOf.call(5)"));
        }

        [TestMethod]
        public void startsWith()
        {
            Assert.AreEqual(true, Evaluate("'To be, or not to be, that is the question.'.startsWith('To be')"));
            Assert.AreEqual(false, Evaluate("'To be, or not to be, that is the question.'.startsWith('not to be')"));
            Assert.AreEqual(true, Evaluate("'To be, or not to be, that is the question.'.startsWith('not to be', 10)"));

            Assert.AreEqual(true, Evaluate("'ABC'.startsWith('AB')"));
            Assert.AreEqual(false, Evaluate("'ABC'.startsWith('abc')"));
            Assert.AreEqual(false, Evaluate("'ABC'.startsWith('ABCD')"));
            Assert.AreEqual(true, Evaluate("'ABC'.startsWith('B', 1)"));
            Assert.AreEqual(false, Evaluate("'ABC'.startsWith('bc', 1)"));
            Assert.AreEqual(false, Evaluate("'ABC'.startsWith('BCD', 1)"));

            // The substring cannot be a regular expression.
            Assert.AreEqual("TypeError", EvaluateExceptionType("'onetwothree'.startsWith(/two/)"));

            // Length
            Assert.AreEqual(1, Evaluate("'To be, or not to be, that is the question.'.startsWith.length"));
        }

        [TestMethod]
        public void endsWith()
        {
            Assert.AreEqual(true, Evaluate("'To be, or not to be, that is the question.'.endsWith('question.')"));
            Assert.AreEqual(false, Evaluate("'To be, or not to be, that is the question.'.endsWith('not to be')"));
            Assert.AreEqual(true, Evaluate("'To be, or not to be, that is the question.'.endsWith('not to be', 19)"));

            Assert.AreEqual(true, Evaluate("'ABC'.endsWith('BC')"));
            Assert.AreEqual(false, Evaluate("'ABC'.endsWith('bc')"));
            Assert.AreEqual(false, Evaluate("'ABC'.endsWith('ABCD')"));
            Assert.AreEqual(true, Evaluate("'ABC'.endsWith('B', 2)"));
            Assert.AreEqual(false, Evaluate("'ABC'.endsWith('bc', 3)"));
            Assert.AreEqual(false, Evaluate("'ABC'.endsWith('BCD', 2)"));
            Assert.AreEqual(true, Evaluate("'ABC'.endsWith('C', 100)"));
            Assert.AreEqual(false, Evaluate("'ABC'.endsWith('C', 1)"));
            Assert.AreEqual(false, Evaluate("'ABC'.endsWith('C', 0)"));
            Assert.AreEqual(false, Evaluate("'ABC'.endsWith('C', -1)"));

            // The substring cannot be a regular expression.
            Assert.AreEqual("TypeError", EvaluateExceptionType("'onetwothree'.endsWith(/two/)"));

            // Length
            Assert.AreEqual(1, Evaluate("'To be, or not to be, that is the question.'.endsWith.length"));
        }

        [TestMethod]
        public void contains()
        {
            Assert.AreEqual(true, Evaluate("'To be, or not to be, that is the question.'.contains('question.')"));
            Assert.AreEqual(true, Evaluate("'To be, or not to be, that is the question.'.contains('not to be')"));
            Assert.AreEqual(false, Evaluate("'To be, or not to be, that is the question.'.contains('NOT')"));
            Assert.AreEqual(false, Evaluate("'To be, or not to be, that is the question.'.contains('not to be', 19)"));

            Assert.AreEqual(true, Evaluate("'ABC'.contains('BC')"));
            Assert.AreEqual(false, Evaluate("'ABC'.contains('bc')"));
            Assert.AreEqual(false, Evaluate("'ABC'.contains('ABCD')"));
            Assert.AreEqual(false, Evaluate("'ABC'.contains('B', 2)"));
            Assert.AreEqual(false, Evaluate("'ABC'.contains('bc', 3)"));
            Assert.AreEqual(false, Evaluate("'ABC'.contains('BCD', 2)"));
            Assert.AreEqual(false, Evaluate("'ABC'.contains('C', 100)"));
            Assert.AreEqual(true, Evaluate("'ABC'.contains('C', 1)"));
            Assert.AreEqual(true, Evaluate("'ABC'.contains('C', 0)"));
            Assert.AreEqual(true, Evaluate("'ABC'.contains('C', -1)"));

            // Length
            Assert.AreEqual(1, Evaluate("'To be, or not to be, that is the question.'.contains.length"));
        }

        [TestMethod]
        public void repeat()
        {
            Assert.AreEqual("", Evaluate("''.repeat(0)"));
            Assert.AreEqual("ab", Evaluate("'ab'.repeat(1)"));
            Assert.AreEqual("abab", Evaluate("'ab'.repeat(2)"));
            Assert.AreEqual("ababab", Evaluate("'ab'.repeat(3)"));

            Assert.AreEqual("RangeError", EvaluateExceptionType("'ABC'.repeat(-1)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("'ABC'.repeat(Infinity)"));

            // Length
            Assert.AreEqual(1, Evaluate("'ab'.repeat.length"));
        }

        [TestMethod]
        public void raw()
        {
            Assert.AreEqual("t0e1s2t", Evaluate("String.raw({ raw: 'test' }, 0, 1, 2)"));

            // The first parameter must be an object with a "raw" property.
            Assert.AreEqual("TypeError", EvaluateExceptionType("String.raw({}, 0, 1, 2)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("String.raw(5, 0, 1, 2)"));
        }
    }
}
