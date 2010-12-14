using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jurassic;
using Jurassic.Compiler;

namespace Jurassic.Library
{

    /// <summary>
    /// Selects a member from a list of candidates and performs type conversion from actual
    /// argument type to formal argument type.
    /// </summary>
    [Serializable]
    internal class FunctionBinder
    {
        private FunctionBinderMethod[] buckets;
        
        internal const int MaximumSupportedParameterCount = 8;

        [NonSerialized]
        private BinderDelegate[] delegateCache;

        /// <summary>
        /// Creates a new FunctionBinder instance.
        /// </summary>
        /// <param name="targetMethods"> An array of methods to bind to. </param>
        public FunctionBinder(params FunctionBinderMethod[] targetMethods)
            : this((IEnumerable<FunctionBinderMethod>)targetMethods)
        {
        }

        /// <summary>
        /// Creates a new FunctionBinder instance.
        /// </summary>
        /// <param name="targetMethods"> An enumerable list of methods to bind to. </param>
        public FunctionBinder(IEnumerable<FunctionBinderMethod> targetMethods)
        {
            if (targetMethods == null)
                throw new ArgumentNullException("targetMethods");
            if (targetMethods.FirstOrDefault() == null)
                throw new ArgumentException("At least one method must be supplied.", "targetMethods");

            // Split the methods by the number of parameters they take.
            this.buckets = new FunctionBinderMethod[MaximumSupportedParameterCount + 1];
            for (int argumentCount = 0; argumentCount < this.buckets.Length; argumentCount++)
            {
                // Find all the methods that have the right number of parameters.
                FunctionBinderMethod preferred = null;
                foreach (var method in targetMethods)
                {
                    if (argumentCount >= method.MinParameterCount && argumentCount <= method.MaxParameterCount)
                    {
                        if (preferred != null)
                            throw new ArgumentException(string.Format("Multiple ambiguous methods detected: {0} and {1}.", method, preferred), "targetMethods");
                        preferred = method;
                    }
                }
                this.buckets[argumentCount] = preferred;
            }

            // If a bucket has no methods, search all previous buckets, then all search forward.
            for (int argumentCount = 0; argumentCount < this.buckets.Length; argumentCount++)
            {
                if (this.buckets[argumentCount] != null)
                    continue;

                // Search previous buckets.
                for (int i = argumentCount - 1; i >= 0; i --)
                    if (this.buckets[i] != null)
                    {
                        this.buckets[argumentCount] = this.buckets[i];
                        break;
                    }

                // If that didn't work, search forward.
                if (this.buckets[argumentCount] == null)
                {
                    for (int i = argumentCount + 1; i < this.buckets.Length; i++)
                        if (this.buckets[i] != null)
                        {
                            this.buckets[argumentCount] = this.buckets[i];
                            break;
                        }
                }

                // If that still didn't work, then we have a problem.
                if (this.buckets[argumentCount] == null)
                    throw new InvalidOperationException("No preferred method could be found.");
            }

            // Allocate an array containing delegates.
            this.delegateCache = new BinderDelegate[MaximumSupportedParameterCount + 1];
        }

        /// <summary>
        /// Calls the method represented by this object.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="thisObject"> The value of the <c>this</c> keyword. </param>
        /// <param name="arguments"> The arguments to pass to the function. </param>
        /// <returns> The result of calling the method. </returns>
        public object Call(ScriptEngine engine, object thisObject, params object[] arguments)
        {
            BinderDelegate delegateToCall;

            // Create a delegate or retrieve it from the cache.
            if (arguments.Length <= MaximumSupportedParameterCount)
            {
                delegateToCall = this.delegateCache[arguments.Length];
                if (delegateToCall == null)
                    delegateToCall = this.delegateCache[arguments.Length] = CreateBinder(arguments.Length);
            }
            else
                delegateToCall = CreateBinder(arguments.Length);

            // Execute the delegate.
            return delegateToCall(engine, thisObject, arguments);
        }

        /// <summary>
        /// Creates a delegate that does type conversion and calls the method represented by this
        /// object.
        /// </summary>
        /// <param name="argumentCount"> The number of arguments that will be passed to the delegate. </param>
        /// <returns> A delegate that does type conversion and calls the method represented by this
        /// object. </returns>
        public BinderDelegate CreateBinder(int argumentCount)
        {
            // Find the method to call.
            var targetMethod = this.buckets[Math.Min(argumentCount, this.buckets.Length - 1)];

            // Create a binding method.
            return CreateSingleMethodBinder(argumentCount, targetMethod);
        }

