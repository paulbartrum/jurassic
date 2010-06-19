using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Contains static methods to help build expression trees.
    /// </summary>
    internal static class ExpressionTreeHelpers
    {

        //     TYPE CONVERSION
        //_________________________________________________________________________________________

        /// <summary>
        /// Converts the given expression to a number using JavaScript type conversion rules.
        /// </summary>
        /// <param name="expression"> The expression to convert. </param>
        /// <returns> An expression with type <c>double</c>. </returns>
        public static Expression ToNumber(Expression expression)
        {
            if (expression.Type == typeof(double))
                return expression;
            return Expression.Call(ReflectionHelpers.TypeConverter_ToNumber, Expression.Convert(expression, typeof(object)));
        }

        /// <summary>
        /// Converts the given expression to a string using JavaScript type conversion rules.
        /// </summary>
        /// <param name="expression"> The expression to convert. </param>
        /// <returns> An expression with type <c>string</c>. </returns>
        public static Expression ToString(Expression expression)
        {
            if (expression.Type == typeof(string))
                return expression;
            return Expression.Call(ReflectionHelpers.TypeConverter_ToString, Expression.Convert(expression, typeof(object)));
        }

        /// <summary>
        /// Converts the given expression to a boolean using JavaScript type conversion rules.
        /// </summary>
        /// <param name="expression"> The expression to convert. </param>
        /// <returns> An expression with type <c>bool</c>. </returns>
        public static Expression ToBoolean(Expression expression)
        {
            if (expression.Type == typeof(bool))
                return expression;
            return Expression.Call(ReflectionHelpers.TypeConverter_ToBoolean, Expression.Convert(expression, typeof(object)));
        }

        /// <summary>
        /// Converts the given expression to an object using JavaScript type conversion rules.
        /// </summary>
        /// <param name="expression"> The expression to convert. </param>
        /// <returns> An expression with type <c>JSObject</c>. </returns>
        public static Expression ToObject(Expression expression)
        {
            if (expression.Type == typeof(ObjectInstance))
                return expression;
            return Expression.Call(ReflectionHelpers.TypeConverter_ToObject, Expression.Convert(expression, typeof(object)));
        }

        /// <summary>
        /// Converts the given expression to a 32-bit integer using JavaScript type conversion rules.
        /// </summary>
        /// <param name="expression"> The expression to convert. </param>
        /// <returns> An expression with type <c>int</c>. </returns>
        public static Expression ToInt32(Expression expression)
        {
            if (expression.Type == typeof(int))
                return expression;
            return Expression.Call(ReflectionHelpers.TypeConverter_ToInt32, Expression.Convert(expression, typeof(object)));
        }

        /// <summary>
        /// Converts the given expression to an unsigned 32-bit integer using JavaScript type conversion rules.
        /// </summary>
        /// <param name="expression"> The expression to convert. </param>
        /// <returns> An expression with type <c>uint</c>. </returns>
        public static Expression ToUint32(Expression expression)
        {
            if (expression.Type == typeof(uint))
                return expression;
            return Expression.Call(ReflectionHelpers.TypeConverter_ToUint32, Expression.Convert(expression, typeof(object)));
        }



        //     OTHER
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates an expression representing "undefined".
        /// </summary>
        /// <returns> An expression representing "undefined". </returns>
        public static Expression Undefined()
        {
            return Expression.Field(null, ReflectionHelpers.Undefined_Value);
        }

        /// <summary>
        /// Creates an expression representing "null".
        /// </summary>
        /// <returns> An expression representing "null". </returns>
        public static Expression Null()
        {
            return Expression.Field(null, ReflectionHelpers.Null_Value);
        }

        /// <summary>
        /// Creates an expression that throws an error at runtime.
        /// </summary>
        /// <param name="type"> The type of error to throw, TypeError, SyntaxError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        /// <returns> An expression that throws an error at runtime. </returns>
        public static Expression Throw(string type, string message)
        {
            return Expression.Throw(Expression.New(ReflectionHelpers.JavaScriptException_Constructor2, Expression.Constant(type), Expression.Constant(message)));
        }

        /// <summary>
        /// Creates a block expression that does not throw an error if no statements are provided.
        /// </summary>
        /// <param name="statements"> The statements to include in the block. </param>
        /// <returns> A block expression, or Expression.Empty() if there are no statements. </returns>
        public static Expression Block(IEnumerable<Expression> statements)
        {
            if (statements.GetEnumerator().MoveNext() == false)
                return Expression.Empty();
            return Expression.Block(statements);
        }
    }

}