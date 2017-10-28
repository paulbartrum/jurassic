using System;
using System.Reflection;

namespace Jurassic.Compiler
{
    public abstract class ISymbolHelper
    {
        public virtual void SetupGeneration(ScriptSource scriptSource, CompilerOptions options)
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

        /// <summary>
        /// Marks a sequence point in the Microsoft intermediate language (MSIL) stream.
        /// </summary>
        /// <param name="startLine"> The line where the sequence point begins. </param>
        /// <param name="startColumn"> The column in the line where the sequence point begins. </param>
        /// <param name="endLine"> The line where the sequence point ends. </param>
        /// <param name="endColumn"> The column in the line where the sequence point ends. </param>
        public abstract void MarkSequencePoint(int startLine, int startColumn, int endLine, int endColumn);

    }

    /// <summary>
    /// The default implementation of symbol generation helper
    /// </summary>
    internal class DefaultSymbolHelper : ISymbolHelper
    {
        private System.Reflection.Emit.DynamicMethod dynamicMethod;

        public override System.Reflection.Emit.ILGenerator BeginMethodGeneration(string methodName, Type[] parametersTypes)
        {
            dynamicMethod = new System.Reflection.Emit.DynamicMethod(
                methodName,                                        // Name of the generated method.
                typeof(object),                                         // Return type of the generated method.
                parametersTypes,                                    // Parameter types of the generated method.
                typeof(MethodGenerator),                                // Owner type.
                true);                                                  // Skip visibility checks.

            return dynamicMethod.GetILGenerator();
        }

        public override Delegate EndMethodGeneration(Type delegateType, string methodName, Type[] parametersTypes)
        {
            return dynamicMethod.CreateDelegate(delegateType);
        }

        public override void MarkSequencePoint(int startLine, int startColumn, int endLine, int endColumn)
        { }
    };

}
