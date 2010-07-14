using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

#if DEBUG

namespace ClrTest.Reflection {
    public class DynamicMethodILProvider : IILProvider {
        static FieldInfo s_fiLen = typeof(ILGenerator).GetField("m_length", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo s_fiStream = typeof(ILGenerator).GetField("m_ILStream", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo s_miBakeByteArray = typeof(ILGenerator).GetMethod("BakeByteArray", BindingFlags.NonPublic | BindingFlags.Instance);

        DynamicMethod m_method;
        byte[] m_byteArray;

        public DynamicMethodILProvider(DynamicMethod method) {
            m_method = method;
        }

        public byte[] GetByteArray() {
            if (m_byteArray == null) {
                ILGenerator ilgen = m_method.GetILGenerator();
                try {
                    m_byteArray = (byte[])s_miBakeByteArray.Invoke(ilgen, null);
                    if (m_byteArray == null) m_byteArray = new byte[0];
                } catch (TargetInvocationException) {
                    int length = (int)s_fiLen.GetValue(ilgen);
                    m_byteArray = new byte[length];
                    Array.Copy((byte[])s_fiStream.GetValue(ilgen), m_byteArray, length);
                }
            }
            return m_byteArray;
        }

    }
}

#endif