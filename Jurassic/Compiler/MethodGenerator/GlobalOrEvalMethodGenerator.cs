using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the information needed to compile global or eval script into a method.
    /// </summary>
    internal class GlobalOrEvalMethodGenerator : MethodGenerator
    {
        /// <summary>
        /// Creates a new EvalMethodGenerator instance.
        /// </summary>
        /// <param name="source"> The script code to execute. </param>
        /// <param name="options"> Options that influence the compiler. </param>
        /// <param name="context"></param>
        public GlobalOrEvalMethodGenerator(ScriptSource source, CompilerOptions options, GeneratorContext context)
            : base(source, options)
        {
            Context = context;
        }

        public enum GeneratorContext
        {
            Global,
            GlobalEval,
            Eval,
        }

        /// <summary>
        /// 
        /// </summary>
        public GeneratorContext Context { get; private set; }

        /// <summary>
        /// Gets a name for the generated method.
        /// </summary>
        /// <returns> A name for the generated method. </returns>
        protected override string GetMethodName()
        {
            return Context == GeneratorContext.Global ? "global" : "eval";
        }

        /// <summary>
        /// Parses the source text into an abstract syntax tree.
        /// </summary>
        public override void Parse()
        {
            using (var lexer = new Lexer(this.Source))
            {
                CodeContext parserContext;
                switch (Context)
                {
                    case GeneratorContext.Global:
                        parserContext = CodeContext.Global;
                        break;
                    case GeneratorContext.GlobalEval:
                        parserContext = CodeContext.GlobalEval;
                        break;
                    case GeneratorContext.Eval:
                        parserContext = CodeContext.Eval;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var parser = new Parser(lexer, this.Options, parserContext);
                
                this.AbstractSyntaxTree = parser.Parse();
                this.StrictMode = parser.StrictMode;
                this.MethodOptimizationHints = parser.MethodOptimizationHints;
                this.BaseScope = parser.BaseScope;
            }
        }

        /// <summary>
        /// Generates IL for the script.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            if (Context != GeneratorContext.Global)
            {
                // Declare a variable to store the eval result.
                optimizationInfo.EvalResult = generator.DeclareVariable(typeof(object));
            }

            // Generate the main body of code.
            this.AbstractSyntaxTree.GenerateCode(generator, optimizationInfo);

            if (Context != GeneratorContext.Global)
            {
                // Make the return value from the method the eval result.
                generator.LoadVariable(optimizationInfo.EvalResult);
            }
            else
            {
                // Code in the global context always returns null.
                generator.LoadNull();
            }
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="engine"> The script engine to use to execute the script. </param>
        /// <param name="parentScope"> The scope of the calling code. </param>
        /// <param name="thisObject"> The value of the "this" keyword in the calling code. </param>
        /// <returns> The result of evaluating the script. </returns>
        public object Execute(ScriptEngine engine, RuntimeScope parentScope, object thisObject)
        {
            if (engine == null)
                throw new ArgumentNullException("engine");

            // Compile the code if it hasn't already been compiled.
            if (this.GeneratedMethod == null)
                GenerateCode();

            // Package up all the runtime state.
            var context = ExecutionContext.CreateGlobalOrEvalContext(engine, parentScope, thisObject);

            // Execute the compiled delegate and store the result.
            object result = ((GlobalCodeDelegate)this.GeneratedMethod.GeneratedDelegate)(context);
            if (result == null)
                result = Undefined.Value;

            // Ensure the abstract syntax tree is kept alive until the eval code finishes running.
            GC.KeepAlive(this);

            // Return the result.
            return result;
        }
    }

}