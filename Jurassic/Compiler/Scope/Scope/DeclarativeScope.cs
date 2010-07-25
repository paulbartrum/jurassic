using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a scope where the variables are statically known.
    /// </summary>
    public class DeclarativeScope : Scope
    {
        /// <summary>
        /// Creates a new declarative scope for use inside a function body.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope.  Can not be <c>null</c>. </param>
        /// <param name="functionName"> The name of the function.  Can be empty for an anonymous function. </param>
        /// <param name="argumentNames"> The names of each of the function arguments. </param>
        /// <returns> A new DeclarativeScope instance. </returns>
        public static DeclarativeScope CreateFunctionScope(Scope parentScope, string functionName, IEnumerable<string> argumentNames)
        {
            if (parentScope == null)
                throw new ArgumentNullException("parentScope", "Function scopes must have a parent scope.");
            if (functionName == null)
                throw new ArgumentNullException("functionName");
            if (argumentNames == null)
                throw new ArgumentNullException("argumentNames");
            var result = new DeclarativeScope(parentScope, 0);
            if (string.IsNullOrEmpty(functionName) == false)
                result.DeclareVariable(functionName);
            result.DeclareVariable("this");
            result.DeclareVariable("arguments");
            foreach (var argumentName in argumentNames)
                result.DeclareVariable(argumentName);
            return result;
        }

        /// <summary>
        /// Creates a new declarative scope for use inside a catch statement.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope.  Can not be <c>null</c>. </param>
        /// <returns> A new DeclarativeScope instance. </returns>
        public static DeclarativeScope CreateCatchScope(Scope parentScope)
        {
            if (parentScope == null)
                throw new ArgumentNullException("parentScope", "Function scopes must have a parent scope.");
            return new DeclarativeScope(parentScope, 0);
        }

        /// <summary>
        /// Creates a new declarative scope for use inside a strict mode eval statement.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope.  Can not be <c>null</c>. </param>
        /// <returns> A new DeclarativeScope instance. </returns>
        public static DeclarativeScope CreateEvalScope(Scope parentScope)
        {
            if (parentScope == null)
                throw new ArgumentNullException("parentScope", "Eval scopes must have a parent scope.");
            return new DeclarativeScope(parentScope, 0);
        }

        /// <summary>
        /// Creates a new declarative scope for use at runtime.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope.  Can not be <c>null</c>. </param>
        /// <param name="declaredVariableNames"> The names of variables that were declared in this scope. </param>
        /// <returns> A new DeclarativeScope instance. </returns>
        public static DeclarativeScope CreateRuntimeScope(Scope parentScope, string[] declaredVariableNames)
        {
            if (parentScope == null)
                throw new ArgumentNullException("parentScope", "Function scopes must have a parent scope.");
            if (declaredVariableNames == null)
                throw new ArgumentNullException("declaredVariableNames");
            var result = new DeclarativeScope(parentScope, declaredVariableNames.Length);
            foreach (string variableName in declaredVariableNames)
                result.DeclareVariable(variableName);
            result.Values = new object[declaredVariableNames.Length];
            return result;
        }

        /// <summary>
        /// Creates a new DeclarativeScope instance.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope, or <c>null</c> if this is
        /// the global scope. </param>
        /// <param name="declaredVariableCount"> The number of variables declared in this scope. </param>
        private DeclarativeScope(Scope parentScope, int declaredVariableCount)
            : base(parentScope, declaredVariableCount)
        {
        }

        /// <summary>
        /// Gets an array of values, one for each variable.  Only available for declarative scopes
        /// created using CreateRuntimeScope().
        /// </summary>
        public object[] Values
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns <c>true</c> if the given variable exists in this scope.
        /// </summary>
        /// <param name="variableName"> The name of the variable to check. </param>
        /// <returns> <c>true</c> if the given variable exists in this scope; <c>false</c>
        /// otherwise. </returns>
        public override bool HasValue(string variableName)
        {
            return this.HasDeclaredVariable(variableName);
        }

        /// <summary>
        /// Returns the value of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <returns> The value of the given variable, or <c>null</c> if the variable doesn't exist
        /// in the scope. </returns>
        /// <exception cref="InvalidOperationException"> The scope was not created using
        /// CreateRuntimeScope(). </exception>
        public override object GetValue(string variableName)
        {
            if (this.Values == null)
                throw new InvalidOperationException("This method can only be used when the DeclarativeScope is created using CreateRuntimeScope().");
            int index = GetDeclaredVariableIndex(variableName);
            if (index < 0)
                return null;
            return this.Values[index];
        }

        /// <summary>
        /// Sets the value of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="value"> The new value of the variable. </param>
        /// <exception cref="InvalidOperationException"> The scope was not created using
        /// CreateRuntimeScope(). </exception>
        public override void SetValue(string variableName, object value)
        {
            int index = GetDeclaredVariableIndex(variableName);
            if (index < 0)
            {
                this.DeclareVariable(variableName);
                index = GetDeclaredVariableIndex(variableName);
            }
            this.Values[index] = value;
        }

        /// <summary>
        /// Generates code that creates a new scope.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        internal override void GenerateScopeCreation(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Create a new declarative scope.
            
            // parentScope
            generator.LoadArgument(0);

            // declaredVariableNames
            generator.LoadInt32(this.DeclaredVariableCount);
            generator.NewArray(typeof(string));
            int i = 0;
            foreach (string variableName in this.DeclaredVariableNames)
            {
                generator.Duplicate();
                generator.LoadInt32(i ++);
                generator.LoadString(variableName);
                generator.StoreArrayElement(typeof(string));
            }

            // DeclarativeScope.CreateRuntimeScope(parentScope, declaredVariableNames)
            generator.Call(ReflectionHelpers.DeclarativeScope_CreateRuntimeScope);

            // Store the new scope in the first method parameter.
            generator.StoreArgument(0);
        }
    }

}