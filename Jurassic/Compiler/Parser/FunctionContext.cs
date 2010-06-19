using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Jurassic.Library;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the variable binding state of the parser.
    /// </summary>
    internal class FunctionContext
    {
        private Parser parser;
        private Dictionary<string, Expression> localVariables;

        public FunctionContext(Parser parser, ScriptContext scriptContext)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            this.parser = parser;
            this.ScriptContext = scriptContext;
            var scopeParameter = Expression.Parameter(typeof(LexicalScope), this.ScriptContext == ScriptContext.Function ? "functionScope" : "scope");
            this.LexicalEnvironment = scopeParameter;
            this.VariableEnvironment = scopeParameter;
            this.localVariables = new Dictionary<string, Expression>();
            this.ReturnValue = Expression.Variable(typeof(object), "returnValue");
            this.EndOfProgram = Expression.Label(typeof(object), "end-of-program");
        }

        /// <summary>
        /// Gets the context the code will run in.
        /// </summary>
        public ScriptContext ScriptContext
        {
            get;
            private set;
        }

        /// <summary>
        /// A variable that holds the top-level LexicalScope.
        /// </summary>
        public ParameterExpression VariableEnvironment
        {
            get;
            private set;
        }

        /// <summary>
        /// A variable that holds the current LexicalScope.  Affected by the with and catch
        /// statements.
        /// </summary>
        public ParameterExpression LexicalEnvironment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of local variable names for the function and their initial values.
        /// </summary>
        public ICollection<KeyValuePair<string, Expression>> LocalVariables
        {
            get { return this.localVariables; }
        }

        /// <summary>
        /// Adds a local variable declaration.
        /// </summary>
        /// <param name="name"> The name of the variable. </param>
        /// <param name="initialValue"> The initial value of the variable, at the top of the
        /// function.  <c>null</c> is the default, which corresponds to an initial value of
        /// <c>undefined</c>. </param>
        public void AddVariable(string name, Expression initialValue = null)
        {
            // A var statement with the same name as a function declaration is ignored.
            if (initialValue == null && this.localVariables.ContainsKey(name) == true)
                return;

            // Record the variable name and initial value.
            this.localVariables[name] = initialValue;
        }

        /// <summary>
        /// Gets or sets a variable that holds the return value for the program.
        /// </summary>
        public ParameterExpression ReturnValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a label that is placed at the end of the function.
        /// </summary>
        public LabelTarget EndOfProgram
        {
            get;
            private set;
        }
    }

}