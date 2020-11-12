using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Used to build array literal and function parameter lists.
    /// </summary>
    public class ArrayBuilder
    {
        private List<object> list;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public ArrayBuilder(int capacity)
        {
            this.list = new List<object>(capacity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(object item)
        {
            this.list.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="iterable"></param>
        public void AddIterable(ScriptEngine engine, object iterable)
        {
            foreach (var obj in TypeUtilities.ForOf(engine, iterable))
                Add(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object[] ToArray()
        {
            return this.list.ToArray();
        }
    }
}
