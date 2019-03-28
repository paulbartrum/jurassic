using System.Diagnostics;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for SetIterator
    /// </summary>
    internal class SetIteratorDebugView
    {
        /// <summary>
        /// The displayed SetIterator
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SetIterator setIterator;

        /// <summary>
        /// Debugger for Set key-value entries
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected SetEntriesDebugView iteratorEntries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="setIterator">The displayed SetIterator</param>
        internal SetIteratorDebugView(SetIterator setIterator)
        {
            this.setIterator = setIterator;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.setIterator.Prototype;
            }
        }

        /// <summary>
        /// SetIterator properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.setIterator.Properties.ToArray();
            }
        }

        /// <summary>
        /// Get if iterator hs more to iterate
        /// </summary>
        public bool IteratorHasMore
        {
            get { return !this.setIterator.Done; }
        }

        /// <summary>
        /// Get the iterator index
        /// </summary>
        public int IteratorIndex
        {
            get { return this.setIterator.IteratorIndex; }
        }

        /// <summary>
        /// Gets the iterator king - key, value or both
        /// </summary>
        public string IteratorKind
        {
            get { return this.setIterator.IteratorKind.ToString(); }
        }

        /// <summary>
        /// Gets the debugger decorator for Set entries
        /// </summary>
        public SetEntriesDebugView Entries
        {
            get
            {
                if (this.iteratorEntries == null)
                    this.iteratorEntries = new SetEntriesDebugView(this.setIterator.Set.Store);

                return this.iteratorEntries;
            }
        }
    }
}
