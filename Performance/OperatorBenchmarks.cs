using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Performance
{

    /// <summary>
    /// Benchmarks the javascript operators.
    /// </summary>
    [TestClass]
    public class OperatorBenchmarks
    {

        [TestMethod]
        public void BitwiseAnd()
        {
            // 32 with no optimizations.
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x & i
                ", 24.3);
        }

        [TestMethod]
        public void BitwiseOr()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x | i
                ", 1400);
        }

        [TestMethod]
        public void BitwiseXor()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x ^ i
                ", 1400);
        }

        [TestMethod]
        public void Add()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x + i
                ", 1400);
        }

        [TestMethod]
        public void Subtract()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x - i
                ", 1400);
        }

        [TestMethod]
        public void Multiply()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x * i
                ", 1400);
        }

        [TestMethod]
        public void Divide()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 1; i < 600000; i++)
                    x = x / i
                ", 1400);
        }

        [TestMethod]
        public void Modulo()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 1; i < 600000; i++)
                    x = x % i
                ", 50);
        }

        [TestMethod]
        public void LeftShift()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x << i
                ", 1380);
        }

        [TestMethod]
        public void SignedRightShift()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x >> i
                ", 1380);
        }

        [TestMethod]
        public void UnsignedRightShift()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x >>> i
                ", 1380);
        }

        [TestMethod]
        public void LessThan()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x < i
                ", 18.5);
        }

        [TestMethod]
        public void LessThanOrEquals()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x <= i
                ", 18.5);
        }

        [TestMethod]
        public void GreaterThan()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x > i
                ", 18.5);
        }

        [TestMethod]
        public void GreaterThanOrEquals()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x >= i
                ", 18.5);
        }

        [TestMethod]
        public void Equals()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x == i
                ", 16);
        }

        [TestMethod]
        public void StrictEquals()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x === i
                ", 25);
        }

        [TestMethod]
        public void NotEquals()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x != i
                ", 18);
        }

        [TestMethod]
        public void StrictNotEquals()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x !== i
                ", 25);
        }

        [TestMethod]
        public void LogicalAnd()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x && i
                ", 36.6);
        }

        [TestMethod]
        public void LogicalOr()
        {
            TestUtils.Benchmark(@"
                x = 0;
                for (var i = 0; i < 600000; i++)
                    x = x || i
                ", 31.9);
        }
    }

}