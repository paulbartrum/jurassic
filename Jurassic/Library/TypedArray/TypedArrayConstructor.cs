using System;


namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript String object.
    /// </summary>
    [Serializable]
    public partial class TypedArrayConstructor : ClrStubFunction
    {
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
            int bytesPerElement;
            switch (type)
            {
                case TypedArrayType.Int8Array:
                case TypedArrayType.Uint8Array:
                case TypedArrayType.Uint8ClampedArray:
                    bytesPerElement = 1;
                    break;
                case TypedArrayType.Int16Array:
                case TypedArrayType.Uint16Array:
                    bytesPerElement = 2;
                    break;
                case TypedArrayType.Int32Array:
                case TypedArrayType.Uint32Array:
                case TypedArrayType.Float32Array:
                    bytesPerElement = 4;
                    break;
                case TypedArrayType.Float64Array:
                    bytesPerElement = 8;
                    break;
                default:
                    throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
            }

            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, type.ToString(), 3, TypedArrayInstance.CreatePrototype(Engine, this));
            properties.Add(new PropertyNameAndValue("BYTES_PER_ELEMENT", bytesPerElement, PropertyAttributes.Sealed));
            FastSetProperties(properties);
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
        /// Creates a new typed array instance.
        /// </summary>
        /// <param name="arg"> </param>
        [JSConstructorFunction]
        public StringInstance Construct(object arg)
        {
            // new Int8Array(length);
            // new Int8Array(typedArray);
            // new Int8Array(object);
            // new Int8Array(buffer[, byteOffset[, length]]);
            throw new NotImplementedException();
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new typed array from an array-like or iterable object.
        /// </summary>
        /// <param name="source"> An array-like or iterable object to convert to a typed array. </param>
        /// <param name="mapFn"> Optional. Map function to call on every element of the typed array. </param>
        /// <param name="thisArg"> Optional. Value to use as this when executing mapFn. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "from")]
        public static TypedArrayInstance From(object source, FunctionInstance mapFn = null, object thisArg = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new typed array with a variable number of arguments.
        /// </summary>
        /// <param name="elements"> Elements of which to create the typed array. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "of")]
        public static TypedArrayInstance Of(params object[] elements)
        {
            throw new NotImplementedException();
        }

    }
}
