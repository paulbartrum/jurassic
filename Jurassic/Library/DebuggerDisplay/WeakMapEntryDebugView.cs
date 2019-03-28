using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for WeakMapInstance entry - key-value pair
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    public class WeakMapEntryDebugView : IDebuggerDisplay
    {
        /// <summary>
        /// The key
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ObjectInstance key;

        /// <summary>
        /// The value
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object value;

        /// <summary>
        /// The key-value pair
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PropertyNameAndValue[] keyAndValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">The key as JS Object</param>
        /// <param name="value">The value</param>
        public WeakMapEntryDebugView(ObjectInstance key, object value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayValue
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.AppendFormat("{0} => {1}",
                    DebuggerDisplayHelper.ShortStringRepresentation(this.key),
                    DebuggerDisplayHelper.ShortStringRepresentation(this.value));
                sb.Append("}");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayValue; }
        }

        /// <summary>
        /// Gets the key-value pairs
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayType
        {
            get { return "WeakMap Entry"; }
        }

        /// <summary>
        /// Gets the key and value as Array
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public PropertyNameAndValue[] KeyAndValue
        {
            get
            {
                if (this.keyAndValue == null)
                {
                    this.keyAndValue = new PropertyNameAndValue[2];
                    this.keyAndValue[0] = new PropertyNameAndValue("key", this.key, PropertyAttributes.FullAccess);
                    this.keyAndValue[1] = new PropertyNameAndValue("value", this.value, PropertyAttributes.FullAccess);
                }

                return this.keyAndValue;
            }
        }
    }
}
