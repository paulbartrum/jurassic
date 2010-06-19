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
        public void ToBoolean()
        {
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
            Assert.AreEqual(true,  TypeConverter.ToBoolean(GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(GlobalObject.Date.Construct(0.0)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(GlobalObject.Object.Construct()));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(GlobalObject.String.Construct("")));
            Assert.AreEqual(true,  TypeConverter.ToBoolean(GlobalObject.String.Construct("test")));
        }

        [TestMethod]
        public new void ToString()
        {
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
            Assert.AreEqual("false",            TypeConverter.ToString(GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual("true",             TypeConverter.ToString(GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual("0",                TypeConverter.ToString(GlobalObject.Date.Construct(0.0)));
            Assert.AreEqual("NaN",              TypeConverter.ToString(GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual("0",                TypeConverter.ToString(GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual("1",                TypeConverter.ToString(GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual("[object Object]",  TypeConverter.ToString(GlobalObject.Object.Construct()));
            Assert.AreEqual("",                 TypeConverter.ToString(GlobalObject.String.Construct("")));
            Assert.AreEqual("test",             TypeConverter.ToString(GlobalObject.String.Construct("test")));
        }

        [TestMethod]
        public void ToNumber()
        {
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
            Assert.AreEqual(10.0,                       TypeConverter.ToNumber("10e"));
            Assert.AreEqual(10.0,                       TypeConverter.ToNumber("10e+"));
            Assert.AreEqual(0.0,                        TypeConverter.ToNumber(GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual(1.0,                        TypeConverter.ToNumber(GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual(0.0,                        TypeConverter.ToNumber(GlobalObject.Date.Construct(0.0)));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber(GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual(0.0,                        TypeConverter.ToNumber(GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual(1.0,                        TypeConverter.ToNumber(GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber(GlobalObject.Object.Construct()));
            Assert.AreEqual(0.0,                        TypeConverter.ToNumber(GlobalObject.String.Construct("")));
            Assert.AreEqual(double.NaN,                 TypeConverter.ToNumber(GlobalObject.String.Construct("test")));
            Assert.AreEqual(1.9,                        TypeConverter.ToNumber(GlobalObject.String.Construct("1.9")));
            Assert.AreEqual(16.0,                       TypeConverter.ToNumber(GlobalObject.String.Construct("0x10")));
        }

        [TestMethod]
        public void ToInteger()
        {
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
            Assert.AreEqual(0, TypeConverter.ToInteger(GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual(1, TypeConverter.ToInteger(GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual(1, TypeConverter.ToInteger(GlobalObject.Date.Construct(1.0)));
            Assert.AreEqual(0, TypeConverter.ToInteger(GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual(0, TypeConverter.ToInteger(GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual(1, TypeConverter.ToInteger(GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual(0, TypeConverter.ToInteger(GlobalObject.Object.Construct()));
            Assert.AreEqual(0, TypeConverter.ToInteger(GlobalObject.String.Construct("")));
            Assert.AreEqual(0, TypeConverter.ToInteger(GlobalObject.String.Construct("test")));
            Assert.AreEqual(1, TypeConverter.ToInteger(GlobalObject.String.Construct("1.9")));
        }

        [TestMethod]
        public void ToInt32()
        {
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
            Assert.AreEqual(0, TypeConverter.ToInt32(GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual(1, TypeConverter.ToInt32(GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual(1, TypeConverter.ToInt32(GlobalObject.Date.Construct(1.0)));
            Assert.AreEqual(0, TypeConverter.ToInt32(GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual(0, TypeConverter.ToInt32(GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual(1, TypeConverter.ToInt32(GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual(0, TypeConverter.ToInt32(GlobalObject.Object.Construct()));
            Assert.AreEqual(0, TypeConverter.ToInt32(GlobalObject.String.Construct("")));
            Assert.AreEqual(0, TypeConverter.ToInt32(GlobalObject.String.Construct("test")));
            Assert.AreEqual(1, TypeConverter.ToInt32(GlobalObject.String.Construct("1.9")));
        }

        [TestMethod]
        public void ToUint32()
        {
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
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(GlobalObject.Date.Construct(1.0)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(GlobalObject.Object.Construct()));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(GlobalObject.String.Construct("")));
            Assert.AreEqual((uint)0, TypeConverter.ToUint32(GlobalObject.String.Construct("test")));
            Assert.AreEqual((uint)1, TypeConverter.ToUint32(GlobalObject.String.Construct("1.9")));
        }

        [TestMethod]
        public void ToUint16()
        {
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
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(GlobalObject.Date.Construct(1.0)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(GlobalObject.Object.Construct()));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(GlobalObject.String.Construct("")));
            Assert.AreEqual((uint)0, TypeConverter.ToUint16(GlobalObject.String.Construct("test")));
            Assert.AreEqual((uint)1, TypeConverter.ToUint16(GlobalObject.String.Construct("1.9")));
        }

        [TestMethod]
        public void ToObject()
        {
            TestUtils.ExpectException<JavaScriptException>(() => TypeConverter.ToObject(Undefined.Value));
            TestUtils.ExpectException<JavaScriptException>(() => TypeConverter.ToObject(Null.Value));
            TestUtils.ExpectException<JavaScriptException>(() => TypeConverter.ToObject(null));

            Assert.IsInstanceOfType(TypeConverter.ToObject(false), typeof(BooleanInstance));
            Assert.AreEqual(false, TypeConverter.ToObject(false).CallMemberFunction("valueOf"));

            Assert.IsInstanceOfType(TypeConverter.ToObject(true), typeof(BooleanInstance));
            Assert.AreEqual(true, TypeConverter.ToObject(true).CallMemberFunction("valueOf"));

            Assert.IsInstanceOfType(TypeConverter.ToObject(13.9), typeof(NumberInstance));
            Assert.AreEqual(13.9, TypeConverter.ToObject(13.9).CallMemberFunction("valueOf"));

            Assert.IsInstanceOfType(TypeConverter.ToObject("test"), typeof(StringInstance));
            Assert.AreEqual("test", TypeConverter.ToObject("test").CallMemberFunction("valueOf"));

            // ToObject returns objects unaltered.
            var obj = TypeConverter.ToObject(GlobalObject.Boolean.Construct(true));
            Assert.AreSame(obj, TypeConverter.ToObject(obj));
        }
    }
}
