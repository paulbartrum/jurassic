using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a function expression.
    /// </summary>
    internal sealed class FunctionExpression : Expression
    {
        /// <summary>
        /// Creates a new instance of FunctionExpression.
        /// </summary>
        /// <param name="functionContext"> The function context to base this expression on. </param>
        public FunctionExpression(FunctionMethodGenerator functionContext)
        {
            if (functionContext == null)
                throw new ArgumentNullException("functionContext");
            this.Context = functionContext;
        }

        /// <summary>
        /// Gets the function context associated with this expression.
        /// </summary>
        public FunctionMethodGenerator Context
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Object; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Generate a new method.
            this.Context.GenerateCode();

            // Create a UserDefinedFunction.

            // prototype
            EmitHelpers.LoadScriptEngine(generator);
            generator.Call(ReflectionHelpers.ScriptEngine_Function);
            generator.Call(ReflectionHelpers.FunctionInstance_InstancePrototype);

            // name
            generator.LoadString(this.Context.Name);

            // argumentNames
            generator.LoadInt32(this.Context.ArgumentNames.Count);
            generator.NewArray(typeof(string));
            for (int i = 0; i < this.Context.ArgumentNames.Count; i++)
            {
                generator.Duplicate();
                generator.LoadInt32(i);
                generator.LoadString(this.Context.ArgumentNames[i]);
                generator.StoreArrayElement(typeof(string));
            }

            // scope
            EmitHelpers.LoadScope(generator);

            // body
            generator.LoadNull();
            generator.LoadMethodPointer(this.Context.GeneratedMethod);
            generator.NewObject(ReflectionHelpers.FunctionDelegate_Constructor);

            // new UserDefinedFunction(ObjectInstance prototype, string name, IList<string> argumentNames, DeclarativeScope scope, Func<Scope, object, object[], object> body)
            generator.NewObject(ReflectionHelpers.UserDefinedFunction_Constructor);
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            return this.Context.ToString();
        }
    }

}