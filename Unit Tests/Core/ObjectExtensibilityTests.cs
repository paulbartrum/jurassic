using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the ObjectInstance overrides.
    /// </summary>
    [TestClass]
    public class ObjectExtensibilityTests
    {
        private class TestClass : ObjectInstance
        {
            public TestClass(ScriptEngine engine)
                : base(engine, engine.Object.InstancePrototype)
            {
                this.PopulateFunctions();
                this.SetPropertyValue("exists", 1, false);
            }

            protected override object GetMissingPropertyValue(string propertyName)
            {
                return propertyName;
            }
        }

        [TestMethod]
        public void GetMissingPropertyValue()
        {
            var engine = new ScriptEngine();

            // Create a new TestClass instance.
            engine.SetGlobalValue("test", new TestClass(engine));

            // Missing properties should return the name of the property.
            Assert.AreEqual("unknown", engine.Evaluate("test.unknown"));
            Assert.AreEqual("unknown", engine.Evaluate("test['unknown']"));
            Assert.AreEqual("1", engine.Evaluate("test[1]"));

            // Existing properties should still work.
            Assert.AreEqual(1, engine.Evaluate("test.exists"));

            // Missing properties return null for Object.getOwnPropertyDescriptor().
            Assert.AreEqual(Undefined.Value, engine.Evaluate("Object.getOwnPropertyDescriptor(test, 'unknown')"));

            // Missing properties return false from hasOwnProperty().
            Assert.AreEqual(false, engine.Evaluate("test.hasOwnProperty('unknown')"));

            // Missing properties return false from propertyIsEnumerable().
            Assert.AreEqual(false, engine.Evaluate("test.propertyIsEnumerable('unknown')"));
        }
    }
}
