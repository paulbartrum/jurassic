using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents the debugger statement.
    /// </summary>
    internal class DebuggerStatement : Statement
    {
        /// <summary>
        /// Creates a new DebuggerStatement instance.
        /// </summary>
        /// <param name="labels"> The labels that are associated with this statement. </param>
        public DebuggerStatement(IList<string> labels)
            : base(labels)
        {
        }

        /// <summary>
        /// Generates CIL for the statement.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            generator.Breakpoint();
        }

        /// <summary>
        /// Converts the statement to a string.
        /// </summary>
        /// <param name="indentLevel"> The number of tabs to include before the statement. </param>
        /// <returns> A string representing this statement. </returns>
        public override string ToString(int indentLevel)
        {
            return new string('\t', indentLevel) + "debugger;";
        }
    }

}