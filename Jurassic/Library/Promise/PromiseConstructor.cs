using System;
using System.Linq;
using System.Threading.Tasks;
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
        /// Creates a new Promise instance from a task.
        /// </summary>
        /// <param name="task"> A task to wait on. </param>
        /// <returns> The Promise instance. </returns>
        /// <remarks>
        /// If the task is of type Task&lt;object&gt; then the result of the task will be used to
        /// resolve the promise. Otherwise, the promise is resolved with "undefined" as the value.
        /// </remarks>
        public PromiseInstance FromTask(Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            // Create a new promise.
            var promise = new PromiseInstance(this.InstancePrototype);

            // Execute some code after the task completes.
            task.ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().OnCompleted(() =>
            {
                // Enqueue an event which resolves the promise.
                Engine.EnqueueEvent(() =>
                {
                    try
                    {
                        if (task is Task<object> objectTask)
                            promise.Resolve(objectTask.Result);
                        else
                            promise.Resolve(Undefined.Value);
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.InnerExceptions.Count == 1)
                        {
                            var innerException = ex.InnerExceptions[0];
                            if (innerException is JavaScriptException innerJSException)
                                promise.Reject(innerJSException.ErrorObject);
                            else
                                promise.Reject(innerException.Message);
                        }
                        else
                            promise.Reject(ex.Message);
                    }
                    catch (JavaScriptException ex)
                    {
                        promise.Reject(ex.ErrorObject);
                    }
                    catch (Exception ex)
                    {
                        promise.Reject(ex.Message);
                    }
                });
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
        /// <param name="result"> The reason. Can be an Error instance. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "reject")]
        public PromiseInstance Reject(object result)
        {
            var promise = new PromiseInstance(this.InstancePrototype);
            promise.Reject(result);
            return promise;
        }

        /// <summary>
        /// Returns either a new promise resolved with the passed argument, or the argument itself
        /// if the argument is a promise produced by this constructor.
        /// </summary>
        /// <param name="value"> Argument to be resolved by this Promise. Can also be a Promise or
        /// a thenable to resolve. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "resolve")]
        public PromiseInstance Resolve(object value)
        {
            // If the constructor of value === this, then return as is.
            if (value is PromiseInstance promise)
                return promise;

            var result = new PromiseInstance(this.InstancePrototype);
            result.Resolve(value);
            return result;
        }

        /// <summary>
        /// Returns a new promise which is settled in the same way as the first passed promise to
        /// settle. All elements of the passed iterable are resolved to promises.
        /// </summary>
        /// <param name="iterable"> An iterable object such as an Array. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "race")]
        public PromiseInstance Race(ObjectInstance iterable)
        {
            if (iterable == null)
                throw new JavaScriptException(Engine, ErrorType.TypeError, "The parameter must be an iterable.");

            var promises = TypeUtilities.ForOf(Engine, iterable);

            var result = new PromiseInstance(Engine.Promise.InstancePrototype);
            foreach (var promiseOrValue in promises)
                Resolve(promiseOrValue).Then(result.ResolveFunction, result.RejectFunction);
            return result;
        }

        /// <summary>
        /// Returns a Promise. It takes one argument: a list of Promises that determine whether
        /// the new Promise is fulfilled or rejected.
        /// </summary>
        /// <param name="iterable"> An iterable object such as an Array. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "all")]
        public PromiseInstance All(ObjectInstance iterable)
        {
            if (iterable == null)
                throw new JavaScriptException(iterable.Engine, ErrorType.TypeError, "The parameter must be an iterable.");

            var promises = TypeUtilities.ForOf(iterable.Engine, iterable).ToList();
            var results = Engine.Array.Construct(new object[promises.Count]);
            var count = promises.Count;

            var promise = new PromiseInstance(iterable.Engine.Promise.InstancePrototype);

            // The promise is resolved immediately if the iterable is empty.
            if (promises.Count == 0)
            {
                promise.Resolve(results);
                return promise;
            }

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
                    p.Then(new ClrStubFunction(Engine.FunctionInstancePrototype, "", 1, (engine, thisObj, args) =>
                    {
                        if (promise.State != PromiseState.Pending)
                            return Undefined.Value;

                        results[j] = args[0];

                        if (--count == 0)
                        {
                            promise.Resolve(results);
                        }
                        return Undefined.Value;
                    }),
                        new ClrStubFunction(Engine.FunctionInstancePrototype, "", 1, (engine, thisObj, args) =>
                        {
                            promise.Reject(args[0]);
                            return Undefined.Value;
                        }));

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

                            then.Call(obj, resolve, promise.RejectFunction);
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