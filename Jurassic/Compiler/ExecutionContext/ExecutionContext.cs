using Jurassic.Library;
using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the runtime state needed to run JS code.
    /// </summary>
    public class ExecutionContext
    {
        object thisValue;

        /// <summary>
        /// Creates an execution context for code running in an eval() scope.
        /// </summary>
        /// <param name="engine"> A script engine. </param>
        /// <param name="parentScope"> The scope that was active when eval() was called. </param>
        /// <param name="thisValue"> The value of the 'this' keyword. </param>
        /// <returns> A new execution context instance. </returns>
        internal static ExecutionContext CreateGlobalOrEvalContext(ScriptEngine engine, RuntimeScope parentScope, object thisValue)
        {
            if (thisValue == null)
                throw new ArgumentNullException(nameof(thisValue));
            return new ExecutionContext(engine, parentScope, BindingStatus.Initialized, thisValue, null, null, null);
        }

        /// <summary>
        /// Creates an execution context for code running as a result of a function call.
        /// </summary>
        /// <param name="engine"> A script engine. </param>
        /// <param name="parentScope"> The scope that was active when the function was declared
        /// (NOT when it was called). </param>
        /// <param name="thisValue"> The value of the 'this' keyword. </param>
        /// <param name="executingFunction"> The function that is being called. </param>
        /// <returns> A new execution context instance. </returns>
        internal static ExecutionContext CreateFunctionContext(ScriptEngine engine, RuntimeScope parentScope, object thisValue, UserDefinedFunction executingFunction)
        {
            if (thisValue == null)
                throw new ArgumentNullException(nameof(thisValue));
            if (executingFunction == null)
                throw new ArgumentNullException(nameof(executingFunction));
            return new ExecutionContext(engine, parentScope, BindingStatus.Initialized, thisValue, executingFunction, null, executingFunction.Container);
        }

        /// <summary>
        /// Creates an execution context for code running as a result of the new operator.
        /// </summary>
        /// <param name="engine"> A script engine. </param>
        /// <param name="parentScope"> The scope that was active when the class was declared. </param>
        /// <param name="thisValue"> The value of the 'this' keyword. </param>
        /// <param name="executingFunction"> A reference to the function that is being executed. </param>
        /// <param name="newTarget"> The target of the new operator. </param>
        /// <param name="functionContainer"> A reference to the object literal or class prototype
        /// the executing function was defined within. Used by the 'super' keyword. </param>
        /// <returns> A new execution context instance. </returns>
        internal static ExecutionContext CreateConstructContext(ScriptEngine engine, RuntimeScope parentScope, object thisValue, UserDefinedFunction executingFunction, FunctionInstance newTarget, ObjectInstance functionContainer)
        {
            return new ExecutionContext(engine, parentScope, BindingStatus.Initialized, thisValue, executingFunction, newTarget, functionContainer);
        }

        /// <summary>
        /// Creates an execution context for code running as a result of the new operator. The
        /// 'this' value is unavailable.
        /// </summary>
        /// <param name="engine"> A script engine. </param>
        /// <param name="parentScope"> The scope that was active when the class was declared. </param>
        /// <param name="executingFunction"> A reference to the function that is being executed. </param>
        /// <param name="newTarget"> The target of the new operator. </param>
        /// <param name="functionContainer"> A reference to the object literal or class prototype
        /// the executing function was defined within. Used by the 'super' keyword. </param>
        /// <returns> A new execution context instance. </returns>
        internal static ExecutionContext CreateDerivedContext(ScriptEngine engine, RuntimeScope parentScope, UserDefinedFunction executingFunction, FunctionInstance newTarget, ObjectInstance functionContainer)
        {
            return new ExecutionContext(engine, parentScope, BindingStatus.Uninitialized, null, executingFunction, newTarget, functionContainer);
        }

        private ExecutionContext(ScriptEngine engine, RuntimeScope parentScope,
            BindingStatus thisBindingStatus, object thisValue,
            UserDefinedFunction executingFunction, FunctionInstance newTarget,
            ObjectInstance functionContainer)
        {
            this.Engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.ParentScope = parentScope;
            this.ThisBindingStatus = thisBindingStatus;
            this.thisValue = thisValue;
            this.ExecutingFunction = executingFunction;
            this.NewTarget = newTarget;
            this.FunctionContainer = functionContainer;
        }

        /// <summary>
        /// A reference to the script engine.
        /// </summary>
        public ScriptEngine Engine { get; private set; }

        /// <summary>
        /// The scope that was active when this execution context was declared. Can be <c>null</c>.
        /// </summary>
        public RuntimeScope ParentScope { get; private set; }

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
                    throw new JavaScriptException(Engine, ErrorType.ReferenceError, "Must call super constructor in derived class before accessing 'this'.");
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
        /// The value of the 'super' keyword, or <c>null</c> if it is not available.
        /// </summary>
        public ObjectInstance SuperValue
        {
            get { return FunctionContainer?.Prototype; }
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

            if (FunctionContainer.Prototype is FunctionInstance super)
            {
                // Call the base class constructor.
                var result = super.ConstructLateBound(NewTarget, argumentValues);
                ThisBindingStatus = BindingStatus.Initialized;
                thisValue = result;
                return result;
            }
            else
                throw new JavaScriptException(Engine, ErrorType.TypeError, $"Super constructor {FunctionContainer.Prototype} of {ExecutingFunction.Name} is not a constructor.");
        }

        /// <summary>
        /// A reference to the executing function. Will be <c>null</c> if running in a global or
        /// eval context.
        /// </summary>
        public UserDefinedFunction ExecutingFunction { get; private set; }

        /// <summary>
        /// If this context was created by the 'new' operator, contains the target of the new
        /// operator. This value can be accessed by JS using the 'new.target' expression.
        /// </summary>
        public FunctionInstance NewTarget { get; private set; }

        /// <summary>
        /// Contains a reference to the object literal or class prototype the executing function
        /// was defined within. Used by the 'super' keyword.
        /// </summary>
        public ObjectInstance FunctionContainer { get; private set; }

        /// <summary>
        /// The same as <see cref="NewTarget"/> except that it returns <see cref="Undefined.Value"/>
        /// instead of <c>null</c> if no new.target value is available.
        /// </summary>
        public object NewTargetObject
        {
            get { return (object)NewTarget ?? Undefined.Value; }
        }

        /// <summary>
        /// Creates a new instance of the 'arguments' object.
        /// </summary>
        /// <param name="functionScope"> The top-level scope for the function. </param>
        /// <param name="arguments"> The argument values that were passed to the function. </param>
        /// <returns> A new instance of the 'arguments' object. </returns>
        public ArgumentsInstance CreateArgumentsInstance(RuntimeScope functionScope, object[] arguments)
        {
            return new ArgumentsInstance(Engine.Object.InstancePrototype, ExecutingFunction, functionScope, arguments);
        }

        /// <summary>
        /// Creates a new RuntimeScope instance, which is used for passing captured variables
        /// between methods.
        /// </summary>
        /// <param name="parent"> The parent scope, or <c>null</c> to use the ParentScope from this
        /// execution context. </param>
        /// <param name="scopeType"></param>
        /// <param name="varNames"></param>
        /// <param name="letNames"></param>
        /// <param name="constNames"></param>
        /// <returns> A new RuntimeScope instance. </returns>
        public RuntimeScope CreateRuntimeScope(RuntimeScope parent, ScopeType scopeType, string[] varNames, string[] letNames, string[] constNames)
        {
            return new RuntimeScope(Engine, parent ?? ParentScope, scopeType, varNames, letNames, constNames);
        }
    }
}
