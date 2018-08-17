using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for MapInstance entries.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    public class MapEntriesDebugView : IDebuggerDisplay
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<object, LinkedListNode<KeyValuePair<object, object>>> mapStore;


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MapEntryDebugView[] entries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mapStore">Internal storage of the MapInstance</param>
        public MapEntriesDebugView(Dictionary<object, LinkedListNode<KeyValuePair<object, object>>> mapStore)
        {
            this.mapStore = mapStore;
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayValue
        {
            get
            {
                return string.Format("[{0}]", DebuggerDisplayHelper.MapRepresentation(this.mapStore));
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
        /// The MapInstance entries
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public MapEntryDebugView[] Entries
        {
            get
            {
                if (this.entries == null)
                {
                    this.entries = new MapEntryDebugView[this.mapStore.Count];
                    int idx = 0;
                    foreach (LinkedListNode<KeyValuePair<object, object>> value in this.mapStore.Values)
                    {
                        this.entries[idx++] = new MapEntryDebugView(value.Value);
                    }
                }

                return this.entries;
            }
        }

        /// <summary>
        /// MapInstance entries count
        /// </summary>
        public int length
        {
            get { return this.mapStore.Count; }
        }
    }
}
