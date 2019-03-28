using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for MapIterator entries - key or value.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    internal class MapKeyOrValuesDebugView : IDebuggerDisplay
    {
        /// <summary>
        /// Internal Map storage
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<object, LinkedListNode<KeyValuePair<object, object>>> mapStore;

        /// <summary>
        /// Entries
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PropertyNameAndValue[] values;


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool displayKeys;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mapStore">Internal Map storage</param>
        /// <param name="displayKeys">What to display - keys or values</param>
        public MapKeyOrValuesDebugView(Dictionary<object, LinkedListNode<KeyValuePair<object, object>>> mapStore, bool displayKeys)
        {
            this.mapStore = mapStore;
            this.displayKeys = displayKeys;
        }

        private void InitValues()
        {
            if (this.values == null)
            {
                this.values = new PropertyNameAndValue[this.mapStore.Count];
                int idx = 0;
                foreach (LinkedListNode<KeyValuePair<object, object>> value in this.mapStore.Values)
                {
                    this.values[idx] = new PropertyNameAndValue(idx++.ToString(CultureInfo.InvariantCulture),
                        this.displayKeys ? value.Value.Key : value.Value.Value, PropertyAttributes.FullAccess);
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
                this.InitValues();

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
            get { return string.Format("Array({0})", this.mapStore.Count); }
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
        /// All keys or all values shown as key-value list. List keys are consecutive numbers
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public PropertyNameAndValue[] Values
        {
            get
            {
                this.InitValues();

                return this.values;
            }
        }

        /// <summary>
        /// Map entries count
        /// </summary>
        public int length
        {
            get { return this.mapStore.Count; }
        }
    }
}
