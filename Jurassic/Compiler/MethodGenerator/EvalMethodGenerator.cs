using System;

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
        /// <param name="engine"> The script engine. </param>
        /// <param name="parentScope"> The scope of the calling code. </param>
        /// <param name="source"> The script code to execute. </param>
        /// <param name="options"> Options that influence the compiler. </param>
        /// <param name="thisObject"> The value of the "this" keyword in the calling code. </param>
        public EvalMethodGenerator(ScriptEngine engine, Scope parentScope, ScriptSource source, CompilerOptions options, object thisObject)
            : base(engine, GetScope(parentScope, options.ForceStrictMode), source, options)
        {
            this.ThisObject = thisObject;
        }

        /// <summary>
        /// Gets the scope to use.  This is the same as the parent scope when
        /// <paramref name="strictMode"/> is <c>false</c>, or a new declarative scope if
        /// <paramref name="strictMode"/> is <c>true</c>.
        /// </summary>
        /// <param name="parentScope"> The scope of the calling code. </param>
        /// <param name="strictMode"> The state of the strict mode flag. </param>
        /// <returns> The scope to use inside the eval statement. </returns>
        private static Scope GetScope(Scope parentScope, bool strictMode)
        {
            if (strictMode == false)
                return parentScope;
            return DeclarativeScope.CreateEvalScope(parentScope);
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
        /// Gets a name for the generated method.
        /// </summary>
        /// <returns> A name for the generated method. </returns>
        protected override string GetMethodName()
        {
            return "eval";
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
        /// <returns> The result of evaluating the script. </returns>
        public object Execute()
        {
            // Compile the code if it hasn't already been compiled.
            if (this.GeneratedMethod == null)
                GenerateCode();

            // Execute the compiled delegate and return the result.
            return ((Func<ScriptEngine, Scope, object, object>)this.CompiledDelegate)(this.Engine, this.InitialScope, this.ThisObject);
        }
    }

}