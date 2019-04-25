using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the Promise object.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(PromiseInstanceDebugView))]
    public partial class PromiseInstance : ObjectInstance
    {
        internal enum PromiseState
        {
            Pending,
            Fulfilled,
            Rejected,
        }

        // Governs how a promise will react to incoming calls to its then() method.
        private PromiseState state = PromiseState.Pending;

        // The value with which the promise has been fulfilled or rejected, if any.  Only
        // meaningful if state is not Pending.
        private object result;

        // 	A list of PromiseReaction records to be processed when/if the promise transitions
        // from the Pending state to the Fulfilled state.
        private readonly List<FunctionInstance> fulfillReactions = new List<FunctionInstance>();

        // 	A list of PromiseReaction records to be processed when/if the promise transitions
        // from the Pending state to the Rejected state.
        private readonly List<FunctionInstance> rejectReactions = new List<FunctionInstance>();


        /// <summary>
        /// Creates a new Promise instance.
        /// </summary>
        /// <param name="prototype"></param>
        /// <param name="executor"></param>
        internal PromiseInstance(ObjectInstance prototype, FunctionInstance executor) : base(prototype)
        {
            FunctionInstance resolveFunc = new ClrStubFunction(Engine.FunctionInstancePrototype, "s", 1, (engine, thisObj, param) => Resolve(param));
            FunctionInstance rejectFunc = new ClrStubFunction(Engine.FunctionInstancePrototype, "t", 1, (engine, thisObj, param) => Reject(param));
            try
            {
                executor.Call(Undefined.Value, resolveFunc, rejectFunc);
            }
            catch (JavaScriptException ex)
            {
                rejectFunc.Call(Undefined.Value, ex.ErrorObject);
            }
        }

        /// <summary>
        /// Creates a new Promise instance.
        /// </summary>
        /// <param name="prototype"></param>
        internal PromiseInstance(ObjectInstance prototype) : base(prototype)
        {

        }

        internal object Reject(params object[] param) =>
            ResolveOrReject(param, PromiseState.Rejected, rejectReactions);

        internal object Resolve(params object[] param) =>
            ResolveOrReject(param, PromiseState.Fulfilled, fulfillReactions);

        internal object ResolveOrReject(object[] param, PromiseState state, List<FunctionInstance> functions)
        {
            if (this.state != PromiseState.Pending)
                throw new JavaScriptException(Engine, ErrorType.TypeError, string.Format("Promise has already been {0}", this.state));

            this.state = state;
            if (param.Length > 0)
            {
                result = param[0];
            }
            else
            {
                result = Undefined.Value;
            }

            foreach (var function in functions)
            {
                ResolveOrReject(function);
            }
            fulfillReactions.Clear();
            rejectReactions.Clear();
            return Undefined.Value;
        }

        internal void ResolveOrReject(FunctionInstance function)
        {
            if (function == null)
            {
                // Do nothing.
            }
            else if (result == Undefined.Value)
            {
                function.Engine.EventLoop.PendCallback(function, function.Engine.Global);
            }
            else
            {
                function.Engine.EventLoop.PendCallback(function, function.Engine.Global, result);
            }
        }

        /// <summary>
        /// Creates the Map prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, PromiseConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(engine.Symbol.ToStringTag, "Promise", PropertyAttributes.Configurable));
            result.FastSetProperties(properties);
            return result;
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a Promise and deals with all cases. It behaves the same as calling
        /// Promise.prototype.then(onFinally, onFinally).
        /// </summary>
        /// <param name="onFinally"> A Function called when the Promise is completed.</param>
        /// <returns></returns>
        [JSInternalFunction(Name = "finally")]
        public PromiseInstance Finally(object onFinally)
        {
            return Then(onFinally, onFinally);
        }

        /// <summary>
        /// Returns a Promise and deals with rejected cases only. It behaves the same as calling
        /// Promise.prototype.then(undefined, onRejected).
        /// </summary>
        /// <param name="onRejected"> A Function called when the Promise is rejected. This function
        /// has one argument, the rejection reason. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "catch")]
        public PromiseInstance Catch(object onRejected)
        {
            return Then(null, onRejected);
        }

        /// <summary>
        /// Returns a Promise. It takes two arguments: callback functions for the success and
        /// failure cases of the Promise.
        /// </summary>
        /// <param name="onFulfilled"> A Function called when the Promise is fulfilled. This
        /// function has one argument, the fulfillment value. </param>
        /// <param name="onRejected"> A Function called when the Promise is rejected. This function
        /// has one argument, the rejection reason. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "then")]
        public PromiseInstance Then(object onFulfilled, object onRejected)
        {
            if (state == PromiseState.Pending)
            {
                if (onFulfilled is FunctionInstance f) fulfillReactions.Add(f);
                if (onRejected is FunctionInstance r) rejectReactions.Add(r);
            }
            else if (state == PromiseState.Fulfilled)
            {
                ResolveOrReject(onFulfilled as FunctionInstance);
            }
            else if (state == PromiseState.Rejected)
            {
                ResolveOrReject(onRejected as FunctionInstance);
            }
            return this;
        }

        /// <summary>
        /// Creates a task that completes when this promise completes.
        /// </summary>
        /// <returns>The task that completes when this promise completes.</returns>
        public Task<object> CreateTask()
        {
            if (state == PromiseState.Fulfilled)
            {
                return Task.FromResult(result);
            }
            else if (state == PromiseState.Rejected)
            {
                throw new JavaScriptException(result, 0, null);
            }

            var tcs = new TaskCompletionSource<object>();
            Then(new ClrStubFunction(Engine.Function.InstancePrototype, (engine, ths, arg) =>
                {
                    var result = arg.Length == 0 ? Undefined.Value : arg[0];
                    tcs.SetResult(result);
                    return Undefined.Value;
                }),
                new ClrStubFunction(Engine.Function.InstancePrototype, (engine, ths, arg) =>
                {
                    var result = arg.Length == 0 ? Undefined.Value : arg[0];
                    tcs.SetException(new JavaScriptException(result, 0, null));
                    return Undefined.Value;
                }));
            return tcs.Task;
        }


        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the promise status. Used by debugger decoration only.
        /// </summary>
        internal PromiseState State
        {
            get { return this.state; }
        }

        /// <summary>
        /// Gets the promise result. Used by debugger decoration only.
        /// </summary>
        internal object Result
        {
            get { return this.result; }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get
            {
                return this.state.ToString();
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayValue; }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayType
        {
            get { return "Promise"; }
        }

    }
}