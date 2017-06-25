﻿using System;
using System.Reflection;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the unit of compilation.
    /// </summary>
    internal abstract class MethodGenerator
    {
        /// <summary>
        /// Creates a new MethodGenerator instance.
        /// </summary>
        /// <param name="scope"> The initial scope. </param>
        /// <param name="source"> The source of javascript code. </param>
        /// <param name="options"> Options that influence the compiler. </param>
        protected MethodGenerator(Scope scope, ScriptSource source, CompilerOptions options)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            this.InitialScope = scope;
            this.Source = source;
            this.Options = options;
            this.StrictMode = this.Options.ForceStrictMode;
        }

        /// <summary>
        /// Gets a reference to any compiler options.
        /// </summary>
        public CompilerOptions Options
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the source of javascript code.
        /// </summary>
        public ScriptSource Source
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value that indicates whether strict mode is enabled.
        /// </summary>
        public bool StrictMode
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the top-level scope associated with the context.
        /// </summary>
        public Scope InitialScope
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the root node of the abstract syntax tree.  This will be <c>null</c> until Parse()
        /// is called.
        /// </summary>
        public Statement AbstractSyntaxTree
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets optimization information.
        /// </summary>
        public MethodOptimizationHints MethodOptimizationHints
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the generated IL.  This will be <c>null</c> until GenerateCode() is
        /// called.
        /// </summary>
        public ILGenerator ILGenerator
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a delegate to the emitted dynamic method, plus any dependencies.  This will be
        /// <c>null</c> until GenerateCode() is called.
        /// </summary>
        public GeneratedMethod GeneratedMethod
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a name for the generated method.
        /// </summary>
        /// <returns> A name for the generated method. </returns>
        protected abstract string GetMethodName();

        /// <summary>
        /// Gets a name for the function, as it appears in the stack trace.
        /// </summary>
        /// <returns> A name for the function, as it appears in the stack trace, or <c>null</c> if
        /// this generator is generating code in the global scope. </returns>
        protected virtual string GetStackName()
        {
            return null;
        }

        /// <summary>
        /// Gets an array of types - one for each parameter accepted by the method generated by
        /// this context.
        /// </summary>
        /// <returns> An array of parameter types. </returns>
        protected virtual Type[] GetParameterTypes()
        {
            return new Type[] {
                typeof(ScriptEngine),   // The script engine.
                typeof(Scope),          // The scope.
                typeof(object),         // The "this" object.
            };
        }

        /// <summary>
        /// Parses the source text into an abstract syntax tree.
        /// </summary>
        public abstract void Parse();

        /// <summary>
        /// Optimizes the abstract syntax tree.
        /// </summary>
        public void Optimize()
        {
        }

        internal class ReflectionEmitModuleInfo
        {
            public System.Reflection.Emit.AssemblyBuilder AssemblyBuilder;
            public System.Reflection.Emit.ModuleBuilder ModuleBuilder;
            public int TypeCount;
        }

        /// <summary>
        /// Gets or sets information needed by Reflection.Emit.
        /// </summary>
        private static ReflectionEmitModuleInfo ReflectionEmitInfo;

        /// <summary>
        /// Generates IL for the script.
        /// </summary>
        public void GenerateCode()
        {
            // Generate the abstract syntax tree if it hasn't already been generated.
            if (this.AbstractSyntaxTree == null)
            {
                Parse();
                Optimize();
            }

            // Initialize global code-gen information.
            var optimizationInfo = new OptimizationInfo();
            optimizationInfo.AbstractSyntaxTree = this.AbstractSyntaxTree;
            optimizationInfo.StrictMode = this.StrictMode;
            optimizationInfo.MethodOptimizationHints = this.MethodOptimizationHints;
            optimizationInfo.FunctionName = this.GetStackName();
            optimizationInfo.Source = this.Source;

            ILGenerator generator;
            if (this.Options.EnableDebugging == false)
            {
                // DynamicMethod requires full trust because of generator.LoadMethodPointer in the
                // FunctionExpression class.

                // Create a new dynamic method.
                System.Reflection.Emit.DynamicMethod dynamicMethod = new System.Reflection.Emit.DynamicMethod(
                    GetMethodName(),                                        // Name of the generated method.
                    typeof(object),                                         // Return type of the generated method.
                    GetParameterTypes(),                                    // Parameter types of the generated method.
                    typeof(MethodGenerator),                                // Owner type.
                    true);                                                  // Skip visibility checks.
#if __MonoCS__ || NETSTANDARD1_5
                generator = new ReflectionEmitILGenerator(dynamicMethod.GetILGenerator());
#else
                generator = new DynamicILGenerator(dynamicMethod);
#endif

                if (this.Options.EnableILAnalysis == true)
                {
                    // Replace the generator with one that logs.
                    generator = new LoggingILGenerator(generator);
                }

                // Initialization code will appear to come from line 1.
                optimizationInfo.MarkSequencePoint(generator, new SourceCodeSpan(1, 1, 1, 1));

                // Generate the IL.
                GenerateCode(generator, optimizationInfo);
                generator.Complete();

                // Create a delegate from the method.
                this.GeneratedMethod = new GeneratedMethod(dynamicMethod.CreateDelegate(GetDelegate()), optimizationInfo.NestedFunctions);

            }
            else
            {
#if WINDOWS_PHONE
                throw new NotImplementedException();
#else
                // Debugging or low trust path.
                ReflectionEmitModuleInfo reflectionEmitInfo = ReflectionEmitInfo;
                if (reflectionEmitInfo == null)
                {
                    reflectionEmitInfo = new ReflectionEmitModuleInfo();

                    // Create a dynamic assembly and module.
#if NETSTANDARD1_5
                    reflectionEmitInfo.AssemblyBuilder = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(
                        new System.Reflection.AssemblyName("Jurassic Dynamic Assembly"), System.Reflection.Emit.AssemblyBuilderAccess.Run);
#else
                    reflectionEmitInfo.AssemblyBuilder = System.Threading.Thread.GetDomain().DefineDynamicAssembly(
                        new System.Reflection.AssemblyName("Jurassic Dynamic Assembly"), System.Reflection.Emit.AssemblyBuilderAccess.Run);
#endif

                    // Mark the assembly as debuggable.  This must be done before the module is created.
                    var debuggableAttributeConstructor = typeof(System.Diagnostics.DebuggableAttribute).GetTypeInfo().GetConstructor(
                        new Type[] { typeof(System.Diagnostics.DebuggableAttribute.DebuggingModes) });
                    reflectionEmitInfo.AssemblyBuilder.SetCustomAttribute(
                        new System.Reflection.Emit.CustomAttributeBuilder(debuggableAttributeConstructor,
                            new object[] {
                                System.Diagnostics.DebuggableAttribute.DebuggingModes.DisableOptimizations |
                                System.Diagnostics.DebuggableAttribute.DebuggingModes.Default }));

                    // Create a dynamic module.
#if NETSTANDARD1_5
                    reflectionEmitInfo.ModuleBuilder = reflectionEmitInfo.AssemblyBuilder.DefineDynamicModule("Module");
#else
                    reflectionEmitInfo.ModuleBuilder = reflectionEmitInfo.AssemblyBuilder.DefineDynamicModule("Module", this.Options.EnableDebugging);
#endif

                    ReflectionEmitInfo = reflectionEmitInfo;
                }

                // Create a new type to hold our method.
                var typeBuilder = reflectionEmitInfo.ModuleBuilder.DefineType("JavaScriptClass" + reflectionEmitInfo.TypeCount.ToString(), System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Class);
                reflectionEmitInfo.TypeCount++;

                // Create a method.
                var methodBuilder = typeBuilder.DefineMethod(this.GetMethodName(),
                    System.Reflection.MethodAttributes.HideBySig | System.Reflection.MethodAttributes.Static | System.Reflection.MethodAttributes.Public,
                    typeof(object), GetParameterTypes());

                // Generate the IL for the method.
                generator = new ReflectionEmitILGenerator(methodBuilder.GetILGenerator());

                if (this.Options.EnableILAnalysis == true)
                {
                    // Replace the generator with one that logs.
                    generator = new LoggingILGenerator(generator);
                }

                if (this.Source.Path != null && this.Options.EnableDebugging == true)
                {
                    // Initialize the debugging information.
#if !NETSTANDARD1_5
                    optimizationInfo.DebugDocument = reflectionEmitInfo.ModuleBuilder.DefineDocument(this.Source.Path, COMHelpers.LanguageType, COMHelpers.LanguageVendor, COMHelpers.DocumentType);
#endif
                    methodBuilder.DefineParameter(1, System.Reflection.ParameterAttributes.None, "scriptEngine");
                    methodBuilder.DefineParameter(2, System.Reflection.ParameterAttributes.None, "scope");
                    methodBuilder.DefineParameter(3, System.Reflection.ParameterAttributes.None, "thisValue");
                }
                optimizationInfo.MarkSequencePoint(generator, new SourceCodeSpan(1, 1, 1, 1));
                GenerateCode(generator, optimizationInfo);
                generator.Complete();

                // Bake it.
                var type = typeBuilder.CreateTypeInfo();
                var methodInfo = type.GetMethod(this.GetMethodName());
#if NETSTANDARD1_5
                this.GeneratedMethod = new GeneratedMethod(methodInfo.CreateDelegate(GetDelegate()), optimizationInfo.NestedFunctions);
#else
                this.GeneratedMethod = new GeneratedMethod(Delegate.CreateDelegate(GetDelegate(), methodInfo), optimizationInfo.NestedFunctions);
#endif
#endif //WINDOWS_PHONE
                }

                if (this.Options.EnableILAnalysis == true)
            {
                // Store the disassembled IL so it can be retrieved for analysis purposes.
                this.GeneratedMethod.DisassembledIL = generator.ToString();
            }
        }

        /// <summary>
        /// Generates IL for the script.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected abstract void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo);

        /// <summary>
        /// Represents a delegate that is used for global code.  For internal use only.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="scope"> The scope (global or eval context) or the parent scope (function
        /// context). </param>
        /// <param name="thisObject"> The value of the <c>this</c> keyword. </param>
        /// <returns> The result of calling the method. </returns>
        protected delegate object GlobalCodeDelegate(ScriptEngine engine, Scope scope, object thisObject);

        /// <summary>
        /// Retrieves a delegate for the generated method.
        /// </summary>
        /// <returns> The delegate type that matches the method parameters. </returns>
        protected virtual Type GetDelegate()
        {
            return typeof(GlobalCodeDelegate);
        }
    }

}