namespace Jurassic.Compiler
{
    
    /// <summary>
    /// Represents a reference to the "new.target" value.
    /// </summary>
    internal sealed class NewTargetExpression : Expression
    {
        /// <summary>
        /// Creates a new NewTargetExpression instance.
        /// </summary>
        public NewTargetExpression()
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
            EmitHelpers.LoadNewTarget(generator);
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            return "new.target";
        }
    }
}