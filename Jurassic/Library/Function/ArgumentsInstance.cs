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
            this.SetProperty("length", argumentValues.Length, PropertyAttributes.NonEnumerable);
            this.SetProperty("callee", callee, PropertyAttributes.NonEnumerable);

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
                }
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
        /// Gets the property descriptor for the property with the given array index.  The
        /// prototype chain is not searched.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes.  The
        /// result will be <c>PropertyDescriptor.Undefined</c> if the property doesn't exist. </returns>
        internal override PropertyDescriptor GetOwnProperty(uint index)
        {
            // Maintain a mapping between the array elements of this object and the corresponding
            // variable names.
            if (index < this.mappedArguments.Length && this.mappedArguments[index] == true)
            {
                if (this.scope.HasValue(this.callee.ArgumentNames[(int)index]) == false)
                    return PropertyDescriptor.Undefined;
                object value = this.scope.GetValue(this.callee.ArgumentNames[(int)index]);
                return new PropertyDescriptor(value, PropertyAttributes.NonEnumerable);
            }

            // Delegate to the base class implementation.
            return base.GetOwnProperty(index);
        }

        /// <summary>
        /// Gets the property descriptor for the property with the given name.  The prototype
        /// chain is not searched.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes.  The
        /// result will be <c>PropertyDescriptor.Undefined</c> if the property doesn't exist. </returns>
        internal override PropertyDescriptor GetOwnProperty(string propertyName)
        {
            // Check if the property name is an array index.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(propertyName);
            if (arrayIndex != uint.MaxValue && arrayIndex < this.mappedArguments.Length && this.mappedArguments[arrayIndex] == true)
                return GetOwnProperty(arrayIndex);

            // "callee" and "caller" should throw TypeErrors in strict mode.
            if (this.callee.StrictMode == true && (propertyName == "callee" || propertyName == "callee"))
                throw new JavaScriptException("TypeError", "It is illegal to access arguments.callee or arguments.caller in strict mode");
            
            // Delegate to the base class implementation.
            return base.GetOwnProperty(propertyName);
        }

        /// <summary>
        /// Sets the value of the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property to set. </param>
        /// <param name="value"> The value to set the property to. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is read-only or if the object is sealed. </param>
        internal override void Put(uint index, object value, bool throwOnError)
        {
            // Maintain a mapping between the array elements of this object and the corresponding
            // variable names.
            if (index >= 0 && index < this.mappedArguments.Length && this.mappedArguments[index] == true)
            {
                this.scope.SetValue(this.callee.ArgumentNames[(int)index], value);
                return;
            }

            // Delegate to the base class implementation.
            base.Put(index, value, throwOnError);
        }

        /// <summary>
        /// Sets the value of the property with the given name.
        /// </summary>
        /// <param name="propertyName"> The name of the property to set. </param>
        /// <param name="value"> The value to set the property to. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is read-only or if the object is sealed. </param>
        internal override void Put(string propertyName, object value, bool throwOnError)
        {
            // "callee" and "caller" should throw TypeErrors in strict mode.
            if (this.callee.StrictMode == true && (propertyName == "callee" || propertyName == "callee"))
                throw new JavaScriptException("TypeError", "It is illegal to access arguments.callee or arguments.caller in strict mode");

            // Delegate to the base class implementation.
            base.Put(propertyName, value, throwOnError);
        }

        /// <summary>
        /// Deletes the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property to delete. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is not configurable.  </param>
        /// <returns> <c>true</c> if the property was successfully deleted; <c>false</c> otherwise. </returns>
        internal override bool Delete(uint index, bool throwOnError)
        {
            // Break the mapping between the array element of this object and the corresponding
            // variable name.
            if (index >= 0 && index < this.mappedArguments.Length)
                this.mappedArguments[index] = false;

            // Delegate to the base class implementation.
            return base.Delete(index, throwOnError);
        }
    }
}
