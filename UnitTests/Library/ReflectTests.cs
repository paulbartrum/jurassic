using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global Reflect object.
    /// </summary>
    [TestClass]
    public class ReflectTests : TestBase
    {
        [TestMethod]
        [Ignore]    // not supported yet.
        public void Constructor()
        {
            // Reflect is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Reflect"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Reflect()"));
        }

        // TODO: Reflect.apply, Reflect.construct, etc.
    }
}
