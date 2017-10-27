using System;
using System.Reflection;

namespace Jurassic.Compiler
{
    public abstract class IMethodGenerationHelper
    {
        /// <summary>
        /// The new SymbolDocumentWriter, if such is set from the helper.
        /// </summary>
        public abstract System.Diagnostics.SymbolStore.ISymbolDocumentWriter DebugDocument
        { get; }

        public virtual void SetupMethodGeneration(ScriptSource scriptSource, CompilerOptions options)
        { }

        /// <summary>
        /// Invoked upon method generation start.
        /// </summary>
        /// <param name="methodName">The name of the method being generated.</param>
        /// <param name="parametersTypes">The parameters' types being passed to this method.</param>
        /// <returns>A MethodInfo compliant instance, which will be used to obtain ILGenerator.</returns>
        public abstract System.Reflection.Emit.ILGenerator BeginMethodGeneration(string methodName, Type[] parametersTypes);

        /// <summary>
        /// Invoked after the code is enitted, to finish the method generation process.
        /// </summary>
        /// <param name="delegateType">The type of delegate that needs to be created.</param>
        /// <param name="methodName">The name of the method being generated.</param>
        /// <param name="parametersTypes">The parameters' types being passed to this method.</param>
        /// <returns>A delegate performing the compiled functionality.</returns>
        public abstract Delegate EndMethodGeneration(Type delegateType, string methodName, Type[] parametersTypes);
    }
}
