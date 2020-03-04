using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript String object.
    /// </summary>
    public partial class TypedArrayConstructor : ClrStubFunction
    {
        private TypedArrayType type;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new typed array constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="type"> Defines the element type and behaviour of the typed array. </param>
        internal TypedArrayConstructor(ObjectInstance prototype, TypedArrayType type)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            this.type = type;

            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, type.ToString(), 3, TypedArrayInstance.CreatePrototype(Engine, this));
            properties.Add(new PropertyNameAndValue("BYTES_PER_ELEMENT", BytesPerElement, PropertyAttributes.Sealed));
            InitializeProperties(properties);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// The data storage size, in bytes, of each array element.
        /// </summary>
        private int BytesPerElement
        {
            get
            {
                switch (type)
                {
                    case TypedArrayType.Int8Array:
                    case TypedArrayType.Uint8Array:
                    case TypedArrayType.Uint8ClampedArray:
                        return 1;
                    case TypedArrayType.Int16Array:
                    case TypedArrayType.Uint16Array:
                        return 2;
                    case TypedArrayType.Int32Array:
                    case TypedArrayType.Uint32Array:
                    case TypedArrayType.Float32Array:
                        return 4;
                    case TypedArrayType.Float64Array:
                        return 8;
                    default:
                        throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
                }
            }
        }



        //     .NET HELPER METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new typed array from a .NET array.
        /// </summary>
        /// <param name="source"> A .NET array </param>
        /// <returns> A new typed array instance. </returns>
        public TypedArrayInstance From(object[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            var result = new TypedArrayInstance(this.InstancePrototype, this.type,
                Engine.ArrayBuffer.Construct(source.Length * BytesPerElement), 0, source.Length);
            for (int i = 0; i < source.Length; i ++)
            {
                result[i] = source[i];
            }
            return result;
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// A reference to the constructor function that is used to create derived objects.
        /// </summary>
        [JSProperty(Name = "@@species")]
        public FunctionInstance Species
        {
            get { return this; }
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the typed array object is invoked like a function, e.g. Int8Array().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, $"Constructor {this["name"]} requires 'new'");
        }

        /// <summary>
        /// Creates a new (empty) typed array instance.
        /// </summary>
        /// <returns> A new typed array instance. </returns>
        [JSConstructorFunction]
        public TypedArrayInstance Construct()
        {
            return new TypedArrayInstance(this.InstancePrototype, this.type, Engine.ArrayBuffer.Construct(0), 0, 0);
        }

        /// <summary>
        /// Creates a new typed array instance.
        /// </summary>
        /// <param name="arg"> Either the length of the new array, or buffer, or an array-like
        /// object. </param>
        /// <param name="byteOffset"> The offset, in bytes, to the first byte in the specified
        /// buffer for the new view to reference. If not specified, the TypedArray will start
        /// with the first byte.  Ignored unless the first parameter is a array buffer. </param>
        /// <param name="length"> The length (in elements) of the typed array.  Ignored unless the
        /// first parameter is a array buffer. </param>
        /// <returns> A new typed array instance. </returns>
        [JSConstructorFunction]
        public TypedArrayInstance Construct(object arg, int byteOffset = 0, int? length = null)
        {
            
            // new Int8Array(typedArray);
            // new Int8Array(object);
            // new Int8Array(buffer[, byteOffset[, length]]);
            if (arg is TypedArrayInstance)
            {
                // new %TypedArray%(typedArray);
                var typedArray = (TypedArrayInstance)arg;

                // Copy the items one by one.
                var result = new TypedArrayInstance(this.InstancePrototype, this.type,
                    Engine.ArrayBuffer.Construct(typedArray.Length * BytesPerElement), 0, typedArray.Length);
                for (int i = 0; i < typedArray.Length; i++)
                {
                    result[i] = typedArray[i];
                }
                return result;
            }
            else if (arg is ArrayBufferInstance)
            {
                // new %TypedArray%(buffer[, byteOffset[, length]]);
                var buffer = (ArrayBufferInstance)arg;
                int bytesPerElement = BytesPerElement;
                int actualLength;
                if (length == null)
                {
                    if (byteOffset < 0)
                        throw new JavaScriptException(Engine, ErrorType.RangeError, "Invalid typed array offset");
                    if ((byteOffset % BytesPerElement) != 0)
                        throw new JavaScriptException(Engine, ErrorType.RangeError, $"Start offset of {this.type} should be a multiple of {BytesPerElement}");
                    if ((buffer.ByteLength % BytesPerElement) != 0)
                        throw new JavaScriptException(Engine, ErrorType.RangeError, $"Byte length of {this.type} should be a multiple of {BytesPerElement}");
                    actualLength = (buffer.ByteLength - byteOffset) / bytesPerElement;
                    if (actualLength < 0)
                        throw new JavaScriptException(Engine, ErrorType.RangeError, "Start offset is too large");
                }
                else
                {
                    actualLength = length.Value;
                    if (byteOffset + actualLength * bytesPerElement > buffer.ByteLength)
                        throw new JavaScriptException(Engine, ErrorType.RangeError, "Invalid typed array length");
                }
                return new TypedArrayInstance(this.InstancePrototype, this.type, buffer, byteOffset, actualLength);
            }
            else if (arg is ObjectInstance)
            {
                // new %TypedArray%(object);
                return From(arg);
            }
            else
            {
                // new %TypedArray%(length);
                if (TypeUtilities.IsUndefined(arg))
                    throw new JavaScriptException(Engine, ErrorType.TypeError, "Argument cannot be undefined");
                int argLength = TypeConverter.ToInteger(arg);
                return new TypedArrayInstance(this.InstancePrototype, this.type, Engine.ArrayBuffer.Construct(argLength * BytesPerElement), 0, argLength);
            }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new typed array from an array-like or iterable object.
        /// </summary>
        /// <param name="source"> An array-like or iterable object to convert to a typed array. </param>
        /// <param name="mapFn"> Optional. Map function to call on every element of the typed array. </param>
        /// <param name="thisArg"> Optional. Value to use as this when executing mapFn. </param>
        /// <returns> A new typed array instance. </returns>
        [JSInternalFunction(Name = "from", Length = 1)]
        public TypedArrayInstance From(object source, FunctionInstance mapFn = null, object thisArg = null)
        {
            var items = TypeConverter.ToObject(Engine, source);

            var iterator = TypeUtilities.GetIterator(Engine, items);
            if (iterator != null)
            {
                // Loop.
                var values = new List<object>();
                foreach (var value in TypeUtilities.Iterate(Engine, iterator))
                {
                    // Collect the values.
                    values.Add(value);
                }

                // Convert the values into a typed array instance.
                var result = new TypedArrayInstance(this.InstancePrototype, this.type,
                    Engine.ArrayBuffer.Construct(values.Count * BytesPerElement), 0, values.Count);
                for (int i = 0; i < values.Count; i++)
                {
                    if (mapFn != null)
                        result[i] = mapFn.Call(thisArg, values[i], i);
                    else
                        result[i] = values[i];
                }
                return result;
            }
            else
            {
                // There was no iterator symbol value, so fall back on the alternate method.
                int length = TypeConverter.ToInt32(items["length"]);
                var result = new TypedArrayInstance(this.InstancePrototype, this.type, Engine.ArrayBuffer.Construct(length * BytesPerElement), 0, length);
                for (int i = 0; i < length; i++)
                {
                    if (mapFn != null)
                        result[i] = mapFn.Call(thisArg, items[i], i);
                    else
                        result[i] = items[i];
                }
                return result;
            }
        }

        /// <summary>
        /// Creates a new typed array with a variable number of elements.
        /// </summary>
        /// <param name="elements"> Elements of which to create the typed array. </param>
        /// <returns> A new typed array with the given elements. </returns>
        [JSInternalFunction(Name = "of", Length = 0)]
        public TypedArrayInstance Of(params object[] elements)
        {
            int length = elements.Length;
            var result = new TypedArrayInstance(this.InstancePrototype, this.type, Engine.ArrayBuffer.Construct(length * BytesPerElement), 0, length);
            for (int i = 0; i < length; i++)
            {
                result[i] = elements[i];
            }
            return result;
        }

    }
}
