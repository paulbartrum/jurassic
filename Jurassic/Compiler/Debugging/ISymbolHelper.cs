using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jurassic.Compiler
{
    public interface ISymbolHelper
    {
        /// <summary>
        /// Invoked upon method generation start.
        /// </summary>
        /// <param name="methodName">The name of the method being generated.</param>
        /// <param name="parametersTypes">The parameters' types being passed to this method.</param>
        /// <param name="parametersNames">The names for all parameters for this method.</param>
        /// <returns>A MethodInfo compliant instance, which will be used to obtain ILGenerator.</returns>
        System.Reflection.Emit.ILGenerator BeginMethodGeneration(string methodName, Type[] parametersTypes, string[] parametersNames);

        /// <summary>
        /// Invoked after the code is enitted, to finish the method generation process.
        /// </summary>
        /// <param name="delegateType">The type of delegate that needs to be created.</param>
        /// <returns>A delegate performing the compiled functionality.</returns>
        Delegate EndMethodGeneration(Type delegateType);

        /// <summary>
        /// Marks a sequence point in the Microsoft intermediate language (MSIL) stream.
        /// </summary>
        /// <param name="startLine"> The line where the sequence point begins. </param>
        /// <param name="startColumn"> The column in the line where the sequence point begins. </param>
        /// <param name="endLine"> The line where the sequence point ends. </param>
        /// <param name="endColumn"> The column in the line where the sequence point ends. </param>
        void MarkSequencePoint(int startLine, int startColumn, int endLine, int endColumn);

        /// <summary>
        /// Informs the symbol helper that there is a local variable to be declared.
        /// </summary>
        /// <param name="localBuilder"></param>
        /// <param name="name"></param>
        void DeclareVariable(System.Reflection.Emit.LocalBuilder localBuilder, string name);
    }

    public delegate ISymbolHelper SymbolHelperFactory(ScriptSource scriptSource, CompilerOptions options);

    /// <summary>
    /// The default implementation of symbol generation helper
    /// </summary>
    internal class DefaultSymbolHelper : ISymbolHelper
    {
        private System.Reflection.Emit.DynamicMethod dynamicMethod;

        public DefaultSymbolHelper()
        {
            dynamicMethod = null;
        }

        public System.Reflection.Emit.ILGenerator BeginMethodGeneration(string methodName, Type[] parametersTypes, string[] parametersNames)
        {
            dynamicMethod = new System.Reflection.Emit.DynamicMethod(
                methodName,                                        // Name of the generated method.
                typeof(object),                                    // Return type of the generated method.
                parametersTypes,                                   // Parameter types of the generated method.
                typeof(MethodGenerator),                           // Owner type.
                true);                                             // Skip visibility checks.

            return dynamicMethod.GetILGenerator();
        }

        public void DeclareMethodParameters(IEnumerable<string> parameterNames)
        { }

        public Delegate EndMethodGeneration(Type delegateType)
        {
            return dynamicMethod.CreateDelegate(delegateType);
        }

        public void MarkSequencePoint(int startLine, int startColumn, int endLine, int endColumn)
        { }

        public void DeclareVariable(System.Reflection.Emit.LocalBuilder localBuilder, string name)
        { }

    };

}
