using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the information needed to compile global code.
    /// </summary>
    internal class GlobalContext : ScriptContext
    {
        /// <summary>
        /// Creates a new GlobalContext instance.
        /// </summary>
        /// <param name="scriptReader"> An object that can read the text of the script to execute. </param>
        /// <param name="scriptPath"> The URL or file system path that the script was sourced from. </param>
        public GlobalContext(System.IO.TextReader scriptReader, string scriptPath)
            : base(ObjectScope.CreateGlobalScope(), scriptPath)
        {
            this.Reader = scriptReader;
        }

        /// <summary>
        /// Gets a TextReader that can read the javascript source text.
        /// </summary>
        public System.IO.TextReader Reader
        {
            get;
            private set;
        }

        /// <summary>
        /// Parses the source text into an abstract syntax tree.
        /// </summary>
        /// <returns> The root node of the abstract syntax tree. </returns>
        protected override Statement ParseCore()
        {
            var lexer = new Lexer(this.Reader, this.Path);
            var parser = new Parser(lexer, this.InitialScope, false);
            var result = parser.Parse();
            this.StrictMode = parser.StrictMode;
            return result;
        }

        /// <summary>
        /// Generates IL for the script.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        protected override void GenerateCode(ILGenerator generator)
        {
            // Store the state of the StrictMode flag in the optimization info instance.
            var optimizationInfo = OptimizationInfo.Empty;
            if (this.StrictMode == true)
                optimizationInfo = optimizationInfo.AddFlags(OptimizationFlags.StrictMode);

            // Initialize any function or variable declarations.
            this.InitialScope.GenerateDeclarations(generator, optimizationInfo);

            // Generate code for the source code.
            this.AbstractSyntaxTree.GenerateCode(generator, optimizationInfo);

            // Code in the global context always returns undefined.
            EmitHelpers.EmitUndefined(generator);
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <returns> The result of evaluating the script. </returns>
        public object Execute()
        {
            // Compile the code if it hasn't already been compiled.
            if (this.DynamicMethod == null)
                GenerateCode();

            // Execute the compiled delegate and return the result.
            return ((Func<Scope, object, object>)this.CompiledDelegate)(this.InitialScope, Library.GlobalObject.Instance);
        }
    }

}