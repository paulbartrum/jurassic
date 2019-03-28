using System.Diagnostics;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for ObjectInstance and some classes inheriting ObjectInstance.
    /// </summary>
    internal class ObjectInstanceDebugView
    {
        /// <summary>
        /// The displayed object
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ObjectInstance objectInstance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="objectInstance"></param>
        public ObjectInstanceDebugView(ObjectInstance objectInstance)
        {
            this.objectInstance = objectInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.objectInstance.Prototype;
            }
        }

        /// <summary>
        /// Object properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.objectInstance.Properties.ToArray();
            }
        }
    }
}
