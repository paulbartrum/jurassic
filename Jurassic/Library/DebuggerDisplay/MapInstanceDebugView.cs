using System.Diagnostics;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for MapInstance.
    /// </summary>
    internal class MapInstanceDebugView
    {
        /// <summary>
        /// The displayed map
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected MapInstance mapInstance;

        /// <summary>
        /// All entries
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected MapEntriesDebugView mapEntries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mapInstance">The displayed map</param>
        public MapInstanceDebugView(MapInstance mapInstance)
        {
            this.mapInstance = mapInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.mapInstance.Prototype;
            }
        }

        /// <summary>
        /// Map entries count
        /// </summary>
        public int size
        {
            get
            {
                return this.mapInstance.Store.Count;
            }
        }

        /// <summary>
        /// Map properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.mapInstance.Properties.ToArray();
            }
        }

        /// <summary>
        /// Map entries list
        /// </summary>
        public MapEntriesDebugView Entries
        {
            get
            {
                if (this.mapEntries == null)
                    this.mapEntries = new MapEntriesDebugView(this.mapInstance.Store);

                return this.mapEntries;
            }
        }
    }
}
