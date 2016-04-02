using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the JavaScript Array object.
    /// </summary>
    [Serializable]
    public partial class ArrayInstance : ObjectInstance
    {
        // The array, if it is dense.
        private object[] dense;

        // The dense array might have holes within the range 0..length-1.
        private bool denseMayContainHoles = false;

        // The array, if it is sparse.
        private SparseArray sparse;

        // The length of the array.
        private uint length;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new array with the given length and capacity.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="length"> The initial value of the length property. </param>
        /// <param name="capacity"> The number of elements to allocate. </param>
        internal ArrayInstance(ObjectInstance prototype, uint length, uint capacity)
            : base(prototype)
        {
            if (length > capacity)
                throw new ArgumentOutOfRangeException("length", "length must be less than or equal to capacity.");
            if (length <= 1000)
            {
                this.dense = new object[(int)capacity];
                this.denseMayContainHoles = length > 0;
            }
            else
            {
                this.sparse = new SparseArray();
            }

            // Create a fake property for length plus initialize the real length property.
            this.length = length;
            FastSetProperty("length", -1, PropertyAttributes.Writable | PropertyAttributes.IsLengthProperty);
        }

        /// <summary>
        /// Creates a new array and initializes it with the given array.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="elements"> The initial values in the array. </param>
        internal ArrayInstance(ObjectInstance prototype, object[] elements)
            : base(prototype)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");
            this.dense = elements;
            
            this.denseMayContainHoles = Array.IndexOf(elements, null) >= 0;

            // Create a fake property for length plus initialize the real length property.
            this.length = (uint)elements.Length;
            FastSetProperty("length", -1, PropertyAttributes.Writable | PropertyAttributes.IsLengthProperty);
        }

        /// <summary>
        /// Creates a new array and initializes it with the given sparse array.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="sparseArray"> The sparse array to use as the backing store. </param>
        /// <param name="length"> The initial value of the length property. </param>
        private ArrayInstance(ObjectInstance prototype, SparseArray sparseArray, uint length)
            : base(prototype)
        {
            if (sparseArray == null)
                throw new ArgumentNullException("sparseArray");
            this.sparse = sparseArray;

            // Create a fake property for length plus initialize the real length property.
            this.length = length;
            FastSetProperty("length", -1, PropertyAttributes.Writable | PropertyAttributes.IsLengthProperty);
        }

        /// <summary>
        /// Creates the Array prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, ArrayConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            result.FastSetProperties(properties);
            return result;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets or sets the number of elements in the array.  Equivalent to the javascript
        /// Array.prototype.length property.
        /// </summary>
        public uint Length
        {
            get { return this.length; }
            set
            {
                uint previousLength = this.length;
                this.length = value;

                if (this.dense != null)
                {
                    if (this.length < this.dense.Length / 2 && this.dense.Length > 100)
                    {
                        // Shrink the array.
                        ResizeDenseArray(this.length + 10, this.length);
                    }
                    else if (this.length > this.dense.Length + 10)
                    {
                        // Switch to a sparse array.
                        this.sparse = SparseArray.FromDenseArray(this.dense, (int)previousLength);
                        this.dense = null;
                    }
                    else if (this.length > this.dense.Length)
                    {
                        // Enlarge the array.
                        ResizeDenseArray(this.length + 10, previousLength);
                        this.denseMayContainHoles = true;
                    }
                    else if (this.length > previousLength)
                    {
                        // Increasing the length property creates an array with holes.
                        this.denseMayContainHoles = true;

                        // Remove all the elements with indices >= length.
                        Array.Clear(this.dense, (int)previousLength, (int)(this.length - previousLength));
                    }
                }
                else
                {
                    // Remove all the elements with indices >= length.
                    if (this.length < previousLength)
                        this.sparse.DeleteRange(this.length, previousLength - this.length);
                }
            }
        }


        /// <summary>
        /// Gets an enumerable list of the defined element values stored in this array.
        /// </summary>
        public IEnumerable<object> ElementValues
        {
            get
            {
                // Enumerate the dense values.
                if (this.dense != null)
                {
                    for (uint i = 0; i < this.dense.Length; i++)
                    {
                        object elementValue = this.dense[i];
                        if (elementValue != null)
                            yield return elementValue;
                    }
                }
                else
                {
                    // Enumerate the sparse values.
                    foreach (var range in this.sparse.Ranges)
                    {
                        for (int i = 0; i < range.Length; i++)
                        {
                            object elementValue = range.Array[i];
                            if (elementValue != null)
                                yield return elementValue;
                        }
                    }
                }
            }
        }



        //     INTERNAL HELPER METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Attempts to parse a string into a valid array index.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> The array index value, or <c>uint.MaxValue</c> if the property name does not reference
        /// an array index. </returns>
        internal static uint ParseArrayIndex(object key)
        {
            if (key is SymbolInstance)
                return uint.MaxValue;
            var propertyName = (string)key;
            if (propertyName.Length == 0)
                return uint.MaxValue;
            int digit = propertyName[0] - '0';
            if (digit < 0 || digit > 9)
                return uint.MaxValue;
            uint result = (uint)digit;
            for (int i = 1; i < propertyName.Length; i++)
            {
                digit = propertyName[i] - '0';
                if (digit < 0 || digit > 9)
                    return uint.MaxValue;
                if (result > 429496728)
                {
                    // Largest number 429496728 * 10 + 9 = 4294967289
                    // Largest number 429496729 * 10 + 9 = 4294967299
                    // Largest valid array index is 4294967294
                    if (result != 429496729 || digit > 4)
                        return uint.MaxValue;
                }
                result = result * 10 + (uint)digit;
            }
            return result;
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
            if (this.dense != null)
            {
                // The array is dense and therefore has at least "length" elements.
                if (index < this.length)
                    return new PropertyDescriptor(this.dense[index], PropertyAttributes.FullAccess);
                return PropertyDescriptor.Undefined;
            }

            // The array is sparse and therefore has "holes".
            return new PropertyDescriptor(this.sparse[index], PropertyAttributes.FullAccess);
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
            value = value ?? Undefined.Value;
            if (this.dense != null)
            {
                if (index < this.length)
                {
                    // The index is inside the existing bounds of the array.
                    this.dense[index] = value;
                }
                else if (index < this.dense.Length)
                {
                    // The index is outside the bounds of the array but inside the allocated buffer.
                    this.dense[index] = value;
                    this.denseMayContainHoles = this.denseMayContainHoles || index > this.length;
                    this.length = index + 1;
                }
                else
                {
                    // The index is out of range - either enlarge the array or switch to sparse.
                    if (index < this.dense.Length + 10)
                    {
                        // Enlarge the dense array.
                        ResizeDenseArray((uint)(this.dense.Length * 2 + 10), this.length);

                        // Set the value.
                        this.dense[index] = value;
                        this.denseMayContainHoles = this.denseMayContainHoles || index > this.length;
                    }
                    else
                    {
                        // Switch to a sparse array.
                        this.sparse = SparseArray.FromDenseArray(this.dense, (int)this.length);
                        this.dense = null;
                        this.sparse[index] = value;
                    }

                    // Update the length.
                    this.length = index + 1;
                }
            }
            else
            {
                // Set the value and update the length.
                this.sparse[index] = value;
                this.length = Math.Max(this.length, index + 1);
            }
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
            if (this.dense != null)
            {
                this.dense[index] = null;
                this.denseMayContainHoles = true;
            }
            else
                this.sparse.Delete(index);
            return true;
        }

        /// <summary>
        /// Defines or redefines the value and attributes of a property.  The prototype chain is
        /// not searched so if the property exists but only in the prototype chain a new property
        /// will be created.
        /// </summary>
        /// <param name="key"> The property key of the property to modify. </param>
        /// <param name="descriptor"> The property value and attributes. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is not configurable or the object is sealed. </param>
        /// <returns> <c>true</c> if the property was successfully modified; <c>false</c> otherwise. </returns>
        public override bool DefineProperty(object key, PropertyDescriptor descriptor, bool throwOnError)
        {
            // Make sure the property name isn't actually an array index.
            uint arrayIndex = ParseArrayIndex(key);
            if (arrayIndex != uint.MaxValue)
            {
                // Spec violation: array elements are never accessor properties.
                if (descriptor.IsAccessor == true)
                {
                    if (throwOnError == true)
                        throw new JavaScriptException(this.Engine, ErrorType.TypeError, "Accessors are not supported for array elements.");
                    return false;
                }

                // Spec violation: array elements are always full access.
                if (descriptor.Attributes != PropertyAttributes.FullAccess)
                {
                    if (throwOnError == true)
                        throw new JavaScriptException(this.Engine, ErrorType.TypeError, "Non-accessible array elements are not supported.");
                    return false;
                }

                // The only thing that is supported is setting the value.
                object value = descriptor.Value ?? Undefined.Value;
                if (this.dense != null)
                    this.dense[arrayIndex] = value;
                else
                    this.sparse[arrayIndex] = value;
                return true;
            }

            // Delegate to the base class.
            return base.DefineProperty(key, descriptor, throwOnError);
        }

        /// <summary>
        /// Gets an enumerable list of every property name and value associated with this object.
        /// </summary>
        public override IEnumerable<PropertyNameAndValue> Properties
        {
            get
            {
                if (this.dense != null)
                {
                    // Enumerate dense array indices.
                    for (uint i = 0; i < this.dense.Length; i++)
                    {
                        object arrayElementValue = this.dense[i];
                        if (arrayElementValue != null)
                            yield return new PropertyNameAndValue(i.ToString(), arrayElementValue, PropertyAttributes.FullAccess);
                    }
                }
                else
                {
                    // Enumerate sparse array indices.
                    foreach (var range in this.sparse.Ranges)
                        for (uint i = range.StartIndex; i < range.StartIndex + range.Array.Length; i++)
                        {
                            object arrayElementValue = this.sparse[i];
                            if (arrayElementValue != null)
                                yield return new PropertyNameAndValue(i.ToString(), arrayElementValue, PropertyAttributes.FullAccess);
                        }
                }

                // Delegate to the base implementation.
                foreach (var nameAndValue in base.Properties)
                    yield return nameAndValue;
            }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a new array consisting of the values of this array plus any number of
        /// additional items.  Values are appended from left to right.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="items"> Any number of items to append. </param>
        /// <returns> A new array consisting of the values of this array plus any number of
        /// additional items. </returns>
        [JSInternalFunction(Name = "concat", Flags = JSFunctionFlags.HasThisObject)]
        public static ArrayInstance Concat(ObjectInstance thisObj, params object[] items)
        {
            // Create a new items array with the thisObject at the beginning.
            var temp = new object[items.Length + 1];
            temp[0] = thisObj;
            Array.Copy(items, 0, temp, 1, items.Length);
            items = temp;

            // Determine if the resulting array should be dense or sparse and calculate the length
            // at the same time.
            bool dense = true;
            uint length = (uint)items.Length;
            foreach (object item in items)
                if (item is ArrayInstance)
                {
                    length += ((ArrayInstance)item).Length - 1;
                    if (((ArrayInstance)item).dense == null)
                        dense = false;
                }

            // This method only supports arrays of length up to 2^31-1, rather than 2^32-1.
            if (length > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, ErrorType.RangeError, "The resulting array is too long");

            if (dense == true)
            {
                // Create a dense array.
                var result = new object[length];

                int index = 0;
                foreach (object item in items)
                {
                    if (item is ArrayInstance)
                    {
                        // Add the items in the array to the end of the resulting array.
                        var array = (ArrayInstance)item;
                        Array.Copy(array.dense, 0, result, index, (int)array.Length);
                        if (array.denseMayContainHoles == true && array.Prototype != null)
                        {
                            // Populate holes from the prototype.
                            for (uint i = 0; i < array.length; i++)
                                if (array.dense[i] == null)
                                    result[index + i] = array.Prototype.GetPropertyValue(i);
                        }
                        index += (int)array.Length;
                    }
                    else
                    {
                        // Add the item to the end of the resulting array.
                        result[index ++] = item;
                    }
                }

                // Return the new dense array.
                return new ArrayInstance(thisObj.Engine.Array.InstancePrototype, result);
            }
            else
            {
                // Create a new sparse array.
                var result = new SparseArray();

                int index = 0;
                foreach (object item in items)
                {
                    if (item is ArrayInstance)
                    {
                        // Add the items in the array to the end of the resulting array.
                        var array = (ArrayInstance)item;
                        if (array.dense != null)
                        {
                            result.CopyTo(array.dense, (uint)index, (int)array.Length);
                            if (array.Prototype != null)
                            {
                                // Populate holes from the prototype.
                                for (uint i = 0; i < array.length; i++)
                                    if (array.dense[i] == null)
                                        result[(uint)index + i] = array.Prototype.GetPropertyValue(i);
                            }
                        }
                        else
                        {
                            result.CopyTo(array.sparse, (uint)index);
                            if (array.Prototype != null)
                            {
                                // Populate holes from the prototype.
                                for (uint i = 0; i < array.Length; i++)
                                    if (array.sparse[i] == null)
                                        result[(uint)index + i] = array.Prototype.GetPropertyValue(i);
                            }
                        }
                        index += (int)array.Length;
                    }
                    else
                    {
                        // Add the item to the end of the resulting array.
                        result[(uint)index] = item;
                        index++;
                    }
                }

                // Return the new sparse array.
                return new ArrayInstance(thisObj.Engine.Array.InstancePrototype, result, length);
            }
        }

        /// <summary>
        /// Removes the last element from the array and returns it.
        /// </summary>
        /// <param name="thisObj"> The array to operate on. </param>
        /// <returns> The last element from the array. </returns>
        [JSInternalFunction(Name = "pop", Flags = JSFunctionFlags.HasThisObject | JSFunctionFlags.MutatesThisObject)]
        public static object Pop(ObjectInstance thisObj)
        {
            // If the "this" object is an array, use the fast version of this method.
            if (thisObj is ArrayInstance)
                return ((ArrayInstance)thisObj).Pop();

            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // Return undefined if the array is empty.
            if (arrayLength == 0)
            {
                SetLength(thisObj, 0);
                return Undefined.Value;
            }

            // Decrement the array length.
            arrayLength -= 1;

            // Get the last element of the array.
            var lastElement = thisObj[arrayLength];

            // Remove the last element from the array.
            thisObj.Delete(arrayLength, true);

            // Update the length.
            SetLength(thisObj, arrayLength);

            // Return the last element.
            return lastElement;
        }

        /// <summary>
        /// Removes the last element from the array and returns it.
        /// </summary>
        /// <returns> The last element from the array. </returns>
        public object Pop()
        {
            // Return undefined if the array is empty.
            if (this.length == 0)
                return Undefined.Value;

            // Decrement the array length.
            this.length -= 1;

            if (this.dense != null)
            {
                // Get the last value.
                var result = this.dense[this.length];

                // If the element does not exist in this array, it may exist in the prototype.
                if (result == null && this.Prototype != null)
                    result = this.Prototype.GetPropertyValue(this.length);

                // Delete it from the array.
                this.dense[this.length] = null;

                // Check if the array should be shrunk.
                if (this.length < this.dense.Length / 2 && this.length > 10)
                    ResizeDenseArray((uint)(this.dense.Length / 2 + 10), this.length);

                // Return the last value.
                return result;
            }
            else
            {
                // Get the last value.
                var result = this.sparse[this.length];

                // If the element does not exist in this array, it may exist in the prototype.
                if (result == null && this.Prototype != null)
                    result = this.Prototype.GetPropertyValue(this.length);

                // Delete it from the array.
                this.sparse.Delete(this.length);

                // Return the last value.
                return result;
            }
        }

        /// <summary>
        /// Appends one or more elements to the end of the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="items"> The items to append to the array. </param>
        [JSInternalFunction(Name = "push", Flags = JSFunctionFlags.HasThisObject | JSFunctionFlags.MutatesThisObject)]
        public static double Push(ObjectInstance thisObj, params object[] items)
        {
            // If the "this" object is an array, use the fast version of this method.
            if (thisObj is ArrayInstance && items.Length == 1)
                return ((ArrayInstance)thisObj).Push(items[0]);

            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            if (arrayLength > uint.MaxValue - items.Length)
            {
                // Even though attempting to push more items than can fit in the array raises an
                // error, the items are still pushed correctly (but the length is stuck at the
                // maximum).
                double arrayLength2 = arrayLength;
                for (int i = 0; i < items.Length; i++)
                {
                    // Append the new item to the array.
                    thisObj.SetPropertyValue((arrayLength2++).ToString(), items[i], true);
                }
                SetLength(thisObj, uint.MaxValue);
                throw new JavaScriptException(thisObj.Engine, ErrorType.RangeError, "Invalid array length");
            }

            // For each item to append.
            for (int i = 0; i < items.Length; i++)
            {
                // Append the new item to the array.
                thisObj.SetPropertyValue(arrayLength ++, items[i], true);
            }

            // Update the length property.
            SetLength(thisObj, arrayLength);

            // Return the new length.
            return (double)arrayLength;
        }

        /// <summary>
        /// Appends one element to the end of the array.
        /// </summary>
        /// <param name="item"> The item to append to the array. </param>
        public int Push(object item)
        {
            if (this.length == uint.MaxValue)
            {
                // Even though attempting to push more items than can fit in the array raises an
                // error, the items are still pushed correctly (but the length is stuck at the
                // maximum).
                SetPropertyValue(this.length.ToString(), item, false);
                throw new JavaScriptException(this.Engine, ErrorType.RangeError, "Invalid array length");
            }

            if (this.dense != null)
            {
                // Check if we need to enlarge the array.
                if (this.length == this.dense.Length)
                    ResizeDenseArray(this.length * 2 + 10, this.length);

                // Append the new item to the array.
                this.dense[this.length++] = item;
            }
            else
            {
                // Append the new item to the array.
                this.sparse[this.length++] = item;
            }

            // Return the new length.
            return (int)this.length;
        }

        /// <summary>
        /// The first element in the array is removed from the array and returned.  All the other
        /// elements are shifted down in the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> The first element in the array. </returns>
        [JSInternalFunction(Name = "shift", Flags = JSFunctionFlags.HasThisObject | JSFunctionFlags.MutatesThisObject)]
        public static object Shift(ObjectInstance thisObj)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // Return undefined if the array is empty.
            if (arrayLength == 0)
            {
                SetLength(thisObj, 0);
                return Undefined.Value;
            }

            // Get the first element.
            var firstElement = thisObj[0];

            // Shift all the other elements down one place.
            for (uint i = 1; i < arrayLength; i++)
            {
                object elementValue = thisObj[i];
                if (elementValue != null)
                    thisObj[i - 1] = elementValue;
                else
                    thisObj.Delete(i - 1, true);
            }

            // Delete the last element.
            thisObj.Delete(arrayLength - 1, true);

            // Update the length property.
            SetLength(thisObj, arrayLength - 1);

            // Return the first element.
            return firstElement;
        }

        /// <summary>
        /// Deletes a range of elements from the array and optionally inserts new elements.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="start"> The index to start deleting from. </param>
        /// <param name="deleteCount"> The number of elements to delete. </param>
        /// <param name="items"> The items to insert. </param>
        /// <returns> An array containing the deleted elements, if any. </returns>
        [JSInternalFunction(Name = "splice", Flags = JSFunctionFlags.HasThisObject | JSFunctionFlags.MutatesThisObject, Length = 2)]
        public static ArrayInstance Splice(ObjectInstance thisObj, int start, int deleteCount, params object[] items)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, ErrorType.RangeError, "The array is too long");

            // Fix the arguments so they are positive and within the bounds of the array.
            if (start < 0)
                start = Math.Max((int)arrayLength + start, 0);
            else
                start = Math.Min(start, (int)arrayLength);
            deleteCount = Math.Min(Math.Max(deleteCount, 0), (int)arrayLength - start);

            // Get the deleted items.
            object[] deletedItems = new object[deleteCount];
            for (int i = 0; i < deleteCount; i++)
                deletedItems[i] = thisObj[(uint)(start + i)];

            // Move the trailing elements.
            int offset = items.Length - deleteCount;
            int newLength = (int)arrayLength + offset;
            if (deleteCount > items.Length)
            {
                for (int i = start + items.Length; i < newLength; i++)
                    thisObj[(uint)i] = thisObj[(uint)(i - offset)];

                // Delete the trailing elements.
                for (int i = newLength; i < arrayLength; i++)
                    thisObj.Delete((uint)i, true);
            }
            else
            {
                for (int i = newLength - 1; i >= start + items.Length; i--)
                    thisObj[(uint)i] = thisObj[(uint)(i - offset)];
            }
            SetLength(thisObj, (uint)newLength);

            // Insert the new elements.
            for (int i = 0; i < items.Length; i++)
                thisObj[(uint)(start + i)] = items[i];

            // Return the deleted items.
            return thisObj.Engine.Array.New(deletedItems);
        }

        /// <summary>
        /// Prepends the given items to the start of the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="items"> The items to prepend. </param>
        /// <returns> The new length of the array. </returns>
        [JSInternalFunction(Name = "unshift", Flags = JSFunctionFlags.HasThisObject | JSFunctionFlags.MutatesThisObject)]
        public static uint Unshift(ObjectInstance thisObj, params object[] items)
        {
            // If the "this" object is an array and the array is dense, use the fast version of this method.
            var array = thisObj as ArrayInstance;
            if (array != null && array.dense != null)
            {
                // Dense arrays are supported up to 2^32-1.
                if (array.length + items.Length > int.MaxValue)
                    throw new JavaScriptException(thisObj.Engine, ErrorType.RangeError, "Invalid array length");

                if (array.denseMayContainHoles == true && array.Prototype != null)
                {
                    // Find all the holes and populate them from the prototype.
                    for (uint i = 0; i < array.length; i++)
                        if (array.dense[i] == null)
                            array.dense[i] = array.Prototype.GetPropertyValue(i);
                }

                // Allocate some more space if required.
                if (array.length + items.Length > array.dense.Length)
                    array.ResizeDenseArray((uint)Math.Max(array.dense.Length * 2 + 10, array.length + items.Length * 10), array.length);

                // Shift all the items up.
                Array.Copy(array.dense, 0, array.dense, items.Length, (int)array.length);

                // Prepend the new items.
                for (int i = 0; i < items.Length; i++)
                    array.dense[i] = items[i];

                // Update the length property.
                array.length += (uint)items.Length;

                // Return the new length of the array.
                return array.length;
            }

            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method supports arrays of length up to 2^32-1.
            if (uint.MaxValue - arrayLength < items.Length)
                throw new JavaScriptException(thisObj.Engine, ErrorType.RangeError, "Invalid array length");

            // Update the length property.
            SetLength(thisObj, arrayLength + (uint)items.Length);

            // Shift all the items up.
            for (int i = (int)arrayLength - 1; i >= 0; i--)
                thisObj[(uint)(i + items.Length)] = thisObj[(uint)i];

            // Prepend the new items.
            for (uint i = 0; i < items.Length; i++)
                thisObj.SetPropertyValue(i, items[i], true);

            // Return the new length of the array.
            return arrayLength + (uint)items.Length;
        }



        //     ARRAY WRAPPER FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Implements a wrapper for regular JS arrays.
        /// </summary>
        private class ArrayInstanceWrapper : ArrayWrapper<object>
        {
            /// <summary>
            /// Creates a new ArrayInstanceWrapper instance.
            /// </summary>
            /// <param name="wrappedInstance"> The array-like object that is being wrapped. </param>
            public ArrayInstanceWrapper(ObjectInstance wrappedInstance)
                : base(wrappedInstance, (int)GetLength(wrappedInstance))
            {
            }

            /// <summary>
            /// Gets or sets an array element within the range 0 .. Length-1 (inclusive).
            /// </summary>
            /// <param name="index"> The index to get or set. </param>
            /// <returns> The value at the given index. </returns>
            public override object this[int index]
            {
                get { return WrappedInstance[index]; }
                set { WrappedInstance[index] = value; }
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
                return Engine.Array.New(values);
            }

            /// <summary>
            /// Convert an untyped value to a typed value.
            /// </summary>
            /// <param name="value"> The value to convert. </param>
            /// <returns> The value converted to type <typeparamref name="T"/>. </returns>
            public override object ConvertValue(object value)
            {
                return value;
            }
        }

        /// <summary>
        /// Concatenates all the elements of the array, using the specified separator between each
        /// element.  If no separator is provided, a comma is used for this purpose.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="separator"> The string to use as a separator. </param>
        /// <returns> A string that consists of the element values separated by the separator string. </returns>
        [JSInternalFunction(Name = "join", Flags = JSFunctionFlags.HasThisObject)]
        public static string Join(ObjectInstance thisObj, string separator = ",")
        {
            return new ArrayInstanceWrapper(thisObj).Join(separator);
        }

        /// <summary>
        /// Reverses the order of the elements in the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> The array that is being operated on. </returns>
        [JSInternalFunction(Name = "reverse", Flags = JSFunctionFlags.HasThisObject | JSFunctionFlags.MutatesThisObject)]
        public static ObjectInstance Reverse(ObjectInstance thisObj)
        {
            return new ArrayInstanceWrapper(thisObj).Reverse();
        }

        /// <summary>
        /// Returns a section of an array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="start"> The index of the first element in the section.  If this value is
        /// negative it is treated as an offset from the end of the array. </param>
        /// <param name="end"> The index of the element just past the last element in the section.
        /// If this value is negative it is treated as an offset from the end of the array.  If
        /// <paramref name="end"/> is less than or equal to <paramref name="start"/> then an empty
        /// array is returned. </param>
        /// <returns> A section of an array. </returns>
        [JSInternalFunction(Name = "slice", Flags = JSFunctionFlags.HasThisObject, Length = 2)]
        public static ObjectInstance Slice(ObjectInstance thisObj, int start, int end = int.MaxValue)
        {
            return new ArrayInstanceWrapper(thisObj).Slice(start, end);
        }

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="comparisonFunction"> A function which determines the order of the
        /// elements.  This function should return a number less than zero if the first argument is
        /// less than the second argument, zero if the arguments are equal or a number greater than
        /// zero if the first argument is greater than Defaults to an ascending ASCII ordering. </param>
        /// <returns> The array that was sorted. </returns>
        [JSInternalFunction(Name = "sort", Flags = JSFunctionFlags.HasThisObject | JSFunctionFlags.MutatesThisObject, Length = 1)]
        public static ObjectInstance Sort(ObjectInstance thisObj, FunctionInstance comparisonFunction = null)
        {
            Func<object, object, int> comparer;
            if (comparisonFunction == null)
            {
                // Default comparer.
                comparer = (a, b) =>
                {
                    if (a == null && b == null)
                        return 0;
                    if (a == null)
                        return 1;
                    if (b == null)
                        return -1;
                    if (a == Undefined.Value && b == Undefined.Value)
                        return 0;
                    if (a == Undefined.Value)
                        return 1;
                    if (b == Undefined.Value)
                        return -1;
                    return string.Compare(TypeConverter.ToString(a), TypeConverter.ToString(b), StringComparison.Ordinal);
                };
            }
            else
            {
                // Custom comparer.
                comparer = (a, b) =>
                {
                    if (a == null && b == null)
                        return 0;
                    if (a == null)
                        return 1;
                    if (b == null)
                        return -1;
                    if (a == Undefined.Value && b == Undefined.Value)
                        return 0;
                    if (a == Undefined.Value)
                        return 1;
                    if (b == Undefined.Value)
                        return -1;
                    var v = TypeConverter.ToNumber(comparisonFunction.CallFromNative("sort", null, a, b));
                    if (double.IsNaN(v))
                        return 0;
                    return Math.Sign(v);
                };
            }

            return new ArrayInstanceWrapper(thisObj).Sort(comparer);
        }

        /// <summary>
        /// Returns a locale-specific string representing this object.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> A locale-specific string representing this object. </returns>
        [JSInternalFunction(Name = "toLocaleString", Flags = JSFunctionFlags.HasThisObject)]
        public static string ToLocaleString(ObjectInstance thisObj)
        {
            return new ArrayInstanceWrapper(thisObj).ToLocaleString();
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> A string representing this object. </returns>
        [JSInternalFunction(Name = "toString", Flags = JSFunctionFlags.HasThisObject)]
        public static string ToString(ObjectInstance thisObj)
        {
            return new ArrayInstanceWrapper(thisObj).ToString();
        }

        /// <summary>
        /// Returns the index of the given search element in the array, starting from
        /// <paramref name="fromIndex"/>.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="searchElement"> The value to search for. </param>
        /// <param name="fromIndex"> The array index to start searching. </param>
        /// <returns> The index of the given search element in the array, or <c>-1</c> if the
        /// element wasn't found. </returns>
        [JSInternalFunction(Name = "indexOf", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static int IndexOf(ObjectInstance thisObj, object searchElement, int fromIndex = 0)
        {
            return new ArrayInstanceWrapper(thisObj).IndexOf(searchElement, fromIndex);
        }

        /// <summary>
        /// Returns the index of the given search element in the array, searching backwards from
        /// the end of the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="searchElement"> The value to search for. </param>
        /// <returns> The index of the given search element in the array, or <c>-1</c> if the
        /// element wasn't found. </returns>
        [JSInternalFunction(Name = "lastIndexOf", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static int LastIndexOf(ObjectInstance thisObj, object searchElement)
        {
            return LastIndexOf(thisObj, searchElement, int.MaxValue);
        }

        /// <summary>
        /// Returns the index of the given search element in the array, searching backwards from
        /// <paramref name="fromIndex"/>.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="searchElement"> The value to search for. </param>
        /// <param name="fromIndex"> The array index to start searching. </param>
        /// <returns> The index of the given search element in the array, or <c>-1</c> if the
        /// element wasn't found. </returns>
        [JSInternalFunction(Name = "lastIndexOf", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static int LastIndexOf(ObjectInstance thisObj, object searchElement, int fromIndex)
        {
            return new ArrayInstanceWrapper(thisObj).LastIndexOf(searchElement, fromIndex);
        }

        /// <summary>
        /// Determines if every element of the array matches criteria defined by the given user-
        /// defined function.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> <c>true</c> if every element of the array matches criteria defined by the
        /// given user-defined function; <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "every", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static bool Every(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new ArrayInstanceWrapper(thisObj).Every(callbackFunction, context);
        }

        /// <summary>
        /// Determines if at least one element of the array matches criteria defined by the given
        /// user-defined function.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> <c>true</c> if at least one element of the array matches criteria defined by
        /// the given user-defined function; <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "some", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static bool Some(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new ArrayInstanceWrapper(thisObj).Some(callbackFunction, context);
        }

        /// <summary>
        /// Calls the given user-defined function once per element in the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        [JSInternalFunction(Name = "forEach", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static void ForEach(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            new ArrayInstanceWrapper(thisObj).ForEach(callbackFunction, context);
        }

        /// <summary>
        /// Creates a new array with the results of calling the given function on every element in
        /// this array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="callbackFunction"> A user-defined function that is called for each element
        /// in the array.  This function is called with three arguments: the value of the element,
        /// the index of the element, and the array that is being operated on.  The value that is
        /// returned from this function is stored in the resulting array. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> A new array with the results of calling the given function on every element
        /// in the array. </returns>
        [JSInternalFunction(Name = "map", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static ObjectInstance Map(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new ArrayInstanceWrapper(thisObj).Map(callbackFunction, context);
        }

        /// <summary>
        /// Returns the first element in the given array that passes the test implemented by the
        /// given function.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> The first element that results in the callback returning <c>true</c>. </returns>
        [JSInternalFunction(Name = "find", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static object Find(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new ArrayInstanceWrapper(thisObj).Find(callbackFunction, context);
        }

        /// <summary>
        /// Creates a new array with the elements from this array that pass the test implemented by
        /// the given function.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> A copy of this array but with only those elements which produce <c>true</c>
        /// when passed to the provided function. </returns>
        [JSInternalFunction(Name = "filter", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static ObjectInstance Filter(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new ArrayInstanceWrapper(thisObj).Filter(callbackFunction, context);
        }

        /// <summary>
        /// Accumulates a single value by calling a user-defined function for each element.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="callbackFunction"> A user-defined function that is called for each element
        /// in the array.  This function is called with four arguments: the current accumulated
        /// value, the value of the element, the index of the element, and the array that is being
        /// operated on.  The return value for this function is the new accumulated value and is
        /// passed to the next invocation of the function. </param>
        /// <param name="initialValue"> The initial accumulated value. </param>
        /// <returns> The accumulated value returned from the last invocation of the callback
        /// function. </returns>
        [JSInternalFunction(Name = "reduce", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static object Reduce(ObjectInstance thisObj, FunctionInstance callbackFunction, object initialValue = null)
        {
            return new ArrayInstanceWrapper(thisObj).Reduce(callbackFunction, initialValue);
        }

        /// <summary>
        /// Accumulates a single value by calling a user-defined function for each element
        /// (starting with the last element in the array).
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="callbackFunction"> A user-defined function that is called for each element
        /// in the array.  This function is called with four arguments: the current accumulated
        /// value, the value of the element, the index of the element, and the array that is being
        /// operated on.  The return value for this function is the new accumulated value and is
        /// passed to the next invocation of the function. </param>
        /// <param name="initialValue"> The initial accumulated value. </param>
        /// <returns> The accumulated value returned from the last invocation of the callback
        /// function. </returns>
        [JSInternalFunction(Name = "reduceRight", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static object ReduceRight(ObjectInstance thisObj, FunctionInstance callbackFunction, object initialValue = null)
        {
            return new ArrayInstanceWrapper(thisObj).ReduceRight(callbackFunction, initialValue);
        }

        /// <summary>
        /// Copies the sequence of array elements within the array to the position starting at
        /// target. The copy is taken from the index positions of the second and third arguments
        /// start and end. The end argument is optional and defaults to the length of the array.
        /// This method has the same algorithm as Array.prototype.copyWithin.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="target"> Target start index position where to copy the elements to. </param>
        /// <param name="start"> Source start index position where to start copying elements from. </param>
        /// <param name="end"> Optional. Source end index position where to end copying elements from. </param>
        [JSInternalFunction(Name = "copyWithin", Length = 2, Flags = JSFunctionFlags.HasThisObject | JSFunctionFlags.MutatesThisObject)]
        public static ObjectInstance CopyWithin(ObjectInstance thisObj, int target, int start, int end = int.MaxValue)
        {
            return new ArrayInstanceWrapper(thisObj).CopyWithin(target, start, end);
        }

        /// <summary>
        /// Fills all the elements of a typed array from a start index to an end index with a
        /// static value.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="value"> The value to fill the typed array with. </param>
        /// <param name="start"> Optional. Start index. Defaults to 0. </param>
        /// <param name="end"> Optional. End index (exclusive). Defaults to the length of the array. </param>
        /// <returns> The array that is being operated on. </returns>
        [JSInternalFunction(Name = "fill", Length = 1, Flags = JSFunctionFlags.HasThisObject | JSFunctionFlags.MutatesThisObject)]
        public static ObjectInstance Fill(ObjectInstance thisObj, object value, int start = 0, int end = int.MaxValue)
        {
            return new ArrayInstanceWrapper(thisObj).Fill(value, start, end);
        }

        /// <summary>
        /// Returns an index in the typed array, if an element in the typed array satisfies the
        /// provided testing function. Otherwise -1 is returned.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> The first element that results in the callback returning <c>true</c>. </returns>
        [JSInternalFunction(Name = "findIndex", Length = 1, Flags = JSFunctionFlags.HasThisObject)]
        public static int FindIndex(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            return new ArrayInstanceWrapper(thisObj).FindIndex(callbackFunction, context);
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the number of items in the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> The number of items in the array. </returns>
        private static uint GetLength(ObjectInstance thisObj)
        {
            if (thisObj is ArrayInstance)
                return ((ArrayInstance)thisObj).length;
            return TypeConverter.ToUint32(thisObj["length"]);
        }

        /// <summary>
        /// Sets the number of items in the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="value"> The new value of the length property. </param>
        private static void SetLength(ObjectInstance thisObj, uint value)
        {
            if (thisObj is ArrayInstance)
                ((ArrayInstance)thisObj).Length = value;
            else
                thisObj.SetPropertyValue("length", (double)value, true);
        }

        /// <summary>
        /// Enlarges the size of the dense array.
        /// </summary>
        /// <param name="newCapacity"> The new capacity of the array. </param>
        /// <param name="length"> The valid number of items in the array. </param>
        private void ResizeDenseArray(uint newCapacity, uint length)
        {
            if (newCapacity < length)
                throw new InvalidOperationException("Cannot resize smaller than the length property.");
            var resizedArray = new object[(int)newCapacity];
            Array.Copy(this.dense, resizedArray, (int)length);
            this.dense = resizedArray;
        }
    }
}
