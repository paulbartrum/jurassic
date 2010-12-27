using System;
using System.Collections.Generic;
using Jurassic.Compiler;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a JavaScript function that throws a type error.
    /// </summary>
    [Serializable]
    internal sealed class ThrowTypeErrorFunction : FunctionInstance
    {
        [NonSerialized]
        private FunctionDelegate body;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new ThrowTypeErrorFunction instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal ThrowTypeErrorFunction(ObjectInstance prototype)
            : base(prototype)
        {
            this.FastSetProperty("length", 0);
            this.IsExtensible = false;
            this.body = new FunctionDelegate((engine, scope, thisObject, functionObject, argumentValues) =>
                {
                    throw new JavaScriptException(this.Engine, "TypeError", "It is illegal to access the 'callee' or 'caller' property in strict mode");
                });
        }



        //     SERIALIZATION
        //_________________________________________________________________________________________

#if !SILVERLIGHT

        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <remarks> Derived classes must call the base class implementation. </remarks>
        protected override void OnDeserializationCallback()
        {
            // Call the base class.
            base.OnDeserializationCallback();

            this.body = new FunctionDelegate((engine, scope, thisObject, functionObject, argumentValues) =>
            {
                throw new JavaScriptException(this.Engine, "TypeError", "It is illegal to access the 'callee' or 'caller' property in strict mode");
            });
        }

#endif



        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="arguments"> An array of argument values to pass to the function. </param>
        /// <returns> The value that was returned from the function. </returns>
        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            return this.body(this.Engine, null, thisObject, this, argumentValues);
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            return "function ThrowTypeError() {{ [native code] }}";
        }
    }
}
