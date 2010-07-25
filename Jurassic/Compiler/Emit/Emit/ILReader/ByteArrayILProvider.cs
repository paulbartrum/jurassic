using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

#if DEBUG

namespace ClrTest.Reflection {
    public class ByteArrayILProvider : IILProvider {
        byte[] m_byteArray;

        public ByteArrayILProvider(byte[] bytes, int length)
        {
            m_byteArray = new byte[length];
            Array.Copy(bytes, m_byteArray, length);
        }

        public byte[] GetByteArray() {
            return m_byteArray;
        }

    }
}

#endif