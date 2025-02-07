using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Selects a method from a list of candidates and performs type conversion from actual
    /// argument type to formal argument type.
    /// </summary>
    internal abstract class MethodBinder : Binder
    {
        private string name;
        private Type declaringType;
        private int functionLength;
        



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Binder instance.
        /// </summary>
        /// <param name="targetMethod"> A method to bind to. </param>
        protected MethodBinder(BinderMethod targetMethod)
        {
            if (targetMethod == null)
                throw new ArgumentNullException(nameof(targetMethod));
            this.name = targetMethod.Name;
            this.declaringType = targetMethod.DeclaringType;
            this.functionLength = targetMethod.RequiredParameterCount +
                targetMethod.OptionalParameterCount + (targetMethod.HasParamArray ? 1 : 0);
        }

        /// <summary>
        /// Creates a new Binder instance.
        /// </summary>
        /// <param name="targetMethods"> An enumerable list of methods to bind to.  At least one
        /// method must be provided.  Every method must have the same name. </param>
        protected MethodBinder(IEnumerable<BinderMethod> targetMethods)
        {
            if (targetMethods == null)
                throw new ArgumentNullException(nameof(targetMethods));

            // At least one method must be provided.
            // Every method must have the same name
            foreach (var method in targetMethods)
            {
                if (this.Name == null)
                {
                    this.name = method.Name;
                    this.declaringType = method.DeclaringType;
                }
                else
                {
                    if (this.Name != method.Name)
                        throw new ArgumentException(nameof(targetMethods));

                    // This code is removed, because now methods with same name from 
                    // the whole inheritance hierarchy are grouped together. 
                    // Otherwise method from the base class is not accessible when there 
                    // is a method with the same name in inherited class.
                    //if (this.declaringType != method.DeclaringType)
                    //    throw new ArgumentException(nameof(targetMethods));
                }
                this.functionLength = Math.Max(this.FunctionLength, method.RequiredParameterCount +
                    method.OptionalParameterCount + (method.HasParamArray ? 1 : 0));
            }
            if (this.Name == null)
                throw new ArgumentException(nameof(targetMethods));
        }




        //     PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the name of the target methods.
        /// </summary>
        public override string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the full name of the target methods, including the type name.
        /// </summary>
        public override string FullName
        {
            get { return string.Format("{0}.{1}", this.declaringType, this.name); }
        }

        /// <summary>
        /// Gets the maximum number of arguments of any of the target methods.  Used to set the
        /// length property on the function.
        /// </summary>
        public override int FunctionLength
        {
            get { return this.functionLength; }
        }
    }
}
