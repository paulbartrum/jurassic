using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Used to build array literal and function parameter lists.
    /// Supports adding individual values as well as iterables.
    /// </summary>
    public class ArrayBuilder
    {
        private List<object> list;

        /// <summary>
        /// Initialize a new ArrayBuilder instance.
        /// </summary>
        /// <param name="capacity"> The expected array length. </param>
        public ArrayBuilder(int capacity)
        {
            this.list = new List<object>(capacity);
        }

        /// <summary>
        /// Adds a value to the array.
        /// </summary>
        /// <param name="item"> The value to add. </param>
        public void Add(object item)
        {
            this.list.Add(item);
        }

        /// <summary>
        /// Adds each element of an iterable separately to the array. An exception is thrown if
        /// the passed in value isn't actually iterable.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="iterable"> The iterable value. </param>
        public void AddIterable(ScriptEngine engine, object iterable)
        {
            foreach (var obj in TypeUtilities.ForOf(engine, iterable))
                Add(obj);
        }

        /// <summary>
        /// Returns an object array.
        /// </summary>
        /// <returns> An object array. </returns>
        public object[] ToArray()
        {
            return this.list.ToArray();
        }
    }
}
