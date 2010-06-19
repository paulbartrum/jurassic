//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Text;

///// <summary>
///// Converts a JavaScript source file into a series of tokens.
///// </summary>
//public class JSLexer
//{
//    private TextReader reader;
//    private int lineNumber;
//    private string sourcePath;

//    /// <summary>
//    /// Initialize the static dictionaries.
//    /// </summary>
//    static JSLexer()
//    {
//        BuildReservedWordsDictionary();
//        BuildPunctuatorsDictionary();
//    }

//    /// <summary>
//    /// Creates a JSLexer instance with the given source of text.
//    /// </summary>
//    /// <param name="reader"> A reader that will supply the javascript source code. </param>
//    /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
//    public JSLexer(TextReader reader, string sourcePath)
//    {
//        if (reader == null)
//            throw new ArgumentNullException("reader");
//        this.reader = reader;
//        this.lineNumber = 1;
//        this.sourcePath = sourcePath;
//        this.Read();    // Fill the buffer.
//    }
    
//    /// <summary>
//    /// Gets the reader that was supplied to the constructor.
//    /// </summary>
//    public TextReader Reader
//    {
//        get { return this.reader; }
//    }

//    // Needed to disambiguate regular expressions.
//    private Token lastSignificantToken;


//    private char[] buffer = new char[4096];
//    private int bufferPos, bufferLength;
//    private int peekChar;

//    private int Read()
//    {
//        int result = this.bufferLength == 0 ? -1 : this.buffer[this.bufferPos++];
//        if (this.bufferPos >= this.bufferLength)
//        {
//            this.bufferLength = this.reader.Read(buffer, 0, buffer.Length);
//            this.bufferPos = 0;
//        }
//        this.peekChar = this.bufferLength == 0 ? -1 : this.buffer[this.bufferPos];
//        return result;
//    }

//    private enum TokenFirstCharacterType
//    {
//        Eof,
//        Invalid,
//        Punctuator,
//        WhiteSpace,
//        IdentifierOrKeyword,
//        StringLiteral,
//        NumericLiteral,
//        LineTerminator,
//        DivideOrRegularExpression,
//    }

