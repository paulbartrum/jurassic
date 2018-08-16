using System.Diagnostics;


namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for ClrInstanceWrapper
    /// </summary>
    public class ClrInstanceWrapperDebugView
    {
        /// <summary>
        /// The displayed object
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ClrInstanceWrapper clrInstanceWrapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clrInstanceWrapper">The displayed object</param>
        public ClrInstanceWrapperDebugView(ClrInstanceWrapper clrInstanceWrapper)
        {
            this.clrInstanceWrapper = clrInstanceWrapper;
        }

        /// <summary>
        /// Original object
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        protected object WrappedInstance
        {
            get
            {
                return this.clrInstanceWrapper.WrappedInstance;
            }
        }
    }
}
