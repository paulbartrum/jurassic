using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    
    /// <summary>
    /// Represents a variable or member access.
    /// </summary>
    internal sealed class MemberJSExpression : OperatorJSExpression
    {
        /// <summary>
        /// Creates a new instance of MemberJSExpression.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        public MemberJSExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Creates a new instance of MemberJSExpression.
        /// </summary>
        /// <param name="name"> The name of the variable or member. </param>
        public MemberJSExpression(string name)
            : base(Operator.MemberAccess)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            this.Push(null);
            this.Push(new LiteralJSExpression(name));
        }

        /// <summary>
        /// Gets an expression that evaluates to the object that is being accessed or modified.  Can
        /// be <c>null</c>.
        /// </summary>
        public JSExpression Base
        {
            get { return this.GetOperand(0); }
        }

        /// <summary>
        /// Gets a bound expression that evaluates to the object that is being accessed or modified.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A bound expression that evaluates to the object that is being accessed or
        /// modified.  Can be of type ObjectInstance or LexicalScope. </returns>
        public Expression GetBaseExpression(ParameterExpression scope)
        {
            if (this.Base == null)
                return scope;
            return ExpressionTreeHelpers.ToObject(this.Base.ToExpression(scope));
        }

        /// <summary>
        /// Gets an expression that resolves to the name of the variable or member.
        /// </summary>
        public JSExpression Name
        {
            get
            {
                if (this.Base == null || this.Operator.Type == OperatorType.Index)
                {
                    return this.GetOperand(1);
                }
                else
                {
                    var name = this.GetOperand(1) as MemberJSExpression;
                    if (name == null)
                        throw new JavaScriptException("SyntaxError", "Expected identifier", 1, "");
                    if (name.Base != null)
                        throw new InvalidOperationException("Base should be null.");
                    return name.Name;
                }
            }
        }

        /// <summary>
        /// Gets a bound expression that evaluates to the name of the variable or member.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A bound expression that evaluates to the name of the variable or member. </returns>
        public Expression GetNameExpression(ParameterExpression scope)
        {
            return ExpressionTreeHelpers.ToString(this.Name.ToExpression(scope));
        }

        /// <summary>
        /// Converts the JSExpression into a regular .NET expression.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A regular .NET expression. </returns>
        public override Expression ToExpression(ParameterExpression scope)
        {
            CheckValid();

            // ((IEnvironmentRecord)ToObject(target-base)).GetBindingValue(ToString(target-name))
            return Expression.Call(
                Expression.Convert(this.GetBaseExpression(scope), typeof(IEnvironmentRecord)),
                ReflectionHelpers.IEnvironmentRecord_GetBindingValue,
                this.GetNameExpression(scope),
                Expression.Constant(false));
        }

        /// <summary>
        /// Gets a bound expression that evaluates to code that sets the reference value.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <param name="setterValue"> An expression that evaluates to the value to set. </param>
        /// <returns> A bound expression that evaluates to <c>true</c> if the reference exists or
        /// <c>false</c> if the reference does not exist. </returns>
        public Expression GetSetterExpression(ParameterExpression scope, Expression setterValue)
        {
            // target-base.SetMutableBinding(target-name, setterValue)
            return Expression.Call(
                Expression.Convert(this.GetBaseExpression(scope), typeof(IEnvironmentRecord)),
                ReflectionHelpers.IEnvironmentRecord_SetMutableBinding.MakeGenericMethod(new Type[] { setterValue.Type }),
                this.GetNameExpression(scope),
                setterValue,
                Expression.Constant(false));
        }

        /// <summary>
        /// Gets a bound expression that evaluates to <c>true</c> if the reference exists or
        /// <c>false</c> if the reference does not exist.
        /// </summary>
        /// <param name="scope"> The lexical scope - used for variable binding. </param>
        /// <returns> A bound expression that evaluates to <c>true</c> if the reference exists or
        /// <c>false</c> if the reference does not exist. </returns>
        public Expression GetReferenceExistsExpression(ParameterExpression scope)
        {
            // ToObject(target-base).HasBinding(ToString(target-name))
            return Expression.Call(
                Expression.Convert(this.GetBaseExpression(scope), typeof(IEnvironmentRecord)),
                ReflectionHelpers.IEnvironmentRecord_HasBinding,
                this.GetNameExpression(scope));
        }
    }
}