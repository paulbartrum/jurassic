using System;
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
            Iterator = new SymbolInstance(instancePrototype, "Symbol.iterator");
            Species = new SymbolInstance(instancePrototype, "Symbol.Species");
            ToPrimitive = new SymbolInstance(instancePrototype, "Symbol.toPrimitive");
            ToStringTag = new SymbolInstance(instancePrototype, "Symbol.toStringTag");

            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>(3);
            InitializeConstructorProperties(properties, "Symbol", 0, instancePrototype);
            //properties.Add(new PropertyNameAndValue("hasInstance", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("isConcatSpreadable", null, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("iterator", Iterator, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("match", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("replace", null, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("search", null, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("species", Species, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("split", null, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("toPrimitive", ToPrimitive, PropertyAttributes.Sealed));
            properties.Add(new PropertyNameAndValue("toStringTag", ToStringTag, PropertyAttributes.Sealed));
            //properties.Add(new PropertyNameAndValue("unscopables", null, PropertyAttributes.Sealed));
            FastSetProperties(properties);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Used to override the default iterator for an object. Used by the for-of statement.
        /// </summary>
        public SymbolInstance Iterator { get; internal set; }

        /// <summary>
        /// A function valued property that is the constructor function that is used to create
        /// derived objects.
        /// </summary>
        public SymbolInstance Species { get; internal set; }

        /// <summary>
        /// Used to override ToPrimitive behaviour.
        /// </summary>
        public SymbolInstance ToPrimitive { get; internal set; }

        /// <summary>
        /// Used to customize the behaviour of Object.prototype.toString().
        /// </summary>
        public SymbolInstance ToStringTag { get; internal set; }



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
