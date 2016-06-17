using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// The WeakSet object lets you store weakly held objects in a collection.
    /// </summary>
    [Serializable]
    public partial class WeakSetConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new WeakSet constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal WeakSetConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>();
            InitializeConstructorProperties(properties, "WeakSet", 0, WeakSetInstance.CreatePrototype(Engine, this));
            FastSetProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the WeakSet object is invoked like a function, e.g. var x = WeakSet().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Constructor WeakSet requires 'new'");
        }

        /// <summary>
        /// Creates a new WeakSet instance.
        /// </summary>
        /// <param name="iterable"> If an iterable object is passed, all of its elements will be
        /// added to the new WeakSet. <c>null</c> is treated as undefined. </param>
        /// <returns> A new WeakSet instance. </returns>
        [JSConstructorFunction]
        public WeakSetInstance Construct(object iterable)
        {
            // Create a new set.
            var result = new WeakSetInstance(this.InstancePrototype);

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
