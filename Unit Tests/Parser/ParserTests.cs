using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Compiler;

namespace UnitTests
{
    /// <summary>
    /// Test the Parser object.
    /// </summary>
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void UnaryPlus()
        {
            Assert.AreEqual(+20, TestUtils.Evaluate("+20"));
            Assert.AreEqual(5, TestUtils.Evaluate("+ '5'"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("+'Hello'"));
            Assert.AreEqual(1, TestUtils.Evaluate("+true"));
        }

        [TestMethod]
        public void UnaryMinus()
        {
            Assert.AreEqual(-20, TestUtils.Evaluate("-20"));
            Assert.AreEqual(-5, TestUtils.Evaluate("- '5'"));
            Assert.AreEqual(-1, TestUtils.Evaluate("-true"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("-'Hello'"));
        }

        [TestMethod]
        public void BitwiseNot()
        {
            Assert.AreEqual(-1, TestUtils.Evaluate("~0"));
            Assert.AreEqual(-21, TestUtils.Evaluate("~20"));
            Assert.AreEqual(19, TestUtils.Evaluate("~-20"));
            Assert.AreEqual(-9, TestUtils.Evaluate("~4294967304"));
            Assert.AreEqual(-21, TestUtils.Evaluate("~ '20'"));

            // Double bitwise not converts the input to a Int32.
            Assert.AreEqual(1, TestUtils.Evaluate("~~'1.2'"));
            Assert.AreEqual(-1, TestUtils.Evaluate("~~'-1.2'"));
            Assert.AreEqual(32, TestUtils.Evaluate("~~17179869216"));
            Assert.AreEqual(-2147483638, TestUtils.Evaluate("~~2147483658"));
            Assert.AreEqual(-2147483637, TestUtils.Evaluate("~~6442450955"));
        }

        [TestMethod]
        public void LogicalNot()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("!false"));
            Assert.AreEqual(false, TestUtils.Evaluate("!true"));
            Assert.AreEqual(false, TestUtils.Evaluate("!10"));
            Assert.AreEqual(true, TestUtils.Evaluate("!0"));
            Assert.AreEqual(false, TestUtils.Evaluate("!'hello'"));
            Assert.AreEqual(true, TestUtils.Evaluate("!''"));
        }

        [TestMethod]
        public void Add()
        {
            Assert.AreEqual(35, TestUtils.Evaluate("15 + 20"));
            Assert.AreEqual(21.5, TestUtils.Evaluate("1.5 + 20"));
            Assert.AreEqual(8589934608.0, TestUtils.Evaluate("4294967304 + 4294967304"));
            Assert.AreEqual("testing", TestUtils.Evaluate("'tes' + 'ting'"));
            Assert.AreEqual(1, TestUtils.Evaluate("true + false"));
            Assert.AreEqual("102", TestUtils.Evaluate("'10' + 2"));
            Assert.AreEqual("10null", TestUtils.Evaluate("'10' + null"));
            Assert.AreEqual("51,2,3", TestUtils.Evaluate("5 + [1,2,3]"));
            Assert.AreEqual("5Thu Jan 01 1970 13:00:00 GMT+1300 (New Zealand Daylight Time)", TestUtils.Evaluate("5 + new Date(5)"));
            Assert.AreEqual("5/abc/g", TestUtils.Evaluate("5 + /abc/g"));
            Assert.AreEqual("5[object Object]", TestUtils.Evaluate("5 + {}"));

            
        }

        [TestMethod]
        public void Subtract()
        {
            Assert.AreEqual(-5,            TestUtils.Evaluate("15 - 20"));
            Assert.AreEqual(-18.5,         TestUtils.Evaluate("1.5 - 20"));
            Assert.AreEqual(-4294967304.0, TestUtils.Evaluate("4294967304 - 8589934608"));
            Assert.AreEqual(double.NaN,    TestUtils.Evaluate("'tes' - 'ting'"));
            Assert.AreEqual(1,             TestUtils.Evaluate("true - false"));
            Assert.AreEqual(8,             TestUtils.Evaluate("'10' - 2"));
            Assert.AreEqual(10,            TestUtils.Evaluate("'10' - null"));
            Assert.AreEqual(-6,            TestUtils.Evaluate("6 - 6 - 6"));
        }

