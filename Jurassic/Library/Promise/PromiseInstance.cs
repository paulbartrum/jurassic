using System.Collections.Generic;
using System.Diagnostics;
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

        private enum ReactionType
        {
            Fulfill,
            Reject
        }

        private class Reaction
        {
            public PromiseInstance Promise { get; set; }

            public ReactionType Type { get; set; }

            public FunctionInstance Handler { get; set; }
        }

        // Governs how a promise will react to incoming calls to its then() method.
        private PromiseState state = PromiseState.Pending;

        // The value with which the promise has been fulfilled or rejected, if any.  Only
        // meaningful if state is not Pending.
        private object result;

        // 	A list of PromiseReaction records to be processed when/if the promise transitions
        // from the Pending state to any other state.
        private List<Reaction> fulfillReactions;
        private List<Reaction> rejectReactions;

        // The function that will resolve the promise.
        private readonly FunctionInstance resolveFunction;

        // The function that will reject the promise.
        private readonly FunctionInstance rejectFunction;

        /// <summary>
        /// Creates a new Promise instance.
        /// </summary>
        /// <param name="prototype"></param>
        /// <param name="executor"></param>
        internal PromiseInstance(ObjectInstance prototype, FunctionInstance executor) : this(prototype)
        {
            try
            {
                executor.Call(Undefined.Value, resolveFunction, rejectFunction);
            }
            catch (JavaScriptException ex)
            {
                rejectFunction.Call(Undefined.Value, ex.GetErrorObject(Engine));
            }
        }

        /// <summary>
        /// Creates a new Promise instance.
        /// </summary>
        /// <param name="prototype"></param>
        internal PromiseInstance(ObjectInstance prototype) : base(prototype)
        {
            resolveFunction = new ClrStubFunction(Engine.FunctionInstancePrototype, "", 1,
                (engine, thisObj, args) =>
                {
                    Resolve(args.Length >= 1 ? args[0] : Undefined.Value);
                    return Undefined.Value;
                });
            rejectFunction = new ClrStubFunction(Engine.FunctionInstancePrototype, "", 1,
                (engine, thisObj, args) =>
                {
                    Reject(args.Length >= 1 ? args[0] : Undefined.Value);
                    return Undefined.Value;
                });
        }

        /// <summary>
        /// Resolves the promise with the given value.  If the value is an object with a "then"
        /// function, the returned promise will "follow" that thenable, adopting its eventual
        /// state; otherwise the returned promise will be fulfilled with the value.
        /// </summary>
        /// <param name="value"> The resolved value of the promise, or a promise or thenable to
        /// follow. </param>
        internal void Resolve(object value)
        {
            // Do nothing if the promise has already been resolved.
            if (state != PromiseState.Pending)
                return;

            if (value == this)
            {
                // Reject promise.
                Reject(Engine.TypeError.Construct("A promise cannot be resolved with itself."));
                return;
            }
            else if (value is ObjectInstance thenObject)
            {
                // Try to call a method on the object called "then".
                try
                {
                    if (thenObject.GetPropertyValue("then") is FunctionInstance thenFunction)
                    {
                        Engine.AddPostExecuteStep(() =>
                        {
                            try
                            {
                                thenFunction.Call(thenObject, resolveFunction, rejectFunction);
                            }
                            catch (JavaScriptException ex)
                            {
                                Reject(ex.GetErrorObject(Engine));
                            }
                        });
                        return;
                    }
                    // If 'then' doesn't exist or is not a function, then fulfill normally.
                }
                catch (JavaScriptException ex)
                {
                    // GetPropertyValue threw an exception.
                    Reject(ex.GetErrorObject(Engine));
                    return;
                }
            }

            // Fulfill promise.
            this.state = PromiseState.Fulfilled;
            this.result = value;
            var reactions = this.fulfillReactions;
            if (reactions != null)
            {
                this.fulfillReactions = null;
                foreach (var reaction in reactions)
                    EnqueueJob(reaction);
            }
        }

        /// <summary>
        /// Rejects the promise with the given reason.
        /// </summary>
        /// <param name="reason"> The reason why this promise rejected. Can be an Error instance. </param>
        internal void Reject(object reason)
        {
            // Do nothing if the promise has already been resolved.
            if (state != PromiseState.Pending)
                return;

            this.state = PromiseState.Rejected;
            this.result = reason;
            var reactions = this.rejectReactions;
            if (reactions != null)
            {
                this.rejectReactions = null;
                foreach (var reaction in reactions)
                    EnqueueJob(reaction);
            }
        }

        /// <summary>
        /// Creates the Promise prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, PromiseConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(engine.Symbol.ToStringTag, "Promise", PropertyAttributes.Configurable));
            result.InitializeProperties(properties);
            return result;
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// When the promise is completed, i.e either fulfilled or rejected, the specified callback
        /// function is executed.
        /// </summary>
        /// <param name="onFinally"> A Function called when the Promise is completed.</param>
        /// <returns></returns>
        [JSInternalFunction(Name = "finally")]
        public PromiseInstance Finally(object onFinally)
        {
            if (onFinally is FunctionInstance onFinallyFunction)
            {
                return Then(new ClrStubFunction(Engine.Function.InstancePrototype, (engine, thisObj, args) =>
                {
                    onFinallyFunction.Call(Undefined.Value);
                    return this.result;
                }),
                new ClrStubFunction(Engine.Function.InstancePrototype, (engine, thisObj, args) =>
                {
                    onFinallyFunction.Call(Undefined.Value);
                    throw new JavaScriptException(this.result);
                }));
            }
            else
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
            return PerformPromiseThen(onFulfilled as FunctionInstance, onRejected as FunctionInstance,
                new PromiseInstance(Prototype));
        }

        private PromiseInstance PerformPromiseThen(FunctionInstance onFulfilled, FunctionInstance onRejected, PromiseInstance result = null)
        {
            var fulfilled = new Reaction { Promise = result, Type = ReactionType.Fulfill, Handler = onFulfilled };
            var rejected = new Reaction { Promise = result, Type = ReactionType.Reject, Handler = onRejected };

            if (state == PromiseState.Pending)
            {
                if (fulfillReactions == null)
                    fulfillReactions = new List<Reaction>(1);
                fulfillReactions.Add(fulfilled);
                if (rejectReactions == null)
                    rejectReactions = new List<Reaction>(1);
                rejectReactions.Add(rejected);
            }
            else if (state == PromiseState.Fulfilled)
            {
                EnqueueJob(fulfilled);
            }
            else if (state == PromiseState.Rejected)
            {
                EnqueueJob(rejected);
            }

            return result;
        }

        private void EnqueueJob(Reaction reaction)
        {
            // If handler is undefined and type is Fulfill, then the handler result should be result.
            // If handler is undefined and type is Reject, then the handler should throw result.
            Engine.AddPostExecuteStep(() =>
            {
                if (reaction.Handler != null)
                {
                    // If a handler has been provided, call it and resolve or reject depending on
                    // the return value.
                    try
                    {
                        var handlerResult = reaction.Handler.Call(Undefined.Value, this.result);
                        if (reaction.Promise != null)
                            reaction.Promise.Resolve(handlerResult ?? Undefined.Value);
                    }
                    catch (JavaScriptException ex)
                    {
                        if (reaction.Promise != null)
                            reaction.Promise.Reject(ex.GetErrorObject(Engine));
                    }
                }
                else if (reaction.Type == ReactionType.Fulfill)
                {
                    // The default onFulfilled action is to resolve with the passed-in value.
                    if (reaction.Promise != null)
                        reaction.Promise.Resolve(this.result ?? Undefined.Value);
                }
                else
                {
                    // The default onRejected action is to reject with the passed-in value.
                    if (reaction.Promise != null)
                        reaction.Promise.Reject(this.result);
                }
            });
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
                throw new JavaScriptException(result);
            }

            // These callbacks shouldn't deadlock on this.sync as they will not immediately fire
            // (they are added to a queue and will execute after the current Evaluate/Execute).
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
                    tcs.SetException(new JavaScriptException(result));
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
        /// Gets a function that will resolve the promise.
        /// </summary>
        internal FunctionInstance ResolveFunction
        {
            get { return this.resolveFunction; }
        }

        /// <summary>
        /// Gets a function that will reject the promise.
        /// </summary>
        internal FunctionInstance RejectFunction
        {
            get { return this.rejectFunction; }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get
            {
                if (state == PromiseState.Pending)
                    return state.ToString();
                else
                    return $"{state}: {result}";
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