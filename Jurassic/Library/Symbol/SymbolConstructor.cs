using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in Symbol object.
    /// </summary>
    [Serializable]
    public partial class SymbolConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Symbol object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal SymbolConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>(3);
            InitializeConstructorProperties(properties, "Symbol", 0, SymbolInstance.CreatePrototype(Engine, this));
            //properties.Add(new PropertyNameAndValue("hasInstance", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("isConcatSpreadable", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("iterator", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("match", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("replace", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("search", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("species", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("split", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("toPrimitive", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("toStringTag", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("unscopables", null, PropertyAttributes.Sealed));
            FastSetProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Symbol object is invoked like a function, e.g. var x = Symbol().
        /// </summary>
        /// <param name="description"> The name or description of the symbol (optional). </param>
        [JSCallFunction]
        public SymbolInstance Call(string description = null)
        {
            return new SymbolInstance(this.InstancePrototype, description);
        }

        /// <summary>
        /// Called when the Symbol object is called using new, e.g. var x = new Symbol().
        /// </summary>
        [JSConstructorFunction]
        public StringInstance Construct()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Function is not a constructor.");
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Searches for existing symbols in a runtime-wide symbol registry with the given key and
        /// returns it if found. Otherwise a new symbol gets created in the global symbol registry
        /// with this key.
        /// </summary>
        /// <param name="key"> The key for the symbol (also used for the description of the symbol). </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "for")]
        public SymbolInstance For(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a shared symbol key from the global symbol registry for the given symbol.
        /// </summary>
        /// <param name="symbol"> The symbol to find a key for. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "keyFor")]
        public string KeyFor(SymbolInstance symbol)
        {
            throw new NotImplementedException();
        }
    }
}
