using System.Diagnostics;
using System.Linq;


namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for WeakSetInstance
    /// </summary>
    public class WeakSetInstanceDebugView
    {
        /// <summary>
        /// The displayed WeakSetInstance
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private WeakSetInstance weakSetInstance;

        /// <summary>
        /// The debugger decorator for WeakSetInstance entries
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected WeakSetEntriesDebugView setEntries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weakSetInstance">The displayed WeakSetInstance</param>
        internal WeakSetInstanceDebugView(WeakSetInstance weakSetInstance)
        {
            this.weakSetInstance = weakSetInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.weakSetInstance.Prototype;
            }
        }

        /// <summary>
        /// WeakSetInstance properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.weakSetInstance.Properties.ToArray();
            }
        }

        /// <summary>
        /// Gets the debugger decorator for WeakSetInstance entries
        /// </summary>
        public WeakSetEntriesDebugView Entries
        {
            get
            {
                if (this.setEntries == null)
                    this.setEntries = new WeakSetEntriesDebugView(this.weakSetInstance.Store);

                return this.setEntries;
            }
        }
    }
}
