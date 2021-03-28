using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an iteration over a Map.
    /// </summary>
    internal partial class MapIterator : ObjectInstance
    {
        private MapInstance map;
        private LinkedList<KeyValuePair<object, object>> list;
        private LinkedListNode<KeyValuePair<object, object>> lastNode;
        private bool done;

        internal enum Kind
        {
            Key,
            Value,
            KeyAndValue,
        }
        private Kind kind;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new map iterator.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="map"> The map to iterate over. </param>
        /// <param name="list"> The linked list to iterate over. </param>
        /// <param name="kind"> The type of values to return. </param>
        internal MapIterator(ObjectInstance prototype, MapInstance map, LinkedList<KeyValuePair<object, object>> list, Kind kind)
            : base(prototype)
        {
            this.map = map;
            this.map.BeforeDelete += Map_BeforeDelete;
            this.list = list;
            this.kind = kind;
        }

        /// <summary>
        /// Creates the array iterator prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine)
        {
            var result = ObjectInstance.CreateRawObject(engine.BaseIteratorPrototype);
            var properties = GetDeclarativeProperties(engine);
            result.InitializeProperties(properties);
            return result;
        }

        /// <summary>
        /// Called before a linked list node is deleted.
        /// </summary>
        /// <param name="node"> The node that is being deleted. </param>
        private void Map_BeforeDelete(LinkedListNode<KeyValuePair<object, object>> node)
        {
            if (node == this.lastNode)
                this.lastNode = node.Previous;
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// The tag value that is used by the base implementation of toString().
        /// </summary>
        [JSProperty(Name = "@@toStringTag")]
        public string ToStringTag
        {
            get { return "Map Iterator"; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Moves the iterator to the next element in the array.
        /// </summary>
        /// <returns> An object containing two properies: value and done. </returns>
        [JSInternalFunction(Name = "next")]
        public ObjectInstance Next()
        {
            // Determine the next node in the list.
            var node = lastNode == null ? this.list.First : lastNode.Next;

            object value;

            if (this.done == true || node == null)
            {
                // We've reached the end of the list.
                value = Undefined.Value;
                if (this.done == false)
                    this.map.BeforeDelete -= Map_BeforeDelete;
                this.done = true;
            }
            else
            {
                // Determine the iterator value, which depends on the kind of iterator.
                switch (this.kind)
                {
                    case Kind.Key:
                        value = node.Value.Key;
                        break;

                    case Kind.Value:
                        value = node.Value.Value;
                        break;

                    case Kind.KeyAndValue:
                        value = Engine.Array.Construct(node.Value.Key, node.Value.Value);
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported array iterator kind {this.kind}.");
                }

                // Move to the next element.
                this.lastNode = node;
            }

            // Return the result.
            var result = Engine.Object.Construct();
            result.InitializeProperties(new List<PropertyNameAndValue>(2)
                {
                    new PropertyNameAndValue("value", value, PropertyAttributes.FullAccess),
                    new PropertyNameAndValue("done", this.done, PropertyAttributes.FullAccess),
                });
            return result;
        }



        //     .NET PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets wheter the end is reached. Used by debugger decoration only.
        /// </summary>
        internal bool Done
        {
            get { return this.done; }
        }

        /// <summary>
        /// Gets current index. Used by debugger decoration only.
        /// </summary>
        internal int IteratorIndex
        {
            get
            {
                return this.lastNode == null
                    ? 0
                    : new List<KeyValuePair<object, object>>(this.list).IndexOf(this.lastNode.Value) + 1;
            }
        }

        /// <summary>
        /// Gets what is iterated - key, value or both
        /// </summary>
        internal Kind IteratorKind
        {
            get { return this.kind; }
        }

        /// <summary>
        /// Gets the iterated Map
        /// </summary>
        internal MapInstance Map
        {
            get { return this.map; }
        }
    }
}
