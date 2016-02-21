using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a function that is implemented with a .NET static method.
    /// Replacement for ClrFunction.  Now used by all the built-in constructors and functions.
    /// </summary>
    [Serializable]
    public class ClrStubFunction : FunctionInstance
    {
        private string name;
        private int length;
        private ObjectInstance instancePrototype;
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
            this.callBinder = call;
        }

        /// <summary>
        /// Creates a new instance of a function which calls a .NET method.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="length"> The "typical" number of arguments expected by the function. </param>
        /// <param name="call"> The delegate to call when calling the JS method. </param>
        internal ClrStubFunction(ObjectInstance prototype, string name, int length, Func<ScriptEngine, object, object[], object> call)
            : base(prototype)
        {
            
            this.name = name;
            this.length = length;
            this.callBinder = call;

            // Add function properties.
            var properties = new List<PropertyNameAndValue>(2);
            AddFunctionProperties(properties);
            FastSetProperties(properties);
        }

        /// <summary>
        /// Creates a new instance of a function which implements a constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="length"> The "typical" number of arguments expected by the function. </param>
        /// <param name="instancePrototype"> The value of the "prototype" property. </param>
        /// <param name="call"> The delegate to call when calling the JS method. </param>
        /// <param name="construct"> The delegate to call when calling the JS method as a constructor. </param>
        internal ClrStubFunction(ObjectInstance prototype, string name, int length, ObjectInstance instancePrototype,
            Func<ScriptEngine, object, object[], object> call, Func<ScriptEngine, object, object[], ObjectInstance> construct)
            : base(prototype)
        {
            this.name = name;
            this.length = length;
            this.instancePrototype = instancePrototype;
            this.callBinder = call;
            this.constructBinder = construct;
            //this.FastSetProperty("name", name, PropertyAttributes.Configurable);
            //this.FastSetProperty("length", 1, PropertyAttributes.Configurable);
            //this.FastSetProperty("prototype", instancePrototype, PropertyAttributes.Sealed);
            //instancePrototype.FastSetProperty("constructor", this, PropertyAttributes.NonEnumerable);
        }

        /// <summary>
        /// Adds properties needed by the function to the list of properties.
        /// </summary>
        /// <param name="properties"> The list of properties to add to. </param>
        protected void AddFunctionProperties(List<PropertyNameAndValue> properties)
        {
            properties.Add(new PropertyNameAndValue("name", this.name, PropertyAttributes.Configurable));
            properties.Add(new PropertyNameAndValue("length", this.length, PropertyAttributes.Configurable));
            if (this.instancePrototype != null)
                properties.Add(new PropertyNameAndValue("prototype", this.instancePrototype, PropertyAttributes.Sealed));
        }



        //     PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the prototype of objects constructed using this function.  Equivalent to
        /// the Function.prototype property.
        /// </summary>
        public new ObjectInstance InstancePrototype
        {
            get { return this.instancePrototype; }
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
