using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Jurassic.Library;

namespace Jurassic.Extensions.SimpleTiming
{
    /// <summary>
    /// Implements the setTimeout, setInterval, clearTimeout and clearInterval functions.
    /// </summary>
    public static partial class SimpleTimingImplementation
    {
        private class TimerPool
        {
            private Dictionary<int, Timer> pool = new Dictionary<int, Timer>();
            private int nextID = 1;

            public int AddTimer(TimerCallback callback, int dueTime, int period)
            {
                int id = nextID++;
                if (dueTime == 0)
                    callback(null);
                else
                    pool[id] = new Timer(callback, null, dueTime, period);
                return id;
            }

            public void Remove(int id)
            {
                if (pool.TryGetValue(id, out Timer timer))
                {
                    timer.Dispose();
                    pool.Remove(id);
                }
            }
        }

        private static ConditionalWeakTable<ScriptEngine, TimerPool> pools =
            new ConditionalWeakTable<ScriptEngine, TimerPool>();

        /// <summary>
        /// Gets a reference to the timer pool associated with the given script engine.
        /// </summary>
        /// <param name="engine"></param>
        /// <returns> The timer pool associated with the given script engine. </returns>
        private static TimerPool GetPool(ScriptEngine engine)
        {
            if (pools.TryGetValue(engine, out var pool))
                return pool;
            throw new InvalidOperationException("AddSimpleTiming() has not been called.");
        }

        /// <summary>
        /// Converts a value which is either a function or a string containing code into an
        /// <see cref="Action"/>.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="funcOrCode"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Action GetEventAction(ScriptEngine engine, object funcOrCode, object[] args)
        {
            if (funcOrCode is FunctionInstance callbackFunction)
                return () => callbackFunction.Call(engine.Global, args);
            else if (funcOrCode is string code)
                return () => GlobalObject.Eval(engine, code);
            else
                throw new JavaScriptException(engine, ErrorType.TypeError, "The first parameter should be either a function or a string.");

        }

        /// <summary>
        /// Adds the setTimeout, setInterval, clearTimeout and clearInterval functions to the
        /// global object.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <remarks>
        /// Can only be called once per script engine.
        /// </remarks>
        internal static void Add(ScriptEngine engine)
        {
            if (pools.TryGetValue(engine, out var pool))
                throw new InvalidOperationException("Please call this method only once per script engine.");
            foreach (var property in GetDeclarativeProperties(engine))
                engine.Global.DefineProperty(property.Key, new PropertyDescriptor(property.Value, property.Attributes), throwOnError: true);
            pools.Add(engine, new TimerPool());
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Repeatedly calls a function or executes a code snippet, with a fixed time delay between
        /// each call.
        /// </summary>
        /// <param name="engine"> The engine to use to execute the code. </param>
        /// <param name="funcOrCode"> Either a function to call after the timeout expires, or a
        /// string containing code to execute after the timeout expires. </param>
        /// <param name="delay"> The time, in milliseconds (thousandths of a second), the timer
        /// should wait before the specified function or code is executed. If this argument is
        /// omitted, a value of 0 is used, meaning execute "immediately", or more accurately, the
        /// next event cycle. </param>
        /// <param name="args"> Additional arguments which are passed through to the function
        /// specified by <paramref name="funcOrCode"/> once the timer expires. Ignored if
        /// <paramref name="funcOrCode"/> is a string. </param>
        /// <returns> A positive integer value which identifies the timer, and which can be passed
        /// to <see cref="ClearTimeout(ScriptEngine, int)"/> to cancel the timeout. </returns>
        [JSInternalFunction(Name = "setTimeout", Flags = JSFunctionFlags.HasEngineParameter, IsEnumerable = true)]
        public static int SetTimeout(ScriptEngine engine, object funcOrCode, int delay = 0, params object[] args)
        {
            // The minimum delay is zero, which means enqueue an event immediately.
            delay = Math.Max(0, delay);

            // Add a timer to the pool.
            var pool = GetPool(engine);
            return pool.AddTimer(state => engine.EnqueueEvent(GetEventAction(engine, funcOrCode, args)),
                delay, Timeout.Infinite);
        }

        /// <summary>
        /// cancels a timeout previously established by calling
        /// <see cref="SetTimeout(ScriptEngine, object, int, object[])"/>.
        /// </summary>
        /// <param name="engine"> The engine to use to execute the code. </param>
        /// <param name="timeoutID"> The identifier of the timeout you want to cancel.
        /// This ID was returned by the corresponding call to
        /// <see cref="SetTimeout(ScriptEngine, object, int, object[])"/>. </param>
        [JSInternalFunction(Name = "clearTimeout", Flags = JSFunctionFlags.HasEngineParameter, IsEnumerable = true)]
        public static void ClearTimeout(ScriptEngine engine, int timeoutID)
        {
            GetPool(engine).Remove(timeoutID);
        }

        /// <summary>
        /// Repeatedly calls a function or executes a code snippet, with a fixed time delay between
        /// each call.
        /// </summary>
        /// <param name="engine"> The engine to use to execute the code. </param>
        /// <param name="funcOrCode"> Either a function to call, or a string containing code. </param>
        /// <param name="delay"> The time, in milliseconds (thousandths of a second), the timer
        /// should delay in between executions of the specified function or code. The minimum is
        /// <c>4</c>. </param>
        /// <param name="args"> Additional arguments which are passed through to the function
        /// specified by <paramref name="funcOrCode"/> once the timer expires. Ignored if
        /// <paramref name="funcOrCode"/> is a string. </param>
        /// <returns> A positive integer value which identifies the timer, and which can be passed
        /// to <see cref="ClearInterval(ScriptEngine, int)"/> to cancel the timeout. </returns>
        [JSInternalFunction(Name = "setInterval", Flags = JSFunctionFlags.HasEngineParameter, IsEnumerable = true)]
        public static int SetInterval(ScriptEngine engine, object funcOrCode, int delay = 0, params object[] args)
        {
            // The minimum interval delay is four, which means enqueue an event immediately.
            delay = Math.Max(4, delay);

            // Add a timer to the pool.
            var pool = GetPool(engine);
            return pool.AddTimer(state => engine.EnqueueEvent(GetEventAction(engine, funcOrCode, args)),
                delay, delay);
        }

        /// <summary>
        /// Cancels a timed, repeating action which was previously established by a call to
        /// <see cref="SetInterval(ScriptEngine, object, int, object[])"/>.
        /// </summary>
        /// <param name="engine"> The engine to use to execute the code. </param>
        /// <param name="intervalID"> The identifier of the repeated action you want to cancel.
        /// This ID was returned by the corresponding call to
        /// <see cref="SetInterval(ScriptEngine, object, int, object[])"/>. </param>
        [JSInternalFunction(Name = "clearInterval", Flags = JSFunctionFlags.HasEngineParameter, IsEnumerable = true)]
        public static void ClearInterval(ScriptEngine engine, int intervalID)
        {
            ClearTimeout(engine, intervalID);
        }
    }
}
