using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript Object object.
    /// </summary>
    public partial class ObjectConstructor : ClrStubFunction
    {
        
        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Object object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="instancePrototype"> The prototype for instances created by this function. </param>
        internal ObjectConstructor(ObjectInstance prototype, ObjectInstance instancePrototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "Object", 1, instancePrototype);
            FastSetProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Object instance.
        /// </summary>
        [JSConstructorFunction]
        public ObjectInstance Construct()
        {
            return ObjectInstance.CreateRawObject(this.InstancePrototype);
        }

        /// <summary>
        /// Converts the given argument to an object.
        /// </summary>
        /// <param name="obj"> The value to convert. </param>
        [JSConstructorFunction]
        public ObjectInstance Construct(object obj)
        {
            if (obj == null || obj == Undefined.Value || obj == Null.Value)
                return this.Engine.Object.Construct();
            return TypeConverter.ToObject(this.Engine, obj);
        }

        /// <summary>
        /// Converts the given argument to an object.
        /// </summary>
        /// <param name="obj"> The value to convert. </param>
        [JSCallFunction]
        public ObjectInstance Call(object obj)
        {
            if (obj == null || obj == Undefined.Value || obj == Null.Value)
                return this.Construct();
            return TypeConverter.ToObject(this.Engine, obj);
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Retrieves the next object in the prototype chain for the given object.
        /// </summary>
        /// <param name="obj"> The object to retrieve the prototype from. </param>
        /// <returns> The next object in the prototype chain for the given object, or <c>null</c>
        /// if the object has no prototype chain. </returns>
        [JSInternalFunction(Name = "getPrototypeOf")]
        public static object GetPrototypeOf(ObjectInstance obj)
        {
            if (obj is FunctionInstance fi && fi.InstancePrototype.Prototype?.Prototype != null)
            {
                var p = fi.InstancePrototype?.Prototype?.GetPropertyValue("constructor");
                if (p != null)
                {
                    return p;
                }
            }
            var result = obj.Prototype;
            if (result == null)
                return Null.Value;
            return result;
        }

        /// <summary>
        /// Gets an object that contains details of the property with the given name.
        /// </summary>
        /// <param name="obj"> The object to retrieve property details for. </param>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> An object containing some of the following properties: configurable,
        /// writable, enumerable, value, get and set. </returns>
        [JSInternalFunction(Name = "getOwnPropertyDescriptor")]
        public static ObjectInstance GetOwnPropertyDescriptor(ObjectInstance obj, object key)
        {
            var descriptor = obj.GetOwnPropertyDescriptor(TypeConverter.ToPropertyKey(key));
            if (descriptor.Exists == false)
                return null;
            return descriptor.ToObject(obj.Engine);
        }

        /// <summary>
        /// Creates an array containing the names of all the named properties on the object (even
        /// the non-enumerable ones).
        /// </summary>
        /// <param name="obj"> The object to retrieve the property names for. </param>
        /// <returns> An array containing property names. </returns>
        [JSInternalFunction(Name = "getOwnPropertyNames")]
        public static ArrayInstance GetOwnPropertyNames(ObjectInstance obj)
        {
            var result = obj.Engine.Array.New();
            foreach (var property in obj.Properties)
                if (property.Key is string)
                    result.Push(property.Key);
            return result;
        }

        /// <summary>
        /// Creates an array containing the symbols of all the symbol-based properties on the
        /// object (even the non-enumerable ones).
        /// </summary>
        /// <param name="obj"> The object to retrieve the property symbols for. </param>
        /// <returns> An array containing symbols. </returns>
        [JSInternalFunction(Name = "getOwnPropertySymbols")]
        public static ArrayInstance GetOwnPropertySymbols(ObjectInstance obj)
        {
            var result = obj.Engine.Array.New();
            foreach (var property in obj.Properties)
                if (property.Key is SymbolInstance)
                    result.Push(property.Key);
            return result;
        }

        /// <summary>
        /// Creates an object with the given prototype and, optionally, a set of properties.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="prototype"> A reference to the next object in the prototype chain for the
        /// created object. </param>
        /// <param name="properties"> An object containing one or more property descriptors. </param>
        /// <returns> A new object instance. </returns>
        [JSInternalFunction(Name = "create", Flags = JSFunctionFlags.HasEngineParameter)]
        public static ObjectInstance Create(ScriptEngine engine, object prototype, ObjectInstance properties = null)
        {
            if ((prototype is ObjectInstance) == false && prototype != Null.Value)
                throw new JavaScriptException(engine, ErrorType.TypeError, "object prototype must be an object or null");
            ObjectInstance result;
            if (prototype == Null.Value)
                result = ObjectInstance.CreateRootObject(engine);
            else
                result = ObjectInstance.CreateRawObject((ObjectInstance)prototype);
            if (properties != null)
                DefineProperties(result, properties);
            return result;
        }

        /// <summary>
        /// Assigns enumerable properties of one or more source objects onto a destination object.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="target"> The destination object to copy properties to. </param>
        /// <param name="sources"> One or more source objects to copy properties from. </param>
        /// <returns> A new object instance. </returns>
        [JSInternalFunction(Name = "assign", Flags = JSFunctionFlags.HasEngineParameter)]
        public static ObjectInstance Assign(ScriptEngine engine, ObjectInstance target, params object[] sources)
        {
            foreach (var rawSource in sources)
            {
                // Ignore undefined or null sources.
                if (rawSource == null || rawSource == Undefined.Value || rawSource == Null.Value)
                    continue;
                var source = TypeConverter.ToObject(engine, rawSource);

                // Copy the enumerable properties from the source object.
                foreach (var property in source.Properties)
                    if (property.IsEnumerable == true)
                        target.SetPropertyValue(property.Key, property.Value, throwOnError: true);
            }
            return target;
        }

        /// <summary>
        /// Modifies the value and attributes of a property.
        /// </summary>
        /// <param name="obj"> The object to define the property on. </param>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <param name="attributes"> A property descriptor containing some of the following
        /// properties: configurable, writable, enumerable, value, get and set. </param>
        /// <returns> The object with the property. </returns>
        [JSInternalFunction(Name = "defineProperty")]
        public static ObjectInstance DefineProperty(ObjectInstance obj, object key, object attributes)
        {
            key = TypeConverter.ToPropertyKey(key);
            var defaults = obj.GetOwnPropertyDescriptor(key);
            if (!(attributes is ObjectInstance))
                throw new JavaScriptException(obj.Engine, ErrorType.TypeError, "Invalid descriptor for property '{propertyName}'.");
            var descriptor = PropertyDescriptor.FromObject((ObjectInstance)attributes, defaults);
            obj.DefineProperty(key, descriptor, true);
            return obj;
        }

        /// <summary>
        /// Modifies multiple properties on an object.
        /// </summary>
        /// <param name="obj"> The object to define the properties on. </param>
        /// <param name="properties"> An object containing one or more property descriptors. </param>
        /// <returns> The object with the properties. </returns>
        [JSInternalFunction(Name = "defineProperties")]
        public static ObjectInstance DefineProperties(object obj, ObjectInstance properties)
        {
            if (!(obj is ObjectInstance))
                throw new JavaScriptException(properties.Engine, ErrorType.TypeError, "Object.defineProperties called on non-object.");
            var obj2 = (ObjectInstance)obj;
            foreach (var property in properties.Properties)
                if (property.IsEnumerable == true)
                    DefineProperty(obj2, property.Key, property.Value);
            return obj2;
        }

        /// <summary>
        /// Prevents the addition or deletion of any properties on the given object.
        /// </summary>
        /// <param name="obj"> The object to modify. </param>
        /// <returns> The object that was affected. </returns>
        [JSInternalFunction(Name = "seal")]
        public static ObjectInstance Seal(ObjectInstance obj)
        {
            var properties = new List<PropertyNameAndValue>();
            foreach (var property in obj.Properties)
                properties.Add(property);
            foreach (var property in properties)
            {
                obj.FastSetProperty(property.Key, property.Value,
                    property.Attributes & ~PropertyAttributes.Configurable, overwriteAttributes: true);
            }
            obj.IsExtensible = false;
            return obj;
        }

        /// <summary>
        /// Prevents the addition, deletion or modification of any properties on the given object.
        /// </summary>
        /// <param name="obj"> The object to modify. </param>
        /// <returns> The object that was affected. </returns>
        [JSInternalFunction(Name = "freeze")]
        public static ObjectInstance Freeze(ObjectInstance obj)
        {
            var properties = new List<PropertyNameAndValue>();
            foreach (var property in obj.Properties)
                properties.Add(property);
            foreach (var property in properties)
            {
                obj.FastSetProperty(property.Key, property.Value,
                    property.Attributes & ~(PropertyAttributes.NonEnumerable), overwriteAttributes: true);
            }
            obj.IsExtensible = false;
            return obj;
        }

        /// <summary>
        /// Prevents the addition of any properties on the given object.
        /// </summary>
        /// <param name="obj"> The object to modify. </param>
        /// <returns> The object that was affected. </returns>
        [JSInternalFunction(Name = "preventExtensions")]
        public static ObjectInstance PreventExtensions(ObjectInstance obj)
        {
            obj.IsExtensible = false;
            return obj;
        }

        /// <summary>
        /// Determines if addition or deletion of any properties on the object is allowed.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> <c>true</c> if properties can be added or at least one property can be
        /// deleted; <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "isSealed")]
        public static bool IsSealed(ObjectInstance obj)
        {
            foreach (var property in obj.Properties)
                if (property.IsConfigurable == true)
                    return false;
            return obj.IsExtensible == false;
        }

        /// <summary>
        /// Determines if addition, deletion or modification of any properties on the object is
        /// allowed.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> <c>true</c> if properties can be added or at least one property can be
        /// deleted or modified; <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "isFrozen")]
        public static bool IsFrozen(ObjectInstance obj)
        {
            foreach (var property in obj.Properties)
                if (property.IsConfigurable == true || property.IsWritable == true)
                    return false;
            return obj.IsExtensible == false;
        }

        /// <summary>
        /// Determines if addition of properties on the object is allowed.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> <c>true</c> if properties can be added to the object; <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "isExtensible")]
        public static new bool IsExtensible(ObjectInstance obj)
        {
            return obj.IsExtensible;
        }

        /// <summary>
        /// Creates an array containing the names of all the enumerable properties on the object.
        /// </summary>
        /// <param name="obj"> The object to retrieve the property names for. </param>
        /// <returns> An array containing the names of all the enumerable properties on the object. </returns>
        [JSInternalFunction(Name = "keys")]
        public static ArrayInstance Keys(ObjectInstance obj)
        {
            var result = obj.Engine.Array.New();
            foreach (var property in obj.Properties)
                if (property.IsEnumerable == true && property.Key is string)
                    result.Push(property.Key);
            return result;
        }

        /// <summary>
        /// Determines whether two values are the same value.  Note that this method considers NaN
        /// to be equal with itself and negative zero is considered different from positive zero.
        /// </summary>
        /// <param name="value1"> The first value to compare. </param>
        /// <param name="value2"> The second value to compare. </param>
        /// <returns> <c>true</c> if the values are the same.  </returns>
        [JSInternalFunction(Name = "is")]
        public static bool Is(object value1, object value2)
        {
            return TypeComparer.SameValue(value1, value2);
        }

        /// <summary>
        /// Transforms a list of key-value pairs into an object.
        /// </summary>
        /// <param name="iterable"> An iterable such as Array or Map. </param>
        /// <returns> A new object whose properties are given by the entries of the iterable. </returns>
        [JSInternalFunction(Name = "fromEntries")]
        public static ObjectInstance FromEntries(ObjectInstance iterable)
        {
            var result = iterable.Engine.Object.Construct();
            var iterator = TypeUtilities.GetIterator(iterable.Engine, iterable);
            foreach (var entry in TypeUtilities.Iterate(iterator.Engine, iterator))
            {
                if (entry is ObjectInstance entryObject)
                {
                    var propertyKey = entryObject[0];
                    if (!(propertyKey is string) && !(propertyKey is SymbolInstance))
                        propertyKey = TypeConverter.ToString(propertyKey);
                    var propertyValue = entryObject[1];
                    result[propertyKey] = propertyValue;
                }
                else
                    throw new JavaScriptException(iterable.Engine, ErrorType.TypeError, $"Iterator value {TypeConverter.ToString(entry)} is not an entry object.");
            }
            return result;
        }
    }
}
