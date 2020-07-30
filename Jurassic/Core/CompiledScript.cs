using System;
using Jurassic.Compiler;

namespace Jurassic
{
    /// <summary>
    /// Represents the result of compiling a script.
    /// </summary>
    public sealed class CompiledScript
    {
        private GlobalMethodGenerator methodGen;

        internal CompiledScript(GlobalMethodGenerator methodGen)
        {
            if (methodGen == null)
                throw new ArgumentNullException(nameof(methodGen));
            this.methodGen = methodGen;
        }

        /// <summary>
        /// Compiles source code into a quickly executed form, using the given compiler options.
        /// </summary>
        /// <param name="source"> The javascript source code to execute. </param>
        /// <param name="options"> Compiler options, or <c>null</c> to use the default options. </param>
        /// <returns> A CompiledScript instance, which can be executed as many times as needed. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="source"/> is a <c>null</c> reference. </exception>
        public static CompiledScript Compile(ScriptSource source, CompilerOptions options = null)
        {
            var methodGen = new GlobalMethodGenerator(
                source,                             // The source code.
                options ?? new CompilerOptions());  // The compiler options.

            // Parse
            methodGen.Parse();

            // Optimize
            methodGen.Optimize();

            // Generate code
            methodGen.GenerateCode();

            return new CompiledScript(methodGen);
        }

        /// <summary>
        /// Gets the body of the generated method in the form of disassembled IL code.  Will be
        /// <c>null</c> unless <see cref="CompilerOptions.EnableILAnalysis"/> was set to
        /// <c>true</c>.
        /// </summary>
        public string DisassembledIL
        {
            get { return this.methodGen.GeneratedMethod.DisassembledIL; }
        }

        /// <summary>
        /// Executes the compiled script.
        /// </summary>
        /// <param name="engine"> The script engine to use to execute the script. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="engine"/> is a <c>null</c> reference. </exception>
        public void Execute(ScriptEngine engine)
        {
            try
            {
                methodGen.Execute(engine, RuntimeScope.CreateGlobalScope(engine), engine.Global);

                // Execute any pending callbacks.
                engine.ExecutePostExecuteSteps();
            }
            finally
            {
                // Ensure the list of post-execute steps is cleared if there is an exception.
                engine.ClearPostExecuteSteps();
            }
        }
    }
}
