using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a JavaScript function.
    /// </summary>
    public abstract class FunctionInstance : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new instance of a built-in function object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        protected FunctionInstance(ObjectInstance prototype)
            : base(prototype)
        {
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "Function"; }
        }

        /// <summary>
        /// Gets the prototype of objects constructed using this function.  Equivalent to
        /// the Function.prototype property.
        /// </summary>
        public ObjectInstance InstancePrototype
        {
            get { return this["prototype"] as ObjectInstance; }
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name
        {
            get { return TypeConverter.ToString(this["name"]); }
        }

        /// <summary>
        /// Gets the number of arguments expected by the function.
        /// </summary>
        public int Length
        {
            get { return TypeConverter.ToInteger("length"); }
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Determines whether the given object inherits from this function.  More precisely, it
        /// checks whether the prototype chain of the object contains the prototype property of
        /// this function.  Used by the "instanceof" operator.
        /// </summary>
        /// <param name="instance"> The instance to check. </param>
        /// <returns> <c>true</c> if the object inherits from this function; <c>false</c>
        /// otherwise. </returns>
        public virtual bool HasInstance(object instance)
        {
            if ((instance is ObjectInstance) == false)
                return false;
            object functionPrototype = this["prototype"];
            if ((functionPrototype is ObjectInstance) == false)
                throw new JavaScriptException("TypeError", "Function has non-object prototype in instanceof check");
            var instancePrototype = ((ObjectInstance)instance).Prototype;
            while (instancePrototype != null)
            {
                if (instancePrototype == functionPrototype)
                    return true;
                instancePrototype = instancePrototype.Prototype;
            }
            return false;
        }

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="arguments"> An array of argument values to pass to the function. </param>
        /// <returns> The value that was returned from the function. </returns>
        public abstract object CallLateBound(object thisObject, params object[] arguments);

        /// <summary>
        /// Creates an object, using this function as the constructor.
        /// </summary>
        /// <param name="arguments"> An array of argument values to pass to the function. </param>
        /// <returns> The object that was created. </returns>
        public abstract ObjectInstance ConstructLateBound(params object[] arguments);



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls the function, passing in parameters from the given array.
        /// </summary>
        /// <param name="thisObj"> The value of <c>this</c> in the context of the function. </param>
        /// <param name="argumentArray"> The arguments passed to the function, as an array. </param>
        /// <returns> The result from the function call. </returns>
        [JSFunction(Name = "apply")]
        public object Apply(object thisObj, object arguments)
        {
            // Convert the arguments parameter into an array.
            object[] argumentsArray;
            if (arguments == null || arguments == Undefined.Value || arguments == Null.Value)
                argumentsArray = new object[0];
            else
            {
                if ((arguments is ObjectInstance) == false)
                    throw new JavaScriptException("TypeError", "The second parameter of apply() must be an array or an array-like object.");
                ObjectInstance argumentsObject = (ObjectInstance)arguments;
                object arrayLengthObj = argumentsObject["length"];
                if (arrayLengthObj == null || arrayLengthObj == Undefined.Value || arrayLengthObj == Null.Value)
                    throw new JavaScriptException("TypeError", "The second parameter of apply() must be an array or an array-like object.");
                uint arrayLength = TypeConverter.ToUint32(arrayLengthObj);
                if (arrayLength != TypeConverter.ToNumber(arrayLengthObj))
                    throw new JavaScriptException("TypeError", "The second parameter of apply() must be an array or an array-like object.");
                argumentsArray = new object[arrayLength];
                for (uint i = 0; i < arrayLength; i++)
                    argumentsArray[i] = argumentsObject[i];
            }

            return this.CallLateBound(thisObj, argumentsArray);
        }

        /// <summary>
        /// Calls the function.
        /// </summary>
        /// <param name="thisObj"> The value of <c>this</c> in the context of the function. </param>
        /// <param name="arguments"> Any number of arguments that will be passed to the function. </param>
        /// <returns> The result from the function call. </returns>
        [JSFunction(Name = "call")]
        public object Call(object thisObj, params object[] arguments)
        {
            return this.CallLateBound(thisObj, arguments);
        }

        /// <summary>
        /// Creates a new function that, when called, calls this function with the given "this"
        /// value and, optionally, one or more more arguments.
        /// </summary>
        /// <param name="boundThis"> The fixed value of "this". </param>
        /// <param name="boundArguments"> Any number of fixed arguments values. </param>
        /// <returns> A new function. </returns>
        [JSFunction(Name = "bind", Length = 1)]
        public FunctionInstance Bind(object boundThis, params object[] boundArguments)
        {
            return new BoundFunction(this, boundThis, boundArguments);
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        [JSFunction(Name = "toString")]
        public new string ToStringJS()
        {
            return this.ToString();
        }
    }
}
