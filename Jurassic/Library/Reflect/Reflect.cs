using System.Diagnostics;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in Reflect class that provides methods for interceptable JavaScript
    /// operations.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public partial class ReflectObject : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Reflect object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal ReflectObject(ObjectInstance prototype)
            : base(prototype)
        {
            var properties = GetDeclarativeProperties(Engine);
            properties.Add(new PropertyNameAndValue(Symbol.ToStringTag, "Reflect", PropertyAttributes.Configurable));
            InitializeProperties(properties);
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls a target function with arguments as specified by the argumentsList parameter.
        /// See also Function.prototype.apply().
        /// </summary>
        /// <param name="target"> The target function to call. </param>
        /// <param name="thisArgument"> The value of this provided for the call to target. </param>
        /// <param name="argumentsList"> An array-like object specifying the arguments with which target should be called. </param>
        /// <returns> The result of calling the given target function with the specified this value and arguments. </returns>
        [JSInternalFunction(Name = "apply")]
        public static object Apply(FunctionInstance target, object thisArgument, ObjectInstance argumentsList)
        {
            return target.CallLateBound(thisArgument, TypeUtilities.CreateListFromArrayLike(argumentsList));
        }

        /// <summary>
        /// The new operator as a function. Equivalent to calling new target(...argumentsList). Also provides the option to specify a different prototype.
        /// </summary>
        /// <param name="target"> The target function to call. </param>
        /// <param name="argumentsList"> An array-like object specifying the arguments with which target should be called. </param>
        /// <param name="newTarget"> The constructor whose prototype should be used. See also the new.target operator. If newTarget is not present, its value defaults to target. </param>
        /// <returns> A new instance of target (or newTarget, if present), initialized by target as a constructor with the given argumentsList. </returns>
        [JSInternalFunction(Name = "construct", Length = 2)]
        public static object Construct(FunctionInstance target, ObjectInstance argumentsList, FunctionInstance newTarget = null)
        {
            if (newTarget == null)
                newTarget = target;
            return target.ConstructLateBound(newTarget, TypeUtilities.CreateListFromArrayLike(argumentsList));
        }

        /// <summary>
        /// Similar to Object.defineProperty(). Returns a Boolean that is true if the property was successfully defined.
        /// </summary>
        /// <param name="target"> The target object on which to define the property. </param>
        /// <param name="propertyKey"> The name of the property to be defined or modified. </param>
        /// <param name="attributes"> The attributes for the property being defined or modified. </param>
        /// <returns> A Boolean indicating whether or not the property was successfully defined. </returns>
        [JSInternalFunction(Name = "defineProperty")]
        public static bool DefineProperty(ObjectInstance target, object propertyKey, object attributes)
        {
            propertyKey = TypeConverter.ToPropertyKey(propertyKey);
            var defaults = target.GetOwnPropertyDescriptor(propertyKey);
            if (!(attributes is ObjectInstance))
                throw new JavaScriptException(ErrorType.TypeError, $"Invalid property descriptor '{attributes}'.");
            var descriptor = PropertyDescriptor.FromObject((ObjectInstance)attributes, defaults);
            return target.DefineProperty(propertyKey, descriptor, throwOnError: false);
        }

        /// <summary>
        /// The delete operator as a function. Equivalent to calling delete target[propertyKey].
        /// </summary>
        /// <param name="target"> The target object on which to delete the property. </param>
        /// <param name="propertyKey"> The name of the property to be deleted. </param>
        /// <returns> A Boolean indicating whether or not the property was successfully deleted. </returns>
        [JSInternalFunction(Name = "deleteProperty")]
        public static bool DeleteProperty(ObjectInstance target, object propertyKey)
        {
            propertyKey = TypeConverter.ToPropertyKey(propertyKey);
            return target.Delete(propertyKey, throwOnError: false);
        }

        /// <summary>
        /// Returns the value of the property. Works like getting a property from an object (target[propertyKey]) as a function.
        /// </summary>
        /// <param name="target"> The target object on which to get the property. </param>
        /// <param name="propertyKey"> The name of the property to get. </param>
        /// <param name="receiver"> The value of this provided for the call to target if a getter
        /// is encountered. When used with Proxy, it can be an object that inherits from target. </param>
        /// <returns> The value of the property. </returns>
        [JSInternalFunction(Name = "get", Length = 2)]
        public static object Get(ObjectInstance target, object propertyKey, object receiver = null)
        {
            propertyKey = TypeConverter.ToPropertyKey(propertyKey);
            if (receiver == null)
                receiver = target;
            object result = target.GetPropertyValue(propertyKey, TypeConverter.ToObject(target.Engine, receiver));
            return result == null ? Undefined.Value : result;
        }

        /// <summary>
        /// Similar to Object.getOwnPropertyDescriptor(). Returns a property descriptor of the given property if it exists on the object,  undefined otherwise.
        /// </summary>
        /// <param name="target"> The target object in which to look for the property. </param>
        /// <param name="propertyKey"> The name of the property to get an own property descriptor for. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "getOwnPropertyDescriptor")]
        public static ObjectInstance GetOwnPropertyDescriptor(ObjectInstance target, object propertyKey)
        {
            return ObjectConstructor.GetOwnPropertyDescriptor(target, propertyKey);
        }

        /// <summary>
        /// Same as Object.getPrototypeOf().
        /// </summary>
        /// <param name="target"> The target object of which to get the prototype. </param>
        /// <returns> The prototype of the given object. If there are no inherited properties, null is returned. </returns>
        [JSInternalFunction(Name = "getPrototypeOf")]
        public static object GetPrototypeOf(ObjectInstance target)
        {
            return ObjectConstructor.GetPrototypeOf(target);
        }

        /// <summary>
        /// Returns a Boolean indicating whether the target has the property. Either as own or
        /// inherited. Works like the in operator as a function.
        /// </summary>
        /// <param name="target"> The target object in which to look for the property. </param>
        /// <param name="propertyKey"> The name of the property to check. </param>
        /// <returns> A Boolean indicating whether or not the target has the property. </returns>
        [JSInternalFunction(Name = "has")]
        public static bool Has(object target, object propertyKey)
        {
            if (target is ObjectInstance targetObjectInstance)
            {
                propertyKey = TypeConverter.ToPropertyKey(propertyKey);
                return targetObjectInstance.HasProperty(propertyKey);
            }
            throw new JavaScriptException(ErrorType.TypeError, "Reflect.has called with non-object.");
        }

        /// <summary>
        /// Same as Object.isExtensible(). Returns a Boolean that is true if the target is extensible.
        /// </summary>
        /// <param name="target"> The target object which to check if it is extensible. </param>
        /// <returns> A Boolean indicating whether or not the target is extensible. </returns>
        [JSInternalFunction(Name = "isExtensible")]
        public static new bool IsExtensible(object target)
        {
            if (target is ObjectInstance targetObjectInstance)
                return targetObjectInstance.IsExtensible;
            throw new JavaScriptException(ErrorType.TypeError, "Reflect.isExtensible called with non-object.");
        }

        /// <summary>
        /// Returns an array of the target object's own (not inherited) property keys.
        /// </summary>
        /// <param name="target"> The target object from which to get the own keys. </param>
        /// <returns> An Array of the target object's own property keys. </returns>
        [JSInternalFunction(Name = "ownKeys")]
        public static ArrayInstance OwnKeys(ObjectInstance target)
        {
            // Indexes should be in numeric order.
            var indexes = new List<uint>();
            foreach (var property in target.Properties)
                if (property.Key is string key)
                {
                    uint arrayIndex = ArrayInstance.ParseArrayIndex(key);
                    if (arrayIndex != uint.MaxValue)
                        indexes.Add(arrayIndex);
                }
            indexes.Sort();
            var result = target.Engine.Array.New();
            foreach (uint index in indexes)
                result.Push(index.ToString());

            // Strings, in insertion order.
            foreach (var property in target.Properties)
                if (property.Key is string key)
                {
                    uint arrayIndex = ArrayInstance.ParseArrayIndex(key);
                    if (arrayIndex == uint.MaxValue)
                        result.Push(property.Key);
                }

            // Symbols, in insertion order.
            foreach (var property in target.Properties)
                if (property.Key is Symbol)
                    result.Push(property.Key);

            return result;
        }

        /// <summary>
        /// Similar to Object.preventExtensions().
        /// </summary>
        /// <param name="target"> The target object on which to prevent extensions. </param>
        /// <returns> A Boolean indicating whether or not the target was successfully set to prevent extensions. </returns>
        [JSInternalFunction(Name = "preventExtensions")]
        public static bool PreventExtensions(object target)
        {
            if (target is ObjectInstance targetObjectInstance)
            {
                targetObjectInstance.IsExtensible = false;
                return true;
            }
            throw new JavaScriptException(ErrorType.TypeError, "Reflect.preventExtensions called with non-object.");
        }

        /// <summary>
        /// A function that assigns values to properties. Returns a Boolean that is true if the update was successful.
        /// </summary>
        /// <param name="target"> The target object on which to set the property. </param>
        /// <param name="propertyKey"> The name of the property to set. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="receiver"> The value of this provided for the call to target if a setter is encountered. </param>
        /// <returns> A Boolean indicating whether or not setting the property was successful. </returns>
        [JSInternalFunction(Name = "set", Length = 3)]
        public static bool Set(ObjectInstance target, object propertyKey, object value, object receiver)
        {
            propertyKey = TypeConverter.ToPropertyKey(propertyKey);
            return target.SetPropertyValue(propertyKey, value, throwOnError: false);
        }

        /// <summary>
        /// A function that sets the prototype of an object. Returns a Boolean that is true if the update was successful.
        /// </summary>
        /// <param name="target"> The target object of which to set the prototype. </param>
        /// <param name="prototype"> The object's new prototype (an object or null). </param>
        /// <returns> A Boolean indicating whether or not the prototype was successfully set. </returns>
        [JSInternalFunction(Name = "setPrototypeOf")]
        public static bool SetPrototypeOf(ObjectInstance target, object prototype)
        {
            // The prototype must be null or an object. Note that null in .NET is actually undefined in JS!
            var prototypeObj = prototype as ObjectInstance;
            if (prototypeObj == null && prototype != Null.Value)
                throw new JavaScriptException(ErrorType.TypeError, "Object prototype may only be an Object or null.");
            return target.SetPrototype(prototypeObj);
        }
    }
}
