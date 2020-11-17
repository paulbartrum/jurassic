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
        /// Checks the expression is valid and throws a SyntaxErrorException if not.
        /// Called after the expression tree is fully built out.
        /// </summary>
        /// <param name="context"> Indicates where the code is located e.g. inside a function, or a constructor, etc. </param>
        /// <param name="parent"> The parent expression in the tree. </param>
        /// <param name="lineNumber"> The line number to use when throwing an exception. </param>
        /// <param name="sourcePath"> The source path to use when throwing an exception. </param>
        public override void CheckValidity(CodeContext context, Expression parent, int lineNumber, string sourcePath)
        {
            if (parent is FunctionCallExpression)
            {
                if (context != CodeContext.DerivedConstructor)
                    throw new SyntaxErrorException("'super' calls can only be made from a derived constructor.", lineNumber, sourcePath);
            }
            else if (parent is MemberAccessExpression)
            {
                if (context != CodeContext.ObjectLiteralFunction &&
                    context != CodeContext.Constructor &&
                    context != CodeContext.DerivedConstructor &&
                    context != CodeContext.ClassFunction)
                    throw new SyntaxErrorException("'super' keyword unexpected here.", lineNumber, sourcePath);
            }
            else
            {
                throw new SyntaxErrorException("'super' keyword cannot be used on it's own.", lineNumber, sourcePath);
            }
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