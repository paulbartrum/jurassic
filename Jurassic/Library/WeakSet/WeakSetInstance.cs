using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Jurassic.Library
{
    /// <summary>
    /// The WeakSet object lets you store weakly held objects in a collection.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(WeakSetInstanceDebugView))]
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


        //     .NET PROPERTIES
        //_________________________________________________________________________________________


        /// <summary>
        /// Gets the internal storage. Used by debugger decoration.
        /// </summary>
        internal ConditionalWeakTable<ObjectInstance, object> Store
        {
            get { return this.store; }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get
            {
                IEnumerable<ObjectInstance> keys = this.store.GetKeys();
                IEnumerable<string> strValues =
                    keys.Select(key => DebuggerDisplayHelper.ShortStringRepresentation(key));

                return string.Format("[{0}]", string.Join(", ", strValues));
            }
        }


        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayType; }
        }


        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayType
        {
            get
            {
                string result = string.Format("WeakSet({0})", this.store.GetKeys().Count());
                return result;
            }
        }
    }
}
