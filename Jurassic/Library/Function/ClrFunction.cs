﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Jurassic.Compiler;
using Binder = Jurassic.Compiler.Binder;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a JavaScript function implemented by one or more .NET methods.
    /// </summary>
    public class ClrFunction : FunctionInstance
    {
        object thisBinding;
        private Binder callBinder;
        private Binder constructBinder;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new instance of a built-in constructor function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="instancePrototype">  </param>
        protected ClrFunction(ObjectInstance prototype, string name, ObjectInstance instancePrototype)
            : base(prototype)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (instancePrototype == null)
                throw new ArgumentNullException(nameof(instancePrototype));

            // This is a constructor so ignore the "this" parameter when the function is called.
            thisBinding = this;

            // Search through every method in this type looking for [JSCallFunction] and [JSConstructorFunction] attributes.
            var callBinderMethods = new List<JSBinderMethod>(1);
            var constructBinderMethods = new List<JSBinderMethod>(1);
            var methods = this.GetType().GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                // Search for the [JSCallFunction] and [JSConstructorFunction] attributes.
                var callAttribute = method.GetCustomAttribute<JSCallFunctionAttribute>();
                var constructorAttribute = method.GetCustomAttribute<JSConstructorFunctionAttribute>();

                // Can't declare both attributes.
                if (callAttribute != null && constructorAttribute != null)
                    throw new InvalidOperationException("Methods cannot be marked with both [JSCallFunction] and [JSConstructorFunction].");

                if (callAttribute != null)
                {
                    // Method is marked with [JSCallFunction]
                    callBinderMethods.Add(new JSBinderMethod(method, callAttribute.Flags));
                }
                else if (constructorAttribute != null)
                {
                    var binderMethod = new JSBinderMethod(method, constructorAttribute.Flags);
                    constructBinderMethods.Add(binderMethod);

                    // Constructors must return ObjectInstance or a derived type.
                    if (typeof(ObjectInstance).GetTypeInfo().IsAssignableFrom(binderMethod.ReturnType) == false)
                        throw new InvalidOperationException(string.Format("Constructors must return {0} (or a derived type).", typeof(ObjectInstance).Name));
                }
            }

            // Initialize the Call function.
            if (callBinderMethods.Count > 0)
                this.callBinder = new JSBinder(callBinderMethods);
            else
                this.callBinder = new EmptyBinder();

            // Initialize the Construct function.
            if (constructBinderMethods.Count > 0)
                this.constructBinder = new JSBinder(constructBinderMethods);
            else
                this.constructBinder = new EmptyBinder();

            // Add function properties.
            this.FastSetProperty("name", name, PropertyAttributes.Configurable);
            this.FastSetProperty("length", this.callBinder.FunctionLength, PropertyAttributes.Configurable);
            this.FastSetProperty("prototype", instancePrototype, PropertyAttributes.Sealed);
            instancePrototype.FastSetProperty("constructor", this, PropertyAttributes.NonEnumerable);
        }

        /// <summary>
        /// Creates a new instance of a function which calls the given delegate.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="delegateToCall"> The delegate to call. </param>
        /// <param name="name"> The name of the function.  Pass <c>null</c> to use the name of the
        /// delegate for the function name. </param>
        /// <param name="length"> The "typical" number of arguments expected by the function.  Pass
        /// <c>-1</c> to use the number of arguments expected by the delegate. </param>
        internal ClrFunction(ObjectInstance prototype, Delegate delegateToCall, string name = null, int length = -1)
            : base(prototype)
        {
            // Initialize the [[Call]] method.
            this.callBinder = new JSBinder(new JSBinderMethod(delegateToCall.GetMethodInfo()));

            // If the delegate has a class instance, use that to call the method.
            this.thisBinding = delegateToCall.Target;

            // Add function properties.
            this.FastSetProperty("name", name != null ? name : this.callBinder.Name, PropertyAttributes.Configurable);
            this.FastSetProperty("length", length >= 0 ? length : this.callBinder.FunctionLength, PropertyAttributes.Configurable);
            //this.FastSetProperty("prototype", this.Engine.Object.Construct());
            //this.InstancePrototype.FastSetProperty("constructor", this, PropertyAttributes.NonEnumerable);
        }

        /// <summary>
        /// Creates a new instance of a function which calls one or more provided methods.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="methods"> An enumerable collection of methods that logically comprise a
        /// single method group. </param>
        /// <param name="name"> The name of the function.  Pass <c>null</c> to use the name of the
        /// provided methods for the function name (in this case all the provided methods must have
        /// the same name). </param>
        /// <param name="length"> The "typical" number of arguments expected by the function.  Pass
        /// <c>-1</c> to use the maximum of arguments expected by any of the provided methods. </param>
        internal ClrFunction(ObjectInstance prototype, IEnumerable<JSBinderMethod> methods, string name = null, int length = -1)
            : base(prototype)
        {
            this.callBinder = new JSBinder(methods);

            // Add function properties.
            this.FastSetProperty("name", name == null ? this.callBinder.Name : name, PropertyAttributes.Configurable);
            this.FastSetProperty("length", length >= 0 ? length : this.callBinder.FunctionLength, PropertyAttributes.Configurable);
            //this.FastSetProperty("prototype", this.Engine.Object.Construct());
            //this.InstancePrototype.FastSetProperty("constructor", this, PropertyAttributes.NonEnumerable);
        }

        /// <summary>
        /// Creates a new instance of a function which calls the given binder.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="binder"> An object representing a collection of methods to bind to. </param>
        internal ClrFunction(ObjectInstance prototype, Binder binder)
            : base(prototype)
        {
            this.callBinder = binder;

            // Add function properties.
            this.FastSetProperty("name", binder.Name, PropertyAttributes.Configurable);
            this.FastSetProperty("length", binder.FunctionLength, PropertyAttributes.Configurable);
            //this.FastSetProperty("prototype", this.Engine.Object.Construct());
            //this.InstancePrototype.FastSetProperty("constructor", this, PropertyAttributes.NonEnumerable);
        }



        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="arguments"> An array of argument values. </param>
        /// <returns> The value that was returned from the function. </returns>
        public override object CallLateBound(object thisObject, params object[] arguments)
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
                return this.callBinder.Call(this.Engine, thisBinding != null ? thisBinding : thisObject, arguments);
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
                throw new JavaScriptException(this.Engine, ErrorType.TypeError, "Objects cannot be constructed from built-in functions");
            return (ObjectInstance)this.constructBinder.Call(this.Engine, this, argumentValues);
        }

        ///// <summary>
        ///// Creates a delegate that does type conversion and calls the method represented by this
        ///// object.
        ///// </summary>
        ///// <param name="argumentTypes"> The types of the arguments that will be passed to the delegate. </param>
        ///// <returns> A delegate that does type conversion and calls the method represented by this
        ///// object. </returns>
        //internal BinderDelegate CreateBinder<T>()
        //{
        //    // Delegate types have an Invoke method containing the relevant parameters.
        //    MethodInfo adapterInvokeMethod = typeof(T).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
        //    if (adapterInvokeMethod == null)
        //        throw new ArgumentException(nameof(T));

        //    // Get the argument types.
        //    Type[] argumentTypes = adapterInvokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();

        //    // Create the binder.
        //    return this.callBinder.CreateBinder(argumentTypes);
        //}


        //     HELPER CLASSES
        //_________________________________________________________________________________________

        /// <summary>
        /// A binder that merely returns undefined.
        /// </summary>
        private class EmptyBinder : Binder
        {
            /// <summary>
            /// Creates a new EmptyBinder instance.
            /// </summary>
            public EmptyBinder()
            {
            }

            /// <summary>
            /// Gets the name of the target methods.
            /// </summary>
            public override string Name
            {
                get { return "undefined"; }
            }

            /// <summary>
            /// Gets the full name of the target methods, including the type name.
            /// </summary>
            public override string FullName
            {
                get { return "undefined"; }
            }

            /// <summary>
            /// Generates a method that does type conversion and calls the bound method.
            /// </summary>
            /// <param name="generator"> The ILGenerator used to output the body of the method. </param>
            /// <param name="argumentCount"> The number of arguments that will be passed to the delegate. </param>
            /// <returns> A delegate that does type conversion and calls the method represented by this
            /// object. </returns>
            protected override void GenerateStub(Jurassic.Compiler.ILGenerator generator, int argumentCount)
            {
                // Emit undefined.
                EmitHelpers.EmitUndefined(generator);

                // End the IL.
                generator.Complete();
            }
        }
    }
}
