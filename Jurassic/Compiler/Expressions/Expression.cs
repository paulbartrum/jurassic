using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the base class of all javascript expressions.
    /// </summary>
    internal abstract class Expression : AstNode
    {
        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public virtual PrimitiveType ResultType
        {
            get { throw new NotImplementedException(); }
        }
    }

}