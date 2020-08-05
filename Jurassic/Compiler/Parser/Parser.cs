﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Converts a series of tokens into an abstract syntax tree.
    /// </summary>
    internal sealed class Parser
    {
        private Lexer lexer;
        private SourceCodePosition positionBeforeWhitespace, positionAfterWhitespace;
        private Token nextToken;
        private bool consumedLineTerminator;
        private ParserExpressionState expressionState;
        private Scope initialScope;
        private Scope currentScope;
        private MethodOptimizationHints methodOptimizationHints;
        private List<string> labelsForCurrentStatement = new List<string>();
        private Token endToken;
        private CompilerOptions options;
        private CodeContext context;
        private bool strictMode;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a Parser instance with the given lexer supplying the tokens.
        /// </summary>
        /// <param name="lexer"> The lexical analyser that provides the tokens. </param>
        /// <param name="options"> Options that influence the compiler. </param>
        /// <param name="context"> The context of the code (global, function or eval). </param>
        /// <param name="methodOptimizationHints"> Hints about whether optimization is possible. </param>
        public Parser(Lexer lexer, CompilerOptions options, CodeContext context, MethodOptimizationHints methodOptimizationHints = null)
        {
            this.lexer = lexer ?? throw new ArgumentNullException(nameof(lexer)); ;
            this.lexer.ParserExpressionState = ParserExpressionState.Literal;
            this.lexer.CompatibilityMode = options.CompatibilityMode;
            SetInitialScope(Scope.CreateGlobalOrEvalScope(context));
            this.methodOptimizationHints = methodOptimizationHints ?? new MethodOptimizationHints();
            this.options = options;
            this.context = context;
            this.StrictMode = options.ForceStrictMode;
            this.Consume();
        }

        /// <summary>
        /// Creates a parser that can read the body of a function.
        /// </summary>
        /// <param name="parser"> The parser for the parent context. </param>
        /// <param name="scope"> The function scope. </param>
        /// <param name="optimizationHints"> Hints about whether optimization is possible. </param>
        /// <param name="codeContext"> Indicates the parsing context. </param>
        /// <returns> A new parser. </returns>
        private static Parser CreateFunctionBodyParser(Parser parser, Scope scope, MethodOptimizationHints optimizationHints, CodeContext codeContext)
        {
            var result = (Parser)parser.MemberwiseClone();
            result.SetInitialScope(scope);
            result.methodOptimizationHints = optimizationHints;
            result.context = codeContext;
            result.endToken = PunctuatorToken.RightBrace;
            return result;
        }



        //     PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the line number of the next token.
        /// </summary>
        public int LineNumber
        {
            get { return this.positionAfterWhitespace.Line; }
        }

        /// <summary>
        /// Gets the position just after the last character of the previously consumed token.
        /// </summary>
        public SourceCodePosition PositionBeforeWhitespace
        {
            get { return this.positionBeforeWhitespace; }
        }

        /// <summary>
        /// Gets the position of the first character of the next token.
        /// </summary>
        public SourceCodePosition PositionAfterWhitespace
        {
            get { return this.positionAfterWhitespace; }
        }

        /// <summary>
        /// Gets the path or URL of the source file.  Can be <c>null</c>.
        /// </summary>
        public string SourcePath
        {
            get { return this.lexer.Source.Path; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the parser is operating in strict mode.
        /// </summary>
        public bool StrictMode
        {
            get { return this.strictMode; }
            set
            {
                this.strictMode = value;
                this.lexer.StrictMode = value;
            }
        }

        /// <summary>
        /// The top-level scope.
        /// </summary>
        public Scope BaseScope
        {
            get { return this.initialScope; }
        }

        /// <summary>
        /// Gets optimization information about the code that was parsed (Parse() must be called
        /// first).
        /// </summary>
        public MethodOptimizationHints MethodOptimizationHints
        {
            get { return this.methodOptimizationHints; }
        }

        /// <summary>
        /// Indicates whether we are parsing in a function context (including constructors and class functions).
        /// </summary>
        public bool IsInFunctionContext
        {
            get {  return this.context == CodeContext.Function ||
                    this.context == CodeContext.ObjectLiteralFunction ||
                    this.context == CodeContext.ClassFunction ||
                    this.context == CodeContext.Constructor ||
                    this.context == CodeContext.DerivedConstructor;  }
        }



        //     VARIABLES
        //_________________________________________________________________________________________
     
        /// <summary>
        /// Throws an exception if the variable name is invalid.
        /// </summary>
        /// <param name="name"> The name of the variable to check. </param>
        private void ValidateVariableName(string name)
        {
            // In strict mode, the variable name cannot be "eval" or "arguments".
            if (this.StrictMode == true && (name == "eval" || name == "arguments"))
                throw new SyntaxErrorException(string.Format("The variable name cannot be '{0}' in strict mode.", name), this.LineNumber, this.SourcePath);

            // Record each occurance of a variable name.
            this.methodOptimizationHints.EncounteredVariable(name);
        }



        //     TOKEN HELPERS
        //_________________________________________________________________________________________

        /// <summary>
        /// Discards the current token and reads the next one.
        /// </summary>
        /// <param name="expressionState"> Indicates whether the next token can be a literal or an
        /// operator. </param>
        private void Consume(ParserExpressionState expressionState = ParserExpressionState.Literal)
        {
            this.expressionState = expressionState;
            this.lexer.ParserExpressionState = expressionState;
            this.consumedLineTerminator = false;
            this.positionBeforeWhitespace = new SourceCodePosition(this.lexer.LineNumber, this.lexer.ColumnNumber);
            this.positionAfterWhitespace = this.positionBeforeWhitespace;
            while (true)
            {
                if (expressionState == ParserExpressionState.TemplateContinuation)
                    this.nextToken = this.lexer.ReadStringLiteral('`');
                else
                    this.nextToken = this.lexer.NextToken();
                if ((this.nextToken is WhiteSpaceToken) == false)
                    break;
                if (((WhiteSpaceToken)this.nextToken).LineTerminatorCount > 0)
                    this.consumedLineTerminator = true;
                this.positionAfterWhitespace = new SourceCodePosition(this.lexer.LineNumber, this.lexer.ColumnNumber);
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
                throw new SyntaxErrorException(string.Format("Expected '{0}' but found {1}", token.Text, Token.ToText(this.nextToken)), this.LineNumber, this.SourcePath);
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
                throw new SyntaxErrorException(string.Format("Expected identifier but found {0}", Token.ToText(this.nextToken)), this.LineNumber, this.SourcePath);
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the current position is a valid position to end
        /// a statement.
        /// </summary>
        /// <returns> <c>true</c> if the current position is a valid position to end a statement;
        /// <c>false</c> otherwise. </returns>
        private bool AtValidEndOfStatement()
        {
            // A statement can be terminator in four ways: by a semi-colon (;), by a right brace (}),
            // by the end of a line or by the end of the program.
            return this.nextToken == PunctuatorToken.Semicolon ||
                this.nextToken == PunctuatorToken.RightBrace ||
                this.consumedLineTerminator == true ||
                this.nextToken == null;
        }

        /// <summary>
        /// Indicates that the next token should end the current statement.  This implies that the
        /// next token is a semicolon, right brace or a line terminator.
        /// </summary>
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
                throw new SyntaxErrorException(string.Format("Expected ';' but found {0}", Token.ToText(this.nextToken)), this.LineNumber, this.SourcePath);
            }
        }

        //     SCOPE HELPERS
        //_________________________________________________________________________________________

        /// <summary>
        /// Sets the initial scope.
        /// </summary>
        /// <param name="initialScope"> The initial scope </param>
        private void SetInitialScope(Scope initialScope)
        {
            if (initialScope == null)
                throw new ArgumentNullException(nameof(initialScope));
            this.currentScope = this.initialScope = initialScope;
        }

        /// <summary>
        /// Helper class to help manage scopes.
        /// </summary>
        private class ScopeContext : IDisposable
        {
            private readonly Parser parser;
            private readonly Scope previousScope;

            public ScopeContext(Parser parser)
            {
                this.parser = parser;
                previousScope = parser.currentScope;
            }

            public void Dispose()
            {
                parser.currentScope = previousScope;
            }
        }

        /// <summary>
        /// Sets the current scope and returns an object which can be disposed to restore the
        /// previous scope.
        /// </summary>
        /// <param name="scope"> The new scope. </param>
        /// <returns> An object which can be disposed to restore the previous scope. </returns>
        private ScopeContext CreateScopeContext(Scope scope)
        {
            var result = new ScopeContext(this);
            this.currentScope = scope ?? throw new ArgumentNullException(nameof(scope));
            return result;
        }



        //     PARSE METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Parses javascript source code.
        /// </summary>
        /// <returns> An expression that can be executed to run the program represented by the
        /// source code. </returns>
        public Statement Parse()
        {
            // Read the directive prologue.
            var result = new BlockStatement(new string[0], this.initialScope);
            while (true)
            {
                // Check if we should stop parsing.
                if (this.nextToken == this.endToken)
                    break;

                // A directive must start with a string literal token.  Record it now so that the
                // escape sequence and line continuation information is not lost.
                var directiveToken = this.nextToken as StringLiteralToken;
                if (directiveToken == null)
                    break;

                // Directives cannot have escape sequences or line continuations.
                if (directiveToken.EscapeSequenceCount != 0 || directiveToken.LineContinuationCount != 0)
                    break;

                // If the statement starts with a string literal, it must be an expression.
                var beforeInitialToken = this.PositionAfterWhitespace;
                var expression = ParseExpression(PunctuatorToken.Semicolon);

                // The statement must be added to the AST so that eval("'test'") works.
                var initialStatement = new ExpressionStatement(this.labelsForCurrentStatement, expression);
                initialStatement.SourceSpan = new SourceCodeSpan(beforeInitialToken, this.PositionBeforeWhitespace);
                result.Statements.Add(initialStatement);

                // In order for the expression to be part of the directive prologue, it must
                // consist solely of a string literal.
                if ((expression is LiteralExpression) == false)
                    break;

                // Strict mode directive.
                if (directiveToken.Value == "use strict")
                    this.StrictMode = true;

                // Read the end of the statement.  This must happen last so that the lexer has a
                // chance to act on the strict mode flag.
                ExpectEndOfStatement();
            }

            // If this is an eval, and strict mode is on, redefine the scope.
            if (this.StrictMode == true)
                this.initialScope.ConvertToStrictMode();

            // Read zero or more regular statements.
            while (true)
            {
                // Check if we should stop parsing.
                if (this.nextToken == this.endToken)
                    break;

                // Parse a single statement.
                result.Statements.Add(ParseStatement(addingToExistingBlock: true));
            }

            return result;
        }

        /// <summary>
        /// Parses any statement other than a function declaration.
        /// </summary>
        /// <param name="addingToExistingBlock"> <c>true</c> if the statement is being added to an
        /// existing block statement, <c>false</c> if the statement represents a new block. </param>
        /// <returns> An expression that represents the statement. </returns>
        private Statement ParseStatement(bool addingToExistingBlock)
        {
            // This is a new statement so clear any labels.
            this.labelsForCurrentStatement.Clear();

            // Parse the statement.
            Statement statement = ParseStatementNoNewContext();

            // Let and const statements are disallowed in single-statement contexts.
            if (!addingToExistingBlock &&
                statement is VarLetOrConstStatement varLetOrConstStatement &&
                varLetOrConstStatement.Keyword != KeywordToken.Var)
                throw new SyntaxErrorException("Lexical declaration cannot appear in a single-statement context.", this.LineNumber, this.SourcePath);

            return statement;
        }

        /// <summary>
        /// Parses any statement other than a function declaration, without beginning a new
        /// statement context.
        /// </summary>
        /// <returns> An expression that represents the statement. </returns>
        private Statement ParseStatementNoNewContext()
        {
            if (this.nextToken == PunctuatorToken.LeftBrace)
                return ParseBlock();
            if (this.nextToken == KeywordToken.Var || this.nextToken == KeywordToken.Let || this.nextToken == KeywordToken.Const)
                return ParseVarLetOrConst((KeywordToken)this.nextToken);
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
            if (this.nextToken == KeywordToken.Function)
                return ParseFunctionDeclaration();
            if (this.nextToken == KeywordToken.Class)
                return ParseClassDeclaration();
            if (this.nextToken == null)
                throw new SyntaxErrorException("Unexpected end of input", this.LineNumber, this.SourcePath);

            // The statement is either a label or an expression.
            return ParseLabelOrExpressionStatement();
        }

        /// <summary>
        /// Parses a block of statements.
        /// </summary>
        /// <returns> A BlockStatement containing the statements. </returns>
        /// <remarks> The value of a block statement is the value of the last statement in the block,
        /// or undefined if there are no statements in the block. </remarks>
        private BlockStatement ParseBlock()
        {
            var scope = Scope.CreateBlockScope(this.currentScope);
            using (CreateScopeContext(scope))
            {

                // Consume the start brace ({).
                this.Expect(PunctuatorToken.LeftBrace);

                // Read zero or more statements.
                var result = new BlockStatement(this.labelsForCurrentStatement, scope);
                while (true)
                {
                    // Check for the end brace (}).
                    if (this.nextToken == PunctuatorToken.RightBrace)
                        break;

                    // Parse a single statement.
                    result.Statements.Add(ParseStatement(addingToExistingBlock: true));
                }

                // Consume the end brace.
                this.Expect(PunctuatorToken.RightBrace);
                return result;

            }
        }

        /// <summary>
        /// Parses a var, let or const statement.
        /// </summary>
        /// <param name="keyword"> Indicates which type of statement is being parsed.  Must be var,
        /// let or const. </param>
        /// <param name="consumeKeyword"> Indicates whether the keyword token needs to be consumed. </param>
        /// <param name="insideForLoop"> Indicates whether we are parsing the initial declaration
        /// inside a for() statement. </param>
        /// <returns> A variable declaration statement. </returns>
        private VarLetOrConstStatement ParseVarLetOrConst(KeywordToken keyword, bool consumeKeyword = true, bool insideForLoop = false)
        {
            var result = new VarLetOrConstStatement(this.labelsForCurrentStatement, this.currentScope);
            result.Keyword = keyword;

            // Read past the first token (var, let or const).
            if (consumeKeyword)
                this.Expect(keyword);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // There can be multiple declarations.
            while (true)
            {
                // The next token must be a variable name.
                var declaration = new VariableDeclaration(keyword, ExpectIdentifier());
                ValidateVariableName(declaration.VariableName);
                if (keyword == KeywordToken.Let && declaration.VariableName == "let")
                    throw new SyntaxErrorException("'let' is not allowed here.", this.LineNumber, this.SourcePath);

                // Add the variable to the current function's list of local variables.
                if (keyword != KeywordToken.Var && this.currentScope.HasDeclaredVariable(declaration.VariableName))
                    throw new SyntaxErrorException($"Identifier '{declaration.VariableName}' has already been declared.", this.LineNumber, this.SourcePath);
                this.currentScope.DeclareVariable(keyword, declaration.VariableName);

                // The next token is either an equals sign (=), a semi-colon or a comma.
                if (this.nextToken == PunctuatorToken.Assignment)
                {
                    // Read past the equals token (=).
                    this.Expect(PunctuatorToken.Assignment);

                    // Read the setter expression.
                    declaration.InitExpression = ParseExpression(PunctuatorToken.Semicolon, PunctuatorToken.Comma);
                }

                // Record the portion of the source document that will be highlighted when debugging.
                declaration.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

                // Add the declaration to the result.
                result.Declarations.Add(declaration);

                // If we are inside a for loop, then 'in' and 'of' are valid terminators.
                // Also, to match ParseExpression(), we don't consume the final semi-colon.
                if (insideForLoop && (this.nextToken == KeywordToken.In || this.nextToken == IdentifierToken.Of ||
                    (this.AtValidEndOfStatement() == true && this.nextToken != PunctuatorToken.Comma)))
                    return result;

                // const declarations must have an initializer, unless they are part of a
                // for-of/for-in statement.
                if (keyword == KeywordToken.Const && declaration.InitExpression == null)
                    throw new SyntaxErrorException("Missing initializer in const declaration.", this.LineNumber, this.SourcePath);

                // Check if we are at the end of the statement.
                if (this.AtValidEndOfStatement() == true && this.nextToken != PunctuatorToken.Comma)
                    break;

                // Read past the comma token.
                this.Expect(PunctuatorToken.Comma);

                // Keep track of the start of the statement so that source debugging works correctly.
                start = this.PositionAfterWhitespace;
            }

            // Consume the end of the statement.
            this.ExpectEndOfStatement();

            return result;
        }

        /// <summary>
        /// Parses an empty statement.
        /// </summary>
        /// <returns> An empty statement. </returns>
        private Statement ParseEmpty()
        {
            var result = new EmptyStatement(this.labelsForCurrentStatement);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Read past the semicolon.
            this.Expect(PunctuatorToken.Semicolon);

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            return result;
        }

        /// <summary>
        /// Parses an if statement.
        /// </summary>
        /// <returns> An expression representing the if statement. </returns>
        private IfStatement ParseIf()
        {
            var result = new IfStatement(this.labelsForCurrentStatement);

            // Consume the if keyword.
            this.Expect(KeywordToken.If);

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Parse the condition.
            result.Condition = ParseExpression(PunctuatorToken.RightParenthesis);

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Read the statements that will be executed when the condition is true.
            result.IfClause = ParseStatement(addingToExistingBlock: false);

            // Optionally, read the else statement.
            if (this.nextToken == KeywordToken.Else)
            {
                // Consume the else keyword.
                this.Consume();

                // Read the statements that will be executed when the condition is false.
                result.ElseClause = ParseStatement(addingToExistingBlock: false);
            }

            return result;
        }

        /// <summary>
        /// Parses a do statement.
        /// </summary>
        /// <returns> An expression representing the do statement. </returns>
        private DoWhileStatement ParseDo()
        {
            var result = new DoWhileStatement(this.labelsForCurrentStatement);

            // Consume the do keyword.
            this.Expect(KeywordToken.Do);

            // Read the statements that will be executed in the loop body.
            result.Body = ParseStatement(addingToExistingBlock: false);

            // Read the while keyword.
            this.Expect(KeywordToken.While);

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Parse the condition.
            start = this.PositionAfterWhitespace;
            result.ConditionStatement = new ExpressionStatement(ParseExpression(PunctuatorToken.RightParenthesis));
            result.ConditionStatement.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Consume the end of the statement. Note this doesn't use ExpectEndOfStatement()
            // because a semi-colon is not required here.
            if (this.nextToken == PunctuatorToken.Semicolon)
                Consume();

            return result;
        }

        /// <summary>
        /// Parses a while statement.
        /// </summary>
        /// <returns> A while statement. </returns>
        private WhileStatement ParseWhile()
        {
            var result = new WhileStatement(this.labelsForCurrentStatement);

            // Consume the while keyword.
            this.Expect(KeywordToken.While);

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Parse the condition.
            var start = this.PositionAfterWhitespace;
            result.ConditionStatement = new ExpressionStatement(ParseExpression(PunctuatorToken.RightParenthesis));
            result.ConditionStatement.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Read the statements that will be executed in the loop body.
            result.Body = ParseStatement(addingToExistingBlock: false);

            return result;
        }

        /// <summary>
        /// When parsing a for statement, used to keep track of what type it is.
        /// </summary>
        private enum ForStatementType
        {
            Unknown,
            For,
            ForIn,
            ForOf,
        }

        /// <summary>
        /// Parses a for statement, for-in statement, or a for-of statement.
        /// </summary>
        /// <returns> A for statement, for-in statement, or a for-of statement. </returns>
        private Statement ParseFor()
        {
            // Consume the for keyword.
            this.Expect(KeywordToken.For);

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // 'let' variables should have their own scope.
            using (CreateScopeContext(Scope.CreateBlockScope(this.currentScope)))
            {
                // Keep track of the start of the statement so that source debugging works correctly.
                var start = this.PositionAfterWhitespace;

                // There are lots of possibilities for the initialization statement:
                // i = 0;
                // var i = 0, j = 1;
                // var i in
                // var i of
                // let i in
                // let i of
                Statement initStatement = null;
                if (this.nextToken == KeywordToken.Var || this.nextToken == KeywordToken.Let || this.nextToken == KeywordToken.Const)
                {
                    // If the next token is var or const then we know we are parsing a declaration.
                    // This doesn't always work for 'let' unfortunately, because 'let' is only a
                    // reserved word in strict mode.
                    initStatement = ParseVarLetOrConst((KeywordToken)this.nextToken, consumeKeyword: true, insideForLoop: true);
                }
                else if (this.nextToken != PunctuatorToken.Semicolon)
                {
                    // Parse the init statement as an expression.
                    var initExpression = ParseExpression(PunctuatorToken.Semicolon, KeywordToken.In, IdentifierToken.Of);
                    if (this.nextToken is IdentifierToken && initExpression is NameExpression nameExpression && nameExpression.Name == "let")
                    {
                        initStatement = ParseVarLetOrConst(KeywordToken.Let, consumeKeyword: false, insideForLoop: true);
                    }
                    else
                    {
                        initStatement = new ExpressionStatement(initExpression);
                        initStatement.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);
                    }
                }

                // The for-in and for-of expressions need a variable to assign to.  Is null for a regular for statement.
                IReferenceExpression forInOfReference = null;
                if (this.nextToken == KeywordToken.In || this.nextToken == IdentifierToken.Of)
                {
                    // This is a for-in statement or a for-of statement.
                    if (initStatement is ExpressionStatement initExpressionStatement)
                    {
                        if ((initExpressionStatement.Expression is IReferenceExpression) == false)
                            throw new SyntaxErrorException("Invalid left-hand side in for loop.", this.LineNumber, this.SourcePath);
                        forInOfReference = (IReferenceExpression)initExpressionStatement.Expression;
                    }
                    else if (initStatement is VarLetOrConstStatement initVarLetOrConstStatement)
                    {
                        if (initVarLetOrConstStatement.Declarations.Count != 1)
                            throw new SyntaxErrorException("Invalid left-hand side in for loop; must have a single binding.", this.LineNumber, this.SourcePath);
                        forInOfReference = new NameExpression(this.currentScope, initVarLetOrConstStatement.Declarations[0].VariableName);
                    }
                }


                if (this.nextToken == KeywordToken.In)
                {
                    // for (x in y)
                    // for (var x in y)
                    // for (let x in y)
                    var result = new ForInStatement(this.labelsForCurrentStatement);
                    result.Scope = this.currentScope;
                    result.Variable = forInOfReference;
                    result.VariableSourceSpan = initStatement.SourceSpan;

                    // Consume the "in".
                    this.Expect(KeywordToken.In);

                    // Parse the right-hand-side expression.
                    start = this.PositionAfterWhitespace;
                    result.TargetObject = ParseExpression(PunctuatorToken.RightParenthesis);
                    result.TargetObjectSourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

                    // Read the right parenthesis.
                    this.Expect(PunctuatorToken.RightParenthesis);

                    // Read the statements that will be executed in the loop body.
                    result.Body = ParseStatement(addingToExistingBlock: false);

                    return result;
                }
                else if (this.nextToken == IdentifierToken.Of)
                {
                    // for (x of y)
                    // for (var x of y)
                    // for (let x of y)
                    var result = new ForOfStatement(this.labelsForCurrentStatement);
                    result.Scope = this.currentScope;
                    result.Variable = forInOfReference;
                    result.VariableSourceSpan = initStatement.SourceSpan;

                    // Consume the "of".
                    this.Expect(IdentifierToken.Of);

                    // Parse the right-hand-side expression.
                    start = this.PositionAfterWhitespace;
                    result.TargetObject = ParseExpression(PunctuatorToken.RightParenthesis, PunctuatorToken.Comma); // Comma is not allowed.
                    result.TargetObjectSourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

                    // Read the right parenthesis.
                    this.Expect(PunctuatorToken.RightParenthesis);

                    // Read the statements that will be executed in the loop body.
                    result.Body = ParseStatement(addingToExistingBlock: false);

                    return result;
                }
                else
                {
                    var result = new ForStatement(this.labelsForCurrentStatement);
                    result.Scope = this.currentScope;

                    // Set the initialization statement.
                    if (initStatement != null)
                        result.InitStatement = initStatement;

                    // Read the semicolon.
                    this.Expect(PunctuatorToken.Semicolon);

                    // Parse the optional condition expression.
                    // Note: if the condition is omitted then it is considered to always be true.
                    if (this.nextToken != PunctuatorToken.Semicolon)
                    {
                        start = this.PositionAfterWhitespace;
                        result.ConditionStatement = new ExpressionStatement(ParseExpression(PunctuatorToken.Semicolon));
                        result.ConditionStatement.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);
                    }

                    // Read the semicolon.
                    // Note: automatic semicolon insertion never inserts a semicolon in the header of a
                    // for statement.
                    this.Expect(PunctuatorToken.Semicolon);

                    // Parse the optional increment expression.
                    if (this.nextToken != PunctuatorToken.RightParenthesis)
                    {
                        start = this.PositionAfterWhitespace;
                        result.IncrementStatement = new ExpressionStatement(ParseExpression(PunctuatorToken.RightParenthesis));
                        result.IncrementStatement.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);
                    }

                    // Read the right parenthesis.
                    this.Expect(PunctuatorToken.RightParenthesis);

                    // Read the statements that will be executed in the loop body.
                    result.Body = ParseStatement(addingToExistingBlock: false);

                    return result;
                }
            }
        }

        /// <summary>
        /// Parses a continue statement.
        /// </summary>
        /// <returns> A continue statement. </returns>
        private ContinueStatement ParseContinue()
        {
            var result = new ContinueStatement(this.labelsForCurrentStatement);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Consume the continue keyword.
            this.Expect(KeywordToken.Continue);

            // The continue statement can have an optional label to jump to.
            if (this.AtValidEndOfStatement() == false)
            {
                // continue [label]

                // Read the label name.
                result.Label = this.ExpectIdentifier();
            }

            // Consume the semi-colon, if there was one.
            this.ExpectEndOfStatement();

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            return result;
        }

        /// <summary>
        /// Parses a break statement.
        /// </summary>
        /// <returns> A break statement. </returns>
        private BreakStatement ParseBreak()
        {
            var result = new BreakStatement(this.labelsForCurrentStatement);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Consume the break keyword.
            this.Expect(KeywordToken.Break);

            // The break statement can have an optional label to jump to.
            if (this.AtValidEndOfStatement() == false)
            {
                // break [label]

                // Read the label name.
                result.Label = this.ExpectIdentifier();
            }

            // Consume the semi-colon, if there was one.
            this.ExpectEndOfStatement();

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            return result;
        }

        /// <summary>
        /// Parses a return statement.
        /// </summary>
        /// <returns> A return statement. </returns>
        private ReturnStatement ParseReturn()
        {
            if (!IsInFunctionContext)
                throw new SyntaxErrorException("Return statements are only allowed inside functions", this.LineNumber, this.SourcePath);

            var result = new ReturnStatement(this.labelsForCurrentStatement);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Consume the return keyword.
            this.Expect(KeywordToken.Return);

            if (this.AtValidEndOfStatement() == false)
            {
                // Parse the return value expression.
                result.Value = ParseExpression(PunctuatorToken.Semicolon);
            }

            // Consume the end of the statement.
            this.ExpectEndOfStatement();

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            return result;
        }

        /// <summary>
        /// Parses a with statement.
        /// </summary>
        /// <returns> An expression representing the with statement. </returns>
        private WithStatement ParseWith()
        {
            // This statement is not allowed in strict mode.
            if (this.StrictMode == true)
                throw new SyntaxErrorException("The with statement is not supported in strict mode", this.LineNumber, this.SourcePath);

            var result = new WithStatement(this.labelsForCurrentStatement);

            // Read past the "with" token.
            this.Expect(KeywordToken.With);

            // Read a left parenthesis token "(".
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Read an object reference.
            result.WithObject = ParseExpression(PunctuatorToken.RightParenthesis);

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            // Read a right parenthesis token ")".
            this.Expect(PunctuatorToken.RightParenthesis);

            // Create a new scope and assign variables within the with statement to the scope.
            result.WithScope = Scope.CreateWithScope(this.currentScope);
            using (CreateScopeContext(result.WithScope))
            {
                // Read the body of the with statement.
                result.Body = ParseStatement(addingToExistingBlock: false);
            }

            return result;
        }

        /// <summary>
        /// Parses a switch statement.
        /// </summary>
        /// <returns> A switch statement. </returns>
        private SwitchStatement ParseSwitch()
        {
            var result = new SwitchStatement(this.labelsForCurrentStatement);

            // Consume the switch keyword.
            this.Expect(KeywordToken.Switch);

            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Parse the switch expression.
            result.Value = ParseExpression(PunctuatorToken.RightParenthesis);

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Consume the start brace ({).
            this.Expect(PunctuatorToken.LeftBrace);

            SwitchCase defaultClause = null;
            while (true)
            {
                if (this.nextToken == KeywordToken.Case)
                {
                    var caseClause = new SwitchCase();

                    // Read the case keyword.
                    this.Expect(KeywordToken.Case);

                    // Parse the case expression.
                    caseClause.Value = ParseExpression(PunctuatorToken.Colon);

                    // Consume the colon.
                    this.Expect(PunctuatorToken.Colon);

                    // Zero or more statements can be added to the case statement.
                    while (this.nextToken != KeywordToken.Case && this.nextToken != KeywordToken.Default && this.nextToken != PunctuatorToken.RightBrace)
                        caseClause.BodyStatements.Add(ParseStatement(addingToExistingBlock: true));

                    // Add the case clause to the switch statement.
                    result.CaseClauses.Add(caseClause);
                }
                else if (this.nextToken == KeywordToken.Default)
                {
                    // Make sure this is the only default clause.
                    if (defaultClause != null)
                        throw new SyntaxErrorException("Only one default clause is allowed.", this.LineNumber, this.SourcePath);

                    defaultClause = new SwitchCase();

                    // Read the case keyword.
                    this.Expect(KeywordToken.Default);

                    // Consume the colon.
                    this.Expect(PunctuatorToken.Colon);

                    // Zero or more statements can be added to the case statement.
                    while (this.nextToken != KeywordToken.Case && this.nextToken != KeywordToken.Default && this.nextToken != PunctuatorToken.RightBrace)
                        defaultClause.BodyStatements.Add(ParseStatement(addingToExistingBlock: true));

                    // Add the default clause to the switch statement.
                    result.CaseClauses.Add(defaultClause);
                }
                else if (this.nextToken == PunctuatorToken.RightBrace)
                {
                    break;
                }
                else
                {
                    // Statements cannot be added directly after the switch.
                    throw new SyntaxErrorException("Expected 'case' or 'default'.", this.LineNumber, this.SourcePath);
                }
            }

            // Consume the end brace.
            this.Consume();

            return result;
        }

        /// <summary>
        /// Parses a throw statement.
        /// </summary>
        /// <returns> A throw statement. </returns>
        private ThrowStatement ParseThrow()
        {
            var result = new ThrowStatement(this.labelsForCurrentStatement);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Consume the throw keyword.
            this.Expect(KeywordToken.Throw);

            // A line terminator is not allowed here.
            if (this.consumedLineTerminator == true)
                throw new SyntaxErrorException("Illegal newline after throw", this.LineNumber, this.SourcePath);

            // Parse the expression to throw.
            result.Value = ParseExpression(PunctuatorToken.Semicolon);

            // Consume the end of the statement.
            this.ExpectEndOfStatement();

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            return result;
        }

        /// <summary>
        /// Parses a try statement.
        /// </summary>
        /// <returns> A try-catch-finally statement. </returns>
        private TryCatchFinallyStatement ParseTry()
        {
            var result = new TryCatchFinallyStatement(this.labelsForCurrentStatement);

            // Consume the try keyword.
            this.Expect(KeywordToken.Try);

            // Parse the try block.
            result.TryBlock = ParseBlock();

            // The next token is either 'catch' or 'finally'.
            if (this.nextToken == KeywordToken.Catch)
            {
                // Consume the catch token.
                this.Expect(KeywordToken.Catch);

                if (this.nextToken == PunctuatorToken.LeftBrace)
                {
                    // catch { }

                    // Parse the statements inside the catch block.
                    result.CatchBlock = ParseBlock();
                }
                else
                {
                    // catch (e) { }

                    // Read the left parenthesis.
                    this.Expect(PunctuatorToken.LeftParenthesis);

                    // Read the name of the variable to assign the exception to.
                    result.CatchVariableName = this.ExpectIdentifier();
                    this.ValidateVariableName(result.CatchVariableName);

                    // Read the right parenthesis.
                    this.Expect(PunctuatorToken.RightParenthesis);

                    // Parse the statements inside the catch block.
                    result.CatchBlock = ParseBlock();

                    // Add the catch variable to the scope.
                    result.CatchBlock.Scope.DeclareVariable(KeywordToken.Let, result.CatchVariableName);
                }
            }

            if (this.nextToken == KeywordToken.Finally)
            {
                // Consume the finally token.
                this.Expect(KeywordToken.Finally);

                // Read the finally statements.
                result.FinallyBlock = ParseBlock();
            }

            // There must be a catch or finally block.
            if (result.CatchBlock == null && result.FinallyBlock == null)
                throw new SyntaxErrorException("Missing catch or finally after try", this.LineNumber, this.SourcePath);

            return result;
        }

        /// <summary>
        /// Parses a debugger statement.
        /// </summary>
        /// <returns> A debugger statement. </returns>
        private DebuggerStatement ParseDebugger()
        {
            var result = new DebuggerStatement(this.labelsForCurrentStatement);

            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Consume the debugger keyword.
            this.Expect(KeywordToken.Debugger);

            // Consume the end of the statement.
            this.ExpectEndOfStatement();

            // Record the portion of the source document that will be highlighted when debugging.
            result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

            return result;
        }

        /// <summary>
        /// Parses a function declaration.
        /// </summary>
        /// <returns> A statement representing the function. </returns>
        private Statement ParseFunctionDeclaration()
        {
            // Record the start of the function.
            var startPosition = this.PositionAfterWhitespace;

            // Consume the function keyword.
            this.Expect(KeywordToken.Function);

            // Read the function name.
            var functionName = this.ExpectIdentifier();
            ValidateVariableName(functionName);

            // Parse the function declaration.
            var expression = ParseFunction(
                        functionType: FunctionDeclarationType.Declaration,
                        parentScope: this.initialScope,
                        name: new PropertyName(functionName),
                        startPosition: startPosition,
                        codeContext: CodeContext.Function);

            // Add the function to the top-level scope.
            this.currentScope.DeclareVariable(KeywordToken.Var, functionName, hoistedFunction: expression);

            // Function declarations do nothing at the point of declaration - everything happens
            // at the top of the function/global code.
            return new EmptyStatement(this.labelsForCurrentStatement);
        }

        /// <summary>
        /// Parses a function declaration or a function expression.
        /// </summary>
        /// <param name="functionType"> The type of function to parse. </param>
        /// <param name="parentScope"> The parent scope for the function. </param>
        /// <param name="name"> The name of the function (can be computed at runtime). </param>
        /// <param name="startPosition"> The position of the start of the function. </param>
        /// <param name="codeContext"> Indicates the parsing context. </param>
        /// <returns> A function expression. </returns>
        private FunctionExpression ParseFunction(FunctionDeclarationType functionType, Scope parentScope, PropertyName name, SourceCodePosition startPosition, CodeContext codeContext)
        {
            // Read the left parenthesis.
            this.Expect(PunctuatorToken.LeftParenthesis);

            // Create a new scope and assign variables within the function body to the scope.
            var functionName = !name.IsGetter && !name.IsSetter && name.HasStaticName &&
                codeContext != CodeContext.Constructor && codeContext != CodeContext.DerivedConstructor ? name.StaticName : null;
            var scope = Scope.CreateFunctionScope(functionName, null);

            // Replace scope and methodOptimizationHints.
            var originalScope = this.currentScope;
            var originalMethodOptimizationHints = this.methodOptimizationHints;
            var newMethodOptimizationHints = new MethodOptimizationHints();
            this.methodOptimizationHints = newMethodOptimizationHints;
            this.currentScope = scope;

            // Read zero or more arguments.
            var arguments = ParseFunctionArguments(PunctuatorToken.RightParenthesis);

            // Restore scope and methodOptimizationHints.
            this.methodOptimizationHints = originalMethodOptimizationHints;
            this.currentScope = originalScope;

            // Getters must have zero arguments.
            if (name.IsGetter && arguments.Count != 0)
                throw new SyntaxErrorException("Getters cannot have arguments", this.LineNumber, this.SourcePath);

            // Setters must have one argument.
            if (name.IsSetter && arguments.Count != 1)
                throw new SyntaxErrorException("Setters must have a single argument", this.LineNumber, this.SourcePath);

            // Read the right parenthesis.
            this.Expect(PunctuatorToken.RightParenthesis);

            // Since the parser reads one token in advance, start capturing the function body here.
            var bodyTextBuilder = new System.Text.StringBuilder();
            var originalBodyTextBuilder = this.lexer.InputCaptureStringBuilder;
            this.lexer.InputCaptureStringBuilder = bodyTextBuilder;

            // Read the start brace.
            this.Expect(PunctuatorToken.LeftBrace);

            // This context has a nested function.
            this.methodOptimizationHints.HasNestedFunction = true;

            // Read the function body.
            var functionParser = CreateFunctionBodyParser(this, scope, newMethodOptimizationHints, codeContext);
            var body = functionParser.Parse();

            // Transfer state back from the function parser.
            this.nextToken = functionParser.nextToken;
            this.lexer.StrictMode = this.StrictMode;
            this.lexer.InputCaptureStringBuilder = originalBodyTextBuilder;
            if (originalBodyTextBuilder != null)
                originalBodyTextBuilder.Append(bodyTextBuilder);

            SourceCodePosition endPosition;
            if (functionType == FunctionDeclarationType.Expression)
            {
                // The end token '}' will be consumed by the parent function.
                if (this.nextToken != PunctuatorToken.RightBrace)
                    throw new SyntaxErrorException("Expected '}'", this.LineNumber, this.SourcePath);

                // Record the end of the function body.
                endPosition = new SourceCodePosition(this.PositionAfterWhitespace.Line, this.PositionAfterWhitespace.Column + 1);
            }
            else
            {
                // Consume the '}'.
                this.Expect(PunctuatorToken.RightBrace);

                // Record the end of the function body.
                endPosition = this.PositionBeforeWhitespace;
            }

            // Create a new function expression.
            var options = this.options.Clone();
            options.ForceStrictMode = functionParser.StrictMode;
            var context = new FunctionMethodGenerator(name, functionType, arguments,
                bodyTextBuilder.ToString(0, bodyTextBuilder.Length - 1), body, scope,
                this.SourcePath, new SourceCodeSpan(startPosition, endPosition), options);
            context.MethodOptimizationHints = functionParser.methodOptimizationHints;
            return new FunctionExpression(context, this.currentScope);
        }

        /// <summary>
        /// Parses a comma-separated list of function arguments.
        /// </summary>
        /// <param name="endToken"> The token that ends parsing. </param>
        /// <returns> A list of parsed arguments. </returns>
        internal List<FunctionArgument> ParseFunctionArguments(Token endToken)
        {
            var arguments = new List<FunctionArgument>();
            if (this.nextToken != endToken)
            {
                while (true)
                {
                    // Read the argument name.
                    var argument = new FunctionArgument();
                    argument.Name = this.ExpectIdentifier();
                    ValidateVariableName(argument.Name);

                    // Check if the argument has a default value.
                    if (this.nextToken == PunctuatorToken.Assignment)
                    {
                        // Read past the assignment token.
                        Consume();

                        // Parse the expression that follows.
                        argument.DefaultValue = ParseExpression(PunctuatorToken.Comma, endToken);
                    }

                    // Add the variable to the scope so that it can be used in the default value of
                    // the next argument.  Do this *after* parsing the default value.
                    this.currentScope.DeclareVariable(KeywordToken.Var, argument.Name);

                    // Add the argument to the list.
                    arguments.Add(argument);

                    if (this.nextToken == PunctuatorToken.Comma)
                    {
                        // Consume the comma.
                        this.Consume();
                    }
                    else if (this.nextToken == endToken)
                        break;
                    else
                        throw new SyntaxErrorException("Expected ',' or ')'", this.LineNumber, this.SourcePath);
                }
            }
            return arguments;
        }

        /// <summary>
        /// Parses a statement consisting of an expression or starting with a label.  These two
        /// cases are disambiguated here.
        /// </summary>
        /// <returns> A statement. </returns>
        private Statement ParseLabelOrExpressionStatement()
        {
            // Keep track of the start of the statement so that source debugging works correctly.
            var start = this.PositionAfterWhitespace;

            // Parse the statement as though it was an expression - but stop if there is an unexpected colon.
            var expression = ParseExpression(PunctuatorToken.Semicolon, PunctuatorToken.Colon);

            if (this.nextToken == PunctuatorToken.Colon && expression is NameExpression)
            {
                // The expression is actually a label.

                // Extract the label name.
                var labelName = ((NameExpression)expression).Name;
                this.labelsForCurrentStatement.Add(labelName);

                // Read past the colon.
                this.Expect(PunctuatorToken.Colon);

                // Read the rest of the statement.
                return ParseStatementNoNewContext();
            }
            else if (this.nextToken is IdentifierToken && expression is NameExpression nameExpression && nameExpression.Name == "let")
            {
                return ParseVarLetOrConst(KeywordToken.Let, consumeKeyword: false);
            }
            else
            {

                // Consume the end of the statement.
                this.ExpectEndOfStatement();

                // Create a new expression statement.
                var result = new ExpressionStatement(this.labelsForCurrentStatement, expression);

                // Record the portion of the source document that will be highlighted when debugging.
                result.SourceSpan = new SourceCodeSpan(start, this.PositionBeforeWhitespace);

                return result;
            }
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
        /// Initializes the token -> operator mapping dictionary.
        /// </summary>
        /// <returns> The token -> operator mapping dictionary. </returns>
        private static Dictionary<OperatorKey, Operator> InitializeOperatorLookup()
        {
            var result = new Dictionary<OperatorKey, Operator>(55);
            foreach (var @operator in Operator.AllOperators)
            {
                result.Add(new OperatorKey() { Token = @operator.Token, PostfixOrInfix = @operator.HasLHSOperand }, @operator);
                if (@operator.SecondaryToken != null)
                {
                    // Note: the secondary token for the grouping operator and function call operator ')' is a duplicate.
                    result[new OperatorKey() { Token = @operator.SecondaryToken, PostfixOrInfix = @operator.HasRHSOperand }] = @operator;
                    if (@operator.InnerOperandIsOptional == true)
                        result[new OperatorKey() { Token = @operator.SecondaryToken, PostfixOrInfix = false }] = @operator;
                }
            }
            return result;
        }

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
            Operator result;
            if (operatorLookup == null)
            {
                // Initialize the operator lookup table.
                var temp = InitializeOperatorLookup();
                System.Threading.Thread.MemoryBarrier();
                operatorLookup = temp;
            }
            if (operatorLookup.TryGetValue(new OperatorKey() { Token = token, PostfixOrInfix = postfixOrInfix }, out result) == false)
            {
                // Tagged template literals are treated like function calls.
                if (token is TemplateLiteralToken)
                    return Operator.FunctionCall;
                return null;
            }
            return result;
        }

        /// <summary>
        /// Parses a javascript expression.
        /// </summary>
        /// <param name="endTokens"> One or more tokens that indicate the end of the expression. </param>
        /// <returns> An expression tree that represents the expression. </returns>
        private Expression ParseExpression(params Token[] endTokens)
        {
            // The root of the expression tree.
            Expression root = null;

            // The active operator, i.e. the one last encountered.
            OperatorExpression unboundOperator = null;

            while (this.nextToken != null)
            {
                if ((this.nextToken is LiteralToken && (!(this.nextToken is TemplateLiteralToken) || this.expressionState == ParserExpressionState.Literal)) ||
                    this.nextToken is IdentifierToken ||
                    this.nextToken == KeywordToken.Function ||
                    this.nextToken == KeywordToken.Class ||
                    this.nextToken == KeywordToken.This ||
                    this.nextToken == KeywordToken.Super ||
                    (this.expressionState == ParserExpressionState.Literal &&
                        (this.nextToken == PunctuatorToken.LeftBrace || this.nextToken == PunctuatorToken.LeftBracket)) ||
                    (this.nextToken is KeywordToken && unboundOperator != null && unboundOperator.OperatorType == OperatorType.MemberAccess && this.expressionState == ParserExpressionState.Literal))
                {
                    // If a literal was found where an operator was expected, insert a semi-colon
                    // automatically (if this would fix the error and a line terminator was
                    // encountered) or throw an error.
                    if (this.expressionState != ParserExpressionState.Literal)
                    {
                        // Check for automatic semi-colon insertion.
                        if (Array.IndexOf(endTokens, PunctuatorToken.Semicolon) >= 0 && this.consumedLineTerminator == true)
                            break;

                        // Check for 'of' contextual keyword.
                        if (Array.IndexOf(endTokens, IdentifierToken.Of) >= 0)
                            break;

                        // Check for 'let' contextual keyword.
                        if (root is NameExpression nameExpression && nameExpression.Name == "let" && this.nextToken is IdentifierToken)
                            break;

                        throw new SyntaxErrorException(string.Format("Expected operator but found {0}", Token.ToText(this.nextToken)), this.LineNumber, this.SourcePath);
                    }

                    // New in ECMAScript 5 is the ability to use keywords as property names.
                    if ((this.nextToken is KeywordToken || (this.nextToken is LiteralToken && ((LiteralToken)this.nextToken).IsKeyword == true)) &&
                        unboundOperator != null &&
                        unboundOperator.OperatorType == OperatorType.MemberAccess &&
                        this.expressionState == ParserExpressionState.Literal)
                    {
                        this.nextToken = IdentifierToken.Create(this.nextToken.Text);
                    }

                    Expression terminal;
                    if (this.nextToken is LiteralToken)
                    {
                        if (this.nextToken is TemplateLiteralToken && ((TemplateLiteralToken)this.nextToken).SubstitutionFollows)
                        {
                            // Handle template literals with substitutions (template literals
                            // without substitutions are parsed the same as regular strings).
                            terminal = ParseTemplateLiteral();
                        }
                        else
                        {
                            // Otherwise, it's a simple literal.
                            terminal = new LiteralExpression(((LiteralToken)this.nextToken).Value);
                        }
                    }
                    else if (this.nextToken is IdentifierToken)
                    {
                        // If the token is an identifier, convert it to a NameExpression.
                        var identifierName = ((IdentifierToken)this.nextToken).Name;
                        terminal = new NameExpression(this.currentScope, identifierName);

                        // Record each occurance of a variable name.
                        if (unboundOperator == null || unboundOperator.OperatorType != OperatorType.MemberAccess)
                            this.methodOptimizationHints.EncounteredVariable(identifierName);
                    }
                    else if (this.nextToken == KeywordToken.This)
                    {
                        // Convert "this" to an expression.
                        terminal = new ThisExpression();

                        // Add method optimization info.
                        this.methodOptimizationHints.HasThis = true;
                    }
                    else if (this.nextToken == KeywordToken.Super)
                    {
                        // Convert "super" to an expression.
                        terminal = new SuperExpression();

                        // Add method optimization info.
                        this.methodOptimizationHints.HasThis = true;
                    }
                    else if (this.nextToken == PunctuatorToken.LeftBracket)
                        // Array literal.
                        terminal = ParseArrayLiteral();
                    else if (this.nextToken == PunctuatorToken.LeftBrace)
                        // Object literal.
                        terminal = ParseObjectLiteral();
                    else if (this.nextToken == KeywordToken.Function)
                        terminal = ParseFunctionExpression();
                    else if (this.nextToken == KeywordToken.Class)
                        terminal = ParseClassExpression();
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
                else if (this.nextToken is PunctuatorToken ||
                         this.nextToken is KeywordToken ||
                        (this.nextToken is TemplateLiteralToken && this.expressionState == ParserExpressionState.Operator))
                {
                    // The token is an operator (o1).
                    Operator newOperator = OperatorFromToken(this.nextToken, postfixOrInfix: this.expressionState == ParserExpressionState.Operator);

                    // Make sure the token is actually an operator and not just a random keyword.
                    if (newOperator == null)
                    {
                        if (this.nextToken == PunctuatorToken.Dot &&
                            unboundOperator != null &&
                            unboundOperator.OperatorType == OperatorType.New &&
                            unboundOperator.AcceptingOperands &&
                            expressionState == ParserExpressionState.Literal)
                        {
                            // new.target is a pseudo-property, ugh.
                            Consume();
                            if (this.nextToken is IdentifierToken identifierToken && identifierToken.Name == "target")
                            {
                                if (!IsInFunctionContext)
                                    throw new SyntaxErrorException("new.target expression is not allowed here.", this.LineNumber, this.SourcePath);
                                Consume();
                                if (root == unboundOperator)
                                {
                                    root = new NewTargetExpression();
                                    unboundOperator = null;
                                }
                                else
                                {
                                    var node = root as OperatorExpression;
                                    while (node != null)
                                    {
                                        if (node.RightBranch == unboundOperator)
                                        {
                                            node.Pop();
                                            node.Push(new NewTargetExpression());
                                            unboundOperator = node;
                                            break;
                                        }
                                        node = node.RightBranch;
                                    }
                                }
                                expressionState = ParserExpressionState.Operator;
                                continue;
                            }
                            else
                                throw new SyntaxErrorException("Expected 'target'", this.LineNumber, this.SourcePath);
                        }

                        // Check if the token is an end token, for example a semi-colon.
                        if (Array.IndexOf(endTokens, this.nextToken) >= 0)
                            break;

                        // Check for automatic semi-colon insertion.
                        if (Array.IndexOf(endTokens, PunctuatorToken.Semicolon) >= 0 && (this.consumedLineTerminator == true || this.nextToken == PunctuatorToken.RightBrace))
                            break;

                        throw new SyntaxErrorException(string.Format("Unexpected token {0} in expression.", Token.ToText(this.nextToken)), this.LineNumber, this.SourcePath);
                    }

                    // Post-fix increment and decrement cannot have a line terminator in between
                    // the operator and the operand.
                    if (this.consumedLineTerminator == true && (newOperator == Operator.PostIncrement || newOperator == Operator.PostDecrement))
                        break;

                    // There are four possibilities:
                    // 1. The token is the second of a two-part operator (for example, the ':' in a
                    //    conditional operator.  In this case, we need to go up the tree until we find
                    //    an instance of the operator and make that the active unbound operator.
                    if (this.nextToken == newOperator.SecondaryToken)
                    {
                        // Traverse down the tree looking for the parent operator that corresponds to
                        // this token.
                        OperatorExpression parentExpression = null;
                        var node = root as OperatorExpression;
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
                            throw new SyntaxErrorException("Mismatched closing token in expression.", this.LineNumber, this.SourcePath);
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
                            var node = root as OperatorExpression;
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
                        var newExpression = OperatorExpression.FromOperator(newOperator, this.currentScope);

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
                                // Two operators in a row is problematic if the left operator is
                                // higher precedence than the right operator (e.g. "new ++a"), so
                                // this is disallowed by the spec.
                                if (unboundOperator.Precedence > newOperator.Precedence)
                                    throw new SyntaxErrorException($"Unexpected token {Token.ToText(this.nextToken)} in expression", this.LineNumber, this.SourcePath);
                                unboundOperator.Push(newExpression);
                            }
                            else
                            {
                                // "5 !" or "5 + 5 !"
                                // Check for automatic semi-colon insertion.
                                if (Array.IndexOf(endTokens, PunctuatorToken.Semicolon) >= 0 && this.consumedLineTerminator == true)
                                    break;
                                throw new SyntaxErrorException("Invalid use of prefix operator.", this.LineNumber, this.SourcePath);
                            }
                        }
                        else
                        {
                            // Search up the tree for an operator that has a lower precedence.
                            // Because we don't store the parent link, we have to traverse down the
                            // tree and take the last one we find instead.
                            OperatorExpression lowPrecedenceOperator = null;
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
                                var node = root as OperatorExpression;
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
                                    throw new SyntaxErrorException("Invalid use of prefix operator.", this.LineNumber, this.SourcePath);
                                }

                                // Two operators in a row is problematic if the right operator is
                                // higher precedence than the left operator (e.g. "a++[0]"), so
                                // this is disallowed by the spec.
                                if (lowPrecedenceOperator.Operator.HasLHSOperand && !lowPrecedenceOperator.Operator.HasRHSOperand &&
                                    newOperator.Precedence > lowPrecedenceOperator.Precedence)
                                {
                                    throw new SyntaxErrorException($"Unexpected token {Token.ToText(this.nextToken)} in expression", this.LineNumber, this.SourcePath);
                                }

                                newExpression.Push(lowPrecedenceOperator.Pop());
                                lowPrecedenceOperator.Push(newExpression);
                            }
                        }

                        if (this.nextToken is TemplateLiteralToken)
                        {
                            // Read the rest of the template literal, and add the strings and
                            // values as a TemplateLiteralExpression, which will turned into
                            // function arguments by the FunctionCallExpression.
                            newExpression.Push(ParseTemplateLiteral());

                            // Make sure no more operands are added to the operator.
                            newExpression.SecondTokenEncountered = true;
                            unboundOperator = null;
                        }
                        else
                        {
                            unboundOperator = newExpression;
                        }
                    }
                }
                else
                {
                    throw new SyntaxErrorException(string.Format("Unexpected token {0} in expression", Token.ToText(this.nextToken)), this.LineNumber, this.SourcePath);
                }

                // Read the next token.
                this.Consume(root != null && (unboundOperator == null || unboundOperator.AcceptingOperands == false) ? ParserExpressionState.Operator : ParserExpressionState.Literal);
            }

            // Empty expressions are invalid.
            if (root == null)
                throw new SyntaxErrorException(string.Format("Expected an expression but found {0} instead", Token.ToText(this.nextToken)), this.LineNumber, this.SourcePath);

            // Check the AST is valid.
            root.CheckValidity(this.context, this.LineNumber, this.SourcePath);

            // A literal is the next valid expression token.
            this.expressionState = ParserExpressionState.Literal;
            this.lexer.ParserExpressionState = expressionState;

            // Resolve all the unbound operators into real operators.
            return root;
        }

        /// <summary>
        /// Parses an array literal (e.g. "[1, 2]").
        /// </summary>
        /// <returns> A literal expression that represents the array literal. </returns>
        private ArrayLiteralExpression ParseArrayLiteral()
        {
            // Read past the initial '[' token.
            Debug.Assert(this.nextToken == PunctuatorToken.LeftBracket);
            this.Consume();

            var items = new List<Expression>();
            while (true)
            {
                // If the next token is ']', then the array literal is complete.
                if (this.nextToken == PunctuatorToken.RightBracket)
                    break;

                // If the next token is ',', then the array element is undefined.
                if (this.nextToken == PunctuatorToken.Comma)
                    items.Add(null);
                else
                    // Otherwise, read the next item in the array.
                    items.Add(ParseExpression(PunctuatorToken.Comma, PunctuatorToken.RightBracket));

                // Read past the comma.
                Debug.Assert(this.nextToken == PunctuatorToken.Comma || this.nextToken == PunctuatorToken.RightBracket);
                if (this.nextToken == PunctuatorToken.Comma)
                    this.Consume();
            }

            // The end token ']' will be consumed by the parent function.
            Debug.Assert(this.nextToken == PunctuatorToken.RightBracket);

            return new ArrayLiteralExpression(items);
        }

        /// <summary>
        /// Parses an object literal (e.g. "{a: 5}").
        /// </summary>
        /// <returns> A literal expression that represents the object literal. </returns>
        private ObjectLiteralExpression ParseObjectLiteral()
        {
            // Read past the initial '{' token.
            Debug.Assert(this.nextToken == PunctuatorToken.LeftBrace);
            this.Consume();

            var properties = new List<PropertyDeclaration>();
            while (true)
            {
                // If the next token is '}', then the object literal is complete.
                if (this.nextToken == PunctuatorToken.RightBrace)
                    break;

                // Record the position in case it's a function.
                var startPosition = this.PositionAfterWhitespace;

                // Read the property name.
                var propertyName = ReadPropertyName(PropertyNameContext.ObjectLiteral);

                // Check if this is a getter or setter.
                Expression propertyValue;
                if (propertyName.IsGetter || propertyName.IsSetter)
                {
                    // Parse the function body.
                    var function = ParseFunction(
                        functionType: FunctionDeclarationType.Declaration,
                        parentScope: this.currentScope,
                        name: propertyName,
                        startPosition: startPosition,
                        codeContext: CodeContext.ObjectLiteralFunction);

                    // Add the getter or setter to the list of properties to set.
                    properties.Add(new PropertyDeclaration(propertyName, function));
                }
                else if (propertyName.HasStaticName && (this.nextToken == PunctuatorToken.Comma || this.nextToken == PunctuatorToken.RightBrace))
                {
                    // This is a shorthand property e.g. "var a = 1, b = 2, c = { a, b }" is the
                    // same as "var a = 1, b = 2, c = { a: a, b: b }".
                    properties.Add(new PropertyDeclaration(propertyName, new NameExpression(this.currentScope, propertyName.StaticName)));
                }
                else if (this.nextToken == PunctuatorToken.LeftParenthesis)
                {
                    // This is a shorthand function e.g. "var a = { b() { return 2; } }" is the
                    // same as "var a = { b: function() { return 2; } }".

                    // Parse the function.
                    var function = ParseFunction(
                        functionType: FunctionDeclarationType.Expression,
                        parentScope: this.currentScope,
                        name: propertyName,
                        startPosition: startPosition,
                        codeContext: CodeContext.ObjectLiteralFunction);

                    // Strangely enough, if declarationType is Expression then the last right
                    // brace ('}') is not consumed.
                    this.Expect(PunctuatorToken.RightBrace);

                    // Add the function to the list of properties to set.
                    properties.Add(new PropertyDeclaration(propertyName, function));
                }
                else
                {
                    // This is a regular property.
                    
                    // Read the colon.
                    this.Expect(PunctuatorToken.Colon);

                    // Now read the property value.
                    propertyValue = ParseExpression(PunctuatorToken.Comma, PunctuatorToken.RightBrace);

                    // Add the property to the list.
                    properties.Add(new PropertyDeclaration(propertyName, propertyValue));
                }

                // Read past the comma.
                Debug.Assert(this.nextToken == PunctuatorToken.Comma || this.nextToken == PunctuatorToken.RightBrace);
                if (this.nextToken == PunctuatorToken.Comma)
                    this.Consume();
            }

            // The end token '}' will be consumed by the parent function.
            Debug.Assert(this.nextToken == PunctuatorToken.RightBrace);

            return new ObjectLiteralExpression(properties);
        }

        private enum PropertyNameContext
        {
            ObjectLiteral,
            ClassBody,
            AfterStatic,
            AfterGetOrSet,
        }

        /// <summary>
        /// Reads a property name, as used in object literals and class bodies.
        /// </summary>
        /// <param name="context">  </param>
        /// <returns> Details on the property name. </returns>
        private PropertyName ReadPropertyName(PropertyNameContext context)
        {
            // Read and consume the next token.
            var token = this.nextToken;
            Consume();

            if (token is LiteralToken literalToken)
            {
                // The property name can be a string or a number or (in ES5) a keyword like false, true or null.
                return new PropertyName(literalToken.ToPropertyName());
            }
            else if (token is IdentifierToken identifierToken)
            {
                // An identifier is also okay.
                // Note: that { get() { return 1; } } is valid (creates a function called 'get'),
                // and { get get() { return 1; } } is valid (creates a getter called 'get'),
                // but { get get get() { return 1; } } is not.
                if (context != PropertyNameContext.AfterGetOrSet)
                {
                    var flags = PropertyNameFlags.None;
                    if (identifierToken.Name == "get")
                        flags = PropertyNameFlags.Get;
                    else if (identifierToken.Name == "set")
                        flags = PropertyNameFlags.Set;
                    if (flags != PropertyNameFlags.None && (!(this.nextToken is PunctuatorToken) || this.nextToken == PunctuatorToken.LeftBracket))
                        return ReadPropertyName(PropertyNameContext.AfterGetOrSet).WithFlags(flags);
                }
                return new PropertyName(identifierToken.Name);
            }
            else if (token is KeywordToken keywordToken)
            {
                // In ES5 a keyword like 'if' is also okay.
                if (keywordToken == KeywordToken.Static && context == PropertyNameContext.ClassBody &&
                    (!(this.nextToken is PunctuatorToken) || this.nextToken == PunctuatorToken.LeftBracket))
                    return ReadPropertyName(PropertyNameContext.AfterStatic).WithFlags(PropertyNameFlags.Static);
                return new PropertyName(keywordToken.Name);
            }
            else if (token == PunctuatorToken.LeftBracket)
            {
                // ES6 computed property.
                var expression = ParseExpression(PunctuatorToken.RightBracket);
                this.Consume();
                return new PropertyName(expression);
            }
            else
                throw new SyntaxErrorException(string.Format("Expected property name but found {0}", Token.ToText(this.nextToken)), this.LineNumber, this.SourcePath);
        }

        /// <summary>
        /// Parses a function expression.
        /// </summary>
        /// <returns> A function expression. </returns>
        private FunctionExpression ParseFunctionExpression()
        {
            // Record the start of the function.
            var startPosition = this.PositionAfterWhitespace;

            // Consume the function keyword.
            this.Expect(KeywordToken.Function);

            // The function name is optional for function expressions.
            var functionName = string.Empty;
            if (this.nextToken is IdentifierToken)
            {
                functionName = this.ExpectIdentifier();
                ValidateVariableName(functionName);
            }

            // Parse the rest of the function.
            return ParseFunction(
                    functionType: FunctionDeclarationType.Expression,
                    parentScope: this.currentScope,
                    name: new PropertyName(functionName),
                    startPosition: startPosition,
                    codeContext: CodeContext.Function);
        }

        /// <summary>
        /// Parses a template literal (e.g. `Bought ${count} items`).
        /// </summary>
        /// <returns> An expression that represents the template literal. </returns>
        private Expression ParseTemplateLiteral()
        {
            Debug.Assert(this.nextToken is TemplateLiteralToken);

            var strings = new List<string>();
            var values = new List<Expression>();
            var rawStrings = new List<string>();
            while (true)
            {
                // Record the template literal token value.
                var templateLiteralToken = (TemplateLiteralToken)this.nextToken;
                strings.Add(templateLiteralToken.Value);
                rawStrings.Add(templateLiteralToken.RawText);

                // Check if we are at the end of the template literal.
                if (templateLiteralToken.SubstitutionFollows == false)
                    break;

                // Parse the substitution.
                this.Consume();     // Consume the template literal token.
                values.Add(ParseExpression(PunctuatorToken.RightBrace));

                // Consume the right brace, and continue scanning the template literal.
                // The TemplateContinuation option here indicates that the lexer should immediately
                // start constructing a template literal token.
                this.Consume(ParserExpressionState.TemplateContinuation);
            }

            // If this is an untagged template literal, return a UntaggedTemplateExpression.
            return new TemplateLiteralExpression(strings, values, rawStrings);
        }



        /// <summary>
        /// Parses a class declaration.
        /// </summary>
        /// <returns> A statement representing the class. </returns>
        private Statement ParseClassDeclaration()
        {
            // Record the start of the function.
            var startPosition = this.PositionAfterWhitespace;

            // Consume the function keyword.
            this.Expect(KeywordToken.Class);

            // Read the class name.
            var className = this.ExpectIdentifier();
            ValidateVariableName(className);

            // Parse the extends bit.
            Expression extends = null;
            if (this.nextToken == KeywordToken.Extends)
            {
                this.Consume();
                extends = ParseExpression(PunctuatorToken.LeftBrace);
            }

            // Parse the class body.
            var classExpression = ParseClassBody(className, extends, startPosition);

            // Consume the end token.
            this.Consume();

            // For 'class A' construct an expression like 'A = class A'.
            this.currentScope.DeclareVariable(KeywordToken.Let, className);
            var result = new ExpressionStatement(this.labelsForCurrentStatement,
                new AssignmentExpression(this.currentScope, className, classExpression));
            result.SourceSpan = new SourceCodeSpan(startPosition, this.PositionBeforeWhitespace);
            return result;
        }

        /// <summary>
        /// Parses a class expression.
        /// </summary>
        /// <returns> A class expression. </returns>
        private ClassExpression ParseClassExpression()
        {
            // Record the start of the function.
            var startPosition = this.PositionAfterWhitespace;

            // Consume the function keyword.
            this.Expect(KeywordToken.Class);

            // The class name is optional for class expressions.
            string className = null;
            if (this.nextToken is IdentifierToken)
            {
                className = this.ExpectIdentifier();
                ValidateVariableName(className);
            }

            // Parse the extends bit.
            Expression extends = null;
            if (this.nextToken == KeywordToken.Extends)
            {
                this.Consume();
                extends = ParseExpression(PunctuatorToken.LeftBrace);
            }

            // Parse the rest of the class.
            return ParseClassBody(className, extends, startPosition);
        }

        /// <summary>
        /// Parses the body of a class declaration or a class expression.
        /// </summary>
        /// <param name="className"> The name of the class (can be empty). </param>
        /// <param name="extends"> The base class, or <c>null</c> if this class doesn't inherit
        /// from another class. </param>
        /// <param name="startPosition"> The position of the start of the function. </param>
        /// <returns> A class expression. </returns>
        private ClassExpression ParseClassBody(string className, Expression extends, SourceCodePosition startPosition)
        {
            // The contents of the class should all be considered to be strict mode.
            var originalStrictMode = StrictMode;
            this.StrictMode = true;

            // Read the left brace.
            this.Expect(PunctuatorToken.LeftBrace);

            // Create a new scope to store the class name, if one was supplied.
            var scope = this.currentScope;
            if (className != null)
            {
                scope = Scope.CreateBlockScope(this.currentScope);
                scope.DeclareVariable(KeywordToken.Let, className);
            }
            using (CreateScopeContext(scope))
            {

                var members = new List<FunctionExpression>();
                FunctionExpression constructor = null;
                while (true)
                {
                    // If the next token is '}', then the class is complete.
                    if (this.nextToken == PunctuatorToken.RightBrace)
                        break;

                    // A bare semi-colon is an allowed class element.
                    if (this.nextToken == PunctuatorToken.Semicolon)
                    {
                        Consume();
                        continue;
                    }

                    // Record the start of the member.
                    var memberStartPosition = this.PositionAfterWhitespace;

                    // Read the name of the next class member.
                    // This will start with 'get', 'set', 'constructor' or a function name.
                    var memberName = ReadPropertyName(PropertyNameContext.ClassBody);

                    // Determine the parser context.
                    bool isConstructor = memberName.HasStaticName && memberName.StaticName == "constructor";
                    var parserContext = isConstructor ? (extends != null ? CodeContext.DerivedConstructor : CodeContext.Constructor) : CodeContext.ClassFunction;

                    // Parse the function declaration.
                    var expression = ParseFunction(
                        functionType: FunctionDeclarationType.Declaration,
                        parentScope: this.currentScope,
                        name: memberName,
                        startPosition: memberStartPosition,
                        codeContext: parserContext);

                    if (memberName.HasStaticName && memberName.StaticName == "constructor")
                    {
                        // Only one constructor is allowed.
                        if (constructor == null)
                            constructor = expression;
                        else
                            throw new SyntaxErrorException("A class may only have one constructor.", this.LineNumber, this.SourcePath);
                    }
                    else if (memberName.IsStatic && memberName.HasStaticName && memberName.StaticName == "prototype")
                    {
                        // A static function called 'prototype' is not allowed.
                        throw new SyntaxErrorException("Classes may not have a static property named 'prototype'.", this.LineNumber, this.SourcePath);
                    }
                    else
                        members.Add(expression);
                }

                // The end token '}' will be consumed by the parent function.
                Debug.Assert(this.nextToken == PunctuatorToken.RightBrace);

                // Restore strict mode.
                this.StrictMode = originalStrictMode;

                return new ClassExpression(this.currentScope, className, extends, constructor, members);

            }
        }
    }

}