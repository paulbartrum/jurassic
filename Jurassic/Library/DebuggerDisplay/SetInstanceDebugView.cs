using System.Diagnostics;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for SetInstance
    /// </summary>
    internal class SetInstanceDebugView
    {
        /// <summary>
        /// The displayed SetInstance
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SetInstance setInstance;

        /// <summary>
        /// Debugger for key-value entries
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected SetEntriesDebugView setEntries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="setInstance">The displayed SetInstance</param>
        internal SetInstanceDebugView(SetInstance setInstance)
        {
            this.setInstance = setInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.setInstance.Prototype;
            }
        }

        /// <summary>
        /// Set properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.setInstance.Properties.ToArray();
            }
        }

        /// <summary>
        /// Gets key-value entries
        /// </summary>
        public SetEntriesDebugView Entries
        {
            get
            {
                if (this.setEntries == null)
                    this.setEntries = new SetEntriesDebugView(this.setInstance.Store);

                return this.setEntries;
            }
        }

        /// <summary>
        /// Gets the Set entries count
        /// </summary>
        public int size
        {
            get { return this.setInstance.Store.Count; }
        }
    }
}
