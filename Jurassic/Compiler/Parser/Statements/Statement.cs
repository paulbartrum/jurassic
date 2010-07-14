using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a javascript statement.
    /// </summary>
    internal abstract class Statement
    {
        private List<string> labels;

        /// <summary>
        /// Creates a new Statement instance.
        /// </summary>
        /// <param name="labels"> The labels that are associated with this statement. </param>
        public Statement(IList<string> labels)
        {
            if (labels != null && labels.Count > 0)
                this.labels = new List<string>(labels);
        }

        /// <summary>
        /// Returns a value that indicates whether the statement has one or more labels attached to
        /// it.
        /// </summary>
        public bool HasLabels
        {
            get { return this.labels != null; }
        }

        /// <summary>
        /// Gets or sets the labels associated with this statement.
        /// </summary>
        public IList<string> Labels
        {
            get { return this.labels; }
        }

        /// <summary>
        /// Gets or sets the portion of source code associated with this statement.
        /// </summary>
        public SourceCodeSpan DebugInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Visits every node in the statement.
        /// </summary>
        /// <param name="visitor"> The visitor callback. </param>
        public virtual void Visit(Action<Statement> visitor)
        {
            visitor(this);
        }

        /// <summary>
        /// Optimizes the expression tree.
        /// </summary>
        public virtual void Optimize()
        {
            Visit(statement =>
                {
                    if (statement != this)
                        statement.Optimize();
                });
        }

        /// <summary>
        /// Generates CIL for the statement.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public virtual void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
#if DEBUG
            // Statements must not produce or consume any values on the stack.
            int originalStackSize = 0;
            if (generator is DynamicILGenerator)
                originalStackSize = ((DynamicILGenerator)generator).StackSize;
#endif

            ILLabel endOfStatement = null;
            if (this.HasLabels == true)
            {
                // Set up the information needed by the break statement.
                endOfStatement = generator.CreateLabel();
                optimizationInfo.PushBreakOrContinueInfo(this.Labels, endOfStatement, null, true);
            }

            // Generate the code.
            this.GenerateCodeCore(generator, optimizationInfo);

            if (this.HasLabels == true)
            {
                // Revert the information needed by the break statement.
                generator.DefineLabelPosition(endOfStatement);
                optimizationInfo.PopBreakOrContinueInfo();
            }

#if DEBUG
            // Check that the stack count is zero.
            if (generator is DynamicILGenerator && ((DynamicILGenerator)generator).StackSize != originalStackSize)
                throw new InvalidOperationException("Encountered unexpected stack imbalance.");
#endif
        }

        /// <summary>
        /// Generates CIL for the statement.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected virtual void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
        }

        /// <summary>
        /// Converts the statement to a string.
        /// </summary>
        /// <param name="indentLevel"> The number of tabs to include before the statement. </param>
        /// <returns> A string representing this statement. </returns>
        public override string ToString()
        {
            return this.ToString(0);
        }

        /// <summary>
        /// Converts the statement to a string.
        /// </summary>
        /// <param name="indentLevel"> The number of tabs to include before the statement. </param>
        /// <returns> A string representing this statement. </returns>
        public abstract string ToString(int indentLevel);
    }

}