        ///// <summary>
        ///// Creates a delegate that matches the given method.
        ///// </summary>
        ///// <param name="binderMethod"> The method to create a delegate for. </param>
        ///// <returns> A delegate that matches the given method. </returns>
        //private static Type CreateDelegateType(FunctionBinderMethod binderMethod)
        //{
        //    var parameters = binderMethod.Method.GetParameters();
        //    bool includeReturnType = binderMethod.Method.ReturnType != typeof(void);
        //    string delegateTypeName = includeReturnType ? "System.Func`{0}" : "System.Action`{0}";
        //    Type[] typeArguments = new Type[parameters.Length + (binderMethod.HasThisParameter ? 1 : 0) + (includeReturnType ? 1 : 0)];
        //    if (binderMethod.HasThisParameter == true)
        //        typeArguments[0] = binderMethod.HasExplicitThisParameter ? parameters[0].ParameterType : binderMethod.Method.DeclaringType;
        //    for (int i = 0; i < parameters.Length - (binderMethod.HasExplicitThisParameter ? 1 : 0); i++)
        //        typeArguments[i + (binderMethod.HasThisParameter ? 1 : 0)] = parameters[i + (binderMethod.HasExplicitThisParameter ? 1 : 0)].ParameterType;
        //    if (includeReturnType == true)
        //        typeArguments[typeArguments.Length - 1] = binderMethod.Method.ReturnType;
        //    return Assembly.GetAssembly(typeof(Func<>)).GetType(string.Format(delegateTypeName, typeArguments.Length)).MakeGenericType(typeArguments);
        //}

        /// <summary>
        /// Creates a delegate with the given type that does parameter conversion as necessary
        /// and then calls the given method.
        /// </summary>
        /// <param name="argumentCount"> The number of arguments that were supplied. </param>
        /// <param name="binderMethod"> The method to call. </param>
        /// <returns> A delegate with the given type that does parameter conversion as necessary
        /// and then calls the given method. </returns>
        private static BinderDelegate CreateSingleMethodBinder(int argumentCount, FunctionBinderMethod binderMethod)
        {
            // Create a new dynamic method.
            System.Reflection.Emit.DynamicMethod dm;
            ILGenerator generator;
#if !SILVERLIGHT
            if (ScriptEngine.LowPrivilegeEnvironment == false)
            {
                // Full trust only - skips visibility checks.
                dm = new System.Reflection.Emit.DynamicMethod(
                    string.Format("binder_for_{0}_{1}", binderMethod.Method.DeclaringType.Name, binderMethod.Name), // Name of the generated method.
                    typeof(object),                                                                                 // Return type of the generated method.
                    new Type[] { typeof(ScriptEngine), typeof(object), typeof(object[]) },                          // Parameter types of the generated method.
                    typeof(FunctionBinder),                                                                         // Owner type.
                    true);                                                                                          // Skips visibility checks.
                generator = new DynamicILGenerator(dm);
            }
            else
            {
#endif
                // Partial trust / silverlight.
                dm = new System.Reflection.Emit.DynamicMethod(
                    string.Format("binder_for_{0}_{1}", binderMethod.Method.DeclaringType.Name, binderMethod.Name), // Name of the generated method.
                    typeof(object),                                                                                 // Return type of the generated method.
                    new Type[] { typeof(ScriptEngine), typeof(object), typeof(object[]) });                         // Parameter types of the generated method.
                generator = new ReflectionEmitILGenerator(dm.GetILGenerator());
#if !SILVERLIGHT
            }
#endif

            // Here is what we are going to generate.
            //private static object SampleBinder(ScriptEngine engine, object thisObject, object[] arguments)
            //{
            //    // Target function signature: int (bool, int, string, object).
            //    bool param1;
            //    int param2;
            //    string param3;
            //    object param4;
            //    param1 = arguments[0] != 0;
            //    param2 = TypeConverter.ToInt32(arguments[1]);
            //    param3 = TypeConverter.ToString(arguments[2]);
            //    param4 = Undefined.Value;
            //    return thisObject.targetMethod(param1, param2, param3, param4);
            //}

            CreateSingleMethodBinder(argumentCount, binderMethod, generator);

            // Convert the DynamicMethod to a delegate.
            return (BinderDelegate)dm.CreateDelegate(typeof(BinderDelegate));
        }

