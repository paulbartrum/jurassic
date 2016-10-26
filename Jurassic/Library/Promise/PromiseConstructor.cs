using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript Promise object.
    /// </summary>
    public partial class PromiseConstructor : ClrStubFunction
    {
        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new promise constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal PromiseConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "Promise", 1, PromiseInstance.CreatePrototype(Engine, this));
            FastSetProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Promise object is invoked like a function, e.g. var x = Promise().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Constructor Promise requires 'new'");
        }

        /// <summary>
        /// Creates a new promise instance.
        /// </summary>
        /// <param name="executor">  </param>
        /// <returns></returns>
        [JSConstructorFunction]
        public PromiseInstance Construct(FunctionInstance executor)
        {
            return new PromiseInstance(this.InstancePrototype, executor);
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// A reference to the constructor function that is used to create derived objects.
        /// </summary>
        [JSProperty(Name = "@@species")]
        public FunctionInstance Species
        {
            get { return this; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a promise that resolves when all of the promises in the iterable argument have
        /// resolved, or rejects with the reason of the first passed promise that rejects.
        /// </summary>
        /// <param name="iterable"> An iterable object, such as an Array. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "all")]
        public PromiseInstance All(ObjectInstance iterable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a promise that resolves or rejects as soon as one of the promises in the
        /// iterable resolves or rejects, with the value or reason from that promise.
        /// </summary>
        /// <param name="iterable"> An iterable object, such as an Array. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "race")]
        public PromiseInstance Race(ObjectInstance iterable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a Promise object that is rejected with the given reason.
        /// </summary>
        /// <param name="reason"> The reason why this Promise is rejected. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "reject")]
        public PromiseInstance Reject(object reason)
        {
            ClrStubFunction func = new ClrStubFunction(this.Engine.FunctionInstancePrototype, (engine, thisObj, args) =>
            {
                FunctionInstance resolve = (FunctionInstance)args[0];
                FunctionInstance reject = (FunctionInstance)args[1];
                reject.Call(thisObj, reason);
                return Undefined.Value;
            });
            return this.Construct(func);
        }

        /// <summary>
        /// Returns a Promise.then object that is resolved with the given value. If the value is a
        /// thenable (i.e. has a "then" method), the returned promise will "follow" that thenable,
        /// adopting its eventual state; otherwise the returned promise will be fulfilled with the
        /// value.
        /// </summary>
        /// <param name="value"> Argument to be resolved by this Promise. Can also be a Promise or
        /// a thenable to resolve. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "resolve")]
        public PromiseInstance Resolve(object x)
        {
            ClrStubFunction func = new ClrStubFunction(this.Engine.FunctionInstancePrototype, (engine, thisObj, args) =>
            {
                FunctionInstance resolve = (FunctionInstance)args[0];
                FunctionInstance reject = (FunctionInstance)args[1];
                resolve.Call(thisObj, x);
                return Undefined.Value;
            });
            return this.Construct(func);
        }
    }
}