using System.Diagnostics;
using System.Globalization;
using System.Linq;


namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for TypedArrayInstance
    /// </summary>
    public class TypedArrayInstanceDebugView
    {
        /// <summary>
        /// The displayed TypedArrayInstance
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected TypedArrayInstance typedArrayInstance;

        /// <summary>
        /// All values
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PropertyNameAndValue[] values;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typedArrayInstance">The displayed TypedArrayInstance</param>
        public TypedArrayInstanceDebugView(TypedArrayInstance typedArrayInstance)
        {
            this.typedArrayInstance = typedArrayInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.typedArrayInstance.Prototype;
            }
        }

        /// <summary>
        /// TypedArrayInstance properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.typedArrayInstance.Properties.ToArray();
            }
        }

        /// <summary>
        /// Gets the internal buffer
        /// </summary>
        public ArrayBufferInstance buffer
        {
            get { return this.typedArrayInstance.Buffer; }
        }

        /// <summary>
        /// Gets the klength in bytes
        /// </summary>
        public int byteLength
        {
            get { return this.typedArrayInstance.Buffer.Buffer.Length; }
        }

        /// <summary>
        /// Gets the buffer offset
        /// </summary>
        public int byteOffset
        {
            get { return this.typedArrayInstance.ByteOffset; }
        }

        /// <summary>
        /// Gets length of array in elements
        /// </summary>
        public int length
        {
            get { return this.typedArrayInstance.Length; }
        }

        /// <summary>
        /// Gets the number of bytes per element
        /// </summary>
        public int BytesPerElement
        {
            get { return this.typedArrayInstance.BytesPerElement; }
        }

        /// <summary>
        /// Gets the array elements
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public PropertyNameAndValue[] Values
        {
            get
            {
                if (this.values == null)
                {
                    this.values = new PropertyNameAndValue[this.length];
                    for (int i = 0; i < this.length; i++)
                    {
                        this.values[i] = new PropertyNameAndValue(i.ToString(CultureInfo.InvariantCulture),
                            this.typedArrayInstance[i], PropertyAttributes.FullAccess);
                    }
                }

                return this.values;
            }
        }
    }
}
