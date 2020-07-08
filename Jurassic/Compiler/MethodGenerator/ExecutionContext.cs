using Jurassic.Library;
using System;
using System.Diagnostics;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the runtime state needed to run JS code.
    /// </summary>
    public class ExecutionContext
    {
        object thisValue;

        public static ExecutionContext CreateGlobalContext(ScriptEngine engine, ObjectScope scope)
        {
            // Convert the initial scope into a runtime scope.
            Debug.Assert(scope != null && scope.ScopeObject == null);
            var runtimeScope = scope.ConvertPlaceholderToRuntimeScope(engine.Global);
            return new ExecutionContext(engine, runtimeScope, BindingStatus.Initialized, engine.Global, null, null);
        }

        public static ExecutionContext CreateEvalContext(ScriptEngine engine, Scope scope, object thisValue)
        {
            return new ExecutionContext(engine, scope, BindingStatus.Initialized, thisValue, null, null);
        }

        public static ExecutionContext CreateFunctionContext(ScriptEngine engine, Scope scope, object thisValue, FunctionInstance executingFunction)
        {
            return new ExecutionContext(engine, scope, BindingStatus.Initialized, thisValue, executingFunction, null);
        }

        public static ExecutionContext CreateConstructContext(ScriptEngine engine, Scope scope, object thisValue, FunctionInstance executingFunction, FunctionInstance newTarget)
        {
            return new ExecutionContext(engine, scope, BindingStatus.Initialized, thisValue, executingFunction, newTarget);
        }

        public static ExecutionContext CreateDerivedContext(ScriptEngine engine, Scope scope, FunctionInstance executingFunction, FunctionInstance newTarget)
        {
            return new ExecutionContext(engine, scope, BindingStatus.Uninitialized, null, executingFunction, newTarget);
        }

        private ExecutionContext(ScriptEngine engine, Scope scope,
            BindingStatus thisBindingStatus, object thisValue,
            FunctionInstance executingFunction, FunctionInstance newTarget)
        {
            this.Engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.ThisBindingStatus = thisBindingStatus;
            this.thisValue = thisValue;
            this.ExecutingFunction = executingFunction;
            this.NewTarget = newTarget;
        }

        /// <summary>
        /// A reference to the script engine.
        /// </summary>
        public ScriptEngine Engine { get; private set; }

        /// <summary>
        /// Holds variable bindings.
        /// </summary>
        public Scope Scope { get; set; }

        /// <summary>
        /// Represents the state of the 'this' value.
        /// </summary>
        public enum BindingStatus
        {
            /// <summary>
            /// A 'this' value is available, although it may be null or undefined.
            /// </summary>
            Initialized,

            /// <summary>
            /// 'this' is unavailable because execution is in a derived class constructor and
            /// super() has not yet been called.
            /// </summary>
            Uninitialized,

            /// <summary>
            /// This is an ArrowFunction and does not have a local this value.
            /// </summary>
            Lexical,
        }

        /// <summary>
        /// Indicates the status of the 'this' value.
        /// </summary>
        public BindingStatus ThisBindingStatus { get; private set; }

        /// <summary>
        /// The value of the 'this' keyword.
        /// </summary>
        public object ThisValue
        {
            get
            {
                if (ThisBindingStatus == BindingStatus.Uninitialized)
                    throw new JavaScriptException(Engine, ErrorType.ReferenceError, "Must call super constructor in derived class before accessing 'this' or returning from derived constructor.");
                return thisValue;
            }
        }

        /// <summary>
        /// Converts <see cref="ThisValue"/> to an object. If <c>this</c> is null or undefined,
        /// then it will be set to the global object.
        /// </summary>
        public void ConvertThisToObject()
        {
            if (thisValue is ObjectInstance)
                return;
            if (thisValue == Null.Value || thisValue == Undefined.Value)
                thisValue = Engine.Global;
            else
                thisValue = TypeConverter.ToObject(Engine, thisValue);
        }

        /// <summary>
        /// Corresponds to a super(...argumentValues) call.
        /// </summary>
        /// <param name="argumentValues"> The parameter values to pass to the base class. </param>
        /// <returns> The initialised object instance. </returns>
        public ObjectInstance CallSuperClass(object[] argumentValues)
        {
            if (ThisBindingStatus != BindingStatus.Uninitialized)
                throw new JavaScriptException(Engine, ErrorType.ReferenceError, "Super constructor may only be called once.");

            if (ExecutingFunction.Prototype is FunctionInstance super)
            {
                // Call the base class constructor.
                var result = super.ConstructLateBound(NewTarget, argumentValues);
                ThisBindingStatus = BindingStatus.Initialized;
                thisValue = result;
                return result;
            }
            else
                throw new JavaScriptException(Engine, ErrorType.TypeError, $"Super constructor {ExecutingFunction.Prototype} of {ExecutingFunction.Name} is not a constructor.");
        }

        /// <summary>
        /// A reference to the executing function. Will be <c>null</c> if running in a global or
        /// eval context.
        /// </summary>
        public FunctionInstance ExecutingFunction { get; private set; }

        /// <summary>
        /// If this context was created by the 'new' operator, contains the target of the new
        /// operator. This value can be accessed by JS using the 'new.target' expression.
        /// </summary>
        public FunctionInstance NewTarget { get; private set; }

        /// <summary>
        /// The same as <see cref="NewTarget"/> except that it returns <see cref="Undefined.Value"/>
        /// instead of <c>null</c> if no new.target value is available.
        /// </summary>
        public object NewTargetObject
        {
            get { return (object)NewTarget ?? Undefined.Value; }
        }
    }
}
