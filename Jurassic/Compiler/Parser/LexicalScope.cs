using System;
using System.Linq.Expressions;
using Jurassic.Library;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a lexical scope.
    /// </summary>
    public sealed class LexicalScope : IEnvironmentRecord
    {
        [ThreadStatic]
        private static LexicalScope global;

        private ObjectInstance scopeObject;
        private bool provideImplicitThis;

        /// <summary>
        /// Gets a reference to the global scope.
        /// </summary>
        public static LexicalScope Global
        {
            get
            {
                if (global == null)
                {
                    // Initialize the global scope.
                    var scope = new LexicalScope(null, GlobalObject.Instance, false);
                    scope.CreateMutableBinding("this", true);
                    scope.SetMutableBinding("this", GlobalObject.Instance, false);
                    global = scope;
                }
                return global;
            }
        }

        /// <summary>
        /// Creates a new declarative lexical scope.
        /// </summary>
        /// <param name="parent"> The parent scope. </param>
        public LexicalScope(LexicalScope parent)
            : this(parent, null, false)
        {
        }

        /// <summary>
        /// Creates a new object lexical scope.
        /// </summary>
        /// <param name="parent"> The parent scope. </param>
        /// <param name="scopeObject"> The object to use as the backing store. </param>
        /// <param name="provideImplicitThis">  </param>
        public LexicalScope(LexicalScope parent, ObjectInstance scopeObject, bool provideImplicitThis)
        {
            if (scopeObject == null)
                scopeObject = ObjectInstance.CreateRootObject();
            this.ParentEnvironment = parent;
            this.scopeObject = scopeObject;
            this.provideImplicitThis = provideImplicitThis;
        }

        /// <summary>
        /// Gets a reference to the parent lexical scope.
        /// </summary>
        public LexicalScope ParentEnvironment
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a reference to the object that contains the property with the given name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns> A reference to the object that contains the property with the given name, or
        /// <c>null</c> if the property name is unresolvable. </returns>
        private ObjectInstance GetScopeObject(string propertyName)
        {
            if (this.scopeObject.HasProperty(propertyName) == true)
                return this.scopeObject;
            if (this.ParentEnvironment == null)
                return null;
            return this.ParentEnvironment.GetScopeObject(propertyName);
        }

        /// <summary>
        /// Gets a value that can be used for the "this" keyword if a function is called from this
        /// scope.
        /// </summary>
        /// <returns> A value that can be used for the "this" keyword if a function is called from
        /// this scope. </returns>
        public object GetImplicitThisValue(string propertyName)
        {
            if (this.scopeObject.HasProperty(propertyName) == true)
                return this.provideImplicitThis ? (object)this.scopeObject : (object)Undefined.Value;
            if (this.ParentEnvironment == null)
                return Undefined.Value;
            return this.ParentEnvironment.GetImplicitThisValue(propertyName);
        }



        //     IEnvironmentRecord IMPLEMENTATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Determines if an environment record has a binding for an identifier.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <returns> <c>true</c> the binding exists; <c>false</c> if it does not. </returns>
        public bool HasBinding(string name)
        {
            return GetScopeObject(name) != null;
        }

        /// <summary>
        /// Determines if an environment record has a binding for an identifier.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <param name="localOnly"> <c>true</c> to check the immediate scope only; <c>false</c>
        /// otherwise. </param>
        /// <returns> <c>true</c> the binding exists; <c>false</c> if it does not. </returns>
        public bool HasBinding(string name, bool localOnly)
        {
            if (localOnly == true)
                return this.scopeObject.HasProperty(name);
            else
                return GetScopeObject(name) != null;
        }

        /// <summary>
        /// Creates a new mutable binding in the environment record.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <param name="mayBeDeleted"> <c>true</c> if the binding can be deleted; <c>false</c>
        /// otherwise. </param>
        public void CreateMutableBinding(string name, bool mayBeDeleted)
        {
            this.scopeObject.SetProperty(name, Undefined.Value, PropertyAttributes.Writable | (mayBeDeleted ? PropertyAttributes.Configurable : 0));
        }

        /// <summary>
        /// Sets the value of an already existing mutable binding in the environment record.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <param name="value"> The new value of the binding. </param>
        /// <param name="strict"> Indicates whether to use strict mode semantics. </param>
        /// <returns> The new value of the binding. </returns>
        public T SetMutableBinding<T>(string name, T value, bool strict)
        {
            var scope = GetScopeObject(name);
            if (scope != null)
            {
                scope.Put(name, value, strict);
            }
            else
            {
                if (strict == true)
                    throw new JavaScriptException("ReferenceError", name + " is not defined");
                Jurassic.Library.GlobalObject.Instance.Put(name, value, false);
            }
            return value;
        }

        /// <summary>
        /// Returns the value of an already existing binding from the environment record.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <param name="strict"> Indicates whether to use strict mode semantics. </param>
        /// <returns> The value of the binding. </returns>
        public object GetBindingValue(string name, bool strict)
        {
            var scope = GetScopeObject(name);
            if (scope == null)
                throw new JavaScriptException("ReferenceError", name + " is not defined");
            return scope.Get(name);
        }

        /// <summary>
        /// Deletes a binding from the environment record.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <returns> <c>true</c> if the binding exists and could be deleted, or if the binding
        /// doesn't exist; <c>false</c> if the binding couldn't be deleted. </returns>
        public bool DeleteBinding(string name)
        {
            var scope = GetScopeObject(name);
            if (scope == null)
                return true;
            return scope.Delete(name, false);
        }
    }

}