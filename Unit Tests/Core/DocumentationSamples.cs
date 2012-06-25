using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Each unit test represents a sample in the documentation.  Do not modify the tests without
    /// also updating the documentation.
    /// </summary>
    [TestClass]
    public class DocumentationSamples
    {
        [TestMethod]
        public void EvaluateExpression1()
        {
            var engine = new Jurassic.ScriptEngine();
            Assert.AreEqual(52, engine.Evaluate("5 * 10 + 2"));
        }

        [TestMethod]
        public void EvaluateExpression2()
        {
            var engine = new Jurassic.ScriptEngine();
            Assert.AreEqual(3, engine.Evaluate<int>("1.5 + 2.4"));
        }

        [TestMethod]
        public void ExecutingScript1()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Core\Sample Files\execute1.js");
        }

        [TestMethod]
        public void AccessingAndModifyingGlobalVariables1()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalValue("interop", 15);
            engine.ExecuteFile(@"..\..\..\Unit Tests\Core\Sample Files\globals1.js");
            Assert.AreEqual(20, engine.GetGlobalValue<int>("interop"));
        }

        [TestMethod]
        public void ConsoleAPI1()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(engine));
        }

        [TestMethod]
        public void CallANETMethod1()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalFunction("test", new Func<int, int, int>((a, b) => a + b));
            Assert.AreEqual(11, engine.Evaluate<int>("test(5, 6)"));
        }

        [TestMethod]
        public void CallJavaScriptFunction1()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.Evaluate("function test(a, b) { return a + b }");
            Assert.AreEqual(11, engine.CallGlobalFunction<int>("test", 5, 6));
        }

        public class AppInfo : ObjectInstance
        {
            public AppInfo(ScriptEngine engine)
                : base(engine)
            {
                this.DefineProperty("name", new PropertyDescriptor("Test Application", PropertyAttributes.Sealed), true);
                this.DefineProperty("version", new PropertyDescriptor(5, PropertyAttributes.Sealed), true);
            }
        }

        [TestMethod]
        public void ExposingANETClass1()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalValue("appInfo", new AppInfo(engine));
            Assert.AreEqual("Test Application 5", engine.Evaluate<string>("appInfo.name + ' ' + appInfo.version"));
        }

        public class Math2 : ObjectInstance
        {
            public Math2(ScriptEngine engine)
                : base(engine)
            {
                this.PopulateFunctions();
            }

            [JSFunction(Name = "log10")]
            public static double Log10(double num)
            {
                return Math.Log10(num);
            }
        }

        [TestMethod]
        public void ExposingANETClass2()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalValue("math2", new Math2(engine));
            Assert.AreEqual(3.0, engine.Evaluate<double>("math2.log10(1000)"));
        }

        public class RandomConstructor : ClrFunction
        {
            public RandomConstructor(ScriptEngine engine)
                : base(engine.Function.InstancePrototype, "Random", new RandomInstance(engine.Object.InstancePrototype))
            {
            }

            [JSConstructorFunction]
            public RandomInstance Construct(int seed)
            {
                return new RandomInstance(this.InstancePrototype, seed);
            }
        }

        public class RandomInstance : ObjectInstance
        {
            private Random random;

            public RandomInstance(ObjectInstance prototype)
                : base(prototype)
            {
                this.PopulateFunctions();
                this.random = new Random(0);
            }

            public RandomInstance(ObjectInstance prototype, int seed)
                : base(prototype)
            {
                this.random = new Random(seed);
            }

            [JSFunction(Name = "nextDouble")]
            public double NextDouble()
            {
                return this.random.NextDouble();
            }
        }

        [TestMethod]
        public void ExposingANETClass3()
        {
            var engine = new Jurassic.ScriptEngine();
            engine.SetGlobalValue("Random", new RandomConstructor(engine));
            Assert.AreEqual(0.15155745910087481, engine.Evaluate<double>("var rand = new Random(1000); rand.nextDouble()"));
        }
    }
}
