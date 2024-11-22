using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a generator that checks commands for correctness.
    /// </summary>
    internal class VerifyingILGenerator : ILGenerator
    {
        private ILGenerator generator;
        private Stack<Type> stack = new Stack<Type>();
        private bool stackIsIndeterminate = false;
        private List<ILLabel> labels = new List<ILLabel>();
        private ParameterInfo[] argumentTypes;

        /// <summary>
        /// Creates a new LoggingILGenerator instance.
        /// </summary>
        /// <param name="generator"> The ILGenerator that is used to output the IL. </param>
        public VerifyingILGenerator(ILGenerator generator)
        {
            this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
            this.argumentTypes = generator.MethodInfo.GetParameters();
        }

        /// <summary>
        /// Gets a reference to the method that we are generating IL for.
        /// </summary>
        public override MethodInfo MethodInfo
        {
            get { return this.generator.MethodInfo; }
        }


        //     LIFECYCLE MANAGEMENT
        //_________________________________________________________________________________________

        /// <summary>
        /// Emits a return statement and finalizes the generated code.  Do not emit any more
        /// instructions after calling this method.
        /// </summary>
        public override void Complete()
        {
            if (MethodInfo.ReturnType != typeof(void))
                Expect(MethodInfo.ReturnType);
            if (this.stack.Count > 0)
                throw new InvalidOperationException($"Complete() called but {this.stack.Count} value(s) still on stack.");

            this.generator.Complete();
        }



        //     STACK MANAGEMENT
        //_________________________________________________________________________________________

        /// <summary>
        /// Pops the value from the top of the stack.
        /// </summary>
        public override void Pop()
        {
            this.stack.Pop();
            this.generator.Pop();
        }

        /// <summary>
        /// Duplicates the value on the top of the stack.
        /// </summary>
        public override void Duplicate()
        {
            var type = this.stack.Pop();
            Push(type);
            Push(type);
            this.generator.Duplicate();
        }



        //     BRANCHING AND LABELS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a label without setting its position.
        /// </summary>
        /// <returns> A new label. </returns>
        public override ILLabel CreateLabel()
        {
            var label = new VerifyingILLabel(this.generator.CreateLabel());
            labels.Add(label);
            return label;
        }

        /// <summary>
        /// Defines the position of the given label.
        /// </summary>
        /// <param name="label"> The label to define. </param>
        public override void DefineLabelPosition(ILLabel label)
        {
            var label2 = (VerifyingILLabel)label;
            if (label2.Marked)
                throw new InvalidOperationException("DefineLabelPosition() has already been called for this label.");
            if (label2.Stack != null)
            {
                if (!this.stackIsIndeterminate && !Equals(this.stack, label2.Stack))
                    throw new InvalidOperationException($"Inconsistent stacks; label has: {ToString(label2.Stack)}, ILGenerator has: {ToString(this.stack)}");
                this.stack = Clone(label2.Stack);
                this.stackIsIndeterminate = false;
            }
            this.generator.DefineLabelPosition(label2.UnderlyingLabel);
        }

        /// <summary>
        /// Unconditionally branches to the given label.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void Branch(ILLabel label)
        {
            ValidateBranch((VerifyingILLabel)label);
            this.stackIsIndeterminate = true;

            this.generator.Branch(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the value on the top of the stack is zero.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfZero(ILLabel label)
        {
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfZero(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the value on the top of the stack is non-zero, true or
        /// non-null.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfNotZero(ILLabel label)
        {
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfNotZero(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the two values on the top of the stack are equal.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfEqual(ILLabel label)
        {
            // The top two types on the stack should be the same.
            var type1 = this.stack.Pop();
            Expect(t => t == type1, type1.ToString());
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfEqual(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the two values on the top of the stack are not equal.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfNotEqual(ILLabel label)
        {
            // The top two types on the stack should be the same.
            var type1 = this.stack.Pop();
            Expect(t => t == type1, type1.ToString());
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfNotEqual(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the first value on the stack is greater than the second
        /// value on the stack.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfGreaterThan(ILLabel label)
        {
            ExpectNumber();
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfGreaterThan(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the first value on the stack is greater than the second
        /// value on the stack.  If the operands are integers then they are treated as if they are
        /// unsigned.  If the operands are floating point numbers then a NaN value will trigger a
        /// branch.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfGreaterThanUnsigned(ILLabel label)
        {
            ExpectNumber();
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfGreaterThanUnsigned(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the first value on the stack is greater than or equal to
        /// the second value on the stack.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfGreaterThanOrEqual(ILLabel label)
        {
            ExpectNumber();
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfGreaterThanOrEqual(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the first value on the stack is greater than or equal to
        /// the second value on the stack.  If the operands are integers then they are treated as
        /// if they are unsigned.  If the operands are floating point numbers then a NaN value will
        /// trigger a branch.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfGreaterThanOrEqualUnsigned(ILLabel label)
        {
            ExpectNumber();
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfGreaterThanOrEqualUnsigned(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the first value on the stack is less than the second
        /// value on the stack.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfLessThan(ILLabel label)
        {
            ExpectNumber();
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfLessThan(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the first value on the stack is less than the second
        /// value on the stack.  If the operands are integers then they are treated as if they are
        /// unsigned.  If the operands are floating point numbers then a NaN value will trigger a
        /// branch.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfLessThanUnsigned(ILLabel label)
        {
            ExpectNumber();
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfLessThanUnsigned(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the first value on the stack is less than or equal to
        /// the second value on the stack.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfLessThanOrEqual(ILLabel label)
        {
            ExpectNumber();
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfLessThanOrEqual(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the first value on the stack is less than or equal to
        /// the second value on the stack.  If the operands are integers then they are treated as
        /// if they are unsigned.  If the operands are floating point numbers then a NaN value will
        /// trigger a branch.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfLessThanOrEqualUnsigned(ILLabel label)
        {
            ExpectNumber();
            ExpectNumber();
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfLessThanOrEqualUnsigned(((VerifyingILLabel)label).UnderlyingLabel);
        }


        /// <summary>
        /// Branches to the given label if the value on the top of the stack is zero, false or
        /// null.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfFalse(ILLabel label)
        {
            Expect(typeof(bool));
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfFalse(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the value on the top of the stack is non-zero, true or
        /// non-null.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfTrue(ILLabel label)
        {
            Expect(typeof(bool));
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfTrue(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the value on the top of the stack is zero, false or
        /// null.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfNull(ILLabel label)
        {
            Expect(t => !t.IsValueType, "reference");
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfNull(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Branches to the given label if the value on the top of the stack is non-zero, true or
        /// non-null.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void BranchIfNotNull(ILLabel label)
        {
            Expect(t => !t.IsValueType, "reference");
            ValidateBranch((VerifyingILLabel)label);

            this.generator.BranchIfNotNull(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// Returns from the current method.  A value is popped from the stack and used as the
        /// return value.
        /// </summary>
        public override void Return()
        {
            if (MethodInfo.ReturnType != typeof(void))
                Expect(MethodInfo.ReturnType);
            this.generator.Return();
            this.stackIsIndeterminate = true;
        }

        /// <summary>
        /// Creates a jump table.  A value is popped from the stack - this value indicates the
        /// index of the label in the <paramref name="labels"/> array to jump to.
        /// </summary>
        /// <param name="labels"> A array of labels. </param>
        public override void Switch(ILLabel[] labels)
        {
            ExpectInt();

            var underlyingLabels = new ILLabel[labels.Length];
            for (int i = 0; i < labels.Length; i++)
                underlyingLabels[i] = ((VerifyingILLabel)labels[i]).UnderlyingLabel;
            this.generator.Switch(underlyingLabels);
        }



        //     LOCAL VARIABLES AND ARGUMENTS
        //_________________________________________________________________________________________

        /// <summary>
        /// Declares a new local variable.
        /// </summary>
        /// <param name="type"> The type of the local variable. </param>
        /// <param name="name"> The name of the local variable. Can be <c>null</c>. </param>
        /// <returns> A new local variable. </returns>
        public override ILLocalVariable DeclareVariable(Type type, string name = null)
        {
            return this.generator.DeclareVariable(type, name);
        }

        /// <summary>
        /// Pushes the value of the given variable onto the stack.
        /// </summary>
        /// <param name="variable"> The variable whose value will be pushed. </param>
        public override void LoadVariable(ILLocalVariable variable)
        {
            Push(variable.Type);
            this.generator.LoadVariable(variable);
        }

        /// <summary>
        /// Pushes the address of the given variable onto the stack.
        /// </summary>
        /// <param name="variable"> The variable whose address will be pushed. </param>
        public override void LoadAddressOfVariable(ILLocalVariable variable)
        {
            Push(typeof(IntPtr));
            this.generator.LoadAddressOfVariable(variable);
        }

        /// <summary>
        /// Pops the value from the top of the stack and stores it in the given local variable.
        /// </summary>
        /// <param name="variable"> The variable to store the value. </param>
        public override void StoreVariable(ILLocalVariable variable)
        {
            ExpectSubclass(variable.Type);
            this.generator.StoreVariable(variable);
        }

        /// <summary>
        /// Pushes the value of the method argument with the given index onto the stack.
        /// </summary>
        /// <param name="argumentIndex"> The index of the argument to push onto the stack. </param>
        public override void LoadArgument(int argumentIndex)
        {
            if (this.argumentTypes == null)
                throw new InvalidOperationException("Call SetArgumentTypes() first.");
            if (argumentIndex < 0 || argumentIndex >= this.argumentTypes.Length)
                throw new ArgumentOutOfRangeException("argumentIndex");
            Push(this.argumentTypes[argumentIndex].ParameterType);

            this.generator.LoadArgument(argumentIndex);
        }

        /// <summary>
        /// Pops a value from the stack and stores it in the method argument with the given index.
        /// </summary>
        /// <param name="argumentIndex"> The index of the argument to store into. </param>
        public override void StoreArgument(int argumentIndex)
        {
            if (argumentIndex < 0 || argumentIndex >= this.argumentTypes.Length)
                throw new ArgumentOutOfRangeException("argumentIndex");
            Expect(this.argumentTypes[argumentIndex].ParameterType);

            this.generator.StoreArgument(argumentIndex);
        }



        //     LOAD CONSTANT
        //_________________________________________________________________________________________

        /// <summary>
        /// Pushes <c>null</c> onto the stack.
        /// </summary>
        public override void LoadNull()
        {
            Push(typeof(DBNull));
            this.generator.LoadNull();
        }

        /// <summary>
        /// Pushes a constant value onto the stack.
        /// </summary>
        /// <param name="value"> The boolean to push onto the stack. </param>
        public override void LoadBoolean(bool value)
        {
            Push(typeof(bool));
            this.generator.LoadBoolean(value);
        }

        /// <summary>
        /// Pushes a constant value onto the stack.
        /// </summary>
        /// <param name="value"> The integer to push onto the stack. </param>
        public override void LoadInt32(int value)
        {
            Push(typeof(int));
            this.generator.LoadInt32(value);
        }

        /// <summary>
        /// Pushes a 64-bit constant value onto the stack.
        /// </summary>
        /// <param name="value"> The 64-bit integer to push onto the stack. </param>
        public override void LoadInt64(long value)
        {
            Push(typeof(long));
            this.generator.LoadInt64(value);
        }

        /// <summary>
        /// Pushes a constant value onto the stack.
        /// </summary>
        /// <param name="value"> The number to push onto the stack. </param>
        public override void LoadDouble(double value)
        {
            Push(typeof(double));
            this.generator.LoadDouble(value);
        }

        /// <summary>
        /// Pushes a constant value onto the stack.
        /// </summary>
        /// <param name="value"> The string to push onto the stack. </param>
        public override void LoadString(string value)
        {
            Push(typeof(string));
            this.generator.LoadString(value);
        }

        /// <summary>
        /// Pushes a constant value onto the stack.
        /// </summary>
        /// <param name="value"> The enum value to push onto the stack. </param>
        public override void LoadEnumValue<T>(T value)
        {
            this.generator.LoadEnumValue(value);
            Push(typeof(T));
        }



        //     RELATIONAL OPERATIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
        /// is equal to the second, or <c>0</c> otherwise.  Produces <c>0</c> if one or both
        /// of the arguments are <c>NaN</c>.
        /// </summary>
        public override void CompareEqual()
        {
            Expect(t => t == typeof(int) || t == typeof(uint) ||
                t == typeof(long) || t == typeof(ulong) ||
                t == typeof(double) || t == typeof(bool), "number or boolean");
            Expect(t => t == typeof(int) || t == typeof(uint) ||
                t == typeof(long) || t == typeof(ulong) ||
                t == typeof(double) || t == typeof(bool), "number or boolean");
            Push(typeof(bool));
            this.generator.CompareEqual();
        }

        /// <summary>
        /// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
        /// is greater than the second, or <c>0</c> otherwise.  Produces <c>0</c> if one or both
        /// of the arguments are <c>NaN</c>.
        /// </summary>
        public override void CompareGreaterThan()
        {
            ExpectNumber();
            ExpectNumber();
            Push(typeof(bool));
            this.generator.CompareGreaterThan();
        }

        /// <summary>
        /// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
        /// is greater than the second, or <c>0</c> otherwise.  Produces <c>1</c> if one or both
        /// of the arguments are <c>NaN</c>.  Integers are considered to be unsigned.
        /// </summary>
        public override void CompareGreaterThanUnsigned()
        {
            ExpectNumber();
            ExpectNumber();
            Push(typeof(bool));
            this.generator.CompareGreaterThanUnsigned();
        }

        /// <summary>
        /// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
        /// is less than the second, or <c>0</c> otherwise.  Produces <c>0</c> if one or both
        /// of the arguments are <c>NaN</c>.
        /// </summary>
        public override void CompareLessThan()
        {
            ExpectNumber();
            ExpectNumber();
            Push(typeof(bool));
            this.generator.CompareLessThan();
        }

        /// <summary>
        /// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
        /// is less than the second, or <c>0</c> otherwise.  Produces <c>1</c> if one or both
        /// of the arguments are <c>NaN</c>.  Integers are considered to be unsigned.
        /// </summary>
        public override void CompareLessThanUnsigned()
        {
            ExpectNumber();
            ExpectNumber();
            Push(typeof(bool));
            this.generator.CompareLessThanUnsigned();
        }



        //     ARITHMETIC AND BITWISE OPERATIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Pops two values from the stack, adds them together, then pushes the result to the
        /// stack.
        /// </summary>
        public override void Add()
        {
            ValidateBinaryOperands();
            this.generator.Add();
        }

        /// <summary>
        /// Pops two values from the stack, subtracts the second from the first, then pushes the
        /// result to the stack.
        /// </summary>
        public override void Subtract()
        {
            ValidateBinaryOperands();
            this.generator.Subtract();
        }

        /// <summary>
        /// Pops two values from the stack, multiplies them together, then pushes the
        /// result to the stack.
        /// </summary>
        public override void Multiply()
        {
            ValidateBinaryOperands();
            this.generator.Multiply();
        }

        /// <summary>
        /// Pops two values from the stack, divides the first by the second, then pushes the
        /// result to the stack.
        /// </summary>
        public override void Divide()
        {
            ValidateBinaryOperands();
            this.generator.Divide();
        }

        /// <summary>
        /// Pops two values from the stack, divides the first by the second, then pushes the
        /// remainder to the stack.
        /// </summary>
        public override void Remainder()
        {
            ValidateBinaryOperands();
            this.generator.Remainder();
        }

        /// <summary>
        /// Pops a value from the stack, negates it, then pushes it back onto the stack.
        /// </summary>
        public override void Negate()
        {
            var type = this.stack.Peek();
            ExpectNumber();
            Push(type);

            this.generator.Negate();
        }

        /// <summary>
        /// Pops two values from the stack, ANDs them together, then pushes the result to the
        /// stack.
        /// </summary>
        public override void BitwiseAnd()
        {
            ValidateBinaryOperands(integerOnly: true);
            this.generator.BitwiseAnd();
        }

        /// <summary>
        /// Pops two values from the stack, ORs them together, then pushes the result to the
        /// stack.
        /// </summary>
        public override void BitwiseOr()
        {
            ValidateBinaryOperands(integerOnly: true);
            this.generator.BitwiseOr();
        }

        /// <summary>
        /// Pops two values from the stack, XORs them together, then pushes the result to the
        /// stack.
        /// </summary>
        public override void BitwiseXor()
        {
            ValidateBinaryOperands(integerOnly: true);
            this.generator.BitwiseXor();
        }

        /// <summary>
        /// Pops a value from the stack, inverts it, then pushes the result to the stack.
        /// </summary>
        public override void BitwiseNot()
        {
            ExpectInt();
            Push(typeof(int));

            this.generator.BitwiseNot();
        }

        /// <summary>
        /// Pops two values from the stack, shifts the first to the left, then pushes the result
        /// to the stack.
        /// </summary>
        public override void ShiftLeft()
        {
            ValidateBinaryOperands(integerOnly: true);
            this.generator.ShiftLeft();
        }

        /// <summary>
        /// Pops two values from the stack, shifts the first to the right, then pushes the result
        /// to the stack.  The sign bit is preserved.
        /// </summary>
        public override void ShiftRight()
        {
            ValidateBinaryOperands(integerOnly: true);
            this.generator.ShiftRight();
        }

        /// <summary>
        /// Pops two values from the stack, shifts the first to the right, then pushes the result
        /// to the stack.  The sign bit is not preserved.
        /// </summary>
        public override void ShiftRightUnsigned()
        {
            ValidateBinaryOperands(integerOnly: true);
            this.generator.ShiftRightUnsigned();
        }



        //     CONVERSIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Pops a value from the stack, converts it to an object reference, then pushes it back onto
        /// the stack.
        /// </summary>
        public override void Box(Type type)
        {
            Expect(t => t.IsValueType, "value type");
            Push(typeof(object));

            this.generator.Box(type);
        }

        /// <summary>
        /// Pops an object reference (representing a boxed value) from the stack, extracts the
        /// address, then pushes that address onto the stack.
        /// </summary>
        /// <param name="type"> The type of the boxed value.  This should be a value type. </param>
        public override void Unbox(Type type)
        {
            Expect(typeof(object));
            Push(typeof(IntPtr));

            this.generator.Unbox(type);
        }

        /// <summary>
        /// Pops an object reference (representing a boxed value) from the stack, extracts the value,
        /// then pushes the value onto the stack.
        /// </summary>
        /// <param name="type"> The type of the boxed value.  This should be a value type. </param>
        public override void UnboxAny(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsValueType == false)
                throw new ArgumentException(nameof(type));

            this.generator.UnboxAny(type);
        }

        /// <summary>
        /// Pops a value from the stack, converts it to a signed integer, then pushes it back onto
        /// the stack.
        /// </summary>
        public override void ConvertToInteger()
        {
            ExpectNumber();
            Push(typeof(int));

            this.generator.ConvertToInteger();
        }

        /// <summary>
        /// Pops a value from the stack, converts it to an unsigned integer, then pushes it back
        /// onto the stack.
        /// </summary>
        public override void ConvertToUnsignedInteger()
        {
            ExpectNumber();
            Push(typeof(uint));

            this.generator.ConvertToUnsignedInteger();
        }

        /// <summary>
        /// Pops a value from the stack, converts it to a signed 64-bit integer, then pushes it
        /// back onto the stack.
        /// </summary>
        public override void ConvertToInt64()
        {
            ExpectNumber();
            Push(typeof(long));

            this.generator.ConvertToInt64();
        }

        /// <summary>
        /// Pops a value from the stack, converts it to an unsigned 64-bit integer, then pushes it
        /// back onto the stack.
        /// </summary>
        public override void ConvertToUnsignedInt64()
        {
            ExpectNumber();
            Push(typeof(ulong));

            this.generator.ConvertToUnsignedInt64();
        }

        /// <summary>
        /// Pops a value from the stack, converts it to a double, then pushes it back onto
        /// the stack.
        /// </summary>
        public override void ConvertToDouble()
        {
            Expect(t => t == typeof(int) || t == typeof(uint) ||
                t == typeof(long) || t == typeof(ulong) ||
                t == typeof(bool), "number or boolean");
            Push(typeof(double));

            this.generator.ConvertToDouble();
        }

        /// <summary>
        /// Pops an unsigned integer from the stack, converts it to a double, then pushes it back onto
        /// the stack.
        /// </summary>
        public override void ConvertUnsignedToDouble()
        {
            ExpectNumber();
            Push(typeof(double));

            this.generator.ConvertUnsignedToDouble();
        }




        //     OBJECTS, METHODS, TYPES AND FIELDS
        //_________________________________________________________________________________________

        /// <summary>
        /// Pops the constructor arguments off the stack and creates a new instance of the object.
        /// </summary>
        /// <param name="constructor"> The constructor that is used to initialize the object. </param>
        public override void NewObject(System.Reflection.ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            for (int i = parameters.Length - 1; i >= 0; i--)
                Expect(parameters[i].ParameterType);
            Push(constructor.DeclaringType);

            this.generator.NewObject(constructor);
        }

        /// <summary>
        /// Pops the method arguments off the stack, calls the given method, then pushes the result
        /// to the stack (if there was one).  This operation can be used to call instance methods,
        /// but virtual overrides will not be called and a null check will not be performed at the
        /// callsite.
        /// </summary>
        /// <param name="method"> The method to call. </param>
        public override void CallStatic(System.Reflection.MethodInfo method)
        {
            ValidateCall(method);
            this.generator.CallStatic(method);
        }

        /// <summary>
        /// Pops the method arguments off the stack, calls the given method, then pushes the result
        /// to the stack (if there was one).  This operation cannot be used to call static methods.
        /// Virtual overrides are obeyed and a null check is performed.
        /// </summary>
        /// <param name="method"> The method to call. </param>
        /// <exception cref="ArgumentException"> The method is static. </exception>
        public override void CallVirtual(System.Reflection.MethodInfo method)
        {
            ValidateCall(method);
            this.generator.CallVirtual(method);
        }

        /// <summary>
        /// Pushes the value of the given field onto the stack.
        /// </summary>
        /// <param name="field"> The field whose value will be pushed. </param>
        public override void LoadField(System.Reflection.FieldInfo field)
        {
            Push(field.FieldType);
            this.generator.LoadField(field);
        }

        /// <summary>
        /// Pops a value off the stack and stores it in the given field.
        /// </summary>
        /// <param name="field"> The field to modify. </param>
        public override void StoreField(System.Reflection.FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            Expect(field.FieldType);
            this.generator.StoreField(field);
        }

        /// <summary>
        /// Pops an object off the stack, checks that the object inherits from or implements the
        /// given type, and pushes the object onto the stack if the check was successful or
        /// throws an InvalidCastException if the check failed.
        /// </summary>
        /// <param name="type"> The type of the class the object inherits from or the interface the
        /// object implements. </param>
        public override void CastClass(Type type)
        {
            var baseType = this.stack.Pop();
            if (!baseType.IsAssignableFrom(type) && !type.IsAssignableFrom(baseType))
                throw new InvalidOperationException($"Cannot cast from {baseType} to {type}.");
            Push(type);

            this.generator.CastClass(type);
        }

        /// <summary>
        /// Changes the type of the value on the top of the stack, for the purpose of passing
        /// verification. Doesn't generate any IL instructions.
        /// </summary>
        /// <param name="type"> The type to convert to. </param>
        public override void ReinterpretCast(Type type)
        {
            this.stack.Pop();
            Push(type);
        }

        /// <summary>
        /// Pops an object off the stack, checks that the object inherits from or implements the
        /// given type, and pushes either the object (if the check was successful) or <c>null</c>
        /// (if the check failed) onto the stack.
        /// </summary>
        /// <param name="type"> The type of the class the object inherits from or the interface the
        /// object implements. </param>
        public override void IsInstance(Type type)
        {
            Expect(t => !t.IsValueType, "reference");
            Push(typeof(bool));

            this.generator.IsInstance(type);
        }

        /// <summary>
        /// Pushes a RuntimeTypeHandle corresponding to the given type onto the evaluation stack.
        /// </summary>
        /// <param name="type"> The type to convert to a RuntimeTypeHandle. </param>
        public override void LoadToken(Type type)
        {
            Push(typeof(RuntimeTypeHandle));
            this.generator.LoadToken(type);
        }

        /// <summary>
        /// Pushes a RuntimeMethodHandle corresponding to the given method onto the evaluation
        /// stack.
        /// </summary>
        /// <param name="method"> The method to convert to a RuntimeMethodHandle. </param>
        public override void LoadToken(MethodBase method)
        {
            Push(typeof(RuntimeTypeHandle));
            this.generator.LoadToken(method);
        }

        /// <summary>
        /// Pushes a RuntimeFieldHandle corresponding to the given field onto the evaluation stack.
        /// </summary>
        /// <param name="field"> The type to convert to a RuntimeFieldHandle. </param>
        public override void LoadToken(FieldInfo field)
        {
            Push(typeof(RuntimeTypeHandle));
            this.generator.LoadToken(field);
        }

        /// <summary>
        /// Pushes a pointer to the native code implementing the given method onto the evaluation
        /// stack.  The virtual qualifier will be ignored, if present.
        /// </summary>
        /// <param name="method"> The method to retrieve a pointer for. </param>
        public override void LoadStaticMethodPointer(System.Reflection.MethodBase method)
        {
            Push(typeof(IntPtr));
            this.generator.LoadStaticMethodPointer(method);
        }

        /// <summary>
        /// Pushes a pointer to the native code implementing the given method onto the evaluation
        /// stack.  This method cannot be used to retrieve a pointer to a static method.
        /// </summary>
        /// <param name="method"> The method to retrieve a pointer for. </param>
        /// <exception cref="ArgumentException"> The method is static. </exception>
        public override void LoadVirtualMethodPointer(System.Reflection.MethodBase method)
        {
            Push(typeof(IntPtr));
            this.generator.LoadVirtualMethodPointer(method);
        }

        /// <summary>
        /// Pops a managed or native pointer off the stack and initializes the referenced type with
        /// zeros.
        /// </summary>
        /// <param name="type"> The type the pointer on the top of the stack is pointing to. </param>
        public override void InitObject(Type type)
        {
            Expect(typeof(IntPtr));
            this.generator.InitObject(type);
        }




        //     ARRAYS
        //_________________________________________________________________________________________

        /// <summary>
        /// Pops the size of the array off the stack and pushes a new array of the given type onto
        /// the stack.
        /// </summary>
        /// <param name="type"> The element type. </param>
        public override void NewArray(Type type)
        {
            ExpectInt();
            Push(type.MakeArrayType());

            this.generator.NewArray(type);
        }

        /// <summary>
        /// Pops the array and index off the stack and pushes the element value onto the stack.
        /// </summary>
        /// <param name="type"> The element type. </param>
        public override void LoadArrayElement(Type type)
        {
            ExpectInt();
            Expect(type.MakeArrayType());
            Push(type);

            this.generator.LoadArrayElement(type);
        }

        /// <summary>
        /// Pops the array, index and value off the stack and stores the value in the array.
        /// </summary>
        /// <param name="type"> The element type. </param>
        public override void StoreArrayElement(Type type)
        {
            ExpectSubclass(type);
            ExpectInt();
            Expect(type.MakeArrayType());

            this.generator.StoreArrayElement(type);
        }

        /// <summary>
        /// Pops an array off the stack and pushes the length of the array onto the stack.
        /// </summary>
        public override void LoadArrayLength()
        {
            ExpectSubclass(typeof(Array));
            Push(typeof(int));

            this.generator.LoadArrayLength();
        }



        //     EXCEPTION HANDLING
        //_________________________________________________________________________________________

        /// <summary>
        /// Pops an exception object off the stack and throws the exception.
        /// </summary>
        public override void Throw()
        {
            ExpectSubclass(typeof(Exception));
            this.generator.Throw();
            this.stackIsIndeterminate = true;
        }

        /// <summary>
        /// Rethrows the current exception.
        /// </summary>
        public override void Rethrow()
        {
            this.generator.Rethrow();
            this.stackIsIndeterminate = true;
        }

        /// <summary>
        /// Begins a try-catch-finally block.  After issuing this instruction any following
        /// instructions are conceptually within the try block.
        /// </summary>
        public override void BeginExceptionBlock()
        {
            this.generator.BeginExceptionBlock();
        }

        /// <summary>
        /// Ends a try-catch-finally block.  BeginExceptionBlock() must have already been called.
        /// </summary>
        public override void EndExceptionBlock()
        {
            if (this.stack.Count != 0 && !this.stackIsIndeterminate)
                throw new InvalidOperationException("The stack should be empty or indeterminate.");
            this.stack.Clear();
            this.stackIsIndeterminate = false;

            this.generator.EndExceptionBlock();
        }

        /// <summary>
        /// Begins a catch block.  BeginExceptionBlock() must have already been called.
        /// </summary>
        /// <param name="exceptionType"> The type of exception to handle. </param>
        public override void BeginCatchBlock(Type exceptionType)
        {
            if (this.stack.Count != 0 && !this.stackIsIndeterminate)
                throw new InvalidOperationException("The stack should be empty or indeterminate.");
            this.stack.Clear();
            this.stackIsIndeterminate = false;
            Push(exceptionType);

            this.generator.BeginCatchBlock(exceptionType);
        }

        /// <summary>
        /// Begins a finally block.  BeginExceptionBlock() must have already been called.
        /// </summary>
        public override void BeginFinallyBlock()
        {
            if (this.stack.Count != 0 && !this.stackIsIndeterminate)
                throw new InvalidOperationException("The stack should be empty or indeterminate.");
            this.stack.Clear();
            this.stackIsIndeterminate = false;

            this.generator.BeginFinallyBlock();
        }

        /// <summary>
        /// Begins a filter block.  BeginExceptionBlock() must have already been called.
        /// </summary>
        public override void BeginFilterBlock()
        {
            if (this.stack.Count != 0 && !this.stackIsIndeterminate)
                throw new InvalidOperationException("The stack should be empty or indeterminate.");
            this.stack.Clear();
            this.stackIsIndeterminate = false;

            this.generator.BeginFilterBlock();
        }

        /// <summary>
        /// Begins a fault block.  BeginExceptionBlock() must have already been called.
        /// </summary>
        public override void BeginFaultBlock()
        {
            if (this.stack.Count != 0 && !this.stackIsIndeterminate)
                throw new InvalidOperationException("The stack should be empty or indeterminate.");
            this.stack.Clear();
            this.stackIsIndeterminate = false;

            this.generator.BeginFaultBlock();
        }

        /// <summary>
        /// Unconditionally branches to the given label.  Unlike the regular branch instruction,
        /// this instruction can exit out of try, filter and catch blocks.
        /// </summary>
        /// <param name="label"> The label to branch to. </param>
        public override void Leave(ILLabel label)
        {
            ValidateBranch((VerifyingILLabel)label);
            this.stackIsIndeterminate = true;

            this.generator.Leave(((VerifyingILLabel)label).UnderlyingLabel);
        }

        /// <summary>
        /// This instruction can be used from within a finally block to resume the exception
        /// handling process.  It is the only valid way of leaving a finally block.
        /// </summary>
        public override void EndFinally()
        {
            if (this.stack.Count != 0 && !this.stackIsIndeterminate)
                throw new InvalidOperationException("The stack should be empty or indeterminate.");
            this.stack.Clear();
            this.stackIsIndeterminate = false;

            this.generator.EndFinally();
        }

        /// <summary>
        /// This instruction can be used from within a filter block to indicate whether the
        /// exception will be handled.  It pops an integer from the stack which should be <c>0</c>
        /// to continue searching for an exception handler or <c>1</c> to use the handler
        /// associated with the filter.  EndFilter() must be called at the end of a filter block.
        /// </summary>
        public override void EndFilter()
        {
            ExpectInt();
            if (this.stack.Count != 0 && !this.stackIsIndeterminate)
                throw new InvalidOperationException("The stack should be empty or indeterminate.");
            this.stack.Clear();
            this.stackIsIndeterminate = false;

            this.generator.EndFilter();
        }



        //     DEBUGGING SUPPORT
        //_________________________________________________________________________________________

        /// <summary>
        /// Triggers a breakpoint in an attached debugger.
        /// </summary>
        public override void Breakpoint()
        {
            this.generator.Breakpoint();
        }



        //     MISC
        //_________________________________________________________________________________________

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override void NoOperation()
        {
            this.generator.NoOperation();
        }



        //     OBJECT OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Converts the object to a string.
        /// </summary>
        /// <returns> A string containing the IL generated by this object. </returns>
        public override string ToString()
        {
            return ToString(this.stack) + (this.stackIsIndeterminate ? "?" : "");
        }



        //     PRIVATE HELPER METHODS
        //_________________________________________________________________________________________

        private void Push(Type type)
        {
            this.stack.Push(type);
        }

        private Type Expect(Func<Type, bool> predicate, string description)
        {
            if (this.stack.Count == 0)
                throw new InvalidOperationException($"Stack is empty; expected stack entry of type {description}.");
            var actualType = this.stack.Pop();
            if (!predicate(actualType))
                throw new InvalidOperationException($"Value on top of stack is of type {actualType}; expected {description}.");
            return actualType;
        }

        private void ExpectInt()
        {
            Expect(t => t == typeof(int) || t == typeof(uint), "integer");
        }

        private void ExpectNumber()
        {
            Expect(t => t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong) || t == typeof(double), "number");
        }

        private void Expect(Type expectedType)
        {
            Expect(t => t == expectedType || (t == typeof(DBNull) && !expectedType.IsValueType), expectedType.Name);
        }

        private void ExpectSubclass(Type expectedType)
        {
            Expect(t => expectedType.IsAssignableFrom(t) || (t == typeof(DBNull) && !expectedType.IsValueType), expectedType.Name + " or derived type");
        }

        private void ValidateBinaryOperands(bool integerOnly = false)
        {
            var invertedintegerOnly = !integerOnly;
            if (this.stack.Count < 2)
                throw new InvalidOperationException($"Stack should contain at least two numbers.");
            var operand1 = this.stack.Pop();
            var operand2 = this.stack.Pop();
            if (operand1 == typeof(int) || operand1 == typeof(uint))
            {
                if (operand2 != typeof(int) && operand2 != typeof(uint))
                    throw new InvalidOperationException($"Value on top of stack is {operand2}; expected an integer.");
                Push(typeof(int));
            }
            else if (operand1 == typeof(bool))
            {
                if (operand2 != typeof(bool))
                    throw new InvalidOperationException($"Value on top of stack is {operand2}; expected a boolean.");
                Push(typeof(bool));
            }
            else if (!integerOnly && operand1 == typeof(double))
            {
                if (operand2 != typeof(double))
                    throw new InvalidOperationException($"Value on top of stack is {operand2}; expected a double.");
                Push(typeof(double));
            }
            else
                throw new InvalidOperationException($"Value on top of stack is {operand1}; expected a number.");
        }

        private void ValidateCall(MethodInfo method)
        {
            var parameters = method.GetParameters();
            for (int i = parameters.Length - 1; i >= 0; i--)
                ExpectSubclass(parameters[i].ParameterType);
            if (method.IsStatic == false)
                ExpectSubclass(method.DeclaringType);
            if (method.ReturnType != typeof(void))
                Push(method.ReturnType);
        }

        private void ValidateBranch(VerifyingILLabel label)
        {
            if (label.Stack != null)
            {
                // Compare stack with existing stack.
                if (!Equals(this.stack, label.Stack))
                    throw new InvalidOperationException($"Inconsistent stacks; label has: {ToString(label.Stack)}, ILGenerator has {ToString(this.stack)}");
            }
            else
            {
                // The label has not been branched to yet.
                label.Stack = Clone(this.stack);
            }
        }

        private static Stack<Type> Clone(Stack<Type> original)
        {
            var arr = new Type[original.Count];
            original.CopyTo(arr, 0);
            Array.Reverse(arr);
            return new Stack<Type>(arr);
        }

        private static bool Equals(Stack<Type> a, Stack<Type> b)
        {
            return ToString(a) == ToString(b);
        }

        private static string ToString(Stack<Type> stack)
        {
            var result = new StringBuilder();
            foreach (var item in stack)
            {
                if (result.Length > 0)
                    result.Append(", ");
                result.Append(item.Name);
            }
            return result.ToString();
        }

        /// <summary>
        /// Represents a label in IL code.
        /// </summary>
        private class VerifyingILLabel : ILLabel
        {
            /// <summary>
            /// Creates a new label instance.
            /// </summary>
            /// <param name="label"> The underlying label. </param>
            public VerifyingILLabel(ILLabel label)
            {
                this.UnderlyingLabel = label ?? throw new ArgumentNullException(nameof(label)); ;
            }

            /// <summary>
            /// Gets the underlying label.
            /// </summary>
            public ILLabel UnderlyingLabel { get; private set; }

            /// <summary>
            /// A copy of the stack at the point the flow branched.
            /// </summary>
            public Stack<Type> Stack { get; set; }

            /// <summary>
            /// Indicates whether the label has been marked. This must be done once and only once.
            /// </summary>
            public bool Marked { get; set; }
        }
    }
}
