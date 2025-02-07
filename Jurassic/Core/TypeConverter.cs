using System;
using Jurassic.Library;

namespace Jurassic
{
    /// <summary>
    /// Provides a hint on how to do the conversion when converting to a primitive.
    /// </summary>
    public enum PrimitiveTypeHint
    {
        /// <summary>
        /// Use the default behaviour.
        /// </summary>
        None,

        /// <summary>
        /// Prefer converting to a number.
        /// </summary>
        Number,

        /// <summary>
        /// Prefer converting to a string.
        /// </summary>
        String,
    }

    /// <summary>
    /// Implements the JavaScript type conversion rules.
    /// </summary>
    public static class TypeConverter
    {
        /// <summary>
        /// Converts the given value to the given type.
        /// </summary>
        /// <param name="engine"> The script engine used to create new objects. </param>
        /// <param name="value"> The value to convert. </param>
        /// <typeparam name="T"> The type to convert the value to. </typeparam>
        /// <returns> The converted value. </returns>
        public static T ConvertTo<T>(ScriptEngine engine, object value)
        {
            return (T)ConvertTo(engine, value, typeof(T));
        }

        /// <summary>
        /// Converts the given value to the given type.
        /// </summary>
        /// <param name="engine"> The script engine used to create new objects. </param>
        /// <param name="value"> The value to convert. </param>
        /// <param name="type"> The type to convert the value to. </param>
        /// <returns> The converted value. </returns>
        public static object ConvertTo(ScriptEngine engine, object value, Type type)
        {
            if (type == typeof(bool))
                return ToBoolean(value);
            if (type == typeof(int))
                return ToInteger(value);
            if (type == typeof(double))
                return ToNumber(value);
            if (type == typeof(string))
                return ToString(value);
            if (typeof(ObjectInstance).IsAssignableFrom(type))
                return ToObject(engine, value);
            if (type == typeof(object))
                return value;
            throw new ArgumentException(string.Format("Cannot convert to '{0}'.  The type is unsupported.", type), nameof(value));
        }

        /// <summary>
        /// Converts any JavaScript value to a primitive boolean value.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A primitive boolean value. </returns>
        public static bool ToBoolean(object value)
        {
            if (value == null || value == Null.Value)
                return false;
            if (value == Undefined.Value)
                return false;
            if (value is bool)
                return (bool)value;
            if (value is int)
                return ((int)value) != 0;
            if (value is uint)
                return ((uint)value) != 0;
            if (value is double)
                return ((double)value) != 0 && double.IsNaN((double)value) == false;
            if (value is string)
                return ((string)value).Length > 0;
            if (value is ConcatenatedString)
                return ((ConcatenatedString)value).Length > 0;
            if (value is Symbol)
                return true;
            if (value is ObjectInstance)
                return true;
            throw new ArgumentException(string.Format("Cannot convert object of type '{0}' to a boolean.", value.GetType()), nameof(value));
        }

        /// <summary>
        /// Converts any JavaScript value to a primitive number value.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A primitive number value. </returns>
        public static double ToNumber(object value)
        {
            if (value is double)
                return (double)value;
            if (value is int)
                return (double)(int)value;
            if (value is uint)
                return (double)(uint)value;
            if (value == null || value == Undefined.Value)
                return double.NaN;
            if (value == Null.Value)
                return +0;
            if (value is bool)
                return (bool)value ? 1 : 0;
            if (value is string)
                return NumberParser.CoerceToNumber((string)value);
            if (value is ConcatenatedString)
                return NumberParser.CoerceToNumber(value.ToString());
            if (value is Symbol)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot convert a Symbol value to a number.");
            if (value is ObjectInstance)
                return ToNumber(ToPrimitive(value, PrimitiveTypeHint.Number));
            throw new ArgumentException(string.Format("Cannot convert object of type '{0}' to a number.", value.GetType()), nameof(value));
        }

