using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for SetInstance entries - key-value pairs.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    internal class SetEntriesDebugView : IDebuggerDisplay
    {
        /// <summary>
        /// The SetInstance internal strorage
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<object, LinkedListNode<object>> setStore;

        /// <summary>
        /// Key-value pairs
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PropertyNameAndValue[] values;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="setStore">The SetInstance internal strorage</param>
        public SetEntriesDebugView(Dictionary<object, LinkedListNode<object>> setStore)
        {
            this.setStore = setStore;
        }

        private void InitValues()
        {
            if (this.values == null)
            {
                this.values = new PropertyNameAndValue[this.setStore.Count];
                int idx = 0;
                foreach (LinkedListNode<object> value in this.setStore.Values)
                {
                    this.values[idx] = new PropertyNameAndValue(idx++.ToString(CultureInfo.InvariantCulture),
                        value.Value, PropertyAttributes.FullAccess);
                }
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayValue
        {
            get
            {
                InitValues();
                IEnumerable<string> strValues =
                    this.values.Select(pnv => DebuggerDisplayHelper.ShortStringRepresentation(pnv.Value));

                return string.Format("[{0}]", string.Join(", ", strValues));
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayShortValue
        {
            get { return string.Format("Array({0})", this.setStore.Count); }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayType
        {
            get { return this.DebuggerDisplayShortValue; }
        }

        /// <summary>
        /// Gets the key-value pairs
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public PropertyNameAndValue[] Values
        {
            get
            {
                InitValues();

                return this.values;
            }
        }

        /// <summary>
        /// Gets the set entries count
        /// </summary>
        public int length
        {
            get { return this.setStore.Count; }
        }
    }
}
