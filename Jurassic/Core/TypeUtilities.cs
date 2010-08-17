using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.Library;

namespace Jurassic
{

    /// <summary>
    /// Contains type-related functionality that isn't conversion or comparison.
    /// </summary>
    public static class TypeUtilities
    {
        /// <summary>
        /// Gets the type name for the given object.  Used by the typeof operator.
        /// </summary>
        /// <param name="obj"> The object to get the type name for. </param>
        /// <returns> The type name for the given object. </returns>
        public static string TypeOf(object obj)
        {
            if (obj == null || obj == Undefined.Value)
                return "undefined";
            if (obj is bool)
                return "boolean";
            if (obj is double || obj is int)
                return "number";
            if (obj is string || obj is ConcatenatedString)
                return "string";
            if (obj is FunctionInstance)
                return "function";
            if (obj is ObjectInstance)
                return "object";
            throw new InvalidOperationException("Unsupported object type.");
        }

        /// <summary>
        /// Returns <c>true</c> if the given value is undefined.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> <c>true</c> if the given value is undefined; <c>false</c> otherwise. </returns>
        public static bool IsUndefined(object obj)
        {
            return obj == null || obj == Undefined.Value;
        }

        /// <summary>
        /// Enumerates the names of the enumerable properties on the given object, including
        /// properties defined on the object's prototype.  Used by the for-in statement.
        /// </summary>
        /// <param name="engine"> The script engine used to convert the given value to an object. </param>
        /// <param name="obj"> The object to enumerate. </param>
        /// <returns> An enumerator that iteratively returns property names. </returns>
        public static IEnumerable<string> EnumeratePropertyNames(ScriptEngine engine, object obj)
        {
            if (obj == null || obj == Undefined.Value || obj == Null.Value)
                return new string[0];
            var obj2 = TypeConverter.ToObject(engine, obj);
            var names = new List<string>();
            do
            {
                foreach (var property in obj2.Properties)
                    if (property.IsEnumerable == true)
                        names.Add(property.Name);
                obj2 = obj2.Prototype;
            } while (obj2 != null);
            return names;
        }

        /// <summary>
        /// Adds two objects together, as if by the javascript addition operator.
        /// </summary>
        /// <param name="left"> The left hand side operand. </param>
        /// <param name="right"> The right hand side operand. </param>
        /// <returns> Either a number or a concatenated string. </returns>
        public static object Add(object left, object right)
        {
            var leftPrimitive = TypeConverter.ToPrimitive(left, PrimitiveTypeHint.None);
            var rightPrimitive = TypeConverter.ToPrimitive(right, PrimitiveTypeHint.None);
            
            if (leftPrimitive is string || leftPrimitive is ConcatenatedString || rightPrimitive is string || rightPrimitive is ConcatenatedString)
            {
                // Convert the left-hand side to a concatenated string.
                //ConcatenatedString leftConcatenatedString;
                //if (leftPrimitive is string)
                //    leftConcatenatedString = new ConcatenatedString((string)leftPrimitive);
                //else if (leftPrimitive is ConcatenatedString)
                //    leftConcatenatedString = (ConcatenatedString)leftPrimitive;
                //else
                //    leftConcatenatedString = new ConcatenatedString(TypeConverter.ToString(leftPrimitive));

                //// Append the right-hand side to the concatenated string.
                //if (rightPrimitive is string)
                //    return leftConcatenatedString.Append((string)rightPrimitive);
                //else if (leftPrimitive is ConcatenatedString)
                //    return leftConcatenatedString.Append((ConcatenatedString)rightPrimitive);
                //else
                //    return leftConcatenatedString.Append(TypeConverter.ToString(rightPrimitive));
                return string.Concat(TypeConverter.ToString(leftPrimitive), TypeConverter.ToString(rightPrimitive));
            }

            return TypeConverter.ToNumber(leftPrimitive) + TypeConverter.ToNumber(rightPrimitive);
        }

        /// <summary>
        /// Determines if the given type is a supported JavaScript primitive type.
        /// </summary>
        /// <param name="type"> The type to test. </param>
        /// <returns> <c>true</c> if the given type is a supported JavaScript primitive type;
        /// <c>false</c> otherwise. </returns>
        internal static bool IsPrimitiveType(Type type)
        {
            return type == typeof(bool) || type == typeof(int) || type == typeof(double) ||
                type == typeof(string) || type == typeof(Null) || type == typeof(Undefined);
        }

        /// <summary>
        /// Throws a TypeError when the given value is <c>null</c> or <c>undefined.</c>
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="value"> The value to check. </param>
        /// <param name="functionName"> The name of the function which is doing the check. </param>
        public static void VerifyThisObject(ScriptEngine engine, object value, string functionName)
        {
            if (value == null || value == Undefined.Value)
                throw new JavaScriptException(engine, "TypeError", string.Format("The function '{0}' does not allow the value of 'this' to be undefined", functionName));
            if (value == Null.Value)
                throw new JavaScriptException(engine, "TypeError", string.Format("The function '{0}' does not allow the value of 'this' to be null", functionName));
        }
    }

}
