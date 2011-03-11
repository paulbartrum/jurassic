using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a javascript with statement.
    /// </summary>
    internal class WithStatement : Statement
    {
        /// <summary>
        /// Creates a new WithStatement instance.
        /// </summary>
        /// <param name="labels"> The labels that are associated with this statement. </param>
        public WithStatement(IList<string> labels)
            : base(labels)
        {
        }

        /// <summary>
        /// Gets or sets the object scope inside the with statement.
        /// </summary>
        public ObjectScope Scope
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the body of the with statement.
        /// </summary>
        public Statement Body
        {
            get;
            set;
        }

        /// <summary>
        /// Generates CIL for the statement.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // This statement is not allowed in strict mode.
            if (optimizationInfo.StrictMode == true)
                throw new JavaScriptException(optimizationInfo.Engine, "SyntaxError", "The with statement is not supported in strict mode");

            // Create the scope.
            this.Scope.GenerateScopeCreation(generator, optimizationInfo);

            // Make sure the scope is reverted even if an exception is thrown.
            generator.BeginExceptionBlock();

            // Generate code for the body statements.
            this.Body.GenerateCode(generator, optimizationInfo);

            // Revert the scope.
            generator.BeginFinallyBlock();
            EmitHelpers.LoadScope(generator);
            generator.Call(ReflectionHelpers.Scope_ParentScope);
            EmitHelpers.StoreScope(generator);
            generator.EndExceptionBlock();
        }

        /// <summary>
        /// Converts the statement to a string.
        /// </summary>
        /// <param name="indentLevel"> The number of tabs to include before the statement. </param>
        /// <returns> A string representing this statement. </returns>
        public override string ToString(int indentLevel)
        {
            var result = new System.Text.StringBuilder();
            result.Append(new string('\t', indentLevel));
            result.Append("with (");
            result.Append(this.Scope.ScopeObjectExpression);
            result.AppendLine(")");
            result.Append(this.Body.ToString(indentLevel + 1));
            return result.ToString();
        }
    }

}