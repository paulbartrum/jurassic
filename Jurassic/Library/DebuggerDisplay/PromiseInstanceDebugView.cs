using System.Diagnostics;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for PromiseInstance.
    /// </summary>
    internal class PromiseInstanceDebugView
    {
        /// <summary>
        /// The displayed PromiseInstance
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected PromiseInstance promiseInstance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="promiseInstance">The displayed PromiseInstance</param>
        public PromiseInstanceDebugView(PromiseInstance promiseInstance)
        {
            this.promiseInstance = promiseInstance;
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.promiseInstance.Prototype;
            }
        }

        /// <summary>
        /// PromiseInstance properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.promiseInstance.Properties.ToArray();
            }
        }

        /// <summary>
        /// Gets the status
        /// </summary>
        public string PromiseStatus
        {
            get { return this.promiseInstance.State.ToString(); }
        }

        /// <summary>
        /// Gets the value
        /// </summary>
        public object PromiseValue
        {
            get { return this.promiseInstance.Result; }
        }
    }
}
