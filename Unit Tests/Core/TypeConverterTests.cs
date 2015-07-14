using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the type conversion routines.
    /// </summary>
    [TestClass]
    public class TypeConverterTests
    {
        [TestMethod]
        public void ConvertTo()
        {
            var engine = new ScriptEngine();

            // ConvertTo object
            Assert.AreEqual(5, TypeConverter.ConvertTo<object>(engine, 5));

            // ConvertTo bool
            Assert.AreEqual(false, TypeConverter.ConvertTo<bool>(engine, 0));
            Assert.AreEqual(false, TypeConverter.ConvertTo<bool>(engine, 0.0));
            Assert.AreEqual(true, TypeConverter.ConvertTo<bool>(engine, 5.1));
            Assert.AreEqual(true, TypeConverter.ConvertTo<bool>(engine, 5.9));
            Assert.AreEqual(false, TypeConverter.ConvertTo<bool>(engine, ""));
            Assert.AreEqual(true, TypeConverter.ConvertTo<bool>(engine, "a"));

            // ConvertTo int
            Assert.AreEqual(5, TypeConverter.ConvertTo<int>(engine, 5.1));
            Assert.AreEqual(5, TypeConverter.ConvertTo<int>(engine, 5.9));
            Assert.AreEqual(-5, TypeConverter.ConvertTo<int>(engine, -5.1));
            Assert.AreEqual(-5, TypeConverter.ConvertTo<int>(engine, -5.9));
            Assert.AreEqual(int.MaxValue, TypeConverter.ConvertTo<int>(engine, 90000000000.0));
            Assert.AreEqual(int.MinValue, TypeConverter.ConvertTo<int>(engine, -90000000000.0));
            Assert.AreEqual(5, TypeConverter.ConvertTo<int>(engine, "5.9"));
            Assert.AreEqual(0, TypeConverter.ConvertTo<int>(engine, "a"));
            Assert.AreEqual(1, TypeConverter.ConvertTo<int>(engine, true));

            // ConvertTo double
            Assert.AreEqual(90000000000.0, TypeConverter.ConvertTo<double>(engine, 90000000000.0));
            Assert.AreEqual(-90000000000.0, TypeConverter.ConvertTo<double>(engine, -90000000000.0));
            Assert.AreEqual(5.9, TypeConverter.ConvertTo<double>(engine, "5.9"));
            Assert.AreEqual(double.NaN, TypeConverter.ConvertTo<double>(engine, "a"));
            Assert.AreEqual(1.0, TypeConverter.ConvertTo<double>(engine, true));

            // ConvertTo string
            Assert.AreEqual("90000000000", TypeConverter.ConvertTo<string>(engine, 90000000000.0));
            Assert.AreEqual("true", TypeConverter.ConvertTo<string>(engine, true));

            // ConvertTo ObjectInstance
            Assert.IsInstanceOfType(TypeConverter.ConvertTo<ObjectInstance>(engine, 100), typeof(NumberInstance));
            Assert.AreEqual(100.0, ((NumberInstance)TypeConverter.ConvertTo<ObjectInstance>(engine, 100)).ValueOf());
            Assert.IsInstanceOfType(TypeConverter.ConvertTo<NumberInstance>(engine, 100), typeof(NumberInstance));
        }

        [TestMethod]
        public void ToBoolean()
        {
            var engine = new ScriptEngine();
            Assert.AreEqual(false, TypeConverter.ToBoolean(Null.Value));
            Assert.AreEqual(false, TypeConverter.ToBoolean(Undefined.Value));
            Assert.AreEqual(false, TypeConverter.ToBoolean(null));
            Assert.AreEqual(false, TypeConverter.ToBoolean(false));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(true));
            Assert.AreEqual(false, TypeConverter.ToBoolean(+0.0));
            Assert.AreEqual(false, TypeConverter.ToBoolean(-0.0));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(1.0));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(13.9));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(6442450954.0));
            Assert.AreEqual(false, TypeConverter.ToBoolean(double.NaN));
            Assert.AreEqual(false, TypeConverter.ToBoolean(""));
            Assert.AreEqual(true,  TypeConverter.ToBoolean("false"));
            Assert.AreEqual(true,  TypeConverter.ToBoolean("true"));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(engine.Boolean.Construct(false)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(engine.Boolean.Construct(true)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(engine.Date.Construct(0.0)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(engine.Date.Construct(double.NaN)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(engine.Number.Construct(0.0)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(engine.Number.Construct(1.0)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(engine.Object.Construct()));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(engine.String.Construct("")));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(engine.String.Construct("test")));
        }

        [TestMethod]
        public new void ToString()
        {
            var engine = new ScriptEngine();
            Assert.AreEqual("undefined",        TypeConverter.ToString(Undefined.Value));
            Assert.AreEqual("undefined",        TypeConverter.ToString(null));
            Assert.AreEqual("null",             TypeConverter.ToString(Null.Value));
            Assert.AreEqual("false",            TypeConverter.ToString(false));
            Assert.AreEqual("true",             TypeConverter.ToString(true));
            Assert.AreEqual("0",                TypeConverter.ToString(+0.0));
            Assert.AreEqual("0",                TypeConverter.ToString(-0.0));
            Assert.AreEqual("1",                TypeConverter.ToString(1.0));
            Assert.AreEqual("13.9",             TypeConverter.ToString(13.9));
            Assert.AreEqual("6442450954",       TypeConverter.ToString(6442450954.0));
            Assert.AreEqual("NaN",              TypeConverter.ToString(double.NaN));
            Assert.AreEqual("",                 TypeConverter.ToString(""));
            Assert.AreEqual("false",            TypeConverter.ToString(engine.Boolean.Construct(false)));
            Assert.AreEqual("true",             TypeConverter.ToString(engine.Boolean.Construct(true)));
            Assert.AreEqual(TestUtils.Evaluate("new Date(0).toString()"), TypeConverter.ToString(engine.Date.Construct(0.0)));  // Note: exact string is time-zone specific.
            Assert.AreEqual("Invalid Date",     TypeConverter.ToString(engine.Date.Construct(double.NaN)));
            Assert.AreEqual("0",                TypeConverter.ToString(engine.Number.Construct(0.0)));
            Assert.AreEqual("1",                TypeConverter.ToString(engine.Number.Construct(1.0)));
            Assert.AreEqual("[object Object]",  TypeConverter.ToString(engine.Object.Construct()));
            Assert.AreEqual("",                 TypeConverter.ToString(engine.String.Construct("")));
            Assert.AreEqual("test",             TypeConverter.ToString(engine.String.Construct("test")));
        }

        [TestMethod]
        public void ToNumber()
        {
            var engine = new ScriptEngine();
            Assert.AreEqual(+0.0,                       TypeConverter.ToNumber(Null.Value));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber(Undefined.Value));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber(null));
            Assert.AreEqual(+0.0,                       TypeConverter.ToNumber(false));
            Assert.AreEqual(1.0,                        TypeConverter.ToNumber(true));
            Assert.AreEqual(+0.0,                       TypeConverter.ToNumber(+0.0));
            Assert.AreEqual(-0.0,                       TypeConverter.ToNumber(-0.0));
            Assert.AreEqual(1.0,                        TypeConverter.ToNumber(1.0));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber(double.NaN));
            Assert.AreEqual(0.0,                        TypeConverter.ToNumber(""));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("a"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("+"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("-"));
            Assert.AreEqual(1.0,                        TypeConverter.ToNumber("1.0"));
            Assert.AreEqual(1.0,                        TypeConverter.ToNumber("1"));
            Assert.AreEqual(1.0,                        TypeConverter.ToNumber("+1.0"));
            Assert.AreEqual(-1.0,                       TypeConverter.ToNumber("-1.0"));
            Assert.AreEqual(100.0,                      TypeConverter.ToNumber("1.0e2"));
            Assert.AreEqual(100.0,                      TypeConverter.ToNumber("1.0e+2"));
            Assert.AreEqual(100.0,                      TypeConverter.ToNumber("1e2"));
            Assert.AreEqual(0.01,                       TypeConverter.ToNumber("1.0e-2"));
            Assert.AreEqual(double.PositiveInfinity,    TypeConverter.ToNumber("Infinity"));
            Assert.AreEqual(double.NegativeInfinity,    TypeConverter.ToNumber("-Infinity"));
            Assert.AreEqual(16.0,                       TypeConverter.ToNumber("0x10"));
            Assert.AreEqual(-16.0,                      TypeConverter.ToNumber("-0x10"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("- 10"));
            Assert.AreEqual(-10.0,                      TypeConverter.ToNumber(" -10"));
            Assert.AreEqual(16.0,                       TypeConverter.ToNumber(" 0x10"));
            Assert.AreEqual(double.PositiveInfinity,    TypeConverter.ToNumber(" Infinity"));
            Assert.AreEqual(16.0,                       TypeConverter.ToNumber("0x10 "));
            Assert.AreEqual(10.0,                       TypeConverter.ToNumber("10.0 "));
            Assert.AreEqual(double.PositiveInfinity,    TypeConverter.ToNumber("Infinity "));
            Assert.AreEqual(10.0,                       TypeConverter.ToNumber("10 "));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("10z"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("0z10"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("10z"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("z10"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("10.0z"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("Infinityz"));
            Assert.AreEqual(10.0,                       TypeConverter.ToNumber("10."));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("10e"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("10e+"));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber("0x"));
            Assert.AreEqual(11.0,                       TypeConverter.ToNumber("011"));
            Assert.AreEqual(0.0,                        TypeConverter.ToNumber(engine.Boolean.Construct(false)));
            Assert.AreEqual(1.0,                        TypeConverter.ToNumber(engine.Boolean.Construct(true)));
            Assert.AreEqual(0.0,                        TypeConverter.ToNumber(engine.Date.Construct(0.0)));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber(engine.Date.Construct(double.NaN)));
            Assert.AreEqual(0.0,                        TypeConverter.ToNumber(engine.Number.Construct(0.0)));
            Assert.AreEqual(1.0,                        TypeConverter.ToNumber(engine.Number.Construct(1.0)));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber(engine.Object.Construct()));
            Assert.AreEqual(0.0,                        TypeConverter.ToNumber(engine.String.Construct("")));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber(engine.String.Construct("test")));
            Assert.AreEqual(1.9,                        TypeConverter.ToNumber(engine.String.Construct("1.9")));
            Assert.AreEqual(16.0,                       TypeConverter.ToNumber(engine.String.Construct("0x10")));
        }

        [TestMethod]
        public void ToInteger()
        {
            var engine = new ScriptEngine();
            Assert.AreEqual(0, TypeConverter.ToInteger(Undefined.Value));
            Assert.AreEqual(0, TypeConverter.ToInteger(Null.Value));
            Assert.AreEqual(0, TypeConverter.ToInteger(null));
            Assert.AreEqual(0, TypeConverter.ToInteger(false));
            Assert.AreEqual(1, TypeConverter.ToInteger(true));
            Assert.AreEqual(0, TypeConverter.ToInteger(+0.0));
            Assert.AreEqual(0, TypeConverter.ToInteger(-0.0));
            Assert.AreEqual(1, TypeConverter.ToInteger(1.0));
            Assert.AreEqual(1, TypeConverter.ToInteger(1.9));
            Assert.AreEqual(-1, TypeConverter.ToInteger(-1.9));
            Assert.AreEqual(int.MaxValue, TypeConverter.ToInteger(double.PositiveInfinity));
            Assert.AreEqual(int.MinValue, TypeConverter.ToInteger(double.NegativeInfinity));
            Assert.AreEqual(0, TypeConverter.ToInteger(double.NaN));
            Assert.AreEqual(0, TypeConverter.ToInteger(""));
            Assert.AreEqual(1, TypeConverter.ToInteger("1.6"));
            Assert.AreEqual(-1, TypeConverter.ToInteger("-1.6"));
            Assert.AreEqual(1, TypeConverter.ToInteger("1"));
            Assert.AreEqual(1, TypeConverter.ToInteger("+1.0"));
            Assert.AreEqual(100, TypeConverter.ToInteger("1.0e2"));
            Assert.AreEqual(int.MaxValue, TypeConverter.ToInteger("4294967304"));
            Assert.AreEqual(int.MaxValue, TypeConverter.ToInteger("2147483658"));
            Assert.AreEqual(int.MaxValue, TypeConverter.ToInteger("6442450954"));
            Assert.AreEqual(0, TypeConverter.ToInteger(engine.Boolean.Construct(false)));
            Assert.AreEqual(1, TypeConverter.ToInteger(engine.Boolean.Construct(true)));
            Assert.AreEqual(1, TypeConverter.ToInteger(engine.Date.Construct(1.0)));
            Assert.AreEqual(0, TypeConverter.ToInteger(engine.Date.Construct(double.NaN)));
            Assert.AreEqual(0, TypeConverter.ToInteger(engine.Number.Construct(0.0)));
            Assert.AreEqual(1, TypeConverter.ToInteger(engine.Number.Construct(1.0)));
            Assert.AreEqual(0, TypeConverter.ToInteger(engine.Object.Construct()));
            Assert.AreEqual(0, TypeConverter.ToInteger(engine.String.Construct("")));
            Assert.AreEqual(0, TypeConverter.ToInteger(engine.String.Construct("test")));
            Assert.AreEqual(1, TypeConverter.ToInteger(engine.String.Construct("1.9")));
        }

        [TestMethod]
        public void ToInt32()
        {
            var engine = new ScriptEngine();
            Assert.AreEqual(0, TypeConverter.ToInt32(Undefined.Value));
            Assert.AreEqual(0, TypeConverter.ToInt32(Null.Value));
            Assert.AreEqual(0, TypeConverter.ToInt32(null));
            Assert.AreEqual(0, TypeConverter.ToInt32(false));
            Assert.AreEqual(1, TypeConverter.ToInt32(true));
            Assert.AreEqual(0, TypeConverter.ToInt32(+0.0));
            Assert.AreEqual(0, TypeConverter.ToInt32(-0.0));
            Assert.AreEqual(1, TypeConverter.ToInt32(1.0));
            Assert.AreEqual(1, TypeConverter.ToInt32(1.9));
            Assert.AreEqual(-1, TypeConverter.ToInt32(-1.9));
            Assert.AreEqual(0, TypeConverter.ToInt32(double.PositiveInfinity));
            Assert.AreEqual(0, TypeConverter.ToInt32(double.NegativeInfinity));
            Assert.AreEqual(0, TypeConverter.ToInt32(double.NaN));
            Assert.AreEqual(0, TypeConverter.ToInt32(""));
            Assert.AreEqual(1, TypeConverter.ToInt32("1.6"));
            Assert.AreEqual(-1, TypeConverter.ToInt32("-1.6"));
            Assert.AreEqual(1, TypeConverter.ToInt32("1"));
            Assert.AreEqual(1, TypeConverter.ToInt32("+1.0"));
            Assert.AreEqual(-1, TypeConverter.ToInt32("-1.0"));
            Assert.AreEqual(100, TypeConverter.ToInt32("1.0e2"));
            Assert.AreEqual(8, TypeConverter.ToInt32("4294967304"));
            Assert.AreEqual(-2147483638, TypeConverter.ToInt32("2147483658"));
            Assert.AreEqual(-2147483638, TypeConverter.ToInt32("6442450954"));
            Assert.AreEqual(0, TypeConverter.ToInt32(engine.Boolean.Construct(false)));
            Assert.AreEqual(1, TypeConverter.ToInt32(engine.Boolean.Construct(true)));
            Assert.AreEqual(1, TypeConverter.ToInt32(engine.Date.Construct(1.0)));
            Assert.AreEqual(0, TypeConverter.ToInt32(engine.Date.Construct(double.NaN)));
            Assert.AreEqual(0, TypeConverter.ToInt32(engine.Number.Construct(0.0)));
            Assert.AreEqual(1, TypeConverter.ToInt32(engine.Number.Construct(1.0)));
            Assert.AreEqual(0, TypeConverter.ToInt32(engine.Object.Construct()));
            Assert.AreEqual(0, TypeConverter.ToInt32(engine.String.Construct("")));
            Assert.AreEqual(0, TypeConverter.ToInt32(engine.String.Construct("test")));
            Assert.AreEqual(1, TypeConverter.ToInt32(engine.String.Construct("1.9")));
        }

        [TestMethod]
        public void ToUint32()
        {
            var engine = new ScriptEngine();
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(Null.Value));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(Undefined.Value));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(null));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(false));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(true));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(+0.0));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(-0.0));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(1.0));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(1.9));
            Assert.AreEqual((uint)4294967295, TypeConverter.ToUint32(-1.9));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(double.PositiveInfinity));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(double.NegativeInfinity));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(double.NaN));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(""));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32("1.6"));
            Assert.AreEqual((uint)4294967295, TypeConverter.ToUint32("-1.6"));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32("1"));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32("+1.0"));
            Assert.AreEqual((uint)4294967295, TypeConverter.ToUint32("-1.0"));
            Assert.AreEqual((uint)100, TypeConverter.ToUint32("1.0e2"));
            Assert.AreEqual((uint)8, TypeConverter.ToUint32("4294967304"));
            Assert.AreEqual((uint)2147483658, TypeConverter.ToUint32("2147483658"));
            Assert.AreEqual((uint)2147483659, TypeConverter.ToUint32("6442450955"));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(engine.Boolean.Construct(false)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(engine.Boolean.Construct(true)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(engine.Date.Construct(1.0)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(engine.Date.Construct(double.NaN)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(engine.Number.Construct(0.0)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(engine.Number.Construct(1.0)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(engine.Object.Construct()));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(engine.String.Construct("")));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(engine.String.Construct("test")));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(engine.String.Construct("1.9")));
        }

        [TestMethod]
        public void ToUint16()
        {
            var engine = new ScriptEngine();
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(Null.Value));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(Undefined.Value));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(null));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(false));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(true));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(+0.0));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(-0.0));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(1.0));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(1.9));
            Assert.AreEqual((uint)65535, TypeConverter.ToUint16(-1.9));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(double.PositiveInfinity));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(double.NegativeInfinity));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(double.NaN));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(""));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16("1.6"));
            Assert.AreEqual((uint)65535, TypeConverter.ToUint16("-1.6"));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16("1"));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16("+1.0"));
            Assert.AreEqual((uint)65535, TypeConverter.ToUint16("-1.0"));
            Assert.AreEqual((uint)100, TypeConverter.ToUint16("1.0e2"));
            Assert.AreEqual((uint)30, TypeConverter.ToUint16("65566"));
            Assert.AreEqual((uint)7616, TypeConverter.ToUint16("-123456"));
            Assert.AreEqual((uint)48128, TypeConverter.ToUint16("6000000000"));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(engine.Boolean.Construct(false)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(engine.Boolean.Construct(true)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(engine.Date.Construct(1.0)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(engine.Date.Construct(double.NaN)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(engine.Number.Construct(0.0)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(engine.Number.Construct(1.0)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(engine.Object.Construct()));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(engine.String.Construct("")));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(engine.String.Construct("test")));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(engine.String.Construct("1.9")));
        }

        [TestMethod]
        public void ToObject()
        {
            var engine = new ScriptEngine();
            TestUtils.ExpectException<JavaScriptException>(() => TypeConverter.ToObject(engine, Undefined.Value));
            TestUtils.ExpectException<JavaScriptException>(() => TypeConverter.ToObject(engine, Null.Value));
            TestUtils.ExpectException<JavaScriptException>(() => TypeConverter.ToObject(engine, null));

            Assert.IsInstanceOfType(TypeConverter.ToObject(engine, false), typeof(BooleanInstance));
            Assert.AreEqual(false, TypeConverter.ToObject(engine, false).CallMemberFunction("valueOf"));

            Assert.IsInstanceOfType(TypeConverter.ToObject(engine, true), typeof(BooleanInstance));
            Assert.AreEqual(true, TypeConverter.ToObject(engine, true).CallMemberFunction("valueOf"));

            Assert.IsInstanceOfType(TypeConverter.ToObject(engine, 13.9), typeof(NumberInstance));
            Assert.AreEqual(13.9, TypeConverter.ToObject(engine, 13.9).CallMemberFunction("valueOf"));

            Assert.IsInstanceOfType(TypeConverter.ToObject(engine, "test"), typeof(StringInstance));
            Assert.AreEqual("test", TypeConverter.ToObject(engine, "test").CallMemberFunction("valueOf"));

            // ToObject returns objects unaltered.
            var obj = TypeConverter.ToObject(engine, engine.Boolean.Construct(true));
            Assert.AreSame(obj, TypeConverter.ToObject(engine, obj));
        }
    }
}
