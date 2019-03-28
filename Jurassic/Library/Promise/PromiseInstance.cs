using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

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
        private readonly List<object> fulfillReactions = new List<object>();

        // 	A list of PromiseReaction records to be processed when/if the promise transitions
        // from the Pending state to the Rejected state.
        private readonly List<object> rejectReactions = new List<object>();


        /// <summary>
        /// Creates a new Promise instance.
        /// </summary>
        /// <param name="prototype"></param>
        /// <param name="executor"></param>
        internal PromiseInstance(ObjectInstance prototype, FunctionInstance executor) : base(prototype)
        {
            FunctionInstance resolveFunc = new ClrStubFunction(Engine.FunctionInstancePrototype, (engine, thisObj, param) =>
            {
                this.state = PromiseState.Fulfilled;
                if (param.Length > 0)
                {
                    result = param[0];
                }
                return Undefined.Value;
            });
            FunctionInstance rejectFunc = new ClrStubFunction(Engine.FunctionInstancePrototype, (engine, thisObj, param) =>
            {
                this.state = PromiseState.Rejected;
                if (param.Length > 0)
                {
                    result = param[0];
                }
                return Undefined.Value;
            });
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
        /// Returns a Promise and deals with rejected cases only. It behaves the same as calling
        /// Promise.prototype.then(undefined, onRejected).
        /// </summary>
        /// <param name="onRejected"> A Function called when the Promise is rejected. This function
        /// has one argument, the rejection reason. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "catch")]
        public PromiseInstance Catch(FunctionInstance onRejected)
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
        public PromiseInstance Then(FunctionInstance onFulfilled, FunctionInstance onRejected)
        {
            throw new NotImplementedException();
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