//    private static TokenFirstCharacterType[] firstCharacterTable = new TokenFirstCharacterType[]
//    {
//        TokenFirstCharacterType.Eof,                    // -1
//        TokenFirstCharacterType.Invalid,                // '\x00'
//        TokenFirstCharacterType.Invalid,                // '\x01'
//        TokenFirstCharacterType.Invalid,                // '\x02'
//        TokenFirstCharacterType.Invalid,                // '\x03'
//        TokenFirstCharacterType.Invalid,                // '\x04'
//        TokenFirstCharacterType.Invalid,                // '\x05'
//        TokenFirstCharacterType.Invalid,                // '\x06'
//        TokenFirstCharacterType.Invalid,                // '\x07'
//        TokenFirstCharacterType.Invalid,                // '\x08'
//        TokenFirstCharacterType.WhiteSpace,             // '\x09'
//        TokenFirstCharacterType.LineTerminator,         // '\x0a'
//        TokenFirstCharacterType.WhiteSpace,             // '\x0b'
//        TokenFirstCharacterType.WhiteSpace,             // '\x0c'
//        TokenFirstCharacterType.LineTerminator,         // '\x0d'
//        TokenFirstCharacterType.Invalid,                // '\x0e'
//        TokenFirstCharacterType.Invalid,                // '\x0f'
//        TokenFirstCharacterType.Invalid,                // '\x10'
//        TokenFirstCharacterType.Invalid,                // '\x11'
//        TokenFirstCharacterType.Invalid,                // '\x12'
//        TokenFirstCharacterType.Invalid,                // '\x13'
//        TokenFirstCharacterType.Invalid,                // '\x14'
//        TokenFirstCharacterType.Invalid,                // '\x15'
//        TokenFirstCharacterType.Invalid,                // '\x16'
//        TokenFirstCharacterType.Invalid,                // '\x17'
//        TokenFirstCharacterType.Invalid,                // '\x18'
//        TokenFirstCharacterType.Invalid,                // '\x19'
//        TokenFirstCharacterType.Invalid,                // '\x1a'
//        TokenFirstCharacterType.Invalid,                // '\x1b'
//        TokenFirstCharacterType.Invalid,                // '\x1c'
//        TokenFirstCharacterType.Invalid,                // '\x1d'
//        TokenFirstCharacterType.Invalid,                // '\x1e'
//        TokenFirstCharacterType.Invalid,                // '\x1f'
//        TokenFirstCharacterType.WhiteSpace,             // ' '
//        TokenFirstCharacterType.Punctuator,             // '!'
//        TokenFirstCharacterType.StringLiteral,          // '"'
//        TokenFirstCharacterType.Invalid,                // '#'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // '$'
//        TokenFirstCharacterType.Punctuator,             // '%'
//        TokenFirstCharacterType.Punctuator,             // '&'
//        TokenFirstCharacterType.StringLiteral,          // '\''
//        TokenFirstCharacterType.Punctuator,             // '('
//        TokenFirstCharacterType.Punctuator,             // ')'
//        TokenFirstCharacterType.Punctuator,             // '*'
//        TokenFirstCharacterType.Punctuator,             // '+'
//        TokenFirstCharacterType.Punctuator,             // ','
//        TokenFirstCharacterType.Punctuator,             // '-'
//        TokenFirstCharacterType.NumericLiteral,         // '.'
//        TokenFirstCharacterType.DivideOrRegularExpression, // '/'
//        TokenFirstCharacterType.NumericLiteral,         // '0'
//        TokenFirstCharacterType.NumericLiteral,         // '1'
//        TokenFirstCharacterType.NumericLiteral,         // '2'
//        TokenFirstCharacterType.NumericLiteral,         // '3'
//        TokenFirstCharacterType.NumericLiteral,         // '4'
//        TokenFirstCharacterType.NumericLiteral,         // '5'
//        TokenFirstCharacterType.NumericLiteral,         // '6'
//        TokenFirstCharacterType.NumericLiteral,         // '7'
//        TokenFirstCharacterType.NumericLiteral,         // '8'
//        TokenFirstCharacterType.NumericLiteral,         // '9'
//        TokenFirstCharacterType.Punctuator,             // ':'
//        TokenFirstCharacterType.Punctuator,             // ';'
//        TokenFirstCharacterType.Punctuator,             // '<'
//        TokenFirstCharacterType.Punctuator,             // '='
//        TokenFirstCharacterType.Punctuator,             // '>'
//        TokenFirstCharacterType.Punctuator,             // '?'
//        TokenFirstCharacterType.Invalid,                // '@'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'A'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'B'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'C'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'D'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'E'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'F'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'G'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'H'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'I'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'J'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'K'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'L'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'M'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'N'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'O'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'P'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'Q'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'R'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'S'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'T'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'U'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'V'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'W'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'X'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'Y'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'Z'
//        TokenFirstCharacterType.Punctuator,             // '['
//        TokenFirstCharacterType.IdentifierOrKeyword,    // '\'
//        TokenFirstCharacterType.Punctuator,             // ']'
//        TokenFirstCharacterType.Punctuator,             // '^'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // '_'
//        TokenFirstCharacterType.Invalid,                // '`'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'a'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'b'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'c'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'd'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'e'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'f'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'g'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'h'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'i'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'j'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'k'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'l'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'm'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'n'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'o'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'p'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'q'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'r'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 's'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 't'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'u'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'v'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'w'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'x'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'y'
//        TokenFirstCharacterType.IdentifierOrKeyword,    // 'z'
//        TokenFirstCharacterType.Punctuator,             // '{'
//        TokenFirstCharacterType.Punctuator,             // '|'
//        TokenFirstCharacterType.Punctuator,             // '}'
//        TokenFirstCharacterType.Punctuator,             // '~'
//    };

