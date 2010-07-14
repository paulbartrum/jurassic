using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

#if DEBUG

namespace ClrTest.Reflection {
    public class ILReaderFactory {
        public static ILReader Create(object obj) {
            Type type = obj.GetType();

            if (type == s_dynamicMethodType || type == s_rtDynamicMethodType) {
                DynamicMethod dm;
                if (type == s_rtDynamicMethodType) {
                    //
                    // if the target is RTDynamicMethod, get the value of 
                    // RTDynamicMethod.m_owner instead
                    //
                    dm = (DynamicMethod)s_fiOwner.GetValue(obj);
                } else {
                    dm = obj as DynamicMethod;
                }

                return new ILReader(new DynamicMethodILProvider(dm), new DynamicScopeTokenResolver(dm));
            }

            if (type == s_runtimeMethodInfoType || type == s_runtimeConstructorInfoType) {
                MethodBase method = obj as MethodBase;
                return new ILReader(method);
            }

            throw new NotSupportedException(string.Format("Reading IL from type {0} is currently not supported", type));
        }

        private static Type s_dynamicMethodType = Type.GetType("System.Reflection.Emit.DynamicMethod");
        private static Type s_runtimeMethodInfoType = Type.GetType("System.Reflection.RuntimeMethodInfo");
        private static Type s_runtimeConstructorInfoType = Type.GetType("System.Reflection.RuntimeConstructorInfo");

        private static Type s_rtDynamicMethodType = Type.GetType("System.Reflection.Emit.DynamicMethod+RTDynamicMethod");
        private static FieldInfo s_fiOwner = s_rtDynamicMethodType.GetField("m_owner", BindingFlags.Instance | BindingFlags.NonPublic);
    }
}

#endif