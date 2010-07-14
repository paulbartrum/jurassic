using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a unary operator expression.
    /// </summary>
    internal sealed class UnaryExpression : OperatorExpression
    {
        /// <summary>
        /// Creates a new instance of UnaryExpression.
        /// </summary>
        /// <param name="operator"> The unary operator to base this expression on. </param>
        public UnaryExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Gets the expression on the left or right side of the unary operator.
        /// </summary>
        public Expression Operand
        {
            get { return this.GetOperand(0); }
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get
            {
                switch (this.OperatorType)
                {
                    case OperatorType.Plus:
                    case OperatorType.Minus:
                        return PrimitiveType.Number;

                    case OperatorType.BitwiseNot:
                        return PrimitiveType.Int32;

                    case OperatorType.LogicalNot:
                        return PrimitiveType.Bool;

                    case OperatorType.Void:
                        return PrimitiveType.Undefined;

                    case OperatorType.Typeof:
                        return PrimitiveType.String;

                    case OperatorType.Delete:
                        return PrimitiveType.Bool;

                    default:
                        throw new NotImplementedException(string.Format("Unsupported operator {0}", this.OperatorType));
                }
            }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Special-case the delete operator.
            if (this.OperatorType == OperatorType.Delete)
            {
                GenerateDelete(generator, optimizationInfo);
                return;
            }

            // If a return value is not expected, generate only the side-effects.
            if (optimizationInfo.SuppressReturnValue == true)
            {
                this.GenerateSideEffects(generator, optimizationInfo);
                return;
            }

            // Special-case the typeof operator.
            if (this.OperatorType == OperatorType.Typeof)
            {
                GenerateTypeof(generator, optimizationInfo);
                return;
            }

            // Load the operand onto the stack.
            this.Operand.GenerateCode(generator, optimizationInfo);

            // Convert the operand to the correct type.
            switch (this.OperatorType)
            {
                case OperatorType.Plus:
                case OperatorType.Minus:
                    EmitConversion.ToNumber(generator, this.Operand.ResultType);
                    break;

                case OperatorType.BitwiseNot:
                    EmitConversion.ToInt32(generator, this.Operand.ResultType);
                    break;

                case OperatorType.LogicalNot:
                    EmitConversion.ToBool(generator, this.Operand.ResultType);
                    break;
            }

            // Apply the operator.
            switch (this.OperatorType)
            {
                case OperatorType.Plus:
                    break;

                case OperatorType.Minus:
                    generator.Negate();
                    break;

                case OperatorType.BitwiseNot:
                    generator.BitwiseNot();
                    break;

                case OperatorType.LogicalNot:
                    generator.LoadBoolean(false);
                    generator.Equal();
                    break;

                case OperatorType.Void:
                    generator.Pop();
                    EmitHelpers.EmitUndefined(generator);
                    break;

                default:
                    throw new NotImplementedException(string.Format("Unsupported operator {0}", this.OperatorType));
            }
        }

        /// <summary>
        /// Generates CIL for the typeof expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        private void GenerateTypeof(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // If a return value is not expected, generate only the side-effects.
            if (optimizationInfo.SuppressReturnValue == true)
            {
                this.GenerateSideEffects(generator, optimizationInfo);
                return;
            }

            if (this.Operand is NameExpression)
            {
                // Unresolvable references must return "undefined" rather than throw an error.
                ((NameExpression)this.Operand).GenerateGet(generator, optimizationInfo, false);
            }
            else
            {
                // Emit code for resolving the value of the operand.
                this.Operand.GenerateCode(generator, optimizationInfo);
            }

            // Convert to System.Object.
            EmitConversion.ToAny(generator, this.Operand.ResultType);

            // Call TypeUtilities.TypeOf(operand).
            generator.Call(ReflectionHelpers.TypeUtilities_TypeOf);
        }

        /// <summary>
        /// Generates CIL for the delete expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        private void GenerateDelete(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Attempting to delete something that isn't a reference returns true but otherwise does nothing.
            if ((this.Operand is IReferenceExpression) == false)
            {
                if (optimizationInfo.SuppressReturnValue == false)
                    generator.LoadBoolean(true);
                return;
            }
            ((IReferenceExpression)this.Operand).GenerateDelete(generator, optimizationInfo);
        }
    }

}