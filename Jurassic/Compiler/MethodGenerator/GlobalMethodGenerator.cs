namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the information needed to compile global code.
    /// </summary>
    internal class GlobalMethodGenerator : EvalMethodGenerator
    {
        /// <summary>
        /// Creates a new GlobalMethodGenerator instance.
        /// </summary>
        /// <param name="source"> The source of javascript code. </param>
        /// <param name="options"> Options that influence the compiler. </param>
        public GlobalMethodGenerator(ScriptSource source, CompilerOptions options)
            : base(source, options)
        {
        }

        /// <summary>
        /// Changes the behaviour of variable declarations in strict mode.
        /// </summary>
        protected override CodeContext ParserContext { get; } = CodeContext.Global;
    }
}