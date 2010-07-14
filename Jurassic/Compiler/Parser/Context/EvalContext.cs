using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the information needed to compile eval script.
    /// </summary>
    internal class EvalContext : ScriptContext
    {
        /// <summary>
        /// Creates a new EvalContext instance.
        /// </summary>
        /// <param name="scope"> The scope to use inside the eval statement. </param>
        /// <param name="thisObject"> The value of the "this" keyword. </param>
        /// <param name="script"> The script code to execute. </param>
        public EvalContext(Scope scope, object thisObject, string script)
            : base(scope, "[eval code]")
        {
            this.ThisObject = thisObject;
            this.Script = script;
        }

        /// <summary>
        /// Gets the value of the "this" keyword inside the eval statement.
        /// </summary>
        public object ThisObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the eval script as a string.
        /// </summary>
        public string Script
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
            var lexer = new Lexer(new System.IO.StringReader(this.Script), this.Path);
            var parser = new Parser(lexer, this.InitialScope, false);
            return parser.Parse();
        }

        /// <summary>
        /// Generates IL for the script.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        protected override void GenerateCode(ILGenerator generator)
        {
            // Declare a variable to store the eval result.
            var optimizationInfo = OptimizationInfo.Empty;
            optimizationInfo.EvalResult = generator.DeclareVariable(typeof(object));

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
        /// <returns> The result of evaluating the script. </returns>
        public object Execute()
        {
            // Compile the code if it hasn't already been compiled.
            if (this.DynamicMethod == null)
                GenerateCode();

            // Execute the compiled delegate and return the result.
            return ((Func<Scope, object, object>)this.CompiledDelegate)(this.InitialScope, this.ThisObject);
        }
    }

}