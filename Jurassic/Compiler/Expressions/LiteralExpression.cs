using System;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a simple literal expression (not an array literal or object literal).
    /// </summary>
    internal sealed class LiteralExpression : Expression
    {
        /// <summary>
        /// Creates a new instance of LiteralExpression.
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
        /// Evaluates the expression, if possible.
        /// </summary>
        /// <returns> The result of evaluating the expression, or <c>null</c> if the expression can
        /// not be evaluated. </returns>
        public override object Evaluate()
        {
            // RegExp literal.
            if (this.Value is RegularExpressionLiteral)
                return null;

            // Everything else.
            return this.Value;
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get
            {
                // RegExp literal.
                if (this.Value is RegularExpressionLiteral)
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
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Literals cannot have side-effects so if a return value is not expected then generate
            // nothing.
            //if (optimizationInfo.SuppressReturnValue == true) 
            //    return;

            if (this.Value is int)
                generator.LoadInt32((int)this.Value);
            else if (this.Value is double)
                generator.LoadDouble((double)this.Value);
            else if (this.Value is string)
                generator.LoadString((string)this.Value);
            else if (this.Value is bool)
                generator.LoadBoolean((bool)this.Value);
            else if (this.Value is RegularExpressionLiteral)
            {
                // RegExp
                var sharedRegExpVariable = optimizationInfo.GetRegExpVariable(generator, (RegularExpressionLiteral)this.Value);
                var label1 = generator.CreateLabel();
                var label2 = generator.CreateLabel();

                // if (sharedRegExp == null) {
                generator.LoadVariable(sharedRegExpVariable);
                generator.LoadNull();
                generator.BranchIfNotEqual(label1);

                // sharedRegExp = Global.RegExp.Construct(source, flags)
                EmitHelpers.LoadScriptEngine(generator);
                generator.Call(ReflectionHelpers.ScriptEngine_RegExp);
                generator.LoadString(((RegularExpressionLiteral)this.Value).Pattern);
                generator.LoadString(((RegularExpressionLiteral)this.Value).Flags);
                generator.Call(ReflectionHelpers.RegExp_Construct);
                generator.Duplicate();
                generator.StoreVariable(sharedRegExpVariable);

                // } else {
                generator.Branch(label2);
                generator.DefineLabelPosition(label1);

                // Global.RegExp.Construct(sharedRegExp, flags)
                EmitHelpers.LoadScriptEngine(generator);
                generator.Call(ReflectionHelpers.ScriptEngine_RegExp);
                generator.LoadVariable(sharedRegExpVariable);
                generator.LoadNull();
                generator.Call(ReflectionHelpers.RegExp_Construct);

                // }
                generator.DefineLabelPosition(label2);
            }
            else if (this.Value == Null.Value)
            {
                // Null.
                EmitHelpers.EmitNull(generator);
            }
            else if (this.Value == Undefined.Value)
            {
                // Undefined.
                EmitHelpers.EmitUndefined(generator);
            }
            else
                throw new NotImplementedException("Unknown literal type.");
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            // RegExp literal.
            if (this.Value is RegularExpressionLiteral)
                return this.Value.ToString();

            // String literal.
            if (this.Value is string)
                return Library.StringInstance.Quote((string)this.Value);

            // Everything else.
            return TypeConverter.ToString(this.Value);
        }
    }


}