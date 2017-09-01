using ErrorType = Jurassic.Library.ErrorType;

namespace Jurassic.Compiler
{
    
    /// <summary>
    /// Represents a variable or member access.
    /// </summary>
    internal sealed class MemberAccessExpression : OperatorExpression, IReferenceExpression
    {
        /// <summary>
        /// Creates a new instance of MemberAccessExpression.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        public MemberAccessExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Gets an expression that evaluates to the object that is being accessed or modified.
        /// </summary>
        public Expression Base
        {
            get { return this.GetOperand(0); }
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Any; }
        }

        /// <summary>
        /// Gets the static type of the reference.
        /// </summary>
        public PrimitiveType Type
        {
            get { return PrimitiveType.Any; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // NOTE: this is a get reference because assignment expressions do not call this method.
            GenerateReference(generator, optimizationInfo);
            GenerateGet(generator, optimizationInfo, false);
        }

        private enum TypeOfMemberAccess
        {
            /// <summary>
            /// Static property access e.g. a.b or a['b']
            /// </summary>
            Static,

            /// <summary>
            /// Numeric array indexer e.g. a[1]
            /// </summary>
            ArrayIndex,

            /// <summary>
            /// Dynamic property access e.g. a[someVariable]
            /// </summary>
            Dynamic,
        }

        /// <summary>
        /// Determines the type of member access.
        /// </summary>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <returns></returns>
        private TypeOfMemberAccess DetermineTypeOfMemberAccess(OptimizationInfo optimizationInfo, out string propertyName)
        {
            // Right-hand-side can be a property name (a.b)
            if (this.OperatorType == OperatorType.MemberAccess)
            {
                var rhs = this.GetOperand(1) as NameExpression;
                if (rhs == null)
                    throw new SyntaxErrorException("Invalid member access", optimizationInfo.SourceSpan.StartLine, optimizationInfo.Source.Path, optimizationInfo.FunctionName);
                propertyName = rhs.Name;
                return TypeOfMemberAccess.Static;
            }

            // Or a constant indexer (a['b'])
            if (this.OperatorType == OperatorType.Index)
            {
                var rhs = this.GetOperand(1) as LiteralExpression;
                if (rhs != null && (PrimitiveTypeUtilities.IsNumeric(rhs.ResultType) || rhs.ResultType == PrimitiveType.String))
                {
                    propertyName = TypeConverter.ToString(rhs.Value);

                    // Or a array index (a[0])
                    if (rhs.ResultType == PrimitiveType.Int32 || (propertyName != null && Library.ArrayInstance.ParseArrayIndex(propertyName) != uint.MaxValue))
                        return TypeOfMemberAccess.ArrayIndex;
                    return TypeOfMemberAccess.Static;
                }
            }

            propertyName = null;
            return TypeOfMemberAccess.Dynamic;
        }


        /// <summary>
        /// Outputs the values needed to get or set this reference.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void GenerateReference(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            string propertyName = null;
            TypeOfMemberAccess memberAccessType = DetermineTypeOfMemberAccess(optimizationInfo, out propertyName);

            if (memberAccessType == TypeOfMemberAccess.ArrayIndex)
            {
                // Array indexer
                // -------------

                // Load the left-hand side and convert to an object instance.
                var lhs = this.GetOperand(0);
                lhs.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToObject(generator, lhs.ResultType, optimizationInfo);

                // Load the right-hand side and convert to a uint32.
                var rhs = this.GetOperand(1);
                rhs.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToUInt32(generator, rhs.ResultType);
            }
            else if (memberAccessType == TypeOfMemberAccess.Static)
            {
                // Named property access (e.g. x = y.property or x = y['property'])
                // ----------------------------------------------------------------

                // Load the left-hand side and convert to an object instance.
                var lhs = this.GetOperand(0);
                lhs.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToObject(generator, lhs.ResultType, optimizationInfo);
            }
            else
            {
                // Dynamic property access
                // -----------------------

                // Load the left-hand side and convert to an object instance.
                var lhs = this.GetOperand(0);
                lhs.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToObject(generator, lhs.ResultType, optimizationInfo);

                // Load the value and convert it to a property key.
                var rhs = this.GetOperand(1);
                rhs.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToPropertyKey(generator, rhs.ResultType);
            }
        }

        /// <summary>
        /// Outputs the values needed to get or set this reference.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void DuplicateReference(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            string propertyName = null;
            TypeOfMemberAccess memberAccessType = DetermineTypeOfMemberAccess(optimizationInfo, out propertyName);

