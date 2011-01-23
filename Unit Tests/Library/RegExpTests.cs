using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global RegExp object.
    /// </summary>
    [TestClass]
    public class RegExpTests
    {
        [TestMethod]
        public void Constructor()
        {
            // toString and valueOf.
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("function RegExp() { [native code] }", TestUtils.Evaluate("RegExp.toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("RegExp.valueOf() === RegExp"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("RegExp.length"));

            // for-in
            Assert.AreEqual("$1,$2,$3,$4,$5,$6,$7,$8,$9,input,lastMatch,lastParen,leftContext,rightContext", TestUtils.Evaluate("y = []; for (var x in RegExp) { y.push(x) } y.sort().toString()"));
            Assert.AreEqual("", TestUtils.Evaluate("y = []; for (var x in new RegExp('abc', 'g')) { y.push(x) } y.sort().toString()"));
        }

        [TestMethod]
        public void Call()
        {
            // RegExp
            Assert.AreEqual("", TestUtils.Evaluate("RegExp().source"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp(undefined).source"));

            // RegExp(pattern)
            TestUtils.Evaluate("var x = RegExp('abc')");
            Assert.AreEqual("abc", TestUtils.Evaluate("x.source"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.global"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.multiline"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));

            // RegExp(pattern, flags)
            TestUtils.Evaluate("var x = RegExp('abc', 'g')");
            Assert.AreEqual("abc", TestUtils.Evaluate("x.source"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.global"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.multiline"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));

            // RegExp(regExp)
            TestUtils.Evaluate("var x = RegExp(new RegExp('abc', 'g'))");
            Assert.AreEqual("abc", TestUtils.Evaluate("x.source"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.global"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.multiline"));
            Assert.AreEqual(true, TestUtils.Evaluate("x === RegExp(x)"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));

            // RegExp(regExp, flags)
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "RegExpError" : "TypeError",
                TestUtils.EvaluateExceptionType("RegExp(new RegExp('abc', 'g'), 'i')"));
        }

        [TestMethod]
        public void Construction()
        {
            // new RegExp()
            Assert.AreEqual("", TestUtils.Evaluate("new RegExp().source"));
            Assert.AreEqual("", TestUtils.Evaluate("new RegExp(undefined).source"));

            // new RegExp(pattern)
            TestUtils.Evaluate("var x = new RegExp('abc')");
            Assert.AreEqual("abc", TestUtils.Evaluate("x.source"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.global"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.multiline"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));

            // new RegExp(pattern, flags)
            TestUtils.Evaluate("var x = new RegExp('abc', 'g')");
            Assert.AreEqual("abc", TestUtils.Evaluate("x.source"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.global"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.multiline"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));

            // new RegExp(regExp)
            TestUtils.Evaluate("var x = new RegExp(new RegExp('abc', 'g'))");
            Assert.AreEqual("abc", TestUtils.Evaluate("x.source"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.global"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.multiline"));
            Assert.AreEqual(false, TestUtils.Evaluate("x === new RegExp(x)"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));

            // new RegExp(regExp, flags)
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "RegExpError" : "TypeError",
                TestUtils.EvaluateExceptionType("new RegExp(new RegExp('abc', 'g'), 'i')"));
            Assert.AreEqual("abc", TestUtils.Evaluate("new RegExp(/abc/, undefined).source"));

            // Flags must be known and unique.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("new RegExp('abc', 'gg')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("new RegExp('abc', 'igi')"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("new RegExp('abc', 'a')"));
        }

        [TestMethod]
        public void source()
        {
            Assert.AreEqual("abc", TestUtils.Evaluate("new RegExp('abc', 'g').source"));
            Assert.AreEqual("abc", TestUtils.Evaluate("/abc/g.source"));
            Assert.AreEqual("[a-z]*", TestUtils.Evaluate("/[a-z]*/g.source"));

            // This property is non-writable, non-configurable and non-enumerable.
            Assert.AreEqual("abc", TestUtils.Evaluate("var x = new RegExp('abc'); x.source = 'test'; x.source"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); delete x.source"));
            Assert.AreEqual("abc", TestUtils.Evaluate("var x = new RegExp('abc'); delete x.source; x.source"));
        }

        [TestMethod]
        public void global()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("new RegExp('abc', 'i').global"));
            Assert.AreEqual(true, TestUtils.Evaluate("new RegExp('abc', 'g').global"));
            Assert.AreEqual(false, TestUtils.Evaluate("/abc/.global"));
            Assert.AreEqual(true, TestUtils.Evaluate("/abc/g.global"));

            // This property is non-writable, non-configurable and non-enumerable.
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.global = true; x.global"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.global = true; delete x.global"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.global = true; delete x.global; x.global"));
        }

        [TestMethod]
        public void ignoreCase()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("new RegExp('abc', 'i').ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("new RegExp('abc', 'g').ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("/abc/.ignoreCase"));
            Assert.AreEqual(true, TestUtils.Evaluate("/abc/i.ignoreCase"));

            // This property is non-writable, non-configurable and non-enumerable.
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.ignoreCase = true; x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.ignoreCase = true; delete x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.ignoreCase = true; delete x.ignoreCase; x.ignoreCase"));
        }

        [TestMethod]
        public void multiline()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("new RegExp('abc', 'i').multiline"));
            Assert.AreEqual(true, TestUtils.Evaluate("new RegExp('abc', 'm').multiline"));
            Assert.AreEqual(false, TestUtils.Evaluate("/abc/.multiline"));
            Assert.AreEqual(true, TestUtils.Evaluate("/abc/m.multiline"));

            // This property is non-writable, non-configurable and non-enumerable.
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.multiline = true; x.multiline"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.multiline = true; delete x.multiline"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.multiline = true; delete x.multiline; x.multiline"));
        }

        [TestMethod]
        public void lastIndex()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("new RegExp('abc', 'i').lastIndex"));
            Assert.AreEqual(0, TestUtils.Evaluate("/abc/.lastIndex"));

            // This property is writable, non-configurable and non-enumerable.
            Assert.AreEqual(5, TestUtils.Evaluate("var x = new RegExp('abc'); x.lastIndex = 5; x.lastIndex"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new RegExp('abc'); x.lastIndex = 5; delete x.lastIndex"));
            Assert.AreEqual(5, TestUtils.Evaluate("var x = new RegExp('abc'); x.lastIndex = 5; delete x.lastIndex; x.lastIndex"));
        }
        
        [TestMethod]
        public void compile()
        {
            // compile(pattern)
            TestUtils.Evaluate("var x = new RegExp('abc', 'g')");
            TestUtils.Evaluate("x.lastIndex = 1;");
            TestUtils.Evaluate("x.compile('cde')");
            Assert.AreEqual("cde", TestUtils.Evaluate("x.source"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.global"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.multiline"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));

            // compile(pattern, flags)
            TestUtils.Evaluate("var x = new RegExp('abc', 'g')");
            TestUtils.Evaluate("x.lastIndex = 1;");
            TestUtils.Evaluate("x.compile('cde', 'i')");
            Assert.AreEqual("cde", TestUtils.Evaluate("x.source"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.global"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.multiline"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));
        }

        [TestMethod]
        public void exec()
        {
            // If there is no match then exec returns null.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate("/abc/.exec('hello')"));

            // If there is a match, it returns an array.
            TestUtils.Evaluate("var result = /abc/.exec('helloabchello')");
            Assert.AreEqual("helloabchello", TestUtils.Evaluate("result.input"));
            Assert.AreEqual(5, TestUtils.Evaluate("result.index"));
            Assert.AreEqual(1, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("abc", TestUtils.Evaluate("result[0]"));

            // Try a capture.
            TestUtils.Evaluate("var result = /li(..)le/.exec('santas little reindeers')");
            Assert.AreEqual("santas little reindeers", TestUtils.Evaluate("result.input"));
            Assert.AreEqual(7, TestUtils.Evaluate("result.index"));
            Assert.AreEqual(2, TestUtils.Evaluate("result.length"));
            Assert.AreEqual("little", TestUtils.Evaluate("result[0]"));
            Assert.AreEqual("tt", TestUtils.Evaluate("result[1]"));

            // The lastIndex field should be used to indicate the start position, but
            // only if the global flag is set.
            TestUtils.Evaluate("var x = new RegExp('abc', 'g')");
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));
            TestUtils.Evaluate("x.exec('helloabchello')");
            Assert.AreEqual(8, TestUtils.Evaluate("x.lastIndex"));
            TestUtils.Evaluate("x.lastIndex = 7");
            TestUtils.Evaluate("x.exec('helloabchello')");
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));

            // If the global flag is not set, ignore the lastIndex field.
            TestUtils.Evaluate("var x = new RegExp('abc', 'm')");
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));
            TestUtils.Evaluate("x.exec('helloabchello')");
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? 8 : 0, TestUtils.Evaluate("x.lastIndex"));
            TestUtils.Evaluate("x.lastIndex = 7");
            TestUtils.Evaluate("x.exec('helloabchello')");
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? 8 : 7, TestUtils.Evaluate("x.lastIndex"));

            // Test the deprecated RegExp properties.
            TestUtils.Evaluate("/li(..)le/.exec('santas little reindeers')");
            Assert.AreEqual("tt", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("santas little reindeers", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("santas little reindeers", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("little", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("little", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("tt", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("tt", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("santas ", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("santas ", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual(" reindeers", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual(" reindeers", TestUtils.Evaluate("RegExp[\"$'\"]"));

            TestUtils.Evaluate("/(li|re)(in)?(tt|deer)/.exec('santas little reindeers')");
            Assert.AreEqual("li", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("tt", TestUtils.Evaluate("RegExp.$3"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$4"));
            Assert.AreEqual("santas little reindeers", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("santas little reindeers", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("litt", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("litt", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("tt", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("tt", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("santas ", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("santas ", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("le reindeers", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("le reindeers", TestUtils.Evaluate("RegExp[\"$'\"]"));

            TestUtils.Evaluate("/nomatch/.exec('santas little reindeers')");
            Assert.AreEqual("li", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("tt", TestUtils.Evaluate("RegExp.$3"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$4"));
            Assert.AreEqual("santas little reindeers", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("santas little reindeers", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("litt", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("litt", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("tt", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("tt", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("santas ", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("santas ", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("le reindeers", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("le reindeers", TestUtils.Evaluate("RegExp[\"$'\"]"));
        }

        [TestMethod]
        public void test()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("/abc/.test('hello')"));
            Assert.AreEqual(true, TestUtils.Evaluate("/abc/.test('helloabchello')"));
            Assert.AreEqual(true, TestUtils.Evaluate(@"/\s^/m.test('\n')"));

            // The lastIndex field should be used to indicate the start position, but
            // only if the global flag is set.
            TestUtils.Evaluate("var x = new RegExp('abc', 'g')");
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.test('helloabchello')"));
            Assert.AreEqual(8, TestUtils.Evaluate("x.lastIndex"));
            TestUtils.Evaluate("x.lastIndex = 7");
            Assert.AreEqual(false, TestUtils.Evaluate("x.test('helloabchello')"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));

            // If the global flag is not set, ignore the lastIndex field.
            TestUtils.Evaluate("var x = new RegExp('abc', 'm')");
            Assert.AreEqual(0, TestUtils.Evaluate("x.lastIndex"));
            Assert.AreEqual(true, TestUtils.Evaluate("x.test('helloabchello')"));
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? 8 : 0, TestUtils.Evaluate("x.lastIndex"));
            TestUtils.Evaluate("x.lastIndex = 7");
            Assert.AreEqual(true, TestUtils.Evaluate("x.test('helloabchello')"));
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? 8 : 7, TestUtils.Evaluate("x.lastIndex"));

            // Test the deprecated RegExp properties.
            TestUtils.Evaluate("/te(..)(s|i)/.test('terrible teens')");
            Assert.AreEqual("rr", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("i", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$3"));
            Assert.AreEqual("terrible teens", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("terrible teens", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("terri", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("terri", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("i", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("i", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("ble teens", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("ble teens", TestUtils.Evaluate("RegExp[\"$'\"]"));

            TestUtils.Evaluate("/notamatch/.test('my little friend')");
            Assert.AreEqual("rr", TestUtils.Evaluate("RegExp.$1"));
            Assert.AreEqual("i", TestUtils.Evaluate("RegExp.$2"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.$3"));
            Assert.AreEqual("terrible teens", TestUtils.Evaluate("RegExp.input"));
            Assert.AreEqual("terrible teens", TestUtils.Evaluate("RegExp.$_"));
            Assert.AreEqual("terri", TestUtils.Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("terri", TestUtils.Evaluate("RegExp['$&']"));
            Assert.AreEqual("i", TestUtils.Evaluate("RegExp.lastParen"));
            Assert.AreEqual("i", TestUtils.Evaluate("RegExp['$+']"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp.leftContext"));
            Assert.AreEqual("", TestUtils.Evaluate("RegExp['$`']"));
            Assert.AreEqual("ble teens", TestUtils.Evaluate("RegExp.rightContext"));
            Assert.AreEqual("ble teens", TestUtils.Evaluate("RegExp[\"$'\"]"));
        }

        //[TestMethod]
        //public void RegExpProperties()
        //{
        //    // exec() that does not match should not change the global RegExp properties.
        //    TestUtils.Evaluate("RegExp.input = 'before'");
        //    TestUtils.Evaluate("/test/.exec('string that does not match')");
        //    Assert.AreEqual("before", TestUtils.Evaluate("RegExp.input"));

        //    // exec() that does match does change the global RegExp properties.
        //    TestUtils.Evaluate("RegExp.input = '1'");
        //    TestUtils.Evaluate("/(ex)(ec|ex)/.exec('for exec testing')");
        //    Assert.AreEqual("for exec testing", TestUtils.Evaluate("RegExp.input"));
        //    Assert.AreEqual("exec", TestUtils.Evaluate("RegExp.lastMatch"));
        //    Assert.AreEqual("ec", TestUtils.Evaluate("RegExp.lastParen"));
        //    Assert.AreEqual("for ", TestUtils.Evaluate("RegExp.leftContext"));
        //    Assert.AreEqual(" testing", TestUtils.Evaluate("RegExp.rightContext"));
        //    Assert.AreEqual("ex", TestUtils.Evaluate("RegExp.$1"));
        //    Assert.AreEqual("ec", TestUtils.Evaluate("RegExp.$2"));
        //    Assert.AreEqual("", TestUtils.Evaluate("RegExp.$3"));

        //    // test() that does not match should not change the global RegExp properties.
        //    TestUtils.Evaluate("RegExp.input = 'before'");
        //    TestUtils.Evaluate("/test/.test('string that does not match')");
        //    Assert.AreEqual("before", TestUtils.Evaluate("RegExp.input"));

        //    // test() that does match does change the global RegExp properties.
        //    TestUtils.Evaluate("RegExp.input = 'before'");
        //    TestUtils.Evaluate("/(te)(st|mp)/.test('for testing')");
        //    Assert.AreEqual("for testing", TestUtils.Evaluate("RegExp.input"));
        //    Assert.AreEqual("test", TestUtils.Evaluate("RegExp.lastMatch"));
        //    Assert.AreEqual("st", TestUtils.Evaluate("RegExp.lastParen"));
        //    Assert.AreEqual("for ", TestUtils.Evaluate("RegExp.leftContext"));
        //    Assert.AreEqual("ing", TestUtils.Evaluate("RegExp.rightContext"));
        //    Assert.AreEqual("te", TestUtils.Evaluate("RegExp.$1"));
        //    Assert.AreEqual("st", TestUtils.Evaluate("RegExp.$2"));
        //    Assert.AreEqual("", TestUtils.Evaluate("RegExp.$3"));
        //}

        [TestMethod]
        public void NonParticipatingGroups()
        {
            // Note: the following tests are buggy in IE 7.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            Assert.AreEqual(true, TestUtils.Evaluate(@"/(x)?\1y/.test('y')"));
            Assert.AreEqual(@"[""y"",null]", TestUtils.Evaluate(@"JSON.stringify(/(x)?\1y/.exec('y'))"));
            Assert.AreEqual(@"[""y"",null]", TestUtils.Evaluate(@"JSON.stringify(/(x)?y/.exec('y'))"));
            Assert.AreEqual(@"[""y"",null]", TestUtils.Evaluate(@"JSON.stringify('y'.match(/(x)?\1y/))"));
            Assert.AreEqual(@"[""y"",null]", TestUtils.Evaluate(@"JSON.stringify('y'.match(/(x)?y/))"));
            Assert.AreEqual(@"[""y""]", TestUtils.Evaluate(@"JSON.stringify('y'.match(/(x)?\1y/g))"));
            Assert.AreEqual(@"["""",null,""""]", TestUtils.Evaluate(@"JSON.stringify('y'.split(/(x)?\1y/))"));
            Assert.AreEqual(@"["""",null,""""]", TestUtils.Evaluate(@"JSON.stringify('y'.split(/(x)?y/))"));
            Assert.AreEqual(0, TestUtils.Evaluate(@"'y'.search(/(x)?\1y/)"));
            Assert.AreEqual("z", TestUtils.Evaluate(@"'y'.replace(/(x)?\1y/, 'z')"));
            Assert.AreEqual("", TestUtils.Evaluate(@"'y'.replace(/(x)?y/, '$1')"));
            Assert.AreEqual("undefined", TestUtils.Evaluate(@"'y'.replace(/(x)?\1y/, function($0, $1){ return String($1); })"));
            Assert.AreEqual("undefined", TestUtils.Evaluate(@"'y'.replace(/(x)?y/, function($0, $1){ return String($1); })"));
            Assert.AreEqual("undefined", TestUtils.Evaluate(@"'y'.replace(/(x)?y/, function($0, $1){ return $1; })"));
        }
    }
}
