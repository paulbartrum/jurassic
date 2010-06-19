using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Jurassic.Library;

namespace Jurassic.Compiler
{

    internal enum ScriptContext
    {
        Global,
        Function,
        Eval,
    }

    /// <summary>
    /// Converts a series of tokens into an abstract syntax tree.
    /// </summary>
    internal sealed class Parser
    {
        private Lexer lexer;
        private Token nextToken;
        private bool consumedLineTerminator;
        private ParserContext context;
        private FunctionContext executionContext;
        private StatementContextStack statementContextStack = new StatementContextStack();
        private SymbolDocumentInfo document;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a Parser instance with the given lexer supplying the tokens.
        /// </summary>
        /// <param name="lexer"> The lexical analyser that provides the tokens. </param>
        /// <param name="scriptContext"> The context the code will run in. </param>
        public Parser(Lexer lexer, ScriptContext scriptContext)
        {
            if (lexer == null)
                throw new ArgumentNullException("lexer");
            this.lexer = lexer;
            this.lexer.ParserContextCallback = () => this.context;
            this.executionContext = new FunctionContext(this, scriptContext);
            this.document = Expression.SymbolDocument(this.lexer.SourcePath);
            this.Consume();
        }



        //     PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the line number of the next token.
        /// </summary>
        public int LineNumber
        {
            get { return this.lexer.LineNumber; }
        }

