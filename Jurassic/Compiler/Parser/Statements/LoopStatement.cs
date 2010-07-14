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
            get { return (this.InitStatement is ExpressionStatement) == false ? null : ((ExpressionStatement)this.InitStatement).Expression; }
        }

        /// <summary>
        /// Gets or sets the expression that checks whether the loop should terminate.
        /// </summary>
        public Expression Condition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the expression that increments (or decrements) the loop variable.
        /// </summary>
        public Expression Increment
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

        ///// <summary>
        ///// Adds information about a reference inside the loop.
        ///// </summary>
        ///// <param name="reference"> The reference. </param>
        ///// <param name="typeHint"> The current type for the reference. </param>
        //private void AddTypeHint(NameExpression reference, PrimitiveType typeHint)
        //{
        //    PrimitiveType existingHint;
        //    if (this.typeHints.TryGetValue(reference, out existingHint) == false)
        //    {
        //        this.typeHints.Add(reference, typeHint);
        //    }
        //    else
        //    {
        //        if (existingHint == typeHint)
        //            return;
        //        if (PrimitiveTypeUtilities.IsNumeric(existingHint) == true && PrimitiveTypeUtilities.IsNumeric(existingHint) == true)
        //            this.typeHints[reference] = PrimitiveType.Number;
        //        else
        //            this.typeHints[reference] = PrimitiveType.Any;
        //    }
        //}

        /// <summary>
        /// Generates CIL for the statement.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
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
            if (this.CheckConditionAtEnd == false && this.Condition != null)
            {
                this.Condition.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToBool(generator, this.Condition.ResultType);
                generator.BranchIfFalse(breakTarget1);
            }

            // Emit the loop body.
            optimizationInfo.PushBreakOrContinueInfo(this.Labels, breakTarget1, continueTarget, false);
            this.Body.GenerateCode(generator, optimizationInfo);
            optimizationInfo.PopBreakOrContinueInfo();

            // Increment the loop variable.
            if (this.Increment != null)
                this.Increment.GenerateCode(generator, optimizationInfo.AddFlags(OptimizationFlags.SuppressReturnValue));

            // Strengthen the variable types.
            //List<Tuple<VariableInfo, VariableInfo>> revertVariableInfo = null;
            //if (this.typeHints != null)
            //{
            //    revertVariableInfo = new List<Tuple<VariableInfo, VariableInfo>>();
            //    foreach (var referenceAndTypeHint in typeHints)
            //    {
            //        var reference = referenceAndTypeHint.Key;
            //        var typeHint = referenceAndTypeHint.Value;

            //        var variableInfo = reference.Scope.GetVariableInfo(reference.Name);
            //        if (variableInfo != null)
            //        {
            //            // Make sure we can revert the type information afterwards.
            //            revertVariableInfo.Add(Tuple.Create(variableInfo, variableInfo.Clone()));

            //            // Create a new variable and store the value in it.
            //            reference.GenerateGet(generator, optimizationInfo);
            //            var local = generator.DeclareVariable(PrimitiveTypeUtilities.ToType(typeHint));
            //            EmitConversion.Convert(generator, variableInfo.Type, typeHint);
            //            generator.StoreVariable(local);

            //            // Alter the type information in the scope.
            //            variableInfo.Type = typeHint;
            //            variableInfo.ILVariable = local;
            //        }
            //    }
            //}

            // The inner loop starts here.
            var startOfLoop = generator.DefineLabelPosition();

            // Check the condition and jump to the end if it is false.
            if (this.Condition != null)
            {
                this.Condition.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToBool(generator, this.Condition.ResultType);
                generator.BranchIfFalse(breakTarget2);
            }

            // Emit the loop body.
            optimizationInfo.PushBreakOrContinueInfo(this.Labels, breakTarget2, continueTarget, false);
            this.Body.GenerateCode(generator, optimizationInfo);
            optimizationInfo.PopBreakOrContinueInfo();

            // The continue statement jumps here.
            generator.DefineLabelPosition(continueTarget);

            // Increment the loop variable.
            if (this.Increment != null)
                this.Increment.GenerateCode(generator, optimizationInfo.AddFlags(OptimizationFlags.SuppressReturnValue));

            // Unconditionally branch back to the start of the loop.
            generator.Branch(startOfLoop);

            // Define the end of the loop (actually just after).
            generator.DefineLabelPosition(breakTarget2);

            // Revert the variable types.
            //if (revertVariableInfo != null)
            //{
            //    foreach (var currentAndPrevious in revertVariableInfo)
            //    {
            //        var current = currentAndPrevious.Item1;
            //        var previous = currentAndPrevious.Item2;
            //        current.Type = previous.Type;
            //        current.ILVariable = previous.ILVariable;
            //    }
            //}

            // Define the end of the loop (actually just after).
            generator.DefineLabelPosition(breakTarget1);
        }
    }

}