            if (memberAccessType == TypeOfMemberAccess.ArrayIndex)
            {
                // Array indexer
                var arg1 = generator.CreateTemporaryVariable(typeof(object));
                var arg2 = generator.CreateTemporaryVariable(typeof(uint));
                generator.StoreVariable(arg2);
                generator.StoreVariable(arg1);
                generator.LoadVariable(arg1);
                generator.LoadVariable(arg2);
                generator.LoadVariable(arg1);
                generator.LoadVariable(arg2);
                generator.ReleaseTemporaryVariable(arg1);
                generator.ReleaseTemporaryVariable(arg2);
            }
            else if (memberAccessType == TypeOfMemberAccess.Static)
            {
                // Named property access
                generator.Duplicate();
            }
            else
            {
                // Dynamic property access
                var arg1 = generator.CreateTemporaryVariable(typeof(object));
                var arg2 = generator.CreateTemporaryVariable(typeof(object));
                generator.StoreVariable(arg2);
                generator.StoreVariable(arg1);
                generator.LoadVariable(arg1);
                generator.LoadVariable(arg2);
                generator.LoadVariable(arg1);
                generator.LoadVariable(arg2);
                generator.ReleaseTemporaryVariable(arg1);
                generator.ReleaseTemporaryVariable(arg2);
            }
        }

        /// <summary>
        /// Pushes the value of the reference onto the stack.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <param name="throwIfUnresolvable"> <c>true</c> to throw a ReferenceError exception if
        /// the name is unresolvable; <c>false</c> to output <c>null</c> instead. </param>
        public void GenerateGet(ILGenerator generator, OptimizationInfo optimizationInfo, bool throwIfUnresolvable)
        {
            string propertyName = null;
            TypeOfMemberAccess memberAccessType = DetermineTypeOfMemberAccess(optimizationInfo, out propertyName);

            if (memberAccessType == TypeOfMemberAccess.ArrayIndex)
            {
                // Array indexer
                // -------------
                // xxx = object[index]

                // Call the indexer.
                generator.Call(ReflectionHelpers.ObjectInstance_GetPropertyValue_Int);
            }
            else if (memberAccessType == TypeOfMemberAccess.Static)
            {
                // Named property access (e.g. x = y.property)
                // -------------------------------------------
                // value = object.GetPropertyValue("property")

                // Call GetPropertyValue
                generator.LoadString(propertyName);
                generator.Call(ReflectionHelpers.ObjectInstance_GetPropertyValue_Object);
            }
            else
            {
                // Dynamic property access
                // -----------------------
                // x = y.GetPropertyValue("property")

                generator.Call(ReflectionHelpers.ObjectInstance_GetPropertyValue_Object);
            }
        }

        /// <summary>
        /// Stores the value on the top of the stack in the reference.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <param name="valueType"> The primitive type of the value that is on the top of the stack. </param>
        /// <param name="throwIfUnresolvable"> <c>true</c> to throw a ReferenceError exception if
        /// the name is unresolvable; <c>false</c> to create a new property instead. </param>
        public void GenerateSet(ILGenerator generator, OptimizationInfo optimizationInfo, PrimitiveType valueType, bool throwIfUnresolvable)
        {
            string propertyName = null;
            TypeOfMemberAccess memberAccessType = DetermineTypeOfMemberAccess(optimizationInfo, out propertyName);

            if (memberAccessType == TypeOfMemberAccess.ArrayIndex)
            {
                // Array indexer
                // -------------
                // xxx = object[index]

                // Call the indexer.
                EmitConversion.ToAny(generator, valueType);
                generator.LoadBoolean(optimizationInfo.StrictMode);
                generator.Call(ReflectionHelpers.ObjectInstance_SetPropertyValue_Int);
            }
            else if (memberAccessType == TypeOfMemberAccess.Static)
            {
                // Named property modification (e.g. object.property = value)
                // ----------------------------------------------------------
                // object.SetPropertyValue(property, value, strictMode)

                // Convert the value to an object and store it in a temporary variable.
                var value = generator.CreateTemporaryVariable(typeof(object));
                EmitConversion.ToAny(generator, valueType);
                generator.StoreVariable(value);

                // Call SetPropertyValue
                generator.LoadString(propertyName);
                generator.LoadVariable(value);
                generator.LoadBoolean(optimizationInfo.StrictMode);
                generator.Call(ReflectionHelpers.ObjectInstance_SetPropertyValue_Object);

                generator.ReleaseTemporaryVariable(value);
            }
            else
            {
                // Dynamic property access
                // -----------------------
                // object.SetPropertyValue(property, value, strictMode)

                EmitConversion.ToAny(generator, valueType);
                generator.LoadBoolean(optimizationInfo.StrictMode);
                generator.Call(ReflectionHelpers.ObjectInstance_SetPropertyValue_Object);
            }
        }

        /// <summary>
        /// Deletes the reference and pushes <c>true</c> if the delete succeeded, or <c>false</c>
        /// if the delete failed.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void GenerateDelete(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Load the left-hand side and convert to an object instance.
            var lhs = this.GetOperand(0);
            lhs.GenerateCode(generator, optimizationInfo);
            EmitConversion.ToObject(generator, lhs.ResultType, optimizationInfo);

            // Load the property name and convert to a string.
            var rhs = this.GetOperand(1);
            if (this.OperatorType == OperatorType.MemberAccess && rhs is NameExpression)
                generator.LoadString((rhs as NameExpression).Name);
            else
            {
                rhs.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToPropertyKey(generator, rhs.ResultType);
            }

            // Call Delete()
            generator.LoadBoolean(optimizationInfo.StrictMode);
            generator.Call(ReflectionHelpers.ObjectInstance_Delete);

            // If the return value is not wanted then pop it from the stack.
            //if (optimizationInfo.SuppressReturnValue == true)
            //    generator.Pop();
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            if (this.OperatorType == OperatorType.MemberAccess)
                return string.Format("{0}.{1}", this.GetRawOperand(0), this.OperandCount >= 2 ? this.GetRawOperand(1).ToString() : "?");
            return base.ToString();
        }
    }
}