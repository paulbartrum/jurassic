using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a binary operator expression.
    /// </summary>
    internal class BinaryJSExpression : OperatorJSExpression
    {
        /// <summary>
        /// Creates a new instance of BinaryJSExpression.
        /// </summary>
        /// <param name="operator"> The binary operator to base this expression on. </param>
        public BinaryJSExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Creates a new instance of BinaryJSExpression.
        /// </summary>
        /// <param name="operator"> The binary operator to base this expression on. </param>
        /// <param name="left"> The operand on the left side of the operator. </param>
        /// <param name="right"> The operand on the right side of the operator. </param>
        public BinaryJSExpression(Operator @operator, JSExpression left, JSExpression right)
            : base(@operator)
        {
            this.Push(left);
            this.Push(right);
        }

        /// <summary>
        /// Gets the expression on the left side of the operator.
        /// </summary>
        public JSExpression Left
        {
            get { return this.GetOperand(0); }
        }

        /// <summary>
        /// Gets the expression on the right side of the operator.
        /// </summary>
        public JSExpression Right
        {
            get { return this.GetOperand(1); }
        }

        /// <summary>
        /// Converts the JSExpression into a regular .NET expression.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A regular .NET expression. </returns>
        public override Expression ToExpression(ParameterExpression scope)
        {
            CheckValid();
            var left = this.Left.ToExpression(scope);
            var right = this.Right.ToExpression(scope);

            switch (this.Operator.Type)
            {
                case OperatorType.Multiply:
                    // ToNumber(left) * ToNumber(right)
                    return Expression.Multiply(ExpressionTreeHelpers.ToNumber(left), ExpressionTreeHelpers.ToNumber(right));

                case OperatorType.Divide:
                    // ToNumber(left) / ToNumber(right)
                    return Expression.Divide(ExpressionTreeHelpers.ToNumber(left), ExpressionTreeHelpers.ToNumber(right));

                case OperatorType.Modulo:
                    // ToNumber(left) % ToNumber(right)
                    return Expression.Modulo(ExpressionTreeHelpers.ToNumber(left), ExpressionTreeHelpers.ToNumber(right));

                case OperatorType.Subtract:
                    // ToNumber(left) - ToNumber(right)
                    return Expression.Subtract(ExpressionTreeHelpers.ToNumber(left), ExpressionTreeHelpers.ToNumber(right));

                case OperatorType.Add:
                    // ToPrimitive(left) is string or ToPrimitive(right) is string ? String.Concat(ToString(left), ToString(right)) : ToNumber(left) + ToNumber(right)
                    return Expression.Call(ReflectionHelpers.TypeUtilities_Add, Expression.Convert(left, typeof(object)), Expression.Convert(right, typeof(object)));

                case OperatorType.LeftShift:
                    {
                        // ToInt32(left) << ToUint32(right & 0x1F)
                        var shiftCount = Expression.And(Expression.Convert(ExpressionTreeHelpers.ToUint32(right), typeof(int)), Expression.Constant(0x1F));
                        return Expression.Convert(Expression.LeftShift(ExpressionTreeHelpers.ToInt32(left), shiftCount), typeof(double));
                    }

                case OperatorType.SignedRightShift:
                    {
                        // ToInt32(left) >> ToUint32(right & 0x1F)
                        var shiftCount = Expression.And(Expression.Convert(ExpressionTreeHelpers.ToUint32(right), typeof(int)), Expression.Constant(0x1F));
                        return Expression.Convert(Expression.RightShift(ExpressionTreeHelpers.ToInt32(left), shiftCount), typeof(double));
                    }

                case OperatorType.UnsignedRightShift:
                    {
                        // ToUint32(left) >> ToUint32(right & 0x1F)
                        var shiftCount = Expression.And(Expression.Convert(ExpressionTreeHelpers.ToUint32(right), typeof(int)), Expression.Constant(0x1F));
                        return Expression.Convert(Expression.RightShift(ExpressionTreeHelpers.ToUint32(left), shiftCount), typeof(double));
                    }

                case OperatorType.Equal:
                    // TypeConverter.Equals(left, right)
                    return Expression.Call(ReflectionHelpers.TypeComparer_Equals,
                        Expression.Convert(left, typeof(object)),
                        Expression.Convert(right, typeof(object)));

                case OperatorType.StrictlyEqual:
                    // TypeConverter.StrictEquals(left, right)
                    return Expression.Call(ReflectionHelpers.TypeComparer_StrictEquals,
                        Expression.Convert(left, typeof(object)),
                        Expression.Convert(right, typeof(object)));

                case OperatorType.NotEqual:
                    // !TypeConverter.Equals(left, right)
                    // !TypeConverter.StrictEquals(left, right)
                    return Expression.Not(Expression.Call(ReflectionHelpers.TypeComparer_Equals,
                        Expression.Convert(left, typeof(object)),
                        Expression.Convert(right, typeof(object))));

                case OperatorType.StrictlyNotEqual:
                    // !TypeConverter.StrictEquals(left, right)
                    return Expression.Not(Expression.Call(ReflectionHelpers.TypeComparer_StrictEquals,
                        Expression.Convert(left, typeof(object)),
                        Expression.Convert(right, typeof(object))));

                case OperatorType.LessThan:
                    // TypeConverter.LessThan(left, right, true)
                    return Expression.Call(ReflectionHelpers.TypeComparer_LessThan,
                        Expression.Convert(left, typeof(object)),
                        Expression.Convert(right, typeof(object)),
                        Expression.Constant(true));

                case OperatorType.LessThanOrEqual:
                    // TypeConverter.LessThanOrEqual(left, right, true)
                    return Expression.Call(ReflectionHelpers.TypeComparer_LessThanOrEqual,
                        Expression.Convert(left, typeof(object)),
                        Expression.Convert(right, typeof(object)),
                        Expression.Constant(true));

                case OperatorType.GreaterThan:
                    // TypeConverter.LessThan(left, right, false)
                    return Expression.Call(ReflectionHelpers.TypeComparer_LessThan,
                        Expression.Convert(right, typeof(object)),
                        Expression.Convert(left, typeof(object)),
                        Expression.Constant(false));

                case OperatorType.GreaterThanOrEqual:
                    // TypeConverter.LessThanOrEqual(left, right, false)
                    return Expression.Call(ReflectionHelpers.TypeComparer_LessThanOrEqual,
                        Expression.Convert(right, typeof(object)),
                        Expression.Convert(left, typeof(object)),
                        Expression.Constant(false));

                case OperatorType.LogicalAnd:
                    // ToBoolean(left) ? right : left
                    return Expression.Condition(ExpressionTreeHelpers.ToBoolean(left),
                        Expression.Convert(right, typeof(object)),
                        Expression.Convert(left, typeof(object)));

                case OperatorType.LogicalOr:
                    // ToBoolean(left) ? left : right
                    return Expression.Condition(ExpressionTreeHelpers.ToBoolean(left),
                        Expression.Convert(left, typeof(object)),
                        Expression.Convert(right, typeof(object)));

                case OperatorType.BitwiseAnd:
                    // (double)(ToInt32(left) & ToInt32(right))
                    return Expression.Convert(Expression.And(ExpressionTreeHelpers.ToInt32(left), ExpressionTreeHelpers.ToInt32(right)), typeof(double));

                case OperatorType.BitwiseXor:
                    // (double)(ToInt32(left) ^ ToInt32(right))
                    return Expression.Convert(Expression.ExclusiveOr(ExpressionTreeHelpers.ToInt32(left), ExpressionTreeHelpers.ToInt32(right)), typeof(double));

                case OperatorType.BitwiseOr:
                    // (double)(ToInt32(left) | ToInt32(right))
                    return Expression.Convert(Expression.Or(ExpressionTreeHelpers.ToInt32(left), ExpressionTreeHelpers.ToInt32(right)), typeof(double));

                case OperatorType.InstanceOf:
                    // Instanceof operator.
                    // if ((right is FunctionInstance) == false)
                    //     throw new JavaScriptException("TypeError", "...")
                    // return ((FunctionInstance)right).HasInstance(left)
                    return Expression.Block(
                        Expression.IfThen(Expression.IsFalse(Expression.TypeIs(right, typeof(FunctionInstance))),
                            ExpressionTreeHelpers.Throw("TypeError", "Function expected")),
                        Expression.Call(Expression.Convert(right, typeof(FunctionInstance)), ReflectionHelpers.FunctionInstance_HasInstance, Expression.Convert(left, typeof(object))));

                case OperatorType.In:
                    // In operator.
                    // if ((right is ObjectInstance) == false)
                    //     throw new JavaScriptException("TypeError", "...")
                    // ((ObjectInstance)right).HasProperty(ToString(left))
                    if (right.Type != typeof(object) && typeof(ObjectInstance).IsAssignableFrom(right.Type) == false)
                        throw new JavaScriptException("TypeError", "In operator requires an object to search", 1, "");
                    return Expression.Block(
                        Expression.IfThen(Expression.IsFalse(Expression.TypeIs(right, typeof(ObjectInstance))),
                            ExpressionTreeHelpers.Throw("TypeError", "In operator requires an object to search")),
                        Expression.Call(
                            Expression.Convert(right, typeof(ObjectInstance)),
                            ReflectionHelpers.ObjectInstance_HasProperty,
                            ExpressionTreeHelpers.ToString(left)));

                default:
                    throw new NotImplementedException("Unsupported binary operator.");
            }
        }
    }

}