using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the current expression state of the parser.
    /// </summary>
    internal enum ExpressionState
    {
        /// <summary>
        /// Indicates the context is not known.  The lexer will guess.
        /// </summary>
        UnknownContext,

        /// <summary>
        /// Indicates the next token can be a literal.
        /// </summary>
        LiteralContext,

        /// <summary>
        /// Indicates the next token can be an operator.
        /// </summary>
        OperatorContext,
    }

    /// <summary>
    /// Converts a JavaScript source file into a series of tokens.
    /// </summary>
    internal class Lexer
    {
        private ScriptEngine engine;
        private ScriptSource source;
        private TextReader reader;
        private int lineNumber, columnNumber;

        /// <summary>
        /// Creates a Lexer instance with the given source of text.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="source"> The source of javascript code. </param>
        public Lexer(ScriptEngine engine, ScriptSource source)
        {
            if (engine == null)
                throw new ArgumentNullException("engine");
            if (source == null)
                throw new ArgumentNullException("source");
            this.engine = engine;
            this.source = source;
            this.reader = source.GetReader();
            this.lineNumber = 1;
            this.columnNumber = 1;
        }

        /// <summary>
        /// Gets the reader that was supplied to the constructor.
        /// </summary>
        public ScriptSource Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the line number of the next token.
        /// </summary>
        public int LineNumber
        {
            get { return this.lineNumber; }
        }

        /// <summary>
        /// Gets the column number of the start of the next token.
        /// </summary>
        public int ColumnNumber
        {
            get { return this.columnNumber; }
        }

        /// <summary>
        /// Gets or sets a callback that interrogates the parser to determine whether a literal or
        /// an operator is valid as the next token.  This is only required to disambiguate the
        /// slash symbol (/) which can be a division operator or a regular expression literal.
        /// </summary>
        public Func<ExpressionState> ExpressionStateCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Reads the next character from the input stream.
        /// </summary>
        /// <returns> The character that was read, or <c>-1</c> if the end of the input stream has
        /// been reached. </returns>
        private int ReadNextChar()
        {
            this.columnNumber++;
            return this.reader.Read();
        }

        // Needed to disambiguate regular expressions.
        private Token lastSignificantToken;

        /// <summary>
        /// Reads the next token from the reader.
        /// </summary>
        /// <returns> A token, or <c>null</c> if there are no more tokens. </returns>
        public Token NextToken()
        {
            int c1 = ReadNextChar();

            if (IsPunctuatorStartChar(c1) == true)
            {
                // Punctuator (puntcuation + operators).
                this.lastSignificantToken = ReadPunctuator(c1);
                return this.lastSignificantToken;
            }
            else if (IsWhiteSpace(c1) == true)
            {
                // White space.
                return ReadWhiteSpace();
            }
            else if (IsIdentifierStartChar(c1) == true)
            {
                // Identifier or reserved word.
                this.lastSignificantToken = ReadIdentifier(c1);
                return this.lastSignificantToken;
            }
            else if (IsStringLiteralStartChar(c1) == true)
            {
                // String literal.
                this.lastSignificantToken = ReadStringLiteral(c1);
                return this.lastSignificantToken;
            }
            else if (IsNumericLiteralStartChar(c1) == true)
            {
                // Number literal.
                this.lastSignificantToken = ReadNumericLiteral(c1);
                return this.lastSignificantToken;
            }
            else if (IsLineTerminator(c1) == true)
            {
                // Line Terminator.
                this.lastSignificantToken = ReadLineTerminator(c1);
                return this.lastSignificantToken;
            }
            else if (c1 == '/')
            {
                // Comment or divide or regular expression.
                this.lastSignificantToken = ReadDivideCommentOrRegularExpression();
                return this.lastSignificantToken;
            }
            else if (c1 == -1)
            {
                // End of input.
                this.lastSignificantToken = null;
                return null;
            }
            else
                throw new JavaScriptException(this.engine, "SyntaxError", string.Format("Unexpected character '{0}'.", (char)c1), this.lineNumber, this.Source.Path);
        }

        /// <summary>
        /// Reads an identifier token.
        /// </summary>
        /// <param name="firstChar"> The first character of the identifier. </param>
        /// <returns> An identifier token, literal token or a keyword token. </returns>
        private Token ReadIdentifier(int firstChar)
        {
            // Process the first character.
            var name = new StringBuilder();
            if (firstChar == '\\')
            {
                // Unicode escape sequence.
                if (ReadNextChar() != 'u')
                    throw new JavaScriptException(this.engine, "SyntaxError", "Invalid escape sequence in identifier.", this.lineNumber, this.Source.Path);
                firstChar = ReadHexNumber(4);
                if (IsIdentifierChar(firstChar) == false)
                    throw new JavaScriptException(this.engine, "SyntaxError", "Invalid character in identifier.", this.lineNumber, this.Source.Path);
            }
            name.Append((char)firstChar);

            // Read characters until we hit the first non-identifier character.
            while (true)
            {
                int c = this.reader.Peek();
                if (IsIdentifierChar(c) == false || c == -1)
                    break;

                if (c == '\\')
                {
                    // Unicode escape sequence.
                    ReadNextChar();
                    if (ReadNextChar() != 'u')
                        throw new JavaScriptException(this.engine, "SyntaxError", "Invalid escape sequence in identifier.", this.lineNumber, this.Source.Path);
                    c = ReadHexNumber(4);
                    if (IsIdentifierChar(c) == false)
                        throw new JavaScriptException(this.engine, "SyntaxError", "Invalid character in identifier.", this.lineNumber, this.Source.Path);
                    name.Append((char)c);
                }
                else
                {
                    // Add the character we peeked at to the identifier name.
                    name.Append((char)c);

                    // Advance the input stream.
                    ReadNextChar();
                }
            }

            // Check if the identifier is actually a keyword, boolean literal, or null literal.
            return KeywordToken.FromString(name.ToString());
        }

        /// <summary>
        /// Reads a punctuation token.
        /// </summary>
        /// <param name="firstChar"> The first character of the punctuation token. </param>
        /// <returns> A punctuation token. </returns>
        private Token ReadPunctuator(int firstChar)
        {
            // The most likely case is the the punctuator is a single character and is followed by a space.
            var punctuator = PunctuatorToken.FromString(new string((char)firstChar, 1));
            if (this.reader.Peek() == ' ')
                return punctuator;

            // Otherwise, read characters until we find a string that is not a punctuator.
            var punctuatorText = new StringBuilder(4);
            punctuatorText.Append((char)firstChar);
            while (true)
            {
                int c = this.reader.Peek();
                if (c == -1)
                    break;

                // Try to parse the text as a punctuator.
                punctuatorText.Append((char)c);
                var longPunctuator = PunctuatorToken.FromString(punctuatorText.ToString());
                if (longPunctuator == null)
                    break;
                punctuator = longPunctuator;

                // Advance the input stream.
                ReadNextChar();
            }
            return punctuator;
        }

        /// <summary>
        /// Reads a numeric literal token.
        /// </summary>
        /// <param name="firstChar"> The first character of the token. </param>
        /// <returns> A numeric literal token. </returns>
        private Token ReadNumericLiteral(int firstChar)
        {
            double result;

            // If the number starts with '0x' or '0X' then the number should be parsed as a hex
            // number.
            if (firstChar == '0')
            {
                // Read the next char - should be 'x' or 'X' if this is a hex number (could be just '0').
                int c = this.reader.Peek();
                if (c == 'x' || c == 'X')
                {
                    // Hex number.
                    ReadNextChar();

                    // Read numeric digits 0-9, a-z or A-Z.
                    result = 0;
                    while (true)
                    {
                        c = this.reader.Peek();
                        if (c >= '0' && c <= '9')
                            result = result * 16 + c - '0';
                        else if (c >= 'a' && c <= 'f')
                            result = result * 16 + c - 'a' + 10;
                        else if (c >= 'A' && c <= 'F')
                            result = result * 16 + c - 'A' + 10;
                        else
                            break;
                        ReadNextChar();
                    }

                    if (result == (double)(int)result)
                        return new LiteralToken((int)result);
                    return new LiteralToken(result);
                }
                else if (c >= '0' && c <= '9')
                {
                    // Octal number.
                    throw new JavaScriptException(this.engine, "SyntaxError", "Octal numbers are not supported.", this.lineNumber, this.Source.Path);
                }
            }

            // Read the integer component.
            int digitsRead;
            if (firstChar == '.')
                result = double.NaN;
            else
                result = ReadInteger(firstChar - '0', out digitsRead);

            if (firstChar == '.' || this.reader.Peek() == '.')
            {
                // Skip past the '.'.
                if (firstChar != '.')
                    ReadNextChar();

                // Read the fractional component.
                double fraction = ReadInteger(0.0, out digitsRead);

                // Check a number was actually provided.
                if (double.IsNaN(result) == true && digitsRead == 0)
                    return PunctuatorToken.Dot;

                // '.5' should return 0.5.
                if (double.IsNaN(result) == true)
                    result = 0;

                // '5.' should return 5.0.
                if (digitsRead > 0)
                {
                    // Apply the fractional component.
                    result += fraction / System.Math.Pow(10, digitsRead);
                }
            }

            if (reader.Peek() == 'e' || reader.Peek() == 'E')
            {
                // Skip past the 'e'.
                reader.Read();

                // Read the sign of the exponent.
                double exponentSign = 1.0;
                int c = this.reader.Peek();
                if (c == '+')
                    ReadNextChar();
                else if (c == '-')
                {
                    ReadNextChar();
                    exponentSign = -1.0;
                }

                // Read the exponent.
                double exponent = ReadInteger(0.0, out digitsRead) * exponentSign;

                // Check a number was actually provided.
                if (double.IsNaN(result) == true || digitsRead == 0)
                    throw new JavaScriptException(this.engine, "SyntaxError", "Invalid number.", this.lineNumber, this.Source.Path);

                // Apply the exponent.
                if (exponent >= 0)
                    result *= System.Math.Pow(10, exponent);
                else
                    result /= System.Math.Pow(10, -exponent);
            }

            if (result == (double)(int)result)
                return new LiteralToken((int)result);
            return new LiteralToken(result);
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
                ReadNextChar();
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
            System.Diagnostics.Debug.Assert(firstChar == '\'' || firstChar == '"');
            var contents = new StringBuilder();
            int lineTerminatorCount = 0;
            int escapeSequenceCount = 0;
            while (true)
            {
                int c = ReadNextChar();
                if (c == firstChar)
                    break;
                if (c == -1)
                    throw new JavaScriptException(this.engine, "SyntaxError", "Unexpected end of input in string literal.", this.lineNumber, this.Source.Path);
                if (IsLineTerminator(c))
                    throw new JavaScriptException(this.engine, "SyntaxError", "Unexpected line terminator in string literal.", this.lineNumber, this.Source.Path);
                if (c == '\\')
                {
                    // Escape sequence or line continuation.
                    c = ReadNextChar();
                    if (IsLineTerminator(c))
                    {
                        // Line continuation.
                        ReadLineTerminator(c);

                        // Keep track of the number of line terminators so the parser can compute
                        // line numbers correctly.
                        lineTerminatorCount++;

                        // Increment the internal line number so errors can be tracked properly.
                        this.lineNumber++;
                        this.columnNumber = 1;
                    }
                    else
                    {
                        // Escape sequence.
                        switch (c)
                        {
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
                            case 'v':
                                // Vertical tab.
                                contents.Append((char)0x0B);
                                break;
                            case 'x':
                                // ASCII escape.
                                contents.Append(ReadHexNumber(2));
                                break;
                            case 'u':
                                // Unicode escape.
                                contents.Append(ReadHexNumber(4));
                                break;
                            case '0':
                                // Null character or octal escape sequence.
                                contents.Append((char)0);
                                c = this.reader.Peek();
                                if (c >= '0' && c <= '9')
                                    throw new JavaScriptException(this.engine, "SyntaxError", "Octal escape sequences are not supported.", this.lineNumber, this.Source.Path);
                                break;
                            default:
                                contents.Append((char)c);
                                break;
                        }
                        escapeSequenceCount ++;
                    }
                }
                else
                {
                    contents.Append((char)c);
                }
            }
            return new StringLiteralToken(contents.ToString(), escapeSequenceCount, lineTerminatorCount);
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
                int c = ReadNextChar();
                contents.Append((char)c);
                if (IsHexDigit(c) == false)
                    throw new JavaScriptException(this.engine, "SyntaxError", string.Format("Invalid hex digit '{0}' in escape sequence.", (char)c), this.lineNumber, this.Source.Path);
            }
            return (char)int.Parse(contents.ToString(), System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Reads past a single line comment.
        /// </summary>
        /// <returns> Always returns <c>null</c>. </returns>
        private Token ReadSingleLineComment()
        {
            // Read all the characters up to the newline.
            // The newline is a seperate token.
            while (true)
            {
                int c = this.reader.Peek();
                if (IsLineTerminator(c) || c == -1)
                    break;
                ReadNextChar();
            }

            return new WhiteSpaceToken(0);
        }

        /// <summary>
        /// Reads past a multi-line comment.
        /// </summary>
        /// <returns> A line terminator token if the multi-line comment contains a newline character;
        /// otherwise returns <c>null</c>. </returns>
        private Token ReadMultiLineComment()
        {
            // Multi-line comments that are actually on multiple lines are treated slighly
            // differently from multi-line comments that only span a single line, with respect
            // to implicit semi-colon insertion.
            int lineTerminatorCount = 0;

            // Read the first character.
            int c1 = ReadNextChar();
            if (c1 == -1)
                throw new JavaScriptException(this.engine, "SyntaxError", "Unexpected end of input in multi-line comment.", this.lineNumber, this.Source.Path);

            // Read all the characters up to the "*/".
            while (true)
            {
                int c2 = ReadNextChar();

                if (IsLineTerminator(c1) == true)
                {
                    // Keep track of the number of line terminators so the parser can compute
                    // line numbers correctly.
                    lineTerminatorCount++;

                    // Increment the internal line number so errors can be tracked properly.
                    this.lineNumber++;
                    this.columnNumber = 1;

                    // If the sequence is CRLF then only count that as one new line rather than two.
                    if (c1 == 0x0D && c2 == 0x0A)   // CRLF
                        c1 = c2 = ReadNextChar();
                }

                // Look for */ combination.
                if (c1 == '*' && c2 == '/')
                    break;
                c1 = c2;
            }

            return new WhiteSpaceToken(lineTerminatorCount);
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
                ReadNextChar();
            }
            return new WhiteSpaceToken(0);
        }

        /// <summary>
        /// Reads a line terminator (a newline).
        /// </summary>
        /// <param name="firstChar"> The first character of the line terminator. </param>
        /// <returns> A newline token. </returns>
        private Token ReadLineTerminator(int firstChar)
        {
            // Check for a CRLF sequence, if so that counts as one line terminator and not two.
            int c = this.reader.Peek();
            if (firstChar == 0x0D && c == 0x0A)   // CRLF
                ReadNextChar();

            // Increment the internal line number so errors can be tracked properly.
            this.lineNumber++;
            this.columnNumber = 1;

            // Return a line terminator token.
            return new WhiteSpaceToken(1);
        }

        /// <summary>
        /// Reads a divide operator ('/' or '/='), a comment ('//' or '/*'), or a regular expression
        /// literal.
        /// </summary>
        /// <returns> A punctuator token or a regular expression token. </returns>
        private Token ReadDivideCommentOrRegularExpression()
        {
            // Comment or divide or regular expression.
            int c2 = this.reader.Peek();
            if (c2 == '*')
            {
                // Multi-line comment.

                // Skip the asterisk.
                ReadNextChar();

                return ReadMultiLineComment();
            }
            else if (c2 == '/')
            {
                // Single-line comment.

                // Skip the slash.
                ReadNextChar();

                return ReadSingleLineComment();
            }
            else
            {
                // Divide or regular expression.

                // Get the current parser context.
                var parserContext = ExpressionState.UnknownContext;
                if (this.ExpressionStateCallback != null)
                    parserContext = this.ExpressionStateCallback();

                // Determine from the context whether the token is a regular expression
                // or a division operator.
                bool isDivisionOperator;
                switch (parserContext)
                {
                    case ExpressionState.LiteralContext:
                        isDivisionOperator = false;
                        break;
                    case ExpressionState.OperatorContext:
                        isDivisionOperator = true;
                        break;
                    default:
                        // If the parser context is unknown, the token before the slash is
                        // what determines whether the token is a divide operator or a
                        // regular expression literal.
                        isDivisionOperator =
                            this.lastSignificantToken is IdentifierToken ||
                            this.lastSignificantToken is LiteralToken ||
                            this.lastSignificantToken == PunctuatorToken.RightParenthesis ||
                            this.lastSignificantToken == PunctuatorToken.Increment ||
                            this.lastSignificantToken == PunctuatorToken.Decrement ||
                            this.lastSignificantToken == PunctuatorToken.RightBracket ||
                            this.lastSignificantToken == PunctuatorToken.RightBrace;
                        break;
                }

                if (isDivisionOperator)
                {
                    // Two division operators: "/" and "/=".
                    if (c2 == '=')
                    {
                        ReadNextChar();
                        return PunctuatorToken.CompoundDivide;
                    }
                    else
                        return PunctuatorToken.Divide;
                }
                else
                {
                    // Regular expression.
                    return ReadRegularExpression();
                }
            }
        }

        /// <summary>
        /// Reads a regular expression literal.
        /// </summary>
        /// <returns> A regular expression token. </returns>
        private Token ReadRegularExpression()
        {
            // The first slash has already been read.

            // Read the regular expression body.
            var body = new StringBuilder();
            bool insideCharacterClass = false;
            while (true)
            {
                // Read the next character.
                int c = ReadNextChar();

                // Check for special cases.
                if (c == '/' && insideCharacterClass == false)
                    break;
                else if (c == '\\')
                {
                    // Escape sequence.  Escaped characters are never special.
                    body.Append((char)c);
                    c = ReadNextChar();
                }
                else if (c == '[')
                    insideCharacterClass = true;
                else if (c == ']')
                    insideCharacterClass = false;
                
                // Note: a line terminator or EOF is not allowed in a regular expression, even if
                // it is escaped with a backslash.  Therefore, these checks have to come after the
                // checks above.
                if (IsLineTerminator(c))
                    throw new JavaScriptException(this.engine, "SyntaxError", "Unexpected line terminator in regular expression literal.", this.lineNumber, this.Source.Path);
                else if (c == -1)
                    throw new JavaScriptException(this.engine, "SyntaxError", "Unexpected end of input in regular expression literal.", this.lineNumber, this.Source.Path);

                // Append the character to the regular expression.
                body.Append((char)c);
            }

            // Read the flags.
            var flags = new StringBuilder();
            while (true)
            {
                int c = this.reader.Peek();
                if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z'))
                    break;
                ReadNextChar();
                flags.Append((char)c);
            }

            // Create a new literal token.
            return new LiteralToken(new RegularExpressionLiteral(body.ToString(), flags.ToString()));
        }

        /// <summary>
        /// Determines if the given character is whitespace.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is whitespace; <c>false</c> otherwise. </returns>
        private static bool IsWhiteSpace(int c)
        {
            return c == 0x09 || c == 0x0B || c == 0x0C || c == 0x20 || c == 0xA0 ||
                c == 0x1680 || c == 0x180E || (c >= 8192 && c <= 8202) || c == 0x202F ||
                c == 0x205F || c == 0x3000 || c == 0xFEFF;
        }

        /// <summary>
        /// Determines if the given character is a line terminator.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is a line terminator; <c>false</c> otherwise. </returns>
        private static bool IsLineTerminator(int c)
        {
            return c == 0x0A || c == 0x0D || c == 0x2028 || c == 0x2029;
        }

        /// <summary>
        /// Determines if the given character is valid as the first character of an identifier.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is is valid as the first character of an identifier;
        /// <c>false</c> otherwise. </returns>
        private static bool IsIdentifierStartChar(int c)
        {
            UnicodeCategory cat = char.GetUnicodeCategory((char)c);
            return c == '$' || c == '_' || c == '\\' ||
                cat == UnicodeCategory.UppercaseLetter ||
                cat == UnicodeCategory.LowercaseLetter ||
                cat == UnicodeCategory.TitlecaseLetter ||
                cat == UnicodeCategory.ModifierLetter ||
                cat == UnicodeCategory.OtherLetter ||
                cat == UnicodeCategory.LetterNumber;
        }

        /// <summary>
        /// Determines if the given character is valid as a character of an identifier.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is is valid as a character of an identifier;
        /// <c>false</c> otherwise. </returns>
        private static bool IsIdentifierChar(int c)
        {
            UnicodeCategory cat = char.GetUnicodeCategory((char)c);
            return c == '$' || c == '\\' ||
                cat == UnicodeCategory.UppercaseLetter ||
                cat == UnicodeCategory.LowercaseLetter ||
                cat == UnicodeCategory.TitlecaseLetter ||
                cat == UnicodeCategory.ModifierLetter ||
                cat == UnicodeCategory.OtherLetter ||
                cat == UnicodeCategory.LetterNumber ||
                cat == UnicodeCategory.NonSpacingMark ||
                cat == UnicodeCategory.SpacingCombiningMark ||
                cat == UnicodeCategory.DecimalDigitNumber ||
                cat == UnicodeCategory.ConnectorPunctuation ||
                c == 0x200C ||  // Zero-width non-joiner.
                c == 0x200D;    // Zero-width joiner.
        }

        /// <summary>
        /// Determines if the given character is valid as the first character of a punctuator.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is is valid as the first character of an punctuator;
        /// <c>false</c> otherwise. </returns>
        private static bool IsPunctuatorStartChar(int c)
        {
            return
                c == '{' || c == '}' || c == '(' || c == ')' || c == '[' || c == ']' || c == ';' ||
                c == ',' || c == '<' || c == '>' || c == '=' || c == '!' || c == '+' || c == '-' ||
                c == '*' || c == '%' || c == '&' || c == '|' || c == '^' || c == '~' || c == '?' ||
                c == ':';
        }

        /// <summary>
        /// Determines if the given character is valid as the first character of a numeric literal.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is is valid as the first character of a numeric
        /// literal; <c>false</c> otherwise. </returns>
        private bool IsNumericLiteralStartChar(int c)
        {
            return c == '.' || (c >= '0' && c <= '9');
        }

        /// <summary>
        /// Determines if the given character is valid as the first character of a string literal.
        /// </summary>
        /// <param name="c"> The character to test. </param>
        /// <returns> <c>true</c> if the character is is valid as the first character of a string
        /// literal; <c>false</c> otherwise. </returns>
        private bool IsStringLiteralStartChar(int c)
        {
            return c == '"' || c == '\'';
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

        /// <summary>
        /// Validates the given string is a valid identifier and returns the identifier name after
        /// escape sequences have been processed.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="str"> The string to resolve into an identifier. </param>
        /// <returns> The identifier name after escape sequences have been processed, or
        /// <c>null</c> if the string is not an identifier. </returns>
        internal static string ResolveIdentifier(ScriptEngine engine, string str)
        {
            var lexer = new Lexer(engine, new StringScriptSource(str));
            var argumentToken = lexer.NextToken();
            if ((argumentToken is IdentifierToken) == false || lexer.NextToken() != null)
                return null;
            return ((Compiler.IdentifierToken)argumentToken).Name;
        }
    }

}