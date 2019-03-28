using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for WeakMapInstance entries
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    internal class WeakMapEntriesDebugView : IDebuggerDisplay
    {
        /// <summary>
        /// The WeakMapInstance internal storage
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ConditionalWeakTable<ObjectInstance, object> weakMapStore;

        /// <summary>
        /// The WeakMapInstance internal storage keys
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerable<ObjectInstance> keys;

        /// <summary>
        /// The WeakMapInstance entries debugger decorator
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private WeakMapEntryDebugView[] entries;

        /// <summary>
        /// The WeakMapInstance entries count
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int entriesCount;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weakMapStore">The WeakMapInstance internal storage</param>
        public WeakMapEntriesDebugView(ConditionalWeakTable<ObjectInstance, object> weakMapStore)
        {
            this.weakMapStore = weakMapStore;

            this.keys = this.weakMapStore.GetKeys();
            this.entriesCount = this.keys.Count();
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
                sb.Append("[");
                DebuggerDisplayHelper.WeakMapRepresentation(sb, this.weakMapStore);
                sb.Append("]");
                return sb.ToString();
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
        /// Gets the WeakMapInstance entries debugger decorator
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public WeakMapEntryDebugView[] Entries
        {
            get
            {
                if (this.entries == null)
                {
                    this.entries = new WeakMapEntryDebugView[this.entriesCount];
                    int idx = 0;
                    foreach (ObjectInstance key in this.keys)
                    {
                        object value;
                        if (this.weakMapStore.TryGetValue(key, out value))
                        {
                            this.entries[idx++] = new WeakMapEntryDebugView(key, value);
                        }
                    }
                }

                return this.entries;
            }
        }

        /// <summary>
        /// Gets the WeakMapInstance entries count
        /// </summary>
        public int length
        {
            get { return this.entriesCount; }
        }
    }
}
