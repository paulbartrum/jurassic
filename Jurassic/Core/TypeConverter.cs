using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.Library;

namespace Jurassic
{
    public enum PrimitiveTypeHint
    {
        None,
        Number,
        String,
    }

    /// <summary>
    /// Implements the JavaScript type conversion rules.
    /// </summary>
    public static class TypeConverter
    {
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
            if (value is double)
                return ((double)value) != 0 && double.IsNaN((double)value) == false;
            if (value is string)
                return ((string)value).Length > 0;
            if (value is ConcatenatedString)
                return ((ConcatenatedString)value).Length > 0;
            if (value is ObjectInstance)
                return true;
            throw new ArgumentException("Unexpected type.", "value");
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
                return GlobalObject.ParseNumber((string)value, allowHexPrefix: true, allowTrailingJunk: false, returnZeroIfEmpty: true);
            if (value is ConcatenatedString)
                return GlobalObject.ParseNumber(((ConcatenatedString)value).ToString(), allowHexPrefix: true, allowTrailingJunk: false, returnZeroIfEmpty: true);
            if (value is ObjectInstance)
                return ToNumber(ToPrimitive(value, PrimitiveTypeHint.Number));
            throw new ArgumentException("Unexpected type.", "value");
        }

        /// <summary>
        /// Converts any JavaScript value to a primitive string value.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A primitive string value. </returns>
        public static string ToString(object value)
        {
            if (value == null || value == Undefined.Value)
                return "undefined";
            if (value == Null.Value)
                return "null";
            if (value is bool)
                return (bool)value ? "true" : "false";
            if (value is int)
                return ((int)value).ToString();
            if (value is double)
                return ((double)value).ToString();
            if (value is string)
                return (string)value;
            if (value is ConcatenatedString)
                return ((ConcatenatedString)value).ToString();
            if (value is ObjectInstance)
                return ToString(ToPrimitive(value, PrimitiveTypeHint.String));
            throw new ArgumentException("Unexpected type.", "value");
        }

        /// <summary>
        /// Converts any JavaScript value to an object.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An object. </returns>
        public static ObjectInstance ToObject(object value)
        {
            if (value is ObjectInstance)
                return (ObjectInstance)value;
            if (value == null || value == Undefined.Value)
                throw new JavaScriptException("TypeError", "undefined cannot be converted to an object");
            if (value == Null.Value)
                throw new JavaScriptException("TypeError", "null cannot be converted to an object");
            if (value is bool)
                return GlobalObject.Boolean.Construct((bool)value);
            if (value is int)
                return GlobalObject.Number.Construct((int)value);
            if (value is double)
                return GlobalObject.Number.Construct((double)value);
            if (value is string)
                return GlobalObject.String.Construct((string)value);
            if (value is ConcatenatedString)
                return GlobalObject.String.Construct(((ConcatenatedString)value).ToString());
            throw new ArgumentException("Unexpected type.", "value");
        }

        /// <summary>
        /// Converts any JavaScript value to a primitive value.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> A primitive (non-object) value. </returns>
        public static object ToPrimitive(object value, PrimitiveTypeHint typeHint)
        {
            if (value is ObjectInstance)
                return ((ObjectInstance)value).GetDefaultValue(typeHint);
            else
                return value;
        }

        /// <summary>
        /// Converts any JavaScript value to an integer.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An integer value. </returns>
        public static int ToInteger(object value)
        {
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
            return (int)(uint)ToNumber(value);
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
            return (uint)ToNumber(value);
        }

        /// <summary>
        /// Converts any JavaScript value to an unsigned 16-bit integer.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> An unsigned 16-bit integer value. </returns>
        public static ushort ToUint16(object value)
        {
            return (ushort)(uint)ToNumber(value);
        }

        /// <summary>
        /// Throws a TypeError when the given value is <c>null</c> or <c>undefined.</c>
        /// </summary>
        /// <param name="value"> The value to check. </param>
        public static void CheckCoercibleToObject(object value)
        {
            if (value == null || value == Undefined.Value)
                throw new JavaScriptException("TypeError", "undefined cannot be converted to an object");
            if (value == Null.Value)
                throw new JavaScriptException("TypeError", "null cannot be converted to an object");
        }

    }

}
