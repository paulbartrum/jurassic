using System;


namespace Jurassic.Library
{
    /// <summary>
    /// The ArrayBuffer object is used to represent a generic, fixed-length raw binary data buffer.
    /// You can not directly manipulate the contents of an ArrayBuffer; instead, you create one of
    /// the typed array objects or a DataView object which represents the buffer in a specific
    /// format, and use that to read and write the contents of the buffer.
    /// </summary>
    public partial class ArrayBufferConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new array buffer constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal ArrayBufferConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "ArrayBuffer", 1, ArrayBufferInstance.CreatePrototype(Engine, this));
            InitializeProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the ArrayBuffer object is invoked like a function, e.g. var x = ArrayBuffer().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Constructor ArrayBuffer requires 'new'");
        }

        /// <summary>
        /// Creates a new ArrayBuffer of the given length in bytes.  Its contents are initialized
        /// to zero.
        /// </summary>
        /// <param name="size"> The size, in bytes, of the array buffer to create. </param>
        /// <returns> A new ArrayBuffer object of the specified size. </returns>
        [JSConstructorFunction]
        public ArrayBufferInstance Construct(int size)
        {
            if (size < 0)
                throw new JavaScriptException(Engine, ErrorType.RangeError, "Invalid array buffer length.");
            return new ArrayBufferInstance(this.InstancePrototype, size);
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



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns <c>true</c> if <paramref name="arg"/> is one of the ArrayBuffer views, such as
        /// typed array objects or a DataView; <c>false</c> otherwise.
        /// </summary>
        /// <param name="arg"> The argument to be checked. </param>
        /// <returns> <c>true</c> if arg is one of the ArrayBuffer views. </returns>
        [JSInternalFunction(Name = "isView")]
        public static bool IsView(object arg)
        {
            return arg is DataViewInstance || arg is TypedArrayInstance;
        }
    }
}
