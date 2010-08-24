using System;
using System.IO;
using System.Text;
using Jurassic.Compiler;

namespace Jurassic.Library
{
    /// <summary>
    /// Converts JSON text into a series of tokens.
    /// </summary>
    internal class JSONLexer
    {
        private ScriptEngine engine;
        private TextReader reader;

        /// <summary>
        /// Creates a JSONLexer instance with the given source of text.
        /// </summary>
        /// <param name="engine"> The script engine used to create error objects. </param>
        /// <param name="reader"> A reader that will supply the JSON source text. </param>
        public JSONLexer(ScriptEngine engine, TextReader reader)
        {
            if (engine == null)
                throw new ArgumentNullException("engine");
            if (reader == null)
                throw new ArgumentNullException("reader");
            this.engine = engine;
            this.reader = reader;
        }

        /// <summary>
        /// Gets the reader that was supplied to the constructor.
        /// </summary>
        public TextReader Reader
        {
            get { return this.reader; }
        }

        /// <summary>
        /// Reads the next token from the reader.
        /// </summary>
        /// <returns> A token, or <c>null</c> if there are no more tokens. </returns>
        public Token NextToken()
        {
            int c = this.reader.Read();

            if (IsWhiteSpace(c) == true)
            {
                // White space.
                return ReadWhiteSpace();
            }
            else if (IsIdentifierChar(c) == true)
            {
                // Reserved word.
                return ReadKeyword(c);
            }
            else if (c == '\"')
            {
                // String literal.
                return ReadStringLiteral(c);
            }
            else if (IsNumericLiteralStartChar(c) == true)
            {
                // Number literal.
                return ReadNumericLiteral(c);
            }
            else
            {
                switch (c)
                {
                    case '{':
                        return PunctuatorToken.LeftBrace;
                    case '[':
                        return PunctuatorToken.LeftBracket;
                    case '}':
                        return PunctuatorToken.RightBrace;
                    case ']':
                        return PunctuatorToken.RightBracket;
                    case ',':
                        return PunctuatorToken.Comma;
                    case ':':
                        return PunctuatorToken.Colon;
                    case -1:
                        // End of input.
                        return null;
                    default:
                        throw new JavaScriptException(this.engine, "SyntaxError", string.Format("Unexpected character '{0}'.", (char)c));
                }
            }
        }

        /// <summary>
        /// Reads an keyword token.
        /// </summary>
        /// <param name="firstChar"> The first character of the identifier. </param>
        /// <returns> An keyword token. </returns>
        private Token ReadKeyword(int firstChar)
        {
            // Process the first character.
            var name = new StringBuilder(5);
            name.Append((char)firstChar);

            // Read characters until we hit the first non-identifier character.
            while (true)
            {
                int c = this.reader.Peek();
                if (IsIdentifierChar(c) == false || c == -1)
                    break;

                // Add the character we peeked at to the identifier name.
                name.Append((char)c);

                // Advance the input stream.
                this.reader.Read();
            }

            // Check if the identifier is boolean literal or null literal.
            string keyword = name.ToString();
            if (keyword == "null")
                return LiteralToken.Null;
            else if (keyword == "false")
                return LiteralToken.False;
            else if (keyword == "true")
                return LiteralToken.True;
            else
                throw new JavaScriptException(this.engine, "SyntaxError", string.Format("Unexpected keyword '{0}'", keyword));
        }

        /// <summary>
        /// Reads a numeric literal token.
        /// </summary>
        /// <param name="firstChar"> The first character of the token. </param>
        /// <returns> A numeric literal token. </returns>
        private Token ReadNumericLiteral(int firstChar)
        {
            double result;

            // The number may start with a negation.
            double sign = firstChar == '-' ? -1.0 : 1.0;

            // Read the integer component.
            int digitsRead;
            result = ReadInteger(firstChar == '-' ? 0 : firstChar - '0', out digitsRead);

            // If the first character is '0' then a period must be the next character.
            if (firstChar == '0' && digitsRead != 0)
                throw new JavaScriptException(this.engine, "SyntaxError", "Invalid number");

            // If the first character is '-' then a digit must be the next character.
            if (firstChar == '-' && digitsRead == 0)
                throw new JavaScriptException(this.engine, "SyntaxError", "Invalid number");

            if (this.reader.Peek() == '.')
            {
                // Skip past the '.'.
                this.reader.Read();

                // Read the fractional component.
                double fraction = ReadInteger(0.0, out digitsRead);
                if (digitsRead == 0)
                    throw new JavaScriptException(this.engine, "SyntaxError", "Invalid number");

                // Apply the fractional component.
                result += MathHelpers.MulPow10(fraction, -digitsRead);
            }

            if (reader.Peek() == 'e' || reader.Peek() == 'E')
            {
                // Skip past the 'e'.
                reader.Read();

                // Read the sign of the exponent.
                double exponentSign = 1.0;
                int c = this.reader.Peek();
                if (c == '+')
                    this.reader.Read();
                else if (c == '-')
                {
                    this.reader.Read();
                    exponentSign = -1.0;
                }

                // 5e05 is invalid.  To detect this error we record the first digit.
                firstChar = this.reader.Peek();

                // Read the exponent.
                double exponent = ReadInteger(0.0, out digitsRead) * exponentSign;

                // Check a number was actually provided.
                if (digitsRead == 0)
                    throw new JavaScriptException(this.engine, "SyntaxError", "Invalid number.");

                // If the first character of the exponent is '0' then a period must be the next character.
                if (firstChar == '0' && digitsRead > 1)
                    throw new JavaScriptException(this.engine, "SyntaxError", "Invalid number");

                // Apply the exponent.
                result = MathHelpers.MulPow10(result, MathHelpers.ClampToInt32(exponent));
            }

            return new LiteralToken(result * sign);
        }

