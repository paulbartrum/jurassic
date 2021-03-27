using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the Proxy class, one that is non-callable and non-constructable.
    /// </summary>
    public partial class ProxyObject : ObjectInstance, IProxyInstance
    {
        private ObjectInstance target;
        private ObjectInstance handler;

        /// <summary>
        /// Creates a new proxy instance.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="target"> A target object to wrap with Proxy. It can be any sort of object,
        /// including a native array, a function, or even another proxy. </param>
        /// <param name="handler"> An object whose properties are functions that define the
        /// behavior of the proxy when an operation is performed on it. </param>
        internal ProxyObject(ScriptEngine engine, ObjectInstance target, ObjectInstance handler)
            : base(engine)
        {
            // target and handler are both non-null.
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
                // Check for revocation.
                if (target == null || handler == null)
                    throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'getPrototypeOf' on a proxy that has been revoked.");

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
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'setPrototypeOf' on a proxy that has been revoked.");

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
                // Check for revocation.
                if (target == null || handler == null)
                    throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'isExtensible' on a proxy that has been revoked.");

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
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'preventExtensions' on a proxy that has been revoked.");

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
        /// Gets a descriptor for the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes. </returns>
        /// <remarks> The prototype chain is not searched. </remarks>
        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index)
        {
            return GetOwnPropertyDescriptor(index.ToString());
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
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'getOwnPropertyDescriptor' on a proxy that has been revoked.");

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
                    throw new JavaScriptException(ErrorType.TypeError, $"'getOwnPropertyDescriptor' on proxy: trap returned descriptor for property '{TypeConverter.ToString(key)}' that is incompatible with the existing property in the proxy target.");
                if (!propertyDescriptor.IsConfigurable)
                {
                    if (!targetDescriptor.Exists || targetDescriptor.IsConfigurable)
                        throw new JavaScriptException(ErrorType.TypeError, $"'getOwnPropertyDescriptor' on proxy: trap reported non-configurability for property '{TypeConverter.ToString(key)}' which is either non-existent or configurable in the proxy target.");
                    if (!propertyDescriptor.IsWritable && targetDescriptor.IsWritable)
                        throw new JavaScriptException(ErrorType.TypeError, $"'getOwnPropertyDescriptor' on proxy: trap reported non-configurable and writable for property '{TypeConverter.ToString(key)}' which is non-configurable, non-writable in the proxy target.");
                }
                return propertyDescriptor;
            }
            else if (result == null || result == Undefined.Value)
            {
                if (!targetDescriptor.Exists)
                    return PropertyDescriptor.Missing;
                if (!targetDescriptor.IsConfigurable)
                    throw new JavaScriptException(ErrorType.TypeError, $"'getOwnPropertyDescriptor' on proxy: trap returned undefined for property '{TypeConverter.ToString(key)}' which is non-configurable in the proxy target.");
                if (!target.IsExtensible)
                    throw new JavaScriptException(ErrorType.TypeError, $"'getOwnPropertyDescriptor' on proxy: trap returned undefined for property '{TypeConverter.ToString(key)}' which exists in the non-extensible proxy target.");
                return PropertyDescriptor.Missing;
            }
            else
                throw new JavaScriptException(ErrorType.TypeError, $"'getOwnPropertyDescriptor' on proxy: trap returned neither object nor undefined for property '{TypeConverter.ToString(key)}'.");
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
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'defineProperty' on a proxy that has been revoked.");

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

        /// <summary>
        /// Determines if a property with the given name exists.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> <c>true</c> if the property exists on this object or in the prototype chain;
        /// <c>false</c> otherwise. </returns>
        public override bool HasProperty(object key)
        {
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'has' on a proxy that has been revoked.");

            // Call the handler, if one exists.
            var trap = handler.GetMethod("has");
            if (trap == null)
                return target.HasProperty(key);
            var result = TypeConverter.ToBoolean(trap.CallLateBound(handler, target, key));

            // Validate.
            if (!result)
            {
                var targetDescriptor = target.GetOwnPropertyDescriptor(key);
                if (targetDescriptor.Exists)
                {
                    if (!targetDescriptor.IsConfigurable)
                        throw new JavaScriptException(ErrorType.TypeError, $"'has' on proxy: trap returned falsish for property '{TypeConverter.ToString(key)}' which exists in the proxy target as non-configurable.");
                    if (!target.IsExtensible)
                        throw new JavaScriptException(ErrorType.TypeError, $"'has' on proxy: trap returned falsish for property '{TypeConverter.ToString(key)}' but the proxy target is not extensible.");
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the value of the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <param name="thisValue"> The value of the "this" keyword inside a getter. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        protected override object GetPropertyValue(uint index, object thisValue)
        {
            return GetPropertyValue(index.ToString(), thisValue);
        }

        /// <summary>
        /// Gets the value of the property with the given name.
        /// </summary>
        /// <param name="propertyReference"> The name of the property. </param>
        /// <returns> The value of the property, or <see cref="Undefined.Value"/> if the property
        /// doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        public override object GetPropertyValue(PropertyReference propertyReference)
        {
            return GetPropertyValue(propertyReference.Name, this);
        }

        /// <summary>
        /// Gets the value of the property with the given name.  The name cannot be an array index.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol).  Cannot be an array index. </param>
        /// <param name="thisValue"> The value of the "this" keyword inside a getter. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        internal override object GetNamedPropertyValue(object key, object thisValue)
        {
            return GetPropertyValue(key, thisValue);
        }

        /// <summary>
        /// Gets the value of the property with the given name.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <param name="thisValue"> The value of the "this" keyword inside a getter. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        public override object GetPropertyValue(object key, object thisValue)
        {
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'get' on a proxy that has been revoked.");

            // Call the handler, if one exists.
            var trap = handler.GetMethod("get");
            if (trap == null)
                return target.GetPropertyValue(key, thisValue);
            var result = trap.CallLateBound(handler, target, key, thisValue);

            // Validate.
            var targetDescriptor = target.GetOwnPropertyDescriptor(key);
            if (targetDescriptor.Exists && !targetDescriptor.IsConfigurable)
            {
                if (!targetDescriptor.IsAccessor && !targetDescriptor.IsWritable && !TypeComparer.SameValue(result, targetDescriptor.Value))
                    throw new JavaScriptException(ErrorType.TypeError, $"'get' on proxy: property '{TypeConverter.ToString(key)}' is a read-only and non-configurable data property on the proxy target but the proxy did not return its actual value (expected '{TypeConverter.ToString(targetDescriptor.Value)}' but got '{TypeConverter.ToString(result)}').");
                if (targetDescriptor.IsAccessor && targetDescriptor.Getter == null && result != null && result != Undefined.Value)
                    throw new JavaScriptException(ErrorType.TypeError, $"'get' on proxy: property '{TypeConverter.ToString(key)}' is a non-configurable accessor property on the proxy target and does not have a getter function, but the trap did not return 'undefined' (got '{TypeConverter.ToString(result)}').");
            }
            return result;
        }

        /// <summary>
        /// Sets the value of the property with the given array index.  If a property with the
        /// given index does not exist, or exists in the prototype chain (and is not a setter) then
        /// a new property is created.
        /// </summary>
        /// <param name="index"> The array index of the property to set. </param>
        /// <param name="value"> The value to set the property to.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="thisValue"> The value of the "this" keyword inside a setter. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is read-only or if the object is sealed. </param>
        /// <returns> <c>false</c> if an error occurred. </returns>
        public override bool SetPropertyValue(uint index, object value, object thisValue, bool throwOnError)
        {
            return SetPropertyValue(index.ToString(), value, thisValue, throwOnError);
        }

        /// <summary>
        /// Sets the value of the property with the given name.  If a property with the given name
        /// does not exist, or exists in the prototype chain (and is not a setter) then a new
        /// property is created.
        /// </summary>
        /// <param name="propertyReference"> The name of the property to set. </param>
        /// <param name="value"> The value to set the property to.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="thisValue"> The value of the "this" keyword inside a setter. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set (i.e. if the property is read-only or if the object is not extensible and a new
        /// property needs to be created). </param>
        /// <returns> <c>false</c> if <paramref name="throwOnError"/> is false and an error
        /// occurred; <c>true</c> otherwise. </returns>
        public override bool SetPropertyValue(PropertyReference propertyReference, object value, object thisValue, bool throwOnError)
        {
            return SetPropertyValue(propertyReference.Name, value, thisValue, throwOnError);
        }

        /// <summary>
        /// Sets the value of the property with the given name.  If a property with the given name
        /// does not exist, or exists in the prototype chain (and is not a setter) then a new
        /// property is created.
        /// </summary>
        /// <param name="key"> The property key of the property to set. </param>
        /// <param name="value"> The value to set the property to.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="thisValue"> The value of the "this" keyword inside a setter. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set (i.e. if the property is read-only or if the object is not extensible and a new
        /// property needs to be created). </param>
        /// <returns> <c>false</c> if <paramref name="throwOnError"/> is false and an error
        /// occurred; <c>true</c> otherwise. </returns>
        public override bool SetPropertyValue(object key, object value, object thisValue, bool throwOnError)
        {
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'set' on a proxy that has been revoked.");

            // Call the handler, if one exists.
            var trap = handler.GetMethod("set");
            if (trap == null)
                return target.SetPropertyValue(key, value, thisValue, throwOnError);
            var result = TypeConverter.ToBoolean(trap.CallLateBound(handler, target, key, value, thisValue));
            if (!result)
                return false;

            // Validate.
            var targetDescriptor = target.GetOwnPropertyDescriptor(key);
            if (targetDescriptor.Exists && !targetDescriptor.IsConfigurable)
            {
                if (!targetDescriptor.IsAccessor && !targetDescriptor.IsWritable && !TypeComparer.SameValue(value, targetDescriptor.Value))
                    throw new JavaScriptException(ErrorType.TypeError, $"'set' on proxy: trap returned truish for property '{TypeConverter.ToString(key)}' which exists in the proxy target as a non-configurable and non-writable data property with a different value.");
                if (targetDescriptor.IsAccessor && targetDescriptor.Setter == null)
                    throw new JavaScriptException(ErrorType.TypeError, $"'set' on proxy: trap returned truish for property '{TypeConverter.ToString(key)}' which exists in the proxy target as a non-configurable and non-writable accessor property without a setter.");
            }
            return true;
        }

        /// <summary>
        /// Deletes the property with the given name.
        /// </summary>
        /// <param name="key"> The property key of the property to delete. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set because the property was marked as non-configurable.  </param>
        /// <returns> <c>true</c> if the property was successfully deleted, or if the property did
        /// not exist; <c>false</c> if the property was marked as non-configurable and
        /// <paramref name="throwOnError"/> was <c>false</c>. </returns>
        public override bool Delete(object key, bool throwOnError)
        {
            // Check for revocation.
            if (target == null || handler == null)
                throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'deleteProperty' on a proxy that has been revoked.");

            // Call the handler, if one exists.
            var trap = handler.GetMethod("deleteProperty");
            if (trap == null)
                return target.Delete(key, throwOnError);
            var result = TypeConverter.ToBoolean(trap.CallLateBound(handler, target, key));
            if (!result)
                return false;

            // Validate.
            var targetDescriptor = target.GetOwnPropertyDescriptor(key);
            if (targetDescriptor.Exists)
            {
                if (!targetDescriptor.IsConfigurable)
                    throw new JavaScriptException(ErrorType.TypeError, $"'deleteProperty' on proxy: trap returned truish for property '{TypeConverter.ToString(key)}' which is non-configurable in the proxy target.");
                if (!target.IsExtensible)
                    throw new JavaScriptException(ErrorType.TypeError, $"'deleteProperty' on proxy: trap returned truish for property '{TypeConverter.ToString(key)}' but the proxy target is non-extensible.");
            }
            return true;
        }

        /// <summary>
        /// Gets an enumerable list of every property name associated with this object.
        /// Does not include properties in the prototype chain.
        /// </summary>
        public override IEnumerable<object> OwnKeys
        {
            get
            {
                // Check for revocation.
                if (target == null || handler == null)
                    throw new JavaScriptException(ErrorType.TypeError, "Cannot call 'ownKeys' on a proxy that has been revoked.");


                // Call the handler, if one exists.
                var trap = handler.GetMethod("ownKeys");
                if (trap == null)
                    return target.OwnKeys;
                var result = trap.CallLateBound(handler, target);

                // Validate.
                if (!(result is ObjectInstance))
                    throw new JavaScriptException(ErrorType.TypeError, $"'ownKeys' on proxy: trap returned non-object ('{TypeConverter.ToString(result)}').");
                var trapResult = TypeUtilities.CreateListFromArrayLike((ObjectInstance)result);

                // Check for duplicates.
                for (int i = 0; i < trapResult.Length; i++)
                {
                    object key = trapResult[i];
                    if (key is string keyStr)
                    {
                        for (int j = 0; j < i; j++)
                            if (trapResult[j] is string keyStr2 && keyStr == keyStr2)
                                throw new JavaScriptException(ErrorType.TypeError, $"'ownKeys' on proxy: trap returned duplicate entries.");
                    }
                    else if (key is Symbol)
                    {
                        for (int j = 0; j < i; j++)
                            if (key == trapResult[j])
                                throw new JavaScriptException(ErrorType.TypeError, $"'ownKeys' on proxy: trap returned duplicate entries.");
                    }
                    else
                        throw new JavaScriptException(ErrorType.TypeError, $"{TypeConverter.ToString(key)} is not a valid property name.");
                }

                // Check that required keys are present.
                var targetNonconfigurableKeys = new List<object>();
                var targetConfigurableKeys = new List<object>();
                foreach (var key in target.OwnKeys)
                {
                    var targetDescriptor = target.GetOwnPropertyDescriptor(key);
                    if (targetDescriptor.Exists && !targetDescriptor.IsConfigurable)
                        targetNonconfigurableKeys.Add(key);
                    else
                        targetConfigurableKeys.Add(key);
                }
                if (target.IsExtensible && targetNonconfigurableKeys.Count == 0)
                    return trapResult;
                var uncheckedResultKeys = new List<object>(trapResult);
                foreach (var key in targetNonconfigurableKeys)
                    if (!uncheckedResultKeys.Remove(key))
                        throw new JavaScriptException(ErrorType.TypeError, $"'ownKeys' on proxy: trap result did not include '{TypeConverter.ToString(key)}'.");
                if (target.IsExtensible)
                    return trapResult;
                foreach (var key in targetConfigurableKeys)
                    if (!uncheckedResultKeys.Remove(key))
                        throw new JavaScriptException(ErrorType.TypeError, $"'ownKeys' on proxy: trap result did not include '{TypeConverter.ToString(key)}'.");
                if (uncheckedResultKeys.Count > 0)
                    throw new JavaScriptException(ErrorType.TypeError, $"'ownKeys' on proxy: trap returned extra keys but proxy target is non-extensible.");

                return trapResult;
            }
        }
    }
}
