using Jurassic.Compiler;
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
        private Scope constructorScope;

        [NonSerialized]
        private GeneratedMethod constructorGeneratedMethod;

        [NonSerialized]
        private FunctionDelegate constructorBody;

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new instance of a user-defined class.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the class. </param>
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
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (instancePrototype == null)
                throw new ArgumentNullException(nameof(instancePrototype));
            if (constructor != null)
            {
                this.constructorScope = constructor.ParentScope;
                this.constructorGeneratedMethod = constructor.GeneratedMethod;
                this.constructorBody = constructor.Body;
            }

            // Initialize the instance prototype.
            instancePrototype.InitializeProperties(new List<PropertyNameAndValue>
            {
                new PropertyNameAndValue("constructor", this, PropertyAttributes.NonEnumerable)
            });

            // Now add properties to this object.
            int length = constructor == null ? 0 : constructor.Length;
            InitializeProperties(new List<PropertyNameAndValue>()
            {
                new PropertyNameAndValue("name", name, PropertyAttributes.Configurable),
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
            throw new JavaScriptException(Engine, ErrorType.TypeError, $"Class constructor {Name} cannot be invoked without 'new'");
        }

        /// <summary>
        /// Creates an object, using this function as the constructor.
        /// </summary>
        /// <param name="newTarget"> The value of 'new.target'. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The object that was created. </returns>
        public override ObjectInstance ConstructLateBound(FunctionInstance newTarget, params object[] argumentValues)
        {
            bool doesExtend = Prototype != null && Prototype != Engine.Function.InstancePrototype;


            // Create a new object and set the prototype to the instance prototype of the function.
            var newObject = ObjectInstance.CreateRawObject(this.InstancePrototype);

            if (this.constructorBody != null)
            {
                // Run the function, with the new object as the "this" keyword.
                // The return value is ignored!
                this.constructorBody(this.Engine, this.constructorScope, newObject, this, newTarget, argumentValues);
            }
            else if (Prototype != Engine.Function.InstancePrototype)
            {
                // Call the base class constructor.
                if (Prototype is FunctionInstance super)
                {
                    super.ConstructLateBound(newTarget, argumentValues);
                }
                else
                    throw new JavaScriptException(Engine, ErrorType.TypeError, $"Super constructor {Prototype} of {Name} is not a constructor.");
            }

            // Otherwise, return the new object.
            return newObject;
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
