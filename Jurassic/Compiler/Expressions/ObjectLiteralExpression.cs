using Jurassic.Library;
using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents an object literal expression.
    /// </summary>
    internal sealed class ObjectLiteralExpression : Expression
    {
        /// <summary>
        /// Creates a new object literal expression.
        /// </summary>
        /// <param name="properties"> A list of property declarations. </param>
        public ObjectLiteralExpression(List<PropertyDeclaration> properties)
        {
            this.Properties = properties;
        }

        /// <summary>
        /// Gets the literal value.
        /// </summary>
        public IReadOnlyList<PropertyDeclaration> Properties
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
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Create a new object.
            EmitHelpers.LoadScriptEngine(generator);
            generator.Call(ReflectionHelpers.ScriptEngine_Object);
            generator.Call(ReflectionHelpers.Object_Construct);

            // Create a variable to hold the container instance value.
            var containerVariable = generator.CreateTemporaryVariable(typeof(ObjectInstance));
            generator.Duplicate();
            generator.StoreVariable(containerVariable);

            foreach (var property in this.Properties)
            {
                generator.Duplicate();

                // The key can be a property name or an expression that evaluates to a name.
                if (property.Name.HasStaticName)
                    generator.LoadString(property.Name.StaticName);
                else
                {
                    property.Name.ComputedName.GenerateCode(generator, optimizationInfo);
                    EmitConversion.ToPropertyKey(generator, property.Name.ComputedName.ResultType);
                }

                // Emit the property value.
                if (property.Value is FunctionExpression functionExpression)
                {
                    functionExpression.ContainerVariable = containerVariable;
                    property.Value.GenerateCode(generator, optimizationInfo);
                    functionExpression.ContainerVariable = null;
                }
                else
                {
                    property.Value.GenerateCode(generator, optimizationInfo);
                }
                EmitConversion.ToAny(generator, property.Value.ResultType);
                if (property.Name.IsGetter)
                {
                    // Add a getter to the object.
                    generator.Call(ReflectionHelpers.ReflectionHelpers_SetObjectLiteralGetter);
                }
                else if (property.Name.IsSetter)
                {
                    // Add a setter to the object.
                    generator.Call(ReflectionHelpers.ReflectionHelpers_SetObjectLiteralSetter);
                }
                else
                {
                    // Add a new property to the object.
                    generator.Call(ReflectionHelpers.ReflectionHelpers_SetObjectLiteralValue);
                }
            }

            // Release the variable that we created above.
            generator.ReleaseTemporaryVariable(containerVariable);
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            var result = new System.Text.StringBuilder("{");
            foreach (var property in this.Properties)
            {
                if (result.Length > 1)
                    result.Append(", ");
                if (property.Value is FunctionExpression functionExpression)
                {
                    result.Append(functionExpression.ToString());
                }
                else
                {
                    if (property.Name.HasStaticName)
                        result.Append(property.Name.StaticName);
                    else
                    {
                        result.Append('[');
                        result.Append(property.Name.ComputedName);
                        result.Append(']');
                    }
                    result.Append(": ");
                    result.Append(property.Value);
                }
            }
            result.Append("}");
            return result.ToString();
        }
    }
}