//    /// <summary>
//    /// Reads the next token from the reader.
//    /// </summary>
//    /// <returns> A token, or <c>null</c> if there are no more tokens. </returns>
//    public Token NextToken()
//    {
//        Token token = null;
//        do
//        {
//            int c1 = this.Read();
//            if (c1 < 127)
//            {
//                var tokenType = firstCharacterTable[c1 + 1];
//                switch (tokenType)
//                {
//                    case TokenFirstCharacterType.Eof:
//                        this.lastSignificantToken = null;
//                        return null;
//                    case TokenFirstCharacterType.Invalid:
//                        throw new JavaScriptException("SyntaxError", string.Format("Unexpected character '{0}'.", (char)c1), this.lineNumber, this.sourcePath);
//                    case TokenFirstCharacterType.Punctuator:
//                        token = ReadPunctuator(c1);
//                        break;
//                    case TokenFirstCharacterType.WhiteSpace:
//                        token = ReadWhiteSpace();
//                        break;
//                    case TokenFirstCharacterType.IdentifierOrKeyword:
//                        token = ReadIdentifier(c1);
//                        break;
//                    case TokenFirstCharacterType.StringLiteral:
//                        token = ReadStringLiteral(c1);
//                        break;
//                    case TokenFirstCharacterType.NumericLiteral:
//                        token = ReadNumericLiteral(c1);
//                        break;
//                    case TokenFirstCharacterType.LineTerminator:
//                        token = ReadLineTerminator(c1);
//                        break;
//                    case TokenFirstCharacterType.DivideOrRegularExpression:
//                        token = ReadDivideOrRegularExpression();
//                        break;
//                }
//                //System.Threading.Thread.Sleep(0);
//            }
//            else
//            {
//                if (IsPunctuatorStartChar(c1) == true)
//                {
//                    // Punctuator (puncuation + operators).
//                    token = ReadPunctuator(c1);
//                }
//                else if (IsWhiteSpace(c1) == true)
//                {
//                    // White space.
//                    token = ReadWhiteSpace();
//                }
//                else if (IsIdentifierStartChar(c1) == true)
//                {
//                    // Identifier or reserved word.
//                    token = ReadIdentifier(c1);
//                }
//                else if (IsStringLiteralStartChar(c1) == true)
//                {
//                    // String literal.
//                    token = ReadStringLiteral(c1);
//                }
//                else if (IsNumericLiteralStartChar(c1) == true)
//                {
//                    // Number literal.
//                    token = ReadNumericLiteral(c1);
//                }
//                else if (IsLineTerminator(c1) == true)
//                {
//                    // Line Terminator.
//                    token = ReadLineTerminator(c1);
//                }
//                else if (c1 == '/')
//                {
//                    token = ReadDivideOrRegularExpression();
//                }
//                else if (c1 == -1)
//                {
//                    // End of input.
//                    this.lastSignificantToken = null;
//                    return null;
//                }
//                else
//                    throw new JavaScriptException("SyntaxError", string.Format("Unexpected character '{0}'.", (char)c1), this.lineNumber, this.sourcePath);
//            }

//            // Record the last non-whitespace token.
//            if (token != null && (token is LineTerminatorToken) == false)
//            {
//                this.lastSignificantToken = token;
//            }

//        } while (token == null);

//        return token;
//    }

//    /// <summary>
//    /// Reads an identifier token.
//    /// </summary>
//    /// <param name="firstChar"> The first character of the identifier. </param>
//    /// <returns> An identifier token, or a literal token. </returns>
//    private Token ReadIdentifier(int firstChar)
//    {
//        // Read characters until we hit the first non-identifier character.
//        var name = new StringBuilder();
//        name.Append((char)firstChar);
//        while (true)
//        {
//            int c = this.peekChar;
//            if (IsIdentifierChar(c) == false || c == -1)
//                break;

//            // Add the character we peeked at to the identifier name.
//            name.Append((char)this.Read());
//        }

//        // Check if the identifier is actually a keyword, boolean literal, or null literal.
//        ReservedWordType reservedWordType = IsReservedWord(name.ToString());
//        switch (reservedWordType)
//        {
//            case JSLexer.ReservedWordType.None:
//                return new IdentifierToken(name.ToString());
//            case JSLexer.ReservedWordType.Keyword:
//                return new KeywordToken(name.ToString());
//            case JSLexer.ReservedWordType.BooleanLiteral:
//                return new LiteralToken(bool.Parse(name.ToString()));
//            case JSLexer.ReservedWordType.NullLiteral:
//                return new LiteralToken(null);
//            default:
//                throw new InvalidOperationException("Unsupported ReservedWordType.");
//        }
//    }

//    /// <summary>
//    /// Reads a punctuation token.
//    /// </summary>
//    /// <param name="firstChar"> The first character of the punctuation token. </param>
//    /// <returns> A punctuation token. </returns>
//    private Token ReadPunctuator(int firstChar)
//    {
//        // Read characters until we find a string that is not a punctuator.
//        var punctuator = new StringBuilder();
//        punctuator.Append((char)firstChar);
//        while (true)
//        {
//            int c = this.peekChar;
//            if (c == -1)
//                break;

