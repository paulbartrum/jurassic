using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace Performance
{

    /// <summary>
    /// Benchmarks the conversion routines.
    /// </summary>
    [TestClass]
    public class ConversionBenchmarks
    {

        [TestMethod]
        public void ToNumber()
        {
            TestUtils.Benchmark(() =>
                {
                    Assert.AreEqual(1.0, TypeConverter.ToNumber("1"));
                    Assert.AreEqual(16.0, TypeConverter.ToNumber("16"));
                    Assert.AreEqual(32.0, TypeConverter.ToNumber("32"));
                    Assert.AreEqual(100.0, TypeConverter.ToNumber("100"));
                    Assert.AreEqual(1.5, TypeConverter.ToNumber("1.5"));
                    Assert.AreEqual(9.01, TypeConverter.ToNumber("9.01"));
                    Assert.AreEqual(3.141592654, TypeConverter.ToNumber("3.141592654"));
                    Assert.AreEqual(1.2e-15, TypeConverter.ToNumber("1.2e-15"));
                    Assert.AreEqual(0.0000000001234, TypeConverter.ToNumber("0.0000000001234"));
                    Assert.AreEqual(18014398509481990.0, TypeConverter.ToNumber("18014398509481990"));
                }, 30705);
        }

        [TestMethod]
        public void ToNumberYardstick()
        {
            TestUtils.Benchmark(() =>
            {
                Assert.AreEqual(1.0, double.Parse("1"));
                Assert.AreEqual(16.0, double.Parse("16"));
                Assert.AreEqual(32.0, double.Parse("32"));
                Assert.AreEqual(100.0, double.Parse("100"));
                Assert.AreEqual(1.5, double.Parse("1.5"));
                Assert.AreEqual(9.01, double.Parse("9.01"));
                Assert.AreEqual(3.141592654, double.Parse("3.141592654"));
                Assert.AreEqual(1.2e-15, double.Parse("1.2e-15"));
                Assert.AreEqual(0.0000000001234, double.Parse("0.0000000001234"));
                Assert.AreEqual(18014398509481990.0, double.Parse("18014398509481990"));
            }, 96365);
        }

        [TestMethod]
        public void ToString1()
        {
            TestUtils.Benchmark(() =>
            {
                Assert.AreEqual("1", TypeConverter.ToString(1.0));
                Assert.AreEqual("16", TypeConverter.ToString(16.0));
                Assert.AreEqual("32", TypeConverter.ToString(32.0));
                Assert.AreEqual("100", TypeConverter.ToString(100.0));
                Assert.AreEqual("65535", TypeConverter.ToString(65535.0));
                Assert.AreEqual("65536", TypeConverter.ToString(65536.0));
                Assert.AreEqual("123456789", TypeConverter.ToString(123456789.0));
                Assert.AreEqual("987654321", TypeConverter.ToString(987654321.0));
                Assert.AreEqual("1234567890", TypeConverter.ToString(1234567890.0));
                Assert.AreEqual("9876543210", TypeConverter.ToString(9876543210.0));
            }, 99503.7);
        }

        [TestMethod]
        public void ToString1Yardstick()
        {
            TestUtils.Benchmark(() =>
            {
                Assert.AreEqual("1", Convert.ToString(1.0));
                Assert.AreEqual("16", Convert.ToString(16.0));
                Assert.AreEqual("32", Convert.ToString(32.0));
                Assert.AreEqual("100", Convert.ToString(100.0));
                Assert.AreEqual("65535", Convert.ToString(65535.0));
                Assert.AreEqual("65536", Convert.ToString(65536.0));
                Assert.AreEqual("123456789", Convert.ToString(123456789.0));
                Assert.AreEqual("987654321", Convert.ToString(987654321.0));
                Assert.AreEqual("1234567890", Convert.ToString(1234567890.0));
                Assert.AreEqual("9876543210", Convert.ToString(9876543210.0));
            }, 45074);
        }

        [TestMethod]
        public void ToString2()
        {
            TestUtils.Benchmark(() =>
            {
                Assert.AreEqual("1.5", TypeConverter.ToString(1.5));
                Assert.AreEqual("0.000123", TypeConverter.ToString(0.000123));
                Assert.AreEqual("9.01", TypeConverter.ToString(9.01));
                Assert.AreEqual("3.141592654", TypeConverter.ToString(3.141592654));
                Assert.AreEqual("1.2e-15", TypeConverter.ToString(1.2e-15));
                Assert.AreEqual("1.234e-10", TypeConverter.ToString(0.0000000001234));
                Assert.AreEqual("18014398509481990", TypeConverter.ToString(18014398509481990.0));
                Assert.AreEqual("8.1234e+99", TypeConverter.ToString(8.1234e99));
                Assert.AreEqual("5.01e-320", TypeConverter.ToString(5.01e-320));
                Assert.AreEqual("1.23456789e+308", TypeConverter.ToString(1.23456789e308));
            }, 19933);
        }

        [TestMethod]
        public void ToString2Yardstick()
        {
            TestUtils.Benchmark(() =>
            {
                Assert.AreEqual("1.5", Convert.ToString(1.5));
                Assert.AreEqual("0.000123", Convert.ToString(0.000123));
                Assert.AreEqual("9.01", Convert.ToString(9.01));
                Assert.AreEqual("3.141592654", Convert.ToString(3.141592654));
                Assert.AreEqual("1.2E-15", Convert.ToString(1.2e-15));
                Assert.AreEqual("1.234E-10", Convert.ToString(0.0000000001234));
                Assert.AreEqual("1.8014398509482E+16", Convert.ToString(18014398509481990.0));
                Assert.AreEqual("8.1234E+99", Convert.ToString(8.1234e99));
                Assert.AreEqual("5.00982564883024E-320", Convert.ToString(5.01e-320));
                Assert.AreEqual("1.23456789E+308", Convert.ToString(1.23456789e308));
            }, 40278);
        }
    }

}