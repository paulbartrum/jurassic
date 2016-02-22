using System;
using System.Collections.Generic;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a constructor for one of the error types: Error, RangeError, SyntaxError, etc.
    /// </summary>
    [Serializable]
    public partial class ErrorConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new derived error function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="typeName"> The name of the error object, e.g. "Error", "RangeError", etc. </param>
        internal ErrorConstructor(ObjectInstance prototype, string typeName)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>(3);
            InitializeConstructorProperties(properties, typeName, 1, new ErrorInstance(this, typeName));
            FastSetProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Error object is invoked like a function, e.g. var x = Error("oh no").
        /// Creates a new derived error instance with the given message.
        /// </summary>
        /// <param name="message"> A description of the error. </param>
        [JSCallFunction]
        public ErrorInstance Call(string message = "")
        {
            return new ErrorInstance(this.InstancePrototype, null, message);
        }

        /// <summary>
        /// Creates a new derived error instance with the given message.
        /// </summary>
        /// <param name="message"> A description of the error. </param>
        [JSConstructorFunction]
        public ErrorInstance Construct(string message = "")
        {
            return new ErrorInstance(this.InstancePrototype, null, message);
        }

    }
}
