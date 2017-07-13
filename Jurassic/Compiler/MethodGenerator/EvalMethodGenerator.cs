﻿using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the information needed to compile eval script into a method.
    /// </summary>
    internal class EvalMethodGenerator : MethodGenerator
    {
        /// <summary>
        /// Creates a new EvalMethodGenerator instance.
        /// </summary>
        /// <param name="parentScope"> The scope of the calling code. </param>
        /// <param name="source"> The script code to execute. </param>
        /// <param name="options"> Options that influence the compiler. </param>
        /// <param name="thisObject"> The value of the "this" keyword in the calling code. </param>
        public EvalMethodGenerator(Scope parentScope, ScriptSource source, CompilerOptions options, object thisObject)
            : base(parentScope, source, options)
        {
            this.ThisObject = thisObject;
        }

        /// <summary>
        /// Gets the value of the "this" keyword inside the eval statement.
        /// </summary>
        public object ThisObject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a name for the generated method.
        /// </summary>
        /// <returns> A name for the generated method. </returns>
        protected override string GetMethodName()
        {
            return "eval";
        }

        /// <summary>
        /// Parses the source text into an abstract syntax tree.
        /// </summary>
        public override void Parse()
        {
            using (var lexer = new Lexer(this.Source))
            {
                var parser = new Parser(lexer, this.InitialScope, this.Options, CodeContext.Eval);
                
                this.AbstractSyntaxTree = parser.Parse();

                this.StrictMode = parser.StrictMode;
                this.InitialScope = parser.BaseScope;
                this.MethodOptimizationHints = parser.MethodOptimizationHints;
            }
        }

        /// <summary>
        /// Generates IL for the script.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Declare a variable to store the eval result.
            optimizationInfo.EvalResult = generator.DeclareVariable(typeof(object));

            if (this.StrictMode == true)
            {
                // Create a new scope.
                this.InitialScope.GenerateScopeCreation(generator, optimizationInfo);
            }

            // Initialize any declarations.
            this.InitialScope.GenerateDeclarations(generator, optimizationInfo);

            // Generate the main body of code.
            this.AbstractSyntaxTree.GenerateCode(generator, optimizationInfo);

            // Make the return value from the method the eval result.
            generator.LoadVariable(optimizationInfo.EvalResult);

            // If the result is null, convert it to undefined.
            var end = generator.CreateLabel();
            generator.Duplicate();
            generator.BranchIfNotNull(end);
            generator.Pop();
            EmitHelpers.EmitUndefined(generator);
            generator.DefineLabelPosition(end);
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="engine"> The script engine to use to execute the script. </param>
        /// <returns> The result of evaluating the script. </returns>
        public object Execute(ScriptEngine engine)
        {
            if (engine == null)
                throw new ArgumentNullException("engine");

            // Compile the code if it hasn't already been compiled.
            if (this.GeneratedMethod == null)
                GenerateCode();

            // Strict mode creates a new scope so pass the parent scope in this case.
            var scope = this.InitialScope;
            if (this.StrictMode == true)
                scope = scope.ParentScope;

            // Execute the compiled delegate and store the result.
            object result = ((GlobalCodeDelegate)this.GeneratedMethod.GeneratedDelegate)(engine, scope, this.ThisObject);

            // Ensure the abstract syntax tree is kept alive until the eval code finishes running.
            GC.KeepAlive(this);

            // Return the result.
            return result;
        }
    }

}