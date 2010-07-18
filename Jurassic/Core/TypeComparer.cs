using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jurassic
{

    /// <summary>
    /// Implements the JavaScript equality and comparison rules.
    /// </summary>
    public static class TypeComparer
    {
        /// <summary>
        /// Compares two objects for equality.  Used by the equality operator (==).
        /// </summary>
        /// <param name="x"> The first object to compare. </param>
        /// <param name="y"> The first object to compare. </param>
        /// <returns> <c>true</c> if the objects are identical; <c>false</c> otherwise. </returns>
        public new static bool Equals(object x, object y)
        {
            x = x ?? Undefined.Value;
            y = y ?? Undefined.Value;
            if (x.GetType() == y.GetType())
            {
                if (x is double && double.IsNaN((double)x) == true)
                    return false;
                return object.Equals(x, y);
            }
            if ((x == Undefined.Value && y == Null.Value) || (x == Null.Value && y == Undefined.Value))
                return true;
            return TypeConverter.ToNumber(x) == TypeConverter.ToNumber(y);
        }

        /// <summary>
        /// Compares two objects for equality.  Used by the strict equality operator (===).
        /// </summary>
        /// <param name="x"> The first object to compare. </param>
        /// <param name="y"> The second object to compare. </param>
        /// <returns> <c>true</c> if the objects are identical; <c>false</c> otherwise. </returns>
        public static bool StrictEquals(object x, object y)
        {
            x = x ?? Undefined.Value;
            y = y ?? Undefined.Value;
            if (x is int)
                x = (double)(int)x;
            if (y is int)
                y = (double)(int)y;
            if (x is double && double.IsNaN((double)x) == true)
                return false;
            return object.Equals(x, y);
        }

        /// <summary>
        /// Determines the ordering of two objects.  Used by the less than operator (&lt;).
        /// </summary>
        /// <param name="x"> The first object to compare. </param>
        /// <param name="y"> The second object to compare. </param>
        /// <param name="leftFirst"> <c>true</c> if <paramref name="x"/> is evaluated first;
        /// <c>false</c> otherwise. </param>
        /// <returns> <c>true</c> if <paramref name="x"/> is less than <paramref name="y"/>;
        /// <c>false</c> otherwise. </returns>
        public static bool LessThan(object x, object y, bool leftFirst)
        {
            if (leftFirst == true)
            {
                x = TypeConverter.ToPrimitive(x, PrimitiveTypeHint.Number);
                y = TypeConverter.ToPrimitive(y, PrimitiveTypeHint.Number);
            }
            else
            {
                y = TypeConverter.ToPrimitive(y, PrimitiveTypeHint.Number);
                x = TypeConverter.ToPrimitive(x, PrimitiveTypeHint.Number);
            }

            if ((x is string || x is ConcatenatedString) && (y is string || y is ConcatenatedString))
            {
                if (x is ConcatenatedString)
                    x = ((ConcatenatedString)x).ToString();
                if (y is ConcatenatedString)
                    y = ((ConcatenatedString)y).ToString();
                return string.CompareOrdinal((string)x, (string)y) < 0;
            }
            else
            {
                return TypeConverter.ToNumber(x) < TypeConverter.ToNumber(y);
            }
        }

        /// <summary>
        /// Determines the ordering of two objects.  Used by the less than or equal operator (&lt;=).
        /// </summary>
        /// <param name="x"> The first object to compare. </param>
        /// <param name="y"> The second object to compare. </param>
        /// <param name="leftFirst"> <c>true</c> if <paramref name="x"/> is evaluated first;
        /// <c>false</c> otherwise. </param>
        /// <returns> <c>true</c> if <paramref name="x"/> is less than or equal to
        /// <paramref name="y"/>; <c>false</c> otherwise. </returns>
        public static bool LessThanOrEqual(object x, object y, bool leftFirst)
        {
            if (leftFirst == true)
            {
                x = TypeConverter.ToPrimitive(x, PrimitiveTypeHint.Number);
                y = TypeConverter.ToPrimitive(y, PrimitiveTypeHint.Number);
            }
            else
            {
                y = TypeConverter.ToPrimitive(y, PrimitiveTypeHint.Number);
                x = TypeConverter.ToPrimitive(x, PrimitiveTypeHint.Number);
            }

            if ((x is string || x is ConcatenatedString) && (y is string || y is ConcatenatedString))
            {
                if (x is ConcatenatedString)
                    x = ((ConcatenatedString)x).ToString();
                if (y is ConcatenatedString)
                    y = ((ConcatenatedString)y).ToString();
                return string.CompareOrdinal((string)x, (string)y) <= 0;
            }
            else
            {
                return TypeConverter.ToNumber(x) <= TypeConverter.ToNumber(y);
            }
        }
    }
}
