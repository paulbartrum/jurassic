using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a JavaScript function.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public abstract partial class FunctionInstance : ObjectInstance
    {
        // Used to speed up access to the prototype property.
        private PropertyReference instancePrototypeProperty = new PropertyReference("prototype");


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new instance of a built-in function object, with the default Function
        /// prototype.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        protected FunctionInstance(ScriptEngine engine)
            : base(engine, engine.Function.InstancePrototype)
        {
        }

        /// <summary>
        /// Creates a new instance of a built-in function object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        protected FunctionInstance(ObjectInstance prototype)
            : base(prototype)
        {
        }

        /// <summary>
        /// Creates a new instance of a built-in function object.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="prototype"> The next object in the prototype chain.  Can be <c>null</c>. </param>
        protected FunctionInstance(ScriptEngine engine, ObjectInstance prototype)
            : base(engine, prototype)
        {
        }

        /// <summary>
        /// Initializes the prototype properties.
        /// </summary>
        /// <param name="obj"> The object to set the properties on. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static void InitializePrototypeProperties(ObjectInstance obj, FunctionConstructor constructor)
        {
            var engine = obj.Engine;
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue("name", "Empty", PropertyAttributes.Configurable));
            properties.Add(new PropertyNameAndValue("length", 0, PropertyAttributes.Configurable));
            obj.InitializeProperties(properties);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the prototype of objects constructed using this function.  Equivalent to
        /// the Function.prototype property.
        /// </summary>
        public ObjectInstance InstancePrototype
        {
            get
            {
                // See 13.2.2

                // Retrieve the value of the prototype property.
                ObjectInstance prototype = GetPropertyValue(instancePrototypeProperty) as ObjectInstance;
                
                // If the prototype property is not set to an object, use the Object prototype property instead.
                if (prototype == null && this != this.Engine.Object)
                    return this.Engine.Object.InstancePrototype;

                return prototype;
            }
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name
        {
            get { return TypeConverter.ToString(this["name"]); }
        }

        /// <summary>
        /// Gets the number of arguments expected by the function.
        /// </summary>
        public int Length
        {
            get { return TypeConverter.ToInteger(this["length"]); }
            protected set { this.FastSetProperty("length", value); }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get
            {
                string name = this.Name;
                if (string.IsNullOrEmpty(name))
                    name = "function";
                string result = string.Format("{0}()", name);
                return result;
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayValue; }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayType
        {
            get { return "Function"; }
        }


        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Determines whether the given object inherits from this function.  More precisely, it
        /// checks whether the prototype chain of the object contains the prototype property of
        /// this function.  Used by the "instanceof" operator.
        /// </summary>
        /// <param name="instance"> The instance to check. </param>
        /// <returns> <c>true</c> if the object inherits from this function; <c>false</c>
        /// otherwise. </returns>
        public virtual bool HasInstance(object instance)
        {
            if ((instance is ObjectInstance) == false)
                return false;
            object functionPrototype = this["prototype"];
            if ((functionPrototype is ObjectInstance) == false)
                throw new JavaScriptException(this.Engine, ErrorType.TypeError, "Function has non-object prototype in instanceof check");
            var instancePrototype = ((ObjectInstance)instance).Prototype;
            while (instancePrototype != null)
            {
                if (instancePrototype == functionPrototype)
                    return true;
                instancePrototype = instancePrototype.Prototype;
            }
            return false;
        }

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The value that was returned from the function. </returns>
        public abstract object CallLateBound(object thisObject, params object[] argumentValues);

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="function"> The name of the caller function. </param>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The value that was returned from the function. </returns>
        internal object CallFromNative(string function, object thisObject, params object[] argumentValues)
        {
            this.Engine.PushStackFrame("native", function, 0, ScriptEngine.CallType.MethodCall);
            try
            {
                return CallLateBound(thisObject, argumentValues);
            }
            finally
            {
                this.Engine.PopStackFrame();
            }
        }

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="path"> The path of the javascript source file that contains the caller. </param>
        /// <param name="function"> The name of the caller function. </param>
        /// <param name="line"> The line number of the statement that is calling this function. </param>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The value that was returned from the function. </returns>
        public object CallWithStackTrace(string path, string function, int line, object thisObject, object[] argumentValues)
        {
            this.Engine.PushStackFrame(path, function, line, ScriptEngine.CallType.MethodCall);
            try
            {
                return CallLateBound(thisObject, argumentValues);
            }
            finally
            {
                this.Engine.PopStackFrame();
            }
        }

        /// <summary>
        /// Creates an object, using this function as the constructor.
        /// </summary>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The object that was created. </returns>
        public virtual ObjectInstance ConstructLateBound(params object[] argumentValues)
        {
            // Create a new object and set the prototype to the instance prototype of the function.
            var newObject = ObjectInstance.CreateRawObject(this.InstancePrototype);

            // Run the function, with the new object as the "this" keyword.
            var result = CallLateBound(newObject, argumentValues);

            // Return the result of the function if it is an object.
            if (result is ObjectInstance)
                return (ObjectInstance)result;

            // Otherwise, return the new object.
            return newObject;
        }

        /// <summary>
        /// Creates an object, using this function as the constructor.
        /// </summary>
        /// <param name="path"> The path of the javascript source file that contains the caller. </param>
        /// <param name="function"> The name of the caller function. </param>
        /// <param name="line"> The line number of the statement that is calling this function. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The object that was created. </returns>
        public ObjectInstance ConstructWithStackTrace(string path, string function, int line, object[] argumentValues)
        {
            this.Engine.PushStackFrame(path, function, line, ScriptEngine.CallType.NewOperator);
            try
            {
                return ConstructLateBound(argumentValues);
            }
            finally
            {
                this.Engine.PopStackFrame();
            }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls the function, passing in parameters from the given array.
        /// </summary>
        /// <param name="thisObj"> The value of <c>this</c> in the context of the function. </param>
        /// <param name="arguments"> The arguments passed to the function, as an array. </param>
        /// <returns> The result from the function call. </returns>
        [JSInternalFunction(Name = "apply")]
        public object Apply(object thisObj, object arguments)
        {
            // Convert the arguments parameter into an array.
            object[] argumentsArray;
            if (arguments == null || arguments == Undefined.Value || arguments == Null.Value)
                argumentsArray = new object[0];
            else
            {
                if ((arguments is ObjectInstance) == false)
                    throw new JavaScriptException(this.Engine, ErrorType.TypeError, "The second parameter of apply() must be an array or an array-like object.");
                ObjectInstance argumentsObject = (ObjectInstance)arguments;
                object arrayLengthObj = argumentsObject["length"];
                if (arrayLengthObj == null || arrayLengthObj == Undefined.Value || arrayLengthObj == Null.Value)
                    throw new JavaScriptException(this.Engine, ErrorType.TypeError, "The second parameter of apply() must be an array or an array-like object.");
                uint arrayLength = TypeConverter.ToUint32(arrayLengthObj);
                if (arrayLength != TypeConverter.ToNumber(arrayLengthObj))
                    throw new JavaScriptException(this.Engine, ErrorType.TypeError, "The second parameter of apply() must be an array or an array-like object.");
                argumentsArray = new object[arrayLength];
                for (uint i = 0; i < arrayLength; i++)
                    argumentsArray[i] = argumentsObject[i];
            }

            return this.CallLateBound(thisObj, argumentsArray);
        }

        /// <summary>
        /// Calls the function.
        /// </summary>
        /// <param name="thisObj"> The value of <c>this</c> in the context of the function. </param>
        /// <param name="arguments"> Any number of arguments that will be passed to the function. </param>
        /// <returns> The result from the function call. </returns>
        [JSInternalFunction(Name = "call", Length = 1)]
        public object Call(object thisObj, params object[] arguments)
        {
            return this.CallLateBound(thisObj, arguments);
        }

        /// <summary>
        /// Creates a new function that, when called, calls this function with the given "this"
        /// value and, optionally, one or more more arguments.
        /// </summary>
        /// <param name="boundThis"> The fixed value of "this". </param>
        /// <param name="boundArguments"> Any number of fixed arguments values. </param>
        /// <returns> A new function. </returns>
        [JSInternalFunction(Name = "bind", Length = 1)]
        public FunctionInstance Bind(object boundThis, params object[] boundArguments)
        {
            return new BoundFunction(this, boundThis, boundArguments);
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        [JSInternalFunction(Name = "toString")]
        public string ToStringJS()
        {
            return this.ToString();
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            return string.Format("function {0}() {{ [native code] }}", this.Name);
        }
    }
}
