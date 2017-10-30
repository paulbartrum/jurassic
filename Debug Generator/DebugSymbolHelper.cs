using System;
using System.Collections.Generic;
using System.Reflection;
using Jurassic.Compiler;

namespace Jurassic.Debugging
{
    public class DebugSymbolHelper : ISymbolHelper
    {
        internal class ReflectionEmitModuleInfo
        {
            public System.Reflection.Emit.AssemblyBuilder AssemblyBuilder;
            public System.Reflection.Emit.ModuleBuilder ModuleBuilder;
            public int TypeCount;
        }

        private static object reflectionEmitInfoLock = new object();

        /// <summary>
        /// Gets or sets information needed by Reflection.Emit.
        /// </summary>
        private static ReflectionEmitModuleInfo ReflectionEmitInfo;

        /// <summary>
        /// Gets the language type GUID for the symbol store.
        /// </summary>
        private static readonly Guid LanguageType =      // JScript
            new Guid("3A12D0B6-C26C-11D0-B442-00A0244A1DD2");

        /// <summary>
        /// Gets the language vendor GUID for the symbol store.
        /// </summary>
        private static readonly Guid LanguageVendor =
            new Guid("CFA05A92-B7CC-4D3D-92E1-4D18CDACDC8D");


        /// <summary>
        /// Gets the document type GUID for the symbol store.
        /// </summary>
        private static readonly Guid DocumentType =
            new Guid("5A869D0B-6611-11D3-BD2A-0000F80849BD");

        /// <summary>
        /// An intermediate storage for the type, building built.
        /// </summary>
        private System.Reflection.Emit.TypeBuilder TypeBuilder;

        /// <summary>
        /// The script source (location), passed at setup process.
        /// </summary>
        private ScriptSource Source;

        /// <summary>
        /// The debug documet created, if at all.
        /// </summary>
        private System.Diagnostics.SymbolStore.ISymbolDocumentWriter DebugDocument;

        /// <summary>
        /// The symbol-aware generator initialiazed upon method generation begin.
        /// </summary>
        private System.Reflection.Emit.ILGenerator generator;

        /// <summary>
        /// The name of the method being built.
        /// </summary>
        private string MethodName;

        public DebugSymbolHelper(ScriptSource scriptSource, CompilerOptions options)
        {
            this.DebugDocument = null;
            this.Source = scriptSource;
        }

        public System.Reflection.Emit.ILGenerator BeginMethodGeneration(string methodName, Type[] parametersTypes, string[] parametersNames)
        {
            // Debugging or low trust path.
            ReflectionEmitModuleInfo reflectionEmitInfo;
            lock (reflectionEmitInfoLock)
            {
                reflectionEmitInfo = ReflectionEmitInfo;
                if (reflectionEmitInfo == null)
                {
                    reflectionEmitInfo = new ReflectionEmitModuleInfo();

                    // Create a dynamic assembly and module.
                    reflectionEmitInfo.AssemblyBuilder = System.Threading.Thread.GetDomain().DefineDynamicAssembly(
                        new System.Reflection.AssemblyName("Jurassic Dynamic Assembly"), System.Reflection.Emit.AssemblyBuilderAccess.Run);

                    // Mark the assembly as debuggable.  This must be done before the module is created.
                    var debuggableAttributeConstructor = typeof(System.Diagnostics.DebuggableAttribute).GetConstructor(
                        new Type[] { typeof(System.Diagnostics.DebuggableAttribute.DebuggingModes) });
                    reflectionEmitInfo.AssemblyBuilder.SetCustomAttribute(
                        new System.Reflection.Emit.CustomAttributeBuilder(debuggableAttributeConstructor,
                            new object[] {
                                System.Diagnostics.DebuggableAttribute.DebuggingModes.DisableOptimizations |
                                System.Diagnostics.DebuggableAttribute.DebuggingModes.Default }));

                    // Create a dynamic module.
                    reflectionEmitInfo.ModuleBuilder = reflectionEmitInfo.AssemblyBuilder.DefineDynamicModule("Module", true);

                    ReflectionEmitInfo = reflectionEmitInfo;
                }

                // Create a new type to hold our method.
                this.TypeBuilder = reflectionEmitInfo.ModuleBuilder.DefineType("JavaScriptClass" + reflectionEmitInfo.TypeCount.ToString(), System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Class);
                reflectionEmitInfo.TypeCount++;
            }

            // Create a method.
           var methodBuilder = this.TypeBuilder.DefineMethod(this.MethodName = methodName,
                MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public,
                typeof(object), 
                parametersTypes);

            if (this.Source.Path != null)
            {
                // Initialize the debugging information.
                this.DebugDocument = reflectionEmitInfo.ModuleBuilder.DefineDocument(this.Source.Path, LanguageType, LanguageVendor, DocumentType);
                for (var i = 0;i < parametersNames.Length; ++i)
                    methodBuilder.DefineParameter(i + 1, ParameterAttributes.In, parametersNames[i]);
            }

            return generator = methodBuilder.GetILGenerator();
        }

        public Delegate EndMethodGeneration(Type delegateType)
        { 
            // Bake it.
            var methodInfo = this.TypeBuilder.CreateType().GetMethod(this.MethodName);
            return Delegate.CreateDelegate(delegateType, methodInfo);
        }

        public void MarkSequencePoint(int startLine, int startColumn, int endLine, int endColumn)
        {
            generator.MarkSequencePoint(this.DebugDocument,
                                        startLine, startColumn,
                                        endLine, endColumn);
            
        }

        public static ISymbolHelper DebugSymbolFactory(ScriptSource scriptSource, CompilerOptions options)
        {
            return new DebugSymbolHelper(scriptSource, options);
        }
    }
}
