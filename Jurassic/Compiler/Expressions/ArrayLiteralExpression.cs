using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents an array literal expression.
    /// </summary>
    internal sealed class ArrayLiteralExpression : Expression
    {
        /// <summary>
        /// Creates a new instance of ArrayLiteralExpression.
        /// </summary>
        /// <param name="items"> A list of values in the array. </param>
        public ArrayLiteralExpression(List<Expression> items)
        {
            this.Items = items;
        }

        /// <summary>
        /// Gets the literal value.
        /// </summary>
        public IReadOnlyList<Expression> Items
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Object; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Operands for ArrayConstructor.New() are: an ArrayConstructor instance (ArrayConstructor), an array (object[])
            // ArrayConstructor
            EmitHelpers.LoadScriptEngine(generator);
            generator.Call(ReflectionHelpers.ScriptEngine_Array);

            // object[]
            generator.LoadInt32(Items.Count);
            generator.NewArray(typeof(object));
            for (int i = 0; i < Items.Count; i ++)
            {
                // Operands for StoreArrayElement() are: an array (object[]), index (int), value (object).
                // Array
                generator.Duplicate();

                // Index
                generator.LoadInt32(i);

                // Value
                var elementExpression = Items[i];
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

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            var result = new System.Text.StringBuilder("[");
            foreach (var item in this.Items)
            {
                if (result.Length > 1)
                    result.Append(", ");
                result.Append(item);
            }
            result.Append("]");
            return result.ToString();
        }
    }


}