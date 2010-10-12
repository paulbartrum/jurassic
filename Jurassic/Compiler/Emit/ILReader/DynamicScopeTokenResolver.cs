using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

#if DEBUG && !SILVERLIGHT

namespace ClrTest.Reflection {
    internal class DynamicScopeTokenResolver : ITokenResolver {
#region Static stuffs
        private static PropertyInfo s_indexer;
        private static FieldInfo s_scopeFi;

        private static Type s_genMethodInfoType;
        private static FieldInfo s_genmethFi1, s_genmethFi2;

        private static Type s_varArgMethodType;
        private static FieldInfo s_varargFi1, s_varargFi2;

        private static Type s_genFieldInfoType;
        private static FieldInfo s_genfieldFi1, s_genfieldFi2;

        static DynamicScopeTokenResolver() {
            BindingFlags s_bfInternal = BindingFlags.NonPublic | BindingFlags.Instance;
            s_indexer = Type.GetType("System.Reflection.Emit.DynamicScope").GetProperty("Item", s_bfInternal);
            s_scopeFi = Type.GetType("System.Reflection.Emit.DynamicILGenerator").GetField("m_scope", s_bfInternal);

            s_varArgMethodType = Type.GetType("System.Reflection.Emit.VarArgMethod");
            s_varargFi1 = s_varArgMethodType.GetField("m_method", s_bfInternal);
            s_varargFi2 = s_varArgMethodType.GetField("m_signature", s_bfInternal);

            s_genMethodInfoType = Type.GetType("System.Reflection.Emit.GenericMethodInfo");
            s_genmethFi1 = s_genMethodInfoType.GetField("m_methodHandle", s_bfInternal);
            s_genmethFi2 = s_genMethodInfoType.GetField("m_context", s_bfInternal);

            s_genFieldInfoType = Type.GetType("System.Reflection.Emit.GenericFieldInfo", false);
            if (s_genFieldInfoType != null) {
                s_genfieldFi1 = s_genFieldInfoType.GetField("m_fieldHandle", s_bfInternal);
                s_genfieldFi2 = s_genFieldInfoType.GetField("m_context", s_bfInternal);
            } else {
                s_genfieldFi1 = s_genfieldFi2 = null;
            }
        }
        #endregion

        object m_scope = null;
        internal object this[int token] {
            get {
                return s_indexer.GetValue(m_scope, new object[] { token });
            }
        }

        public DynamicScopeTokenResolver(DynamicMethod dm) {
            m_scope = s_scopeFi.GetValue(dm.GetILGenerator());
        }

        public String AsString(int token) {
            return this[token] as string;
        }

        public FieldInfo AsField(int token) {
            if (this[token] is RuntimeFieldHandle)
                return FieldInfo.GetFieldFromHandle((RuntimeFieldHandle)this[token]);

            if (this[token].GetType() == s_genFieldInfoType) {
                return FieldInfo.GetFieldFromHandle(
                        (RuntimeFieldHandle)s_genfieldFi1.GetValue(this[token]),
                        (RuntimeTypeHandle)s_genfieldFi2.GetValue(this[token]));
            }

            Debug.Assert(false, string.Format("unexpected type: {0}", this[token].GetType()));
            return null;
        }

        public Type AsType(int token) {
            return Type.GetTypeFromHandle((RuntimeTypeHandle)this[token]);
        }

        public MethodBase AsMethod(int token) {
            if (this[token] is DynamicMethod)
                return this[token] as DynamicMethod;

            if (this[token] is RuntimeMethodHandle)
                return MethodBase.GetMethodFromHandle((RuntimeMethodHandle)this[token]);

            if (this[token].GetType() == s_genMethodInfoType)
                return MethodBase.GetMethodFromHandle(
                    (RuntimeMethodHandle)s_genmethFi1.GetValue(this[token]),
                    (RuntimeTypeHandle)s_genmethFi2.GetValue(this[token]));

            if (this[token].GetType() == s_varArgMethodType)
                return (MethodInfo)s_varargFi1.GetValue(this[token]);

            Debug.Assert(false, string.Format("unexpected type: {0}", this[token].GetType()));
            return null;
        }

        public MemberInfo AsMember(int token) {
            if ((token & 0x02000000) == 0x02000000)
                return this.AsType(token);
            if ((token & 0x06000000) == 0x06000000)
                return this.AsMethod(token);
            if ((token & 0x04000000) == 0x04000000)
                return this.AsField(token);

            Debug.Assert(false, string.Format("unexpected token type: {0:x8}", token));
            return null;
        }

        public byte[] AsSignature(int token) {
            return this[token] as byte[];
        }
    }
}

#endif