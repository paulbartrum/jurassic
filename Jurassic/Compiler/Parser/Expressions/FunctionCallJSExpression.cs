using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a function call expression.
    /// </summary>
    internal class FunctionCallJSExpression : OperatorJSExpression
    {
        /// <summary>
        /// Intializes static members of the class.
        /// </summary>
        static FunctionCallJSExpression()
        {
            
        }

        /// <summary>
        /// Creates a new instance of FunctionCallJSExpression.
        /// </summary>
        /// <param name="operator"> The binary operator to base this expression on. </param>
        public FunctionCallJSExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Gets an expression that evaluates to the function instance.
        /// </summary>
        public JSExpression Target
        {
            get { return this.GetOperand(0); }
        }

        /// <summary>
        /// Gets the arguments of this function call as an expression.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A new array expression. </returns>
        public NewArrayExpression GetArgumentArrayExpression(ParameterExpression scope)
        {
            // Right hand side is either null, or a single expression, or a comma-delimited list.
            if (this.OperandCount < 2)
            {
                // No parameters.
                return Expression.NewArrayInit(typeof(object));
            }
            else
            {
                var right = this.GetOperand(1);
                if (right is ListJSExpression)
                {
                    // Multiple parameters.
                    return Expression.NewArrayInit(typeof(object), ((ListJSExpression)right).GetItemExpressions(scope, true));
                }
                else
                {
                    // Single parameter.
                    return Expression.NewArrayInit(typeof(object), Expression.Convert(right.ToExpression(scope), typeof(object)));
                }
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

            // Get an expression for the function instance.
            var function = this.Target;

            // Determine the "this" value.  There are three cases.
            // 1. function is a member expression with no base (e.g. "parseInt()").      this = scope.GetImplicitThisValue()
            // 2. function is a member expression with a base (e.g. "Math.cos()").       this = Math
            // 3. function is not a member expression (e.g. "new Number(5).toString()")  this = undefined
            Expression thisExpression = ExpressionTreeHelpers.Undefined();
            if (function is MemberJSExpression)
            {
                if (((MemberJSExpression)function).Base == null)
                    thisExpression = Expression.Call(
                        scope,
                        ReflectionHelpers.LexicalScope_GetImplicitThisValue,
                        ((MemberJSExpression)function).GetNameExpression(scope));
                else
                    thisExpression = ((MemberJSExpression)function).GetBaseExpression(scope);
            }

            // Call the function.
            // object functionInstance = lhs;
            // if ((functionInstance is FunctionInstance) == false)
            //     throw new JavaScriptException("TypeError", "Expected function")
            // ((FunctionInstance)functionInstance).CallLateBound(thisValue, arguments);
            var functionInstance = Expression.Variable(typeof(object), "functionInstance");
            return Expression.Block(
                new ParameterExpression[] { functionInstance },
                Expression.Assign(functionInstance, function.ToExpression(scope)),
                Expression.IfThen(Expression.IsFalse(Expression.TypeIs(functionInstance, typeof(FunctionInstance))), ExpressionTreeHelpers.Throw("TypeError", "Expected function")),
                Expression.Call(
                    Expression.Convert(functionInstance, typeof(FunctionInstance)),
                    ReflectionHelpers.FunctionInstance_CallLateBound,
                    thisExpression,
                    this.GetArgumentArrayExpression(scope)));
        }
    }

}