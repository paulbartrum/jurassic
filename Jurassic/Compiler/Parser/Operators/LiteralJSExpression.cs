using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a literal expression.
    /// </summary>
    internal sealed class LiteralJSExpression : JSExpression
    {
        /// <summary>
        /// Creates a new instance of LiteralJSExpression.
        /// </summary>
        /// <param name="value"> The literal value. </param>
        public LiteralJSExpression(object value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the literal value.
        /// </summary>
        public object Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Converts the JSExpression into a regular .NET expression.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A regular .NET expression. </returns>
        public override Expression ToExpression(ParameterExpression scope)
        {
            if (this.Value is RegularExpressionLiteralValue)
            {
                // This is a regular expression literal.
                var regExpValue = (RegularExpressionLiteralValue)this.Value;
                return Expression.Call(
                        Expression.Call(ReflectionHelpers.Global_RegExp),
                        ReflectionHelpers.RegExp_Construct,
                        Expression.Constant(regExpValue.Body),
                        Expression.Constant(regExpValue.Flags));
            }
            else if (this.Value is JSExpression[])
            {
                // This is an array literal.
                var items = (JSExpression[])this.Value;
                var itemExpressions = new Expression[items.Length];
                for (int i = 0; i < itemExpressions.Length; i++)
                    itemExpressions[i] = Expression.Convert(items[i].ToExpression(scope), typeof(object));
                return Expression.Call(
                    Expression.Call(ReflectionHelpers.Global_Array),
                    ReflectionHelpers.Array_New,
                    Expression.NewArrayInit(typeof(object), itemExpressions));
            }
            else if (this.Value is Dictionary<string, JSExpression>)
            {
                // This is an object literal.
                var properties = (Dictionary<string, JSExpression>)this.Value;
                var instanceVariable = Expression.Variable(typeof(ObjectInstance));
                var statements = new List<Expression>(properties.Count + 2);
                statements.Add(
                    Expression.Assign(instanceVariable,
                        Expression.Call(
                            Expression.Call(ReflectionHelpers.Global_Object),
                            ReflectionHelpers.Object_Construct)));
                foreach (var keyValuePair in properties)
                    statements.Add(Expression.Call(instanceVariable,
                        ReflectionHelpers.ObjectInstance_Put,
                        Expression.Constant(keyValuePair.Key),
                        Expression.Convert(keyValuePair.Value.ToExpression(scope), typeof(object)),
                        Expression.Constant(false)));
                statements.Add(instanceVariable);
                return Expression.Block(new ParameterExpression[] { instanceVariable }, statements);
            }
            else if (this.Value is Expression)
            {
                // This is a function expression.
                return (Expression)this.Value;
            }
            else if (this.Value == Null.Value)
            {
                // This is the null keyword.
                return ExpressionTreeHelpers.Null();
            }
            else
                // This is a boolean, number or string literal.
                return Expression.Constant(this.Value);
        }
    }


}