using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the global Math object.
    /// </summary>
    [TestClass]
    public class MathTests : TestBase
    {
        [TestMethod]
        public void Initialization()
        {
            // Math is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Math"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Math()"));

            // Test the object overrides.
            Assert.AreEqual("[object Math]", Evaluate("Math.toString()"));
            Assert.AreEqual("Math", Evaluate("Math[Symbol.toStringTag]"));
            Assert.AreEqual(true, Evaluate("Math.valueOf() === Math"));

            // Test the Math constants.
            Assert.AreEqual(2.7182818284590451, (double)Evaluate("Math.E"), 0.000000000000001);
            Assert.AreEqual(0.6931471805599453, (double)Evaluate("Math.LN2"), 0.000000000000001);
            Assert.AreEqual(2.3025850929940456, (double)Evaluate("Math.LN10"), 0.000000000000001);
            Assert.AreEqual(1.4426950408889634, (double)Evaluate("Math.LOG2E"), 0.000000000000001);
            Assert.AreEqual(0.4342944819032518, (double)Evaluate("Math.LOG10E"), 0.000000000000001);
            Assert.AreEqual(3.1415926535897932, (double)Evaluate("Math.PI"), 0.000000000000001);
            Assert.AreEqual(0.7071067811865475, (double)Evaluate("Math.SQRT1_2"), 0.000000000000001);
            Assert.AreEqual(1.4142135623730950, (double)Evaluate("Math.SQRT2"), 0.000000000000001);

            // Constants are not enumerable, writable or configurable.
            Assert.AreEqual(false, Evaluate("delete Math.E"));
            Assert.AreEqual(false, Evaluate("Math.E = 5; Math.E === 5"));
        }

        [TestMethod]
        public void abs()
        {
            Assert.AreEqual(1, Evaluate("Math.abs(1)"));
            Assert.AreEqual(1.5, Evaluate("Math.abs(1.5)"));
            Assert.AreEqual(1, Evaluate("Math.abs(-1)"));
            Assert.AreEqual(1.5, Evaluate("Math.abs(-1.5)"));
            Assert.AreEqual(0, Evaluate("Math.abs(0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.abs(Infinity)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.abs(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.abs(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.abs(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.abs('bob')"));
            Assert.AreEqual(0, Evaluate("Math.abs(null)"));
        }

        [TestMethod]
        public void acos()
        {
            Assert.AreEqual(0, Evaluate("Math.acos(1)"));
            Assert.AreEqual(Math.PI, Evaluate("Math.acos(-1)"));
            Assert.AreEqual(Math.PI / 2, Evaluate("Math.acos(0)"));
            Assert.AreEqual(Math.Acos(0.5), (double)Evaluate("Math.acos(0.5)"), 0.000000000000001);
            Assert.AreEqual(double.NaN, Evaluate("Math.acos(2)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acos(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acos(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acos(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acos(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acos('bob')"));
        }

        [TestMethod]
        public void asin()
        {
            Assert.AreEqual(Math.PI / 2, Evaluate("Math.asin(1)"));
            Assert.AreEqual(-Math.PI / 2, Evaluate("Math.asin(-1)"));
            Assert.AreEqual(0, Evaluate("Math.asin(0)"));
            Assert.AreEqual(Math.Asin(0.5), Evaluate("Math.asin(0.5)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.asin(2)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.asin(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.asin(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.asin(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.asin(undefined)"));
            Assert.AreEqual(0, Evaluate("Math.asin(null)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.asin('bob')"));
        }

        [TestMethod]
        public void atan()
        {
            Assert.AreEqual(Math.PI / 4, Evaluate("Math.atan(1)"));
            Assert.AreEqual(-Math.PI / 4, Evaluate("Math.atan(-1)"));
            Assert.AreEqual(0, Evaluate("Math.atan(0)"));
            Assert.AreEqual(Math.Atan(0.5), Evaluate("Math.atan(0.5)"));
            Assert.AreEqual(Math.Atan(2), Evaluate("Math.atan(2)"));
            Assert.AreEqual(Math.PI / 2, Evaluate("Math.atan(Infinity)"));
            Assert.AreEqual(-Math.PI / 2, Evaluate("Math.atan(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atan(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atan(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atan('bob')"));
        }

        [TestMethod]
        public void atan2()
        {
            Assert.AreEqual(0, Evaluate("Math.atan2(0, 0)"));
            Assert.AreEqual(Math.PI / 2, Evaluate("Math.atan2(1, 0)"));
            Assert.AreEqual(Math.PI, Evaluate("Math.atan2(0, -1)"));
            Assert.AreEqual(-Math.PI / 2, Evaluate("Math.atan2(-1, 0)"));
            Assert.AreEqual(0, Evaluate("Math.atan2(0, 1)"));
            Assert.AreEqual(Math.PI / 4, Evaluate("Math.atan2(1, 1)"));
            Assert.AreEqual(Math.PI / 4, Evaluate("Math.atan2(19, 19)"));
            Assert.AreEqual(Math.PI / 4, Evaluate("Math.atan2(Infinity, Infinity)"));
            Assert.AreEqual(3 * Math.PI / 4, Evaluate("Math.atan2(Infinity, -Infinity)"));
            Assert.AreEqual(-Math.PI / 4, Evaluate("Math.atan2(-Infinity, Infinity)"));
            Assert.AreEqual(-3 * Math.PI / 4, Evaluate("Math.atan2(-Infinity, -Infinity)"));
            Assert.AreEqual(0, Evaluate("Math.atan2(0, Infinity)"));
            Assert.AreEqual(Math.PI, Evaluate("Math.atan2(0, -Infinity)"));
            Assert.AreEqual(Math.PI / 2, Evaluate("Math.atan2(Infinity, 0)"));
            Assert.AreEqual(-Math.PI / 2, Evaluate("Math.atan2(-Infinity, 0)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atan2(NaN, 0)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atan2(0, NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atan2(0, undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atan2('bob', 'bob')"));
        }

        [TestMethod]
        public void ceil()
        {
            Assert.AreEqual(1, Evaluate("Math.ceil(1.0)"));
            Assert.AreEqual(2, Evaluate("Math.ceil(1.2)"));
            Assert.AreEqual(2, Evaluate("Math.ceil(1.6)"));
            Assert.AreEqual(2, Evaluate("Math.ceil(1.5)"));

            Assert.AreEqual(-1, Evaluate("Math.ceil(-1.0)"));
            Assert.AreEqual(-1, Evaluate("Math.ceil(-1.2)"));
            Assert.AreEqual(-1, Evaluate("Math.ceil(-1.6)"));
            Assert.AreEqual(-1, Evaluate("Math.ceil(-1.5)"));

            Assert.AreEqual(0, Evaluate("Math.ceil(0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.ceil(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.ceil(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.ceil(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.ceil(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.ceil('bob')"));
        }

        [TestMethod]
        public void cos()
        {
            Assert.AreEqual(Math.Cos(1), Evaluate("Math.cos(1)"));
            Assert.AreEqual(Math.Cos(-1), Evaluate("Math.cos(-1)"));
            Assert.AreEqual(1, Evaluate("Math.cos(0)"));
            Assert.AreEqual(Math.Cos(0.5), Evaluate("Math.cos(0.5)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.cos(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.cos(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.cos(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.cos(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.cos('bob')"));
        }

        [TestMethod]
        public void exp()
        {
            Assert.AreEqual(Math.E, Evaluate("Math.exp(1)"));
            Assert.AreEqual(Math.Pow(Math.E, 1.5), Evaluate("Math.exp(1.5)"));
            Assert.AreEqual(1 / Math.E, Evaluate("Math.exp(-1)"));
            Assert.AreEqual(1, Evaluate("Math.exp(0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.exp(Infinity)"));
            Assert.AreEqual(0, Evaluate("Math.exp(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.exp(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.exp(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.exp('bob')"));
        }

        [TestMethod]
        public void floor()
        {
            Assert.AreEqual(1, Evaluate("Math.floor(1.0)"));
            Assert.AreEqual(1, Evaluate("Math.floor(1.2)"));
            Assert.AreEqual(1, Evaluate("Math.floor(1.6)"));
            Assert.AreEqual(1, Evaluate("Math.floor(1.5)"));

            Assert.AreEqual(-1, Evaluate("Math.floor(-1.0)"));
            Assert.AreEqual(-2, Evaluate("Math.floor(-1.2)"));
            Assert.AreEqual(-2, Evaluate("Math.floor(-1.6)"));
            Assert.AreEqual(-2, Evaluate("Math.floor(-1.5)"));

            Assert.AreEqual(0, Evaluate("Math.floor(0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.floor(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.floor(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.floor(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.floor(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.floor('bob')"));
        }

        [TestMethod]
        public void log()
        {
            Assert.AreEqual(0, Evaluate("Math.log(1)"));
            Assert.AreEqual(Math.Log(1.5), Evaluate("Math.log(1.5)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log(-1)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.log(0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.log(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log('bob')"));
        }

        [TestMethod]
        public void max()
        {
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.max()"));
            Assert.AreEqual(1, Evaluate("Math.max(1)"));
            Assert.AreEqual(2, Evaluate("Math.max(1, 2)"));
            Assert.AreEqual(3, Evaluate("Math.max(1, 2, 3)"));
            Assert.AreEqual(3, Evaluate("Math.max(3, 2, 1)"));
            Assert.AreEqual(3, Evaluate("Math.max(1, 3, 2)"));
            Assert.AreEqual(3.5, Evaluate("Math.max(1, 3.5, 2)"));
            Assert.AreEqual(3.5, Evaluate("Math.max(1.1, 3.5, 2.1)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.max(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.max(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.max(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.max(1, NaN, 2)"));

            // The spec says call ToNumber() on each parameter - JScript does not do this.
            Assert.AreEqual(double.NaN, Evaluate("Math.max(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.max('bob')"));
            Assert.AreEqual(double.NaN, Evaluate("Math.max('bob', 'dick', 'harry')"));

            // length
            Assert.AreEqual(2, Evaluate("Math.max.length"));
        }

        [TestMethod]
        public void min()
        {
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.min()"));
            Assert.AreEqual(1, Evaluate("Math.min(1)"));
            Assert.AreEqual(1, Evaluate("Math.min(1, 2)"));
            Assert.AreEqual(1, Evaluate("Math.min(1, 2, 3)"));
            Assert.AreEqual(1, Evaluate("Math.min(3, 2, 1)"));
            Assert.AreEqual(1, Evaluate("Math.min(1, 3, 2)"));
            Assert.AreEqual(1, Evaluate("Math.min(1, 3.5, 2)"));
            Assert.AreEqual(1.1, Evaluate("Math.min(1.1, 3.5, 2.1)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.min(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.min(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.min(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.min(1, NaN, 2)"));

            // The spec says call ToNumber() on each parameter - JScript does not do this.
            Assert.AreEqual(double.NaN, Evaluate("Math.min(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.min('bob')"));
            Assert.AreEqual(double.NaN, Evaluate("Math.min('bob', 'dick', 'harry')"));

            // length
            Assert.AreEqual(2, Evaluate("Math.min.length"));
        }

        [TestMethod]
        public void pow()
        {
            Assert.AreEqual(4, Evaluate("Math.pow(2, 2)"));
            Assert.AreEqual(Math.Pow(2.5, 2.5), Evaluate("Math.pow(2.5, 2.5)"));
            Assert.AreEqual(1, Evaluate("Math.pow(0, 0)"));
            Assert.AreEqual(1, Evaluate("Math.pow(1, 0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.pow(0, -1)"));
            Assert.AreEqual(1, Evaluate("Math.pow(-1, 0)"));
            Assert.AreEqual(0, Evaluate("Math.pow(0, 1)"));
            Assert.AreEqual(0, Evaluate("Math.pow(0, Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.pow(1, Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.pow(1, -Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.pow(-1, Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.pow(-1, -Infinity)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.pow(2, Infinity)"));
            Assert.AreEqual(0, Evaluate("Math.pow(2, -Infinity)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.pow(Infinity, Infinity)"));
            Assert.AreEqual(0, Evaluate("Math.pow(Infinity, -Infinity)"));
            Assert.AreEqual(1, Evaluate("Math.pow(Infinity, 0)"));
            Assert.AreEqual(1, Evaluate("Math.pow(-Infinity, 0)"));
            Assert.AreEqual(1, Evaluate("Math.pow(NaN, 0)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.pow(NaN, 1)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.pow(0, NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.pow(0, undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.pow('bob', 'bob')"));
        }

        [TestMethod]
        public void random()
        {
            var randomValue = (double)Evaluate("Math.random()");
            Assert.IsTrue(randomValue >= 0 && randomValue < 1.0);
        }

        [TestMethod]
        public void round()
        {
            Assert.AreEqual(1, Evaluate("Math.round(0.5)"));
            Assert.AreEqual(1, Evaluate("Math.round(1.0)"));
            Assert.AreEqual(1, Evaluate("Math.round(1.2)"));
            Assert.AreEqual(2, Evaluate("Math.round(1.6)"));
            Assert.AreEqual(2, Evaluate("Math.round(1.5)"));
            Assert.AreEqual(3, Evaluate("Math.round(2.5)"));

            Assert.AreEqual(0, Evaluate("Math.round(-0.5)"));
            Assert.AreEqual(-1, Evaluate("Math.round(-1.0)"));
            Assert.AreEqual(-1, Evaluate("Math.round(-1.2)"));
            Assert.AreEqual(-2, Evaluate("Math.round(-1.6)"));
            Assert.AreEqual(-1, Evaluate("Math.round(-1.5)"));
            Assert.AreEqual(-2, Evaluate("Math.round(-2.5)"));

            // -0.1 rounds to -0, 0.1 rounds to +0
            Assert.AreEqual(double.PositiveInfinity, Evaluate("1 / Math.round(0.1)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("1 / Math.round(-0.1)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("1 / Math.round(0)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("1 / Math.round(-0)"));

            Assert.AreEqual(0, Evaluate("Math.round(0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.round(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.round(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.round(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.round(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.round('bob')"));
        }

        [TestMethod]
        public void sin()
        {
            Assert.AreEqual(Math.Sin(1), Evaluate("Math.sin(1)"));
            Assert.AreEqual(Math.Sin(-1), Evaluate("Math.sin(-1)"));
            Assert.AreEqual(0, Evaluate("Math.sin(0)"));
            Assert.AreEqual(Math.Sin(0.5), Evaluate("Math.sin(0.5)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sin(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sin(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sin(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sin(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sin('bob')"));
        }

        [TestMethod]
        public void sqrt()
        {
            Assert.AreEqual(Math.Sqrt(2), Evaluate("Math.sqrt(2)"));
            Assert.AreEqual(Math.Sqrt(2.5), Evaluate("Math.sqrt(2.5)"));
            Assert.AreEqual(1, Evaluate("Math.sqrt(1)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sqrt(-1)"));
            Assert.AreEqual(0, Evaluate("Math.sqrt(0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.sqrt(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sqrt(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sqrt(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sqrt(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sqrt('bob')"));
        }

        [TestMethod]
        public void tan()
        {
            Assert.AreEqual(Math.Tan(1), Evaluate("Math.tan(1)"));
            Assert.AreEqual(Math.Tan(-1), Evaluate("Math.tan(-1)"));
            Assert.AreEqual(0, Evaluate("Math.tan(0)"));
            Assert.AreEqual(Math.Tan(0.5), Evaluate("Math.tan(0.5)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.tan(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.tan(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.tan(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.tan(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.tan('bob')"));
        }

        [TestMethod]
        public void log10()
        {
            Assert.AreEqual(0, Evaluate("Math.log10(1)"));
            Assert.AreEqual(Math.Log10(1.5), Evaluate("Math.log10(1.5)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log10(-1)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.log10(0)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.log10(-0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.log10(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log10(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log10(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log10(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log10('bob')"));
        }

        [TestMethod]
        public void log2()
        {
            Assert.AreEqual(0, Evaluate("Math.log2(1)"));
            Assert.AreEqual(Math.Log(1.5) / Math.Log(2), Evaluate("Math.log2(1.5)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log2(-1)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.log2(0)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.log2(-0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.log2(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log2(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log2(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log2(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log2('bob')"));
        }

        [TestMethod]
        public void log1p()
        {
            Assert.AreEqual(0.00000000999999995, (double)Evaluate("Math.log1p(0.00000001)"), 0.0000000000001);
            Assert.AreEqual(Math.Log(2), Evaluate("Math.log1p(1)"));
            Assert.AreEqual(Math.Log(2.5), Evaluate("Math.log1p(1.5)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.log1p(-1)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.log1p(0), 0)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.log1p(-0), -0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.log1p(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log1p(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log1p(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log1p(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.log1p('bob')"));
        }

        [TestMethod]
        public void expm1()
        {
            Assert.AreEqual(0.00000000999999995, (double)Evaluate("Math.expm1(0.00000001)"), 0.0000000000001);
            Assert.AreEqual(Math.E - 1.0, Evaluate("Math.expm1(1)"));
            Assert.AreEqual(Math.Pow(Math.E, 1.5) - 1.0, Evaluate("Math.expm1(1.5)"));
            Assert.AreEqual(1 / Math.E - 1.0, Evaluate("Math.expm1(-1)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.expm1(0), 0)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.expm1(-0), -0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.expm1(Infinity)"));
            Assert.AreEqual(-1, Evaluate("Math.expm1(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.expm1(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.expm1(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.expm1('bob')"));
        }

        [TestMethod]
        public void cosh()
        {
            Assert.AreEqual(Math.Cosh(1), Evaluate("Math.cosh(1)"));
            Assert.AreEqual(Math.Cosh(-1), Evaluate("Math.cosh(-1)"));
            Assert.AreEqual(1, Evaluate("Math.cosh(0)"));
            Assert.AreEqual(1, Evaluate("Math.cosh(-0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.cosh(Infinity)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.cosh(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.cosh(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.cosh(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.cosh('bob')"));
        }

        [TestMethod]
        public void sinh()
        {
            Assert.AreEqual(Math.Sinh(1), Evaluate("Math.sinh(1)"));
            Assert.AreEqual(Math.Sinh(-1), Evaluate("Math.sinh(-1)"));
            Assert.AreEqual(true, Evaluate("Object.is(0, Math.sinh(0))"));
            Assert.AreEqual(true, Evaluate("Object.is(-0, Math.sinh(-0))"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.sinh(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.sinh(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sinh(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sinh(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sinh('bob')"));
        }

        [TestMethod]
        public void tanh()
        {
            Assert.AreEqual(Math.Tanh(1), Evaluate("Math.tanh(1)"));
            Assert.AreEqual(Math.Tanh(-1), Evaluate("Math.tanh(-1)"));
            Assert.AreEqual(true, Evaluate("Object.is(0, Math.tanh(0))"));
            Assert.AreEqual(true, Evaluate("Object.is(-0, Math.tanh(-0))"));
            Assert.AreEqual(1, Evaluate("Math.tanh(Infinity)"));
            Assert.AreEqual(-1, Evaluate("Math.tanh(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.tanh(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.tanh(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.tanh('bob')"));
        }

        [TestMethod]
        public void acosh()
        {
            Assert.AreEqual(0.96242365011920689499, (double)Evaluate("Math.acosh(1.5)"), 0.000000000001);
            Assert.AreEqual(0, Evaluate("Math.acosh(1)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acosh(-1)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acosh(0)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acosh(-0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.acosh(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acosh(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acosh(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acosh(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.acosh('bob')"));
        }

        [TestMethod]
        public void asinh()
        {
            Assert.AreEqual(0.48121182505960344749775, (double)Evaluate("Math.asinh(0.5)"), 0.000000000001);
            Assert.AreEqual(0.88137358701954302523260932, (double)Evaluate("Math.asinh(1)"), 0.000000000001);
            Assert.AreEqual(-0.88137358701954302523260932, (double)Evaluate("Math.asinh(-1)"), 0.000000000001);
            Assert.AreEqual(true, Evaluate("Object.is(0, Math.asinh(0))"));
            Assert.AreEqual(true, Evaluate("Object.is(-0, Math.asinh(-0))"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.asinh(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.asinh(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.asinh(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.asinh(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.asinh('bob')"));
        }

        [TestMethod]
        public void atanh()
        {
            Assert.AreEqual(0.549306144334054845697, (double)Evaluate("Math.atanh(0.5)"), 0.000000000001);
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.atanh(1)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.atanh(-1)"));
            Assert.AreEqual(true, Evaluate("Object.is(0, Math.atanh(0))"));
            Assert.AreEqual(true, Evaluate("Object.is(-0, Math.atanh(-0))"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atanh(Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atanh(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atanh(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atanh(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.atanh('bob')"));
        }

        [TestMethod]
        public void trunc()
        {
            Assert.AreEqual(1, Evaluate("Math.trunc(1)"));
            Assert.AreEqual(1, Evaluate("Math.trunc(1.3)"));
            Assert.AreEqual(1, Evaluate("Math.trunc(1.5)"));
            Assert.AreEqual(1, Evaluate("Math.trunc(1.7)"));
            Assert.AreEqual(-1, Evaluate("Math.trunc(-1)"));
            Assert.AreEqual(-1, Evaluate("Math.trunc(-1.3)"));
            Assert.AreEqual(-1, Evaluate("Math.trunc(-1.5)"));
            Assert.AreEqual(-1, Evaluate("Math.trunc(-1.7)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.trunc(0), 0)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.trunc(-0), -0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.trunc(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.trunc(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.trunc(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.trunc(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.trunc('bob')"));
        }

        [TestMethod]
        public void sign()
        {
            Assert.AreEqual(1, Evaluate("Math.sign(1)"));
            Assert.AreEqual(1, Evaluate("Math.sign(1.5)"));
            Assert.AreEqual(-1, Evaluate("Math.sign(-1)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.sign(0), 0)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.sign(-0), -0)"));
            Assert.AreEqual(1, Evaluate("Math.sign(Infinity)"));
            Assert.AreEqual(-1, Evaluate("Math.sign(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sign(NaN)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sign(undefined)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.sign('bob')"));
        }

        [TestMethod]
        public void hypot()
        {
            Assert.AreEqual(5, Evaluate("Math.hypot(3, 4)"));
            Assert.AreEqual(Math.Sqrt(50), Evaluate("Math.hypot(3, 4, 5)"));
            Assert.AreEqual(0, Evaluate("Math.hypot()"));
            Assert.AreEqual(double.NaN, Evaluate("Math.hypot(5, NaN)"));
            Assert.AreEqual(1.4142135623730952e+300, Evaluate("Math.hypot(1e300, 1e300)"));
            Assert.AreEqual(1.414213562373095e-300, (double)Evaluate("Math.hypot(1e-300, 1e-300)"), 1e-315);
            Assert.AreEqual(1000.0000000005, Evaluate("Math.hypot(1e3, 1e-3)"));
        }

        [TestMethod]
        public void imul()
        {
            Assert.AreEqual(8, Evaluate("Math.imul(2, 4)"));
            Assert.AreEqual(-8, Evaluate("Math.imul(-1, 8)"));
            Assert.AreEqual(4, Evaluate("Math.imul(-2, -2)"));
            Assert.AreEqual(-5, Evaluate("Math.imul(0xffffffff, 5)"));
            Assert.AreEqual(-10, Evaluate("Math.imul(0xfffffffe, 5)"));
            Assert.AreEqual(0, Evaluate("Math.imul(NaN, 5)"));
        }

        [TestMethod]
        public void fround()
        {
            Assert.AreEqual(1, Evaluate("Math.fround(1.0000000001)"));
            Assert.AreEqual(1.25, Evaluate("Math.fround(1.25)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.fround(0), 0)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.fround(-0), -0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.fround(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.fround(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.fround(NaN)"));
        }

        [TestMethod]
        public void clz32()
        {
            Assert.AreEqual(32, Evaluate("Math.clz32(0)"));
            Assert.AreEqual(31, Evaluate("Math.clz32(1)"));
            Assert.AreEqual(5, Evaluate("Math.clz32(123456789)"));
            Assert.AreEqual(1, Evaluate("Math.clz32(1234567890123456789)"));
            Assert.AreEqual(0, Evaluate("Math.clz32(-1)"));
            Assert.AreEqual(29, Evaluate("Math.clz32(5.8)"));
            Assert.AreEqual(32, Evaluate("Math.clz32(Infinity)"));
            Assert.AreEqual(32, Evaluate("Math.clz32(-Infinity)"));
            Assert.AreEqual(32, Evaluate("Math.clz32(NaN)"));
        }

        [TestMethod]
        public void cbrt()
        {
            Assert.AreEqual(0, Evaluate("Math.cbrt(0)"));
            Assert.AreEqual(1, Evaluate("Math.cbrt(1)"));
            Assert.AreEqual(3, Evaluate("Math.cbrt(27)"));
            Assert.AreEqual(-3, Evaluate("Math.cbrt(-27)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.cbrt(0), 0)"));
            Assert.AreEqual(true, Evaluate("Object.is(Math.cbrt(-0), -0)"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("Math.cbrt(Infinity)"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("Math.cbrt(-Infinity)"));
            Assert.AreEqual(double.NaN, Evaluate("Math.cbrt(NaN)"));
        }
    }
}
