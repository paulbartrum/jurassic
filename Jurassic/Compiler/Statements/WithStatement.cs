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

        public Expression WithObject { get; set; }

        /// <summary>
        /// Gets or sets the body of the with statement.
        /// </summary>
        public Statement Body { get; set; }

        /// <summary>
        /// Generates CIL for the statement.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Generate code for the start of the statement.
            var statementLocals = new StatementLocals();
            GenerateStartOfStatement(generator, optimizationInfo, statementLocals);

            // executionContext.StartWithBlock(ObjectInstance withObject)
            EmitHelpers.LoadExecutionContext(generator);
            this.WithObject.GenerateCode(generator, optimizationInfo);
            EmitConversion.ToObject(generator, this.WithObject.ResultType, optimizationInfo);
            generator.Call(ReflectionHelpers.ExecutionContext_StartWithBlock);

            // Store the result of the previous call into a temporary variable.
            var previousScope = generator.CreateTemporaryVariable(typeof(RuntimeScope));
            generator.StoreVariable(previousScope);

            // Make sure the scope is reverted even if an exception is thrown.
            generator.BeginExceptionBlock();

            // Setting the InsideTryCatchOrFinally flag converts BR instructions into LEAVE
            // instructions so that the finally block is executed correctly.
            var previousInsideTryCatchOrFinally = optimizationInfo.InsideTryCatchOrFinally;
            optimizationInfo.InsideTryCatchOrFinally = true;

            // Generate code for the body statements.
            this.Body.GenerateCode(generator, optimizationInfo);

            // Reset the InsideTryCatchOrFinally flag.
            optimizationInfo.InsideTryCatchOrFinally = previousInsideTryCatchOrFinally;

            // finally { executionContext.EndWithBlock(previousScope) }
            generator.BeginFinallyBlock();
            EmitHelpers.LoadExecutionContext(generator);
            generator.LoadVariable(previousScope);
            generator.Call(ReflectionHelpers.ExecutionContext_EndWithBlock);
            generator.EndExceptionBlock();

            // Generate code for the end of the statement.
            GenerateEndOfStatement(generator, optimizationInfo, statementLocals);
        }

        /// <summary>
        /// Gets an enumerable list of child nodes in the abstract syntax tree.
        /// </summary>
        public override IEnumerable<AstNode> ChildNodes
        {
            get
            {
                yield return this.WithObject;
                yield return this.Body;
            }
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
            result.Append(this.WithObject);
            result.AppendLine(")");
            result.Append(this.Body.ToString(indentLevel + 1));
            return result.ToString();
        }
    }

}