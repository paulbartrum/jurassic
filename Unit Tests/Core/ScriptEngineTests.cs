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
            engine.SetGlobalFunction("glob", new Func<object>(() => engine.Global));
        }

    }
}
