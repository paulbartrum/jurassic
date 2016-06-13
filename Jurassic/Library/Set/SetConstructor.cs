using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// The Set object lets you store unique values of any type, whether primitive values or object references.
    /// </summary>
    [Serializable]
    public partial class SetConstructor : ClrStubFunction
    {
        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new set constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal SetConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>(3);
            InitializeConstructorProperties(properties, "Set", 0, SetInstance.CreatePrototype(Engine, this));
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
            throw new JavaScriptException(Engine, ErrorType.TypeError, $"Constructor Set requires 'new'");
        }

        /// <summary>
        /// Creates a new set instance.
        /// </summary>
        /// <param name="iterable"> If an iterable object is passed, all of its elements will be
        /// added to the new Set. <c>null</c> is treated as undefined. </param>
        /// <returns> A new set instance. </returns>
        [JSConstructorFunction]
        public SetInstance Construct(object iterable)
        {
            // Create a new set.
            var result = new SetInstance(this.InstancePrototype);

            // If iterable is not null or undefined, then iterate through the values and add them to the set.
            if (iterable != Undefined.Value && iterable != Null.Value)
            {
                var iterator = TypeUtilities.GetIterator(Engine, TypeConverter.ToObject(Engine, iterable));
                if (iterator != null)
                {
                    // Get a reference to the add function.
                    var addFunc = result["add"] as FunctionInstance;
                    if (addFunc == null)
                        throw new JavaScriptException(Engine, ErrorType.TypeError, "Missing 'add' function.");

                    // Call the add function for each value.
                    foreach (var value in TypeUtilities.Iterate(Engine, iterator))
                    {
                        addFunc.Call(result, value);
                    }
                }
            }

            return result;
        }
    }
}
