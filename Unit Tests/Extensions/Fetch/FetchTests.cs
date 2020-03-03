using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Extensions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace UnitTests
{
    /// <summary>
    /// Test the Headers object (part of the fetch() API).
    /// </summary>
    [TestClass]
    public class FetchTests : TestBase
    {
        HttpContent response;

        private class MockingHttpMessageHandler : HttpMessageHandler
        {
            private FetchTests tests;
            public MockingHttpMessageHandler(FetchTests tests)
            {
                this.tests = tests;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = this.tests.response;
                response.RequestMessage = request;
                return Task.FromResult(response);
            }
        }

        protected override ScriptEngine InitializeScriptEngine()
        {
            var scriptEngine = base.InitializeScriptEngine();
            scriptEngine.AddFetch(new FetchOptions()
            {
                HttpClientFactory = () => new HttpClient(new MockingHttpMessageHandler(this))
            });
            return scriptEngine;
        }
        
        [TestMethod, Ignore]
        public void Fetch()
        {
            response = new StringContent(@"{ ""a"": ""b"" }", Encoding.UTF8, "application/json");
            Assert.AreEqual(Null.Value, Evaluate(@"var response = '';
                fetch('http://www.example.com')
                .then(function (response) {
                    response = response.text();
                });
                response"));
        }
        
    }
}
