using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a context that code can be run in.
    /// </summary>
    internal enum CodeContext
    {
        /// <summary>
        /// The default context.
        /// </summary>
        Global,
        GlobalEval,

        /// <summary>
        /// The context inside function bodies.
        /// </summary>
        Function,
        ObjectLiteralFunction,
        ClassFunction,
        Constructor,
        DerivedConstructor,

        /// <summary>
        /// The context inside the eval() function.
        /// </summary>
        Eval,
    }

}