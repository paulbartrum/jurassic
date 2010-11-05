using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test .NET serialization.
    /// </summary>
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void SerializeEngine()
        {
            // Set up a script engine.
            var scriptEngine = new ScriptEngine();
            scriptEngine.SetGlobalValue("test", "one");

            // Attempt to serialize and then deserialize the entire script engine.
            var scriptEngine2 = (ScriptEngine)Clone(scriptEngine);

            // Verify it was deserialized correctly.
            Assert.AreEqual("one", scriptEngine2.GetGlobalValue<string>("test"));
            Assert.AreEqual(scriptEngine2, scriptEngine2.Global.Engine);
        }

        [TestMethod]
        public void SerializeObject()
        {
            var scriptEngine = new ScriptEngine();
            ScriptEngine.DeserializationEnvironment = scriptEngine;

            // Create a test object.
            scriptEngine.Execute(@"
                _obj = { };
                Object.defineProperty(_obj, 'a', { configurable: true, writable: true, enumerable: true, value: 5 });
                Object.defineProperty(_obj, 'b', { configurable: false, writable: false, enumerable: false, value: 10 });
                Object.defineProperty(_obj, 'c', { configurable: true, enumerable: true, get: function() { return 3; } });
                Object.defineProperty(_obj, 'd', { configurable: true, enumerable: true, get: function() { return this._value; }, set: function(value) { this._value = value + 1 } });
                _obj.nested = { a: 1 };");

            // Clone the object using serialization.
            scriptEngine.SetGlobalValue("_obj2", Clone(scriptEngine.GetGlobalValue("_obj")));
            
            // Check the cloned object is not simply a pointer to the old object.
            Assert.AreEqual(true, scriptEngine.Evaluate("delete _obj.e; _obj2.e = 11; _obj.e === undefined"));

            // Check the properties have been cloned successfully.
            Assert.AreEqual(5, scriptEngine.Evaluate("_obj2.a"));
            scriptEngine.Execute("var descriptor = Object.getOwnPropertyDescriptor(_obj2, 'a')");
            Assert.AreEqual(true, scriptEngine.Evaluate("descriptor.configurable"));
            Assert.AreEqual(true, scriptEngine.Evaluate("descriptor.writable"));
            Assert.AreEqual(true, scriptEngine.Evaluate("descriptor.enumerable"));
            Assert.AreEqual(5, scriptEngine.Evaluate("descriptor.value"));

            // Check the properties have been cloned successfully.
            Assert.AreEqual(10, scriptEngine.Evaluate("_obj2.b"));
            scriptEngine.Execute("var descriptor = Object.getOwnPropertyDescriptor(_obj2, 'b')");
            Assert.AreEqual(false, scriptEngine.Evaluate("descriptor.configurable"));
            Assert.AreEqual(false, scriptEngine.Evaluate("descriptor.writable"));
            Assert.AreEqual(false, scriptEngine.Evaluate("descriptor.enumerable"));
            Assert.AreEqual(10, scriptEngine.Evaluate("descriptor.value"));

            // Check the properties have been cloned successfully.
            Assert.AreEqual(3, scriptEngine.Evaluate("_obj2.c"));
            scriptEngine.Execute("var descriptor = Object.getOwnPropertyDescriptor(_obj2, 'c')");
            Assert.AreEqual(true, scriptEngine.Evaluate("descriptor.configurable"));
            Assert.AreEqual(true, scriptEngine.Evaluate("descriptor.enumerable"));
            Assert.AreEqual("function", scriptEngine.Evaluate("typeof descriptor.get"));

            // Check the properties have been cloned successfully.
            Assert.AreEqual(11, scriptEngine.Evaluate("_obj2.d = 10; _obj2.d"));
            scriptEngine.Execute("var descriptor = Object.getOwnPropertyDescriptor(_obj2, 'd')");
            Assert.AreEqual(true, scriptEngine.Evaluate("descriptor.configurable"));
            Assert.AreEqual(true, scriptEngine.Evaluate("descriptor.enumerable"));
            Assert.AreEqual("function", scriptEngine.Evaluate("typeof descriptor.get"));
            Assert.AreEqual("function", scriptEngine.Evaluate("typeof descriptor.set"));

            // Check the properties have been cloned successfully.
            Assert.AreEqual(1, scriptEngine.Evaluate("_obj2.nested.a"));

            // Make sure the extensible flag works.
            scriptEngine.Execute(@"
                _obj3 = { };
                Object.preventExtensions(_obj3);");

            // Clone the object using serialization.
            scriptEngine.SetGlobalValue("_obj4", Clone(scriptEngine.GetGlobalValue("_obj3")));

            // Check the flag was cloned successfully.
            Assert.AreEqual(true, scriptEngine.Evaluate("Object.isExtensible(_obj)"));
            Assert.AreEqual(true, scriptEngine.Evaluate("Object.isExtensible(_obj2)"));
            Assert.AreEqual(false, scriptEngine.Evaluate("Object.isExtensible(_obj3)"));
            Assert.AreEqual(false, scriptEngine.Evaluate("Object.isExtensible(_obj4)"));
        }

        [TestMethod]
        public void SerializeFunction()
        {
            // Set up a script engine.
            var scriptEngine = new ScriptEngine();
            scriptEngine.Execute(@"function outer(a, b) { function inner() { return a + b; } return inner(); }");

            // Attempt to serialize and then deserialize the function.
            ScriptEngine.DeserializationEnvironment = scriptEngine;
            var function = (FunctionInstance)Clone(scriptEngine.GetGlobalValue("outer"));

            // Verify it was deserialized correctly.
            Assert.AreEqual(11.0, function.Call(null, 5, 6));
        }





        // Clone an object using serialization.
        private static object Clone(object objectToSerialize)
        {
            var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var stream = new System.IO.MemoryStream();
            serializer.Serialize(stream, objectToSerialize);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return serializer.Deserialize(stream);
        }
    }
}
