using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the JavaScript Boolean object.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public partial class BooleanInstance : ObjectInstance
    {
        private bool value;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new boolean instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="value"> The value to initialize the instance with. </param>
        public BooleanInstance(ObjectInstance prototype, bool value)
            : base(prototype)
        {
            this.value = value;
        }

        /// <summary>
        /// Creates the Boolean prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, BooleanConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            result.InitializeProperties(properties);
            return result;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the primitive value of this object.
        /// </summary>
        public bool Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get
            {
                return this.Value.ToString(CultureInfo.InvariantCulture);
            }
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
            get { return "Boolean"; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns the underlying primitive value of the current object.
        /// </summary>
        /// <returns> The underlying primitive value of the current object. </returns>
        [JSInternalFunction(Name = "valueOf")]
        public new bool ValueOf()
        {
            return this.value;
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        [JSInternalFunction(Name = "toString")]
        public string ToStringJS()
        {
            return this.value ? "true" : "false";
        }
    }
}
