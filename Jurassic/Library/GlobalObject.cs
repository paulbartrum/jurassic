using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents functions and properties within the global scope.
    /// </summary>
    public class GlobalObject : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Global object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal GlobalObject(ObjectInstance prototype)
            : base(prototype)
        {
            // Add the global constants.
            // Infinity, NaN and undefined are read-only in ECMAScript 5.
            this.FastSetProperty("Infinity", double.PositiveInfinity, PropertyAttributes.Sealed);
            this.FastSetProperty("NaN", double.NaN, PropertyAttributes.Sealed);
            this.FastSetProperty("undefined", Undefined.Value, PropertyAttributes.Sealed);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "Global"; }
        }





        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________


        /// <summary>
        /// Decodes a string that was encoded with the encodeURI function.
        /// </summary>
        /// <param name="input"> The associated script engine. </param>
        /// <returns> The string, as it was before encoding. </returns>
        [JSFunction(Name = "decodeURI", Flags = FunctionBinderFlags.HasEngineParameter)]
        public static string DecodeURI(ScriptEngine engine, string input)
        {
            return Decode(engine, input, ";/?:@&=+$,#");
        }

        /// <summary>
        /// Decodes a string that was encoded with the decodeURIComponent function.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="input"> The string to decode. </param>
        /// <returns> The string, as it was before encoding. </returns>
        [JSFunction(Name = "decodeURIComponent", Flags = FunctionBinderFlags.HasEngineParameter)]
        public static string DecodeURIComponent(ScriptEngine engine, string input)
        {
            return Decode(engine, input, "");
        }

        /// <summary>
        /// Encodes a string containing a Uniform Resource Identifier (URI).
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="input"> The string to encode. </param>
        /// <returns> A copy of the given URI with the special characters encoded. </returns>
        [JSFunction(Name = "encodeURI", Flags = FunctionBinderFlags.HasEngineParameter)]
        public static string EncodeURI(ScriptEngine engine, string input)
        {
            return Encode(engine, input, ";/?:@&=+$,-_.!~*'()#");
        }

        /// <summary>
        /// Encodes a string containing a portion of a Uniform Resource Identifier (URI).
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="input"> The string to encode. </param>
        /// <returns> A copy of the given URI with the special characters encoded. </returns>
        [JSFunction(Name = "encodeURIComponent", Flags = FunctionBinderFlags.HasEngineParameter)]
        public static string EncodeURIComponent(ScriptEngine engine, string input)
        {
            return Encode(engine, input, "-_.!~*'()");
        }

        /// <summary>
        /// Encodes a string using an encoding similar to that used in URLs.
        /// </summary>
        /// <param name="input"> The string to encode. </param>
        /// <returns> A copy of the given string with the special characters encoded. </returns>
        [JSFunction(Deprecated = true, Name = "escape")]
        public static string Escape(string input)
        {
            var result = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') ||
                    c == '@' || c == '*' || c == '_' || c == '+' || c == '-' || c == '.' || c == '/')
                    result.Append(c);
                else if (c < 256)
                    result.AppendFormat("%{0:X2}", (int)c);
                else
                    result.AppendFormat("%u{0:X4}", (int)c);
            }
            return result.ToString();
        }

        /// <summary>
        /// Evaluates the given javascript source code and returns the result.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="code"> The source code to evaluate. </param>
        /// <returns> The value of the last statement that was executed, or <c>undefined</c> if
        /// there were no executed statements. </returns>
        [JSFunction(Name = "eval", Flags = FunctionBinderFlags.HasEngineParameter)]
        public static object Eval(ScriptEngine engine, object code)
        {
            if (TypeUtilities.IsString(code) == false)
                return code;
            var evalGen = new Jurassic.Compiler.EvalMethodGenerator(
                engine,                                                 // The script engine.
                engine.CreateGlobalScope(),                             // The scope to run the code in.
                new StringScriptSource(TypeConverter.ToString(code)),   // The source code to execute.
                new Compiler.CompilerOptions(),                         // Options.
                engine.Global);                                         // The value of the "this" keyword.
            return evalGen.Execute();
        }

        /// <summary>
        /// Evaluates the given javascript source code and returns the result.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="code"> The source code to evaluate. </param>
        /// <param name="scope"> The containing scope. </param>
        /// <param name="thisObject"> The value of the "this" keyword in the containing scope. </param>
        /// <param name="strictMode"> Indicates whether the eval statement is being called from
        /// strict mode code. </param>
        /// <returns> The value of the last statement that was executed, or <c>undefined</c> if
        /// there were no executed statements. </returns>
        public static object Eval(ScriptEngine engine, object code, Compiler.Scope scope, object thisObject, bool strictMode)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (TypeUtilities.IsString(code) == false)
                return code;

            var options = new Compiler.CompilerOptions() { ForceStrictMode = strictMode };
            var evalGen = new Jurassic.Compiler.EvalMethodGenerator(
                engine,                                                 // The script engine.
                scope,                                                  // The scope to run the code in.
                new StringScriptSource(TypeConverter.ToString(code)),   // The source code to execute.
                options,                                                // Options.
                thisObject);                                            // The value of the "this" keyword.
            return evalGen.Execute();
        }

        /// <summary>
        /// Determines whether the given number is finite.
        /// </summary>
        /// <param name="value"> The number to test. </param>
        /// <returns> <c>false</c> if the number is NaN or positive or negative infinity,
        /// <c>true</c> otherwise. </returns>
        [JSFunction(Name = "isFinite")]
        public static bool IsFinite(double value)
        {
            return double.IsNaN(value) == false && double.IsInfinity(value) == false;
        }

        /// <summary>
        /// Determines whether the given number is NaN.
        /// </summary>
        /// <param name="value"> The number to test. </param>
        /// <returns> <c>true</c> if the number is NaN, <c>false</c> otherwise. </returns>
        [JSFunction(Name = "isNaN")]
        public static bool IsNaN(double value)
        {
            return double.IsNaN(value);
        }

        /// <summary>
        /// Parses the given string and returns the equivalent numeric value. 
        /// </summary>
        /// <param name="input"> The string to parse. </param>
        /// <returns> The equivalent numeric value of the given string. </returns>
        /// <remarks> Leading whitespace is ignored.  Parsing continues until the first invalid
        /// character, at which point parsing stops.  No error is returned in this case. </remarks>
        [JSFunction(Name = "parseFloat")]
        public static double ParseFloat(string input)
        {
            return NumberParser.ParseFloat(input);
        }

        /// <summary>
        /// Parses the given string and returns the equivalent integer value. 
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="input"> The string to parse. </param>
        /// <param name="dblRadix"> The numeric base to use for parsing.  The default is to use base 10
        /// except when the input string starts with '0x' or '0X' in which case base 16 is used
        /// instead. </param>
        /// <returns> The equivalent integer value of the given string. </returns>
        /// <remarks> Leading whitespace is ignored.  Parsing continues until the first invalid
        /// character, at which point parsing stops.  No error is returned in this case. </remarks>
        [JSFunction(Name = "parseInt", Flags = FunctionBinderFlags.HasEngineParameter)]
        public static double ParseInt(ScriptEngine engine, string input, double dblRadix = 0)
        {
            // Check for a valid radix.
            // Note: this is the only function that uses TypeConverter.ToInt32() for parameter
            // conversion (as opposed to the normal method which is TypeConverter.ToInteger() so
            // the radix parameter must be converted to an integer in code.
            int radix = TypeConverter.ToInt32(dblRadix);
            if (radix < 0 || radix == 1 || radix > 36)
                return double.NaN;

            var reader = new System.IO.StringReader(input);
            
            // Skip whitespace and line terminators.
            while (IsWhiteSpaceOrLineTerminator(reader.Peek()))
                reader.Read();

            // Determine the sign.
            double sign = 1;
            if (reader.Peek() == '+')
            {
                reader.Read();
            }
            else if (reader.Peek() == '-')
            {
                sign = -1;
                reader.Read();
            }

            // Hex prefix should be stripped if the radix is 0, undefined or 16.
            bool stripPrefix = radix == 0 || radix == 16;

            // Default radix is 10.
            if (radix == 0)
                radix = 10;

            // If the input is empty, then return NaN.
            double result = double.NaN;

            // Skip past the prefix, if there is one.
            if (stripPrefix == true)
            {
                if (reader.Peek() == '0')
                {
                    reader.Read();
                    result = 0;     // Note: required for parsing "0z11" correctly (when radix = 0).

                    int c = reader.Peek();
                    if (c == 'x' || c == 'X')
                    {
                        // Hex number.
                        reader.Read();
                        radix = 16;
                    }

                    if (c >= '0' && c <= '9' && engine.CompatibilityMode == CompatibilityMode.ECMAScript3)
                    {
                        // Octal number.
                        radix = 8;
                    }
                }
            }

            // Read numeric digits 0-9, a-z or A-Z.
            while (true)
            {
                int numericValue = -1;
                int c = reader.Read();
                if (c >= '0' && c <= '9')
                    numericValue = c - '0';
                if (c >= 'a' && c <= 'z')
                    numericValue = c - 'a' + 10;
                if (c >= 'A' && c <= 'Z')
                    numericValue = c - 'A' + 10;
                if (numericValue == -1 || numericValue >= radix)
                    break;
                if (double.IsNaN(result))
                    result = numericValue;
                else
                    result = result * radix + numericValue;
            }

            return result * sign;
        }

        /// <summary>
        /// Decodes a string that has been encoded using escape().
        /// </summary>
        /// <param name="input"> The string to decode. </param>
        /// <returns> A copy of the given string with the escape sequences decoded. </returns>
        [JSFunction(Deprecated = true, Name = "unescape")]
        public static string Unescape(string input)
        {
            var result = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '%')
                {
                    // Make sure the string is long enough.
                    if (i == input.Length - 1)
                        break;

                    if (input[i + 1] == 'u')
                    {
                        // 4 digit escape sequence %uXXXX.
                        // Make sure the string is long enough and has valid hex digits.
                        if (i >= input.Length - 5 ||
                            IsHexDigit(input[i + 2]) == false || IsHexDigit(input[i + 3]) == false ||
                            IsHexDigit(input[i + 4]) == false || IsHexDigit(input[i + 5]) == false)
                        {
                            result.Append('%');
                            continue;
                        }
                        result.Append((char)int.Parse(input.Substring(i + 2, 4), System.Globalization.NumberStyles.HexNumber));
                        i += 5;
                    }
                    else
                    {
                        // 2 digit escape sequence %XX.
                        // Make sure the string is long enough and has valid hex digits.
                        if (i >= input.Length - 2 ||
                            IsHexDigit(input[i + 1]) == false || IsHexDigit(input[i + 2]) == false)
                        {
                            result.Append('%');
                            continue;
                        }
                        result.Append((char)int.Parse(input.Substring(i + 1, 2), System.Globalization.NumberStyles.HexNumber));
                        i += 2;
                    }
                }
                else
                    result.Append(c);
            }
            return result.ToString();
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Decodes a string containing a URI or a portion of a URI.
        /// </summary>
        /// <param name="engine"> The script engine used to create the error objects. </param>
        /// <param name="input"> The string to decode. </param>
        /// <param name="unescapedSet"> A string containing the set of characters that should not
        /// be escaped.  Alphanumeric characters should not be included. </param>
        /// <returns> A copy of the given string with the escape sequences decoded. </returns>
        private static string Decode(ScriptEngine engine, string input, string reservedSet)
        {
            var result = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '%')
                {
                    // 2 digit escape sequence %XX.

                    // Make sure the string is long enough and the next two digits are valid hex digits.
                    if (i >= input.Length - 2 || IsHexDigit(input[i + 1]) == false || IsHexDigit(input[i + 2]) == false)
                        throw new JavaScriptException(engine, "URIError", "URI malformed");

                    // Decode the %XX encoding.
                    int utf8Byte = int.Parse(input.Substring(i + 1, 2), System.Globalization.NumberStyles.HexNumber);
                    i += 2;

                    // If the high bit is not set, then this is a single byte ASCII character.
                    if ((utf8Byte & 0x80) == 0)
                    {
                        // Decode only if the character is not reserved.
                        if (reservedSet.Contains((char)utf8Byte))
                        {
                            // Leave the escape sequence as is.
                            result.Append(input.Substring(i - 2, 3));
                        }
                        else
                        {
                            result.Append((char)utf8Byte);
                        }
                    }
                    else
                    {
                        // Otherwise, this character was encoded to multiple bytes.

                        // Check for an invalid UTF-8 start value.
                        if (utf8Byte == 0xc0 || utf8Byte == 0xc1)
                            throw new JavaScriptException(engine, "URIError", "URI malformed");

                        // Count the number of high bits set (this is the number of bytes required for the character).
                        int utf8ByteCount = 1;
                        for (int j = 6; j >= 0; j--)
                        {
                            if ((utf8Byte & (1 << j)) != 0)
                                utf8ByteCount++;
                            else
                                break;
                        }
                        if (utf8ByteCount < 2 || utf8ByteCount > 4)
                            throw new JavaScriptException(engine, "URIError", "URI malformed");

                        // Read the additional bytes.
                        byte[] utf8Bytes = new byte[utf8ByteCount];
                        utf8Bytes[0] = (byte)utf8Byte;
                        for (int j = 1; j < utf8ByteCount; j++)
                        {
                            // An additional escape sequence is expected.
                            if (i >= input.Length - 1 || input[++i] != '%')
                                throw new JavaScriptException(engine, "URIError", "URI malformed");

                            // Make sure the string is long enough and the next two digits are valid hex digits.
                            if (i >= input.Length - 2 || IsHexDigit(input[i + 1]) == false || IsHexDigit(input[i + 2]) == false)
                                throw new JavaScriptException(engine, "URIError", "URI malformed");

                            // Decode the %XX encoding.
                            utf8Byte = int.Parse(input.Substring(i + 1, 2), System.Globalization.NumberStyles.HexNumber);

                            // Top two bits must be 10 (i.e. byte must be 10XXXXXX in binary).
                            if ((utf8Byte & 0xC0) != 0x80)
                                throw new JavaScriptException(engine, "URIError", "URI malformed");

                            // Store the byte.
                            utf8Bytes[j] = (byte)utf8Byte;

                            // Update the character position.
                            i += 2;
                        }

                        // Decode the UTF-8 sequence.
                        result.Append(System.Text.Encoding.UTF8.GetString(utf8Bytes));
                    }
                }
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// Encodes a string containing a URI or a portion of a URI.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="input"> The string to encode. </param>
        /// <param name="unescapedSet"> A string containing the set of characters that should not
        /// be escaped.  Alphanumeric characters should not be included. </param>
        /// <returns> A copy of the given URI with the special characters encoded. </returns>
        private static string Encode(ScriptEngine engine, string input, string unescapedSet)
        {
            var result = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                // Get the next UTF-32 code point.  This detects invalid surrogate pairs.
                int c;
                try
                {
                    c = char.ConvertToUtf32(input, i);
                }
                catch (ArgumentException)
                {
                    throw new JavaScriptException(engine, "URIError", "URI malformed");
                }

                // Detect if the code point is a surrogate pair.
                bool isSurrogatePair = c >= 0x10000;

                if (isSurrogatePair == false && ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                    (c >= '0' && c <= '9') || unescapedSet.Contains((char)c)))
                {
                    // Character should not be escaped.
                    result.Append((char)c);
                }
                else
                {
                    // Character should be escaped.
                    byte[] utf8Bytes = new byte[4];
                    int utf8ByteCount = System.Text.Encoding.UTF8.GetBytes(input, i, isSurrogatePair ? 2 : 1, utf8Bytes, 0);
                    for (int j = 0; j < utf8ByteCount; j++)
                        result.AppendFormat("%{0:X2}", utf8Bytes[j]);
                }

                // Surrogate pairs need to advance an extra character position.
                if (isSurrogatePair == true)
                    i++;
            }
            return result.ToString();
        }

        /// <summary>
        /// Reads an integer value using the given reader.
        /// </summary>
        /// <param name="reader"> The reader to read characters from. </param>
        /// <param name="digitsRead"> Upon returning, contains the number of digits that were read. </param>
        /// <returns> The numeric value, or <c>double.NaN</c> if no number was present. </returns>
        private static double ReadInteger(System.IO.StringReader reader, out int digitsRead)
        {
            // If the input is empty, then return NaN.
            double result = double.NaN;
            digitsRead = 0;

            while (true)
            {
                int c = reader.Peek();
                if (c < '0' || c > '9')
                    break;
                reader.Read();
                digitsRead++;
                if (double.IsNaN(result))
                    result = c - '0';
                else
                    result = result * 10 + (c - '0');
            }

            return result;
        }

        /// <summary>
        /// Determines if the given character is whitespace or a line terminator.
        /// </summary>
        /// <param name="c"> The unicode code point for the character. </param>
        /// <returns> <c>true</c> if the character is whitespace or a line terminator; <c>false</c>
        /// otherwise. </returns>
        private static bool IsWhiteSpaceOrLineTerminator(int c)
        {
            return c == 9 || c == 0x0b || c == 0x0c || c == ' ' || c == 0xa0 || c == 0xfeff ||
                c == 0x1680 || c == 0x180e || (c >= 0x2000 && c <= 0x200a) || c == 0x202f || c == 0x205f || c == 0x3000 ||
                c == 0x0a || c == 0x0d || c == 0x2028 || c == 0x2029;
        }

        /// <summary>
        /// Determines if the given character is a hexidecimal digit (0-9, a-z, A-Z).
        /// </summary>
        /// <param name="c"> The unicode code point for the character. </param>
        /// <returns> <c>true</c> if the character is a hexidecimal digit; <c>false</c> otherwise. </returns>
        private static bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
        }
    }
}
