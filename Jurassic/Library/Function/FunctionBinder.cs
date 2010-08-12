using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Jurassic;

namespace Jurassic.Library
{

    /// <summary>
    /// Selects a member from a list of candidates and performs type conversion from actual
    /// argument type to formal argument type.
    /// </summary>
    internal class FunctionBinder
    {
        private struct FunctionBinderBucket
        {
            public FunctionBinderMethod PreferredMethod;
            public FunctionBinderMethod[] OtherMethods;
        }
        private FunctionBinderBucket[] buckets;
        
        internal const int MaximumSupportedParameterCount = 8;

        private Dictionary<Type[], BinderDelegate> delegateCache;

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
            this.buckets = new FunctionBinderBucket[MaximumSupportedParameterCount + 1];
            for (int argumentCount = 0; argumentCount < this.buckets.Length; argumentCount++)
            {
                // Find all the methods that have the right number of parameters.
                FunctionBinderMethod preferred = null;
                List<FunctionBinderMethod> other = new List<FunctionBinderMethod>();
                foreach (var method in targetMethods)
                {
                    if (argumentCount >= method.MinParameterCount && argumentCount <= method.MaxParameterCount)
                    {
                        if (method.Preferred == true && preferred != null)
                            throw new ArgumentException(string.Format("Multiple ambiguous preferred methods detected: {0} and {1}.", method, preferred), "targetMethods");
                        if (method.Preferred == true)
                            preferred = method;
                        else
                            other.Add(method);
                    }
                }
                if (preferred == null && other.Count > 1)
                    throw new ArgumentException(string.Format("Multiple ambiguous non-preferred methods detected: {0}.", string.Join(", ", other)), "targetMethods");
                if (preferred == null && other.Count == 1)
                {
                    preferred = other[0];
                    other.RemoveAt(0);
                }
                this.buckets[argumentCount].PreferredMethod = preferred;
                this.buckets[argumentCount].OtherMethods = other.Count == 0 ? null : other.ToArray();
            }

            // If a bucket has no methods, search all previous buckets, then all search forward.
            for (int argumentCount = 0; argumentCount < this.buckets.Length; argumentCount++)
            {
                if (this.buckets[argumentCount].PreferredMethod != null)
                    continue;

                // Search previous buckets.
                for (int i = argumentCount - 1; i >= 0; i --)
                    if (this.buckets[i].PreferredMethod != null)
                    {
                        this.buckets[argumentCount].PreferredMethod = this.buckets[i].PreferredMethod;
                        break;
                    }

                // If that didn't work, search forward.
                if (this.buckets[argumentCount].PreferredMethod == null)
                {
                    for (int i = argumentCount + 1; i < this.buckets.Length; i++)
                        if (this.buckets[i].PreferredMethod != null)
                        {
                            this.buckets[argumentCount].PreferredMethod = this.buckets[i].PreferredMethod;
                            break;
                        }
                }

                // If that still didn't work, then we have a problem.
                if (this.buckets[argumentCount].PreferredMethod == null)
                    throw new InvalidOperationException("No preferred method could be found.");
            }
        }

        /// <summary>
        /// Implements a comparer that compares an array of types.  Types that inherit from
        /// ObjectInstance are considered identical.
        /// </summary>
        private class TypeArrayComparer : IEqualityComparer<Type[]>
        {
            public bool Equals(Type[] x, Type[] y)
            {
                if (x.Length != y.Length)
                    return false;
                for (int i = 0; i < x.Length; i++)
                    if (x[i] != y[i] && (typeof(ObjectInstance).IsAssignableFrom(x[i]) == false ||
                        typeof(ObjectInstance).IsAssignableFrom(y[i]) == false))
                        return false;
                return true;
            }

            public int GetHashCode(Type[] obj)
            {
                int total = 352654597;
                foreach (var type in obj)
                {
                    int typeHash = typeof(ObjectInstance).IsAssignableFrom(type) == true ?
                        typeof(ObjectInstance).GetHashCode() : type.GetHashCode();
                    total = (((total << 5) + total) + (total >> 27)) ^ typeHash;
                }
                return total;
            }
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
            // Extract the argument types.
            Type[] argumentTypes = GetArgumentTypes(arguments);

