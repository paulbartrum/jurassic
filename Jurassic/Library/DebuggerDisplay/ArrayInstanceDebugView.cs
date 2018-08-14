using System.Diagnostics;
using System.Linq;


namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for ArrayInstance.
    /// </summary>
    public class ArrayInstanceDebugView
    {
        /// <summary>
        /// The watched array
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ArrayInstance arrayInstance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="arrayInstance">The watched object</param>
        public ArrayInstanceDebugView(ArrayInstance arrayInstance)
        {
            this.arrayInstance = arrayInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.arrayInstance.Prototype;
            }
        }


        /// <summary>
        /// Array properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.arrayInstance.Properties.Where(
                    pnv => (pnv.Key is string) && (pnv.Key as string) != "length").ToArray();
            }
        }

        /// <summary>
        /// The count of array elements
        /// </summary>
        public uint length
        {
            get
            {
                return this.arrayInstance.Length;
            }
        }
    }
}
