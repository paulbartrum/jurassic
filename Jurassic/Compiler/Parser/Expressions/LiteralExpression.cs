using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a literal expression.
    /// </summary>
    internal sealed class LiteralExpression : Expression
    {
        /// <summary>
        /// Creates a new instance of LiteralJSExpression.
        /// </summary>
        /// <param name="value"> The literal value. </param>
        public LiteralExpression(object value)
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
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get
            {
                // Array literal.
                if (this.Value is List<Expression>)
                    return PrimitiveType.Object;

                // Object literal.
                if (this.Value is Dictionary<string, Expression>)
                    return PrimitiveType.Object;

                // Everything else.
                return PrimitiveTypeUtilities.ToPrimitiveType(this.Value.GetType());
            }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Literals cannot have side-effects so if a return value is not expected then generate
            // nothing.
            if (optimizationInfo.SuppressReturnValue == true) 
                return;

            if (this.Value is int)
                generator.LoadInt32((int)this.Value);
            else if (this.Value is double)
                generator.LoadDouble((double)this.Value);
            else if (this.Value is string)
                generator.LoadString((string)this.Value);
            else if (this.Value is bool)
                generator.LoadBoolean((bool)this.Value);
            else if (this.Value is Library.RegExpInstance)
            {
                // RegExp
                generator.Call(ReflectionHelpers.Global_RegExp);
                generator.LoadString(((Library.RegExpInstance)this.Value).Source);
                generator.LoadString(((Library.RegExpInstance)this.Value).Flags);
                generator.Call(ReflectionHelpers.RegExp_Construct);
            }
            else if (this.Value == Null.Value)
            {
                // Null.
                EmitHelpers.EmitNull(generator);
            }
            else if (this.Value is List<Expression>)
            {
                // Construct an array literal.
                var arrayLiteral = (List<Expression>)this.Value;

                // Operands for ArrayConstructor.New() are: an ArrayConstructor instance (ArrayConstructor), an array (object[])
                // ArrayConstructor
                generator.Call(ReflectionHelpers.Global_Array);

                // object[]
                generator.LoadInt32(arrayLiteral.Count);
                generator.NewArray(typeof(object));
                for (int i = 0; i < arrayLiteral.Count; i ++)
                {
                    // Operands for StoreArrayElement() are: an array (object[]), index (int), value (object).
                    // Array
                    generator.Duplicate();

                    // Index
                    generator.LoadInt32(i);

                    // Value
                    var elementExpression = arrayLiteral[i];
                    if (elementExpression == null)
                        generator.LoadNull();
                    else
                    {
                        elementExpression.GenerateCode(generator, optimizationInfo);
                        EmitConversion.ToAny(generator, elementExpression.ResultType);
                    }

                    // Store the element value.
                    generator.StoreArrayElement(typeof(object));
                }

                // ArrayConstructor.New(object[])
                generator.Call(ReflectionHelpers.Array_New);
            }
            else if (this.Value is Dictionary<string, Expression>)
            {
                // This is an object literal.
                var properties = (Dictionary<string, Expression>)this.Value;

                // Create a new object.
                generator.Call(ReflectionHelpers.Global_Object);
                generator.Call(ReflectionHelpers.Object_Construct);

                foreach (var keyValuePair in properties)
                {
                    string propertyName = keyValuePair.Key;
                    Expression propertyValue = keyValuePair.Value;

                    // Add a new property to the object.
                    generator.Duplicate();
                    generator.LoadString(propertyName);
                    propertyValue.GenerateCode(generator, optimizationInfo);
                    EmitConversion.ToAny(generator, propertyValue.ResultType);
                    generator.Call(ReflectionHelpers.ObjectInstance_SetItem_String);
                }
            }
            else if (PrimitiveTypeUtilities.ToPrimitiveType(this.Value.GetType()) != PrimitiveType.Undefined)
                throw new NotImplementedException("TODO: literal values should be actual RegExps, ArrayInstances, etc.");
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            return TypeConverter.ToString(this.Value);
        }
    }


}