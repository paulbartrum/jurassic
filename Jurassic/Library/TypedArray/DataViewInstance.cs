using System;

namespace Jurassic.Library
{
    /// <summary>
    /// The DataView view provides a low-level interface for reading and writing multiple number
    /// types in an ArrayBuffer irrespective of the platform's endianness.
    /// </summary>
    [Serializable]
    public partial class DataViewInstance : ObjectInstance
    {
        private ArrayBufferInstance buffer;
        private int byteOffset;
        private int byteLength;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates an empty DataView instance for use as a prototype.
        /// </summary>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal DataViewInstance(DataViewConstructor constructor)
            : base(constructor.Engine.Object.InstancePrototype)
        {
            // Initialize the prototype properties.
            var properties = GetDeclarativeProperties(Engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            FastSetProperties(properties);
        }

        /// <summary>
        /// Creates a new DataView instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="buffer"> An existing ArrayBuffer to use as the storage for the new
        /// DataView object. </param>
        /// <param name="byteOffset"> The offset, in bytes, to the first byte in the specified
        /// buffer for the new view to reference. If not specified, the view of the buffer will
        /// start with the first byte. </param>
        /// <param name="byteLength"> The number of elements in the byte array. If unspecified,
        /// length of the view will match the buffer's length. </param>
        internal DataViewInstance(ObjectInstance prototype, ArrayBufferInstance buffer, int byteOffset, int byteLength)
            : base(prototype)
        {
            this.buffer = buffer;
            this.byteOffset = byteOffset;
            this.byteLength = byteLength;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "DataView"; }
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// The ArrayBuffer referenced by the DataView at construction time.
        /// </summary>
        [JSProperty(Name = "buffer")]
        public ArrayBufferInstance Buffer
        {
            get { return this.buffer; }
        }

        /// <summary>
        /// The offset (in bytes) of this view from the start of its ArrayBuffer.
        /// </summary>
        [JSProperty(Name = "byteOffset")]
        public int ByteOffset
        {
            get { return this.byteOffset; }
        }

        /// <summary>
        /// The length (in bytes) of this view from the start of its ArrayBuffer.
        /// </summary>
        [JSProperty(Name = "byteLength")]
        public int ByteLength
        {
            get { return this.byteLength; }
        }




        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets a 32-bit floating point number at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The 32-bit floating point number at the specified byte offset from the start
        /// of the DataView. </returns>
        [JSInternalFunction(Name = "getFloat32", RequiredArgumentCount = 1)]
        public unsafe double GetFloat32(int byteOffset, bool littleEndian)
        {
            int temp = GetInt32(byteOffset, littleEndian);
            return *(float*)&temp;
        }

        /// <summary>
        /// Gets a 64-bit floating point number at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The 64-bit floating point number at the specified byte offset from the start
        /// of the DataView. </returns>
        [JSInternalFunction(Name = "getFloat64", RequiredArgumentCount = 1)]
        public unsafe double GetFloat64(int byteOffset, bool littleEndian)
        {
            long temp = GetInt64(byteOffset, littleEndian);
            return *(double*)&temp;
        }

        /// <summary>
        /// Gets a signed 16-bit integer at the specified byte offset from the start of the DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The signed 16-bit integer at the specified byte offset from the start of the
        /// DataView. </returns>
        [JSInternalFunction(Name = "getInt16", RequiredArgumentCount = 1)]
        public unsafe int GetInt16(int byteOffset, bool littleEndian)
        {
            if (byteOffset < 0 || byteOffset > this.byteLength - 2)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Offset is outside the bounds of the DataView.");
            fixed (byte* ptr = &buffer.Buffer[this.byteOffset + byteOffset])
            {
                if (littleEndian)
                {
                    return (short)((*ptr) | (*(ptr + 1) << 8));
                }
                else
                {
                    return (short)((*ptr << 8) | (*(ptr + 1)));
                }
            }
        }

        /// <summary>
        /// Gets a signed 32-bit integer at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The signed 32-bit integer at the specified byte offset from the start
        /// of the DataView. </returns>
        [JSInternalFunction(Name = "getInt32", RequiredArgumentCount = 1)]
        public unsafe int GetInt32(int byteOffset, bool littleEndian)
        {
            if (byteOffset < 0 || byteOffset > this.byteLength - 4)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Offset is outside the bounds of the DataView.");
            fixed (byte* ptr = &buffer.Buffer[this.byteOffset + byteOffset])
            {
                if (littleEndian)
                {
                    return (*ptr) | (*(ptr + 1) << 8) | (*(ptr + 2) << 16) | (*(ptr + 3) << 24);
                }
                else
                {
                    return (*ptr << 24) | (*(ptr + 1) << 16) | (*(ptr + 2) << 8) | (*(ptr + 3));
                }
            }
        }

        /// <summary>
        /// Gets a signed 64-bit integer at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The signed 64-bit integer at the specified byte offset from the start
        /// of the DataView. </returns>
        public unsafe long GetInt64(int byteOffset, bool littleEndian)
        {
            if (byteOffset < 0 || byteOffset > this.byteLength - 8)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Offset is outside the bounds of the DataView.");
            fixed (byte* ptr = &buffer.Buffer[this.byteOffset + byteOffset])
            {
                if (littleEndian)
                {
                    int temp1 = (*ptr) | (*(ptr + 1) << 8) | (*(ptr + 2) << 16) | (*(ptr + 3) << 24);
                    int temp2 = (*(ptr + 4)) | (*(ptr + 5) << 8) | (*(ptr + 6) << 16) | (*(ptr + 7) << 24);
                    return (uint)temp1 | ((long)temp2 << 32);
                }
                else
                {
                    int temp1 = (*ptr << 24) | (*(ptr + 1) << 16) | (*(ptr + 2) << 8) | (*(ptr + 3));
                    int temp2 = (*(ptr + 4) << 24) | (*(ptr + 5) << 16) | (*(ptr + 6) << 8) | (*(ptr + 7));
                    return (uint)temp2 | ((long)temp1 << 32);
                }
            }
        }

        /// <summary>
        /// Gets a signed 8-bit integer (byte) at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <returns> The signed 8-bit integer (byte) at the specified byte offset from the start
        /// of the DataView. </returns>
        [JSInternalFunction(Name = "getInt8", RequiredArgumentCount = 1)]
        public int GetInt8(int byteOffset)
        {
            if (byteOffset < 0 || byteOffset > this.byteLength - 1)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Offset is outside the bounds of the DataView.");
            return (sbyte)buffer.Buffer[this.byteOffset + byteOffset];
        }

        /// <summary>
        /// Gets an unsigned 8-bit integer (byte) at the specified byte offset from the start of
        /// the DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The unsigned 8-bit integer (byte) at the specified byte offset from the start
        /// of the DataView. </returns>
        [JSInternalFunction(Name = "getUint16", RequiredArgumentCount = 1)]
        public int GetUint16(int byteOffset, bool littleEndian)
        {
            return (ushort)GetInt16(byteOffset, littleEndian);
        }

        /// <summary>
        /// Gets an unsigned 32-bit integer at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The unsigned 32-bit integer at the specified byte offset from the start
        /// of the DataView. </returns>
        [JSInternalFunction(Name = "getUint32", RequiredArgumentCount = 1)]
        public uint GetUint32(int byteOffset, bool littleEndian)
        {
            return (uint)GetInt32(byteOffset, littleEndian);
        }

        /// <summary>
        /// Gets an unsigned 8-bit integer (byte) at the specified byte offset from the start of
        /// the DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <returns> The unsigned 8-bit integer (byte) at the specified byte offset from the start
        /// of the DataView. </returns>
        [JSInternalFunction(Name = "getUint8", RequiredArgumentCount = 1)]
        public int GetUint8(int byteOffset)
        {
            if (byteOffset < 0 || byteOffset > this.byteLength - 1)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Offset is outside the bounds of the DataView.");
            return buffer.Buffer[this.byteOffset + byteOffset];
        }

        /// <summary>
        /// Stores a signed 32-bit float value at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is written. </param>
        [JSInternalFunction(Name = "setFloat32", RequiredArgumentCount = 2)]
        public void SetFloat32(int byteOffset, double value, bool littleEndian)
        {
            SetCore(byteOffset, BitConverter.GetBytes((float)value), littleEndian);
        }

        /// <summary>
        /// Stores a signed 64-bit float value at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="littleEndian"> Indicates whether the 64-bit float is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is written. </param>
        [JSInternalFunction(Name = "setFloat64", RequiredArgumentCount = 2)]
        public void SetFloat64(int byteOffset, double value, bool littleEndian)
        {
            SetCore(byteOffset, BitConverter.GetBytes(value), littleEndian);
        }

        /// <summary>
        /// Stores a signed 16-bit integer value at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is written. </param>
        [JSInternalFunction(Name = "setInt16", RequiredArgumentCount = 2)]
        public void SetInt16(int byteOffset, object value, bool littleEndian)
        {
            SetCore(byteOffset, BitConverter.GetBytes(TypeConverter.ToInt16(value)), littleEndian);
        }

        /// <summary>
        /// Stores a signed 32-bit integer value at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is written. </param>
        [JSInternalFunction(Name = "setInt32", RequiredArgumentCount = 2)]
        public void SetInt32(int byteOffset, object value, bool littleEndian)
        {
            SetCore(byteOffset, BitConverter.GetBytes(TypeConverter.ToInt32(value)), littleEndian);
        }

        /// <summary>
        /// Stores a signed 8-bit integer value at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="value"> The value to set. </param>
        [JSInternalFunction(Name = "setInt8", RequiredArgumentCount = 2)]
        public void SetInt8(int byteOffset, object value)
        {
            if (byteOffset < 0 || byteOffset > this.byteLength - 1)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Offset is outside the bounds of the DataView.");
            buffer.Buffer[this.byteOffset + byteOffset] = (byte)TypeConverter.ToInt8(value);
        }

        /// <summary>
        /// Stores an unsigned 16-bit integer value at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is written. </param>
        [JSInternalFunction(Name = "setUint16", RequiredArgumentCount = 2)]
        public void SetUint16(int byteOffset, object value, bool littleEndian)
        {
            SetCore(byteOffset, BitConverter.GetBytes(TypeConverter.ToUint16(value)), littleEndian);
        }

        /// <summary>
        /// Stores an unsigned 32-bit integer value at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is written. </param>
        [JSInternalFunction(Name = "setUint32", RequiredArgumentCount = 2)]
        public void SetUint32(int byteOffset, object value, bool littleEndian)
        {
            SetCore(byteOffset, BitConverter.GetBytes(TypeConverter.ToUint32(value)), littleEndian);
        }

        /// <summary>
        /// Stores an unsigned 8-bit integer value at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="value"> The value to set. </param>
        [JSInternalFunction(Name = "setUint8", RequiredArgumentCount = 2)]
        public void SetUint8(int byteOffset, object value)
        {
            if (byteOffset < 0 || byteOffset > this.byteLength - 1)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Offset is outside the bounds of the DataView.");
            buffer.Buffer[this.byteOffset + byteOffset] = TypeConverter.ToUint8(value);
        }



        //     .NET HELPER FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Stores a series of bytes at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="bytes"> The bytes to store. </param>
        /// <param name="littleEndian"> Indicates whether the bytes are stored in little- or
        /// big-endian format. If false, a big-endian value is written. </param>
        private void SetCore(int byteOffset, byte[] bytes, bool littleEndian)
        {
            if (byteOffset < 0 || byteOffset > this.byteLength - bytes.Length)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Offset is outside the bounds of the DataView.");
            if (littleEndian)
            {
                for (int i = 0; i < bytes.Length; i++)
                    buffer.Buffer[this.byteOffset + byteOffset + i] = bytes[i];
            }
            else
            {
                for (int i = 0; i < bytes.Length; i++)
                    buffer.Buffer[this.byteOffset + byteOffset + bytes.Length - 1 - i] = bytes[i];
            }
        }

    }
}
