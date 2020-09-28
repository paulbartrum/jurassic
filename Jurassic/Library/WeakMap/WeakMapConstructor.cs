using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// The WeakMap object is a collection of key/value pairs in which the keys are weakly
    /// referenced.  The keys must be objects and the values can be arbitrary values.
    /// </summary>
    public partial class WeakMapConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new map constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal WeakMapConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>();
            InitializeConstructorProperties(properties, "WeakMap", 0, WeakMapInstance.CreatePrototype(Engine, this));
            InitializeProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the WeakMap object is invoked like a function, e.g. var x = WeakMap().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(ErrorType.TypeError, "Constructor WeakMap requires 'new'");
        }

        /// <summary>
        /// Creates a new WeakMap.
        /// </summary>
        /// <param name="iterable"> An Array or other iterable object whose elements are key-value
        /// pairs (2-element Arrays). Each key-value pair is added to the new WeakMap. <c>null</c>
        /// is treated as undefined. </param>
        /// <returns> A new WeakMap object, either empty or initialised with the given values. </returns>
        [JSConstructorFunction]
        public WeakMapInstance Construct(object iterable)
        {
            // Create a new set.
            var result = new WeakMapInstance(this.InstancePrototype);

            // If iterable is not null or undefined, then iterate through the values and add them to the set.
            if (iterable != Undefined.Value && iterable != Null.Value)
            {
                var iterator = TypeUtilities.RequireIterator(Engine, iterable);

                // Get a reference to the set function.
                var setFunc = result["set"] as FunctionInstance;
                if (setFunc == null)
                    throw new JavaScriptException(ErrorType.TypeError, "Missing 'set' function");

                // Call the set function for each value.
                foreach (var value in TypeUtilities.Iterate(Engine, iterator))
                {
                    var obj = value as ObjectInstance;
                    if (obj == null)
                        throw new JavaScriptException(ErrorType.TypeError, $"Expected iterator return value to be an object, but was {value}");
                    setFunc.Call(result, obj[0], obj[1]);
                }
            }

            return result;
        }

    }
}
