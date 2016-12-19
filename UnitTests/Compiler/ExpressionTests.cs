using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Language expression tests.
    /// </summary>
    [TestClass]
    public class ExpressionTests : TestBase
    {
        [TestMethod]
        public void UnaryPlus()
        {
            Assert.AreEqual(+20, Evaluate("+20"));
            Assert.AreEqual(5, Evaluate("+ '5'"));
            Assert.AreEqual(double.NaN, Evaluate("+'Hello'"));
            Assert.AreEqual(1, Evaluate("+true"));
            Assert.AreEqual(-5, Evaluate("x = '-5'; +x"));
            Assert.AreEqual(1e20, Evaluate("+1e20"));
            Assert.AreEqual(3.1415, Evaluate("+3.1415"));
            Assert.AreEqual(5, Evaluate("+new Date(5)"));
            Assert.AreEqual(double.NaN, Evaluate("+new Object()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("+Symbol()"));
        }

        [TestMethod]
        public void UnaryMinus()
        {
            Assert.AreEqual(-20, Evaluate("-20"));
            Assert.AreEqual(-5, Evaluate("- '5'"));
            Assert.AreEqual(-1, Evaluate("-true"));
            Assert.AreEqual(double.NaN, Evaluate("-'Hello'"));
            Assert.AreEqual(-5, Evaluate("x = '5'; -x"));
            Assert.AreEqual(-1e20, Evaluate("-1e20"));
            Assert.AreEqual(-3.1415, Evaluate("-3.1415"));
            Assert.AreEqual(-5, Evaluate("-new Date(5)"));
            Assert.AreEqual(double.NaN, Evaluate("-new Object()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("-Symbol()"));
        }

        [TestMethod]
        public void BitwiseNot()
        {
            Assert.AreEqual(-1, Evaluate("~0"));
            Assert.AreEqual(-21, Evaluate("~20"));
            Assert.AreEqual(19, Evaluate("~-20"));
            Assert.AreEqual(-9, Evaluate("~4294967304"));
            Assert.AreEqual(-21, Evaluate("~ '20'"));

            // Double bitwise not converts the input to a Int32.
            Assert.AreEqual(1, Evaluate("~~'1.2'"));
            Assert.AreEqual(-1, Evaluate("~~'-1.2'"));
            Assert.AreEqual(32, Evaluate("~~17179869216"));
            Assert.AreEqual(-2147483638, Evaluate("~~2147483658"));
            Assert.AreEqual(-2147483637, Evaluate("~~6442450955"));

            // Objects
            Assert.AreEqual(-6, Evaluate("~new Number(5)"));

            // Variables
            Assert.AreEqual(-1, Evaluate("x = 0; ~x"));
            Assert.AreEqual(-21, Evaluate("x = 20; ~x"));
            Assert.AreEqual(19, Evaluate("x = -20; ~x"));
            Assert.AreEqual(-9, Evaluate("x = 4294967304; ~x"));
            Assert.AreEqual(-21, Evaluate("x =  '20'; ~x"));
        }

        [TestMethod]
        public void LogicalNot()
        {
            Assert.AreEqual(true, Evaluate("!false"));
            Assert.AreEqual(false, Evaluate("!true"));
            Assert.AreEqual(false, Evaluate("!10"));
            Assert.AreEqual(true, Evaluate("!0"));
            Assert.AreEqual(false, Evaluate("!'hello'"));
            Assert.AreEqual(true, Evaluate("!''"));

            // Objects
            Assert.AreEqual(false, Evaluate("!new Number(5)"));
            Assert.AreEqual(false, Evaluate("!Symbol()"));

            // Variables
            Assert.AreEqual(true, Evaluate("x = false; !x"));
            Assert.AreEqual(false, Evaluate("x = true; !x"));
            Assert.AreEqual(false, Evaluate("x = 10; !x"));
            Assert.AreEqual(true, Evaluate("x = 0; !x"));
            Assert.AreEqual(false, Evaluate("x = 'hello'; !x"));
            Assert.AreEqual(true, Evaluate("x = ''; !x"));
        }

        [TestMethod]
        public void Add()
        {
            Assert.AreEqual(35, Evaluate("15 + 20"));
            Assert.AreEqual(21.5, Evaluate("1.5 + 20"));
            Assert.AreEqual(8589934608.0, Evaluate("4294967304 + 4294967304"));
            Assert.AreEqual("testing", Evaluate("'tes' + 'ting'"));
            Assert.AreEqual(1, Evaluate("true + false"));
            Assert.AreEqual("102", Evaluate("'10' + 2"));
            Assert.AreEqual("10null", Evaluate("'10' + null"));
            Assert.AreEqual("51,2,3", Evaluate("5 + [1,2,3]"));
            StringAssert.StartsWith((string)Evaluate("5 + new Date(10)"), "5");
            Assert.AreEqual("5/abc/g", Evaluate("5 + /abc/g"));
            Assert.AreEqual("5[object Object]", Evaluate("5 + {}"));

            // Objects
            Assert.AreEqual(11, Evaluate("new Number(5) + new Number(6)"));
            Assert.AreEqual("test6", Evaluate("'test' + new Number(6)"));
            Assert.AreEqual("5test", Evaluate("new Number(5) + 'test'"));
            Assert.AreEqual("1", Evaluate("({valueOf: function() {return 1}, toString: function() {return 0}}) + ''"));
            Assert.AreEqual("1test", Evaluate("({valueOf: function() {return 1}, toString: function() {return 0}}) + 'test'"));
            Assert.AreEqual(5, Evaluate("({valueOf: function() {return 1}, toString: function() {return 0}}) + 4"));
            Assert.AreEqual("14", Evaluate("({valueOf: function() {return '1'}, toString: function() {return 0}}) + 4"));
            Assert.AreEqual("1", Evaluate("'' + {valueOf: function() {return 1}, toString: function() {return 0}}"));
            Assert.AreEqual("test1", Evaluate("'test' + {valueOf: function() {return 1}, toString: function() {return 0}}"));
            Assert.AreEqual(3, Evaluate("1 + {valueOf: function() {return 2}, toString: function() {return 3}}"));
            Assert.AreEqual("12", Evaluate("1 + {valueOf: function() {return '2'}, toString: function() {return '3'}}"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("1 + Symbol()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Symbol() + 1"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("Symbol() + new Number(6)"));

            // Variables
            Assert.AreEqual(35, Evaluate("x = 15; x + 20"));
            Assert.AreEqual(21.5, Evaluate("x = 1.5; x + 20"));
            Assert.AreEqual(8589934608.0, Evaluate("x = 4294967304; x + 4294967304"));
            Assert.AreEqual("testing", Evaluate("x = 'tes'; x + 'ting'"));
            Assert.AreEqual(1, Evaluate("x = true; x + false"));
            Assert.AreEqual("102", Evaluate("x = 2; '10' + x"));
            Assert.AreEqual("10null", Evaluate("x = '10'; x + null"));
            Assert.AreEqual("51,2,3", Evaluate("x = 5; x + [1,2,3]"));
            StringAssert.StartsWith((string)Evaluate("x = 5; x + new Date(10)"), "5");
            Assert.AreEqual("5/abc/g", Evaluate("x = 5; x + /abc/g"));
            Assert.AreEqual("5[object Object]", Evaluate("x = 5; x + {}"));
            StringAssert.StartsWith((string)Evaluate("new Date('24 Apr 2010 23:59:57') + new Date('24 Apr 2010 23:59:57')"), "Sat Apr 24");

            // String concatenation.
            Assert.AreEqual("123456123789", Evaluate(@"
                a = '123';
                b = '456';
                c = '789';
                d = a + b;
                e = a + c;
                d + e"));
            Assert.AreEqual("12451278", Evaluate(@"
                a = '1' + '2';
                b = '4' + '5';
                c = '7' + '8';
                d = a + b;
                e = a + c;
                d + e"));
            Assert.AreEqual("abcdefghi", Evaluate("a = 'abc'; b = 'ghi'; a += 'def' + b"));
            Assert.AreEqual(@"([A-Za-z_:]|[^\x00-\x7F])([A-Za-z0-9_:.-]|[^\x00-\x7F])*(\?>|[\n\r\t ][^?]*\?+([^>?][^?]*\?+)*>)?", Evaluate(@"
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
            Assert.AreEqual("TypeError", EvaluateExceptionType("Symbol() + 'test'"));
        }

        [TestMethod]
        public void Subtract()
        {
            Assert.AreEqual(-5,            Evaluate("15 - 20"));
            Assert.AreEqual(-18.5,         Evaluate("1.5 - 20"));
            Assert.AreEqual(-4294967304.0, Evaluate("4294967304 - 8589934608"));
            Assert.AreEqual(double.NaN,    Evaluate("'tes' - 'ting'"));
            Assert.AreEqual(1,             Evaluate("true - false"));
            Assert.AreEqual(8,             Evaluate("'10' - 2"));
            Assert.AreEqual(10,            Evaluate("'10' - null"));
            Assert.AreEqual(-6,            Evaluate("6 - 6 - 6"));

            // Objects
            Assert.AreEqual(-1, Evaluate("new Number(5) - new Number(6)"));
            Assert.AreEqual(double.NaN, Evaluate("'test' - new Number(6)"));
            Assert.AreEqual(double.NaN, Evaluate("new Number(5) - 'test'"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Number(5) - Symbol()"));

            // Variables
            Assert.AreEqual(-5, Evaluate("x = 15; x - 20"));
            Assert.AreEqual(-18.5, Evaluate("x = 20; 1.5 - x"));
            Assert.AreEqual(-4294967304.0, Evaluate("x = 8589934608; 4294967304 - x"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'tes'; x - 'ting'"));
            Assert.AreEqual(1, Evaluate("x = true; x - false"));
            Assert.AreEqual(8, Evaluate("x = '10'; x - 2"));
            Assert.AreEqual(10, Evaluate("x = null; '10' - x"));
        }

        [TestMethod]
        public void Multiply()
        {
            Assert.AreEqual(300, Evaluate("15 * 20"));
            Assert.AreEqual(30, Evaluate("1.5 * 20"));
            Assert.AreEqual(8589934608.0, Evaluate("4294967304 * 2"));
            Assert.AreEqual(double.NaN, Evaluate("'tes' * 'ting'"));
            Assert.AreEqual(1, Evaluate("true * true"));
            Assert.AreEqual(20, Evaluate("'10' * 2"));
            Assert.AreEqual(0, Evaluate("'10' * null"));

            // Objects
            Assert.AreEqual(30, Evaluate("new Number(5) * new Number(6)"));
            Assert.AreEqual(double.NaN, Evaluate("'test' * new Number(6)"));
            Assert.AreEqual(double.NaN, Evaluate("new Number(5) * 'test'"));

            // Variables
            Assert.AreEqual(300, Evaluate("x = 15; x * 20"));
            Assert.AreEqual(30, Evaluate("x = 1.5; x * 20"));
            Assert.AreEqual(8589934608.0, Evaluate("x = 4294967304; x * 2"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'ting'; 'tes' * x"));
            Assert.AreEqual(1, Evaluate("x = true; true * x"));
            Assert.AreEqual(20, Evaluate("x = '10'; x * 2"));
            Assert.AreEqual(0, Evaluate("x = null; '10' * x"));
        }

        [TestMethod]
        public void Divide()
        {
            Assert.AreEqual(0.75, Evaluate("15 / 20"));
            Assert.AreEqual(0.075, Evaluate("1.5 / 20"));
            Assert.AreEqual(2147483652.0, Evaluate("4294967304 / 2"));
            Assert.AreEqual(double.NaN, Evaluate("'tes' / 'ting'"));
            Assert.AreEqual(1, Evaluate("true / true"));
            Assert.AreEqual(5, Evaluate("'10' / 2"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("'10' / null"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("'-10' / null"));
            Assert.AreEqual(double.NaN, Evaluate("0 / 0"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("10 / 0"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("-10 / 0"));

            // Objects
            Assert.AreEqual(2, Evaluate("new Number(12) / new Number(6)"));
            Assert.AreEqual(double.NaN, Evaluate("'test' / new Number(6)"));
            Assert.AreEqual(double.NaN, Evaluate("new Number(5) / 'test'"));

            // Variables
            Assert.AreEqual(0.75, Evaluate("x = 15; x / 20"));
            Assert.AreEqual(0.075, Evaluate("x = 1.5; x / 20"));
            Assert.AreEqual(2147483652.0, Evaluate("x = 4294967304; x / 2"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'ting'; 'tes' / x"));
            Assert.AreEqual(1, Evaluate("x = true; true / x"));
            Assert.AreEqual(5, Evaluate("x = 2; '10' / x"));
            Assert.AreEqual(double.PositiveInfinity, Evaluate("x = '10'; x / null"));
            Assert.AreEqual(double.NegativeInfinity, Evaluate("x = '-10'; x / null"));
            Assert.AreEqual(double.NaN, Evaluate("x = 0; x / 0"));
        }

        [TestMethod]
        public void Remainder()
        {
            Assert.AreEqual(2, Evaluate("17 % 5"));
            Assert.AreEqual(2, Evaluate("17 % -5"));
            Assert.AreEqual(-2, Evaluate("-17 % 5"));
            Assert.AreEqual(-2, Evaluate("-17 % -5"));
            Assert.AreEqual(2.2, (double)Evaluate("17.2 % 5"), 0.00000000000001);
            Assert.AreEqual(2.8, (double)Evaluate("17.8 % 5"), 0.00000000000001);
            Assert.AreEqual(-2.2, (double)Evaluate("-17.2 % 5"), 0.00000000000001);
            Assert.AreEqual(-2.8, (double)Evaluate("-17.8 % 5"), 0.00000000000001);

            // Objects
            Assert.AreEqual(1, Evaluate("new Number(7) % new Number(6)"));
            Assert.AreEqual(double.NaN, Evaluate("'test' % new Number(6)"));
            Assert.AreEqual(double.NaN, Evaluate("new Number(5) % 'test'"));

            // Variables
            Assert.AreEqual(2, Evaluate("x = 17; x % 5"));
            Assert.AreEqual(2, Evaluate("x = -5; 17 % x"));
            Assert.AreEqual(-2, Evaluate("x = 5; -17 % x"));
            Assert.AreEqual(-2, Evaluate("x = -5; -17 % x"));
            Assert.AreEqual(2.2, (double)Evaluate("x = 17.2; x % 5"), 0.00000000000001);
            Assert.AreEqual(2.8, (double)Evaluate("x = 17.8; x % 5"), 0.00000000000001);
            Assert.AreEqual(-2.2, (double)Evaluate("x = -17.2; x % 5"), 0.00000000000001);
            Assert.AreEqual(-2.8, (double)Evaluate("x = -17.8; x % 5"), 0.00000000000001);
        }

        [TestMethod]
        public void LeftShift()
        {
            Assert.AreEqual(40, Evaluate("10 << 2"));
            Assert.AreEqual(-400, Evaluate("-100 << 2"));
            Assert.AreEqual(20, Evaluate("10 << 1.2"));
            Assert.AreEqual(20, Evaluate("10 << 1.8"));
            Assert.AreEqual(16, Evaluate("4294967304 << 1"));
            Assert.AreEqual(0, Evaluate("8 << -2"));

            // Objects
            Assert.AreEqual(448, Evaluate("new Number(7) << new Number(6)"));
            Assert.AreEqual(0, Evaluate("'test' << new Number(6)"));
            Assert.AreEqual(5, Evaluate("new Number(5) << 'test'"));

            // Variables
            Assert.AreEqual(40, Evaluate("x = 10; x << 2"));
            Assert.AreEqual(-400, Evaluate("x = 2; -100 << x"));
            Assert.AreEqual(20, Evaluate("x = 1.2; 10 << x"));
            Assert.AreEqual(20, Evaluate("x = 10; x << 1.8"));
            Assert.AreEqual(16, Evaluate("x = 4294967304; x << 1"));
            Assert.AreEqual(0, Evaluate("x = 8; x << -2"));
        }

        [TestMethod]
        public void SignedRightShift()
        {
            Assert.AreEqual(2, Evaluate("10 >> 2"));
            Assert.AreEqual(-25, Evaluate("-100 >> 2"));
            Assert.AreEqual(5, Evaluate("10 >> 1.2"));
            Assert.AreEqual(5, Evaluate("10 >> 1.8"));
            Assert.AreEqual(4, Evaluate("4294967304 >> 1"));
            Assert.AreEqual(0, Evaluate("8 >> -2"));

            // Signed right shift by zero converts the input to a Int32.
            Assert.AreEqual(32, Evaluate("17179869216 >> 0"));
            Assert.AreEqual(-2147483638, Evaluate("2147483658 >> 0"));
            Assert.AreEqual(-2147483637, Evaluate("6442450955 >> 0"));

            // Objects
            Assert.AreEqual(3, Evaluate("new Number(7) >> new Number(1)"));
            Assert.AreEqual(0, Evaluate("'test' >> new Number(6)"));
            Assert.AreEqual(5, Evaluate("new Number(5) >> 'test'"));

            // Variables
            Assert.AreEqual(2, Evaluate("x = 10; x >> 2"));
            Assert.AreEqual(-25, Evaluate("x = 2; -100 >> x"));
            Assert.AreEqual(5, Evaluate("x = 1.2; 10 >> x"));
            Assert.AreEqual(5, Evaluate("x = 10; x >> 1.8"));
            Assert.AreEqual(4, Evaluate("x = 4294967304; x >> 1"));
            Assert.AreEqual(0, Evaluate("x = -2; 8 >> x"));
        }

        [TestMethod]
        public void UnsignedRightShift()
        {
            Assert.AreEqual(2, Evaluate("10 >>> 2"));
            Assert.AreEqual(1073741799, Evaluate("-100 >>> 2"));
            Assert.AreEqual(5, Evaluate("10 >>> 1.2"));
            Assert.AreEqual(5, Evaluate("10 >>> 1.8"));
            Assert.AreEqual(4, Evaluate("4294967304 >>> 1"));
            Assert.AreEqual(0, Evaluate("8 >>> -2"));

            // Unsigned right shift by zero converts the input to a Uint32.
            Assert.AreEqual(32, Evaluate("17179869216 >>> 0"));
            Assert.AreEqual(2147483658.0, Evaluate("2147483658 >>> 0"));
            Assert.AreEqual(2147483659.0, Evaluate("6442450955 >>> 0"));

            // Objects
            Assert.AreEqual(3, Evaluate("new Number(7) >>> new Number(1)"));
            Assert.AreEqual(0, Evaluate("'test' >>> new Number(6)"));
            Assert.AreEqual(5, Evaluate("new Number(5) >>> 'test'"));

            // Variables
            Assert.AreEqual(2, Evaluate("x = 10; x >>> 2"));
            Assert.AreEqual(1073741799, Evaluate("x = 2; -100 >>> x"));
            Assert.AreEqual(5, Evaluate("x = 1.2; 10 >>> x"));
            Assert.AreEqual(5, Evaluate("x = 10; x >>> 1.8"));
            Assert.AreEqual(4, Evaluate("x = 4294967304; x >>> 1"));
            Assert.AreEqual(0, Evaluate("x = -2; 8 >>> x"));
        }

        [TestMethod]
        public void Void()
        {
            Assert.AreEqual(Undefined.Value, Evaluate("void true"));
            Assert.AreEqual(Undefined.Value, Evaluate("void 2"));
            Assert.AreEqual(Undefined.Value, Evaluate("void 'test'"));

            // Make sure side-effects are still evaluated.
            Assert.AreEqual("abc", Evaluate("void (x = 'abc'); x"));
        }

        [TestMethod]
        public void Equals()
        {
            // Both operands are of the same type.
            Assert.AreEqual(false, Evaluate("false == true"));
            Assert.AreEqual(true, Evaluate("false == false"));
            Assert.AreEqual(true, Evaluate("10 == 10"));
            Assert.AreEqual(false, Evaluate("10 == 11"));
            Assert.AreEqual(true, Evaluate("'test' == 'test'"));
            Assert.AreEqual(false, Evaluate("'test' == 'TEST'"));
            Assert.AreEqual(true, Evaluate("'' == ''"));

            // Operands of different types.
            Assert.AreEqual(false, Evaluate("true == 2"));
            Assert.AreEqual(true, Evaluate("true == 1"));
            Assert.AreEqual(false, Evaluate("2 == true"));
            Assert.AreEqual(true, Evaluate("1 == true"));
            Assert.AreEqual(true, Evaluate("1 == '1'"));
            Assert.AreEqual(false, Evaluate("1 == '0'"));

            // Undefined and null.
            Assert.AreEqual(true, Evaluate("null == null"));
            Assert.AreEqual(true, Evaluate("undefined == undefined"));
            Assert.AreEqual(true, Evaluate("null == undefined"));
            Assert.AreEqual(true, Evaluate("Math.abcdef == Math.abcdefghi"));
            Assert.AreEqual(true, Evaluate("Math.abcdef == undefined"));
            Assert.AreEqual(false, Evaluate("null == 5"));

            // Symbols.
            Assert.AreEqual(false, Evaluate("Symbol() == Symbol()"));
            Assert.AreEqual(false, Evaluate("Symbol('test') == Symbol('test')"));

            // NaN
            Assert.AreEqual(false, Evaluate("NaN == NaN"));

            // Doug Crockford's truth table.
            Assert.AreEqual(false, Evaluate("''         ==   '0'           "));
            Assert.AreEqual(true,  Evaluate("0          ==   ''            "));
            Assert.AreEqual(true,  Evaluate("0          ==   '0'           "));
            Assert.AreEqual(false, Evaluate("false      ==   'false'       "));
            Assert.AreEqual(true,  Evaluate("false      ==   '0'           "));
            Assert.AreEqual(false, Evaluate("false      ==   undefined     "));
            Assert.AreEqual(false, Evaluate("false      ==   null          "));
            Assert.AreEqual(true,  Evaluate("null       ==   undefined     "));
            Assert.AreEqual(true,  Evaluate(@"' \t\r\n' ==   0             "));

            // Variables
            Assert.AreEqual(true, Evaluate("var x = new Number(10.0); x == 10"));
            Assert.AreEqual(true, Evaluate("var x = new Number(10.0); x.valueOf() == 10"));
            Assert.AreEqual(true, Evaluate("var x = new Number(10.0); 10 == x.valueOf()"));
            Assert.AreEqual(false, Evaluate("var x = new Number(10); x == new Number(10)"));
            Assert.AreEqual(true, Evaluate("var x = new Number(10); x == x"));

            // Arrays
            Assert.AreEqual(true, Evaluate("2 == [2]"));
            Assert.AreEqual(true, Evaluate("2 == [[[2]]]"));
        }

        [TestMethod]
        public void NotEquals()
        {
            // Both operands are of the same type.
            Assert.AreEqual(true, Evaluate("false != true"));
            Assert.AreEqual(false, Evaluate("false != false"));
            Assert.AreEqual(false, Evaluate("10 != 10"));
            Assert.AreEqual(true, Evaluate("10 != 11"));
            Assert.AreEqual(false, Evaluate("'test' != 'test'"));
            Assert.AreEqual(true, Evaluate("'test' != 'TEST'"));
            Assert.AreEqual(false, Evaluate("'' != ''"));

            // Operands of different types.
            Assert.AreEqual(true, Evaluate("true != 2"));
            Assert.AreEqual(false, Evaluate("true != 1"));
            Assert.AreEqual(true, Evaluate("2 != true"));
            Assert.AreEqual(false, Evaluate("1 != true"));
            Assert.AreEqual(false, Evaluate("1 != '1'"));
            Assert.AreEqual(true, Evaluate("1 != '0'"));

            // Undefined and null.
            Assert.AreEqual(false, Evaluate("null != null"));
            Assert.AreEqual(false, Evaluate("undefined != undefined"));
            Assert.AreEqual(false, Evaluate("null != undefined"));
            Assert.AreEqual(false, Evaluate("Math.abcdef != Math.abcdefghi"));
            Assert.AreEqual(false, Evaluate("Math.abcdef != undefined"));
            Assert.AreEqual(true, Evaluate("null != 5"));

            // Symbols.
            Assert.AreEqual(true, Evaluate("Symbol() != Symbol()"));
            Assert.AreEqual(true, Evaluate("Symbol('test') != Symbol('test')"));

            // NaN
            Assert.AreEqual(true, Evaluate("NaN != NaN"));

            // Variables
            Assert.AreEqual(false, Evaluate("var x = new Number(10.0); x != 10"));
            Assert.AreEqual(false, Evaluate("var x = new Number(10.0); x.valueOf() != 10"));
            Assert.AreEqual(false, Evaluate("var x = new Number(10.0); 10 != x.valueOf()"));
            Assert.AreEqual(true, Evaluate("var x = new Number(10); x != new Number(10)"));
            Assert.AreEqual(false, Evaluate("var x = new Number(10); x != x"));
        }

        [TestMethod]
        public void StrictEquals()
        {
            // Both operands are of the same type.
            Assert.AreEqual(false, Evaluate("false === true"));
            Assert.AreEqual(true, Evaluate("false === false"));
            Assert.AreEqual(true, Evaluate("10 === 10"));
            Assert.AreEqual(true, Evaluate("10.0 === 10"));
            Assert.AreEqual(false, Evaluate("10 === 11"));
            Assert.AreEqual(true, Evaluate("'test' === 'test'"));
            Assert.AreEqual(false, Evaluate("'test' === 'TEST'"));
            Assert.AreEqual(true, Evaluate("'' === ''"));

            // Operands of different types.
            Assert.AreEqual(false, Evaluate("true === 2"));
            Assert.AreEqual(false, Evaluate("true === 1"));
            Assert.AreEqual(false, Evaluate("2 === true"));
            Assert.AreEqual(false, Evaluate("1 === true"));
            Assert.AreEqual(false, Evaluate("1 === '1'"));
            Assert.AreEqual(false, Evaluate("1 === '0'"));

            // Undefined and null.
            Assert.AreEqual(true, Evaluate("null === null"));
            Assert.AreEqual(true, Evaluate("undefined === undefined"));
            Assert.AreEqual(false, Evaluate("null === undefined"));
            Assert.AreEqual(true, Evaluate("Math.abcdef === Math.abcdefghi"));
            Assert.AreEqual(true, Evaluate("Math.abcdef === undefined"));
            Assert.AreEqual(false, Evaluate("null === 5"));

            // Symbols.
            Assert.AreEqual(false, Evaluate("Symbol() === Symbol()"));
            Assert.AreEqual(false, Evaluate("Symbol('test') === Symbol('test')"));

            // NaN
            Assert.AreEqual(false, Evaluate("NaN === NaN"));

            // Variables
            Assert.AreEqual(false, Evaluate("var x = new Number(10.0); x === 10"));
            Assert.AreEqual(true, Evaluate("var x = new Number(10.0); x.valueOf() === 10"));
            Assert.AreEqual(true, Evaluate("var x = new Number(10.0); 10 === x.valueOf()"));
            Assert.AreEqual(false, Evaluate("var x = new Number(10); x === new Number(10)"));
            Assert.AreEqual(true, Evaluate("var x = new Number(10); x === x"));
        }

        [TestMethod]
        public void StrictNotEquals()
        {
            // Both operands are of the same type.
            Assert.AreEqual(true, Evaluate("false !== true"));
            Assert.AreEqual(false, Evaluate("false !== false"));
            Assert.AreEqual(false, Evaluate("10 !== 10"));
            Assert.AreEqual(true, Evaluate("10 !== 11"));
            Assert.AreEqual(false, Evaluate("'test' !== 'test'"));
            Assert.AreEqual(true, Evaluate("'test' !== 'TEST'"));
            Assert.AreEqual(false, Evaluate("'' !== ''"));

            // Operands of different types.
            Assert.AreEqual(true, Evaluate("true !== 2"));
            Assert.AreEqual(true, Evaluate("true !== 1"));
            Assert.AreEqual(true, Evaluate("2 !== true"));
            Assert.AreEqual(true, Evaluate("1 !== true"));
            Assert.AreEqual(true, Evaluate("1 !== '1'"));
            Assert.AreEqual(true, Evaluate("1 !== '0'"));

            // Undefined and null.
            Assert.AreEqual(false, Evaluate("null !== null"));
            Assert.AreEqual(false, Evaluate("undefined !== undefined"));
            Assert.AreEqual(true, Evaluate("null !== undefined"));
            Assert.AreEqual(false, Evaluate("Math.abcdef !== Math.abcdefghi"));
            Assert.AreEqual(false, Evaluate("Math.abcdef !== undefined"));
            Assert.AreEqual(true, Evaluate("null !== 5"));

            // Symbols.
            Assert.AreEqual(true, Evaluate("Symbol() !== Symbol()"));
            Assert.AreEqual(true, Evaluate("Symbol('test') !== Symbol('test')"));

            // NaN
            Assert.AreEqual(true, Evaluate("NaN !== NaN"));

            // Variables
            Assert.AreEqual(true, Evaluate("var x = new Number(10.0); x !== 10"));
            Assert.AreEqual(false, Evaluate("var x = new Number(10.0); x.valueOf() !== 10"));
            Assert.AreEqual(false, Evaluate("var x = new Number(10.0); 10 !== x.valueOf()"));
            Assert.AreEqual(true, Evaluate("var x = new Number(10); x !== new Number(10)"));
            Assert.AreEqual(false, Evaluate("var x = new Number(10); x !== x"));
        }

        [TestMethod]
        public void LessThan()
        {
            Assert.AreEqual(false, Evaluate("7 < 5"));
            Assert.AreEqual(true, Evaluate("5 < 7"));
            Assert.AreEqual(false, Evaluate("5 < 5"));
            Assert.AreEqual(true, Evaluate("-5 < 5"));
            Assert.AreEqual(true, Evaluate("5.6 < 5.7"));
            Assert.AreEqual(false, Evaluate("5.6 < 5.6"));
            Assert.AreEqual(false, Evaluate("5.6 < 5.5"));
            Assert.AreEqual(false, Evaluate("5 < NaN"));
            Assert.AreEqual(false, Evaluate("NaN < NaN"));
            Assert.AreEqual(true, Evaluate("'a' < 'b'"));
            Assert.AreEqual(false, Evaluate("'a' < 'a'"));
            Assert.AreEqual(false, Evaluate("'a' < 'A'"));
            Assert.AreEqual(false, Evaluate("'2' < '15'"));
            Assert.AreEqual(true, Evaluate("2 < '15'"));
            Assert.AreEqual(false, Evaluate("'15' < 2"));
            Assert.AreEqual(true, Evaluate("false < true"));
            Assert.AreEqual(true, Evaluate("x = 0.3; y = 0.5; x < y"));
            Assert.AreEqual(false, Evaluate("x = 0.4; y = 0.4; x < y"));
            Assert.AreEqual(false, Evaluate("x = 0.5; y = 0.3; x < y"));

            // Objects
            Assert.AreEqual(false, Evaluate("0 < {valueOf: function() {return -2}, toString: function() {return '2'}}"));
            Assert.AreEqual(false, Evaluate("'0' < {valueOf: function() {return -2}, toString: function() {return '2'}}"));
            Assert.AreEqual(true, Evaluate("({valueOf: function() {return -2}, toString: function() {return '2'}}) < 0"));
            Assert.AreEqual(true, Evaluate("({valueOf: function() {return -2}, toString: function() {return '2'}}) < '0'"));
            Assert.AreEqual(false, Evaluate("var object = {valueOf: function() {return '-2'}, toString: function() {return 2}}; object < '-1'"));

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", Evaluate(@"
                var x = { valueOf: function () { throw 'x'; } };
                var y = { valueOf: function () { throw 'y'; } };
                try { x < y; } catch (e) { e }"));
        }

        [TestMethod]
        public void LessThanOrEqual()
        {
            Assert.AreEqual(false, Evaluate("7 <= 5"));
            Assert.AreEqual(true, Evaluate("5 <= 7"));
            Assert.AreEqual(true, Evaluate("5 <= 5"));
            Assert.AreEqual(true, Evaluate("-5 <= 5"));
            Assert.AreEqual(true, Evaluate("5.6 <= 5.7"));
            Assert.AreEqual(true, Evaluate("5.6 <= 5.6"));
            Assert.AreEqual(false, Evaluate("5.6 <= 5.5"));
            Assert.AreEqual(false, Evaluate("5 <= NaN"));
            Assert.AreEqual(false, Evaluate("NaN <= NaN"));
            Assert.AreEqual(true, Evaluate("'a' <= 'b'"));
            Assert.AreEqual(true, Evaluate("'a' <= 'a'"));
            Assert.AreEqual(false, Evaluate("'a' <= 'A'"));
            Assert.AreEqual(false, Evaluate("'2' <= '15'"));
            Assert.AreEqual(true, Evaluate("2 <= '15'"));
            Assert.AreEqual(false, Evaluate("'15' <= 2"));
            Assert.AreEqual(true, Evaluate("false <= true"));
            Assert.AreEqual(true, Evaluate("x = 0.3; y = 0.5; x <= y"));
            Assert.AreEqual(true, Evaluate("x = 0.4; y = 0.4; x <= y"));
            Assert.AreEqual(false, Evaluate("x = 0.5; y = 0.3; x <= y"));

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", Evaluate(@"
                var x = { valueOf: function () { throw 'x'; } };
                var y = { valueOf: function () { throw 'y'; } };
                try { x <= y; } catch (e) { e }"));
        }

        [TestMethod]
        public void GreaterThan()
        {
            Assert.AreEqual(true, Evaluate("7 > 5"));
            Assert.AreEqual(false, Evaluate("5 > 7"));
            Assert.AreEqual(false, Evaluate("5 > 5"));
            Assert.AreEqual(false, Evaluate("-5 > 5"));
            Assert.AreEqual(false, Evaluate("5.6 > 5.7"));
            Assert.AreEqual(false, Evaluate("5.6 > 5.6"));
            Assert.AreEqual(true, Evaluate("5.6 > 5.5"));
            Assert.AreEqual(false, Evaluate("5 > NaN"));
            Assert.AreEqual(false, Evaluate("NaN > NaN"));
            Assert.AreEqual(false, Evaluate("'a' > 'b'"));
            Assert.AreEqual(false, Evaluate("'a' > 'a'"));
            Assert.AreEqual(true, Evaluate("'a' > 'A'"));
            Assert.AreEqual(true, Evaluate("'2' > '15'"));
            Assert.AreEqual(false, Evaluate("2 > '15'"));
            Assert.AreEqual(true, Evaluate("'15' > 2"));
            Assert.AreEqual(false, Evaluate("false > true"));
            Assert.AreEqual(false, Evaluate("x = 0.3; y = 0.5; x > y"));
            Assert.AreEqual(false, Evaluate("x = 0.4; y = 0.4; x > y"));
            Assert.AreEqual(true, Evaluate("x = 0.5; y = 0.3; x > y"));

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", Evaluate(@"
                var x = { valueOf: function () { throw 'x'; } };
                var y = { valueOf: function () { throw 'y'; } };
                try { x >= y; } catch (e) { e }"));
        }

        [TestMethod]
        public void GreaterThanOrEqual()
        {
            Assert.AreEqual(true, Evaluate("7 >= 5"));
            Assert.AreEqual(false, Evaluate("5 >= 7"));
            Assert.AreEqual(true, Evaluate("5 >= 5"));
            Assert.AreEqual(false, Evaluate("-5 >= 5"));
            Assert.AreEqual(false, Evaluate("5.6 >= 5.7"));
            Assert.AreEqual(true, Evaluate("5.6 >= 5.6"));
            Assert.AreEqual(true, Evaluate("5.6 >= 5.5"));
            Assert.AreEqual(false, Evaluate("5 >= NaN"));
            Assert.AreEqual(false, Evaluate("NaN >= NaN"));
            Assert.AreEqual(false, Evaluate("'a' >= 'b'"));
            Assert.AreEqual(true, Evaluate("'a' >= 'a'"));
            Assert.AreEqual(true, Evaluate("'a' >= 'A'"));
            Assert.AreEqual(true, Evaluate("'2' >= '15'"));
            Assert.AreEqual(false, Evaluate("2 >= '15'"));
            Assert.AreEqual(true, Evaluate("'15' >= 2"));
            Assert.AreEqual(false, Evaluate("false >= true"));
            Assert.AreEqual(false, Evaluate("x = 0.3; y = 0.5; x >= y"));
            Assert.AreEqual(true, Evaluate("x = 0.4; y = 0.4; x >= y"));
            Assert.AreEqual(true, Evaluate("x = 0.5; y = 0.3; x >= y"));

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", Evaluate(@"
                var x = { valueOf: function () { throw 'x'; } };
                var y = { valueOf: function () { throw 'y'; } };
                try { x >= y; } catch (e) { e }"));
        }

        [TestMethod]
        public void BitwiseAnd()
        {
            // Constants
            Assert.AreEqual(3, Evaluate("11 & 7"));
            Assert.AreEqual(9, Evaluate("11 & -7"));
            Assert.AreEqual(8, Evaluate("4294967304 & 255"));
            Assert.AreEqual(16, Evaluate("42949673042 & -401929233123"));
            Assert.AreEqual(1, Evaluate("11.9 & 1.5"));
            Assert.AreEqual(0, Evaluate("NaN & NaN"));

            // Variables
            Assert.AreEqual(3, Evaluate("x = 11; x & 7"));
            Assert.AreEqual(9, Evaluate("x = 11; x & -7"));
            Assert.AreEqual(8, Evaluate("x = 4294967304; x & 255"));
            Assert.AreEqual(16, Evaluate("x = 42949673042; x & -401929233123"));
            Assert.AreEqual(1, Evaluate("x = 11.5; x & 1.5"));
        }

        [TestMethod]
        public void BitwiseXor()
        {
            // Constants
            Assert.AreEqual(12, Evaluate("11 ^ 7"));
            Assert.AreEqual(-14, Evaluate("11 ^ -7"));
            Assert.AreEqual(247, Evaluate("4294967304 ^ 255"));
            Assert.AreEqual(10, Evaluate("11.5 ^ 1.5"));
            Assert.AreEqual(3, Evaluate("'5' ^ '6'"));
            Assert.AreEqual(1, Evaluate("'a' ^ 1"));

            // Variables
            Assert.AreEqual(12, Evaluate("x = 11; x ^ 7"));
            Assert.AreEqual(-14, Evaluate("x = 11; x ^ -7"));
            Assert.AreEqual(247, Evaluate("x = 4294967304; x ^ 255"));
            Assert.AreEqual(1797692751, Evaluate("x = 42949673042; x ^ -401929233123"));
            Assert.AreEqual(10, Evaluate("x = 11.5; x ^ 1.5"));
        }

        [TestMethod]
        public void BitwiseOr()
        {
            Assert.AreEqual(15, Evaluate("11 | 7"));
            Assert.AreEqual(-5, Evaluate("11 | -7"));
            Assert.AreEqual(255, Evaluate("8 | 255"));
            Assert.AreEqual(11, Evaluate("11.5 | 1.5"));
            Assert.AreEqual(7, Evaluate("'5' | '6'"));
            Assert.AreEqual(1, Evaluate("'a' | 1"));

            // Variables
            Assert.AreEqual(15, Evaluate("x = 11; x | 7"));
            Assert.AreEqual(-5, Evaluate("x = -7; 11 | x"));
            Assert.AreEqual(255, Evaluate("x = 8; x | 255"));
            Assert.AreEqual(11, Evaluate("x = 1.5; 11.5 | x"));
            Assert.AreEqual(7, Evaluate("x = '5'; y = '6'; x | y"));
            Assert.AreEqual(1, Evaluate("x = 1; 'a' | x"));
        }

        [TestMethod]
        public void LogicalAnd()
        {
            // Boolean arguments.
            Assert.AreEqual(false, Evaluate("false && false"));
            Assert.AreEqual(false, Evaluate("false && true"));
            Assert.AreEqual(false, Evaluate("true && false"));
            Assert.AreEqual(true, Evaluate("true && true"));

            // Numeric arguments.
            Assert.AreEqual(0, Evaluate("0 && 7"));
            Assert.AreEqual(0, Evaluate("11 && 0"));
            Assert.AreEqual(7, Evaluate("11 && 7"));

            // Mixed.
            Assert.AreEqual(true, Evaluate("11 && true"));
            Assert.AreEqual(11, Evaluate("true && 11"));
            Assert.AreEqual(false, Evaluate("false && 11"));

            // Variables.
            Assert.AreEqual(false, Evaluate("x = false; x && false"));
            Assert.AreEqual(false, Evaluate("x = true; false && x"));
            Assert.AreEqual(false, Evaluate("x = true; y = false; x && y"));
            Assert.AreEqual(true, Evaluate("x = true; y = true; x && y"));
            Assert.AreEqual(false, Evaluate("x = false; y = 11; x && y"));
        }

        [TestMethod]
        public void LogicalOr()
        {
            // Boolean arguments.
            Assert.AreEqual(false, Evaluate("false || false"));
            Assert.AreEqual(true, Evaluate("false || true"));
            Assert.AreEqual(true, Evaluate("true || false"));
            Assert.AreEqual(true, Evaluate("true || true"));

            // Numeric arguments.
            Assert.AreEqual(7, Evaluate("0 || 7"));
            Assert.AreEqual(11, Evaluate("11 || 0"));
            Assert.AreEqual(11, Evaluate("11 || 7"));

            // Mixed.
            Assert.AreEqual(11, Evaluate("11 || true"));
            Assert.AreEqual(true, Evaluate("true || 11"));
            Assert.AreEqual(11, Evaluate("false || 11"));

            // Variables.
            Assert.AreEqual(false, Evaluate("x = false; x || false"));
            Assert.AreEqual(true, Evaluate("x = true; false || x"));
            Assert.AreEqual(true, Evaluate("x = true; y = false; x || y"));
            Assert.AreEqual(true, Evaluate("x = true; y = true; x || y"));
            Assert.AreEqual(11, Evaluate("x = false; y = 11; x || y"));
        }

        [TestMethod]
        public void Comma()
        {
            Assert.AreEqual(2, Evaluate("1, 2"));
            Assert.AreEqual("aliens", Evaluate("1, 'aliens'"));
            Assert.AreEqual(true, Evaluate("'go', true"));
            Assert.AreEqual(3, Evaluate("1, 2, 3"));
            Assert.AreEqual(3, Evaluate("var x = 1, y = 2, z = 3; x, y, z"));
            Assert.AreEqual(3, Evaluate("var x = [1, 2, 3]; x[0], x[1], x[2]"));
            Assert.AreEqual(3, Evaluate("Object((1, 2, 3)).valueOf()"));
            Assert.AreEqual(3, Evaluate("Object((1, 2, 3), 4, 5, 6).valueOf()"));
        }

        [TestMethod]
        public void Conditional()
        {
            Assert.AreEqual(8, Evaluate("true ? 8 : 'test'"));
            Assert.AreEqual("hello", Evaluate("true ? 'hello' : 'nope'"));
            Assert.AreEqual(7, Evaluate("5 ? 7 : 8"));
            Assert.AreEqual(8, Evaluate("0 ? 7 : 8"));
            Assert.AreEqual(true, Evaluate("true ? true : false ? 8 : 9"));
            Assert.AreEqual(3, Evaluate("1 ? 2 ? 3 : 4 : 5"));
            Assert.AreEqual(4, Evaluate("1 ? 0 ? 3 : 4 : 5"));
            Assert.AreEqual(5, Evaluate("0 ? 1 ? 3 : 4 : 5"));

            // Test the precedence at the start of the conditional.
            Assert.AreEqual(1, Evaluate("x = 5; x + 0 ? 1 : 2"));
            Assert.AreEqual(1, Evaluate("x = 5; x || 0 ? 1 : 2"));
            Assert.AreEqual(2, Evaluate("x = 5; x = 0 ? 1 : 2"));
            Assert.AreEqual(2, Evaluate("x = 5; x, 0 ? 1 : 2"));

            // Test the precedence in the middle of the conditional.
            Assert.AreEqual(1, Evaluate("x = 5; true ? x = 1 : 2"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = 5; true ? 1, x : 2"));

            // Test the precedence at the end of the conditional.
            Assert.AreEqual(1, Evaluate("x = 4; true ? 1 : x = 2"));
            Assert.AreEqual(1, Evaluate("x = 4; true ? 1 : x += 2"));
            Assert.AreEqual(2, Evaluate("x = 3; true ? 1 : x, 2"));

            // Variables
            Assert.AreEqual(2, Evaluate("var x = 1, y = 2, z = 3; x ? y : z"));
            Assert.AreEqual(3, Evaluate("var x = 0, y = 2, z = 3; x ? y : z"));
        }

        [TestMethod]
        public void Assignment()
        {
            // Numeric operations.
            Assert.AreEqual(4, Evaluate("x = 4"));
            Assert.AreEqual(6, Evaluate("x = 4; x += 2"));
            Assert.AreEqual(2, Evaluate("x = 4; x -= 2"));
            Assert.AreEqual(8, Evaluate("x = 4; x *= 2"));
            Assert.AreEqual(2, Evaluate("x = 4; x /= 2"));
            Assert.AreEqual(1, Evaluate("x = 4; x %= 3"));
            Assert.AreEqual(8, Evaluate("x = 4; x <<= 1"));
            Assert.AreEqual(2, Evaluate("x = 4; x >>= 1"));
            Assert.AreEqual(2, Evaluate("x = 4; x >>>= 1"));
            Assert.AreEqual(0, Evaluate("x = 4; x &= 1"));
            Assert.AreEqual(5, Evaluate("x = 4; x |= 1"));
            Assert.AreEqual(5, Evaluate("x = 4; x ^= 1"));
            Assert.AreEqual(4, Evaluate("x = 4; x"));
            Assert.AreEqual(6, Evaluate("x = 4; x += 2; x"));
            Assert.AreEqual(2, Evaluate("x = 4; x -= 2; x"));
            Assert.AreEqual(8, Evaluate("x = 4; x *= 2; x"));
            Assert.AreEqual(2, Evaluate("x = 4; x /= 2; x"));
            Assert.AreEqual(1, Evaluate("x = 4; x %= 3; x"));
            Assert.AreEqual(8, Evaluate("x = 4; x <<= 1; x"));
            Assert.AreEqual(2, Evaluate("x = 4; x >>= 1; x"));
            Assert.AreEqual(2, Evaluate("x = 4; x >>>= 1; x"));
            Assert.AreEqual(0, Evaluate("x = 4; x &= 1; x"));
            Assert.AreEqual(5, Evaluate("x = 4; x |= 1; x"));
            Assert.AreEqual(5, Evaluate("x = 4; x ^= 1; x"));

            // String operations.
            Assert.AreEqual("hah", Evaluate("x = 'hah'"));
            Assert.AreEqual("hah2", Evaluate("x = 'hah'; x += 2"));
            Assert.AreEqual("32", Evaluate("x = '3'; x += '2'"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'hah'; x -= 2"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'hah'; x *= 2"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'hah'; x /= 2"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'hah'; x %= 3"));
            Assert.AreEqual(0, Evaluate("x = 'hah'; x <<= 1"));
            Assert.AreEqual(0, Evaluate("x = 'hah'; x >>= 1"));
            Assert.AreEqual(0, Evaluate("x = 'hah'; x >>>= 1"));
            Assert.AreEqual(0, Evaluate("x = 'hah'; x &= 1"));
            Assert.AreEqual(1, Evaluate("x = 'hah'; x |= 1"));
            Assert.AreEqual(1, Evaluate("x = 'hah'; x ^= 1"));
            Assert.AreEqual("hah", Evaluate("x = 'hah'; x"));
            Assert.AreEqual("hah2", Evaluate("x = 'hah'; x += 2; x"));
            Assert.AreEqual("32", Evaluate("x = '3'; x += '2'; x"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'hah'; x -= 2; x"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'hah'; x *= 2; x"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'hah'; x /= 2; x"));
            Assert.AreEqual(double.NaN, Evaluate("x = 'hah'; x %= 3; x"));
            Assert.AreEqual(0, Evaluate("x = 'hah'; x <<= 1; x"));
            Assert.AreEqual(0, Evaluate("x = 'hah'; x >>= 1; x"));
            Assert.AreEqual(0, Evaluate("x = 'hah'; x >>>= 1; x"));
            Assert.AreEqual(0, Evaluate("x = 'hah'; x &= 1; x"));
            Assert.AreEqual(1, Evaluate("x = 'hah'; x |= 1; x"));
            Assert.AreEqual(1, Evaluate("x = 'hah'; x ^= 1; x"));

            // Evaluated left to right.
            Assert.AreEqual(7, Evaluate("x = 1; (x = 2) + x + (x = 3)"));

            // The left hand side is evaluated before the right hand side.
            Assert.AreEqual("123", Evaluate("x = '123'; (x = ['123'])[0] = x[0]"));

            // The left hand side should only be evaluated once.
            Assert.AreEqual("1/3", Evaluate("x = [[2],[5]]; (x = x[0])[0] = 3; x.length + '/' + x.toString()"));
            Assert.AreEqual("1/9", Evaluate("x = [[2],[5]]; (x = x[0])[0] += 7; x.length + '/' + x.toString()"));

            // Strict mode: attempts to set a variable that has not been declared is disallowed.
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("'use strict'; asddfsgwqewert = 'test'"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("function foo() { 'use strict'; asddfsgwqewert = 'test'; } foo()"));

            // Strict mode: cannot write to a non-writable property.
            Assert.AreEqual("TypeError", EvaluateExceptionType("'use strict'; var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: true}); x.a = 5;"));

            // Strict mode: cannot write to a non-existant property when the object is non-extensible.
            Assert.AreEqual("TypeError", EvaluateExceptionType("'use strict'; var x = {}; Object.preventExtensions(x); x.a = 5;"));

            // Strict mode: cannot write to a property that has a getter but no setter.
            Assert.AreEqual("TypeError", EvaluateExceptionType("'use strict'; var x = {}; Object.defineProperty(x, 'a', {get: function() { return 1 }}); x.a = 5;"));

            // Strict mode: left-hand side cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; eval = 5;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; arguments = 5;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { eval = 5; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { arguments = 5; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; eval = 5; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; arguments = 5; } f()"));

            // Strict mode: left-hand side cannot be 'eval' or 'arguments' (compound assignment).
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; eval += 5;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; arguments += 5;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { eval += 5; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { arguments += 5; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; eval += 5; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; arguments += 5; } f()"));
        }

        [TestMethod]
        public void PreIncrement()
        {
            Assert.AreEqual(1, Evaluate("x = 0; ++ x"));
            Assert.AreEqual(1, Evaluate("x = 0; ++ x; x"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("++ 2"));

            // The operand should only be evaluated once.
            Assert.AreEqual(3, Evaluate("x = [[2]]; ++(x = x[0])[0]"));
            Assert.AreEqual("1/3", Evaluate("x = [[2]]; ++(x = x[0])[0]; x.length + '/' + x.toString()"));

            // Strict mode: reference cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; ++ eval;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; ++ arguments;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { ++ eval; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { ++ arguments; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; ++ eval; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; ++ arguments; } f()"));
        }

        [TestMethod]
        public void PreDecrement()
        {
            Assert.AreEqual(-1, Evaluate("x = 0; -- x"));
            Assert.AreEqual(-1, Evaluate("x = 0; -- x; x"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("-- 2"));

            // The operand should only be evaluated once.
            Assert.AreEqual(1, Evaluate("x = [[2]]; --(x = x[0])[0]"));
            Assert.AreEqual("1/1", Evaluate("x = [[2]]; --(x = x[0])[0]; x.length + '/' + x.toString()"));

            // Strict mode: reference cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; -- eval;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; -- arguments;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { -- eval; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { -- arguments; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; -- eval; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; -- arguments; } f()"));
        }

        [TestMethod]
        public void PostIncrement()
        {
            Assert.AreEqual(0, Evaluate("x = 0; x ++"));
            Assert.AreEqual(1, Evaluate("x = 0; x ++; x"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("2 ++"));

            // The operand should only be evaluated once.
            Assert.AreEqual(2, Evaluate("x = [[2]]; (x = x[0])[0]++"));
            Assert.AreEqual("1/3", Evaluate("x = [[2]]; (x = x[0])[0]++; x.length + '/' + x.toString()"));

            // Strict mode: left-hand side cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; eval ++;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; arguments ++;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { eval ++; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { arguments ++; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; eval ++; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; arguments ++; } f()"));
        }

        [TestMethod]
        public void PostDecrement()
        {
            Assert.AreEqual(0, Evaluate("x = 0; x --"));
            Assert.AreEqual(-1, Evaluate("x = 0; x --; x"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("2 --"));

            // The operand should only be evaluated once.
            Assert.AreEqual(2, Evaluate("x = [[2]]; (x = x[0])[0]--"));
            Assert.AreEqual("1/1", Evaluate("x = [[2]]; (x = x[0])[0]--; x.length + '/' + x.toString()"));

            // Strict mode: left-hand side cannot be 'eval' or 'arguments'.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; eval --;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; arguments --;"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { eval --; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function f() { arguments --; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; eval --; } f()"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("function f() { 'use strict'; arguments --; } f()"));
        }

        [TestMethod]
        public void Grouping()
        {
            Assert.AreEqual(22, Evaluate("(5 + 6) * 2"));
            Assert.AreEqual(27, Evaluate("5 + (5 + 6) * 2"));
            Assert.AreEqual(33, Evaluate("5 + (5 * 6) - 2"));
            Assert.AreEqual(32, Evaluate("(5 + (5 + 6)) * 2"));
        }

        [TestMethod]
        public void FunctionCall()
        {
            Assert.AreEqual("[object Math]", Evaluate("(Math.toString)()"));
            Assert.AreEqual("[object Math]", Evaluate("Math.toString()"));
            Assert.AreEqual(2, Evaluate("Math.ceil(1.2)"));
            Assert.AreEqual(0, Evaluate("Math.atan2(0, 2)"));
            Assert.AreEqual("[object Math]", Evaluate("(Math.toString)()"));

            // Call functions in the global scope.
            Assert.AreEqual("a%20b", Evaluate("encodeURI('a b')"));
            Assert.AreEqual(true, Evaluate("b = 5; this.hasOwnProperty('b')"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("b = 5; hasOwnProperty('b')"));

            // Argument conversion.
            Assert.AreEqual(123, Evaluate("Math.abs('-123')"));

            // Extra arguments are ignored.
            Assert.AreEqual(2, Evaluate("Math.ceil(1.2, 5)"));
            Assert.AreEqual(5, Evaluate("function test(test) { return test; } test(5, 4, 3);"));

            // Too few arguments are passed as "undefined".
            Assert.AreEqual(double.NaN, Evaluate("Math.ceil()"));
            Assert.AreEqual(true, Evaluate("isNaN()"));
            Assert.AreEqual(false, Evaluate("isFinite()"));
            Assert.AreEqual(3, Evaluate("function test(test) { test = 3; return test; } test();"));

            // Object must be a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("x = { a: 1 }; x()"));

            // Passing a function in an argument.
            Assert.AreEqual(3, Evaluate("function a(b) { return b + 2; } function c(func) { return func(1); } c(a)"));

            // Arguments should only be evaluated once.
            Assert.AreEqual(1, Evaluate("var i = 0; Function({ toString: function() { return ++i } }).apply(null); i"));

            // In compatibility mode, undefined and null are converted to objects.
            CompatibilityMode = CompatibilityMode.ECMAScript3;
            try
            {
                Assert.AreEqual(true, Evaluate("hasOwnProperty('NaN')"));
                Assert.AreEqual(true, Evaluate("hasOwnProperty.call(null, 'NaN')"));
                Assert.AreEqual(true, Evaluate("hasOwnProperty.call(undefined, 'NaN')"));
            }
            finally
            {
                CompatibilityMode = CompatibilityMode.Latest;
            }

            // 'arguments' and 'caller' must be undefined in strict mode.
            Assert.AreEqual(Undefined.Value, Evaluate("'use strict'; function test(){ function inner(){ return test.arguments; } return inner(); } test()"));
            Assert.AreEqual(Undefined.Value, Evaluate("'use strict'; function test(){ function inner(){ return inner.caller; } return inner(); } test()"));
            Assert.AreEqual(Undefined.Value, Evaluate("'use strict'; function test(){ function inner(){ test.arguments = 5; } return inner(); } test()"));
            Assert.AreEqual(Undefined.Value, Evaluate("'use strict'; function test(){ function inner(){ inner.caller = 5; } return inner(); } test()"));
        }

        [TestMethod]
        public void New()
        {
            Assert.AreEqual("five", Evaluate("(new String('five')).toString()"));
            Assert.AreEqual("five", Evaluate("new String('five').toString()"));
            Assert.AreEqual("5", Evaluate("new Number(5).toString()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new (String('five'))"));
            Assert.AreEqual("TypeError: The new operator requires a function, found a 'string' instead", EvaluateExceptionMessage("new (String('five'))"));

            // Precedence tests.
            Assert.AreEqual("[object Object]", Evaluate("x = {}; x.f = function() { }; (new x.f()).toString()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("(new Function.valueOf)()"));  // new (Function.valueOf) is not a function.
            Assert.AreEqual("function anonymous() {\n\n}", Evaluate("(new (Function.valueOf())).toString()"));
            Assert.AreEqual("function anonymous() {\n\n}", Evaluate("((new Function).valueOf()).toString()"));
            Assert.AreEqual("[object Object]", Evaluate("x = {}; x.f = function() { }; (new (x.f)()).toString()"));

            // New user-defined function.
            Assert.AreEqual(5, Evaluate("function f() { this.a = 5; return 2; }; a = new f(); a.a"));
            Assert.AreEqual(true, Evaluate("function f() { this.a = 5; return 2; }; a = new f(); Object.getPrototypeOf(a) === f.prototype"));

            // Returning an object returns that as the new object.
            Assert.AreEqual(6, Evaluate("function f() { this.a = 5; return { a: 6 }; }; x = new f(); x.a"));

            // Built-in functions cannot be constructed.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Function.valueOf()"));
        }

        [TestMethod]
        public void InstanceOf()
        {
            // Primitive types always return false.
            Assert.AreEqual(false, Evaluate("5 instanceof Boolean"));
            Assert.AreEqual(false, Evaluate("5 instanceof Number"));
            Assert.AreEqual(false, Evaluate("5 instanceof String"));
            Assert.AreEqual(false, Evaluate("true instanceof Boolean"));
            Assert.AreEqual(false, Evaluate("'hello' instanceof Number"));
            Assert.AreEqual(false, Evaluate("'hello' instanceof String"));

            // Arrays and regular expressions return true for Array and RegExp respectively.
            // Since these objects inherit from Object, that returns true as well.
            Assert.AreEqual(true, Evaluate("/abc/ instanceof RegExp"));
            Assert.AreEqual(false, Evaluate("/abc/ instanceof Array"));
            Assert.AreEqual(true, Evaluate("[] instanceof Array"));
            Assert.AreEqual(false, Evaluate("[] instanceof Number"));
            Assert.AreEqual(true, Evaluate("/abc/ instanceof Object"));
            Assert.AreEqual(true, Evaluate("[] instanceof Object"));

            // Object literals return true for Object only.  Note: "{} instanceof Object" is invalid.
            Assert.AreEqual(true, Evaluate("x = {}; x instanceof Object"));
            Assert.AreEqual(false, Evaluate("x = {}; x instanceof Array"));

            // Global functions return true for Object and Function.
            Assert.AreEqual(true, Evaluate("Array instanceof Function"));
            Assert.AreEqual(true, Evaluate("Array instanceof Object"));

            // Both sides can be a variable.
            Assert.AreEqual(true, Evaluate("x = Function; x instanceof x"));
            Assert.AreEqual(false, Evaluate("x = Array; x instanceof x"));

            // Right-hand-side must be a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("5 instanceof Math"));
            Assert.AreEqual("TypeError: The instanceof operator expected a function, but found 'object' instead", EvaluateExceptionMessage("5 instanceof Math"));

            // Test newly constructed objects.
            Assert.AreEqual(true, Evaluate("new Number(5) instanceof Number"));
            Assert.AreEqual(true, Evaluate("new Number(5) instanceof Object"));
            Assert.AreEqual(false, Evaluate("new Number(5) instanceof String"));

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", Evaluate(@"
                var x = function () { throw 'x'; };
                var y = function () { throw 'y'; };
                try { x() instanceof y(); } catch (e) { e }"));
        }

        [TestMethod]
        public void In()
        {
            Assert.AreEqual(true, Evaluate("'atan2' in Math"));
            Assert.AreEqual(true, Evaluate("1 in [5, 6]"));
            Assert.AreEqual(false, Evaluate("2 in [5, 6]"));
            Assert.AreEqual(true, Evaluate("'a' in {a: 1, b: 2}"));
            Assert.AreEqual(false, Evaluate("'c' in {a: 1, b: 2}"));
            Assert.AreEqual(true, Evaluate("'valueOf' in {a: 1, b: 2}"));
            Assert.AreEqual(true, Evaluate("'toString' in new Number(5)"));
            Assert.AreEqual(false, Evaluate("'abcdefgh' in new Number(5)"));
            Assert.AreEqual(true, Evaluate("'toString' in new String()"));
            Assert.AreEqual(true, Evaluate("var x = 'atan2', y = Math; x in y"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("'toString' in 5"));
            Assert.AreEqual("TypeError: The in operator expected an object, but found 'number' instead", EvaluateExceptionMessage("'toString' in 5"));

            // Check order of evaluation - should be left to right.
            Assert.AreEqual("x", Evaluate(@"
                var x = function () { throw 'x'; };
                var y = function () { throw 'y'; };
                try { x() in y(); } catch (e) { e }"));
        }

        [TestMethod]
        public void MemberAccess()
        {
            Assert.AreEqual(double.NaN, Evaluate("NaN"));

            // Member operator (get)
            Assert.AreEqual("abc", Evaluate("'abc'.toString()"));
            Assert.AreEqual("5.6", Evaluate("5.6.toString()"));
            Assert.AreEqual("5", Evaluate("5 .toString()"));
            Assert.AreEqual(1, Evaluate("x = { a: 1 }; x.a"));

            // Member operator (set)
            Assert.AreEqual(2, Evaluate("x = { a: 1 }; x.a = 2; x.a"));
            
            // Index operator (get)
            Assert.AreEqual("5.6", Evaluate("5.6['toString']()"));
            Assert.AreEqual("b", Evaluate("'abc'[1]"));
            Assert.AreEqual(Undefined.Value, Evaluate("'abc'[3]"));
            Assert.AreEqual("b", Evaluate("y = 1; 'abc'[y]"));
            Assert.AreEqual(1, Evaluate("x = { a: 1 }; x['a']"));
            Assert.AreEqual(1, Evaluate("x = { a: 1 }; y = 'a'; x[y]"));
            Assert.AreEqual(3, Evaluate("var a = {false: 3}; a[false]"));
            Assert.AreEqual(4, Evaluate("var a = [4]; a[[[[0]]]]"));
            Assert.AreEqual(5, Evaluate("var a = { 'abc' : 5 }; a[[[['abc']]]]"));

            // Index operator (set)
            Assert.AreEqual("5.6", Evaluate("var x = 5.6; x['toString'] = function() { }; x.toString()"));
            Assert.AreEqual("d", Evaluate("'abc'[1] = 'd'"));
            Assert.AreEqual("abc", Evaluate("var x = 'abc'; x[1] = 'd'; x"));
            Assert.AreEqual("d", Evaluate("'abc'[3] = 'd'"));
            Assert.AreEqual("abc", Evaluate("y = 1; var x = 'abc'; x[y] = 'd'; x"));
            Assert.AreEqual(2, Evaluate("x = { a: 1 }; x['a'] = 2; x['a']"));
            Assert.AreEqual(2, Evaluate("x = { a: 1 }; y = 'a'; x[y] = 2; x[y]"));

            // Check details of hidden class functionality.
            Assert.AreEqual(2, Evaluate("x = {}; x.a = 6; x.a = 2; x.a"));
            Assert.AreEqual(3, Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b"));
            Assert.AreEqual(3, Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b = 4; y.a"));
            Assert.AreEqual(4, Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b = 4; y.b"));
            Assert.AreEqual(3, Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b = 4; delete y.b; y.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = {}; x.a = 6; x.b = 2; y = {}; y.a = 3; y.b = 4; delete y.b; y.b"));

            // Symbols.
            Assert.AreEqual(7, Evaluate("var obj = { }; var symbol = Symbol(); obj[symbol] = 7; obj[symbol]"));
            Assert.AreEqual(8, Evaluate("var obj = { }; var sym1 = Symbol(); obj[sym1] = 8; var sym2 = Symbol(); obj[sym2] = 9; obj[sym1]"));

            // Ensure you can create at least 16384 properties.
            Assert.AreEqual(16383, Evaluate(@"
                var x = new Object();
                for (var i = 0; i < 16384; i ++)
                    x['prop' + i] = i;
                x.prop16383"));

            Assert.AreEqual("ReferenceError", EvaluateExceptionType("abcdefghij"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("5.toString"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("qwerty345.prop"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("null.prop"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("undefined.prop"));
        }

        [TestMethod]
        public void ArrayLiteral()
        {
            Assert.AreEqual(0, Evaluate("[].length"));
            Assert.AreEqual(1, Evaluate("[,].length"));   // JScript says 2.
            Assert.AreEqual(1, Evaluate("[1,].length"));   // JScript says 2.
            Assert.AreEqual(1, Evaluate("a = 1; [a][0]"));
            Assert.AreEqual(6, Evaluate("[[1, 2, 3], [4, 5, 6]][1][2]"));
            Assert.AreEqual(1, Evaluate("[1].length"));
            Assert.AreEqual(2, Evaluate("[1, 2].length"));
            Assert.AreEqual(true, Evaluate("[1,,2].hasOwnProperty('0')"));
            Assert.AreEqual(false, Evaluate("[1,,2].hasOwnProperty('1')"));
            Assert.AreEqual(true, Evaluate("[1,,2].hasOwnProperty('2')"));
            Assert.AreEqual(true, Evaluate("[1,undefined,2].hasOwnProperty('1')"));
            Assert.AreEqual(true, Evaluate("[] instanceof Array"));
        }

        [TestMethod]
        public void ObjectLiteral()
        {
            Assert.AreEqual("[object Object]", Evaluate("x = {}; x.toString()"));
            Assert.AreEqual(1, Evaluate("x = {a: 1}.a"));
            Assert.AreEqual(1, Evaluate("x = {a: 1, }.a"));
            Assert.AreEqual(1, Evaluate("x = {a: 1, b: 2}.a"));
            Assert.AreEqual(1, Evaluate("x = {'a': 1, 'b': 2}.a"));
            Assert.AreEqual(1, Evaluate("x = {0: 1, 1: 2}[0]"));
            Assert.AreEqual(2, Evaluate("x = {a: 1, b: 2}.b"));
            Assert.AreEqual(2, Evaluate("x = {a: 1, a: 2}.a"));   // This is an error in strict mode.
            Assert.AreEqual(3, Evaluate("var obj = {valueOf:0, toString:1, foo:2}; x = 0; for (var y in obj) { x ++; } x"));

            // Keywords are allowed in ES5.
            Assert.AreEqual(1, Evaluate("x = {if: 1}; x.if"));
            Assert.AreEqual(1, Evaluate("x = {eval: 1}; x.eval"));
            Assert.AreEqual(1, Evaluate("x = {false: 1}; x.false"));
            Assert.AreEqual(1, Evaluate("x = {true: 1}; x.true"));
            Assert.AreEqual(1, Evaluate("x = {null: 1}; x.null"));

            // Object literals can have getters and setters.
            Assert.AreEqual(1, Evaluate("x = {get f() { return 1; }}; x.f"));
            Assert.AreEqual(5, Evaluate("x = {set f(value) { this.a = value; }}; x.f = 5; x.a"));
            Assert.AreEqual(5, Evaluate("x = {get f() { return this.a; }, set f(value) { this.a = value; }}; x.f = 5; x.f"));
            Assert.AreEqual(1, Evaluate("x = {get f() { return 1; }}; x.f = 5; x.f"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = {set f(value) { this.a = value; }}; x.f"));
            Assert.AreEqual(2, Evaluate("x = {get: 2}; x.get"));
            Assert.AreEqual(3, Evaluate("x = {set: 3}; x.set"));
            Assert.AreEqual(1, Evaluate("x = {get 'f'() { return 1; }}; x.f = 5; x.f"));
            Assert.AreEqual(1, Evaluate("x = {get 0() { return 1; }}; x[0] = 5; x[0]"));
            Assert.AreEqual(4, Evaluate("var f = 4; x = {get f() { return f; }}; x.f"));

            // Check accessibility of getters and setters.
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor({ get a() {} }, 'a').configurable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor({ get a() {} }, 'a').enumerable"));
            Assert.AreEqual("get a", Evaluate("x = { get a() {} }; Object.getOwnPropertyDescriptor(x, 'a').get.name"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor({ get a() {}, set a(val) {} }, 'a').configurable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor({ get a() {}, set a(val) {} }, 'a').enumerable"));
            Assert.AreEqual("set a", Evaluate("x = { get a() {}, set a(val) {} }; Object.getOwnPropertyDescriptor(x, 'a').set.name"));

            // Check that "this" is correct inside getters and setters.
            Assert.AreEqual(9, Evaluate("x = { get b() { return this.a; } }; y = Object.create(x); y.a = 9; y.b"));
            Assert.AreEqual(9, Evaluate("x = { get b() { return this.a; } }; y = Object.create(x); y.a = 9; z = 'b'; y[z]"));
            Assert.AreEqual(9, Evaluate("x = { get '2'() { return this.a; } }; y = Object.create(x); y.a = 9; y[2]"));
            Assert.AreEqual(9, Evaluate("x = { get '2'() { return this.a; } }; y = Object.create(x); y.a = 9; z = 2; y[z]"));
            Assert.AreEqual(9, Evaluate("x = { set b(value) { this.a = value; } }; y = Object.create(x); y.b = 9; y.a"));
            Assert.AreEqual(true, Evaluate("x = { set b(value) { this.a = value; } }; y = Object.create(x); y.b = 9; y.hasOwnProperty('a')"));
            Assert.AreEqual(true, Evaluate("x = { set b(value) { this.a = value; } }; y = Object.create(x); z = 'b'; y[z] = 9; y.hasOwnProperty('a')"));

            // Duplicate property names are okay now, for some reason.
            Assert.AreEqual(3, Evaluate("x = {get a() { return 2 }, get a() { return 3 }}; x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = {set a(value) { }, set a(value) { }}; x.a"));
            Assert.AreEqual(2, Evaluate("x = {a: 1, get a() { return 2 }}; x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = {a: 1, set a(value) { }}; x.a"));
            Assert.AreEqual(3, Evaluate("x = {get a() { return 2 }, a: 3}; x.a"));
            Assert.AreEqual(1, Evaluate("x = {set a(value) { }, a: 1}; x.a"));

            // Errors
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("{a: 1, b: 2}"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = {a: 1,, }.a"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = {'get' f() { return 1; }}"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = {get f(a) { return 1; }}"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = {set f() { return 1; }}"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("x = {set f(a, b) { return 1; }}"));

            // Strict mode: eval is allowed in a object literal property.
            Assert.AreEqual(1, Evaluate("'use strict'; x = {eval: 1}; x.eval"));

            // Computed property names.
            Assert.AreEqual(1, Evaluate("var x = 'y'; ({ [x]: 1 }).y"));
            Assert.AreEqual(1, Evaluate("var x = 'y'; ({ get [x]() { return 1 } }).y"));
            Assert.AreEqual(true, Evaluate(@"var x1 = 'y', x2 = 'y',
                valueSet,
                obj = {
                  get [x1] () { return 1 },
                  set [x2] (value) { valueSet = value }
                };
                obj.y = 'foo';
                obj.y === 1 && valueSet === 'foo';"));

            // Shorthand properties.
            Assert.AreEqual(1, Evaluate("var a = 1, b = 2; x = {a, b}; x.a"));
            Assert.AreEqual(2, Evaluate("var a = 1, b = 2; x = {a, b}; x.b"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("var a = 1, b = { a: 3 }; x = {a, b.a}; x.a"));

            // Shorthand functions.
            Assert.AreEqual(3, Evaluate("var x = { a() { return 3; } }; x.a()"));
            Assert.AreEqual("a", Evaluate("var x = { a() { return 3; } }; x.a.name"));
            Assert.AreEqual(3, Evaluate("var x = { 5.5() { return 3; } }; x[5.5]()"));
            Assert.AreEqual(3, Evaluate("var x = { this() { return 3; } }; x.this()"));
            Assert.AreEqual(17, Evaluate("var x = { 'baby superman'() { return 17; } }; x['baby superman']()"));
        }

        [TestMethod]
        public void Delete()
        {
            // Delete property.
            Assert.AreEqual(true, Evaluate("x = {a: 1, b: 2}; delete x.a"));
            Assert.AreEqual(true, Evaluate("x = {a: 1, b: 2}; delete(x.a)"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = {a: 1, b: 2}; delete x.a; x.a"));

            // Delete property (alternate syntax).
            Assert.AreEqual(true, Evaluate("x = {a: 1, b: 2}; delete x['a']"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = {a: 1, b: 2}; delete x['a']; x.a"));
            Assert.AreEqual(true, Evaluate("x = {a: 1, b: 2}; y = 'a'; delete x[y]"));
            Assert.AreEqual(Undefined.Value, Evaluate("x = {a: 1, b: 2}; y = 'a'; delete x[y]; x.a"));

            // Delete does not operate against the prototype chain.
            Assert.AreEqual(true, Evaluate("x = Object.create({a: 1}); delete x.a"));
            Assert.AreEqual(1, Evaluate("x = Object.create({a: 1}); delete x.a; x.a"));

            // Delete non-configurable property.
            Assert.AreEqual(false, Evaluate("delete Number.prototype"));

            // Deleting a global variable fails.
            Execute("var delete_test_1 = 1; var delete_test_2 = delete delete_test_1;");
            Assert.AreEqual(1, Evaluate("delete_test_1"));
            Assert.AreEqual(false, Evaluate("delete_test_2"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(this, 'delete_test_1').configurable"));

            // Deleting function variables fails.
            Assert.AreEqual(false, Evaluate("(function f(a) { return delete a; })(1)"));
            Assert.AreEqual(1, Evaluate("(function f(a) { delete a; return a; })(1)"));

            // Delete non-reference.
            Assert.AreEqual(true, Evaluate("delete 5"));  // does nothing

            // Delete this in an object scope.
            Assert.AreEqual(true, Evaluate("delete this"));

            // Delete this in a function scope.
            // IE: throws an error
            // Firefox: true
            // Chrome: false
            Assert.AreEqual(true, Evaluate("var f = function () { return delete this; }; var x = {'a': 1, 'f': f}; x.f()"));

            // Delete from a parent scope.
            Assert.AreEqual(false, Evaluate("a = 5; function f() { delete a } f(); this.hasOwnProperty('a')"));

            // Deleting variables defined within an eval statement inside a global scope succeeds.
            Assert.AreEqual(true, Evaluate("abcdefg = 1; delete abcdefg"));
            Assert.AreEqual(false, Evaluate("abcdefg = 1; delete abcdefg; this.hasOwnProperty('abcdefg')"));
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("x = 5; delete x; x"));

            // Deleting variables defined within an eval statement inside a function scope succeeds.
            Assert.AreEqual(true, Evaluate("(function() { var a = 5; return eval('var b = a; delete b'); })()"));
            Assert.AreEqual(true, Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); return delete b; })()"));
            Assert.AreEqual(1, Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); delete b; })(); b;"));
            Assert.AreEqual(1, Evaluate("b = 1; (function() { var a = 5; eval('var b = a'); delete b; return b; })()"));

            // Make sure delete calls functions.
            Assert.AreEqual(true, Evaluate("called = false; function f() { called = true; } delete f(); called"));

            // Strict mode: cannot delete a non-configurable property.
            Assert.AreEqual("TypeError", EvaluateExceptionType("'use strict'; var x = {}; Object.defineProperty(x, 'a', {value: 7, enumerable: true, writable: false, configurable: false}); delete x.a;"));

            // Strict mode: deleting a variable fails.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; var foo = 'test'; delete foo"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; function test(){} delete test"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("'use strict'; (function(arg) { delete arg; })()"));

            // Delete a symbol.
            Assert.AreEqual(true, Evaluate("delete ''[Symbol.iterator]"));
            Assert.AreEqual(5, Evaluate("var x = {}; x[Symbol.iterator] = 5; x[Symbol.iterator]"));
            Assert.AreEqual(true, Evaluate("delete x[Symbol.iterator]"));
            Assert.AreEqual(Undefined.Value, Evaluate("x[Symbol.iterator]"));
        }

        [TestMethod]
        public void Typeof()
        {
            Assert.AreEqual("undefined", Evaluate("typeof abcdefg"));
            Assert.AreEqual("undefined", Evaluate("typeof undefined"));
            Assert.AreEqual("object", Evaluate("typeof null"));
            Assert.AreEqual("undefined", Evaluate("typeof Math.abcdefg"));
            Assert.AreEqual("boolean", Evaluate("typeof true"));
            Assert.AreEqual("number", Evaluate("typeof 1"));
            Assert.AreEqual("number", Evaluate("typeof (NaN)"));
            Assert.AreEqual("number", Evaluate("typeof 1.5"));
            Assert.AreEqual("string", Evaluate("typeof 'hello'"));
            Assert.AreEqual("object", Evaluate("typeof /abc/"));
            Assert.AreEqual("object", Evaluate("typeof {}"));
            Assert.AreEqual("object", Evaluate("typeof []"));
            Assert.AreEqual("function", Evaluate("typeof Math.toString"));
            Assert.AreEqual("number", Evaluate("x = 1.5; typeof x"));
            Assert.AreEqual("string", Evaluate("x = 'hello'; typeof x"));
            Assert.AreEqual("number", Evaluate("x = 5; (function() { return typeof(x) })()"));
            Assert.AreEqual("number", Evaluate("typeof([1, 2].length)"));
        }

        [TestMethod]
        public void This()
        {
            // "this" is set to the global object by default.
            Assert.AreEqual(5, Evaluate("this.x = 5; this.x"));

            // In ES3 functions will get the global object as the "this" value by default.
            Assert.AreEqual(true, Evaluate("(function(){ return this; }).call(null) === this"));
            Assert.AreEqual(true, Evaluate("(function(){ return this; }).call(undefined) === this"));
            Assert.AreEqual(6, Evaluate("(function(){ return this; }).call(5) + 1"));
            Assert.AreEqual("object", Evaluate("typeof((function(){ return this; }).call(5))"));
            Assert.AreEqual(6, Evaluate("(function(){ return eval('this'); }).call(5) + 1"));

            // Check that the this parameter is passed correctly.
            Assert.AreEqual(true, Evaluate("x = { f: function() { return this } }; x.f() === x"));
            Assert.AreEqual(5, Evaluate("function x() { this.a = 5; this.f = function() { return this } }; new x().f().a"));

            // The "this" value cannot be modified.
            Assert.AreEqual("ReferenceError", EvaluateExceptionType("this = 5;"));

            // Strict mode: the "this" object is not coerced to an object.
            Assert.AreEqual(Null.Value, Evaluate("'use strict'; (function(){ return this; }).call(null)"));
            Assert.AreEqual(Undefined.Value, Evaluate("'use strict'; (function(){ return this; }).call(undefined)"));
            Assert.AreEqual(5, Evaluate("'use strict'; (function(){ return this; }).call(5)"));
            Assert.AreEqual("number", Evaluate("'use strict'; typeof((function(){ return this; }).call(5))"));
        }

        [TestMethod]
        [Ignore]    // tagged strings array should be frozen.
        public void TemplateLiterals()
        {
            Assert.AreEqual("nine", Evaluate("`nine`"));

            // Escape sequences
            Assert.AreEqual(" \x08 \x09 \x0a \x0b \x0c \x0d \x22 \x27 \x5c \x00 ", Evaluate(@"` \b \t \n \v \f \r \"" \' \\ \0 `"));
            Assert.AreEqual(@" $ $$ $\ ", Evaluate(@"` $ $$ $\\ `"));

            // Substitutions
            Assert.AreEqual("1 + 1 = 2", Evaluate("`1 + 1 = ${1 + 1}`"));
            Assert.AreEqual("Fifteen is 15 and not 20.", Evaluate("var a = 5; var b = 10; `Fifteen is ${a + b} and not ${2 * a + b}.`"));
            Assert.AreEqual("Object [object Object]", Evaluate("`Object ${{}}`"));

            // Nested substitutions
            Assert.AreEqual("1 + 1 = Hello 3 world", Evaluate("`1 + 1 = ${`Hello ${3} world`}`"));

            // Tagged templates
            Assert.AreEqual("Hello ,  world ,  | 15 | 50 | undefined", Evaluate(@"
                function tag(strings, value1, value2, value3) {
                    return strings.join(', ') + ' | ' + value1 + ' | ' + value2 + ' | ' + value3;
                }

                var a = 5, b = 10;
                tag`Hello ${a + b} world ${a * b}`;"));
            Assert.AreEqual("3 | ,, | 11 | 2 | undefined", Evaluate(@"
                function tag(strings, value1, value2, value3) {
                    return strings.length + ' | ' + strings.join(',') + ' | ' + value1 + ' | ' + value2 + ' | ' + value3;
                }
                tag `${11}${2}`;"));

            // Raw strings.
            Assert.AreEqual(@"2 | one\r\n,\r\nthree | two | undefined | undefined", Evaluate(@"
                function tag(strings, value1, value2, value3) {
                    return strings.raw.length + ' | ' + strings.raw.join(',') + ' | ' + value1 + ' | ' + value2 + ' | ' + value3;
                }
                tag `one\r\n${ 'two'}\r\nthree`;"));

            // Newline normalization.
            Assert.AreEqual("a\nb", Evaluate("`a\rb`"));
            Assert.AreEqual("a\nb", Evaluate("`a\nb`"));
            Assert.AreEqual("a\nb", Evaluate("`a\r\nb`"));

            // Check accessibility.
            Assert.AreEqual(true, Evaluate(@"function tag(strings) { return Object.isFrozen(strings); } tag `test`;"));
            Assert.AreEqual(true, Evaluate(@"function tag(strings) { return Object.isFrozen(strings.raw); } tag `test`;"));

            // Syntax errors
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`unterminated"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`unterminated\r\n"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"`sd\xfgf`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType(@"`te\ufffg`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`test\""));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`test'"));

            // Octal escape sequences are not supported in templates.
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\05`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\05a`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\011`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\0377`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\0400`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\09`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\0444`"));
            Assert.AreEqual("SyntaxError", EvaluateExceptionType("`\\44`"));
        }
    }
}
