using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Extensions;

namespace UnitTests
{
    /// <summary>
    /// Test the Reponse object (part of the fetch() API).
    /// </summary>
    [TestClass]
    public class ResponseTests : TestBase
    {
        protected override ScriptEngine InitializeScriptEngine()
        {
            var scriptEngine = base.InitializeScriptEngine();
            scriptEngine.AddFetch();
            return scriptEngine;
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
    }
}
