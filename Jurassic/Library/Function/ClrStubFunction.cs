using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a function that is implemented with a .NET static method.
    /// Faster, but less flexible version of ClrFunction.  Now used by all the built-in constructors and functions.
    /// </summary>
    [Serializable]
    public class ClrStubFunction : FunctionInstance
    {
        private Func<ScriptEngine, object, object[], object> callBinder;
        private Func<ScriptEngine, object, object[], ObjectInstance> constructBinder;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a function which calls a .NET method, with no name or length.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="call"> The delegate to call when calling the JS method. </param>
        internal ClrStubFunction(ObjectInstance prototype, Func<ScriptEngine, object, object[], object> call)
            : base(prototype)
        {
            // Let's not even bother with setting the name and length!
            // Use sparingly.
            this.callBinder = call;
        }

        /// <summary>
        /// Creates a new instance of a function which calls a .NET method.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="length"> The "typical" number of arguments expected by the function. </param>
        /// <param name="call"> The delegate to call when calling the JS method. </param>
        public ClrStubFunction(ObjectInstance prototype, string name, int length, Func<ScriptEngine, object, object[], object> call)
            : base(prototype)
        {
            this.callBinder = call;

            // Set name and length properties.
            var properties = new List<PropertyNameAndValue>(2);
            properties.Add(new PropertyNameAndValue("name", name, PropertyAttributes.Configurable));
            properties.Add(new PropertyNameAndValue("length", length, PropertyAttributes.Configurable));
            FastSetProperties(properties);
        }

        /// <summary>
        /// Creates a new constructor function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="construct"> The delegate to call when calling the JS method as a constructor. </param>
        /// <param name="call"> The delegate to call when function is called. </param>
        internal ClrStubFunction(ObjectInstance prototype,
            Func<ScriptEngine, object, object[], ObjectInstance> construct,
            Func<ScriptEngine, object, object[], object> call)
            : base(prototype)
        {
            this.callBinder = call;
            this.constructBinder = construct;
        }

        /// <summary>
        /// Adds properties needed by the function to the list of properties.
        /// </summary>
        /// <param name="properties"> The list of properties to add to. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="length"> The "typical" number of arguments expected by the function. </param>
        /// <param name="instancePrototype"> The value of the "prototype" property. </param>
        protected void InitializeConstructorProperties(List<PropertyNameAndValue> properties, string name, int length, ObjectInstance instancePrototype)
        {
            properties.Add(new PropertyNameAndValue("name", name, PropertyAttributes.Configurable));
            properties.Add(new PropertyNameAndValue("length", length, PropertyAttributes.Configurable));
            properties.Add(new PropertyNameAndValue("prototype", instancePrototype, PropertyAttributes.Sealed));
        }

        /// <summary>
        /// Creates a new constructor function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="length"> The "typical" number of arguments expected by the function. </param>
        /// <param name="instancePrototype"> The value of the "prototype" property. </param>
        /// <param name="construct"> The delegate to call when calling the JS method as a constructor. </param>
        /// <param name="call"> The delegate to call when function is called. </param>
        protected ClrStubFunction(ObjectInstance prototype,
            string name, int length, ObjectInstance instancePrototype,
            Func<ScriptEngine, object, object[], ObjectInstance> construct,
            Func<ScriptEngine, object, object[], object> call)
            : base(prototype)
        {
            this.callBinder = call;
            this.constructBinder = construct;

            var properties = new List<PropertyNameAndValue>(3);
            InitializeConstructorProperties(properties, name, length, instancePrototype);
            FastSetProperties(properties);
        }



        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The value that was returned from the function. </returns>
        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            if (this.Engine.CompatibilityMode == CompatibilityMode.ECMAScript3)
            {
                // Convert null or undefined to the global object.
                if (TypeUtilities.IsUndefined(thisObject) == true || thisObject == Null.Value)
                    thisObject = this.Engine.Global;
                else
                    thisObject = TypeConverter.ToObject(this.Engine, thisObject);
            }
            try
            {
                return this.callBinder(this.Engine, constructBinder != null ? this : thisObject, argumentValues);
            }
            catch (JavaScriptException ex)
            {
                if (ex.FunctionName == null && ex.SourcePath == null && ex.LineNumber == 0)
                {
                    ex.FunctionName = this.DisplayName;
                    ex.SourcePath = "native";
                    ex.PopulateStackTrace();
                }
                throw;
            }
        }

        /// <summary>
        /// Creates an object, using this function as the constructor.
        /// </summary>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The object that was created. </returns>
        public override ObjectInstance ConstructLateBound(params object[] argumentValues)
        {
            if (this.constructBinder == null)
                throw new JavaScriptException(this.Engine, "TypeError", "Objects cannot be constructed from built-in functions.");
            return (ObjectInstance)this.constructBinder(this.Engine, this, argumentValues);
        }
    }
}
