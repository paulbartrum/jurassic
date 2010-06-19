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
    /// Test the Lexer object.
    /// </summary>
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void CharacterTests()
        {
            for (int i = 0; i < 65536; i++)
            {
                //// Whitespace.
                //bool isWhitespace = i == 0x9 || i == 0xB || i == 0xC || i == 0x20 || i == 0xA0 ||
                //    i == 0xFEFF || char.GetUnicodeCategory((char)i) == UnicodeCategory.SpaceSeparator;
                //Assert.AreEqual(isWhitespace, JSLexer.IsWhiteSpace(i), string.Format("Character {0} should be whitespace.", i));

                //// Line terminator.
                //bool isLineTerminator = i == 0xA || i == 0xD || i == 0x2028 || i == 0x2029;
                //Assert.AreEqual(isLineTerminator, JSLexer.IsLineTerminator(i), string.Format("Character {0} should be a line terminator.", i));

                //// Identifier start character.
                //bool isIdentifierStartChar = char.GetUnicodeCategory((char)i) == UnicodeCategory.UppercaseLetter ||
                //    char.GetUnicodeCategory((char)i) == UnicodeCategory.LowercaseLetter ||
                //    char.GetUnicodeCategory((char)i) == UnicodeCategory.TitlecaseLetter ||
                //    char.GetUnicodeCategory((char)i) == UnicodeCategory.ModifierLetter ||
                //    char.GetUnicodeCategory((char)i) == UnicodeCategory.OtherLetter ||
                //    char.GetUnicodeCategory((char)i) == UnicodeCategory.LetterNumber ||
                //    i == '$' || i == '_' || i == '\\';
                //Assert.AreEqual(isIdentifierStartChar, Lexer.IsIdentifierStartChar(i), string.Format("Character {0} should be an identifier start character.", i));

                //// Identifier character.
                //bool isIdentifierChar = isIdentifierStartChar ||
                //    char.GetUnicodeCategory((char)i) == UnicodeCategory.NonSpacingMark ||
                //    char.GetUnicodeCategory((char)i) == UnicodeCategory.SpacingCombiningMark ||
                //    char.GetUnicodeCategory((char)i) == UnicodeCategory.DecimalDigitNumber ||
                //    char.GetUnicodeCategory((char)i) == UnicodeCategory.ConnectorPunctuation ||
                //    i == 0x200C || i == 0x200D;
                //Assert.AreEqual(isIdentifierChar, Lexer.IsIdentifierChar(i), string.Format("Character {0} should be a identifier character.", i));
            }
        }

        [TestMethod]
        public void Comments()
        {
            // Single-line comment.
            Assert.AreEqual("", ToTokenString("// testing"));
            Assert.AreEqual("{LT 1}", ToTokenString("// testing\r\n"));

            // Multi-line comment.
            Assert.AreEqual("", ToTokenString("/* testing */"));
            Assert.AreEqual("{LT 1}", ToTokenString("/* test\r\ning */"));
            Assert.AreEqual("{LT 2}", ToTokenString("/*\r\n test\r\ning */"));
            Assert.AreEqual("{LT 1}", ToTokenString("/* testing \r\n*/"));
            Assert.AreEqual("{LT 1}", ToTokenString("/* test\ning */"));
            Assert.AreEqual("{LT 1}", ToTokenString("/* test\ring */"));
            Assert.AreEqual("", ToTokenString("/* te*sting**/"));
        }

        [TestMethod]
        public void Identifier()
        {
            Assert.AreEqual("{Identifier identifier}", ToTokenString("identifier"));
            Assert.AreEqual("{Identifier $}", ToTokenString("$"));
            Assert.AreEqual("{Identifier dung}", ToTokenString("d\\u0075ng"));
            Assert.AreEqual("{Identifier another}", ToTokenString("\\u0061nother"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("ident\\u0020ifier"));
        }

        [TestMethod]
        public void Null()
        {
            Assert.AreEqual("{Literal null}", ToTokenString("null"));
        }

        [TestMethod]
        public void Boolean()
        {
            Assert.AreEqual("{Literal False}", ToTokenString("false"));
            Assert.AreEqual("{Literal True}", ToTokenString("true"));
        }

        [TestMethod]
        public void Number()
        {
            Assert.AreEqual("{Literal 0}", ToTokenString("0"));
            Assert.AreEqual("{Literal 34}", ToTokenString("34"));
            Assert.AreEqual("{Literal 34.5}", ToTokenString("34.5"));
            Assert.AreEqual("{Literal 3400}", ToTokenString("34e2"));
            Assert.AreEqual("{Literal 3.45}", ToTokenString("34.5e-1"));
            Assert.AreEqual("{Literal 0.345}", ToTokenString("34.5E-2"));
            Assert.AreEqual("{Literal 11}", ToTokenString(" 11"));
            Assert.AreEqual("{Literal 0.5}", ToTokenString("0.5"));
            Assert.AreEqual("{Literal 0.005}", ToTokenString("0.005"));
            Assert.AreEqual("{Literal 255}", ToTokenString("0xff"));
            Assert.AreEqual("{Literal 241}", ToTokenString("0xF1"));
            Assert.AreEqual("{Literal 0.5}", ToTokenString(".5"));
            Assert.AreEqual("{Literal 50}", ToTokenString(".5e2"));
            Assert.AreEqual("{Literal 5}", ToTokenString("5."));
            Assert.AreEqual("{Literal 5}{Identifier g}", ToTokenString("0x5g"));
            Assert.AreEqual("{Punctuator .}", ToTokenString("."));
            Assert.AreEqual("{Punctuator .}{Identifier e}", ToTokenString(".e"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("5.e"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("5e"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("5e+"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("5e.5"));
        }

        [TestMethod]
        public void String()
        {
            Assert.AreEqual("{Literal nine}", ToTokenString("'nine'"));
            Assert.AreEqual("{Literal eight}", ToTokenString("\"eight\""));
            Assert.AreEqual("{Literal  \x08 \x09 \x0a \x0b \x0c \x0d \x22 \x27 \x5c \x00 }", ToTokenString(@"' \b \t \n \v \f \r \"" \' \\ \0 '"));
            Assert.AreEqual("{Literal ÿ}", ToTokenString(@"'\xfF'"));
            Assert.AreEqual("{Literal ①ﬄ}", ToTokenString(@"'\u2460\ufB04'"));
            Assert.AreEqual("{MLLiteral line-continuation 3}", ToTokenString("'line-\\\r\ncon\\\rtin\\\nuation'"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("'unterminated"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("'unterminated\r\n"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString(@"'sd\xfgf'"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString(@"'te\ufffg'"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("'te\r\nst'"));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("'test\""));
            TestUtils.ExpectException<JavaScriptException>(() => ToTokenString("\"test'"));
        }

        [TestMethod]
        public void Punctuator()
        {
            Assert.AreEqual("{Identifier x}{Punctuator +=}{Literal 5}", ToTokenString("x += 5"));
            Assert.AreEqual("{Identifier x}{Punctuator /}{Literal 5}", ToTokenString("x / 5"));
            Assert.AreEqual("{Identifier x}{Punctuator /=}{Literal 5}", ToTokenString("x /= 5"));
        }

        [TestMethod]
        public void RegExp()
        {
            Assert.AreEqual(@"{Literal /abc/gi}", ToTokenString("/abc/gi"));

            // A slash inside a character class does not terminate the regular expression.
            Assert.AreEqual(@"{Literal /[/]/g}", ToTokenString("/[/]/g"));

            // An escaped slash also does not terminate the regular expression.
            Assert.AreEqual(@"{Literal /\//i}", ToTokenString(@"/\//i"));

            // The closing bracket can be escaped.
            Assert.AreEqual(@"{Literal /[\]/]/i}", ToTokenString(@"/[\]/]/i"));
        }

        [TestMethod]
        public void Keyword()
        {
            Assert.AreEqual("{Keyword for}", ToTokenString("for"));
            Assert.AreEqual("{Keyword this}", ToTokenString("this"));
        }

        private string ToTokenString(string source)
        {
            var result = new StringBuilder();
            var lexer = new Lexer(new System.IO.StringReader(source), null);
            while (true)
            {
                var token = lexer.NextToken();
                if (token == null)
                    break;
                if (token is IdentifierToken)
                    result.AppendFormat("{{Identifier {0}}}", ((IdentifierToken)token).Name);
                else if (token is MultiLineLiteralToken)
                    result.AppendFormat("{{MLLiteral {0} {1}}}", ((MultiLineLiteralToken)token).Value, ((MultiLineLiteralToken)token).LineTerminatorCount);
                else if (token is LiteralToken)
                    result.AppendFormat("{{Literal {0}}}", ((LiteralToken)token).Value);
                else if (token is WhiteSpaceToken)
                    result.AppendFormat("{{LT {0}}}", ((WhiteSpaceToken)token).LineTerminatorCount);
                else if (token is KeywordToken)
                    result.AppendFormat("{{Keyword {0}}}", ((KeywordToken)token).Name);
                else if (token is PunctuatorToken)
                    result.AppendFormat("{{Punctuator {0}}}", ((PunctuatorToken)token).Text);
                else
                    result.AppendFormat("{{{0}}}", token.GetType().Name);
            }
            return result.ToString();
        }
    }
}
