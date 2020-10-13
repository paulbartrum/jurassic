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
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "Symbol", 0, instancePrototype);
            properties.Add(new PropertyNameAndValue("hasInstance", Symbol.HasInstance, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("isConcatSpreadable", Symbol.IsConcatSpreadable, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("iterator", Symbol.Iterator, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("match", Symbol.Match, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("matchAll", Symbol.MatchAll, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("replace", Symbol.Replace, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("search", Symbol.Search, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("species", Symbol.Species, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("split", Symbol.Split, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("toPrimitive", Symbol.ToPrimitive, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("toStringTag", Symbol.ToStringTag, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("unscopables", Symbol.Unscopables, PropertyAttributes.Sealed));
            InitializeProperties(properties);
        }



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
