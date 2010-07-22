using System;

namespace Jurassic.Compiler
{
    
    /// <summary>
    /// Represents a variable or part of a member reference.
    /// </summary>
    internal sealed class NameExpression : Expression, IReferenceExpression
    {
        /// <summary>
        /// Creates a new NameExpression instance.
        /// </summary>
        /// <param name="scope"> The current scope. </param>
        /// <param name="name"> The name of the variable or member that is being referenced. </param>
        public NameExpression(Scope scope, string name)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (name == null)
                throw new ArgumentNullException("name");
            this.Scope = scope;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the scope the name is contained within.
        /// </summary>
        public Scope Scope
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the variable or member.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Any; }
        }

        /// <summary>
        /// Gets the static type of the reference.
        /// </summary>
        public PrimitiveType Type
        {
            get { return this.ResultType; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // NOTE: this is a get reference because assignment expressions do not call this method.
            GenerateGet(generator, optimizationInfo, true);
        }

        /// <summary>
        /// Pushes the value of the reference onto the stack.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <param name="throwIfUnresolvable"> <c>true</c> to throw a ReferenceError exception if
        /// the name is unresolvable; <c>false</c> to output <c>null</c> instead. </param>
        public void GenerateGet(ILGenerator generator, OptimizationInfo optimizationInfo, bool throwIfUnresolvable)
        {
            var scope = this.Scope;
            ILLocalVariable scopeVariable = null;
            var endOfGet = generator.CreateLabel();
            do
            {
                if (scope is DeclarativeScope)
                {
                    // Get the index of the variable in the Values array.
                    int variableIndex = ((DeclarativeScope)scope).GetDeclaredVariableIndex(this.Name);
                    if (variableIndex >= 0)
                    {
                        // scope.Values[index]
                        if (scopeVariable == null)
                            generator.LoadArgument(0);
                        else
                            generator.LoadVariable(scopeVariable);
                        generator.CastClass(typeof(DeclarativeScope));
                        generator.Call(ReflectionHelpers.DeclarativeScope_Values);
                        generator.LoadInt32(variableIndex);
                        generator.LoadArrayElement(typeof(object));
                        break;
                    }
                }
                else
                {
                    // This method gets the value of a variable in an object scope.
                    if (scopeVariable == null)
                        generator.LoadArgument(0);
                    else
                        generator.LoadVariable(scopeVariable);
                    generator.CastClass(typeof(ObjectScope));
                    generator.Call(ReflectionHelpers.ObjectScope_ScopeObject);
                    generator.LoadString(this.Name);
                    generator.Call(ReflectionHelpers.ObjectInstance_GetPropertyValue_String);

                    // Check if the value is null.
                    generator.Duplicate();
                    generator.BranchIfNotNull(endOfGet);
                    if (scope.ParentScope != null)
                        generator.Pop();
                }

                // Try the parent scope.
                scope = scope.ParentScope;
                if (scope != null)
                {
                    if (scopeVariable == null)
                    {
                        scopeVariable = generator.CreateTemporaryVariable(typeof(Scope));
                        generator.LoadArgument(0);
                    }
                    else
                    {
                        generator.LoadVariable(scopeVariable);
                    }
                    generator.Call(ReflectionHelpers.Scope_ParentScope);
                    generator.StoreVariable(scopeVariable);
                }

            } while (scope != null);

            // Throw an error if the name does not exist and throwIfUnresolvable is true.
            if (scope == null && throwIfUnresolvable == true)
                EmitHelpers.EmitThrow(generator, "ReferenceError", this.Name + " is not defined");

            // Release the temporary variable.
            if (scopeVariable != null)
                generator.ReleaseTemporaryVariable(scopeVariable);

            // Define a label at the end.
            generator.DefineLabelPosition(endOfGet);

            // Object scope references may have side-effects (because of getters) so if the value
            // is to be ignored we evaluate the value then pop the value from the stack.
            if (optimizationInfo.SuppressReturnValue == true)
                generator.Pop();
        }

        /// <summary>
        /// Stores the value on the top of the stack in the reference.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <param name="valueType"> The primitive type of the value that is on the top of the stack. </param>
        /// <param name="throwIfUnresolvable"> <c>true</c> to throw a ReferenceError exception if
        /// the name is unresolvable; <c>false</c> to create a new property instead. </param>
        public void GenerateSet(ILGenerator generator, OptimizationInfo optimizationInfo, PrimitiveType valueType, bool throwIfUnresolvable)
        {
            // Convert the value to an object and store it in a variable.
            EmitConversion.Convert(generator, valueType, PrimitiveType.Any);
            var valueVariable = generator.CreateTemporaryVariable(typeof(object));
            generator.StoreVariable(valueVariable);

            var scope = this.Scope;
            ILLocalVariable scopeVariable = null;
            var endOfSet = generator.CreateLabel();
            do
            {
                if (scope is DeclarativeScope)
                {
                    // Get the index of the variable in the Values array.
                    int variableIndex = ((DeclarativeScope)scope).GetDeclaredVariableIndex(this.Name);
                    if (variableIndex >= 0)
                    {
                        // scope.Values[index] = value
                        if (scopeVariable == null)
                            generator.LoadArgument(0);
                        else
                            generator.LoadVariable(scopeVariable);
                        generator.CastClass(typeof(DeclarativeScope));
                        generator.Call(ReflectionHelpers.DeclarativeScope_Values);
                        generator.LoadInt32(variableIndex);
                        generator.LoadVariable(valueVariable);
                        generator.StoreArrayElement(typeof(object));
                        break;
                    }
                }
                else
                {
                    if (scopeVariable == null)
                        generator.LoadArgument(0);
                    else
                        generator.LoadVariable(scopeVariable);
                    generator.CastClass(typeof(ObjectScope));
                    generator.Call(ReflectionHelpers.ObjectScope_ScopeObject);
                    generator.LoadString(this.Name);
                    generator.LoadVariable(valueVariable);
                    generator.LoadBoolean(optimizationInfo.StrictMode);

                    if (scope.ParentScope == null && throwIfUnresolvable == false)
                    {
                        // Set the property value unconditionally.
                        generator.Call(ReflectionHelpers.ObjectInstance_SetPropertyValue_String);
                    }
                    else
                    {
                        // Set the property value if the property exists.
                        generator.Call(ReflectionHelpers.ObjectInstance_SetPropertyValueIfExists);

                        // The return value is true if the property was defined, and false if it wasn't.
                        generator.BranchIfTrue(endOfSet);
                    }
                }

                // Try the parent scope.
                scope = scope.ParentScope;
                if (scope != null)
                {
                    if (scopeVariable == null)
                    {
                        scopeVariable = generator.CreateTemporaryVariable(typeof(Scope));
                        generator.LoadArgument(0);
                    }
                    else
                    {
                        generator.LoadVariable(scopeVariable);
                    }
                    generator.Call(ReflectionHelpers.Scope_ParentScope);
                    generator.StoreVariable(scopeVariable);
                }

            } while (scope != null);

            // Throw an error if the name does not exist and throwIfUnresolvable is true.
            if (scope == null && throwIfUnresolvable == true)
                EmitHelpers.EmitThrow(generator, "ReferenceError", this.Name + " is not defined");

            // Release the temporary variables.
            generator.ReleaseTemporaryVariable(valueVariable);
            if (scopeVariable != null)
                generator.ReleaseTemporaryVariable(scopeVariable);

            // Define a label at the end.
            generator.DefineLabelPosition(endOfSet);
        }

        /// <summary>
        /// Deletes the reference and pushes <c>true</c> if the delete succeeded, or <c>false</c>
        /// if the delete failed.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void GenerateDelete(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            var endOfDelete = generator.CreateLabel();
            var scope = this.Scope;
            ILLocalVariable scopeVariable = generator.CreateTemporaryVariable(typeof(Scope));
            generator.LoadArgument(0);
            generator.StoreVariable(scopeVariable);
            do
            {
                if (scope is DeclarativeScope)
                {
                    // delete on a DeclarativeScope throws an exception in strict more or does nothing otherwise.
                    if (scope.HasDeclaredVariable(this.Name))
                    {
                        // The variable exists in the declarative scope - return false.
                        generator.LoadBoolean(false);
                        break;
                    }
                }
                else
                {
                    // Check if the property exists by calling scope.ScopeObject.HasProperty(propertyName)
                    generator.LoadVariable(scopeVariable);
                    generator.CastClass(typeof(ObjectScope));
                    generator.Call(ReflectionHelpers.ObjectScope_ScopeObject);
                    generator.Duplicate();
                    generator.LoadString(this.Name);
                    generator.Call(ReflectionHelpers.ObjectInstance_HasProperty);

                    // Jump past the delete if the property doesn't exist.
                    var endOfExistsCheck = generator.CreateLabel();
                    generator.BranchIfFalse(endOfExistsCheck);

                    // Call scope.ScopeObject.Delete(propertyName, false)
                    generator.LoadString(this.Name);
                    generator.LoadBoolean(false);
                    generator.Call(ReflectionHelpers.ObjectInstance_Delete);
                    generator.Branch(endOfDelete);

                    generator.DefineLabelPosition(endOfExistsCheck);
                    generator.Pop();

                    // If the name is not defined, return true.
                    if (scope.ParentScope == null)
                    {
                        generator.LoadBoolean(true);
                    }
                }

                // Try the parent scope.
                scope = scope.ParentScope;
                if (scope != null)
                {
                    generator.LoadVariable(scopeVariable);
                    generator.Call(ReflectionHelpers.Scope_ParentScope);
                    generator.StoreVariable(scopeVariable);
                }

            } while (scope != null);

            // Release the temporary variable.
            generator.ReleaseTemporaryVariable(scopeVariable);

            // Define a label at the end.
            generator.DefineLabelPosition(endOfDelete);

            // Delete obviously has side-effects so we evaluate the return value then pop it from
            // the stack.
            if (optimizationInfo.SuppressReturnValue == true)
                generator.Pop();
        }

        /// <summary>
        /// Generates code to push the "this" value for a function call.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        public void GenerateThis(ILGenerator generator)
        {
            // Optimization: if there are no with scopes, simply emit undefined.
            bool scopeChainHasWithScope = false;
            var scope = this.Scope;
            do
            {
                if (scope is ObjectScope && ((ObjectScope)scope).ProvidesImplicitThisValue == true)
                {
                    scopeChainHasWithScope = true;
                    break;
                }
                scope = scope.ParentScope;
            } while (scope != null);

            if (scopeChainHasWithScope == false)
            {
                // No with scopes in the scope chain, use undefined as the "this" value.
                EmitHelpers.EmitUndefined(generator);
                return;
            }

            var end = generator.CreateLabel();

            scope = this.Scope;
            ILLocalVariable scopeVariable = generator.CreateTemporaryVariable(typeof(Scope));
            generator.LoadArgument(0);
            generator.StoreVariable(scopeVariable);

            do
            {
                if (scope is DeclarativeScope)
                {
                    // delete on a DeclarativeScope throws an exception in strict more or does nothing otherwise.
                    int variableIndex = ((DeclarativeScope)scope).GetDeclaredVariableIndex(this.Name);
                    if (scope.HasDeclaredVariable(this.Name))
                    {
                        // The variable exists but declarative scopes always produce undefined for
                        // the "this" value.
                        EmitHelpers.EmitUndefined(generator);
                        break;
                    }
                }
                else
                {
                    var objectScope = (ObjectScope)scope;

                    // Check if the property exists by calling scope.ScopeObject.HasProperty(propertyName)
                    if (objectScope.ProvidesImplicitThisValue == false)
                        EmitHelpers.EmitUndefined(generator);
                    generator.LoadVariable(scopeVariable);
                    generator.CastClass(typeof(ObjectScope));
                    generator.Call(ReflectionHelpers.ObjectScope_ScopeObject);
                    if (objectScope.ProvidesImplicitThisValue == true)
                        generator.Duplicate();
                    generator.LoadString(this.Name);
                    generator.Call(ReflectionHelpers.ObjectInstance_HasProperty);
                    generator.BranchIfTrue(end);
                    generator.Pop();

                    // If the name is not defined, use undefined for the "this" value.
                    if (scope.ParentScope == null)
                    {
                        EmitHelpers.EmitUndefined(generator);
                    }
                }

                // Try the parent scope.
                scope = scope.ParentScope;
                if (scope != null)
                {
                    generator.LoadVariable(scopeVariable);
                    generator.Call(ReflectionHelpers.Scope_ParentScope);
                    generator.StoreVariable(scopeVariable);
                }

            } while (scope != null);

            // Release the temporary variable.
            generator.ReleaseTemporaryVariable(scopeVariable);

            // Define a label at the end.
            generator.DefineLabelPosition(end);
        }

        /// <summary>
        /// Calculates the hash code for this object.
        /// </summary>
        /// <returns> The hash code for this object. </returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Scope.GetHashCode();
        }

        /// <summary>
        /// Determines if the given object is equal to this one.
        /// </summary>
        /// <param name="obj"> The object to compare. </param>
        /// <returns> <c>true</c> if the given object is equal to this one; <c>false</c> otherwise. </returns>
        public override bool Equals(object obj)
        {
            if ((obj is NameExpression) == false)
                return false;
            return this.Name == ((NameExpression)obj).Name && this.Scope == ((NameExpression)obj).Scope;
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}