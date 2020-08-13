using Jurassic.Library;
using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// A place for storing captured variable values at runtime.
    /// 
    /// So this JS code:
    /// 
    /// function f() {
    ///   let a = 1;
    ///   {
    ///     let a = 2;
    ///     (function g() {
    ///       a = 3;
    ///     })();
    ///     console.log(a); // Logs 3.
    ///   }
    /// }
    /// 
    /// Translates to something like this (in C#):
    /// 
    /// object f(ExecutionContext executionContext, object[] arguments)
    /// {
    ///   var scope1 = executionContext.CreateRuntimeScope(null);
    ///   var scope2 = executionContext.CreateRuntimeScope(scope1);
    ///   var g = ReflectionHelpers.CreateFunction(..., scope2, ...)
    ///   scope1.SetValue("a", 1);
    ///   scope2.SetValue("a", 2);
    ///   g.Call()
    ///   ((FunctionInstance)TypeConverter.ToObject(scope2.GetValue("console"))["log"]).Call(scope2.GetValue("a"));
    /// }
    /// 
    /// object g(ExecutionContext executionContext, object[] arguments)
    /// {
    ///   executionContext.ParentScope.SetValue("a", 3);
    /// }
    /// 
    /// The with(...) statement is handled specially:
    /// 
    /// function f() {
    ///   with (Math) {
    ///     console.log(E); // Logs 2.718281828459045.
    ///   }
    /// }
    /// 
    /// C# translation:
    /// 
    /// object f(ExecutionContext executionContext, object[] arguments)
    /// {
    ///   var scope1 = executionContext.CreateRuntimeScope(null);
    ///   var scope2 = executionContext.CreateRuntimeScope(scope1);
    ///   scope2.BindTo(scope1.GetValue("Math"));
    ///   ((FunctionInstance)TypeConverter.ToObject(scope2.GetValue("console"))["log"]).Call(scope2.GetValue("E"));
    /// }
    /// </summary>
    public sealed class RuntimeScope
    {
        [Flags]
        private enum LocalFlags
        {
            Deletable = 1,
            ReadOnly = 2,
        }

        private struct LocalValue
        {
            public LocalFlags Flags;

            public object Value;

            public override string ToString()
            {
                return Value == null ? string.Empty : Value.ToString();
            }
        }

        private Dictionary<string, LocalValue> values;

        /// <summary>
        /// Creates a global scope.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <returns> A new RuntimeScope instance. </returns>
        public static RuntimeScope CreateGlobalScope(ScriptEngine engine)
        {
            return new RuntimeScope(engine);
        }

        private RuntimeScope(ScriptEngine engine)
        {
            this.Engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.Parent = null;
            this.ScopeType = ScopeType.Global;
            this.ScopeObject = engine.Global;
        }

        /// <summary>
        /// Creates a new RuntimeScope instance.
        /// </summary>
        /// <param name="engine"> The script engine this scope is associated with. </param>
        /// <param name="parent"> The parent scope, or <c>null</c> if this is the root scope. </param>
        /// <param name="scopeType"></param>
        /// <param name="varNames"></param>
        /// <param name="letNames"></param>
        /// <param name="constNames"></param>
        internal RuntimeScope(ScriptEngine engine, RuntimeScope parent, ScopeType scopeType,
            string[] varNames, string[] letNames, string[] constNames)
        {
            this.Engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.Parent = parent;
            this.ScopeType = scopeType;
            if (varNames != null && scopeType != ScopeType.TopLevelFunction && scopeType != ScopeType.EvalStrict)
            {
                var scope = this;
                while (scope != null && scope.ScopeObject == null &&
                    scope.ScopeType != ScopeType.TopLevelFunction && scopeType != ScopeType.EvalStrict)
                    scope = scope.Parent;
                if (scope != null)
                {
                    if (scope.ScopeType == ScopeType.TopLevelFunction || scopeType == ScopeType.EvalStrict)
                    {
                        foreach (var name in varNames)
                            if (!scope.values.ContainsKey(name))
                                scope.values[name] = new LocalValue { Value = Undefined.Value, Flags = LocalFlags.Deletable };
                    }
                    else
                    {
                        var attributes = scopeType == ScopeType.Eval ? PropertyAttributes.FullAccess : PropertyAttributes.Enumerable | PropertyAttributes.Writable;
                        foreach (var name in varNames)
                            scope.ScopeObject.InitializeMissingProperty(name, attributes);
                    }
                }
                varNames = null;
            }
            if (varNames != null || letNames != null || constNames != null)
            {
                values = new Dictionary<string, LocalValue>(
                    (varNames != null ? varNames.Length : 0) +
                    (letNames != null ? letNames.Length : 0) +
                    (constNames != null ? constNames.Length : 0));
            }
            if (varNames != null)
            {
                foreach (string variableName in varNames)
                    values[variableName] = new LocalValue { Value = Undefined.Value };
            }
            if (letNames != null)
            {
                foreach (string variableName in letNames)
                    values[variableName] = new LocalValue();
            }
            if (constNames != null)
            {
                foreach (string variableName in constNames)
                    values[variableName] = new LocalValue { Flags = LocalFlags.ReadOnly };
            }
        }

        /// <summary>
        /// The current execution context.
        /// </summary>
        public ScriptEngine Engine { get; private set; }

        /// <summary>
        /// A reference to the parent scope. If a variable cannot be found in this scope then the
        /// parent scope can be checked instead.
        /// </summary>
        public RuntimeScope Parent { get; private set; }

        /// <summary>
        /// Gets the type of scope, e.g. global, function, eval, with, etc.
        /// </summary>
        public ScopeType ScopeType { get; private set; }

        /// <summary>
        /// Gets the object that stores the values of the variables in the scope, if any. Can be <c>null</c>.
        /// </summary>
        public ObjectInstance ScopeObject { get; private set; }

        /// <summary>
        /// Determines the 'this' value passed to a function when the function call is of the form
        /// simple_func(). This is normally 'undefined' but can be some other value inside a with()
        /// statement.
        /// </summary>
        public object ImplicitThis
        {
            get
            {
                var scope = this;
                while (scope.Parent != null)
                {
                    if (scope.ScopeObject != null)
                        return scope.ScopeObject;
                    scope = scope.Parent;
                }
                return Undefined.Value;
            }
        }

        /// <summary>
        /// Binds the scope to a scope object. This is used by the 'with' statement.
        /// </summary>
        /// <param name="scopeObject"> The object to use. </param>
        public void With(object scopeObject)
        {
            if (scopeObject == null)
                throw new ArgumentNullException(nameof(scopeObject));
            ScopeObject = TypeConverter.ToObject(Engine, scopeObject);
        }

        /// <summary>
        /// Returns the value of the given variable. An error is thrown if the variable doesn't
        /// exist.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="lineNumber"> The line number in the source file the variable was accessed. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        /// <returns> The value of the given variable. </returns>
        public object GetValue(string variableName, int lineNumber, string sourcePath)
        {
            object result = GetValueCore(variableName, lineNumber, sourcePath);
            if (result == null)
                throw new JavaScriptException(Engine, ErrorType.ReferenceError, $"{variableName} is not defined.", lineNumber, sourcePath);
            return result;
        }

        /// <summary>
        /// Returns the value of the given variable. Returns <see cref="Undefined.Value"/> if the
        /// variable doesn't exist.
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="lineNumber"> The line number in the source file the variable was accessed. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        /// <returns> The value of the given variable, or <see cref="Undefined.Value"/> if the
        /// variable doesn't exist. </returns>
        public object GetValueNoThrow(string variableName, int lineNumber, string sourcePath)
        {
            return GetValueCore(variableName, lineNumber, sourcePath) ?? Undefined.Value;
        }

        /// <summary>
        /// Returns the value of the given variable. Returns <c>null</c> if the variable doesn't
        /// exist.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="lineNumber"> The line number in the source file the variable was accessed. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        /// <returns> The value of the given variable, or <c>null</c> if the variable doesn't exist
        /// in the scope. </returns>
        private object GetValueCore(string variableName, int lineNumber, string sourcePath)
        {
            if (variableName == null)
                throw new ArgumentNullException(nameof(variableName));
            var scope = this;
            do
            {
                if (scope.values != null && scope.values.TryGetValue(variableName, out var localValue))
                {
                    if (localValue.Value == null)
                        throw new JavaScriptException(Engine, ErrorType.ReferenceError, $"Cannot access '{variableName}' before initialization.", lineNumber, sourcePath);
                    return localValue.Value;
                }
                if (scope.ScopeObject != null)
                {
                    var result = scope.ScopeObject.GetPropertyValue(variableName);
                    if (result != null)
                        return result;
                }
                scope = scope.Parent;
            } while (scope != null);
            return null;
        }

        /// <summary>
        /// Sets the value of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="value"> The new value of the variable. </param>
        /// <param name="lineNumber"> The line number in the source file the variable was set. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        public void SetValue(string variableName, object value, int lineNumber, string sourcePath)
        {
            SetValueCore(variableName, value, strictMode: false, lineNumber, sourcePath);
        }

        /// <summary>
        /// Sets the value of the given variable, using strict mode behaviour.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="value"> The new value of the variable. </param>
        /// <param name="lineNumber"> The line number in the source file the variable was set. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        public void SetValueStrict(string variableName, object value, int lineNumber, string sourcePath)
        {
            SetValueCore(variableName, value, strictMode: true, lineNumber, sourcePath);
        }

        /// <summary>
        /// Sets the value of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="value"> The new value of the variable. </param>
        /// <param name="strictMode"> Indicates whether to use strict mode behaviour when setting
        /// the variable. </param>
        /// <param name="lineNumber"> The line number in the source file the variable was set. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        private void SetValueCore(string variableName, object value, bool strictMode, int lineNumber, string sourcePath)
        {
            if (variableName == null)
                throw new ArgumentNullException(nameof(variableName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var scope = this;
            do
            {
                if (scope.values != null && scope.values.TryGetValue(variableName, out var localValue))
                {
                    if (localValue.Value != null && localValue.Flags.HasFlag(LocalFlags.ReadOnly))
                        throw new JavaScriptException(Engine, ErrorType.TypeError, $"Illegal assignment to constant variable '{variableName}'.", lineNumber, sourcePath);
                    scope.values[variableName] = new LocalValue { Value = value, Flags = localValue.Flags };
                    return;
                }
                if (scope.ScopeObject != null)
                {
                    // If there's no parent and the property doesn't exist, set it anyway.
                    if (!strictMode && scope.Parent == null)
                    {
                        scope.ScopeObject[variableName] = value;
                        return;
                    }

                    // Only set the value if it exists.
                    var exists = scope.ScopeObject.SetPropertyValueIfExists(variableName, value, throwOnError: strictMode);
                    if (exists)
                        return;
                    
                    // Strict mode: throw an exception if the variable is undefined.
                    if (scope.Parent == null)
                        throw new JavaScriptException(Engine, ErrorType.ReferenceError, $"{variableName} is not defined.", lineNumber, sourcePath);
                }
                scope = scope.Parent;
            } while (scope != null);
            throw new InvalidOperationException("Expected scope with ScopeObject at root of chain.");
        }

        /// <summary>
        /// Deletes the variable from the scope.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        public bool Delete(string variableName)
        {
            if (variableName == null)
                throw new ArgumentNullException(nameof(variableName));
            if (values != null && values.TryGetValue(variableName, out var localValue))
            {
                if (localValue.Flags.HasFlag(LocalFlags.Deletable))
                {
                    values.Remove(variableName);
                    return true;
                }
                return false;
            }
            if (ScopeObject != null)
                return ScopeObject.Delete(variableName, false);
            if (Parent != null)
                return Parent.Delete(variableName);
            return false;
        }
    }
}