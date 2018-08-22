using System.Diagnostics;
using System.Linq;


namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for StringIterator
    /// </summary>
    public class StringIteratorDebugView
    {
        /// <summary>
        /// The displayed stringIterator
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private StringIterator stringIterator;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="stringIterator">The displayed stringIterator</param>
        internal StringIteratorDebugView(StringIterator stringIterator)
        {
            this.stringIterator = stringIterator;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.stringIterator.Prototype;
            }
        }

        /// <summary>
        /// StringIterator properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.stringIterator.Properties.ToArray();
            }
        }

        /// <summary>
        /// Gets if iterator hs more to iterate
        /// </summary>
        public bool IteratorHasMore
        {
            get { return !this.stringIterator.Done; }
        }

        /// <summary>
        /// Gets the iterator index
        /// </summary>
        public int IteratorIndex
        {
            get { return this.stringIterator.IteratorIndex; }
        }
    }
}
