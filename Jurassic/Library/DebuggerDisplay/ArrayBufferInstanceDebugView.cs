using System.Diagnostics;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for ArrayBufferInstance
    /// </summary>
    internal class ArrayBufferInstanceDebugView
    {
        /// <summary>
        /// The displayed ArrayBufferInstance
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ArrayBufferInstance arrayBufferInstance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="arrayBufferInstance">The displayed ArrayBufferInstance</param>
        public ArrayBufferInstanceDebugView(ArrayBufferInstance arrayBufferInstance)
        {
            this.arrayBufferInstance = arrayBufferInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.arrayBufferInstance.Prototype;
            }
        }

        /// <summary>
        /// ArrayBufferInstance properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.arrayBufferInstance.Properties.ToArray();
            }
        }

        /// <summary>
        /// Gets the length of ArrayBuffer in bytes
        /// </summary>
        public int byteLength
        {
            get { return this.arrayBufferInstance.Buffer.Length; }
        }

        /// <summary>
        /// Gets the bytes array
        /// </summary>
        public byte[] Int8Array
        {
            get { return this.arrayBufferInstance.Buffer; }
        }
    }
}
