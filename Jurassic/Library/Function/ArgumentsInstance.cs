using System;
using System.Collections.Generic;
using System.Linq;
using Jurassic.Compiler;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an arguments object.
    /// </summary>
    public class ArgumentsInstance : ObjectInstance
    {
        private UserDefinedFunction callee;
        private DeclarativeScope scope;
        private bool[] mappedArguments;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Arguments instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="callee"> The function that was called. </param>
        /// <param name="scope"> The function scope. </param>
        /// <param name="argumentValues"> The argument values that were passed to the function. </param>
        internal ArgumentsInstance(ObjectInstance prototype, UserDefinedFunction callee, DeclarativeScope scope, object[] argumentValues)
            : base(prototype)
        {
            if (callee == null)
                throw new ArgumentNullException("callee");
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (argumentValues == null)
                throw new ArgumentNullException("argumentValues");
            this.callee = callee;
            this.scope = scope;
            this.FastSetProperty("length", argumentValues.Length, PropertyAttributes.NonEnumerable);
            this.FastSetProperty("callee", callee, PropertyAttributes.NonEnumerable);

            // Create an array mappedArguments where mappedArguments[i] = true means a mapping is
            // maintained between arguments[i] and the corresponding variable.
            this.mappedArguments = new bool[argumentValues.Length];
            var mappedNames = new HashSet<string>();
            for (int i = argumentValues.Length - 1; i >= 0; i--)
            {
                this[(uint)i] = argumentValues[i];    // Note: this.mappedArguments[i] is currently false.
                if (i < callee.ArgumentNames.Count)
                {
                    // The argument name is mapped if it hasn't been seen before.
                    this.mappedArguments[i] = mappedNames.Add(callee.ArgumentNames[i]);
                    if (this.mappedArguments[i] == true)
                    {
                        var getter = new UserDefinedFunction(GlobalObject.Function.InstancePrototype, "ArgumentGetter", new string[0], this.scope, ArgumentGetter);
                        getter.SetPropertyValue("argumentIndex", i, false);
                        var setter = new UserDefinedFunction(GlobalObject.Function.InstancePrototype, "ArgumentSetter", new string[0], this.scope, ArgumentSetter);
                        setter.SetPropertyValue("argumentIndex", i, false);
                        this.DefineProperty(i.ToString(), new PropertyDescriptor(getter, setter, PropertyAttributes.FullAccess), false);
                    }
                }
            }

            // In strict mode, accessing caller or callee is illegal.
            if (this.callee.StrictMode == true)
            {
                var throwErrorFunction = new ThrowTypeErrorFunction(GlobalObject.Function.InstancePrototype);
                this.DefineProperty("caller", new PropertyDescriptor(throwErrorFunction, throwErrorFunction, PropertyAttributes.Sealed), false);
                this.DefineProperty("callee", new PropertyDescriptor(throwErrorFunction, throwErrorFunction, PropertyAttributes.Sealed), false);
            }
        }



        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "Arguments"; }
        }

        /// <summary>
        /// Used to retrieve the value of an argument.
        /// </summary>
        /// <param name="scope"> The scope (global or eval context) or the parent scope (function
        /// context). </param>
        /// <param name="thisObject"> The value of the <c>this</c> keyword. </param>
        /// <param name="functionObject"> The function object. </param>
        /// <param name="arguments"> The arguments that were passed to the function. </param>
        /// <returns> The result of calling the method. </returns>
        private object ArgumentGetter(Compiler.Scope scope, object thisObject, Library.FunctionInstance functionObject, object[] argumentValues)
        {
            int argumentIndex = TypeConverter.ToInteger(functionObject.GetPropertyValue("argumentIndex"));
            return this.scope.GetValue(this.callee.ArgumentNames[argumentIndex]);
        }

        /// <summary>
        /// Used to set the value of an argument.
        /// </summary>
        /// <param name="scope"> The scope (global or eval context) or the parent scope (function
        /// context). </param>
        /// <param name="thisObject"> The value of the <c>this</c> keyword. </param>
        /// <param name="functionObject"> The function object. </param>
        /// <param name="arguments"> The arguments that were passed to the function. </param>
        /// <returns> The result of calling the method. </returns>
        private object ArgumentSetter(Compiler.Scope scope, object thisObject, Library.FunctionInstance functionObject, object[] argumentValues)
        {
            int argumentIndex = TypeConverter.ToInteger(functionObject.GetPropertyValue("argumentIndex"));
            if (argumentValues != null && argumentValues.Length >= 1)
            {
                object value = argumentValues[0];
                this.scope.SetValue(this.callee.ArgumentNames[argumentIndex], value);
            }
            return Undefined.Value;
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
        public override bool Delete(uint index, bool throwOnError)
        {
            // Break the mapping between the array element of this object and the corresponding
            // variable name.
            if (index < this.mappedArguments.Length && this.mappedArguments[index] == true)
            {
                this.mappedArguments[index] = false;
                var currentValue = this.scope.GetValue(this.callee.ArgumentNames[(int)index]);
                DefineProperty(index.ToString(), new PropertyDescriptor(currentValue, PropertyAttributes.FullAccess), false);
            }

            // Delegate to the base class implementation.
            return base.Delete(index, throwOnError);
        }
    }
}
