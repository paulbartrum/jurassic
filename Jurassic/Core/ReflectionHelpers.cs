using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic.Compiler;
using Jurassic.Library;

namespace Jurassic
{

    /// <summary>
    /// Contains static methods to ease reflection operations.
    /// Not used at runtime (used as a source by TricksGen).
    /// </summary>
    internal static class ReflectionHelpers
    {
        internal static MethodInfo TypeConverter_ToString;
        internal static MethodInfo TypeConverter_ToNumber;
        internal static MethodInfo TypeConverter_ToBoolean;
        internal static MethodInfo TypeConverter_ToObject;
        internal static MethodInfo TypeConverter_ToInteger;
        internal static MethodInfo TypeConverter_ToInt32;
        internal static MethodInfo TypeConverter_ToUint32;

        internal static MethodInfo TypeComparer_Equals;
        internal static MethodInfo TypeComparer_StrictEquals;
        internal static MethodInfo TypeComparer_LessThan;
        internal static MethodInfo TypeComparer_LessThanOrEqual;

        internal static MethodInfo TypeUtilities_TypeOf;
        internal static MethodInfo TypeUtilities_EnumeratePropertyNames;
        internal static MethodInfo TypeUtilities_Add;
        internal static MethodInfo TypeUtilities_VerifyThisObject;

        internal static MethodInfo FunctionInstance_HasInstance;
        internal static MethodInfo FunctionInstance_ConstructLateBound;
        internal static MethodInfo FunctionInstance_CallLateBound;
        internal static MethodInfo FunctionInstance_InstancePrototype;

        internal static MethodInfo Global_Instance;
        internal static MethodInfo Global_Boolean;
        internal static MethodInfo Global_Function;
        internal static MethodInfo Global_RegExp;
        internal static MethodInfo Global_Array;
        internal static MethodInfo Global_Object;
        internal static MethodInfo Global_Eval;

        internal static MethodInfo String_Concat;
        internal static MethodInfo String_Length;
        internal static MethodInfo String_CompareOrdinal;
        internal static MethodInfo String_Format;

        internal static ConstructorInfo ConcatenatedString_Constructor;
        internal static MethodInfo ConcatenatedString_Append_String;
        internal static MethodInfo ConcatenatedString_Append_ConcatenatedString;
        internal static MethodInfo ConcatenatedString_Length;
        internal static MethodInfo ConcatenatedString_ToString;

        internal static MethodInfo IEnumerable_GetEnumerator;
        internal static MethodInfo IEnumerator_MoveNext;
        internal static MethodInfo IEnumerator_Current;

        internal static MethodInfo Debugger_Break;
        internal static MethodInfo JavaScriptException_ErrorObject;
        internal static MethodInfo Boolean_Construct;
        internal static MethodInfo Object_Construct;
        
        internal static MethodInfo RegExp_Construct1;
        internal static MethodInfo RegExp_Construct2;
        internal static MethodInfo Array_New;
        internal static MethodInfo Delegate_CreateDelegate;
        internal static MethodInfo Type_GetTypeFromHandle;
        internal static MethodInfo MethodBase_GetMethodFromHandle;

        internal static MethodInfo ObjectInstance_Delete;
        internal static MethodInfo ObjectInstance_DefineProperty;
        internal static MethodInfo ObjectInstance_HasProperty;
        internal static MethodInfo ObjectInstance_GetPropertyValue_String;
        internal static MethodInfo ObjectInstance_GetPropertyValue_Int;
        internal static MethodInfo ObjectInstance_SetPropertyValue_String;
        internal static MethodInfo ObjectInstance_SetPropertyValue_Int;
        internal static MethodInfo ObjectInstance_SetPropertyValueIfExists;
        internal static MethodInfo ObjectInstance_InlinePropertyValues;
        internal static MethodInfo ObjectInstance_InlineCacheKey;
        internal static MethodInfo ObjectInstance_InlineGetPropertyValue;
        internal static MethodInfo ObjectInstance_InlineSetPropertyValue;
        internal static MethodInfo ObjectInstance_InlineSetPropertyValueIfExists;

        internal static MethodInfo Scope_ParentScope;
        internal static MethodInfo ObjectScope_CreateRuntimeScope;
        internal static MethodInfo ObjectScope_ScopeObject;
        internal static MethodInfo DeclarativeScope_CreateRuntimeScope;
        internal static MethodInfo DeclarativeScope_Values;

        internal static ConstructorInfo JavaScriptException_Constructor2;
        internal static ConstructorInfo JavaScriptException_Constructor3;
        internal static ConstructorInfo UserDefinedFunction_Constructor;
        internal static ConstructorInfo FunctionDelegate_Constructor;
        internal static ConstructorInfo Arguments_Constructor;
        internal static ConstructorInfo PropertyDescriptor_Constructor;

