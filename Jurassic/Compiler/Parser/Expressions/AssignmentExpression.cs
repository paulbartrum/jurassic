using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents an assignment expression (++, --, =, +=, -=, *=, /=, %=, &=, |=, ^=, &lt;&lt;=, &gt;&gt;=, &gt;&gt;&gt;=).
    /// </summary>
    internal class AssignmentExpression : OperatorExpression
    {
        /// <summary>
        /// Creates a new instance of AssignmentExpression.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        public AssignmentExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Creates a simple variable assignment expression.
        /// </summary>
        /// <param name="scope"> The scope the variable is defined within. </param>
        /// <param name="name"> The name of the variable to set. </param>
        /// <param name="value"> The value to set the variable to. </param>
        public AssignmentExpression(Scope scope, string name, Expression value)
            : base(Operator.Assignment)
        {
            this.Push(new NameExpression(scope, name));
            this.Push(value);
        }

        /// <summary>
        /// Gets the target of the assignment.
        /// </summary>
        public IReferenceExpression Target
        {
            get { return this.GetOperand(0) as IReferenceExpression; }
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

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get
            {
                var type = this.OperatorType;
                if (type == OperatorType.PostIncrement ||
                    type == OperatorType.PostDecrement ||
                    type == OperatorType.PreIncrement ||
                    type == OperatorType.PreDecrement)
                    return PrimitiveType.Number;
                if (type == OperatorType.Assignment)
                    return this.GetOperand(1).ResultType;
                var compoundOperator = new BinaryExpression(GetCompoundBaseOperator(type), this.GetOperand(0), this.GetOperand(1));
                return compoundOperator.ResultType;
            }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // The left hand side needs to be a variable reference or member access.
            var target = this.GetOperand(0) as IReferenceExpression;
            if (target == null)
            {
                // Emit an error message.
                switch (this.OperatorType)
                {
                    case OperatorType.PostIncrement:
                    case OperatorType.PostDecrement:
                        EmitHelpers.EmitThrow(generator, "ReferenceError", "Invalid left-hand side in postfix operation");
                        break;
                    case OperatorType.PreIncrement:
                    case OperatorType.PreDecrement:
                        EmitHelpers.EmitThrow(generator, "ReferenceError", "Invalid left-hand side in prefix operation");
                        break;
                    case OperatorType.Assignment:
                    default:
                        EmitHelpers.EmitThrow(generator, "ReferenceError", "Invalid left-hand side in assignment");
                        break;
                }
                if (optimizationInfo.SuppressReturnValue == false)
                    EmitHelpers.EmitDummyValue(generator, this.ResultType);
                return;
            }

            switch (this.OperatorType)
            {
                case OperatorType.Assignment:

                    // Load the value to assign.
                    var rhs = this.GetOperand(1);
                    rhs.GenerateCode(generator, optimizationInfo.RemoveFlags(OptimizationFlags.SuppressReturnValue));
                    
                    // Duplicate the value so it remains on the stack afterwards.
                    if (optimizationInfo.SuppressReturnValue == false)
                        generator.Duplicate();

                    // Store the value.
                    target.GenerateSet(generator, optimizationInfo, rhs.ResultType, optimizationInfo.StrictMode);
                    break;

                case OperatorType.PostIncrement:
                    GenerateIncrementOrDecrement(generator, optimizationInfo, target, true, 1);
                    break;
                case OperatorType.PostDecrement:
                    GenerateIncrementOrDecrement(generator, optimizationInfo, target, true, -1);
                    break;
                case OperatorType.PreIncrement:
                    GenerateIncrementOrDecrement(generator, optimizationInfo, target, false, 1);
                    break;
                case OperatorType.PreDecrement:
                    GenerateIncrementOrDecrement(generator, optimizationInfo, target, false, -1);
                    break;

                default:
                    // Load the value to assign.
                    var compoundOperator = new BinaryExpression(GetCompoundBaseOperator(this.OperatorType), this.GetOperand(0), this.GetOperand(1));
                    compoundOperator.GenerateCode(generator, optimizationInfo.RemoveFlags(OptimizationFlags.SuppressReturnValue));

                    // Duplicate the value so it remains on the stack afterwards.
                    if (optimizationInfo.SuppressReturnValue == false)
                        generator.Duplicate();

                    // Store the value.
                    target.GenerateSet(generator, optimizationInfo, compoundOperator.ResultType, optimizationInfo.StrictMode);
                    break;
            }
        }

        /// <summary>
        /// Generates CIL for an increment or decrement expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <param name="target"> The target to modify. </param>
        private void GenerateIncrementOrDecrement(ILGenerator generator, OptimizationInfo optimizationInfo, IReferenceExpression target, bool returnOriginalValue, int increment)
        {
            // Note: increment and decrement can produce a number that is out of range if the
            // target is of type Int32.  The only time this should happen is for a loop variable
            // where the range has been carefully checked to make sure an out of range condition
            // cannot happen.

            // Get the target value.
            target.GenerateGet(generator, optimizationInfo.RemoveFlags(OptimizationFlags.SuppressReturnValue), true);

            // Convert it to a number.
            if (target.Type != PrimitiveType.Int32)
                EmitConversion.ToNumber(generator, target.Type);

            // If this is PostIncrement or PostDecrement, duplicate the value so it can be produced as the return value.
            if (returnOriginalValue == true && optimizationInfo.SuppressReturnValue == false)
                generator.Duplicate();

            // Load the increment constant.
            if (target.Type == PrimitiveType.Int32)
                generator.LoadInt32(increment);
            else
                generator.LoadDouble((double)increment);

            // Add the constant to the target value.
            generator.Add();

            // If this is PreIncrement or PreDecrement, duplicate the value so it can be produced as the return value.
            if (returnOriginalValue == false && optimizationInfo.SuppressReturnValue == false)
                generator.Duplicate();

            // Store the value.
            target.GenerateSet(generator, optimizationInfo, target.Type == PrimitiveType.Int32 ? PrimitiveType.Int32 : PrimitiveType.Number, optimizationInfo.StrictMode);
        }
    }

}