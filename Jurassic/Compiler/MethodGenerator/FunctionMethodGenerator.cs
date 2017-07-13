﻿using System;
using System.Collections.Generic;
using System.Linq;
using ErrorType = Jurassic.Library.ErrorType;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents how the function was defined.
    /// </summary>
    internal enum FunctionDeclarationType
    {
        /// <summary>
        /// The function was declared as a statement.
        /// </summary>
        Declaration,

        /// <summary>
        /// The function was declared as an expression.
        /// </summary>
        Expression,

        /// <summary>
        /// The function is a getter in an object literal.
        /// </summary>
        Getter,

        /// <summary>
        /// The function is a setter in an object literal.
        /// </summary>
        Setter,
    }

    /// <summary>
    /// Represents the declaration of a function parameter.
    /// </summary>
    internal struct FunctionArgument
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the parameter, if no value was passed to the function (can be null).
        /// </summary>
        public Expression DefaultValue { get; set; }

        /// <summary>
        /// Returns the textual representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (DefaultValue == null)
                return Name;
            return $"{Name} = {DefaultValue}";
        }
    }

    /// <summary>
    /// Represents the information needed to compile a function.
    /// </summary>
    internal class FunctionMethodGenerator : MethodGenerator
    {
        /// <summary>
        /// Creates a new FunctionMethodGenerator instance.
        /// </summary>
        /// <param name="scope"> The function scope. </param>
        /// <param name="functionName"> The name of the function. </param>
        /// <param name="declarationType"> Indicates how the function was declared. </param>
        /// <param name="arguments"> The names and default values of the arguments. </param>
        /// <param name="bodyText"> The source code of the function. </param>
        /// <param name="body"> The root of the abstract syntax tree for the body of the function. </param>
        /// <param name="scriptPath"> The URL or file system path that the script was sourced from. </param>
        /// <param name="span"> The extent of the function in the source code. </param>
        /// <param name="options"> Options that influence the compiler. </param>
        public FunctionMethodGenerator(DeclarativeScope scope, string functionName, FunctionDeclarationType declarationType,
            IList<FunctionArgument> arguments, string bodyText, Statement body, string scriptPath, SourceCodeSpan span,
            CompilerOptions options)
            : base(scope, new DummyScriptSource(scriptPath), options)
        {
            this.Name = functionName;
            this.DeclarationType = declarationType;
            this.Arguments = arguments;
            this.BodyRoot = body;
            this.BodyText = bodyText;
            Validate(span.StartLine, scriptPath);
        }

        /// <summary>
        /// Dummy implementation of ScriptSource.
        /// </summary>
        private class DummyScriptSource : ScriptSource
        {
            private string path;

            public DummyScriptSource(string path)
            {
                this.path = path;
            }

            public override string Path
            {
                get { return this.path; }
            }

            public override System.IO.TextReader GetReader()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates a new FunctionContext instance.
        /// </summary>
        /// <param name="scope"> The function scope. </param>
        /// <param name="functionName"> The name of the function. </param>
        /// <param name="argumentsText"> A comma-separated list of arguments. </param>
        /// <param name="body"> The source code for the body of the function. </param>
        /// <param name="options"> Options that influence the compiler. </param>
        public FunctionMethodGenerator(DeclarativeScope scope, string functionName,
            string argumentsText, string body, CompilerOptions options)
            : base(scope, new StringScriptSource(body), options)
        {
            this.Name = functionName;
            this.ArgumentsText = argumentsText;
            this.BodyText = body;
        }

        /// <summary>
        /// The name of the function.  Getters and setters do not include "get" and "set" in their
        /// name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates how the function was declared.
        /// </summary>
        public FunctionDeclarationType DeclarationType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the display name for the function.  This is statically inferred from the
        /// context if the function is the target of an assignment or if the function is within an
        /// object literal.  Only set if the function name is empty.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a comma-separated list of arguments.
        /// </summary>
        public string ArgumentsText
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list of argument names and default values.
        /// </summary>
        public IList<FunctionArgument> Arguments
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root of the abstract syntax tree for the body of the function.
        /// </summary>
        public Statement BodyRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the source code for the body of the function.
        /// </summary>
        public string BodyText
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a name for the generated method.
        /// </summary>
        /// <returns> A name for the generated method. </returns>
        protected override string GetMethodName()
        {
            if (this.DisplayName != null)
                return this.DisplayName;
            else if (string.IsNullOrEmpty(this.Name))
                return "anonymous";
            else
                return this.Name;
        }

        /// <summary>
        /// Gets a name for the function, as it appears in the stack trace.
        /// </summary>
        /// <returns> A name for the function, as it appears in the stack trace, or <c>null</c> if
        /// this generator is generating code in the global scope. </returns>
        protected override string GetStackName()
        {
            return GetMethodName();
        }

        /// <summary>
        /// Gets an array of types - one for each parameter accepted by the method generated by
        /// this context.
        /// </summary>
        /// <returns> An array of parameter types. </returns>
        protected override Type[] GetParameterTypes()
        {
            return new Type[] {
                typeof(ScriptEngine),               // The script engine.
                typeof(Scope),                      // The parent scope.
                typeof(object),                     // The "this" object.
                typeof(Library.FunctionInstance),   // The function object.
                typeof(object[])                    // The argument values.
            };
        }

        /// <summary>
        /// Checks whether the function is valid (in strict mode the function cannot be named
        /// 'arguments' or 'eval' and the argument names cannot be duplicated).
        /// </summary>
        /// <param name="lineNumber"> The line number in the source file. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        private void Validate(int lineNumber, string sourcePath)
        {
            if (this.StrictMode == true)
            {
                // If the function body is strict mode, then the function name cannot be 'eval' or 'arguments'.
                if (this.Name == "arguments" || this.Name == "eval")
                    throw new SyntaxErrorException(string.Format("Functions cannot be named '{0}' in strict mode.", this.Name), lineNumber, sourcePath);

                // If the function body is strict mode, then the argument names cannot be 'eval' or 'arguments'.
                foreach (var argument in this.Arguments)
                    if (argument.Name == "arguments" || argument.Name == "eval")
                        throw new SyntaxErrorException(string.Format("Arguments cannot be named '{0}' in strict mode.", argument.Name), lineNumber, sourcePath);

                // If the function body is strict mode, then the argument names cannot be duplicates.
                var duplicateCheck = new HashSet<string>();
                foreach (var argument in this.Arguments)
                {
                    if (duplicateCheck.Contains(argument.Name) == true)
                        throw new SyntaxErrorException(string.Format("Duplicate argument name '{0}' is not allowed in strict mode.", argument.Name), lineNumber, sourcePath);
                    duplicateCheck.Add(argument.Name);
                }
            }
        }

        /// <summary>
        /// Parses the source text into an abstract syntax tree.
        /// </summary>
        /// <returns> The root node of the abstract syntax tree. </returns>
        public override void Parse()
        {
            if (this.BodyRoot != null)
            {
                this.AbstractSyntaxTree = this.BodyRoot;
            }
            else
            {
                Parser argumentsParser;
                using (var argumentsLexer = new Lexer(new StringScriptSource(this.ArgumentsText)))
                {
                    argumentsParser = new Parser(argumentsLexer, this.InitialScope, this.Options, CodeContext.Function);
                    this.Arguments = argumentsParser.ParseFunctionArguments(endToken: null);
                }
                using (var lexer = new Lexer(this.Source))
                {
                    var parser = new Parser(lexer, this.InitialScope, this.Options, CodeContext.Function, argumentsParser.MethodOptimizationHints);
                    this.AbstractSyntaxTree = parser.Parse();
                    this.StrictMode = parser.StrictMode;
                    this.MethodOptimizationHints = parser.MethodOptimizationHints;
                }
                Validate(1, this.Source.Path);
            }
        }

        /// <summary>
        /// Retrieves a delegate for the generated method.
        /// </summary>
        /// <returns> The delegate type that matches the method parameters. </returns>
        protected override Type GetDelegate()
        {
            return typeof(Library.FunctionDelegate);
        }

        /// <summary>
        /// Generates IL for the script.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Method signature: object FunctionDelegate(Compiler.Scope scope, object thisObject, Library.FunctionInstance functionObject, object[] arguments)

            // Initialize the scope (note: the initial scope for a function is always declarative).
            this.InitialScope.GenerateScopeCreation(generator, optimizationInfo);

            // In ES3 the "this" value must be an object.  See 10.4.3 in the spec.
            if (this.StrictMode == false && this.MethodOptimizationHints.HasThis == true)
            {
                // if (thisObject == null || thisObject == Null.Value || thisObject == Undefined.Value)
                EmitHelpers.LoadThis(generator);
                generator.LoadNull();
                generator.CompareEqual();
                EmitHelpers.LoadThis(generator);
                EmitHelpers.EmitNull(generator);
                generator.CompareEqual();
                generator.BitwiseOr();
                EmitHelpers.LoadThis(generator);
                EmitHelpers.EmitUndefined(generator);
                generator.CompareEqual();
                generator.BitwiseOr();

                // {
                var startOfFalse = generator.CreateLabel();
                generator.BranchIfFalse(startOfFalse);

                // thisObject = engine.Global;
                EmitHelpers.LoadScriptEngine(generator);
                generator.Call(ReflectionHelpers.ScriptEngine_Global);
                
                // } else {
                var endOfIf = generator.CreateLabel();
                generator.Branch(endOfIf);
                generator.DefineLabelPosition(startOfFalse);
                
                // thisObject = TypeConverter.ToObject(thisObject);
                EmitHelpers.LoadThis(generator);
                EmitConversion.ToObject(generator, PrimitiveType.Any, optimizationInfo);

                // }
                generator.DefineLabelPosition(endOfIf);
                EmitHelpers.StoreThis(generator);
            }

            // Transfer the function name into the scope.
            if (string.IsNullOrEmpty(this.Name) == false &&
                (this.DeclarationType != FunctionDeclarationType.Getter && this.DeclarationType != FunctionDeclarationType.Setter) &&
                this.Arguments.Any(a => a.Name == this.Name) == false &&
                optimizationInfo.MethodOptimizationHints.HasVariable(this.Name))
            {
                EmitHelpers.LoadFunction(generator);
                var functionName = new NameExpression(this.InitialScope, this.Name);
                functionName.GenerateSet(generator, optimizationInfo, PrimitiveType.Any, false);
            }

            // Transfer the arguments object into the scope.
            if (this.MethodOptimizationHints.HasArguments == true && this.Arguments.Any(a => a.Name == "arguments") == false)
            {
                // prototype
                EmitHelpers.LoadScriptEngine(generator);
                generator.Call(ReflectionHelpers.ScriptEngine_Object);
                generator.Call(ReflectionHelpers.FunctionInstance_InstancePrototype);
                // callee
                EmitHelpers.LoadFunction(generator);
                generator.CastClass(typeof(Library.UserDefinedFunction));
                // scope
                EmitHelpers.LoadScope(generator);
                generator.CastClass(typeof(DeclarativeScope));
                // argumentValues
                EmitHelpers.LoadArgumentsArray(generator);
                generator.NewObject(ReflectionHelpers.Arguments_Constructor);
                var arguments = new NameExpression(this.InitialScope, "arguments");
                arguments.GenerateSet(generator, optimizationInfo, PrimitiveType.Any, false);
            }

            // Transfer the argument values into the scope.
            // Note: the arguments array can be smaller than expected.
            if (this.Arguments.Count > 0)
            {
                for (int i = 0; i < this.Arguments.Count; i++)
                {
                    // Check if a duplicate argument name exists.
                    bool duplicate = false;
                    for (int j = i + 1; j < this.Arguments.Count; j++)
                        if (this.Arguments[i].Name == this.Arguments[j].Name)
                        {
                            duplicate = true;
                            break;
                        }
                    if (duplicate == true)
                        continue;

                    var loadDefaultValue = generator.CreateLabel();
                    var storeValue = generator.CreateLabel();

                    // Check if an array element exists.
                    EmitHelpers.LoadArgumentsArray(generator);
                    generator.LoadArrayLength();
                    generator.LoadInt32(i);
                    generator.BranchIfLessThanOrEqual(loadDefaultValue);

                    // Load the parameter value from the parameters array.
                    EmitHelpers.LoadArgumentsArray(generator);
                    generator.LoadInt32(i);
                    generator.LoadArrayElement(typeof(object));

                    if (this.Arguments[i].DefaultValue == null)
                    {
                        // Branch to the part where it stores the value.
                        generator.Branch(storeValue);

                        // Load undefined.
                        generator.DefineLabelPosition(loadDefaultValue);
                        EmitHelpers.EmitUndefined(generator);
                    }
                    else
                    {
                        // Check if it's undefined.
                        generator.Duplicate();
                        EmitHelpers.EmitUndefined(generator);
                        generator.BranchIfNotEqual(storeValue);
                        generator.Pop();

                        // Load the default value.
                        generator.DefineLabelPosition(loadDefaultValue);
                        this.Arguments[i].DefaultValue.GenerateCode(generator, optimizationInfo);
                        EmitConversion.ToAny(generator, this.Arguments[i].DefaultValue.ResultType);
                    }

                    // Store the value in the scope.
                    generator.DefineLabelPosition(storeValue);
                    var argument = new NameExpression(this.InitialScope, this.Arguments[i].Name);
                    argument.GenerateSet(generator, optimizationInfo, PrimitiveType.Any, false);
                }
            }

            // Initialize any declarations.
            this.InitialScope.GenerateDeclarations(generator, optimizationInfo);

            // Generate code for the body of the function.
            this.AbstractSyntaxTree.GenerateCode(generator, optimizationInfo);

            // Define the return target - this is where the return statement jumps to.
            // ReturnTarget can be null if there were no return statements.
            if (optimizationInfo.ReturnTarget != null)
                generator.DefineLabelPosition(optimizationInfo.ReturnTarget);

            // Load the return value.  If the variable is null, there were no return statements.
            if (optimizationInfo.ReturnVariable != null)
                // Return the value stored in the variable.  Will be null if execution hits the end
                // of the function without encountering any return statements.
                generator.LoadVariable(optimizationInfo.ReturnVariable);
            else
                // There were no return statements - return null.
                generator.LoadNull();
        }

        /// <summary>
        /// Converts this object to a string.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            if (this.BodyRoot != null)
                return string.Format("function {0}({1}) {2}", this.Name, StringHelpers.Join(", ", this.Arguments), this.BodyRoot);
            return string.Format("function {0}({1}) {{\n{2}\n}}", this.Name, StringHelpers.Join(", ", this.Arguments), this.BodyText);
        }
    }

}