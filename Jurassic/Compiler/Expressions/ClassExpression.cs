using Jurassic.Library;
using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a class expression.
    /// </summary>
    internal sealed class ClassExpression : Expression
    {
        /// <summary>
        /// Creates a new class expression.
        /// </summary>
        /// <param name="name"> The class name. </param>
        /// <param name="extends"> The base class, or <c>null</c> if this class doesn't inherit
        /// from another class. </param>
        /// <param name="constructor"> The constructor, or <c>null</c> if the class doesn't have one. </param>
        /// <param name="members"> A list of class members. </param>
        public ClassExpression(string name, Expression extends, FunctionExpression constructor, List<FunctionExpression> members)
        {
            this.Name = name;
            this.Extends = extends;
            this.Constructor = constructor;
            this.Members = members ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// The class name, or <c>string.Empty</c> if none were specified.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The base class, or <c>null</c> if this class doesn't inherit from another class.
        /// </summary>
        public Expression Extends { get; private set; }

        /// <summary>
        /// The constructor, or <c>null</c> if the class doesn't have one.
        /// </summary>
        public FunctionExpression Constructor { get; private set; }

        /// <summary>
        /// Gets the list of class members, exluding the constructor.
        /// </summary>
        public IReadOnlyList<FunctionExpression> Members { get; private set; }

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
            // engine
            EmitHelpers.LoadScriptEngine(generator);

            // name
            generator.LoadString(this.Name);

            // extends
            if (Extends == null)
                generator.LoadNull();
            else
            {
                Extends.GenerateCode(generator, optimizationInfo);
                EmitConversion.ToAny(generator, Extends.ResultType);
            }

            // constructor
            if (Constructor == null)
                generator.LoadNull();
            else
                Constructor.GenerateCode(generator, optimizationInfo);

            // ConstructClass(ScriptEngine engine, string name, object extends, FunctionInstance constructor)
            generator.CallStatic(ReflectionHelpers.ReflectionHelpers_ConstructClass);

            // Create a variable to hold the container instance value.
            var containerVariable = generator.CreateTemporaryVariable(typeof(ObjectInstance));

            foreach (var member in this.Members)
            {
                // class.InstancePrototype
                generator.Duplicate();
                if (!member.Name.IsStatic)
                    generator.Call(ReflectionHelpers.FunctionInstance_InstancePrototype);

                // Store this in a variable so that FunctionExpression.GenerateCode can retrieve it.
                generator.Duplicate();
                generator.StoreVariable(containerVariable);

                // The key can be a property name or an expression that evaluates to a name.
                if (member.Name.HasStaticName)
                    generator.LoadString(member.Name.StaticName);
                else
                {
                    member.Name.ComputedName.GenerateCode(generator, optimizationInfo);
                    EmitConversion.ToPropertyKey(generator, member.Name.ComputedName.ResultType);
                }

                // Emit the function value.
                member.ContainerVariable = containerVariable;
                member.GenerateCode(generator, optimizationInfo);
                member.ContainerVariable = null;

                // Support the inferred function displayName property.
                //if (property.Name is LiteralExpression && ((LiteralExpression)property.Name).Value is string)
                //    functionValue.GenerateDisplayName(generator, optimizationInfo, "get " + (string)((LiteralExpression)property.Name).Value, true);

                var functionValue = member;
                if (member.Name.IsGetter)
                {
                    // Add a getter to the object.
                    generator.Call(ReflectionHelpers.ReflectionHelpers_SetClassGetter);
                }
                else if (member.Name.IsSetter)
                {
                    // Add a setter to the object.
                    generator.Call(ReflectionHelpers.ReflectionHelpers_SetClassSetter);
                }
                else
                {
                    // Add a new property to the object.
                    generator.Call(ReflectionHelpers.ReflectionHelpers_SetClassValue);
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
            var result = new System.Text.StringBuilder("class ");
            if (!string.IsNullOrEmpty(Name))
            {
                result.Append(Name);
                result.Append(' ');
            }
            if (Extends != null)
            {
                result.Append("extends ");
                result.Append(Extends);
                result.Append(' ');
            }
            result.Append('{');
            result.AppendLine();
            foreach (var member in this.Members)
            {
                result.Append("    ");
                result.Append(member);
            }
            result.Append("}");
            return result.ToString();
        }
    }
}