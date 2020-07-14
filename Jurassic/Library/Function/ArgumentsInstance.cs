﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Jurassic.Compiler;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an arguments object.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public partial class ArgumentsInstance : ObjectInstance
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
        public ArgumentsInstance(ObjectInstance prototype, UserDefinedFunction callee, DeclarativeScope scope, object[] argumentValues)
            : base(prototype)
        {
            if (callee == null)
                throw new ArgumentNullException(nameof(callee));
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (argumentValues == null)
                throw new ArgumentNullException(nameof(argumentValues));
            this.callee = callee;
            this.scope = scope;
            InitializeProperties(GetDeclarativeProperties(Engine));
            this.FastSetProperty("length", argumentValues.Length, PropertyAttributes.NonEnumerable);

            if (this.callee.StrictMode == false)
            {
                this.FastSetProperty("callee", callee, PropertyAttributes.NonEnumerable);


                // Create an array mappedArguments where mappedArguments[i] = true means a mapping is
                // maintained between arguments[i] and the corresponding variable.
                this.mappedArguments = new bool[argumentValues.Length];
                var mappedNames = new Dictionary<string, int>();    // maps argument name -> index
                for (int i = 0; i < argumentValues.Length; i++)
                {
                    if (i < callee.ArgumentNames.Count)
                    {
                        // Check if the argument name appeared previously in the argument list.
                        int previousIndex;
                        if (mappedNames.TryGetValue(callee.ArgumentNames[i], out previousIndex) == true)
                        {
                            // The argument name has appeared before.  Remove the getter/setter.
                            this.DefineProperty(previousIndex.ToString(), new PropertyDescriptor(argumentValues[previousIndex], PropertyAttributes.FullAccess), false);

                            // The argument is no longer mapped.
                            this.mappedArguments[previousIndex] = false;
                        }

                        // Add the argument name and index to the hashtable.
                        mappedNames[callee.ArgumentNames[i]] = i;

                        // The argument is mapped by default.
                        this.mappedArguments[i] = true;

                        // Define a getter and setter so that the property value reflects that of the argument.
                        var getter = new UserDefinedFunction(this.Engine.Function.InstancePrototype, "ArgumentGetter", new string[0], this.scope, "return " + callee.ArgumentNames[i], ArgumentGetter, true);
                        getter.SetPropertyValue("argumentIndex", i, false);
                        var setter = new UserDefinedFunction(this.Engine.Function.InstancePrototype, "ArgumentSetter", new string[] { "value" }, this.scope, callee.ArgumentNames[i] + " = value", ArgumentSetter, true);
                        setter.SetPropertyValue("argumentIndex", i, false);
                        this.DefineProperty(i.ToString(), new PropertyDescriptor(getter, setter, PropertyAttributes.FullAccess), false);
                    }
                    else
                    {
                        // This argument is unnamed - no mapping needs to happen.
                        this[(uint)i] = argumentValues[i];
                    }
                }
            }
            else
            {
                // In strict mode, arguments items are not connected to the variables.
                for (int i = 0; i < argumentValues.Length; i++)
                    this[(uint)i] = argumentValues[i];

                // In strict mode, accessing caller or callee is illegal.
                var throwErrorFunction = new ThrowTypeErrorFunction(this.Engine.Function.InstancePrototype);
                this.DefineProperty("caller", new PropertyDescriptor(throwErrorFunction, throwErrorFunction, PropertyAttributes.Sealed), false);
                this.DefineProperty("callee", new PropertyDescriptor(throwErrorFunction, throwErrorFunction, PropertyAttributes.Sealed), false);
            }
        }



        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Used to retrieve the value of an argument.
        /// </summary>
        /// <param name="context"> The script engine, this value, etc. </param>
        /// <param name="argumentValues"> The arguments that were passed to the function. </param>
        /// <returns> The result of calling the method. </returns>
        private object ArgumentGetter(ExecutionContext context, object[] argumentValues)
        {
            int argumentIndex = TypeConverter.ToInteger(context.ExecutingFunction.GetPropertyValue("argumentIndex"));
            return this.scope.GetValue(this.callee.ArgumentNames[argumentIndex]);
        }

        /// <summary>
        /// Used to set the value of an argument.
        /// </summary>
        /// <param name="context"> The script engine, this value, etc. </param>
        /// <param name="argumentValues"> The arguments that were passed to the function. </param>
        /// <returns> The result of calling the method. </returns>
        private object ArgumentSetter(ExecutionContext context, object[] argumentValues)
        {
            int argumentIndex = TypeConverter.ToInteger(context.ExecutingFunction.GetPropertyValue("argumentIndex"));
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

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");

                bool comma = false;
                int len = (int)this["length"];
                for (int i = 0; i < len; i++)
                {
                    object arg = this[i.ToString(CultureInfo.InvariantCulture)];
                    object value;
                    if (arg is PropertyAccessorValue)
                    {
                        value = (arg as PropertyAccessorValue).GetValue(this);
                    }
                    else
                    {
                        value = arg;
                    }

                    if (comma)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(DebuggerDisplayHelper.ShortStringRepresentation(value));
                    comma = true;
                }

                sb.Append("]");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayType; }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayType
        {
            get { return string.Format("Arguments({0})", this["length"]); }
        }



        //     JS FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns an iterator that iterates over the argument values.
        /// </summary>
        /// <returns> An iterator for the arguments instance. </returns>
        [JSInternalFunction(Name = "@@iterator")]
        public ObjectInstance GetIterator()
        {
            return new ArrayIterator(Engine.ArrayIteratorPrototype, this, ArrayIterator.Kind.Value);
        }
    }
}
