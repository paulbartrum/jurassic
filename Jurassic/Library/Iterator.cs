using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a general iterator.
    /// </summary>
    internal partial class Iterator : ObjectInstance
    {
        private IEnumerator<object> enumerator;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new iterator.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="enumerable"> The enumerable to iterate over. </param>
        internal Iterator(ObjectInstance prototype, IEnumerable<object> enumerable)
            : base(prototype)
        {
            this.enumerator = enumerable.GetEnumerator();
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
            get { return "Iterator"; }
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
            // Move to the next element.
            var done = !enumerator.MoveNext();
            var value = done == true ? Undefined.Value : enumerator.Current;

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
