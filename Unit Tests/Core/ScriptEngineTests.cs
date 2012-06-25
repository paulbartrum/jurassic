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
        private class CustomObjectInstance : ObjectInstance
        {
            public CustomObjectInstance(ScriptEngine engine)
                : base(engine)
            {
                this.PopulateFunctions();
            }

            protected CustomObjectInstance(ObjectInstance prototype)
                : base(prototype)
            {
            }

            [JSFunction(Name = "a")]
            public virtual int Test1()
            {
                return 5;
            }
        }

        private class InheritedObjectInstance1 : CustomObjectInstance
        {
            public InheritedObjectInstance1(ScriptEngine engine)
                : base(new CustomObjectInstance(engine))
            {
                this.PopulateFunctions();
            }

            [JSFunction(Name = "b")]
            public int Test2()
            {
                return 6;
            }
        }

        private class InheritedObjectInstance2 : CustomObjectInstance
        {
            public InheritedObjectInstance2(ScriptEngine engine)
                : base(engine)
            {
                this.PopulateFunctions();
            }

            [JSFunction(Name = "a")]
            public override int Test1()
            {
                return 7;
            }
        }

        [TestMethod]
        public void SetGlobalValue()
        {
            var engine = new ScriptEngine();

            // Try setting a value using the standard types.
            engine.SetGlobalValue("test", true);
            Assert.AreEqual(true, engine.Evaluate("test === true"));
            engine.SetGlobalValue("test", 5.1);
            Assert.AreEqual(true, engine.Evaluate("test === 5.1"));
            engine.SetGlobalValue("test", uint.MaxValue);
            Assert.AreEqual(true, engine.Evaluate("test === 4294967295"));
            engine.SetGlobalValue("test", 1);
            Assert.AreEqual(true, engine.Evaluate("test === 1.0"));
            engine.SetGlobalValue("test", "test");
            Assert.AreEqual(true, engine.Evaluate("test === 'test'"));
            engine.SetGlobalValue("test", engine.String.Construct("test"));
            Assert.AreEqual(true, engine.Evaluate("test == 'test'"));
            engine.SetGlobalValue("test", new CustomObjectInstance(engine));
            Assert.AreEqual(true, engine.Evaluate("test.a() === 5"));
            Assert.AreEqual(true, engine.Evaluate("test.Test1 === undefined"));
            engine.SetGlobalValue("test", new InheritedObjectInstance1(engine));
            Assert.AreEqual(true, engine.Evaluate("test.a() === 5"));
            Assert.AreEqual(true, engine.Evaluate("test.b() === 6"));
            engine.SetGlobalValue("test", new InheritedObjectInstance2(engine));
            Assert.AreEqual(true, engine.Evaluate("test.a() === 7"));
        }

        private class TestClass
        {
            public static void Test1() { return; }
            public static bool Test2(bool p) { return p; }
            public static byte Test3(byte p) { return p == 255 ? (byte)250 : (byte)0; }
            public static sbyte Test4(sbyte p) { return p == -100 ? (sbyte)-123 : (sbyte)0; }
            public static short Test5(short p) { return p == -1001 ? (short) -1234 : (short)0; }
            public static ushort Test6(ushort p) { return p == 61234 ? (ushort)60123 : (ushort)0; }
            public static int Test7(int p) { return p == -123456789 ? -1234567890 : 0; }
            public static uint Test8(uint p) { return p == 123456789 ? 1234567890u : 0; }
            public static long Test9(long p) { return p == -123456789012345 ? -12345678901234 : 0; }
            public static ulong Test10(ulong p) { return p == 123456789012345 ? 12345678901234ul : 0; }
            public static float Test11(float p) { return p == 3.1415926535897932384626433832795f ? 2.7182818284590452353602874713527f : 0; }
            public static double Test12(double p) { return p == 3.1415926535897932384626433832795 ? 2.7182818284590452353602874713527 : 0; }
            public static decimal Test13(decimal p) { return p == 1.25m ? 3.75m : 0; }
            public static char Test14(char p) { return p == 'a' ? 'b' : 'c'; }
            public static string Test15(string p) { return p == "test" ? "correct" : "wrong"; }
            public static object Test16() { return null; }
            public static object Test17(object p) { return p == null ? "correct" : null; }
            public static object Test18(object p) { return (string)p == "test" ? "correct" : null; }
            public static string Test19(string p) { return p == null ? "correct" : "wrong"; }
            public static object Test20(object p) { return (double)p == 5.1 ? 6.1 : 0.0; }

            public static int Optional1(int a, int b = 2, int c = 3) { return a + b + c; }
            public static int Optional2([System.Runtime.InteropServices.Optional] int a) { return a; }

            public static bool Params1(params int[] p) { return p.Length == 2 && p[0] == 1 && p[1] == 2; }

            public static bool Overload1(int a, int b) { return true; }
            public static bool Overload1(double a, double b) { return false; }
            public static string Overload1(string a, string b) { return a + b; }
        }

        [TestMethod]
        public void SetGlobalValueType()
        {
            var engine = new ScriptEngine();

            TestUtils.ExpectException<JavaScriptException>(() => engine.SetGlobalValue("Math", typeof(Math)));

            engine.EnableExposedClrTypes = true;

            // Try setting a few Types.
            engine.SetGlobalValue("Math", typeof(Math));
            engine.SetGlobalValue("TestClass", typeof(TestClass));

            // Basic tests.
            Assert.AreEqual(-0.9589242746631384, engine.Evaluate("Math.Sin(5)"));
            Assert.AreEqual(Undefined.Value, engine.Evaluate("TestClass.Test1()"));
            Assert.AreEqual(false, engine.Evaluate("TestClass.Test2(false)"));
            Assert.AreEqual(true, engine.Evaluate("TestClass.Test2(true)"));
            Assert.AreEqual(250, engine.Evaluate("TestClass.Test3(255)"));
            Assert.AreEqual(-123, engine.Evaluate("TestClass.Test4(-100)"));
            Assert.AreEqual(-1234, engine.Evaluate("TestClass.Test5(-1001)"));
            Assert.AreEqual(60123, engine.Evaluate("TestClass.Test6(61234)"));
            Assert.AreEqual(-1234567890, engine.Evaluate("TestClass.Test7(-123456789)"));
            Assert.AreEqual(-1234567890, engine.Evaluate("TestClass.Test7(-123456789.9)"));
            Assert.AreEqual(0, engine.Evaluate("TestClass.Test7(-123456788.9)"));
            Assert.AreEqual(1234567890, engine.Evaluate("TestClass.Test8(123456789)"));
            Assert.AreEqual(1234567890, engine.Evaluate("TestClass.Test8(123456789.9)"));
            Assert.AreEqual(0, engine.Evaluate("TestClass.Test8(123456788.9)"));
            Assert.AreEqual(-12345678901234d, engine.Evaluate("TestClass.Test9(-123456789012345)"));
            Assert.AreEqual(12345678901234d, engine.Evaluate("TestClass.Test10(123456789012345)"));
            Assert.AreEqual(2.7182817459106445, engine.Evaluate("TestClass.Test11(3.14159265358979323)"));
            Assert.AreEqual(2.71828182845904523, engine.Evaluate("TestClass.Test12(3.14159265358979323)"));
            Assert.AreEqual(3.75, engine.Evaluate("TestClass.Test13(1.25)"));
            Assert.AreEqual("b", engine.Evaluate("TestClass.Test14('a')"));
            Assert.AreEqual("correct", engine.Evaluate("TestClass.Test15('test')"));
            Assert.AreEqual(Null.Value, engine.Evaluate("TestClass.Test16()"));
            Assert.AreEqual("correct", engine.Evaluate("TestClass.Test17(null)"));
            Assert.AreEqual("correct", engine.Evaluate("TestClass.Test18('test')"));
            Assert.AreEqual("correct", engine.Evaluate("TestClass.Test19(null)"));
            Assert.AreEqual(6.1, engine.Evaluate("TestClass.Test20(5.1)"));

            // Optional parameters.
            Assert.AreEqual(6, engine.Evaluate("TestClass.Optional1(1)"));
            Assert.AreEqual(9, engine.Evaluate("TestClass.Optional1(1, 5)"));
            Assert.AreEqual(12, engine.Evaluate("TestClass.Optional1(1, 5, 6)"));
            Assert.AreEqual(0, engine.Evaluate("TestClass.Optional2()"));

            // ParamArray.
            Assert.AreEqual(false, engine.Evaluate("TestClass.Params1()"));
            Assert.AreEqual(false, engine.Evaluate("TestClass.Params1(1)"));
            Assert.AreEqual(true, engine.Evaluate("TestClass.Params1(1, 2)"));

            // Overload test.
            Assert.AreEqual(4, engine.Evaluate("Math.Max(3, 4)"));
            Assert.AreEqual(3, engine.Evaluate("Math.Abs(-3)"));
            Assert.AreEqual(false, engine.Evaluate("TestClass.Overload1(1, 2)"));
            Assert.AreEqual("Hello, world", engine.Evaluate<string>("TestClass.Overload1('Hello, ', 'world')"));
            engine.SetGlobalValue("Int32", typeof(int));
            Assert.AreEqual(true, engine.Evaluate("TestClass.Overload1(new Int32(1), new Int32(2))"));

            // Incorrect number of arguments.
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("Math.Sin()"));
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("Math.Sin(5, 6)"));

            // Cannot convert between types.
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test2(5)"));
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test7('test')"));
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test12('test')"));
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test15(5)"));

            // Strings are converted to characters, but only if they are exactly one character in length.
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test14('')"));
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test14('abc')"));

            // Undefined can only be passed as an argument if the argument is object.
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test2(undefined)"));
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test7(undefined)"));
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test12(undefined)"));

            // Null can only be passed if the target type is a reference type.
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test2(null)"));
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test7(null)"));
            TestUtils.ExpectException<JavaScriptException>(() => engine.Evaluate("TestClass.Test12(null)"));
        }

        private class TestInstance
        {
            public int value;

            public TestInstance(int value)
            {
                this.value = value;
            }

            public TestInstance(TestInstance value)
            {
                this.value = value.value;
            }

            public int Value
            {
                get { return this.value; }
                set { this.value = value; }
            }

            public void Add(int delta)
            {
                this.value += delta;
            }

            public static TestInstance Add(TestInstance a, TestInstance b)
            {
                return new TestInstance(a.Value + b.Value);
            }

            public override string ToString()
            {
                return this.value.ToString();
            }
        }

        private class TestInstance2
        {
            public int value;
        }

        [TestMethod]
        public void SetGlobalValueClassInstance()
        {
            var engine = new ScriptEngine();

            engine.EnableExposedClrTypes = false;
            TestUtils.ExpectException<JavaScriptException>(() => engine.SetGlobalValue("TestInstance", typeof(TestInstance)));
            TestUtils.ExpectException<JavaScriptException>(() => engine.SetGlobalValue("TestInstance2", typeof(TestInstance2)));

            engine.EnableExposedClrTypes = true;

            // Try setting up some types.
            engine.SetGlobalValue("TestInstance", typeof(TestInstance));
            engine.SetGlobalValue("TestInstance2", typeof(TestInstance2));

            // Constructor.
            engine.Execute("var instance = new TestInstance(5)");

            // Keys.
            Assert.AreEqual("Add,Equals,ReferenceEquals,length,length,name,name", engine.Evaluate("var keys = []; x = TestInstance; while (x != null) { keys = keys.concat(Object.getOwnPropertyNames(x)); x = Object.getPrototypeOf(x); } keys.sort().toString()"));
            Assert.AreEqual("Add,Equals,GetHashCode,GetType,ToString,ToString,Value,value", engine.Evaluate("var keys = []; x = instance; while (x != null) { keys = keys.concat(Object.getOwnPropertyNames(x)); x = Object.getPrototypeOf(x); } keys.sort().toString()"));

            // Property
            Assert.AreEqual(5, engine.Evaluate("instance.Value"));
            engine.Execute("instance.Value = 6");
            Assert.AreEqual(6, engine.Evaluate("instance.Value"));

            // Field
            Assert.AreEqual(6, engine.Evaluate("instance.value"));
            engine.Execute("instance.value = 7");
            Assert.AreEqual(7, engine.Evaluate("instance.value"));

            // Instance method.
            engine.Execute("instance.Add(10)");
            Assert.AreEqual(17, engine.Evaluate("instance.Value"));
            Assert.AreEqual("17", engine.Evaluate("instance.ToString()"));

            // Static method.
            engine.Execute("var instance2 = TestInstance.Add(instance, instance)");
            Assert.AreEqual(17, engine.Evaluate("instance.Value"));
            Assert.AreEqual(34, engine.Evaluate("instance2.Value"));

            // Constructor overloading.
            engine.Execute("instance2 = new TestInstance(instance)");
            Assert.AreEqual(17, engine.Evaluate("instance2.Value"));

            // Types get unwrapped when they are returned from ScriptEngine APIs.
            Assert.AreEqual(typeof(TestInstance), engine.Evaluate("TestInstance"));
            Assert.AreEqual(typeof(TestInstance), engine.GetGlobalValue("TestInstance"));

            // Class with no constructors.
            engine.Execute("var instance2 = new TestInstance2()");
            Assert.AreEqual(0, engine.Evaluate("instance2.value"));
        }

        private struct TestStruct
        {
            public int value;

            public TestStruct(int value)
            {
                this.value = value;
            }

            public TestStruct(TestStruct value)
            {
                this.value = value.value;
            }

            public int Value
            {
                get { return this.value; }
                set { this.value = value; }
            }

            public void Add(int delta)
            {
                this.value += delta;
            }

            public static TestStruct Add(TestStruct a, TestStruct b)
            {
                return new TestStruct(a.Value + b.Value);
            }

            public override string ToString()
            {
                return this.value.ToString();
            }
        }

        [TestMethod]
        public void SetGlobalValueClassStruct()
        {
            var engine = new ScriptEngine();

            engine.EnableExposedClrTypes = false;
            TestUtils.ExpectException<JavaScriptException>(() => engine.SetGlobalValue("TestStruct", typeof(TestStruct)));

            // Try setting a type.
            engine.EnableExposedClrTypes = true;
            engine.SetGlobalValue("TestStruct", typeof(TestStruct));

            // Constructor.
            engine.Execute("var instance = new TestStruct(5)");

            // Keys.
            Assert.AreEqual("Add,Equals,ReferenceEquals,length,length,name,name,name", engine.Evaluate("var keys = []; x = TestStruct; while (x != null) { keys = keys.concat(Object.getOwnPropertyNames(x)); x = Object.getPrototypeOf(x); } keys.sort().toString()"));
            Assert.AreEqual("Add,Equals,Equals,GetHashCode,GetHashCode,GetType,ToString,ToString,ToString,Value,value", engine.Evaluate("var keys = []; x = instance; while (x != null) { keys = keys.concat(Object.getOwnPropertyNames(x)); x = Object.getPrototypeOf(x); } keys.sort().toString()"));

            // Property
            Assert.AreEqual(5, engine.Evaluate("instance.Value"));
            engine.Execute("instance.Value = 6");
            Assert.AreEqual(6, engine.Evaluate("instance.Value"));

            // Field
            Assert.AreEqual(6, engine.Evaluate("instance.value"));
            engine.Execute("instance.value = 7");
            Assert.AreEqual(7, engine.Evaluate("instance.value"));

            // Instance method.
            engine.Execute("instance.Add(10)");
            Assert.AreEqual(17, engine.Evaluate("instance.Value"));
            Assert.AreEqual("17", engine.Evaluate("instance.ToString()"));

            // Static method.
            engine.Execute("var instance2 = TestStruct.Add(instance, instance)");
            Assert.AreEqual(17, engine.Evaluate("instance.Value"));
            Assert.AreEqual(34, engine.Evaluate("instance2.Value"));

            // Constructor overloading.
            engine.Execute("instance2 = new TestStruct(instance)");
            Assert.AreEqual(17, engine.Evaluate("instance2.Value"));

            // Try dates.
            engine.SetGlobalValue("DateTime", typeof(DateTime));
            engine.Execute("date = new DateTime(2011, 3, 9, 7, 49, 0)");
            Assert.AreEqual(2011, engine.Evaluate("date.Year"));
            Assert.AreEqual(3, engine.Evaluate("date.DayOfWeek"));
            Assert.AreEqual(TimeSpan.FromDays(1), engine.Evaluate("date.Subtract(new DateTime(2011, 3, 8, 7, 49, 0))"));
        }

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

        private class TestScriptSource : ScriptSource
        {
            public override string Path
            {
                get { return "test"; }
            }

            public override System.IO.TextReader GetReader()
            {
                GetReaderCount++;
                return new TestReader(this);
            }

            public int GetReaderCount = 0;
            public int DisposeCount = 0;

            private class TestReader : System.IO.StringReader
            {
                private TestScriptSource parent;

                public TestReader(TestScriptSource parent)
                    : base("92")
                {
                    this.parent = parent;
                }

                protected override void Dispose(bool disposing)
                {
                    this.parent.DisposeCount++;
                    base.Dispose(disposing);
                }
            }
        }

        [TestMethod]
        public void ScriptSourceTests()
        {
            // Check that Dispose is called for every Reader that is created.
            var engine = new ScriptEngine();
            var source = new TestScriptSource();
            Assert.AreEqual(92, engine.Evaluate(source));
            Assert.AreEqual(source.GetReaderCount, source.DisposeCount);
            engine.Execute(source);
            Assert.AreEqual(source.GetReaderCount, source.DisposeCount);
        }

        [TestMethod]
        public void TestPropertyMarshalling()
        {
            ScriptEngine engine = new ScriptEngine();
            TestPropertyMarshalObj testObj = new TestPropertyMarshalObj(engine.Object.Prototype, 10);
            engine.SetGlobalValue("TestObject", testObj);
            Assert.AreEqual(10, engine.Evaluate<int>("TestObject.Value2"));
            engine.Execute("TestObject.Value = 5;");
            Assert.AreEqual(5, engine.Evaluate<int>("TestObject.Value"));
            Assert.AreEqual("5; 10", engine.Evaluate<string>("TestObject.toString()"));
        }

        private class TestPropertyMarshalObj : ObjectInstance
        {
            public TestPropertyMarshalObj(ObjectInstance prototype, int val2)
                : base(prototype)
            {
                this.PopulateFunctions();
                _val2 = val2;
            }

            private int _value;
            [JSProperty]
            public int Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }

            private int _val2;
            [JSProperty]
            public int Value2
            {
                get
                {
                    return _val2;
                }
            }

            [JSFunction(Name = "toString")]
            public override string ToString()
            {
                return _value + "; " + _val2;
            }
        }
    }
}
