using System;
using System.Collections.Generic;
using Jurassic.Library;

namespace Jurassic
{

    /// <summary>
    /// Represents a string that is composed of one or more strings appended together.  This class
    /// is immutable.
    /// </summary>
    internal sealed class ConcatenatedString
    {
        private string[] strings;
        private int count;
        private int length = -1;

        /// <summary>
        /// Creates a ConcatenatedString instance with an initial value.
        /// </summary>
        /// <param name="initialValue"> The initial value. </param>
        public ConcatenatedString(string initialValue)
        {
            if (initialValue == null)
                throw new ArgumentNullException("initialValue");
            this.strings = new string[10];
            if (initialValue.Length > 0)
            {
                this.strings[0] = initialValue;
                this.count = 1;
            }
        }
        
        /// <summary>
        /// Creates a new ConcatenatedString instance.
        /// </summary>
        /// <param name="strings"> An array of strings. </param>
        /// <param name="count"> The number of strings that are valid in the array. </param>
        private ConcatenatedString(string[] strings, int count)
        {
            this.strings = strings;
            this.count = count;
        }

        /// <summary>
        /// Gets the number of characters in this ConcatenatedString.
        /// </summary>
        public int Length
        {
            get
            {
                if (this.length == -1)
                {
                    var length = 0;
                    for (int i = 0; i < this.count; i++)
                        length += this.strings[i].Length;
                    this.length = length;
                }
                return this.length;
            }
        }

        /// <summary>
        /// Appends a string to the end of this StringBuilder.
        /// </summary>
        /// <param name="value"> The string to append. </param>
        /// <returns> A new StringBuilder with the string appended. </returns>
        public ConcatenatedString Append(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            var strings = this.strings;
            if (this.count == strings.Length)
                Array.Resize(ref strings, this.strings.Length * 2);
            strings[this.count] = value;
            return new ConcatenatedString(strings, this.count + 1);
        }

        /// <summary>
        /// Appends a ConcatenatedString to the end of this ConcatenatedString.
        /// </summary>
        /// <param name="value"> The ConcatenatedString to append. </param>
        /// <returns> A new ConcatenatedString with the ConcatenatedString appended. </returns>
        public ConcatenatedString Append(ConcatenatedString value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            var result = this;
            for (int i = 0; i < value.count; i++)
                result = result.Append(value.strings[i]);
            return result;
        }

        /// <summary>
        /// Converts the ConcatenatedString to a string.
        /// </summary>
        /// <returns> The string representation of this object. </returns>
        public override string ToString()
        {
            bool truncateNecessary = false;
            for (int i = this.count; i < this.strings.Length; i++)
                if (this.strings[i] != null)
                    truncateNecessary = true;
            var stringsArray = this.strings;
            if (truncateNecessary == true)
            {
                stringsArray = new string[this.count];
                Array.Copy(this.strings, stringsArray, this.count);
            }
            return string.Concat(stringsArray);
        }
    }

}
