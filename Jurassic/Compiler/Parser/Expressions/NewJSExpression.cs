using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a "new" expression.
    /// </summary>
    internal sealed class NewJSExpression : OperatorJSExpression
    {
        /// <summary>
        /// Creates a new instance of NewCallJSExpression.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        public NewJSExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Gets the precedence of the operator.
        /// </summary>
        public override int Precedence
        {
            get
            {
                // The expression "new String('').toString()" is parsed as
                // "new (String('').toString())" rather than "(new String('')).toString()".
                // There is no way to express this constraint properly using the standard operator
                // rules so we artificially boost the precedence of the operator if a function
                // call is encountered.  Note: GetRawOperand() is used instead of GetOperand(0)
                // because parentheses around the function call affect the result.
                return this.OperandCount == 1 && this.GetRawOperand(0) is FunctionCallJSExpression ?
                    int.MaxValue : this.Operator.Precedence;
            }
        }

        /// <summary>
        /// Converts the JSExpression into a regular .NET expression.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A regular .NET expression. </returns>
        public override Expression ToExpression(ParameterExpression scope)
        {
            CheckValid();

            // Note: we use GetRawOperand() so that grouping operators are not ignored.
            var operand = this.GetRawOperand(0);

            // There is only one operand, and it can be either a reference or a function call.
            // We need to split the operand into a function and some arguments.
            // If the operand is a reference, it is equivalent to a function call with no arguments.
            Expression function;
            NewArrayExpression arguments;
            if (operand is FunctionCallJSExpression)
            {
                function = ((FunctionCallJSExpression)operand).Target.ToExpression(scope);
                arguments = ((FunctionCallJSExpression)operand).GetArgumentArrayExpression(scope);
            }
            else
            {
                function = operand.ToExpression(scope);
                arguments = Expression.NewArrayInit(typeof(object));
            }

            // Call the function.
            return Expression.Block(
                Expression.IfThen(Expression.IsFalse(Expression.TypeIs(function, typeof(FunctionInstance))),
                    ExpressionTreeHelpers.Throw("TypeError", "Expected function in new expression")),
                Expression.Call(
                    Expression.Convert(function, typeof(FunctionInstance)),
                    ReflectionHelpers.FunctionInstance_ConstructLateBound,
                    arguments));
        }
    }

}