//            punctuator.Append((char)c);
//            if (IsPunctuator(punctuator.ToString()) == false)
//            {
//                // Back off the last character.
//                punctuator.Remove(punctuator.Length - 1, 1);
//                break;
//            }

//            // Advance stream.
//            this.Read();
//        }
//        return new PunctuatorToken(punctuator.ToString());
//    }

//    /// <summary>
//    /// Reads a numeric literal token.
//    /// </summary>
//    /// <param name="firstChar"> The first character of the token. </param>
//    /// <returns> A numeric literal token. </returns>
//    private Token ReadNumericLiteral(int firstChar)
//    {
//        double result;

//        // If the number starts with '0x' or '0X' then the number should be parsed as a hex
//        // number.
//        if (firstChar == '0')
//        {
//            // Read the next char - should be 'x' or 'X' if this is a hex number (could be just '0').
//            int c = this.peekChar;
//            if (c == 'x' || c == 'X')
//            {
//                this.Read();

//                // Read numeric digits 0-9, a-z or A-Z.
//                result = 0;
//                while (true)
//                {
//                    c = this.peekChar;
//                    if (c >= '0' && c <= '9')
//                        result = result * 16 + c - '0';
//                    else if (c >= 'a' && c <= 'f')
//                        result = result * 16 + c - 'a' + 10;
//                    else if (c >= 'A' && c <= 'F')
//                        result = result * 16 + c - 'A' + 10;
//                    else
//                        break;
//                    this.Read();
//                }
//                return new LiteralToken(result);
//            }
//        }

//        // Read the integer component.
//        if (firstChar == '.')
//            result = double.NaN;
//        else
//            result = ReadInteger(firstChar - '0');

//        if (firstChar == '.' || this.peekChar == '.')
//        {
//            // Skip past the '.'.
//            if (firstChar != '.')
//                this.Read();

//            // Read the fractional component.
//            double fraction = ReadInteger();

//            // Check a number was actually provided.
//            if (double.IsNaN(result) == true && double.IsNaN(fraction) == true)
//                return new PunctuatorToken(".");

//            // '.5' should return 0.5.
//            if (double.IsNaN(result) == true)
//                result = 0;

//            // '5.' should return 5.0.
//            if (double.IsNaN(fraction) == false)
//            {
//                // Apply the fractional component.
//                result += fraction / System.Math.Pow(10, System.Math.Ceiling(System.Math.Log10(fraction + 1)));
//            }
//        }

//        if (this.peekChar == 'e' || this.peekChar == 'E')
//        {
//            // Skip past the 'e'.
//            Read();

//            // Read the sign of the exponent.
//            double exponentSign = 1.0;
//            int c = this.peekChar;
//            if (c == '+')
//                this.Read();
//            else if (c == '-')
//            {
//                this.Read();
//                exponentSign = -1.0;
//            }

//            // Read the exponent.
//            double exponent = ReadInteger() * exponentSign;

//            // Check a number was actually provided.
//            if (double.IsNaN(result) == true || double.IsNaN(exponent) == true)
//                throw new JavaScriptException("SyntaxError", "Invalid number.", this.lineNumber, this.sourcePath);

//            // Apply the exponent.
//            if (exponent >= 0)
//                result *= System.Math.Pow(10, exponent);
//            else
//                result /= System.Math.Pow(10, -exponent);
//        }

//        return new LiteralToken(result);
//    }

//    /// <summary>
//    /// Reads an integer value.
//    /// </summary>
//    /// <param name="initialValue"> The initial value, derived from the first character. </param>
//    /// <returns> The numeric value, or <c>double.NaN</c> if no number was present. </returns>
//    private double ReadInteger(double initialValue = double.NaN)
//    {
//        double result = initialValue;

//        while (true)
//        {
//            int c = this.peekChar;
//            if (c < '0' || c > '9')
//                break;
//            this.Read();
//            if (double.IsNaN(result))
//                result = c - '0';
//            else
//                result = result * 10 + (c - '0');
//        }

//        return result;
//    }

