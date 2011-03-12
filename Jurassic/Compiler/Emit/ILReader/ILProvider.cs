using System;
using System.Reflection;

#if DEBUG

namespace ClrTest.Reflection {
    internal interface IILProvider
    {
        Byte[] GetByteArray();
    }

    internal class MethodBaseILProvider : IILProvider
    {
        MethodBase m_method;
        byte[] m_byteArray;

        public MethodBaseILProvider(MethodBase method) {
            m_method = method;
        }

        public byte[] GetByteArray() {
            if (m_byteArray == null) {
                MethodBody methodBody = m_method.GetMethodBody();
                m_byteArray = (methodBody == null) ? new Byte[0] : methodBody.GetILAsByteArray();
            }
            return m_byteArray;
        }
    }
}

#endif