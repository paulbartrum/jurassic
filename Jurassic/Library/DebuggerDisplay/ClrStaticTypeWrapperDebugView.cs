using System;
using System.Diagnostics;

namespace Jurassic.Library
{
    /// <summary>
    /// Debuger decorator for ClrStaticTypeWrapper
    /// </summary>
    internal class ClrStaticTypeWrapperDebugView
    {
        /// <summary>
        /// The displayed type
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ClrStaticTypeWrapper clrStaticTypeWrapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clrStaticTypeWrapper">The displayed type</param>
        public ClrStaticTypeWrapperDebugView(ClrStaticTypeWrapper clrStaticTypeWrapper)
        {
            this.clrStaticTypeWrapper = clrStaticTypeWrapper;
        }


        /// <summary>
        /// Original type
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        protected Type WrappedType
        {
            get
            {
                return this.clrStaticTypeWrapper.WrappedType;
            }
        }
    }
}
