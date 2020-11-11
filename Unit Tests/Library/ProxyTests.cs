using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the Proxy object.
    /// </summary>
    [TestClass]
    public class ProxyTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // new Proxy(target, handler)


            // Both parameters must be objects.
            Assert.AreEqual("TypeError: Cannot create proxy with a non-object as target or handler.", EvaluateExceptionMessage("new Proxy()"));
            Assert.AreEqual("TypeError: Cannot create proxy with a non-object as target or handler.", EvaluateExceptionMessage("new Proxy({})"));
            Assert.AreEqual("TypeError: Cannot create proxy with a non-object as target or handler.", EvaluateExceptionMessage("new Proxy({}, 5)"));

            // Proxy() cannot be called as a function.
            Assert.AreEqual("TypeError: Constructor Proxy requires 'new'", EvaluateExceptionMessage("Proxy()"));

            // name, length and prototype.
            Assert.AreEqual("Proxy", Evaluate("Proxy.name"));
            Assert.AreEqual(2, Evaluate("Proxy.length"));
            Assert.AreEqual(Undefined.Value, Evaluate("Proxy.prototype"));
        }

        [TestMethod]
        public void get()
        {
            Assert.AreEqual("original value, replaced value", Evaluate(@"
                const target = {
                  notProxied: 'original value',
                  proxied: 'original value'
                };

                const handler = {
                  get: function(target, prop, receiver) {
                    if (prop === 'proxied') {
                      return 'replaced value';
                    }
                    return Reflect.get(...arguments);
                  }
                };

                const proxy = new Proxy(target, handler);

                proxy.notProxied + ', ' + proxy.proxied);"));
        }
    }
}