        /// <summary>
        /// Gets the path or URL of the source file.  Can be <c>null</c>.
        /// </summary>
        public string SourcePath
        {
            get { return this.lexer.SourcePath; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the parser is operating in strict mode.
        /// </summary>
        public bool StrictMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the javascript source can be debugged.
        /// Setting this to <c>true</c> disables optimizations and negatively impacts memory usage.
        /// </summary>
        public bool EnableDebugging
        {
            get;
            set;
        }


        //     VARIABLES
        //_________________________________________________________________________________________

        /// <summary>
        /// Adds a local variable declaration.
        /// </summary>
        /// <param name="name"> The name of the variable. </param>
        /// <param name="initialValue"> The initial value of the variable, at the top of the
        /// function.  <c>null</c> is the default, which corresponds to an initial value of
        /// <c>undefined</c>. </param>
        private void AddVariable(string name, Expression initialValue = null)
        {
            // Check the variable name is valid.
            this.ValidateVariableName(name);

            // Add the variable to the function context.
            this.executionContext.AddVariable(name, initialValue);
        }
                    
        /// <summary>
        /// Throws an exception if the variable name is invalid.
        /// </summary>
        /// <param name="name"> The name of the variable to check. </param>
        private void ValidateVariableName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be null or empty.", "name");

            // In strict mode, the variable name cannot be "eval" or "arguments".
            if (this.StrictMode == true && (name == "eval" || name == "arguments"))
                throw new JavaScriptException("SyntaxError", string.Format("The variable name cannot be '{0}' in strict mode.", name), this.LineNumber, this.SourcePath);
        }



        //     TOKEN HELPERS
        //_________________________________________________________________________________________

        /// <summary>
        /// Discards the current token and reads the next one.
        /// </summary>
        /// <param name="context"> Indicates whether the next token can be a literal or an
        /// operator. </param>
        private void Consume(ParserContext context = ParserContext.LiteralContext)
        {
            this.context = context;
            this.consumedLineTerminator = false;
            while (true)
            {
                this.nextToken = this.lexer.NextToken();
                if ((this.nextToken is WhiteSpaceToken) == false)
                    break;
                this.consumedLineTerminator = true;
            }
        }

        /// <summary>
        /// Indicates that the next token is identical to the given one.  Throws an exception if
        /// this is not the case.  Consumes the token.
        /// </summary>
        /// <param name="token"> The expected token. </param>
        private void Expect(Token token)
        {
            if (this.nextToken == token)
                Consume();
            else
                throw new JavaScriptException("SyntaxError", string.Format("Expected '{0}'", token.Text), this.LineNumber, this.SourcePath);
        }

        /// <summary>
        /// Indicates that the next token should be an identifier.  Throws an exception if this is
        /// not the case.  Consumes the token.
        /// </summary>
        /// <returns> The identifier name. </returns>
        private string ExpectIdentifier()
        {
            var token = this.nextToken;
            if (token is IdentifierToken)
            {
                Consume();
                return ((IdentifierToken)token).Name;
            }
            else
            {
                throw new JavaScriptException("SyntaxError", "Expected identifier", this.LineNumber, this.SourcePath);
            }
        }

        private bool AtValidEndOfStatement()
        {
            // A statement can be terminator in four ways: by a semi-colon (;), by a right brace (}),
            // by the end of a line or by the end of the program.
            return this.nextToken == PunctuatorToken.Semicolon ||
                this.nextToken == PunctuatorToken.RightBrace ||
                this.consumedLineTerminator == true ||
                this.nextToken == null;
        }

        private void ExpectEndOfStatement()
        {
            if (this.nextToken == PunctuatorToken.Semicolon)
                Consume();
            else
            {
                // Automatic semi-colon insertion.
                // If an illegal token is found then a semicolon is automatically inserted before
                // the offending token if one or more of the following conditions is true: 
                // 1. The offending token is separated from the previous token by at least one LineTerminator.
                // 2. The offending token is '}'.
                if (this.consumedLineTerminator == true || this.nextToken == PunctuatorToken.RightBrace)
                    return;

                // If the end of the input stream of tokens is encountered and the parser is unable
                // to parse the input token stream as a single complete ECMAScript Program, then a
                // semicolon is automatically inserted at the end of the input stream.
                if (this.nextToken == null)
                    return;

                // Otherwise, throw an error.
                throw new JavaScriptException("SyntaxError", "Expected ';'", this.LineNumber, this.SourcePath);
            }
        }



        //     PARSE METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Parses javascript source code.
        /// </summary>
        /// <returns> An expression that can be executed to run the program represented by the
        /// source code. </returns>
        public Expression Parse()
        {
            // Read zero or more statements.
            var statements = new List<Expression>();
            while (this.nextToken != null)
            {
                // Check if we are at the end of a function.
                if (this.executionContext.ScriptContext == ScriptContext.Function && this.nextToken == PunctuatorToken.RightBrace)
                    break;

                // Parse a single statement.
                statements.Add(ParseSourceStatement());
            }

            // Bind any local variables at the top of the function/global code.
            var initializationStatements = new List<Expression>(this.executionContext.LocalVariables.Count + 2);
            initializationStatements.Add(Expression.DebugInfo(this.document, 1, 1, 1, int.MaxValue));
            
            foreach (KeyValuePair<string, Expression> variableNameAndValue in this.executionContext.LocalVariables)
            {
                string variableName = variableNameAndValue.Key;
                initializationStatements.Add(Expression.Call(
                    this.executionContext.LexicalEnvironment,
                    ReflectionHelpers.IEnvironmentRecord_CreateMutableBinding,
                    Expression.Constant(variableName),
                    Expression.Constant(true)));

                // If the variable is a function, initialize the variable.
                Expression initialValue = variableNameAndValue.Value;
                if (initialValue != null)
                    initializationStatements.Add(new MemberJSExpression(variableName).GetSetterExpression(
                        this.executionContext.LexicalEnvironment, initialValue));
            }

            if (this.executionContext.ScriptContext != ScriptContext.Function)
            {
                // Initialize the return value for the block to "undefined".
                initializationStatements.Add(Expression.Assign(this.executionContext.ReturnValue, ExpressionTreeHelpers.Undefined()));

                // Construct a block expression containing the initialization statements and the
                // program statements.
                return Expression.Block(
                    new ParameterExpression[] { this.executionContext.ReturnValue },
                    ExpressionTreeHelpers.Block(initializationStatements),
                    ExpressionTreeHelpers.Block(statements),
                    this.executionContext.ReturnValue);
            }
            else
            {
                // Construct a block expression containing the initialization statements and the
                // program statements.
                return Expression.Block(
                    ExpressionTreeHelpers.Block(initializationStatements),
                    ExpressionTreeHelpers.Block(statements),
                    Expression.Label(this.executionContext.EndOfProgram, ExpressionTreeHelpers.Undefined()));
            }
        }

        /// <summary>
        /// Parses any statement.
        /// </summary>
        /// <returns> An expression that represents the statement. </returns>
        private Expression ParseSourceStatement()
        {
            if (this.nextToken == KeywordToken.Function)
                return ParseFunction();
            return ParseStatement();
        }

        /// <summary>
        /// Parses any statement other than a function declaration.
        /// </summary>
        /// <returns> An expression that represents the statement. </returns>
        private Expression ParseStatement()
        {
            // Push an empty context for the new statement.
            this.statementContextStack.BeginStatement();

            // Record the line number of the start of the statement.
            int startLine = this.LineNumber;

            // Parse the statement.
            Expression statement = ParseStatementNoNewContext();

            // Record the line number of the end of the statement and insert debug info.
            statement = Expression.Block(Expression.DebugInfo(this.document, startLine, 1, Math.Max(startLine, this.LineNumber - 1), int.MaxValue), statement);

            // Pop any context related to the current statement.
            this.statementContextStack.EndStatement();

            return statement;
        }

        /// <summary>
        /// Parses any statement other than a function declaration, without beginning a new
        /// statement context.
        /// </summary>
        /// <returns> An expression that represents the statement. </returns>
        private Expression ParseStatementNoNewContext()
        {
            if (this.nextToken == PunctuatorToken.LeftBrace)
                return ParseBlock();
            if (this.nextToken == KeywordToken.Var)
                return ParseVar();
            if (this.nextToken == PunctuatorToken.Semicolon)
                return ParseEmpty();
            if (this.nextToken == KeywordToken.If)
                return ParseIf();
            if (this.nextToken == KeywordToken.Do)
                return ParseDo();
            if (this.nextToken == KeywordToken.While)
                return ParseWhile();
            if (this.nextToken == KeywordToken.For)
                return ParseFor();
            if (this.nextToken == KeywordToken.Continue)
                return ParseContinue();
            if (this.nextToken == KeywordToken.Break)
                return ParseBreak();
            if (this.nextToken == KeywordToken.Return)
                return ParseReturn();
            if (this.nextToken == KeywordToken.With)
                return ParseWith();
            if (this.nextToken == KeywordToken.Switch)
                return ParseSwitch();
            if (this.nextToken == KeywordToken.Throw)
                return ParseThrow();
            if (this.nextToken == KeywordToken.Try)
                return ParseTry();
            if (this.nextToken == KeywordToken.Debugger)
                return ParseDebugger();
            if (this.nextToken == null)
                throw new JavaScriptException("SyntaxError", "Unexpected end of input", this.LineNumber, this.SourcePath);

            // The statement is either a label or an expression.
            return ParseLabelOrExpressionStatement();
        }

        /// <summary>
        /// Parses a block of statements.
        /// </summary>
        /// <returns> An expression containing the statements. </returns>
        /// <remarks> The value of a block statement is the value of the last statement in the block,
        /// or undefined if there are no statements in the block. </remarks>
        private Expression ParseBlock()
        {
            // Consume the start brace ({).
            Debug.Assert(this.nextToken == PunctuatorToken.LeftBrace);
            this.Consume();

            // Read zero or more statements.
            var statements = new List<Expression>();
            while (true)
            {
                // Check for the end brace (}).
                if (this.nextToken == PunctuatorToken.RightBrace)
                    break;

                // Parse a single statement.
                statements.Add(ParseStatement());
            }

            // Consume the end brace.
            this.Consume();

            // Construct a block expression.
            return ExpressionTreeHelpers.Block(statements);
        }

        /// <summary>
        /// Parses a var statement.
        /// </summary>
        /// <returns> An expression representing the var statement. </returns>
        private Expression ParseVar()
        {
            // Read past the var token.
            this.Expect(KeywordToken.Var);

            // There can be multiple initializers.
            var statements = new List<Expression>(1);

            while (true)
            {
                // The next token must be a variable name.
                string variableName = this.ExpectIdentifier();

                // Add the variable to the current function's list of local variables.
                this.AddVariable(variableName);

                // The next token is either an equals sign (=), a semi-colon or a comma.
                if (this.nextToken == PunctuatorToken.Assignment)
                {
                    // Read past the equals token (=).
                    this.Expect(PunctuatorToken.Assignment);

                    // Read the setter expression.
                    var setterExpression = ParseJSExpression(PunctuatorToken.Semicolon, PunctuatorToken.Comma);

                    // Construct a new initialization expression that initializes the variable.
                    statements.Add(new AssignmentJSExpression(variableName, setterExpression).ToExpression(
                        this.executionContext.LexicalEnvironment));
                }

                // Check if we are at the end of the statement.
                if (this.AtValidEndOfStatement() == true)
                    break;

                // Read past the comma token.
                this.Expect(PunctuatorToken.Comma);
            }

            // Consume the end of the statement.
            this.ExpectEndOfStatement();

            // Construct a block expression.
            return ExpressionTreeHelpers.Block(statements);
        }

        /// <summary>
        /// Parses an empty statement.
        /// </summary>
        /// <returns> An empty expression. </returns>
        private Expression ParseEmpty()
        {
            this.Consume();
            return Expression.Empty();
        }

        /// <summary>
        /// Parses an if statement.
        /// </summary>
        /// <returns> An expression representing the if statement. </returns>
        private Expression ParseIf()
        {
            // Consume the if keyword.
            Debug.Assert(this.nextToken == KeywordToken.If);
            this.Consume();

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Parse the condition.
            var condition = ParseExpression(PunctuatorToken.RightParenthesis);

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Read the statements that will be executed when the condition is true.
            var trueStatements = ParseStatement();

            // Optionally, read the else statement.
            Expression falseStatements = Expression.Empty();
            if (this.nextToken == KeywordToken.Else)
            {
                // Consume the else keyword.
                this.Consume();

                // Read the statements that will be executed when the condition is false.
                falseStatements = ParseStatement();
            }

            // Construct an if-then-else expression.
            return Expression.IfThenElse(
                ExpressionTreeHelpers.ToBoolean(condition),
                trueStatements,
                falseStatements);
        }

        /// <summary>
        /// Parses a do statement.
        /// </summary>
        /// <returns> An expression representing the do statement. </returns>
        private Expression ParseDo()
        {
            // Consume the do keyword.
            Debug.Assert(this.nextToken == KeywordToken.Do);
            this.Consume();

            // Set up the labels that will be jumped to if the break or continue statements are
            // encountered.
            this.statementContextStack.Loop("do");

            // Read the statements that will be executed in the loop body.
            var statements = ParseStatement();

            // Read the while keyword.
            this.Expect(KeywordToken.While);

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Parse the condition.
            var condition = ParseExpression(PunctuatorToken.RightParenthesis);

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Consume the end of the statement.
            this.ExpectEndOfStatement();

            // Construct a loop expression.
            // while (true) {
            //   <body statements>
            //
            //   continue-target:
            //   if (condition == false)
            //     goto break-target;
            // }
            // break-target:
            return
                Expression.Loop(
                    Expression.Block(
                        statements,
                        Expression.Label(this.CurrentStatement.ContinueTarget),
                        Expression.IfThen(Expression.IsFalse(ExpressionTreeHelpers.ToBoolean(condition)), Expression.Break(this.CurrentStatement.BreakTarget))
                    ),
                    this.CurrentStatement.BreakTarget);
        }

        /// <summary>
        /// Parses a while statement.
        /// </summary>
        /// <returns> An expression representing the while statement. </returns>
        private Expression ParseWhile()
        {
            // Consume the while keyword.
            Debug.Assert(this.nextToken == KeywordToken.While);
            this.Consume();

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Parse the condition.
            var condition = ParseExpression(PunctuatorToken.RightParenthesis);

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Set up the labels that will be jumped to if the break or continue statements are
            // encountered.
            this.statementContextStack.Loop("while");

            // Read the statements that will be executed in the loop body.
            var statements = ParseStatement();

            // Construct a loop expression.
            // while (true) {
            //   continue-target:
            //   if (condition == false)
            //     goto break-target;
            //
            //   <body statements>
            // }
            // break-target:
            return
                Expression.Loop(
                    Expression.Block(
                        Expression.IfThen(Expression.IsFalse(ExpressionTreeHelpers.ToBoolean(condition)), Expression.Break(this.CurrentStatement.BreakTarget)),
                        statements
                    ),
                    this.CurrentStatement.BreakTarget, this.CurrentStatement.ContinueTarget);
        }

        /// <summary>
        /// Parses a for statement.
        /// </summary>
        /// <returns> An expression representing the for statement. </returns>
        private Expression ParseFor()
        {
            // Consume the for keyword.
            this.Expect(KeywordToken.For);

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // The initialization expression.  Is null for a for-in statement.
            Expression initializationExpression = null;

            // The for-in expression needs a variable to assign to.  Is null for a regular for statement.
            MemberJSExpression forInReference = null;

            if (this.nextToken == KeywordToken.Var)
            {
                // Read past the var token.
                this.Expect(KeywordToken.Var);

                // There can be multiple initializers (but not for for-in statements).
                var initializationStatements = new List<Expression>(1);

                // Only a simple variable name is allowed for for-in statements.
                bool cannotBeForIn = false;

                while (true)
                {
                    // The next token must be a variable name.
                    string variableName = this.ExpectIdentifier();

                    // Add the variable to the current function's list of local variables.
                    this.AddVariable(variableName);

                    // The next token is either an equals sign (=), a semi-colon, a comma, or the "in" keyword.
                    if (this.nextToken == PunctuatorToken.Assignment)
                    {
                        // Read past the equals token (=).
                        this.Expect(PunctuatorToken.Assignment);

                        // Read the setter expression.
                        var setterExpression = ParseJSExpression(PunctuatorToken.Semicolon, PunctuatorToken.Comma);

                        // Construct a new initialization expression that initializes the variable.
                        initializationStatements.Add(new AssignmentJSExpression(variableName, setterExpression).ToExpression(
                            this.executionContext.LexicalEnvironment));

                        // This is a regular for statement.
                        cannotBeForIn = true;
                    }

                    if (this.nextToken == PunctuatorToken.Semicolon)
                    {
                        // This is a regular for statement.
                        break;
                    }
                    else if (this.nextToken == KeywordToken.In && cannotBeForIn == false)
                    {
                        // This is a for-in statement.
                        forInReference = new MemberJSExpression(variableName);
                        break;
                    }
                    else if (this.nextToken != PunctuatorToken.Comma)
                        throw new JavaScriptException("SyntaxError", string.Format("Unexpected token '{0}'", this.nextToken.Text), 1, "");

                    // Read past the comma token.
                    this.Expect(PunctuatorToken.Comma);

                    // Multiple initializers are not allowed in for-in statements.
                    cannotBeForIn = true;
                }

                // Create a block expression containing all the initialization statements.
                if (forInReference == null)
                    initializationExpression = ExpressionTreeHelpers.Block(initializationStatements);
            }
            else
            {
                // Not a var initializer - can be a simple variable name then "in" or any expression ending with a semi-colon.
                // The expression can be empty.
                if (this.nextToken != PunctuatorToken.Semicolon)
                {
                    var initializer = ParseJSExpression(PunctuatorToken.Semicolon, KeywordToken.In);

                    if (this.nextToken == KeywordToken.In)
                    {
                        // This is a for-in statement.
                        if ((initializer is MemberJSExpression) == false)
                            throw new JavaScriptException("ReferenceError", "Invalid left-hand side in for-in", 1, "");
                        forInReference = (MemberJSExpression)initializer;
                    }
                    else
                    {
                        // This is a regular for statement.
                        initializationExpression = initializer.ToExpression(this.executionContext.LexicalEnvironment);
                    }
                }
                else
                    // The initializer expression was empty.
                    initializationExpression = Expression.Empty();
            }

            // Set up the labels that will be jumped to if the break or continue statements are
            // encountered.
            this.statementContextStack.Loop("for");

            if (forInReference != null)
            {
                // for (x in y)
                // for (var x in y)
                
                // Consume the "in".
                this.Expect(KeywordToken.In);

                // Parse the right-hand-side expression.
                var rightSideExpression = ParseExpression(PunctuatorToken.RightParenthesis);

                // Read the right parenthesis.
                this.Expect(PunctuatorToken.RightParenthesis);

                // Read the statements that will be executed in the loop body.
                var statements = ParseStatement();
                
                // Construct a loop expression.
                // var enumerator = TypeUtilities.EnumeratePropertyNames(rhs).GetEnumerator();
                // while (true) {
                //   continue-target:
                //   if (enumerator.MoveNext() == false)
                //     goto break-target;
                //   lhs = enumerator.Current;
                //
                //   <body statements>
                // }
                // break-target:
                var keys = Expression.Variable(typeof(IEnumerator<string>), "enumerator");
                return
                    Expression.Block(
                        new ParameterExpression[] { keys },
                        Expression.Assign(keys, Expression.Call(Expression.Call(ReflectionHelpers.TypeUtilities_EnumeratePropertyNames, Expression.Convert(rightSideExpression, typeof(object))), ReflectionHelpers.IEnumerable_GetEnumerator)),
                        Expression.Loop(
                            Expression.Block(
                                Expression.IfThen(Expression.IsFalse(Expression.Call(keys, ReflectionHelpers.IEnumerator_MoveNext)), Expression.Break(this.CurrentStatement.BreakTarget)),
                                forInReference.GetSetterExpression(this.executionContext.LexicalEnvironment, Expression.Call(keys, ReflectionHelpers.IEnumerator_Current)),
                                statements
                            ),
                            this.CurrentStatement.BreakTarget, this.CurrentStatement.ContinueTarget));
            }
            else
            {
                // Read the semicolon.
                this.Expect(PunctuatorToken.Semicolon);

                // Parse the optional condition expression.
                // Note: if the condition is omitted then it is considered to always be true. 
                var condition = this.nextToken != PunctuatorToken.Semicolon ? ParseExpression(PunctuatorToken.Semicolon) : Expression.Constant(true);

                // Read the semicolon.
                // Note: automatic semicolon insertion never inserts a semicolon in the header of a
                // for statement.
                this.Expect(PunctuatorToken.Semicolon);

                // Parse the optional update expression.
                var update = this.nextToken != PunctuatorToken.RightParenthesis ? ParseExpression(PunctuatorToken.RightParenthesis) : Expression.Empty();

                // Read the right parenthesis.
                this.Expect(PunctuatorToken.RightParenthesis);

                // Read the statements that will be executed in the loop body.
                var statements = ParseStatement();

                // Construct a loop expression.
                // <initializer>
                // while (true) {
                //   if (condition == false)
                //     break;
                //
                //   <body statements>
                //
                //   continue-target:
                //   <update expression>
                // }
                // break-target:
                return
                    Expression.Block(
                        initializationExpression,
                        Expression.Loop(
                            Expression.Block(
                                Expression.IfThen(Expression.IsFalse(ExpressionTreeHelpers.ToBoolean(condition)), Expression.Break(this.CurrentStatement.BreakTarget)),
                                statements,
                                Expression.Label(this.CurrentStatement.ContinueTarget),
                                update
                            ),
                            this.CurrentStatement.BreakTarget));
            }
        }

        /// <summary>
        /// Parses a continue statement.
        /// </summary>
        /// <returns> An expression representing the continue statement. </returns>
        public Expression ParseContinue()
        {
            // Consume the continue keyword.
            this.Expect(KeywordToken.Continue);

            // The continue statement can have an optional label to jump to.
            string labelName = null;
            if (this.AtValidEndOfStatement() == false)
            {
                // continue [label]

                // Read the label name.
                labelName = this.ExpectIdentifier();
            }

            // Consume the semi-colon, if there was one.
            this.ExpectEndOfStatement();

            // Make sure there is a valid continue target.
            var continueTarget = this.statementContextStack.FindContinueTarget(labelName);
            if (continueTarget == null)
                throw new JavaScriptException("SyntaxError", "Invalid continue statement", this.LineNumber, this.SourcePath);

            // Construct an expression to jump to the start of the nearest loop body.
            return Expression.Continue(continueTarget);
        }

        /// <summary>
        /// Parses a break statement.
        /// </summary>
        /// <returns> An expression representing the break statement. </returns>
        public Expression ParseBreak()
        {
            // Consume the break keyword.
            this.Expect(KeywordToken.Break);

            // The break statement can have an optional label to jump to.
            string labelName = null;
            if (this.AtValidEndOfStatement() == false)
            {
                // break [label]

                // Read the label name.
                labelName = this.ExpectIdentifier();
            }

            // Consume the semi-colon, if there was one.
            this.ExpectEndOfStatement();

            // Make sure there is a valid break target.
            var breakTarget = this.statementContextStack.FindBreakTarget(labelName);
            if (breakTarget == null)
                throw new JavaScriptException("SyntaxError", "Invalid break statement", this.LineNumber, this.SourcePath);

            // Construct an expression to jump to the start of the nearest loop body.
            return Expression.Break(breakTarget);
        }

        /// <summary>
        /// Parses a return statement.
        /// </summary>
        /// <returns> An expression representing the return statement. </returns>
        public Expression ParseReturn()
        {
            // Consume the return keyword.
            Debug.Assert(this.nextToken == KeywordToken.Return);
            this.Consume();

            Expression returnValue = ExpressionTreeHelpers.Undefined();
            if (this.AtValidEndOfStatement() == false)
            {
                // Parse the return value expression.
                returnValue = ParseExpression(PunctuatorToken.Semicolon);
            }

            // Consume the end of the statement.
            this.ExpectEndOfStatement();

            if (this.executionContext.ScriptContext == ScriptContext.Function)
            {
                // Jump to the end of the function.
                if (returnValue.Type != typeof(object))
                    returnValue = Expression.Convert(returnValue, typeof(object));
                return Expression.Return(this.executionContext.EndOfProgram, returnValue);
            }
            else
            {
                // Return is an error otherwise.
                return ExpressionTreeHelpers.Throw("SyntaxError", "Illegal return statement");
            }
        }

        /// <summary>
        /// Parses a with statement.
        /// </summary>
        /// <returns> An expression representing the with statement. </returns>
        public Expression ParseWith()
        {
            // This statement is not allowed in strict mode.
            if (this.StrictMode == true)
                throw new JavaScriptException("SyntaxError", "The with statement is not supported in strict mode", this.LineNumber, this.SourcePath);

            // Read past the "with" token.
            this.Expect(KeywordToken.With);

            // Read a left parenthesis token "(".
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Read an object reference.
            var expression = ParseExpression(PunctuatorToken.RightParenthesis);

            // Read a right parenthesis token ")".
            this.Expect(PunctuatorToken.RightParenthesis);

            // We need to modify the lexical environment, then restore it afterwards.
            var originalLexicalEnvironment = this.executionContext.LexicalEnvironment;
            this.executionContext.LexicalEnvironment = Expression.Variable(typeof(LexicalScope), "withScope");

            // Read the body of the with statement.
            var body = ParseStatement();

            // Initialize the dynamic part of the lexical environment.
            body = Expression.Block(
                new ParameterExpression[] { this.executionContext.LexicalEnvironment },
                Expression.Assign(this.executionContext.LexicalEnvironment, Expression.New(
                    ReflectionHelpers.LexicalScope_Constructor,
                    originalLexicalEnvironment,
                    ExpressionTreeHelpers.ToObject(expression),
                    Expression.Constant(true))),
                body);

            // Restore the lexical environment.
            this.executionContext.LexicalEnvironment = originalLexicalEnvironment;

            return body;
        }

        /// <summary>
        /// Parses a switch statement.
        /// </summary>
        /// <returns> An expression representing the switch statement. </returns>
        public Expression ParseSwitch()
        {
            // Consume the switch keyword.
            Debug.Assert(this.nextToken == KeywordToken.Switch);
            this.Consume();

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Parse the switch expression.
            var testExpression = ParseExpression(PunctuatorToken.RightParenthesis);

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Consume the start brace ({).
            Debug.Assert(this.nextToken == PunctuatorToken.LeftBrace);
            this.Consume();
            
            // Break statements need to jump to the end of the switch statement.
            this.statementContextStack.Switch();

            // Read zero or more case statements.
            var cases = new List<SwitchCase>();
            Expression defaultCase = null;
            Expression caseExpression = null;
            List<Expression> caseStatements = null;
            LabelTarget nextCaseLabel = null;

            while (true)
            {
                // Construct a SwitchCase expression once we have finished reading the case statements.
                if (caseStatements != null && (this.nextToken == KeywordToken.Case || this.nextToken == KeywordToken.Default || this.nextToken == PunctuatorToken.RightBrace))
                {
                    // Expression.Switch() requires that all case statements plus the default statement have the same type.
                    // The jump to the next case block accomplishes this.
                    caseStatements.Add(Expression.Goto(nextCaseLabel));

                    if (caseExpression != null)
                        cases.Add(Expression.SwitchCase(
                            ExpressionTreeHelpers.Block(caseStatements),
                            Expression.Convert(caseExpression, typeof(object))));
                    else if (defaultCase == null)
                        defaultCase = ExpressionTreeHelpers.Block(caseStatements);
                    else
                        throw new JavaScriptException("SyntaxError", "Only one default clause is allowed in a switch statement", this.LineNumber, this.SourcePath);
                }

                if (this.nextToken == KeywordToken.Case)
                {
                    // Read the case keyword.
                    this.Expect(KeywordToken.Case);

                    // Parse the case expression.
                    caseExpression = ParseExpression(PunctuatorToken.Colon);

                    // Consume the colon.
                    this.Expect(PunctuatorToken.Colon);

                    // Zero or more statements can be added to the case statement.
                    caseStatements = new List<Expression>();

                    // Start with a label so fallthrough works.
                    if (nextCaseLabel != null)
                        caseStatements.Add(Expression.Label(nextCaseLabel));
                    nextCaseLabel = Expression.Label("case-or-default-start");
                }
                else if (this.nextToken == KeywordToken.Default)
                {
                    // Read the default keyword.
                    this.Expect(KeywordToken.Default);

                    // Consume the colon.
                    this.Expect(PunctuatorToken.Colon);

                    // Zero or more statements can be added to the default statement.
                    caseExpression = null;
                    caseStatements = new List<Expression>();

                    // Start with a label so fallthrough works.
                    if (nextCaseLabel != null)
                        caseStatements.Add(Expression.Label(nextCaseLabel));
                    nextCaseLabel = Expression.Label("case-or-default-start");
                }
                else if (this.nextToken == PunctuatorToken.RightBrace)
                {
                    break;
                }
                else
                {
                    // Statements cannot be added directly after the switch.
                    if (caseStatements == null)
                        throw new JavaScriptException("SyntaxError", "Expected 'case' or 'default'.", this.LineNumber, this.SourcePath);

                    // Parse a single statement.
                    caseStatements.Add(ParseStatement());
                }
            }

            // Consume the end brace.
            this.Consume();

            // Account for the special case when there are no case statements.
            // Side-effects from the test expression must still be evaluated.
            if (cases.Count == 0)
            {
                if (defaultCase == null)
                    return testExpression;
                return Expression.Block(
                    testExpression,
                    defaultCase,
                    Expression.Label(nextCaseLabel),
                    Expression.Label(this.CurrentStatement.BreakTarget));
            }

            // Construct a switch expression.
            return Expression.Block(
                Expression.Switch(
                    Expression.Convert(testExpression, typeof(object)),
                    defaultCase,
                    ReflectionHelpers.TypeComparer_StrictEquals,
                    cases),
                Expression.Label(nextCaseLabel),
                Expression.Label(this.CurrentStatement.BreakTarget));
        }

        /// <summary>
        /// Parses a throw statement.
        /// </summary>
        /// <returns> An expression representing the throw statement. </returns>
        public Expression ParseThrow()
        {
            // Get the line number at the start of the throw statement.
            int lineNumber = this.LineNumber;

            // Consume the throw keyword.
            this.Expect(KeywordToken.Throw);

            // A line terminator is not allowed here.
            if (this.consumedLineTerminator == true)
                throw new JavaScriptException("SyntaxError", "Illegal newline after throw", 1, "");

            // Parse the expression to throw.
            var expression = ParseExpression(PunctuatorToken.Semicolon);

            // Consume the end of the statement.
            this.ExpectEndOfStatement();

            // Construct a throw expression.
            return Expression.Throw(Expression.New(
                ReflectionHelpers.JavaScriptException_Constructor3,
                Expression.Convert(expression, typeof(object)),
                Expression.Constant(this.LineNumber),
                Expression.Constant(this.SourcePath)));
        }

        /// <summary>
        /// Parses a try statement.
        /// </summary>
        /// <returns> An expression representing the try statement. </returns>
        public Expression ParseTry()
        {
            // Consume the try keyword.
            this.Expect(KeywordToken.Try);

            // Parse the try block.
            var tryBlock = Expression.Block(ParseBlock(), Expression.Empty());

            // The next token is either 'catch' or 'finally'.
            CatchBlock[] catchBlockArray = null;
            if (this.nextToken == KeywordToken.Catch)
            {
                // Consume the catch token.
                this.Expect(KeywordToken.Catch);

                // Read the left parenthesis.
                this.Expect(PunctuatorToken.LeftParenthesis);

                // Read the identifier to assign the exception to.
                var catchVariableName = this.ExpectIdentifier();
                var catchVariable = Expression.Variable(typeof(JavaScriptException), catchVariableName);

                // Check the variable name is valid.
                this.ValidateVariableName(catchVariableName);

                // Read the right parenthesis.
                this.Expect(PunctuatorToken.RightParenthesis);

                // We need to modify the lexical environment, then restore it afterwards.
                var originalLexicalEnvironment = this.executionContext.LexicalEnvironment;
                this.executionContext.LexicalEnvironment = Expression.Variable(typeof(LexicalScope), "catchScope");

                // Parse the catch statements.
                var catchBlock = ParseBlock();

                // Initialize the dynamic part of the lexical environment.
                catchBlock = Expression.Block(
                    new ParameterExpression[] { this.executionContext.LexicalEnvironment },
                    Expression.Assign(this.executionContext.LexicalEnvironment, Expression.New(
                        ReflectionHelpers.LexicalScope_Constructor,
                        originalLexicalEnvironment,
                        Expression.Constant(null, typeof(ObjectInstance)),
                        Expression.Constant(false))),
                    Expression.Call(
                        this.executionContext.LexicalEnvironment,
                        ReflectionHelpers.IEnvironmentRecord_CreateMutableBinding,
                        Expression.Constant(catchVariableName),
                        Expression.Constant(true)),
                    new MemberJSExpression(catchVariableName).GetSetterExpression(
                        this.executionContext.LexicalEnvironment,
                        Expression.Call(catchVariable, ReflectionHelpers.JavaScriptException_ErrorObject)), 
                    catchBlock,
                    Expression.Empty());

                // Create a single catch block expression.
                catchBlockArray = new CatchBlock[] {
                    Expression.Catch(catchVariable, catchBlock)
                };

                // Restore the lexical environment.
                this.executionContext.LexicalEnvironment = originalLexicalEnvironment;
            }

            Expression finallyBlock = null;
            if (this.nextToken == KeywordToken.Finally)
            {
                // Consume the finally token.
                this.Expect(KeywordToken.Finally);

                // Read the finally statements.
                finallyBlock = Expression.Block(ParseBlock(), Expression.Empty());
            }

            // Construct a try-catch-finally expression.
            return Expression.TryCatchFinally(
                tryBlock,
                finallyBlock,
                catchBlockArray);
        }

        /// <summary>
        /// Parses a debugger statement.
        /// </summary>
        /// <returns> An expression representing the debugger statement. </returns>
        public Expression ParseDebugger()
        {
            // Consume the debugger keyword.
            Debug.Assert(this.nextToken == KeywordToken.Debugger);
            this.Consume();

            // Consume the end of the statement.
            this.ExpectEndOfStatement();

            // Construct a debugger expression.
            return Expression.Call(ReflectionHelpers.Debugger_Break);
        }

        /// <summary>
        /// Parses a function.
        /// </summary>
        /// <param name="insideExpression"> <c>true</c> if this is a function expression; <c>false</c> if
        /// it is a function declaration. </param>
        /// <returns> An expression representing the function. </returns>
        public Expression ParseFunction(bool insideExpression = false)
        {
            // Consume the function keyword.
            Debug.Assert(this.nextToken == KeywordToken.Function);
            this.Consume();

            // Optionally read the function name.
            var functionName = string.Empty;
            if (insideExpression == false || this.nextToken is IdentifierToken)
            {
                // The function name is optional for function expressions.
                functionName = this.ExpectIdentifier();
            }

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Read zero or more parameter names.
            var parameterNames = new List<string>();

            // Read the first parameter name.
            if (this.nextToken != PunctuatorToken.RightParenthesis)
                parameterNames.Add(this.ExpectIdentifier());

            while (true)
            {
                if (this.nextToken == PunctuatorToken.Comma)
                {
                    // Consume the comma.
                    this.Consume();

                    // Read the parameter name.
                    parameterNames.Add(this.ExpectIdentifier());
                }
                else if (this.nextToken == PunctuatorToken.RightParenthesis)
                    break;
                else
                    throw new JavaScriptException("SyntaxError", "Expected ',' or ')'", 1, "1");
            }

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Create a new execution context.
            var originalContext = this.executionContext;
            this.executionContext = new FunctionContext(this, ScriptContext.Function);

            // Read the start brace.
            this.Expect(PunctuatorToken.LeftBrace);

            // Read the function body.
            var body = Parse();

            // Read the end brace.
            if (insideExpression == true)
            {
                // The end token '}' will be consumed by the parent function.
                if (this.nextToken != PunctuatorToken.RightBrace)
                    throw new JavaScriptException("SyntaxError", "Expected '}'", this.LineNumber, this.SourcePath);
            }
            else
                this.Expect(PunctuatorToken.RightBrace);

            // Compile the expression tree.
            Func<LexicalScope, object> compiledBody = Compile(body, functionName);

            // Restore the original context.
            this.executionContext = originalContext;

            // Construct an expression that creates the delegate that we need to pass to the UserDefinedFunction constructor.
            Expression delegateExpression;
            if (this.EnableDebugging == true)
            {
                delegateExpression = Expression.Convert(Expression.Call(ReflectionHelpers.Delegate_CreateDelegate,
                    Expression.Constant(typeof(Func<LexicalScope, object>), typeof(Type)),
                    Expression.Constant(compiledBody.Method, typeof(System.Reflection.MethodInfo))), typeof(Func<LexicalScope, object>));
            }
            else
            {
                delegateExpression = Expression.Constant(compiledBody);
            }

            // Construct an expression that creates a new UserDefinedFunction.
            var newFunctionExpression = Expression.New(ReflectionHelpers.UserDefinedFunction_Constructor,
                Expression.Call(Expression.Call(ReflectionHelpers.Global_Function), ReflectionHelpers.FunctionInstance_InstancePrototype),
                Expression.Constant(functionName),
                Expression.NewArrayInit(typeof(string), parameterNames.ConvertAll(name => Expression.Constant(name))),
                insideExpression == true ? this.executionContext.LexicalEnvironment : this.executionContext.VariableEnvironment,
                delegateExpression);

            // If this is a function expression, return the UserDefinedFunction.
            if (insideExpression == true)
                return newFunctionExpression;

            // Otherwise, add the function to the scope.
            this.AddVariable(functionName, newFunctionExpression);

            // Function declarations do nothing at the point of declaration - everything happens
            // at the top of the function/global code.
            return Expression.Empty();
        }

        /// <summary>
        /// Parses a statement consisting of an expression or starting with a label.  These two
        /// cases are disambiguated here.
        /// </summary>
        /// <returns> An expression representing the statement. </returns>
        private Expression ParseLabelOrExpressionStatement()
        {
            Expression result;

            // Parse the statement as though it was an expression - but stop if there is an unexpected colon.
            var expr = ParseJSExpression(PunctuatorToken.Semicolon, PunctuatorToken.Colon);

            if (this.nextToken == PunctuatorToken.Colon && expr is MemberJSExpression && ((MemberJSExpression)expr).Base == null)
            {
                // The expression is actually a label.

                // Extract the label name.
                this.statementContextStack.Label(((LiteralJSExpression)((MemberJSExpression)expr).Name).Value.ToString());

                // Read past the colon.
                this.Expect(PunctuatorToken.Colon);

                // Read the rest of the statement.
                result = ParseStatementNoNewContext();

                // Define a label at the end of the statement, but only if the statement wasn't a
                // loop or switch statement (these statements already have a label at the end.
                if (this.CurrentStatement.IsLoopOrSwitchStatement == false)
                    result = Expression.Block(
                        result,
                        Expression.Label(this.CurrentStatement.BreakTarget));
            }
            else
            {
                // Convert the expression into a usable form.
                result = expr.ToExpression(this.executionContext.LexicalEnvironment);

                // If this is global or eval code (i.e. not in a function) then store the result of
                // the expression in a variable so it can be used as the return value for the
                // program.
                if (this.executionContext.ScriptContext != ScriptContext.Function)
                {
                    if (result.Type != typeof(object))
                        result = Expression.Convert(result, typeof(object));
                    result = Expression.Assign(this.executionContext.ReturnValue, result);
                }

                // Consume the end of the statement.
                this.ExpectEndOfStatement();
            }
            return result;
        }



        //     EXPRESSION PARSER
        //_________________________________________________________________________________________

        /// <summary>
        /// Represents a key by which to look up an operator.
        /// </summary>
        private struct OperatorKey
        {
            public Token Token;
            public bool PostfixOrInfix;

            public override int GetHashCode()
            {
                return Token.GetHashCode() ^ PostfixOrInfix.GetHashCode();
            }
        }

        /// <summary>
        /// Gets or sets a mapping from token -> operator.  There can be at most two operators per
        /// token (the prefix version and the infix/postfix version).
        /// </summary>
        private static Dictionary<OperatorKey, Operator> operatorLookup;

        /// <summary>
        /// Finds a operator given a token and an indication whether the prefix or infix/postfix
        /// version is desired.
        /// </summary>
        /// <param name="token"> The token to search for. </param>
        /// <param name="postfixOrInfix"> <c>true</c> if the infix/postfix version of the operator
        /// is desired; <c>false</c> otherwise. </param>
        /// <returns> An Operator instance, or <c>null</c> if the operator could not be found. </returns>
        private static Operator OperatorFromToken(Token token, bool postfixOrInfix)
        {
            if (operatorLookup == null)
            {
                operatorLookup = new Dictionary<OperatorKey, Operator>(55);
                foreach (var @operator in Operator.AllOperators)
                {
                    operatorLookup.Add(new OperatorKey() { Token = @operator.Token, PostfixOrInfix = @operator.HasLHSOperand }, @operator);
                    if (@operator.SecondaryToken != null)
                    {
                        // Note: the secondary token for the grouping operator and function call operator ')' is a duplicate.
                        operatorLookup[new OperatorKey() { Token = @operator.SecondaryToken, PostfixOrInfix = @operator.HasRHSOperand }] = @operator;
                        if (@operator.InnerOperandIsOptional == true)
                            operatorLookup[new OperatorKey() { Token = @operator.SecondaryToken, PostfixOrInfix = false }] = @operator;
                    }
                }
            }

            Operator result;
            if (operatorLookup.TryGetValue(new OperatorKey() { Token = token, PostfixOrInfix = postfixOrInfix }, out result) == false)
                return null;
            return result;
        }

        /// <summary>
        /// Parses a javascript expression.
        /// </summary>
        /// <param name="endToken"> A token that indicates the end of the expression. </param>
        /// <returns> An expression tree that represents the expression. </returns>
        public Expression ParseExpression(params Token[] endTokens)
        {
            return ParseJSExpression(endTokens).ToExpression(this.executionContext.LexicalEnvironment);
        }

        /// <summary>
        /// Parses a javascript expression.
        /// </summary>
        /// <param name="endToken"> A token that indicates the end of the expression. </param>
        /// <returns> An expression tree that represents the expression. </returns>
        public JSExpression ParseJSExpression(params Token[] endTokens)
        {
            // The root of the expression tree.
            JSExpression root = null;

            // The active operator, i.e. the one last encountered.
            OperatorJSExpression unboundOperator = null;

            // Literals are always valid at the start of an expression.
            this.context = ParserContext.LiteralContext;

            while (this.nextToken != null)
            {
                if (this.nextToken is LiteralToken || this.nextToken is IdentifierToken ||
                    this.nextToken == KeywordToken.Function || this.nextToken == KeywordToken.This ||
                    this.nextToken == PunctuatorToken.LeftBrace ||
                    (this.context == ParserContext.LiteralContext && this.nextToken == PunctuatorToken.LeftBracket))
                {
                    // If a literal was found where an operator was expected, insert a semi-colon
                    // automatically (if this would fix the error and a line terminator was
                    // encountered) or throw an error.
                    if (this.context != ParserContext.LiteralContext)
                    {
                        // Check for automatic semi-colon insertion.
                        if (Array.IndexOf(endTokens, PunctuatorToken.Semicolon) >= 0 && this.consumedLineTerminator == true)
                            break;
                        throw new JavaScriptException("SyntaxError", "Unexpected variable or literal.", this.LineNumber, this.SourcePath);
                    }

                    JSExpression terminal;
                    if (this.nextToken is LiteralToken)
                        // If the token is a literal, convert it to a constant.
                        terminal = new LiteralJSExpression(((LiteralToken)this.nextToken).Value);
                    else if (this.nextToken is IdentifierToken)
                        // If the token is an identifier, convert it to a member access expression.
                        terminal = new MemberJSExpression(((IdentifierToken)this.nextToken).Name);
                    else if (this.nextToken == KeywordToken.This)
                        // Treat the "this" keyword like an identifier.
                        terminal = new MemberJSExpression("this");
                    else if (this.nextToken == PunctuatorToken.LeftBracket)
                        // Array literal.
                        terminal = ParseArrayLiteral();
                    else if (this.nextToken == PunctuatorToken.LeftBrace)
                        // Object literal.
                        terminal = ParseObjectLiteral();
                    else if (this.nextToken == KeywordToken.Function)
                        terminal = ParseFunctionExpression();
                    else
                        throw new InvalidOperationException("Unsupported literal type.");

                    // Push the literal to the most recent unbound operator, or, if there is none, to
                    // the root of the tree.
                    if (root == null)
                    {
                        // This is the first term in an expression.
                        root = terminal;
                    }
                    else
                    {
                        Debug.Assert(unboundOperator != null && unboundOperator.AcceptingOperands == true);
                        unboundOperator.Push(terminal);
                    }
                }
                else if (this.nextToken is PunctuatorToken || this.nextToken is KeywordToken)
                {
                    // The token is an operator (o1).
                    Operator newOperator = OperatorFromToken(this.nextToken, postfixOrInfix: this.context == ParserContext.OperatorContext);

                    // Make sure the token is actually an operator and not just a random keyword.
                    if (newOperator == null)
                    {
                        // Check if the token is an end token, for example a semi-colon.
                        if (Array.IndexOf(endTokens, this.nextToken) >= 0)
                            break;
                        // Check for automatic semi-colon insertion.
                        if (Array.IndexOf(endTokens, PunctuatorToken.Semicolon) >= 0 && (this.consumedLineTerminator == true || this.nextToken == PunctuatorToken.RightBrace))
                            break;
                        throw new JavaScriptException("SyntaxError", string.Format("Unexpected token '{0}' in expression.", this.nextToken.Text), this.LineNumber, this.SourcePath);
                    }

                    // There are four possibilities:
                    // 1. The token is the second of a two-part operator (for example, the ':' in a
                    //    conditional operator.  In this case, we need to go up the tree until we find
                    //    an instance of the operator and make that the active unbound operator.
                    if (this.nextToken == newOperator.SecondaryToken)
                    {
                        // Traverse down the tree looking for the parent operator that corresponds to
                        // this token.
                        OperatorJSExpression parentExpression = null;
                        var node = root as OperatorJSExpression;
                        while (node != null)
                        {
                            if (node.Operator.Token == newOperator.Token && node.SecondTokenEncountered == false)
                                parentExpression = node;
                            if (node == unboundOperator)
                                break;
                            node = node.RightBranch;
                        }

                        // If the operator was not found, then this is a mismatched token, unless
                        // it is the end token.  For example, if an unbalanced right parenthesis is
                        // found in an if statement then it is merely the end of the test expression.
                        if (parentExpression == null)
                        {
                            // Check if the token is an end token, for example a right parenthesis.
                            if (Array.IndexOf(endTokens, this.nextToken) >= 0)
                                break;
                            // Check for automatic semi-colon insertion.
                            if (Array.IndexOf(endTokens, PunctuatorToken.Semicolon) >= 0 && this.consumedLineTerminator == true)
                                break;
                            throw new JavaScriptException("SyntaxError", "Mismatched closing token in expression.", this.LineNumber, this.SourcePath);
                        }

                        // Mark that we have seen the closing token.
                        unboundOperator = parentExpression;
                        unboundOperator.SecondTokenEncountered = true;
                    }
                    else
                    {
                        // Check if the token is an end token, for example the comma in a variable
                        // declaration.
                        if (Array.IndexOf(endTokens, this.nextToken) >= 0)
                        {
                            // But make sure the token isn't inside an operator.
                            // For example, in the expression "var x = f(a, b)" the comma does not
                            // indicate the start of a new variable clause because it is inside the
                            // function call operator.
                            bool insideOperator = false;
                            var node = root as OperatorJSExpression;
                            while (node != null)
                            {
                                if (node.Operator.SecondaryToken != null && node.SecondTokenEncountered == false)
                                    insideOperator = true;
                                if (node == unboundOperator)
                                    break;
                                node = node.RightBranch;
                            }
                            if (insideOperator == false)
                                break;
                        }

                        // All the other situations involve the creation of a new operator.
                        var newExpression = OperatorJSExpression.FromOperator(newOperator);

                        // 2. The new operator is a prefix operator.  The new operator becomes an operand
                        //    of the previous operator.
                        if (newOperator.HasLHSOperand == false)
                        {
                            if (root == null)
                                // "!"
                                root = newExpression;
                            else if (unboundOperator != null && unboundOperator.AcceptingOperands == true)
                            {
                                // "5 + !"
                                unboundOperator.Push(newExpression);
                            }
                            else
                            {
                                // "5 !" or "5 + 5 !"
                                // Check for automatic semi-colon insertion.
                                if (Array.IndexOf(endTokens, PunctuatorToken.Semicolon) >= 0 && this.consumedLineTerminator == true)
                                    break;
                                throw new JavaScriptException("SyntaxError", "Invalid use of prefix operator.", this.LineNumber, this.SourcePath);
                            }
                        }
                        else
                        {
                            // Search up the tree for an operator that has a lower precedence.
                            // Because we don't store the parent link, we have to traverse down the
                            // tree and take the last one we find instead.
                            OperatorJSExpression lowPrecedenceOperator = null;
                            if (unboundOperator == null ||
                                (newOperator.Associativity == OperatorAssociativity.LeftToRight && unboundOperator.Precedence < newOperator.Precedence) ||
                                (newOperator.Associativity == OperatorAssociativity.RightToLeft && unboundOperator.Precedence <= newOperator.Precedence))
                            {
                                // Performance optimization: look at the previous operator first.
                                lowPrecedenceOperator = unboundOperator;
                            }
                            else
                            {
                                // Search for a lower precedence operator by traversing the tree.
                                var node = root as OperatorJSExpression;
                                while (node != null && node != unboundOperator)
                                {
                                    if ((newOperator.Associativity == OperatorAssociativity.LeftToRight && node.Precedence < newOperator.Precedence) ||
                                        (newOperator.Associativity == OperatorAssociativity.RightToLeft && node.Precedence <= newOperator.Precedence))
                                        lowPrecedenceOperator = node;
                                    node = node.RightBranch;
                                }
                            }

                            if (lowPrecedenceOperator == null)
                            {
                                // 3. The new operator has a lower precedence (or if the associativity is left to
                                //    right, a lower or equal precedence) than all the parent operators.  The new
                                //    operator goes to the root of the tree and the previous operator becomes the
                                //    first operand for the new operator.
                                if (root != null)
                                    newExpression.Push(root);
                                root = newExpression;
                            }
                            else
                            {
                                // 4. Otherwise, the new operator can steal the last operand from the previous
                                //    operator and then put itself in the place of that last operand.
                                if (lowPrecedenceOperator.OperandCount == 0)
                                {
                                    // "! ++"
                                    // Check for automatic semi-colon insertion.
                                    if (Array.IndexOf(endTokens, PunctuatorToken.Semicolon) >= 0 && this.consumedLineTerminator == true)
                                        break;
                                    throw new JavaScriptException("SyntaxError", "Invalid use of prefix operator.", this.LineNumber, this.SourcePath);
                                }
                                newExpression.Push(lowPrecedenceOperator.Pop());
                                lowPrecedenceOperator.Push(newExpression);
                            }
                        }

                        unboundOperator = newExpression;
                    }
                }
                else
                {
                    throw new JavaScriptException("SyntaxError", string.Format("Unexpected token '{0}' in expression", this.nextToken.Text), this.LineNumber, this.SourcePath);
                }

                // Read the next token.
                this.Consume(root != null && (unboundOperator == null || unboundOperator.AcceptingOperands == false) ? ParserContext.OperatorContext : ParserContext.LiteralContext);
            }

            // Empty expressions are invalid.
            if (root == null)
                throw new JavaScriptException("SyntaxError", "Expected an expression", this.LineNumber, this.SourcePath);

            // Resolve all the unbound operators into real operators.
            return root;
        }

        /// <summary>
        /// Parses an array literal (e.g. "[1, 2]").
        /// </summary>
        /// <returns> A literal expression that represents the array literal. </returns>
        private LiteralJSExpression ParseArrayLiteral()
        {
            // Read past the initial '[' token.
            Debug.Assert(this.nextToken == PunctuatorToken.LeftBracket);
            this.Consume();

            var items = new List<JSExpression>();
            while (true)
            {
                // If the next token is ']', then the array literal is complete.
                if (this.nextToken == PunctuatorToken.RightBracket)
                    break;

                // If the next token is ',', then the array element is undefined.
                if (this.nextToken == PunctuatorToken.Comma)
                    items.Add(new LiteralJSExpression(null));
                else
                    // Otherwise, read the next item in the array.
                    items.Add(ParseJSExpression(PunctuatorToken.Comma, PunctuatorToken.RightBracket));

                // Read past the comma.
                Debug.Assert(this.nextToken == PunctuatorToken.Comma || this.nextToken == PunctuatorToken.RightBracket);
                if (this.nextToken == PunctuatorToken.Comma)
                    this.Consume();
            }

            // The end token ']' will be consumed by the parent function.
            Debug.Assert(this.nextToken == PunctuatorToken.RightBracket);

            return new LiteralJSExpression(items.ToArray());
        }

        /// <summary>
        /// Parses an object literal (e.g. "{a: 5}").
        /// </summary>
        /// <returns> A literal expression that represents the object literal. </returns>
        private LiteralJSExpression ParseObjectLiteral()
        {
            // Read past the initial '{' token.
            Debug.Assert(this.nextToken == PunctuatorToken.LeftBrace);
            this.Consume();

            var properties = new Dictionary<string, JSExpression>();
            while (true)
            {
                // If the next token is '}', then the object literal is complete.
                if (this.nextToken == PunctuatorToken.RightBrace)
                    break;

                // Read the next property name.
                string propertyName;
                if (this.nextToken is LiteralToken)
                {
                    // The property name can be a string or a number.
                    object literalValue = ((LiteralToken)this.nextToken).Value;
                    if ((literalValue is string || literalValue is double) == false)
                        throw new JavaScriptException("SyntaxError", "Expected property name", 1, "");
                    propertyName = ((LiteralToken)this.nextToken).Value.ToString();
                }
                else if (this.nextToken is IdentifierToken)
                {
                    // An identifier is also okay.
                    propertyName = ((IdentifierToken)this.nextToken).Name;
                 
                    // Check the property name is valid.
                    ValidateVariableName(propertyName);
                }
                else
                    throw new JavaScriptException("SyntaxError", "Expected property name", 1, "");
                this.Consume();
                
                // Read the colon.
                this.Expect(PunctuatorToken.Colon);

                // Now read the property value.
                var propertyValue = ParseJSExpression(PunctuatorToken.Comma, PunctuatorToken.RightBrace);

                // In strict mode, properties cannot be added twice.
                if (this.StrictMode == true && properties.ContainsKey(propertyName) == true)
                    throw new JavaScriptException("SyntaxError", string.Format("Property name '{0}' already exists in object literal", propertyName), 1, "");

                // Add the property setter to the list.
                properties[propertyName] = propertyValue;

                // Read past the comma.
                Debug.Assert(this.nextToken == PunctuatorToken.Comma || this.nextToken == PunctuatorToken.RightBrace);
                if (this.nextToken == PunctuatorToken.Comma)
                    this.Consume();
            }

            // The end token '}' will be consumed by the parent function.
            Debug.Assert(this.nextToken == PunctuatorToken.RightBrace);

            return new LiteralJSExpression(properties);
        }

        /// <summary>
        /// Parses a function expression.
        /// </summary>
        /// <returns> A literal expression representing the value of the function. </returns>
        public LiteralJSExpression ParseFunctionExpression()
        {
            // Create an expression representing the function.
            var expression = ParseFunction(insideExpression: true);

            // Return a literal containing the expression.
            return new LiteralJSExpression(expression);
        }

        ///// <summary>
        ///// Converts all UnboundOperatorExpression expressions in the given expression tree to real
        ///// expressions.
        ///// </summary>
        ///// <param name="expression"> The root of the expression tree to convert. </param>
        ///// <returns> A compilable expression tree. </returns>
        //private Expression BindOperators(Expression expression)
        //{
        //    // Only unbound operators are transformed.
        //    if ((expression is UnboundOperatorExpression) == false)
        //        return expression;
        //    var unboundExpression = (UnboundOperatorExpression)expression;

        //    // Pop the operands off the stack.  The operands come off the stack in reverse order, so
        //    // we reverse the order here.
        //    Expression[] operands = new Expression[unboundExpression.Operands.Count];
        //    for (int i = operands.Length - 1; i >= 0; i--)
        //        operands[i] = BindOperators(unboundExpression.Operands.Pop());
        //    throw new NotImplementedException();
        //    //return unboundExpression.Operator.GetStaticExpression(operands);
        //}



        //     STATEMENT CONTEXT
        //_________________________________________________________________________________________

        private class StatementContext
        {
            /// <summary>
            /// Gets the names of any labels.
            /// </summary>
            public List<string> LabelNames;

            /// <summary>
            /// Gets the label to jump to if a break statement is encountered.
            /// </summary>
            public LabelTarget BreakTarget;

            /// <summary>
            /// Gets the label to jump to if a continue statement is encountered.
            /// </summary>
            public LabelTarget ContinueTarget;

            /// <summary>
            /// Gets a value that indicates whether the current statement is a loop or switch
            /// statement.
            /// </summary>
            public bool IsLoopOrSwitchStatement;
        }

        private class StatementContextStack
        {
            private Stack<StatementContext> stack = new Stack<StatementContext>();
            private StatementContext current;

            /// <summary>
            /// Begins a new statement.
            /// </summary>
            public void BeginStatement()
            {
                if (this.current != null)
                    this.stack.Push(this.current);
                this.current = new StatementContext();
            }

            /// <summary>
            /// Ends a statement.
            /// </summary>
            public void EndStatement()
            {
                if (this.stack.Count == 0)
                    this.current = null;
                else
                    this.current = this.stack.Pop();
            }

            /// <summary>
            /// Indicates the current statement has a label.
            /// </summary>
            /// <param name="labelName"> The name of the label. </param>
            public void Label(string labelName)
            {
                // Check that the label doesn't already exist in the stack.
                // This relies on the fact that all labels have break targets.
                if (FindBreakTarget(labelName) != null)
                    throw new JavaScriptException("SyntaxError", string.Format("Label '{0}' has already been declared", labelName), 1, "");

                // Add a new label.
                if (this.current.LabelNames == null)
                    this.current.LabelNames = new List<string>();
                this.current.LabelNames.Add(labelName);
                this.current.BreakTarget = Expression.Label(labelName + "-break");
            }

            /// <summary>
            /// Indicates the current statement is a loop.
            /// </summary>
            /// <param name="loopType"> The type of loop, e.g. "do", "while" or "for". </param>
            public void Loop(string loopType)
            {
                this.current.ContinueTarget = Expression.Label(loopType + "-continue");
                this.current.BreakTarget = Expression.Label(loopType + "-break");
                this.current.IsLoopOrSwitchStatement = true;
            }

            /// <summary>
            /// Indicates the current statement is a switch statement.
            /// </summary>
            public void Switch()
            {
                this.current.BreakTarget = Expression.Label("switch-break");
                this.current.IsLoopOrSwitchStatement = true;
            }

            /// <summary>
            /// Gets the context for the current statement.
            /// </summary>
            public StatementContext CurrentStatement
            {
                get { return this.current; }
            }

            /// <summary>
            /// Finds a break target to jump to for the given label name.
            /// </summary>
            /// <param name="labelName"> The label to search for. </param>
            /// <returns> A label to jump to, or <c>null</c> if the label wasn't found. </returns>
            public LabelTarget FindBreakTarget(string labelName = null)
            {
                if (labelName == null)
                {
                    if (this.current.IsLoopOrSwitchStatement == true)
                        return this.current.BreakTarget;
                    foreach (var context in stack)
                        if (context.IsLoopOrSwitchStatement == true)
                            return context.BreakTarget;
                }
                else
                {
                    if (this.current.LabelNames != null && this.current.LabelNames.Contains(labelName))
                        return this.current.BreakTarget;
                    foreach (var context in stack)
                        if (context.LabelNames != null && context.LabelNames.Contains(labelName))
                            return context.BreakTarget;
                }
                return null;
            }

            /// <summary>
            /// Finds a continue target to jump to for the given label name.
            /// </summary>
            /// <param name="labelName"> The label to search for. </param>
            /// <returns> A label to jump to, or <c>null</c> if the label wasn't found. </returns>
            public LabelTarget FindContinueTarget(string labelName = null)
            {
                if (labelName == null)
                {
                    if (this.current.IsLoopOrSwitchStatement == true && this.current.ContinueTarget != null)
                        return this.current.ContinueTarget;
                    foreach (var context in stack)
                        if (context.IsLoopOrSwitchStatement == true && context.ContinueTarget != null)
                            return context.ContinueTarget;
                }
                else
                {
                    if (this.current.LabelNames != null && this.current.LabelNames.Contains(labelName) && this.current.ContinueTarget != null)
                        return this.current.ContinueTarget;
                    foreach (var context in stack)
                        if (context.LabelNames != null && context.LabelNames.Contains(labelName) && context.ContinueTarget != null)
                            return context.ContinueTarget;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the context for the current statement.
        /// </summary>
        private StatementContext CurrentStatement
        {
            get { return this.statementContextStack.CurrentStatement; }
        }



        //     COMPILATION
        //_________________________________________________________________________________________

        private System.Reflection.Emit.ModuleBuilder module;
        private int methodCount = 0;

        /// <summary>
        /// Compiles the given expression and produces a delegate that can be called to execute the
        /// code.
        /// </summary>
        /// <param name="expression"> The expression to compile. </param>
        /// <param name="functionName"> The name of the function that is being compiled.  Can be
        /// <c>null</c> or empty if a function name is not available. </param>
        /// <returns> A delegate that can be used to execute the expression. </returns>
        internal Func<LexicalScope, object> Compile(Expression expression, string functionName = null)
        {
            if (this.EnableDebugging == false)
            {
                // Compile the expression tree.
                var lambdaExpression = Expression.Lambda<Func<LexicalScope, object>>(
                    expression,
                    this.executionContext.VariableEnvironment);
                return lambdaExpression.Compile();
            }
            else
            {
                // Create a module if we haven't already.
                if (module == null)
                {
                    // create a dynamic assembly and module 
                    var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new System.Reflection.AssemblyName("JavaScript"), System.Reflection.Emit.AssemblyBuilderAccess.Run);

                    // Mark generated code as debuggable.
                    // See http://blogs.msdn.com/rmbyers/archive/2005/06/26/432922.aspx for explanation.
                    Type daType = typeof(DebuggableAttribute);
                    var daCtor = daType.GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
                    var daBuilder = new System.Reflection.Emit.CustomAttributeBuilder(daCtor, new object[] { 
                        DebuggableAttribute.DebuggingModes.DisableOptimizations | 
                        DebuggableAttribute.DebuggingModes.Default });
                    assemblyBuilder.SetCustomAttribute(daBuilder);

                    module = assemblyBuilder.DefineDynamicModule("Module", true);
                }

                // create a new type to hold our Main method
                var typeBuilder = module.DefineType("JavaScript" + (methodCount++).ToString(), System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Class);

                // Create a method to hold the javascript code.
                functionName = string.IsNullOrEmpty(functionName) ? "Main" : functionName;
                var methodbuilder = typeBuilder.DefineMethod(functionName, System.Reflection.MethodAttributes.HideBySig | System.Reflection.MethodAttributes.Static | System.Reflection.MethodAttributes.Public,
                    typeof(object), new Type[] { typeof(LexicalScope) });

                // Convert to a lamba expression and compile it.
                var lambdaExpression = Expression.Lambda<Func<LexicalScope, object>>(
                    expression,
                    this.executionContext.VariableEnvironment);
                lambdaExpression.CompileToMethod(methodbuilder, System.Runtime.CompilerServices.DebugInfoGenerator.CreatePdbGenerator());

                // bake it 
                var helloWorldType = typeBuilder.CreateType();

                // get the method
                var method = helloWorldType.GetMethod(functionName);

                return (Func<LexicalScope, object>)Delegate.CreateDelegate(typeof(Func<LexicalScope, object>), method);
            }
        }

        /// <summary>
        /// Parses and compiles the code and produces a delegate that can be called to execute the
        /// code.
        /// </summary>
        /// <returns> A delegate that can be used to execute the code. </returns>
        public Func<LexicalScope, object> Compile()
        {
            // Convert the source code into an expression tree.
            var expressionTree = this.Parse();

            // Compile the expression tree.
            return Compile(expressionTree);
        }



        //     EVAL SUPPORT
        //_________________________________________________________________________________________

        /// <summary>
        /// Evaluates the given javascript source code and returns the result.
        /// </summary>
        /// <param name="code"> The source code to evaluate. </param>
        /// <param name="scriptContext"> The context the provided code will run in. </param>
        /// <returns> The value of the last statement that was executed, or <c>undefined</c> if
        /// there were no executed statements. </returns>
        public static object Eval(string code)
        {
            // Create a new parser.
            var parser = new Parser(new Lexer(new System.IO.StringReader(code), "[eval code]"), ScriptContext.Eval);

            // Parse and compile the code.
            var expressionFunc = parser.Compile();

            // Execute the compiled lambda.
            object result = expressionFunc(LexicalScope.Global);

            // Transform null -> undefined.
            if (result == null)
                result = Undefined.Value;
            return result;
        }
    }

}