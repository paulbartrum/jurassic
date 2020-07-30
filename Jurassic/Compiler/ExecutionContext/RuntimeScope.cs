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
    ///   var g = ReflectionHelpers.CreateFunction(..., scope3, ...)
    ///   scope1.SetValue("a", 5);
    ///   var scope2 = executionContext.CreateRuntimeScope(scope1);
    ///   scope2.SetValue("a", 4);
    ///   g.Call()
    ///   ((FunctionInstance)TypeConverter.ToObject(scope2.GetValue("console"))["log"]).Call(scope2.GetValue("a"));
    /// }
    /// 
    /// object g(ExecutionContext executionContext, object[] arguments)
    /// {
    ///   var scope1 = executionContext.CreateRuntimeScope(null);
    ///   scope1.SetValue("a", 6);
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
        private Dictionary<string, object> values;

        public static RuntimeScope CreateGlobalScope(ScriptEngine engine)
        {
            var result = new RuntimeScope(engine, null, null, null, null);
            result.With(engine.Global);
            return result;
        }

        /// <summary>
        /// Creates a new RuntimeScope instance.
        /// </summary>
        /// <param name="engine"> The script engine this scope is associated with. </param>
        /// <param name="parent"> The parent scope, or <c>null</c> if this is the root scope. </param>
        /// <param name="varNames"></param>
        /// <param name="letNames"></param>
        /// <param name="constNames"></param>
        public RuntimeScope(ScriptEngine engine, RuntimeScope parent, string[] varNames, string[] letNames, string[] constNames)
        {
            this.Engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.Parent = parent;
            if (varNames != null)
            {
                var scope = this;
                while (scope != null && scope.ScopeObject == null)
                    scope = scope.Parent;
                if (scope != null)
                {
                    foreach (var name in varNames)
                        scope.ScopeObject.InitializeMissingProperty(name, PropertyAttributes.Enumerable | PropertyAttributes.Writable);
                }
            }
            if (letNames != null || constNames != null)
            {
                values = new Dictionary<string, object>(
                    (varNames != null ? varNames.Length : 0) +
                    (constNames != null ? constNames.Length : 0));
            }
            if (letNames != null)
            {
                foreach (string variableName in letNames)
                    values[variableName] = null;
            }
            if (constNames != null)
            {
                foreach (string variableName in constNames)
                    values[variableName] = null;
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
            ScopeObject = TypeConverter.ToObject(Engine, scopeObject);
        }

        /// <summary>
        /// Returns the value of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <returns> The value of the given variable, or <c>null</c> if the variable doesn't exist
        /// in the scope. </returns>
        public object GetValue(string variableName)
        {
            var scope = this;
            do
            {
                if (scope.values != null && scope.values.TryGetValue(variableName, out var value))
                {
                    if (value == null)
                        throw new JavaScriptException(Engine, ErrorType.ReferenceError, $"Cannot access '{variableName}' before initialization.");
                    return value;
                }
                if (scope.ScopeObject != null)
                {
                    var result = scope.ScopeObject[variableName];
                    if (result != null)
                        return result;
                }
                scope = scope.Parent;
            } while (scope != null);
            throw new JavaScriptException(Engine, ErrorType.ReferenceError, $"{variableName} is not defined.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public object GetValueNoThrow(string variableName)
        {
            var scope = this;
            do
            {
                if (scope.values != null && scope.values.TryGetValue(variableName, out var value))
                {
                    if (value == null)
                        throw new JavaScriptException(Engine, ErrorType.ReferenceError, $"Cannot access '{variableName}' before initialization.");
                    return value;
                }
                if (scope.ScopeObject != null)
                {
                    var result = scope.ScopeObject[variableName];
                    if (result != null)
                        return result;
                }
                scope = scope.Parent;
            } while (scope != null);
            return Undefined.Value;
        }

        /// <summary>
        /// Sets the value of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="value"> The new value of the variable. </param>
        public void SetValue(string variableName, object value)
        {
            var scope = this;
            do
            {
                if (scope.values != null && scope.values.ContainsKey(variableName))
                {
                    values[variableName] = value;
                    return;
                }
                if (scope.ScopeObject != null)
                {
                    // If there's no parent and the property doesn't exist, set it anyway.
                    if (scope.Parent == null)
                    {
                        scope.ScopeObject[variableName] = value;
                        return;
                    }

                    // For a with() scope, only set the value if it exists.
                    var exists = scope.ScopeObject.SetPropertyValueIfExists(variableName, value, throwOnError: false);
                    if (exists)
                        return;
                }
                scope = scope.Parent;
            } while (scope != null);
            throw new InvalidOperationException("Expected scope with ScopeObject at root of chain.");
        }

        /// <summary>
        /// Sets the value of the given variable, using strict mode behaviour.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="value"> The new value of the variable. </param>
        public void SetValueStrict(string variableName, object value)
        {
            var scope = this;
            do
            {
                if (scope.values != null && scope.values.ContainsKey(variableName))
                {
                    values[variableName] = value;
                    return;
                }
                if (scope.ScopeObject != null)
                {
                    // Only set the value if it exists.
                    var exists = scope.ScopeObject.SetPropertyValueIfExists(variableName, value, throwOnError: true);
                    if (exists)
                        return;
                    if (scope.Parent == null)
                        throw new JavaScriptException(Engine, ErrorType.ReferenceError, $"{variableName} is not defined.");
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
            if (values != null && values.TryGetValue(variableName, out var value))
                return false;   // Can't delete a local variable.
            if (ScopeObject != null)
                return ScopeObject.Delete(variableName, false);
            if (Parent != null)
                return Parent.Delete(variableName);
            return false;
        }
    }
}