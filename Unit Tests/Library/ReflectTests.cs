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
    public class ReflectTests
    {
        [TestMethod]
        public void Constructor()
        {
            // Reflect is not a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Reflect"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("Reflect()"));
        }

        // TODO: Reflect.apply, Reflect.construct, etc.
    }
}
