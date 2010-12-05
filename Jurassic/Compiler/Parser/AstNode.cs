using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the base class of expressions and statements.
    /// </summary>
    internal abstract class AstNode
    {
        private static AstNode[] emptyNodeList = new AstNode[0];

        /// <summary>
        /// Gets an enumerable list of child nodes in the abstract syntax tree.
        /// </summary>
        public virtual IEnumerable<AstNode> ChildNodes
        {
            get { return emptyNodeList; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public abstract void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo);
    }
}
