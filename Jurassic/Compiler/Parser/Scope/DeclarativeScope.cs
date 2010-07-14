using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a scope where the variables are statically known.
    /// </summary>
    public class DeclarativeScope : Scope
    {
        /// <summary>
        /// Creates a new declarative scope for use inside a method body.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope.  Can not be <c>null</c>. </param>
        /// <returns> A new DeclarativeScope instance. </returns>
        public static DeclarativeScope CreateFunctionScope(Scope parentScope)
        {
            if (parentScope == null)
                throw new ArgumentException("Function scopes must have a parent scope.");
            return new DeclarativeScope(parentScope);
        }

        /// <summary>
        /// Creates a new declarative scope for use inside a catch statement.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope.  Can not be <c>null</c>. </param>
        /// <returns> A new DeclarativeScope instance. </returns>
        public static DeclarativeScope CreateCatchScope(Scope parentScope)
        {
            if (parentScope == null)
                throw new ArgumentException("Function scopes must have a parent scope.");
            return new DeclarativeScope(parentScope);
        }

        /// <summary>
        /// Creates a new declarative scope for use at runtime.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope.  Can not be <c>null</c>. </param>
        /// <param name="variableCount"> The number of variables that were declared in this scope. </param>
        /// <returns> A new DeclarativeScope instance. </returns>
        public static DeclarativeScope CreateRuntimeScope(Scope parentScope, int variableCount)
        {
            if (parentScope == null)
                throw new ArgumentException("Function scopes must have a parent scope.");
            if (variableCount < 0)
                throw new ArgumentOutOfRangeException("variableCount");
            var result = new DeclarativeScope(parentScope);
            result.Values = new object[variableCount];
            return result;
        }

        /// <summary>
        /// Creates a new DeclarativeScope instance.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope, or <c>null</c> if this is
        /// the global scope. </param>
        private DeclarativeScope(Scope parentScope)
            : base(parentScope)
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
                throw new InvalidOperationException("");
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
            // DeclarativeScope.CreateRuntimeScope(scope, variableCount)
            generator.LoadArgument(0);
            generator.LoadInt32(this.DeclaredVariableCount);
            generator.Call(ReflectionHelpers.DeclarativeScope_CreateRuntimeScope);
            generator.StoreArgument(0);
        }
    }

}