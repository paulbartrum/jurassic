using System;
using System.Collections.Generic;
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
        /// Executes the compiled script.
        /// </summary>
        public void Execute()
        {
            methodGen.Execute();
        }
    }
}
