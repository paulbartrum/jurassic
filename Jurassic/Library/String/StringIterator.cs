using System;
using System.Collections.Generic;
using System.Globalization;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an iteration over a String.
    /// </summary>
    internal partial class StringIterator : ObjectInstance
    {
        private TextElementEnumerator enumerator;
        private bool done;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new string iterator.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="iteratedString"> The string to iterate over. </param>
        internal StringIterator(ObjectInstance prototype, string iteratedString)
            : base(prototype)
        {
            this.enumerator = StringInfo.GetTextElementEnumerator(iteratedString);
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



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// The tag value that is used by the base implementation of toString().
        /// </summary>
        [JSProperty(Name = "@@toStringTag")]
        public string ToStringTag
        {
            get { return "String Iterator"; }
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
            object value;

            if (this.enumerator.MoveNext())
            {
                // Return the text element.
                value = this.enumerator.Current;
                this.done = false;
            }
            else
            {
                // We've reached the end of the string.
                value = Undefined.Value;
                this.done = true;
            }

            // Return the result.
            var result = Engine.Object.Construct();
            result.InitializeProperties(new List<PropertyNameAndValue>(2)
                {
                    new PropertyNameAndValue("value", value, PropertyAttributes.FullAccess),
                    new PropertyNameAndValue("done", done, PropertyAttributes.FullAccess),
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
                try
                {
                    return this.enumerator.ElementIndex;
                }
                catch (Exception)
                {
                    return 0;
                }
            }

        }

        /// <summary>
        /// Gets current character. Used by debugger decoration only.
        /// </summary>
        public string Current
        {
            get
            {
                try
                {
                    return this.enumerator.Current as string;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
