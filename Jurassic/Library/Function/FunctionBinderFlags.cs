using System;

namespace Jurassic.Library
{
    [Flags]
    public enum FunctionBinderFlags
    {
        /// <summary>
        /// No flags were specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The first parameter to the function is the <c>this</c> value.
        /// </summary>
        HasThisObject = 1,

        /// <summary>
        /// When multiple methods are found that are equally valid for the purposes of finding a
        /// method to call, this method should be chosen.  Usually required when there are multiple
        /// methods with the same number of parameters.
        /// </summary>
        Preferred = 2,
    }
}
