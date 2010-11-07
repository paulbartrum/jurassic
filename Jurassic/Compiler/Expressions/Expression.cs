using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the base class of all javascript expressions.
    /// </summary>
    internal abstract class Expression
    {
        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public virtual PrimitiveType ResultType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        public void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
#if DEBUG && !SILVERLIGHT
            // Expressions must produce either zero or one value.
            int originalStackSize = 0;
            if (generator is DynamicILGenerator)
                originalStackSize = ((DynamicILGenerator)generator).StackSize;
#endif

            // Generate the code.
            this.GenerateCodeCore(generator, optimizationInfo);

#if DEBUG && !SILVERLIGHT
            /*if (optimizationInfo.SuppressReturnValue == true)
            {
                // Check that the stack count did not change.
                if (generator is DynamicILGenerator &&  ((DynamicILGenerator)generator).StackSize != originalStackSize)
                    throw new InvalidOperationException(string.Format("Encountered unexpected stack imbalance for expression '{0}'.", this));
            }
            else
            {*/
                // Check that the stack count increased by one.
                if (generator is DynamicILGenerator && ((DynamicILGenerator)generator).StackSize != originalStackSize + 1)
                    throw new InvalidOperationException(string.Format("Encountered unexpected stack imbalance for expression '{0}'.", this));
            //}
#endif
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected virtual void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            throw new NotImplementedException();
        }
    }

}