            // Create a delegate or retrieve it from the cache.
            var delegateToCall = CreateBinder(argumentTypes);

            // Execute the delegate.
            return delegateToCall(engine, thisObject, arguments);
        }

        /// <summary>
        /// Given an array of arguments, returns an array of types, one for each argument.
        /// </summary>
        /// <param name="arguments"> The arguments passed to the function. </param>
        /// <returns> An array of types. </returns>
        private Type[] GetArgumentTypes(object[] arguments)
        {
            // Possibly use Type.GetTypeArray instead?
            Type[] argumentTypes = new Type[arguments.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == null)
                    argumentTypes[i] = typeof(Undefined);
                else
                    argumentTypes[i] = arguments[i].GetType();
            }
            return argumentTypes;
        }

        /// <summary>
        /// Creates a delegate that does type conversion and calls the method represented by this
        /// object.
        /// </summary>
        /// <param name="argumentTypes"> The types of the arguments that will be passed to the delegate. </param>
        /// <returns> A delegate that does type conversion and calls the method represented by this
        /// object. </returns>
        public BinderDelegate CreateBinder(Type[] argumentTypes)
        {
            // Look up the delegate cache.
            if (this.delegateCache == null)
                this.delegateCache = new Dictionary<Type[], BinderDelegate>(2, new TypeArrayComparer());
            BinderDelegate result;
            if (this.delegateCache.TryGetValue(argumentTypes, out result) == true)
                return result;

            // Find the method to call.
            var targetMethod = FindBestMatchMethod(argumentTypes);

            // Create a binding method.
            var dynamicMethod = CreateSingleMethodBinder(argumentTypes, targetMethod);

            // Store the dynamic method in the cache.
            this.delegateCache.Add(argumentTypes, dynamicMethod);

            return dynamicMethod;
        }

        /// <summary>
        /// Selects a method based on the given argument types.
        /// </summary>
        /// <param name="argumentTypes"> An array of types corresponding to the arguments passed by
        /// the caller. </param>
        /// <returns> The selected method. </returns>
        private FunctionBinderMethod FindBestMatchMethod(Type[] argumentTypes)
        {
            var bucket = this.buckets[Math.Min(argumentTypes.Length, this.buckets.Length - 1)];

            // If there is only one candidate method, use that.
            if (bucket.OtherMethods == null)
                return bucket.PreferredMethod;

            // Search the other objects to see if the argument types match exactly.
            foreach (var binderMethod in bucket.OtherMethods)
            {
                var parameterTypes = binderMethod.Method.GetParameters();
                int indexOffset = (binderMethod.HasEngineParameter ? 1 : 0) + (binderMethod.HasExplicitThisParameter ? 1 : 0);
                var useThisMethod = true;
                for (int i = 0; i < Math.Min(argumentTypes.Length, parameterTypes.Length - indexOffset); i++)
                    if (parameterTypes[i + indexOffset].ParameterType != argumentTypes[i])
                    {
                        useThisMethod = false;
                        break;
                    }
                if (useThisMethod == true)
                    return binderMethod;
            }

            // A perfect match was not found, return the preferred match.
            return bucket.PreferredMethod;
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
        /// <param name="argumentTypes"> The types of the arguments that were supplied. </param>
        /// <param name="binderMethod"> The method to call. </param>
        /// <returns> A delegate with the given type that does parameter conversion as necessary
        /// and then calls the given method. </returns>
        private static BinderDelegate CreateSingleMethodBinder(Type[] argumentTypes, FunctionBinderMethod binderMethod)
        {
            // Create a new dynamic method.
            var dm = new DynamicMethod(
                "Binder",                                                               // Name of the generated method.
                typeof(object),                                                         // Return type of the generated method.
                new Type[] { typeof(ScriptEngine), typeof(object), typeof(object[]) },  // Parameter types of the generated method.
                typeof(FunctionBinder),                                                 // Owner type.
                true);                                                                  // Skip visibility checks.

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

            CreateSingleMethodBinder(argumentTypes, binderMethod, dm.GetILGenerator());

            // Convert the DynamicMethod to a delegate.
            return (BinderDelegate)dm.CreateDelegate(typeof(BinderDelegate));
        }

        /// <summary>
        /// Outputs IL that does parameter conversion as necessary and then calls the given method.
        /// </summary>
        /// <param name="argumentTypes"> The types of the arguments that were supplied. </param>
        /// <param name="binderMethod"> The method to call. </param>
        /// <param name="il"> The ILGenerator to output to. </param>
        internal static void CreateSingleMethodBinder(Type[] argumentTypes, FunctionBinderMethod binderMethod, ILGenerator il)
        {

            // Get information about the target method.
            var targetMethod = binderMethod.Method;
            ParameterInfo[] targetParameters = targetMethod.GetParameters();
            ParameterInfo targetReturnParameter = targetMethod.ReturnParameter;

            // Emit the "engine" parameter.
            if (binderMethod.HasEngineParameter)
            {
                // Load the "engine" parameter passed by the client.
                il.Emit(OpCodes.Ldarg_0);
            }

            // Emit the "this" parameter.
            if (binderMethod.HasThisParameter)
            {
                // Load the "this" parameter passed by the client.
                il.Emit(OpCodes.Ldarg_1);

                if (binderMethod.ThisType != typeof(object))
                {
                    // If the target "this" object type is not of type object, throw an error if
                    // the value is undefined or null.
                    il.Emit(OpCodes.Dup);
                    var temp = il.DeclareLocal(typeof(object));
                    il.Emit(OpCodes.Stloc_S, temp);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldloc_S, temp);
                    il.Emit(OpCodes.Ldstr, binderMethod.Name);
                    il.EmitCall(OpCodes.Call, ReflectionHelpers.TypeUtilities_VerifyThisObject, null);
                }

                // Convert to the target type.
                EmitConversion(il, typeof(object), binderMethod.ThisType);

                if (binderMethod.ThisType != typeof(ObjectInstance) && typeof(ObjectInstance).IsAssignableFrom(binderMethod.ThisType))
                {
                    // EmitConversionToObjectInstance can emit null if the toType is derived from ObjectInstance.
                    // Therefore, if the value emitted is null it means that the "thisObject" is a type derived
                    // from ObjectInstance (e.g. FunctionInstance) and the value provided is a different type
                    // (e.g. ArrayInstance).  In this case, throw an exception explaining that the function is
                    // not generic.
                    var endOfThrowLabel = il.DefineLabel();
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Brtrue_S, endOfThrowLabel);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldstr, "TypeError");
                    il.Emit(OpCodes.Ldstr, string.Format("The method '{0}' is not generic", binderMethod.Name));
                    il.Emit(OpCodes.Newobj, ReflectionHelpers.JavaScriptException_Constructor_Error);
                    il.Emit(OpCodes.Throw);
                    il.MarkLabel(endOfThrowLabel);
                }
            }

            // Emit the parameters to the target function.
            int offset = (binderMethod.HasEngineParameter ? 1 : 0) + (binderMethod.HasExplicitThisParameter ? 1 : 0);
            int initialEmitCount = targetParameters.Length - offset - (binderMethod.HasParamArray ? 1 : 0);
            for (int i = 0; i < initialEmitCount; i++)
            {
                var targetParameter = targetParameters[i + offset];
                if (i < argumentTypes.Length)
                {
                    // Load the argument onto the stack.
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Ldc_I4_S, (byte)i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    if (argumentTypes[i].IsClass == false)
                        il.Emit(OpCodes.Unbox_Any, argumentTypes[i]);

                    if (Attribute.GetCustomAttribute(targetParameter, typeof(JSDoNotConvertAttribute)) == null)
                    {
                        // Convert the input parameter to the correct type.
                        EmitConversion(il, argumentTypes[i], targetParameter);
                    }
                    else
                    {
                        // Don't do argument conversion.
                        if (targetParameter.ParameterType != typeof(ObjectInstance))
                            throw new NotImplementedException("[JSDoNotConvert] is only supported for arguments of type ObjectInstance.");
                        
                        var endOfThrowLabel = il.DefineLabel();
                        if (argumentTypes[i].IsClass == true)
                        {
                            il.Emit(OpCodes.Isinst, typeof(ObjectInstance));
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Brtrue_S, endOfThrowLabel);
                        }
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldstr, "TypeError");
                        il.Emit(OpCodes.Ldstr, string.Format("The {1} parameter of {0}() must be an object", binderMethod.Name,
                            i == 0 ? "first" : i == 1 ? "second" : i == 2 ? "third" : string.Format("{0}th", i + 1)));
                        il.Emit(OpCodes.Newobj, ReflectionHelpers.JavaScriptException_Constructor_Error);
                        il.Emit(OpCodes.Throw);
                        il.MarkLabel(endOfThrowLabel);
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
                il.Emit(OpCodes.Ldc_I4, Math.Max(argumentTypes.Length - initialEmitCount, 0));
                il.Emit(OpCodes.Newarr, elementType);

                for (int i = initialEmitCount; i < argumentTypes.Length; i++)
                {
                    // Emit the array and index.
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldc_I4_S, (byte)(i - initialEmitCount));

                    // Extract the input parameter and do type conversion as normal.
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Ldc_I4_S, (byte)i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    if (elementType != typeof(object))
                    {
                        if (argumentTypes[i].IsClass == false)
                            il.Emit(OpCodes.Unbox_Any, argumentTypes[i]);
                        EmitConversion(il, argumentTypes[i], elementType);
                    }

                    // Store each parameter in the array.
                    il.Emit(OpCodes.Stelem, elementType);
                }
            }

            // Emit the call.
            if (targetMethod.IsStatic == true)
                il.Emit(OpCodes.Call, targetMethod);
            else
                il.Emit(OpCodes.Callvirt, targetMethod);

            // Convert the return value.
            if (targetReturnParameter.ParameterType == typeof(void))
                EmitUndefined(il, typeof(object));
            else
            {
                if (targetReturnParameter.ParameterType == typeof(uint))
                {
                    // Convert a uint return value to a double
                    il.Emit(OpCodes.Conv_R_Un);
                    il.Emit(OpCodes.Conv_R8);
                    EmitConversion(il, typeof(double), typeof(object));
                }
                else
                    EmitConversion(il, targetReturnParameter.ParameterType, typeof(object));
            }

            // End the IL.
            il.Emit(OpCodes.Ret);

            
        }

        ///// <summary>
        ///// Creates a delegate with the given type that does parameter conversion as necessary
        ///// and then calls the given delegate.
        ///// </summary>
        ///// <param name="resultingDelegateType"> The type of the resulting delegate. </param>
        ///// <param name="targetDelegate"> The delegate to call. </param>
        ///// <returns> A delegate with the given type that does parameter conversion as necessary
        ///// and then calls the given delegate. </returns>
        //public static Delegate Create(Type resultingDelegateType, Delegate targetDelegate)
        //{
        //    // Make sure a binder is actually required.
        //    if (resultingDelegateType == targetDelegate.GetType())
        //        return targetDelegate;
        //    return Create(resultingDelegateType, targetDelegate.Method);
        //}

        ///// <summary>
        ///// Creates a delegate with the given type that does parameter conversion as necessary
        ///// and then calls the given method.
        ///// </summary>
        ///// <param name="resultingDelegateType"> The type of the resulting delegate. </param>
        ///// <param name="targetMethod"> The method to call. </param>
        ///// <returns> A delegate with the given type that does parameter conversion as necessary
        ///// and then calls the given method. </returns>
        //public static Delegate Create(Type resultingDelegateType, MethodInfo targetMethod)
        //{
        //    // Delegate types have an Invoke method containing the relevant parameters.
        //    MethodInfo adapterInvokeMethod = resultingDelegateType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
        //    if (adapterInvokeMethod == null)
        //        throw new ArgumentException("The type parameter T must be delegate type.", "T");

        //    // Get the return type and parameter types.
        //    ParameterInfo adapterReturnParameter = adapterInvokeMethod.ReturnParameter;
        //    ParameterInfo[] adapterParameters = adapterInvokeMethod.GetParameters();

        //    // Construct an array containing the adapter parameter types.
        //    // The first value is the delegate to call.
        //    Type[] adapterParameterTypes = new Type[adapterParameters.Length];
        //    for (int i = 0; i < adapterParameters.Length; i++)
        //        adapterParameterTypes[i] = adapterParameters[i].ParameterType;

        //    // Create a new dynamic method.
        //    DynamicMethod dm = new DynamicMethod(
        //        "Binder",                                   // Name of the generated method.
        //        adapterReturnParameter.ParameterType,       // Return type of the generated method.
        //        adapterParameterTypes,                      // Parameter types of the generated method.
        //        typeof(FunctionBinder),                     // Owner type.
        //        true);                                      // Skip visibility checks.

        //    // Here is what we are going to generate.
        //    //private static bool SampleAdapter(int a, object b, NumberInstance c)
        //    //{
        //    //    // Target function signature: int (bool, int, string, object).
        //    //    bool param1;
        //    //    int param2;
        //    //    string param3;
        //    //    object param4;
        //    //    param1 = a != 0;
        //    //    param2 = TypeConverter.ToInt32(b);
        //    //    param3 = TypeConverter.ToString(c);
        //    //    param4 = Undefined.Value;
        //    //    return targetMethod(param1, param2, param3, param4) != 0;
        //    //}

        //    ILGenerator il = dm.GetILGenerator();

        //    // Get information about the target method.
        //    ParameterInfo[] targetParameters = targetMethod.GetParameters();
        //    ParameterInfo targetReturnParameter = targetMethod.ReturnParameter;

        //    // Emit the parameters to the target function.
        //    for (int i = 0; i < targetParameters.Length; i++)
        //    {
        //        if (adapterParameters.Length > i)
        //        {
        //            il.Emit(OpCodes.Ldarg_S, i);
        //            EmitConversion(il, adapterParameters[i].ParameterType, targetParameters[i].ParameterType);
        //        }
        //        else
        //            EmitUndefined(il, targetParameters[i].ParameterType);
        //    }

        //    // Emit the call.
        //    il.Emit(OpCodes.Call, targetMethod);

        //    // Convert the return value.
        //    if (adapterReturnParameter.ParameterType != targetReturnParameter.ParameterType)
        //    {
        //        if (targetReturnParameter.ParameterType == typeof(void))
        //            EmitUndefined(il, adapterReturnParameter.ParameterType);
        //        else
        //            EmitConversion(il, targetReturnParameter.ParameterType, adapterReturnParameter.ParameterType);
        //    }

        //    // End the IL.
        //    il.Emit(OpCodes.Ret);

        //    // Convert the DynamicMethod to a delegate.
        //    return dm.CreateDelegate(resultingDelegateType);
        //}

        /// <summary>
        /// Pops the value on the stack, converts it from one type to another, then pushes the
        /// result onto the stack.  Undefined is converted to the given default value.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        /// <param name="targetParameter"> The type to convert to and the default value, if there is one. </param>
        private static void EmitConversion(ILGenerator il, Type fromType, ParameterInfo targetParameter)
        {
            if (fromType == typeof(Undefined))
            {
                il.Emit(OpCodes.Pop);
                EmitUndefined(il, targetParameter);
            }
            else
                EmitConversion(il, fromType, targetParameter.ParameterType);
        }

        /// <summary>
        /// Pops the value on the stack, converts it from one type to another, then pushes the
        /// result onto the stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        /// <param name="toType"> The type to convert to. </param>
        private static void EmitConversion(ILGenerator il, Type fromType, Type toType)
        {
            // If the source type equals the destination type, then there is nothing to do.
            if (fromType == toType)
                return;

            // Emit for each type of argument we support.
            if (toType == typeof(bool))
                EmitConversionToBool(il, fromType);
            else if (toType == typeof(int))
                EmitConversionToInt(il, fromType);
            else if (toType == typeof(double))
                EmitConversionToDouble(il, fromType);
            else if (toType == typeof(string))
                EmitConversionToString(il, fromType);
            else if (toType == typeof(object))
                EmitConversionToObject(il, fromType);
            else if (typeof(ObjectInstance).IsAssignableFrom(toType))
                EmitConversionToObjectInstance(il, fromType, toType);
            else
                throw new NotSupportedException(string.Format("Cannot convert to type '{0}'.  Supported types are bool, int, double, string, object and ObjectInstance.", toType.FullName));
        }

        /// <summary>
        /// Pops the value on the stack, converts it to a boolean, then pushes the boolean result
        /// onto the stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        private static void EmitConversionToBool(ILGenerator il, Type fromType)
        {
            if (fromType == typeof(Undefined) || fromType == typeof(Null))
            {
                // Easy case: convert from undefined or null.
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_I4_0);
            }
            else if (fromType == typeof(int))
            {
                // Easy case: convert from int.
                // output = input != 0
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Cgt_Un);
            }
            else if (fromType == typeof(double))
            {
                // Easy case: convert from double.
                // output = input != 0 && input == input;

                // input != 0
                var temp = il.DeclareLocal(fromType);   // }
                il.Emit(OpCodes.Stloc_S, temp);         // } Needed for return values but not for parameters (can use ldarg instead).
                il.Emit(OpCodes.Ldloc_S, temp);         // }
                il.Emit(OpCodes.Ldc_R8, 0.0);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);

                // input == input
                il.Emit(OpCodes.Ldloc_S, temp);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ceq);

                // &&
                il.Emit(OpCodes.Ceq);
            }
            else if (fromType == typeof(string) || fromType == typeof(ConcatenatedString))
            {
                // Easy case: convert from string or StringBuilder.
                // output = input != null && input.Length > 0

                if (fromType == typeof(ConcatenatedString))
                {
                    // Convert to a string first.
                    il.Emit(OpCodes.Callvirt, ReflectionHelpers.ConcatenatedString_ToString);
                }

                // input != null
                var temp = il.DeclareLocal(fromType);   // }
                il.Emit(OpCodes.Stloc_S, temp);         // } Needed for return values but not for parameters (can use ldarg instead).
                il.Emit(OpCodes.Ldloc_S, temp);         // }
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Cgt_Un);

                // Short circuit if input == null.
                var shortCircuitLabel = il.DefineLabel();
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Brfalse_S, shortCircuitLabel);

                // input.Length > 0
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldloc_S, temp);
                il.Emit(OpCodes.Callvirt, ReflectionHelpers.String_Length);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Cgt);

                // Define short circuit label.
                il.MarkLabel(shortCircuitLabel);
            }
            else
            {
                // Convert from any other type: call TypeConverter.ToBoolean()
                // output = TypeConverter.ToBoolean(input)
                if (fromType.IsValueType == true)
                    il.Emit(OpCodes.Box, fromType);
                il.Emit(OpCodes.Call, ReflectionHelpers.TypeConverter_ToBoolean);
            }
        }

        /// <summary>
        /// Pops the value on the stack, converts it to an integer, then pushes the integer result
        /// onto the stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        private static void EmitConversionToInt(ILGenerator il, Type fromType)
        {
            if (fromType == typeof(Undefined) || fromType == typeof(Null))
            {
                // Easy case: convert from undefined or null.
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_I4_0);
            }
            else if (fromType == typeof(double))
            {
                // Convert from double.

                // bool isPositiveInfinity = input > 2147483647.0
                var isPositiveInfinity = il.DeclareLocal(typeof(bool));
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_R8, 2147483647.0);
                il.Emit(OpCodes.Cgt);
                il.Emit(OpCodes.Stloc_S, isPositiveInfinity);

                // bool notNaN = input == input
                var notNaN = il.DeclareLocal(typeof(bool));
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Stloc_S, notNaN);

                // input = (int)input
                // Infinity -> -2147483648
                // -Infinity -> -2147483648
                // NaN -> -2147483648
                il.Emit(OpCodes.Conv_I4);

                // input = input & -((int)notNaN)
                il.Emit(OpCodes.Ldloc_S, notNaN);
                il.Emit(OpCodes.Neg);
                il.Emit(OpCodes.And);

                // input = input - (int)isPositiveInfinity
                il.Emit(OpCodes.Ldloc_S, isPositiveInfinity);
                il.Emit(OpCodes.Sub);
            }
            else if (fromType == typeof(bool))
            {
                // Easy case: convert from bool.
                // output = (int) input
                il.Emit(OpCodes.Conv_I4);
            }
            else
            {
                // Convert from any other type: call TypeConverter.ToInteger()
                // output = TypeConverter.ToInteger(input)
                if (fromType.IsValueType == true)
                    il.Emit(OpCodes.Box, fromType);
                il.Emit(OpCodes.Call, ReflectionHelpers.TypeConverter_ToInteger);
            }
        }

        /// <summary>
        /// Pops the value on the stack, converts it to a double, then pushes the double result
        /// onto the stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        private static void EmitConversionToDouble(ILGenerator il, Type fromType)
        {
            if (fromType == typeof(Undefined))
            {
                // Easy case: convert from undefined.
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_R8, double.NaN);
            }
            else if (fromType == typeof(Null))
            {
                // Easy case: convert from null.
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_R8, 0.0);
            }
            else if (fromType == typeof(bool))
            {
                // Easy case: convert from bool.
                // output = (double) input
                il.Emit(OpCodes.Conv_R8);
            }
            else if (fromType == typeof(int))
            {
                // Easy case: convert from int.
                // output = (double) input
                il.Emit(OpCodes.Conv_R8);
            }
            else
            {
                // Convert from any other type: call TypeConverter.ToNumber()
                // output = TypeConverter.ToNumber(input)
                if (fromType.IsValueType == true)
                    il.Emit(OpCodes.Box, fromType);
                il.Emit(OpCodes.Call, ReflectionHelpers.TypeConverter_ToNumber);
            }
        }

        /// <summary>
        /// Pops the value on the stack, converts it to a string, then pushes the result onto the
        /// stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        private static void EmitConversionToString(ILGenerator il, Type fromType)
        {
            //if (fromType.IsClass == true)
            //{
            //    // if (input == null)
            //    //     return "null";
            //    // else
            //    //     return input.ToString();

            //    // if (input == null)
            //    il.Emit(OpCodes.Dup);
            //    var afterIf = il.DefineLabel();
            //    il.Emit(OpCodes.Brtrue_S, afterIf);

            //    // return "null"
            //    il.Emit(OpCodes.Pop);
            //    il.Emit(OpCodes.Ldstr, "null");
            //    var endOfBlock = il.DefineLabel();
            //    il.Emit(OpCodes.Br_S, endOfBlock);

            //    // return input.ToString()
            //    il.MarkLabel(afterIf);
            //    MethodInfo toStringMethod = typeof(object).GetMethod("ToString", new Type[0]);
            //    if (toStringMethod == null)
            //        throw new InvalidOperationException("Object.ToString does not exist.");
            //    il.Emit(OpCodes.Callvirt, toStringMethod);
            //    il.MarkLabel(endOfBlock);
            //}
            //else
            //{
            //    // return input.ToString()
            //    MethodInfo toStringMethod = fromType.GetMethod("ToString", new Type[0]);
            //    if (toStringMethod == null)
            //        throw new InvalidOperationException(string.Format("{0}.ToString does not exist.", fromType.FullName));
            //    il.Emit(OpCodes.Call, toStringMethod);
            //}

            // output = TypeConverter.ToString(input)
            if (fromType.IsValueType == true)
                il.Emit(OpCodes.Box, fromType);
            il.Emit(OpCodes.Call, ReflectionHelpers.TypeConverter_ToString);
        }

        /// <summary>
        /// Pops the value on the stack, converts it to an object, then pushes the result onto the
        /// stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        private static void EmitConversionToObject(ILGenerator il, Type fromType)
        {
            // output = (object)input
            if (fromType.IsValueType == true)
                il.Emit(OpCodes.Box, fromType);
        }

        /// <summary>
        /// Pops the value on the stack, converts it to an ObjectInstance, then pushes the result
        /// onto the stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="fromType"> The type to convert from. </param>
        private static void EmitConversionToObjectInstance(ILGenerator il, Type fromType, Type toType)
        {
            if (fromType == typeof(bool))
            {
                // Easy case: convert from bool.
                // output = Global.Boolean.Construct(input)
                var temp = il.DeclareLocal(fromType);
                il.Emit(OpCodes.Stloc_S, temp);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, ReflectionHelpers.ScriptEngine_Boolean);
                il.Emit(OpCodes.Ldloc_S, temp);
                il.Emit(OpCodes.Callvirt, ReflectionHelpers.Boolean_Construct);
            }
            else
            {
                // Convert from any other type: call TypeConverter.ToObject()
                // output = TypeConverter.ToObject(engine, input)
                var temp = il.DeclareLocal(fromType);
                il.Emit(OpCodes.Stloc_S, temp);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, temp);
                if (fromType.IsValueType == true)
                    il.Emit(OpCodes.Box, fromType);
                il.Emit(OpCodes.Call, ReflectionHelpers.TypeConverter_ToObject);
            }

            if (toType != typeof(ObjectInstance))
            {
                // Convert to null if the from type isn't compatible with the to type.
                // For example, if the target type is FunctionInstance and the from type is ArrayInstance, then pass null.
                il.Emit(OpCodes.Isinst, toType);
            }
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
                if (targetParameter.DefaultValue is int)
                    il.Emit(OpCodes.Ldc_I4, (int)targetParameter.DefaultValue);
                else if (targetParameter.DefaultValue is double)
                    il.Emit(OpCodes.Ldc_R8, (double)targetParameter.DefaultValue);
                else if (targetParameter.DefaultValue == null)
                    il.Emit(OpCodes.Ldnull);
                else if (targetParameter.DefaultValue is string)
                    il.Emit(OpCodes.Ldstr, (string)targetParameter.DefaultValue);
                else
                    throw new NotImplementedException(string.Format("Unsupported default value type '{1}' for parameter '{0}'.",
                        targetParameter.Name, targetParameter.DefaultValue.GetType()));
            }
            else
            {
                // Convert Undefined to the target type and emit.
                EmitUndefined(il, targetParameter.ParameterType);
            }
        }

        /// <summary>
        /// Pushes the result of converting <c>undefined</c> to the given type onto the stack.
        /// </summary>
        /// <param name="il"> The IL generator. </param>
        /// <param name="toType"> The type to convert to. </param>
        private static void EmitUndefined(ILGenerator il, Type toType)
        {
            // Emit for each type of argument we support.
            if (toType == typeof(bool) || toType == typeof(int))
                il.Emit(OpCodes.Ldc_I4_0);
            else if (toType == typeof(double))
                il.Emit(OpCodes.Ldc_R8, double.NaN);
            else if (toType == typeof(string))
                il.Emit(OpCodes.Ldstr, "undefined");
            else if (toType == typeof(object))
                il.Emit(OpCodes.Ldsfld, ReflectionHelpers.Undefined_Value);
            else if (typeof(ObjectInstance).IsAssignableFrom(toType))
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, "TypeError");
                il.Emit(OpCodes.Ldstr, "Undefined cannot be converted to an object");
                il.Emit(OpCodes.Newobj, ReflectionHelpers.JavaScriptException_Constructor_Error);
                il.Emit(OpCodes.Throw);
            }
            else
                throw new InvalidOperationException(string.Format("Cannot convert undefined to {0}.", toType.FullName));
        }

    }
}
