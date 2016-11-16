using System;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a typed array instance.
    /// </summary>
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

            // From the spec: the initial value of the @@iterator property is the same function
            // object as the initial value of the %TypedArray%.prototype.values property.
            PropertyNameAndValue valuesProperty = properties.Find(p => "values".Equals(p.Key));
            if (valuesProperty == null)
                throw new InvalidOperationException("Expected values property.");
            properties.Add(new PropertyNameAndValue(engine.Symbol.Iterator, valuesProperty.Value, PropertyAttributes.NonEnumerable));

            result.FastSetProperties(properties);
            return result;
        }



        //     .NET PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// The type of each element in the array.
        /// </summary>
        internal TypedArrayType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets or sets the item at the given index.
        /// </summary>
        /// <param name="index"> The array index. </param>
        /// <returns> The value of the item at the given index. </returns>
        public new object this[int index]
        {
            get
            {
                // Out of range indices return undefined.
                if (index < 0 || index >= this.length)
                    return Undefined.Value;
                switch (type)
                {
                    case TypedArrayType.Int8Array:
                        return (int)(sbyte)this.buffer.Buffer[this.byteOffset + index];
                    case TypedArrayType.Uint8Array:
                    case TypedArrayType.Uint8ClampedArray:
                        return (int)this.buffer.Buffer[this.byteOffset + index];
                    case TypedArrayType.Int16Array:
                        return (int)BitConverter.ToInt16(this.buffer.Buffer, this.byteOffset + index * 2);
                    case TypedArrayType.Uint16Array:
                        return (int)BitConverter.ToUInt16(this.buffer.Buffer, this.byteOffset + index * 2);
                    case TypedArrayType.Int32Array:
                        return BitConverter.ToInt32(this.buffer.Buffer, this.byteOffset + index * 4);
                    case TypedArrayType.Uint32Array:
                        return BitConverter.ToUInt32(this.buffer.Buffer, this.byteOffset + index * 4);
                    case TypedArrayType.Float32Array:
                        return (double)BitConverter.ToSingle(this.buffer.Buffer, this.byteOffset + index * 4);
                    case TypedArrayType.Float64Array:
                        return BitConverter.ToDouble(this.buffer.Buffer, this.byteOffset + index * 8);
                    default:
                        throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
                }
            }
            set
            {
                // Out of range indices have no effect.
                if (index < 0 || index >= this.length)
                    return;
                switch (type)
                {
                    case TypedArrayType.Int8Array:
                        this.buffer.Buffer[this.byteOffset + index] = (byte)TypeConverter.ToInt8(value);
                        break;

                    case TypedArrayType.Uint8Array:
                        this.buffer.Buffer[this.byteOffset + index] = TypeConverter.ToUint8(value);
                        break;

                    case TypedArrayType.Uint8ClampedArray:

                        // This algorithm is defined as ToUint8Clamp in the spec.
                        double number = TypeConverter.ToNumber(value);
                        int result;
                        if (number <= 0)
                            result = 0;
                        else if (number >= 255)
                            result = 255;
                        else
                        {
                            var f = Math.Floor(number);
                            if (f + 0.5 < number)
                                result = (int)f + 1;
                            else if (number < f + 0.5)
                                result = (int)f;
                            else if ((int)f % 2 == 0)
                                result = (int)f;
                            else
                                result = (int)f + 1;
                        }
                        this.buffer.Buffer[this.byteOffset + index] = (byte)result;
                        break;

                    case TypedArrayType.Int16Array:
                        Array.Copy(BitConverter.GetBytes(TypeConverter.ToInt16(value)), 0, this.buffer.Buffer, this.byteOffset + index * 2, 2);
                        break;

                    case TypedArrayType.Uint16Array:
                        Array.Copy(BitConverter.GetBytes(TypeConverter.ToUint16(value)), 0, this.buffer.Buffer, this.byteOffset + index * 2, 2);
                        break;

                    case TypedArrayType.Int32Array:
                        Array.Copy(BitConverter.GetBytes(TypeConverter.ToInt32(value)), 0, this.buffer.Buffer, this.byteOffset + index * 4, 4);
                        break;

                    case TypedArrayType.Uint32Array:
                        Array.Copy(BitConverter.GetBytes(TypeConverter.ToUint32(value)), 0, this.buffer.Buffer, this.byteOffset + index * 4, 4);
                        break;

                    case TypedArrayType.Float32Array:
                        Array.Copy(BitConverter.GetBytes((float)TypeConverter.ToNumber(value)), 0, this.buffer.Buffer, this.byteOffset + index * 4, 4);
                        break;

                    case TypedArrayType.Float64Array:
                        Array.Copy(BitConverter.GetBytes(TypeConverter.ToNumber(value)), 0, this.buffer.Buffer, this.byteOffset + index * 8, 8);
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
                }
            }
        }

        /// <summary>
        /// The data storage size, in bytes, of each array element.
        /// </summary>
        private int BytesPerElement
        {
            get
            {
                switch (type)
                {
                    case TypedArrayType.Int8Array:
                    case TypedArrayType.Uint8Array:
                    case TypedArrayType.Uint8ClampedArray:
                        return 1;
                    case TypedArrayType.Int16Array:
                    case TypedArrayType.Uint16Array:
                        return 2;
                    case TypedArrayType.Int32Array:
                    case TypedArrayType.Uint32Array:
                    case TypedArrayType.Float32Array:
                        return 4;
                    case TypedArrayType.Float64Array:
                        return 8;
                    default:
                        throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
                }
            }
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
            get { return this.type.ToString(); }
        }



        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets a descriptor for the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes. </returns>
        /// <remarks> The prototype chain is not searched. </remarks>
        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index)
        {
            return new PropertyDescriptor(this[(int)index], PropertyAttributes.Writable | PropertyAttributes.Enumerable);
        }

        /// <summary>
        /// Sets the value of the property with the given array index.  If a property with the
        /// given index does not exist, or exists in the prototype chain (and is not a setter) then
        /// a new property is created.
        /// </summary>
        /// <param name="index"> The array index of the property to set. </param>
        /// <param name="value"> The value to set the property to.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is read-only or if the object is sealed. </param>
        public override void SetPropertyValue(uint index, object value, bool throwOnError)
        {
            this[(int)index] = value;
        }

        /// <summary>
        /// Deletes the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property to delete. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set because the property was marked as non-configurable.  </param>
        /// <returns> <c>true</c> if the property was successfully deleted, or if the property did
        /// not exist; <c>false</c> if the property was marked as non-configurable and
        /// <paramref name="throwOnError"/> was <c>false</c>. </returns>
        public override bool Delete(uint index, bool throwOnError)
        {
            return false;
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a new array iterator object that contains the key/value pairs for each index in
        /// the array.
        /// </summary>
        /// <returns> An array iterator object that contains the key/value pairs for each index in
        /// the array. </returns>
        [JSInternalFunction(Name = "entries")]
        public ObjectInstance Entries()
        {
            return new ArrayIterator(Engine.ArrayIteratorPrototype, this, ArrayIterator.Kind.KeyAndValue);
        }

        /// <summary>
        /// Returns a new array iterator object that contains the keys for each index in the array.
        /// </summary>
        /// <returns> An array iterator object that contains the keys for each index in the array. </returns>
        [JSInternalFunction(Name = "keys")]
        public ObjectInstance Keys()
        {
            return new ArrayIterator(Engine.ArrayIteratorPrototype, this, ArrayIterator.Kind.Key);
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
        /// <returns> A new typed array that shares the same ArrayBuffer store. </returns>
        [JSInternalFunction(Name = "subarray")]
        public TypedArrayInstance Subarray(int begin = 0, int end = int.MaxValue)
        {
            // Constrain the input parameters to valid values.
            begin = begin < 0 ? Math.Max(this.Length + begin, 0) : Math.Min(begin, this.Length);
            end = end < 0 ? Math.Max(this.Length + end, 0) : Math.Min(end, this.Length);
            int newLength = Math.Max(end - begin, 0);

            // Create a new typed array that uses the same ArrayBuffer.
            return new TypedArrayInstance(this.Prototype, this.type, this.Buffer, this.ByteOffset + begin * BytesPerElement, newLength);
        }

        /// <summary>
        /// Returns a new array iterator object that contains the values for each index in the
        /// array.
        /// </summary>
        /// <returns> An array iterator object that contains the values for each index in the
        /// array. </returns>
        [JSInternalFunction(Name = "values")]
        public ObjectInstance Values()
        {
            return new ArrayIterator(Engine.ArrayIteratorPrototype, this, ArrayIterator.Kind.Value);
        }



        //     ARRAY ADAPTER FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Implements an adapter for typed arrays.
        /// </summary>
        private class TypedArrayAdapter : ArrayAdapter<object>
        {
            /// <summary>
            /// Creates a new TypedArrayWrapper instance.
            /// </summary>
            /// <param name="wrappedInstance"> The typed array that is being wrapped. </param>
            public TypedArrayAdapter(TypedArrayInstance wrappedInstance)
                : base(wrappedInstance, wrappedInstance.Length)
            {
            }

            /// <summary>
            /// Gets or sets an array element within the range 0 .. Length-1 (inclusive).
            /// </summary>
            /// <param name="index"> The index to get or set. </param>
            /// <returns> The value at the given index. </returns>
            public override object this[int index]
            {
                get { return ((TypedArrayInstance)WrappedInstance)[index]; }
                set { ((TypedArrayInstance)WrappedInstance)[index] = value; }
            }

            /// <summary>
            /// Deletes the value at the given array index, throwing an exception on error.
            /// </summary>
            /// <param name="index"> The array index to delete. </param>
            public override void Delete(int index)
            {
                WrappedInstance.Delete((uint)index, true);
            }

            /// <summary>
            /// Creates a new array of the same type as this one.
            /// </summary>
            /// <param name="values"> The values in the new array. </param>
            /// <returns> A new array object. </returns>
            public override ObjectInstance ConstructArray(object[] values)
            {
                var typedArray = (TypedArrayInstance)WrappedInstance;
                switch (typedArray.Type)
                {
                    case TypedArrayType.Int8Array:
                        return typedArray.Engine.Int8Array.From(values);
                    case TypedArrayType.Uint8Array:
                        return typedArray.Engine.Uint8Array.From(values);
                    case TypedArrayType.Uint8ClampedArray:
                        return typedArray.Engine.Uint8ClampedArray.From(values);
                    case TypedArrayType.Int16Array:
                        return typedArray.Engine.Int16Array.From(values);
                    case TypedArrayType.Uint16Array:
                        return typedArray.Engine.Uint16Array.From(values);
                    case TypedArrayType.Int32Array:
                        return typedArray.Engine.Int32Array.From(values);
                    case TypedArrayType.Uint32Array:
                        return typedArray.Engine.Uint32Array.From(values);
                    case TypedArrayType.Float32Array:
                        return typedArray.Engine.Float32Array.From(values);
                    case TypedArrayType.Float64Array:
                        return typedArray.Engine.Float64Array.From(values);
                    default:
                        throw new NotSupportedException($"Unsupported TypedArray '{typedArray.Type}'.");
                }
            }

            /// <summary>
            /// Convert an untyped value to a typed value.
            /// </summary>
            /// <param name="value"> The value to convert. </param>
            /// <returns> The value converted to type Object. </returns>
            public override object ConvertValue(object value)
            {
                return value;
            }
        }

        /// <summary>
        /// Copies the sequence of array elements within the array to the position starting at
        /// target. The copy is taken from the index positions of the second and third arguments
        /// start and end. The end argument is optional and defaults to the length of the array.
        /// This method has the same algorithm as Array.prototype.copyWithin.
        /// </summary>
        /// <param name="target"> Target start index position where to copy the elements to. </param>
        /// <param name="start"> Source start index position where to start copying elements from. </param>
        /// <param name="end"> Optional. Source end index position where to end copying elements from. </param>
        /// <returns> The array that was operated on. </returns>
        [JSInternalFunction(Name = "copyWithin", Length = 2)]
        public ObjectInstance CopyWithin(int target, int start, int end = int.MaxValue)
        {
            return new TypedArrayAdapter(this).CopyWithin(target, start, end);
        }

        /// <summary>
        /// Determines if every element of the array matches criteria defined by the given user-
        /// defined function.
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> <c>true</c> if every element of the array matches criteria defined by the
        /// given user-defined function; <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "every", Length = 1)]
        public bool Every(FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new TypedArrayAdapter(this).Every(callbackFunction, context);
        }

        /// <summary>
        /// Fills all the elements of a typed array from a start index to an end index with a
        /// static value.
        /// </summary>
        /// <param name="value"> The value to fill the typed array with. </param>
        /// <param name="start"> Optional. Start index. Defaults to 0. </param>
        /// <param name="end"> Optional. End index (exclusive). Defaults to the length of the array. </param>
        /// <returns> The array that is being operated on. </returns>
        [JSInternalFunction(Name = "fill", Length = 1)]
        public ObjectInstance Fill(object value, int start = 0, int end = int.MaxValue)
        {
            return new TypedArrayAdapter(this).Fill(value, start, end);
        }

        /// <summary>
        /// Creates a new array with the elements from this array that pass the test implemented by
        /// the given function.
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> A copy of this array but with only those elements which produce <c>true</c>
        /// when passed to the provided function. </returns>
        [JSInternalFunction(Name = "filter", Length = 1)]
        public TypedArrayInstance Filter(FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return (TypedArrayInstance)new TypedArrayAdapter(this).Filter(callbackFunction, context);
        }

        /// <summary>
        /// Returns the first element in the given array that passes the test implemented by the
        /// given function.
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> The first element that results in the callback returning <c>true</c>. </returns>
        [JSInternalFunction(Name = "find", Length = 1)]
        public object Find(FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new TypedArrayAdapter(this).Find(callbackFunction, context);
        }

        /// <summary>
        /// Returns an index in the typed array, if an element in the typed array satisfies the
        /// provided testing function. Otherwise -1 is returned.
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> The first element that results in the callback returning <c>true</c>. </returns>
        [JSInternalFunction(Name = "findIndex", Length = 1)]
        public int FindIndex(FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new TypedArrayAdapter(this).FindIndex(callbackFunction, context);
        }

        /// <summary>
        /// Calls the given user-defined function once per element in the array.
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        [JSInternalFunction(Name = "forEach", Length = 1)]
        public void ForEach(FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            new TypedArrayAdapter(this).ForEach(callbackFunction, context);
        }

        /// <summary>
        /// Returns the index of the given search element in the array, starting from
        /// <paramref name="fromIndex"/>.
        /// </summary>
        /// <param name="searchElement"> The value to search for. </param>
        /// <param name="fromIndex"> The array index to start searching. </param>
        /// <returns> The index of the given search element in the array, or <c>-1</c> if the
        /// element wasn't found. </returns>
        [JSInternalFunction(Name = "indexOf", Length = 1)]
        public int IndexOf(object searchElement, int fromIndex = 0)
        {
            return new TypedArrayAdapter(this).IndexOf(searchElement, fromIndex);
        }

        /// <summary>
        /// Concatenates all the elements of the array, using the specified separator between each
        /// element.  If no separator is provided, a comma is used for this purpose.
        /// </summary>
        /// <param name="separator"> The string to use as a separator. </param>
        /// <returns> A string that consists of the element values separated by the separator string. </returns>
        [JSInternalFunction(Name = "join", Length = 1)]
        public string Join(string separator = ",")
        {
            return new TypedArrayAdapter(this).Join(separator);
        }

        /// <summary>
        /// Returns the index of the given search element in the array, searching backwards from
        /// the end of the array.
        /// </summary>
        /// <param name="searchElement"> The value to search for. </param>
        /// <returns> The index of the given search element in the array, or <c>-1</c> if the
        /// element wasn't found. </returns>
        [JSInternalFunction(Name = "lastIndexOf", Length = 1)]
        public int LastIndexOf(object searchElement)
        {
            return LastIndexOf(searchElement, int.MaxValue);
        }

        /// <summary>
        /// Returns the index of the given search element in the array, searching backwards from
        /// <paramref name="fromIndex"/>.
        /// </summary>
        /// <param name="searchElement"> The value to search for. </param>
        /// <param name="fromIndex"> The array index to start searching. </param>
        /// <returns> The index of the given search element in the array, or <c>-1</c> if the
        /// element wasn't found. </returns>
        [JSInternalFunction(Name = "lastIndexOf", Length = 1)]
        public int LastIndexOf(object searchElement, int fromIndex)
        {
            return new TypedArrayAdapter(this).LastIndexOf(searchElement, fromIndex);
        }

        /// <summary>
        /// Creates a new array with the results of calling the given function on every element in
        /// this array.
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element
        /// in the array.  This function is called with three arguments: the value of the element,
        /// the index of the element, and the array that is being operated on.  The value that is
        /// returned from this function is stored in the resulting array. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> A new array with the results of calling the given function on every element
        /// in the array. </returns>
        [JSInternalFunction(Name = "map", Length = 1)]
        public TypedArrayInstance Map(FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return (TypedArrayInstance)new TypedArrayAdapter(this).Map(callbackFunction, context);
        }

        /// <summary>
        /// Accumulates a single value by calling a user-defined function for each element.
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element
        /// in the array.  This function is called with four arguments: the current accumulated
        /// value, the value of the element, the index of the element, and the array that is being
        /// operated on.  The return value for this function is the new accumulated value and is
        /// passed to the next invocation of the function. </param>
        /// <param name="initialValue"> The initial accumulated value. </param>
        /// <returns> The accumulated value returned from the last invocation of the callback
        /// function. </returns>
        [JSInternalFunction(Name = "reduce", Length = 1)]
        public object Reduce(FunctionInstance callbackFunction, object initialValue = null)
        {
            return new TypedArrayAdapter(this).Reduce(callbackFunction, initialValue);
        }

        /// <summary>
        /// Accumulates a single value by calling a user-defined function for each element
        /// (starting with the last element in the array).
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element
        /// in the array.  This function is called with four arguments: the current accumulated
        /// value, the value of the element, the index of the element, and the array that is being
        /// operated on.  The return value for this function is the new accumulated value and is
        /// passed to the next invocation of the function. </param>
        /// <param name="initialValue"> The initial accumulated value. </param>
        /// <returns> The accumulated value returned from the last invocation of the callback
        /// function. </returns>
        [JSInternalFunction(Name = "reduceRight", Length = 1)]
        public object ReduceRight(FunctionInstance callbackFunction, object initialValue = null)
        {
            return new TypedArrayAdapter(this).ReduceRight(callbackFunction, initialValue);
        }

        /// <summary>
        /// Reverses the order of the elements in the array.
        /// </summary>
        /// <returns> The array that is being operated on. </returns>
        [JSInternalFunction(Name = "reverse", Flags = JSFunctionFlags.MutatesThisObject)]
        public TypedArrayInstance Reverse()
        {
            return (TypedArrayInstance)new TypedArrayAdapter(this).Reverse();
        }

        /// <summary>
        /// Stores multiple values in the typed array, reading input values from a specified array.
        /// </summary>
        /// <param name="array"> The array from which to copy values. All values from the source
        /// array are copied into the target array, unless the length of the source array plus the
        /// offset exceeds the length of the target array, in which case an exception is thrown. </param>
        /// <param name="offset"> The offset into the target array at which to begin writing values
        /// from the source array. If you omit this value, 0 is assumed (that is, the source array
        /// will overwrite values in the target array starting at index 0). </param>
        [JSInternalFunction(Name = "set", Length = 1)]
        public void Set(ObjectInstance array, int offset = 0)
        {
            if (offset < 0)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Start offset cannot be negative");
            if (array is TypedArrayInstance)
            {
                var typedArray = (TypedArrayInstance)array;
                if (typedArray.Length + offset > this.Length)
                    throw new JavaScriptException(Engine, ErrorType.RangeError, "Source array is too large");
                if (this.Buffer == typedArray.Buffer && this.ByteOffset + offset * BytesPerElement > typedArray.ByteOffset)
                {
                    // Copy in the opposite direction.
                    for (int i = typedArray.Length - 1; i >= 0; i--)
                    {
                        this[offset + i] = typedArray[i];
                    }
                }
                else
                {
                    // Copy in the normal direction.
                    for (int i = 0; i < typedArray.Length; i++)
                    {
                        this[offset + i] = typedArray[i];
                    }
                }
            }
            else
            {
                int arrayLength = TypeConverter.ToInteger(array["length"]);
                if (arrayLength + offset > this.Length)
                    throw new JavaScriptException(Engine, ErrorType.RangeError, "Source array is too large");
                for (int i = 0; i < arrayLength; i ++)
                {
                    this[offset + i] = array[i];
                }
            }
        }

        /// <summary>
        /// Returns a section of an array.
        /// </summary>
        /// <param name="start"> The index of the first element in the section.  If this value is
        /// negative it is treated as an offset from the end of the array. </param>
        /// <param name="end"> The index of the element just past the last element in the section.
        /// If this value is negative it is treated as an offset from the end of the array.  If
        /// <paramref name="end"/> is less than or equal to <paramref name="start"/> then an empty
        /// array is returned. </param>
        /// <returns> A section of an array. </returns>
        [JSInternalFunction(Name = "slice", Length = 2)]
        public TypedArrayInstance Slice(int start, int end = int.MaxValue)
        {
            return (TypedArrayInstance)new TypedArrayAdapter(this).Slice(start, end);
        }

        /// <summary>
        /// Determines if at least one element of the array matches criteria defined by the given
        /// user-defined function.
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> <c>true</c> if at least one element of the array matches criteria defined by
        /// the given user-defined function; <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "some", Length = 1)]
        public bool Some(FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new TypedArrayAdapter(this).Some(callbackFunction, context);
        }

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="comparisonFunction"> A function which determines the order of the
        /// elements.  This function should return a number less than zero if the first argument is
        /// less than the second argument, zero if the arguments are equal or a number greater than
        /// zero if the first argument is greater than Defaults to an ascending ASCII ordering. </param>
        /// <returns> The array that was sorted. </returns>
        [JSInternalFunction(Name = "sort", Flags = JSFunctionFlags.MutatesThisObject, Length = 1)]
        public TypedArrayInstance Sort(FunctionInstance comparisonFunction = null)
        {
            Func<object, object, int> comparer;
            if (comparisonFunction == null)
            {
                // Default comparer.
                comparer = (a, b) =>
                {
                    double x = TypeConverter.ToNumber(a);
                    double y = TypeConverter.ToNumber(b);
                    if (x < y)
                        return -1;
                    if (x > y)
                        return 1;
                    if (double.IsNaN(x) && double.IsNaN(y))
                        return 0;
                    if (double.IsNaN(x))
                        return 1;
                    if (double.IsNaN(y))
                        return -1;
                    if (TypeUtilities.IsNegativeZero(x) && TypeUtilities.IsPositiveZero(y))
                        return -1;
                    if (TypeUtilities.IsPositiveZero(x) && TypeUtilities.IsNegativeZero(y))
                        return 1;
                    return 0;
                };
            }
            else
            {
                // Custom comparer.
                comparer = (a, b) =>
                {
                    var v = TypeConverter.ToNumber(comparisonFunction.CallFromNative("sort", null, a, b));
                    if (double.IsNaN(v))
                        return 0;
                    return Math.Sign(v);
                };
            }

            return (TypedArrayInstance)new TypedArrayAdapter(this).Sort(comparer);
        }

        /// <summary>
        /// Returns a locale-specific string representing this object.
        /// </summary>
        /// <returns> A locale-specific string representing this object. </returns>
        [JSInternalFunction(Name = "toLocaleString")]
        public new string ToLocaleString()
        {
            return new TypedArrayAdapter(this).ToLocaleString();
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> A string representing this object. </returns>
        [JSInternalFunction(Name = "toString", Flags = JSFunctionFlags.HasThisObject)]
        public static string ToString(ObjectInstance thisObj)
        {
            if (!(thisObj is TypedArrayInstance))
                throw new JavaScriptException(thisObj.Engine, ErrorType.TypeError, "This function is not generic.");
            return new TypedArrayAdapter((TypedArrayInstance)thisObj).ToString();
        }
    }
}
