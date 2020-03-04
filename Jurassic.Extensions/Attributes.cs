using Jurassic.Library;

namespace Jurassic.Extensions
{
    /// <summary>
    /// Marks a method as being visible to javascript code.
    /// Used internally - has different defaults to what you would expect.
    /// </summary>
    internal class JSInternalFunctionAttribute : JSFunctionAttribute
    {
        /// <summary>
        /// Gets or sets the number of parameters that are required.  If the function is called
        /// with fewer than this number of arguments, then a TypeError will be thrown.
        /// </summary>
        public int RequiredArgumentCount
        {
            get;
            set;
        }
    }
}
