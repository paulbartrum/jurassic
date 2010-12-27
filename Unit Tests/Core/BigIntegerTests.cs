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
    /// Test the BigInteger struct.
    /// </summary>
    [TestClass]
    public class BigIntegerTests
    {
        string[] significantNumbers =
        {
            "0",
            "1",
            "-1",
            "100",
            "-100",
            "429496729",
            "4294967295",
            "-4294967295",
            "4294967296",
            "-4294967296",
            "10000000000",
            "-10000000000",
            "123456789012345678901",
            "-123456789012345678901",
        };

        [TestMethod]
        public void Add()
        {
            for (int i = 0; i < significantNumbers.Length; i ++)
                for (int j = 0; j < significantNumbers.Length; j++)
                {
                    var expected = System.Numerics.BigInteger.Parse(significantNumbers[i]) + System.Numerics.BigInteger.Parse(significantNumbers[j]);
                    var actual = BigInteger.Add(BigInteger.Parse(significantNumbers[i]), BigInteger.Parse(significantNumbers[j]));
                    Assert.AreEqual(expected, Convert(actual), string.Format("Computing {0} + {1}", significantNumbers[i], significantNumbers[j]));
                }
        }

        [TestMethod]
        public void Subtract()
        {
            for (int i = 0; i < significantNumbers.Length; i++)
                for (int j = 0; j < significantNumbers.Length; j++)
                {
                    var expected = System.Numerics.BigInteger.Parse(significantNumbers[i]) - System.Numerics.BigInteger.Parse(significantNumbers[j]);
                    var actual = BigInteger.Subtract(BigInteger.Parse(significantNumbers[i]), BigInteger.Parse(significantNumbers[j]));
                    Assert.AreEqual(expected, Convert(actual), string.Format("Computing {0} - {1}", significantNumbers[i], significantNumbers[j]));
                }
        }

        [TestMethod]
        public void Multiply()
        {
            for (int i = 0; i < significantNumbers.Length; i++)
                for (int j = 0; j < significantNumbers.Length; j++)
                {
                    var expected = System.Numerics.BigInteger.Parse(significantNumbers[i]) * System.Numerics.BigInteger.Parse(significantNumbers[j]);
                    var actual = BigInteger.Multiply(BigInteger.Parse(significantNumbers[i]), BigInteger.Parse(significantNumbers[j]));
                    Assert.AreEqual(expected, Convert(actual), string.Format("Computing {0} * {1}", significantNumbers[i], significantNumbers[j]));
                }
        }

        [TestMethod]
        public void LessThan()
        {
            for (int i = 0; i < significantNumbers.Length; i++)
                for (int j = 0; j < significantNumbers.Length; j++)
                {
                    var expected = System.Numerics.BigInteger.Parse(significantNumbers[i]) < System.Numerics.BigInteger.Parse(significantNumbers[j]);
                    var actual = BigInteger.Compare(BigInteger.Parse(significantNumbers[i]), BigInteger.Parse(significantNumbers[j])) < 0;
                    Assert.AreEqual(expected, actual, string.Format("Computing {0} < {1}", significantNumbers[i], significantNumbers[j]));
                }
        }

        [TestMethod]
        public void LessThanOrEqual()
        {
            for (int i = 0; i < significantNumbers.Length; i++)
                for (int j = 0; j < significantNumbers.Length; j++)
                {
                    var expected = System.Numerics.BigInteger.Parse(significantNumbers[i]) <= System.Numerics.BigInteger.Parse(significantNumbers[j]);
                    var actual = BigInteger.Compare(BigInteger.Parse(significantNumbers[i]), BigInteger.Parse(significantNumbers[j])) <= 0;
                    Assert.AreEqual(expected, actual, string.Format("Computing {0} <= {1}", significantNumbers[i], significantNumbers[j]));
                }
        }

        [TestMethod]
        public void LeftShift()
        {
            for (int i = 0; i < significantNumbers.Length; i++)
                for (int j = 0; j < 100; j += 19)
                {
                    var expected = System.Numerics.BigInteger.Parse(significantNumbers[i]) << j;
                    var actual = BigInteger.LeftShift(Jurassic.BigInteger.Parse(significantNumbers[i]), j);
                    Assert.AreEqual(expected, Convert(actual), string.Format("Computing {0} << {1}", significantNumbers[i], j));
                }
        }

        [TestMethod]
        public void RightShift()
        {
            for (int i = 0; i < significantNumbers.Length; i++)
                for (int j = 0; j < 100; j += 19)
                {
                    var expected = System.Numerics.BigInteger.Parse(significantNumbers[i]);
                    if (expected.Sign > 0)
                        expected = expected >> j;
                    else
                        expected = System.Numerics.BigInteger.Negate(System.Numerics.BigInteger.Negate(expected) >> j);
                    var actual = BigInteger.RightShift(BigInteger.Parse(significantNumbers[i]), j);
                    Assert.AreEqual(expected, Convert(actual), string.Format("Computing {0} >> {1}", significantNumbers[i], j));
                }
        }

        [TestMethod]
        public void Pow()
        {
            int[] radixValues = new int[] { 2, 3, 10 };
            for (int i = 0; i < radixValues.Length; i++)
                for (int j = 0; j < 50; j ++)
                {
                    var expected = System.Numerics.BigInteger.Pow(radixValues[i], j);
                    var actual = Jurassic.BigInteger.Pow(radixValues[i], j);
                    Assert.AreEqual(expected, Convert(actual), string.Format("Computing Pow({0}, {1})", radixValues[i], j));
                }
        }

        [TestMethod]
        public new void ToString()
        {
            Jurassic.BigInteger.Parse("4294967295").ToString();
            for (int i = 0; i < significantNumbers.Length; i++)
            {
                var expected = System.Numerics.BigInteger.Parse(significantNumbers[i]).ToString();
                var actual = Jurassic.BigInteger.Parse(significantNumbers[i]).ToString();
                Assert.AreEqual(expected, actual, string.Format("Computing ToString({0})", significantNumbers[i]));
            }
        }

        [TestMethod]
        public void Abs()
        {
            for (int i = 0; i < significantNumbers.Length; i++)
            {
                var expected = System.Numerics.BigInteger.Abs(System.Numerics.BigInteger.Parse(significantNumbers[i]));
                var actual = Jurassic.BigInteger.Abs(Jurassic.BigInteger.Parse(significantNumbers[i]));
                Assert.AreEqual(expected, Convert(actual), string.Format("Computing Abs({0})", significantNumbers[i]));
            }
        }

        [TestMethod]
        public void Log()
        {
            int[] baseValues = new int[] { 2, 3, 10 };
            for (int i = 0; i < significantNumbers.Length; i++)
            {
                for (int j = 0; j < baseValues.Length; j++)
                {
                    var expected = System.Numerics.BigInteger.Log(System.Numerics.BigInteger.Parse(significantNumbers[i]), baseValues[j]);
                    var actual = BigInteger.Log(Jurassic.BigInteger.Parse(significantNumbers[i]), baseValues[j]);
                    Assert.AreEqual(expected, actual, string.Format("Computing Log({0}, {1})", significantNumbers[i], baseValues[j]));
                }
            }
        }

        [TestMethod]
        public void FromDouble()
        {
            for (int i = 0; i < significantNumbers.Length; i++)
            {
                var expected = new System.Numerics.BigInteger(double.Parse(significantNumbers[i]));
                var actual = BigInteger.FromDouble(double.Parse(significantNumbers[i]));
                Assert.AreEqual(expected, Convert(actual), string.Format("Computing FromDouble({0})", significantNumbers[i]));
            }
        }

        [TestMethod]
        public void ToDouble()
        {
            for (int i = 0; i < significantNumbers.Length; i++)
            {
                var expected = (double)System.Numerics.BigInteger.Parse(significantNumbers[i]);
                var actual = Jurassic.BigInteger.Parse(significantNumbers[i]).ToDouble();
                Assert.AreEqual(expected, actual, string.Format("Computing ToDouble({0})", significantNumbers[i]));
            }
        }

        // Helper method.
        private System.Numerics.BigInteger Convert(Jurassic.BigInteger value)
        {
            var result = System.Numerics.BigInteger.Zero;
            for (int i = value.WordCount - 1; i >= 0; i --)
            {
                result <<= 32;
                result += value.Words[i];
            }
            if (value.Sign == -1)
                result = result * -1;
            return result;
        }
    }
}
