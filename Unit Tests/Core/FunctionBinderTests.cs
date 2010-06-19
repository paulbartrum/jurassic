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
    /// Test the function binder.
    /// </summary>
    [TestClass]
    public class FunctionBinderTests
    {
        [TestMethod]
        public void ToBoolean()
        {
            var function = new ClrFunction(GlobalObject.Function.InstancePrototype, new FunctionBinderMethod[] {
                new FunctionBinderMethod(new Func<bool, bool>(arg => arg).Method, FunctionBinderFlags.None)
            });
            Assert.AreEqual(false, function.CallLateBound(null, Null.Value));
            Assert.AreEqual(false, function.CallLateBound(null, Undefined.Value));
            Assert.AreEqual(false, function.CallLateBound(null, new object[] { null }));
            Assert.AreEqual(false, function.CallLateBound(null, false));
            Assert.AreEqual(true, function.CallLateBound(null, true));
            Assert.AreEqual(false, function.CallLateBound(null, +0.0));
            Assert.AreEqual(false, function.CallLateBound(null, -0.0));
            Assert.AreEqual(true, function.CallLateBound(null, 1.0));
            Assert.AreEqual(false, function.CallLateBound(null, double.NaN));
            Assert.AreEqual(false, function.CallLateBound(null, ""));
            Assert.AreEqual(true, function.CallLateBound(null, "false"));
            Assert.AreEqual(true, function.CallLateBound(null, "true"));
            Assert.AreEqual(true, function.CallLateBound(null, GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual(true, function.CallLateBound(null, GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual(true, function.CallLateBound(null, GlobalObject.Date.Construct(0.0)));
            Assert.AreEqual(true, function.CallLateBound(null, GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual(true, function.CallLateBound(null, GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual(true, function.CallLateBound(null, GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual(true, function.CallLateBound(null, GlobalObject.Object.Construct()));
            Assert.AreEqual(true, function.CallLateBound(null, GlobalObject.String.Construct("")));
            Assert.AreEqual(true, function.CallLateBound(null, GlobalObject.String.Construct("test")));
        }

        [TestMethod]
        public new void ToString()
        {
            var function = new ClrFunction(GlobalObject.Function.InstancePrototype, new FunctionBinderMethod[] {
                new FunctionBinderMethod(new Func<string, string>(arg => arg).Method, FunctionBinderFlags.None)
            });
            Assert.AreEqual("undefined", function.CallLateBound(null, Undefined.Value));
            Assert.AreEqual("undefined", function.CallLateBound(null, new object[] { null }));
            Assert.AreEqual("null", function.CallLateBound(null, Null.Value));
            Assert.AreEqual("false", function.CallLateBound(null, false));
            Assert.AreEqual("true", function.CallLateBound(null, true));
            Assert.AreEqual("0", function.CallLateBound(null, +0.0));
            Assert.AreEqual("0", function.CallLateBound(null, -0.0));
            Assert.AreEqual("1", function.CallLateBound(null, 1.0));
            Assert.AreEqual("13.9", function.CallLateBound(null, 13.9));
            Assert.AreEqual("6442450954", function.CallLateBound(null, 6442450954.0));
            Assert.AreEqual("NaN", function.CallLateBound(null, double.NaN));
            Assert.AreEqual("", function.CallLateBound(null, ""));
            Assert.AreEqual("false", function.CallLateBound(null, GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual("true", function.CallLateBound(null, GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual("0", function.CallLateBound(null, GlobalObject.Date.Construct(0.0)));
            Assert.AreEqual("NaN", function.CallLateBound(null, GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual("0", function.CallLateBound(null, GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual("1", function.CallLateBound(null, GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual("[object Object]", function.CallLateBound(null, GlobalObject.Object.Construct()));
            Assert.AreEqual("", function.CallLateBound(null, GlobalObject.String.Construct("")));
            Assert.AreEqual("test", function.CallLateBound(null, GlobalObject.String.Construct("test")));
        }

        [TestMethod]
        public void ToNumber()
        {
            var function = new ClrFunction(GlobalObject.Function.InstancePrototype, new FunctionBinderMethod[] {
                new FunctionBinderMethod(new Func<double, double>(arg => arg).Method, FunctionBinderFlags.None)
            });
            Assert.AreEqual(+0.0, function.CallLateBound(null, Null.Value));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, Undefined.Value));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, new object[] { null }));
            Assert.AreEqual(+0.0, function.CallLateBound(null, false));
            Assert.AreEqual(1.0, function.CallLateBound(null, true));
            Assert.AreEqual(+0.0, function.CallLateBound(null, +0.0));
            Assert.AreEqual(-0.0, function.CallLateBound(null, -0.0));
            Assert.AreEqual(1.0, function.CallLateBound(null, 1.0));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, double.NaN));
            Assert.AreEqual(0.0, function.CallLateBound(null, ""));
            Assert.AreEqual(1.0, function.CallLateBound(null, "1.0"));
            Assert.AreEqual(1.0, function.CallLateBound(null, "1"));
            Assert.AreEqual(1.0, function.CallLateBound(null, "+1.0"));
            Assert.AreEqual(-1.0, function.CallLateBound(null, "-1.0"));
            Assert.AreEqual(100.0, function.CallLateBound(null, "1.0e2"));
            Assert.AreEqual(100.0, function.CallLateBound(null, "1.0e+2"));
            Assert.AreEqual(100.0, function.CallLateBound(null, "1e2"));
            Assert.AreEqual(0.01, function.CallLateBound(null, "1.0e-2"));
            Assert.AreEqual(double.PositiveInfinity, function.CallLateBound(null, "Infinity"));
            Assert.AreEqual(double.NegativeInfinity, function.CallLateBound(null, "-Infinity"));
            Assert.AreEqual(16.0, function.CallLateBound(null, "0x10"));
            Assert.AreEqual(-16.0, function.CallLateBound(null, "-0x10"));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, "- 10"));
            Assert.AreEqual(-10.0, function.CallLateBound(null, " -10"));
            Assert.AreEqual(16.0, function.CallLateBound(null, " 0x10"));
            Assert.AreEqual(double.PositiveInfinity, function.CallLateBound(null, " Infinity"));
            Assert.AreEqual(16.0, function.CallLateBound(null, "0x10 "));
            Assert.AreEqual(10.0, function.CallLateBound(null, "10.0 "));
            Assert.AreEqual(double.PositiveInfinity, function.CallLateBound(null, "Infinity "));
            Assert.AreEqual(10.0, function.CallLateBound(null, "10 "));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, "10z"));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, "0z10"));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, "10z"));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, "z10"));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, "10.0z"));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, "Infinityz"));
            Assert.AreEqual(10.0, function.CallLateBound(null, "10."));
            Assert.AreEqual(10.0, function.CallLateBound(null, "10e"));
            Assert.AreEqual(10.0, function.CallLateBound(null, "10e+"));
            Assert.AreEqual(0.0, function.CallLateBound(null, GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual(1.0, function.CallLateBound(null, GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual(0.0, function.CallLateBound(null, GlobalObject.Date.Construct(0.0)));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual(0.0, function.CallLateBound(null, GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual(1.0, function.CallLateBound(null, GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, GlobalObject.Object.Construct()));
            Assert.AreEqual(0.0, function.CallLateBound(null, GlobalObject.String.Construct("")));
            Assert.AreEqual(double.NaN, function.CallLateBound(null, GlobalObject.String.Construct("test")));
            Assert.AreEqual(1.9, function.CallLateBound(null, GlobalObject.String.Construct("1.9")));
            Assert.AreEqual(16.0, function.CallLateBound(null, GlobalObject.String.Construct("0x10")));
        }

        [TestMethod]
        public void ToInteger()
        {
            var function = new ClrFunction(GlobalObject.Function.InstancePrototype, new FunctionBinderMethod[] {
                new FunctionBinderMethod(new Func<int, int>(arg => arg).Method, FunctionBinderFlags.None)
            });
            Assert.AreEqual(0, function.CallLateBound(null, Undefined.Value));
            Assert.AreEqual(0, function.CallLateBound(null, Null.Value));
            Assert.AreEqual(0, function.CallLateBound(null, new object[] { null }));
            Assert.AreEqual(0, function.CallLateBound(null, false));
            Assert.AreEqual(1, function.CallLateBound(null, true));
            Assert.AreEqual(0, function.CallLateBound(null, +0.0));
            Assert.AreEqual(0, function.CallLateBound(null, -0.0));
            Assert.AreEqual(1, function.CallLateBound(null, 1.0));
            Assert.AreEqual(1, function.CallLateBound(null, 1.9));
            Assert.AreEqual(-1, function.CallLateBound(null, -1.9));
            Assert.AreEqual(int.MaxValue, function.CallLateBound(null, 6442450954.0));
            Assert.AreEqual(int.MinValue, function.CallLateBound(null, -6442450954.0));
            Assert.AreEqual(int.MaxValue, function.CallLateBound(null, double.PositiveInfinity));
            Assert.AreEqual(int.MinValue, function.CallLateBound(null, double.NegativeInfinity));
            Assert.AreEqual(0, function.CallLateBound(null, double.NaN));
            Assert.AreEqual(0, function.CallLateBound(null, ""));
            Assert.AreEqual(1, function.CallLateBound(null, "1.6"));
            Assert.AreEqual(-1, function.CallLateBound(null, "-1.6"));
            Assert.AreEqual(1, function.CallLateBound(null, "1"));
            Assert.AreEqual(1, function.CallLateBound(null, "+1.0"));
            Assert.AreEqual(100, function.CallLateBound(null, "1.0e2"));
            Assert.AreEqual(int.MaxValue, function.CallLateBound(null, "4294967304"));
            Assert.AreEqual(int.MaxValue, function.CallLateBound(null, "2147483658"));
            Assert.AreEqual(int.MaxValue, function.CallLateBound(null, "6442450954"));
            Assert.AreEqual(0, function.CallLateBound(null, GlobalObject.Boolean.Construct(false)));
            Assert.AreEqual(1, function.CallLateBound(null, GlobalObject.Boolean.Construct(true)));
            Assert.AreEqual(1, function.CallLateBound(null, GlobalObject.Date.Construct(1.0)));
            Assert.AreEqual(0, function.CallLateBound(null, GlobalObject.Date.Construct(double.NaN)));
            Assert.AreEqual(0, function.CallLateBound(null, GlobalObject.Number.Construct(0.0)));
            Assert.AreEqual(1, function.CallLateBound(null, GlobalObject.Number.Construct(1.0)));
            Assert.AreEqual(0, function.CallLateBound(null, GlobalObject.Object.Construct()));
            Assert.AreEqual(0, function.CallLateBound(null, GlobalObject.String.Construct("")));
            Assert.AreEqual(0, function.CallLateBound(null, GlobalObject.String.Construct("test")));
            Assert.AreEqual(1, function.CallLateBound(null, GlobalObject.String.Construct("1.9")));
        }
    }
}
