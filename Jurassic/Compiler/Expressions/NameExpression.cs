using System;
using Jurassic.Library;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a variable or part of a member reference.
    /// </summary>
    internal sealed class NameExpression : Expression, IReferenceExpression
    {
        /// <summary>
        /// Creates a new NameExpression instance.
        /// </summary>
        /// <param name="scope"> The current scope. </param>
        /// <param name="name"> The name of the variable or member that is being referenced. </param>
        public NameExpression(Scope scope, string name)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            this.Scope = scope;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the scope the name is contained within.
        /// </summary>
        public Scope Scope
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the variable or member.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get
            {
                var variableInfo = this.Scope.FindStaticVariable(this.Name);
                if (variableInfo != null)
                    return variableInfo.Type;
                return PrimitiveType.Any;
            }
        }

        /// <summary>
        /// Gets the static type of the reference.
        /// </summary>
        public PrimitiveType Type
        {
            get { return this.ResultType; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // NOTE: this is a get reference because assignment expressions do not call this method.
            GenerateGet(generator, optimizationInfo, true);
        }

        /// <summary>
        /// Outputs the values needed to get or set this reference.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void GenerateReference(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Do nothing.
        }

        /// <summary>
        /// Outputs the values needed to get or set this reference.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void DuplicateReference(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Do nothing.
        }

        /// <summary>
        /// Pushes the value of the reference onto the stack.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <param name="throwIfUnresolvable"> <c>true</c> to throw a ReferenceError exception if
        /// the name is unresolvable; <c>false</c> to output <c>null</c> instead. </param>
        public void GenerateGet(ILGenerator generator, OptimizationInfo optimizationInfo, bool throwIfUnresolvable)
        {
            // If we have allocated an IL variable, use it.
            var variableInfo = Scope.FindStaticVariable(Name);
            if (variableInfo != null && variableInfo.Store != null)
            {
                generator.LoadVariable(variableInfo.Store);
                if (variableInfo.Keyword != KeywordToken.Var)
                {
                    var afterIf = generator.CreateLabel();
                    generator.Duplicate();
                    generator.BranchIfNotNull(afterIf);
                    EmitHelpers.EmitThrow(generator, ErrorType.ReferenceError, $"Cannot access '{Name}' before initialization.");
                    generator.DefineLabelPosition(afterIf);

                }
                return;
            }

            // Fallback: call RuntimeScope.GetValue() or RuntimeScope.GetValueNoThrow().
            Scope.GenerateReference(generator, optimizationInfo);
            generator.LoadString(Name);
            generator.LoadInt32(optimizationInfo.SourceSpan.StartLine);
            generator.LoadStringOrNull(optimizationInfo.Source.Path);
            generator.Call(throwIfUnresolvable ? ReflectionHelpers.RuntimeScope_GetValue : ReflectionHelpers.RuntimeScope_GetValueNoThrow);
        }

        /// <summary>
        /// Stores the value on the top of the stack in the reference.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        /// <param name="valueType"> The primitive type of the value that is on the top of the stack. </param>
        public void GenerateSet(ILGenerator generator, OptimizationInfo optimizationInfo, PrimitiveType valueType)
        {
            // TODO: Optimize this.
            //
            // ++ or --
            // GenerateReference
            // DuplicateReference
            // GenerateGet
            // ToNumber
            // if (postfix) Dup/Store in variable
            // Increment/Decrement
            // if (prefix) Dup/Store in variable
            // GenerateSet
            // Load variable
            // 
            // +=
            // GenerateReference
            // DuplicateReference
            // GenerateGet
            // Dup/Store in variable
            // GenerateSet
            // Load variable
            // 
            // for (in/of)
            // GenerateReference
            // LoadVariable(enumerator)
            // GenerateGet
            // GenerateSet
            // 
            // =
            // GenerateReference
            // target.GenerateGet
            // Dup/Store in variable
            // GenerateSet
            // Load variable

            // If we have allocated an IL variable, use it.
            var variableInfo = Scope.FindStaticVariable(Name);
            if (variableInfo != null && variableInfo.Store != null)
            {
                EmitConversion.Convert(generator, valueType, variableInfo.Type);
                generator.StoreVariable(variableInfo.Store);
                return;
            }

            // Fallback: call RuntimeScope.SetValue() or RuntimeScope.SetValueStrict().
            var temp = generator.CreateTemporaryVariable(valueType);
            generator.StoreVariable(temp);
            Scope.GenerateReference(generator, optimizationInfo);
            generator.LoadString(Name);
            generator.LoadVariable(temp);
            EmitConversion.ToAny(generator, valueType);
            generator.LoadInt32(optimizationInfo.SourceSpan.StartLine);
            generator.LoadStringOrNull(optimizationInfo.Source.Path);
            generator.Call(optimizationInfo.StrictMode ? ReflectionHelpers.RuntimeScope_SetValueStrict : ReflectionHelpers.RuntimeScope_SetValue);
            generator.ReleaseTemporaryVariable(temp);
        }

        /// <summary>
        /// Deletes the reference and pushes <c>true</c> if the delete succeeded, or <c>false</c>
        /// if the delete failed.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void GenerateDelete(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            // Deleting a variable is not allowed in strict mode.
            if (optimizationInfo.StrictMode == true)
                throw new SyntaxErrorException($"Cannot delete {Name} because deleting a variable or argument is not allowed in strict mode", optimizationInfo.SourceSpan.StartLine, optimizationInfo.Source.Path, optimizationInfo.FunctionName);

            // If we have allocated an IL variable, then always return false, as we don't support
            // optimizing deletable variables.
            var variableInfo = Scope.FindStaticVariable(Name);
            if (variableInfo != null && variableInfo.Store != null)
            {
                generator.LoadBoolean(false);
                return;
            }

            Scope.GenerateReference(generator, optimizationInfo);
            generator.LoadString(Name);
            generator.Call(ReflectionHelpers.RuntimeScope_Delete);
        }

        /// <summary>
        /// Calculates the hash code for this object.
        /// </summary>
        /// <returns> The hash code for this object. </returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Scope.GetHashCode();
        }

        /// <summary>
        /// Determines if the given object is equal to this one.
        /// </summary>
        /// <param name="obj"> The object to compare. </param>
        /// <returns> <c>true</c> if the given object is equal to this one; <c>false</c> otherwise. </returns>
        public override bool Equals(object obj)
        {
            if ((obj is NameExpression) == false)
                return false;
            return this.Name == ((NameExpression)obj).Name && this.Scope == ((NameExpression)obj).Scope;
        }

        /// <summary>
        /// Converts the expression to a string.
        /// </summary>
        /// <returns> A string representing this expression. </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}