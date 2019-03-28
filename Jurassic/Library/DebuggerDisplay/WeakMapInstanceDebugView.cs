using System.Diagnostics;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for WeakMapInstance entry
    /// </summary>
    internal class WeakMapInstanceDebugView
    {
        /// <summary>
        /// The displayed WeakMapInstance
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected WeakMapInstance weakMapInstance;

        /// <summary>
        /// The WeakMapInstance entries debugger decorator
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected WeakMapEntriesDebugView mapEntries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weakMapInstance">The displayed WeakMapInstance</param>
        public WeakMapInstanceDebugView(WeakMapInstance weakMapInstance)
        {
            this.weakMapInstance = weakMapInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.weakMapInstance.Prototype;
            }
        }

        /// <summary>
        /// WeakMapInstance properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.weakMapInstance.Properties.ToArray();
            }
        }

        /// <summary>
        /// Gets the WeakMapInstance entries debugger decorator
        /// </summary>
        public WeakMapEntriesDebugView Entries
        {
            get
            {
                if (this.mapEntries == null)
                    this.mapEntries = new WeakMapEntriesDebugView(this.weakMapInstance.Store);

                return this.mapEntries;
            }
        }
    }
}
