using System;
using System.Reflection;
using Jurassic.Compiler;

namespace Jurassic.Debugging
{
    internal class DebugReflectionEmitILGenerator : ReflectionEmitILGenerator
    {

    }

    public class MethodGenerationHelper : IMethodGenerationHelper
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
        private System.Diagnostics.SymbolStore.ISymbolDocumentWriter _debugDocument;

        public override System.Diagnostics.SymbolStore.ISymbolDocumentWriter DebugDocument
        { get => this._debugDocument; }

        public override void SetupMethodGeneration(ScriptSource scriptSource, CompilerOptions options)
        {
            this._debugDocument = null;
            this.Source = scriptSource;
        }

        public override System.Reflection.Emit.ILGenerator BeginMethodGeneration(string methodName, Type[] parametersTypes)
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
            var methodBuilder = this.TypeBuilder.DefineMethod(methodName,
                System.Reflection.MethodAttributes.HideBySig | System.Reflection.MethodAttributes.Static | System.Reflection.MethodAttributes.Public,
                typeof(object), parametersTypes);

            if (this.Source.Path != null)
            {
                // Initialize the debugging information.
                this._debugDocument = reflectionEmitInfo.ModuleBuilder.DefineDocument(this.Source.Path, LanguageType, LanguageVendor, DocumentType);
                methodBuilder.DefineParameter(1, System.Reflection.ParameterAttributes.None, "scriptEngine");
                methodBuilder.DefineParameter(2, System.Reflection.ParameterAttributes.None, "scope");
                methodBuilder.DefineParameter(3, System.Reflection.ParameterAttributes.None, "thisValue");
            }

            return methodBuilder.GetILGenerator();
        }

        public override Delegate EndMethodGeneration(Type delegateType, string methodName, Type[] parametersTypes)
        { 
            // Bake it.
            var methodInfo = this.TypeBuilder.CreateType().GetMethod(methodName);
            return Delegate.CreateDelegate(delegateType, methodInfo);
        }
    }
}
