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
        /// Generates CIL to set the display name of the function.  The function should be on top of the stack.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <param name="displayName"> The display name of the function. </param>
        /// <param name="force"> <c>true</c> to set the displayName property, even if the function has a name already. </param>
        public void GenerateDisplayName(ILGenerator generator, OptimizationInfo optimizationInfo, string displayName, bool force)
        {
            if (displayName == null)
                throw new ArgumentNullException("displayName");

            // We only infer names for functions if the function doesn't have a name.
            if (force == true || string.IsNullOrEmpty(this.Context.Name))
            {
                // Statically set the display name.
                this.Context.DisplayName = displayName;

                // Generate code to set the display name at runtime.
                generator.Duplicate();
                generator.LoadString("displayName");
                generator.LoadString(displayName);
                generator.LoadBoolean(false);
                generator.Call(ReflectionHelpers.ObjectInstance_SetPropertyValue_String);
            }
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