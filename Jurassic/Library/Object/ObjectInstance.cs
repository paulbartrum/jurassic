using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jurassic.Library
{
    /// <summary>
    /// Provides functionality common to all JavaScript objects.
    /// </summary>
    public class ObjectInstance
    {
        // Internal prototype chain.
        private ObjectInstance prototype;

        // Stores the property names and attributes for this object.
        private HiddenClassSchema schema = HiddenClassSchema.Empty;

        // Stores the property values for this object.
        private object[] propertyValues = new object[4];

        [Flags]
        private enum ObjectFlags
        {
            /// <summary>
            /// Indicates whether properties can be added to this object.
            /// </summary>
            Extensible = 1,

            /// <summary>
            /// Indicates that all properties are FullAccess and the object is extensible.
            /// </summary>
            FullyAccessible = 2,

            /// <summary>
            /// Indicates this object has an accessor property.
            /// </summary>
            HasAccessorProperty = 4,
        }

        // Stores flags related to this object.
        private ObjectFlags flags = ObjectFlags.Extensible | ObjectFlags.FullyAccessible;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates an Object with no prototype.
        /// </summary>
        private ObjectInstance()
        {
        }

        /// <summary>
        /// Called by derived classes to create a new object instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        protected ObjectInstance(ObjectInstance prototype)
        {
            if (prototype == null)
                throw new ArgumentNullException("prototype");
            this.prototype = prototype;
        }

        /// <summary>
        /// Creates an Object with no prototype to serve as the base prototype of all objects.
        /// </summary>
        /// <returns> An Object with no prototype. </returns>
        internal static ObjectInstance CreateRootObject()
        {
            return new ObjectInstance();
        }

        /// <summary>
        /// Creates an Object instance (use ObjectConstructor.Construct rather than this).
        /// </summary>
        /// <returns> An Object instance. </returns>
        internal static ObjectInstance CreateRawObject(ObjectInstance prototype)
        {
            return new ObjectInstance(prototype);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected virtual string InternalClassName
        {
            get { return "Object"; }
        }

        /// <summary>
        /// Gets the next object in the prototype chain.
        /// </summary>
        public ObjectInstance Prototype
        {
            get { return this.prototype; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the object can have new properties added
        /// to it.
        /// </summary>
        internal bool IsExtensible
        {
            get { return (this.flags & ObjectFlags.Extensible) != 0; }
            set
            {
                if (value == true)
                    throw new InvalidOperationException("Once an object has been made non-extensible it cannot be made extensible again.");
                this.flags &= ~ObjectFlags.Extensible;
                this.IsFullyAccessible = false;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the object has properties that are
        /// read-only, non-configurable or non-enumerable.
        /// </summary>
        private bool IsFullyAccessible
        {
            get { return (this.flags & ObjectFlags.FullyAccessible) != 0; }
            set
            {
                if (value == true)
                    this.flags |= ObjectFlags.FullyAccessible;
                else
                    this.flags &= ~ObjectFlags.FullyAccessible;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this object has any accessor properties.
        /// </summary>
        private bool HasAccessorProperty
        {
            get { return (this.flags & ObjectFlags.HasAccessorProperty) != 0; }
            set
            {
                if (value == true)
                    this.flags |= ObjectFlags.HasAccessorProperty;
                else
                    this.flags &= ~ObjectFlags.HasAccessorProperty;
            }
        }

        /// <summary>
        /// Gets or sets the value of a named property.
        /// </summary>
        /// <param name="propertyName"> The name of the property to get or set. </param>
        /// <returns> The property value, or <c>null</c> if the property doesn't exist. </returns>
        public object this[string propertyName]
        {
            get { return Get(propertyName); }
            set { Put(propertyName, value, false); }
        }

        /// <summary>
        /// Gets or sets the value of an array-indexed property.
        /// </summary>
        /// <param name="index"> The index of the property to retrieve. </param>
        /// <returns> The property value, or <c>null</c> if the property doesn't exist. </returns>
        public object this[uint index]
        {
            get { return Get(index); }
            set { Put(index, value, false); }
        }

        /// <summary>
        /// Gets an enumerable list of every property name and value associated with this object.
        /// </summary>
        public virtual IEnumerable<PropertyNameAndValue> Properties
        {
            get
            {
                // Enumerate named properties.
                return this.schema.EnumeratePropertyNamesAndValues(this.PropertyValues);
            }
        }



        //     INLINE CACHING
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets an object to use as a cache key.  Adding, deleting, or changing attributes will cause
        /// the value of this property to change.
        /// </summary>
        public object CacheKey
        {
            get { return this.schema; }
        }

        /// <summary>
        /// Gets the values stored against this object - one for each property.  The first element
        /// is always "undefined".
        /// </summary>
        public object[] PropertyValues
        {
            get { return this.propertyValues; }
        }

        /// <summary>
        /// Gets the value of the given property plus the information needed to speed up access to
        /// the property in future.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <param name="index"> Set to a zero-based index that can be used to retrieve the
        /// property value in future (provided the version number is equal to <paramref name="version"/>). </param>
        /// <param name="version"> Set to the version of the class.  Can be <c>-1</c> to prohibit
        /// caching. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        public object InlineGetPropertyValue(string name, out int index, out object cacheKey)
        {
            index = this.schema.GetPropertyIndex(name);
            if (index < 0)
            {
                // The property is in the prototype or is non-existant.
                cacheKey = null;
                if (this.Prototype == null)
                    return null;
                return this.Prototype.Get(name);
            }
            cacheKey = this.CacheKey;
            return this.PropertyValues[index];
        }

        /// <summary>
        /// Sets the value of the given property plus retrieves the information needed to speed up
        /// access to the property in future.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <param name="value"> The value of the property. </param>
        /// <param name="index"> Set to a zero-based index that can be used to retrieve the
        /// property value in future (provided the version number is equal to <paramref name="version"/>). </param>
        /// <param name="version"> Set to the version of the class.  Can be <c>-1</c> to prohibit
        /// caching. </param>
        public void InlineSetPropertyValue(string name, object value, out int index, out object cacheKey)
        {
            index = this.schema.GetPropertyIndex(name);
            if (index < 0)
            {
                // The property is in the prototype or doesn't exist - add a new property.
                this.schema = this.schema.AddProperty(name);
                index = this.schema.GetPropertyIndex(name);
                if (index >= this.PropertyValues.Length)
                {
                    // Resize the value array.
                    Array.Resize(ref this.propertyValues, this.PropertyValues.Length * 2);
                }
            }
            cacheKey = this.CacheKey;
            this.PropertyValues[index] = value;
        }

        /// <summary>
        /// Sets the value of the given property plus retrieves the information needed to speed up
        /// access to the property in future.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <param name="value"> The value of the property. </param>
        /// <param name="index"> Set to a zero-based index that can be used to retrieve the
        /// property value in future (provided the version number is equal to <paramref name="version"/>). </param>
        /// <param name="version"> Set to the version of the class.  Can be <c>-1</c> to prohibit
        /// caching. </param>
        /// <returns> <c>true</c> if the property value exists; <c>false</c> otherwise. </returns>
        public bool InlineSetPropertyValueIfExists(string name, object value, out int index, out object cacheKey)
        {
            index = this.schema.GetPropertyIndex(name);
            if (index < 0)
            {
                // The property is in the prototype or is non-existant.
                cacheKey = null;
                if (this.Prototype == null)
                    return false;
                if (this.Prototype.HasProperty(name) == true)
                {
                    this.Prototype[name] = value;
                    return true;
                }
                return false;
            }
            cacheKey = this.CacheKey;
            this.PropertyValues[index] = value;
            return true;
        }



        //     .NET METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Sets a property value and attributes, or adds a new property if it doesn't already
        /// exist.  Any existing attributes are overwritten.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <param name="value"> The intended value of the property. </param>
        /// <param name="attributes"> Attributes that indicate whether the property is writable,
        /// configurable and enumerable. </param>
        public void SetProperty(string propertyName, object value, PropertyAttributes attributes = PropertyAttributes.Sealed)
        {
            int index = this.schema.GetPropertyIndex(propertyName);
            if (index < 0)
            {
                // The property is in the prototype or doesn't exist - add a new property.
                this.schema = this.schema.AddProperty(propertyName, attributes);
                index = this.schema.GetPropertyIndex(propertyName);
                if (index >= this.PropertyValues.Length)
                {
                    // Resize the value array.
                    Array.Resize(ref this.propertyValues, this.PropertyValues.Length * 2);
                }
            }
            else
                this.schema = this.schema.SetPropertyAttributes(propertyName, attributes);
            this.PropertyValues[index] = value;
            if (attributes != PropertyAttributes.FullAccess)
                this.IsFullyAccessible = false;
        }

        /// <summary>
        /// Sets a property value, but only if the property exists.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <param name="value"> The intended value of the property. </param>
        /// <returns> <c>true</c> if the property value exists; <c>false</c> otherwise. </returns>
        public bool SetPropertyIfExists(string propertyName, object value)
        {
            int index = this.schema.GetPropertyIndex(propertyName);
            if (index < 0)
            {
                // The property is in the prototype or doesn't exist.
                if (this.Prototype == null)
                    return false;
                if (this.Prototype.HasProperty(propertyName) == false)
                    return false;
                // Add a new property.
                this.schema = this.schema.AddProperty(propertyName, PropertyAttributes.FullAccess);
                index = this.schema.GetPropertyIndex(propertyName);
                if (index >= this.PropertyValues.Length)
                {
                    // Resize the value array.
                    Array.Resize(ref this.propertyValues, this.PropertyValues.Length * 2);
                }
            }
            this.PropertyValues[index] = value;
            return true;
        }

        /// <summary>
        /// Returns a primitive value that represents the current object.  Used by the addition and
        /// equality operators.
        /// </summary>
        /// <param name="hint"> Indicates the preferred type of the result. </param>
        /// <returns> A primitive value that represents the current object. </returns>
        protected internal virtual object GetDefaultValue(PrimitiveTypeHint typeHint)
        {
            if (typeHint == PrimitiveTypeHint.None || typeHint == PrimitiveTypeHint.Number)
            {

                // Try calling valueOf().
                object valueOfResult;
                if (TryCallMemberFunction(out valueOfResult, "valueOf") == true)
                {
                    // Return value must be primitive.
                    if (valueOfResult is double || IsPrimitive(valueOfResult.GetType()) == true)
                        return valueOfResult;
                }

                // Try calling toString().
                object toStringResult;
                if (TryCallMemberFunction(out toStringResult, "toString") == true)
                {
                    // Return value must be primitive.
                    if (toStringResult is string || IsPrimitive(toStringResult.GetType()) == true)
                        return toStringResult;
                }

            }
            else
            {

                // Try calling toString().
                object toStringResult;
                if (TryCallMemberFunction(out toStringResult, "toString") == true)
                {
                    // Return value must be primitive.
                    if (toStringResult is string || IsPrimitive(toStringResult.GetType()) == true)
                        return toStringResult;
                }

                // Try calling valueOf().
                object valueOfResult;
                if (TryCallMemberFunction(out valueOfResult, "valueOf") == true)
                {
                    // Return value must be primitive.
                    if (valueOfResult is double || IsPrimitive(valueOfResult.GetType()) == true)
                        return valueOfResult;
                }

            }

            throw new JavaScriptException("TypeError", "Attempted conversion of the object to a primitive value failed.  Check the toString() and valueOf() functions.");
        }

        /// <summary>
        /// Determines if the given type is a supported JavaScript primitive type.
        /// </summary>
        /// <param name="type"> The type to test. </param>
        /// <returns> <c>true</c> if the given type is a supported JavaScript primitive type;
        /// <c>false</c> otherwise. </returns>
        private bool IsPrimitive(Type type)
        {
            return type == typeof(bool) || type == typeof(int) || type == typeof(double) ||
                type == typeof(string) || type == typeof(Null) || type == typeof(Undefined);
        }

        /// <summary>
        /// Gets the property descriptor for the property with the given name.  The prototype
        /// chain is not searched.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes.  The
        /// result will be <c>PropertyDescriptor.Undefined</c> if the property doesn't exist. </returns>
        internal virtual PropertyDescriptor GetOwnProperty(string propertyName)
        {
            PropertyAttributes attributes;
            int index = this.schema.GetPropertyIndexAndAttributes(propertyName, out attributes);
            if (index == -1)
                return PropertyDescriptor.Undefined;
            return new PropertyDescriptor(this.propertyValues[index], attributes);
        }

        /// <summary>
        /// Gets the property descriptor for the property with the given array index.  The
        /// prototype chain is not searched.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes.  The
        /// result will be <c>PropertyDescriptor.Undefined</c> if the property doesn't exist. </returns>
        internal virtual PropertyDescriptor GetOwnProperty(uint index)
        {
            return GetOwnProperty(index.ToString());
        }

        /// <summary>
        /// Gets the property descriptor for the property with the given name.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes.  The
        /// value will be <c>PropertyDescriptor.Undefined</c> if the property doesn't exist. </returns>
        internal PropertyDescriptor GetProperty(string propertyName)
        {
            ObjectInstance instance = this;
            do
            {
                // Retrieve the property spec.
                PropertyDescriptor descriptor = instance.GetOwnProperty(propertyName);

                if (descriptor.Exists == true)
                {
                    // The property was found!
                    return descriptor;
                }

                // Traverse up the prototype chain.
                instance = instance.prototype;
            } while (instance != null);

            // Property doesn't exist.
            return PropertyDescriptor.Undefined;
        }

        /// <summary>
        /// Gets the value of the property with the given name.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        internal object Get(string propertyName)
        {
            // Get the property details, or return null if the property doesn't exist.
            return GetProperty(propertyName).GetValue(this);
        }

        /// <summary>
        /// Gets the value of the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        internal object Get(uint index)
        {
            ObjectInstance instance = this;
            do
            {
                // Retrieve the property spec.
                PropertyDescriptor descriptor = instance.GetOwnProperty(index);

                if (descriptor.Exists == true)
                {
                    // The array element was found!
                    return descriptor.Value;
                }

                // Traverse up the prototype chain.
                instance = instance.prototype;
            } while (instance != null);

            // Array element doesn't exist.
            return null;
        }

        /// <summary>
        /// Determines if the property with the given name is writable.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> <c>true</c> if a property with the given name is writable; <c>false</c>
        /// otherwise. </returns>
        internal bool CanPut(string propertyName)
        {
            // Retrieve the property on this object.
            PropertyDescriptor descriptor = GetOwnProperty(propertyName);
            if (descriptor.Exists == true)
                return descriptor.IsWritable;

            // The property doesn't exist on this object.  A new property would normally be
            // created, unless an accessor function exists in the prototype chain.
            if (this.prototype != null)
            {
                descriptor = this.prototype.GetProperty(propertyName);
                if (descriptor.IsAccessor == true)
                    return descriptor.SetAccessor != null;
            }

            // A new property can be created if the object is extensible.
            return this.IsExtensible;
        }

        /// <summary>
        /// Sets the value of the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property to set. </param>
        /// <param name="value"> The value to set the property to. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is read-only or if the object is sealed. </param>
        internal virtual void Put(uint index, object value, bool throwOnError)
        {
            Put(index.ToString(), value, throwOnError);
        }

        /// <summary>
        /// Sets the value of the property with the given name.
        /// </summary>
        /// <param name="propertyName"> The name of the property to set. </param>
        /// <param name="value"> The value to set the property to. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is read-only or if the object is sealed. </param>
        internal virtual void Put(string propertyName, object value, bool throwOnError)
        {
            PropertyDescriptor property;

            // Do not store nulls - null represents a non-existant value.
            value = value ?? Undefined.Value;

            // Try to prove that we don't need to read the properties dictionary.
            if ((this.flags & (ObjectFlags.FullyAccessible | ObjectFlags.HasAccessorProperty)) == ObjectFlags.FullyAccessible)
            {
                // This is the "fast path".

                // We still need to check the prototype chain for accessor properties.
                ObjectInstance obj = this.prototype;
                while (obj != null)
                {
                    if (obj.HasAccessorProperty == true)
                    {
                        property = obj.GetOwnProperty(propertyName);
                        if (property.IsAccessor == true)
                        {
                            // The property contains an accessor function.  Set the property value by calling the accessor.
                            property.SetAccessor.CallLateBound(this, value);
                            return;
                        }
                        break;
                    }
                    obj = obj.prototype;
                }

                // Add or modify the property.
                this.SetProperty(propertyName, value, PropertyAttributes.FullAccess);
                return;
            }

            // Retrieve the property on this object.
            property = GetOwnProperty(propertyName);
            if (property.Exists == true)
            {
                // Check if the property is read-only.
                if (property.IsWritable == false)
                {
                    // The property is read-only.
                    if (throwOnError == true)
                        throw new JavaScriptException("TypeError", string.Format("The property '{0}' is read-only.", propertyName));
                    return;
                }

                if (property.IsAccessor == false)
                {
                    // The property contains a simple value.  Set the property value.
                    this.SetProperty(propertyName, value, property.Attributes);
                }
                else
                {
                    // The property contains an accessor function.  Set the property value by calling the accessor.
                    property.SetAccessor.CallLateBound(this, value);
                }
                return;
            }

            // Search the prototype chain for a accessor function.
            if (this.prototype != null)
            {
                property = this.prototype.GetProperty(propertyName);
                if (property.Exists == true && property.IsAccessor == true)
                {
                    if (property.IsWritable == false)
                    {
                        // The property is read-only.
                        if (throwOnError == true)
                            throw new JavaScriptException("TypeError", string.Format("The property '{0}' is read-only.", propertyName));
                        return;
                    }

                    // Set the property value using the accessor function.
                    property.SetAccessor.CallLateBound(this, value);
                    return;
                }
            }

            // Otherwise, a new property must be created.
            // Make sure this is allowed.
            if (this.IsExtensible == false)
            {
                if (throwOnError == true)
                    throw new JavaScriptException("TypeError", string.Format("The property '{0}' cannot be created as the object is sealed.", propertyName));
                return;
            }

            // Create a new property.
            this.SetProperty(propertyName, value, PropertyAttributes.FullAccess);
        }

        /// <summary>
        /// Determines if a property with the given name exists.
        /// </summary>
        /// <param name="propertyName"> The name of the property to check. </param>
        /// <returns> <c>true</c> if the property exists in the prototype chain; <c>false</c>
        /// otherwise. </returns>
        internal bool HasProperty(string propertyName)
        {
            return this.GetProperty(propertyName).Exists;
        }
        
        /// <summary>
        /// Deletes the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property to delete. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is not configurable.  </param>
        /// <returns> <c>true</c> if the property was successfully deleted; <c>false</c> otherwise. </returns>
        internal virtual bool Delete(uint index, bool throwOnError)
        {
            return Delete(index.ToString(), throwOnError);
        }

        /// <summary>
        /// Deletes the property with the given name.
        /// </summary>
        /// <param name="propertyName"> The name of the property to delete. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is not configurable. </param>
        /// <returns> <c>true</c> if the property was successfully deleted; <c>false</c> otherwise. </returns>
        internal virtual bool Delete(string propertyName, bool throwOnError)
        {
            // Retrieve the property on this object.
            PropertyDescriptor property = this.GetOwnProperty(propertyName);
            if (property.Exists == false)
                return true;    // Property doesn't exist - delete succeeded!

            // Check if the property can be deleted.
            if (property.IsConfigurable == false)
            {
                if (throwOnError == true)
                    throw new JavaScriptException("TypeError", string.Format("The property '{0}' cannot be deleted.", propertyName));
                return false;
            }

            // Delete the property.
            this.schema = this.schema.DeleteProperty(propertyName);
            return true;
        }

        /// <summary>
        /// Defines or redefines the value and attributes of a property.
        /// </summary>
        /// <param name="propertyName"> The name of the property to modify. </param>
        /// <param name="descriptor"> The property value and attributes. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is not configurable or the object is sealed. </param>
        /// <returns> <c>true</c> if the property was successfully modified; <c>false</c> otherwise. </returns>
        internal virtual bool DefineOwnProperty(string propertyName, PropertyDescriptor descriptor, bool throwOnError)
        {
            // Retrieve the property on this object.
            PropertyDescriptor current = GetOwnProperty(propertyName);

            if (current.Exists == false)
            {
                // Check if the object is sealed.
                if (this.IsExtensible == false)
                {
                    // A new property cannot be created.
                    if (throwOnError == true)
                        throw new JavaScriptException("TypeError", string.Format("The property '{0}' cannot be created as the object is sealed.", propertyName));
                    return false;
                }

                // Create a new property.
                this.SetProperty(propertyName, descriptor.Value, descriptor.Attributes);

                // If the property is not full access, set a flag to that effect.
                if (descriptor.Attributes != PropertyAttributes.FullAccess)
                    this.IsFullyAccessible = false;

                // If the property is an accessor property, set a flag to that effect.
                if (descriptor.IsAccessor == true)
                    this.HasAccessorProperty = true;

                return true;
            }

            // If the current property is not configurable, then the only change that is allowed is
            // a change from one simple value to another (i.e. accessors are not allowed) and only
            // if the writable attribute is set.
            if (current.IsConfigurable == false)
            {
                if (descriptor.Attributes != current.Attributes ||
                    (descriptor.IsAccessor == true && (object.Equals(current.GetAccessor, descriptor.GetAccessor) == false || object.Equals(current.SetAccessor, descriptor.SetAccessor) == false)) ||
                    (descriptor.IsAccessor == false && current.IsWritable == false && object.Equals(current.Value, descriptor.Value) == false))
                {
                    if (throwOnError == true)
                        throw new JavaScriptException("TypeError", string.Format("The property '{0}' is non-configurable.", propertyName));
                    return false;
                }
            }

            // Set the property value and attributes.
            this.SetProperty(propertyName, descriptor.Value, descriptor.Attributes);

            // If the property is not full access, set a flag to that effect.
            if (descriptor.Attributes != PropertyAttributes.FullAccess)
                this.IsFullyAccessible = false;

            // If the property is an accessor property, set a flag to that effect.
            if (descriptor.IsAccessor == true)
                this.HasAccessorProperty = true;

            return true;
        }

        /// <summary>
        /// Calls the function with the given name.  The function must exist on this object or an
        /// exception will be thrown.
        /// </summary>
        /// <param name="functionName"> The name of the function to call. </param>
        /// <param name="parameters"> The parameters to pass to the function. </param>
        /// <returns> The result of calling the function. </returns>
        public object CallMemberFunction(string functionName, params object[] parameters)
        {
            var function = Get(functionName);
            if (function == null || function == Undefined.Value)
                throw new JavaScriptException("TypeError", string.Format("Object {0} has no method '{1}'", this.ToString(), functionName));
            if ((function is FunctionInstance) == false)
                throw new JavaScriptException("TypeError", string.Format("Property '{1}' of object {0} is not a function", this.ToString(), functionName));
            return ((FunctionInstance)function).CallLateBound(this, parameters);
        }

        /// <summary>
        /// Calls the function with the given name.
        /// </summary>
        /// <param name="result"> The result of calling the function. </param>
        /// <param name="functionName"> The name of the function to call. </param>
        /// <param name="parameters"> The parameters to pass to the function. </param>
        /// <returns> <c>true</c> if the function was called successfully; <c>false</c> otherwise. </returns>
        public bool TryCallMemberFunction(out object result, string functionName, params object[] parameters)
        {
            var function = Get(functionName);
            if ((function is FunctionInstance) == false)
            {
                result = null;
                return false;
            }
            result = ((FunctionInstance)function).CallLateBound(this, parameters);
            return true;
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            try
            {
                // Does a toString method/property exist?
                if (HasProperty("toString") == true)
                {
                    // Call the toString() method.
                    var result = CallMemberFunction("toString");

                    // Make sure we don't recursively loop forever.
                    if (result == this)
                        return "<error>";

                    // Convert to a string.
                    return TypeConverter.ToString(result);
                }
                else
                {
                    // Otherwise, return the type name.
                    var constructor = this["constructor"];
                    if (constructor is FunctionInstance)
                        return string.Format("[{0}]", ((FunctionInstance) constructor).Name);
                    return "<unknown>";
                }
            }
            catch (JavaScriptException)
            {
                return "<error>";
            }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Determines if a property with the given name exists on this object.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> <c>true</c> if a property with the given name exists on this object,
        /// <c>false</c> otherwise. </returns>
        /// <remarks> Objects in the prototype chain are not considered. </remarks>
        [JSFunction(Name = "hasOwnProperty", Flags = FunctionBinderFlags.HasThisObject)]
        public static bool HasOwnProperty(object thisObject, string propertyName)
        {
            return TypeConverter.ToObject(thisObject).GetOwnProperty(propertyName).Exists;
        }

        /// <summary>
        /// Determines if this object is in the prototype chain of the given object.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> <c>true</c> if this object is in the prototype chain of the given object;
        /// <c>false</c> otherwise. </returns>
        [JSFunction(Name = "isPrototypeOf", Flags = FunctionBinderFlags.HasThisObject)]
        public static bool IsPrototypeOf(object thisObject, object obj)
        {
            if ((obj is ObjectInstance) == false)
                return false;
            TypeConverter.CheckCoercibleToObject(thisObject);
            var obj2 = obj as ObjectInstance;
            while (true)
            {
                obj2 = obj2.Prototype;
                if (obj2 == null)
                    return false;
                if (obj2 == thisObject)
                    return true;
            }
        }

        /// <summary>
        /// Determines if a property with the given name exists on this object and is enumerable.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> <c>true</c> if a property with the given name exists on this object and is
        /// enumerable, <c>false</c> otherwise. </returns>
        /// <remarks> Objects in the prototype chain are not considered. </remarks>
        [JSFunction(Name = "propertyIsEnumerable", Flags = FunctionBinderFlags.HasThisObject)]
        public static bool PropertyIsEnumerable(object thisObject, string propertyName)
        {
            PropertyDescriptor descriptor = TypeConverter.ToObject(thisObject).GetOwnProperty(propertyName);
            return descriptor.Exists && descriptor.IsEnumerable;
        }

        /// <summary>
        /// Returns a locale-dependant string representing the current object.
        /// </summary>
        /// <returns> Returns a locale-dependant string representing the current object. </returns>
        [JSFunction(Name = "toLocaleString")]
        public string ToLocaleString()
        {
            return TypeConverter.ToString(CallMemberFunction("toString"));
        }

        /// <summary>
        /// Returns a primitive value associated with the object.
        /// </summary>
        /// <returns> A primitive value associated with the object. </returns>
        [JSFunction(Name = "valueOf")]
        public ObjectInstance ValueOf()
        {
            return this;
        }

        /// <summary>
        /// Returns a string representing the current object.
        /// </summary>
        /// <returns> A string representing the current object. </returns>
        [JSFunction(Name = "toString")]
        public string ToStringJS()
        {
            return string.Format("[object {0}]", this.InternalClassName);
        }



        ////     IEnvironmentRecord IMPLEMENTATION
        ////_________________________________________________________________________________________

        ///// <summary>
        ///// Determines if an environment record has a binding for an identifier.
        ///// </summary>
        ///// <param name="name"> The name of the identifier. </param>
        ///// <returns> <c>true</c> the binding exists; <c>false</c> if it does not. </returns>
        //bool Compiler.IEnvironmentRecord.HasBinding(string name)
        //{
        //    return this.HasProperty(name);
        //}

        ///// <summary>
        ///// Creates a new mutable binding in the environment record.
        ///// </summary>
        ///// <param name="name"> The name of the identifier. </param>
        ///// <param name="mayBeDeleted"> <c>true</c> if the binding can be deleted; <c>false</c>
        ///// otherwise. </param>
        //void Compiler.IEnvironmentRecord.CreateMutableBinding(string name, bool mayBeDeleted)
        //{
        //    throw new NotSupportedException();
        //}

        ///// <summary>
        ///// Sets the value of an already existing mutable binding in the environment record.
        ///// </summary>
        ///// <param name="name"> The name of the identifier. </param>
        ///// <param name="value"> The new value of the binding. </param>
        ///// <param name="strict"> Indicates whether to use strict mode semantics. </param>
        ///// <returns> The new value of the binding. </returns>
        //T Compiler.IEnvironmentRecord.SetMutableBinding<T>(string name, T value, bool strict)
        //{
        //    this.Put(name, value, strict);
        //    return value;
        //}

        ///// <summary>
        ///// Returns the value of an already existing binding from the environment record.
        ///// </summary>
        ///// <param name="name"> The name of the identifier. </param>
        ///// <param name="strict"> Indicates whether to use strict mode semantics. </param>
        ///// <returns> The value of the binding. </returns>
        //object Compiler.IEnvironmentRecord.GetBindingValue(string name, bool strict)
        //{
        //    object result = this.Get(name);
        //    if (result == null && strict == true)
        //        throw new JavaScriptException("ReferenceError", name + " is not defined");
        //    return result;
        //}

        ///// <summary>
        ///// Deletes a binding from the environment record.
        ///// </summary>
        ///// <param name="name"> The name of the identifier. </param>
        ///// <returns> <c>true</c> if the binding exists and could be deleted, or if the binding
        ///// doesn't exist; <c>false</c> if the binding couldn't be deleted. </returns>
        //bool Compiler.IEnvironmentRecord.DeleteBinding(string name)
        //{
        //    return this.Delete(name, false);
        //}


        //     ATTRIBUTE-BASED PROTOTYPE POPULATION
        //_________________________________________________________________________________________

        private class MethodGroup
        {
            public List<FunctionBinderMethod> Methods;
            public int Length;
        }

        /// <summary>
        /// Populates the object with functions and properties.  Should be called only once at
        /// startup.
        /// </summary>
        internal protected void PopulateFunctions(Type type = null)
        {
            if (type == null)
                type = this.GetType();
            
            // Group the methods on the given type by name.
            var functions = new Dictionary<string, MethodGroup>(20);
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                // Make sure the method has the [JSFunction] attribute.
                var attribute = (JSFunctionAttribute)Attribute.GetCustomAttribute(method, typeof(JSFunctionAttribute));
                if (attribute == null)
                    continue;

                // Determine the name of the method.
                string name;
                if (attribute.Name != null)
                {
                    name = Compiler.Lexer.ResolveIdentifier(attribute.Name);
                    if (name == null)
                        throw new InvalidOperationException(string.Format("The name provided to [JSFunction] on {0} is not a valid identifier.", method));
                }
                else
                    name = method.Name;

                // Get a reference to the method group.
                MethodGroup methodGroup;
                if (functions.ContainsKey(name) == false)
                {
                    methodGroup = new MethodGroup { Methods = new List<FunctionBinderMethod>(1), Length = -1 };
                    functions.Add(name, methodGroup);
                }
                else
                    methodGroup = functions[name];

                // Add the method to the list.
                methodGroup.Methods.Add(new FunctionBinderMethod(method, attribute.Flags));

                // If the length doesn't equal -1, that indicates an explicit length has been set.
                // Make sure it is consistant with the other methods.
                if (attribute.Length >= 0)
                {
                    if (methodGroup.Length != -1 && methodGroup.Length != attribute.Length)
                        throw new InvalidOperationException(string.Format("Inconsistant Length property detected on {0}.", method));
                    methodGroup.Length = attribute.Length;
                }
            }

            // Now set the relevant properties on the object.
            foreach (KeyValuePair<string, MethodGroup> pair in functions)
            {
                string name = pair.Key;
                MethodGroup methodGroup = pair.Value;

                // Add the function as a property of the object.
                this.SetProperty(name, new ClrFunction(GlobalObject.Function.InstancePrototype, methodGroup.Methods, name, methodGroup.Length));
            }
        }

        /// <summary>
        /// Populates the object with functions and properties.  Should be called only once at
        /// startup.
        /// </summary>
        internal protected void PopulateFields(Type type = null)
        {
            if (type == null)
                type = this.GetType();

            // Find all fields with [JsField]
            foreach (var field in type.GetFields())
            {
                var attribute = (JSFieldAttribute)Attribute.GetCustomAttribute(field, typeof(JSFieldAttribute));
                if (attribute == null)
                    continue;
                this.SetProperty(field.Name, field.GetValue(this));
            }
        }
    }
}