        internal static FieldInfo Undefined_Value;
        internal static FieldInfo Null_Value;

        /// <summary>
        /// Initializes static members of this class.
        /// </summary>
        static ReflectionHelpers()
        {
            // Retrieve the various MethodInfos used for type conversion.
            TypeConverter_ToString = GetStaticMethod(typeof(TypeConverter), "ToString", typeof(object));
            TypeConverter_ToNumber = GetStaticMethod(typeof(TypeConverter), "ToNumber", typeof(object));
            TypeConverter_ToBoolean = GetStaticMethod(typeof(TypeConverter), "ToBoolean", typeof(object));
            TypeConverter_ToObject = GetStaticMethod(typeof(TypeConverter), "ToObject", typeof(object));
            TypeConverter_ToInteger = GetStaticMethod(typeof(TypeConverter), "ToInteger", typeof(object));
            TypeConverter_ToInt32 = GetStaticMethod(typeof(TypeConverter), "ToInt32", typeof(object));
            TypeConverter_ToUint32 = GetStaticMethod(typeof(TypeConverter), "ToUint32", typeof(object));

            TypeComparer_Equals = GetStaticMethod(typeof(TypeComparer), "Equals", typeof(object), typeof(object));
            TypeComparer_StrictEquals = GetStaticMethod(typeof(TypeComparer), "StrictEquals", typeof(object), typeof(object));
            TypeComparer_LessThan = GetStaticMethod(typeof(TypeComparer), "LessThan", typeof(object), typeof(object), typeof(bool));
            TypeComparer_LessThanOrEqual = GetStaticMethod(typeof(TypeComparer), "LessThanOrEqual", typeof(object), typeof(object), typeof(bool));

            TypeUtilities_TypeOf = GetStaticMethod(typeof(TypeUtilities), "TypeOf", typeof(object));
            TypeUtilities_EnumeratePropertyNames = GetStaticMethod(typeof(TypeUtilities), "EnumeratePropertyNames", typeof(object));
            TypeUtilities_Add = GetStaticMethod(typeof(TypeUtilities), "Add", typeof(object), typeof(object));
            TypeUtilities_VerifyThisObject = GetStaticMethod(typeof(TypeUtilities), "VerifyThisObject", typeof(object), typeof(string));

            ObjectInstance_Delete = GetInstanceMethod(typeof(ObjectInstance), "Delete", typeof(string), typeof(bool));
            ObjectInstance_DefineProperty = GetInstanceMethod(typeof(ObjectInstance), "DefineProperty", typeof(string), typeof(PropertyDescriptor), typeof(bool));
            ObjectInstance_HasProperty = GetInstanceMethod(typeof(ObjectInstance), "HasProperty", typeof(string));
            ObjectInstance_GetPropertyValue_String = GetInstanceMethod(typeof(ObjectInstance), "GetPropertyValue", typeof(string));
            ObjectInstance_GetPropertyValue_Int = GetInstanceMethod(typeof(ObjectInstance), "GetPropertyValue", typeof(uint));
            ObjectInstance_SetPropertyValue_String = GetInstanceMethod(typeof(ObjectInstance), "SetPropertyValue", typeof(string), typeof(object), typeof(bool));
            ObjectInstance_SetPropertyValue_Int = GetInstanceMethod(typeof(ObjectInstance), "SetPropertyValue", typeof(uint), typeof(object), typeof(bool));
            ObjectInstance_SetPropertyValueIfExists = GetInstanceMethod(typeof(ObjectInstance), "SetPropertyValueIfExists", typeof(string), typeof(object), typeof(bool));
            ObjectInstance_InlinePropertyValues = GetInstanceMethod(typeof(ObjectInstance), "get_InlinePropertyValues");
            ObjectInstance_InlineCacheKey = GetInstanceMethod(typeof(ObjectInstance), "get_InlineCacheKey");
            ObjectInstance_InlineGetPropertyValue = GetInstanceMethod(typeof(ObjectInstance), "InlineGetPropertyValue",
                new Type[] { typeof(string), typeof(int).MakeByRefType(), typeof(object).MakeByRefType() });
            ObjectInstance_InlineSetPropertyValue = GetInstanceMethod(typeof(ObjectInstance), "InlineSetPropertyValue",
                new Type[] { typeof(string), typeof(object), typeof(int).MakeByRefType(), typeof(object).MakeByRefType() });
            ObjectInstance_InlineSetPropertyValueIfExists = GetInstanceMethod(typeof(ObjectInstance), "InlineSetPropertyValueIfExists",
                new Type[] { typeof(string), typeof(object), typeof(int).MakeByRefType(), typeof(object).MakeByRefType() });

            Scope_ParentScope = GetInstanceMethod(typeof(Scope), "get_ParentScope");
            ObjectScope_CreateRuntimeScope = GetStaticMethod(typeof(ObjectScope), "CreateRuntimeScope", typeof(Scope), typeof(ObjectInstance));
            ObjectScope_ScopeObject = GetInstanceMethod(typeof(ObjectScope), "get_ScopeObject");
            DeclarativeScope_CreateRuntimeScope = GetStaticMethod(typeof(DeclarativeScope), "CreateRuntimeScope", typeof(Scope), typeof(string[]));
            DeclarativeScope_Values = GetInstanceMethod(typeof(DeclarativeScope), "get_Values");

            FunctionInstance_HasInstance = GetInstanceMethod(typeof(FunctionInstance), "HasInstance", typeof(object));
            FunctionInstance_ConstructLateBound = GetInstanceMethod(typeof(FunctionInstance), "ConstructLateBound", typeof(object[]));
            FunctionInstance_CallLateBound = GetInstanceMethod(typeof(FunctionInstance), "CallLateBound", typeof(object), typeof(object[]));
            FunctionInstance_InstancePrototype = GetInstanceMethod(typeof(FunctionInstance), "get_InstancePrototype");

            Global_Instance = GetStaticMethod(typeof(GlobalObject), "get_Instance");
            Global_Boolean = GetStaticMethod(typeof(GlobalObject), "get_Boolean");
            Global_Function = GetStaticMethod(typeof(GlobalObject), "get_Function");
            Global_RegExp = GetStaticMethod(typeof(GlobalObject), "get_RegExp");
            Global_Array = GetStaticMethod(typeof(GlobalObject), "get_Array");
            Global_Object = GetStaticMethod(typeof(GlobalObject), "get_Object");
            Global_Eval = GetStaticMethod(typeof(GlobalObject), "Eval", typeof(Scope), typeof(object), typeof(string));

            String_Concat = GetStaticMethod(typeof(string), "Concat", typeof(string), typeof(string));
            String_Length = GetInstanceMethod(typeof(string), "get_Length");
            String_CompareOrdinal = GetStaticMethod(typeof(string), "CompareOrdinal", typeof(string), typeof(string));
            String_Format = GetStaticMethod(typeof(string), "Format", typeof(string), typeof(object[]));

            ConcatenatedString_Constructor = GetConstructor(typeof(ConcatenatedString), typeof(string));
            ConcatenatedString_Append_String = GetInstanceMethod(typeof(ConcatenatedString), "Append", typeof(string));
            ConcatenatedString_Append_ConcatenatedString = GetInstanceMethod(typeof(ConcatenatedString), "Append", typeof(ConcatenatedString));
            ConcatenatedString_Length = GetInstanceMethod(typeof(ConcatenatedString), "get_Length");
            ConcatenatedString_ToString = GetInstanceMethod(typeof(ConcatenatedString), "ToString");

            JavaScriptException_Constructor2 = GetConstructor(typeof(JavaScriptException), typeof(string), typeof(string));
            JavaScriptException_Constructor3 = GetConstructor(typeof(JavaScriptException), typeof(object), typeof(int), typeof(string));
            IEnumerable_GetEnumerator = GetInstanceMethod(typeof(IEnumerable<string>), "GetEnumerator");
            IEnumerator_MoveNext = GetInstanceMethod(typeof(System.Collections.IEnumerator), "MoveNext");
            IEnumerator_Current = GetInstanceMethod(typeof(IEnumerator<string>), "get_Current");
            Debugger_Break = GetStaticMethod(typeof(System.Diagnostics.Debugger), "Break");
            JavaScriptException_ErrorObject = GetInstanceMethod(typeof(JavaScriptException), "get_ErrorObject");
            Boolean_Construct = GetInstanceMethod(typeof(BooleanConstructor), "Construct", typeof(bool));
            
            RegExp_Construct1 = GetInstanceMethod(typeof(Jurassic.Library.RegExpConstructor), "Construct", typeof(string), typeof(string));
            RegExp_Construct2 = GetInstanceMethod(typeof(Jurassic.Library.RegExpConstructor), "Construct", typeof(RegExpInstance), typeof(string));
            Array_New = GetInstanceMethod(typeof(ArrayConstructor), "New", typeof(object[]));
            Object_Construct = GetInstanceMethod(typeof(ObjectConstructor), "Construct");
            UserDefinedFunction_Constructor = GetConstructor(typeof(UserDefinedFunction), typeof(ObjectInstance),
                typeof(string), typeof(IList<string>), typeof(Scope), typeof(Library.FunctionDelegate));
            Delegate_CreateDelegate = GetStaticMethod(typeof(Delegate), "CreateDelegate", typeof(Type), typeof(MethodInfo));
            Type_GetTypeFromHandle = GetStaticMethod(typeof(Type), "GetTypeFromHandle", typeof(RuntimeTypeHandle));
            MethodBase_GetMethodFromHandle = GetStaticMethod(typeof(MethodBase), "GetMethodFromHandle", typeof(RuntimeMethodHandle));
            FunctionDelegate_Constructor = GetConstructor(typeof(Library.FunctionDelegate), typeof(object), typeof(IntPtr));
            Arguments_Constructor = GetConstructor(typeof(ArgumentsInstance), typeof(ObjectInstance), typeof(UserDefinedFunction), typeof(DeclarativeScope), typeof(object[]));
            PropertyDescriptor_Constructor = GetConstructor(typeof(PropertyDescriptor), typeof(FunctionInstance), typeof(FunctionInstance), typeof(Library.PropertyAttributes));

            Undefined_Value = GetField(typeof(Undefined), "Value");
            Null_Value = GetField(typeof(Null), "Value");
        }

