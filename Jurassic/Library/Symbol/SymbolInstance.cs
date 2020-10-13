using System.Diagnostics;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the Symbol object.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public partial class SymbolInstance : ObjectInstance
    {
        private Symbol symbol;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new symbol instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="symbol"> The symbol primitive. </param>
        public SymbolInstance(ObjectInstance prototype, Symbol symbol)
            : base(prototype)
        {
            this.symbol = symbol;
        }

        /// <summary>
        /// Initializes the prototype properties.
        /// </summary>
        /// <param name="obj"> The object to set the properties on. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static void InitializePrototypeProperties(ObjectInstance obj, SymbolConstructor constructor)
        {
            var engine = obj.Engine;
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(Symbol.ToStringTag, "Symbol", PropertyAttributes.Configurable));
            obj.InitializeProperties(properties);
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a string representing the object.
        /// </summary>
        /// <returns> A string representing the object. </returns>
        [JSInternalFunction(Name = "toString")]
        public override string ToString()
        {
            return this.symbol.ToString();
        }

        /// <summary>
        /// Returns the primitive value of a Symbol object.
        /// </summary>
        /// <returns> The primitive value of a Symbol object. </returns>
        [JSInternalFunction(Name = "valueOf")]
        public new Symbol ValueOf()
        {
            return this.symbol;
        }

        /// <summary>
        /// Converts a Symbol object to a primitive value.
        /// </summary>
        /// <param name="hint"> Specifies the conversion behaviour.  Must be "default", "string" or "number". </param>
        /// <returns> The primitive value of a Symbol object. </returns>
        [JSInternalFunction(Name = "@@toPrimitive")]
        public Symbol ToPrimitive(string hint)
        {
            return this.symbol;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________


        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayValue; }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayType
        {
            get { return "Symbol"; }
        }
    }
}
