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
    /// Language expression tests.
    /// </summary>
    [TestClass]
    public class ExpressionTests
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
            StringAssert.StartsWith((string)TestUtils.Evaluate("5 + new Date(10)"), "5");
            Assert.AreEqual("5/abc/g", TestUtils.Evaluate("5 + /abc/g"));
            Assert.AreEqual("5[object Object]", TestUtils.Evaluate("5 + {}"));

            // Objects
            Assert.AreEqual(11, TestUtils.Evaluate("new Number(5) + new Number(6)"));
            Assert.AreEqual("test6", TestUtils.Evaluate("'test' + new Number(6)"));
            Assert.AreEqual("5test", TestUtils.Evaluate("new Number(5) + 'test'"));
            Assert.AreEqual("1", TestUtils.Evaluate("({valueOf: function() {return 1}, toString: function() {return 0}}) + ''"));
            Assert.AreEqual("1test", TestUtils.Evaluate("({valueOf: function() {return 1}, toString: function() {return 0}}) + 'test'"));
            Assert.AreEqual(5, TestUtils.Evaluate("({valueOf: function() {return 1}, toString: function() {return 0}}) + 4"));
            Assert.AreEqual("14", TestUtils.Evaluate("({valueOf: function() {return '1'}, toString: function() {return 0}}) + 4"));
            Assert.AreEqual("1", TestUtils.Evaluate("'' + {valueOf: function() {return 1}, toString: function() {return 0}}"));
            Assert.AreEqual("test1", TestUtils.Evaluate("'test' + {valueOf: function() {return 1}, toString: function() {return 0}}"));
            Assert.AreEqual(3, TestUtils.Evaluate("1 + {valueOf: function() {return 2}, toString: function() {return 3}}"));
            Assert.AreEqual("12", TestUtils.Evaluate("1 + {valueOf: function() {return '2'}, toString: function() {return '3'}}"));

            // Variables
            Assert.AreEqual(35, TestUtils.Evaluate("x = 15; x + 20"));
            Assert.AreEqual(21.5, TestUtils.Evaluate("x = 1.5; x + 20"));
            Assert.AreEqual(8589934608.0, TestUtils.Evaluate("x = 4294967304; x + 4294967304"));
            Assert.AreEqual("testing", TestUtils.Evaluate("x = 'tes'; x + 'ting'"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = true; x + false"));
            Assert.AreEqual("102", TestUtils.Evaluate("x = 2; '10' + x"));
            Assert.AreEqual("10null", TestUtils.Evaluate("x = '10'; x + null"));
            Assert.AreEqual("51,2,3", TestUtils.Evaluate("x = 5; x + [1,2,3]"));
            StringAssert.StartsWith((string)TestUtils.Evaluate("x = 5; x + new Date(10)"), "5");
            Assert.AreEqual("5/abc/g", TestUtils.Evaluate("x = 5; x + /abc/g"));
            Assert.AreEqual("5[object Object]", TestUtils.Evaluate("x = 5; x + {}"));

            // String concatenation.
            Assert.AreEqual("123456123789", TestUtils.Evaluate(@"
                a = '123';
                b = '456';
                c = '789';
                d = a + b;
                e = a + c;
                d + e"));
            Assert.AreEqual("12451278", TestUtils.Evaluate(@"
                a = '1' + '2';
                b = '4' + '5';
                c = '7' + '8';
                d = a + b;
                e = a + c;
                d + e"));
            Assert.AreEqual("abcdefghi", TestUtils.Evaluate("a = 'abc'; b = 'ghi'; a += 'def' + b"));
            Assert.AreEqual(@"([A-Za-z_:]|[^\x00-\x7F])([A-Za-z0-9_:.-]|[^\x00-\x7F])*(\?>|[\n\r\t ][^?]*\?+([^>?][^?]*\?+)*>)?", TestUtils.Evaluate(@"
                TextSE = ""[^<]+"";
                UntilHyphen = ""[^-]*-"";
                Until2Hyphens = UntilHyphen + ""([^-]"" + UntilHyphen + "")*-"";
                CommentCE = Until2Hyphens + "">?"";
                UntilRSBs = ""[^]]*]([^]]+])*]+"";
                CDATA_CE = UntilRSBs + ""([^]>]"" + UntilRSBs + "")*>"";
                S = ""[ \\n\\t\\r]+"";
                NameStrt = ""[A-Za-z_:]|[^\\x00-\\x7F]"";
                NameChar = ""[A-Za-z0-9_:.-]|[^\\x00-\\x7F]"";
                Name = ""("" + NameStrt + "")("" + NameChar + "")*"";
                QuoteSE = '""[^""]' + ""*"" + '""' + ""|'[^']*'"";
                DT_IdentSE = S + Name + ""("" + S + ""("" + Name + ""|"" + QuoteSE + ""))*"";
                MarkupDeclCE = ""([^]\""'><]+|"" + QuoteSE + "")*>"";
                S1 = ""[\\n\\r\\t ]"";
                UntilQMs = ""[^?]*\\?+"";
                PI_Tail = ""\\?>|"" + S1 + UntilQMs + ""([^>?]"" + UntilQMs + "")*>"";
                DT_ItemSE = ""<(!(--"" + Until2Hyphens + "">|[^-]"" + MarkupDeclCE + "")|\\?"" + Name + ""("" + PI_Tail + ""))|%"" + Name + "";|"" + S;
                DocTypeCE = DT_IdentSE + ""("" + S + "")?(\\[("" + DT_ItemSE + "")*]("" + S + "")?)?>?"";
                DeclCE = ""--("" + CommentCE + "")?|\\[CDATA\\[("" + CDATA_CE + "")?|DOCTYPE("" + DocTypeCE + "")?"";
                PI_CE = Name + ""("" + PI_Tail + "")?"";
                PI_CE"));
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
            Assert.AreEqual(double.PositiveInfinity, TestUtils.Evaluate("10 / 0"));
            Assert.AreEqual(double.NegativeInfinity, TestUtils.Evaluate("-10 / 0"));

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
            Assert.AreEqual(false, TestUtils.Evaluate("''         ==   '0'           "));
            Assert.AreEqual(true,  TestUtils.Evaluate("0          ==   ''            "));
            Assert.AreEqual(true,  TestUtils.Evaluate("0          ==   '0'           "));
            Assert.AreEqual(false, TestUtils.Evaluate("false      ==   'false'       "));
            Assert.AreEqual(true,  TestUtils.Evaluate("false      ==   '0'           "));
            Assert.AreEqual(false, TestUtils.Evaluate("false      ==   undefined     "));
            Assert.AreEqual(false, TestUtils.Evaluate("false      ==   null          "));
            Assert.AreEqual(true,  TestUtils.Evaluate("null       ==   undefined     "));
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

            // Objects
            Assert.AreEqual(false, TestUtils.Evaluate("0 < {valueOf: function() {return -2}, toString: function() {return '2'}}"));
            Assert.AreEqual(false, TestUtils.Evaluate("'0' < {valueOf: function() {return -2}, toString: function() {return '2'}}"));
            Assert.AreEqual(true, TestUtils.Evaluate("({valueOf: function() {return -2}, toString: function() {return '2'}}) < 0"));
            Assert.AreEqual(true, TestUtils.Evaluate("({valueOf: function() {return -2}, toString: function() {return '2'}}) < '0'"));
            Assert.AreEqual(false, TestUtils.Evaluate("var object = {valueOf: function() {return '-2'}, toString: function() {return 2}}; object < '-1'"));

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", TestUtils.Evaluate(@"
                var x = { valueOf: function () { throw 'x'; } };
                var y = { valueOf: function () { throw 'y'; } };
                try { x < y; } catch (e) { e }"));
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

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", TestUtils.Evaluate(@"
                var x = { valueOf: function () { throw 'x'; } };
                var y = { valueOf: function () { throw 'y'; } };
                try { x <= y; } catch (e) { e }"));
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

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", TestUtils.Evaluate(@"
                var x = { valueOf: function () { throw 'x'; } };
                var y = { valueOf: function () { throw 'y'; } };
                try { x >= y; } catch (e) { e }"));
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

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", TestUtils.Evaluate(@"
                var x = { valueOf: function () { throw 'x'; } };
                var y = { valueOf: function () { throw 'y'; } };
                try { x >= y; } catch (e) { e }"));
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
            Assert.AreEqual(3, TestUtils.Evaluate("Object((1, 2, 3)).valueOf()"));
            Assert.AreEqual(3, TestUtils.Evaluate("Object((1, 2, 3), 4, 5, 6).valueOf()"));
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

            // Strict mode: attempts to set a variable that has not been declared is disallowed.
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("'use strict'; asddfsgwqewert = 'test'"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("function foo() { 'use strict'; asddfsgwqewert = 'test'; } foo()"));

            // Strict mode: cannot write to a non-writable property.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}); x.a = 5;"));

            // Strict mode: cannot write to a non-existant property when the object is non-extensible.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; Object.preventExtensions(x); x.a = 5;"));

            // Strict mode: cannot write to a property that has a getter but no setter.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; Object.defineProperty(x, 'a', {get: function() { return 1 }}); x.a = 5;"));

            // Strict mode: left-hand side cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; eval = 5;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; arguments = 5;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { eval = 5; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { arguments = 5; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; eval = 5; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; arguments = 5; } f()"));

            // Strict mode: left-hand side cannot be 'eval' or 'arguments' (compound assignment).
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; eval += 5;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; arguments += 5;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { eval += 5; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { arguments += 5; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; eval += 5; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; arguments += 5; } f()"));
        }

        [TestMethod]
        public void PreIncrement()
        {
            Assert.AreEqual(1, TestUtils.Evaluate("x = 0; ++ x"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 0; ++ x; x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("++ 2"));

            // Strict mode: reference cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; ++ eval;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; ++ arguments;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { ++ eval; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { ++ arguments; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; ++ eval; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; ++ arguments; } f()"));
        }

        [TestMethod]
        public void PreDecrement()
        {
            Assert.AreEqual(-1, TestUtils.Evaluate("x = 0; -- x"));
            Assert.AreEqual(-1, TestUtils.Evaluate("x = 0; -- x; x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("-- 2"));

            // Strict mode: reference cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; -- eval;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; -- arguments;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { -- eval; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { -- arguments; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; -- eval; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; -- arguments; } f()"));
        }

        [TestMethod]
        public void PostIncrement()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; x ++"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = 0; x ++; x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("2 ++"));

            // Strict mode: left-hand side cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; eval ++;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; arguments ++;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { eval ++; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { arguments ++; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; eval ++; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; arguments ++; } f()"));
        }

        [TestMethod]
        public void PostDecrement()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("x = 0; x --"));
            Assert.AreEqual(-1, TestUtils.Evaluate("x = 0; x --; x"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("2 --"));

            // Strict mode: left-hand side cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; eval --;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; arguments --;"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { eval --; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function f() { arguments --; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; eval --; } f()"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("function f() { 'use strict'; arguments --; } f()"));
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

            // Arguments should only be evaluated once.
            Assert.AreEqual(1, TestUtils.Evaluate("var i = 0; Function({ toString: function() { return ++i } }).apply(null); i"));

            // In compatibility mode, undefined and null are converted to objects.
            TestUtils.CompatibilityMode = CompatibilityMode.ECMAScript3;
            try
            {
                Assert.AreEqual(true, TestUtils.Evaluate("hasOwnProperty('NaN')"));
                Assert.AreEqual(true, TestUtils.Evaluate("hasOwnProperty.call(null, 'NaN')"));
                Assert.AreEqual(true, TestUtils.Evaluate("hasOwnProperty.call(undefined, 'NaN')"));
            }
            finally
            {
                TestUtils.CompatibilityMode = CompatibilityMode.Latest;
            }

            // 'arguments' and 'caller' must be undefined in strict mode.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'use strict'; function test(){ function inner(){ return test.arguments; } return inner(); } test()"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'use strict'; function test(){ function inner(){ return inner.caller; } return inner(); } test()"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'use strict'; function test(){ function inner(){ test.arguments = 5; } return inner(); } test()"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'use strict'; function test(){ function inner(){ inner.caller = 5; } return inner(); } test()"));
        }

        [TestMethod]
        public void New()
        {
            Assert.AreEqual("five", TestUtils.Evaluate("(new String('five')).toString()"));
            Assert.AreEqual("five", TestUtils.Evaluate("new String('five').toString()"));
            Assert.AreEqual("5", TestUtils.Evaluate("new Number(5).toString()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new (String('five'))"));

            // Precedence tests.
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("x = {}; x.f = function() { }; (new x.f()).toString()"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("(new Function.valueOf)()"));  // new (Function.valueOf) is not a function.
            Assert.AreEqual("function anonymous() {\n\n}", TestUtils.Evaluate("(new (Function.valueOf())).toString()"));
            Assert.AreEqual("function anonymous() {\n\n}", TestUtils.Evaluate("((new Function).valueOf()).toString()"));
            Assert.AreEqual("[object Object]", TestUtils.Evaluate("x = {}; x.f = function() { }; (new (x.f)()).toString()"));

            // New user-defined function.
            Assert.AreEqual(5, TestUtils.Evaluate("function f() { this.a = 5; return 2; }; a = new f(); a.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("function f() { this.a = 5; return 2; }; a = new f(); Object.getPrototypeOf(a) === f.prototype"));

            // Returning an object returns that as the new object.
            Assert.AreEqual(6, TestUtils.Evaluate("function f() { this.a = 5; return { a: 6 }; }; x = new f(); x.a"));

            // Built-in functions cannot be constructed.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("new Function.valueOf()"));
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

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", TestUtils.Evaluate(@"
                var x = function () { throw 'x'; };
                var y = function () { throw 'y'; };
                try { x() instanceof y(); } catch (e) { e }"));
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

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", TestUtils.Evaluate(@"
                var x = function () { throw 'x'; };
                var y = function () { throw 'y'; };
                try { x() in y(); } catch (e) { e }"));
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

            // Check details of hidden class functionality.
            Assert.AreEqual(2, TestUtils.Evaluate("x = {}; x.a = 6; x.a = 2; x.a"));
            Assert.AreEqual(3, TestUtils.Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b"));
            Assert.AreEqual(3, TestUtils.Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b = 4; y.a"));
            Assert.AreEqual(4, TestUtils.Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b = 4; y.b"));
            Assert.AreEqual(3, TestUtils.Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b = 4; delete y.b; y.a"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b = 4; delete y.b; y.b"));

            // Ensure you can create at least 16384 properties.
            Assert.AreEqual(16383, TestUtils.Evaluate(@"
                var x = new Object();
                for (var i = 0; i < 16384; i ++)
                    x['prop' + i] = i;
                x.prop16383"));

            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "ReferenceError", TestUtils.EvaluateExceptionType("abcdefghij"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("5.toString"));
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("qwerty345.prop"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("null.prop"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("undefined.prop"));
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
            Assert.AreEqual(1, TestUtils.Evaluate("x = {get 'f'() { return 1; }}; x.f = 5; x.f"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = {get 0() { return 1; }}; x[0] = 5; x[0]"));
            Assert.AreEqual(4, TestUtils.Evaluate("var f = 4; x = {get f() { return f; }}; x.f"));

            // Check that "this" is correct inside getters and setters.
            Assert.AreEqual(9, TestUtils.Evaluate("x = { get b() { return this.a; } }; y = Object.create(x); y.a = 9; y.b"));
            Assert.AreEqual(9, TestUtils.Evaluate("x = { get b() { return this.a; } }; y = Object.create(x); y.a = 9; z = 'b'; y[z]"));
            Assert.AreEqual(9, TestUtils.Evaluate("x = { get '2'() { return this.a; } }; y = Object.create(x); y.a = 9; y[2]"));
            Assert.AreEqual(9, TestUtils.Evaluate("x = { get '2'() { return this.a; } }; y = Object.create(x); y.a = 9; z = 2; y[z]"));
            Assert.AreEqual(9, TestUtils.Evaluate("x = { set b(value) { this.a = value; } }; y = Object.create(x); y.b = 9; y.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = { set b(value) { this.a = value; } }; y = Object.create(x); y.b = 9; y.hasOwnProperty('a')"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = { set b(value) { this.a = value; } }; y = Object.create(x); z = 'b'; y[z] = 9; y.hasOwnProperty('a')"));

            // Errors
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("{a: 1, b: 2}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {a: 1,, }.a"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {get a() { return 2 }, get a() { return 2 }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {set a(value) { }, set a(value) { }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {a: 1, get a() { return 2 }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {a: 1, set a(value) { }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {get a() { return 2 }, a: 1}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {set a(value) { }, a: 1}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {'get' f() { return 1; }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {get f(a) { return 1; }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {set f() { return 1; }}"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("x = {set f(a, b) { return 1; }}"));

            // Strict mode: defining a property more than once fails.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; var x = {a: 1, a: 2};"));

            // Strict mode: eval is allowed in a object literal property.
            Assert.AreEqual(1, TestUtils.Evaluate("'use strict'; x = {eval: 1}; x.eval"));
        }

        [TestMethod]
        public void Delete()
        {
            // Delete property.
            Assert.AreEqual(true, TestUtils.Evaluate("x = {a: 1, b: 2}; delete x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = {a: 1, b: 2}; delete(x.a)"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = {a: 1, b: 2}; delete x.a; x.a"));

            // Delete property (alternate syntax).
            Assert.AreEqual(true, TestUtils.Evaluate("x = {a: 1, b: 2}; delete x['a']"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = {a: 1, b: 2}; delete x['a']; x.a"));
            Assert.AreEqual(true, TestUtils.Evaluate("x = {a: 1, b: 2}; y = 'a'; delete x[y]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x = {a: 1, b: 2}; y = 'a'; delete x[y]; x.a"));

            // Delete does not operate against the prototype chain.
            Assert.AreEqual(true, TestUtils.Evaluate("x = Object.create({a: 1}); delete x.a"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = Object.create({a: 1}); delete x.a; x.a"));

            // Delete non-configurable property.
            Assert.AreEqual(false, TestUtils.Evaluate("delete Number.prototype"));

            // Deleting a global variable fails.
            TestUtils.Execute("var delete_test_1 = 1; var delete_test_2 = delete delete_test_1;");
            Assert.AreEqual(1, TestUtils.Evaluate("delete_test_1"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete_test_2"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(this, 'delete_test_1').configurable"));

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

            // Deleting variables defined within an eval statement inside a global scope succeeds.
            Assert.AreEqual(true, TestUtils.Evaluate("abcdefg = 1; delete abcdefg"));
            Assert.AreEqual(false, TestUtils.Evaluate("abcdefg = 1; delete abcdefg; this.hasOwnProperty('abcdefg')"));
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "TypeError" : "ReferenceError", TestUtils.EvaluateExceptionType("x = 5; delete x; x"));

            // Deleting variables defined within an eval statement inside a function scope succeeds.
            Assert.AreEqual(true, TestUtils.Evaluate("(function() { var a = 5; return eval('var b = a; delete b'); })()"));
            Assert.AreEqual(true, TestUtils.Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); return delete b; })()"));
            Assert.AreEqual(1, TestUtils.Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); delete b; })(); b;"));
            Assert.AreEqual(1, TestUtils.Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); delete b; return b; })()"));

            // Make sure delete calls functions.
            Assert.AreEqual(true, TestUtils.Evaluate("called = false; function f() { called = true; } delete f(); called"));

            // Strict mode: cannot delete a non-configurable property.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("'use strict'; var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: false}); delete x.a;"));

            // Strict mode: deleting a variable fails.
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; var foo = 'test'; delete foo"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; function test(){} delete test"));
            Assert.AreEqual("SyntaxError", TestUtils.EvaluateExceptionType("'use strict'; (function(arg) { delete arg; })()"));
        }

        [TestMethod]
        public void Typeof()
        {
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof abcdefg"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof undefined"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof null"));
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
            Assert.AreEqual("number", TestUtils.Evaluate("typeof([1, 2].length)"));
        }

        [TestMethod]
        public void This()
        {
            // "this" is set to the global object by default.
            Assert.AreEqual(5, TestUtils.Evaluate("this.x = 5; this.x"));

            // In ES3 functions will get the global object as the "this" value by default.
            Assert.AreEqual(true, TestUtils.Evaluate("(function(){ return this; }).call(null) === this"));
            Assert.AreEqual(true, TestUtils.Evaluate("(function(){ return this; }).call(undefined) === this"));
            Assert.AreEqual(6, TestUtils.Evaluate("(function(){ return this; }).call(5) + 1"));
            Assert.AreEqual("object", TestUtils.Evaluate("typeof((function(){ return this; }).call(5))"));
            Assert.AreEqual(6, TestUtils.Evaluate("(function(){ return eval('this'); }).call(5) + 1"));

            // Check that the this parameter is passed correctly.
            Assert.AreEqual(true, TestUtils.Evaluate("x = { f: function() { return this } }; x.f() === x"));
            Assert.AreEqual(5, TestUtils.Evaluate("function x() { this.a = 5; this.f = function() { return this } }; new x().f().a"));

            // The "this" value cannot be modified.
            Assert.AreEqual("ReferenceError", TestUtils.EvaluateExceptionType("this = 5;"));

            // Strict mode: the "this" object is not coerced to an object.
            Assert.AreEqual(Null.Value, TestUtils.Evaluate("'use strict'; (function(){ return this; }).call(null)"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("'use strict'; (function(){ return this; }).call(undefined)"));
            Assert.AreEqual(5, TestUtils.Evaluate("'use strict'; (function(){ return this; }).call(5)"));
            Assert.AreEqual("number", TestUtils.Evaluate("'use strict'; typeof((function(){ return this; }).call(5))"));
        }
    }
}
