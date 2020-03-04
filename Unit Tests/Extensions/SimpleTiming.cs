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
    /// Test the setTimeout, setInterval, clearTimeout and clearInterval functions.
    /// </summary>
    [TestClass]
    public class SimpleTimingTests : TestBase
    {
        protected override ScriptEngine InitializeScriptEngine()
        {
            var scriptEngine = base.InitializeScriptEngine();
            scriptEngine.AddSimpleTiming();
            return scriptEngine;
        }
        
        [TestMethod]
        public void SetTimeout_Code()
        {
            // setTimeout does not execute the callback, even if the delay is zero.
            ScriptEngine.SetGlobalValue("a", 10);
            Execute("setTimeout('a = 20', 0)");
            Assert.AreEqual(10, ScriptEngine.GetGlobalValue<int>("a"));

            // You have to pump the event loop.
            ScriptEngine.ExecuteNextEventQueueAction();
            Assert.AreEqual(20, ScriptEngine.GetGlobalValue<int>("a"));
        }

        [TestMethod]
        public async Task SetTimeout_Function()
        {
            // Note that a delay time of 0ms is guaranteed to synchronously place an action on the
            // event queue.

            Execute("a = 10; setTimeout(function (val) { a = 20 })");
            ScriptEngine.ExecuteNextEventQueueAction();
            Assert.AreEqual(20, ScriptEngine.GetGlobalValue<int>("a"));

            Execute("a = 10; setTimeout(function (val) { a = 20 }, 0)");
            ScriptEngine.ExecuteNextEventQueueAction();
            Assert.AreEqual(20, ScriptEngine.GetGlobalValue<int>("a"));

            Execute("a = 10; setTimeout(function (val) { a = val }, 0, 20)");
            ScriptEngine.ExecuteNextEventQueueAction();
            Assert.AreEqual(20, ScriptEngine.GetGlobalValue<int>("a"));

            // Test a 20ms timeout.
            Execute("a = 10; setTimeout(function (val) { a = val }, 20, 15)");
            
            // Wait 5ms. The timeout should not have been queued yet.
            await Task.Delay(5);
            ScriptEngine.ExecuteNextEventQueueAction();
            Assert.AreEqual(10, ScriptEngine.GetGlobalValue<int>("a"));

            // Wait 40ms. The timeout should have been queued now.
            await Task.Delay(40);
            ScriptEngine.ExecuteNextEventQueueAction();
            Assert.AreEqual(15, ScriptEngine.GetGlobalValue<int>("a"));
        }
    }
}
