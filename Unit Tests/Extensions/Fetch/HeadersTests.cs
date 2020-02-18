using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Extensions;

namespace UnitTests
{
    /// <summary>
    /// Test the Headers object (part of the fetch() API).
    /// </summary>
    [TestClass]
    public class HeadersTests : TestBase
    {
        protected override ScriptEngine InitializeScriptEngine()
        {
            var scriptEngine = base.InitializeScriptEngine();
            scriptEngine.AddFetch();
            return scriptEngine;
        }
        
        [TestMethod]
        public void Constructor()
        {
            // Empty Headers instance.
            Assert.AreEqual(Null.Value, Evaluate("new Headers().get('Content-Type')"));

            // Set headers using an array e.g. [ [ "Content-Type", "text/xml" ], [ "User-Agent", "Test" ] ]
            Assert.AreEqual("text/xml", Evaluate("new Headers([['Content-Type', 'text/xml']]).get('Content-Type')"));
            Assert.AreEqual("text/xml, text/json", Evaluate("new Headers([['Content-Type', 'text/xml'], ['Content-Type', 'text/json']]).get('Content-Type')"));

            // Set headers using property keys and values e.g. { "Content-Type": "text/xml", "User-Agent": "Test" }
            Assert.AreEqual("text/xml", Evaluate("new Headers({'Content-Type': 'text/xml'}).get('Content-Type')"));
            Assert.AreEqual("content-type,text/xml", Evaluate("Array.from(new Headers({'Content-Type': 'text/xml'})).toString()"));

            // Only properties that are enumerable are considered.
            Assert.AreEqual("text/plain", Evaluate(@"
                var x = {}
                Object.defineProperty(x, 'Content-Type', { value: 'text/plain', enumerable: true })
                new Headers(x).get('Content-Type')"));
            Assert.AreEqual(Null.Value, Evaluate(@"
                var x = {}
                Object.defineProperty(x, 'Content-Type', { value: 'text/plain', enumerable: false })
                new Headers(x).get('Content-Type')"));

            // Prototype properties are ignored.
            Assert.AreEqual(Null.Value, Evaluate(@"
                var proto = { 'Content-Type': 'text/xml' };
                new Headers(Object.create(proto)).get('Content-Type')"));
        }

        [TestMethod]
        public void Append()
        {
            Assert.AreEqual("text/xml", Evaluate(@"
                var headers = new Headers();
                headers.append('Content-Type', 'text/xml');
                headers.get('Content-Type')"));
            Assert.AreEqual("text/xml, text/plain", Evaluate(@"
                var headers = new Headers();
                headers.append('Content-Type', 'text/xml');
                headers.append('Content-Type', 'text/plain');
                headers.get('Content-Type')"));
        }

        [TestMethod]
        public void Delete()
        {
            Assert.AreEqual(Null.Value, Evaluate(@"
                var headers = new Headers({'Content-Type': 'text/xml'});
                headers.delete('Content-Type');
                headers.delete('abc');
                headers.get('Content-Type')"));
        }

        [TestMethod]
        public void Entries()
        {
            Assert.AreEqual(@"[[""content-type"",""text/xml""],[""user-agent"",""MyBrowser""]]",
                Evaluate("JSON.stringify(Array.from(new Headers({'Content-Type': 'text/xml', 'User-Agent': 'MyBrowser'}).entries()))"));
        }

        [TestMethod]
        public void Get()
        {
            // Retrieve a set value.
            Assert.AreEqual("text/xml", Evaluate("new Headers({'Content-Type': 'text/xml'}).get('Content-Type')"));

            // Case-insensitive.
            Assert.AreEqual("text/xml", Evaluate("new Headers({'Content-Type': 'text/xml'}).get('CONTENT-type')"));

            // Non-existent header returns null.
            Assert.AreEqual(Null.Value, Evaluate("new Headers({'Content-Type': 'text/xml'}).get('abc')"));
        }


        [TestMethod]
        public void Has()
        {
            Assert.AreEqual(false, Evaluate("new Headers({'Content-Type': 'text/xml'}).has('abc')"));
            Assert.AreEqual(true, Evaluate("new Headers({'Content-Type': 'text/xml'}).has('Content-Type')"));
            Assert.AreEqual(true, Evaluate("new Headers({'Content-Type': 'text/xml'}).has('CONTENT-type')"));
        }


        [TestMethod]
        public void Keys()
        {
            Assert.AreEqual("content-type,user-agent", Evaluate("Array.from(new Headers({'Content-Type': 'text/xml', 'User-Agent': 'MyBrowser'}).keys()).toString()"));
        }


        [TestMethod]
        public void Set()
        {
            Assert.AreEqual("text/xml", Evaluate(@"
                var headers = new Headers();
                headers.set('Content-Type', 'text/xml');
                headers.get('Content-Type')"));
            Assert.AreEqual("text/plain", Evaluate(@"
                var headers = new Headers();
                headers.set('Content-Type', 'text/xml');
                headers.set('Content-Type', 'text/plain');
                headers.get('Content-Type')"));
        }


        [TestMethod]
        public void Values()
        {
            Assert.AreEqual("text/xml,MyBrowser", Evaluate("Array.from(new Headers({'Content-Type': 'text/xml', 'User-Agent': 'MyBrowser'}).values()).toString()"));
        }
    }
}
