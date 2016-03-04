using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the Symbol object.
    /// </summary>
    [Serializable]
    public partial class SymbolInstance : ObjectInstance
    {
        private string description;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new symbol instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="description"> An optional description of the symbol. </param>
        public SymbolInstance(ObjectInstance prototype, string description)
            : base(prototype)
        {
            this.description = description;
        }

        /// <summary>
        /// Creates the symbol prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, SymbolConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            result.FastSetProperties(properties);
            return result;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "Symbol"; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a string representing the object.
        /// </summary>
        /// <returns> A string representing the object. </returns>
        [JSInternalFunction(Name = "toString")]
        public string ToStringJS()
        {
            return $"Symbol({this.description})";
        }

        /// <summary>
        /// Returns the primitive value of a Symbol object.
        /// </summary>
        /// <returns> The primitive value of a Symbol object. </returns>
        [JSInternalFunction(Name = "valueOf")]
        public new SymbolInstance ValueOf()
        {
            return this;
        }
    }
}
