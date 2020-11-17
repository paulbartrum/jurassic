using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the base class of all javascript expressions.
    /// </summary>
    internal abstract class Expression : AstNode
    {
        /// <summary>
        /// Evaluates the expression, if possible.
        /// </summary>
        /// <returns> The result of evaluating the expression, or <c>null</c> if the expression can
        /// not be evaluated. </returns>
        public virtual object Evaluate()
        {
            return null;
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public virtual PrimitiveType ResultType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Checks the expression is valid and throws a SyntaxErrorException if not.
        /// Called after the expression tree is fully built out.
        /// </summary>
        /// <param name="context"> Indicates where the code is located e.g. inside a function, or a constructor, etc. </param>
        /// <param name="parent"> The parent expression in the tree. </param>
        /// <param name="lineNumber"> The line number to use when throwing an exception. </param>
        /// <param name="sourcePath"> The source path to use when throwing an exception. </param>
        public virtual void CheckValidity(CodeContext context, Expression parent, int lineNumber, string sourcePath)
        {
            // Override this to check validity.
        }
    }

}