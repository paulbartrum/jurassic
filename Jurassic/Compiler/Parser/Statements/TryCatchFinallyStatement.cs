using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a try-catch-finally statement.
    /// </summary>
    internal class TryCatchFinallyStatement : Statement
    {
        /// <summary>
        /// Creates a new TryCatchFinallyStatement instance.
        /// </summary>
        public TryCatchFinallyStatement(IList<string> labels)
            : base(labels)
        {
        }

        /// <summary>
        /// Gets or sets the statement(s) inside the try block.
        /// </summary>
        public BlockStatement TryBlock
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the statement(s) inside the catch block.  Can be <c>null</c>.
        /// </summary>
        public BlockStatement CatchBlock
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scope of the variable to receive the exception.  Can be <c>null</c> but
        /// only if CatchStatement is also <c>null</c>.
        /// </summary>
        public Scope CatchScope
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the variable to receive the exception.  Can be <c>null</c> but
        /// only if CatchStatement is also <c>null</c>.
        /// </summary>
        public string CatchVariableName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the statement(s) inside the finally block.  Can be <c>null</c>.
        /// </summary>
        public BlockStatement FinallyBlock
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
            // Begin the exception block.
            generator.BeginExceptionBlock();

            // Generate code for the try block.
            this.TryBlock.GenerateCode(generator, optimizationInfo);

            // Generate code for the catch block.
            if (this.CatchBlock != null)
            {
                generator.BeginCatchBlock(typeof(JavaScriptException));

                // Create a new DeclarativeScope.
                this.CatchScope.GenerateScopeCreation(generator, optimizationInfo);

                // The exception is on the top of the stack.
                // Store the exception object in the variable provided.
                generator.Call(ReflectionHelpers.JavaScriptException_ErrorObject);
                var catchVariable = new NameExpression(this.CatchScope, this.CatchVariableName);
                catchVariable.GenerateSet(generator, optimizationInfo, PrimitiveType.Any, false);

                // Emit code for the statements within the catch block.
                this.CatchBlock.GenerateCode(generator, optimizationInfo);

                // Revert the scope.
                generator.LoadArgument(0);
                generator.Call(ReflectionHelpers.Scope_ParentScope);
                generator.StoreArgument(0);
            }

            // Generate code for the finally block.
            if (this.FinallyBlock != null)
            {
                generator.BeginFinallyBlock();
                this.FinallyBlock.GenerateCode(generator, optimizationInfo);
            }

            // End the exception block.
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
            result.AppendLine("try");
            result.AppendLine(this.TryBlock.ToString(indentLevel + 1));
            if (this.CatchBlock != null)
            {
                result.Append(new string('\t', indentLevel));
                result.Append("catch (");
                result.Append(this.CatchVariableName);
                result.AppendLine(")");
                result.AppendLine(this.CatchBlock.ToString(indentLevel + 1));
            }
            if (this.FinallyBlock != null)
            {
                result.Append(new string('\t', indentLevel));
                result.AppendLine("finally");
                result.AppendLine(this.FinallyBlock.ToString(indentLevel + 1));
            }
            return result.ToString();
        }
    }

}