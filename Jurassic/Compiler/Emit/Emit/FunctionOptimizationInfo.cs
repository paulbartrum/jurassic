using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents information useful for optimizing a function.
    /// </summary>
    internal class FunctionOptimizationInfo
    {
        /// <summary>
        /// Gets a value that indicates whether the function being generated contains an eval
        /// statement.
        /// </summary>
        public bool HasEval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value that indicates whether the function being generated contains a nested
        /// function declaration or expression.
        /// </summary>
        public bool HasNestedFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value that indicates whether the function being generated contains a reference
        /// to the arguments object.
        /// </summary>
        public bool HasArguments
        {
            get;
            set;
        }
    }

}
