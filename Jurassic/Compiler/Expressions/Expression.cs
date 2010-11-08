using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the base class of all javascript expressions.
    /// </summary>
    internal abstract class Expression
    {
        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public virtual PrimitiveType ResultType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public abstract void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo);
    }

}