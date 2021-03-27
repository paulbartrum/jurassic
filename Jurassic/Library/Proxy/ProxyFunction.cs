using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a callable, constructable instance of the Proxy class.
    /// </summary>
    internal partial class ProxyFunction : FunctionInstance, IProxyInstance
    {
        private FunctionInstance target;
        private ObjectInstance handler;

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
        /// The proxy target.
        /// </summary>
        ObjectInstance IProxyInstance.Target
        {
            get { return target; }
        }

        /// <summary>
        /// Invalidates (switches off) the proxy.
        /// </summary>
        void IProxyInstance.Revoke()
        {
            this.target = null;
            this.handler = null;
        }

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The value that was returned from the function. </returns>
        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'apply' on a proxy that has been revoked.");

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
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'construct' on a proxy that has been revoked.");

            if (!this.target.IsConstructor)
                throw new JavaScriptException(ErrorType.TypeError, "proxy is not a constructor.");

            // Call the handler, if one exists.
            var trap = handler.GetMethod("construct");
            if (trap == null)
                return target.ConstructLateBound(newTarget, argumentValues);
            var array = Engine.Array.New(argumentValues);
            var result = trap.CallLateBound(handler, target, array, newTarget);

            // Validate.
            if (result is ObjectInstance objectInstance)
                return objectInstance;
            throw new JavaScriptException(ErrorType.TypeError, $"'construct' on proxy: trap returned non-object ('{TypeConverter.ToString(result)}').");
        }

        /// <inheritdoc/>
        public override ObjectInstance Prototype
        {
            get { return new ProxyObject(Engine, target, handler).Prototype; }
        }

        /// <inheritdoc/>
        internal override bool SetPrototype(ObjectInstance prototype, bool throwOnError)
        {
            return new ProxyObject(Engine, target, handler).SetPrototype(prototype, throwOnError);
        }

        /// <inheritdoc/>
        internal override bool IsExtensible
        {
            get { return new ProxyObject(Engine, target, handler).IsExtensible; }
        }

        /// <inheritdoc/>
        internal override bool PreventExtensions(bool throwOnError)
        {
            return new ProxyObject(Engine, target, handler).PreventExtensions(throwOnError);
        }

        /// <inheritdoc/>
        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index)
        {
            return new ProxyObject(Engine, target, handler).GetOwnPropertyDescriptor(index);
        }

        /// <inheritdoc/>
        public override PropertyDescriptor GetOwnPropertyDescriptor(object key)
        {
            return new ProxyObject(Engine, target, handler).GetOwnPropertyDescriptor(key);
        }

        /// <inheritdoc/>
        public override bool DefineProperty(object key, PropertyDescriptor descriptor, bool throwOnError)
        {
            return new ProxyObject(Engine, target, handler).DefineProperty(key, descriptor, throwOnError);
        }

        /// <inheritdoc/>
        public override bool HasProperty(object key)
        {
            return new ProxyObject(Engine, target, handler).HasProperty(key);
        }

        /// <inheritdoc/>
        protected override object GetPropertyValue(uint index, object thisValue)
        {
            return new ProxyObject(Engine, target, handler).GetPropertyValue(index, thisValue);
        }

        /// <inheritdoc/>
        public override object GetPropertyValue(PropertyReference propertyReference)
        {
            return new ProxyObject(Engine, target, handler).GetPropertyValue(propertyReference);
        }

        /// <inheritdoc/>
        internal override object GetNamedPropertyValue(object key, object thisValue)
        {
            return new ProxyObject(Engine, target, handler).GetNamedPropertyValue(key, thisValue);
        }

        /// <inheritdoc/>
        public override object GetPropertyValue(object key, object thisValue = null)
        {
            return new ProxyObject(Engine, target, handler).GetPropertyValue(key, thisValue);
        }

        /// <inheritdoc/>
        public override bool SetPropertyValue(uint index, object value, object thisValue, bool throwOnError)
        {
            return new ProxyObject(Engine, target, handler).SetPropertyValue(index, value, thisValue, throwOnError);
        }

        /// <inheritdoc/>
        public override bool SetPropertyValue(PropertyReference propertyReference, object value, object thisValue, bool throwOnError)
        {
            return new ProxyObject(Engine, target, handler).SetPropertyValue(propertyReference, value, thisValue, throwOnError);
        }

        /// <inheritdoc/>
        public override bool SetPropertyValue(object key, object value, object thisValue, bool throwOnError)
        {
            return new ProxyObject(Engine, target, handler).SetPropertyValue(key, value, thisValue, throwOnError);
        }

        /// <inheritdoc/>
        public override bool Delete(object key, bool throwOnError)
        {
            return new ProxyObject(Engine, target, handler).Delete(key, throwOnError);
        }

        /// <inheritdoc/>
        public override IEnumerable<object> OwnKeys
        {
            get { return new ProxyObject(Engine, target, handler).OwnKeys; }
        }
    }
}
