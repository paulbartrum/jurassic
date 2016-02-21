using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript Boolean object.
    /// </summary>
    [Serializable]
    public partial class BooleanConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Boolean object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal BooleanConstructor(ObjectInstance prototype)
            : base(prototype, "Boolean", 1, new BooleanInstance(prototype.Engine.Object.InstancePrototype, false), __STUB__Call, __STUB__Construct)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties();
            AddFunctionProperties(properties);
            FastSetProperties(properties);

            // Initialize the prototype properties.
            var instancePrototype = (BooleanInstance)InstancePrototype;
            properties = instancePrototype.GetDeclarativeProperties();
            properties.Add(new PropertyNameAndValue("constructor", this, PropertyAttributes.NonEnumerable));
            instancePrototype.FastSetProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Boolean object is invoked like a function, e.g. var x = Boolean("5").
        /// Converts the given argument into a boolean value (not a Boolean object).
        /// </summary>
        [JSCallFunction]
        public bool Call(bool value)
        {
            // Note: the parameter conversion machinery handles the required conversion.
            return value;
        }

        /// <summary>
        /// Creates a new Boolean instance and initializes it to the given value.
        /// </summary>
        /// <param name="value"> The value to initialize to.  Defaults to false. </param>
        [JSConstructorFunction]
        public BooleanInstance Construct([DefaultParameterValue(false)] bool value = false)
        {
            return new BooleanInstance(this.InstancePrototype, value);
        }

    }
}