        /// <summary>
        /// Gets an enumerable list of all the MemberInfos that are statically known to be used by this DLL.
        /// </summary>
        /// <returns> An enumerable list of all the MemberInfos that are used by this DLL. </returns>
        internal static IEnumerable<Tuple<string, MemberInfo>> GetMembers()
        {
            foreach (FieldInfo field in typeof(ReflectionHelpers).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (field.FieldType != typeof(MethodInfo) && field.FieldType != typeof(ConstructorInfo) && field.FieldType != typeof(FieldInfo))
                    continue;
                yield return Tuple.Create(field.Name, (MemberInfo)field.GetValue(null));
            }
        }

        /// <summary>
        /// Gets the FieldInfo for a field.  Throws an exception if the search fails.
        /// </summary>
        /// <param name="type"> The type to search. </param>
        /// <param name="name"> The name of the field. </param>
        /// <returns> The FieldInfo for a field. </returns>
        public static FieldInfo GetField(Type type, string name)
        {
            FieldInfo result = type.GetField(name);
            if (result == null)
                throw new InvalidOperationException(string.Format("The field '{1}' does not exist on type '{0}'.", type, name));
            return result;
        }

        /// <summary>
        /// Gets the ConstructorInfo for a constructor.  Throws an exception if the search fails.
        /// </summary>
        /// <param name="type"> The type to search. </param>
        /// <param name="parameterTypes"> The types of the parameters accepted by the constructor. </param>
        /// <returns> The ConstructorInfo for the constructor. </returns>
        public static ConstructorInfo GetConstructor(Type type, params Type[] parameterTypes)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            ConstructorInfo result = type.GetConstructor(flags, null, parameterTypes, null);
            if (result == null)
                throw new InvalidOperationException(string.Format("The constructor {0}({1}) does not exist.", type.FullName, string.Join<Type>(", ", parameterTypes)));
            return result;
        }

