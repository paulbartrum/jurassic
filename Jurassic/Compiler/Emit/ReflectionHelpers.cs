using System;
using System.Collections.Generic;
using System.Reflection;
using Jurassic.Library;
using PropertyAttributes = Jurassic.Library.PropertyAttributes;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Used by the code generator.
    /// Not intended for user code (the class needs to be public because when using Reflection
    /// Emit, all calls into Jurassic.dll are cross-assembly and thus must be public).
    /// </summary>
    public static class ReflectionHelpers
    {
        internal static MethodInfo TypeConverter_ToString;
        internal static MethodInfo TypeConverter_ToConcatenatedString;
        internal static MethodInfo TypeConverter_ToNumber;
        internal static MethodInfo TypeConverter_ToBoolean;
        internal static MethodInfo TypeConverter_ToObject;
        internal static MethodInfo TypeConverter_ToInteger;
        internal static MethodInfo TypeConverter_ToInt32;
        internal static MethodInfo TypeConverter_ToUint32;
        internal static MethodInfo TypeConverter_ToPrimitive;
        internal static MethodInfo TypeConverter_ToPropertyKey;

        internal static MethodInfo TypeComparer_Equals;
        internal static MethodInfo TypeComparer_StrictEquals;
        internal static MethodInfo TypeComparer_LessThan;
        internal static MethodInfo TypeComparer_LessThanOrEqual;
        internal static MethodInfo TypeComparer_GreaterThan;
        internal static MethodInfo TypeComparer_GreaterThanOrEqual;

        internal static MethodInfo TypeUtilities_TypeOf;
        internal static MethodInfo TypeUtilities_EnumeratePropertyNames;
        internal static MethodInfo TypeUtilities_Add;
        internal static MethodInfo TypeUtilities_IsPrimitiveOrObject;
        internal static MethodInfo TypeUtilities_VerifyThisObject;
        internal static MethodInfo TypeUtilities_ForOf;

        internal static MethodInfo FunctionInstance_HasInstance;
        internal static MethodInfo FunctionInstance_ConstructWithStackTrace;
        internal static MethodInfo FunctionInstance_CallWithStackTrace;
        internal static MethodInfo FunctionInstance_InstancePrototype;

        internal static MethodInfo ScriptEngine_Global;
        internal static MethodInfo ScriptEngine_Boolean;
        internal static MethodInfo ScriptEngine_Function;
        internal static MethodInfo ScriptEngine_RegExp;
        internal static MethodInfo ScriptEngine_Array;
        internal static MethodInfo ScriptEngine_Object;
        internal static MethodInfo ScriptEngine_CanCatchException;
        internal static MethodInfo Global_Eval;

        internal static ConstructorInfo String_Constructor_Char_Int;
        internal static MethodInfo String_Concat;
        internal static MethodInfo String_Concat_String_String;
        internal static MethodInfo String_Length;
        internal static MethodInfo String_CompareOrdinal;
        internal static MethodInfo String_Format;
        internal static MethodInfo String_GetChars;

        internal static ConstructorInfo ConcatenatedString_Constructor_String;
        internal static ConstructorInfo ConcatenatedString_Constructor_String_String;
        internal static MethodInfo ConcatenatedString_Length;
        internal static MethodInfo ConcatenatedString_Concatenate_Object;
        internal static MethodInfo ConcatenatedString_Concatenate_String;
        internal static MethodInfo ConcatenatedString_Concatenate_ConcatenatedString;
        internal static MethodInfo ConcatenatedString_Append_Object;
        internal static MethodInfo ConcatenatedString_Append_String;
        internal static MethodInfo ConcatenatedString_Append_ConcatenatedString;
        internal static MethodInfo ConcatenatedString_ToString;

        internal static MethodInfo IEnumerable_String_GetEnumerator;
        internal static MethodInfo IEnumerable_Object_GetEnumerator;
        internal static MethodInfo IEnumerator_MoveNext;
        internal static MethodInfo IEnumerator_String_Current;
        internal static MethodInfo IEnumerator_Object_Current;

        internal static MethodInfo JavaScriptException_ErrorObject;
        internal static MethodInfo Boolean_Construct;
        internal static MethodInfo Object_Construct;

        internal static MethodInfo RegExp_Construct;
        internal static MethodInfo Array_New;
        internal static MethodInfo Delegate_CreateDelegate;
        internal static MethodInfo Type_GetTypeFromHandle;
        internal static MethodInfo MethodBase_GetMethodFromHandle;
        internal static MethodInfo GeneratedMethod_Load;
        internal static MethodInfo ClrInstanceWrapper_GetWrappedInstance;
        internal static MethodInfo ClrInstanceWrapper_Create;
        internal static MethodInfo Decimal_ToDouble;
        internal static MethodInfo BinderUtilities_ResolveOverloads;
        internal static MethodInfo Convert_ToInt32_Double;
        internal static MethodInfo Math_Pow;

        internal static MethodInfo ObjectInstance_Delete;
        internal static MethodInfo ObjectInstance_DefineProperty;
        internal static MethodInfo ObjectInstance_HasProperty;
        internal static MethodInfo ObjectInstance_Indexer_Object;
        internal static MethodInfo ObjectInstance_Indexer_UInt;
        internal static MethodInfo ObjectInstance_GetPropertyValue_PropertyReference;
        internal static MethodInfo ObjectInstance_SetPropertyValue_Object;
        internal static MethodInfo ObjectInstance_SetPropertyValue_Int;
        internal static MethodInfo ObjectInstance_SetPropertyValue_PropertyReference;
        internal static MethodInfo ObjectInstance_SetPropertyValueIfExists;
        internal static MethodInfo ObjectInstance_InitializeMissingProperty;

        internal static ConstructorInfo JavaScriptException_Constructor_Error;
        internal static ConstructorInfo JavaScriptException_Constructor_Object;
        internal static ConstructorInfo PropertyDescriptor_Constructor2;
        internal static ConstructorInfo PropertyDescriptor_Constructor3;
        internal static ConstructorInfo Decimal_Constructor_Double;
        internal static ConstructorInfo PropertyName_Constructor;

        internal static FieldInfo Undefined_Value;
        internal static FieldInfo Null_Value;

        internal static ConstructorInfo LongJumpException_Constructor;
        internal static MethodInfo LongJumpException_RouteID;

        internal static MethodInfo ReflectionHelpers_CreateFunction;
        internal static MethodInfo ReflectionHelpers_SetObjectLiteralValue;
        internal static MethodInfo ReflectionHelpers_SetObjectLiteralGetter;
        internal static MethodInfo ReflectionHelpers_SetObjectLiteralSetter;
        internal static MethodInfo ReflectionHelpers_ConstructClass;
        internal static MethodInfo ReflectionHelpers_SetClassValue;
        internal static MethodInfo ReflectionHelpers_SetClassGetter;
        internal static MethodInfo ReflectionHelpers_SetClassSetter;
        internal static MethodInfo ReflectionHelpers_GetCachedTemplateStringsArray;
        internal static MethodInfo ReflectionHelpers_CreateTemplateStringsArray;

        internal static MethodInfo ExecutionContext_GetEngine;
        internal static MethodInfo ExecutionContext_GetThisValue;
        internal static MethodInfo ExecutionContext_GetSuperValue;
        internal static MethodInfo ExecutionContext_GetExecutingFunction;
        internal static MethodInfo ExecutionContext_GetNewTargetObject;
        internal static MethodInfo ExecutionContext_ConvertThisToObject;
        internal static MethodInfo ExecutionContext_CallSuperClass;
        internal static MethodInfo ExecutionContext_CreateArgumentsInstance;
        internal static MethodInfo ExecutionContext_ParentScope;
        internal static MethodInfo ExecutionContext_CreateRuntimeScope;

        internal static MethodInfo RuntimeScope_GetValue;
        internal static MethodInfo RuntimeScope_GetValueNoThrow;
        internal static MethodInfo RuntimeScope_SetValue;
        internal static MethodInfo RuntimeScope_SetValueStrict;
        internal static MethodInfo RuntimeScope_Delete;
        internal static MethodInfo RuntimeScope_With;
        internal static MethodInfo RuntimeScope_ImplicitThis;

        /// <summary>
        /// Initializes static members of this class.
        /// </summary>
        static ReflectionHelpers()
        {
            // Retrieve the various MethodInfos used for type conversion.
            TypeConverter_ToString = GetStaticMethod(typeof(TypeConverter), "ToString", typeof(object));
            TypeConverter_ToConcatenatedString = GetStaticMethod(typeof(TypeConverter), "ToConcatenatedString", typeof(object));
            TypeConverter_ToNumber = GetStaticMethod(typeof(TypeConverter), "ToNumber", typeof(object));
            TypeConverter_ToBoolean = GetStaticMethod(typeof(TypeConverter), "ToBoolean", typeof(object));
            TypeConverter_ToObject = GetStaticMethod(typeof(TypeConverter), "ToObject", typeof(ScriptEngine), typeof(object), typeof(int), typeof(string), typeof(string));
            TypeConverter_ToInteger = GetStaticMethod(typeof(TypeConverter), "ToInteger", typeof(object));
            TypeConverter_ToInt32 = GetStaticMethod(typeof(TypeConverter), "ToInt32", typeof(object));
            TypeConverter_ToUint32 = GetStaticMethod(typeof(TypeConverter), "ToUint32", typeof(object));
            TypeConverter_ToPrimitive = GetStaticMethod(typeof(TypeConverter), "ToPrimitive", typeof(object), typeof(PrimitiveTypeHint));
            TypeConverter_ToPropertyKey = GetStaticMethod(typeof(TypeConverter), "ToPropertyKey", typeof(object));

            TypeComparer_Equals = GetStaticMethod(typeof(TypeComparer), "Equals", typeof(object), typeof(object));
            TypeComparer_StrictEquals = GetStaticMethod(typeof(TypeComparer), "StrictEquals", typeof(object), typeof(object));
            TypeComparer_LessThan = GetStaticMethod(typeof(TypeComparer), "LessThan", typeof(object), typeof(object));
            TypeComparer_LessThanOrEqual = GetStaticMethod(typeof(TypeComparer), "LessThanOrEqual", typeof(object), typeof(object));
            TypeComparer_GreaterThan = GetStaticMethod(typeof(TypeComparer), "GreaterThan", typeof(object), typeof(object));
            TypeComparer_GreaterThanOrEqual = GetStaticMethod(typeof(TypeComparer), "GreaterThanOrEqual", typeof(object), typeof(object));

            TypeUtilities_TypeOf = GetStaticMethod(typeof(TypeUtilities), "TypeOf", typeof(object));
            TypeUtilities_EnumeratePropertyNames = GetStaticMethod(typeof(TypeUtilities), "EnumeratePropertyNames", typeof(ScriptEngine), typeof(object));
            TypeUtilities_Add = GetStaticMethod(typeof(TypeUtilities), "Add", typeof(object), typeof(object));
            TypeUtilities_IsPrimitiveOrObject = GetStaticMethod(typeof(TypeUtilities), "IsPrimitiveOrObject", typeof(object));
            TypeUtilities_VerifyThisObject = GetStaticMethod(typeof(TypeUtilities), "VerifyThisObject", typeof(ScriptEngine), typeof(object), typeof(string));
            TypeUtilities_ForOf = GetStaticMethod(typeof(TypeUtilities), "ForOf", typeof(ScriptEngine), typeof(object));

            ObjectInstance_Delete = GetInstanceMethod(typeof(ObjectInstance), "Delete", typeof(object), typeof(bool));
            ObjectInstance_DefineProperty = GetInstanceMethod(typeof(ObjectInstance), "DefineProperty", typeof(object), typeof(PropertyDescriptor), typeof(bool));
            ObjectInstance_HasProperty = GetInstanceMethod(typeof(ObjectInstance), "HasProperty", typeof(object));
            ObjectInstance_Indexer_Object = GetInstanceMethod(typeof(ObjectInstance), "get_Item", typeof(object));
            ObjectInstance_Indexer_UInt = GetInstanceMethod(typeof(ObjectInstance), "get_Item", typeof(uint));
            ObjectInstance_GetPropertyValue_PropertyReference = GetInstanceMethod(typeof(ObjectInstance), "GetPropertyValue", typeof(PropertyReference));
            ObjectInstance_SetPropertyValue_Object = GetInstanceMethod(typeof(ObjectInstance), "SetPropertyValue", typeof(object), typeof(object), typeof(bool));
            ObjectInstance_SetPropertyValue_Int = GetInstanceMethod(typeof(ObjectInstance), "SetPropertyValue", typeof(uint), typeof(object), typeof(bool));
            ObjectInstance_SetPropertyValue_PropertyReference = GetInstanceMethod(typeof(ObjectInstance), "SetPropertyValue", typeof(PropertyReference), typeof(object), typeof(bool));
            ObjectInstance_SetPropertyValueIfExists = GetInstanceMethod(typeof(ObjectInstance), "SetPropertyValueIfExists", typeof(object), typeof(object), typeof(bool));
            ObjectInstance_InitializeMissingProperty = GetInstanceMethod(typeof(ObjectInstance), "InitializeMissingProperty", typeof(object), typeof(PropertyAttributes));

            FunctionInstance_HasInstance = GetInstanceMethod(typeof(FunctionInstance), "HasInstance", typeof(object));
            FunctionInstance_ConstructWithStackTrace = GetInstanceMethod(typeof(FunctionInstance), "ConstructWithStackTrace", typeof(string), typeof(string), typeof(int), typeof(FunctionInstance), typeof(object[]));
            FunctionInstance_CallWithStackTrace = GetInstanceMethod(typeof(FunctionInstance), "CallWithStackTrace", typeof(string), typeof(string), typeof(int), typeof(object), typeof(object[]));
            FunctionInstance_InstancePrototype = GetInstanceMethod(typeof(FunctionInstance), "get_InstancePrototype");

            ScriptEngine_Global = GetInstanceMethod(typeof(ScriptEngine), "get_Global");
            ScriptEngine_Boolean = GetInstanceMethod(typeof(ScriptEngine), "get_Boolean");
            ScriptEngine_Function = GetInstanceMethod(typeof(ScriptEngine), "get_Function");
            ScriptEngine_RegExp = GetInstanceMethod(typeof(ScriptEngine), "get_RegExp");
            ScriptEngine_Array = GetInstanceMethod(typeof(ScriptEngine), "get_Array");
            ScriptEngine_Object = GetInstanceMethod(typeof(ScriptEngine), "get_Object");
            ScriptEngine_CanCatchException = GetInstanceMethod(typeof(ScriptEngine), "CanCatchException", typeof(object));
            Global_Eval = GetStaticMethod(typeof(GlobalObject), nameof(GlobalObject.Eval), typeof(ScriptEngine), typeof(object), typeof(RuntimeScope), typeof(object), typeof(bool));

            String_Constructor_Char_Int = GetConstructor(typeof(string), typeof(char), typeof(int));
            String_Concat = GetStaticMethod(typeof(string), "Concat", typeof(string[]));
            String_Concat_String_String = GetStaticMethod(typeof(string), "Concat", typeof(string), typeof(string));
            String_Length = GetInstanceMethod(typeof(string), "get_Length");
            String_CompareOrdinal = GetStaticMethod(typeof(string), "CompareOrdinal", typeof(string), typeof(string));
            String_Format = GetStaticMethod(typeof(string), "Format", typeof(string), typeof(object[]));
            String_GetChars = GetInstanceMethod(typeof(string), "get_Chars", typeof(int));

            ConcatenatedString_Constructor_String = GetConstructor(typeof(ConcatenatedString), typeof(string));
            ConcatenatedString_Constructor_String_String = GetConstructor(typeof(ConcatenatedString), typeof(string), typeof(string));
            ConcatenatedString_Length = GetInstanceMethod(typeof(ConcatenatedString), "get_Length");
            ConcatenatedString_Concatenate_Object = GetInstanceMethod(typeof(ConcatenatedString), "Concatenate", typeof(object));
            ConcatenatedString_Concatenate_String = GetInstanceMethod(typeof(ConcatenatedString), "Concatenate", typeof(string));
            ConcatenatedString_Concatenate_ConcatenatedString = GetInstanceMethod(typeof(ConcatenatedString), "Concatenate", typeof(ConcatenatedString));
            ConcatenatedString_Append_Object = GetInstanceMethod(typeof(ConcatenatedString), "Append", typeof(object));
            ConcatenatedString_Append_String = GetInstanceMethod(typeof(ConcatenatedString), "Append", typeof(string));
            ConcatenatedString_Append_ConcatenatedString = GetInstanceMethod(typeof(ConcatenatedString), "Append", typeof(ConcatenatedString));
            ConcatenatedString_ToString = GetInstanceMethod(typeof(ConcatenatedString), "ToString");

            JavaScriptException_Constructor_Error = GetConstructor(typeof(JavaScriptException), typeof(ScriptEngine), typeof(ErrorType), typeof(string), typeof(int), typeof(string), typeof(string));
            JavaScriptException_Constructor_Object = GetConstructor(typeof(JavaScriptException), typeof(object), typeof(int), typeof(string), typeof(string));
            IEnumerable_String_GetEnumerator = GetInstanceMethod(typeof(IEnumerable<string>), "GetEnumerator");
            IEnumerable_Object_GetEnumerator = GetInstanceMethod(typeof(IEnumerable<object>), "GetEnumerator");
            IEnumerator_MoveNext = GetInstanceMethod(typeof(System.Collections.IEnumerator), "MoveNext");
            IEnumerator_String_Current = GetInstanceMethod(typeof(IEnumerator<string>), "get_Current");
            IEnumerator_Object_Current = GetInstanceMethod(typeof(IEnumerator<object>), "get_Current");
            JavaScriptException_ErrorObject = GetInstanceMethod(typeof(JavaScriptException), "get_ErrorObject");
            Boolean_Construct = GetInstanceMethod(typeof(BooleanConstructor), "Construct", typeof(bool));
            
            RegExp_Construct = GetInstanceMethod(typeof(RegExpConstructor), "Construct", typeof(object), typeof(string));
            Array_New = GetInstanceMethod(typeof(ArrayConstructor), "New", typeof(object[]));
            Object_Construct = GetInstanceMethod(typeof(ObjectConstructor), "Construct");
            Delegate_CreateDelegate = GetStaticMethod(typeof(Delegate), "CreateDelegate", typeof(Type), typeof(MethodInfo));
            Type_GetTypeFromHandle = GetStaticMethod(typeof(Type), "GetTypeFromHandle", typeof(RuntimeTypeHandle));
            MethodBase_GetMethodFromHandle = GetStaticMethod(typeof(MethodBase), "GetMethodFromHandle", typeof(RuntimeMethodHandle));
            PropertyDescriptor_Constructor2 = GetConstructor(typeof(PropertyDescriptor), typeof(object), typeof(PropertyAttributes));
            PropertyDescriptor_Constructor3 = GetConstructor(typeof(PropertyDescriptor), typeof(FunctionInstance), typeof(FunctionInstance), typeof(PropertyAttributes));
            Decimal_Constructor_Double = GetConstructor(typeof(decimal), typeof(double));
            PropertyName_Constructor = GetConstructor(typeof(PropertyReference), typeof(string));

            GeneratedMethod_Load = GetStaticMethod(typeof(GeneratedMethod), "Load", typeof(long));
            ClrInstanceWrapper_GetWrappedInstance = GetInstanceMethod(typeof(ClrInstanceWrapper), "get_WrappedInstance");
            ClrInstanceWrapper_Create = GetStaticMethod(typeof(ClrInstanceWrapper), "Create", new Type[] { typeof(ScriptEngine), typeof(object) });
            Decimal_ToDouble = GetStaticMethod(typeof(decimal), "ToDouble", typeof(decimal));
            BinderUtilities_ResolveOverloads = GetStaticMethod(typeof(BinderUtilities), "ResolveOverloads", typeof(RuntimeMethodHandle[]), typeof(ScriptEngine), typeof(object), typeof(object[]));
            Convert_ToInt32_Double = GetStaticMethod(typeof(Convert), "ToInt32", typeof(double));
            Math_Pow = GetStaticMethod(typeof(MathObject), "Pow", typeof(double), typeof(double));

            Undefined_Value = GetField(typeof(Undefined), "Value");
            Null_Value = GetField(typeof(Null), "Value");

            LongJumpException_Constructor = GetConstructor(typeof(LongJumpException), typeof(int));
            LongJumpException_RouteID = GetInstanceMethod(typeof(LongJumpException), "get_RouteID");

            // Functions
            ReflectionHelpers_CreateFunction = GetStaticMethod(typeof(ReflectionHelpers), "CreateFunction", typeof(ObjectInstance),
                typeof(string), typeof(IList<string>), typeof(RuntimeScope), typeof(string), typeof(GeneratedMethod), typeof(bool), typeof(ObjectInstance));

            // Object literals
            ReflectionHelpers_SetObjectLiteralValue = GetStaticMethod(typeof(ReflectionHelpers), "SetObjectLiteralValue", typeof(ObjectInstance), typeof(object), typeof(object));
            ReflectionHelpers_SetObjectLiteralGetter = GetStaticMethod(typeof(ReflectionHelpers), "SetObjectLiteralGetter", typeof(ObjectInstance), typeof(object), typeof(UserDefinedFunction));
            ReflectionHelpers_SetObjectLiteralSetter = GetStaticMethod(typeof(ReflectionHelpers), "SetObjectLiteralSetter", typeof(ObjectInstance), typeof(object), typeof(UserDefinedFunction));

            // Classes
            ReflectionHelpers_ConstructClass = GetStaticMethod(typeof(ReflectionHelpers), "ConstructClass", typeof(ScriptEngine), typeof(string), typeof(object), typeof(UserDefinedFunction));
            ReflectionHelpers_SetClassValue = GetStaticMethod(typeof(ReflectionHelpers), "SetClassValue", typeof(ObjectInstance), typeof(object), typeof(object));
            ReflectionHelpers_SetClassGetter = GetStaticMethod(typeof(ReflectionHelpers), "SetClassGetter", typeof(ObjectInstance), typeof(object), typeof(UserDefinedFunction));
            ReflectionHelpers_SetClassSetter = GetStaticMethod(typeof(ReflectionHelpers), "SetClassSetter", typeof(ObjectInstance), typeof(object), typeof(UserDefinedFunction));

            // Template literals
            ReflectionHelpers_GetCachedTemplateStringsArray = GetStaticMethod(typeof(ReflectionHelpers), nameof(GetCachedTemplateStringsArray), typeof(ScriptEngine), typeof(int));
            ReflectionHelpers_CreateTemplateStringsArray = GetStaticMethod(typeof(ReflectionHelpers), nameof(CreateTemplateStringsArray), typeof(ScriptEngine), typeof(int), typeof(string[]), typeof(string[]));

            // ExecutionContext
            ExecutionContext_GetEngine = GetInstanceMethod(typeof(ExecutionContext), "get_" + nameof(ExecutionContext.Engine));
            ExecutionContext_GetThisValue = GetInstanceMethod(typeof(ExecutionContext), "get_" + nameof(ExecutionContext.ThisValue));
            ExecutionContext_GetSuperValue = GetInstanceMethod(typeof(ExecutionContext), "get_" + nameof(ExecutionContext.SuperValue));
            ExecutionContext_GetExecutingFunction = GetInstanceMethod(typeof(ExecutionContext), "get_" + nameof(ExecutionContext.ExecutingFunction));
            ExecutionContext_GetNewTargetObject = GetInstanceMethod(typeof(ExecutionContext), "get_" + nameof(ExecutionContext.NewTargetObject));
            ExecutionContext_ConvertThisToObject = GetInstanceMethod(typeof(ExecutionContext), nameof(ExecutionContext.ConvertThisToObject));
            ExecutionContext_CallSuperClass = GetInstanceMethod(typeof(ExecutionContext), nameof(ExecutionContext.CallSuperClass), typeof(object[]));
            ExecutionContext_CreateArgumentsInstance = GetInstanceMethod(typeof(ExecutionContext), nameof(ExecutionContext.CreateArgumentsInstance), typeof(RuntimeScope), typeof(object[]));
            ExecutionContext_ParentScope = GetInstanceMethod(typeof(ExecutionContext), "get_" + nameof(ExecutionContext.ParentScope));
            ExecutionContext_CreateRuntimeScope = GetInstanceMethod(typeof(ExecutionContext), nameof(ExecutionContext.CreateRuntimeScope), typeof(RuntimeScope), typeof(ScopeType), typeof(string[]), typeof(string[]), typeof(string[]));

            // RuntimeScope
            RuntimeScope_GetValue = GetInstanceMethod(typeof(RuntimeScope), nameof(RuntimeScope.GetValue), typeof(string), typeof(int), typeof(string));
            RuntimeScope_GetValueNoThrow = GetInstanceMethod(typeof(RuntimeScope), nameof(RuntimeScope.GetValueNoThrow), typeof(string), typeof(int), typeof(string));
            RuntimeScope_SetValue = GetInstanceMethod(typeof(RuntimeScope), nameof(RuntimeScope.SetValue), typeof(string), typeof(object), typeof(int), typeof(string));
            RuntimeScope_SetValueStrict = GetInstanceMethod(typeof(RuntimeScope), nameof(RuntimeScope.SetValueStrict), typeof(string), typeof(object), typeof(int), typeof(string));
            RuntimeScope_Delete = GetInstanceMethod(typeof(RuntimeScope), nameof(RuntimeScope.Delete), typeof(string));
            RuntimeScope_With = GetInstanceMethod(typeof(RuntimeScope), nameof(RuntimeScope.With), typeof(object));
            RuntimeScope_ImplicitThis = GetInstanceMethod(typeof(RuntimeScope), "get_" + nameof(RuntimeScope.ImplicitThis));

#if DEBUG && ENABLE_DEBUGGING
            // When using Reflection Emit, all calls into Jurassic.dll are cross-assembly and thus
            // must be public.
            var text = new System.Text.StringBuilder();
            foreach (var reflectionField in GetMembers())
            {
                var methodBase = reflectionField.MemberInfo as MethodBase;
                if (methodBase != null && (methodBase.Attributes & MethodAttributes.Public) != MethodAttributes.Public)
                {
                    text.Append(methodBase.DeclaringType.ToString());
                    text.Append("/");
                    text.AppendLine(methodBase.ToString());
                }
                var field = reflectionField.MemberInfo as FieldInfo;
                if (field != null && (field.Attributes & FieldAttributes.Public) != FieldAttributes.Public)
                    text.AppendLine(field.ToString());
                if ((reflectionField.MemberInfo.DeclaringType.Attributes & TypeAttributes.Public) != TypeAttributes.Public)
                    text.AppendLine(reflectionField.MemberInfo.DeclaringType.ToString());
            }
            if (text.Length > 0)
                throw new InvalidOperationException("The following members need to be public: " + Environment.NewLine + text.ToString());
#endif
        }



        //     CODE-GEN METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new instance of a user-defined function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="argumentNames"> The names of the arguments. </param>
        /// <param name="parentScope"> The scope at the point the function is declared. </param>
        /// <param name="bodyText"> The source code for the function body. </param>
        /// <param name="generatedMethod"> A delegate which represents the body of the function plus any dependencies. </param>
        /// <param name="strictMode"> <c>true</c> if the function body is strict mode; <c>false</c> otherwise. </param>
        /// <param name="container"> A reference to the containing class prototype or object literal (or <c>null</c>). </param>
        /// <remarks> This is used by functions declared in JavaScript code (including getters and setters). </remarks>
        public static UserDefinedFunction CreateFunction(ObjectInstance prototype, string name, IList<string> argumentNames,
            RuntimeScope parentScope, string bodyText, GeneratedMethod generatedMethod, bool strictMode, ObjectInstance container)
        {
            return new UserDefinedFunction(prototype, name, argumentNames, parentScope, bodyText, generatedMethod, strictMode, container);
        }

        /// <summary>
        /// Sets the value of a object literal property to a value.
        /// </summary>
        /// <param name="obj"> The object to set the property on. </param>
        /// <param name="key"> The property key (can be a string or a symbol). </param>
        /// <param name="value"> The value to set. </param>
        public static void SetObjectLiteralValue(ObjectInstance obj, object key, object value)
        {
            obj.DefineProperty(key, new PropertyDescriptor(value, PropertyAttributes.FullAccess), throwOnError: false);
        }

        /// <summary>
        /// Sets the value of a object literal property to a getter.  If the value already has a
        /// setter then it will be retained.
        /// </summary>
        /// <param name="obj"> The object to set the property on. </param>
        /// <param name="key"> The property key (can be a string or a symbol). </param>
        /// <param name="getter"> The getter function. </param>
        public static void SetObjectLiteralGetter(ObjectInstance obj, object key, UserDefinedFunction getter)
        {
            var descriptor = obj.GetOwnPropertyDescriptor(key);
            if (descriptor.Exists == false || !descriptor.IsAccessor)
                obj.DefineProperty(key, new PropertyDescriptor(getter, null, PropertyAttributes.FullAccess), throwOnError: false);
            else
                obj.DefineProperty(key, new PropertyDescriptor(getter, descriptor.Setter, PropertyAttributes.FullAccess), throwOnError: false);
        }

        /// <summary>
        /// Sets the value of a object literal property to a setter.  If the value already has a
        /// getter then it will be retained.
        /// </summary>
        /// <param name="obj"> The object to set the property on. </param>
        /// <param name="key"> The property key (can be a string or a symbol).</param>
        /// <param name="setter"> The setter function. </param>
        public static void SetObjectLiteralSetter(ObjectInstance obj, object key, UserDefinedFunction setter)
        {
            var descriptor = obj.GetOwnPropertyDescriptor(key);
            if (descriptor.Exists == false || !descriptor.IsAccessor)
                obj.DefineProperty(key, new PropertyDescriptor(null, setter, PropertyAttributes.FullAccess), throwOnError: false);
            else
                obj.DefineProperty(key, new PropertyDescriptor(descriptor.Getter, setter, PropertyAttributes.FullAccess), throwOnError: false);
        }

        /// <summary>
        /// Retrieves a cached template string array, using the given call site ID as the cache key.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="callSiteId"> The call site ID to use as a cache key. </param>
        /// <returns></returns>
        public static ArrayInstance GetCachedTemplateStringsArray(ScriptEngine engine, int callSiteId)
        {
            return engine.GetCachedTemplateStringsArray(callSiteId);
        }

        /// <summary>
        /// Creates an array suitable for passing to a tag function.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="callSiteId"> The call site ID to use as a cache key. </param>
        /// <param name="strings"> An array of strings that make up the template literal,
        /// with escape character processing. </param>
        /// <param name="rawStrings"> An array of strings that make up the template literal,
        /// without any escape character processing. </param>
        /// <returns> A JS array suitable for passing to a tag function. </returns>
        public static ArrayInstance CreateTemplateStringsArray(ScriptEngine engine, int callSiteId, string[] strings, string[] rawStrings)
        {
            // The result is an array with a 'raw' property which is also an array.
            var result = engine.Array.New(strings);
            result["raw"] = ObjectConstructor.Freeze(engine.Array.New(rawStrings));
            ObjectConstructor.Freeze(result);
            engine.SetCachedTemplateStringsArray(callSiteId, result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="name"></param>
        /// <param name="extends"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static FunctionInstance ConstructClass(ScriptEngine engine, string name, object extends, UserDefinedFunction constructor)
        {
            if (extends is FunctionInstance extendsFunction)
            {
                // If extends doesn't have [[Construct]] then throw a TypeError.
                return new ClassFunction(extendsFunction, name, ObjectInstance.CreateRawObject(extendsFunction.InstancePrototype), constructor);
            }
            else if (extends == Null.Value)
            {
                return new ClassFunction(engine.Function.InstancePrototype, name, ObjectInstance.CreateRootObject(engine), constructor);
            }
            else
            {
                if (extends != null)
                    throw new JavaScriptException(engine, ErrorType.TypeError, $"Class {name} cannot extend '{extends}' as it is not a constructor.");
                return new ClassFunction(engine.Function.InstancePrototype, name, engine.Object.Construct(), constructor);
            }
        }

        /// <summary>
        /// Sets the value of a class property to a value.
        /// </summary>
        /// <param name="obj"> The object to set the property on. </param>
        /// <param name="key"> The property key (can be a string or a symbol). </param>
        /// <param name="value"> The value to set. </param>
        public static void SetClassValue(ObjectInstance obj, object key, object value)
        {
            obj.DefineProperty(key, new PropertyDescriptor(value, Library.PropertyAttributes.NonEnumerable), throwOnError: false);
        }

        /// <summary>
        /// Sets the value of a class property to a getter.  If the value already has a
        /// setter then it will be retained.
        /// </summary>
        /// <param name="obj"> The object to set the property on. </param>
        /// <param name="key"> The property key (can be a string or a symbol). </param>
        /// <param name="getter"> The getter function. </param>
        public static void SetClassGetter(ObjectInstance obj, object key, UserDefinedFunction getter)
        {
            var descriptor = obj.GetOwnPropertyDescriptor(key);
            if (descriptor.Exists == false || !descriptor.IsAccessor)
                obj.DefineProperty(key, new PropertyDescriptor(getter, null, Library.PropertyAttributes.NonEnumerable), throwOnError: false);
            else
                obj.DefineProperty(key, new PropertyDescriptor(getter, descriptor.Setter, Library.PropertyAttributes.NonEnumerable), throwOnError: false);
        }

        /// <summary>
        /// Sets the value of a class property to a setter.  If the value already has a
        /// getter then it will be retained.
        /// </summary>
        /// <param name="obj"> The object to set the property on. </param>
        /// <param name="key"> The property key (can be a string or a symbol).</param>
        /// <param name="setter"> The setter function. </param>
        public static void SetClassSetter(ObjectInstance obj, object key, UserDefinedFunction setter)
        {
            var descriptor = obj.GetOwnPropertyDescriptor(key);
            if (descriptor.Exists == false || !descriptor.IsAccessor)
                obj.DefineProperty(key, new PropertyDescriptor(null, setter, Library.PropertyAttributes.NonEnumerable), throwOnError: false);
            else
                obj.DefineProperty(key, new PropertyDescriptor(descriptor.Getter, setter, Library.PropertyAttributes.NonEnumerable), throwOnError: false);
        }



        //     HELPER METHODS
        //_________________________________________________________________________________________


        internal struct ReflectionField
        {
            public string FieldName;
            public MemberInfo MemberInfo;
        }

        /// <summary>
        /// Gets an enumerable list of all the MemberInfos that are statically known to be used by this DLL.
        /// </summary>
        /// <returns> An enumerable list of all the MemberInfos that are used by this DLL. </returns>
        internal static IEnumerable<ReflectionField> GetMembers()
        {
            foreach (FieldInfo field in typeof(ReflectionHelpers).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (field.FieldType != typeof(MethodInfo) && field.FieldType != typeof(ConstructorInfo) && field.FieldType != typeof(FieldInfo))
                    continue;
                yield return new ReflectionField() { FieldName = field.Name, MemberInfo = (MemberInfo)field.GetValue(null) };
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