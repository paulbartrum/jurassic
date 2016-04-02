using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Implements the array algorithms that are shared between arrays and typed arrays.
    /// </summary>
    internal abstract class ArrayWrapper<T>
    {
        private ObjectInstance wrappedInstance;
        private int length;

        /// <summary>
        /// Creates a new ArrayWrapper instance.
        /// </summary>
        /// <param name="wrappedInstance"> The array-like object that is being wrapped. </param>
        /// <param name="length"> The number of elements in the array. </param>
        protected ArrayWrapper(ObjectInstance wrappedInstance, int length)
        {
            this.wrappedInstance = wrappedInstance;
            this.length = length;
        }


        //     ABSTRACT OPERATIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// The current scripting environment.
        /// </summary>
        public ScriptEngine Engine
        {
            get { return this.wrappedInstance.Engine; }
        }

        /// <summary>
        /// The array-like object that is being wrapped.
        /// </summary>
        public ObjectInstance WrappedInstance
        {
            get { return this.wrappedInstance; }
        }

        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public int Length
        {
            get { return this.length; }
        }

        /// <summary>
        /// Gets or sets an array element within the range 0 .. Length-1 (inclusive).
        /// </summary>
        /// <param name="index"> The index to get or set. </param>
        /// <returns> The value at the given index. </returns>
        public abstract T this[int index]
        {
            get;
            set;
        }

        /// <summary>
        /// Deletes the value at the given array index, throwing an exception on error.
        /// </summary>
        /// <param name="index"> The array index to delete. </param>
        public abstract void Delete(int index);

        /// <summary>
        /// Creates a new array of the same type as this one.
        /// </summary>
        /// <param name="values"> The values in the new array. </param>
        /// <returns> A new array object. </returns>
        public abstract ObjectInstance ConstructArray(T[] values);

        /// <summary>
        /// Convert an untyped value to a typed value.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> The value converted to type <typeparamref name="T"/>. </returns>
        public abstract T ConvertValue(object value);



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________


        /// <summary>
        /// Concatenates all the elements of the array, using the specified separator between each
        /// element.  If no separator is provided, a comma is used for this purpose.
        /// </summary>
        /// <param name="separator"> The string to use as a separator. </param>
        /// <returns> A string that consists of the element values separated by the separator string. </returns>
        public string Join(string separator)
        {
            var result = new System.Text.StringBuilder(Length * 2);
            for (int i = 0; i < Length; i++)
            {
                if (i > 0)
                    result.Append(separator);
                object element = this[i];
                if (element != null && element != Undefined.Value && element != Null.Value)
                    result.Append(TypeConverter.ToString(element));
            }
            return result.ToString();
        }

        /// <summary>
        /// Reverses the order of the elements in the array.
        /// </summary>
        /// <returns> The array that is being operated on. </returns>
        public ObjectInstance Reverse()
        {
            // Reverse each element.
            for (int lowIndex = 0; lowIndex < Length / 2; lowIndex++)
            {
                int highIndex = Length - lowIndex - 1;

                // Swap the two values.
                T low = this[lowIndex];
                T high = this[highIndex];
                if (high != null)
                    this[lowIndex] = high;
                else
                    Delete(lowIndex);
                if (low != null)
                    this[highIndex] = low;
                else
                    Delete(highIndex);
            }
            return WrappedInstance;
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
        public ObjectInstance Slice(int start, int end)
        {
            // Fix the arguments so they are positive and within the bounds of the array.
            if (start < 0)
                start += Length;
            if (end < 0)
                end += Length;
            if (end <= start)
                return ConstructArray(new T[0]);
            start = Math.Min(Math.Max(start, 0), Length);
            end = Math.Min(Math.Max(end, 0), Length);

            // Populate a new array.
            var result = new T[end - start];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = this[start + i];
            }
            return ConstructArray(result);
        }

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="comparer"> A comparison function. </param>
        /// <returns> The array that was sorted. </returns>
        public ObjectInstance Sort(Func<T, T, int> comparer)
        {
            // An array of size 1 or less is already sorted.
            if (Length <= 1)
                return WrappedInstance;

            try
            {
                // Sort the array.
                QuickSort(comparer, 0, Length - 1);
            }
            catch (IndexOutOfRangeException)
            {
                throw new JavaScriptException(Engine, ErrorType.TypeError, "Invalid comparison function");
            }

            return WrappedInstance;
        }
        
        /// <summary>
        /// Returns a locale-specific string representing this object.
        /// </summary>
        /// <returns> A locale-specific string representing this object. </returns>
        public string ToLocaleString()
        {
            var result = new System.Text.StringBuilder(Length * 2);

            // Get the culture-specific list separator.
            var listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            try
            {

                for (int i = 0; i < Length; i++)
                {
                    // Append the localized list separator.
                    if (i > 0)
                        result.Append(listSeparator);

                    // Retrieve the array element.
                    object element = this[i];

                    // Convert the element to a string and append it to the result.
                    if (element != null && element != Undefined.Value && element != Null.Value)
                        result.Append(TypeConverter.ToObject(Engine, element).CallMemberFunction("toLocaleString"));
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                throw new JavaScriptException(Engine, ErrorType.RangeError, "The array is too long");
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            // Try calling WrappedInstance.join().
            object result;
            if (WrappedInstance.TryCallMemberFunction(out result, "join") == true)
                return TypeConverter.ToString(result);

            // Otherwise, use the default Object.prototype.toString() method.
            return ObjectInstance.ToStringJS(Engine, WrappedInstance);
        }



        //     JAVASCRIPT FUNCTIONS (NEW IN ES5)
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns the index of the given search element in the array, starting from
        /// <paramref name="fromIndex"/>.
        /// </summary>
        /// <param name="searchElement"> The value to search for. </param>
        /// <param name="fromIndex"> The array index to start searching. </param>
        /// <returns> The index of the given search element in the array, or <c>-1</c> if the
        /// element wasn't found. </returns>
        public int IndexOf(object searchElement, int fromIndex)
        {
            // If fromIndex is less than zero, it is an offset from the end of the array.
            if (fromIndex < 0)
                fromIndex += Length;

            for (int i = Math.Max(fromIndex, 0); i < Length; i++)
            {
                // Get the value of the array element.
                object elementValue = this[i];

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
        /// <param name="searchElement"> The value to search for. </param>
        /// <param name="fromIndex"> The array index to start searching. </param>
        /// <returns> The index of the given search element in the array, or <c>-1</c> if the
        /// element wasn't found. </returns>
        public int LastIndexOf(object searchElement, int fromIndex)
        {
            // If fromIndex is less than zero, it is an offset from the end of the array.
            if (fromIndex < 0)
                fromIndex += Length;

            for (int i = Math.Min(Length - 1, fromIndex); i >= 0; i--)
            {
                // Get the value of the array element.
                object elementValue = this[i];

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
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on.  The function should
        /// return <c>true</c> or <c>false</c>. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        /// <returns> <c>true</c> if every element of the array matches criteria defined by the
        /// given user-defined function; <c>false</c> otherwise. </returns>
        public bool Every(FunctionInstance callbackFunction, ObjectInstance context)
        {
            for (int i = 0; i < Length; i ++)
            {
                // Get the value of the array element.
                object elementValue = this[i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    if (TypeConverter.ToBoolean(callbackFunction.CallFromNative("every", context, elementValue, i, WrappedInstance)) == false)
                        return false;
                }
            }
            return true;
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
        public bool Some(FunctionInstance callbackFunction, ObjectInstance context)
        {
            for (int i = 0; i < Length; i++)
            {
                // Get the value of the array element.
                object elementValue = this[i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    if (TypeConverter.ToBoolean(callbackFunction.CallFromNative("some", context, elementValue, i, WrappedInstance)) == true)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calls the given user-defined function once per element in the array.
        /// </summary>
        /// <param name="callbackFunction"> A user-defined function that is called for each element in the
        /// array.  This function is called with three arguments: the value of the element, the
        /// index of the element, and the array that is being operated on. </param>
        /// <param name="context"> The value of <c>this</c> in the context of the callback function. </param>
        public void ForEach(FunctionInstance callbackFunction, ObjectInstance context)
        {
            for (int i = 0; i < Length; i++)
            {
                // Get the value of the array element.
                object elementValue = this[i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    callbackFunction.CallFromNative("forEach", context, elementValue, i, WrappedInstance);
                }
            }
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
        public ObjectInstance Map(FunctionInstance callbackFunction, ObjectInstance context)
        {
            // Create a new array to hold the new values.
            // The length of the output array is always equal to the length of the input array.
            var resultArray = new T[Length];

            for (int i = 0; i < Length; i++)
            {
                // Get the value of the array element.
                object elementValue = this[i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    object result = callbackFunction.CallFromNative("map", context, elementValue, i, WrappedInstance);

                    // Store the result.
                    resultArray[i] = ConvertValue(result);
                }
            }

            return ConstructArray(resultArray);
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
        public object Find(FunctionInstance callbackFunction, ObjectInstance context)
        {
            for (int i = 0; i < Length; i++)
            {
                // Get the value of the array element.
                object elementValue = this[i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    bool result = TypeConverter.ToBoolean(callbackFunction.CallFromNative("find", context, elementValue, i, WrappedInstance));

                    // Return if the result was true.
                    if (result == true)
                        return elementValue;
                }
            }

            // No matches, return undefined.
            return Undefined.Value;
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
        public ObjectInstance Filter(FunctionInstance callbackFunction, ObjectInstance context)
        {
            // Create a new array to hold the new values.
            var result = new List<T>(Length / 2);

            for (int i = 0; i < Length; i++)
            {
                // Get the value of the array element.
                T elementValue = this[i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    bool includeInArray = TypeConverter.ToBoolean(callbackFunction.CallFromNative("filter", context, elementValue, i, WrappedInstance));

                    // Store the result if the callback function returned true.
                    if (includeInArray == true)
                        result.Add(elementValue);
                }
            }
            return ConstructArray(result.ToArray());
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
        public object Reduce(FunctionInstance callbackFunction, object initialValue)
        {
            // If an initial value is not provided, the initial value is the first (defined) element.
            int i = 0;
            object accumulatedValue = initialValue;
            if (accumulatedValue == null)
            {
                // Scan for a defined element.
                for (; i < Length; i++)
                {
                    if (this[i] != null)
                    {
                        accumulatedValue = this[i++];
                        break;
                    }
                }
                if (accumulatedValue == null)
                    throw new JavaScriptException(Engine, ErrorType.TypeError, "Reduce of empty array with no initial value");
            }

            // Scan from low to high.
            for (; i < Length; i++)
            {
                // Get the value of the array element.
                object elementValue = this[i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    accumulatedValue = callbackFunction.CallFromNative("reduce", Undefined.Value, accumulatedValue, elementValue, i, WrappedInstance);
                }
            }

            return accumulatedValue;
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
        public object ReduceRight(FunctionInstance callbackFunction, object initialValue)
        {
            // If an initial value is not provided, the initial value is the last (defined) element.
            int i = Length - 1;
            object accumulatedValue = initialValue;
            if (accumulatedValue == null)
            {
                // Scan for a defined element.
                for (; i >= 0; i--)
                {
                    if (this[i] != null)
                    {
                        accumulatedValue = this[i--];
                        break;
                    }
                }
                if (accumulatedValue == null)
                    throw new JavaScriptException(Engine, ErrorType.TypeError, "Reduce of empty array with no initial value");
            }

            // Scan from high to to low.
            for (; i >= 0; i--)
            {
                // Get the value of the array element.
                object elementValue = this[i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    accumulatedValue = callbackFunction.CallFromNative("reduceRight", Undefined.Value, accumulatedValue, elementValue, i, WrappedInstance);
                }
            }

            return accumulatedValue;
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
        /// <returns> The array that is being operated on. </returns>
        public ObjectInstance CopyWithin(int target, int start, int end)
        {
            // Negative values represent offsets from the end of the array.
            target = target < 0 ? Math.Max(Length + target, 0) : Math.Min(target, Length);
            start = start < 0 ? Math.Max(Length + start, 0) : Math.Min(start, Length);
            end = end < 0 ? Math.Max(Length + end, 0) : Math.Min(end, Length);
            
            // Calculate the number of values to copy.
            int count = Math.Min(end - start, Length - target);

            // Check if we need to copy in reverse due to an overlap.
            int direction = 1;
            if (start < target && target < start + count)
            {
                direction = -1;
                start += count - 1;
                target += count - 1;
            }

            while (count > 0)
            {
                // Get the value of the array element.
                T elementValue = this[start];

                if (elementValue != null)
                {
                    // Copy the value to the new position.
                    this[target] = elementValue;
                }
                else
                {
                    // Delete the element at the new position.
                    Delete(target);
                }

                // Progress to the next element.
                start += direction;
                target += direction;
                count--;
            }

            return WrappedInstance;
        }

        /// <summary>
        /// Fills all the elements of a typed array from a start index to an end index with a
        /// static value.
        /// </summary>
        /// <param name="value"> The value to fill the typed array with. </param>
        /// <param name="start"> Optional. Start index. Defaults to 0. </param>
        /// <param name="end"> Optional. End index (exclusive). Defaults to the length of the array. </param>
        /// <returns> The array that is being operated on. </returns>
        public ObjectInstance Fill(T value, int start, int end)
        {
            // Negative values represent offsets from the end of the array.
            start = start < 0 ? Math.Max(Length + start, 0) : Math.Min(start, Length);
            end = end < 0 ? Math.Max(Length + end, 0) : Math.Min(end, Length);

            for (; start < end; start ++)
            {
                this[start] = value;
            }

            return WrappedInstance;
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
        public int FindIndex(FunctionInstance callbackFunction, ObjectInstance context)
        {
            for (int i = 0; i < Length; i++)
            {
                // Get the value of the array element.
                object elementValue = this[i];

                // Only call the callback function for array elements that exist in the array.
                if (elementValue != null)
                {
                    // Call the callback function.
                    bool result = TypeConverter.ToBoolean(callbackFunction.CallFromNative("findIndex", context, elementValue, i, WrappedInstance));

                    // Return if the result was true.
                    if (result == true)
                        return i;
                }
            }

            // No matches, return undefined.
            return -1;
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Sorts a array using the quicksort algorithm.
        /// </summary>
        /// <param name="comparer"> A comparison function. </param>
        /// <param name="start"> The first index in the range. </param>
        /// <param name="end"> The last index in the range. </param>
        private void QuickSort(Func<T, T, int> comparer, int start, int end)
        {
            if (end - start < 30)
            {
                // Insertion sort is faster than quick sort for small arrays.
                InsertionSort(comparer, start, end);
                return;
            }

            // Choose a random pivot.
            int pivotIndex = start + (int)(MathObject.Random() * (end - start));

            // Get the pivot value.
            T pivotValue = this[pivotIndex];

            // Send the pivot to the back.
            Swap(pivotIndex, end);

            // Sweep all the low values to the front of the array and the high values to the back
            // of the array.  This version of quicksort never gets into an infinite loop even if
            // the comparer function is not consistent.
            int newPivotIndex = start;
            for (int i = start; i < end; i++)
            {
                if (comparer(this[i], pivotValue) <= 0)
                {
                    Swap(i, newPivotIndex);
                    newPivotIndex++;
                }
            }

            // Swap the pivot back to where it belongs.
            Swap(end, newPivotIndex);

            // Quick sort the array to the left of the pivot.
            if (newPivotIndex > start)
                QuickSort(comparer, start, newPivotIndex - 1);

            // Quick sort the array to the right of the pivot.
            if (newPivotIndex < end)
                QuickSort(comparer, newPivotIndex + 1, end);
        }

        /// <summary>
        /// Sorts a array using the insertion sort algorithm.
        /// </summary>
        /// <param name="comparer"> A comparison function. </param>
        /// <param name="start"> The first index in the range. </param>
        /// <param name="end"> The last index in the range. </param>
        private void InsertionSort(Func<T, T, int> comparer, int start, int end)
        {
            for (int i = start + 1; i <= end; i++)
            {
                T value = this[i];
                int j;
                for (j = i - 1; j > start && comparer(this[j], value) > 0; j--)
                    this[j + 1] = this[j];

                // Normally the for loop above would continue until j < start but since we are
                // using uint it doesn't work when start == 0.  Therefore the for loop stops one
                // short of start then the extra loop iteration runs below.
                if (j == start && comparer(this[j], value) > 0)
                {
                    this[j + 1] = this[j];
                    j--;
                }

                this[j + 1] = value;
            }
        }

        /// <summary>
        /// Swaps the elements at two locations in the array.
        /// </summary>
        /// <param name="index1"> The location of the first element. </param>
        /// <param name="index2"> The location of the second element. </param>
        private void Swap(int index1, int index2)
        {
            T temp = this[index1];
            this[index1] = this[index2];
            this[index2] = temp;
        }
    }
}
