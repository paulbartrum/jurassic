using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a 'super' reference.
    /// </summary>
    internal sealed class SuperExpression : Expression
    {
        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Object; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            EmitHelpers.LoadExecutionContext(generator);
            generator.Call(ReflectionHelpers.ExecutionContext_GetSuperValue);
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