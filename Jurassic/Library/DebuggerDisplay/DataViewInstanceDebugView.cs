using System.Diagnostics;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for DataViewInstance
    /// </summary>
    internal class DataViewInstanceDebugView
    {
        /// <summary>
        /// The displayed DataViewInstance
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected DataViewInstance dataViewInstance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataViewInstance">The displayed DataViewInstance</param>
        public DataViewInstanceDebugView(DataViewInstance dataViewInstance)
        {
            this.dataViewInstance = dataViewInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.dataViewInstance.Prototype;
            }
        }

        /// <summary>
        /// DataViewInstance properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.dataViewInstance.Properties.ToArray();
            }
        }

        /// <summary>
        /// Gets the internal buffer
        /// </summary>
        public ArrayBufferInstance buffer
        {
            get { return this.dataViewInstance.Buffer; }
        }

        /// <summary>
        /// Gets the buffer length in bytes
        /// </summary>
        public int byteLength
        {
            get { return this.dataViewInstance.Buffer.Buffer.Length; }
        }

        /// <summary>
        /// Gets the buffer offset
        /// </summary>
        public int byteOffset
        {
            get { return this.dataViewInstance.ByteOffset; }
        }
    }
}
