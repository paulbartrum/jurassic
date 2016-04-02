using System;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a typed array instance.
    /// </summary>
    [Serializable]
    public partial class TypedArrayInstance : ObjectInstance
    {
        private TypedArrayType type;
        private ArrayBufferInstance buffer;
        private int byteOffset;
        private int length;

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new typed array instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="type"> Defines the element type and behaviour of the typed array. </param>
        /// <param name="buffer"> An existing ArrayBuffer to use as the storage for the new
        /// TypedArray object. </param>
        /// <param name="byteOffset"> The offset, in bytes, to the first byte in the specified
        /// buffer for the new view to reference. If not specified, the TypedArray will start
        /// with the first byte. </param>
        /// <param name="length"> The length (in elements) of the typed array. </param>
        internal TypedArrayInstance(ObjectInstance prototype, TypedArrayType type, ArrayBufferInstance buffer, int byteOffset, int length)
            : base(prototype)
        {
            this.type = type;
            this.buffer = buffer;
            this.byteOffset = byteOffset;
            this.length = length;
        }

        /// <summary>
        /// Creates the TypedArray prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, TypedArrayConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            result.FastSetProperties(properties);
            return result;
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// The ArrayBuffer referenced by the TypedArray at construction time.
        /// </summary>
        [JSProperty(Name = "buffer")]
        public ArrayBufferInstance Buffer
        {
            get { return this.buffer; }
        }

        /// <summary>
        /// The offset (in bytes) of the typed array from the start of its ArrayBuffer.
        /// </summary>
        [JSProperty(Name = "byteOffset")]
        public int ByteOffset
        {
            get { return this.byteOffset; }
        }

        /// <summary>
        /// The length (in bytes) of the typed array from the start of its ArrayBuffer.
        /// </summary>
        [JSProperty(Name = "byteLength")]
        public int ByteLength
        {
            get
            {
                switch (type)
                {
                    case TypedArrayType.Int8Array:
                    case TypedArrayType.Uint8Array:
                    case TypedArrayType.Uint8ClampedArray:
                        return this.length;
                    case TypedArrayType.Int16Array:
                    case TypedArrayType.Uint16Array:
                        return this.length * 2;
                    case TypedArrayType.Int32Array:
                    case TypedArrayType.Uint32Array:
                    case TypedArrayType.Float32Array:
                        return this.length * 4;
                    case TypedArrayType.Float64Array:
                        return this.length * 8;
                    default:
                        throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
                }
            }
        }

        /// <summary>
        /// The length (in elements) of the typed array.
        /// </summary>
        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.length; }
        }

        /// <summary>
        /// The length (in elements) of the typed array.
        /// </summary>
        [JSProperty(Name = "@@toStringTag")]
        internal string ToStringTag
        {
            get { throw new NotImplementedException(); }
        }


        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a new Array Iterator object that contains the key/value pairs for each index in
        /// the array.
        /// </summary>
        [JSInternalFunction(Name = "entries")]
        public void Entries()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a new Array Iterator object that contains the keys for each index in the array.
        /// </summary>
        [JSInternalFunction(Name = "keys")]
        public static void Keys()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a new typed array on the same ArrayBuffer store and with the same element types
        /// as for this TypedArray object. The begin offset is inclusive and the end offset is
        /// exclusive.
        /// </summary>
        /// <param name="begin"> Element to begin at. The offset is inclusive. </param>
        /// <param name="end"> Element to end at. The offset is exclusive. If not specified, all
        /// elements from the one specified by begin to the end of the array are included in the
        /// new view. </param>
        [JSInternalFunction(Name = "subarray", Length = 2)]
        public void Subarray(int begin = 0, int end = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a new Array Iterator object that contains the values for each index in the
        /// array.
        /// </summary>
        [JSInternalFunction(Name = "values")]
        public void Values()
        {
            throw new NotImplementedException();
        }



        //     ARRAY WRAPPER FUNCTIONS
        //_________________________________________________________________________________________

        ///// <summary>
        ///// Implements a wrapper for typed arrays.
        ///// </summary>
        //private class TypedArrayWrapper : ArrayWrapper<object>
        //{
        //    /// <summary>
        //    /// Creates a new TypedArrayWrapper instance.
        //    /// </summary>
        //    /// <param name="wrappedInstance"> The typed array that is being wrapped. </param>
        //    public TypedArrayWrapper(TypedArrayInstance wrappedInstance)
        //        : base(wrappedInstance, wrappedInstance.Length)
        //    {
        //    }

        //    /// <summary>
        //    /// Gets or sets an array element within the range 0 .. Length-1 (inclusive).
        //    /// </summary>
        //    /// <param name="index"> The index to get or set. </param>
        //    /// <returns> The value at the given index. </returns>
        //    public override object this[int index]
        //    {
        //        get { return WrappedInstance[index]; }
        //        set { WrappedInstance[index] = value; }
        //    }

        //    /// <summary>
        //    /// Deletes the value at the given array index, throwing an exception on error.
        //    /// </summary>
        //    /// <param name="index"> The array index to delete. </param>
        //    public override void Delete(int index)
        //    {
        //        WrappedInstance.Delete((uint)index, true);
        //    }

        //    /// <summary>
        //    /// Creates a new array of the same type as this one.
        //    /// </summary>
        //    /// <param name="values"> The values in the new array. </param>
        //    /// <returns> A new array object. </returns>
        //    public override ArrayWrapper<object> ConstructArray(object[] values)
        //    {
        //        return new ArrayInstanceWrapper(new TypedArrayInstance(values));
        //    }

        //    /// <summary>
        //    /// Convert an untyped value to a typed value.
        //    /// </summary>
        //    /// <param name="value"> The value to convert. </param>
        //    /// <returns> The value converted to type <typeparamref name="T"/>. </returns>
        //    public override object ConvertValue(object value)
        //    {
        //        return value;
        //    }
        //}

        ///// <summary>
        ///// Copies the sequence of array elements within the array to the position starting at
        ///// target. The copy is taken from the index positions of the second and third arguments
        ///// start and end. The end argument is optional and defaults to the length of the array.
        ///// This method has the same algorithm as Array.prototype.copyWithin.
        ///// </summary>
        ///// <param name="target"> Target start index position where to copy the elements to. </param>
        ///// <param name="start"> Source start index position where to start copying elements from. </param>
        ///// <param name="end"> Optional. Source end index position where to end copying elements from. </param>
        //[JSInternalFunction(Name = "copyWithin", Length = 2)]
        //public void CopyWithin(int target, int start, int end = int.MaxValue)
        //{
        //    return new TypedArrayWrapper(this).CopyWithin(target, start, end);
        //}

        ///// <summary>
        ///// Determines if every element of the array matches criteria defined by the given user-
        ///// defined function.
        ///// </summary>
        ///// <param name="callbackFunction"> A user-defined function that is called for each element in the
        ///// array.  This function is called with three arguments: the value of the element, the
        ///// index of the element, and the array that is being operated on.  The function should
        ///// return <c>true</c> or <c>false</c>. </param>
        ///// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        ///// <returns> <c>true</c> if every element of the array matches criteria defined by the
        ///// given user-defined function; <c>false</c> otherwise. </returns>
        //[JSInternalFunction(Name = "every", Length = 1)]
        //public bool Every(FunctionInstance callbackFunction, ObjectInstance context = null)
        //{
        //    return new TypedArrayWrapper(this).Every(callbackFunction, context);
        //}

        ///// <summary>
        ///// Fills all the elements of a typed array from a start index to an end index with a
        ///// static value.
        ///// </summary>
        ///// <param name="value"> The value to fill the typed array with. </param>
        ///// <param name="start"> Optional. Start index. Defaults to 0. </param>
        ///// <param name="end"> Optional. End index (exclusive). Defaults to the length of the array. </param>
        ///// <returns> The array that is being operated on. </returns>
        //[JSInternalFunction(Name = "fill", Length = 1)]
        //public ObjectInstance Fill(object value, int start = 0, int end = int.MaxValue)
        //{
        //    return new TypedArrayWrapper(this).Fill(value, start, end);
        //}

        ///// <summary>
        ///// Creates a new array with the elements from this array that pass the test implemented by
        ///// the given function.
        ///// </summary>
        ///// <param name="callbackFunction"> A user-defined function that is called for each element in the
        ///// array.  This function is called with three arguments: the value of the element, the
        ///// index of the element, and the array that is being operated on.  The function should
        ///// return <c>true</c> or <c>false</c>. </param>
        ///// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        ///// <returns> A copy of this array but with only those elements which produce <c>true</c>
        ///// when passed to the provided function. </returns>
        //[JSInternalFunction(Name = "filter", Length = 1)]
        //public TypedArrayInstance Filter(FunctionInstance callbackFunction, ObjectInstance context = null)
        //{
        //    return (TypedArrayInstance)new TypedArrayWrapper(this).Filter(callbackFunction, context).WrappedInstance;
        //}

        ///// <summary>
        ///// Returns the first element in the given array that passes the test implemented by the
        ///// given function.
        ///// </summary>
        ///// <param name="callbackFunction"> A user-defined function that is called for each element in the
        ///// array.  This function is called with three arguments: the value of the element, the
        ///// index of the element, and the array that is being operated on.  The function should
        ///// return <c>true</c> or <c>false</c>. </param>
        ///// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        ///// <returns> The first element that results in the callback returning <c>true</c>. </returns>
        //[JSInternalFunction(Name = "find", Length = 1)]
        //public object Find(FunctionInstance callbackFunction, ObjectInstance context = null)
        //{
        //    return new TypedArrayWrapper(this).Find(callbackFunction, context);
        //}

        ///// <summary>
        ///// Returns an index in the typed array, if an element in the typed array satisfies the
        ///// provided testing function. Otherwise -1 is returned.
        ///// </summary>
        ///// <param name="callbackFunction"> A user-defined function that is called for each element in the
        ///// array.  This function is called with three arguments: the value of the element, the
        ///// index of the element, and the array that is being operated on.  The function should
        ///// return <c>true</c> or <c>false</c>. </param>
        ///// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        ///// <returns> The first element that results in the callback returning <c>true</c>. </returns>
        //[JSInternalFunction(Name = "findIndex", Length = 1)]
        //public int FindIndex(FunctionInstance callbackFunction, ObjectInstance context = null)
        //{
        //    return new TypedArrayWrapper(this).FindIndex(callbackFunction, context);
        //}

        ///// <summary>
        ///// Calls the given user-defined function once per element in the array.
        ///// </summary>
        ///// <param name="callbackFunction"> A user-defined function that is called for each element in the
        ///// array.  This function is called with three arguments: the value of the element, the
        ///// index of the element, and the array that is being operated on. </param>
        ///// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        //[JSInternalFunction(Name = "forEach", Length = 1)]
        //public void ForEach(FunctionInstance callbackFunction, ObjectInstance context = null)
        //{
        //    new TypedArrayWrapper(this).ForEach(callbackFunction, context);
        //}

        ///// <summary>
        ///// Returns the index of the given search element in the array, starting from
        ///// <paramref name="fromIndex"/>.
        ///// </summary>
        ///// <param name="searchElement"> The value to search for. </param>
        ///// <param name="fromIndex"> The array index to start searching. </param>
        ///// <returns> The index of the given search element in the array, or <c>-1</c> if the
        ///// element wasn't found. </returns>
        //[JSInternalFunction(Name = "indexOf", Length = 1)]
        //public int IndexOf(object searchElement, int fromIndex = 0)
        //{
        //    return new TypedArrayWrapper(this).IndexOf(searchElement, fromIndex);
        //}

        ///// <summary>
        ///// Concatenates all the elements of the array, using the specified separator between each
        ///// element.  If no separator is provided, a comma is used for this purpose.
        ///// </summary>
        ///// <param name="separator"> The string to use as a separator. </param>
        ///// <returns> A string that consists of the element values separated by the separator string. </returns>
        //[JSInternalFunction(Name = "join", Length = 1)]
        //public string Join(string separator = ",")
        //{
        //    return new TypedArrayWrapper(this).Join(separator);
        //}

        ///// <summary>
        ///// Returns the index of the given search element in the array, searching backwards from
        ///// <paramref name="fromIndex"/>.
        ///// </summary>
        ///// <param name="searchElement"> The value to search for. </param>
        ///// <param name="fromIndex"> The array index to start searching. </param>
        ///// <returns> The index of the given search element in the array, or <c>-1</c> if the
        ///// element wasn't found. </returns>
        //[JSInternalFunction(Name = "lastIndexOf", Length = 1)]
        //public int LastIndexOf(object searchElement, int fromIndex = int.MaxValue)
        //{
        //    return new TypedArrayWrapper(this).LastIndexOf(searchElement, fromIndex);
        //}

        ///// <summary>
        ///// Creates a new array with the results of calling the given function on every element in
        ///// this array.
        ///// </summary>
        ///// <param name="callbackFunction"> A user-defined function that is called for each element
        ///// in the array.  This function is called with three arguments: the value of the element,
        ///// the index of the element, and the array that is being operated on.  The value that is
        ///// returned from this function is stored in the resulting array. </param>
        ///// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        ///// <returns> A new array with the results of calling the given function on every element
        ///// in the array. </returns>
        //[JSInternalFunction(Name = "map", Length = 1)]
        //public TypedArrayInstance Map(FunctionInstance callbackFunction, ObjectInstance context = null)
        //{
        //    return (TypedArrayInstance)new TypedArrayWrapper(this).Map(callbackFunction, context).WrappedInstance;
        //}

        ///// <summary>
        ///// Accumulates a single value by calling a user-defined function for each element.
        ///// </summary>
        ///// <param name="callbackFunction"> A user-defined function that is called for each element
        ///// in the array.  This function is called with four arguments: the current accumulated
        ///// value, the value of the element, the index of the element, and the array that is being
        ///// operated on.  The return value for this function is the new accumulated value and is
        ///// passed to the next invocation of the function. </param>
        ///// <param name="initialValue"> The initial accumulated value. </param>
        ///// <returns> The accumulated value returned from the last invocation of the callback
        ///// function. </returns>
        //[JSInternalFunction(Name = "reduce", Length = 1)]
        //public object Reduce(FunctionInstance callbackFunction, object initialValue = null)
        //{
        //    return new TypedArrayWrapper(this).Reduce(callbackFunction, initialValue);
        //}

        ///// <summary>
        ///// Accumulates a single value by calling a user-defined function for each element
        ///// (starting with the last element in the array).
        ///// </summary>
        ///// <param name="callbackFunction"> A user-defined function that is called for each element
        ///// in the array.  This function is called with four arguments: the current accumulated
        ///// value, the value of the element, the index of the element, and the array that is being
        ///// operated on.  The return value for this function is the new accumulated value and is
        ///// passed to the next invocation of the function. </param>
        ///// <param name="initialValue"> The initial accumulated value. </param>
        ///// <returns> The accumulated value returned from the last invocation of the callback
        ///// function. </returns>
        //[JSInternalFunction(Name = "reduceRight", Length = 1)]
        //public object ReduceRight(FunctionInstance callbackFunction, object initialValue = null)
        //{
        //    return new TypedArrayWrapper(this).ReduceRight(callbackFunction, initialValue);
        //}

        ///// <summary>
        ///// Reverses the order of the elements in the array.
        ///// </summary>
        ///// <returns> The array that is being operated on. </returns>
        //[JSInternalFunction(Name = "reverse", Flags = JSFunctionFlags.MutatesThisObject)]
        //public TypedArrayInstance Reverse()
        //{
        //    return (TypedArrayInstance)new TypedArrayWrapper(this).Reverse().WrappedInstance;
        //}

        ///// <summary>
        ///// Stores multiple values in the typed array, reading input values from a specified array.
        ///// </summary>
        ///// <param name="array"> The array from which to copy values. All values from the source
        ///// array are copied into the target array, unless the length of the source array plus the
        ///// offset exceeds the length of the target array, in which case an exception is thrown. </param>
        ///// <param name="offset"> The offset into the target array at which to begin writing values
        ///// from the source array. If you omit this value, 0 is assumed (that is, the source array
        ///// will overwrite values in the target array starting at index 0). </param>
        //[JSInternalFunction(Name = "set", Length = 1)]
        //public void Set(ObjectInstance array, int offset = 0)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Returns a section of an array.
        ///// </summary>
        ///// <param name="start"> The index of the first element in the section.  If this value is
        ///// negative it is treated as an offset from the end of the array. </param>
        ///// <param name="end"> The index of the element just past the last element in the section.
        ///// If this value is negative it is treated as an offset from the end of the array.  If
        ///// <paramref name="end"/> is less than or equal to <paramref name="start"/> then an empty
        ///// array is returned. </param>
        ///// <returns> A section of an array. </returns>
        //[JSInternalFunction(Name = "slice", Length = 2)]
        //public TypedArrayInstance Slice(int start, int end = int.MaxValue)
        //{
        //    return (TypedArrayInstance)new TypedArrayWrapper(this).Slice(start, end).WrappedInstance;
        //}

        ///// <summary>
        ///// Determines if at least one element of the array matches criteria defined by the given
        ///// user-defined function.
        ///// </summary>
        ///// <param name="callbackFunction"> A user-defined function that is called for each element in the
        ///// array.  This function is called with three arguments: the value of the element, the
        ///// index of the element, and the array that is being operated on.  The function should
        ///// return <c>true</c> or <c>false</c>. </param>
        ///// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        ///// <returns> <c>true</c> if at least one element of the array matches criteria defined by
        ///// the given user-defined function; <c>false</c> otherwise. </returns>
        //[JSInternalFunction(Name = "some", Length = 1)]
        //public bool Some(FunctionInstance callbackFunction, ObjectInstance context = null)
        //{
        //    return new TypedArrayWrapper(this).Some(callbackFunction, context);
        //}

        ///// <summary>
        ///// Sorts the array.
        ///// </summary>
        ///// <param name="comparisonFunction"> A function which determines the order of the
        ///// elements.  This function should return a number less than zero if the first argument is
        ///// less than the second argument, zero if the arguments are equal or a number greater than
        ///// zero if the first argument is greater than Defaults to an ascending ASCII ordering. </param>
        ///// <returns> The array that was sorted. </returns>
        //[JSInternalFunction(Name = "sort", Flags = JSFunctionFlags.MutatesThisObject, Length = 1)]
        //public TypedArrayInstance Sort(FunctionInstance comparisonFunction = null)
        //{
        //    Func<object, object, int> comparer;
        //    if (comparisonFunction == null)
        //    {
        //        // Default comparer.
        //        comparer = (a, b) =>
        //        {
        //            if (a == null && b == null)
        //                return 0;
        //            if (a == null)
        //                return 1;
        //            if (b == null)
        //                return -1;
        //            if (a == Undefined.Value && b == Undefined.Value)
        //                return 0;
        //            if (a == Undefined.Value)
        //                return 1;
        //            if (b == Undefined.Value)
        //                return -1;
        //            return string.Compare(TypeConverter.ToString(a), TypeConverter.ToString(b), StringComparison.Ordinal);
        //        };
        //    }
        //    else
        //    {
        //        // Custom comparer.
        //        comparer = (a, b) =>
        //        {
        //            if (a == null && b == null)
        //                return 0;
        //            if (a == null)
        //                return 1;
        //            if (b == null)
        //                return -1;
        //            if (a == Undefined.Value && b == Undefined.Value)
        //                return 0;
        //            if (a == Undefined.Value)
        //                return 1;
        //            if (b == Undefined.Value)
        //                return -1;
        //            var v = TypeConverter.ToNumber(comparisonFunction.CallFromNative("sort", null, a, b));
        //            if (double.IsNaN(v))
        //                return 0;
        //            return Math.Sign(v);
        //        };
        //    }

        //    return (TypedArrayInstance)new TypedArrayWrapper(this).Sort(comparer).WrappedInstance;
        //}

        ///// <summary>
        ///// Returns a locale-specific string representing this object.
        ///// </summary>
        ///// <returns> A locale-specific string representing this object. </returns>
        //[JSInternalFunction(Name = "toLocaleString")]
        //public new string ToLocaleString()
        //{
        //    return new TypedArrayWrapper(this).ToLocaleString();
        //}

        ///// <summary>
        ///// Returns a string representing this object.
        ///// </summary>
        ///// <param name="thisObj"> The array that is being operated on. </param>
        ///// <returns> A string representing this object. </returns>
        //[JSInternalFunction(Name = "toString", Flags = JSFunctionFlags.HasThisObject)]
        //public static string ToString(ObjectInstance thisObj)
        //{
        //    return new TypedArrayWrapper(this).ToString();
        //}
    }
}
