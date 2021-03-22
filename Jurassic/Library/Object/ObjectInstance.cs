using System;
using System.Collections.Generic;
using System.Reflection;
using Jurassic.Compiler;
using System.Linq;
using System.Diagnostics;

namespace Jurassic.Library
{
    /// <summary>
    /// Provides functionality common to all JavaScript objects.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public partial class ObjectInstance : IDebuggerDisplay
    {
        // The script engine associated with this object.
        [NonSerialized]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ScriptEngine engine;

        // Internal prototype chain.
        private ObjectInstance prototype;

        // Stores the property names and attributes for this object.
        private HiddenClassSchema schema;

        // Stores the property values for this object.
        private object[] propertyValues = new object[4];

        [Flags]
        private enum ObjectFlags
        {
            /// <summary>
            /// Indicates whether properties can be added to this object.
            /// </summary>
            Extensible = 1,
        }

        // Stores flags related to this object.
        private ObjectFlags flags = ObjectFlags.Extensible;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates an Object with the default prototype.
        /// </summary>
        /// <param name="engine"> The script engine associated with this object. </param>
        protected ObjectInstance(ScriptEngine engine)
            : this(engine, engine.Object.InstancePrototype)
        {
        }

        /// <summary>
        /// Called by derived classes to create a new object instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. Cannot be <c>null</c>. </param>
        protected ObjectInstance(ObjectInstance prototype)
        {
            if (prototype == null)
                throw new ArgumentNullException(nameof(prototype));
            this.prototype = prototype;
            this.engine = prototype.Engine;
            this.schema = this.engine.EmptySchema;
        }

        /// <summary>
        /// Called by derived classes to create a new object instance.
        /// </summary>
        /// <param name="engine"> The script engine associated with this object. </param>
        /// <param name="prototype"> The next object in the prototype chain. Can be <c>null</c>. </param>
        protected ObjectInstance(ScriptEngine engine, ObjectInstance prototype)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));
            this.engine = engine;
            this.prototype = prototype;
            this.schema = engine.EmptySchema;
        }

        /// <summary>
        /// Creates an Object with no prototype to serve as the base prototype of all objects.
        /// </summary>
        /// <param name="engine"> The script engine associated with this object. </param>
        /// <returns> An Object with no prototype. </returns>
        internal static ObjectInstance CreateRootObject(ScriptEngine engine)
        {
            return new ObjectInstance(engine, null);
        }

        /// <summary>
        /// Creates an Object instance (use ObjectConstructor.Construct rather than this).
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <returns> An Object instance. </returns>
        internal static ObjectInstance CreateRawObject(ObjectInstance prototype)
        {
            return new ObjectInstance(prototype);
        }

        /// <summary>
        /// Initializes the prototype properties.
        /// </summary>
        /// <param name="obj"> The object to set the properties on. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static void InitializePrototypeProperties(ObjectInstance obj, ObjectConstructor constructor)
        {
            var engine = obj.Engine;
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            obj.InitializeProperties(properties);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets a reference to the script engine associated with this object.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ScriptEngine Engine
        {
            get { return this.engine; }
        }

        /// <summary>
        /// Gets the next object in the prototype chain.  There is no corresponding property in
        /// javascript (it is is *not* the same as the prototype property), instead use
        /// Object.getPrototypeOf(). Returns <c>null</c> for the root object in the prototype
        /// chain. Use <see cref="SetPrototype(ObjectInstance, bool)"/> to set this value.
        /// </summary>
        public virtual ObjectInstance Prototype
        {
            get { return this.prototype; }
        }

        /// <summary>
        /// Gets a value that indicates whether the object can have new properties added to it.
        /// Called by Object.isExtensible(). Use <see cref="PreventExtensions"/> to set this value.
        /// </summary>
        internal virtual bool IsExtensible
        {
            get { return (this.flags & ObjectFlags.Extensible) != 0; }
        }

        /// <summary>
        /// Gets or sets the value of a named property.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> The property value, or <see cref="Undefined.Value"/> if the property doesn't
        /// exist. </returns>
        public object this[object key]
        {
            get { return GetPropertyValue(key) ?? Undefined.Value; }
            set { SetPropertyValue(key, value, throwOnError: false); }
        }

        /// <summary>
        /// Gets or sets the value of an array-indexed property.
        /// </summary>
        /// <param name="index"> The index of the property to retrieve. </param>
        /// <returns> The property value, or <see cref="Undefined.Value"/> if the property doesn't
        /// exist. </returns>
        public object this[uint index]
        {
            get { return GetPropertyValue(index) ?? Undefined.Value; }
            set { SetPropertyValue(index, value, throwOnError: false); }
        }

        /// <summary>
        /// Gets or sets the value of an array-indexed property.
        /// </summary>
        /// <param name="index"> The index of the property to retrieve. </param>
        /// <returns> The property value, or <see cref="Undefined.Value"/> if the property doesn't
        /// exist. </returns>
        public object this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index");
                return GetPropertyValue((uint)index) ?? Undefined.Value;
            }
            set
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index");
                SetPropertyValue((uint)index, value, throwOnError: false);
            }
        }

        /// <summary>
        /// Gets an enumerable list of every property name and value associated with this object.
        /// Does not include properties in the prototype chain.
        /// </summary>
        public virtual IEnumerable<PropertyNameAndValue> Properties
        {
            get
            {
                // Enumerate named properties.
                return this.schema.EnumeratePropertyNamesAndValues(this.propertyValues);
            }
        }
        
        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public virtual string DebuggerDisplayValue
        {
            get
            {
                IEnumerable<string> strValues =
                    this.Properties.Select(pnv => 
                        string.Format("{0}: {1}", pnv.Key, DebuggerDisplayHelper.ShortStringRepresentation(pnv.Value)));

                return string.Format("{{{0}}}", string.Join(", ", strValues));
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public virtual string DebuggerDisplayShortValue
        {
            get { return "{\u2026}"; }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public virtual string DebuggerDisplayType
        {
            get { return "Object"; }
        }



        //     PROPERTY MANAGEMENT
        //_________________________________________________________________________________________

        /// <summary>
        /// Sets the next object in the prototype chain. Can be <c>null</c>, which indicates there
        /// are no further objects in the chain.
        /// </summary>
        /// <param name="prototype"> The new prototype. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the prototype could not
        /// be set.  This can happen if the object is non-extensible or if setting the prototype
        /// would introduce a cyclic dependency. </param>
        /// <returns> <c>true</c> if the prototype was successfully applied; <c>false</c> otherwise. </returns>
        internal virtual bool SetPrototype(ObjectInstance prototype, bool throwOnError)
        {
            // If the new prototype is the same as the existing one, return success.
            if (this.prototype == prototype)
                return true;

            // Can only set the prototype on extensible objects.
            if (!IsExtensible)
            {
                if (throwOnError)
                    throw new JavaScriptException(ErrorType.TypeError, "Object is not extensible.");
                return false;
            }

            // Check there are no circular references in the prototype chain.
            var ancestor = prototype;
            while (ancestor != null)
            {
                if (ancestor == this)
                {
                    if (throwOnError)
                        throw new JavaScriptException(ErrorType.TypeError, "Prototype chain contains a cyclic reference.");
                    return false;
                }
                ancestor = ancestor.Prototype;
            }

            // Set the new prototype.
            this.prototype = prototype;
            return true;
        }

        /// <summary>
        /// Makes this object non-extensible, which means no new properties can be added to it.
        /// </summary>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the object could not
        /// be made non-extensible. </param>
        /// <returns> <c>true</c> if the operation was successful, <c>false</c> otherwise. The
        /// default implementation always returns <c>true</c>. </returns>
        internal virtual bool PreventExtensions(bool throwOnError)
        {
            this.flags &= ~ObjectFlags.Extensible;
            return true;
        }

        /// <summary>
        /// Determines if a property with the given name exists.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> <c>true</c> if the property exists on this object or in the prototype chain;
        /// <c>false</c> otherwise. </returns>
        public bool HasProperty(object key)
        {
            // Check if the name of the property qualifies it as an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(key);

            ObjectInstance prototypeObject = this;
            do
            {
                if (arrayIndex == uint.MaxValue)
                {
                    // Named property.
                    var property = prototypeObject.schema.GetPropertyIndexAndAttributes(key);
                    if (property.Exists == true)
                        return true;
                }
                else
                {
                    // Indexed property.
                    var property = this.GetOwnPropertyDescriptor(arrayIndex);
                    if (property.Exists == true)
                        return true;
                }

                // Traverse the prototype chain.
                prototypeObject = prototypeObject.prototype;
            } while (prototypeObject != null);

            return false;
        }

        /// <summary>
        /// Gets the value of the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        public object GetPropertyValue(uint index)
        {
            return GetPropertyValue(index, this);
        }

        /// <summary>
        /// Gets the value of the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <param name="thisValue"> The value of the "this" keyword inside a getter. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        private object GetPropertyValue(uint index, ObjectInstance thisValue)
        {
            // Get the descriptor for the property.
            var property = this.GetOwnPropertyDescriptor(index);
            if (property.Exists == true)
            {
                // The property was found!  Call the getter if there is one.
                object value = property.Value;
                var accessor = value as PropertyAccessorValue;
                if (accessor != null)
                    return accessor.GetValue(thisValue);
                return value;
            }

            // The property might exist in the prototype.
            if (this.prototype == null)
                return this.GetMissingPropertyValue(index.ToString());
            return this.prototype.GetPropertyValue(index, thisValue);
        }

        /// <summary>
        /// Gets the value of the property with the given name.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <param name="thisValue"> The value of the "this" keyword inside a getter. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        public object GetPropertyValue(object key, ObjectInstance thisValue = null)
        {
            // Check if the property is an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(key);
            if (arrayIndex != uint.MaxValue)
                return GetPropertyValue(arrayIndex, thisValue ?? this);

            // Otherwise, the property is a name.
            return GetNamedPropertyValue(key, thisValue ?? this);
        }

        /// <summary>
        /// Gets the value of the property with the given name.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <param name="value"> Receives the value of the property, or <c>null</c> if the property
        /// doesn't exist. </param>
        /// <returns> <c>true</c> if the value exists, <c>false</c> otherwise. </returns>
        public bool TryGetPropertyValue(object key, out object value)
        {
            value = GetPropertyValue(key);
            return value != null;
        }

        /// <summary>
        /// Gets the value of the property with the given name.
        /// </summary>
        /// <param name="propertyReference"> The name of the property. </param>
        /// <returns> The value of the property, or <see cref="Undefined.Value"/> if the property
        /// doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        public object GetPropertyValue(PropertyReference propertyReference)
        {
            // Check if anything has changed.
            if (propertyReference.CachedSchema == this.schema)
            {
                // Fast path: nothing has changed.
                return this.propertyValues[propertyReference.CachedIndex];
            }

            var propertyInfo = this.schema.GetPropertyIndexAndAttributes(propertyReference.Name);
            if (propertyInfo.Exists == true)
            {
                // The property exists; it can be cached as long as it is not an accessor property.
                if ((propertyInfo.Attributes & (PropertyAttributes.IsAccessorProperty | PropertyAttributes.IsLengthProperty)) != 0)
                {
                    // Getters and the length property cannot be cached.
                    propertyReference.ClearCache();

                    // Call the getter if there is one.
                    if (propertyInfo.IsAccessor == true)
                        return ((PropertyAccessorValue)this.propertyValues[propertyInfo.Index]).GetValue(this);

                    // Otherwise, the property is the "magic" length property.
                    return ((ArrayInstance)this).Length;
                }

                // The property can be cached for next time.
                propertyReference.CachePropertyDetails(this.schema, propertyInfo.Index);
                return this.propertyValues[propertyReference.CachedIndex];
            }
            else
            {
                // The property is in the prototype or is non-existent.
                propertyReference.ClearCache();
                if (this.Prototype == null)
                    return this.GetMissingPropertyValue(propertyReference.Name) ?? Undefined.Value;
                return this.Prototype.GetNamedPropertyValue(propertyReference.Name, this) ?? Undefined.Value;
            }
        }

        /// <summary>
        /// Gets the value of the property with the given name.  The name cannot be an array index.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol).  Cannot be an array index. </param>
        /// <param name="thisValue"> The value of the "this" keyword inside a getter. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        private object GetNamedPropertyValue(object key, ObjectInstance thisValue)
        {
            ObjectInstance prototypeObject = this;
            do
            {
                // Retrieve information about the property.
                var property = prototypeObject.schema.GetPropertyIndexAndAttributes(key);
                if (property.Exists == true)
                {
                    // The property was found!
                    object value = prototypeObject.propertyValues[property.Index];
                    if ((property.Attributes & (PropertyAttributes.IsAccessorProperty | PropertyAttributes.IsLengthProperty)) == 0)
                        return value;

                    // Call the getter if there is one.
                    if (property.IsAccessor == true)
                        return ((PropertyAccessorValue)value).GetValue(thisValue);

                    // Otherwise, the property is the "magic" length property.
                    return ((ArrayInstance)prototypeObject).Length;
                }

                // Traverse the prototype chain.
                prototypeObject = prototypeObject.prototype;
            } while (prototypeObject != null);

            // The property doesn't exist.
            return thisValue.GetMissingPropertyValue(key);
        }

        /// <summary>
        /// Retrieves the value of a property which doesn't exist on the object.  This method can
        /// be overridden to effectively construct properties on the fly.  The default behavior is
        /// to return <c>undefined</c>.
        /// </summary>
        /// <param name="key"> The property key of the missing property. </param>
        /// <returns> The value of the missing property. </returns>
        /// <remarks> When overriding, call the base class implementation only if you want to
        /// revert to the default behavior. </remarks>
        protected virtual object GetMissingPropertyValue(object key)
        {
            if (this.prototype == null)
                return null;
            return this.prototype.GetMissingPropertyValue(key);
        }

        /// <summary>
        /// Gets a descriptor for the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes. </returns>
        /// <remarks> The prototype chain is not searched. </remarks>
        public virtual PropertyDescriptor GetOwnPropertyDescriptor(uint index)
        {
            var property = this.schema.GetPropertyIndexAndAttributes(index.ToString());
            if (property.Exists == true)
                return new PropertyDescriptor(this.propertyValues[property.Index], property.Attributes);
            return PropertyDescriptor.Missing;
        }

        /// <summary>
        /// Gets a descriptor for the property with the given name.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> A property descriptor containing the property value and attributes. </returns>
        /// <remarks> The prototype chain is not searched. </remarks>
        public virtual PropertyDescriptor GetOwnPropertyDescriptor(object key)
        {
            // Check if the property is an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(key);
            if (arrayIndex != uint.MaxValue)
                return GetOwnPropertyDescriptor(arrayIndex);

            // Retrieve information about the property.
            var property = this.schema.GetPropertyIndexAndAttributes(key);
            if (property.Exists == true)
            {
                if (property.IsLength == false)
                    return new PropertyDescriptor(this.propertyValues[property.Index], property.Attributes);

                // The property is the "magic" length property.
                return new PropertyDescriptor(((ArrayInstance)this).Length, property.Attributes);
            }

            // The property doesn't exist.
            return PropertyDescriptor.Missing;
        }

        /// <summary>
        /// Returns the function with the given name, if it exists.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). Cannot be a number. </param>
        /// <returns> The method with the given name, if it exists; otherwise <c>null</c>. </returns>
        /// <exception cref="JavaScriptException"> A property exists with the given name, but it's not callable. </exception>
        public FunctionInstance GetMethod(object key)
        {
            var value = GetNamedPropertyValue(key, this);
            if (value == null || value == Undefined.Value || value == Null.Value)
                return null;
            if (value is FunctionInstance f)
                return f;
            throw new JavaScriptException(ErrorType.TypeError, $"'{TypeConverter.ToString(value)}' returned for property '{TypeConverter.ToString(key)}' of object '{TypeConverter.ToString(this)}' is not a function.");
        }

        /// <summary>
        /// Sets the value of the property with the given array index.  If a property with the
        /// given index does not exist, or exists in the prototype chain (and is not a setter) then
        /// a new property is created.
        /// </summary>
        /// <param name="index"> The array index of the property to set. </param>
        /// <param name="value"> The value to set the property to.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is read-only or if the object is sealed. </param>
        /// <returns> <c>false</c> if an error occurred. </returns>
        public virtual bool SetPropertyValue(uint index, object value, bool throwOnError)
        {
            string indexStr = index.ToString();
            if (!SetPropertyValueIfExists(indexStr, value, throwOnError, out bool exists))
                return false;
            if (exists == false)
            {
                // The property doesn't exist - add it.
                return AddProperty(indexStr, value, PropertyAttributes.FullAccess, throwOnError);
            }
            return true;
        }

        /// <summary>
        /// Sets the value of the property with the given name.  If a property with the given name
        /// does not exist, or exists in the prototype chain (and is not a setter) then a new
        /// property is created.
        /// </summary>
        /// <param name="key"> The property key of the property to set. </param>
        /// <param name="value"> The value to set the property to.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set (i.e. if the property is read-only or if the object is not extensible and a new
        /// property needs to be created). </param>
        /// <returns> <c>false</c> if <paramref name="throwOnError"/> is false and an error
        /// occurred; <c>true</c> otherwise. </returns>
        public bool SetPropertyValue(object key, object value, bool throwOnError)
        {
            // Check if the property is an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(key);
            if (arrayIndex != uint.MaxValue)
                return SetPropertyValue(arrayIndex, value, throwOnError);

            if (!SetPropertyValueIfExists(key, value, throwOnError, out bool exists))
                return false;
            if (exists == false)
            {
                // The property doesn't exist - add it.
                return AddProperty(key, value, PropertyAttributes.FullAccess, throwOnError);
            }
            return true;
        }

        /// <summary>
        /// Sets the value of the property with the given name.  If a property with the given name
        /// does not exist, or exists in the prototype chain (and is not a setter) then a new
        /// property is created.
        /// </summary>
        /// <param name="propertyReference"> The name of the property to set. </param>
        /// <param name="value"> The value to set the property to.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set (i.e. if the property is read-only or if the object is not extensible and a new
        /// property needs to be created). </param>
        /// <returns> <c>false</c> if <paramref name="throwOnError"/> is false and an error
        /// occurred; <c>true</c> otherwise. </returns>
        public bool SetPropertyValue(PropertyReference propertyReference, object value, bool throwOnError)
        {
            // Check if anything has changed.
            if (propertyReference.CachedSchema == this.schema)
            {
                // Fast path: nothing has changed.
                this.propertyValues[propertyReference.CachedIndex] = value ?? Undefined.Value;
                return true;
            }

            var propertyInfo = this.schema.GetPropertyIndexAndAttributes(propertyReference.Name);
            if (propertyInfo.Exists == true)
            {
                // The property exists; it can be cached as long as it is not read-only or an accessor property.
                if ((propertyInfo.Attributes & (PropertyAttributes.Writable | PropertyAttributes.IsAccessorProperty | PropertyAttributes.IsLengthProperty)) != PropertyAttributes.Writable)
                {
                    propertyReference.ClearCache();
                    return this.SetPropertyValue(propertyReference.Name, value, throwOnError);
                }
                else
                {
                    // The property can be cached.
                    propertyReference.CachePropertyDetails(this.schema, propertyInfo.Index);
                    this.propertyValues[propertyReference.CachedIndex] = value ?? Undefined.Value;
                    return true;
                }
            }
            else
            {
                // The property is in the prototype or is non-existent.
                propertyReference.ClearCache();
                return this.SetPropertyValue(propertyReference.Name, value, throwOnError);
            }
        }

        /// <summary>
        /// Sets the value of the given property.  If a property with the given name exists, but
        /// only in the prototype chain, then a new property is created (unless the property is a
        /// setter, in which case the setter is called and no property is created).  If the
        /// property does not exist at all, then no property is created and the method returns
        /// <c>false</c>.  This method is used to set the value of a variable reference within a
        /// with statement.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). Cannot be an array index. </param>
        /// <param name="value"> The desired value of the property. This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set (i.e. if the property is read-only or if the object is not extensible and a new
        /// property needs to be created). </param>
        /// <param name="exists"> Set to <c>true</c> if the property value exists; <c>false</c> otherwise. </param>
        /// <returns> <c>false</c> if <paramref name="throwOnError"/> is false and an error
        /// occurred; <c>true</c> otherwise. </returns>
        public bool SetPropertyValueIfExists(object key, object value, bool throwOnError, out bool exists)
        {
            // Do not store nulls - null represents a non-existant value.
            value = value ?? Undefined.Value;

            // Retrieve information about the property.
            var property = this.schema.GetPropertyIndexAndAttributes(key);
            if (property.Exists == true)
            {
                exists = true;

                // Check if the property is read-only.
                if (property.IsWritable == false)
                {
                    // The property is read-only.
                    if (throwOnError == true)
                        throw new JavaScriptException(ErrorType.TypeError, string.Format("The property '{0}' is read-only.", key));
                    return false;
                }

                if ((property.Attributes & (PropertyAttributes.IsAccessorProperty | PropertyAttributes.IsLengthProperty)) == 0)
                {
                    // The property contains a simple value.  Set the property value.
                    this.propertyValues[property.Index] = value;
                }
                else if (property.IsAccessor == true)
                {
                    // The property contains an accessor function.  Set the property value by calling the accessor.
                    ((PropertyAccessorValue)this.propertyValues[property.Index]).SetValue(this, value);
                }
                else
                {
                    // Otherwise, the property is the "magic" length property.
                    double length = TypeConverter.ToNumber(value);
                    uint lengthUint32 = TypeConverter.ToUint32(length);
                    if (length != (double)lengthUint32)
                        throw new JavaScriptException(ErrorType.RangeError, "Invalid array length");
                    ((ArrayInstance)this).Length = lengthUint32;
                }
                return true;
            }

            // Search the prototype chain for a accessor function.  If one is found, it will
            // prevent the creation of a new property.
            bool propertyExistsInPrototype = false;
            ObjectInstance prototypeObject = this.prototype;
            while (prototypeObject != null)
            {
                property = prototypeObject.schema.GetPropertyIndexAndAttributes(key);
                if (property.Exists == true)
                {
                    if (property.IsAccessor == true)
                    {
                        // The property contains an accessor function.  Set the property value by calling the accessor.
                        ((PropertyAccessorValue)prototypeObject.propertyValues[property.Index]).SetValue(this, value);
                        exists = true;
                        return true;
                    }
                    propertyExistsInPrototype = true;
                    break;
                }
                prototypeObject = prototypeObject.prototype;
            }

            // If the property exists in the prototype, create a new property.
            if (propertyExistsInPrototype == true)
            {
                AddProperty(key, value, PropertyAttributes.FullAccess, throwOnError);
                exists = true;
                return true;
            }

            // The property does not exist.
            exists = false;
            return true;
        }
        
        /// <summary>
        /// Deletes the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property to delete. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set because the property was marked as non-configurable.  </param>
        /// <returns> <c>true</c> if the property was successfully deleted, or if the property did
        /// not exist; <c>false</c> if the property was marked as non-configurable and
        /// <paramref name="throwOnError"/> was <c>false</c>. </returns>
        public virtual bool Delete(uint index, bool throwOnError)
        {
            string indexStr = index.ToString();

            // Retrieve the attributes for the property.
            var propertyInfo = this.schema.GetPropertyIndexAndAttributes(indexStr);
            if (propertyInfo.Exists == false)
                return true;    // Property doesn't exist - delete succeeded!

            // Delete the property.
            this.schema = this.schema.DeleteProperty(indexStr);
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
        public bool Delete(object key, bool throwOnError)
        {
            // Check if the property is an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(key);
            if (arrayIndex != uint.MaxValue)
                return Delete(arrayIndex, throwOnError);

            // Retrieve the attributes for the property.
            var propertyInfo = this.schema.GetPropertyIndexAndAttributes(key);
            if (propertyInfo.Exists == false)
                return true;    // Property doesn't exist - delete succeeded!

            // Check if the property can be deleted.
            if (propertyInfo.IsConfigurable == false)
            {
                if (throwOnError == true)
                    throw new JavaScriptException(ErrorType.TypeError, string.Format("The property '{0}' cannot be deleted.", key));
                return false;
            }

            // Delete the property.
            this.schema = this.schema.DeleteProperty(key);
            return true;
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
        public virtual bool DefineProperty(object key, PropertyDescriptor descriptor, bool throwOnError)
        {
            // Retrieve info on the property.
            var current = this.schema.GetPropertyIndexAndAttributes(key);

            if (current.Exists == false)
            {
                // Create a new property.
                return AddProperty(key, descriptor.Value, descriptor.Attributes, throwOnError);
            }

            // If the current property is not configurable, then the only change that is allowed is
            // a change from one simple value to another (i.e. accessor changes are not allowed) and
            // only if the writable attribute is currently set.
            if (!PropertyDescriptor.IsCompatible(descriptor, new PropertyDescriptor(this.propertyValues[current.Index], current.Attributes)))
            {
                if (throwOnError == true)
                    throw new JavaScriptException(ErrorType.TypeError, string.Format("The property '{0}' is non-configurable.", key));
                return false;
            }

            // Set the property attributes.
            if (descriptor.Attributes != current.Attributes)
                this.schema = this.schema.SetPropertyAttributes(key, descriptor.Attributes);

            // Set the property value.
            this.propertyValues[current.Index] = descriptor.Value;

            return true;
        }

        /// <summary>
        /// Checks whether the given descriptor is compatible with the current descriptor.
        /// </summary>
        /// <param name="isExtensible"> Indicates whether the target object is extensible. </param>
        /// <param name="descriptor"> The new descriptor. </param>
        /// <param name="current"> The descriptor corresponding to the currently existing property. </param>
        /// <returns> <c>true</c> if the new descriptor is compatible with the old one; <c>false</c> otherwise. </returns>
        internal static bool IsCompatiblePropertyDescriptor(bool isExtensible, PropertyDescriptor descriptor, PropertyDescriptor current)
        {
            // If the current property doesn't exist, then the new descriptor is always compatible,
            // unless isExtensible is false.
            if (!current.Exists)
                return isExtensible;

            // If the current property is not configurable, then the only change that is allowed is
            // a change from one simple value to another (i.e. accessor changes are not allowed) and
            // only if the writable attribute is currently set.
            return PropertyDescriptor.IsCompatible(descriptor, current);
        }

        /// <summary>
        /// Adds a property to this object.  The property must not already exist.
        /// </summary>
        /// <param name="key"> The property key of the property to add. </param>
        /// <param name="value"> The desired value of the property.  This can be a
        /// <see cref="PropertyAccessorValue"/>. </param>
        /// <param name="attributes"> Attributes describing how the property may be modified. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be added (i.e. if the object is not extensible). </param>
        /// <returns> <c>true</c> if the property was successfully added; <c>false</c> otherwise. </returns>
        private bool AddProperty(object key, object value, PropertyAttributes attributes, bool throwOnError)
        {
            // Make sure adding a property is allowed.
            if (this.IsExtensible == false)
            {
                if (throwOnError == true)
                    throw new JavaScriptException(ErrorType.TypeError, string.Format("The property '{0}' cannot be created as the object is not extensible.", key));
                return false;
            }

            // To avoid running out of memory, restrict the number of properties.
            if (this.schema.PropertyCount == 16384)
                throw new JavaScriptException(ErrorType.Error, "Maximum number of named properties reached.");

            // Do not store nulls - null represents a non-existant value.
            value = value ?? Undefined.Value;

            // Add a new property to the schema.
            this.schema = this.schema.AddProperty(key, attributes);

            // Check if the value array needs to be resized.
            int propertyIndex = this.schema.NextValueIndex - 1;
            if (propertyIndex >= this.propertyValues.Length)
                Array.Resize(ref this.propertyValues, this.propertyValues.Length * 2);

            // Set the value of the property.
            this.propertyValues[propertyIndex] = value;

            // Success.
            return true;
        }

        /// <summary>
        /// Sets a property value and attributes, or adds a new property if it doesn't already
        /// exist.  Any existing attributes are ignored (and not modified).
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <param name="value"> The intended value of the property. </param>
        /// <param name="attributes"> Attributes that indicate whether the property is writable,
        /// configurable and enumerable. </param>
        /// <param name="overwriteAttributes"> Indicates whether to overwrite any existing attributes. </param>
        internal void FastSetProperty(object key, object value, PropertyAttributes attributes = PropertyAttributes.Sealed, bool overwriteAttributes = false)
        {
            int index = this.schema.GetPropertyIndex(key);
            if (index < 0)
            {
                // The property is doesn't exist - add a new property.
                AddProperty(key, value, attributes, false);
                return;
            }
            if (overwriteAttributes == true)
                this.schema = this.schema.SetPropertyAttributes(key, attributes);
            this.propertyValues[index] = value;
        }

        /// <summary>
        /// Sets up multiple properties at once. Can only be called on an empty object.
        /// </summary>
        /// <param name="properties"> The list of properties to set. </param>
        public void InitializeProperties(IEnumerable<PropertyNameAndValue> properties)
        {
            if (this.schema.NextValueIndex != 0)
                throw new InvalidOperationException("This method can only be called on an empty object (one with no properties).");

            if (this.propertyValues.Length < properties.Count())
                this.propertyValues = new object[properties.Count()];
            var propertyDictionary = new Dictionary<object, SchemaProperty>(properties.Count());
            int nextValueIndex = 0;
            foreach (var property in properties)
            {
                this.propertyValues[nextValueIndex] = property.Value;
                propertyDictionary.Add(property.Key, new SchemaProperty(nextValueIndex++, property.Attributes));
            }
            this.schema = new HiddenClassSchema(propertyDictionary, nextValueIndex);
        }

        /// <summary>
        /// Initializes a property to be <c>undefined</c>, if the property doesn't exist.
        /// If it does exist, then this method does nothing.
        /// </summary>
        /// <param name="key"> The property key of the property. </param>
        /// <param name="attributes"> The attributes of the new property, if it doesn't exist. </param>
        public void InitializeMissingProperty(object key, PropertyAttributes attributes)
        {
            // Retrieve info on the property.
            var current = this.schema.GetPropertyIndexAndAttributes(key);

            if (current.Exists == false)
            {
                // Create a new property.
                AddProperty(key, Undefined.Value, attributes, false);
            }
        }


        //     OTHERS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a primitive value that represents the current object.  Used by the addition and
        /// equality operators.
        /// </summary>
        /// <param name="typeHint"> Indicates the preferred type of the result. </param>
        /// <returns> A primitive value that represents the current object. </returns>
        internal object GetPrimitiveValue(PrimitiveTypeHint typeHint)
        {
            // The first step is to try calling the @@toPrimitive symbol.
            string hintStr;
            switch (typeHint)
            {
                case PrimitiveTypeHint.None:
                    hintStr = "default";
                    break;
                case PrimitiveTypeHint.Number:
                    hintStr = "number";
                    break;
                case PrimitiveTypeHint.String:
                    hintStr = "string";
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported PrimitiveTypeHint value '{typeHint}'.");
            }
            object toPrimitiveResult;
            if (TryCallMemberFunction(out toPrimitiveResult, Symbol.ToPrimitive, hintStr) == true)
            {
                // Return value must be primitive.
                if (TypeUtilities.IsPrimitive(toPrimitiveResult) == false)
                    throw new JavaScriptException(ErrorType.TypeError, "Cannot convert object to primitive value.");
                return toPrimitiveResult;
            }

            // If that didn't work.
            return GetPrimitiveValuePreES6(typeHint);
        }

        /// <summary>
        /// Returns a primitive value that represents the current object, without using the
        /// @@toPrimitive symbol.
        /// </summary>
        /// <param name="typeHint"> Indicates the preferred type of the result. </param>
        /// <returns> A primitive value that represents the current object. </returns>
        internal object GetPrimitiveValuePreES6(PrimitiveTypeHint typeHint)
        {
            if (typeHint == PrimitiveTypeHint.None || typeHint == PrimitiveTypeHint.Number)
            {

                // Try calling valueOf().
                object valueOfResult;
                if (TryCallMemberFunction(out valueOfResult, "valueOf") == true)
                {
                    // Return value must be primitive.
                    if (valueOfResult is double || TypeUtilities.IsPrimitive(valueOfResult) == true)
                        return valueOfResult;
                }

                // Try calling toString().
                object toStringResult;
                if (TryCallMemberFunction(out toStringResult, "toString") == true)
                {
                    // Return value must be primitive.
                    if (toStringResult is string || TypeUtilities.IsPrimitive(toStringResult) == true)
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
                    if (toStringResult is string || TypeUtilities.IsPrimitive(toStringResult) == true)
                        return toStringResult;
                }

                // Try calling valueOf().
                object valueOfResult;
                if (TryCallMemberFunction(out valueOfResult, "valueOf") == true)
                {
                    // Return value must be primitive.
                    if (valueOfResult is double || TypeUtilities.IsPrimitive(valueOfResult) == true)
                        return valueOfResult;
                }

            }

            throw new JavaScriptException(ErrorType.TypeError, "Attempted conversion of the object to a primitive value failed.  Check the toString() and valueOf() functions.");
        }

        /// <summary>
        /// Calls the function with the given name.  The function must exist on this object or an
        /// exception will be thrown.
        /// </summary>
        /// <param name="functionName"> The name of the function to call (or a symbol). </param>
        /// <param name="parameters"> The parameters to pass to the function. </param>
        /// <returns> The result of calling the function. </returns>
        public object CallMemberFunction(object functionName, params object[] parameters)
        {
            var function = GetPropertyValue(TypeConverter.ToPropertyKey(functionName));
            if (function == null)
                throw new JavaScriptException(ErrorType.TypeError, string.Format("Object {0} has no method '{1}'", this.ToString(), functionName));
            if ((function is FunctionInstance) == false)
                throw new JavaScriptException(ErrorType.TypeError, string.Format("Property '{1}' of object {0} is not a function", this.ToString(), functionName));
            return ((FunctionInstance)function).CallLateBound(this, parameters);
        }

        /// <summary>
        /// Calls the function with the given name.
        /// </summary>
        /// <param name="result"> The result of calling the function. </param>
        /// <param name="key"> The name or symbol of the function to call. </param>
        /// <param name="parameters"> The parameters to pass to the function. </param>
        /// <returns> <c>true</c> if the function was called successfully; <c>false</c> otherwise. </returns>
        public bool TryCallMemberFunction(out object result, object key, params object[] parameters)
        {
            var function = GetPropertyValue(TypeConverter.ToPropertyKey(key));
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
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="thisObject"> The object that is being operated on. </param>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> <c>true</c> if a property with the given name exists on this object,
        /// <c>false</c> otherwise. </returns>
        /// <remarks> Objects in the prototype chain are not considered. </remarks>
        [JSInternalFunction(Name = "hasOwnProperty", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static bool HasOwnProperty(ScriptEngine engine, object thisObject, object key)
        {
            key = TypeConverter.ToPropertyKey(key);
            TypeUtilities.VerifyThisObject(engine, thisObject, "hasOwnProperty");
            return TypeConverter.ToObject(engine, thisObject).GetOwnPropertyDescriptor(key).Exists;
        }

        /// <summary>
        /// Determines if this object is in the prototype chain of the given object.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="thisObject"> The object that is being operated on. </param>
        /// <param name="obj"> The object to check. </param>
        /// <returns> <c>true</c> if this object is in the prototype chain of the given object;
        /// <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "isPrototypeOf", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static bool IsPrototypeOf(ScriptEngine engine, object thisObject, object obj)
        {
            if ((obj is ObjectInstance) == false)
                return false;
            TypeUtilities.VerifyThisObject(engine, thisObject, "isPrototypeOf");
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
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="thisObject"> The object that is being operated on. </param>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> <c>true</c> if a property with the given name exists on this object and is
        /// enumerable, <c>false</c> otherwise. </returns>
        /// <remarks> Objects in the prototype chain are not considered. </remarks>
        [JSInternalFunction(Name = "propertyIsEnumerable", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static bool PropertyIsEnumerable(ScriptEngine engine, object thisObject, object key)
        {
            key = TypeConverter.ToPropertyKey(key);
            TypeUtilities.VerifyThisObject(engine, thisObject, "propertyIsEnumerable");
            var property = TypeConverter.ToObject(engine, thisObject).GetOwnPropertyDescriptor(key);
            return property.Exists && property.IsEnumerable;
        }

        /// <summary>
        /// Returns a locale-dependant string representing the current object.
        /// </summary>
        /// <returns> Returns a locale-dependant string representing the current object. </returns>
        [JSInternalFunction(Name = "toLocaleString")]
        public string ToLocaleString()
        {
            return TypeConverter.ToString(CallMemberFunction("toString"));
        }

        /// <summary>
        /// Returns a primitive value associated with the object.
        /// </summary>
        /// <returns> A primitive value associated with the object. </returns>
        [JSInternalFunction(Name = "valueOf")]
        public ObjectInstance ValueOf()
        {
            return this;
        }

        /// <summary>
        /// Returns a string representing the current object.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="thisObject"> The value of the "this" keyword. </param>
        /// <returns> A string representing the current object. </returns>
        [JSInternalFunction(Name = "toString", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static string ToStringJS(ScriptEngine engine, object thisObject)
        {
            if (thisObject == null || thisObject == Undefined.Value)
                return "[object Undefined]";
            if (thisObject == Null.Value)
                return "[object Null]";
            var obj = TypeConverter.ToObject(engine, thisObject);

            // ES6 - if the value of @@toStringTag is a string, use it to form the result.
            object tag = obj.GetPropertyValue(Symbol.ToStringTag);
            if (tag is string)
                return $"[object {tag}]";

            // Fall back to previous behaviour.
            if (obj is ArrayInstance)
                return "[object Array]";
            if (obj is StringInstance)
                return "[object String]";
            if (obj is ArgumentsInstance)
                return "[object Arguments]";
            if (obj is FunctionInstance)
                return "[object Function]";
            if (obj is ErrorInstance)
                return "[object Error]";
            if (obj is BooleanInstance)
                return "[object Boolean]";
            if (obj is NumberInstance)
                return "[object Number]";
            if (obj is DateInstance)
                return "[object Date]";
            if (obj is RegExpInstance)
                return "[object RegExp]";
            return "[object Object]";
        }



        //     ATTRIBUTE-BASED PROTOTYPE POPULATION
        //_________________________________________________________________________________________

        private class MethodGroup
        {
            public List<JSBinderMethod> Methods;
            public int Length;
            public PropertyAttributes PropertyAttributes;
        }

        /// <summary>
        /// Populates the object with functions by searching a .NET type for methods marked with
        /// the [JSFunction] attribute.  Should be called only once at startup.  Also automatically
        /// populates properties marked with the [JSProperty] attribute.
        /// </summary>
        internal protected void PopulateFunctions()
        {
            PopulateFunctions(null);
        }

        /// <summary>
        /// Populates the object with functions by searching a .NET type for methods marked with
        /// the [JSFunction] attribute.  Should be called only once at startup.  Also automatically
        /// populates properties marked with the [JSProperty] attribute.
        /// </summary>
        /// <param name="type"> The type to search for methods. </param>
        internal protected void PopulateFunctions(Type type)
        {
            PopulateFunctions(type, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        }

        /// <summary>
        /// Populates the object with functions by searching a .NET type for methods marked with
        /// the [JSFunction] attribute.  Should be called only once at startup.  Also automatically
        /// populates properties marked with the [JSProperty] attribute.
        /// </summary>
        /// <param name="type"> The type to search for methods. </param>
        /// <param name="bindingFlags"> The binding flags to use to search for properties and methods. </param>
        internal protected void PopulateFunctions(Type type, BindingFlags bindingFlags)
        {
            if (type == null)
                type = this.GetType();
            
            // Group the methods on the given type by name.
            var functions = new Dictionary<string, MethodGroup>(20);
            var methods = type.GetMethods(bindingFlags);
            foreach (var method in methods)
            {
                // Make sure the method has the [JSInternalFunction] attribute.
                var attribute = (JSFunctionAttribute)Attribute.GetCustomAttribute(method, typeof(JSFunctionAttribute));
                if (attribute == null)
                    continue;

                // Determine the name of the method.
                string name;
                if (attribute.Name != null)
                    name = attribute.Name;
                else
                    name = method.Name;

                // Get a reference to the method group.
                MethodGroup methodGroup;
                if (functions.ContainsKey(name) == false)
                {
                    methodGroup = new MethodGroup { Methods = new List<JSBinderMethod>(1), Length = -1 };
                    functions.Add(name, methodGroup);
                }
                else
                    methodGroup = functions[name];

                // Internal functions return nulls as undefined.
                if (attribute is JSInternalFunctionAttribute)
                    attribute.Flags |= JSFunctionFlags.ConvertNullReturnValueToUndefined;

                // Add the method to the list.
                methodGroup.Methods.Add(new JSBinderMethod(method, attribute.Flags));

                // If the length doesn't equal -1, that indicates an explicit length has been set.
                // Make sure it is consistant with the other methods.
                if (attribute.Length >= 0)
                {
                    if (methodGroup.Length != -1 && methodGroup.Length != attribute.Length)
                        throw new InvalidOperationException(string.Format("Inconsistant Length property detected on {0}.", method));
                    methodGroup.Length = attribute.Length;
                }

                // Check property attributes.
                var descriptorAttributes = PropertyAttributes.Sealed;
                if (attribute.IsEnumerable)
                    descriptorAttributes |= PropertyAttributes.Enumerable;
                if (attribute.IsConfigurable)
                    descriptorAttributes |= PropertyAttributes.Configurable;
                if (attribute.IsWritable)
                    descriptorAttributes |= PropertyAttributes.Writable;
                if (methodGroup.Methods.Count > 1 && methodGroup.PropertyAttributes != descriptorAttributes)
                    throw new InvalidOperationException(string.Format("Inconsistant property attributes detected on {0}.", method));
                methodGroup.PropertyAttributes = descriptorAttributes;
            }

            // Now set the relevant properties on the object.
            foreach (KeyValuePair<string, MethodGroup> pair in functions)
            {
                string name = pair.Key;
                MethodGroup methodGroup = pair.Value;

                // Add the function as a property of the object.
                this.FastSetProperty(name, new ClrFunction(this.Engine.Function.InstancePrototype, methodGroup.Methods, name, methodGroup.Length), methodGroup.PropertyAttributes);
            }

            PropertyInfo[] properties = type.GetProperties(bindingFlags);
            foreach (PropertyInfo prop in properties)
            {
                var attribute = Attribute.GetCustomAttribute(prop, typeof(JSPropertyAttribute), false) as JSPropertyAttribute;
                if (attribute == null)
                    continue;

                // The property name.
                string name;
                if (attribute.Name != null)
                    name = attribute.Name;
                else
                    name = prop.Name;

                // The property getter.
                ClrFunction getter = null;
                if (prop.CanRead)
                {
                    var getMethod = prop.GetGetMethod(true);
                    getter = new ClrFunction(engine.Function.InstancePrototype, new JSBinderMethod[] { new JSBinderMethod(getMethod) }, name, 0);
                }

                // The property setter.
                ClrFunction setter = null;
                if (prop.CanWrite)
                {
                    var setMethod = prop.GetSetMethod((bindingFlags & BindingFlags.NonPublic) != 0);
                    if (setMethod != null)
                        setter = new ClrFunction(engine.Function.InstancePrototype, new JSBinderMethod[] { new JSBinderMethod(setMethod) }, name, 1);
                }

                // The property attributes.
                var descriptorAttributes = PropertyAttributes.Sealed;
                if (attribute.IsEnumerable)
                    descriptorAttributes |= PropertyAttributes.Enumerable;
                if (attribute.IsConfigurable)
                    descriptorAttributes |= PropertyAttributes.Configurable;

                // Define the property.
                var descriptor = new PropertyDescriptor(getter, setter, descriptorAttributes);
                this.DefineProperty(name, descriptor, true);
            }
        }

        /// <summary>
        /// Populates the object with properties by searching a .NET type for fields marked with
        /// the [JSField] attribute.  Should be called only once at startup.
        /// </summary>
        internal protected void PopulateFields()
        {
            PopulateFields(null);
        }

        /// <summary>
        /// Populates the object with properties by searching a .NET type for fields marked with
        /// the [JSField] attribute.  Should be called only once at startup.
        /// </summary>
        /// <param name="type"> The type to search for fields. </param>
        internal protected void PopulateFields(Type type)
        {
            if (type == null)
                type = this.GetType();

            // Find all fields with [JsField]
            foreach (var field in type.GetFields())
            {
                var attribute = (JSFieldAttribute)Attribute.GetCustomAttribute(field, typeof(JSFieldAttribute));
                if (attribute == null)
                    continue;
                this.FastSetProperty(field.Name, field.GetValue(this));
            }
        }
    }
}