        /// <summary>
        /// Outputs IL that does parameter conversion as necessary and then calls the given method.
        /// </summary>
        /// <param name="argumentCount"> The number of arguments that were supplied. </param>
        /// <param name="binderMethod"> The method to call. </param>
        /// <param name="il"> The ILGenerator to output to. </param>
        internal static void CreateSingleMethodBinder(int argumentCount, FunctionBinderMethod binderMethod, ILGenerator il)
        {

            // Get information about the target method.
            var targetMethod = binderMethod.Method;
            ParameterInfo[] targetParameters = targetMethod.GetParameters();
            ParameterInfo targetReturnParameter = targetMethod.ReturnParameter;

            // Emit the "engine" parameter.
            if (binderMethod.HasEngineParameter)
            {
                // Load the "engine" parameter passed by the client.
                il.LoadArgument(0);
            }

            // Emit the "this" parameter.
            if (binderMethod.HasThisParameter)
            {
                // Load the "this" parameter passed by the client.
                il.LoadArgument(1);

                bool inheritsFromObjectInstance = typeof(ObjectInstance).IsAssignableFrom(binderMethod.ThisType);
                if (binderMethod.ThisType.IsClass == true && inheritsFromObjectInstance == false &&
                    binderMethod.ThisType != typeof(string) && binderMethod.ThisType != typeof(object))
                {
                    // If the "this" object is an unsupported class, pass it through unmodified.
                    il.CastClass(binderMethod.ThisType);
                }
                else
                {
                    if (binderMethod.ThisType != typeof(object))
                    {
                        // If the target "this" object type is not of type object, throw an error if
                        // the value is undefined or null.
                        il.Duplicate();
                        var temp = il.CreateTemporaryVariable(typeof(object));
                        il.StoreVariable(temp);
                        il.LoadArgument(0);
                        il.LoadVariable(temp);
                        il.LoadString(binderMethod.Name);
                        il.Call(ReflectionHelpers.TypeUtilities_VerifyThisObject);
                        il.ReleaseTemporaryVariable(temp);
                    }

                    // Convert to the target type.
                    EmitTypeConversion(il, typeof(object), binderMethod.ThisType);

                    if (binderMethod.ThisType != typeof(ObjectInstance) && inheritsFromObjectInstance == true)
                    {
                        // EmitConversionToObjectInstance can emit null if the toType is derived from ObjectInstance.
                        // Therefore, if the value emitted is null it means that the "thisObject" is a type derived
                        // from ObjectInstance (e.g. FunctionInstance) and the value provided is a different type
                        // (e.g. ArrayInstance).  In this case, throw an exception explaining that the function is
                        // not generic.
                        var endOfThrowLabel = il.CreateLabel();
                        il.Duplicate();
                        il.BranchIfNotNull(endOfThrowLabel);
                        il.LoadArgument(0);
                        EmitHelpers.EmitThrow(il, "TypeError", string.Format("The method '{0}' is not generic", binderMethod.Name));
                        il.DefineLabelPosition(endOfThrowLabel);
                    }
                }
            }

            // Emit the parameters to the target function.
            int offset = (binderMethod.HasEngineParameter ? 1 : 0) + (binderMethod.HasExplicitThisParameter ? 1 : 0);
            int initialEmitCount = targetParameters.Length - offset - (binderMethod.HasParamArray ? 1 : 0);
            for (int i = 0; i < initialEmitCount; i++)
            {
                var targetParameter = targetParameters[i + offset];
                if (i < argumentCount)
                {
                    // Load the argument onto the stack.
                    il.LoadArgument(2);
                    il.LoadInt32(i);
                    il.LoadArrayElement(typeof(object));

                    if (Attribute.GetCustomAttribute(targetParameter, typeof(JSDoNotConvertAttribute)) == null)
                    {
                        // Convert the input parameter to the correct type.
                        EmitTypeConversion(il, typeof(object), targetParameter);
                    }
                    else
                    {
                        // Don't do argument conversion.
                        if (targetParameter.ParameterType != typeof(ObjectInstance))
                            throw new NotImplementedException("[JSDoNotConvert] is only supported for arguments of type ObjectInstance.");
                        
                        var endOfThrowLabel = il.CreateLabel();
                        il.IsInstance(typeof(ObjectInstance));
                        il.Duplicate();
                        il.BranchIfNotNull(endOfThrowLabel);
                        EmitHelpers.EmitThrow(il, "TypeError", string.Format("The {1} parameter of {0}() must be an object", binderMethod.Name,
                            i == 0 ? "first" : i == 1 ? "second" : i == 2 ? "third" : string.Format("{0}th", i + 1)));
                        il.DefineLabelPosition(endOfThrowLabel);
                    }
                }
                else
                {
                    // The target method has more parameters than we have input values.
                    EmitUndefined(il, targetParameter);
                }
            }

            // Emit any ParamArray arguments.
            if (binderMethod.HasParamArray)
            {
                // Create an array to pass to the ParamArray parameter.
                var elementType = targetParameters[targetParameters.Length - 1].ParameterType.GetElementType();
                il.LoadInt32(Math.Max(argumentCount - initialEmitCount, 0));
                il.NewArray(elementType);

                for (int i = initialEmitCount; i < argumentCount; i++)
                {
                    // Emit the array and index.
                    il.Duplicate();
                    il.LoadInt32(i - initialEmitCount);

                    // Extract the input parameter and do type conversion as normal.
                    il.LoadArgument(2);
                    il.LoadInt32(i);
                    il.LoadArrayElement(typeof(object));
                    if (elementType != typeof(object))
                    {
                        // Convert to the target type.
                        EmitTypeConversion(il, typeof(object), elementType);
                    }

                    // Store each parameter in the array.
                    il.StoreArrayElement(elementType);
                }
            }

            // Emit the call.
            il.Call(targetMethod);

            // Convert the return value.
            if (targetReturnParameter.ParameterType == typeof(void))
                EmitHelpers.EmitUndefined(il);
            else
                EmitTypeConversion(il, targetReturnParameter.ParameterType, typeof(object));

            // End the IL.
            il.Complete();
        }

