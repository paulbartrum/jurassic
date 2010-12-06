using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a javascript loop statement (for, for-in, while and do-while).
    /// </summary>
    internal abstract class LoopStatement : Statement
    {
        /// <summary>
        /// Creates a new LoopStatement instance.
        /// </summary>
        /// <param name="labels"> The labels that are associated with this statement. </param>
        public LoopStatement(IList<string> labels)
            : base(labels)
        {
        }

        /// <summary>
        /// Gets or sets the statement that initializes the loop variable.
        /// </summary>
        public Statement InitStatement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the var statement that initializes the loop variable.
        /// </summary>
        public VarStatement InitVarStatement
        {
            get { return this.InitStatement as VarStatement; }
        }

        /// <summary>
        /// Gets the expression that initializes the loop variable.
        /// </summary>
        public Expression InitExpression
        {
            get { return (this.InitStatement is ExpressionStatement) == true ? ((ExpressionStatement)this.InitStatement).Expression : null; }
        }

        /// <summary>
        /// Gets or sets the statement that checks whether the loop should terminate.
        /// </summary>
        public ExpressionStatement ConditionStatement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the expression that checks whether the loop should terminate.
        /// </summary>
        public Expression Condition
        {
            get { return this.ConditionStatement.Expression; }
        }

        /// <summary>
        /// Gets or sets the statement that increments (or decrements) the loop variable.
        /// </summary>
        public ExpressionStatement IncrementStatement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the expression that increments (or decrements) the loop variable.
        /// </summary>
        public Expression Increment
        {
            get { return this.IncrementStatement.Expression; }
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
        /// Gets a value that indicates whether the condition should be checked at the end of the
        /// loop.
        /// </summary>
        protected virtual bool CheckConditionAtEnd
        {
            get { return false; }
        }

        //private Dictionary<NameExpression, PrimitiveType> typeHints;

        ///// <summary>
        ///// Optimizes the expression tree.
        ///// </summary>
        //public override void Optimize()
        //{
        //    // Find the types of all the references inside the loop.
        //    this.typeHints = new Dictionary<NameExpression, PrimitiveType>();
        //    FindTypeHints(this.Body);

        //    // Special case for the common case of an incrementing or decrementing loop variable.
        //    // The loop must be one of the following forms:
        //    //     for (var i = <int>; i < <int>; i ++)
        //    //     for (var i = <int>; i < <int>; ++ i)
        //    //     for (var i = <int>; i <= <int>; i ++)
        //    //     for (var i = <int>; i <= <int>; ++ i)
        //    //     for (i = <int>; i < <int>; i ++)
        //    //     for (i = <int>; i < <int>; ++ i)
        //    //     for (i = <int>; i < <int>; i ++)
        //    //     for (i = <int>; i < <int>; ++ i)
        //    // Additionally, the loop variable must not be modified inside the loop body.
        //    NameExpression loopVariable = null;
        //    bool loopVariableIsInteger = false;
        //    if (this.Increment != null && this.Increment is AssignmentExpression)
        //        loopVariable = ((AssignmentExpression)this.Increment).Target as NameExpression;
        //    if (loopVariable != null && loopVariable.Scope is DeclarativeScope && this.typeHints.ContainsKey(loopVariable) == false)
        //    {
        //        bool initIsOkay = false;
        //        if (this.InitExpression != null)
        //        {
        //            initIsOkay = this.InitExpression is AssignmentExpression &&
        //                object.Equals(((AssignmentExpression)InitExpression).Target, loopVariable) == true &&
        //                this.InitExpression.ResultType == PrimitiveType.Int32;
        //        }
        //        else if (this.InitVarStatement != null)
        //        {
        //            initIsOkay = this.InitVarStatement.Declarations.Count == 1 &&
        //                object.Equals(new NameExpression(this.InitVarStatement.Scope, this.InitVarStatement.Declarations[0].VariableName), loopVariable) == true &&
        //                this.InitVarStatement.Declarations[0].InitExpression.ResultType == PrimitiveType.Int32;
        //        }
        //        bool conditionIsOkay = this.Condition is BinaryExpression &&
        //            (((BinaryExpression)this.Condition).Operator.Type == OperatorType.LessThan ||
        //            ((BinaryExpression)this.Condition).Operator.Type == OperatorType.LessThanOrEqual) &&
        //            object.Equals(((AssignmentExpression)this.Increment).Target, loopVariable) == true;
        //        bool incrementIsOkay = this.Increment is AssignmentExpression &&
        //            (((AssignmentExpression)this.Increment).Operator.Type == OperatorType.PostIncrement ||
        //            ((AssignmentExpression)this.Increment).Operator.Type == OperatorType.PreIncrement);
        //        loopVariableIsInteger = initIsOkay && conditionIsOkay && incrementIsOkay;
        //    }

        //    if (loopVariableIsInteger == true)
        //    {
        //        // The loop variable is an integer.
        //        this.typeHints.Add(loopVariable, PrimitiveType.Int32);
        //    }
        //    else
        //    {
        //        // Find the rest of the references inside the loop.
        //        FindTypeHints(this.ConditionStatement);
        //        FindTypeHints(this.IncrementStatement);
        //    }

        //    // Optimize the child statements.
        //    base.Optimize();
        //}

        //private void FindTypeHints(Statement statementToSearch)
        //{
        //    statementToSearch.Visit(statement =>
        //    {
        //        if (statement is ExpressionStatement)
        //            ((ExpressionStatement)statement).Expression.Visit(expression =>
        //                {
        //                    if (expression is AssignmentExpression)
        //                    {
        //                        var target = ((AssignmentExpression)expression).Target;
        //                        if (target is NameExpression && ((NameExpression)target).Scope is DeclarativeScope)
        //                            AddTypeHint((NameExpression)target, ((AssignmentExpression)expression).ResultType);
        //                    }
        //                });
        //    });
        //}

        /// <summary>
        /// Generates CIL for the statement.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Generate code for the start of the statement.
            var statementLocals = new StatementLocals() { NonDefaultBreakStatementBehavior = true, NonDefaultDebugInfoBehavior = true };
            GenerateStartOfStatement(generator, optimizationInfo, statementLocals);

            // <initializer>
            // if (<condition>)
            // {
            // 	 <loop body>
            //   <increment>
            //   while (true) {
            //     if (<condition> == false)
            //       break;
            //
            //     <body statements>
            //
            //     continue-target:
            //     <increment>
            //   }
            // }
            // break-target:

            // Set up some labels.
            var continueTarget = generator.CreateLabel();
            var breakTarget1 = generator.CreateLabel();
            var breakTarget2 = generator.CreateLabel();

            // Emit the initialization statement.
            if (this.InitStatement != null)
                this.InitStatement.GenerateCode(generator, optimizationInfo);

            // Check the condition and jump to the end if it is false.
            if (this.CheckConditionAtEnd == false && this.ConditionStatement != null)
            {
                if (optimizationInfo.DebugDocument != null)
                    generator.MarkSequencePoint(optimizationInfo.DebugDocument, this.ConditionStatement.DebugInfo);
                this.Condition.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToBool(generator, this.Condition.ResultType);
                generator.BranchIfFalse(breakTarget1);
            }

            // Emit the loop body.
            optimizationInfo.PushBreakOrContinueInfo(this.Labels, breakTarget1, continueTarget, false);
            this.Body.GenerateCode(generator, optimizationInfo);
            optimizationInfo.PopBreakOrContinueInfo();

            // Increment the loop variable.
            if (this.IncrementStatement != null)
                this.IncrementStatement.GenerateCode(generator, optimizationInfo);

            // Strengthen the variable types.
            List<KeyValuePair<Scope.DeclaredVariable, PrimitiveType>> previousVariableTypes = null;
            if (optimizationInfo.OptimizeDeclarativeScopes == true)
            {
                // Keep a record of the variable types before strengthening.
                previousVariableTypes = new List<KeyValuePair<Scope.DeclaredVariable, PrimitiveType>>();

                var typedVariables = FindTypedVariables();
                foreach (var variableAndType in typedVariables)
                {
                    var variable = variableAndType.Key;
                    var variableType = variableAndType.Value;
                    if (variableType != variable.Type)
                    {
                        // Save the previous type so we can restore it later.
                        var previousType = variable.Type;
                        previousVariableTypes.Add(new KeyValuePair<Scope.DeclaredVariable, PrimitiveType>(variable, previousType));

                        // Load the existing value.
                        var nameExpression = new NameExpression(variable.Scope, variable.Name);
                        nameExpression.GenerateGet(generator, optimizationInfo, false);

                        // Store the typed value.
                        variable.Store = generator.DeclareVariable(variableType);
                        variable.Type = variableType;
                        nameExpression.GenerateSet(generator, optimizationInfo, previousType, false);
                    }
                }

                // The variables must be reverted even in the presence of exceptions.
                if (previousVariableTypes.Count > 0)
                    generator.BeginExceptionBlock();
            }

            // The inner loop starts here.
            var startOfLoop = generator.DefineLabelPosition();

            // Check the condition and jump to the end if it is false.
            if (this.ConditionStatement != null)
            {
                if (optimizationInfo.DebugDocument != null)
                    generator.MarkSequencePoint(optimizationInfo.DebugDocument, this.ConditionStatement.DebugInfo);
                this.Condition.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToBool(generator, this.Condition.ResultType);
                generator.BranchIfFalse(breakTarget2);
            }

            // Emit the loop body.
            optimizationInfo.PushBreakOrContinueInfo(this.Labels, breakTarget2, continueTarget, labelledOnly: false);
            this.Body.GenerateCode(generator, optimizationInfo);
            optimizationInfo.PopBreakOrContinueInfo();

            // The continue statement jumps here.
            generator.DefineLabelPosition(continueTarget);

            // Increment the loop variable.
            if (this.IncrementStatement != null)
                this.IncrementStatement.GenerateCode(generator, optimizationInfo);

            // Unconditionally branch back to the start of the loop.
            generator.Branch(startOfLoop);

            // Define the end of the loop (actually just after).
            generator.DefineLabelPosition(breakTarget2);

            // Revert the variable types.
            if (previousVariableTypes != null && previousVariableTypes.Count > 0)
            {
                // Revert the variable types within a finally block.
                generator.BeginFinallyBlock();

                foreach (var previousVariableAndType in previousVariableTypes)
                {
                    var variable = previousVariableAndType.Key;
                    var variableType = previousVariableAndType.Value;

                    // Load the existing value.
                    var nameExpression = new NameExpression(variable.Scope, variable.Name);
                    nameExpression.GenerateGet(generator, optimizationInfo, false);

                    // Store the typed value.
                    var previousType = variable.Type;
                    variable.Store = generator.DeclareVariable(variableType);
                    variable.Type = variableType;
                    nameExpression.GenerateSet(generator, optimizationInfo, previousType, false);
                }

                // End the exception block.
                generator.EndExceptionBlock();
            }

            // Define the end of the loop (actually just after).
            generator.DefineLabelPosition(breakTarget1);

            // Generate code for the end of the statement.
            GenerateEndOfStatement(generator, optimizationInfo, statementLocals);
        }


        /// <summary>
        /// Finds variables that were assigned to and determines their types.
        /// </summary>
        /// <param name="root"> The root of the abstract syntax tree to search. </param>
        /// <param name="variableTypes"> A dictionary containing the variables that were assigned to. </param>
        private Dictionary<Scope.DeclaredVariable, PrimitiveType> FindTypedVariables()
        {
            var result = new Dictionary<Scope.DeclaredVariable,PrimitiveType>();

            // Special case for the common case of an incrementing or decrementing loop variable.
            // The loop must be one of the following forms:
            //     for (var i = <int>; i < <int>; i ++)
            //     for (var i = <int>; i < <int>; ++ i)
            //     for (var i = <int>; i > <int>; i --)
            //     for (var i = <int>; i > <int>; -- i)
            //     for (i = <int>; i < <int>; i ++)
            //     for (i = <int>; i < <int>; ++ i)
            //     for (i = <int>; i > <int>; i --)
            //     for (i = <int>; i > <int>; -- i)
            Scope loopVariableScope = null;
            string loopVariableName = null;

            // First, check the init statement.
            bool initIsOkay = false;
            if (this.InitVarStatement != null &&
                this.InitVarStatement.Declarations.Count == 1 &&
                this.InitVarStatement.Declarations[0].InitExpression != null &&
                this.InitVarStatement.Declarations[0].InitExpression.ResultType == PrimitiveType.Int32)
            {
                // for (var i = <int>; ?; ?)
                loopVariableScope = this.InitVarStatement.Scope;
                loopVariableName = this.InitVarStatement.Declarations[0].VariableName;
                initIsOkay = true;
            }
            if (this.InitExpression != null &&
                this.InitExpression is AssignmentExpression &&
                ((AssignmentExpression)this.InitExpression).ResultType == PrimitiveType.Int32 &&
                ((AssignmentExpression)this.InitExpression).Target is NameExpression)
            {
                // for (i = <int>; ?; ?)
                loopVariableScope = ((NameExpression)((AssignmentExpression)this.InitExpression).Target).Scope;
                loopVariableName = ((NameExpression)((AssignmentExpression)this.InitExpression).Target).Name;
                initIsOkay = true;
            }

            // Next, check the condition expression.
            bool conditionIsOkay = false;
            bool lessThan = true;
            if (initIsOkay == true &&
                this.ConditionStatement != null &&
                this.Condition is BinaryExpression &&
                ((BinaryExpression)this.Condition).OperatorType == OperatorType.LessThan &&
                ((BinaryExpression)this.Condition).Left is NameExpression &&
                ((NameExpression)((BinaryExpression)this.Condition).Left).Name == loopVariableName &&
                ((BinaryExpression)this.Condition).Right.ResultType == PrimitiveType.Int32)
            {
                // for (?; i < <int>; ?)
                lessThan = true;
                conditionIsOkay = true;
            }
            if (initIsOkay == true &&
                this.ConditionStatement != null &&
                this.Condition is BinaryExpression &&
                ((BinaryExpression)this.Condition).OperatorType == OperatorType.GreaterThan &&
                ((BinaryExpression)this.Condition).Left is NameExpression &&
                ((NameExpression)((BinaryExpression)this.Condition).Left).Name == loopVariableName &&
                ((BinaryExpression)this.Condition).Right.ResultType == PrimitiveType.Int32)
            {
                // for (?; i > <int>; ?)
                lessThan = false;
                conditionIsOkay = true;
            }

            // Next, check the increment expression.
            bool incrementIsOkay = false;
            if (conditionIsOkay == true &&
                lessThan == true &&
                this.IncrementStatement != null &&
                this.Increment is AssignmentExpression &&
                (((AssignmentExpression)this.Increment).OperatorType == OperatorType.PostIncrement ||
                ((AssignmentExpression)this.Increment).OperatorType == OperatorType.PreIncrement) &&
                ((NameExpression)((AssignmentExpression)this.Increment).Target).Name == loopVariableName)
            {
                // for (?; i < <int>; i ++)
                // for (?; i < <int>; ++ i)
                incrementIsOkay = true;
            }
            if (conditionIsOkay == true &&
                lessThan == false &&
                this.IncrementStatement != null &&
                this.Increment is AssignmentExpression &&
                (((AssignmentExpression)this.Increment).OperatorType == OperatorType.PostDecrement ||
                ((AssignmentExpression)this.Increment).OperatorType == OperatorType.PreDecrement) &&
                ((NameExpression)((AssignmentExpression)this.Increment).Target).Name == loopVariableName)
            {
                // for (?; i > <int>; i --)
                // for (?; i > <int>; -- i)
                incrementIsOkay = true;
            }

            if (incrementIsOkay == true)
            {
                // The loop variable can be optimized to an integer.
                var variable = loopVariableScope.GetDeclaredVariable(loopVariableName);
                if (variable != null)
                    result.Add(variable, PrimitiveType.Int32);
                FindTypedVariables(this.Body, result);
            }
            else
            {
                // Unoptimized.
                FindTypedVariables(this, result);
            }

            return result;
        }

        /// <summary>
        /// Finds variables that were assigned to and determines their types.
        /// </summary>
        /// <param name="root"> The root of the abstract syntax tree to search. </param>
        /// <param name="variableTypes"> A dictionary containing the variables that were assigned to. </param>
        private static void FindTypedVariables(AstNode root, Dictionary<Scope.DeclaredVariable, PrimitiveType> variableTypes)
        {
            if (root is AssignmentExpression)
            {
                // Found an assignment.
                var assignment = (AssignmentExpression)root;
                if (assignment.Target is NameExpression)
                {
                    // Found an assignment to a variable.
                    var name = (NameExpression)assignment.Target;
                    var variable = name.Scope.GetDeclaredVariable(name.Name);
                    if (variable != null)
                    {
                        // The variable is in the top-most scope.
                        // Check if the variable has been seen before.
                        PrimitiveType existingType;
                        if (variableTypes.TryGetValue(variable, out existingType) == false)
                        {
                            // This is the first time the variable has been encountered.
                            variableTypes.Add(variable, assignment.ResultType);
                        }
                        else
                        {
                            // The variable has been seen before.
                            variableTypes[variable] = PrimitiveTypeUtilities.GetCommonType(existingType, assignment.ResultType);
                        }
                    }
                }
            }
            else
            {
                // Search child nodes.
                foreach (var node in root.ChildNodes)
                    FindTypedVariables(node, variableTypes);
            }
        }

        /// <summary>
        /// Gets an enumerable list of child nodes in the abstract syntax tree.
        /// </summary>
        public override IEnumerable<AstNode> ChildNodes
        {
            get
            {
                if (this.InitStatement != null)
                    yield return this.InitStatement;
                if (this.CheckConditionAtEnd == false && this.ConditionStatement != null)
                    yield return this.ConditionStatement;
                yield return this.Body;
                if (this.IncrementStatement != null)
                    yield return this.IncrementStatement;
                if (this.CheckConditionAtEnd == true && this.ConditionStatement != null)
                    yield return this.ConditionStatement;
            }
        }
    }

}