﻿using System;
using System.Collections.Generic;
using System.Reflection;
using ErrorType = Jurassic.Library.ErrorType;

namespace Jurassic.Compiler
{

    /// <summary>
    /// This class is intended only for internal use.
    /// </summary>
    internal static class BinderUtilities
    {
        /// <summary>
        /// Given a set of methods and a set of arguments, determines whether one of the methods
        /// can be unambiguously selected.  Throws an exception if this is not the case.
        /// </summary>
        /// <param name="methodHandles"> An array of handles to the candidate methods. </param>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="thisValue"> The value of the "this" keyword. </param>
        /// <param name="arguments"> An array of parameter values. </param>
        /// <returns> The index of the selected method. </returns>
        public static int ResolveOverloads(RuntimeMethodHandle[] methodHandles, ScriptEngine engine, object thisValue, object[] arguments)
        {
            // Get methods from the handles.
            var methods = new BinderMethod[methodHandles.Length];
            for (int i = 0; i < methodHandles.Length; i++)
                methods[i] = new BinderMethod(MethodBase.GetMethodFromHandle(methodHandles[i]));

            // Keep a score for each method.  Add one point if a type conversion is required, or
            // a million points if a type conversion cannot be performed.
            int[] demeritPoints = new int[methods.Length];
            const int disqualification = 65536;
            for (int i = 0; i < methods.Length; i++)
            {
                IEnumerable<BinderArgument> binderArguments = methods[i].GetArguments(arguments.Length);
                foreach (var argument in binderArguments)
                {
                    // Get the input parameter.
                    object input;
                    switch (argument.Source)
                    {
                        case BinderArgumentSource.ThisValue:
                            input = thisValue;
                            break;
                        case BinderArgumentSource.InputParameter:
                            input = arguments[argument.InputParameterIndex];
                            break;
                        default:
                            continue;
                    }

                    // Unwrap the input parameter.
                    if (input is Jurassic.Library.ClrInstanceWrapper)
                        input = ((Jurassic.Library.ClrInstanceWrapper)input).WrappedInstance;

                    // Get the type of the output parameter.
                    Type outputType = argument.Type;
                    TypeCode typeCode = Type.GetTypeCode(outputType);


                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            if ((input is bool) == false)
                                demeritPoints[i] += disqualification;
                            break;

                        case TypeCode.SByte:
                        case TypeCode.Byte:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                            Dictionary<TypeCode, int> offsetDict = new Dictionary<TypeCode, int>() { { TypeCode.SByte, 10 },
                                { TypeCode.Byte, 9 },
                                { TypeCode.UInt16, 8 },
                                { TypeCode.UInt32, 7 },
                                { TypeCode.UInt64, 6 },
                                { TypeCode.Int16, 5 },
                                { TypeCode.Int32, 4 },
                                { TypeCode.Int64, 3 },
                                { TypeCode.Single, 2 },
                                { TypeCode.Decimal, 1 },
                                { TypeCode.Double, 0 }
                            };

                            // To fix ambiguous methods error when there are method with numeric parameters
                            // double has maximal priority
                            if (TypeUtilities.IsNumeric(input) == true)
                                demeritPoints[i] += offsetDict[typeCode];
                            else
                                demeritPoints[i] += disqualification;
                            break;

                        case TypeCode.Char:
                            if (TypeUtilities.IsString(input) == true)
                                demeritPoints[i]++;
                            else
                                demeritPoints[i] += disqualification;
                            break;

                        case TypeCode.String:
                            if (TypeUtilities.IsString(input) == false && input != Null.Value)
                                demeritPoints[i] += disqualification;
                            break;

                        case TypeCode.DateTime:
                        case TypeCode.Object:
                            if (input == null || input == Undefined.Value)
                            {
                                demeritPoints[i] += disqualification;
                            }
                            else if (input == Null.Value)
                            {
                                if (outputType.IsValueType == true)
                                    demeritPoints[i] += disqualification;
                            }
                            else if (outputType.IsAssignableFrom(input.GetType()) == false)
                            {
                                demeritPoints[i] += disqualification;
                            }
                            else if (outputType != input.GetType())
                            {
                                // To fix ambiguous when the parameter is of type object and there is another method 
                                // with parameter which inherits object.
                                demeritPoints[i]++;
                            }
                            break;


                        case TypeCode.Empty:
                        case TypeCode.DBNull:
                            throw new NotSupportedException(string.Format("{0} is not a supported parameter type.", outputType));
                    }
                    // To fix ambiguous methods error when there are method with smilar parameters, for example int32[] and int32
                    if (argument.IsParamArrayArgument)
                    {
                        demeritPoints[i] += 100;
                    }
                }
                
            }

