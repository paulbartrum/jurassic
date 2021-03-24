using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Used internally to allow branching out of a finally block.
    /// </summary>
    internal class LongJumpException : Exception
    {
        /// <summary>
        /// Creates a new LongJumpException instance.
        /// </summary>
        /// <param name="routeID"> The route ID. </param>
        public LongJumpException(int routeID)
        {
            this.RouteID = routeID;
        }

        /// <summary>
        /// Gets the route ID.
        /// </summary>
        public int RouteID
        {
            get;
            private set;
        }
    }

}
