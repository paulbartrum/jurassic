using Jurassic.Library;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Jurassic
{
    /// <summary>
    /// Represents a list of pending callbacks.
    /// </summary>
    public sealed class EventLoop
    {
        private struct EventLoopInstance
        {
            private readonly FunctionInstance callback;
            private readonly object thisObj;
            private readonly object[] arguments;

            public EventLoopInstance(FunctionInstance callback, object thisObj, object[] arguments)
            {
                this.callback = callback;
                this.thisObj = thisObj;
                this.arguments = arguments;
            }

            public void Invoke() => callback.Call(thisObj, arguments);
        }

        /// <summary>
        /// Gets the number of pending callbacks.
        /// </summary>
        public int Count => pendingCallbacks.Count;

        private readonly ConcurrentQueue<EventLoopInstance> pendingCallbacks;

        internal EventLoop()
        {
            pendingCallbacks = new ConcurrentQueue<EventLoopInstance>();
        }

        /// <summary>
        /// Appends a callback to the EventLoop that will be executed the next time
        /// <see cref="ExecutePendingCallbacks"/> is invoked.
        /// </summary>
        /// <param name="callback">The callback function.</param>
        /// <param name="thisObj"> The value of <c>this</c> in the context of the function. </param>
        /// <param name="arguments"> Any number of arguments that will be passed to the function. </param>
        public void PendCallback(FunctionInstance callback, object thisObj, params object[] arguments)
        {
            if (callback != null)
            {
                var instance = new EventLoopInstance(callback, thisObj, arguments);
                pendingCallbacks.Enqueue(instance);
            }
        }

        /// <summary>
        /// Executes any callbacks registered with <see cref="PendCallback(FunctionInstance, object, object[])"/>.
        /// </summary>
        /// <returns>A value indicating whether any callbacks were invoked.</returns>
        public bool ExecutePendingCallbacks()
        {
            var result = false;
            while (pendingCallbacks.TryDequeue(out EventLoopInstance instance))
            {
                result = true;
                instance.Invoke();
            }
            return result;
        }
    }
}