        // Single-item cache.
        private class NumberToStringCache
        {
            public double Value;
            public string Result;
        }
        private static NumberToStringCache numberToStringCache = new NumberToStringCache() { Value = 0.0, Result = "0" };

        /// <summary>
        /// Converts any JavaScript value to a primitive string value.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A primitive string value. </returns>
        public static string ToString(object value)
        {
            return ToString(value, "undefined");
        }

        /// <summary>
        /// Converts any JavaScript value to a primitive string value.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <param name="defaultValue"> The value to return if the input is undefined. </param>
        /// <returns> A primitive string value. </returns>
        internal static string ToString(object value, string defaultValue)
        {
            if (value == null || value == Undefined.Value)
                return defaultValue;
            if (value == Null.Value)
                return "null";
            if (value is bool)
                return (bool)value ? "true" : "false";
            if (value is int)
                return ((int)value).ToString();
            if (value is uint)
                return ((uint)value).ToString();
            if (value is double)
            {
                // Check if the value is in the cache.
                double doubleValue = (double)value;
                var cache = numberToStringCache;
                if (doubleValue == cache.Value)
                    return cache.Result;

                // Convert the number to a string.
                var result = NumberFormatter.ToString((double)value, 10, NumberFormatter.Style.Regular);

                // Cache the result.
                // This is thread-safe on Intel but not architectures with weak write ordering.
                numberToStringCache = new NumberToStringCache() { Value = doubleValue, Result = result };

                return result;
            }
            if (value is string)
                return (string)value;
            if (value is ConcatenatedString)
                return value.ToString();
            if (value is Symbol)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot convert a Symbol value to a string.");
            if (value is ObjectInstance)
                return ToString(ToPrimitive(value, PrimitiveTypeHint.String));
            return value.ToString();
        }

        /// <summary>
        /// Converts any JavaScript value to a concatenated string value.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A concatenated string value. </returns>
        public static ConcatenatedString ToConcatenatedString(object value)
        {
            if (value is ConcatenatedString)
                return (ConcatenatedString)value;
            return new ConcatenatedString(ToString(value));
        }

        /// <summary>
        /// Converts any JavaScript value to an object.
        /// </summary>
        /// <param name="engine"> The script engine used to create new objects. </param>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An object. </returns>
        public static ObjectInstance ToObject(ScriptEngine engine, object value)
        {
            return ToObject(engine, value, 0, null, null);
        }

