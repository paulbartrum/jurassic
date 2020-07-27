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
            var result = new RuntimeScope(engine, null, null);
            result.BindTo(engine.Global);
            return result;
        }

        /// <summary>
        /// Creates a new RuntimeScope instance.
        /// </summary>
        /// <param name="engine"> The script engine this scope is associated with. </param>
        /// <param name="parent"> The parent scope, or <c>null</c> if this is the root scope. </param>
        /// <param name="declaredVariableNames"> A list of variable names that were declared in this scope. </param>
        public RuntimeScope(ScriptEngine engine, RuntimeScope parent, string[] declaredVariableNames)
        {
            this.Engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.Parent = parent;
            if (declaredVariableNames != null)
            {
                values = new Dictionary<string, object>(declaredVariableNames.Length);
                foreach (string variableName in declaredVariableNames)
                    values[variableName] = Undefined.Value;
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
        /// Binds the scope to a scope object. This is used by the 'with' statement.
        /// </summary>
        /// <param name="scopeObject"> The object to use. </param>
        public void BindTo(object scopeObject)
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
            if (values != null && values.TryGetValue(variableName, out var value))
                return value;
            if (ScopeObject != null)
                return ScopeObject[variableName];
            if (Parent != null)
                return Parent.GetValue(variableName);
            throw new JavaScriptException(Engine, ErrorType.ReferenceError, $"{variableName} is not defined.");
        }

        /// <summary>
        /// Sets the value of the given variable.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        /// <param name="value"> The new value of the variable. </param>
        public void SetValue(string variableName, object value)
        {
            if (values != null && values.ContainsKey(variableName))
                values[variableName] = value;
            else if (ScopeObject != null)
                ScopeObject[variableName] = value;
            else if (Parent != null)
                Parent.SetValue(variableName, value);
            else
                throw new InvalidOperationException("urgh");
        }

        /// <summary>
        /// Deletes the variable from the scope.
        /// </summary>
        /// <param name="variableName"> The name of the variable. </param>
        public bool Delete(string variableName)
        {
            return this.ScopeObject.Delete(variableName, false);
            return false;
        }
    }
}