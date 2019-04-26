using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using static Jurassic.Library.PromiseInstance;

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
        /// Creates a new Promise instance.
        /// </summary>
        /// <param name="executor">The function that the promise executes.</param>
        /// <returns>The Promise instance.</returns>
        [JSConstructorFunction]
        public PromiseInstance Construct(FunctionInstance executor)
        {
            return new PromiseInstance(this.InstancePrototype, executor);
        }

        /// <summary>
        /// Creates a new Promise instance.
        /// </summary>
        /// <param name="notify">A <see cref="INotifyCompletion"/> that will signal the success or failure of the promise.</param>
        /// <returns>The Promise instance.</returns>
        public PromiseInstance Construct<T>(T notify)
            where T : INotifyCompletion
        {
            var promise = new PromiseInstance(this.InstancePrototype);
            if (notify == null) return promise;

            notify.OnCompleted(() =>
            {
                try
                {
                    promise.Resolve(TaskAwaiterCache.GetResult(notify));
                }
                catch (JavaScriptException jex)
                {
                    promise.Reject(jex.ErrorObject);
                }
                catch (Exception e)
                {
                    promise.Reject(e.Message);
                }
            });

            return promise;
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
        /// Returns a Promise that is rejected for the specified reason.
        /// </summary>
        /// <param name="result">The reason.</param>
        /// <returns></returns>
        [JSInternalFunction(Name = "reject")]
        public PromiseInstance Reject(object result)
        {
            var promise = new PromiseInstance(this.InstancePrototype);
            promise.Reject(result);
            return promise;
        }

        /// <summary>
        /// Returns a Promise that is resolved with the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        [JSInternalFunction(Name = "resolve")]
        public PromiseInstance Resolve(object result)
        {
            var promise = new PromiseInstance(this.InstancePrototype);
            promise.Resolve(result);
            return promise;
        }

        /// <summary>
        /// Returns a Promise. It takes one argument: a list of Promises that determine whether
        /// the new Promise is fulfilled or rejected.
        /// </summary>
        /// <param name="iterable">The list of Promises.</param>
        /// <returns></returns>
        [JSInternalFunction(Name = "race")]
        public PromiseInstance Race(ObjectInstance iterable)
        {
            if (iterable == null)
                throw new JavaScriptException(iterable.Engine, ErrorType.TypeError, "The parameter must be an iterable.");

            var iterator = TypeUtilities.GetIterator(iterable.Engine, iterable);
            var promises = TypeUtilities.Iterate(iterator.Engine, iterator);

            var promise = new PromiseInstance(iterable.Engine.Promise.InstancePrototype);

            foreach (var promiseOrValue in promises)
            {
                if (promise.State != PromiseState.Pending) break;
                promise.Resolve(promiseOrValue);
            }

            return promise;
        }

        /// <summary>
        /// Returns a Promise. It takes one argument: a list of Promises that determine whether
        /// the new Promise is fulfilled or rejected.
        /// </summary>
        /// <param name="iterable">The list of Promises.</param>
        /// <returns></returns>
        [JSInternalFunction(Name = "all")]
        public PromiseInstance All(ObjectInstance iterable)
        {
            if (iterable == null)
                throw new JavaScriptException(iterable.Engine, ErrorType.TypeError, "The parameter must be an iterable.");

            var iterator = TypeUtilities.GetIterator(iterable.Engine, iterable);
            var promises = TypeUtilities.Iterate(iterator.Engine, iterator).ToList();
            var results = iterable.Engine.Array.Construct(new object[promises.Count]);
            var count = promises.Count;

            var promise = new PromiseInstance(iterable.Engine.Promise.InstancePrototype);

            for (var i = 0; i < promises.Count; i++)
            {
                if (promise.State != PromiseState.Pending) break;

                if (promises[i] is PromiseInstance p)
                {
                    if (p.State == PromiseState.Rejected)
                    {
                        promise.Reject(p.Result);
                        break;
                    }
                    else if (p.State == PromiseState.Fulfilled)
                    {
                        results[i] = p.Result;
                        if (--count == 0)
                        {
                            promise.Resolve(results);
                            break;
                        }
                        continue;
                    }

                    var j = i; // Some C# versions need this.
                    p.Then(
                        arg =>
                        {
                            if (promise.State != PromiseState.Pending) return;

                            results[j] = arg;

                            if (--count == 0)
                            {
                                promise.Resolve(results);
                            }
                        },
                        arg =>
                        {
                            promise.Reject(arg);
                        });

                    continue;
                }
                else if (promises[i] is ObjectInstance obj && obj.HasProperty("then"))
                {
                    FunctionInstance then;
                    try
                    {
                        then = obj.GetPropertyValue("then") as FunctionInstance;
                    }
                    catch (JavaScriptException jex)
                    {
                        promise.Reject(jex.ErrorObject);
                        break;
                    }

                    if (then != null)
                    {
                        try
                        {
                            var j = i; // Some C# versions need this.
                            var resolve = new ClrStubFunction(iterable.Engine.Function.InstancePrototype, (engine, ths, arg) =>
                            {
                                if (promise.State != PromiseState.Pending) return Undefined.Value;

                                results[j] = arg.Length == 0
                                    ? Undefined.Value
                                    : arg[0];

                                if (--count == 0)
                                {
                                    promise.Resolve(results);
                                }
                                return Undefined.Value;
                            });

                            then.Call(obj, resolve, promise.RejectPromise);
                            continue;
                        }
                        catch (JavaScriptException jex)
                        {
                            promise.Reject(jex.ErrorObject);
                            break;
                        }
                    }
                }

                results[i] = promises[i];
                if (--count == 0)
                {
                    promise.Resolve(results);
                    break;
                }
            }

            return promise;
        }
    }
}