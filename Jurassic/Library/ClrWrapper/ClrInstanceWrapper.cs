using System;
using System.Reflection;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a non-native CLR object instance.
    /// </summary>
    [Serializable]
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
                throw new ArgumentNullException("engine");
            if (instance == null)
                throw new ArgumentNullException("instance");
            return ClrInstanceTypeWrapper.FromCache(engine, instance.GetType());
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
