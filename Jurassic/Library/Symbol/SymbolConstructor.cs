using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in Symbol object.
    /// </summary>
    public partial class SymbolConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Symbol object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="instancePrototype"> The prototype for instances created by this function. </param>
        internal SymbolConstructor(ObjectInstance prototype, ObjectInstance instancePrototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            HasInstance = new Symbol("Symbol.hasInstance");
            IsConcatSpreadable = new Symbol("Symbol.isConcatSpreadable");
            Iterator = new Symbol("Symbol.iterator");
            Match = new Symbol("Symbol.match");
            MatchAll = new Symbol("Symbol.matchAll");
            Replace = new Symbol("Symbol.replace");
            Search = new Symbol("Symbol.search");
            Species = new Symbol("Symbol.species");
            Split = new Symbol("Symbol.split");
            ToPrimitive = new Symbol("Symbol.toPrimitive");
            ToStringTag = new Symbol("Symbol.toStringTag");
            Unscopables = new Symbol("Symbol.unscopables");

            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "Symbol", 0, instancePrototype);
            properties.Add(new PropertyNameAndValue("hasInstance", HasInstance, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("isConcatSpreadable", IsConcatSpreadable, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("iterator", Iterator, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("match", Match, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("matchAll", MatchAll, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("replace", Replace, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("search", Search, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("species", Species, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("split", Split, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("toPrimitive", ToPrimitive, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("toStringTag", ToStringTag, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("unscopables", Unscopables, PropertyAttributes.Sealed));
            InitializeProperties(properties);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// A method that determines if a constructor object recognizes an object as one of the
        /// constructor's instances. Used by the 'instanceof' operator.
        /// </summary>
        public new Symbol HasInstance { get; private set; }

        /// <summary>
        /// A Boolean valued property that if true indicates that an object should be flattened to
        /// its array elements by Array.prototype.concat.
        /// </summary>
        public Symbol IsConcatSpreadable { get; private set; }

        /// <summary>
        /// Used to override the default iterator for an object. Used by the for-of statement.
        /// </summary>
        public Symbol Iterator { get; private set; }

        /// <summary>
        /// A regular expression method that matches the regular expression against a string.
        /// Used by the String.prototype.match method.
        /// </summary>
        public Symbol Match { get; private set; }

        /// <summary>
        /// A regular expression method that returns an iterator, that yields matches of the
        /// regular expression against a string. Used by the String.prototype.matchAll method.
        /// </summary>
        public Symbol MatchAll { get; private set; }

        /// <summary>
        /// A regular expression method that replaces matched substrings of a string. Used by the
        /// String.prototype.replace method.
        /// </summary>
        public Symbol Replace { get; private set; }

        /// <summary>
        /// A regular expression method that returns the index within a string that matches the
        /// regular expression. Used by the String.prototype.search method.
        /// </summary>
        public Symbol Search { get; private set; }

        /// <summary>
        /// A function valued property that is the constructor function that is used to create
        /// derived objects.
        /// </summary>
        public Symbol Species { get; private set; }

        /// <summary>
        /// A regular expression method that splits a string at the indices that match the regular
        /// expression. Used by the String.prototype.split method.
        /// </summary>
        public Symbol Split { get; private set; }

        /// <summary>
        /// Used to override ToPrimitive behaviour.
        /// </summary>
        public Symbol ToPrimitive { get; private set; }

        /// <summary>
        /// Used to customize the behaviour of Object.prototype.toString().
        /// </summary>
        public Symbol ToStringTag { get; private set; }

        /// <summary>
        /// An object valued property whose own and inherited property names are property names
        /// that are excluded from the with environment bindings of the associated object.
        /// </summary>
        public Symbol Unscopables { get; private set; }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Symbol object is invoked like a function, e.g. var x = Symbol().
        /// </summary>
        /// <param name="description"> The name or description of the symbol (optional). </param>
        [JSCallFunction]
        public Symbol Call(string description = null)
        {
            return new Symbol(description);
        }

        /// <summary>
        /// Called when the Symbol object is called using new, e.g. var x = new Symbol().
        /// </summary>
        [JSConstructorFunction]
        public StringInstance Construct()
        {
            throw new JavaScriptException(ErrorType.TypeError, "Function is not a constructor.");
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        private static ConcurrentDictionary<string, Symbol> symbolRegistry = new ConcurrentDictionary<string, Symbol>();

        /// <summary>
        /// Searches for existing symbols in a runtime-wide symbol registry with the given key and
        /// returns it if found. Otherwise a new symbol gets created in the global symbol registry
        /// with this key.
        /// </summary>
        /// <param name="key"> The key for the symbol (also used for the description of the symbol). </param>
        /// <returns> An existing symbol with the given key if found; otherwise, a new symbol is
        /// created and returned. </returns>
        [JSInternalFunction(Name = "for")]
        public static Symbol For(string key)
        {
            return symbolRegistry.GetOrAdd(key, key2 => new Symbol(key2));
        }

        /// <summary>
        /// Retrieves a shared symbol key from the global symbol registry for the given symbol.
        /// </summary>
        /// <param name="symbol"> The symbol to find a key for. </param>
        /// <returns> A string representing the key for the given symbol if one is found on the
        /// global registry; otherwise, undefined. </returns>
        [JSInternalFunction(Name = "keyFor")]
        public static string KeyFor(Symbol symbol)
        {
            // We assume here that the symbol description is immutable.
            string key = symbol.Description;
            symbolRegistry.TryGetValue(key, out Symbol result);
            if (result == symbol)
                return key;
            return null;
        }
    }
}
