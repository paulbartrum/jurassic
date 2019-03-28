using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for WeakSetInstance entries
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    internal class WeakSetEntriesDebugView : IDebuggerDisplay
    {
        /// <summary>
        /// The WeakSetInstance internal storage
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ConditionalWeakTable<ObjectInstance, object> weakSetStore;

        /// <summary>
        /// The WeakSetInstance internal storage keys
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerable<ObjectInstance> keys;

        /// <summary>
        /// The WeakSetInstance entries count
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int entriesCount;

        /// <summary>
        /// The WeakSetInstance values Array
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PropertyNameAndValue[] values;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weakSetStore">The WeakSetInstance internal storage</param>
        public WeakSetEntriesDebugView(ConditionalWeakTable<ObjectInstance, object> weakSetStore)
        {
            this.weakSetStore = weakSetStore;

            this.keys = weakSetStore.GetKeys();
            this.entriesCount = this.keys.Count();
        }

        private void InitValues()
        {
            if (this.values == null)
            {
                this.values = new PropertyNameAndValue[this.entriesCount];
                int idx = 0;
                foreach (ObjectInstance key in keys)
                {
                    object dummyValue;
                    if (this.weakSetStore.TryGetValue(key, out dummyValue))
                    {
                        this.values[idx] = new PropertyNameAndValue(idx++.ToString(CultureInfo.InvariantCulture),
                            key, PropertyAttributes.FullAccess);
                    }
                }
            }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
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
            get { return string.Format("Array({0})", this.entriesCount); }
        }

        /// <summary>
        /// Gets the key-value pairs
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayType
        {
            get { return this.DebuggerDisplayShortValue; }
        }

        /// <summary>
        /// Gets the WeakSetInstance values
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
        /// Gets the WeakSetInstance elements count
        /// </summary>
        public int length
        {
            get { return this.entriesCount; }
        }
    }
}
