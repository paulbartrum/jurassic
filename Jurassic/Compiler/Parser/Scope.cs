using System;
using System.Collections.Generic;
using System.Linq;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents an enclosing context where variables are uniquely defined.
    /// </summary>
    public sealed class Scope
    {
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

            // The name of the variable.
            public string Name;

            // The initial value of the variable (used for function declarations only).
            [NonSerialized]
            public Expression ValueAtTopOfScope;

            // true if the variable can be modified.
            public bool Writable;

            // true if the variable can be deleted.
            public bool Deletable;

            // The storage container for the variable.
            [NonSerialized]
            public ILLocalVariable Store;

            // The statically-determined storage type for the variable.
            [NonSerialized]
            public PrimitiveType Type = PrimitiveType.Any;
        }

        /// <summary>
        /// Creates a new declarative scope for use inside a function body (and within function
        /// argument default values).
        /// </summary>
        /// <param name="parentScope"> A reference to the parent scope.  Can not be <c>null</c>. </param>
        /// <param name="functionName"> The name of the function.  Can be empty for an anonymous function. </param>
        /// <param name="argumentNames"> The names of each of the function arguments.  Can be <c>null</c>. </param>
        /// <returns> A new DeclarativeScope instance. </returns>
        internal static Scope CreateFunctionScope(Scope parentScope, string functionName, IEnumerable<string> argumentNames)
        {
            if (parentScope == null)
                throw new ArgumentNullException("parentScope", "Function scopes must have a parent scope.");
            var result = new Scope(parentScope, (argumentNames != null ? argumentNames.Count() : 0) + 3);
            result.IsVarScope = true;
            if (functionName != null)
                result.DeclareVariable(KeywordToken.Let, functionName);
            result.DeclareVariable(KeywordToken.Let, "arguments");
            if (argumentNames != null)
            {
                foreach (var argumentName in argumentNames)
                    result.DeclareVariable(KeywordToken.Let, argumentName);
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
            return new Scope(parentScope, 0);
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
            return new Scope(parentScope, 0) { IsVarScope = true };
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
            this.IsVarScope = false;
        }

        /// <summary>
        /// Gets a reference to the parent scope.  Can be <c>null</c> if this is the global scope.
        /// </summary>
        public Scope ParentScope { get; private set; }

        /// <summary>
        /// Gets a value indicating whether 'var' variables can be declared within this scope.
        /// </summary>
        public bool IsVarScope { get; set; }

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
        /// Gets an enumerable list of the declared variables, in no particular order.
        /// </summary>
        internal IEnumerable<DeclaredVariable> DeclaredVariables
        {
            get { return this.variables.Values; }
        }

        /// <summary>
        /// Gets the index of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <returns> The index of the given variable, or <c>-1</c> if the variable doesn't exist
        /// in the scope. </returns>
        internal DeclaredVariable GetDeclaredVariable(string variableName)
        {
            if (variableName == null)
                throw new ArgumentNullException(nameof(variableName));
            DeclaredVariable variable;
            if (this.variables.TryGetValue(variableName, out variable) == false)
                return null;
            return variable;
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
        /// <param name="name"> The name of the variable. </param>
        /// <param name="valueAtTopOfScope"> The value of the variable at the top of the scope.
        /// Can be <c>null</c> to indicate the variable does not need initializing. </param>
        /// <param name="writable"> <c>true</c> if the variable can be modified; <c>false</c>
        /// otherwise.  Defaults to <c>true</c>. </param>
        /// <param name="deletable"> <c>true</c> if the variable can be deleted; <c>false</c>
        /// otherwise.  Defaults to <c>true</c>. </param>
        /// <returns> A reference to the variable that was declared. </returns>
        internal void DeclareVariable(KeywordToken keyword, string name, Expression valueAtTopOfScope = null, bool writable = true, bool deletable = false)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            // If variables cannot be declared in the scope, try the parent scope instead.
            if (keyword == KeywordToken.Var && this.IsVarScope == false)
            {
                // If we run out of scopes, then the variable is defined by the (runtime-only) parent scope.
                if (this.ParentScope == null)
                    return;
                this.ParentScope.DeclareVariable(keyword, name, valueAtTopOfScope, writable, deletable);
            }

            DeclaredVariable variable;
            this.variables.TryGetValue(name, out variable);
            if (variable == null)
            {
                // This is a local variable that has not been declared before.
                variable = new DeclaredVariable()
                {
                    Scope = this,
                    Index = this.variables.Count,
                    Name = name,
                    Writable = writable && keyword != KeywordToken.Const,
                    Deletable = deletable
                };
                this.variables.Add(name, variable);
            }

            // Set the initial value, if one was provided.
            if (valueAtTopOfScope != null)
            {
                // Function expressions override literals.
                if ((valueAtTopOfScope is LiteralExpression && variable.ValueAtTopOfScope is FunctionExpression) == false)
                    variable.ValueAtTopOfScope = valueAtTopOfScope;
            }
        }

        /// <summary>
        /// Generates code that creates a new scope.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        //internal abstract void GenerateScopeCreation(ILGenerator generator, OptimizationInfo optimizationInfo);

        /// <summary>
        /// Generates code that initializes the variable and function declarations.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /*internal virtual void GenerateDeclarations(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Initialize the declared variables and functions.
            foreach (var variable in this.variables.Values)
            {
                // Emit the initialization code.
                if (this is ObjectScope)
                {
                    // Determine the property attributes.
                    var attributes = Library.PropertyAttributes.Enumerable;
                    if (variable.Writable == true)
                        attributes |= Library.PropertyAttributes.Writable;
                    if (variable.Deletable == true)
                        attributes |= Library.PropertyAttributes.Configurable;

                    if (variable.ValueAtTopOfScope == null)
                    {
                        // void InitializeMissingProperty(object key, PropertyAttributes attributes)
                        EmitHelpers.LoadScope(generator);
                        generator.CastClass(typeof(ObjectScope));
                        generator.Call(ReflectionHelpers.ObjectScope_ScopeObject);
                        generator.LoadString(variable.Name);
                        generator.LoadInt32((int)attributes);
                        generator.Call(ReflectionHelpers.ObjectInstance_InitializeMissingProperty);
                    }
                    else
                    {
                        // bool DefineProperty(string propertyName, PropertyDescriptor descriptor, bool throwOnError)
                        EmitHelpers.LoadScope(generator);
                        generator.CastClass(typeof(ObjectScope));
                        generator.Call(ReflectionHelpers.ObjectScope_ScopeObject);
                        generator.LoadString(variable.Name);
                        variable.ValueAtTopOfScope.GenerateCode(generator, optimizationInfo);
                        EmitConversion.Convert(generator, variable.ValueAtTopOfScope.ResultType, PrimitiveType.Any, optimizationInfo);
                        generator.LoadInt32((int)attributes);
                        generator.NewObject(ReflectionHelpers.PropertyDescriptor_Constructor2);
                        generator.LoadBoolean(false);
                        generator.Call(ReflectionHelpers.ObjectInstance_DefineProperty);
                        generator.Pop();
                    }
                }
                else if (variable.ValueAtTopOfScope != null)
                {
                    variable.ValueAtTopOfScope.GenerateCode(generator, optimizationInfo);
                    var name = new NameExpression(this, variable.Name);
                    name.GenerateSet(generator, optimizationInfo, variable.ValueAtTopOfScope.ResultType, false);
                }
            }
        }*/

        /// <summary>
        /// Generates code that restores the parent scope as the active scope.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /*internal void GenerateScopeDestruction(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            if (this.ExistsAtRuntime == false)
                return;

            // Modify the scope variable so it points at the parent scope.
            EmitHelpers.LoadScope(generator);
            generator.Call(ReflectionHelpers.Scope_ParentScope);
            EmitHelpers.StoreScope(generator);
        }*/

        
        internal void GenerateReference(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            if (GeneratedRuntimeScope == null)
                throw new InvalidOperationException("Call GenerateScopeCreation() first.");
            generator.LoadVariable(GeneratedRuntimeScope);
        }

        /// <summary>
        /// Generates code that creates a new scope.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        internal void GenerateScopeCreation(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // TODO:
            // optimizationInfo.OptimizeDeclarativeScopes

            EmitHelpers.LoadExecutionContext(generator);

            // parentScope
            generator.LoadNull();

            // declaredVariableNames
            generator.LoadInt32(this.DeclaredVariableCount);
            generator.NewArray(typeof(string));
            int i = 0;
            foreach (string variableName in this.DeclaredVariableNames)
            {
                generator.Duplicate();
                generator.LoadInt32(i++);
                generator.LoadString(variableName);
                generator.StoreArrayElement(typeof(string));
            }

            // executionContext.CreateRuntimeScope(parentScope, declaredVariableNames)
            generator.Call(ReflectionHelpers.ExecutionContext_CreateRuntimeScope);

            // Store the RuntimeScope instance in a variable.
            GeneratedRuntimeScope = generator.DeclareVariable(typeof(RuntimeScope), "scope");
            generator.StoreVariable(GeneratedRuntimeScope);
        }

        /// <summary>
        /// Generates code that initializes the variable and function declarations.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        internal void GenerateDeclarations(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Initialize the declared variables and functions.
            /*foreach (var variable in this.variables.Values)
            {
                // Emit the initialization code.
                if (this is ObjectScope)
                {
                    // Determine the property attributes.
                    var attributes = Library.PropertyAttributes.Enumerable;
                    if (variable.Writable == true)
                        attributes |= Library.PropertyAttributes.Writable;
                    if (variable.Deletable == true)
                        attributes |= Library.PropertyAttributes.Configurable;

                    if (variable.ValueAtTopOfScope == null)
                    {
                        // void InitializeMissingProperty(object key, PropertyAttributes attributes)
                        EmitHelpers.LoadScope(generator);
                        generator.CastClass(typeof(ObjectScope));
                        generator.Call(ReflectionHelpers.ObjectScope_ScopeObject);
                        generator.LoadString(variable.Name);
                        generator.LoadInt32((int)attributes);
                        generator.Call(ReflectionHelpers.ObjectInstance_InitializeMissingProperty);
                    }
                    else
                    {
                        // bool DefineProperty(string propertyName, PropertyDescriptor descriptor, bool throwOnError)
                        EmitHelpers.LoadScope(generator);
                        generator.CastClass(typeof(ObjectScope));
                        generator.Call(ReflectionHelpers.ObjectScope_ScopeObject);
                        generator.LoadString(variable.Name);
                        variable.ValueAtTopOfScope.GenerateCode(generator, optimizationInfo);
                        EmitConversion.Convert(generator, variable.ValueAtTopOfScope.ResultType, PrimitiveType.Any, optimizationInfo);
                        generator.LoadInt32((int)attributes);
                        generator.NewObject(ReflectionHelpers.PropertyDescriptor_Constructor2);
                        generator.LoadBoolean(false);
                        generator.Call(ReflectionHelpers.ObjectInstance_DefineProperty);
                        generator.Pop();
                    }
                }
                else if (variable.ValueAtTopOfScope != null)
                {
                    variable.ValueAtTopOfScope.GenerateCode(generator, optimizationInfo);
                    var name = new NameExpression(this, variable.Name);
                    name.GenerateSet(generator, optimizationInfo, variable.ValueAtTopOfScope.ResultType, false);
                }
            }*/
        }
    }

}