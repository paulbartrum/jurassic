using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a non-native CLR object instance.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ClrInstanceWrapperDebugView))]
    public class ClrInstanceWrapper : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new ClrInstanceWrapper object.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="instance"> The CLR object instance to wrap. </param>
        public ClrInstanceWrapper(ScriptEngine engine, object instance)
            : base(GetPrototypeObject(engine, instance))
        {
            this.WrappedInstance = instance;
        }

        /// <summary>
        /// Returns an object instance to serve as the next object in the prototype chain.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="instance"> The CLR object instance to wrap. </param>
        /// <returns> The next object in the prototype chain. </returns>
        private static ObjectInstance GetPrototypeObject(ScriptEngine engine, object instance)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            return ClrInstanceTypeWrapper.FromCache(engine, instance.GetType());
        }

        /// <summary>
        /// Creates an instance of ClrInstanceWrapper or ArrayInstance based on object type.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="instance"> The CLR object instance to wrap. </param>
        public static ObjectInstance Create(ScriptEngine engine, object instance)
        {
            if (instance is IEnumerable enumerable)
            {
                var wrappedList = instance is ICollection ? new List<object>(((ICollection)instance).Count) : new List<object>();
                var enumerator = enumerable.GetEnumerator();
                while (enumerator.MoveNext())
                    wrappedList.Add(Create(engine, enumerator.Current));
                return engine.Array.New(wrappedList.ToArray());
            }

            return new ClrInstanceWrapper(engine, instance);
        }

        //     PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the .NET instance this object represents.
        /// </summary>
        public object WrappedInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get { return this.WrappedInstance?.ToString(); }
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
            get { return this.WrappedInstance?.GetType().ToString(); }
        }




        //     OBJECTINSTANCE OVERRIDES
        //_________________________________________________________________________________________

        ///// <summary>
        ///// Returns a primitive value that represents the current object.  Used by the addition and
        ///// equality operators.
        ///// </summary>
        ///// <param name="hint"> Indicates the preferred type of the result. </param>
        ///// <returns> A primitive value that represents the current object. </returns>
        //internal override object GetPrimitiveValue(PrimitiveTypeHint typeHint)
        //{
        //    // If this wrapper is for a primitive.
        //    if (TypeUtilities.IsPrimitive(this.WrappedInstance) == true)
        //        return this.WrappedInstance;

        //    // Otherwise, use the default implementation.
        //    return base.GetPrimitiveValue(typeHint);
        //}




        //     OBJECT OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a textual representation of this object.
        /// </summary>
        /// <returns> A textual representation of this object. </returns>
        public override string ToString()
        {
            return this.WrappedInstance.ToString();
        }
    }
}
