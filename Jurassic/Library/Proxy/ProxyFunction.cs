namespace Jurassic.Library
{
    /// <summary>
    /// Represents a callable, constructable instance of the Proxy class.
    /// </summary>
    internal partial class ProxyFunction : FunctionInstance
    {
        private readonly FunctionInstance target;
        private readonly ObjectInstance handler;

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new proxy instance.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="target"> A target object to wrap with Proxy. It can be any sort of object,
        /// including a native array, a function, or even another proxy. </param>
        /// <param name="handler"> An object whose properties are functions that define the
        /// behavior of the proxy when an operation is performed on it. </param>
        internal ProxyFunction(ScriptEngine engine, FunctionInstance target, ObjectInstance handler)
            : base(engine)
        {
            this.target = target;
            this.handler = handler;
        }

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The value that was returned from the function. </returns>
        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            // Call the handler, if one exists.
            var trap = handler.GetMethod("apply");
            if (trap == null)
                return target.CallLateBound(thisObject, argumentValues);
            var array = Engine.Array.New(argumentValues);
            return trap.CallLateBound(handler, target, thisObject, array);
        }

        /// <summary>
        /// Creates an object, using this function as the constructor.
        /// </summary>
        /// <param name="newTarget"> The value of 'new.target'. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The object that was created. </returns>
        public override ObjectInstance ConstructLateBound(FunctionInstance newTarget, params object[] argumentValues)
        {
            // Call the handler, if one exists.
            var trap = handler.GetMethod("construct");
            if (trap == null)
                return target.ConstructLateBound(newTarget, argumentValues);
            var array = Engine.Array.New(argumentValues);
            var result = trap.CallLateBound(handler, target, array, newTarget);

            // Validate.
            if (result is ObjectInstance objectInstance)
                return objectInstance;
            throw new JavaScriptException(ErrorType.TypeError, "'construct' on proxy.");
        }

        /// <inheritdoc/>
        public override ObjectInstance Prototype
        {
            get { return new ProxyInstance(Engine, target, handler).Prototype; }
        }

        /// <inheritdoc/>
        internal override bool SetPrototype(ObjectInstance prototype, bool throwOnError)
        {
            return new ProxyInstance(Engine, target, handler).SetPrototype(prototype, throwOnError);
        }

        /// <inheritdoc/>
        internal override bool IsExtensible
        {
            get { return new ProxyInstance(Engine, target, handler).IsExtensible; }
        }

        /// <inheritdoc/>
        internal override bool PreventExtensions(bool throwOnError)
        {
            return new ProxyInstance(Engine, target, handler).PreventExtensions(throwOnError);
        }

        /// <inheritdoc/>
        public override PropertyDescriptor GetOwnPropertyDescriptor(object key)
        {
            return new ProxyInstance(Engine, target, handler).GetOwnPropertyDescriptor(key);
        }

        /// <inheritdoc/>
        public override bool DefineProperty(object key, PropertyDescriptor descriptor, bool throwOnError)
        {
            return new ProxyInstance(Engine, target, handler).DefineProperty(key, descriptor, throwOnError);
        }

        /// <inheritdoc/>
        public override bool HasProperty(object key)
        {
            return new ProxyInstance(Engine, target, handler).HasProperty(key);
        }

        /// <inheritdoc/>
        public override object GetPropertyValue(object key, object thisValue = null)
        {
            return new ProxyInstance(Engine, target, handler).GetPropertyValue(key, thisValue);
        }

        /// <inheritdoc/>
        public override bool SetPropertyValue(object key, object value, object thisValue, bool throwOnError)
        {
            return new ProxyInstance(Engine, target, handler).SetPropertyValue(key, value, thisValue, throwOnError);
        }

        /// <inheritdoc/>
        public override bool Delete(object key, bool throwOnError)
        {
            return new ProxyInstance(Engine, target, handler).Delete(key, throwOnError);
        }
    }
}