//    /// <summary>
//    /// Reads a string literal.
//    /// </summary>
//    /// <param name="firstChar"> The first character of the string literal. </param>
//    /// <returns> A string literal. </returns>
//    private Token ReadStringLiteral(int firstChar)
//    {
//        System.Diagnostics.Debug.Assert(firstChar == '\'' || firstChar == '"');
//        var contents = new StringBuilder();
//        while (true)
//        {
//            int c = this.Read();
//            if (c == firstChar)
//                break;
//            if (c == -1)
//                throw new JavaScriptException("SyntaxError", "Unexpected end of input in string literal.", this.lineNumber, this.sourcePath);
//            if (IsLineTerminator(c))
//                throw new JavaScriptException("SyntaxError", "Unexpected line terminator in string literal.", this.lineNumber, this.sourcePath);
//            if (c == '\\')
//            {
//                // Escape sequence or line continuation.
//                c = this.Read();
//                if (IsLineTerminator(c))
//                {
//                    // Line continuation.
//                    ReadLineTerminator(c);
//                }
//                else
//                {
//                    // Escape sequence.
//                    switch (c)
//                    {
//                        case 'b':
//                            // Backspace.
//                            contents.Append((char)0x08);
//                            break;
//                        case 'f':
//                            // Form feed.
//                            contents.Append((char)0x0C);
//                            break;
//                        case 'n':
//                            // Line feed.
//                            contents.Append((char)0x0A);
//                            break;
//                        case 'r':
//                            // Carriage return.
//                            contents.Append((char)0x0D);
//                            break;
//                        case 't':
//                            // Horizontal tab.
//                            contents.Append((char)0x09);
//                            break;
//                        case 'v':
//                            // Vertical tab.
//                            contents.Append((char)0x0B);
//                            break;
//                        case 'x':
//                            // ASCII escape.
//                            contents.Append(ReadHexEscape(2));
//                            break;
//                        case 'u':
//                            // Unicode escape.
//                            contents.Append(ReadHexEscape(4));
//                            break;
//                        case '0':
//                            // Zero.
//                            contents.Append((char)0);
//                            break;
//                        default:
//                            contents.Append((char)c);
//                            break;
//                    }
                    
//                }
//            }
//            else
//            {
//                contents.Append((char)c);
//            }
//        }
//        return new LiteralToken(contents.ToString());
//    }

//    /// <summary>
//    /// Reads a hexidecimal number with the given number of digits and turns it into a character.
//    /// </summary>
//    /// <returns> The character corresponding to the escape sequence, or the content that was read
//    /// from the input if a valid hex number was not read. </returns>
//    private char ReadHexEscape(int digitCount)
//    {
//        var contents = new StringBuilder(digitCount);
//        for (int i = 0; i < digitCount; i++)
//        {
//            int c = this.Read();
//            contents.Append((char)c);
//            if (IsHexDigit(c) == false)
//                throw new JavaScriptException("SyntaxError", string.Format("Invalid hex digit '{0}' in escape sequence.", (char)c), this.lineNumber, this.sourcePath);
//        }
//        return (char)int.Parse(contents.ToString(), System.Globalization.NumberStyles.HexNumber);
//    }

//    /// <summary>
//    /// Reads past a single line comment.
//    /// </summary>
//    /// <returns> Always returns <c>null</c>. </returns>
//    private Token ReadSingleLineComment()
//    {
//        // Read all the characters up to the newline.
//        // The newline is a seperate token.
//        while (true)
//        {
//            int c = this.peekChar;
//            if (IsLineTerminator(c) || c == -1)
//                break;
//            this.Read();
//        }

//        return null;
//    }

//    /// <summary>
//    /// Reads past a multi-line comment.
//    /// </summary>
//    /// <returns> A line terminator token if the multi-line comment contains a newline character;
//    /// otherwise returns <c>null</c>. </returns>
//    private Token ReadMultiLineComment()
//    {
//        // Multi-line comments that are actually on multiple lines are treated slighly
//        // differently from multi-line comments that only span a single line, with respect
//        // to implicit semi-colon insertion.
//        bool hasLineTerminator = false;

//        // Read the first character.
//        int c1 = this.Read();
//        if (c1 == -1)
//            throw new JavaScriptException("SyntaxError", "Unexpected end of input in multi-line comment.", this.lineNumber, this.sourcePath);

//        // Read all the characters up to the "*/".
//        while (true)
//        {
//            int c2 = this.Read();

//            if (IsLineTerminator(c1) == true)
//            {
//                hasLineTerminator = true;

//                // Increment the internal line number so errors can be tracked properly.
//                this.lineNumber ++;

//                // If the sequence is CRLF then only count that as one new line rather than two.
//                if (c1 == 0x0D && c2 == 0x0A)   // CRLF
//                    c1 = c2 = this.Read();
//            }

