using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a JS class, which is really just a special type of function. Classes cannot be
    /// called directly, but they can be instantiated using the 'new' operator.
    /// </summary>
    public class ClassFunction : FunctionInstance
    {
        private FunctionInstance constructor;

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new instance of a user-defined class.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the class. </param>
        /// <param name="constructor"> A function that represents the constructor, if the class has
        /// one, or <c>null</c> otherwise. </param>
        public ClassFunction(ObjectInstance prototype, string name, FunctionInstance constructor)
            : base(prototype)
        {
            this.constructor = constructor;

            // Create an object to serve as the instance prototype.
            var instancePrototype = this.Engine.Object.Construct();
            instancePrototype.InitializeProperties(new List<PropertyNameAndValue>
            {
                new PropertyNameAndValue("constructor", this, PropertyAttributes.NonEnumerable)
            });

            // Now add properties to this object.
            int length = constructor == null ? 0 : constructor.Length;
            InitializeProperties(new List<PropertyNameAndValue>()
            {
                new PropertyNameAndValue("name", name, PropertyAttributes.Configurable),
                new PropertyNameAndValue("length", length, PropertyAttributes.Configurable),
                new PropertyNameAndValue("prototype", instancePrototype, PropertyAttributes.Writable),
            });
        }


        
        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values to pass to the function. </param>
        /// <returns> The value that was returned from the function. </returns>
        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Class constructor A cannot be invoked without 'new'");
        }

        /// <summary>
        /// Creates an object, using this function as the constructor.
        /// </summary>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The object that was created. </returns>
        public override ObjectInstance ConstructLateBound(params object[] argumentValues)
        {
            // Create a new object and set the prototype to the instance prototype of the function.
            var newObject = ObjectInstance.CreateRawObject(this.InstancePrototype);

            if (constructor != null)
            {
                // Run the function, with the new object as the "this" keyword.
                // The return value is ignored!
                constructor.CallLateBound(newObject, argumentValues);
            }

            // Otherwise, return the new object.
            return newObject;
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            // TODO: fix this.
            return "class";
        }
    }
}
