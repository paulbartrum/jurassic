using System;
using System.Collections.Generic;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in JavaScript Function object.
    /// </summary>
    public partial class FunctionConstructor : ClrStubFunction
    {
        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Function object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="instancePrototype"> The prototype for instances created by this function. </param>
        internal FunctionConstructor(ObjectInstance prototype, FunctionInstance instancePrototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>(3);
            InitializeConstructorProperties(properties, "Function", 1, instancePrototype);
            FastSetProperties(properties);
        }


        //     CONSTRUCTORS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Function object is invoked like a function, e.g. var x = Function("5").
        /// Creates a new function instance.
        /// </summary>
        /// <param name="argumentsAndBody"> The argument names plus the function body. </param>
        /// <returns> A new function instance. </returns>
        [JSCallFunction]
        public FunctionInstance Call(params string[] argumentsAndBody)
        {
            return this.Construct(argumentsAndBody);
        }

        /// <summary>
        /// Creates a new function instance.
        /// </summary>
        /// <param name="argumentsAndBody"> The argument names plus the function body. </param>
        /// <returns> A new function instance. </returns>
        [JSConstructorFunction]
        public FunctionInstance Construct(params string[] argumentsAndBody)
        {
            // Passing no arguments results in an empty function.
            if (argumentsAndBody.Length == 0)
                return new UserDefinedFunction(this.InstancePrototype, "anonymous", string.Empty, string.Empty);

            // Concatenate the function arguments (every parameter except the last one).
            var argumentsString = string.Join(",", argumentsAndBody, 0, argumentsAndBody.Length - 1);

            // Create a new function.
            return new UserDefinedFunction(this.InstancePrototype, "anonymous", argumentsString, argumentsAndBody[argumentsAndBody.Length - 1]);
        }
    }
}
