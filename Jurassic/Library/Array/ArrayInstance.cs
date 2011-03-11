using System;
using System.Collections.Generic;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the JavaScript Array object.
    /// </summary>
    public class ArrayInstance : ObjectInstance
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



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "Array"; }
        }

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
                    // Resize the array.
                    ResizeDenseArray(this.length);
                    if (this.length > previousLength)
                        this.denseMayContainHoles = true;
                }
                else
                {
                    // Remove all the elements with indices > length.
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
        /// <param name="propertyName"> The property name to parse. </param>
        /// <returns> The array index value, or <c>uint.MaxValue</c> if the property name does not reference
        /// an array index. </returns>
        internal static uint ParseArrayIndex(string propertyName)
        {
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
        /// <param name="propertyName"> The array index of the property. </param>
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
                    if (index < this.dense.Length * 2 + 10)
                    {
                        // Enlarge the dense array.
                        ResizeDenseArray((uint)(this.dense.Length * 2 + 10));

                        // Set the value.
                        this.dense[index] = value;
                        this.denseMayContainHoles = this.denseMayContainHoles || index > this.length;
                    }
                    else
                    {
                        // Switch to a sparse array.
                        this.sparse = SparseArray.FromDenseArray(this.dense);
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
        /// <param name="propertyName"> The name of the property to modify. </param>
        /// <param name="descriptor"> The property value and attributes. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is not configurable or the object is sealed. </param>
        /// <returns> <c>true</c> if the property was successfully modified; <c>false</c> otherwise. </returns>
        public override bool DefineProperty(string propertyName, PropertyDescriptor descriptor, bool throwOnError)
        {
            // Make sure the property name isn't actually an array index.
            uint arrayIndex = ParseArrayIndex(propertyName);
            if (arrayIndex != uint.MaxValue)
            {
                // Spec violation: array elements are never accessor properties.
                if (descriptor.IsAccessor == true)
                {
                    if (throwOnError == true)
                        throw new JavaScriptException(this.Engine, "TypeError", string.Format("Accessors are not supported for array elements.", propertyName));
                    return false;
                }

                // Spec violation: array elements are always full access.
                if (descriptor.Attributes != PropertyAttributes.FullAccess)
                {
                    if (throwOnError == true)
                        throw new JavaScriptException(this.Engine, "TypeError", string.Format("Non-accessible array elements are not supported.", propertyName));
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
            return base.DefineProperty(propertyName, descriptor, throwOnError);
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
                            yield return new PropertyNameAndValue(i.ToString(), new PropertyDescriptor(arrayElementValue, PropertyAttributes.FullAccess));
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
                                yield return new PropertyNameAndValue(i.ToString(), new PropertyDescriptor(arrayElementValue, PropertyAttributes.FullAccess));
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
        [JSFunction(Name = "concat", Flags = FunctionBinderFlags.HasThisObject)]
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
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The resulting array is too long");

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
                            result.CopyTo(array.dense, (uint)index, (int)array.Length);
                        else
                            result.CopyTo(array.sparse, (uint)index);
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
        /// Concatenates all the elements of the array, using the specified separator between each
        /// element.  If no separator is provided, a comma is used for this purpose.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="separator"> The string to use as a separator. </param>
        /// <returns> A string that consists of the element values separated by the separator string. </returns>
        [JSFunction(Name = "join", Flags = FunctionBinderFlags.HasThisObject)]
        public static string Join(ObjectInstance thisObj, string separator = ",")
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports strings of length up to 2^31-1.
            if (arrayLength > int.MaxValue / 2)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            var result = new System.Text.StringBuilder((int)arrayLength * 2);
            try
            {

                for (uint i = 0; i < arrayLength; i++)
                {
                    if (i > 0)
                        result.Append(separator);
                    object element = thisObj[i];
                    if (element != null && element != Undefined.Value && element != Null.Value)
                        result.Append(TypeConverter.ToString(element));
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");
            }
            return result.ToString();
        }

        /// <summary>
        /// Removes the last element from the array and returns it.
        /// </summary>
        /// <param name="thisObj"> The array to operate on. </param>
        /// <returns> The last element from the array. </returns>
        [JSFunction(Name = "pop", Flags = FunctionBinderFlags.HasThisObject)]
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

                // Delete it from the array.
                this.dense[this.length] = null;

                // Check if the array should be shrunk.
                if (this.length < this.dense.Length / 2 && this.length > 10)
                    ResizeDenseArray((uint)(this.dense.Length / 2 + 10));

                // Return the last value.
                return result;
            }
            else
            {
                // Get the last value.
                var result = this.sparse[this.length];

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
        [JSFunction(Name = "push", Flags = FunctionBinderFlags.HasThisObject)]
        public static int Push(ObjectInstance thisObj, params object[] items)
        {
            // If the "this" object is an array, use the fast version of this method.
            if (thisObj is ArrayInstance && items.Length == 1)
                return ((ArrayInstance)thisObj).Push(items[0]);

            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports strings of length up to 2^31-1.
            if (arrayLength > uint.MaxValue - items.Length)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            // For each item to append.
            for (int i = 0; i < items.Length; i++)
            {
                // Append the new item to the array.
                thisObj.SetPropertyValue(arrayLength ++, items[i], true);
            }

            // Update the length property.
            SetLength(thisObj, arrayLength);

            // Return the new length.
            return (int)arrayLength;
        }

        /// <summary>
        /// Appends one element to the end of the array.
        /// </summary>
        /// <param name="item"> The item to append to the array. </param>
        public int Push(object item)
        {
            if (this.dense != null)
            {
                // Check if we need to enlarge the array.
                if (this.length == this.dense.Length)
                    ResizeDenseArray(this.length * 2 + 10);

                // Append the new item to the array.
                this.dense[this.length++] = item;
            }
            else
            {
                // Append the new item to the array.
                this.sparse[this.length++] = item;
            }

            // Return the new length.
            return (int) this.length;
        }

        /// <summary>
        /// Reverses the order of the elements in the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> The array that is being operated on. </returns>
        [JSFunction(Name = "reverse", Flags = FunctionBinderFlags.HasThisObject)]
        public static ObjectInstance Reverse(ObjectInstance thisObj)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // Reverse each element.
            for (uint lowIndex = 0; lowIndex < arrayLength / 2; lowIndex++)
            {
                uint highIndex = arrayLength - lowIndex - 1;

                // Swap the two values.
                object low = thisObj[lowIndex];
                object high = thisObj[highIndex];
                if (high != null)
                    thisObj[lowIndex] = high;
                else
                    thisObj.Delete(lowIndex, true);
                if (low != null)
                    thisObj[highIndex] = low;
                else
                    thisObj.Delete(highIndex, true);
            }

            return thisObj;
        }

        /// <summary>
        /// The first element in the array is removed from the array and returned.  All the other
        /// elements are shifted down in the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> The first element in the array. </returns>
        [JSFunction(Name = "shift", Flags = FunctionBinderFlags.HasThisObject)]
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
        [JSFunction(Name = "slice", Flags = FunctionBinderFlags.HasThisObject, Length = 2)]
        public static ArrayInstance Slice(ObjectInstance thisObj, int start, int end = int.MaxValue)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            // Fix the arguments so they are positive and within the bounds of the array.
            if (start < 0)
                start += (int)arrayLength;
            if (end < 0)
                end += (int)arrayLength + 1;
            if (end <= start)
                return thisObj.Engine.Array.New(new object[0]);
            start = Math.Min(Math.Max(start, 0), (int)arrayLength);
            end = Math.Min(Math.Max(end, 0), (int)arrayLength);

            // Populate a new array.
            object[] result = new object[end - start];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = thisObj[(uint)(start + i)];
            }
            return thisObj.Engine.Array.New(result);
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
        [JSFunction(Name = "sort", Flags = FunctionBinderFlags.HasThisObject)]
        public static ObjectInstance Sort(ObjectInstance thisObj, FunctionInstance comparisonFunction = null)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // Create a comparer delegate.
            Comparison<object> comparer;
            if (comparisonFunction == null)
                comparer = (a, b) => string.Compare(TypeConverter.ToString(a), TypeConverter.ToString(b), StringComparison.Ordinal);
            else
                comparer = (a, b) => TypeConverter.ToInt32(comparisonFunction.CallLateBound(thisObj.Engine.Global, a, b));

            try
            {
                // Sort the array.
                QuickSort(thisObj, comparer, 0, arrayLength - 1);
            }
            catch (IndexOutOfRangeException)
            {
                throw new JavaScriptException(thisObj.Engine, "TypeError", "Invalid comparison function");
            }

            return thisObj;
        }

        /// <summary>
        /// Deletes a range of elements from the array and optionally inserts new elements.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="start"> The index to start deleting from. </param>
        /// <param name="deleteCount"> The number of elements to delete. </param>
        /// <param name="items"> The items to insert. </param>
        /// <returns> An array containing the deleted elements, if any. </returns>
        [JSFunction(Name = "splice", Flags = FunctionBinderFlags.HasThisObject, Length = 2)]
        public static ArrayInstance Splice(ObjectInstance thisObj, int start, int deleteCount, params object[] items)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

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
            }
            else
            {
                for (int i = newLength; i > start + items.Length; i--)
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
        [JSFunction(Name = "unshift", Flags = FunctionBinderFlags.HasThisObject)]
        public static uint Unshift(ObjectInstance thisObj, params object[] items)
        {
            // If the "this" object is an array and the array is dense, use the fast version of this method.
            var array = thisObj as ArrayInstance;
            if (array != null && array.dense != null)
            {
                // Dense arrays are supported up to 2^32-1.
                if (array.length + items.Length > int.MaxValue)
                    throw new JavaScriptException(thisObj.Engine, "RangeError", "Invalid array length");

                if (array.denseMayContainHoles == true && array.Prototype != null)
                {
                    // Find all the holes and populate them from the prototype.
                    for (int i = 0; i < array.length; i++)
                        if (array.dense[i] == null)
                            array.dense[i] = array.Prototype.GetPropertyValue((uint)i);
                }

                // Allocate some more space if required.
                if (array.length + items.Length > array.dense.Length)
                    array.ResizeDenseArray((uint)Math.Max(array.dense.Length * 2 + 10, array.length + items.Length * 10));

                // Shift all the items up.
                Array.Copy(array.dense, 0, array.dense, items.Length, array.length);

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
                throw new JavaScriptException(thisObj.Engine, "RangeError", "Invalid array length");

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

        /// <summary>
        /// Returns a locale-specific string representing this object.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> A locale-specific string representing this object. </returns>
        [JSFunction(Name = "toLocaleString", Flags = FunctionBinderFlags.HasThisObject)]
        public static string ToLocaleString(ObjectInstance thisObj)
        {
            // Get the length of the array.
            var arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^30-1.
            if (arrayLength > int.MaxValue / 2)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            var result = new System.Text.StringBuilder((int)arrayLength * 2);

            // Get the culture-specific list separator.
            var listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            try
            {

                for (uint i = 0; i < arrayLength; i++)
                {
                    // Append the localized list separator.
                    if (i > 0)
                        result.Append(listSeparator);

                    // Retrieve the array element.
                    var element = thisObj[i];

                    // Convert the element to a string and append it to the result.
                    if (element != null && element != Undefined.Value && element != Null.Value)
                        result.Append(TypeConverter.ToObject(thisObj.Engine, element).CallMemberFunction("toLocaleString"));
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <returns> A string representing this object. </returns>
        [JSFunction(Name = "toString", Flags = FunctionBinderFlags.HasThisObject)]
        public static string ToString(ObjectInstance thisObj)
        {
            // Try calling thisObj.join().
            object result;
            if (thisObj.TryCallMemberFunction(out result, "join") == true)
                return TypeConverter.ToString(result);

            // Otherwise, use the default Object.prototype.toString() method.
            return ObjectInstance.ToStringJS(thisObj.Engine, thisObj);
        }



        //     JAVASCRIPT FUNCTIONS (NEW IN ES5)
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns the index of the given search element in the array, starting from
        /// <paramref name="fromIndex"/>.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="searchElement"> The value to search for. </param>
        /// <param name="fromIndex"> The array index to start searching. </param>
        /// <returns> The index of the given search element in the array, or <c>-1</c> if the
        /// element wasn't found. </returns>
        [JSFunction(Name = "indexOf", Flags = FunctionBinderFlags.HasThisObject, Length = 1)]
        public static int IndexOf(ObjectInstance thisObj, object searchElement, int fromIndex = 0)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            // If fromIndex is less than zero, it is an offset from the end of the array.
            if (fromIndex < 0)
                fromIndex += (int)arrayLength;

            for (int i = Math.Max(fromIndex, 0); i < arrayLength; i++)
            {
                // Get the value of the array element.
                object elementValue = thisObj[(uint)i];

                // Compare the given search element with the array element.
                if (elementValue != null && TypeComparer.StrictEquals(searchElement, elementValue) == true)
                    return i;
            }

            // The search element wasn't found.
            return -1;
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
        [JSFunction(Name = "lastIndexOf", Flags = FunctionBinderFlags.HasThisObject, Length = 1)]
        public static int LastIndexOf(ObjectInstance thisObj, object searchElement, int fromIndex = int.MaxValue)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            // If fromIndex is less than zero, it is an offset from the end of the array.
            if (fromIndex < 0)
                fromIndex += (int)arrayLength;

            for (int i = Math.Min((int)arrayLength - 1, fromIndex); i >= 0; i --)
            {
                // Get the value of the array element.
                object elementValue = thisObj[(uint)i];

                // Compare the given search element with the array element.
                if (elementValue != null && TypeComparer.StrictEquals(searchElement, elementValue) == true)
                    return i;
            }

            // The search element wasn't found.
            return -1;
        }

        /// <summary>
        /// Determines if every element of the array matches criteria defined by the given user-
        /// defined function.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="function"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> <c>true</c> if every element of the array matches criteria defined by the
        /// given user-defined function; <c>false</c> otherwise. </returns>
        [JSFunction(Name = "every", Flags = FunctionBinderFlags.HasThisObject, Length = 1)]
        public static bool Every(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            for (int i = 0; i < arrayLength; i ++)
            {
                // Get the value of the array element.
                object elementValue = thisObj[(uint)i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    if (TypeConverter.ToBoolean(callbackFunction.CallLateBound(context, elementValue, i, thisObj)) == false)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if at least one element of the array matches criteria defined by the given
        /// user-defined function.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="function"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> <c>true</c> if at least one element of the array matches criteria defined by
        /// the given user-defined function; <c>false</c> otherwise. </returns>
        [JSFunction(Name = "some", Flags = FunctionBinderFlags.HasThisObject, Length = 1)]
        public static bool Some(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            for (int i = 0; i < arrayLength; i++)
            {
                // Get the value of the array element.
                object elementValue = thisObj[(uint)i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    if (TypeConverter.ToBoolean(callbackFunction.CallLateBound(context, elementValue, i, thisObj)) == true)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calls the given user-defined function once per element in the array.
        /// </summary>
        /// <param name="thisObj"> The array that is being operated on. </param>
        /// <param name="function"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        [JSFunction(Name = "forEach", Flags = FunctionBinderFlags.HasThisObject, Length = 1)]
        public static void ForEach(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            for (int i = 0; i < arrayLength; i++)
            {
                // Get the value of the array element.
                object elementValue = thisObj[(uint)i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    callbackFunction.CallLateBound(context, elementValue, i, thisObj);
                }
            }
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
        [JSFunction(Name = "map", Flags = FunctionBinderFlags.HasThisObject, Length = 1)]
        public static ArrayInstance Map(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            // Create a new array to hold the new values.
            var resultArray = thisObj.Engine.Array.New();

            for (int i = 0; i < arrayLength; i++)
            {
                // Get the value of the array element.
                object elementValue = thisObj[(uint)i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    object result = callbackFunction.CallLateBound(context, elementValue, i, thisObj);

                    // Store the result.
                    resultArray[(uint)i] = result;
                }
            }
            return resultArray;
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
        [JSFunction(Name = "filter", Flags = FunctionBinderFlags.HasThisObject, Length = 1)]
        public static ArrayInstance Filter(ObjectInstance thisObj, FunctionInstance callbackFunction, ObjectInstance context = null)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            // Create a new array to hold the new values.
            var result = thisObj.Engine.Array.New();

            for (int i = 0; i < arrayLength; i++)
            {
                // Get the value of the array element.
                object elementValue = thisObj[(uint)i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    bool includeInArray = TypeConverter.ToBoolean(callbackFunction.CallLateBound(context, elementValue, i, thisObj));

                    // Store the result if the callback function returned true.
                    if (includeInArray == true)
                        result.Push(elementValue);
                }
            }
            return result;
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
        [JSFunction(Name = "reduce", Flags = FunctionBinderFlags.HasThisObject, Length = 1)]
        public static object Reduce(ObjectInstance thisObj, FunctionInstance callbackFunction, object initialValue = null)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            // If an initial value is not provided, the initial value is the first (defined) element.
            int i = 0;
            object accumulatedValue = initialValue;
            if (accumulatedValue == null)
            {
                // Scan for a defined element.
                for (; i < arrayLength; i++)
                {
                    if (thisObj[(uint)i] != null)
                    {
                        accumulatedValue = thisObj[(uint)(i++)];
                        break;
                    }
                }
                if (accumulatedValue == null)
                    throw new JavaScriptException(thisObj.Engine, "TypeError", "Reduce of empty array with no initial value");
            }

            // Scan from low to high.
            for (; i < arrayLength; i++)
            {
                // Get the value of the array element.
                object elementValue = thisObj[(uint)i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    accumulatedValue = callbackFunction.CallLateBound(null, accumulatedValue, elementValue, i, thisObj);
                }
            }

            return accumulatedValue;
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
        [JSFunction(Name = "reduceRight", Flags = FunctionBinderFlags.HasThisObject, Length = 1)]
        public static object ReduceRight(ObjectInstance thisObj, FunctionInstance callbackFunction, object initialValue = null)
        {
            // Get the length of the array.
            uint arrayLength = GetLength(thisObj);

            // This method only supports arrays of length up to 2^31-1.
            if (arrayLength > int.MaxValue)
                throw new JavaScriptException(thisObj.Engine, "RangeError", "The array is too long");

            // If an initial value is not provided, the initial value is the last (defined) element.
            int i = (int)arrayLength - 1;
            object accumulatedValue = initialValue;
            if (accumulatedValue == null)
            {
                // Scan for a defined element.
                for (; i >= 0; i--)
                {
                    if (thisObj[(uint)i] != null)
                    {
                        accumulatedValue = thisObj[(uint)(i--)];
                        break;
                    }
                }
                if (accumulatedValue == null)
                    throw new JavaScriptException(thisObj.Engine, "TypeError", "Reduce of empty array with no initial value");
            }

            // Scan from high to to low.
            for (; i >= 0; i--)
            {
                // Get the value of the array element.
                object elementValue = thisObj[(uint)i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    accumulatedValue = callbackFunction.CallLateBound(null, accumulatedValue, elementValue, i, thisObj);
                }
            }

            return accumulatedValue;
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
                ((ArrayInstance)thisObj).length = value;
            thisObj.SetPropertyValue("length", (double)value, true);
        }

        /// <summary>
        /// Enlarges the size of the dense array.
        /// </summary>
        /// <param name="newCapacity"> The new capacity of the array. </param>
        private void ResizeDenseArray(uint newCapacity)
        {
            var resizedArray = new object[(int)newCapacity];
            Array.Copy(this.dense, resizedArray, Math.Min(this.dense.Length, (int)newCapacity));
            this.dense = resizedArray;
        }

        /// <summary>
        /// Sorts a array using the quicksort algorithm.
        /// </summary>
        /// <param name="array"> The array to sort. </param>
        /// <param name="comparer"> A comparison function. </param>
        /// <param name="start"> The first index in the range. </param>
        /// <param name="end"> The last index in the range. </param>
        private static void QuickSort(ObjectInstance array, Comparison<object> comparer, uint start, uint end)
        {
            do
            {
                uint low = start;
                uint high = end;

                // Median of three.
                uint median = low + (high - low) / 2;
                SwapIfGreater(array, comparer, low, median);
                SwapIfGreater(array, comparer, low, high);
                SwapIfGreater(array, comparer, median, high);

                object y = array[median];
                do
                {
                    while (comparer(array[low], y) < 0)
                        low++;
                    while (comparer(y, array[high]) < 0)
                        high--;
                    if (low > high)
                        break;
                    if (low < high)
                    {
                        object temp = array[low];
                        array[low] = array[high];
                        array[high] = temp;
                    }
                    low++;
                    high--;
                } while (low <= high);

                if (high - start <= end - low)
                {
                    if (start < high)
                    {
                        if (high - start < 30)
                            InsertionSort(array, comparer, start, high);
                        else
                            QuickSort(array, comparer, start, high);
                    }
                    start = low;
                }
                else
                {
                    if (low < end)
                    {
                        if (end - low < 30)
                            InsertionSort(array, comparer, low, end);
                        else
                            QuickSort(array, comparer, low, end);
                    }
                    end = high;
                }
            } while (start < end);
        }

        /// <summary>
        /// Sorts a array using the insertion sort algorithm.
        /// </summary>
        /// <param name="array"> The array to sort. </param>
        /// <param name="comparer"> A comparison function. </param>
        /// <param name="start"> The first index in the range. </param>
        /// <param name="end"> The last index in the range. </param>
        private static void InsertionSort(ObjectInstance array, Comparison<object> comparer, uint start, uint end)
        {
            for (uint i = start + 1; i <= end; i++)
            {
                object value = array[i];
                uint j;
                for (j = i - 1; j > start && comparer(array[j], value) > 0; j--)
                    array[j + 1] = array[j];

                // Normally the for loop above would continue until j < start but since we are
                // using uint it doesn't work when start == 0.  Therefore the for loop stops one
                // short of start then the extra loop iteration runs below.
                if (j == start && comparer(array[j], value) > 0)
                {
                    array[j + 1] = array[j];
                    j--;
                }

                array[j + 1] = value;
            }
        }

        /// <summary>
        /// Swaps the elements at two locations in the array, if the first element is greater than
        /// the second element.
        /// </summary>
        /// <param name="array"> The array object. </param>
        /// <param name="comparer"> A comparison function. </param>
        /// <param name="index1"> The location of the first element. </param>
        /// <param name="index2"> The location of the second element. </param>
        private static void SwapIfGreater(ObjectInstance array, Comparison<object> comparer, uint index1, uint index2)
        {
            if (index1 == index2)
                return;
            if (comparer(array[index1], array[index2]) > 0)
            {
                object obj2 = array[index1];
                array[index1] = array[index2];
                array[index2] = obj2;
            }
        }
    }
}
