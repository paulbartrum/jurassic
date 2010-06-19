using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Jurassic.Library
{

    /// <summary>
    /// Represents a single method that the function binder can call.
    /// </summary>
    public class FunctionBinderMethod
    {
        /// <summary>
        /// Creates a new FunctionBinderMethod instance.
        /// </summary>
        /// <param name="method"> The method to call. </param>
        /// <param name="flags"> Flags that modify the binding process. </param>
        public FunctionBinderMethod(MethodInfo method, FunctionBinderFlags flags = FunctionBinderFlags.None)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            this.Method = method;
            this.Flags = flags;
            this.Preferred = (flags & FunctionBinderFlags.Preferred) != 0;
            this.HasExplicitThisParameter = method.IsStatic == true ? (flags & FunctionBinderFlags.HasThisObject) != 0 : false;

            var parameters = method.GetParameters();

            // Methods with more than eight parameters are not supported (Func<> and Action<> only go up to eight).
            this.DelegateParameterCount = this.HasThisParameter ? parameters.Length + 1 : parameters.Length;
            if (this.DelegateParameterCount > FunctionBinder.MaximumSupportedParameterCount)
                throw new NotImplementedException(string.Format("Methods with more than {0} parameters are not supported.", FunctionBinder.MaximumSupportedParameterCount));
            
            // If there is a "this" parameter, it must be of type ObjectInstance (or derived from it).
            if (this.HasExplicitThisParameter == true)
            {
                if (parameters.Length == 0)
                    throw new InvalidOperationException("Static methods with hasThisParameter=true must have at least one parameter.");
                this.ThisType = parameters[0].ParameterType;
            }
            else if (method.IsStatic == false)
            {
                this.ThisType = method.DeclaringType;
                if (typeof(ObjectInstance).IsAssignableFrom(this.ThisType) == false)
                    throw new InvalidOperationException(string.Format("Instance methods must be declared in a type deriving from {0}.", typeof(ObjectInstance).Name));
            }

            // Calculate the min and max parameter count.

            // Count the number of optional parameters to find the minimum parameter count.
            this.MinParameterCount = parameters.Count(p => (p.Attributes & ParameterAttributes.Optional) == 0);
            this.MaxParameterCount = parameters.Length;

            // If the last parameter is a ParamArray, then extend the maximum parameter count to unlimited.
            this.HasParamArray = parameters.Length > 0 && Attribute.IsDefined(parameters[parameters.Length - 1], typeof(ParamArrayAttribute));
            if (this.HasParamArray == true)
            {
                this.MinParameterCount -= 1;
                this.MaxParameterCount = Math.Max(this.MaxParameterCount, this.HasThisParameter ?
                    FunctionBinder.MaximumSupportedParameterCount - 1 : FunctionBinder.MaximumSupportedParameterCount);
            }

            // Methods with an explicit "this" parameter effectively have one less parameter.
            if (this.HasExplicitThisParameter == true)
            {
                this.MinParameterCount -= 1;
                this.MaxParameterCount -= 1;
            }

            // Check the parameter types (the this parameter has already been checked).
            // Only certain types are supported.
            for (int i = this.HasExplicitThisParameter ? 1 : 0; i < parameters.Length; i++)
            {
                Type type = parameters[i].ParameterType;

                // ParamArray types must be an array of a supported type.
                if (this.HasParamArray == true && i == parameters.Length - 1)
                {
                    if (type.IsArray == false)
                        throw new NotImplementedException(string.Format("Unsupported varargs parameter type '{0}'.", type));
                    type = type.GetElementType();
                }

                if (type != typeof(bool) &&
                    type != typeof(int) &&
                    type != typeof(double) &&
                    type != typeof(string) &&
                    type != typeof(object) &&
                    typeof(ObjectInstance).IsAssignableFrom(type) == false)
                    throw new NotImplementedException(string.Format("Unsupported parameter type '{0}'.", type));
            }
        }

        /// <summary>
        /// Gets a reference to a method.
        /// </summary>
        public MethodInfo Method
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the flags that were passed to the constructor.
        /// </summary>
        public FunctionBinderFlags Flags
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string Name
        {
            get { return this.Method.Name; }
        }

        /// <summary>
        /// Gets a value that indicates that this method is preferred when there are multiple
        /// methods to choose from.
        /// </summary>
        public bool Preferred
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value that indicates whether the "this" object should be passed as the first
        /// parameter.  Always false for instance methods.
        /// </summary>
        public bool HasExplicitThisParameter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the "this" value passed to this method.  Will be <c>null</c> if this
        /// is a static method and the "this" object is discarded.
        /// </summary>
        public Type ThisType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value that indicates whether the "this" object should be passed to the method.
        /// Always true for instance methods.
        /// </summary>
        public bool HasThisParameter
        {
            get { return this.ThisType != null; }
        }

        /// <summary>
        /// Gets the minimum number of parameters that this method requires (excluding the implicit
        /// this parameter).
        /// </summary>
        public int MinParameterCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of parameters that the method requires (excluding the implicit
        /// this parameter and the ParamArray parameter, if these are present).
        /// </summary>
        public int ParameterCount
        {
            get { return this.Method.GetParameters().Length - (this.HasExplicitThisParameter ? 1 : 0); }
        }

        /// <summary>
        /// Gets the maximum number of parameters that this method requires (excluding the implicit
        /// this parameter).
        /// </summary>
        public int MaxParameterCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of parameters a delegate based on this method requires (including the
        /// implicit this parameter).
        /// </summary>
        public int DelegateParameterCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value that indicates whether the last parameter is a ParamArray.
        /// </summary>
        public bool HasParamArray
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            return this.Method.ToString();
        }
    }

}
