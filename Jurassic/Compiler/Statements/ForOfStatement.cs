﻿using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a javascript for-of statement.
    /// </summary>
    internal class ForOfStatement : Statement
    {
        /// <summary>
        /// Creates a new ForOfStatement instance.
        /// </summary>
        /// <param name="labels"> The labels that are associated with this statement. </param>
        public ForOfStatement(IList<string> labels)
            : base(labels)
        {
        }

        /// <summary>
        /// Gets or sets a reference to mutate on each iteration of the loop.
        /// </summary>
        public IReferenceExpression Variable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the portion of source code associated with the variable.
        /// </summary>
        public SourceCodeSpan VariableSourceSpan
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an expression that evaluates to the object to enumerate.
        /// </summary>
        public Expression TargetObject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the portion of source code associated with the target object.
        /// </summary>
        public SourceCodeSpan TargetObjectSourceSpan
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the loop body.
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
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Generate code for the start of the statement.
            var statementLocals = new StatementLocals() { NonDefaultBreakStatementBehavior = true, NonDefaultSourceSpanBehavior = true };
            GenerateStartOfStatement(generator, optimizationInfo, statementLocals);

            // Construct a loop expression.
            // var iterator = TypeUtilities.GetIterator(obj);
            // while (true) {
            //   continue-target:
            //   if (enumerator.MoveNext() == false)
            //     goto break-target;
            //   lhs = enumerator.Current;
            //
            //   <body statements>
            // }
            // break-target:

            // Call: ObjectInstance GetIterator(ScriptEngine engine, ObjectInstance iterable)
            // Then call: IEnumerable<object> Iterate(ScriptEngine engine, ObjectInstance iterator)
            optimizationInfo.MarkSequencePoint(generator, this.TargetObjectSourceSpan);
            EmitHelpers.LoadScriptEngine(generator);
            generator.Duplicate();
            this.TargetObject.GenerateCode(generator, optimizationInfo);
            EmitConversion.ToObject(generator, this.TargetObject.ResultType, optimizationInfo);
            generator.Call(ReflectionHelpers.TypeUtilities_GetIterator);
            generator.Call(ReflectionHelpers.TypeUtilities_Iterate);

            // Call IEnumerable<object>.GetEnumerator()
            generator.Call(ReflectionHelpers.IEnumerable_Object_GetEnumerator);

            // Store the enumerator in a temporary variable.
            var enumerator = generator.CreateTemporaryVariable(typeof(IEnumerator<object>));
            generator.StoreVariable(enumerator);

            var breakTarget = generator.CreateLabel();
            var continueTarget = generator.DefineLabelPosition();

#if NET40
            // Emit debugging information.
            if (optimizationInfo.DebugDocument != null)
                generator.MarkSequencePoint(optimizationInfo.DebugDocument, this.VariableSourceSpan);
#endif

            //   if (enumerator.MoveNext() == false)
            //     goto break-target;
            generator.LoadVariable(enumerator);
            generator.Call(ReflectionHelpers.IEnumerator_MoveNext);
            generator.BranchIfFalse(breakTarget);

            // lhs = enumerator.Current;
            this.Variable.GenerateReference(generator, optimizationInfo);
            generator.LoadVariable(enumerator);
            generator.Call(ReflectionHelpers.IEnumerator_Object_Current);
            this.Variable.GenerateSet(generator, optimizationInfo, PrimitiveType.Any, false);

            // Emit the body statement(s).
            optimizationInfo.PushBreakOrContinueInfo(this.Labels, breakTarget, continueTarget, labelledOnly: false);
            this.Body.GenerateCode(generator, optimizationInfo);
            optimizationInfo.PopBreakOrContinueInfo();

            generator.Branch(continueTarget);
            generator.DefineLabelPosition(breakTarget);

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
                yield return this.TargetObject;

                // Fake a string assignment to the target variable so it gets the correct type.
                var fakeAssignment = new AssignmentExpression(Operator.Assignment);
                fakeAssignment.Push((Expression)this.Variable);
                fakeAssignment.Push(new LiteralExpression(""));
                yield return fakeAssignment;

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
            result.AppendFormat("for ({0} of {1})",
                this.Variable.ToString(),
                this.TargetObject.ToString());
            result.AppendLine();
            result.Append(this.Body.ToString(indentLevel + 1));
            return result.ToString();
        }
    }

}