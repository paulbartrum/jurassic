using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a template literal expression, with substitutions.
    /// Code gen treats template literals with no substitutions just like string literals.
    /// </summary>
    internal sealed class TemplateLiteralExpression : Expression
    {
        /// <summary>
        /// Creates a new instance of TemplateLiteralExpression.
        /// </summary>
        /// <param name="strings"> The literal string parts of the template.  For example `1${2}3`
        /// has the string literal parts "1" and "3". </param>
        /// <param name="values"> The substitution expressions in the template.  For example
        /// `1${2}3` has the substitution expression "2". </param>
        /// <param name="rawStrings"> The literal string parts of the template, prior to performing
        /// escape sequence processing. </param>
        public TemplateLiteralExpression(List<string> strings, List<Expression> values, List<string> rawStrings)
        {
            // There should be one more string than there are values.
            Debug.Assert(strings.Count == values.Count + 1);
            Debug.Assert(strings.Count == rawStrings.Count);

            this.Strings = strings;
            this.Values = values;
            this.RawStrings = rawStrings;
        }

        /// <summary>
        /// The literal string parts of the template.  For example `1${2}3` has the string literal
        /// parts "1" and "3".
        /// </summary>
        public List<string> Strings { get; private set; }

        /// <summary>
        /// The substitution expressions in the template.  For example `1${2}3` has the
        /// substitution expression "2".
        /// </summary>
        public List<Expression> Values { get; private set; }

        /// <summary>
        /// The literal string parts of the template, prior to performing escape sequence
        /// processing.
        /// </summary>
        public List<string> RawStrings { get; private set; }

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
            // This code is only used for untagged template literals.
            // Tagged template literals are handled by FunctionCallExpression.

            // Construct a .NET format string.
            var formatString = new StringBuilder();
            for (int i = 0; i < this.Values.Count; i++)
            {
                formatString.Append(this.Strings[i]);
                formatString.Append("{");
                formatString.Append(i);
                formatString.Append("}");
            }
            formatString.Append(this.Strings[this.Strings.Count - 1]);

            // Load the format string onto the stack.
            generator.LoadString(formatString.ToString());

            // Load the values array onto the stack.
            generator.LoadInt32(this.Values.Count);
            generator.NewArray(typeof(object));
            for (int i = 0; i < this.Values.Count; i++)
            {
                // Operands for StoreArrayElement() are: an array (object[]), index (int), value (object).
                // Array
                generator.Duplicate();

                // Index
                generator.LoadInt32(i);

                // Value
                this.Values[i].GenerateCode(generator, optimizationInfo);
                EmitConversion.ToAny(generator, this.Values[i].ResultType);

                // Store the element value.
                generator.StoreArrayElement(typeof(object));
            }

            // Call String.Format(string, object[])
            generator.CallStatic(ReflectionHelpers.String_Format);
        }
    }


}