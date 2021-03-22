namespace Jurassic.Library
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ProxyInstance : ObjectInstance
    {
        private readonly ObjectInstance target;
        private readonly ObjectInstance handler;

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new proxy instance.
        /// </summary>
        /// <param name="engine"> The next object in the prototype chain. </param>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        internal ProxyInstance(ScriptEngine engine, ObjectInstance target, ObjectInstance handler)
            : base(engine.Object.InstancePrototype)
        {
            // target and handler are both non-null.
            this.target = target;
            this.handler = handler;
        }

        /// <summary>
        /// Gets the next object in the prototype chain.  There is no corresponding property in
        /// javascript (it is is *not* the same as the prototype property), instead use
        /// Object.getPrototypeOf(). Returns <c>null</c> for the root object in the prototype
        /// chain. Use <see cref="SetPrototype(ObjectInstance, bool)"/> to set this value.
        /// </summary>
        /// <remarks>
        /// Enforces the following invariants:
        /// * The return value must be either an Object or null.
        /// * If the target object is not extensible, the return value must be the same as
        ///   Object.getPrototypeOf() applied to the proxy object's target object.
        /// </remarks>
        public override ObjectInstance Prototype
        {
            get
            {
                // Call the handler, if one exists.
                var trap = handler.GetMethod("getPrototypeOf");
                if (trap == null)
                    return target.Prototype;
                var result = trap.CallLateBound(handler, target);

                // Validate.
                ObjectInstance handlerPrototype;
                if (result == Null.Value)
                    handlerPrototype = null;
                else if (result is ObjectInstance obj)
                    handlerPrototype = obj;
                else
                    throw new JavaScriptException(ErrorType.TypeError, "'getPrototypeOf' on proxy: trap returned neither object nor null.");
                if (target.IsExtensible)
                    return handlerPrototype;
                if (target.Prototype != handlerPrototype)
                    throw new JavaScriptException(ErrorType.TypeError, "'getPrototypeOf' on proxy: proxy target is non-extensible but the trap did not return its actual prototype.");
                return handlerPrototype;
            }
        }

        /// <summary>
        /// Sets the next object in the prototype chain. Can be <c>null</c>, which indicates there
        /// are no further objects in the chain.
        /// </summary>
        /// <param name="prototype"> The new prototype. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the prototype could not
        /// be set.  This can happen if the object is non-extensible or if setting the prototype
        /// would introduce a cyclic dependency. </param>
        /// <returns> <c>true</c> if the prototype was successfully applied; <c>false</c> otherwise. </returns>
        internal override bool SetPrototype(ObjectInstance prototype, bool throwOnError)
        {
            // Call the handler, if one exists.
            var trap = handler.GetMethod("setPrototypeOf");
            if (trap == null)
                return target.SetPrototype(prototype, throwOnError);
            var result = TypeConverter.ToBoolean(trap.CallLateBound(handler, target, prototype));

            // Validate.
            if (result == false)
            {
                if (throwOnError)
                    throw new JavaScriptException(ErrorType.TypeError, "'setPrototypeOf' on proxy: trap returned falsish.");
                return false;
            }
            if (target.IsExtensible)
                return true;
            // Trap returned true, and target is non-extensible.
            if (target.Prototype != prototype)
                throw new JavaScriptException(ErrorType.TypeError, "'setPrototypeOf' on proxy: proxy target is non-extensible but the trap did not return its actual prototype.");
            return true;
        }

        /// <summary>
        /// Gets a value that indicates whether the object can have new properties added to it.
        /// Called by Object.isExtensible(). Use <see cref="PreventExtensions"/> to set this value.
        /// </summary>
        internal override bool IsExtensible
        {
            get
            {
                // Call the handler, if one exists.
                var trap = handler.GetMethod("isExtensible");
                if (trap == null)
                    return target.IsExtensible;
                var result = TypeConverter.ToBoolean(trap.CallLateBound(handler, target));

                // Validate.
                var targetResult = target.IsExtensible;
                if (result != targetResult)
                    throw new JavaScriptException(ErrorType.TypeError, $"'isExtensible' on proxy: trap result does not reflect extensibility of proxy target (which is '{TypeConverter.ToString(targetResult)}').");
                return result;
            }
        }

        /// <summary>
        /// Makes this object non-extensible, which means no new properties can be added to it.
        /// </summary>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the object could not
        /// be made non-extensible. </param>
        /// <returns> <c>true</c> if the operation was successful, <c>false</c> otherwise. </returns>
        internal override bool PreventExtensions(bool throwOnError)
        {
            // Call the handler, if one exists.
            var trap = handler.GetMethod("preventExtensions");
            if (trap == null)
                return target.PreventExtensions(throwOnError);
            var result = TypeConverter.ToBoolean(trap.CallLateBound(handler, target));

            // Validate.
            if (!result)
            {
                if (throwOnError)
                    throw new JavaScriptException(ErrorType.TypeError, "'preventExtensions' on proxy: trap returned falsish.");
                return false;
            }
            var targetIsExtensible = target.IsExtensible;
            if (targetIsExtensible)
                throw new JavaScriptException(ErrorType.TypeError, "'preventExtensions' on proxy: trap returned truish but the proxy target is extensible.");
            return true;
        }

        /// <summary>
        /// Gets a descriptor for the property with the given name.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> A property descriptor containing the property value and attributes. </returns>
        /// <remarks>
        /// Enforces the following invariants:
        /// * The result of [[GetOwnProperty]] must be either an Object or undefined.
        /// * A property cannot be reported as non-existent, if it exists as a non-configurable own
        ///   property of the target object.
        /// * A property cannot be reported as non-existent, if the target object is not
        ///   extensible, unless it does not exist as an own property of the target object.
        /// * A property cannot be reported as existent, if the target object is not extensible,
        ///   unless it exists as an own property of the target object.
        /// * A property cannot be reported as non-configurable, unless it exists as a
        ///   non-configurable own property of the target object.
        /// * A property cannot be reported as both non-configurable and non-writable, unless it
        ///   exists as a non-configurable, non-writable own property of the target object.
        /// </remarks>
        public override PropertyDescriptor GetOwnPropertyDescriptor(object key)
        {
            // Call the handler, if one exists.
            var trap = handler.GetMethod("getOwnPropertyDescriptor");
            if (trap == null)
                return target.GetOwnPropertyDescriptor(key);
            var result = trap.CallLateBound(handler, target, key);

            // Validate.
            var targetDescriptor = target.GetOwnPropertyDescriptor(key);
            if (result is ObjectInstance propertyDescriptorObject)
            {
                var propertyDescriptor = PropertyDescriptor.FromObject(propertyDescriptorObject, PropertyDescriptor.Undefined);
                if (!IsCompatiblePropertyDescriptor(target.IsExtensible, propertyDescriptor, targetDescriptor))
                    throw new JavaScriptException(ErrorType.TypeError, "sdff");
                if (!propertyDescriptor.IsConfigurable)
                {
                    if (!targetDescriptor.Exists || targetDescriptor.IsConfigurable)
                        throw new JavaScriptException(ErrorType.TypeError, "sdff");
                    if (!propertyDescriptor.IsWritable && targetDescriptor.IsWritable)
                        throw new JavaScriptException(ErrorType.TypeError, "sdff");
                }
                return propertyDescriptor;
            }
            else if (result == null || result == Undefined.Value)
            {
                if (!targetDescriptor.Exists)
                    return PropertyDescriptor.Missing;
                if (!targetDescriptor.IsConfigurable)
                    throw new JavaScriptException(ErrorType.TypeError, "sdff");
                if (!target.IsExtensible)
                    throw new JavaScriptException(ErrorType.TypeError, "sdff");
                return PropertyDescriptor.Missing;
            }
            else
                throw new JavaScriptException(ErrorType.TypeError, "sdff");
        }

        /// <summary>
        /// Defines or redefines the value and attributes of a property.  The prototype chain is
        /// not searched so if the property exists but only in the prototype chain a new property
        /// will be created.
        /// </summary>
        /// <param name="key"> The property key of the property to modify. </param>
        /// <param name="descriptor"> The property value and attributes. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is not configurable or the object is sealed. </param>
        /// <returns> <c>true</c> if the property was successfully modified; <c>false</c> otherwise. </returns>
        public override bool DefineProperty(object key, PropertyDescriptor descriptor, bool throwOnError)
        {
            // Call the handler, if one exists.
            var trap = handler.GetMethod("defineProperty");
            if (trap == null)
                return target.DefineProperty(key, descriptor, throwOnError);
            var result = TypeConverter.ToBoolean(trap.CallLateBound(handler, target, key, descriptor.ToObject(Engine)));

            // Validate.
            if (!result)
            {
                if (throwOnError)
                    throw new JavaScriptException(ErrorType.TypeError, $"'defineProperty' on proxy: trap returned falsish for property '{TypeConverter.ToString(key)}'.");
                return false;
            }
            var targetDescriptor = target.GetOwnPropertyDescriptor(key);
            if (targetDescriptor.Exists)
            {
                if (!IsCompatiblePropertyDescriptor(target.IsExtensible, descriptor, targetDescriptor))
                    throw new JavaScriptException(ErrorType.TypeError, $"'defineProperty' on proxy: trap returned truish for adding property '{TypeConverter.ToString(key)}' that is incompatible with the existing property in the proxy target.");
                if (descriptor.Exists && !descriptor.IsConfigurable && targetDescriptor.IsConfigurable)
                    throw new JavaScriptException(ErrorType.TypeError, $"'defineProperty' on proxy: trap returned truish for defining non-configurable property '{TypeConverter.ToString(key)}' which is either non-existent or configurable in the proxy target.");
                if (!targetDescriptor.IsAccessor && !targetDescriptor.IsConfigurable && targetDescriptor.IsWritable)
                    if (descriptor.Exists && !descriptor.IsWritable)
                        throw new JavaScriptException(ErrorType.TypeError, $"'defineProperty' on proxy: trap returned truish for defining non-configurable property '{TypeConverter.ToString(key)}' which cannot be non-writable, unless there exists a corresponding non-configurable, non-writable own property of the target object.");
            }
            else
            {
                if (!target.IsExtensible)
                    throw new JavaScriptException(ErrorType.TypeError, $"'defineProperty' on proxy: trap returned truish for adding property '{TypeConverter.ToString(key)}' to the non-extensible proxy target.");
                if (descriptor.Exists && !descriptor.IsConfigurable)
                    throw new JavaScriptException(ErrorType.TypeError, $"'defineProperty' on proxy: trap returned truish for defining non-configurable property '{TypeConverter.ToString(key)}' which is either non-existent or configurable in the proxy target.");
            }
            return true;
        }
    }
}
