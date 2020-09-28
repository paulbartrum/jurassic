﻿using Jurassic.Compiler;
using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a JS class, which is really just a special type of function. Classes cannot be
    /// called directly, but they can be instantiated using the 'new' operator.
    /// </summary>
    public class ClassFunction : FunctionInstance
    {
        private UserDefinedFunction constructor;

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new instance of a user-defined class.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the class. Can be <c>null</c> if none were supplied. </param>
        /// <param name="instancePrototype"> The value of the 'prototype' property. </param>
        /// <param name="constructor"> A function that represents the constructor, if the class has
        /// one, or <c>null</c> otherwise. </param>
        /// <remarks>
        /// A class that doesn't extend looks like this:
        /// new ClassFunction(engine.Function.InstancePrototype, name, engine.Object.Construct(), constructor)
        /// 
        /// A class that extends A looks like this:
        /// new ClassFunction(A, name, ObjectInstance.CreateRawObject(A.InstancePrototype), constructor)
        /// 
        /// A class that extends null looks like this:
        /// new ClassFunction(engine.Function.InstancePrototype, name, ObjectInstance.CreateRawObject(null), constructor)
        /// </remarks>
        public ClassFunction(ObjectInstance prototype, string name, ObjectInstance instancePrototype, UserDefinedFunction constructor)
            : base(prototype)
        {
            if (instancePrototype == null)
                throw new ArgumentNullException(nameof(instancePrototype));
            this.constructor = constructor;

            // Initialize the instance prototype.
            instancePrototype.InitializeProperties(new List<PropertyNameAndValue>
            {
                new PropertyNameAndValue("constructor", this, PropertyAttributes.NonEnumerable)
            });

            // Now add properties to this object.
            int length = constructor == null ? 0 : constructor.Length;
            InitializeProperties(new List<PropertyNameAndValue>()
            {
                new PropertyNameAndValue("name", name ?? string.Empty, PropertyAttributes.Configurable),
                new PropertyNameAndValue("length", length, PropertyAttributes.Configurable),
                new PropertyNameAndValue("prototype", instancePrototype, PropertyAttributes.Writable),
            });
        }


        
        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values to pass to the function. </param>
        /// <returns> The value that was returned from the function. </returns>
        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            throw new JavaScriptException(ErrorType.TypeError, $"Class constructor {Name} cannot be invoked without 'new'");
        }

        /// <summary>
        /// Creates an object, using this function as the constructor.
        /// </summary>
        /// <param name="newTarget"> The value of 'new.target'. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The object that was created. </returns>
        public override ObjectInstance ConstructLateBound(FunctionInstance newTarget, params object[] argumentValues)
        {
            if (this.constructor != null)
            {
                // This class has a constructor.
                ExecutionContext context;

                if (Prototype != null && Prototype != Engine.Function.InstancePrototype)
                {
                    // This class extends another. In that case 'this' is unavailable.
                    context = ExecutionContext.CreateDerivedContext(
                        engine: this.Engine,
                        parentScope: this.constructor.ParentScope,
                        executingFunction: this.constructor,
                        newTarget: newTarget,
                        functionContainer: this);
                }
                else
                {
                    // This class doesn't extend from another class. Create a new object and set
                    // 'this' equal to the newly created object.
                    context = ExecutionContext.CreateConstructContext(
                        engine: this.Engine,
                        parentScope: this.constructor.ParentScope,
                        thisValue: ObjectInstance.CreateRawObject(newTarget.InstancePrototype),
                        executingFunction: this.constructor,
                        newTarget: newTarget,
                        functionContainer: this);
                }

                // Call the function.
                var result = this.constructor.Body(context, argumentValues);

                // If the constructor returned an object, use that, otherwise use the 'this' value.
                if (result is ObjectInstance resultObject)
                    return resultObject;

                // Make sure super() was called in derived classes.
                if (context.ThisBindingStatus == ExecutionContext.BindingStatus.Uninitialized)
                    throw new JavaScriptException(ErrorType.ReferenceError, "Must call super constructor in derived class before returning.");
                return (ObjectInstance)context.ThisValue;   // Must be ObjectInstance since it is only set in ExecutionContext.CallSuperClass().
            }
            else if (Prototype != Engine.Function.InstancePrototype)
            {
                // Call the base class constructor.
                if (Prototype is FunctionInstance super)
                {
                    return super.ConstructLateBound(newTarget, argumentValues);
                }
                else
                    throw new JavaScriptException(ErrorType.TypeError, $"Super constructor {Prototype} of {Name} is not a constructor.");
            }
            else
            {
                // There's no constructor and no base class.
                return ObjectInstance.CreateRawObject(newTarget.InstancePrototype);
            }
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            // TODO: fix this.
            return "class";
        }
    }
}
