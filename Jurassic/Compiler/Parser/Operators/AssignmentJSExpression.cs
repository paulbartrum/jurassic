using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents an assignment expression (++, --, =, +=, -=, *=, /=, %=, &=, |=, ^=, &lt;&lt;=, &gt;&gt;=, &gt;&gt;&gt;=).
    /// </summary>
    internal class AssignmentJSExpression : OperatorJSExpression
    {
        /// <summary>
        /// Creates a new instance of AssignmentJSExpression.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        public AssignmentJSExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Creates a variable assignment expression.
        /// </summary>
        /// <param name="variableName"> The name of the variable to set. </param>
        /// <param name="setterExpression"> The value to set the variable to. </param>
        public AssignmentJSExpression(string variableName, JSExpression setterExpression)
            : base(Operator.Assignment)
        {
            this.Push(new MemberJSExpression(variableName));
            this.Push(setterExpression);
        }

        /// <summary>
        /// Converts the JSExpression into a regular .NET expression.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A regular .NET expression. </returns>
        public override Expression ToExpression(ParameterExpression scope)
        {
            CheckValid();
            var target = this.GetOperand(0) as MemberJSExpression;
            if (target == null)
                throw new JavaScriptException("ReferenceError", "Invalid left-hand side in assignment", 1, "");

            // The value to store in the variable.
            Expression setterValue;

            // The value to return from the expression.
            bool returnValueSet = false;
            ParameterExpression returnValue = Expression.Variable(typeof(double));

            switch (this.Operator.Type)
            {
                case OperatorType.PostIncrement:
                    // target-base[target-name] = ToNumber(target) + 1, ToNumber(target)
                    setterValue = Expression.Increment(Expression.Assign(returnValue, ExpressionTreeHelpers.ToNumber(target.ToExpression(scope))));
                    returnValueSet = true;
                    break;

                case OperatorType.PostDecrement:
                    // target-base[target-name] = ToNumber(target) - 1, ToNumber(target)
                    setterValue = Expression.Decrement(Expression.Assign(returnValue, ExpressionTreeHelpers.ToNumber(target.ToExpression(scope))));
                    returnValueSet = true;
                    break;

                case OperatorType.PreIncrement:
                    // target-base[target-name] = ToNumber(target) + 1
                    setterValue = Expression.Increment(ExpressionTreeHelpers.ToNumber(target.ToExpression(scope)));
                    break;

                case OperatorType.PreDecrement:
                    // target-base[target-name] = ToNumber(target) - 1
                    setterValue = Expression.Decrement(ExpressionTreeHelpers.ToNumber(target.ToExpression(scope)));
                    break;

                default:
                    // Either a simple assignment operator or a compound assignment operator.
                    // All of which are binary operators.
                    if (this.Operator.Type == OperatorType.Assignment)
                    {
                        // The operator is a simple assignment operator.
                        setterValue = Expression.Convert(this.GetOperand(1).ToExpression(scope), typeof(object));
                    }
                    else
                    {
                        // The operator is a compound operator.
                        Operator baseOperator = GetCompoundBaseOperator(this.Operator.Type);
                        if (baseOperator == null)
                            throw new NotImplementedException("Unsupported assignment operator.");
                        setterValue = new BinaryJSExpression(baseOperator, target, this.GetOperand(1)).ToExpression(scope);
                    }
                    break;
            }

            // target-base.SetMutableBinding(target-name, setterValue)
            var result = target.GetSetterExpression(scope, setterValue);

            // If the return value is different from the setter value, use a temp variable to hold the intermediate value.
            if (returnValueSet == true)
            {
                result = Expression.Block(
                    new ParameterExpression[] { returnValue },
                    result,
                    returnValue);
            }

            return result;
        }

        /// <summary>
        /// Gets the underlying base operator for the given compound operator.
        /// </summary>
        /// <param name="compoundOperatorType"> The type of compound operator. </param>
        /// <returns> The underlying base operator, or <c>null</c> if the type is not a compound
        /// operator. </returns>
        private static Operator GetCompoundBaseOperator(OperatorType compoundOperatorType)
        {
            switch (compoundOperatorType)
            {
                case OperatorType.CompoundAdd:
                    return Operator.Add;
                case OperatorType.CompoundBitwiseAnd:
                    return Operator.BitwiseAnd;
                case OperatorType.CompoundBitwiseOr:
                    return Operator.BitwiseOr;
                case OperatorType.CompoundBitwiseXor:
                    return Operator.BitwiseXor;
                case OperatorType.CompoundDivide:
                    return Operator.Divide;
                case OperatorType.CompoundLeftShift:
                    return Operator.LeftShift;
                case OperatorType.CompoundModulo:
                    return Operator.Modulo;
                case OperatorType.CompoundMultiply:
                    return Operator.Multiply;
                case OperatorType.CompoundSignedRightShift:
                    return Operator.SignedRightShift;
                case OperatorType.CompoundSubtract:
                    return Operator.Subtract;
                case OperatorType.CompoundUnsignedRightShift:
                    return Operator.UnsignedRightShift;
            }
            return null;
        }
    }

}