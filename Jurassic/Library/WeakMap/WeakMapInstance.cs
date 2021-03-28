using System;
using System.Runtime.CompilerServices;

namespace Jurassic.Library
{
    /// <summary>
    /// The WeakMap object is a collection of key/value pairs in which the keys are weakly
    /// referenced.  The keys must be objects and the values can be arbitrary values.
    /// </summary>
    public partial class WeakMapInstance : ObjectInstance
    {
        private readonly ConditionalWeakTable<ObjectInstance, object> store;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new WeakMap instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        public WeakMapInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.store = new ConditionalWeakTable<ObjectInstance, object>();
        }

        /// <summary>
        /// Creates the WeakMap prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, WeakMapConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(Symbol.ToStringTag, "WeakMap", PropertyAttributes.Configurable));
            result.InitializeProperties(properties);
            return result;
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Removes the specified element from a Map object.
        /// </summary>
        /// <param name="key"> The key of the element to remove from the Map object. </param>
        /// <returns> <c>true</c> if an element in the Map object existed and has been removed, or
        /// <c>false</c> if the element does not exist. </returns>
        [JSInternalFunction(Name = "delete")]
        public bool Delete(object key)
        {
            var keyObj = key as ObjectInstance;
            if (keyObj == null)
                return false;

            var removed = this.store.Remove(keyObj);
            GC.KeepAlive(keyObj);
            return removed;
        }

        /// <summary>
        /// Returns a specified element from a Map object.
        /// </summary>
        /// <param name="key"> The key of the element to return from the Map object. </param>
        /// <returns> The element associated with the specified key, or undefined if the key can't
        /// be found in the Map object. </returns>
        [JSInternalFunction(Name = "get")]
        public object Get(object key)
        {
            var keyObj = key as ObjectInstance;
            if (keyObj == null)
                return Undefined.Value;

            object result;
            if (!this.store.TryGetValue(keyObj, out result))
                return Undefined.Value;
            return result;
        }

        /// <summary>
        /// Returns a boolean indicating whether an element with the specified key exists or not.
        /// </summary>
        /// <param name="key"> The key of the element to test for presence in the Map object. </param>
        /// <returns> <c>true</c> if an element with the specified key exists in the Map object;
        /// otherwise <c>false</c>. </returns>
        [JSInternalFunction(Name = "has")]
        public bool Has(object key)
        {
            var keyObj = key as ObjectInstance;
            if (keyObj == null)
                return false;

            object result;
            return this.store.TryGetValue(keyObj, out result);
        }
        
        /// <summary>
        /// Adds a new element with a specified key and value to a Map object.
        /// </summary>
        /// <param name="key"> The key of the element to add to the Map object. </param>
        /// <param name="value"> The value of the element to add to the Map object. </param>
        /// <returns> The WeakMap object. </returns>
        [JSInternalFunction(Name = "set")]
        public WeakMapInstance Set(object key, object value)
        {
            var keyObj = key as ObjectInstance;
            if (keyObj == null)
                throw new JavaScriptException(ErrorType.TypeError, "Invalid value used as weak map key");

            // Gah!  There's no update/set method on ConditionalWeakTable (Add throws an exception
            // if the key exists).
            object existingValue;
            if (this.store.TryGetValue(keyObj, out existingValue))
            {
                // The key exists, so remove it, and then re-add it (yay)
                this.store.Remove(keyObj);
                this.store.Add(keyObj, value);
            }
            else
            {
                // The key doesn't exist, so just add it.
                this.store.Add(keyObj, value);
            }
            return this;
        }



        //     .NET PROPERTIES
        //_________________________________________________________________________________________

        
        /// <summary>
        /// Gets the internal storage. Used by debugger decoration.
        /// </summary>
        internal ConditionalWeakTable<ObjectInstance, object> Store
        {
            get { return this.store; }
        }
    }
}
