using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript Array object.
    /// </summary>
    public partial class ArrayConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Array object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal ArrayConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "Array", 1, ArrayInstance.CreatePrototype(Engine, this));
            InitializeProperties(properties);
        }


        /// <summary>
        /// Creates a new Array instance.
        /// </summary>
        public ArrayInstance New()
        {
            return new ArrayInstance(this.InstancePrototype, 0, 10);
        }

        /// <summary>
        /// Creates a new Array instance.
        /// </summary>
        /// <param name="elements"> The initial elements of the new array. </param>
        public ArrayInstance New(object[] elements)
        {
            // Copy the array if it is not an object array (for example, if it is a string[]).
            if (elements.GetType() != typeof(object[]))
            {
                var temp = new object[elements.Length];
                Array.Copy(elements, temp, elements.Length);
                return new ArrayInstance(this.InstancePrototype, temp);
            }

            return new ArrayInstance(this.InstancePrototype, elements);
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// A reference to the constructor function that is used to create derived objects.
        /// </summary>
        [JSProperty(Name = "@@species")]
        public FunctionInstance Species
        {
            get { return this; }
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Array instance and initializes the contents of the array.
        /// Called when the Array object is invoked like a function, e.g. var x = Array(length).
        /// </summary>
        /// <param name="elements"> The initial elements of the new array. </param>
        [JSCallFunction]
        public ArrayInstance Call(params object[] elements)
        {
            return Construct(elements);
        }

        /// <summary>
        /// Creates a new Array instance and initializes the contents of the array.
        /// Called when the new expression is used on this object, e.g. var x = new Array(length).
        /// </summary>
        /// <param name="elements"> The initial elements of the new array. </param>
        [JSConstructorFunction]
        public ArrayInstance Construct(params object[] elements)
        {
            if (elements.Length == 1)
            {
                if (TypeUtilities.IsNumeric(elements[0]))
                {
                    double specifiedLength = TypeConverter.ToNumber(elements[0]);
                    uint actualLength = TypeConverter.ToUint32(elements[0]);
                    if (specifiedLength != (double)actualLength)
                        throw new JavaScriptException(ErrorType.RangeError, "Invalid array length");
                    return new ArrayInstance(this.InstancePrototype, actualLength, actualLength);
                }
            }

            // Transform any nulls into undefined.
            for (int i = 0; i < elements.Length; i++)
                if (elements[i] == null)
                    elements[i] = Undefined.Value;

            return New(elements);
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Tests if the given value is an Array instance.
        /// </summary>
        /// <param name="value"> The value to test. </param>
        /// <returns> <c>true</c> if the given value is an Array instance, <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "isArray")]
        public static bool IsArray(object value)
        {
            return value is ArrayInstance;
        }

        /// <summary>
        /// Creates a new Array instance from a variable number of arguments.
        /// 
        /// The difference between Array.of() and the Array constructor is in the handling of
        /// integer arguments: Array.of(7) creates an array with a single element, 7, whereas
        /// Array(7) creates an empty array with a length property of 7.
        /// </summary>
        /// <param name="engine"> The script engine to use. </param>
        /// <param name="elements"> The elements of the new array. </param>
        [JSInternalFunction(Name = "of", Flags = JSFunctionFlags.HasEngineParameter, Length = 0)]
        public static ArrayInstance Of(ScriptEngine engine, params object[] elements)
        {
            return engine.Array.New(elements);
        }

        /// <summary>
        /// The Array.from() method creates a new, shallow-copied Array instance from an array-like
        /// or iterable object.
        /// </summary>
        /// <param name="engine"> The script engine to use. </param>
        /// <param name="iterable"> An array-like or iterable object to convert to an array. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "from", Flags = JSFunctionFlags.HasEngineParameter, Length = 1, RequiredArgumentCount = 1)]
        public static ArrayInstance From(ScriptEngine engine, ObjectInstance iterable)
        {
            return From(engine, iterable, null, null);
        }

        /// <summary>
        /// The Array.from() method creates a new, shallow-copied Array instance from an array-like
        /// or iterable object.
        /// </summary>
        /// <param name="engine"> The script engine to use. </param>
        /// <param name="iterable"> An array-like or iterable object to convert to an array. </param>
        /// <param name="mapFunction"> Map function to call on every element of the array. </param>
        /// <param name="thisArg"> Value to use as <c>this</c> when executing <paramref name="mapFunction"/>. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "from", Flags = JSFunctionFlags.HasEngineParameter, Length = 1, RequiredArgumentCount = 1)]
        public static ArrayInstance From(ScriptEngine engine, ObjectInstance iterable, FunctionInstance mapFunction, object thisArg)
        {
            var result = new List<object>();
            var iterator = TypeUtilities.GetIterator(engine, iterable);
            if (iterator != null)
            {
                // Initialize the array from an iterator.
                int index = 0;
                foreach (var item in TypeUtilities.Iterate(engine, iterator))
                {
                    object mappedValue = mapFunction?.Call(thisArg ?? Undefined.Value, item, index) ?? item;
                    result.Add(mappedValue);
                    index++;
                }
            }
            else
            {
                // Initialize the array from an array-like object.
                uint length = ArrayInstance.GetLength(iterable);
                for (int i = 0; i < length; i++)
                {
                    object mappedValue = mapFunction?.Call(thisArg ?? Undefined.Value, iterable[i], i) ?? iterable[i];
                    result.Add(mappedValue);
                }
            }
            return engine.Array.New(result.ToArray());
        }
    }
}
