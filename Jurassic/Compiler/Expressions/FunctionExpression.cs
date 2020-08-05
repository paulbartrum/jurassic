using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a function expression.
    /// </summary>
    internal sealed class FunctionExpression : Expression
    {
        private FunctionMethodGenerator context;

        /// <summary>
        /// Creates a new instance of FunctionExpression.
        /// </summary>
        /// <param name="functionContext"> The function context to base this expression on. </param>
        /// <param name="scope"> The scope that was in effect where the function was declared. </param>
        public FunctionExpression(FunctionMethodGenerator functionContext, Scope scope)
        {
            this.context = functionContext ?? throw new ArgumentNullException(nameof(functionContext));
            this.Scope = scope;
        }

        /// <summary>
        /// Indicates how the function was declared.
        /// </summary>
        public FunctionDeclarationType DeclarationType
        {
            get { return this.context.DeclarationType; }
        }

        /// <summary>
        /// Gets the name of the function. Can be <c>null</c>.
        /// </summary>
        public PropertyName Name
        {
            get { return this.context.Name; }
        }

        /// <summary>
        /// Gets a list of argument names and default values.
        /// </summary>
        public IList<FunctionArgument> Arguments
        {
            get { return this.context.Arguments; }
        }

        /// <summary>
        /// Gets the source code for the body of the function.
        /// </summary>
        public string BodyText
        {
            get { return this.context.BodyText; }
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Object; }
        }

        /// <summary>
        /// The scope that was in effect where the function was declared.
        /// </summary>
        public Scope Scope { get; set; }

        /// <summary>
        /// A variable that contains the declaring object.
        /// 1. In an object literal, the object literal instance.
        /// 2. In a class instance method, the class prototype.
        /// 3. In a class static method, the class itself.
        /// Used when generating code.
        /// </summary>
        public ILLocalVariable ContainerVariable { get; set; }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Generate a new method.
            this.context.GenerateCode();

            // Add the generated method to the nested function list.
            if (optimizationInfo.NestedFunctions == null)
                optimizationInfo.NestedFunctions = new List<GeneratedMethod>();
            optimizationInfo.NestedFunctions.Add(this.context.GeneratedMethod);

            // Add all the nested methods to the parent list.
            if (this.context.GeneratedMethod.Dependencies != null)
            {
                foreach (var nestedFunctionExpression in this.context.GeneratedMethod.Dependencies)
                    optimizationInfo.NestedFunctions.Add(nestedFunctionExpression);
            }

            // Store the generated method in the cache.
            long generatedMethodID = GeneratedMethod.Save(this.context.GeneratedMethod);

            // Create a UserDefinedFunction.

            // prototype
            EmitHelpers.LoadScriptEngine(generator);
            generator.Call(ReflectionHelpers.ScriptEngine_Function);
            generator.Call(ReflectionHelpers.FunctionInstance_InstancePrototype);

            // name
            string prefix = null;
            if (Name.IsGetter)
                prefix = "get ";
            else if (Name.IsSetter)
                prefix = "set ";
            if (Name.HasStaticName)
                generator.LoadString(prefix + Name.StaticName);
            else
            {
                // Compute the name at runtime.
                if (prefix != null)
                    generator.LoadString(prefix);
                Name.ComputedName.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToString(generator, Name.ComputedName.ResultType);
                if (prefix != null)
                    generator.CallStatic(ReflectionHelpers.String_Concat_String_String);
            }

            // argumentNames
            generator.LoadInt32(this.Arguments.Count);
            generator.NewArray(typeof(string));
            for (int i = 0; i < this.Arguments.Count; i++)
            {
                generator.Duplicate();
                generator.LoadInt32(i);
                generator.LoadString(this.Arguments[i].Name);
                generator.StoreArrayElement(typeof(string));
            }

            // scope
            Scope.GenerateReference(generator, optimizationInfo);

            // bodyText
            generator.LoadString(this.BodyText);

            // body
            generator.LoadInt64(generatedMethodID);
            generator.Call(ReflectionHelpers.GeneratedMethod_Load);

            // strictMode
            generator.LoadBoolean(this.context.StrictMode);

            // container
            if (ContainerVariable != null)
                generator.LoadVariable(ContainerVariable);
            else
                generator.LoadNull();

            // CreateFunction(ObjectInstance prototype, string name, IList<string> argumentNames,
            //   RuntimeScope scope, Func<Scope, object, object[], object> body,
            //   bool strictMode, FunctionInstance container)
            generator.CallStatic(ReflectionHelpers.ReflectionHelpers_CreateFunction);
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            var prefix = "function ";
            if (Name.IsGetter)
                prefix = "get ";
            else if (Name.IsSetter)
                prefix = "set ";

            return string.Format("{0} {1}({2}) {{\n{3}\n}}", prefix, this.Name,
                StringHelpers.Join(", ", this.Arguments), this.BodyText);
        }
    }

}