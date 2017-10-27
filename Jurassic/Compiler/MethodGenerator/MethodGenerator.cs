﻿using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// The default implementation of method generation helper
    /// </summary>
    internal class DefaultMethodGenerationHelper : IMethodGenerationHelper
    {
        private System.Reflection.Emit.DynamicMethod _dynamicMethod;

        public override System.Diagnostics.SymbolStore.ISymbolDocumentWriter DebugDocument
        { get => null; }

        public override System.Reflection.Emit.ILGenerator BeginMethodGeneration(string methodName, Type[] parametersTypes)
        {
            _dynamicMethod = new System.Reflection.Emit.DynamicMethod(
                methodName,                                        // Name of the generated method.
                typeof(object),                                         // Return type of the generated method.
                parametersTypes,                                    // Parameter types of the generated method.
                typeof(MethodGenerator),                                // Owner type.
                true);                                                  // Skip visibility checks.

            return _dynamicMethod.GetILGenerator();
        }

        public override Delegate EndMethodGeneration(Type delegateType, string methodName, Type[] parametersTypes)
        {
            return _dynamicMethod.CreateDelegate(delegateType);
        }

    };


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
            System.Reflection.Emit.DynamicMethod dynamicMethod;
            IMethodGenerationHelper methodHelper = this.Options.MethodGenerationHelper;

            if (methodHelper != null)
            {
                methodHelper.SetupMethodGeneration(this.Source, this.Options);
                generator = new ReflectionEmitILGenerator(methodHelper.BeginMethodGeneration(
                    GetMethodName(), 
                    GetParameterTypes())
                );
                optimizationInfo.DebugDocument = methodHelper.DebugDocument;
                dynamicMethod = null;
            }
            else
            {
                // Create a new dynamic method.
                dynamicMethod = new System.Reflection.Emit.DynamicMethod(
                    GetMethodName(),                                        // Name of the generated method.
                    typeof(object),                                         // Return type of the generated method.
                    GetParameterTypes(),                                    // Parameter types of the generated method.
                    typeof(MethodGenerator),                                // Owner type.
                    true);                                                  // Skip visibility checks.
#if USE_DYNAMIC_IL_INFO
                generator = new DynamicILGenerator(dynamicMethod);
#else
                generator = new ReflectionEmitILGenerator(dynamicMethod.GetILGenerator());
#endif
            }

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

            Delegate methodDelegate = this.Options.MethodGenerationHelper != null ?
                methodHelper.EndMethodGeneration(GetDelegate(), GetMethodName(), GetParameterTypes()) :
                dynamicMethod.CreateDelegate(GetDelegate());

            this.GeneratedMethod = new GeneratedMethod(methodDelegate, optimizationInfo.NestedFunctions);

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