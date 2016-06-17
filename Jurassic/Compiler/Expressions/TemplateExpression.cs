using System;
using System.Collections.Generic;
using System.Text;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a template literal expression, with substitutions.  Template literals without
    /// substitutions use LiteralExpression instead.
    /// </summary>
    internal sealed class TemplateExpression : Expression
    {
        private string formatString;
        private List<Expression> values;

        /// <summary>
        /// Creates a new instance of TemplateExpression.
        /// </summary>
        /// <param name="tag"> The tag name, or <c>null</c> if none were specified. </param>
        /// <param name="strings"> The literal string parts of the template.  For example `1${2}3`
        /// has the string literal parts "1" and "3". </param>
        /// <param name="values"> The substitution expressions in the template.  For example
        /// `1${2}3` has the substitution expression "2". </param>
        public TemplateExpression(Expression tag, List<string> strings, List<Expression> values)
        {
            // The template literal `Purchased ${count} item${count == 1 ? "" : "s"}`
            // translates to this function call:
            // String.raw(["Purchased ", " items"], count, count == 1 ? "" : "s")

            // The template literal tag`Purchased ${count} item${count == 1 ? "" : "s"}`
            // translates to this function call:
            // tag(["Purchased ", " items"], count, count == 1 ? "" : "s")

            //// Convert the list of strings into a list of string literals.
            //var stringsExpression = new List<Expression>(strings.Count);
            //foreach (var str in strings)
            //{
            //    stringsExpression.Add(new LiteralExpression(str));
            //}
            //Push(new LiteralExpression(stringsExpression));

            var result = new StringBuilder();
            for (int i = 0; i < values.Count; i ++)
            {
                result.Append(strings[i]);
                result.Append("{");
                result.Append(i);
                result.Append("}");
            }
            result.Append(strings[strings.Count - 1]);
            this.formatString = result.ToString();
            this.values = values;
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.String; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Load the format string onto the stack.
            generator.LoadString(this.formatString);

            // Load the values array onto the stack.
            generator.LoadInt32(this.values.Count);
            generator.NewArray(typeof(object));
            for (int i = 0; i < this.values.Count; i++)
            {
                // Operands for StoreArrayElement() are: an array (object[]), index (int), value (object).
                // Array
                generator.Duplicate();

                // Index
                generator.LoadInt32(i);

                // Value
                this.values[i].GenerateCode(generator, optimizationInfo);
                EmitConversion.ToAny(generator, this.values[i].ResultType);

                // Store the element value.
                generator.StoreArrayElement(typeof(object));
            }

            // Call String.Format(string, object[])
            generator.CallStatic(ReflectionHelpers.String_Format);
        }
    }


}