        /// <summary>
        /// Pops the value on the stack, converts it from one type to another, then pushes the
        /// result onto the stack.  Undefined is converted to the given default value.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        /// <param name="targetParameter"> The type to convert to and the default value, if there is one. </param>
        private static void EmitTypeConversion(ILGenerator generator, Type fromType, ParameterInfo targetParameter)
        {
            // Emit either the default value if there is one, otherwise emit "undefined".
            if ((targetParameter.Attributes & ParameterAttributes.HasDefault) != ParameterAttributes.None)
            {
                // Check if the input value is undefined.
                var elseClause = generator.CreateLabel();
                generator.Duplicate();
                generator.BranchIfNull(elseClause);
                generator.Duplicate();
                generator.LoadField(ReflectionHelpers.Undefined_Value);
                generator.CompareEqual();
                generator.BranchIfTrue(elseClause);

                // Convert as per normal.
                EmitTypeConversion(generator, fromType, targetParameter.ParameterType);

                // Jump to the end.
                var endOfIf = generator.CreateLabel();
                generator.Branch(endOfIf);
                generator.DefineLabelPosition(elseClause);

                // Pop the existing value and emit the default value.
                generator.Pop();
                EmitUndefined(generator, targetParameter);

                // Define the end of the block.
                generator.DefineLabelPosition(endOfIf);
            }
            else
            {
                // Convert as per normal.
                EmitTypeConversion(generator, fromType, targetParameter.ParameterType);
            }
        }

        /// <summary>
        /// Pops the value on the stack, converts it from one type to another, then pushes the
        /// result onto the stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        /// <param name="toType"> The type to convert to. </param>
        private static void EmitTypeConversion(ILGenerator il, Type fromType, Type toType)
        {
            // If the source type equals the destination type, then there is nothing to do.
            if (fromType == toType)
                return;

            // Emit for each type of argument we support.
            if (toType == typeof(int))
                EmitConversion.ToInteger(il, PrimitiveTypeUtilities.ToPrimitiveType(fromType));
            else if (typeof(ObjectInstance).IsAssignableFrom(toType))
            {
                EmitConversion.Convert(il, PrimitiveTypeUtilities.ToPrimitiveType(fromType), PrimitiveType.Object);
                if (toType != typeof(ObjectInstance))
                {
                    // Convert to null if the from type isn't compatible with the to type.
                    // For example, if the target type is FunctionInstance and the from type is ArrayInstance, then pass null.
                    il.IsInstance(toType);
                }
            }
            else
                EmitConversion.Convert(il, PrimitiveTypeUtilities.ToPrimitiveType(fromType), PrimitiveTypeUtilities.ToPrimitiveType(toType));
        }

        /// <summary>
        /// Pushes the result of converting <c>undefined</c> to the given type onto the stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="targetParameter"> The type to convert to, and optionally a default value. </param>
        private static void EmitUndefined(ILGenerator il, ParameterInfo targetParameter)
        {
            // Emit either the default value if there is one, otherwise emit "undefined".
            if ((targetParameter.Attributes & ParameterAttributes.HasDefault) != ParameterAttributes.None)
            {
                // Emit the default value.
                if (targetParameter.DefaultValue is bool)
                    il.LoadInt32(((bool)targetParameter.DefaultValue) ? 1 : 0);
                else if (targetParameter.DefaultValue is int)
                    il.LoadInt32((int)targetParameter.DefaultValue);
                else if (targetParameter.DefaultValue is double)
                    il.LoadDouble((double)targetParameter.DefaultValue);
                else if (targetParameter.DefaultValue == null)
                    il.LoadNull();
                else if (targetParameter.DefaultValue is string)
                    il.LoadString((string)targetParameter.DefaultValue);
                else
                    throw new NotImplementedException(string.Format("Unsupported default value type '{1}' for parameter '{0}'.",
                        targetParameter.Name, targetParameter.DefaultValue.GetType()));
            }
            else
            {
                // Convert Undefined to the target type and emit.
                EmitHelpers.EmitUndefined(il);
                EmitTypeConversion(il, typeof(object), targetParameter.ParameterType);
            }
        }
    }
}
