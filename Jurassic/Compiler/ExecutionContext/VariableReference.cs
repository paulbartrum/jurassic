using Jurassic.Library;
using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a 'slow' variable reference. Used for global variables and variable references
    /// within a with() statement, and other slow cases.
    /// </summary>
    internal class VariableReference
    {
        public VariableReference(ExecutionContext executionContext, string name)
        {
            ExecutionContext = executionContext;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets the associated execution context.
        /// </summary>
        public ExecutionContext ExecutionContext { get; private set; }

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the value of the variable.
        /// </summary>
        public object Value
        {
            get
            {
                var bindings = ExecutionContext.VariableBindings;
                while (bindings != null)
                {
                    var value = bindings.GetValue(Name);
                    if (value != null)
                        return value;
                    bindings = bindings.Parent;
                }
                throw new JavaScriptException(ExecutionContext.Engine, ErrorType.ReferenceError, $"{Name} is not defined.");
            }
            set
            {
                /*var bindings = ExecutionContext.VariableBindings;
                while (bindings != null)
                {
                    var value = bindings.SetValue(Name, value);
                    if (value != null)
                        return value;
                    bindings = bindings.Parent;
                }*/
                throw new JavaScriptException(ExecutionContext.Engine, ErrorType.ReferenceError, $"{Name} is not defined.");
            }
        }
    }
}