using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the script engine class.
    /// </summary>
    [TestClass]
    public class ScriptEngineTests
    {
        [TestMethod]
        public void SetGlobalFunction()
        {
            var engine = new ScriptEngine();

            // Try a static delegate.
            engine.SetGlobalFunction("add", new Func<int, int, int>((a, b) => a + b));
            Assert.AreEqual(11, engine.Evaluate("add(5, 6)"));

            // Try an instance delegate.
            engine.SetGlobalFunction("global", new Func<object>(() => engine.Global));
            Assert.AreEqual(5, engine.Evaluate("global().parseInt('5')"));
        }

    }
}
