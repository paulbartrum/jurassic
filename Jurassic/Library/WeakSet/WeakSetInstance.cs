using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jurassic.Library
{
    /// <summary>
    /// The WeakSet object lets you store weakly held objects in a collection.
    /// </summary>
    public partial class WeakSetInstance : ObjectInstance
    {
        private readonly ConditionalWeakTable<ObjectInstance, object> store;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new WeakSet instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        public WeakSetInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.store = new ConditionalWeakTable<ObjectInstance, object>();
        }

        /// <summary>
        /// Creates the WeakSet prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, WeakSetConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(engine.Symbol.ToStringTag, "WeakSet", PropertyAttributes.Configurable));
            result.FastSetProperties(properties);
            return result;
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Appends a new element with a specified value to the end of the WeakSet.
        /// </summary>
        /// <param name="value"> The value of the element to add to the WeakSet. </param>
        /// <returns> The WeakSet object. </returns>
        [JSInternalFunction(Name = "add")]
        public WeakSetInstance Add(object value)
        {
            var valueObj = value as ObjectInstance;
            if (valueObj == null)
                throw new JavaScriptException(Engine, ErrorType.TypeError, "Invalid value used in weak set");

            this.store.GetValue(valueObj, obj => null);
            return this;
        }

        /// <summary>
        /// Removes the specified value from the WeakSet.
        /// </summary>
        /// <param name="value"> The value of the element to remove from the WeakSet. </param>
        /// <returns> <c>true</c> if an element in the WeakSet object has been removed
        /// successfully; otherwise <c>false</c>. </returns>
        [JSInternalFunction(Name = "delete")]
        public bool Delete(object value)
        {
            var valueObj = value as ObjectInstance;
            if (valueObj == null)
                return false;

            return this.store.Remove(valueObj);
        }

        /// <summary>
        /// Returns a boolean indicating whether an element with the specified value exists in the
        /// WeakSet or not.
        /// </summary>
        /// <param name="value"> The value to test for presence in the WeakSet. </param>
        /// <returns> <c>true</c> if an element with the specified value exists in the WeakSet
        /// object; otherwise <c>false</c>. </returns>
        [JSInternalFunction(Name = "has")]
        public bool Has(object value)
        {
            var valueObj = value as ObjectInstance;
            if (valueObj == null)
                return false;

            object result;
            return this.store.TryGetValue(valueObj, out result);
        }
    }
}