//            // Look for */ combination.
//            if (c1 == '*' && c2 == '/')
//                break;
//            c1 = c2;
//        }

//        return hasLineTerminator ? new LineTerminatorToken() : null;
//    }

//    /// <summary>
//    /// Reads past whitespace.
//    /// </summary>
//    /// <returns> Always returns <c>null</c>. </returns>
//    private Token ReadWhiteSpace()
//    {
//        // Read all the characters up to the next non-whitespace character.
//        while (true)
//        {
//            int c = this.peekChar;
//            if (IsWhiteSpace(c) == false || c == -1)
//                break;

//            // Advance the reader.
//            this.Read();
//        }
//        return null;
//    }

//    /// <summary>
//    /// Reads a line terminator (a newline).
//    /// </summary>
//    /// <param name="firstChar"> The first character of the line terminator. </param>
//    /// <returns> A newline token. </returns>
//    private Token ReadLineTerminator(int firstChar)
//    {
//        // Check for a CRLF sequence, if so that counts as one line terminator and not two.
//        int c = this.peekChar;
//        if (firstChar == 0x0D && c == 0x0A)   // CRLF
//            this.Read();

//        // Increment the internal line number so errors can be tracked properly.
//        this.lineNumber ++;

//        // Return a line terminator token.
//        return new LineTerminatorToken();
//    }

//    /// <summary>
//    /// Reads a divide operator ('/' or '/=') or a regular expression literal.
//    /// </summary>
//    /// <returns> A punctuator token or a regular expression token. </returns>
//    private Token ReadDivideOrRegularExpression()
//    {
//        // Comment or divide or regular expression.
//        int c2 = this.peekChar;
//        if (c2 == '*')
//        {
//            // Multi-line comment.

//            // Skip the asterisk.
//            this.Read();

//            return ReadMultiLineComment();
//        }
//        else if (c2 == '/')
//        {
//            // Single-line comment.

//            // Skip the slash.
//            this.Read();

//            return ReadSingleLineComment();
//        }
//        else
//        {
//            // Divide or regular expression.  The token before the slash is what determines
//            // whether the token is a divide operator or a regular expression literal.
//            if (this.lastSignificantToken is IdentifierToken ||
//                this.lastSignificantToken is LiteralToken ||
//                this.lastSignificantToken is RegExpToken ||
//                (this.lastSignificantToken is PunctuatorToken &&
//                (((PunctuatorToken)this.lastSignificantToken).Text == ")" ||
//                ((PunctuatorToken)this.lastSignificantToken).Text == "++" ||
//                ((PunctuatorToken)this.lastSignificantToken).Text == "--" ||
//                ((PunctuatorToken)this.lastSignificantToken).Text == "]" ||
//                ((PunctuatorToken)this.lastSignificantToken).Text == "}")) ||
//                (this.lastSignificantToken is KeywordToken && ((KeywordToken)this.lastSignificantToken).Text == "this"))
//            {
//                // Two divide operators: "/" and "/=".
//                if (c2 == '=')
//                    return new PunctuatorToken("/=");
//                else
//                    return new PunctuatorToken("/");
//            }
//            else
//            {
//                // Regular expression.
//                return ReadRegularExpression();
//            }
//        }
//    }

//    /// <summary>
//    /// Reads a regular expression literal.
//    /// </summary>
//    /// <returns> A regular expression token. </returns>
//    private Token ReadRegularExpression()
//    {
//        // The first slash has already been read.

//        // Read the regular expression body.
//        var body = new StringBuilder();
//        while (true)
//        {
//            int c = this.Read();
//            if (c == '/')
//                break;
//            if (c == -1)
//                throw new JavaScriptException("SyntaxError", "Unexpected end of input in regular expression literal.", this.lineNumber, this.sourcePath);
//            if (IsLineTerminator(c))
//                throw new JavaScriptException("SyntaxError", "Unexpected line terminator in regular expression literal.", this.lineNumber, this.sourcePath);
//            if (c == '\\')
//            {
//                // Escape sequence.
//                c = this.Read();
//            }
//            body.Append((char)c);
//        }

//        // Read the flags.
//        var flags = new StringBuilder();
//        while (true)
//        {
//            int c = this.peekChar;
//            if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z'))
//                break;
//            this.Read();
//            flags.Append((char)c);
//        }

//        return new RegExpToken(body.ToString(), flags.ToString());
//    }

