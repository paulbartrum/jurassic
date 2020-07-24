using Jurassic.Library;
using System;
using ErrorType = Jurassic.Library.ErrorType;

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
                throw new ArgumentNullException(nameof(scope));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
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
            get
            {
                // Typed variables are only allowed for declarative scopes that have been optimized away.
                if (this.Scope.ExistsAtRuntime == false)
                {
                    var scope = this.Scope;
                    do
                    {
                        var variableInfo = this.Scope.GetDeclaredVariable(this.Name);
                        if (variableInfo != null)
                            return variableInfo.Type;
                        scope = scope.ParentScope;
                    } while (scope != null && scope is DeclarativeScope && scope.ExistsAtRuntime == false);
                }
                return PrimitiveType.Any;
            }
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
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // NOTE: this is a get reference because assignment expressions do not call this method.
            GenerateGet(generator, optimizationInfo, true);
        }

        /// <summary>
        /// Outputs the values needed to get or set this reference.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void GenerateReference(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Do nothing.
        }

        /// <summary>
        /// Outputs the values needed to get or set this reference.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void DuplicateReference(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Do nothing.
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
            // TODO: optimize this using optimizationInfo.OptimizeDeclarativeScopes

            // executionContext.CreateRuntimeScope(parentScope);
            //EmitHelpers.LoadExecutionContext(generator);
            //generator.Call(ReflectionHelpers.ExecutionContext_CreateRuntimeScope);

            generator.LoadVariable(Scope.GeneratedRuntimeVariable);
            generator.LoadString(Name);
            generator.Call(ReflectionHelpers.RuntimeScope_GetValue);
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
            generator.LoadVariable(Scope.GeneratedRuntimeVariable);
            generator.LoadString(Name);
            generator.Call(ReflectionHelpers.RuntimeScope_SetValue);
        }

        /// <summary>
        /// Deletes the reference and pushes <c>true</c> if the delete succeeded, or <c>false</c>
        /// if the delete failed.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void GenerateDelete(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Deleting a variable is not allowed in strict mode.
            if (optimizationInfo.StrictMode == true)
                throw new SyntaxErrorException(string.Format("Cannot delete {0} because deleting a variable or argument is not allowed in strict mode", this.Name), optimizationInfo.SourceSpan.StartLine, optimizationInfo.Source.Path, optimizationInfo.FunctionName);

            var endOfDelete = generator.CreateLabel();
            var scope = this.Scope;
            ILLocalVariable scopeVariable = generator.CreateTemporaryVariable(typeof(Scope));
            EmitHelpers.LoadScope(generator);
            generator.StoreVariable(scopeVariable);
            do
            {
                if (scope is DeclarativeScope)
                {
                    var variable = scope.GetDeclaredVariable(this.Name);
                    if (variable != null)
                    {
                        // The variable is known at compile-time.
                        if (variable.Deletable == false)
                        {
                            // The variable cannot be deleted - return false.
                            generator.LoadBoolean(false);
                        }
                        else
                        {
                            // The variable can be deleted (it was declared inside an eval()).
                            // Delete the variable.
                            generator.LoadVariable(scopeVariable);
                            generator.CastClass(typeof(DeclarativeScope));
                            generator.LoadString(this.Name);
                            generator.Call(ReflectionHelpers.Scope_Delete);
                        }
                        break;
                    }
                    else
                    {
                        // The variable was not defined at compile time, but may have been
                        // introduced by an eval() statement.
                        if (optimizationInfo.MethodOptimizationHints.HasEval == true)
                        {
                            // Check the variable exists: if (scope.HasValue(variableName) == true) {
                            generator.LoadVariable(scopeVariable);
                            generator.CastClass(typeof(DeclarativeScope));
                            generator.LoadString(this.Name);
                            generator.Call(ReflectionHelpers.Scope_HasValue);
                            var hasValueClause = generator.CreateLabel();
                            generator.BranchIfFalse(hasValueClause);

                            // If the variable does exist, return true.
                            generator.LoadVariable(scopeVariable);
                            generator.CastClass(typeof(DeclarativeScope));
                            generator.LoadString(this.Name);
                            generator.Call(ReflectionHelpers.Scope_Delete);
                            generator.Branch(endOfDelete);

                            // }
                            generator.DefineLabelPosition(hasValueClause);
                        }
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

                    // Call scope.ScopeObject.Delete(key, false)
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
                if (scope.ParentScope != null && scope.ExistsAtRuntime == true)
                {
                    generator.LoadVariable(scopeVariable);
                    generator.Call(ReflectionHelpers.Scope_ParentScope);
                    generator.StoreVariable(scopeVariable);
                }
                scope = scope.ParentScope;

            } while (scope != null);

            // Release the temporary variable.
            generator.ReleaseTemporaryVariable(scopeVariable);

            // Define a label at the end.
            generator.DefineLabelPosition(endOfDelete);

            // Delete obviously has side-effects so we evaluate the return value then pop it from
            // the stack.
            //if (optimizationInfo.SuppressReturnValue == true)
            //    generator.Pop();
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
            EmitHelpers.LoadScope(generator);
            generator.StoreVariable(scopeVariable);

            do
            {
                if (scope is DeclarativeScope)
                {
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
                if (scope.ParentScope != null && scope.ExistsAtRuntime == true)
                {
                    generator.LoadVariable(scopeVariable);
                    generator.Call(ReflectionHelpers.Scope_ParentScope);
                    generator.StoreVariable(scopeVariable);
                }
                scope = scope.ParentScope;

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