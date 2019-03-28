using System.Diagnostics;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for MapIterator.
    /// </summary>
    internal class MapIteratorDebugView
    {
        /// <summary>
        /// The displayed iterator
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MapIterator mapIterator;

        /// <summary>
        /// Decorator for the entries
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected object iteratorEntries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mapIterator">The displayed iterator</param>
        internal MapIteratorDebugView(MapIterator mapIterator)
        {
            this.mapIterator = mapIterator;
        }


        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.mapIterator.Prototype;
            }
        }

        /// <summary>
        /// MapIterator properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.mapIterator.Properties.ToArray();
            }
        }

        /// <summary>
        /// Gets if iterator has more to iterate
        /// </summary>
        public bool IteratorHasMore
        {
            get { return !this.mapIterator.Done; }
        }

        /// <summary>
        /// Get current index
        /// </summary>
        public int IteratorIndex
        {
            get { return this.mapIterator.IteratorIndex; }
        }

        /// <summary>
        /// Get the iterator king as string - key, value or both
        /// </summary>
        public string IteratorKind
        {
            get { return this.mapIterator.IteratorKind.ToString(); }
        }

        /// <summary>
        /// Gets a decorator for the entries
        /// </summary>
        public object Entries
        {
            get
            {
                if (this.iteratorEntries == null)
                {
                    if (this.mapIterator.IteratorKind == MapIterator.Kind.KeyAndValue)
                    {
                        this.iteratorEntries = new MapEntriesDebugView(this.mapIterator.Map.Store);
                    }
                    else
                    {
                        this.iteratorEntries = new MapKeyOrValuesDebugView(this.mapIterator.Map.Store,
                            this.mapIterator.IteratorKind == MapIterator.Kind.Key);
                    }
                }

                return this.iteratorEntries;
            }
        }
    }
}
