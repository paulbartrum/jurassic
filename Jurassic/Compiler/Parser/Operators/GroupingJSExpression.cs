using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a grouping expression.
    /// </summary>
    internal sealed class GroupingJSExpression : OperatorJSExpression
    {
        /// <summary>
        /// Creates a new instance of GroupingJSExpression.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        public GroupingJSExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Gets the expression inside the grouping operator.
        /// </summary>
        public JSExpression Operand
        {
            get { return this.GetOperand(0); }
        }

        /// <summary>
        /// Converts the JSExpression into a regular .NET expression.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A regular .NET expression. </returns>
        public override Expression ToExpression(ParameterExpression scope)
        {
            CheckValid();
            return this.Operand.ToExpression(scope);
        }
    }

}