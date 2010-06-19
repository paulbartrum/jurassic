using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a mutable operator expression.  Once all the operands are determined, the
    /// expression is converted into a real operator expression.
    /// </summary>
    internal abstract class OperatorJSExpression : JSExpression
    {
        private JSExpression[] operands;

        /// <summary>
        /// Creates a new instance of OperatorJSExpression.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        public OperatorJSExpression(Operator @operator)
        {
            if (@operator == null)
                throw new ArgumentNullException("operator");
            this.Operator = @operator;
            this.operands = new JSExpression[@operator.Arity];
        }

        /// <summary>
        /// Creates a derived instance of OperatorJSExpression from the given operator.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        /// <returns> A derived OperatorJSExpression instance. </returns>
        public static OperatorJSExpression FromOperator(Operator @operator)
        {
            if (@operator == null)
                throw new ArgumentNullException("operator");
            switch (@operator.Type)
            {
                case OperatorType.Grouping:
                    return new GroupingJSExpression(@operator);

                case OperatorType.FunctionCall:
                    return new FunctionCallJSExpression(@operator);

                case OperatorType.MemberAccess:
                case OperatorType.Index:
                    return new MemberJSExpression(@operator);

                case OperatorType.New:
                    return new NewJSExpression(@operator);

                case OperatorType.PostIncrement:
                case OperatorType.PostDecrement:
                case OperatorType.PreIncrement:
                case OperatorType.PreDecrement:
                case OperatorType.Assignment:
                case OperatorType.CompoundAdd:
                case OperatorType.CompoundBitwiseAnd:
                case OperatorType.CompoundBitwiseOr:
                case OperatorType.CompoundBitwiseXor:
                case OperatorType.CompoundDivide:
                case OperatorType.CompoundLeftShift:
                case OperatorType.CompoundModulo:
                case OperatorType.CompoundMultiply:
                case OperatorType.CompoundSignedRightShift:
                case OperatorType.CompoundSubtract:
                case OperatorType.CompoundUnsignedRightShift:
                    return new AssignmentJSExpression(@operator);

                case OperatorType.Conditional:
                    return new TernaryJSExpression(@operator);

                case OperatorType.Comma:
                    return new ListJSExpression(@operator);
            }
            if (@operator.Arity == 1)
                return new UnaryJSExpression(@operator);
            if (@operator.Arity == 2)
                return new BinaryJSExpression(@operator);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the operator this expression refers to.
        /// </summary>
        public Operator Operator
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the operand with the given index.  No parameter validation and grouping operator
        /// elimination is performed.
        /// </summary>
        /// <param name="index"> The index of the operand to retrieve. </param>
        /// <returns> The operand with the given index. </returns>
        protected JSExpression GetRawOperand(int index)
        {
            return this.operands[index];
        }

        /// <summary>
        /// Gets the operand with the given index.
        /// </summary>
        /// <param name="index"> The index of the operand to retrieve. </param>
        /// <returns> The operand with the given index. </returns>
        public JSExpression GetOperand(int index)
        {
            if (index < 0 || index >= this.OperandCount)
                throw new ArgumentOutOfRangeException("index");
            JSExpression result = this.operands[index];
            if (result is GroupingJSExpression)
                return ((GroupingJSExpression)result).Operand;
            return result;
        }

        /// <summary>
        /// Gets the number of operands that have been added.
        /// </summary>
        public int OperandCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds an operand.
        /// </summary>
        /// <param name="operand"> The expression representing the operand to add. </param>
        public void Push(JSExpression operand)
        {
            if (this.OperandCount >= this.operands.Length)
                throw new InvalidOperationException("Too many operands.");
            this.operands[this.OperandCount] = operand;
            this.OperandCount ++;
        }

        /// <summary>
        /// Removes and returns the most recently added operand.
        /// </summary>
        /// <returns> The most recently added operand. </returns>
        public JSExpression Pop()
        {
            if (this.OperandCount == 0)
                throw new InvalidOperationException("Not enough operands.");
            this.OperandCount --;
            return this.operands[this.OperandCount];
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the second token in a ternary operator
        /// was encountered.
        /// </summary>
        public bool SecondTokenEncountered
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value that indicates whether a new operand is acceptable given the state of
        /// this operator.  For ternary operators only two operands are acceptable until the
        /// second token of the sequence is encountered.
        /// </summary>
        public bool AcceptingOperands
        {
            get
            {
                if (this.SecondTokenEncountered == true && this.Operator.HasSecondaryRHSOperand == false)
                    return false;
                return this.OperandCount <
                    (this.Operator.HasLHSOperand ? 1 : 0) + (this.Operator.HasRHSOperand ? 1 : 0) +
                    (this.Operator.HasSecondaryRHSOperand && this.SecondTokenEncountered ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets the right-most operand as an unbound operator, or <c>null</c> if the operator
        /// has no operands or the right-most operand is not an operator.
        /// </summary>
        public OperatorJSExpression RightBranch
        {
            get { return this.OperandCount == 0 ? null : this.operands[this.OperandCount - 1] as OperatorJSExpression; }
        }

        /// <summary>
        /// Gets the precedence of the operator.  For ternary operators this is -MinValue if
        /// parsing is currently between the two tokens.
        /// </summary>
        public virtual int Precedence
        {
            get { return this.SecondTokenEncountered == false ? this.Operator.SecondaryPrecedence : this.Operator.TertiaryPrecedence; }
        }

        /// <summary>
        /// Gets a value that indicates whether this 
        /// </summary>
        public void CheckValid()
        {
            if (this.Operator.IsValidNumberOfOperands(this.OperandCount) == false)
                throw new JavaScriptException("SyntaxError", "Wrong number of operands", 1, "");
            if (this.Operator.SecondaryToken != null && this.SecondTokenEncountered == false)
                throw new JavaScriptException("SyntaxError", string.Format("Missing closing token '{0}'", this.Operator.SecondaryToken.Text), 1, "");
        }
    }

}