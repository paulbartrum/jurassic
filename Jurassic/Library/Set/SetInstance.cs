using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// The Set object lets you store unique values of any type, whether primitive values or object references.
    /// </summary>
    public partial class SetInstance : ObjectInstance
    {
        private readonly Dictionary<object, LinkedListNode<object>> store;
        private readonly LinkedList<object> list;

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new set instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal SetInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.store = new Dictionary<object, LinkedListNode<object>>(new SameValueZeroComparer());
            this.list = new LinkedList<object>();
        }

        /// <summary>
        /// Creates the Set prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, SetConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(Symbol.ToStringTag, "Set", PropertyAttributes.Configurable));

            // From the spec: the initial value of the @@iterator property is the same function
            // object as the initial value of the Set.prototype.values property.
            PropertyNameAndValue valuesProperty = properties.Find(p => "values".Equals(p.Key));
            if (valuesProperty == null)
                throw new InvalidOperationException("Expected values property.");
            properties.Add(new PropertyNameAndValue(Symbol.Iterator, valuesProperty.Value, PropertyAttributes.NonEnumerable));

            result.InitializeProperties(properties);
            return result;
        }



        //     .NET PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Called before a linked list node is deleted.
        /// </summary>
        internal event Action<LinkedListNode<object>> BeforeDelete;

        /// <summary>
        /// Gets the internal storage. Used by debugger decoration only.
        /// </summary>
        internal Dictionary<object, LinkedListNode<object>> Store
        {
            get { return this.store; }
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// The number of elements in the Set.
        /// </summary>
        [JSProperty(Name = "size")]
        public int Size
        {
            get { return this.store.Count; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Appends a new element with a specified value to the end of the Set.
        /// </summary>
        /// <param name="value"> The value of the element to add to the Set. </param>
        /// <returns> The Set object. </returns>
        [JSInternalFunction(Name = "add")]
        public SetInstance Add(object value)
        {
            value = value ?? Undefined.Value;

            if (this.store.ContainsKey(value))
                return this;
            if (value is double && TypeUtilities.IsNegativeZero((double)value))
                value = 0;
            var node = this.list.AddLast(value);
            this.store.Add(value, node);
            return this;
        }

        /// <summary>
        /// Removes all elements from a Set
        /// </summary>
        [JSInternalFunction(Name = "clear")]
        public void Clear()
        {
            this.store.Clear();
            this.list.Clear();
        }

        /// <summary>
        /// Removes the specified value from the Set.
        /// </summary>
        /// <param name="value"> The value of the element to remove from the Set. </param>
        /// <returns> <c>true</c> if an element in the Set object has been removed successfully;
        /// otherwise <c>false</c>. </returns>
        [JSInternalFunction(Name = "delete")]
        public bool Delete(object value)
        {
            LinkedListNode<object> node;
            bool found = this.store.TryGetValue(value, out node);
            if (!found)
                return false;
            this.store.Remove(value);
            BeforeDelete?.Invoke(node);
            this.list.Remove(node);
            return true;
        }

        /// <summary>
        /// Returns a new array iterator object that contains the key/value pairs for each index in
        /// the array.
        /// </summary>
        /// <returns> An array iterator object that contains the key/value pairs for each index in
        /// the array. </returns>
        [JSInternalFunction(Name = "entries")]
        public ObjectInstance Entries()
        {
            return new SetIterator(Engine.SetIteratorPrototype, this, this.list, SetIterator.Kind.KeyAndValue);
        }

        /// <summary>
        /// Executes a provided function once per each value in the Set, in insertion order.
        /// </summary>
        /// <param name="callback"> Function to execute for each element. </param>
        /// <param name="thisArg"> Value to use as this when executing callback. </param>
        [JSInternalFunction(Name = "forEach", Length = 1)]
        public void ForEach(FunctionInstance callback, object thisArg)
        {
            var node = this.list.First;

            // This event handler is for when the user-supplied callback deletes the current
            // linked list node while we're iterating over it.
            Action<LinkedListNode<object>> beforeDeleteHandler = (deletedNode) =>
            {
                if (deletedNode == node)
                    node = node.Previous;
            };
            BeforeDelete += beforeDeleteHandler;
            try
            {

                while (node != null)
                {
                    // Call the user-supplied callback.
                    callback.Call(thisArg, node.Value, node.Value, this);

                    // Go to the next node in the linked list.
                    node = node == null ? this.list.First : node.Next;
                }

            }
            finally
            {
                BeforeDelete -= beforeDeleteHandler;
            }
        }

        /// <summary>
        /// Returns a boolean indicating whether an element with the specified value exists in the
        /// Set or not.
        /// </summary>
        /// <param name="value"> The value to test for presence in the Set. </param>
        /// <returns> <c>true</c> if an element with the specified value exists in the Set object;
        /// otherwise <c>false</c>. </returns>
        [JSInternalFunction(Name = "has")]
        public bool Has(object value)
        {
            return this.store.ContainsKey(value);
        }

        /// <summary>
        /// Returns a new array iterator object that contains the keys for each index in the array.
        /// </summary>
        /// <returns> An array iterator object that contains the keys for each index in the array. </returns>
        [JSInternalFunction(Name = "keys")]
        public ObjectInstance Keys()
        {
            return new SetIterator(Engine.SetIteratorPrototype, this, this.list, SetIterator.Kind.Key);
        }

        /// <summary>
        /// Returns a new array iterator object that contains the values for each index in the
        /// array.
        /// </summary>
        /// <returns> An array iterator object that contains the values for each index in the
        /// array. </returns>
        [JSInternalFunction(Name = "values")]
        public ObjectInstance Values()
        {
            return new SetIterator(Engine.SetIteratorPrototype, this, this.list, SetIterator.Kind.Value);
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
