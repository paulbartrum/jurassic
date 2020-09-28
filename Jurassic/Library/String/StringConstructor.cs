﻿using System;


namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript String object.
    /// </summary>
    public partial class StringConstructor : ClrStubFunction
    {
        
        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new String object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal StringConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "String", 1, StringInstance.CreatePrototype(Engine, this));
            InitializeProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the String object is invoked like a function, e.g. var x = String().
        /// Returns an empty string.
        /// </summary>
        [JSCallFunction]
        public string Call()
        {
            return string.Empty;
        }

        /// <summary>
        /// Called when the String object is invoked like a function, e.g. var x = String(NaN).
        /// Converts the given argument into a string value (not a String object).
        /// </summary>
        [JSCallFunction]
        public string Call(object value)
        {
            if (value is Symbol symbol)
                return symbol.ToString();
            return TypeConverter.ToString(value);
        }

        /// <summary>
        /// Creates a new String instance and initializes it to the empty string.
        /// </summary>
        [JSConstructorFunction]
        public StringInstance Construct()
        {
            return new StringInstance(this.InstancePrototype, "");
        }

        /// <summary>
        /// Creates a new String instance and initializes it to the given value.
        /// </summary>
        /// <param name="value"> The value to initialize to. </param>
        [JSConstructorFunction]
        public StringInstance Construct(string value)
        {
            return new StringInstance(this.InstancePrototype, value);
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a string created by using the specified sequence of Unicode values.
        /// </summary>
        /// <param name="charCodes"> An array of 16-bit character codes. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "fromCharCode")]
        public static string FromCharCode(params double[] charCodes)
        {
            // Note: charCodes must be an array of doubles, because the default marshalling
            // rule to int uses ToInteger() and there are no marshalling rules for short, ushort
            // or uint.  ToInteger() doesn't preserve the wrapping behaviour we need.
            var result = new System.Text.StringBuilder(charCodes.Length);
            foreach (double charCode in charCodes)
                result.Append((char)TypeConverter.ToUint16(charCode));
            return result.ToString();
        }

        /// <summary>
        /// Returns a string created by using the specified sequence of Unicode codepoints.
        /// </summary>
        /// <param name="scriptEngine"> The script engine. </param>
        /// <param name="codePoints"> An array of unicode code points. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "fromCodePoint", Flags = JSFunctionFlags.HasEngineParameter)]
        public static string FromCodePoint(ScriptEngine scriptEngine, params double[] codePoints)
        {
            // Note: charCodes must be an array of doubles, because the default marshalling
            // rule to int uses ToInteger() and ToInteger() does not throw a RangeError if the
            // input value is not an integer.
            var result = new System.Text.StringBuilder(codePoints.Length);
            foreach (double codePointDouble in codePoints)
            {
                int codePoint = (int) codePointDouble;
                if (codePoint < 0 || codePoint > 0x10FFFF || (double)codePoint != codePointDouble)
                    throw new JavaScriptException(ErrorType.RangeError, string.Format("Invalid code point {0}", codePointDouble));
                if (codePoint <= 65535)
                    result.Append((char)codePoint);
                else
                {
                    result.Append((char)((codePoint - 65536)/1024 + 0xD800));
                    result.Append((char)((codePoint - 65536) % 1024 + 0xDC00));
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// A tag function of template literals which is used to get the raw string form of
        /// template strings (that is, the original, uninterpreted text).
        /// </summary>
        /// <param name="scriptEngine"> The script engine. </param>
        /// <param name="template"> Well-formed template call site object e.g. { raw: ['one', 'two'] }. </param>
        /// <param name="substitutions"> Contains substitution values. </param>
        /// <returns> A formatted string containing the raw template literal text (with
        /// substitutions included). </returns>
        [JSInternalFunction(Name = "raw", Flags = JSFunctionFlags.HasEngineParameter)]
        public static string Raw(ScriptEngine scriptEngine, ObjectInstance template, params object[] substitutions)
        {
            var raw = TypeConverter.ToObject(scriptEngine, template["raw"]);
            int length = TypeConverter.ToInteger(raw["length"]);
            if (length <= 0)
                return string.Empty;
            var result = new System.Text.StringBuilder();
            for (int i = 0; i < length; i ++)
            {
                result.Append(TypeConverter.ToString(raw[i]));
                if (i < length - 1 && i < substitutions.Length)
                    result.Append(TypeConverter.ToString(substitutions[i]));
            }
            return result.ToString();
        }
    }
}