//    /// <summary>
//    /// Determines if the given character is whitespace.
//    /// </summary>
//    /// <param name="c"> The character to test. </param>
//    /// <returns> <c>true</c> if the character is whitespace; <c>false</c> otherwise. </returns>
//    private static bool IsWhiteSpace(int c)
//    {
//        return c == 9 || c == 11 || c == 12 || c == 32 || c == 160 || c == 5760 || c == 6158 || (c >= 8192 && c <= 8202) || c == 8239 || c == 8287 || c == 12288 || c == 0xFEFF;
//    }

//    /// <summary>
//    /// Determines if the given character is a line terminator.
//    /// </summary>
//    /// <param name="c"> The character to test. </param>
//    /// <returns> <c>true</c> if the character is a line terminator; <c>false</c> otherwise. </returns>
//    private static bool IsLineTerminator(int c)
//    {
//        return c == 10 || c == 13 || c == 8232 || c == 8233;
//    }



//    private static string[] keywords = new string[]
//    {
//        "break", "case", "catch", "continue", "debugger", "default", "delete",
//        "do", "else", "finally", "for", "function", "if", "in",
//        "instanceof", "new", "return", "switch", "this", "throw", "try",
//        "typeof", "var", "void", "while", "with"
//    };

//    private static string[] futureReservedWords = new string[]
//    {
//        "class", "const", "enum", "export", "extends", "import", "super"
//    };

//    private static string[] strictFutureReservedWords = new string[]
//    {
//        "implements", "interface", "let", "package", "private", "protected", "public", "static", "yield"
//    };

//    private static string[] booleanLiteralWords = new string[] { "true", "false" };

//    private static string[] nullLiteralWords = new string[] { "null" };

//    private enum ReservedWordType
//    {
//        None,
//        Keyword,
//        FutureReservedWord,
//        BooleanLiteral,
//        NullLiteral
//    }

//    private static Dictionary<string, ReservedWordType> reservedWordsDictionary;

//    /// <summary>
//    /// Builds up a list of reserved words.
//    /// </summary>
//    private static void BuildReservedWordsDictionary()
//    {
//        reservedWordsDictionary = new Dictionary<string, ReservedWordType>();
//        foreach (string keyword in keywords)
//            reservedWordsDictionary.Add(keyword, ReservedWordType.Keyword);
//        foreach (string futureReservedWord in futureReservedWords)
//            reservedWordsDictionary.Add(futureReservedWord, ReservedWordType.FutureReservedWord);
//        foreach (string booleanLiteralWord in booleanLiteralWords)
//            reservedWordsDictionary.Add(booleanLiteralWord, ReservedWordType.BooleanLiteral);
//        foreach (string nullLiteralWord in nullLiteralWords)
//            reservedWordsDictionary.Add(nullLiteralWord, ReservedWordType.NullLiteral);
//    }

//    /// <summary>
//    /// Determines if the given string is a reserved word.
//    /// </summary>
//    /// <param name="str"> The string to test. </param>
//    /// <returns> <c>true</c> if the string is a reserved word; <c>false</c> otherwise. </returns>
//    private static ReservedWordType IsReservedWord(string str)
//    {
//        ReservedWordType result = default(ReservedWordType);
//        if (reservedWordsDictionary.TryGetValue(str, out result) == false) return ReservedWordType.None;
//        return result;
//    }

//    /// <summary>
//    /// Determines if the given character is valid as the first character of an identifier.
//    /// </summary>
//    /// <param name="c"> The character to test. </param>
//    /// <returns> <c>true</c> if the character is is valid as the first character of an identifier;
//    /// <c>false</c> otherwise. </returns>
//    public static bool IsIdentifierStartChar(int c)
//    {
//        UnicodeCategory cat = char.GetUnicodeCategory((char)c);
//        return c == '$' || c == '_' || c == '\\' ||
//            cat == UnicodeCategory.UppercaseLetter ||
//            cat == UnicodeCategory.LowercaseLetter ||
//            cat == UnicodeCategory.TitlecaseLetter ||
//            cat == UnicodeCategory.ModifierLetter ||
//            cat == UnicodeCategory.OtherLetter ||
//            cat == UnicodeCategory.LetterNumber;
//    }

