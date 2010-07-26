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
        // There is exactly one global instance per thread.
        [ThreadStatic]
        private static GlobalObject instance;

        // The built-in objects.
        private ArrayConstructor arrayConstructor;
        private BooleanConstructor booleanConstructor;
        private DateConstructor dateConstructor;
        private FunctionConstructor functionConstructor;
        private JSONObject jsonObject;
        private MathObject mathObject;
        private NumberConstructor numberConstructor;
        private ObjectConstructor objectConstructor;
        private RegExpConstructor regExpConstructor;
        private StringConstructor stringConstructor;

        // The built-in error objects.
        private ErrorConstructor errorConstructor;
        private ErrorConstructor rangeErrorConstructor;
        private ErrorConstructor typeErrorConstructor;
        private ErrorConstructor syntaxErrorConstructor;
        private ErrorConstructor uriErrorConstructor;
        private ErrorConstructor evalErrorConstructor;
        private ErrorConstructor referenceErrorConstructor;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Global object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        private GlobalObject(ObjectInstance prototype)
            : base(prototype)
        {
            // Add the global constants.
            this.FastSetProperty("Infinity", double.PositiveInfinity, PropertyAttributes.Sealed);
            this.FastSetProperty("NaN", double.NaN, PropertyAttributes.Sealed);
            this.FastSetProperty("undefined", Undefined.Value, PropertyAttributes.Sealed);
        }

        /// <summary>
        /// Creates the globally accessible objects.
        /// </summary>
        private void InitializeObjects()
        {
            // NOTE: separating this method from the constructor prevents a stack overflow.
            var baseObject = this.Prototype;

            // Create the function object that second to last in the prototype chain.
            var baseFunction = UserDefinedFunction.CreateEmptyFunction(baseObject);

            // Object must be created first, then function.
            this.objectConstructor = new ObjectConstructor(baseFunction, baseObject);
            this.functionConstructor = new FunctionConstructor(baseFunction, baseFunction);

            // Create all the built-in objects.
            this.mathObject = new MathObject(baseObject);
            this.jsonObject = new JSONObject(baseObject);

            // Create all the built-in functions.
            this.arrayConstructor = new ArrayConstructor(baseFunction);
            this.booleanConstructor = new BooleanConstructor(baseFunction);
            this.dateConstructor = new DateConstructor(baseFunction);
            this.numberConstructor = new NumberConstructor(baseFunction);
            this.regExpConstructor = new RegExpConstructor(baseFunction);
            this.stringConstructor = new StringConstructor(baseFunction);

            // Create the error functions.
            this.errorConstructor = new ErrorConstructor(baseFunction, "Error");
            this.rangeErrorConstructor = new ErrorConstructor(baseFunction, "RangeError");
            this.typeErrorConstructor = new ErrorConstructor(baseFunction, "TypeError");
            this.syntaxErrorConstructor = new ErrorConstructor(baseFunction, "SyntaxError");
            this.uriErrorConstructor = new ErrorConstructor(baseFunction, "URIError");
            this.evalErrorConstructor = new ErrorConstructor(baseFunction, "EvalError");
            this.referenceErrorConstructor = new ErrorConstructor(baseFunction, "ReferenceError");

            // Populate the instance prototypes (TODO: optimize this, currently takes about 15ms).
            this.PopulateFunctions();
            this.objectConstructor.PopulateFunctions();
            this.objectConstructor.InstancePrototype.PopulateFunctions();
            this.functionConstructor.InstancePrototype.PopulateFunctions(typeof(FunctionInstance));
            this.mathObject.PopulateFunctions();
            this.mathObject.PopulateFields();
            this.jsonObject.PopulateFunctions();
            this.arrayConstructor.InstancePrototype.PopulateFunctions();
            this.booleanConstructor.InstancePrototype.PopulateFunctions();
            this.dateConstructor.PopulateFunctions();
            this.dateConstructor.InstancePrototype.PopulateFunctions();
            this.numberConstructor.InstancePrototype.PopulateFunctions();
            this.numberConstructor.PopulateFields();
            this.regExpConstructor.InstancePrototype.PopulateFunctions();
            this.stringConstructor.PopulateFunctions();
            this.stringConstructor.InstancePrototype.PopulateFunctions();
            this.errorConstructor.InstancePrototype.PopulateFunctions();

            // Add them as JavaScript-accessible properties of the global instance.
            this["Array"] = instance.arrayConstructor;
            this["Boolean"] = instance.booleanConstructor;
            this["Date"] = instance.dateConstructor;
            this["Function"] = instance.functionConstructor;
            this["JSON"] = instance.jsonObject;
            this["Math"] = instance.mathObject;
            this["Number"] = instance.numberConstructor;
            this["Object"] = instance.objectConstructor;
            this["RegExp"] = instance.regExpConstructor;
            this["String"] = instance.stringConstructor;

            // And the errors.
            this["Error"] = instance.errorConstructor;
            this["RangeError"] = instance.rangeErrorConstructor;
            this["TypeError"] = instance.typeErrorConstructor;
            this["SyntaxError"] = instance.syntaxErrorConstructor;
            this["URIError"] = instance.uriErrorConstructor;
            this["EvalError"] = instance.evalErrorConstructor;
            this["ReferenceError"] = instance.referenceErrorConstructor;
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

        /// <summary>
        /// Gets the instance that is used when no other limiting scope is present.
        /// </summary>
        public static GlobalObject Instance
        {
            get
            {
                if (instance == null)
                {
                    // Create the base of the prototype chain.
                    var baseObject = ObjectInstance.CreateRootObject();

                    // Create the global object.
                    instance = new GlobalObject(baseObject);

                    // Create the rest of the objects.
                    instance.InitializeObjects();
                }
                return instance;
            }
        }

        /// <summary>
        /// Gets the globally accessible Array object.
        /// </summary>
        public static ArrayConstructor Array
        {
            get { return Instance.arrayConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible Boolean object.
        /// </summary>
        public static BooleanConstructor Boolean
        {
            get { return Instance.booleanConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible Date object.
        /// </summary>
        public static DateConstructor Date
        {
            get { return Instance.dateConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible Function object.
        /// </summary>
        public static FunctionConstructor Function
        {
            get { return Instance.functionConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible Math object.
        /// </summary>
        public static MathObject Math
        {
            get { return Instance.mathObject; }
        }

        /// <summary>
        /// Gets the globally accessible Number object.
        /// </summary>
        public static NumberConstructor Number
        {
            get { return Instance.numberConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible Object object.
        /// </summary>
        public static ObjectConstructor Object
        {
            get { return Instance.objectConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible RegExp object.
        /// </summary>
        public static RegExpConstructor RegExp
        {
            get { return Instance.regExpConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible String object.
        /// </summary>
        public static StringConstructor String
        {
            get { return Instance.stringConstructor; }
        }


        /// <summary>
        /// Gets the globally accessible Error object.
        /// </summary>
        public static ErrorConstructor Error
        {
            get { return Instance.errorConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible RangeError object.
        /// </summary>
        public static ErrorConstructor RangeError
        {
            get { return Instance.rangeErrorConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible TypeError object.
        /// </summary>
        public static ErrorConstructor TypeError
        {
            get { return Instance.typeErrorConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible SyntaxError object.
        /// </summary>
        public static ErrorConstructor SyntaxError
        {
            get { return Instance.syntaxErrorConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible URIError object.
        /// </summary>
        public static ErrorConstructor URIError
        {
            get { return Instance.uriErrorConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible EvalError object.
        /// </summary>
        public static ErrorConstructor EvalError
        {
            get { return Instance.evalErrorConstructor; }
        }

        /// <summary>
        /// Gets the globally accessible ReferenceError object.
        /// </summary>
        public static ErrorConstructor ReferenceError
        {
            get { return Instance.referenceErrorConstructor; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________


        /// <summary>
        /// Decodes a string that was encoded with the encodeURI function.
        /// </summary>
        /// <param name="input"> The string to decode. </param>
        /// <returns> The string, as it was before encoding. </returns>
        [JSFunction(Name = "decodeURI")]
        public static string DecodeURI(string input)
        {
            return Decode(input, ";/?:@&=+$,#");
        }

        /// <summary>
        /// Decodes a string that was encoded with the decodeURIComponent function.
        /// </summary>
        /// <param name="input"> The string to decode. </param>
        /// <returns> The string, as it was before encoding. </returns>
        [JSFunction(Name = "decodeURIComponent")]
        public static string DecodeURIComponent(string input)
        {
            return Decode(input, "");
        }

        /// <summary>
        /// Encodes a string containing a Uniform Resource Identifier (URI).
        /// </summary>
        /// <param name="input"> The string to encode. </param>
        /// <returns> A copy of the given URI with the special characters encoded. </returns>
        [JSFunction(Name = "encodeURI")]
        public static string EncodeURI(string input)
        {
            return Encode(input, ";/?:@&=+$,-_.!~*'()#");
        }

        /// <summary>
        /// Encodes a string containing a portion of a Uniform Resource Identifier (URI).
        /// </summary>
        /// <param name="input"> The string to encode. </param>
        /// <returns> A copy of the given URI with the special characters encoded. </returns>
        [JSFunction(Name = "encodeURIComponent")]
        public static string EncodeURIComponent(string input)
        {
            return Encode(input, "-_.!~*'()");
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
        /// <param name="code"> The source code to evaluate. </param>
        /// <returns> The value of the last statement that was executed, or <c>undefined</c> if
        /// there were no executed statements. </returns>
        [JSFunction(Name = "eval")]
        public static object Eval(string code)
        {
            var scope = Compiler.ObjectScope.CreateGlobalScope();
            var evalContext = new Jurassic.Compiler.EvalContext(scope, scope.ScopeObject, code, false);
            return evalContext.Execute();
        }

        /// <summary>
        /// Evaluates the given javascript source code and returns the result.
        /// </summary>
        /// <param name="code"> The source code to evaluate. </param>
        /// <param name="scope"> The containing scope. </param>
        /// <param name="thisObject"> The value of the "this" keyword in the containing scope. </param>
        /// <param name="strictMode"> Indicates whether the eval statement is being called from
        /// strict mode code. </param>
        /// <returns> The value of the last statement that was executed, or <c>undefined</c> if
        /// there were no executed statements. </returns>
        public static object Eval(string code, Compiler.Scope scope, object thisObject, bool strictMode)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (code == null)
                return Undefined.Value;
            var evalContext = new Jurassic.Compiler.EvalContext(scope, thisObject, code, strictMode);
            return evalContext.Execute();
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
            return ParseNumber(input, allowHexPrefix: false, allowTrailingJunk: true, returnZeroIfEmpty: false);
        }

        /// <summary>
        /// Parses the given string and returns the equivalent integer value. 
        /// </summary>
        /// <param name="input"> The string to parse. </param>
        /// <param name="dblRadix"> The numeric base to use for parsing.  The default is to use base 10
        /// except when the input string starts with '0x' or '0X' in which case base 16 is used
        /// instead. </param>
        /// <returns> The equivalent integer value of the given string. </returns>
        /// <remarks> Leading whitespace is ignored.  Parsing continues until the first invalid
        /// character, at which point parsing stops.  No error is returned in this case. </remarks>
        [JSFunction(Name = "parseInt")]
        public static double ParseInt(string input, double dblRadix = 0)
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

                    if (reader.Peek() == 'x' || reader.Peek() == 'X')
                    {
                        reader.Read();
                        radix = 16;
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



        //     INTERNAL HELPER METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Converts a string to a number.
        /// </summary>
        /// <param name="input"> The string to convert. </param>
        /// <param name="allowHexPrefix"> Indicates whether to allow and honour a leading hex
        /// prefix ("0x"). </param>
        /// <param name="allowTrailingJunk"> If <c>true</c>, indicates that the function should
        /// return a partial result if there are invalid characters found at the end of the string.
        /// Otherwise, this method returns NaN upon encountering invalid characters. </param>
        /// <param name="returnZeroIfEmpty"> If <c>true</c>, indicates that the function should
        /// return zero if the string is empty or solely consists of whitespace.  Otherwise, NaN
        /// is returned. </param>
        /// <returns> The result of parsing the string as a number. </returns>
        internal static double ParseNumber(string input, bool allowHexPrefix, bool allowTrailingJunk, bool returnZeroIfEmpty)
        {
            var reader = new System.IO.StringReader(input);

            // Skip whitespace and line terminators.
            while (IsWhiteSpaceOrLineTerminator(reader.Peek()))
                reader.Read();

            // Type conversion returns zero for an empty string.
            if (returnZeroIfEmpty == true && reader.Peek() == -1)
                return 0.0;

            // Determine the sign.
            double sign = ReadSign(reader);

            double result;
            bool leadingZeroSwallowed = false;
            if (allowHexPrefix == true)
            {
                // If the number starts with '0x' or '0X' then the number should be parsed as a hex
                // number.
                if (reader.Peek() == '0')
                {
                    // Read past the zero.
                    reader.Read();
                    leadingZeroSwallowed = true;     // Note: required for parsing "0z11" correctly (when radix = 0).

                    if (reader.Peek() == 'x' || reader.Peek() == 'X')
                    {
                        // Read past the 'x'.
                        reader.Read();

                        // Read numeric digits 0-9, a-z or A-Z.
                        result = 0;
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
                            if (numericValue == -1 || numericValue >= 16)
                            {
                                // We may have found some trailing junk.
                                if (c != -1 && IsWhiteSpaceOrLineTerminator(c) == false && allowTrailingJunk == false)
                                    return double.NaN;
                                break;
                            }
                            result = result * 16 + numericValue;
                        }

                        // Skip whitespace and line terminators.
                        while (IsWhiteSpaceOrLineTerminator(reader.Peek()))
                            reader.Read();

                        // We may have found some trailing junk.
                        if (reader.Peek() != -1 && allowTrailingJunk == false)
                            return double.NaN;

                        // Otherwise, return the result.
                        return result * sign;
                    }
                }
            }

            // Read numeric digits 0-9.
            result = ReadInteger(reader);
            
            // If ReadInteger couldn't read any digits, and we read a zero earlier, set the result
            // to zero.
            if (leadingZeroSwallowed == true && double.IsNaN(result) == true)
                result = 0;

            if (reader.Peek() == '.')
            {
                // Skip past the '.'.
                reader.Read();

                // Read the fractional component.
                double fraction = ReadInteger(reader);

                // Apply the fractional component.
                if (double.IsNaN(fraction) == false)
                {
                    // parseFloat('.5') should return 0.5.
                    if (double.IsNaN(result) == true)
                        result = 0;
                    result += fraction / System.Math.Pow(10, System.Math.Ceiling(System.Math.Log10(fraction + 1)));
                }
            }

            if (reader.Peek() == 'e' || reader.Peek() == 'E')
            {
                // Skip past the 'e'.
                reader.Read();

                // Read the sign of the exponent.
                double exponentSign = ReadSign(reader);

                // Read the exponent.
                double exponent = ReadInteger(reader) * exponentSign;

                // Apply the exponent.
                if (double.IsNaN(exponent) == false)
                {
                    if (exponent >= 0)
                        result *= System.Math.Pow(10, exponent);
                    else
                        result /= System.Math.Pow(10, -exponent);
                }
            }

            // Infinity or -Infinity are also valid.
            string restOfString = reader.ReadToEnd();
            if (double.IsNaN(result) == true && restOfString.StartsWith("Infinity") == true)
            {
                result = double.PositiveInfinity;
                restOfString = restOfString.Substring("Infinity".Length);
            }

            if (allowTrailingJunk == false)
            {
                // Check the end of the string for junk.
                for (int i = 0; i < restOfString.Length; i++)
                    if (IsWhiteSpaceOrLineTerminator(restOfString[i]) == false)
                        return double.NaN;
            }

            return result * sign;
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Decodes a string containing a URI or a portion of a URI.
        /// </summary>
        /// <param name="input"> The string to decode. </param>
        /// <param name="unescapedSet"> A string containing the set of characters that should not
        /// be escaped.  Alphanumeric characters should not be included. </param>
        /// <returns> A copy of the given string with the escape sequences decoded. </returns>
        private static string Decode(string input, string reservedSet)
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
                        throw new JavaScriptException("URIError", "URI malformed");

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
                            throw new JavaScriptException("URIError", "URI malformed");

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
                            throw new JavaScriptException("URIError", "URI malformed");

                        // Read the additional bytes.
                        byte[] utf8Bytes = new byte[utf8ByteCount];
                        utf8Bytes[0] = (byte)utf8Byte;
                        for (int j = 1; j < utf8ByteCount; j++)
                        {
                            // An additional escape sequence is expected.
                            if (i >= input.Length - 1 || input[++i] != '%')
                                throw new JavaScriptException("URIError", "URI malformed");

                            // Make sure the string is long enough and the next two digits are valid hex digits.
                            if (i >= input.Length - 2 || IsHexDigit(input[i + 1]) == false || IsHexDigit(input[i + 2]) == false)
                                throw new JavaScriptException("URIError", "URI malformed");

                            // Decode the %XX encoding.
                            utf8Byte = int.Parse(input.Substring(i + 1, 2), System.Globalization.NumberStyles.HexNumber);

                            // Top two bits must be 10 (i.e. byte must be 10XXXXXX in binary).
                            if ((utf8Byte & 0xC0) != 0x80)
                                throw new JavaScriptException("URIError", "URI malformed");

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
        /// <param name="input"> The string to encode. </param>
        /// <param name="unescapedSet"> A string containing the set of characters that should not
        /// be escaped.  Alphanumeric characters should not be included. </param>
        /// <returns> A copy of the given URI with the special characters encoded. </returns>
        private static string Encode(string input, string unescapedSet)
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
                    throw new JavaScriptException("URIError", "URI malformed");
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
        /// <returns> The numeric value, or <c>double.NaN</c> if no number was present. </returns>
        private static double ReadInteger(System.IO.StringReader reader)
        {
            // If the input is empty, then return NaN.
            double result = double.NaN;

            while (true)
            {
                int c = reader.Peek();
                if (c < '0' || c > '9')
                    break;
                reader.Read();
                if (double.IsNaN(result))
                    result = c - '0';
                else
                    result = result * 10 + (c - '0');
            }

            return result;
        }

        /// <summary>
        /// Reads a sign character using the given reader.
        /// </summary>
        /// <param name="reader"> The reader to read characters from. </param>
        /// <returns> <c>-1</c> if a negative sign was present, <c>+1</c> otherwise. </returns>
        private static double ReadSign(System.IO.StringReader reader)
        {
            if (reader.Peek() == '+')
                reader.Read();
            else if (reader.Peek() == '-')
            {
                reader.Read();
                return -1.0;
            }
            return 1.0;
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