        [TestMethod]
        public void Multiply()
        {
            Assert.AreEqual(300, TestUtils.Evaluate("15 * 20"));
            Assert.AreEqual(30, TestUtils.Evaluate("1.5 * 20"));
            Assert.AreEqual(8589934608.0, TestUtils.Evaluate("4294967304 * 2"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("'tes' * 'ting'"));
            Assert.AreEqual(1, TestUtils.Evaluate("true * true"));
            Assert.AreEqual(20, TestUtils.Evaluate("'10' * 2"));
            Assert.AreEqual(0, TestUtils.Evaluate("'10' * null"));
        }

        [TestMethod]
        public void Divide()
        {
            Assert.AreEqual(0.75, TestUtils.Evaluate("15 / 20"));
            Assert.AreEqual(0.075, TestUtils.Evaluate("1.5 / 20"));
            Assert.AreEqual(2147483652.0, TestUtils.Evaluate("4294967304 / 2"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("'tes' / 'ting'"));
            Assert.AreEqual(1, TestUtils.Evaluate("true / true"));
            Assert.AreEqual(5, TestUtils.Evaluate("'10' / 2"));
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("'10' / null"));
            Assert.AreEqual(double.NegativeInfinity, TestUtils.Evaluate("'-10' / null"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("0 / 0"));
        }

        [TestMethod]
        public void Remainder()
        {
            Assert.AreEqual(2, TestUtils.Evaluate("17 % 5"));
            Assert.AreEqual(2, TestUtils.Evaluate("17 % -5"));
            Assert.AreEqual(-2, TestUtils.Evaluate("-17 % 5"));
            Assert.AreEqual(-2, TestUtils.Evaluate("-17 % -5"));
            Assert.AreEqual(2.2, (double)TestUtils.Evaluate("17.2 % 5"), 000000000000001);
            Assert.AreEqual(2.8, (double)TestUtils.Evaluate("17.8 % 5"), 000000000000001);
            Assert.AreEqual(-2.2, (double)TestUtils.Evaluate("-17.2 % 5"), 000000000000001);
            Assert.AreEqual(-2.8, (double)TestUtils.Evaluate("-17.8 % 5"), 000000000000001);
        }

        [TestMethod]
        public void LeftShift()
        {
            Assert.AreEqual(40, TestUtils.Evaluate("10 << 2"));
            Assert.AreEqual(-400, TestUtils.Evaluate("-100 << 2"));
            Assert.AreEqual(20, TestUtils.Evaluate("10 << 1.2"));
            Assert.AreEqual(20, TestUtils.Evaluate("10 << 1.8"));
            Assert.AreEqual(16, TestUtils.Evaluate("4294967304 << 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("8 << -2"));
        }

        [TestMethod]
        public void SignedRightShift()
        {
            Assert.AreEqual(2, TestUtils.Evaluate("10 >> 2"));
            Assert.AreEqual(-25, TestUtils.Evaluate("-100 >> 2"));
            Assert.AreEqual(5, TestUtils.Evaluate("10 >> 1.2"));
            Assert.AreEqual(5, TestUtils.Evaluate("10 >> 1.8"));
            Assert.AreEqual(4, TestUtils.Evaluate("4294967304 >> 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("8 >> -2"));

            // Signed right shift by zero converts the input to a Int32.
            Assert.AreEqual(32, TestUtils.Evaluate("17179869216 >> 0"));
            Assert.AreEqual(-2147483638, TestUtils.Evaluate("2147483658 >> 0"));
            Assert.AreEqual(-2147483637, TestUtils.Evaluate("6442450955 >> 0"));
        }

        [TestMethod]
        public void UnsignedRightShift()
        {
            Assert.AreEqual(2, TestUtils.Evaluate("10 >>> 2"));
            Assert.AreEqual(1073741799, TestUtils.Evaluate("-100 >>> 2"));
            Assert.AreEqual(5, TestUtils.Evaluate("10 >>> 1.2"));
            Assert.AreEqual(5, TestUtils.Evaluate("10 >>> 1.8"));
            Assert.AreEqual(4, TestUtils.Evaluate("4294967304 >>> 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("8 >>> -2"));

            // Unsigned right shift by zero converts the input to a Uint32.
            Assert.AreEqual(32, TestUtils.Evaluate("17179869216 >>> 0"));
            Assert.AreEqual(2147483658.0, TestUtils.Evaluate("2147483658 >>> 0"));
            Assert.AreEqual(2147483659.0, TestUtils.Evaluate("6442450955 >>> 0"));
        }

        [TestMethod]
        public void Void()
        {
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("void true"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("void 2"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("void 'test'"));

            // Make sure side-effects are still evaluated.
            Assert.AreEqual("abc", TestUtils.Evaluate("void (x = 'abc'); x"));
        }

        [TestMethod]
        public void Equals()
        {
            // Both operands are of the same type.
            Assert.AreEqual(false, TestUtils.Evaluate("false == true"));
            Assert.AreEqual(true, TestUtils.Evaluate("false == false"));
            Assert.AreEqual(true, TestUtils.Evaluate("10 == 10"));
            Assert.AreEqual(false, TestUtils.Evaluate("10 == 11"));
            Assert.AreEqual(true, TestUtils.Evaluate("'test' == 'test'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'test' == 'TEST'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'' == ''"));

            // Operands are different types.
            Assert.AreEqual(false, TestUtils.Evaluate("true == 2"));
            Assert.AreEqual(true, TestUtils.Evaluate("true == 1"));
            Assert.AreEqual(false, TestUtils.Evaluate("2 == true"));
            Assert.AreEqual(true, TestUtils.Evaluate("1 == true"));
            Assert.AreEqual(true, TestUtils.Evaluate("1 == '1'"));
            Assert.AreEqual(false, TestUtils.Evaluate("1 == '0'"));

            // Undefined and null.
            Assert.AreEqual(true, TestUtils.Evaluate("null == null"));
            Assert.AreEqual(true, TestUtils.Evaluate("undefined == undefined"));
            Assert.AreEqual(true, TestUtils.Evaluate("null == undefined"));
            Assert.AreEqual(true, TestUtils.Evaluate("Math.abcdef == Math.abcdefghi"));
            Assert.AreEqual(true, TestUtils.Evaluate("Math.abcdef == undefined"));
            Assert.AreEqual(false, TestUtils.Evaluate("null == 5"));

            // NaN
            Assert.AreEqual(false, TestUtils.Evaluate("NaN == NaN"));
        }

        [TestMethod]
        public void NotEquals()
        {
            // Both operands are of the same type.
            Assert.AreEqual(true, TestUtils.Evaluate("false != true"));
            Assert.AreEqual(false, TestUtils.Evaluate("false != false"));
            Assert.AreEqual(false, TestUtils.Evaluate("10 != 10"));
            Assert.AreEqual(true, TestUtils.Evaluate("10 != 11"));
            Assert.AreEqual(false, TestUtils.Evaluate("'test' != 'test'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'test' != 'TEST'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'' != ''"));

            // Operands are different types.
            Assert.AreEqual(true, TestUtils.Evaluate("true != 2"));
            Assert.AreEqual(false, TestUtils.Evaluate("true != 1"));
            Assert.AreEqual(true, TestUtils.Evaluate("2 != true"));
            Assert.AreEqual(false, TestUtils.Evaluate("1 != true"));
            Assert.AreEqual(false, TestUtils.Evaluate("1 != '1'"));
            Assert.AreEqual(true, TestUtils.Evaluate("1 != '0'"));

            // Undefined and null.
            Assert.AreEqual(false, TestUtils.Evaluate("null != null"));
            Assert.AreEqual(false, TestUtils.Evaluate("undefined != undefined"));
            Assert.AreEqual(false, TestUtils.Evaluate("null != undefined"));
            Assert.AreEqual(false, TestUtils.Evaluate("Math.abcdef != Math.abcdefghi"));
            Assert.AreEqual(false, TestUtils.Evaluate("Math.abcdef != undefined"));
            Assert.AreEqual(true, TestUtils.Evaluate("null != 5"));

            // NaN
            Assert.AreEqual(true, TestUtils.Evaluate("NaN != NaN"));
        }

        [TestMethod]
        public void StrictEquals()
        {
            // Both operands are of the same type.
            Assert.AreEqual(false, TestUtils.Evaluate("false === true"));
            Assert.AreEqual(true, TestUtils.Evaluate("false === false"));
            Assert.AreEqual(true, TestUtils.Evaluate("10 === 10"));
            Assert.AreEqual(false, TestUtils.Evaluate("10 === 11"));
            Assert.AreEqual(true, TestUtils.Evaluate("'test' === 'test'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'test' === 'TEST'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'' === ''"));

            // Operands are different types.
            Assert.AreEqual(false, TestUtils.Evaluate("true === 2"));
            Assert.AreEqual(false, TestUtils.Evaluate("true === 1"));
            Assert.AreEqual(false, TestUtils.Evaluate("2 === true"));
            Assert.AreEqual(false, TestUtils.Evaluate("1 === true"));
            Assert.AreEqual(false, TestUtils.Evaluate("1 === '1'"));
            Assert.AreEqual(false, TestUtils.Evaluate("1 === '0'"));

            // Undefined and null.
            Assert.AreEqual(true, TestUtils.Evaluate("null === null"));
            Assert.AreEqual(true, TestUtils.Evaluate("undefined === undefined"));
            Assert.AreEqual(false, TestUtils.Evaluate("null === undefined"));
            Assert.AreEqual(true, TestUtils.Evaluate("Math.abcdef === Math.abcdefghi"));
            Assert.AreEqual(true, TestUtils.Evaluate("Math.abcdef === undefined"));
            Assert.AreEqual(false, TestUtils.Evaluate("null === 5"));

            // NaN
            Assert.AreEqual(false, TestUtils.Evaluate("NaN === NaN"));
        }

        [TestMethod]
        public void StrictNotEquals()
        {
            // Both operands are of the same type.
            Assert.AreEqual(true, TestUtils.Evaluate("false !== true"));
            Assert.AreEqual(false, TestUtils.Evaluate("false !== false"));
            Assert.AreEqual(false, TestUtils.Evaluate("10 !== 10"));
            Assert.AreEqual(true, TestUtils.Evaluate("10 !== 11"));
            Assert.AreEqual(false, TestUtils.Evaluate("'test' !== 'test'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'test' !== 'TEST'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'' !== ''"));

            // Operands are different types.
            Assert.AreEqual(true, TestUtils.Evaluate("true !== 2"));
            Assert.AreEqual(true, TestUtils.Evaluate("true !== 1"));
            Assert.AreEqual(true, TestUtils.Evaluate("2 !== true"));
            Assert.AreEqual(true, TestUtils.Evaluate("1 !== true"));
            Assert.AreEqual(true, TestUtils.Evaluate("1 !== '1'"));
            Assert.AreEqual(true, TestUtils.Evaluate("1 !== '0'"));

            // Undefined and null.
            Assert.AreEqual(false, TestUtils.Evaluate("null !== null"));
            Assert.AreEqual(false, TestUtils.Evaluate("undefined !== undefined"));
            Assert.AreEqual(true, TestUtils.Evaluate("null !== undefined"));
            Assert.AreEqual(false, TestUtils.Evaluate("Math.abcdef !== Math.abcdefghi"));
            Assert.AreEqual(false, TestUtils.Evaluate("Math.abcdef !== undefined"));
            Assert.AreEqual(true, TestUtils.Evaluate("null !== 5"));

            // NaN
            Assert.AreEqual(true, TestUtils.Evaluate("NaN !== NaN"));
        }

        [TestMethod]
        public void LessThan()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("7 < 5"));
            Assert.AreEqual(true, TestUtils.Evaluate("5 < 7"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 < 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 < NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("NaN < NaN"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' < 'b'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' < 'a'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' < 'A'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'2' < '15'"));
            Assert.AreEqual(true, TestUtils.Evaluate("2 < '15'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'15' < 2"));
            Assert.AreEqual(true, TestUtils.Evaluate("false < true"));
        }

        [TestMethod]
        public void LessThanOrEqual()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("7 <= 5"));
            Assert.AreEqual(true, TestUtils.Evaluate("5 <= 7"));
            Assert.AreEqual(true, TestUtils.Evaluate("5 <= 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 <= NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("NaN <= NaN"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' <= 'b'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' <= 'a'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' <= 'A'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'2' <= '15'"));
            Assert.AreEqual(true, TestUtils.Evaluate("2 <= '15'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'15' <= 2"));
            Assert.AreEqual(true, TestUtils.Evaluate("false <= true"));
        }

        [TestMethod]
        public void GreaterThan()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("7 > 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 > 7"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 > 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 > NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("NaN > NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' > 'b'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' > 'a'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' > 'A'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'2' > '15'"));
            Assert.AreEqual(false, TestUtils.Evaluate("2 > '15'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'15' > 2"));
            Assert.AreEqual(false, TestUtils.Evaluate("false > true"));
        }

        [TestMethod]
        public void GreaterThanOrEqual()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("7 >= 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 >= 7"));
            Assert.AreEqual(true, TestUtils.Evaluate("5 >= 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 >= NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("NaN >= NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' >= 'b'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' >= 'a'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' >= 'A'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'2' >= '15'"));
            Assert.AreEqual(false, TestUtils.Evaluate("2 >= '15'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'15' >= 2"));
            Assert.AreEqual(false, TestUtils.Evaluate("false >= true"));
        }

        [TestMethod]
        public void BitwiseAnd()
        {
            Assert.AreEqual(3, TestUtils.Evaluate("11 & 7"));
            Assert.AreEqual(9, TestUtils.Evaluate("11 & -7"));
            Assert.AreEqual(8, TestUtils.Evaluate("4294967304 & 255"));
            Assert.AreEqual(1, TestUtils.Evaluate("11.5 & 1.5"));
        }

        [TestMethod]
        public void BitwiseXor()
        {
            Assert.AreEqual(12, TestUtils.Evaluate("11 ^ 7"));
            Assert.AreEqual(-14, TestUtils.Evaluate("11 ^ -7"));
            Assert.AreEqual(247, TestUtils.Evaluate("4294967304 ^ 255"));
            Assert.AreEqual(10, TestUtils.Evaluate("11.5 ^ 1.5"));
        }

        [TestMethod]
        public void BitwiseOr()
        {
            Assert.AreEqual(15, TestUtils.Evaluate("11 | 7"));
            Assert.AreEqual(-5, TestUtils.Evaluate("11 | -7"));
            Assert.AreEqual(255, TestUtils.Evaluate("4294967304 | 255"));
            Assert.AreEqual(11, TestUtils.Evaluate("11.5 | 1.5"));
        }

        [TestMethod]
        public void LogicalAnd()
        {
            // Boolean arguments.
            Assert.AreEqual(false, TestUtils.Evaluate("false && false"));
            Assert.AreEqual(false, TestUtils.Evaluate("false && true"));
            Assert.AreEqual(false, TestUtils.Evaluate("true && false"));
            Assert.AreEqual(true, TestUtils.Evaluate("true && true"));

            // Numeric arguments.
            Assert.AreEqual(0, TestUtils.Evaluate("0 && 7"));
            Assert.AreEqual(0, TestUtils.Evaluate("11 && 0"));
            Assert.AreEqual(7, TestUtils.Evaluate("11 && 7"));

            // Mixed.
            Assert.AreEqual(true, TestUtils.Evaluate("11 && true"));
            Assert.AreEqual(11, TestUtils.Evaluate("true && 11"));
            Assert.AreEqual(false, TestUtils.Evaluate("false && 11"));
        }

        [TestMethod]
        public void LogicalOr()
        {
            // Boolean arguments.
            Assert.AreEqual(false, TestUtils.Evaluate("false || false"));
            Assert.AreEqual(true, TestUtils.Evaluate("false || true"));
            Assert.AreEqual(true, TestUtils.Evaluate("true || false"));
            Assert.AreEqual(true, TestUtils.Evaluate("true || true"));

            // Numeric arguments.
            Assert.AreEqual(7, TestUtils.Evaluate("0 || 7"));
            Assert.AreEqual(11, TestUtils.Evaluate("11 || 0"));
            Assert.AreEqual(11, TestUtils.Evaluate("11 || 7"));

            // Mixed.
            Assert.AreEqual(11, TestUtils.Evaluate("11 || true"));
            Assert.AreEqual(true, TestUtils.Evaluate("true || 11"));
            Assert.AreEqual(11, TestUtils.Evaluate("false || 11"));
        }

        [TestMethod]
        public void Comma()
        {
            Assert.AreEqual(2, TestUtils.Evaluate("1, 2"));
            Assert.AreEqual("aliens", TestUtils.Evaluate("1, 'aliens'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'go', true"));
        }

        [TestMethod]
        public void Conditional()
        {
            Assert.AreEqual(8, TestUtils.Evaluate("true ? 8 : 'test'"));
            Assert.AreEqual("hello", TestUtils.Evaluate("true ? 'hello' : 'nope'"));
            Assert.AreEqual(7, TestUtils.Evaluate("5 ? 7 : 8"));
            Assert.AreEqual(8, TestUtils.Evaluate("0 ? 7 : 8"));
            Assert.AreEqual(true, TestUtils.Evaluate("true ? true : false ? 8 : 9"));
            Assert.AreEqual(3, TestUtils.Evaluate("1 ? 2 ? 3 : 4 : 5"));
            Assert.AreEqual(4, TestUtils.Evaluate("1 ? 0 ? 3 : 4 : 5"));
            Assert.AreEqual(5, TestUtils.Evaluate("0 ? 1 ? 3 : 4 : 5"));

            // Test the precedence at the start of the conditional.
            Assert.AreEqual(1, TestUtils.Evaluate("x = 5; x + 0 ? 1 : 2"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 5; x || 0 ? 1 : 2"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 5; x = 0 ? 1 : 2"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 5; x, 0 ? 1 : 2"));

            //// Test the precedence in the middle of the conditional.
            Assert.AreEqual(1, TestUtils.Evaluate("x = 5; true ? x = 1 : 2"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = 5; true ? 1, x : 2"));

            // Test the precedence at the end of the conditional.
            Assert.AreEqual(1, TestUtils.Evaluate("x = 4; true ? 1 : x = 2"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 4; true ? 1 : x += 2"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 3; true ? 1 : x, 2"));
        }

        [TestMethod]
        public void Assignment()
        {
            // Numeric operations.
            Assert.AreEqual(4, TestUtils.Evaluate("x = 4"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 4; x += 2"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 4; x -= 2"));
            Assert.AreEqual(8, TestUtils.Evaluate("x = 4; x *= 2"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 4; x /= 2"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 4; x %= 3"));
            Assert.AreEqual(8, TestUtils.Evaluate("x = 4; x <<= 1"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 4; x >>= 1"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 4; x >>>= 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 4; x &= 1"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 4; x |= 1"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 4; x ^= 1"));

            // String operations.
            Assert.AreEqual("hah", TestUtils.Evaluate("x = 'hah'"));
            Assert.AreEqual("hah2", TestUtils.Evaluate("x = 'hah'; x += 2"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'hah'; x -= 2"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'hah'; x *= 2"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'hah'; x /= 2"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'hah'; x %= 3"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 'hah'; x <<= 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 'hah'; x >>= 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 'hah'; x >>>= 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 'hah'; x &= 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 'hah'; x |= 1"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 'hah'; x ^= 1"));
        }

        [TestMethod]
        public void PreIncrement()
        {
            Assert.AreEqual(1, TestUtils.Evaluate("x = 0; ++ x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 0; ++ x; x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("++ 2"));
        }

        [TestMethod]
        public void PreDecrement()
        {
            Assert.AreEqual(-1, TestUtils.Evaluate("x = 0; -- x"));
            Assert.AreEqual(-1, TestUtils.Evaluate("x = 0; -- x; x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("-- 2"));
        }

        [TestMethod]
        public void PostIncrement()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; x ++"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 0; x ++; x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("2 ++"));
        }

        [TestMethod]
        public void PostDecrement()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; x --"));
            Assert.AreEqual(-1, TestUtils.Evaluate("x = 0; x --; x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("2 --"));
        }

        [TestMethod]
        public void Grouping()
        {
            Assert.AreEqual(22, TestUtils.Evaluate("(5 + 6) * 2"));
            Assert.AreEqual(27, TestUtils.Evaluate("5 + (5 + 6) * 2"));
            Assert.AreEqual(33, TestUtils.Evaluate("5 + (5 * 6) - 2"));
            Assert.AreEqual(32, TestUtils.Evaluate("(5 + (5 + 6)) * 2"));
        }

        [TestMethod]
        public void FunctionCall()
        {
            Assert.AreEqual("[object Math]", TestUtils.Evaluate("(Math.toString)()"));
            Assert.AreEqual("[object Math]", TestUtils.Evaluate("Math.toString()"));
            Assert.AreEqual(2, TestUtils.Evaluate("Math.ceil(1.2)"));
            Assert.AreEqual(0, TestUtils.Evaluate("Math.atan2(0, 2)"));
            Assert.AreEqual("[object Math]", TestUtils.Evaluate("(Math.toString)()"));

            // Call functions in the global scope.
            Assert.AreEqual("a%20b", TestUtils.Evaluate("encodeURI('a b')"));

            // Argument conversion.
            Assert.AreEqual(123, TestUtils.Evaluate("Math.abs('-123')"));

            // Extra arguments are ignored.
            Assert.AreEqual(2, TestUtils.Evaluate("Math.ceil(1.2, 5)"));

            // Too few arguments are passed as "undefined".
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Math.ceil()"));
            Assert.AreEqual(true, TestUtils.Evaluate("isNaN()"));
            Assert.AreEqual(false, TestUtils.Evaluate("isFinite()"));

            // Object must be a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("x = { a: 1 }; x()"));
        }

        [TestMethod]
        public void New()
        {
            Assert.AreEqual("five", TestUtils.Evaluate("(new String('five')).toString()"));
            Assert.AreEqual("five", TestUtils.Evaluate("new String('five').toString()"));
            Assert.AreEqual("5", TestUtils.Evaluate("new Number(5).toString()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new (String('five'))"));

            // Precedence tests.
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("[object Object]", TestUtils.Evaluate("(new Function.valueOf()).toString()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("(new Function.valueOf)()"));  // new (Function.valueOf) is not a function.
            Assert.AreEqual("function anonymous() {\n\n}", TestUtils.Evaluate("(new (Function.valueOf())).toString()"));
            Assert.AreEqual("function anonymous() {\n\n}", TestUtils.Evaluate("((new Function).valueOf()).toString()"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("[object Object]", TestUtils.Evaluate("(new (Function.valueOf)()).toString()"));

            // New user-defined function.
            Assert.AreEqual(5, TestUtils.Evaluate("function f() { this.a = 5; return 2; }; a = new f(); a.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("function f() { this.a = 5; return 2; }; a = new f(); Object.getPrototypeOf(a) === f.prototype"));
        }

        [TestMethod]
        public void InstanceOf()
        {
            // Primitive types always return false.
            Assert.AreEqual(false, TestUtils.Evaluate("5 instanceof Boolean"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 instanceof Number"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 instanceof String"));
            Assert.AreEqual(false, TestUtils.Evaluate("true instanceof Boolean"));
            Assert.AreEqual(false, TestUtils.Evaluate("'hello' instanceof Number"));
            Assert.AreEqual(false, TestUtils.Evaluate("'hello' instanceof String"));

            // Arrays and regular expressions return true for Array and RegExp respectively.
            // Since these objects inherit from Object, that returns true as well.
            Assert.AreEqual(true, TestUtils.Evaluate("/abc/ instanceof RegExp"));
            Assert.AreEqual(false, TestUtils.Evaluate("/abc/ instanceof Array"));
            Assert.AreEqual(true, TestUtils.Evaluate("[] instanceof Array"));
            Assert.AreEqual(false, TestUtils.Evaluate("[] instanceof Number"));
            Assert.AreEqual(true, TestUtils.Evaluate("/abc/ instanceof Object"));
            Assert.AreEqual(true, TestUtils.Evaluate("[] instanceof Object"));

            // Object literals return true for Object only.  Note: "{} instanceof Object" is invalid.
            Assert.AreEqual(true, TestUtils.Evaluate("x = {}; x instanceof Object"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = {}; x instanceof Array"));

            // Global functions return true for Object and Function.
            Assert.AreEqual(true, TestUtils.Evaluate("Array instanceof Function"));
            Assert.AreEqual(true, TestUtils.Evaluate("Array instanceof Object"));

            // Both sides can be a variable.
            Assert.AreEqual(true, TestUtils.Evaluate("x = Function; x instanceof x"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = Array; x instanceof x"));

            // Right-hand-side must be a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("5 instanceof Math"));

            // Test newly constructed objects.
            Assert.AreEqual(true, TestUtils.Evaluate("new Number(5) instanceof Number"));
            Assert.AreEqual(true, TestUtils.Evaluate("new Number(5) instanceof Object"));
            Assert.AreEqual(false, TestUtils.Evaluate("new Number(5) instanceof String"));
        }

        [TestMethod]
        public void In()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("'atan2' in Math"));
            Assert.AreEqual(true, TestUtils.Evaluate("1 in [5, 6]"));
            Assert.AreEqual(false, TestUtils.Evaluate("2 in [5, 6]"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' in {a: 1, b: 2}"));
            Assert.AreEqual(false, TestUtils.Evaluate("'c' in {a: 1, b: 2}"));
            Assert.AreEqual(true, TestUtils.Evaluate("'toString' in new Number(5)"));
            Assert.AreEqual(false, TestUtils.Evaluate("'abcdefgh' in new Number(5)"));
            Assert.AreEqual(true, TestUtils.Evaluate("'toString' in new String()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'toString' in 5"));
        }

        [TestMethod]
        public void MemberAccess()
        {
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("NaN"));
            Assert.AreEqual("abc", TestUtils.Evaluate("'abc'.toString()"));
            Assert.AreEqual("5.6", TestUtils.Evaluate("5.6.toString()"));
            Assert.AreEqual("5", TestUtils.Evaluate("5 .toString()"));
            Assert.AreEqual("5.6", TestUtils.Evaluate("5.6['toString']()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("5.toString"));
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "ReferenceError", TestUtils.EvaluateExceptionType("abcdefghij"));
            Assert.AreEqual("b", TestUtils.Evaluate("'abc'[1]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'abc'[3]"));
        }

        [TestMethod]
        public void ArrayLiteral()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("[].length"));
            if (TestUtils.Engine != JSEngine.JScript)
            {
                Assert.AreEqual(1, TestUtils.Evaluate("[,].length"));   // JScript says 2.
                Assert.AreEqual(1, TestUtils.Evaluate("[1,].length"));   // JScript says 2.
            }
            Assert.AreEqual(1, TestUtils.Evaluate("a = 1; [a][0]"));
            Assert.AreEqual(6, TestUtils.Evaluate("[[1, 2, 3], [4, 5, 6]][1][2]"));
            Assert.AreEqual(1, TestUtils.Evaluate("[1].length"));
            Assert.AreEqual(2, TestUtils.Evaluate("[1, 2].length"));
            Assert.AreEqual(true, TestUtils.Evaluate("[1,,2].hasOwnProperty('0')"));
            Assert.AreEqual(false, TestUtils.Evaluate("[1,,2].hasOwnProperty('1')"));
            Assert.AreEqual(true, TestUtils.Evaluate("[1,,2].hasOwnProperty('2')"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(true, TestUtils.Evaluate("[1,undefined,2].hasOwnProperty('1')"));
            Assert.AreEqual(true, TestUtils.Evaluate("[] instanceof Array"));
        }

        [TestMethod]
        public void ObjectLiteral()
        {
            Assert.AreEqual(1, TestUtils.Evaluate("x = {a: 1, b: 2}.a"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {'a': 1, 'b': 2}.a"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {0: 1, 1: 2}[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = {a: 1, b: 2}.b"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = {a: 1, a: 2}.a"));   // This is an error in strict mode.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("{a: 1, b: 2}"));
        }

        [TestMethod]
        public void Delete()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("x = {a: 1, b: 2}; delete x.a"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete Number.prototype"));
            Assert.AreEqual(true, TestUtils.Evaluate("delete abcdefg"));
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "ReferenceError", TestUtils.EvaluateExceptionType("x = 5; delete x; x"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("x = {a: 1, b: 2}; delete x.a; typeof(x.a)"));
        }

        [TestMethod]
        public void Typeof()
        {
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof abcdefg"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof undefined"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof Math.abcdefg"));
            Assert.AreEqual("boolean", TestUtils.Evaluate("typeof true"));
            Assert.AreEqual("number", TestUtils.Evaluate("typeof 1"));
            Assert.AreEqual("number", TestUtils.Evaluate("typeof 1.5"));
            Assert.AreEqual("string", TestUtils.Evaluate("typeof 'hello'"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof /abc/"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof {}"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof []"));
            Assert.AreEqual("function", TestUtils.Evaluate("typeof Math.toString"));
            Assert.AreEqual("number", TestUtils.Evaluate("x = 5; (function() { return typeof(x) })()"));
        }






        [TestMethod]
        public void If()
        {
            Assert.AreEqual(5, TestUtils.Evaluate("if (true) 5"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("if (false) 5"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("if (true) 5 else 6"));
            Assert.AreEqual(5, TestUtils.Evaluate("if (true) 5; else 6"));
            Assert.AreEqual(6, TestUtils.Evaluate("if (false) 5; else 6"));

            // Nested if statements.
            Assert.AreEqual(1, TestUtils.Evaluate("if (true) if (true) 1; else 2; else 3"));
            Assert.AreEqual(2, TestUtils.Evaluate("if (true) if (false) 1; else 2; else 3"));
            Assert.AreEqual(3, TestUtils.Evaluate("if (false) if (true) 1; else 2; else 3"));
            Assert.AreEqual(3, TestUtils.Evaluate("if (false) if (false) 1; else 2; else 3"));
        }

        [TestMethod]
        public void Empty()
        {
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate(""));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate(";"));
        }

        [TestMethod]
        public void Do()
        {
            Assert.AreEqual(7, TestUtils.Evaluate("x = 1; do { x = x + 3 } while (x < 5); x"));
            Assert.AreEqual(9, TestUtils.Evaluate("x = 6; do { x = x + 3 } while (x < 5); x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 1; do { x = x + 1 } while (x < 5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("5; do { } while(false)"));
        }

        [TestMethod]
        public void While()
        {
            Assert.AreEqual(7, TestUtils.Evaluate("x = 1; while (x < 5) { x = x + 3 } x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 6; while (x < 5) { x = x + 3 } x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 6; while (x < 5) { x = x + 3; x = x + 1 } x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 6; while (x < 5) { } x"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = 6; while () { x = x + 3 } x"));
            Assert.AreEqual(7, TestUtils.Evaluate("x = 1; while (x < 5) { x = x + 3 }"));
        }

        [TestMethod]
        public void For()
        {
            // for (<initial>; <test>; <increment>)
            Assert.AreEqual(11, TestUtils.Evaluate("y = 1; for (x = 1; x < 5; x ++) { y = y + x } y"));
            Assert.AreEqual(100, TestUtils.Evaluate("for (;;) { y = 100; break; } y"));

            // for (var x = <initial>; <test>; <increment>)
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; for (var x; x < 5; x ++) { }"));
            Assert.AreEqual(11, TestUtils.Evaluate("y = 1; for (var x = 1; x < 5; x ++) { y = y + x } y"));
            Assert.AreEqual(11, TestUtils.Evaluate("for (var x = 1, y = 1; x < 5; x ++) { y = y + x } y"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("for (var x + 1; x < 5; x ++) { }"));

            // for (x in <expression>)
            Assert.AreEqual("ab", TestUtils.Evaluate("y = ''; for (x in {a: 1, b: 2}) { y += x } y"));
            Assert.AreEqual("1", TestUtils.Evaluate("y = 0; for (x in [7, 5]) { y = x } y"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; for (x in null) { x = 1 } x"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; for (x in undefined) { x = 1 } x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("for (5 in [1, 2]) {}"));
            Assert.AreEqual("atest", TestUtils.Evaluate("Object.prototype.test = 19; y = ''; for (x in {a: 5}) { y += x } delete Object.prototype.test; y"));

            // for (var x in <expression>)
            Assert.AreEqual("1", TestUtils.Evaluate("y = 0; for (var x in [7, 5]) { y = x } y"));
            Assert.AreEqual("01234", TestUtils.Evaluate("y = ''; for (var x in 'hello') { y += x } y"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("for (var 5 in [1, 2])"));

            // Modify an object while enumerating it should return the keys as they were originally.
            Assert.AreEqual("bc", TestUtils.Evaluate("var a = {b: 2, c: 3}; var keys = ''; for (var x in a) { a.d = 5; keys += x; }"));
        }

        [TestMethod]
        public void Continue()
        {
            // continue
            Assert.AreEqual(1, TestUtils.Evaluate("x = y = 1; while(x < 5) { x ++; continue; y ++ } y"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 1; do { x ++; continue; x += 10 } while(false); x"));
            Assert.AreEqual(5, TestUtils.Evaluate("for(x = 1; x < 5; x ++) { continue; x += 10 } x"));

            // continue [label]
            Assert.AreEqual(1, TestUtils.Evaluate("x = y = 1; test: while(x < 5) { x ++; continue test; y ++ } y"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 0; test: do { x ++; while(x < 5) { x ++; continue test; x += 10 } x += 20 } while(false); x"));

            // The label must be an enclosing *iteration* statement in the same function.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = 1; test: continue test; x"));
        }

        [TestMethod]
        public void Break()
        {
            // break
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; while(x < 5) { break; x ++ } x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; do { break; x = 5 } while(false); x"));
            Assert.AreEqual(1, TestUtils.Evaluate("for(x = 1; x < 5; x ++) { break; } x"));

            // break [label]
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; test: do { break test; x = 5 } while(false); x"));
            Assert.AreEqual(3, TestUtils.Evaluate("x = 1; test: do { x = 2; do { x = 3; break test; x = 4 } while(false); x = 5 } while(false); x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; test: break test; x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; test: { break test; x = 5 } x"));

            // Duplicate nested labels are not allowed.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("label: { } label: { }"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("label: { label: { } }"));

            // The label must be an enclosing statement in the same function.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = 1; test: x ++; break test; x")); // Not an enclosing statement.
        }

        [TestMethod]
        public void Var()
        {
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("var x"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("var x; x"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("var x, y"));
            Assert.AreEqual(5, TestUtils.Evaluate("var x = 5; x"));
            Assert.AreEqual(6, TestUtils.Evaluate("var x, y = 6; y"));
            Assert.AreEqual(1, TestUtils.Evaluate("var x = 1, y = 2; x"));
            Assert.AreEqual(2, TestUtils.Evaluate("var x = 1, y = 2; y"));
            Assert.AreEqual(2, TestUtils.Evaluate("var x = Math.max(1, 2); x"));
        }

        [TestMethod]
        public void Return()
        {
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("return 5"));
        }

        [TestMethod]
        public void With()
        {
            Assert.AreEqual(234, TestUtils.Evaluate("x = { a: 234 }; with (x) { a }"));
            Assert.AreEqual(234, TestUtils.Evaluate("x = { a: 234 }; a = 5; with (x) { a }"));
            Assert.AreEqual(15, TestUtils.Evaluate("x = { a: 234 }; b = 15; with (x) { b }"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } a"));
            Assert.AreEqual(13, TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } b"));
            Assert.AreEqual(12, TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } x.b"));
            Assert.AreEqual("number", TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, typeof(b) }"));

            // Implicit this.
            Assert.AreEqual(1970, TestUtils.Evaluate("x = new Date(5); f = x.getFullYear; x.f = f; with (x) { f() }"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("x = new Date(5); f = x.getFullYear; with (x) { f() }"));

            // With and var.
            Assert.AreEqual(5, TestUtils.Evaluate("x = { a: 234 }; with (x) { var a = 5; } x.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = { a: 234 }; with (x) { var a = 5; } a"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = { a: 234 }; with (x) { var b = 5; } b"));
        }

        [TestMethod]
        public void Switch()
        {
            Assert.AreEqual(5, TestUtils.Evaluate("x = 5; switch (x) { }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x ++) { } x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { case 5: 6 }"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 5; switch (x) { case 4: 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { default: 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { case 4: case 5: 6 }"));
            Assert.AreEqual(7, TestUtils.Evaluate("x = 5; switch (x) { case 5: 6; case 6: 7; }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { case 5: 6; break; 7 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 5; switch (x) { case 5: 6; break; case 6: 7 }"));
            Assert.AreEqual(8, TestUtils.Evaluate("x = 5; switch (x) { case 4: 6; case 5: 7; default: 8 }"));
            Assert.AreEqual(5, TestUtils.Evaluate("(function(x) { switch (x) { case 8: return 4; case 9: return 5; default: return 7 } })(9)"));
        }

        [TestMethod]
        public void Throw()
        {
            Assert.AreEqual(Undefined.Value, TestUtils.EvaluateExceptionType("throw 5"));
            Assert.AreEqual(5, TestUtils.Evaluate("try { throw 5 } catch (e) { e }"));

            // Catch creates a new scope.
            Assert.AreEqual(5, TestUtils.Evaluate("e = 5; try { throw 6; } catch (e) { } e"));

            // Exceptions can be swalled using finally.
            Assert.AreEqual(5, TestUtils.Evaluate("function f() { try { throw 5 } finally { return 6 } } f()"));
        }

        [TestMethod]
        public void Function()
        {
            Assert.AreEqual(6, TestUtils.Evaluate("function f(a, b, c) { return a + b + c; } f(1, 2, 3)"));

            // Multiple variable definitions.
            Assert.AreEqual(5, TestUtils.Evaluate("var a = 5; function a() { return 6 }; a"));
            Assert.AreEqual(5, TestUtils.Evaluate("function a() { return 6 }; var a = 5; a"));
            Assert.IsInstanceOfType(TestUtils.Evaluate("var a; function a() { return 6 }; a"), typeof(Jurassic.Library.FunctionInstance));
            Assert.IsInstanceOfType(TestUtils.Evaluate("function a() { return 6 }; var a; a"), typeof(Jurassic.Library.FunctionInstance));
            Assert.AreEqual(7, TestUtils.Evaluate("function a() { return 6 }; function a() { return 7 } a()"));

            // Closures.
            //Assert.AreEqual(5, TestUtils.Evaluate("a = 5; obj = { a: 123 }; with(obj) { function f() { return a } f() }"));
            Assert.AreEqual(7, TestUtils.Evaluate("function f(a, b, c) { return f2(a + 1); function f2(d) { return d + b + c; } } f(1, 2, 3)"));

            // Function expressions.
            Assert.AreEqual(3, TestUtils.Evaluate("var f = function () { return 3; }; f()"));

            // This
            Assert.AreEqual(1, TestUtils.Evaluate("var f = function () { return this.a; }; var x = {'a': 1, 'f': f}; x.f()"));
            Assert.AreEqual(false, TestUtils.Evaluate("var f = function () { return delete this; }; var x = {'a': 1, 'f': f}; x.f()"));
        }

        [TestMethod]
        public void This()
        {
            // "this" is set to the global object by default.
            Assert.AreEqual(Jurassic.Library.GlobalObject.Instance, TestUtils.Evaluate("this"));
            Assert.AreEqual(5, TestUtils.Evaluate("this.x = 5; this.x"));

            // In ES3 functions will get the global object as the "this" value by default.
            Assert.AreEqual(Jurassic.Library.GlobalObject.Instance, TestUtils.Evaluate("(function() { return this })()"));
        }

        [TestMethod]
        public void Scope()
        {
            // Global scope - var has no effect.
            Assert.AreEqual(5, TestUtils.Evaluate("x = 10; var x = 5; x"));

            // Local variable.
            Assert.AreEqual(3, TestUtils.Evaluate("function test(test) { test = 3; return test; } test(); test();"));
        }

        [TestMethod]
        public void Associativity()
        {
            // "a[b[]]

            // precedence.
            Assert.AreEqual(true, TestUtils.Evaluate("true == 5 > false"));
            Assert.AreEqual(5, TestUtils.Evaluate("5 ^ 1 & 2"));
            Assert.AreEqual(7, TestUtils.Evaluate("5 | 9 ^ 11"));
        }

        [TestMethod]
        public void DivisionAmbiguity()
        {
            Assert.AreEqual("/abc/", TestUtils.Evaluate("/abc/.toString()"));
            Assert.AreEqual(3, TestUtils.Evaluate("abc = 2; 6/abc"));
            Assert.AreEqual("/abc/", TestUtils.Evaluate("if (true) /abc/.toString()"));
            Assert.AreEqual(1.5, TestUtils.Evaluate("g = 2; (5 + 1) /2/g"));
            Assert.AreEqual("/abc/", TestUtils.Evaluate("if (true) {} /abc/.toString()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("a = {} / 2"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("{} / 2"));
        }

        [TestMethod]
        public void AutomaticSemicolonInsertion()
        {
            // Automatic semi-colon insertion.
            Assert.AreEqual(6, TestUtils.Evaluate("x = 1 + \r\n 5"));
            Assert.AreEqual("11,2", TestUtils.Evaluate("x = 1 + \r\n [1, 2]"));

            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("var y = 6; var x = y ++ 5"));
            Assert.AreEqual(5, TestUtils.Evaluate("var y = 6; var x = y ++ \r\n 5"));
            TestUtils.Evaluate("var y = 6; var x = y ++ \r\n function f() {}");

            TestUtils.Evaluate("var x = \r\nfunction f() { }");
            TestUtils.Evaluate("var x = 5\r\nfunction f() { }");
        }
    }
}
