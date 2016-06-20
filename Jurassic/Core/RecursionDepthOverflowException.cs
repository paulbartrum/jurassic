using System;

namespace Jurassic
{
    /// <summary>
    /// The exception that is thrown when the allowed recursion depth for user-defined functions
    /// of a script engine has been exceeded.
    /// </summary>
    public class RecursionDepthOverflowException : Exception
    {
        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new RecursionDepthOverflowException instance.
        /// </summary>
        public RecursionDepthOverflowException()
            : base("The allowed recursion depth of the script engine has been exceeded.")
        {
        }
    }
}
