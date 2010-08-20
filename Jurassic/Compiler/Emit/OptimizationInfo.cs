using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents one or more code generation optimizations.
    /// </summary>
    internal enum OptimizationFlags
    {
        None = 0,

        /// <summary>
        /// Indicates that strict mode is enabled.
        /// </summary>
        StrictMode = 1,

        /// <summary>
        /// Indicates the return value from an expression is not used and therefore should not be
        /// generated.
        /// </summary>
        //SuppressReturnValue = 2,

        /// <summary>
        /// Indicates that object scope variable lookup and property access should be optimized
        /// using the hidden class as the cache key.
        /// </summary>
        EnableInlineCaching = 4,

        /// <summary>
        /// Indicates that the types of local variables should be determined, if possible.
        /// </summary>
        EnableTypeAnalysis = 8,
    }
    
    /// <summary>
    /// Represents information about one or more code generation optimizations.
    /// </summary>
    internal class OptimizationInfo
    {
        /// <summary>
        /// Creates a new OptimizationInfo instance.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        public OptimizationInfo(ScriptEngine engine)
        {
            this.Engine = engine;
        }

        /// <summary>
        /// Gets the associated script engine.
        /// </summary>
        public ScriptEngine Engine
        {
            get;
            private set;
        }

        //private OptimizationFlags flags;

        ///// <summary>
        ///// Creates a new OptimizationInfo instance.
        ///// </summary>
        ///// <param name="flags"> Determines the optimizations to perform. </param>
        //private OptimizationInfo(OptimizationFlags flags)
        //{
        //    this.flags = flags;
        //}

        /////// <summary>
        /////// If <c>true</c>, indicates the return value from an expression is not used and
        /////// therefore should not be generated.
        /////// </summary>
        ////public bool SuppressReturnValue
        ////{
        ////    get { return (this.flags & OptimizationFlags.SuppressReturnValue) != 0; }
        ////}

        ///// <summary>
        ///// If <c>true</c>, indicates that object scope variable lookup and property access should
        ///// be optimized using the hidden class as the cache key.
        ///// </summary>
        //public bool InlineCachingEnabled
        //{
        //    get { return (this.flags & OptimizationFlags.EnableInlineCaching) != 0; }
        //}

        /// <summary>
        /// Gets a value that indicates whether strict mode is enabled.
        /// </summary>
        public bool StrictMode;

        ///// <summary>
        ///// Gets a reference to an OptimizationInfo that performs no optimizations.
        ///// </summary>
        //public static OptimizationInfo Empty
        //{
        //    get { return new OptimizationInfo(OptimizationFlags.None); }
        //}

        ///// <summary>
        ///// Clones this instance and adds one or more flags.
        ///// </summary>
        ///// <param name="flags"> The flags to add. </param>
        ///// <returns> A new instance that is based on this one. </returns>
        //public OptimizationInfo AddFlags(OptimizationFlags flags)
        //{
        //    var result = this.Clone();
        //    result.flags |= flags;
        //    return result;
        //}

        ///// <summary>
        ///// Clones this instance and removes one or more flags.
        ///// </summary>
        ///// <param name="flags"> The flags to remove. </param>
        ///// <returns> A new instance that is based on this one. </returns>
        //public OptimizationInfo RemoveFlags(OptimizationFlags flags)
        //{
        //    var result = this.Clone();
        //    result.flags &= ~flags;
        //    return result;
        //}

        ///// <summary>
        ///// Clones the optimization information.
        ///// </summary>
        ///// <returns> A clone of the optimization information. </returns>
        //private OptimizationInfo Clone()
        //{
        //    return (OptimizationInfo)this.MemberwiseClone();
        //}



        //     DEBUGGING
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets or sets the symbol store to write debugging information to.
        /// </summary>
        public System.Diagnostics.SymbolStore.ISymbolDocumentWriter DebugDocument
        {
            get;
            set;
        }


        //     FUNCTION OPTIMIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets or sets function optimization information.
        /// </summary>
        public MethodOptimizationHints FunctionOptimizationInfo
        {
            get;
            set;
        }


        //     VARIABLES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets or sets the local variable to store the result of the eval() call.  Will be
        /// <c>null</c> if code is being generated outside an eval context.
        /// </summary>
        public ILLocalVariable EvalResult
        {
            get;
            set;
        }

        private Dictionary<RegularExpressionLiteral, ILLocalVariable> regularExpressionVariables;

        /// <summary>
        /// Retrieves a variable that can be used to store a shared instance of a regular
        /// expression.
        /// </summary>
        /// <param name="generator"> The IL generator used to create the variable. </param>
        /// <param name="literal"> The regular expression literal. </param>
        /// <returns> A varaible that can be used to store a shared instance of a regular
        /// expression. </returns>
        public ILLocalVariable GetRegExpVariable(ILGenerator generator, RegularExpressionLiteral literal)
        {
            if (literal == null)
                throw new ArgumentNullException("literal");

            // Create a new Dictionary if it hasn't been created before.
            if (this.regularExpressionVariables == null)
                this.regularExpressionVariables = new Dictionary<RegularExpressionLiteral, ILLocalVariable>();

            // Check if the literal already exists in the dictionary.
            ILLocalVariable variable;
            if (this.regularExpressionVariables.TryGetValue(literal, out variable) == false)
            {
                // The literal does not exist - add it.
                variable = generator.DeclareVariable(typeof(Library.RegExpInstance));
                this.regularExpressionVariables.Add(literal, variable);
            }
            return variable;
        }


        //     RETURN SUPPORT
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets or sets the label the return statement should jump to (with the return value on
        /// top of the stack).  Will be <c>null</c> if code is being generated outside a function
        /// context.
        /// </summary>
        public ILLabel ReturnTarget
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the variable that holds the return value for the function.  Will be
        /// <c>null</c> if code is being generated outside a function context.
        /// </summary>
        public ILLocalVariable ReturnVariable
        {
            get;
            set;
        }



        //     BREAK AND CONTINUE SUPPORT
        //_________________________________________________________________________________________

        private class BreakOrContinueInfo
        {
            public IList<string> LabelNames;
            public bool LabelledOnly;
            public ILLabel BreakTarget;
            public ILLabel ContinueTarget;
        }

        private Stack<BreakOrContinueInfo> breakOrContinueStack = new Stack<BreakOrContinueInfo>();

        /// <summary>
        /// Pushes information about break or continue targets to a stack.
        /// </summary>
        /// <param name="labelNames"> The label names associated with the break or continue target.
        /// Can be <c>null</c>. </param>
        /// <param name="breakTarget"> The IL label to jump to if a break statement is encountered. </param>
        /// <param name="continueTarget"> The IL label to jump to if a continue statement is
        /// encountered.  Can be <c>null</c>. </param>
        /// <param name="labelledOnly"> <c>true</c> if break or continue statements without a label
        /// should ignore this entry; <c>false</c> otherwise. </param>
        public void PushBreakOrContinueInfo(IList<string> labelNames, ILLabel breakTarget, ILLabel continueTarget, bool labelledOnly)
        {
            if (breakTarget == null)
                throw new ArgumentNullException("breakTarget");

            // Check the label doesn't already exist.
            if (labelNames != null)
            {
                foreach (var labelName in labelNames)
                    foreach (var info in this.breakOrContinueStack)
                        if (info.LabelNames != null && info.LabelNames.Contains(labelName) == true)
                            throw new JavaScriptException(this.Engine, "SyntaxError", string.Format("Label '{0}' has already been declared", labelName));
            }

            // Push the info to the stack.
            this.breakOrContinueStack.Push(new BreakOrContinueInfo()
            {
                LabelNames = labelNames,
                LabelledOnly = labelledOnly,
                BreakTarget = breakTarget,
                ContinueTarget = continueTarget
            });
        }

        /// <summary>
        /// Removes the top-most break or continue information from the stack.
        /// </summary>
        public void PopBreakOrContinueInfo()
        {
            this.breakOrContinueStack.Pop();
        }

        /// <summary>
        /// Returns the break target for the statement with the given label, if one is provided, or
        /// the top-most break target otherwise.
        /// </summary>
        /// <param name="labelName"> The label associated with the break target.  Can be
        /// <c>null</c>. </param>
        /// <returns> The break target for the statement with the given label. </returns>
        public ILLabel GetBreakTarget(string labelName = null)
        {
            if (labelName == null)
            {
                foreach (var info in this.breakOrContinueStack)
                {
                    if (info.LabelledOnly == false)
                        return info.BreakTarget;
                }
                throw new JavaScriptException(this.Engine, "SyntaxError", "Illegal break statement");
            }
            else
            {
                foreach (var info in this.breakOrContinueStack)
                {
                    if (info.LabelNames != null && info.LabelNames.Contains(labelName) == true)
                        return info.BreakTarget;
                }
                throw new JavaScriptException(this.Engine, "SyntaxError", string.Format("Undefined label '{0}'", labelName));
            }
        }

        /// <summary>
        /// Returns the continue target for the statement with the given label, if one is provided, or
        /// the top-most continue target otherwise.
        /// </summary>
        /// <param name="labelName"> The label associated with the continue target.  Can be
        /// <c>null</c>. </param>
        /// <returns> The continue target for the statement with the given label. </returns>
        public ILLabel GetContinueTarget(string labelName = null)
        {
            if (labelName == null)
            {
                foreach (var info in this.breakOrContinueStack)
                {
                    if (info.ContinueTarget != null && info.LabelledOnly == false)
                        return info.ContinueTarget;
                }
                throw new JavaScriptException(this.Engine, "SyntaxError", "Illegal continue statement");
            }
            else
            {
                foreach (var info in this.breakOrContinueStack)
                {
                    if (info.LabelNames != null && info.LabelNames.Contains(labelName) == true)
                    {
                        if (info.ContinueTarget == null)
                            throw new JavaScriptException(this.Engine, "SyntaxError", string.Format("The statement with label '{0}' is not a loop", labelName));
                        return info.ContinueTarget;
                    }
                }
                throw new JavaScriptException(this.Engine, "SyntaxError", string.Format("Undefined label '{0}'", labelName));
            }
        }
    }

}
