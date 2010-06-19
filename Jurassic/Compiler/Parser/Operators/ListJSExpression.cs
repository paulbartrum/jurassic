using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a comma-delimited list.
    /// </summary>
    internal class ListJSExpression : OperatorJSExpression
    {
        /// <summary>
        /// Creates a new instance of ListJSExpression.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        public ListJSExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Gets an array of expressions, one for each item in the list.
        /// </summary>
        public JSExpression[] Items
        {
            get
            {
                var result = new List<JSExpression>();
                JSExpression leftHandSide = this;
                while (leftHandSide is ListJSExpression)
                {
                    result.Add(((ListJSExpression)leftHandSide).GetOperand(1));
                    leftHandSide = ((ListJSExpression)leftHandSide).GetOperand(0);
                }
                result.Add(leftHandSide);
                result.Reverse();
                return result.ToArray();
            }
        }

        /// <summary>
        /// Gets an array of .NET expressions, one for each item in the list.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <param name="convertToObject"> <c>true</c> if each expression should be cast to
        /// object; <c>false</c> otherwise. </param>
        /// <returns> An array of .NET expressions, one for each item in the list. </returns>
        public Expression[] GetItemExpressions(ParameterExpression scope, bool convertToObject)
        {
            var items = this.Items;
            var result = new Expression[items.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = items[i].ToExpression(scope);
                if (convertToObject)
                    result[i] = Expression.Convert(items[i].ToExpression(scope), typeof(object));
            }
            return result;
        }

        /// <summary>
        /// Converts the JSExpression into a regular .NET expression.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A regular .NET expression. </returns>
        public override Expression ToExpression(ParameterExpression scope)
        {
            CheckValid();
            return Expression.Block(GetItemExpressions(scope, false));
        }
    }

}