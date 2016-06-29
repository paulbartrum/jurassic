using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an iteration over an array-like object.
    /// </summary>
    internal partial class ArrayIterator : ObjectInstance
    {
        private ObjectInstance iteratedObject;
        private int nextIndex;

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
        /// Creates a new array iterator.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="array"> The array-like object to iterate over. </param>
        /// <param name="kind"> The type of values to return. </param>
        internal ArrayIterator(ObjectInstance prototype, ObjectInstance array, Kind kind)
            : base(prototype)
        {
            this.iteratedObject = array;
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
            result.FastSetProperties(properties);
            return result;
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// The tag value that is used by the base implementation of toString().
        /// </summary>
        [JSProperty(Name = "@@toStringTag")]
        public string ToStringTag
        {
            get { return "Array Iterator"; }
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
            // Determine the length of the array-like object.
            int length;
            if (this.iteratedObject is TypedArrayInstance)
                length = ((TypedArrayInstance)this.iteratedObject).Length;
            else
                length = TypeConverter.ToInt32(this.iteratedObject["length"]);

            object value;
            bool done = false;

            if (this.nextIndex >= length)
            {
                // We've reached the end of the list.
                value = Undefined.Value;
                done = true;
            }
            else
            {
                // Determine the iterator value, which depends on the kind of iterator.
                switch (this.kind)
                {
                    case Kind.Key:
                        value = this.nextIndex;
                        break;

                    case Kind.Value:
                        value = this.iteratedObject[this.nextIndex];
                        break;

                    case Kind.KeyAndValue:
                        value = Engine.Array.Construct(this.nextIndex, this.iteratedObject[this.nextIndex]);
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported array iterator kind {this.kind}.");
                }

                // Move to the next element.
                this.nextIndex++;
            }


            // Return the result.
            var result = Engine.Object.Construct();
            result.FastSetProperties(new List<PropertyNameAndValue>(2)
            {
                new PropertyNameAndValue("value", value, PropertyAttributes.FullAccess),
                new PropertyNameAndValue("done", done, PropertyAttributes.FullAccess),
            });
            return result;
        }
    }
}
