using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a return statement.
    /// </summary>
    internal class ReturnStatement : Statement
    {
        /// <summary>
        /// Creates a new ReturnStatement instance.
        /// </summary>
        /// <param name="labels"> The labels that are associated with this statement. </param>
        public ReturnStatement(IList<string> labels)
            : base(labels)
        {
        }

        /// <summary>
        /// Gets or sets the expression to return.  Can be <c>null</c> to return "undefined".
        /// </summary>
        public Expression Value
        {
            get;
            set;
        }

        /// <summary>
        /// Generates CIL for the statement.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Emit the return value.
            if (this.Value == null)
                EmitHelpers.EmitUndefined(generator);
            else
            {
                this.Value.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToAny(generator, this.Value.ResultType);
            }

            // Store the return value in a variable.
            generator.StoreVariable(optimizationInfo.ReturnVariable);

            // Branch to the end of the function.  Note: the return statement might be branching
            // from inside a try { } or finally { } block to outside.  EmitLongJump() handles this.
            optimizationInfo.EmitLongJump(generator, optimizationInfo.ReturnTarget);
        }

        /// <summary>
        /// Converts the statement to a string.
        /// </summary>
        /// <param name="indentLevel"> The number of tabs to include before the statement. </param>
        /// <returns> A string representing this statement. </returns>
        public override string ToString(int indentLevel)
        {
            var result = new System.Text.StringBuilder();
            result.Append(new string('\t', indentLevel));
            result.Append("return");
            if (this.Value != null)
            {
                result.Append(" ");
                result.Append(this.Value);
            }
            result.Append(";");
            return result.ToString();
        }
    }

}