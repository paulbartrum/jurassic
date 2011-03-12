using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jurassic
{

    /// <summary>
    /// Represents the JavaScript "null" type and provides the one and only instance of that type.
    /// </summary>
    public sealed class Null
    {
        /// <summary>
        /// Creates a new Null instance.
        /// </summary>
        private Null()
        {
        }

        /// <summary>
        /// Gets the one and only "null" instance.
        /// </summary>
        public static readonly Null Value = new Null();

        /// <summary>
        /// Returns a string representing the current object.
        /// </summary>
        /// <returns> A string representing the current object. </returns>
        public override string ToString()
        {
            return "null";
        }
    }

}
