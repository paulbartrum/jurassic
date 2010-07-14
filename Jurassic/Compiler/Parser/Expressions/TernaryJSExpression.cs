using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a ternary operator expression.
    /// </summary>
    internal sealed class TernaryJSExpression : OperatorJSExpression
    {
        /// <summary>
        /// Creates a new instance of TernaryJSExpression.
        /// </summary>
        /// <param name="operator"> The ternary operator to base this expression on. </param>
        public TernaryJSExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Converts the JSExpression into a regular .NET expression.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A regular .NET expression. </returns>
        public override Expression ToExpression(ParameterExpression scope)
        {
            CheckValid();
            var operand1 = this.GetOperand(0).ToExpression(scope);
            var operand2 = this.GetOperand(1).ToExpression(scope);
            var operand3 = this.GetOperand(2).ToExpression(scope);
            return Expression.Condition(
                ExpressionTreeHelpers.ToBoolean(operand1),
                Expression.Convert(operand2, typeof(object)),
                Expression.Convert(operand3, typeof(object)));
        }
    }

}