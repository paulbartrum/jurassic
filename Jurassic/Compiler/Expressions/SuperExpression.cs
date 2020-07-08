using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a 'super' reference.
    /// </summary>
    internal sealed class SuperExpression : Expression
    {
        /// <summary>
        /// Creates a new instance of SuperExpression.
        /// </summary>
        public SuperExpression()
        {
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Any; }
        }

        /// <summary>
        /// Gets the static type of the reference.
        /// </summary>
        public PrimitiveType Type
        {
            get { return PrimitiveType.Any; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks the expression is valid and throws a SyntaxErrorException if not.
        /// Called after the expression tree is fully built out.
        /// </summary>
        /// <param name="lineNumber"> The line number to use when throwing an exception. </param>
        /// <param name="sourcePath"> The source path to use when throwing an exception. </param>
        public override void CheckValidity(int lineNumber, string sourcePath)
        {
            throw new SyntaxErrorException("'super' keyword unexpected here.", lineNumber, sourcePath);
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            return "super";
        }
    }
}