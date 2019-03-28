using System;
using System.Diagnostics;


namespace Jurassic.Library
{
    /// <summary>
    /// Debuger decorator for ClrInstanceTypeWrapper
    /// </summary>
    internal class ClrInstanceTypeWrapperDebugView
    {
        /// <summary>
        /// The displayed object
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ClrInstanceTypeWrapper clrInstanceTypeWrapper;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clrInstanceTypeWrapper">The displayed object</param>
        public ClrInstanceTypeWrapperDebugView(ClrInstanceTypeWrapper clrInstanceTypeWrapper)
        {
            this.clrInstanceTypeWrapper = clrInstanceTypeWrapper;
        }


        /// <summary>
        /// Original object
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        protected Type WrappedType
        {
            get
            {
                return this.clrInstanceTypeWrapper.WrappedType;
            }
        }
    }
}
