using System;

namespace Jurassic.Library
{
    /// <summary>
    /// The Set object lets you store unique values of any type, whether primitive values or object references.
    /// </summary>
    public partial class ProxyConstructor : ClrStubFunction
    {
        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new set constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal ProxyConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "Proxy", 2, null);
            InitializeProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the typed array object is invoked like a function, e.g. Proxy().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(ErrorType.TypeError, $"Constructor Proxy requires 'new'");
        }

        /// <summary>
        /// Creates a proxy object.
        /// </summary>
        /// <param name="target"> A target object to wrap with Proxy. It can be any sort of object, including a native array, a function, or even another proxy. </param>
        /// <param name="handler"> An object whose properties are functions that define the behavior of the proxy when an operation is performed on it. </param>
        /// <returns> A new proxy object. </returns>
        [JSConstructorFunction]
        public ObjectInstance Construct(object target, object handler)
        {
            if (target is ObjectInstance targetObject && handler is ObjectInstance handlerObject)
            {
                if (target is FunctionInstance targetFunction)
                    return new ProxyFunction(Engine, targetFunction, handlerObject);
                return new ProxyInstance(Engine, targetObject, handlerObject);
            }
            throw new JavaScriptException(ErrorType.TypeError, "Cannot create proxy with a non-object as target or handler.");
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a revocable proxy object.
        /// </summary>
        /// <param name="target"> A target object to wrap with Proxy. It can be any sort of object, including a native array, a function, or even another proxy. </param>
        /// <param name="handler"> An object whose properties are functions that define the behavior of the proxy when an operation is performed on it. </param>
        /// <returns> A new proxy object. </returns>
        [JSInternalFunction(Name = "revocable")]
        public static ProxyInstance Revocable(object target, object handler)
        {
            throw new NotSupportedException();
        }
    }
}
