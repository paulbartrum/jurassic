using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the [JsProperty] attribute.
    /// </summary>
    [TestClass]
    public class JsPropertyTests
    {
        public class ClassWithProperty : ObjectInstance
        {
            public ClassWithProperty(ScriptEngine engine)
                : base(engine)
            {
                this.PopulateFunctions();
                this.TestStr = "Initial";
                this.TestInt = 9;
            }

            [JSProperty]
            public string TestStr { get; set; }

            [JSProperty(IsEnumerable = false)]
            public int TestInt { get; set; }

            [JSProperty(IsConfigurable = true)]
            public object TestObj { get; set; }
        }

        [TestMethod]
        public void StringProperty()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalValue("test", new ClassWithProperty(engine));

            Assert.AreEqual("Initial", engine.Evaluate<string>("test.TestStr"));
            Assert.AreEqual("test", engine.Evaluate<string>("test.TestStr = 'test'; test.TestStr"));
            Assert.AreEqual("null", engine.Evaluate<string>("test.TestStr = null; test.TestStr"));
            Assert.AreEqual("5", engine.Evaluate<string>("test.TestStr = 5; test.TestStr"));
        }

        [TestMethod]
        public void IntProperty()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalValue("test", new ClassWithProperty(engine));

            Assert.AreEqual(9, engine.Evaluate<int>("test.TestInt"));
            Assert.AreEqual(5, engine.Evaluate<int>("test.TestInt = 5; test.TestInt"));
            Assert.AreEqual(0, engine.Evaluate<int>("test.TestInt = 'test'; test.TestInt"));
        }

        [TestMethod]
        public void ObjectProperty()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalValue("test", new ClassWithProperty(engine));

            Assert.AreEqual(5, engine.Evaluate<object>("test.TestObj = 5; test.TestObj"));
            Assert.AreEqual("test", engine.Evaluate<object>("test.TestObj = 'test'; test.TestObj"));
            Assert.AreEqual(Undefined.Value, engine.Evaluate<object>("test.TestObj = undefined; test.TestObj"));
            Assert.AreEqual(Null.Value, engine.Evaluate<object>("test.TestObj = null; test.TestObj"));
        }

        [TestMethod]
        public void PropertyAttributes()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalValue("test", new ClassWithProperty(engine));

            // IsEnumerable.
            Assert.AreEqual("TestStr,TestObj", engine.Evaluate<string>("array = []; for (var key in test) array.push(key); array.join(',')"));

            // IsConfigurable
            Assert.AreEqual(false, engine.Evaluate<bool>("delete test.TestStr"));
            Assert.AreEqual(true, engine.Evaluate<bool>("delete test.TestObj"));
        }
    }
}