//    /// <summary>
//    /// Determines if the given character is valid as a character of an identifier.
//    /// </summary>
//    /// <param name="c"> The character to test. </param>
//    /// <returns> <c>true</c> if the character is is valid as a character of an identifier;
//    /// <c>false</c> otherwise. </returns>
//    public static bool IsIdentifierChar(int c)
//    {
//        UnicodeCategory cat = char.GetUnicodeCategory((char)c);
//        return c == '$' || c == '\\' ||
//            cat == UnicodeCategory.UppercaseLetter ||
//            cat == UnicodeCategory.LowercaseLetter ||
//            cat == UnicodeCategory.TitlecaseLetter ||
//            cat == UnicodeCategory.ModifierLetter ||
//            cat == UnicodeCategory.OtherLetter ||
//            cat == UnicodeCategory.LetterNumber ||
//            cat == UnicodeCategory.NonSpacingMark ||
//            cat == UnicodeCategory.SpacingCombiningMark ||
//            cat == UnicodeCategory.DecimalDigitNumber ||
//            cat == UnicodeCategory.ConnectorPunctuation ||
//            c == 0x200C ||  // Zero-width non-joiner.
//            c == 0x200D;    // Zero-width joiner.
//    }


//    private static string[] m_punctuators = new string[] {
//        "{", "}", "(", ")", "[", "]", ";", ",", "<", 
//        ">", "<=", ">=", "==", "!=", "===", "!==", "+", "-", "*", 
//        "%", "++", "--", "<<", ">>", ">>>", "&", "|", "^", "!", 
//        "~", "&&", "||", "?", ":", "=", "+=", "-=", "*=", "%=", 
//        "<<=", ">>=", ">>>=", "&=", "|=", "^="
//        // ".", "/", "/=" are handled specially.
//    };

//    private static Dictionary<string, bool> m_punctuatorsDictionary;

//    /// <summary>
//    /// Builds up a list of valid punctuation.
//    /// </summary>
//    private static void BuildPunctuatorsDictionary()
//    {
//        m_punctuatorsDictionary = new Dictionary<string, bool>();
//        foreach (string punctuator in m_punctuators)
//        {
//            m_punctuatorsDictionary.Add(punctuator, true);
//        }
//    }

//    /// <summary>
//    /// Determines if the given character is valid as the first character of a punctuator.
//    /// </summary>
//    /// <param name="c"> The character to test. </param>
//    /// <returns> <c>true</c> if the character is is valid as the first character of an punctuator;
//    /// <c>false</c> otherwise. </returns>
//    private static bool IsPunctuatorStartChar(int c)
//    {
//        return
//            c == '{' || c == '}' || c == '(' || c == ')' || c == '[' || c == ']' || c == ';' ||
//            c == ',' || c == '<' || c == '>' || c == '=' || c == '!' || c == '+' || c == '-' ||
//            c == '*' || c == '%' || c == '&' || c == '|' || c == '^' || c == '~' || c == '?' ||
//            c == ':';
//    }

//    /// <summary>
//    /// Determines if the given string is a punctuator.
//    /// </summary>
//    /// <param name="str"> The string to test. </param>
//    /// <returns> <c>true</c> if the string is is a punctuator; <c>false</c> otherwise. </returns>
//    private static bool IsPunctuator(string str)
//    {
//        return m_punctuatorsDictionary.ContainsKey(str);
//    }

//    /// <summary>
//    /// Determines if the given character is valid as the first character of a numeric literal.
//    /// </summary>
//    /// <param name="c"> The character to test. </param>
//    /// <returns> <c>true</c> if the character is is valid as the first character of a numeric
//    /// literal; <c>false</c> otherwise. </returns>
//    public bool IsNumericLiteralStartChar(int c)
//    {
//        return c == '.' || (c >= '0' && c <= '9');
//    }

//    /// <summary>
//    /// Determines if the given character is valid as the first character of a string literal.
//    /// </summary>
//    /// <param name="c"> The character to test. </param>
//    /// <returns> <c>true</c> if the character is is valid as the first character of a string
//    /// literal; <c>false</c> otherwise. </returns>
//    public bool IsStringLiteralStartChar(int c)
//    {
//        return c == '"' || c == '\'';
//    }

//    /// <summary>
//    /// Determines if the given character is valid in a hexidecimal number.
//    /// </summary>
//    /// <param name="c"> The character to test. </param>
//    /// <returns> <c>true</c> if the given character is valid in a hexidecimal number; <c>false</c>
//    /// otherwise. </returns>
//    private static bool IsHexDigit(int c)
//    {
//        return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
//    }
//}