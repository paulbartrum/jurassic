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
            // Create a temporary variable to store the exception and set it to null.
            var exception = generator.CreateTemporaryVariable(typeof(JavaScriptException));
            generator.LoadNull();
            generator.StoreVariable(exception);

            // Begin the exception block.
            generator.BeginExceptionBlock();

            // Generate code for the try block.
            this.TryBlock.GenerateCode(generator, optimizationInfo);

            // Generate a catch block, but do not generate any user code inside the catch block!
            // This is because code inside a catch block has numerous restrictions that javascript
            // does not have.  For example, in javascript a return statement is allowed within
            // catch and finally blocks, but the CLR does not allow this.  Therefore we have to
            // simulate the catch and finally blocks.

            // Store the exception in the temporary variable.
            // The exception is on the top of the stack.
            generator.BeginCatchBlock(typeof(JavaScriptException));
            generator.StoreVariable(exception);
            generator.EndExceptionBlock();

            // Generate code for the catch block.
            if (this.CatchBlock != null)
            {
                // if (exception != null)
                var endOfCatch = generator.CreateLabel();
                generator.LoadVariable(exception);
                generator.LoadNull();
                generator.BranchIfEqual(endOfCatch);

                // Create a new DeclarativeScope.
                this.CatchScope.GenerateScopeCreation(generator, optimizationInfo);

                // Make sure the scope is reverted even if an exception is thrown.
                generator.BeginExceptionBlock();

                // Store the error object in the variable provided.
                generator.LoadVariable(exception);
                generator.Call(ReflectionHelpers.JavaScriptException_ErrorObject);
                var catchVariable = new NameExpression(this.CatchScope, this.CatchVariableName);
                catchVariable.GenerateSet(generator, optimizationInfo, PrimitiveType.Any, false);

                // Emit code for the statements within the catch block.
                this.CatchBlock.GenerateCode(generator, optimizationInfo);

                // Revert the scope.
                generator.BeginFinallyBlock();
                EmitHelpers.LoadScope(generator);
                generator.Call(ReflectionHelpers.Scope_ParentScope);
                EmitHelpers.StoreScope(generator);
                generator.EndExceptionBlock();

                // Branch here if no exception was thrown.
                generator.DefineLabelPosition(endOfCatch);
            }

            // Generate code for the finally block.
            if (this.FinallyBlock != null)
            {
                this.FinallyBlock.GenerateCode(generator, optimizationInfo);
            }

            // Generate code to rethrow the exception if no catch block was present.
            if (this.CatchBlock == null)
            {
                // if (exception != null)
                var endOfCatch2 = generator.CreateLabel();
                generator.LoadVariable(exception);
                generator.LoadNull();
                generator.BranchIfEqual(endOfCatch2);

                // Rethrow the exception.
                generator.LoadVariable(exception);
                generator.Throw();

                // Branch here if no exception was thrown.
                generator.DefineLabelPosition(endOfCatch2);
            }

            // We no longer need the temporary variable.
            generator.ReleaseTemporaryVariable(exception);
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