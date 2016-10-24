using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// The Map object is a simple key/value map. Any value (both objects and primitive values) may
    /// be used as either a key or a value.
    /// </summary>
    public partial class MapInstance : ObjectInstance
    {
        private readonly Dictionary<object, LinkedListNode<KeyValuePair<object, object>>> store;
        private readonly LinkedList<KeyValuePair<object, object>> list;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Map instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        public MapInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.store = new Dictionary<object, LinkedListNode<KeyValuePair<object, object>>>(new SameValueZeroComparer());
            this.list = new LinkedList<KeyValuePair<object, object>>();
        }

        /// <summary>
        /// Creates the Map prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, MapConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(engine.Symbol.ToStringTag, "Map", PropertyAttributes.Configurable));

            // From the spec: the initial value of the @@iterator property is the same function
            // object as the initial value of the Map.prototype.entries property.
            PropertyNameAndValue entriesProperty = properties.Find(p => "entries".Equals(p.Key));
            if (entriesProperty == null)
                throw new InvalidOperationException("Expected entries property.");
            properties.Add(new PropertyNameAndValue(engine.Symbol.Iterator, entriesProperty.Value, PropertyAttributes.NonEnumerable));

            result.FastSetProperties(properties);
            return result;
        }



        //     .NET PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Called before a linked list node is deleted.
        /// </summary>
        internal event Action<LinkedListNode<KeyValuePair<object, object>>> BeforeDelete;



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// The number of elements in the Map.
        /// </summary>
        [JSProperty(Name = "size")]
        public int Size
        {
            get { return this.store.Count; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Removes all elements from a Map object.
        /// </summary>
        [JSInternalFunction(Name = "clear")]
        public void Clear()
        {
            this.store.Clear();
            this.list.Clear();
        }

        /// <summary>
        /// Removes the specified element from a Map object.
        /// </summary>
        /// <param name="key"> The key of the element to remove from the Map object. </param>
        /// <returns> <c>true</c> if an element in the Map object existed and has been removed, or
        /// <c>false</c> if the element does not exist. </returns>
        [JSInternalFunction(Name = "delete")]
        public bool Delete(object key)
        {
            LinkedListNode<KeyValuePair<object, object>> node;
            bool found = this.store.TryGetValue(key, out node);
            if (!found)
                return false;
            this.store.Remove(key);
            BeforeDelete?.Invoke(node);
            this.list.Remove(node);
            return true;
        }

        /// <summary>
        /// Returns a new Iterator object that contains the [key, value] pairs for each element in
        /// the Map object in insertion order.
        /// </summary>
        /// <returns> A new Iterator object. </returns>
        [JSInternalFunction(Name = "entries")]
        public ObjectInstance Entries()
        {
            return new MapIterator(Engine.MapIteratorPrototype, this, this.list, MapIterator.Kind.KeyAndValue);
        }


        /// <summary>
        /// Executes a provided function once per each key/value pair in the Map object, in
        /// insertion order.
        /// </summary>
        /// <param name="callback"> Function to execute for each element. </param>
        /// <param name="thisArg"> Value to use as this when executing callback. </param>
        [JSInternalFunction(Name = "forEach", Length = 1)]
        public void ForEach(FunctionInstance callback, object thisArg)
        {
            foreach (var keyValue in TypeUtilities.Iterate(Engine, Entries()))
            {
                var keyValueObj = keyValue as ObjectInstance;
                if (keyValueObj == null)
                    throw new JavaScriptException(Engine, ErrorType.TypeError, "Invalid iterator return value.");
                callback.Call(thisArg, keyValueObj[0], keyValueObj[1], this);
            }
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
            LinkedListNode<KeyValuePair<object, object>> node;
            if (this.store.TryGetValue(key, out node))
                return node.Value.Value;
            return Undefined.Value;
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
            return this.store.ContainsKey(key);
        }

        /// <summary>
        /// Returns a new Iterator object that contains the keys for each element in the Map object
        /// in insertion order.
        /// </summary>
        /// <returns> A new Iterator object. </returns>
        [JSInternalFunction(Name = "keys")]
        public ObjectInstance Keys()
        {
            return new MapIterator(Engine.MapIteratorPrototype, this, this.list, MapIterator.Kind.Key);
        }

        /// <summary>
        /// Adds a new element with a specified key and value to a Map object.
        /// </summary>
        /// <param name="key"> The key of the element to add to the Map object. </param>
        /// <param name="value"> The value of the element to add to the Map object. </param>
        /// <returns> The Map object. </returns>
        [JSInternalFunction(Name = "set")]
        public MapInstance Set(object key, object value)
        {
            LinkedListNode<KeyValuePair<object, object>> node;
            if (this.store.TryGetValue(key, out node))
            {
                node.Value = new KeyValuePair<object, object>(node.Value.Key, value);
                return this;
            }
            if (key is double && TypeUtilities.IsNegativeZero((double)key))
                key = 0;
            node = this.list.AddLast(new KeyValuePair<object, object>(key, value));
            this.store.Add(key, node);
            return this;
        }

        /// <summary>
        /// Returns a new Iterator object that contains the values for each element in the Map
        /// object in insertion order.
        /// </summary>
        /// <returns> A new Iterator object. </returns>
        [JSInternalFunction(Name = "values")]
        public ObjectInstance Values()
        {
            return new MapIterator(Engine.MapIteratorPrototype, this, this.list, MapIterator.Kind.Value);
        }



        //     HELPER CLASSES
        //_________________________________________________________________________________________

        /// <summary>
        /// Implements the SameValueZero comparison operation.
        /// </summary>
        private class SameValueZeroComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y)
            {
                return TypeComparer.SameValueZero(x, y);
            }

            public int GetHashCode(object obj)
            {
                return TypeUtilities.NormalizeValue(obj).GetHashCode();
            }
        }
    }
}
