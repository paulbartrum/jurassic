using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global JSON object.
    /// </summary>
    [TestClass]
    public class JSONTests
    {
        [TestMethod]
        public void parse()
        {
            Assert.AreEqual(5, TestUtils.Evaluate("JSON.parse(5, 5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("JSON.parse(5, {})"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("JSON.parse.length"));

            // Errors
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('')"));
        }

        [TestMethod]
        public void parseKeyword()
        {
            // Literal keywords
            Assert.AreEqual(Null.Value, TestUtils.Evaluate("JSON.parse('null')"));
            Assert.AreEqual(false, TestUtils.Evaluate("JSON.parse('false')"));
            Assert.AreEqual(true, TestUtils.Evaluate("JSON.parse('true')"));

            // With reviver.
            Assert.AreEqual("null!", TestUtils.Evaluate(@"JSON.parse('null', function(name, value) { return value + '!' })"));
            Assert.AreEqual("", TestUtils.Evaluate(@"JSON.parse('null', function(name, value) { return name })"));
            Assert.AreEqual(true, TestUtils.Evaluate(@"JSON.parse('false', function(name, value) { return !value })"));
        }

        [TestMethod]
        public void parseNumber()
        {
            // Numbers
            Assert.AreEqual(0, TestUtils.Evaluate("JSON.parse('0')"));
            Assert.AreEqual(5, TestUtils.Evaluate("JSON.parse('5')"));
            Assert.AreEqual(-5, TestUtils.Evaluate("JSON.parse('-5')"));
            Assert.AreEqual(5.6, TestUtils.Evaluate("JSON.parse('5.6')"));
            Assert.AreEqual(-5.6, TestUtils.Evaluate("JSON.parse('-5.6')"));
            Assert.AreEqual(560, TestUtils.Evaluate("JSON.parse('5.6e2')"));
            Assert.AreEqual(1, TestUtils.Evaluate("JSON.parse('100e-2')"));
            Assert.AreEqual(500, TestUtils.Evaluate("JSON.parse('5E2')"));
            Assert.AreEqual(5, TestUtils.Evaluate("JSON.parse('5E0')"));
            Assert.AreEqual(-500, TestUtils.Evaluate("JSON.parse('-5E+2')"));

            // With reviver.
            Assert.AreEqual(6, TestUtils.Evaluate(@"JSON.parse('5', function(name, value) { return value + 1 })"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate(@"JSON.parse('5', function(name, value) { })"));

            // Invalid numbers
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('05')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('.5')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('.e5')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('-.5')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('5.')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('5e')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('5e+')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('5e-')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('+5')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('5e05')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('12\\t\\r\\n 34')"));
        }

        [TestMethod]
        public void parseString()
        {
            // Strings
            Assert.AreEqual("", TestUtils.Evaluate(@"JSON.parse('""""')"));
            Assert.AreEqual("test", TestUtils.Evaluate(@"JSON.parse('""test""')"));
            Assert.AreEqual("lobster's paridise", TestUtils.Evaluate(@"JSON.parse('""lobster\'s paridise""')"));
            Assert.AreEqual("escapes: \"\\/\b\f\n\r\t", TestUtils.Evaluate(@"JSON.parse('""escapes: \\""\\\\\/\\b\\f\\n\\r\\t""')"));
            Assert.AreEqual("Z①", TestUtils.Evaluate(@"JSON.parse('""\\u005A\\u2460""')"));

            // With reviver.
            Assert.AreEqual("onetwo", TestUtils.Evaluate(@"JSON.parse('""one""', function(name, value) { return value + 'two' })"));

            // Invalid strings
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('""unterminated')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('""tab \t tab""')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('\'test\'')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('""te \\m st""')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('""te \\u st""')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('""te \\u""')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('""te \\uF""')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('""te \\uFF""')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('""te \\uFFF""')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse(""\\u0022abc\\u0022"")"));
        }

        [TestMethod]
        public void parseArray()
        {
            // Arrays
            Assert.AreEqual("", TestUtils.Evaluate("JSON.parse('[]').toString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("JSON.parse('[]').length"));
            Assert.AreEqual("1,2,3", TestUtils.Evaluate("JSON.parse('[1, 2, 3]').toString()"));
            Assert.AreEqual(3, TestUtils.Evaluate("JSON.parse('[1,2,3]').length"));

            // Nested arrays
            Assert.AreEqual("1,2", TestUtils.Evaluate("JSON.parse('[[1 , 2]]').toString()"));
            Assert.AreEqual(1, TestUtils.Evaluate("JSON.parse('[[1 , 2]]').length"));
            Assert.AreEqual(2, TestUtils.Evaluate("JSON.parse('[[1 , 2]]')[0].length"));

            // With reviver.
            Assert.AreEqual("2,3,41", TestUtils.Evaluate(@"JSON.parse('[1,2,3]', function(name, value) { return this[name] + 1 })"));
            Assert.AreEqual("2,3,41", TestUtils.Evaluate(@"JSON.parse('[1,2,3]', function(name, value) { return value + 1 })"));
            Assert.AreEqual("1,,3", TestUtils.Evaluate(@"JSON.parse('[1,2,3]', function(name, value) { return value == 2 ? undefined : value }).toString()"));

            // Invalid arrays
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('[')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('[5')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('[,]')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('[5,]')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('[,5]')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('[5,,5]')"));
        }

        [TestMethod]
        public void parseObject()
        {
            // Objects
            Assert.AreEqual("", TestUtils.Evaluate("Object.keys(JSON.parse('{}')).toString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("Object.keys(JSON.parse('{}')).length"));
            Assert.AreEqual("a", TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": 2}')).toString()"));
            Assert.AreEqual(1, TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": 2}')).length"));
            Assert.AreEqual(2, TestUtils.Evaluate(@"JSON.parse('{""a"": 2}').a"));
            Assert.AreEqual("a,b", TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": 2, ""b"": 3}')).toString()"));
            Assert.AreEqual(2, TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": 2, ""b"": 3}')).length"));
            Assert.AreEqual(3, TestUtils.Evaluate(@"JSON.parse('{""a"": 2, ""b"": 3}').b"));

            // Duplicate keys override earlier values.
            Assert.AreEqual("a", TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": 2, ""a"": 3}')).toString()"));
            Assert.AreEqual(1, TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": 2, ""a"": 3}')).length"));
            Assert.AreEqual(3, TestUtils.Evaluate(@"JSON.parse('{""a"": 2, ""a"": 3}').a"));

            // Nested objects.
            Assert.AreEqual("a", TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": {}}')).toString()"));
            Assert.AreEqual(1, TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": {}}')).length"));
            Assert.AreEqual("", TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": {}}').a).toString()"));

            // With reviver.
            Assert.AreEqual(3, TestUtils.Evaluate(@"JSON.parse('{""a"": 2, ""b"": 3}', function(name, value) { return name == 'a' ? value + 1 : value }).a"));
            Assert.AreEqual("", TestUtils.Evaluate(@"JSON.parse('{""a"": 2, ""b"": 3}', function(name, value) { return name })"));
            Assert.AreEqual(2, TestUtils.Evaluate(@"JSON.parse('{""a"": 2, ""b"": 3}', function(name, value) { return this[name] }).a"));
            Assert.AreEqual("b", TestUtils.Evaluate(@"Object.keys(JSON.parse('{""a"": 2, ""b"": 3}', function(name, value) { return name == 'a' ? undefined : value })).toString()"));

            // Invalid objects
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('{')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('{5')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('{5}')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('{a: 1}')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('{5: 1}')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("JSON.parse('{{}}')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('{""a"":}')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('{""a"" 5}')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('{""a"": 5,}')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('{""a"": 5, ""b""}')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"JSON.parse('{""a"": 5 ""b""}')"));
        }

        [TestMethod]
        public void stringify()
        {
            // Undefined and null.
            Assert.AreEqual("null", TestUtils.Evaluate("JSON.stringify(null)"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("JSON.stringify(undefined)"));

            // Boolean.
            Assert.AreEqual("false", TestUtils.Evaluate("JSON.stringify(false)"));
            Assert.AreEqual("true", TestUtils.Evaluate("JSON.stringify(true)"));
            Assert.AreEqual("false", TestUtils.Evaluate("JSON.stringify(new Boolean(false))"));
            Assert.AreEqual("true", TestUtils.Evaluate("JSON.stringify(new Boolean(true))"));

            // Numbers.
            Assert.AreEqual("5", TestUtils.Evaluate("JSON.stringify(5)"));
            Assert.AreEqual("5e+100", TestUtils.Evaluate("JSON.stringify(5e100)"));
            Assert.AreEqual("5.1", TestUtils.Evaluate("JSON.stringify(5.1)"));
            Assert.AreEqual("5.1", TestUtils.ChangeLocale("es-ES", () => TestUtils.Evaluate("JSON.stringify(5.1)")));
            Assert.AreEqual("null", TestUtils.Evaluate("JSON.stringify(-Infinity)"));
            Assert.AreEqual("null", TestUtils.Evaluate("JSON.stringify(Infinity)"));
            Assert.AreEqual("null", TestUtils.Evaluate("JSON.stringify(NaN)"));
            Assert.AreEqual("5", TestUtils.Evaluate("JSON.stringify(new Number(5))"));
            Assert.AreEqual("null", TestUtils.Evaluate("JSON.stringify(new Number(NaN))"));

            // Strings.
            Assert.AreEqual(@"""test""", TestUtils.Evaluate("JSON.stringify('test')"));
            Assert.AreEqual(@"""\b\f\n\r\t""", TestUtils.Evaluate(@"JSON.stringify('\b\f\n\r\t')"));
            Assert.AreEqual(@"""\u0000""", TestUtils.Evaluate("JSON.stringify(String.fromCharCode(0))"));
            Assert.AreEqual(@"""\u001f""", TestUtils.Evaluate("JSON.stringify(String.fromCharCode(0x1F))"));
            Assert.AreEqual(@"""test""", TestUtils.Evaluate("JSON.stringify(new String('test'))"));
            Assert.AreEqual(@"""te\""st""", TestUtils.Evaluate("JSON.stringify('te\"st')"));
            Assert.AreEqual(@"""te'st""", TestUtils.Evaluate("JSON.stringify('te\\'st')"));

            // Dates.
            Assert.AreEqual("null", TestUtils.Evaluate("JSON.stringify(Date.prototype)"));
            Assert.AreEqual(@"""1970-01-01T00:00:00.005Z""", TestUtils.Evaluate("JSON.stringify(new Date(5))"));

            // Arrays.
            Assert.AreEqual("[]", TestUtils.Evaluate("JSON.stringify([])"));
            Assert.AreEqual("[0,1,2]", TestUtils.Evaluate("JSON.stringify([0, 1, 2])"));
            Assert.AreEqual("[0,null,2]", TestUtils.Evaluate("JSON.stringify([0, , 2])"));
            Assert.AreEqual("[0,null,2]", TestUtils.Evaluate("JSON.stringify([0, undefined, 2])"));

            // Functions.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("JSON.stringify(function() {})"));
            Assert.AreEqual("[null]", TestUtils.Evaluate("JSON.stringify([function() {}])"));
            Assert.AreEqual("{}", TestUtils.Evaluate("JSON.stringify({f: function() {}})"));

            // Objects.
            Assert.AreEqual("{}", TestUtils.Evaluate("JSON.stringify({})"));
            Assert.AreEqual(@"{""a"":1,""b"":2}", TestUtils.Evaluate("JSON.stringify({a: 1, b: 2})"));

            // Cyclic reference
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("a = []; a[0] = a; JSON.stringify(a)"));
        }

        [TestMethod]
        public void stringifyWithReplacer()
        {
            // The replacer can be a function.
            Assert.AreEqual("6", TestUtils.Evaluate("JSON.stringify(5, function(key, value) { return value + 1 })"));
            Assert.AreEqual("6", TestUtils.Evaluate("JSON.stringify(5, function(key, value) { return this[key] + 1 })"));
            Assert.AreEqual(@"""[object Object]1""", TestUtils.Evaluate("JSON.stringify({a: 1, b: 2}, function(key, value) { return value + 1 })"));
            Assert.AreEqual(@"{""x"":3,""arr"":[3]}", TestUtils.Evaluate("JSON.stringify({x:1,arr:[1]}, function (k,v) { return typeof v === 'number' ? 3 : v })"));

            // Or the replacer can be an array of property names (only affects objects, not arrays or primitives).
            Assert.AreEqual("5", TestUtils.Evaluate("JSON.stringify(5, ['b'])"));
            Assert.AreEqual("[5]", TestUtils.Evaluate("JSON.stringify([5], ['b'])"));
            Assert.AreEqual(@"{""b"":2}", TestUtils.Evaluate("JSON.stringify({a: 1, b: 2, c: 3}, ['b'])"));
            Assert.AreEqual(@"{""b"":2}", TestUtils.Evaluate("JSON.stringify({a: 1, b: 2, c: 3}, ['b', 'b'])"));    // IE, Chrome and Firefox all get this wrong as of 16 June 10.

            // Torture test
            //Assert.AreEqual("5", TestUtils.Evaluate("arr = new Array(); arr[4294967294] = 'b'; JSON.stringify({a: 1, b: 2, c: 3}, arr)"));
        }

        [TestMethod]
        public void stringifyWithSpacer()
        {
            // Primitives (the spacer has no effect).
            Assert.AreEqual("null", TestUtils.Evaluate("JSON.stringify(null, undefined, 4)"));
            Assert.AreEqual("false", TestUtils.Evaluate("JSON.stringify(false, undefined, 4)"));
            Assert.AreEqual("true", TestUtils.Evaluate("JSON.stringify(true, undefined, 4)"));
            Assert.AreEqual("5", TestUtils.Evaluate("JSON.stringify(5, undefined, 4)"));
            Assert.AreEqual("null", TestUtils.Evaluate("JSON.stringify(NaN, undefined, 4)"));
            Assert.AreEqual(@"""test""", TestUtils.Evaluate("JSON.stringify('test', undefined, 4)"));

            // Arrays.
            Assert.AreEqual("[]", TestUtils.Evaluate("JSON.stringify([], undefined, 4)"));
            Assert.AreEqual("[\n    0,\n    1,\n    2\n]", TestUtils.Evaluate("JSON.stringify([0, 1, 2], undefined, 4)"));
            Assert.AreEqual("[\n    0,\n    null,\n    2\n]", TestUtils.Evaluate("JSON.stringify([0, , 2], undefined, 4)"));
            Assert.AreEqual("[\n    [\n        1,\n        2\n    ],\n    [\n        3,\n        4\n    ]\n]",
                TestUtils.Evaluate("JSON.stringify([[1, 2], [3, 4]], undefined, 4)"));
            Assert.AreEqual("[\nabc0,\nabc1,\nabc2\n]", TestUtils.Evaluate("JSON.stringify([0, 1, 2], undefined, 'abc')"));
            Assert.AreEqual("[\nabc[\nabcabc1,\nabcabc2\nabc],\nabc[\nabcabc3,\nabcabc4\nabc]\n]",
                TestUtils.Evaluate("JSON.stringify([[1, 2], [3, 4]], undefined, 'abc')"));

            // Objects.
            Assert.AreEqual("{}", TestUtils.Evaluate("JSON.stringify({}, undefined, 4)"));
            Assert.AreEqual("{\n    \"a\": 1,\n    \"b\": 2\n}", TestUtils.Evaluate("JSON.stringify({a: 1, b: 2}, undefined, 4)"));
            Assert.AreEqual("{\n    \"a\": {\n        \"b\": 1\n    },\n    \"c\": {\n        \"d\": 2\n    }\n}",
                TestUtils.Evaluate("JSON.stringify({a: {b: 1}, c: {d: 2}}, undefined, 4)"));
            Assert.AreEqual("{\nabc\"a\": 1,\nabc\"b\": 2\n}", TestUtils.Evaluate("JSON.stringify({a: 1, b: 2}, undefined, 'abc')"));
            Assert.AreEqual("{\nabc\"a\": {\nabcabc\"b\": 1\nabc},\nabc\"c\": {\nabcabc\"d\": 2\nabc}\n}",
                TestUtils.Evaluate("JSON.stringify({a: {b: 1}, c: {d: 2}}, undefined, 'abc')"));

            // Test bounds.
            Assert.AreEqual("[1]", TestUtils.Evaluate("JSON.stringify([1], undefined, -1)"));
            Assert.AreEqual("[1]", TestUtils.Evaluate("JSON.stringify([1], undefined, 0)"));
            Assert.AreEqual("[\n         1\n]", TestUtils.Evaluate("JSON.stringify([1], undefined, 9)"));
            Assert.AreEqual("[\n          1\n]", TestUtils.Evaluate("JSON.stringify([1], undefined, 10)"));
            Assert.AreEqual("[\n          1\n]", TestUtils.Evaluate("JSON.stringify([1], undefined, 11)"));
            Assert.AreEqual("[\n0123456781\n]", TestUtils.Evaluate("JSON.stringify([1], undefined, '012345678')"));
            Assert.AreEqual("[\n01234567891\n]", TestUtils.Evaluate("JSON.stringify([1], undefined, '0123456789')"));
            Assert.AreEqual("[\n01234567891\n]", TestUtils.Evaluate("JSON.stringify([1], undefined, '01234567890')"));
        }
    }
}
