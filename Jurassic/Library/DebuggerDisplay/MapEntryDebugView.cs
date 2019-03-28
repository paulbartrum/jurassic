using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for MapInstance entry.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    internal class MapEntryDebugView : IDebuggerDisplay
    {
        /// <summary>
        /// The displayed entry
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private KeyValuePair<object, object> entry;

        /// <summary>
        /// Key and value as PropertyNameAndValue Array
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PropertyNameAndValue[] keyAndValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entry">Entry key-value pair</param>
        public MapEntryDebugView(KeyValuePair<object, object> entry)
        {
            this.entry = entry;
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayValue
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.AppendFormat("{0} => {1}",
                    DebuggerDisplayHelper.ShortStringRepresentation(this.entry.Key),
                    DebuggerDisplayHelper.ShortStringRepresentation(this.entry.Value));
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
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayType
        {
            get { return "Map Entry"; }
        }


        /// <summary>
        /// Key and value as PropertyNameAndValue Array
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public PropertyNameAndValue[] KeyAndValue
        {
            get
            {
                if (this.keyAndValue == null)
                {
                    this.keyAndValue = new PropertyNameAndValue[2];
                    this.keyAndValue[0] = new PropertyNameAndValue("key", this.entry.Key, PropertyAttributes.FullAccess);
                    this.keyAndValue[1] = new PropertyNameAndValue("value", this.entry.Value, PropertyAttributes.FullAccess);
                }

                return this.keyAndValue;
            }
        }
    }
}