        /// <summary>
        /// Gets the MethodInfo for an instance method.  Throws an exception if the search fails.
        /// </summary>
        /// <param name="type"> The type to search. </param>
        /// <param name="name"> The name of the method to search for. </param>
        /// <param name="parameterTypes"> The types of the parameters accepted by the method. </param>
        /// <returns> The MethodInfo for the method. </returns>
        public static MethodInfo GetInstanceMethod(Type type, string name, params Type[] parameterTypes)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding;
            MethodInfo result = type.GetMethod(name, flags, null, parameterTypes, null);
            if (result == null)
                throw new InvalidOperationException(string.Format("The instance method {0}.{1}({2}) does not exist.", type.FullName, name, string.Join<Type>(", ", parameterTypes)));
            return result;
        }

        /// <summary>
        /// Gets the MethodInfo for a static method.  Throws an exception if the search fails.
        /// </summary>
        /// <param name="type"> The type to search. </param>
        /// <param name="name"> The name of the method to search for. </param>
        /// <param name="parameterTypes"> The types of the parameters accepted by the method. </param>
        /// <returns> The MethodInfo for the method. </returns>
        public static MethodInfo GetStaticMethod(Type type, string name, params Type[] parameterTypes)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding;
            MethodInfo result = type.GetMethod(name, flags, null, parameterTypes, null);
            if (result == null)
                throw new InvalidOperationException(string.Format("The static method {0}.{1}({2}) does not exist.", type.FullName, name, string.Join<Type>(", ", parameterTypes)));
            return result;
        }

        /// <summary>
        /// Gets the MethodInfo for a generic instance method.  Throws an exception if the search fails.
        /// </summary>
        /// <param name="type"> The type to search. </param>
        /// <param name="name"> The name of the method to search for. </param>
        /// <returns> The MethodInfo for the method. </returns>
        private static MethodInfo GetGenericInstanceMethod(Type type, string name)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            MethodInfo result = type.GetMethod(name, flags);
            if (result == null)
                throw new InvalidOperationException(string.Format("The instance method {0}.{1}(...) does not exist.", type.FullName, name));
            return result;
        }

    }

}