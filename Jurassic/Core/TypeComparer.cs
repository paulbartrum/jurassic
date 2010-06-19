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
            object x2, y2;
            if (leftFirst == true)
            {
                x2 = TypeConverter.ToPrimitive(x, PrimitiveTypeHint.Number);
                y2 = TypeConverter.ToPrimitive(y, PrimitiveTypeHint.Number);
            }
            else
            {
                y2 = TypeConverter.ToPrimitive(y, PrimitiveTypeHint.Number);
                x2 = TypeConverter.ToPrimitive(x, PrimitiveTypeHint.Number);
            }

            if (x2 is string && y is string)
            {
                return string.CompareOrdinal((string)x2, (string)y2) < 0;
            }
            else
            {
                return TypeConverter.ToNumber(x2) < TypeConverter.ToNumber(y2);
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
            object x2, y2;
            if (leftFirst == true)
            {
                x2 = TypeConverter.ToPrimitive(x, PrimitiveTypeHint.Number);
                y2 = TypeConverter.ToPrimitive(y, PrimitiveTypeHint.Number);
            }
            else
            {
                y2 = TypeConverter.ToPrimitive(y, PrimitiveTypeHint.Number);
                x2 = TypeConverter.ToPrimitive(x, PrimitiveTypeHint.Number);
            }

            if (x2 is string && y is string)
            {
                return string.CompareOrdinal((string)x2, (string)y2) <= 0;
            }
            else
            {
                return TypeConverter.ToNumber(x2) <= TypeConverter.ToNumber(y2);
            }
        }
    }
}
