﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents an enclosing context where variables are uniquely defined.
    /// </summary>
    public sealed class Scope
    {
        private ScopeType Type { get; set; }

        // A dictionary containing the variables declared in this scope.
        private Dictionary<string, DeclaredVariable> variables;

        /// <summary>
        /// Represents a variable declared in a scope.
        /// </summary>
        internal class DeclaredVariable
        {
            // The scope the variable was declared in.
            public Scope Scope;

            // The index of the variable (in the order it was added).
            public int Index;

            // The keyword that was used to declare the variable (var, let or const).
            public KeywordToken Keyword;

            // The name of the variable.
            public string Name;

            // The storage container for the variable.
            public ILLocalVariable Store;

            // The statically-determined storage type for the variable.
            public PrimitiveType Type = PrimitiveType.Any;
        }

        private Dictionary<string, FunctionExpression> hoistedFunctions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeContext"></param>
        /// <returns></returns>
        internal static Scope CreateGlobalOrEvalScope(CodeContext codeContext)
        {
            return new Scope(null, 0) { Type = codeContext == CodeContext.Eval ? ScopeType.Eval : ScopeType.Global };
        }

        /// <summary>
        /// Creates a new declarative scope for use inside a function body (and within function
        /// argument default values).
        /// </summary>
        /// <param name="functionName"> The name of the function.  Can be empty for an anonymous function. </param>
        /// <param name="argumentNames"> The names of each of the function arguments.  Can be <c>null</c>. </param>
        /// <returns> A new DeclarativeScope instance. </returns>
        internal static Scope CreateFunctionScope(string functionName, IEnumerable<string> argumentNames)
        {
            var result = new Scope(null, (argumentNames != null ? argumentNames.Count() : 0) + 3);
            result.Type = ScopeType.TopLevelFunction;
            if (functionName != null)
                result.DeclareVariable(KeywordToken.Var, functionName);
            result.DeclareVariable(KeywordToken.Var, "arguments");
            if (argumentNames != null)
            {
                foreach (var argumentName in argumentNames)
                    result.DeclareVariable(KeywordToken.Var, argumentName);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentScope"></param>
        /// <returns></returns>
        internal static Scope CreateBlockScope(Scope parentScope)
        {
            if (parentScope == null)
                throw new ArgumentException("Block scopes must have a parent scope.");
            return new Scope(parentScope, 0) { Type = ScopeType.Block };
        }

        /// <summary>
        /// Creates a new object scope for use inside a with statement.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope.  Can not be <c>null</c>. </param>
        /// <returns> A new ObjectScope instance. </returns>
        internal static Scope CreateWithScope(Scope parentScope)
        {
            if (parentScope == null)
                throw new ArgumentException("With scopes must have a parent scope.");
            return new Scope(parentScope, 0) { Type = ScopeType.With };
        }

        /// <summary>
        /// Creates a new Scope instance.
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope, or <c>null</c> if this is
        /// the global scope. </param>
        /// <param name="declaredVariableCount"> The number of variables declared in this scope. </param>
        private Scope(Scope parentScope, int declaredVariableCount)
        {
            this.ParentScope = parentScope;
            this.variables = new Dictionary<string, DeclaredVariable>(declaredVariableCount);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ConvertToStrictMode()
        {
            if (Type == ScopeType.Eval)
                Type = ScopeType.EvalStrict;
        }

        /// <summary>
        /// Gets a reference to the parent scope.  Can be <c>null</c> if this is the global scope.
        /// </summary>
        public Scope ParentScope { get; private set; }

        /// <summary>
        /// Gets the number of variables declared in this scope.
        /// </summary>
        private int DeclaredVariableCount
        {
            get { return this.variables.Count; }
        }

        /// <summary>
        /// After GenerateScopeCreation is called, gets a variable containing the RuntimeScope
        /// instance. Can be <c>null</c> if this scope contains no variables.
        /// </summary>
        private ILLocalVariable GeneratedRuntimeScope { get; set; }

        /// <summary>
        /// Indicates whether <see cref="GenerateScopeCreation(ILGenerator, OptimizationInfo)"/>
        /// was called.
        /// </summary>
        private bool GenerateScopeCreationWasCalled { get; set; }

        /// <summary>
        /// Gets an enumerable list of the declared variables, in no particular order.
        /// </summary>
        internal IEnumerable<DeclaredVariable> DeclaredVariables
        {
            get { return this.variables.Values; }
        }

        /// <summary>
        /// Gets a reference to the variable with the given name, in this scope or any parent
        /// scope.
        /// </summary>
        /// <param name="variableName"> The name of the variable to find. </param>
        /// <returns> The variable details, or <c>null</c> if the variable doesn't exist in the
        /// scope. </returns>
        internal DeclaredVariable FindStaticVariable(string variableName)
        {
            if (variableName == null)
                throw new ArgumentNullException(nameof(variableName));
            Scope scope = this;
            while (scope != null && (scope.Type == ScopeType.Block || scope.Type == ScopeType.TopLevelFunction || scope.Type == ScopeType.EvalStrict))
            {
                if (scope.variables.TryGetValue(variableName, out var variable))
                    return variable;
                scope = scope.ParentScope;
            }
            return null;
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
        /// Declares a variable or function in this scope.  This will be initialized with the value
        /// of the given expression.
        /// </summary>
        /// <param name="keyword"> The keyword that was used to declare the variable (var, let or
        /// const). </param>
        /// <param name="name"> The name of the variable. </param>
        /// <param name="hoistedFunction"> The function value to hoist to the top of the scope.
        /// Should be <c>null</c> for everything except function declarations. </param>
        /// <returns> A reference to the variable that was declared. </returns>
        internal void DeclareVariable(KeywordToken keyword, string name, FunctionExpression hoistedFunction = null)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            // If variables cannot be declared in the scope, try the parent scope instead.
            var declarationScope = this;
            if (keyword == KeywordToken.Var)
            {
                while (declarationScope != null && (declarationScope.Type == ScopeType.Block || declarationScope.Type == ScopeType.With))
                    declarationScope = declarationScope.ParentScope;
            }

            if (declarationScope != null && !declarationScope.variables.ContainsKey(name))
            {
                // This is a local variable that has not been declared before.
                var variable = new DeclaredVariable()
                {
                    Scope = declarationScope,
                    Index = declarationScope.variables.Count,
                    Keyword = keyword,
                    Name = name,
                };
                declarationScope.variables.Add(name, variable);
            }

            if (hoistedFunction != null)
            {
                var hoistedScope = this;
                while (hoistedScope != null && hoistedScope.Type == ScopeType.Block)
                    hoistedScope = hoistedScope.ParentScope;
                if (hoistedScope.hoistedFunctions == null)
                    hoistedScope.hoistedFunctions = new Dictionary<string, FunctionExpression>();
                hoistedScope.hoistedFunctions[name] = hoistedFunction;
            }
        }

        /// <summary>
        /// Generates code that creates a new scope.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        internal void GenerateScopeCreation(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Make sure we don't generate the scope twice.
            if (GenerateScopeCreationWasCalled)
                return;
            GenerateScopeCreationWasCalled = true;

            // We can optimize this away if there are zero variables declared in the scope,
            // UNLESS it's a with scope (as then we need something to bind to).
            if (this.variables.Count == 0 && Type != ScopeType.With)
                return;

            // If there is no eval(), no arguments usage and no nested functions, then we can use
            // IL variables instead of using RuntimeScope.
            if ((Type == ScopeType.TopLevelFunction || Type == ScopeType.Block) &&
                optimizationInfo.OptimizeDeclarativeScopes)
            {
                foreach (var variable in this.variables.Values)
                {
                    variable.Store = generator.DeclareVariable(variable.Type, variable.Name);
                    if (variable.Type == PrimitiveType.Any)
                    {
                        generator.LoadNull();
                        generator.StoreVariable(variable.Store);
                    }
                }
                return;
            }

            // The fallback: use RuntimeScope.
            EmitHelpers.LoadExecutionContext(generator);

            // parentScope
            if (ParentScope != null)
                ParentScope.GenerateReference(generator, optimizationInfo);
            else
                generator.LoadNull();

            var varList = new List<DeclaredVariable>();
            var letList = new List<DeclaredVariable>();
            var constList = new List<DeclaredVariable>();
            foreach (var variable in this.variables.Values)
            {
                if (variable.Keyword == KeywordToken.Var)
                    varList.Add(variable);
                else if (variable.Keyword == KeywordToken.Const)
                    constList.Add(variable);
                else
                    letList.Add(variable);
            }
            varList.Sort((a, b) => a.Index - b.Index);
            letList.Sort((a, b) => a.Index - b.Index);
            constList.Sort((a, b) => a.Index - b.Index);
            int i;

            // scopeType
            generator.LoadEnumValue(Type);

            // varNames
            if (varList.Count == 0)
                generator.LoadNull();
            else
            {
                generator.LoadInt32(varList.Count);
                generator.NewArray(typeof(string));
                i = 0;
                foreach (var variable in varList)
                {
                    generator.Duplicate();
                    generator.LoadInt32(i++);
                    generator.LoadString(variable.Name);
                    generator.StoreArrayElement(typeof(string));
                }
            }

            // letNames
            if (letList.Count == 0)
                generator.LoadNull();
            else
            {
                generator.LoadInt32(letList.Count);
                generator.NewArray(typeof(string));
                i = 0;
                foreach (var variable in letList)
                {
                    generator.Duplicate();
                    generator.LoadInt32(i++);
                    generator.LoadString(variable.Name);
                    generator.StoreArrayElement(typeof(string));
                }
            }

            // constNames
            if (constList.Count == 0)
                generator.LoadNull();
            else
            {
                generator.LoadInt32(constList.Count);
                generator.NewArray(typeof(string));
                i = 0;
                foreach (var variable in constList)
                {
                    generator.Duplicate();
                    generator.LoadInt32(i++);
                    generator.LoadString(variable.Name);
                    generator.StoreArrayElement(typeof(string));
                }
            }

            // executionContext.CreateRuntimeScope(parentScope, varNames, letNames, constNames)
            generator.Call(ReflectionHelpers.ExecutionContext_CreateRuntimeScope);

            // Store the RuntimeScope instance in a variable.
            GeneratedRuntimeScope = generator.DeclareVariable(typeof(RuntimeScope), "scope");
            generator.StoreVariable(GeneratedRuntimeScope);
        }

        /// <summary>
        /// Generates code that pushes a RuntimeScope instance to the top of the stack.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        internal void GenerateReference(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            if (!GenerateScopeCreationWasCalled)
                GenerateScopeCreation(generator, optimizationInfo);
            if (GeneratedRuntimeScope != null)
                generator.LoadVariable(GeneratedRuntimeScope);
            else if (ParentScope != null)
                ParentScope.GenerateReference(generator, optimizationInfo);
            else
            {
                EmitHelpers.LoadExecutionContext(generator);
                generator.CallVirtual(ReflectionHelpers.ExecutionContext_ParentScope);
            }
        }

        /// <summary>
        /// Generates code that initializes the variable and function declarations.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        internal void GenerateHoistedDeclarations(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            if (this.hoistedFunctions != null)
            {
                foreach (var nameAndValue in this.hoistedFunctions)
                {
                    // Make sure the scope is valid.
                    nameAndValue.Value.Scope.GenerateScopeCreation(generator, optimizationInfo);

                    // Create the function.
                    nameAndValue.Value.GenerateCode(generator, optimizationInfo);

                    // Assign it to the variable.
                    var name = new NameExpression(this, nameAndValue.Key);
                    name.GenerateSet(generator, optimizationInfo, nameAndValue.Value.ResultType);
                }

                // In case GenerateHoistedDeclarations() is called twice.
                this.hoistedFunctions = null;
            }
        }
    }

}