            // Find the method(s) with the fewest number of demerit points.
            int lowestScore;
            var lowestIndices = _LowestIndices(methods, demeritPoints, out lowestScore);

            // Try to get the method from the most close base type 
            if (lowestIndices.Count > 1)
            {
                for (int i = 0; i < demeritPoints.Length; i++)
                {
                    demeritPoints[i] = disqualification;
                }
                for (int i = 0; i < lowestIndices.Count; i++)
                {
                    int index = lowestIndices[i];
                    demeritPoints[index] = _CalcMethodDistance(_GetThisType(thisValue), methods[index].DeclaringType);
                }
                lowestIndices = _LowestIndices(methods, demeritPoints, out lowestScore);
            }

            // Try to get the method with most close arguments count
            if (lowestIndices.Count > 1)
            {
                for (int i = 0; i < demeritPoints.Length; i++)
                {
                    demeritPoints[i] = disqualification;
                }
                for (int i = 0; i < lowestIndices.Count; i++)
                {
                    int index = lowestIndices[i];
                    demeritPoints[index] = _CalcArgumentsPoint(methods[index], arguments.Length);
                }
                lowestIndices = _LowestIndices(methods, demeritPoints, out lowestScore);
            }

            // Throw an error if the match is ambiguous.
            if (lowestIndices.Count > 1)
            {
                var ambiguousMethods = new List<BinderMethod>(lowestIndices.Count);
                foreach (var index in lowestIndices)
                    ambiguousMethods.Add(methods[index]);
                throw new JavaScriptException(ErrorType.TypeError, "The method call is ambiguous between the following methods: " + StringHelpers.Join(", ", ambiguousMethods));
            }

            // Throw an error is there is an invalid argument.
            if (lowestIndices.Count == 1 && lowestScore >= disqualification)
                throw new JavaScriptException(ErrorType.TypeError, string.Format("The best method overload {0} has some invalid arguments", methods[lowestIndices[0]]));

            return lowestIndices[0];
        }


        private static List<int> _LowestIndices(BinderMethod[] methods, int[] demeritPoints, out int lowestScore)
        {
            lowestScore = int.MaxValue;
            List<int> lowestIndices = new List<int>();
            for (int i = 0; i < methods.Length; i++)
            {
                if (demeritPoints[i] < lowestScore)
                {
                    lowestScore = demeritPoints[i];
                    lowestIndices.Clear();
                }
                if (demeritPoints[i] <= lowestScore)
                    lowestIndices.Add(i);
            }

            return lowestIndices;
        }


        private static Type _GetThisType(object thisValue)
        {
            object thisUnwrapped = thisValue;
            if (thisUnwrapped is Jurassic.Library.ClrInstanceWrapper)
            {
                thisUnwrapped = ((Jurassic.Library.ClrInstanceWrapper)thisUnwrapped).WrappedInstance;
            }
            else if (thisUnwrapped is Jurassic.Library.ClrInstanceTypeWrapper)
            {
                thisUnwrapped = ((Jurassic.Library.ClrInstanceTypeWrapper)thisUnwrapped).WrappedType;
            }
            else if (thisUnwrapped is Jurassic.Library.ClrStaticTypeWrapper)
            {
                thisUnwrapped = ((Jurassic.Library.ClrStaticTypeWrapper)thisUnwrapped).WrappedType;
            }
            return thisUnwrapped.GetType();
        }


        private static int _CalcMethodDistance(Type thisType, Type declaringType)
        {
            Type currentType = thisType;

            int result = 0;
            while (currentType != null && currentType != declaringType)
            {
                result++;
                currentType = currentType.BaseType;
            }

            return result;
        }


        private static int _CalcArgumentsPoint(BinderMethod method, int argumentsCount)
        {
            int points = Math.Max(method.GetParameters().Length - argumentsCount, 0);
            return points;
        }
    }

}
