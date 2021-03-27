﻿using System;
using System.Collections;
using System.Collections.Generic;
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
            if (obj == Null.Value)
                return "object";
            if (obj is bool)
                return "boolean";
            if (obj is double || obj is int || obj is uint)
                return "number";
            if (obj is string || obj is ConcatenatedString)
                return "string";
            if (obj is FunctionInstance)
                return "function";
            if (obj is Symbol)
                return "symbol";
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
        /// Returns <c>true</c> if the given value is a supported numeric type.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> <c>true</c> if the given value is a supported numeric type; <c>false</c>
        /// otherwise. </returns>
        public static bool IsNumeric(object obj)
        {
            return obj is double || obj is int || obj is uint;
        }

        /// <summary>
        /// Returns <c>true</c> if the given value is a supported string type.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> <c>true</c> if the given value is a supported string type; <c>false</c>
        /// otherwise. </returns>
        public static bool IsString(object obj)
        {
            return obj is string || obj is ConcatenatedString;
        }

        /// <summary>
        /// Converts the given value into a standard .NET type, suitable for returning from an API.
        /// </summary>
        /// <param name="obj"> The value to normalize. </param>
        /// <returns> The value as a standard .NET type. </returns>
        internal static object NormalizeValue(object obj)
        {
            if (obj == null)
                return Undefined.Value;
            else if (obj is double)
            {
                var numericResult = (double)obj;
                if (((int)numericResult) == numericResult)
                    return (int)numericResult;
            }
            else if (obj is uint)
            {
                var uintValue = (uint)obj;
                if ((int)uintValue >= 0)
                    return (int)uintValue;
                return (double)uintValue;
            }
            else if (obj is ConcatenatedString)
                obj = ((ConcatenatedString)obj).ToString();
            else if (obj is ClrInstanceWrapper)
                obj = ((ClrInstanceWrapper)obj).WrappedInstance;
            else if (obj is ClrStaticTypeWrapper)
                obj = ((ClrStaticTypeWrapper)obj).WrappedType;
            return obj;
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
            if (IsUndefined(obj) == true || obj == Null.Value)
                yield break;
            var objectInstance = TypeConverter.ToObject(engine, obj);
            var names = new HashSet<string>();
            do
            {
                foreach (var key in objectInstance.OwnKeys)
                {
                    // Only enumerate string-based property keys, not symbols.
                    if (key is string name)
                    {
                        // Check whether the property is shadowed.
                        if (names.Contains(name) == false)
                        {
                            // Only return enumerable properties.
                            var propertyDescriptor = objectInstance.GetOwnPropertyDescriptor(name);
                            if (propertyDescriptor.Exists && propertyDescriptor.IsEnumerable == true)
                            {
                                yield return name;
                            }

                            // Record the name so we can check if it was shadowed.
                            names.Add(name);
                        }
                    }
                }
                objectInstance = objectInstance.Prototype;
            } while (objectInstance != null);
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

            if (leftPrimitive is ConcatenatedString)
            {
                return ((ConcatenatedString)leftPrimitive).Concatenate(rightPrimitive);
            }
            else if (leftPrimitive is string || rightPrimitive is string || rightPrimitive is ConcatenatedString)
            {
                return new ConcatenatedString(TypeConverter.ToString(leftPrimitive), TypeConverter.ToString(rightPrimitive));
            }

            return TypeConverter.ToNumber(leftPrimitive) + TypeConverter.ToNumber(rightPrimitive);
        }

        /// <summary>
        /// Determines if the given value is a supported JavaScript primitive.
        /// </summary>
        /// <param name="value"> The value to test. </param>
        /// <returns> <c>true</c> if the given value is a supported JavaScript primitive;
        /// <c>false</c> otherwise. </returns>
        public static bool IsPrimitive(object value)
        {
            if (value == null)
                return true;
            var type = value.GetType();
            return type == typeof(bool) ||
                type == typeof(int) || type == typeof(uint) || type == typeof(double) ||
                type == typeof(string) || type == typeof(ConcatenatedString) ||
                type == typeof(Null) || type == typeof(Undefined) ||
                type == typeof(Symbol);
        }

        /// <summary>
        /// Determines if the given value is a supported JavaScript primitive or derives from
        /// ObjectInstance.
        /// </summary>
        /// <param name="value"> The value to test. </param>
        /// <returns> <c>true</c> if the given value is a supported JavaScript primitive or derives
        /// from ObjectInstance; <c>false</c> otherwise. </returns>
        public static bool IsPrimitiveOrObject(object value)
        {
            if (value == null)
                return true;
            var type = value.GetType();
            return type == typeof(bool) ||
                type == typeof(int) || type == typeof(uint) || type == typeof(double) ||
                type == typeof(string) || type == typeof(ConcatenatedString) ||
                type == typeof(Null) || type == typeof(Undefined) ||
                typeof(ObjectInstance).IsAssignableFrom(type);
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
                throw new JavaScriptException(ErrorType.TypeError, string.Format("The function '{0}' does not allow the value of 'this' to be undefined", functionName));
            if (value == Null.Value)
                throw new JavaScriptException(ErrorType.TypeError, string.Format("The function '{0}' does not allow the value of 'this' to be null", functionName));
        }

        /// <summary>
        /// Utility method for create a slice of an array.
        /// </summary>
        /// <param name="args"> The array to slice. </param>
        /// <param name="offset"> The offset to begin the resulting array. </param>
        /// <returns> An array containing the input array elements, excluding the first
        /// <paramref name="offset"/> entries. </returns>
        internal static object[] SliceArray(object[] args, int offset)
        {
            if (offset == 0)
                return args;
            if (offset >= args.Length)
                return new object[0];
            var result = new object[args.Length - offset];
            Array.Copy(args, offset, result, 0, args.Length - offset);
            return result;
        }

        private static readonly long negativeZeroBits = BitConverter.DoubleToInt64Bits(-0.0);

        /// <summary>
        /// Determines if the given number is negative zero.
        /// </summary>
        /// <param name="value"> The value to test. </param>
        /// <returns> <c>true</c> if the value is negative zero; <c>false</c> otherwise. </returns>
        public static bool IsNegativeZero(double value)
        {
            return BitConverter.DoubleToInt64Bits(value) == negativeZeroBits;
        }

        private static readonly long positiveZeroBits = BitConverter.DoubleToInt64Bits(0.0);

        /// <summary>
        /// Determines if the given number is positive zero.
        /// </summary>
        /// <param name="value"> The value to test. </param>
        /// <returns> <c>true</c> if the value is positive zero; <c>false</c> otherwise. </returns>
        public static bool IsPositiveZero(double value)
        {
            return BitConverter.DoubleToInt64Bits(value) == positiveZeroBits;
        }

        /// <summary>
        /// Converts an iteratable object into a iterator by looking up the @@iterator property,
        /// then calling that value as a function.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="iterable"> The object to get a iterator from. </param>
        /// <returns> An iterator object, with a next function, or <c>null</c> if the iterator
        /// symbol value is undefined or null. </returns>
        public static ObjectInstance GetIterator(ScriptEngine engine, ObjectInstance iterable)
        {
            if (iterable == null)
                throw new ArgumentNullException(nameof(iterable));

            // Get the iterator symbol value.
            var iteratorValue = iterable[Symbol.Iterator];
            if (iteratorValue == Undefined.Value || iteratorValue == Null.Value)
                return null;

            // If a value is present, it must be a function.
            var iteratorFunc = iteratorValue as FunctionInstance;
            if (iteratorFunc == null)
                throw new JavaScriptException(ErrorType.TypeError, "The iterator symbol value must be a function");

            // Call the function to get the iterator.
            var iterator = iteratorFunc.Call(iterable) as ObjectInstance;
            if (iterator == null)
                throw new JavaScriptException(ErrorType.TypeError, "Invalid iterator");
            return iterator;
        }

        /// <summary>
        /// Creates an iterator object that can iterate over a .NET enumerable collection. The
        /// returned object also supports the iterable protocol, meaning it can be used in a for-of
        /// loop.
        /// </summary>
        /// <param name="engine"> The script engine to associate the new object with. </param>
        /// <param name="enumerable"> The enumerable collection. The item type must be a supported
        /// type. </param>
        /// <returns> An iterator object that also supports the iterable protocol. </returns>
        public static ObjectInstance CreateIterator(ScriptEngine engine, IEnumerable enumerable)
        {
            return new Iterator(engine, enumerable);
        }

        /// <summary>
        /// Converts an iteratable object into a iterator by looking up the @@iterator property,
        /// then calling that value as a function. Throws an exception if the object isn't iterable.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="iterable"> The object to get a iterator from. </param>
        /// <returns> An iterator object, with a next function. </returns>
        public static ObjectInstance RequireIterator(ScriptEngine engine, object iterable)
        {

            if (iterable == Undefined.Value || iterable == Null.Value)
                throw new JavaScriptException(ErrorType.TypeError, $"{iterable} is not iterable.");
            var iterator = GetIterator(engine, TypeConverter.ToObject(engine, iterable));
            if (iterator == null)
                throw new JavaScriptException(ErrorType.TypeError, $"{iterable} is not iterable.");
            return iterator;
        }

        /// <summary>
        /// Iterate over the values in an iterator.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="iterator"> The iterator object.  Must contain a next function. </param>
        /// <returns> An enumerable list of iterator values. </returns>
        public static IEnumerable<object> Iterate(ScriptEngine engine, ObjectInstance iterator)
        {
            if (iterator == null)
                throw new ArgumentNullException(nameof(iterator));

            // Okay, we have the iterator.  Now get a reference to the next function.
            var nextFunc = iterator["next"] as FunctionInstance;
            if (nextFunc == null)
                throw new JavaScriptException(ErrorType.TypeError, "Missing iterator next function");

            // Loop.
            var values = new List<object>();
            while (true)
            {
                // Call the next function to get the next value.
                var iteratorResult = nextFunc.Call(iterator) as ObjectInstance;
                if (iteratorResult == null)
                    throw new JavaScriptException(ErrorType.TypeError, "Invalid iterator next return value");

                // Check if iteration is done.
                if (TypeConverter.ToBoolean(iteratorResult["done"]))
                    break;

                // Return the value.
                yield return iteratorResult["value"];
            }
        }

        /// <summary>
        /// Implements the logic for the for-of operator.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="iterable"> The object to get a iterator from. </param>
        /// <returns> An enumerable list of iterator values. </returns>
        public static IEnumerable<object> ForOf(ScriptEngine engine, object iterable)
        {
            return Iterate(engine, RequireIterator(engine, iterable));
        }

        /// <summary>
        /// Create an array whose elements are provided by the indexed properties of an array-like object.
        /// </summary>
        /// <param name="arrayLike"> The array-like object to convert. </param>
        /// <returns> An array. </returns>
        internal static object[] CreateListFromArrayLike(ObjectInstance arrayLike)
        {
            if (arrayLike is ArrayInstance arrayInstance)
                return arrayInstance.ToArray();

            uint length = ArrayInstance.GetLength(arrayLike);
            var result = new object[length];
            for (uint i = 0; i < length; i++)
                result[i] = arrayLike[i];
            return result;
        }

        /// <summary>
        /// Retrieves the constructor that should be used to create new objects that are derived
        /// from the argument object <paramref name="objectInstance"/>.
        /// </summary>
        /// <param name="objectInstance"> The object to check. </param>
        /// <param name="defaultConstructor"> The constructor to return if no @@species property
        /// can be found. </param>
        /// <returns> A constructor that can be used to create new objects. </returns>
        internal static FunctionInstance GetSpeciesConstructor(ObjectInstance objectInstance, FunctionInstance defaultConstructor)
        {
            var constructor = objectInstance["constructor"];
            if (constructor == Undefined.Value)
                return defaultConstructor;
            if (constructor is ObjectInstance constructorObjectInstance)
            {
                var species = constructorObjectInstance[Symbol.Species];
                if (species == Undefined.Value || species == Null.Value)
                    return defaultConstructor;
                if (species is FunctionInstance speciesConstructor)
                    return speciesConstructor;
                throw new JavaScriptException(ErrorType.TypeError, "@@species value must be a constructor.");
            }
            else
                throw new JavaScriptException(ErrorType.TypeError, "'constructor' value is not an object.");
        }
    }

}
