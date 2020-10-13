namespace Jurassic.Library
{
    /// <summary>
    /// Represents a symbol primitive.
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Creates a new symbol primitive.
        /// </summary>
        /// <param name="description"> An optional description of the symbol. </param>
        public Symbol(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// An optional description of the symbol.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Converts the value to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Symbol({Description})";
        }



        //     WELL-KNOWN SYMBOLS
        //_________________________________________________________________________________________

        /// <summary>
        /// A method that determines if a constructor object recognizes an object as one of the
        /// constructor's instances. Used by the 'instanceof' operator.
        /// </summary>
        public static readonly Symbol HasInstance = new Symbol("Symbol.hasInstance");

        /// <summary>
        /// A Boolean valued property that if true indicates that an object should be flattened to
        /// its array elements by Array.prototype.concat.
        /// </summary>
        public static readonly Symbol IsConcatSpreadable = new Symbol("Symbol.isConcatSpreadable");

        /// <summary>
        /// Used to override the default iterator for an object. Used by the for-of statement.
        /// </summary>
        public static readonly Symbol Iterator = new Symbol("Symbol.iterator");

        /// <summary>
        /// A regular expression method that matches the regular expression against a string.
        /// Used by the String.prototype.match method.
        /// </summary>
        public static readonly Symbol Match = new Symbol("Symbol.match");

        ///// <summary>
        ///// A regular expression method that returns an iterator, that yields matches of the
        ///// regular expression against a string. Used by the String.prototype.matchAll method.
        ///// </summary>
        //public static readonly Symbol MatchAll = new Symbol("Symbol.matchAll");

        /// <summary>
        /// A regular expression method that replaces matched substrings of a string. Used by the
        /// String.prototype.replace method.
        /// </summary>
        public static readonly Symbol Replace = new Symbol("Symbol.replace");

        /// <summary>
        /// A regular expression method that returns the index within a string that matches the
        /// regular expression. Used by the String.prototype.search method.
        /// </summary>
        public static readonly Symbol Search = new Symbol("Symbol.search");

        /// <summary>
        /// A function valued property that is the constructor function that is used to create
        /// derived objects.
        /// </summary>
        public static readonly Symbol Species = new Symbol("Symbol.species");

        /// <summary>
        /// A regular expression method that splits a string at the indices that match the regular
        /// expression. Used by the String.prototype.split method.
        /// </summary>
        public static readonly Symbol Split = new Symbol("Symbol.split");

        /// <summary>
        /// Used to override ToPrimitive behaviour.
        /// </summary>
        public static readonly Symbol ToPrimitive = new Symbol("Symbol.toPrimitive");

        /// <summary>
        /// Used to customize the behaviour of Object.prototype.toString().
        /// </summary>
        public static readonly Symbol ToStringTag = new Symbol("Symbol.toStringTag");

        /// <summary>
        /// An object valued property whose own and inherited property names are property names
        /// that are excluded from the with environment bindings of the associated object.
        /// </summary>
        public static readonly Symbol Unscopables = new Symbol("Symbol.unscopables");
    }
}
