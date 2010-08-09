using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents an enclosing context where variables are uniquely defined.
    /// </summary>
    public abstract class Scope
    {
        private Dictionary<string, DeclaredVariable> variables;

        /// <summary>
        /// Represents a variable declared in a scope.
        /// </summary>
        private class DeclaredVariable
        {
            public int Index;
            public string Name;
            public FunctionExpression ValueAtTopOfScope;
            public SourceCodeSpan DebugInfo;
        }

        /// <summary>
        /// Creates a new Scope instance.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope, or <c>null</c> if this is
        /// the global scope. </param>
        protected Scope(Scope parentScope)
            : this(parentScope, 0)
        {
        }

        /// <summary>
        /// Creates a new Scope instance.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope, or <c>null</c> if this is
        /// the global scope. </param>
        /// <param name="declaredVariableCount"> The number of variables declared in this scope. </param>
        protected Scope(Scope parentScope, int declaredVariableCount)
        {
            this.ParentScope = parentScope;
            this.variables = new Dictionary<string, DeclaredVariable>(declaredVariableCount);
        }

        /// <summary>
        /// Gets a reference to the parent scope.  Can be <c>null</c> if this is the global scope.
        /// </summary>
        public Scope ParentScope
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of variables declared in this scope.
        /// </summary>
        internal int DeclaredVariableCount
        {
            get { return this.variables.Count; }
        }

        /// <summary>
        /// Gets an enumerable list of the names of all the declared variables (including function
        /// declarations), listed in the order they were declared.
        /// </summary>
        internal IEnumerable<string> DeclaredVariableNames
        {
            get
            {
                var declaredVariables = new List<DeclaredVariable>(this.variables.Values);
                declaredVariables.Sort((a, b) => a.Index - b.Index);
                var names = new string[declaredVariables.Count];
                for (int i = 0; i < declaredVariables.Count; i++)
                    names[i] = declaredVariables[i].Name;
                return names;
            }
        }

        /// <summary>
        /// Gets the index of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <returns> The index of the given variable, or <c>-1</c> if the variable doesn't exist
        /// in the scope. </returns>
        internal int GetDeclaredVariableIndex(string variableName)
        {
            if (variableName == null)
                throw new ArgumentNullException("variableName");
            DeclaredVariable variable;
            if (this.variables.TryGetValue(variableName, out variable) == false)
                return -1;
            return variable.Index;
        }

        /// <summary>
        /// Returns <c>true</c> if the given variable has been declared in this scope.
        /// </summary>
        /// <param name="name"> The name of the variable. </param>
        /// <returns> <c>true</c> if the given variable has been declared in this scope;
        /// <c>false</c> otherwise. </returns>
        internal bool HasDeclaredVariable(string name)
        {
            return this.variables.ContainsKey(name);
        }

        /// <summary>
        /// Declares a variable in this scope.  The variable will be set to "undefined" at the top
        /// of the scope.
        /// </summary>
        /// <param name="name"> The name of the variable. </param>
        internal void DeclareVariable(string name)
        {
            DeclareVariableOrFunction(name, null, null);
        }

        /// <summary>
        /// Declares a function in this scope.  This will be initialized with the value of the
        /// given expression.
        /// </summary>
        /// <param name="name"> The name of the variable. </param>
        /// <param name="valueAtTopOfScope"> The value at the top of the scope.  Only used by
        /// function declarations (not function expressions). </param>
        internal void DeclareFunction(string name, FunctionExpression valueAtTopOfScope, SourceCodeSpan debugInfo)
        {
            if (valueAtTopOfScope == null)
                throw new ArgumentNullException("valueAtTopOfScope");
            DeclareVariableOrFunction(name, valueAtTopOfScope, debugInfo);
        }

        /// <summary>
        /// Declares a variable or function in this scope.  This will be initialized with the value
        /// of the given expression.
        /// </summary>
        /// <param name="name"> The name of the variable. </param>
        /// <param name="valueAtTopOfScope"> The value at the top of the scope.  Only used by
        /// function declarations (not function expressions). </param>
        internal virtual void DeclareVariableOrFunction(string name, FunctionExpression valueAtTopOfScope, SourceCodeSpan debugInfo)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            DeclaredVariable variable;
            this.variables.TryGetValue(name, out variable);
            if (variable == null)
            {
                // This is a local variable that has not been declared before.
                variable = new DeclaredVariable() { Index = this.variables.Count, Name = name };
                this.variables.Add(name, variable);
            }

            // Set the initial value, if one was provided.
            if (valueAtTopOfScope != null)
            {
                variable.ValueAtTopOfScope = valueAtTopOfScope;
                variable.DebugInfo = debugInfo;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether any functions have been declared in this scope.
        /// </summary>
        /// <returns> <c>true</c> if one or more functions have been declared in this scope;
        /// <c>false</c> otherwise. </returns>
        internal bool ContainsFunctionDeclarations()
        {
            foreach (var variable in this.variables.Values)
                if (variable.ValueAtTopOfScope != null)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the given variable exists in this scope.
        /// </summary>
        /// <param name="variableName"> The name of the variable to check. </param>
        /// <returns> <c>true</c> if the given variable exists in this scope; <c>false</c>
        /// otherwise. </returns>
        public abstract bool HasValue(string variableName);

        /// <summary>
        /// Returns the value of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <returns> The value of the given variable, or <c>null</c> if the variable doesn't exist
        /// in the scope. </returns>
        public abstract object GetValue(string variableName);

        /// <summary>
        /// Sets the value of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="value"> The new value of the variable. </param>
        public abstract void SetValue(string variableName, object value);

        /// <summary>
        /// Generates code that creates a new scope.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        internal abstract void GenerateScopeCreation(ILGenerator generator, OptimizationInfo optimizationInfo);

        /// <summary>
        /// Generates code that initializes the variable and function declarations.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        internal void GenerateDeclarations(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Initialize the declared variables and functions.
            foreach (var variable in this.variables.Values)
            {
                if (variable.ValueAtTopOfScope == null)
                {
                    // Emit a variable declaration (this is only needed for object scopes).
                    if (this is ObjectScope)
                    {
                        EmitHelpers.EmitUndefined(generator);
                        var variableDeclaration = new NameExpression(this, variable.Name);
                        variableDeclaration.GenerateSet(generator, optimizationInfo, PrimitiveType.Undefined, false);
                    }
                }
                else
                {
                    // Emit a function declaration.
                    if (optimizationInfo.DebugDocument != null && variable.DebugInfo != null)
                        generator.MarkSequencePoint(optimizationInfo.DebugDocument, variable.DebugInfo);
                    variable.ValueAtTopOfScope.GenerateCode(generator, optimizationInfo);
                    var functionDeclaration = new NameExpression(this, variable.Name);
                    functionDeclaration.GenerateSet(generator, optimizationInfo, variable.ValueAtTopOfScope.ResultType, false);
                }
            }
        }
    }

}