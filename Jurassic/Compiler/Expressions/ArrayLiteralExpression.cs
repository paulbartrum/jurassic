using System.Collections.Generic;
using System.Linq;

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
        public IList<Expression> Items
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

            // Construct a object[] from the list of expressions.
            GenerateObjectArrayCode(generator, optimizationInfo, Items);

            // ArrayConstructor.New(object[])
            generator.Call(ReflectionHelpers.Array_New);
        }

        /// <summary>
        /// Generates an array from a list of expressions.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <param name="items"> The items to add to the array. </param>
        public static void GenerateObjectArrayCode(ILGenerator generator, OptimizationInfo optimizationInfo, IList<Expression> items)
        {
            if (items.Any(item => item is UnaryExpression opEx && opEx.OperatorType == OperatorType.SpreadSyntax))
            {
                // The array contains the spread syntax operator '...'

                // new ArrayBuilder(items.Count)
                generator.LoadInt32(items.Count);
                generator.NewObject(ReflectionHelpers.ArrayBuilder_Constructor_Int);

                for (int i = 0; i < items.Count; i++)
                {
                    generator.Duplicate();
                    var elementExpression = items[i];
                    if (elementExpression == null)
                    {
                        // arrayBuilder.Add(null)
                        generator.LoadNull();
                        generator.Call(ReflectionHelpers.ArrayBuilder_Add_Object);
                    }
                    else if (elementExpression is UnaryExpression opEx && opEx.OperatorType == OperatorType.SpreadSyntax)
                    {
                        // arrayBuilder.AddIterable(scriptEngine, item)
                        EmitHelpers.LoadScriptEngine(generator);
                        opEx.Operand.GenerateCode(generator, optimizationInfo);
                        EmitConversion.ToAny(generator, opEx.Operand.ResultType);
                        generator.Call(ReflectionHelpers.ArrayBuilder_AddIterable_Object);
                    }
                    else
                    {
                        // arrayBuilder.Add(item)
                        elementExpression.GenerateCode(generator, optimizationInfo);
                        EmitConversion.ToAny(generator, elementExpression.ResultType);
                        generator.Call(ReflectionHelpers.ArrayBuilder_Add_Object);
                    }
                }

                // arrayBuilder.ToArray()
                generator.Call(ReflectionHelpers.ArrayBuilder_ToArray);
            }
            else
            {
                // object[]
                generator.LoadInt32(items.Count);
                generator.NewArray(typeof(object));
                for (int i = 0; i < items.Count; i++)
                {
                    // Operands for StoreArrayElement() are: an array (object[]), index (int), value (object).
                    // Array
                    generator.Duplicate();

                    // Index
                    generator.LoadInt32(i);

                    // Value
                    var elementExpression = items[i];
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
            }
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