        /// <summary>
        /// Reads an integer value.
        /// </summary>
        /// <param name="initialValue"> The initial value, derived from the first character. </param>
        /// <param name="digitsRead"> The number of digits that were read from the stream. </param>
        /// <returns> The numeric value, or <c>double.NaN</c> if no number was present. </returns>
        private double ReadInteger(double initialValue, out int digitsRead)
        {
            double result = initialValue;
            digitsRead = 0;

            while (true)
            {
                int c = this.reader.Peek();
                if (c < '0' || c > '9')
                    break;
                this.reader.Read();
                digitsRead++;
                result = result * 10 + (c - '0');
            }

            return result;
        }

        /// <summary>
        /// Reads a string literal.
        /// </summary>
        /// <param name="firstChar"> The first character of the string literal. </param>
        /// <returns> A string literal. </returns>
        private Token ReadStringLiteral(int firstChar)
        {
            System.Diagnostics.Debug.Assert(firstChar == '"');
            var contents = new StringBuilder();
            while (true)
            {
                int c = this.reader.Read();
                if (c == '"')
                    break;
                if (c == -1)
                    throw new JavaScriptException(this.engine, "SyntaxError", "Unexpected end of input in string literal");
                if (c < 0x20)
                    throw new JavaScriptException(this.engine, "SyntaxError", "Unexpected character in string literal");

                if (c == '\\')
                {
                    // Escape sequence.
                    c = this.reader.Read();
                    switch (c)
                    {
                        case '\"':
                            // Double quote.
                            contents.Append('\"');
                            break;
                        case '/':
                            // Slash.
                            contents.Append('/');
                            break;
                        case '\\':
                            // Backslash.
                            contents.Append('\\');
                            break;
                        case 'b':
                            // Backspace.
                            contents.Append((char)0x08);
                            break;
                        case 'f':
                            // Form feed.
                            contents.Append((char)0x0C);
                            break;
                        case 'n':
                            // Line feed.
                            contents.Append((char)0x0A);
                            break;
                        case 'r':
                            // Carriage return.
                            contents.Append((char)0x0D);
                            break;
                        case 't':
                            // Horizontal tab.
                            contents.Append((char)0x09);
                            break;
                        case 'u':
                            // Unicode escape sequence.
                            contents.Append(ReadHexNumber(4));
                            break;
                        default:
                            throw new JavaScriptException(this.engine, "SyntaxError", "Unexpected character in escape sequence.");
                    }
                }
                else
                {
                    contents.Append((char)c);
                }
            }
            return new LiteralToken(contents.ToString());
        }

        /// <summary>
        /// Reads a hexidecimal number with the given number of digits and turns it into a character.
        /// </summary>
        /// <returns> The character corresponding to the escape sequence, or the content that was read
        /// from the input if a valid hex number was not read. </returns>
        private char ReadHexNumber(int digitCount)
        {
            var contents = new StringBuilder(digitCount);
            for (int i = 0; i < digitCount; i++)
            {
                int c = this.reader.Read();
                contents.Append((char)c);
                if (IsHexDigit(c) == false)
                    throw new JavaScriptException(this.engine, "SyntaxError", string.Format("Invalid hex digit '{0}' in escape sequence.", (char)c));
            }
            return (char)int.Parse(contents.ToString(), System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Reads past whitespace.
        /// </summary>
        /// <returns> Always returns <c>null</c>. </returns>
        private Token ReadWhiteSpace()
        {
            // Read all the characters up to the next non-whitespace character.
            while (true)
            {
                int c = this.reader.Peek();
                if (IsWhiteSpace(c) == false || c == -1)
                    break;

                // Advance the reader.
                this.reader.Read();
            }
            return new WhiteSpaceToken(0);
        }

        /// <summary>
        /// Determines if the given character is whitespace or a line terminator.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is whitespace or a line terminator; <c>false</c>
        /// otherwise. </returns>
        private static bool IsWhiteSpace(int c)
        {
            return c == 0x09 || c == 0x0A || c == 0x0D || c == 0x20;
        }

        /// <summary>
        /// Determines if the given character is valid as a character of an identifier.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is is valid as a character of an identifier;
        /// <c>false</c> otherwise. </returns>
        private static bool IsIdentifierChar(int c)
        {
            return c >= 'a' && c <= 'z';
        }

        /// <summary>
        /// Determines if the given character is valid as the first character of a numeric literal.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is is valid as the first character of a numeric
        /// literal; <c>false</c> otherwise. </returns>
        private bool IsNumericLiteralStartChar(int c)
        {
            return (c >= '0' && c <= '9') || c == '-';
        }

        /// <summary>
        /// Determines if the given character is valid in a hexidecimal number.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the given character is valid in a hexidecimal number; <c>false</c>
        /// otherwise. </returns>
        private static bool IsHexDigit(int c)
        {
            return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
        }
    }

}