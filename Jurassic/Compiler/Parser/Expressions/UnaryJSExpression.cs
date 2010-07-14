using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a unary operator expression.
    /// </summary>
    internal sealed class UnaryJSExpression : OperatorJSExpression
    {
        /// <summary>
        /// Creates a new instance of UnaryJSExpression.
        /// </summary>
        /// <param name="operator"> The unary operator to base this expression on. </param>
        public UnaryJSExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Gets the expression on the left or right side of the unary operator.
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

            var operand = this.Operand.ToExpression(scope);
            switch (this.Operator.Type)
            {
                case OperatorType.Plus:
                    // ToNumber(operand)
                    return ExpressionTreeHelpers.ToNumber(operand);

                case OperatorType.Minus:
                    // -ToNumber(operand)
                    return Expression.Negate(ExpressionTreeHelpers.ToNumber(operand));

                case OperatorType.BitwiseNot:
                    // ~ToInt32(operand)
                    return Expression.Convert(Expression.Not(ExpressionTreeHelpers.ToInt32(operand)), typeof(double));

                case OperatorType.LogicalNot:
                    // !ToBoolean(operand)
                    return Expression.Not(ExpressionTreeHelpers.ToBoolean(operand));

                case OperatorType.Void:
                    return Expression.Block(operand, ExpressionTreeHelpers.Undefined());

                case OperatorType.Typeof:
                    // unresolvable references must return "undefined" rather than throw an error.
                    if (this.Operand is MemberJSExpression)
                        return Expression.Condition(
                            ((MemberJSExpression)this.Operand).GetReferenceExistsExpression(scope),
                            Expression.Call(ReflectionHelpers.TypeUtilities_TypeOf, Expression.Convert(operand, typeof(object))),
                            Expression.Constant("undefined"));

                    // All other expression types are resolvable.
                    return Expression.Call(ReflectionHelpers.TypeUtilities_TypeOf, Expression.Convert(operand, typeof(object)));

                case OperatorType.Delete:
                    if (this.Operand is MemberJSExpression)
                    {
                        // delete a or delete a.b
                        return Expression.Call(
                            ((MemberJSExpression)this.Operand).GetBaseExpression(scope),
                            ReflectionHelpers.IEnvironmentRecord_DeleteBinding,
                            ((MemberJSExpression)this.Operand).GetNameExpression(scope));
                    }
                    else
                    {
                        // delete "toString"
                        return Expression.Call(
                            scope,
                            ReflectionHelpers.IEnvironmentRecord_DeleteBinding,
                            ExpressionTreeHelpers.ToString(operand));
                    }

                default:
                    throw new InvalidOperationException("Unsupported ExpressionType.");
            }
        }
    }

}