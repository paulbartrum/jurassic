using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global RegExp object.
    /// </summary>
    [TestClass]
    public class RegExpTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // toString and valueOf.
            Assert.AreEqual("function RegExp() { [native code] }", Evaluate("RegExp.toString()"));
            Assert.AreEqual(true, Evaluate("RegExp.valueOf() === RegExp"));

            // length
            Assert.AreEqual(2, Evaluate("RegExp.length"));

            // for-in
            Assert.AreEqual("$1,$2,$3,$4,$5,$6,$7,$8,$9,input,lastMatch,lastParen,leftContext,rightContext", Evaluate("y = []; for (var x in RegExp) { y.push(x) } y.sort().toString()"));
            Assert.AreEqual("", Evaluate("y = []; for (var x in new RegExp('abc', 'g')) { y.push(x) } y.sort().toString()"));
        }

        [TestMethod]
        public void Call()
        {
            // RegExp
            Assert.AreEqual("", Evaluate("RegExp().source"));
            Assert.AreEqual("", Evaluate("RegExp(undefined).source"));

            // RegExp(pattern)
            Evaluate("var x = RegExp('abc')");
            Assert.AreEqual("abc", Evaluate("x.source"));
            Assert.AreEqual(false, Evaluate("x.global"));
            Assert.AreEqual(false, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // RegExp(pattern, flags)
            Evaluate("var x = RegExp('abc', 'g')");
            Assert.AreEqual("abc", Evaluate("x.source"));
            Assert.AreEqual(true, Evaluate("x.global"));
            Assert.AreEqual(false, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // RegExp(regExp)
            Evaluate("var x = RegExp(new RegExp('abc', 'g'))");
            Assert.AreEqual("abc", Evaluate("x.source"));
            Assert.AreEqual(true, Evaluate("x.global"));
            Assert.AreEqual(false, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(true, Evaluate("x === RegExp(x)"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // RegExp(regExp, flags)
            Evaluate("var x = RegExp(new RegExp('abc', 'g'), 'i')");
            Assert.AreEqual("abc", Evaluate("x.source"));
            Assert.AreEqual(false, Evaluate("x.global"));
            Assert.AreEqual(true, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));
        }

        [TestMethod]
        public void Construction()
        {
            // new RegExp()
            Assert.AreEqual("", Evaluate("new RegExp().source"));
            Assert.AreEqual("", Evaluate("new RegExp(undefined).source"));

            // new RegExp(pattern)
            Evaluate("var x = new RegExp('abc')");
            Assert.AreEqual("abc", Evaluate("x.source"));
            Assert.AreEqual(false, Evaluate("x.global"));
            Assert.AreEqual(false, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // new RegExp(pattern, flags)
            Evaluate("var x = new RegExp('abc', 'g')");
            Assert.AreEqual("abc", Evaluate("x.source"));
            Assert.AreEqual(true, Evaluate("x.global"));
            Assert.AreEqual(false, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // new RegExp(regExp)
            Evaluate("var x = new RegExp(new RegExp('abc', 'g'))");
            Assert.AreEqual("abc", Evaluate("x.source"));
            Assert.AreEqual(true, Evaluate("x.global"));
            Assert.AreEqual(false, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(false, Evaluate("x === new RegExp(x)"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // new RegExp(regExp, flags)
            Evaluate("var x = new RegExp(new RegExp('abc', 'g'), 'i')");
            Assert.AreEqual("abc", Evaluate("x.source"));
            Assert.AreEqual(false, Evaluate("x.global"));
            Assert.AreEqual(true, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // Flags must be known and unique.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("new RegExp('abc', 'gg')"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("new RegExp('abc', 'igi')"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("new RegExp('abc', 'a')"));
        }

        [TestMethod]
        public void source()
        {
            Assert.AreEqual("abc", Evaluate("new RegExp('abc', 'g').source"));
            Assert.AreEqual("abc", Evaluate("/abc/g.source"));
            Assert.AreEqual("[a-z]*", Evaluate("/[a-z]*/g.source"));

            // This property is non-writable, non-configurable and non-enumerable.
            Assert.AreEqual("abc", Evaluate("var x = new RegExp('abc'); x.source = 'test'; x.source"));
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); delete x.source"));
            Assert.AreEqual("abc", Evaluate("var x = new RegExp('abc'); delete x.source; x.source"));
        }

        [TestMethod]
        public void flags()
        {
            Assert.AreEqual("g", Evaluate("new RegExp('abc', 'g').flags"));
            Assert.AreEqual("g", Evaluate("(/abc/g).flags"));
            Assert.AreEqual("", Evaluate("(/abc/).flags"));

            // This property is non-writable, non-configurable and non-enumerable.
            Assert.AreEqual("g", Evaluate("var x = new RegExp('abc', 'g'); x.flags = 'test'; x.flags"));
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc', 'g'); delete x.flags"));
            Assert.AreEqual("g", Evaluate("var x = new RegExp('abc', 'g'); delete x.flags; x.flags"));
        }

        [TestMethod]
        public void global()
        {
            Assert.AreEqual(false, Evaluate("new RegExp('abc', 'i').global"));
            Assert.AreEqual(true, Evaluate("new RegExp('abc', 'g').global"));
            Assert.AreEqual(false, Evaluate("/abc/.global"));
            Assert.AreEqual(true, Evaluate("/abc/g.global"));

            // This property is non-writable, non-configurable and non-enumerable.
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.global = true; x.global"));
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.global = true; delete x.global"));
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.global = true; delete x.global; x.global"));
        }

        [TestMethod]
        public void ignoreCase()
        {
            Assert.AreEqual(true, Evaluate("new RegExp('abc', 'i').ignoreCase"));
            Assert.AreEqual(false, Evaluate("new RegExp('abc', 'g').ignoreCase"));
            Assert.AreEqual(false, Evaluate("/abc/.ignoreCase"));
            Assert.AreEqual(true, Evaluate("/abc/i.ignoreCase"));

            // This property is non-writable, non-configurable and non-enumerable.
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.ignoreCase = true; x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.ignoreCase = true; delete x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.ignoreCase = true; delete x.ignoreCase; x.ignoreCase"));
        }

        [TestMethod]
        public void multiline()
        {
            Assert.AreEqual(false, Evaluate("new RegExp('abc', 'i').multiline"));
            Assert.AreEqual(true, Evaluate("new RegExp('abc', 'm').multiline"));
            Assert.AreEqual(false, Evaluate("/abc/.multiline"));
            Assert.AreEqual(true, Evaluate("/abc/m.multiline"));

            // This property is non-writable, non-configurable and non-enumerable.
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.multiline = true; x.multiline"));
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.multiline = true; delete x.multiline"));
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.multiline = true; delete x.multiline; x.multiline"));
        }

        [TestMethod]
        public void lastIndex()
        {
            Assert.AreEqual(0, Evaluate("new RegExp('abc', 'i').lastIndex"));
            Assert.AreEqual(0, Evaluate("/abc/.lastIndex"));

            // This property is writable, non-configurable and non-enumerable.
            Assert.AreEqual(5, Evaluate("var x = new RegExp('abc'); x.lastIndex = 5; x.lastIndex"));
            Assert.AreEqual(false, Evaluate("var x = new RegExp('abc'); x.lastIndex = 5; delete x.lastIndex"));
            Assert.AreEqual(5, Evaluate("var x = new RegExp('abc'); x.lastIndex = 5; delete x.lastIndex; x.lastIndex"));
        }
        
        [TestMethod]
        public void compile()
        {
            // compile(pattern)
            Evaluate("var x = new RegExp('abc', 'g')");
            Evaluate("x.lastIndex = 1;");
            Evaluate("x.compile('cde')");
            Assert.AreEqual("cde", Evaluate("x.source"));
            Assert.AreEqual(false, Evaluate("x.global"));
            Assert.AreEqual(false, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // compile(pattern, flags)
            Evaluate("var x = new RegExp('abc', 'g')");
            Evaluate("x.lastIndex = 1;");
            Evaluate("x.compile('cde', 'i')");
            Assert.AreEqual("cde", Evaluate("x.source"));
            Assert.AreEqual(false, Evaluate("x.global"));
            Assert.AreEqual(true, Evaluate("x.ignoreCase"));
            Assert.AreEqual(false, Evaluate("x.multiline"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));
        }

        [TestMethod]
        public void exec()
        {
            // If there is no match then exec returns null.
            Assert.AreEqual(Null.Value, Evaluate("/abc/.exec('hello')"));

            // If there is a match, it returns an array.
            Evaluate("var result = /abc/.exec('helloabchello')");
            Assert.AreEqual("helloabchello", Evaluate("result.input"));
            Assert.AreEqual(5, Evaluate("result.index"));
            Assert.AreEqual(1, Evaluate("result.length"));
            Assert.AreEqual("abc", Evaluate("result[0]"));

            // Try a capture.
            Evaluate("var result = /li(..)le/.exec('santas little reindeers')");
            Assert.AreEqual("santas little reindeers", Evaluate("result.input"));
            Assert.AreEqual(7, Evaluate("result.index"));
            Assert.AreEqual(2, Evaluate("result.length"));
            Assert.AreEqual("little", Evaluate("result[0]"));
            Assert.AreEqual("tt", Evaluate("result[1]"));

            // The lastIndex field should be used to indicate the start position, but
            // only if the global flag is set.
            Evaluate("var x = new RegExp('abc', 'g')");
            Assert.AreEqual(0, Evaluate("x.lastIndex"));
            Evaluate("x.exec('helloabchello')");
            Assert.AreEqual(8, Evaluate("x.lastIndex"));
            Evaluate("x.lastIndex = 7");
            Evaluate("x.exec('helloabchello')");
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // If the global flag is not set, ignore the lastIndex field.
            Evaluate("var x = new RegExp('abc', 'm')");
            Assert.AreEqual(0, Evaluate("x.lastIndex"));
            Evaluate("x.exec('helloabchello')");
            Assert.AreEqual(0, Evaluate("x.lastIndex"));
            Evaluate("x.lastIndex = 7");
            Evaluate("x.exec('helloabchello')");
            Assert.AreEqual(7, Evaluate("x.lastIndex"));

            // Test the deprecated RegExp properties.
            Evaluate("/li(..)le/.exec('santas little reindeers')");
            Assert.AreEqual("tt", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("santas little reindeers", Evaluate("RegExp.input"));
            Assert.AreEqual("santas little reindeers", Evaluate("RegExp.$_"));
            Assert.AreEqual("little", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("little", Evaluate("RegExp['$&']"));
            Assert.AreEqual("tt", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("tt", Evaluate("RegExp['$+']"));
            Assert.AreEqual("santas ", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("santas ", Evaluate("RegExp['$`']"));
            Assert.AreEqual(" reindeers", Evaluate("RegExp.rightContext"));
            Assert.AreEqual(" reindeers", Evaluate("RegExp[\"$'\"]"));

            Evaluate("/(li|re)(in)?(tt|deer)/.exec('santas little reindeers')");
            Assert.AreEqual("li", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("tt", Evaluate("RegExp.$3"));
            Assert.AreEqual("", Evaluate("RegExp.$4"));
            Assert.AreEqual("santas little reindeers", Evaluate("RegExp.input"));
            Assert.AreEqual("santas little reindeers", Evaluate("RegExp.$_"));
            Assert.AreEqual("litt", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("litt", Evaluate("RegExp['$&']"));
            Assert.AreEqual("tt", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("tt", Evaluate("RegExp['$+']"));
            Assert.AreEqual("santas ", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("santas ", Evaluate("RegExp['$`']"));
            Assert.AreEqual("le reindeers", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("le reindeers", Evaluate("RegExp[\"$'\"]"));

            Evaluate("/nomatch/.exec('santas little reindeers')");
            Assert.AreEqual("li", Evaluate("RegExp.$1"));
            Assert.AreEqual("", Evaluate("RegExp.$2"));
            Assert.AreEqual("tt", Evaluate("RegExp.$3"));
            Assert.AreEqual("", Evaluate("RegExp.$4"));
            Assert.AreEqual("santas little reindeers", Evaluate("RegExp.input"));
            Assert.AreEqual("santas little reindeers", Evaluate("RegExp.$_"));
            Assert.AreEqual("litt", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("litt", Evaluate("RegExp['$&']"));
            Assert.AreEqual("tt", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("tt", Evaluate("RegExp['$+']"));
            Assert.AreEqual("santas ", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("santas ", Evaluate("RegExp['$`']"));
            Assert.AreEqual("le reindeers", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("le reindeers", Evaluate("RegExp[\"$'\"]"));
        }

        [TestMethod]
        public void test()
        {
            Assert.AreEqual(false, Evaluate("/abc/.test('hello')"));
            Assert.AreEqual(true, Evaluate("/abc/.test('helloabchello')"));
            Assert.AreEqual(true, Evaluate(@"/\s^/m.test('\n')"));

            // The lastIndex field should be used to indicate the start position, but
            // only if the global flag is set.
            Evaluate("var x = new RegExp('abc', 'g')");
            Assert.AreEqual(0, Evaluate("x.lastIndex"));
            Assert.AreEqual(true, Evaluate("x.test('helloabchello')"));
            Assert.AreEqual(8, Evaluate("x.lastIndex"));
            Evaluate("x.lastIndex = 7");
            Assert.AreEqual(false, Evaluate("x.test('helloabchello')"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));

            // If the global flag is not set, ignore the lastIndex field.
            Evaluate("var x = new RegExp('abc', 'm')");
            Assert.AreEqual(0, Evaluate("x.lastIndex"));
            Assert.AreEqual(true, Evaluate("x.test('helloabchello')"));
            Assert.AreEqual(0, Evaluate("x.lastIndex"));
            Evaluate("x.lastIndex = 7");
            Assert.AreEqual(true, Evaluate("x.test('helloabchello')"));
            Assert.AreEqual(7, Evaluate("x.lastIndex"));

            // Test the deprecated RegExp properties.
            Evaluate("/te(..)(s|i)/.test('terrible teens')");
            Assert.AreEqual("rr", Evaluate("RegExp.$1"));
            Assert.AreEqual("i", Evaluate("RegExp.$2"));
            Assert.AreEqual("", Evaluate("RegExp.$3"));
            Assert.AreEqual("terrible teens", Evaluate("RegExp.input"));
            Assert.AreEqual("terrible teens", Evaluate("RegExp.$_"));
            Assert.AreEqual("terri", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("terri", Evaluate("RegExp['$&']"));
            Assert.AreEqual("i", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("i", Evaluate("RegExp['$+']"));
            Assert.AreEqual("", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("", Evaluate("RegExp['$`']"));
            Assert.AreEqual("ble teens", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("ble teens", Evaluate("RegExp[\"$'\"]"));

            Evaluate("/notamatch/.test('my little friend')");
            Assert.AreEqual("rr", Evaluate("RegExp.$1"));
            Assert.AreEqual("i", Evaluate("RegExp.$2"));
            Assert.AreEqual("", Evaluate("RegExp.$3"));
            Assert.AreEqual("terrible teens", Evaluate("RegExp.input"));
            Assert.AreEqual("terrible teens", Evaluate("RegExp.$_"));
            Assert.AreEqual("terri", Evaluate("RegExp.lastMatch"));
            Assert.AreEqual("terri", Evaluate("RegExp['$&']"));
            Assert.AreEqual("i", Evaluate("RegExp.lastParen"));
            Assert.AreEqual("i", Evaluate("RegExp['$+']"));
            Assert.AreEqual("", Evaluate("RegExp.leftContext"));
            Assert.AreEqual("", Evaluate("RegExp['$`']"));
            Assert.AreEqual("ble teens", Evaluate("RegExp.rightContext"));
            Assert.AreEqual("ble teens", Evaluate("RegExp[\"$'\"]"));
        }

        //[TestMethod]
        //public void RegExpProperties()
        //{
        //    // exec() that does not match should not change the global RegExp properties.
        //    Evaluate("RegExp.input = 'before'");
        //    Evaluate("/test/.exec('string that does not match')");
        //    Assert.AreEqual("before", Evaluate("RegExp.input"));

        //    // exec() that does match does change the global RegExp properties.
        //    Evaluate("RegExp.input = '1'");
        //    Evaluate("/(ex)(ec|ex)/.exec('for exec testing')");
        //    Assert.AreEqual("for exec testing", Evaluate("RegExp.input"));
        //    Assert.AreEqual("exec", Evaluate("RegExp.lastMatch"));
        //    Assert.AreEqual("ec", Evaluate("RegExp.lastParen"));
        //    Assert.AreEqual("for ", Evaluate("RegExp.leftContext"));
        //    Assert.AreEqual(" testing", Evaluate("RegExp.rightContext"));
        //    Assert.AreEqual("ex", Evaluate("RegExp.$1"));
        //    Assert.AreEqual("ec", Evaluate("RegExp.$2"));
        //    Assert.AreEqual("", Evaluate("RegExp.$3"));

        //    // test() that does not match should not change the global RegExp properties.
        //    Evaluate("RegExp.input = 'before'");
        //    Evaluate("/test/.test('string that does not match')");
        //    Assert.AreEqual("before", Evaluate("RegExp.input"));

        //    // test() that does match does change the global RegExp properties.
        //    Evaluate("RegExp.input = 'before'");
        //    Evaluate("/(te)(st|mp)/.test('for testing')");
        //    Assert.AreEqual("for testing", Evaluate("RegExp.input"));
        //    Assert.AreEqual("test", Evaluate("RegExp.lastMatch"));
        //    Assert.AreEqual("st", Evaluate("RegExp.lastParen"));
        //    Assert.AreEqual("for ", Evaluate("RegExp.leftContext"));
        //    Assert.AreEqual("ing", Evaluate("RegExp.rightContext"));
        //    Assert.AreEqual("te", Evaluate("RegExp.$1"));
        //    Assert.AreEqual("st", Evaluate("RegExp.$2"));
        //    Assert.AreEqual("", Evaluate("RegExp.$3"));
        //}

        [TestMethod]
        public void NonParticipatingGroups()
        {
            Assert.AreEqual(true, Evaluate(@"/(x)?\1y/.test('y')"));
            Assert.AreEqual(@"[""y"",null]", Evaluate(@"JSON.stringify(/(x)?\1y/.exec('y'))"));
            Assert.AreEqual(@"[""y"",null]", Evaluate(@"JSON.stringify(/(x)?y/.exec('y'))"));
            Assert.AreEqual(@"[""y"",null]", Evaluate(@"JSON.stringify('y'.match(/(x)?\1y/))"));
            Assert.AreEqual(@"[""y"",null]", Evaluate(@"JSON.stringify('y'.match(/(x)?y/))"));
            Assert.AreEqual(@"[""y""]", Evaluate(@"JSON.stringify('y'.match(/(x)?\1y/g))"));
            Assert.AreEqual(@"["""",null,""""]", Evaluate(@"JSON.stringify('y'.split(/(x)?\1y/))"));
            Assert.AreEqual(@"["""",null,""""]", Evaluate(@"JSON.stringify('y'.split(/(x)?y/))"));
            Assert.AreEqual(0, Evaluate(@"'y'.search(/(x)?\1y/)"));
            Assert.AreEqual("z", Evaluate(@"'y'.replace(/(x)?\1y/, 'z')"));
            Assert.AreEqual("", Evaluate(@"'y'.replace(/(x)?y/, '$1')"));
            Assert.AreEqual("undefined", Evaluate(@"'y'.replace(/(x)?\1y/, function($0, $1){ return String($1); })"));
            Assert.AreEqual("undefined", Evaluate(@"'y'.replace(/(x)?y/, function($0, $1){ return String($1); })"));
            Assert.AreEqual("undefined", Evaluate(@"'y'.replace(/(x)?y/, function($0, $1){ return $1; })"));
        }
    }
}
