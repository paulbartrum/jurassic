using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// The DataView view provides a low-level interface for reading and writing multiple number
    /// types in an ArrayBuffer irrespective of the platform's endianness.
    /// </summary>
    [Serializable]
    public partial class DataViewConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new DataView constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal DataViewConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>(3);
            InitializeConstructorProperties(properties, "DataView", 3, new DataViewInstance(this));
            FastSetProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the DataView object is invoked like a function, e.g. var x = DataView().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(Engine, "TypeError", "Constructor DataView requires 'new'");
        }

        /// <summary>
        /// Creates a new DataView.
        /// </summary>
        /// <param name="buffer"> An existing ArrayBuffer to use as the storage for the new
        /// DataView object. </param>
        /// <param name="byteOffset"> The offset, in bytes, to the first byte in the specified
        /// buffer for the new view to reference. If not specified, the view of the buffer will
        /// start with the first byte. </param>
        /// <param name="byteLength"> The number of elements in the byte array. If unspecified,
        /// length of the view will match the buffer's length. </param>
        /// <returns> A new DataView object of the specified size. </returns>
        [JSConstructorFunction]
        public DataViewInstance Construct(ArrayBufferInstance buffer = null, int byteOffset = 0, int? byteLength = null)
        {
            if (buffer == null)
                throw new JavaScriptException(Engine, "TypeError", "First argument to DataView constructor must be an ArrayBuffer.");
            if (byteOffset >= buffer.ByteLength)
                throw new JavaScriptException(Engine, "RangeError", "Start offset is outside the bounds of the buffer.");
            int byteLengthValue = byteLength ?? buffer.ByteLength - byteOffset;
            if (byteOffset + byteLengthValue > buffer.ByteLength)
                throw new JavaScriptException(Engine, "RangeError", "Invalid data view length.");
            return new DataViewInstance(this.InstancePrototype, buffer, byteOffset, byteLengthValue);
        }
    }
}
