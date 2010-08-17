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
            Assert.AreEqual(-5, TestUtils.Evaluate("x = '-5'; +x"));
            Assert.AreEqual(1e20, TestUtils.Evaluate("+1e20"));
            Assert.AreEqual(3.1415, TestUtils.Evaluate("+3.1415"));
            Assert.AreEqual(5, TestUtils.Evaluate("+new Date(5)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("+new Object()"));
        }

        [TestMethod]
        public void UnaryMinus()
        {
            Assert.AreEqual(-20, TestUtils.Evaluate("-20"));
            Assert.AreEqual(-5, TestUtils.Evaluate("- '5'"));
            Assert.AreEqual(-1, TestUtils.Evaluate("-true"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("-'Hello'"));
            Assert.AreEqual(-5, TestUtils.Evaluate("x = '5'; -x"));
            Assert.AreEqual(-1e20, TestUtils.Evaluate("-1e20"));
            Assert.AreEqual(-3.1415, TestUtils.Evaluate("-3.1415"));
            Assert.AreEqual(-5, TestUtils.Evaluate("-new Date(5)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("-new Object()"));
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

            // Objects
            Assert.AreEqual(-6, TestUtils.Evaluate("~new Number(5)"));

            // Variables
            Assert.AreEqual(-1, TestUtils.Evaluate("x = 0; ~x"));
            Assert.AreEqual(-21, TestUtils.Evaluate("x = 20; ~x"));
            Assert.AreEqual(19, TestUtils.Evaluate("x = -20; ~x"));
            Assert.AreEqual(-9, TestUtils.Evaluate("x = 4294967304; ~x"));
            Assert.AreEqual(-21, TestUtils.Evaluate("x =  '20'; ~x"));
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

            // Objects
            Assert.AreEqual(false, TestUtils.Evaluate("!new Number(5)"));

            // Variables
            Assert.AreEqual(true, TestUtils.Evaluate("x = false; !x"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = true; !x"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = 10; !x"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = 0; !x"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = 'hello'; !x"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = ''; !x"));
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

            // Objects
            Assert.AreEqual(11, TestUtils.Evaluate("new Number(5) + new Number(6)"));
            Assert.AreEqual("test6", TestUtils.Evaluate("'test' + new Number(6)"));
            Assert.AreEqual("5test", TestUtils.Evaluate("new Number(5) + 'test'"));

            // Variables
            Assert.AreEqual(35, TestUtils.Evaluate("x = 15; x + 20"));
            Assert.AreEqual(21.5, TestUtils.Evaluate("x = 1.5; x + 20"));
            Assert.AreEqual(8589934608.0, TestUtils.Evaluate("x = 4294967304; x + 4294967304"));
            Assert.AreEqual("testing", TestUtils.Evaluate("x = 'tes'; x + 'ting'"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = true; x + false"));
            Assert.AreEqual("102", TestUtils.Evaluate("x = 2; '10' + x"));
            Assert.AreEqual("10null", TestUtils.Evaluate("x = '10'; x + null"));
            Assert.AreEqual("51,2,3", TestUtils.Evaluate("x = 5; x + [1,2,3]"));
            Assert.AreEqual("5Thu Jan 01 1970 13:00:00 GMT+1300 (New Zealand Daylight Time)", TestUtils.Evaluate("x = 5; x + new Date(5)"));
            Assert.AreEqual("5/abc/g", TestUtils.Evaluate("x = 5; x + /abc/g"));
            Assert.AreEqual("5[object Object]", TestUtils.Evaluate("x = 5; x + {}"));
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

            // Objects
            Assert.AreEqual(-1, TestUtils.Evaluate("new Number(5) - new Number(6)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("'test' - new Number(6)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Number(5) - 'test'"));

            // Variables
            Assert.AreEqual(-5, TestUtils.Evaluate("x = 15; x - 20"));
            Assert.AreEqual(-18.5, TestUtils.Evaluate("x = 20; 1.5 - x"));
            Assert.AreEqual(-4294967304.0, TestUtils.Evaluate("x = 8589934608; 4294967304 - x"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'tes'; x - 'ting'"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = true; x - false"));
            Assert.AreEqual(8, TestUtils.Evaluate("x = '10'; x - 2"));
            Assert.AreEqual(10, TestUtils.Evaluate("x = null; '10' - x"));
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

            // Objects
            Assert.AreEqual(30, TestUtils.Evaluate("new Number(5) * new Number(6)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("'test' * new Number(6)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Number(5) * 'test'"));

            // Variables
            Assert.AreEqual(300, TestUtils.Evaluate("x = 15; x * 20"));
            Assert.AreEqual(30, TestUtils.Evaluate("x = 1.5; x * 20"));
            Assert.AreEqual(8589934608.0, TestUtils.Evaluate("x = 4294967304; x * 2"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'ting'; 'tes' * x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = true; true * x"));
            Assert.AreEqual(20, TestUtils.Evaluate("x = '10'; x * 2"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = null; '10' * x"));
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

            // Objects
            Assert.AreEqual(2, TestUtils.Evaluate("new Number(12) / new Number(6)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("'test' / new Number(6)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Number(5) / 'test'"));

            // Variables
            Assert.AreEqual(0.75, TestUtils.Evaluate("x = 15; x / 20"));
            Assert.AreEqual(0.075, TestUtils.Evaluate("x = 1.5; x / 20"));
            Assert.AreEqual(2147483652.0, TestUtils.Evaluate("x = 4294967304; x / 2"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'ting'; 'tes' / x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = true; true / x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 2; '10' / x"));
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("x = '10'; x / null"));
            Assert.AreEqual(double.NegativeInfinity, TestUtils.Evaluate("x = '-10'; x / null"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 0; x / 0"));
        }

        [TestMethod]
        public void Remainder()
        {
            Assert.AreEqual(2, TestUtils.Evaluate("17 % 5"));
            Assert.AreEqual(2, TestUtils.Evaluate("17 % -5"));
            Assert.AreEqual(-2, TestUtils.Evaluate("-17 % 5"));
            Assert.AreEqual(-2, TestUtils.Evaluate("-17 % -5"));
            Assert.AreEqual(2.2, (double)TestUtils.Evaluate("17.2 % 5"), 0.00000000000001);
            Assert.AreEqual(2.8, (double)TestUtils.Evaluate("17.8 % 5"), 0.00000000000001);
            Assert.AreEqual(-2.2, (double)TestUtils.Evaluate("-17.2 % 5"), 0.00000000000001);
            Assert.AreEqual(-2.8, (double)TestUtils.Evaluate("-17.8 % 5"), 0.00000000000001);

            // Objects
            Assert.AreEqual(1, TestUtils.Evaluate("new Number(7) % new Number(6)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("'test' % new Number(6)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Number(5) % 'test'"));

            // Variables
            Assert.AreEqual(2, TestUtils.Evaluate("x = 17; x % 5"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = -5; 17 % x"));
            Assert.AreEqual(-2, TestUtils.Evaluate("x = 5; -17 % x"));
            Assert.AreEqual(-2, TestUtils.Evaluate("x = -5; -17 % x"));
            Assert.AreEqual(2.2, (double)TestUtils.Evaluate("x = 17.2; x % 5"), 0.00000000000001);
            Assert.AreEqual(2.8, (double)TestUtils.Evaluate("x = 17.8; x % 5"), 0.00000000000001);
            Assert.AreEqual(-2.2, (double)TestUtils.Evaluate("x = -17.2; x % 5"), 0.00000000000001);
            Assert.AreEqual(-2.8, (double)TestUtils.Evaluate("x = -17.8; x % 5"), 0.00000000000001);
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

            // Objects
            Assert.AreEqual(448, TestUtils.Evaluate("new Number(7) << new Number(6)"));
            Assert.AreEqual(0, TestUtils.Evaluate("'test' << new Number(6)"));
            Assert.AreEqual(5, TestUtils.Evaluate("new Number(5) << 'test'"));

            // Variables
            Assert.AreEqual(40, TestUtils.Evaluate("x = 10; x << 2"));
            Assert.AreEqual(-400, TestUtils.Evaluate("x = 2; -100 << x"));
            Assert.AreEqual(20, TestUtils.Evaluate("x = 1.2; 10 << x"));
            Assert.AreEqual(20, TestUtils.Evaluate("x = 10; x << 1.8"));
            Assert.AreEqual(16, TestUtils.Evaluate("x = 4294967304; x << 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 8; x << -2"));
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

            // Objects
            Assert.AreEqual(3, TestUtils.Evaluate("new Number(7) >> new Number(1)"));
            Assert.AreEqual(0, TestUtils.Evaluate("'test' >> new Number(6)"));
            Assert.AreEqual(5, TestUtils.Evaluate("new Number(5) >> 'test'"));

            // Variables
            Assert.AreEqual(2, TestUtils.Evaluate("x = 10; x >> 2"));
            Assert.AreEqual(-25, TestUtils.Evaluate("x = 2; -100 >> x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 1.2; 10 >> x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 10; x >> 1.8"));
            Assert.AreEqual(4, TestUtils.Evaluate("x = 4294967304; x >> 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = -2; 8 >> x"));
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

            // Objects
            Assert.AreEqual(3, TestUtils.Evaluate("new Number(7) >>> new Number(1)"));
            Assert.AreEqual(0, TestUtils.Evaluate("'test' >>> new Number(6)"));
            Assert.AreEqual(5, TestUtils.Evaluate("new Number(5) >>> 'test'"));

            // Variables
            Assert.AreEqual(2, TestUtils.Evaluate("x = 10; x >>> 2"));
            Assert.AreEqual(1073741799, TestUtils.Evaluate("x = 2; -100 >>> x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 1.2; 10 >>> x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 10; x >>> 1.8"));
            Assert.AreEqual(4, TestUtils.Evaluate("x = 4294967304; x >>> 1"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = -2; 8 >>> x"));
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

            // Operands of different types.
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

            // Doug Crockford's truth table.
            Assert.AreEqual(false, TestUtils.Evaluate( "''        ==   '0'           "));
            Assert.AreEqual(true,  TestUtils.Evaluate( "0         ==   ''            "));
            Assert.AreEqual(true,  TestUtils.Evaluate( "0         ==   '0'           "));
            Assert.AreEqual(false, TestUtils.Evaluate( "false     ==   'false'       "));
            Assert.AreEqual(true,  TestUtils.Evaluate( "false     ==   '0'           "));
            Assert.AreEqual(false, TestUtils.Evaluate( "false     ==   undefined     "));
            Assert.AreEqual(true,  TestUtils.Evaluate( "false     ==   null          "));
            Assert.AreEqual(true,  TestUtils.Evaluate( "null      ==   undefined     "));
            Assert.AreEqual(true,  TestUtils.Evaluate(@"' \t\r\n' ==   0             "));

            // Variables
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10.0); x == 10"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10.0); x.valueOf() == 10"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10.0); 10 == x.valueOf()"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10); x == new Number(10)"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10); x == x"));

            // Arrays
            Assert.AreEqual(true, TestUtils.Evaluate("2 == [2]"));
            Assert.AreEqual(true, TestUtils.Evaluate("2 == [[[2]]]"));
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

            // Operands of different types.
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

            // Variables
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10.0); x != 10"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10.0); x.valueOf() != 10"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10.0); 10 != x.valueOf()"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10); x != new Number(10)"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10); x != x"));
        }

        [TestMethod]
        public void StrictEquals()
        {
            // Both operands are of the same type.
            Assert.AreEqual(false, TestUtils.Evaluate("false === true"));
            Assert.AreEqual(true, TestUtils.Evaluate("false === false"));
            Assert.AreEqual(true, TestUtils.Evaluate("10 === 10"));
            Assert.AreEqual(true, TestUtils.Evaluate("10.0 === 10"));
            Assert.AreEqual(false, TestUtils.Evaluate("10 === 11"));
            Assert.AreEqual(true, TestUtils.Evaluate("'test' === 'test'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'test' === 'TEST'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'' === ''"));

            // Operands of different types.
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

            // Variables
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10.0); x === 10"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10.0); x.valueOf() === 10"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10.0); 10 === x.valueOf()"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10); x === new Number(10)"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10); x === x"));
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

            // Operands of different types.
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

            // Variables
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10.0); x !== 10"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10.0); x.valueOf() !== 10"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10.0); 10 !== x.valueOf()"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = new Number(10); x !== new Number(10)"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = new Number(10); x !== x"));
        }

        [TestMethod]
        public void LessThan()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("7 < 5"));
            Assert.AreEqual(true, TestUtils.Evaluate("5 < 7"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 < 5"));
            Assert.AreEqual(true, TestUtils.Evaluate("-5 < 5"));
            Assert.AreEqual(true, TestUtils.Evaluate("5.6 < 5.7"));
            Assert.AreEqual(false, TestUtils.Evaluate("5.6 < 5.6"));
            Assert.AreEqual(false, TestUtils.Evaluate("5.6 < 5.5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 < NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("NaN < NaN"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' < 'b'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' < 'a'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' < 'A'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'2' < '15'"));
            Assert.AreEqual(true, TestUtils.Evaluate("2 < '15'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'15' < 2"));
            Assert.AreEqual(true, TestUtils.Evaluate("false < true"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = 0.3; y = 0.5; x < y"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = 0.4; y = 0.4; x < y"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = 0.5; y = 0.3; x < y"));
        }

        [TestMethod]
        public void LessThanOrEqual()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("7 <= 5"));
            Assert.AreEqual(true, TestUtils.Evaluate("5 <= 7"));
            Assert.AreEqual(true, TestUtils.Evaluate("5 <= 5"));
            Assert.AreEqual(true, TestUtils.Evaluate("-5 <= 5"));
            Assert.AreEqual(true, TestUtils.Evaluate("5.6 <= 5.7"));
            Assert.AreEqual(true, TestUtils.Evaluate("5.6 <= 5.6"));
            Assert.AreEqual(false, TestUtils.Evaluate("5.6 <= 5.5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 <= NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("NaN <= NaN"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' <= 'b'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' <= 'a'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' <= 'A'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'2' <= '15'"));
            Assert.AreEqual(true, TestUtils.Evaluate("2 <= '15'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'15' <= 2"));
            Assert.AreEqual(true, TestUtils.Evaluate("false <= true"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = 0.3; y = 0.5; x <= y"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = 0.4; y = 0.4; x <= y"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = 0.5; y = 0.3; x <= y"));
        }

        [TestMethod]
        public void GreaterThan()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("7 > 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 > 7"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 > 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("-5 > 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5.6 > 5.7"));
            Assert.AreEqual(false, TestUtils.Evaluate("5.6 > 5.6"));
            Assert.AreEqual(true, TestUtils.Evaluate("5.6 > 5.5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 > NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("NaN > NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' > 'b'"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' > 'a'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' > 'A'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'2' > '15'"));
            Assert.AreEqual(false, TestUtils.Evaluate("2 > '15'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'15' > 2"));
            Assert.AreEqual(false, TestUtils.Evaluate("false > true"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = 0.3; y = 0.5; x > y"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = 0.4; y = 0.4; x > y"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = 0.5; y = 0.3; x > y"));
        }

        [TestMethod]
        public void GreaterThanOrEqual()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("7 >= 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 >= 7"));
            Assert.AreEqual(true, TestUtils.Evaluate("5 >= 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("-5 >= 5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5.6 >= 5.7"));
            Assert.AreEqual(true, TestUtils.Evaluate("5.6 >= 5.6"));
            Assert.AreEqual(true, TestUtils.Evaluate("5.6 >= 5.5"));
            Assert.AreEqual(false, TestUtils.Evaluate("5 >= NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("NaN >= NaN"));
            Assert.AreEqual(false, TestUtils.Evaluate("'a' >= 'b'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' >= 'a'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'a' >= 'A'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'2' >= '15'"));
            Assert.AreEqual(false, TestUtils.Evaluate("2 >= '15'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'15' >= 2"));
            Assert.AreEqual(false, TestUtils.Evaluate("false >= true"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = 0.3; y = 0.5; x >= y"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = 0.4; y = 0.4; x >= y"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = 0.5; y = 0.3; x >= y"));
        }

        [TestMethod]
        public void BitwiseAnd()
        {
            // Constants
            Assert.AreEqual(3, TestUtils.Evaluate("11 & 7"));
            Assert.AreEqual(9, TestUtils.Evaluate("11 & -7"));
            Assert.AreEqual(8, TestUtils.Evaluate("4294967304 & 255"));
            Assert.AreEqual(16, TestUtils.Evaluate("42949673042 & -401929233123"));
            Assert.AreEqual(1, TestUtils.Evaluate("11.9 & 1.5"));
            Assert.AreEqual(0, TestUtils.Evaluate("NaN & NaN"));

            // Variables
            Assert.AreEqual(3, TestUtils.Evaluate("x = 11; x & 7"));
            Assert.AreEqual(9, TestUtils.Evaluate("x = 11; x & -7"));
            Assert.AreEqual(8, TestUtils.Evaluate("x = 4294967304; x & 255"));
            Assert.AreEqual(16, TestUtils.Evaluate("x = 42949673042; x & -401929233123"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 11.5; x & 1.5"));
        }

        [TestMethod]
        public void BitwiseXor()
        {
            // Constants
            Assert.AreEqual(12, TestUtils.Evaluate("11 ^ 7"));
            Assert.AreEqual(-14, TestUtils.Evaluate("11 ^ -7"));
            Assert.AreEqual(247, TestUtils.Evaluate("4294967304 ^ 255"));
            Assert.AreEqual(10, TestUtils.Evaluate("11.5 ^ 1.5"));
            Assert.AreEqual(3, TestUtils.Evaluate("'5' ^ '6'"));
            Assert.AreEqual(1, TestUtils.Evaluate("'a' ^ 1"));

            // Variables
            Assert.AreEqual(12, TestUtils.Evaluate("x = 11; x ^ 7"));
            Assert.AreEqual(-14, TestUtils.Evaluate("x = 11; x ^ -7"));
            Assert.AreEqual(247, TestUtils.Evaluate("x = 4294967304; x ^ 255"));
            Assert.AreEqual(1797692751, TestUtils.Evaluate("x = 42949673042; x ^ -401929233123"));
            Assert.AreEqual(10, TestUtils.Evaluate("x = 11.5; x ^ 1.5"));
        }

        [TestMethod]
        public void BitwiseOr()
        {
            Assert.AreEqual(15, TestUtils.Evaluate("11 | 7"));
            Assert.AreEqual(-5, TestUtils.Evaluate("11 | -7"));
            Assert.AreEqual(255, TestUtils.Evaluate("8 | 255"));
            Assert.AreEqual(11, TestUtils.Evaluate("11.5 | 1.5"));
            Assert.AreEqual(7, TestUtils.Evaluate("'5' | '6'"));
            Assert.AreEqual(1, TestUtils.Evaluate("'a' | 1"));

            // Variables
            Assert.AreEqual(15, TestUtils.Evaluate("x = 11; x | 7"));
            Assert.AreEqual(-5, TestUtils.Evaluate("x = -7; 11 | x"));
            Assert.AreEqual(255, TestUtils.Evaluate("x = 8; x | 255"));
            Assert.AreEqual(11, TestUtils.Evaluate("x = 1.5; 11.5 | x"));
            Assert.AreEqual(7, TestUtils.Evaluate("x = '5'; y = '6'; x | y"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 1; 'a' | x"));
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

            // Variables.
            Assert.AreEqual(false, TestUtils.Evaluate("x = false; x && false"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = true; false && x"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = true; y = false; x && y"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = true; y = true; x && y"));
            Assert.AreEqual(false, TestUtils.Evaluate("x = false; y = 11; x && y"));
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

            // Variables.
            Assert.AreEqual(false, TestUtils.Evaluate("x = false; x || false"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = true; false || x"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = true; y = false; x || y"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = true; y = true; x || y"));
            Assert.AreEqual(11, TestUtils.Evaluate("x = false; y = 11; x || y"));
        }

        [TestMethod]
        public void Comma()
        {
            Assert.AreEqual(2, TestUtils.Evaluate("1, 2"));
            Assert.AreEqual("aliens", TestUtils.Evaluate("1, 'aliens'"));
            Assert.AreEqual(true, TestUtils.Evaluate("'go', true"));
            Assert.AreEqual(3, TestUtils.Evaluate("1, 2, 3"));
            Assert.AreEqual(3, TestUtils.Evaluate("var x = 1, y = 2, z = 3; x, y, z"));
            Assert.AreEqual(3, TestUtils.Evaluate("var x = [1, 2, 3]; x[0], x[1], x[2]"));
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

            // Test the precedence in the middle of the conditional.
            Assert.AreEqual(1, TestUtils.Evaluate("x = 5; true ? x = 1 : 2"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = 5; true ? 1, x : 2"));

            // Test the precedence at the end of the conditional.
            Assert.AreEqual(1, TestUtils.Evaluate("x = 4; true ? 1 : x = 2"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 4; true ? 1 : x += 2"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 3; true ? 1 : x, 2"));

            // Variables
            Assert.AreEqual(2, TestUtils.Evaluate("var x = 1, y = 2, z = 3; x ? y : z"));
            Assert.AreEqual(3, TestUtils.Evaluate("var x = 0, y = 2, z = 3; x ? y : z"));
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
            Assert.AreEqual(4, TestUtils.Evaluate("x = 4; x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x = 4; x += 2; x"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 4; x -= 2; x"));
            Assert.AreEqual(8, TestUtils.Evaluate("x = 4; x *= 2; x"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 4; x /= 2; x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 4; x %= 3; x"));
            Assert.AreEqual(8, TestUtils.Evaluate("x = 4; x <<= 1; x"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 4; x >>= 1; x"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = 4; x >>>= 1; x"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 4; x &= 1; x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 4; x |= 1; x"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = 4; x ^= 1; x"));

            // String operations.
            Assert.AreEqual("hah", TestUtils.Evaluate("x = 'hah'"));
            Assert.AreEqual("hah2", TestUtils.Evaluate("x = 'hah'; x += 2"));
            Assert.AreEqual("32", TestUtils.Evaluate("x = '3'; x += '2'"));
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
            Assert.AreEqual("hah", TestUtils.Evaluate("x = 'hah'; x"));
            Assert.AreEqual("hah2", TestUtils.Evaluate("x = 'hah'; x += 2; x"));
            Assert.AreEqual("32", TestUtils.Evaluate("x = '3'; x += '2'; x"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'hah'; x -= 2; x"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'hah'; x *= 2; x"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'hah'; x /= 2; x"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x = 'hah'; x %= 3; x"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 'hah'; x <<= 1; x"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 'hah'; x >>= 1; x"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 'hah'; x >>>= 1; x"));
            Assert.AreEqual(0, TestUtils.Evaluate("x = 'hah'; x &= 1; x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 'hah'; x |= 1; x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 'hah'; x ^= 1; x"));
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
            Assert.AreEqual(true, TestUtils.Evaluate("b = 5; this.hasOwnProperty('b')"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("b = 5; hasOwnProperty('b')"));

            // Argument conversion.
            Assert.AreEqual(123, TestUtils.Evaluate("Math.abs('-123')"));

            // Extra arguments are ignored.
            Assert.AreEqual(2, TestUtils.Evaluate("Math.ceil(1.2, 5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("function test(test) { return test; } test(5, 4, 3);"));

            // Too few arguments are passed as "undefined".
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Math.ceil()"));
            Assert.AreEqual(true, TestUtils.Evaluate("isNaN()"));
            Assert.AreEqual(false, TestUtils.Evaluate("isFinite()"));
            Assert.AreEqual(3, TestUtils.Evaluate("function test(test) { test = 3; return test; } test();"));

            // Object must be a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("x = { a: 1 }; x()"));

            // Passing a function in an argument.
            Assert.AreEqual(3, TestUtils.Evaluate("function a(b) { return b + 2; } function c(func) { return func(1); } c(a)"));
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
            Assert.AreEqual(true, TestUtils.Evaluate("'valueOf' in {a: 1, b: 2}"));
            Assert.AreEqual(true, TestUtils.Evaluate("'toString' in new Number(5)"));
            Assert.AreEqual(false, TestUtils.Evaluate("'abcdefgh' in new Number(5)"));
            Assert.AreEqual(true, TestUtils.Evaluate("'toString' in new String()"));
            Assert.AreEqual(true, TestUtils.Evaluate("var x = 'atan2', y = Math; x in y"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'toString' in 5"));
        }

        [TestMethod]
        public void MemberAccess()
        {
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("NaN"));

            // Member operator (get)
            Assert.AreEqual("abc", TestUtils.Evaluate("'abc'.toString()"));
            Assert.AreEqual("5.6", TestUtils.Evaluate("5.6.toString()"));
            Assert.AreEqual("5", TestUtils.Evaluate("5 .toString()"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = { a: 1 }; x.a"));

            // Member operator (set)
            Assert.AreEqual(2, TestUtils.Evaluate("x = { a: 1 }; x.a = 2; x.a"));
            
            // Index operator (get)
            Assert.AreEqual("5.6", TestUtils.Evaluate("5.6['toString']()"));
            Assert.AreEqual("b", TestUtils.Evaluate("'abc'[1]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'abc'[3]"));
            Assert.AreEqual("b", TestUtils.Evaluate("y = 1; 'abc'[y]"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = { a: 1 }; x['a']"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = { a: 1 }; y = 'a'; x[y]"));
            Assert.AreEqual(3, TestUtils.Evaluate("var a = {false: 3}; a[false]"));
            Assert.AreEqual(4, TestUtils.Evaluate("var a = [4]; a[[[[0]]]]"));
            Assert.AreEqual(5, TestUtils.Evaluate("var a = { 'abc' : 5 }; a[[[['abc']]]]"));

            // Index operator (set)
            Assert.AreEqual("5.6", TestUtils.Evaluate("var x = 5.6; x['toString'] = function() { }; x.toString()"));
            Assert.AreEqual("d", TestUtils.Evaluate("'abc'[1] = 'd'"));
            Assert.AreEqual("abc", TestUtils.Evaluate("var x = 'abc'; x[1] = 'd'; x"));
            Assert.AreEqual("d", TestUtils.Evaluate("'abc'[3] = 'd'"));
            Assert.AreEqual("abc", TestUtils.Evaluate("y = 1; var x = 'abc'; x[y] = 'd'; x"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = { a: 1 }; x['a'] = 2; x['a']"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = { a: 1 }; y = 'a'; x[y] = 2; x[y]"));

            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "ReferenceError", TestUtils.EvaluateExceptionType("abcdefghij"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("5.toString"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("qwerty345.prop"));
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
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("x = {}; x.toString()"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {a: 1}.a"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {a: 1, }.a"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {a: 1, b: 2}.a"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {'a': 1, 'b': 2}.a"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {0: 1, 1: 2}[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = {a: 1, b: 2}.b"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = {a: 1, a: 2}.a"));   // This is an error in strict mode.
            Assert.AreEqual(3, TestUtils.Evaluate("var obj = {valueOf:0, toString:1, foo:2}; x = 0; for (var y in obj) { x ++; } x"));

            // Keywords are allowed in ES5.
            Assert.AreEqual(1, TestUtils.Evaluate("x = {if: 1}; x.if"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {eval: 1}; x.eval"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {false: 1}; x.false"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {true: 1}; x.true"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {null: 1}; x.null"));

            // Object literals can have getters and setters.
            Assert.AreEqual(1, TestUtils.Evaluate("x = {get f() { return 1; }}; x.f"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = {set f(value) { this.a = value; }}; x.f = 5; x.a"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = {get f() { return this.a; }, set f(value) { this.a = value; }}; x.f = 5; x.f"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {get f() { return 1; }}; x.f = 5; x.f"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = {set f(value) { this.a = value; }}; x.f"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = {get: 2}; x.get"));
            Assert.AreEqual(3, TestUtils.Evaluate("x = {set: 3}; x.set"));

            // Errors
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("{a: 1, b: 2}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {a: 1,, }.a"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {get a() { return 2 }, get a() { return 2 }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {set a(value) { }, set a(value) { }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {a: 1, get a() { return 2 }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {a: 1, set a(value) { }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {get a() { return 2 }, a: 1}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {set a(value) { }, a: 1}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {set a() { }, a: 1}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {'get' f() { return 1; }}"));
        }

        [TestMethod]
        public void Delete()
        {
            // Delete property.
            Assert.AreEqual(true, TestUtils.Evaluate("x = {a: 1, b: 2}; delete x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = {a: 1, b: 2}; delete(x.a)"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = {a: 1, b: 2}; delete x.a; x.a"));

            // Delete does not operate against the prototype chain.
            Assert.AreEqual(true, TestUtils.Evaluate("x = Object.create({a: 1}); delete x.a"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = Object.create({a: 1}); delete x.a; x.a"));

            // Delete non-configurable property.
            Assert.AreEqual(false, TestUtils.Evaluate("delete Number.prototype"));

            // Delete global variable.
            Assert.AreEqual(true, TestUtils.Evaluate("abcdefg = 1; delete abcdefg"));
            Assert.AreEqual(false, TestUtils.Evaluate("abcdefg = 1; delete abcdefg; this.hasOwnProperty('abcdefg')"));
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "ReferenceError", TestUtils.EvaluateExceptionType("x = 5; delete x; x"));

            // Deleting function variables fails.
            Assert.AreEqual(false, TestUtils.Evaluate("(function f(a) { return delete a; })(1)"));
            Assert.AreEqual(1, TestUtils.Evaluate("(function f(a) { delete a; return a; })(1)"));

            // Delete non-reference.
            Assert.AreEqual(true, TestUtils.Evaluate("delete 5"));  // does nothing

            // Delete this in an object scope.
            Assert.AreEqual(true, TestUtils.Evaluate("delete this"));

            // Delete this in a function scope.
            // IE: throws an error
            // Firefox: true
            // Chrome: false
            Assert.AreEqual(true, TestUtils.Evaluate("var f = function () { return delete this; }; var x = {'a': 1, 'f': f}; x.f()"));

            // Delete from a parent scope.
            Assert.AreEqual(false, TestUtils.Evaluate("a = 5; function f() { delete a } f(); this.hasOwnProperty('a')"));

            // Deleting variables defined within an eval statement inside a function scope succeeds.
            Assert.AreEqual(true, TestUtils.Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); return delete b; })()"));
            Assert.AreEqual(1, TestUtils.Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); delete b; })(); b;"));
            Assert.AreEqual(1, TestUtils.Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); delete b; return b; })()"));

            // Make sure delete calls functions.
            Assert.AreEqual(true, TestUtils.Evaluate("called = false; function f() { called = true; } delete f(); called"));
        }

        [TestMethod]
        public void Typeof()
        {
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof abcdefg"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof undefined"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof Math.abcdefg"));
            Assert.AreEqual("boolean", TestUtils.Evaluate("typeof true"));
            Assert.AreEqual("number", TestUtils.Evaluate("typeof 1"));
            Assert.AreEqual("number", TestUtils.Evaluate("typeof (NaN)"));
            Assert.AreEqual("number", TestUtils.Evaluate("typeof 1.5"));
            Assert.AreEqual("string", TestUtils.Evaluate("typeof 'hello'"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof /abc/"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof {}"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof []"));
            Assert.AreEqual("function", TestUtils.Evaluate("typeof Math.toString"));
            Assert.AreEqual("number", TestUtils.Evaluate("x = 1.5; typeof x"));
            Assert.AreEqual("string", TestUtils.Evaluate("x = 'hello'; typeof x"));
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
            Assert.AreEqual(6, TestUtils.Evaluate("x = 6; test: with (x) { break test; 5 }"));

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
            Assert.AreEqual(5, TestUtils.Evaluate("function f() { return 5 } f()"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("function f() { } f()"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("function f() { return } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("return 5"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("eval('return 5')"));
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
            Assert.AreEqual(6, TestUtils.Evaluate("b = 5; with (x) { y = b; x.b = 6; b; }"));
            Assert.AreEqual(3, TestUtils.Evaluate("x = Object.create({b: 3}); b = 2; with (x) { b }"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, b = 13 } x.b"));
            Assert.AreEqual("number", TestUtils.Evaluate("x = { a: 234 }; a = 1; b = 2; with (x) { a = 12, typeof(b) }"));

            // Implicit this.
            Assert.AreEqual(1970, TestUtils.Evaluate("x = new Date(5); x.f = x.getFullYear; with (x) { f() }"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = { a: 1, b: 2 }; with (x) { (function() { return this })() === this }"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("x = new Date(5); f = x.getFullYear; with (x) { f() }"));
            Assert.AreEqual(1970, TestUtils.Evaluate("x = new Date(5); x.f = x.getFullYear; with (x) { (function b() { return f() })() }"));

            // With and var.
            Assert.AreEqual(5, TestUtils.Evaluate("x = { a: 234 }; with (x) { var a = 5; } x.a"));
            Assert.AreEqual(0, TestUtils.Evaluate("a = 0; x = { a: 234 }; with (x) { var a = 5; } a"));
            Assert.AreEqual(5, TestUtils.Evaluate("b = 0; x = { a: 234 }; with (x) { var b = 5; } b"));
            Assert.AreEqual(4, TestUtils.Evaluate("foo = {x: 4}; with (foo) { var x; x }"));

            // With and prototype chains.
            Assert.AreEqual(10, TestUtils.Evaluate("x = Object.create({ b: 5 }); with (x) { b = 10 } x.b"));
            Assert.AreEqual(5, TestUtils.Evaluate("x = Object.create({ b: 5 }); with (x) { b = 10 } Object.getPrototypeOf(x).b"));

            // With statements are syntax errors in strict mode.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; with (x) { }"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"eval(""'use strict'; var o = {}; with (o) {}"")"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"'use strict'; eval(""var o = {}; with (o) {}"")"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType(@"eval(""function f() { 'use strict'; var o = {}; with (o) {} }"")"));
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
            Assert.AreEqual(1, TestUtils.Evaluate("switch (5) { default: 3; case 4: 1 }"));
            Assert.AreEqual(5, TestUtils.Evaluate("(function(x) { switch (x) { case 8: return 4; case 9: return 5; default: return 7 } })(9)"));
        }

        [TestMethod]
        public void Throw()
        {
            Assert.AreEqual("Error", TestUtils.EvaluateExceptionType("throw new Error('test')"));
        }

        [TestMethod]
        public void TryCatchFinally()
        {
            // Try to catch various values.
            Assert.AreEqual(5, TestUtils.Evaluate("try { throw 5 } catch (e) { e }"));
            Assert.AreEqual("test", TestUtils.Evaluate("try { throw 'test' } catch (e) { e }"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("try { throw undefined } catch (e) { e }"));
            Assert.AreEqual("test", TestUtils.Evaluate("try { throw new String('test') } catch (e) { e.valueOf() }"));

            // Test the finally block runs in all circumstances.
            Assert.AreEqual(6, TestUtils.Evaluate("try { } finally { 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("try { } catch (e) { e } finally { 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("try { throw 5 } catch (e) { e } finally { 6 }"));
            Assert.AreEqual(6, TestUtils.Evaluate("try { try { throw 5 } finally { 6 } } catch (e) { }"));

            // Exceptions can be swallowed using finally.
            Assert.AreEqual(6, TestUtils.Evaluate("function f() { try { throw 5 } finally { return 6 } } f()"));

            // Catch creates a new scope - but only for the exception variable.
            Assert.AreEqual(5, TestUtils.Evaluate("e = 5; try { throw 6; } catch (e) { } e"));
            Assert.AreEqual(5, TestUtils.Evaluate("e = 5; try { throw 6; } catch (e) { var e = 10; } e"));
            Assert.AreEqual(5, TestUtils.Evaluate("var b = 2; try { throw 6; } catch (e) { var b = 5; } b"));

            // Try without catch or finally is an error.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("try { }"));

            // Cannot declare a function inside a catch block.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("try { throw 6; } catch (e) { function foo() { return 2; } foo() }"));
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
            Assert.AreEqual(4, TestUtils.Evaluate("a(); function a() { return 1 } function a() { return 2 } function a() { return 3 } function a() { return 4 }"));

            // Closures.
            Assert.AreEqual(7, TestUtils.Evaluate("function f(a, b, c) { return f2(a + 1); function f2(d) { return d + b + c; } } f(1, 2, 3)"));
            Assert.AreEqual(11, TestUtils.Evaluate("function makeAdder(a) { return function(b) { return a + b; } } x = makeAdder(5); x(6)"));
            Assert.AreEqual(27, TestUtils.Evaluate("function makeAdder(a) { return function(b) { return a + b; } } y = makeAdder(20); y(7)"));

            // Function expressions.
            Assert.AreEqual(3, TestUtils.Evaluate("var f = function () { return 3; }; f()"));

            // This
            Assert.AreEqual(1, TestUtils.Evaluate("var f = function () { return this.a; }; var x = {'a': 1, 'f': f}; x.f()"));

            // Function expressions are not visible in the local scope.
            Assert.AreEqual(1, TestUtils.Evaluate("function f() {return 1} x = function f() {return 2}; f()"));

            // The function name is defined in the function scope.
            Assert.AreEqual(24, TestUtils.Evaluate("var fact='test'; Math.factorial = function fact(n) {return n<=1?1:n*fact(n-1)}; Math.factorial(4)"));

            // Argument names override the function name and 'arguments'.
            Assert.AreEqual(5, TestUtils.Evaluate("(function test(test) { return test; })(5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("function test(test) { return test; } test(5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("(function f(arguments) { return arguments })(5)"));
            Assert.AreEqual(5, TestUtils.Evaluate("(function f(a) { arguments = 5; return arguments })(5)"));
            Assert.AreEqual("[object Arguments]", TestUtils.Evaluate("function arguments() { return arguments } arguments().toString()"));
        }

        [TestMethod]
        public void This()
        {
            // "this" is set to the global object by default.
            Assert.AreEqual(5, TestUtils.Evaluate("this.x = 5; this.x"));

            // In ES3 functions will get the global object as the "this" value by default.
            Assert.AreEqual(true, TestUtils.Evaluate("(function(){ return this; }).call(null) === this"));
            Assert.AreEqual(true, TestUtils.Evaluate("(function(){ return this; }).call(undefined) === this"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof (function(){ return this; }).call(5)"));

            // Check that the this parameter is passed correctly.
            Assert.AreEqual(true, TestUtils.Evaluate("x = { f: function() { return this } }; x.f() === x"));
            Assert.AreEqual(5, TestUtils.Evaluate("function x() { this.a = 5; this.f = function() { return this } }; new x().f().a"));

            // The "this" value cannot be modified.
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("this = 5;"));
        }

        [TestMethod]
        public void Lexer()
        {
            // Octal numbers and escape sequences are not supported.
            Assert.AreEqual(0, TestUtils.Evaluate("0"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("05"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("09"));
            Assert.AreEqual("\0", TestUtils.Evaluate("'\\0'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'\\05'"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'\\09'"));

            // Division and regular expressions are ambiguous in the lexical grammar.  The parser
            // is required to resolve the ambiguity.
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

            TestUtils.Evaluate("x = { a: 1 }; y = x.a \r\n var b;");
        }

        [TestMethod]
        public void StrictMode()
        {
            // Attempts to set a variable that has not been declared is disallowed.
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("'use strict'; asddfsgwqewert = 'test'"));

            // Cannot write to a non-writable property.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}); x.a = 5;"));

            // Cannot write to a non-existant property when the object is non-extensible.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; Object.preventExtensions(x); x.a = 5;"));

            // Cannot write to a property that has a getter but no setter.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; Object.defineProperty(x, 'a', {get: function() { return 1 }}); x.a = 5;"));

            // Cannot delete a non-configurable property.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: false}); delete x.a;"));

            // Deleting a variable fails.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; var foo = 'test'; delete foo"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function test(){} delete test"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; (function(arg) { delete arg; })()"));

            // Defining a property more than once fails.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; var x = {a: 1, a: 2};"));

            // Eval is allowed for a object literal property.
            Assert.AreEqual(1, TestUtils.Evaluate("'use strict'; x = {eval: 1}; x.eval"));

            // Attempts to use the name "eval" are not allowed in strict mode.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; var eval = 5"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; for (var eval in {a:1}) {}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function eval(){}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function test(eval){}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function(eval){}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; new Function('eval', '')"));

            // Eval has it's own scope.
            Assert.AreEqual("undefined", TestUtils.Evaluate("'use strict'; eval('var a = false'); typeof a"));

            // The "this" object is not coerced to an object.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate("'use strict'; (function(){ return this; }).call(null)"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'use strict'; (function(){ return this; }).call(undefined)"));
            Assert.AreEqual("number", TestUtils.Evaluate("'use strict'; typeof (function(){ return this; }).call(5)"));

            // Argument names cannot be identical.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; (function(arg, arg) { })()"));

            // Arguments cannot be redefined.
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("'use strict'; (function(arg) { arguments = 5; })()"));

            // Arguments and caller don't exist outside the function.
            Assert.AreEqual("undefined", TestUtils.EvaluateExceptionType("'use strict'; function test(){ function inner(){ return typeof(test.arguments); } return inner(); } test()"));
            Assert.AreEqual("undefined", TestUtils.EvaluateExceptionType("'use strict'; function test(){ function inner(){ return typeof(inner.caller); } return inner(); } test()"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("'use strict'; function test(){ function inner(){ test.arguments = 5; } return inner(); } test()"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("'use strict'; function test(){ function inner(){ inner.caller = 5; } return inner(); } test()"));
        }
    }
}
