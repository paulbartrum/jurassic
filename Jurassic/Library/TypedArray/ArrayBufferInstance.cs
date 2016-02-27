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

        /// <summary>
        /// Creates a new ArrayBuffer instance from an existing buffer.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="buffer"> The buffer to use. </param>
        private ArrayBufferInstance(ObjectInstance prototype, byte[] buffer)
            : base(prototype)
        {
            this.buffer = buffer;
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

        /// <summary>
        /// Gets the internal array for this ArrayBuffer.
        /// </summary>
        internal byte[] Buffer
        {
            get { return this.buffer; }
        }


        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the length of an ArrayBuffer in bytes.  Returns 0 if this ArrayBuffer has been
        /// detached.
        /// </summary>
        [JSProperty(Name = "byteLength")]
        public int ByteLength
        {
            get { return this.buffer.Length; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a new ArrayBuffer whose contents are a copy of this ArrayBuffer's bytes from
        /// begin, inclusive, up to end, exclusive.
        /// </summary>
        /// <param name="begin"> Zero-based byte index at which to begin slicing. </param>
        /// <returns> A new ArrayBuffer object. </returns>
        [JSInternalFunction(Name = "slice")]
        public ArrayBufferInstance Slice(int begin)
        {
            return Slice(begin, this.buffer.Length);
        }

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
        public ArrayBufferInstance Slice(int begin, int end)
        {
            // 1. Let O be the this value.
            // 2. If Type(O) is not Object, throw a TypeError exception.
            // 3. If O does not have an[[ArrayBufferData]] internal slot, throw a TypeError exception.
            // 4. If IsDetachedBuffer(O) is true, throw a TypeError exception.
            // 5. Let len be the value of O’s[[ArrayBufferByteLength]] internal slot.
            // 6. Let relativeStart be ToInteger(start).
            // 7. ReturnIfAbrupt(relativeStart).
            // 8. If relativeStart< 0, let first be max((len + relativeStart),0); else let first be min(relativeStart, len).
            // 9. If end is undefined, let relativeEnd be len; else let relativeEnd be ToInteger(end).
            // 10. ReturnIfAbrupt(relativeEnd).
            // 11. If relativeEnd< 0, let final be max((len + relativeEnd),0); else let final be min(relativeEnd, len).
            // 12. Let newLen be max(final-first,0).
            // 13. Let ctor be SpeciesConstructor(O, %ArrayBuffer%).
            // 14. ReturnIfAbrupt(ctor).
            // 15. Let new be Construct(ctor, «newLen»).
            // 16. ReturnIfAbrupt(new).
            // 17. If new does not have an[[ArrayBufferData]] internal slot, throw a TypeError exception.
            // 18. If IsDetachedBuffer(new) is true, throw a TypeError exception.
            // 19. If SameValue(new, O) is true, throw a TypeError exception.
            // 20. If the value of new’s[[ArrayBufferByteLength]] internal slot<newLen, throw a TypeError exception.
            // 21. NOTE: Side-effects of the above steps may have detached O.
            // 22. If IsDetachedBuffer(O) is true, throw a TypeError exception.
            // 23. Let fromBuf be the value of O’s[[ArrayBufferData]] internal slot.
            // 24. Let toBuf be the value of new’s[[ArrayBufferData]] internal slot.
            // 25. Perform CopyDataBlockBytes(toBuf, 0, fromBuf, first, newLen).
            // 26. Return new.

            if (begin < 0)
                begin += this.buffer.Length;
            begin = Math.Min(Math.Max(begin, 0), this.buffer.Length);
            if (end < 0)
                end += this.buffer.Length;
            end = Math.Min(Math.Max(end, begin), this.buffer.Length);

            byte[] newBuffer = new byte[end - begin];
            Array.Copy(this.buffer, begin, newBuffer, 0, end - begin);
            return new ArrayBufferInstance(this.Prototype, newBuffer);
        }
    }
}
