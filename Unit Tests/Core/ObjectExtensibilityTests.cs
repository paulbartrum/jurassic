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
        private class TestClass1 : ObjectInstance
        {
            public TestClass1(ScriptEngine engine)
                : base(engine, engine.Object.InstancePrototype)
            {
                this.SetPropertyValue("exists", 1, false);
            }

            protected override object GetMissingPropertyValue(string propertyName)
            {
                return propertyName;
            }
        }

        private class TestClass2 : ObjectInstance
        {
            public TestClass2(ScriptEngine engine)
                : base(engine, engine.Object.InstancePrototype)
            {
            }

            private class MyFunction : FunctionInstance
            {
                private string name;

                public MyFunction(ScriptEngine engine, string name)
                    : base(engine)
                {
                    this.name = name;
                }

                public override object CallLateBound(object thisObject, params object[] argumentValues)
                {
                    if ((thisObject is TestClass2) == false)
                        throw new JavaScriptException(this.Engine, "TypeError", "Invalid 'this' value.");
                    return ((TestClass2)thisObject).CallFunction(this.name, argumentValues);
                }
            }

            protected override object GetMissingPropertyValue(string propertyName)
            {
                return new MyFunction(this.Engine, propertyName);
            }

            private object CallFunction(string functionName, params object[] argumentValues)
            {
                return functionName + argumentValues.Length;
            }
        }

        [TestMethod]
        public void GetMissingPropertyValue()
        {
            var engine = new ScriptEngine();

            // Create a new TestClass1 instance.
            engine.SetGlobalValue("test", new TestClass1(engine));

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

            // Create a new TestClass2 instance.
            engine.SetGlobalValue("test", new TestClass2(engine));

            // Missing properties should return a function which can be called.
            Assert.AreEqual("unknown0", engine.Evaluate("test.unknown()"));
            Assert.AreEqual("unknown1", engine.Evaluate("test.unknown(1)"));
        }
    }
}
