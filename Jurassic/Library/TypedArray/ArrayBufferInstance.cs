using System;

namespace Jurassic.Library
{
    /// <summary>
    /// The ArrayBuffer object is used to represent a generic, fixed-length raw binary data buffer.
    /// You can not directly manipulate the contents of an ArrayBuffer; instead, you create one of
    /// the typed array objects or a DataView object which represents the buffer in a specific
    /// format, and use that to read and write the contents of the buffer.
    /// </summary>
    [Serializable]
    public partial class ArrayBufferInstance : ObjectInstance
    {
        private byte[] buffer;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates an empty ArrayBuffer instance for use as a prototype.
        /// </summary>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal ArrayBufferInstance(ArrayBufferConstructor constructor)
            : base(constructor.Engine.Object.InstancePrototype)
        {
            // Initialize the prototype properties.
            var properties = GetDeclarativeProperties();
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            FastSetProperties(properties);
        }

        /// <summary>
        /// Creates a new ArrayBuffer instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="size"> The size, in bytes, of the array buffer to create. </param>
        public ArrayBufferInstance(ObjectInstance prototype, int size)
            : base(prototype)
        {
            this.buffer = new byte[size];
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "ArrayBuffer"; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a new ArrayBuffer whose contents are a copy of this ArrayBuffer's bytes from
        /// begin, inclusive, up to end, exclusive.
        /// </summary>
        /// <param name="begin"> Zero-based byte index at which to begin slicing. </param>
        /// <param name="end"> Byte index to end slicing. If end is unspecified, the new
        /// ArrayBuffer contains all bytes from begin to the end of this ArrayBuffer. The range
        /// specified by the begin and end values is clamped to the valid index range for the
        /// current array. If the computed length of the new ArrayBuffer would be negative, it is
        /// clamped to zero. </param>
        /// <returns> A new ArrayBuffer object. </returns>
        [JSInternalFunction(Name = "slice")]
        public static string Slice(int begin, int end)
        {
            throw new NotImplementedException();
        }

    }
}
