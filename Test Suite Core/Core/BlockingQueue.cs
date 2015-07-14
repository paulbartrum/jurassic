using System;
using System.Collections.Generic;
using System.Threading;

namespace Jurassic.TestSuite
{

    /// <summary>
    /// Implements a thread-safe blocking queue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Based on the class here: http://stackoverflow.com/questions/530211/creating-a-blocking-queuet-in-net/530228#530228
    /// By Marc Gravell
    /// </remarks>
    public class BlockingQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly int maxSize;

        /// <summary>
        /// Creates a new BlockingQueue instance with the given maximum size.
        /// </summary>
        /// <param name="maxSize"> The maximum number of items the queue can hold. </param>
        public BlockingQueue(int maxSize)
        {
            this.maxSize = maxSize;
        }

        bool closing;

        /// <summary>
        /// Indicates that no more items will be added to the queue.
        /// </summary>
        public void Close()
        {
            lock (queue)
            {
                closing = true;
                Monitor.PulseAll(queue);
            }
        }

        /// <summary>
        /// Adds a new item to the end of the queue.  Blocks if the queue already has the maximum
        /// number of items.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            lock (queue)
            {
                while (queue.Count >= maxSize)
                {
                    Monitor.Wait(queue);
                }
                queue.Enqueue(item);
                if (queue.Count == 1)
                {
                    // wake up any blocked dequeue
                    Monitor.PulseAll(queue);
                }
            }
        }

        /// <summary>
        /// Trys to remove an item from start of the queue.  Blocks if there are no items in the
        /// queue, unless the queue has been closed.
        /// </summary>
        /// <param name="value"> Set to the queue item after the method returns. </param>
        /// <returns> <c>true</c> if a value was successfully retrieved; <c>false</c> if the queue
        /// was empty and has been closed. </returns>
        public bool TryDequeue(out T value)
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    if (closing)
                    {
                        value = default(T);
                        return false;
                    }
                    Monitor.Wait(queue);
                }
                value = queue.Dequeue();
                if (queue.Count == maxSize - 1)
                {
                    // wake up any blocked enqueue
                    Monitor.PulseAll(queue);
                }
                return true;
            }
        }
    }
}