        /// <summary>
        /// Converts any JavaScript value to an object.
        /// </summary>
        /// <param name="engine"> The script engine used to create new objects. </param>
        /// <param name="value"> The value to convert. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        /// <param name="functionName"> The name of the function.  Can be <c>null</c>. </param>
        /// <returns> An object. </returns>
        public static ObjectInstance ToObject(ScriptEngine engine, object value, int lineNumber, string sourcePath, string functionName)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));
            if (value is ObjectInstance)
                return (ObjectInstance)value;
            if (value == null || value == Undefined.Value)
                throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object", lineNumber, sourcePath, functionName);
            if (value == Null.Value)
                throw new JavaScriptException(ErrorType.TypeError, "null cannot be converted to an object", lineNumber, sourcePath, functionName);

            ObjectInstance result;
            if (value is bool)
                result = engine.Boolean.Construct((bool)value);
            else if (value is int)
                result = engine.Number.Construct((int)value);
            else if (value is uint)
                result = engine.Number.Construct((uint)value);
            else if (value is double)
                result = engine.Number.Construct((double)value);
            else if (value is string)
                result = engine.String.Construct((string)value);
            else if (value is ConcatenatedString)
                result = engine.String.Construct(value.ToString());
            else if (value is Symbol symbolValue)
                result = new SymbolInstance(engine.Symbol.InstancePrototype, symbolValue);
            else
                throw new ArgumentException(string.Format("Cannot convert object of type '{0}' to an object.", value.GetType()), nameof(value));

            return result;
        }

        /// <summary>
        /// Converts any JavaScript value to an object.
        /// </summary>
        /// <param name="engine"> The script engine used to create new objects. </param>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An object. </returns>
        public static T ToObject<T>(ScriptEngine engine, object value) where T : ObjectInstance
        {
            if (value is T)
                return (T)value;
            throw new JavaScriptException(ErrorType.TypeError, "Incorrect argument type.");
        }

        /// <summary>
        /// Converts any JavaScript value to a primitive value.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <param name="preferredType"> Specifies whether toString() or valueOf() should be
        /// preferred when converting to a primitive. </param>
        /// <returns> A primitive (non-object) value. </returns>
        public static object ToPrimitive(object value, PrimitiveTypeHint preferredType)
        {
            if (value is ObjectInstance)
                return ((ObjectInstance)value).GetPrimitiveValue(preferredType);
            else
                return value;
        }

        /// <summary>
        /// Converts a value to a property key (either a string or a symbol).
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A property key value. </returns>
        public static object ToPropertyKey(object value)
        {
            if (value is Symbol)
                return value;
            return ToString(value);
        }

        /// <summary>
        /// Converts any JavaScript value to an integer.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An integer value. </returns>
        public static int ToInteger(object value)
        {
            // HACK ALERT: per the spec, this should actually return a double.

            if (value == null || value is Undefined)
                return 0;
            double num = ToNumber(value);
            if (num > 2147483647.0)
                return 2147483647;
#pragma warning disable 1718
            if (num != num)
                return 0;
#pragma warning restore 1718
            return (int)num;
        }

        /// <summary>
        /// Converts any JavaScript value to a 32-bit integer.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A 32-bit integer value. </returns>
        public static int ToInt32(object value)
        {
            if (value is int)
                return (int)value;
            double num = ToNumber(value);
            if (num < long.MinValue || num > long.MaxValue)
            {
                if (double.IsNegativeInfinity(num) || double.IsPositiveInfinity(num))
                    return 0;
                return (int)Math.IEEERemainder(num, 4294967296.0);
            }
            return (int)(long)num;
        }

        /// <summary>
        /// Converts any JavaScript value to an unsigned 32-bit integer.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An unsigned 32-bit integer value. </returns>
        public static uint ToUint32(object value)
        {
            if (value is uint)
                return (uint)value;
            return (uint)ToInt32(value);
        }

        /// <summary>
        /// Converts any JavaScript value to a signed 16-bit integer.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A signed 16-bit integer value. </returns>
        public static short ToInt16(object value)
        {
            return (short)ToInt32(value);
        }

        /// <summary>
        /// Converts any JavaScript value to an unsigned 16-bit integer.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An unsigned 16-bit integer value. </returns>
        public static ushort ToUint16(object value)
        {
            return (ushort)ToInt32(value);
        }

        /// <summary>
        /// Converts any JavaScript value to a signed 8-bit integer.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A signed 8-bit integer value. </returns>
        public static sbyte ToInt8(object value)
        {
            return (sbyte)ToInt32(value);
        }

        /// <summary>
        /// Converts any JavaScript value to an unsigned 8-bit integer.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An unsigned 8-bit integer value. </returns>
        public static byte ToUint8(object value)
        {
            return (byte)ToInt32(value);
        }

        /// <summary>
        /// Utility method to convert an object array to a typed array.
        /// </summary>
        /// <typeparam name="T"> The type to convert to. </typeparam>
        /// <param name="engine"> The script engine used to create new objects. </param>
        /// <param name="args"> The array to convert. </param>
        /// <param name="offset"> The number of elements to skip at the beginning of the array. </param>
        /// <returns> A typed array. </returns>
        internal static T[] ConvertParameterArrayTo<T>(ScriptEngine engine, object[] args, int offset)
        {
            if (offset >= args.Length)
                return new T[0];
            var result = new T[args.Length - offset];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ConvertTo<T>(engine, args[offset + i]);
            }
            return result;
        }
    }

}
