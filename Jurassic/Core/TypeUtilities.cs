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
            if (obj is string)
                return "string";
            if (obj is FunctionInstance)
                return "function";
            if (obj is ObjectInstance)
                return "object";
            throw new InvalidOperationException("Unsupported object type.");
        }

        /// <summary>
        /// Enumerates the names of the enumerable properties on the given object, including
        /// properties defined on the object's prototype.  Used by the for-in statement.
        /// </summary>
        /// <param name="obj"> The object to enumerate. </param>
        /// <returns> An enumerator that iteratively returns property names. </returns>
        public static IEnumerable<string> EnumeratePropertyNames(object obj)
        {
            if (obj == null || obj == Undefined.Value || obj == Null.Value)
                return new string[0];
            var obj2 = TypeConverter.ToObject(obj);
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
            if (leftPrimitive is string || rightPrimitive is string)
                return string.Concat(TypeConverter.ToString(leftPrimitive), TypeConverter.ToString(rightPrimitive));
            return TypeConverter.ToNumber(leftPrimitive) + TypeConverter.ToNumber(rightPrimitive);
        }
    }

}
