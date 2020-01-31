using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Extensions;

namespace UnitTests
{
    /// <summary>
    /// Test the fetch() API.
    /// </summary>
    [TestClass]
    public class FetchTests : TestBase
    {
        protected override ScriptEngine InitializeScriptEngine()
        {
            var scriptEngine = base.InitializeScriptEngine();
            scriptEngine.AddFetch();
            return scriptEngine;
        }

        [TestMethod]
        public void Request_constructor()
        {
            // Invalid URL.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Request('http:::')"));

            // Credentials are not allowed in the URL.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Request('http://user:password@example.com')"));

            // Invalid modes.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Request('http://example.com', { mode: 'sdfsdf' })"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Request('http://example.com', { mode: 'CORS' })"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Request('http://example.com', { mode: 'navigate' })"));
        }

        [TestMethod]
        public void Response_error()
        {
            Assert.AreEqual("error", Evaluate("Response.error().type"));
            Assert.AreEqual("", Evaluate("Response.error().url"));
            Assert.AreEqual(false, Evaluate("Response.error().redirected"));
            Assert.AreEqual(0, Evaluate("Response.error().status"));
            Assert.AreEqual(false, Evaluate("Response.error().ok"));
            Assert.AreEqual("", Evaluate("Response.error().statusText"));
            Assert.AreEqual("", Evaluate("Array.from(Response.error().headers).toString()"));
        }

        [TestMethod]
        public void Response_redirect()
        {
            // redirect(url)
            Assert.AreEqual("default", Evaluate("Response.redirect('http://about.com').type"));
            Assert.AreEqual("", Evaluate("Response.redirect('http://about.com').url"));
            Assert.AreEqual(false, Evaluate("Response.redirect('http://about.com').redirected"));
            Assert.AreEqual(302, Evaluate("Response.redirect('http://about.com').status"));
            Assert.AreEqual(false, Evaluate("Response.redirect('http://about.com').ok"));
            Assert.AreEqual("", Evaluate("Response.redirect('http://about.com').statusText"));
            Assert.AreEqual("location,http://about.com/", Evaluate("Array.from(Response.redirect('http://about.com').headers).toString()"));

            // redirect(url, status)
            Assert.AreEqual(301, Evaluate("Response.redirect('http://about.com', 301).status"));

            // Invalid URL.
            Assert.AreEqual("TypeError", EvaluateExceptionType("Response.redirect('http:::')"));

            // Invalid status code.
            Assert.AreEqual("RangeError", EvaluateExceptionType("Response.redirect('http://about.com', 500)"));
        }

        [TestMethod]
        public void Headers_constructor()
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
        public void Headers_append()
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
        public void Headers_delete()
        {
            Assert.AreEqual(Null.Value, Evaluate(@"
                var headers = new Headers({'Content-Type': 'text/xml'});
                headers.delete('Content-Type');
                headers.delete('abc');
                headers.get('Content-Type')"));
        }

        [TestMethod]
        public void Headers_entries()
        {
            Assert.AreEqual(@"[[""content-type"",""text/xml""],[""user-agent"",""MyBrowser""]]",
                Evaluate("JSON.stringify(Array.from(new Headers({'Content-Type': 'text/xml', 'User-Agent': 'MyBrowser'}).entries()))"));
        }

        [TestMethod]
        public void Headers_get()
        {
            // Retrieve a set value.
            Assert.AreEqual("text/xml", Evaluate("new Headers({'Content-Type': 'text/xml'}).get('Content-Type')"));

            // Case-insensitive.
            Assert.AreEqual("text/xml", Evaluate("new Headers({'Content-Type': 'text/xml'}).get('CONTENT-type')"));

            // Non-existent header returns null.
            Assert.AreEqual(Null.Value, Evaluate("new Headers({'Content-Type': 'text/xml'}).get('abc')"));
        }


        [TestMethod]
        public void Headers_has()
        {
            Assert.AreEqual(false, Evaluate("new Headers({'Content-Type': 'text/xml'}).has('abc')"));
            Assert.AreEqual(true, Evaluate("new Headers({'Content-Type': 'text/xml'}).has('Content-Type')"));
            Assert.AreEqual(true, Evaluate("new Headers({'Content-Type': 'text/xml'}).has('CONTENT-type')"));
        }


        [TestMethod]
        public void Headers_keys()
        {
            Assert.AreEqual("content-type,user-agent", Evaluate("Array.from(new Headers({'Content-Type': 'text/xml', 'User-Agent': 'MyBrowser'}).keys()).toString()"));
        }


        [TestMethod]
        public void Headers_set()
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
        public void Headers_values()
        {
            Assert.AreEqual("text/xml,MyBrowser", Evaluate("Array.from(new Headers({'Content-Type': 'text/xml', 'User-Agent': 'MyBrowser'}).values()).toString()"));
        }
    }
}
