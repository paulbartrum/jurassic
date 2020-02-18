using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Extensions;

namespace UnitTests
{
    /// <summary>
    /// Test the Request object (part of the fetch() API).
    /// </summary>
    [TestClass]
    public class RequestTests : TestBase
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
            // Request(url)
            Assert.AreEqual("http://www.example.com/", Evaluate("new Request('http://www.example.com').url"));

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
        public void Body()
        {
        }

        [TestMethod]
        public void BodyUsed()
        {
        }

        [TestMethod]
        public void Cache()
        {
            Assert.AreEqual("default", Evaluate("new Request('http://www.example.com').cache"));
        }

        [TestMethod]
        public void Credentials()
        {
            Assert.AreEqual("same-origin", Evaluate("new Request('http://www.example.com').credentials"));
        }

        [TestMethod]
        public void Destination()
        {
            Assert.AreEqual("", Evaluate("new Request('http://www.example.com').destination"));
        }

        [TestMethod]
        public void Headers()
        {
            Assert.AreEqual(@"[[""test"",""abc""]]", Evaluate("var req = new Request('http://www.example.com', { headers: { 'test': 'abc' } }); JSON.stringify(Array.from(req.headers))"));
            Assert.AreEqual(@"[[""test"",""abc""]]", Evaluate("var req2 = new Request(req); JSON.stringify(Array.from(req2.headers))"));
            Assert.AreEqual(@"[[""test"",""abc2""]]", Evaluate("var req2 = new Request(req, { headers: { 'test': 'abc2' } }); JSON.stringify(Array.from(req2.headers))"));
            Assert.AreEqual(@"[[""test2"",""abc2""]]", Evaluate("var req2 = new Request(req, { headers: { 'test2': 'abc2' } }); JSON.stringify(Array.from(req2.headers))"));
        }

        [TestMethod]
        public void Integrity()
        {
            Assert.AreEqual("", Evaluate("new Request('http://www.example.com').integrity"));
        }

        [TestMethod]
        public void Method()
        {
            Assert.AreEqual("GET", Evaluate("new Request('http://www.example.com').method"));
            Assert.AreEqual("POST", Evaluate("new Request('http://www.example.com', { method: 'post' }).method"));

            // Method is read-only.
            Assert.AreEqual("GET", Evaluate("var req = new Request('http://www.example.com'); req.method = 'POST'; req.method"));
        }

        [TestMethod]
        public void Mode()
        {
            Assert.AreEqual("cors", Evaluate("new Request('http://www.example.com').mode"));
        }

        [TestMethod]
        public void Redirect()
        {
            Assert.AreEqual("follow", Evaluate("new Request('http://www.example.com').redirect"));
        }

        [TestMethod]
        public void Referrer()
        {
            Assert.AreEqual("about:client", Evaluate("new Request('http://www.example.com').referrer"));
            Assert.AreEqual("", Evaluate("new Request('http://www.example.com', { referrer: '' }).referrer"));
        }

        [TestMethod]
        public void ReferrerPolicy()
        {
            Assert.AreEqual("", Evaluate("new Request('http://www.example.com').referrerPolicy"));
        }

        [TestMethod]
        public void Url()
        {
            Assert.AreEqual("http://www.example.com/", Evaluate("new Request('http://www.example.com').url"));
        }
    }
}
