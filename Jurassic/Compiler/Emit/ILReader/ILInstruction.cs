using System;
using System.Reflection.Emit;
using System.Reflection;

#if DEBUG && !SILVERLIGHT

namespace ClrTest.Reflection {
    internal abstract class ILInstruction
    {
        protected Int32 m_offset;
        protected OpCode m_opCode;

        internal ILInstruction(Int32 offset, OpCode opCode) {
            this.m_offset = offset;
            this.m_opCode = opCode;
        }

        public Int32 Offset { get { return m_offset; } }
        public OpCode OpCode { get { return m_opCode; } }

        public abstract void Accept(ILInstructionVisitor vistor);
    }

    internal class InlineNoneInstruction : ILInstruction
    {
        internal InlineNoneInstruction(Int32 offset, OpCode opCode)
            : base(offset, opCode) { }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineNoneInstruction(this); }
    }

    internal class InlineBrTargetInstruction : ILInstruction
    {
        private Int32 m_delta;

        internal InlineBrTargetInstruction(Int32 offset, OpCode opCode, Int32 delta)
            : base(offset, opCode) {
            this.m_delta = delta;
        }

        public Int32 Delta { get { return m_delta; } }
        public Int32 TargetOffset { get { return m_offset + m_delta + 1 + 4; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineBrTargetInstruction(this); }
    }

    internal class ShortInlineBrTargetInstruction : ILInstruction
    {
        private SByte m_delta;

        internal ShortInlineBrTargetInstruction(Int32 offset, OpCode opCode, SByte delta)
            : base(offset, opCode) {
            this.m_delta = delta;
        }

        public SByte Delta { get { return m_delta; } }
        public Int32 TargetOffset { get { return m_offset + m_delta + 1 + 1; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitShortInlineBrTargetInstruction(this); }
    }

    internal class InlineSwitchInstruction : ILInstruction
    {
        private Int32[] m_deltas;
        private Int32[] m_targetOffsets;

        internal InlineSwitchInstruction(Int32 offset, OpCode opCode, Int32[] deltas)
            : base(offset, opCode) {
            this.m_deltas = deltas;
        }

        public Int32[] Deltas { get { return (Int32[])m_deltas.Clone(); } }
        public Int32[] TargetOffsets {
            get {
                if (m_targetOffsets == null) {
                    int cases = m_deltas.Length;
                    int itself = 1 + 4 + 4 * cases;
                    m_targetOffsets = new Int32[cases];
                    for (Int32 i = 0; i < cases; i++)
                        m_targetOffsets[i] = m_offset + m_deltas[i] + itself;
                }
                return m_targetOffsets;
            }
        }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineSwitchInstruction(this); }
    }

    internal class InlineIInstruction : ILInstruction
    {
        private Int32 m_int32;

        internal InlineIInstruction(Int32 offset, OpCode opCode, Int32 value)
            : base(offset, opCode) {
            this.m_int32 = value;
        }

        public Int32 Int32 { get { return m_int32; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineIInstruction(this); }
    }
    internal class InlineI8Instruction : ILInstruction
    {
        private Int64 m_int64;

        internal InlineI8Instruction(Int32 offset, OpCode opCode, Int64 value)
            : base(offset, opCode) {
            this.m_int64 = value;
        }

        public Int64 Int64 { get { return m_int64; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineI8Instruction(this); }

    }
    internal class ShortInlineIInstruction : ILInstruction
    {
        private Byte m_int8;

        internal ShortInlineIInstruction(Int32 offset, OpCode opCode, Byte value)
            : base(offset, opCode) {
            this.m_int8 = value;
        }

        public Byte Byte { get { return m_int8; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitShortInlineIInstruction(this); }
    }

    internal class InlineRInstruction : ILInstruction
    {
        private Double m_value;

        internal InlineRInstruction(Int32 offset, OpCode opCode, Double value)
            : base(offset, opCode) {
            this.m_value = value;
        }

        public Double Double { get { return m_value; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineRInstruction(this); }
    }

    internal class ShortInlineRInstruction : ILInstruction
    {
        private Single m_value;

        internal ShortInlineRInstruction(Int32 offset, OpCode opCode, Single value)
            : base(offset, opCode) {
            this.m_value = value;
        }

        public Single Single { get { return m_value; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitShortInlineRInstruction(this); }
    }

    internal class InlineFieldInstruction : ILInstruction
    {
        ITokenResolver m_resolver;
        Int32 m_token;
        FieldInfo m_field;

        internal InlineFieldInstruction(ITokenResolver resolver, Int32 offset, OpCode opCode, Int32 token)
            : base(offset, opCode) {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public FieldInfo Field {
            get {
                if (m_field == null) {
                    m_field = m_resolver.AsField(m_token);
                }
                return m_field;
            }
        }
        public Int32 Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineFieldInstruction(this); }
    }
    internal class InlineMethodInstruction : ILInstruction
    {
        private ITokenResolver m_resolver;
        private Int32 m_token;
        private MethodBase m_method;

        internal InlineMethodInstruction(Int32 offset, OpCode opCode, Int32 token, ITokenResolver resolver)
            : base(offset, opCode) {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public MethodBase Method {
            get {
                if (m_method == null) {
                    m_method = m_resolver.AsMethod(m_token);
                }
                return m_method;
            }
        }
        public Int32 Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineMethodInstruction(this); }
    }
    internal class InlineTypeInstruction : ILInstruction
    {
        private ITokenResolver m_resolver;
        private Int32 m_token;
        private Type m_type;

        internal InlineTypeInstruction(Int32 offset, OpCode opCode, Int32 token, ITokenResolver resolver)
            : base(offset, opCode) {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public Type Type {
            get {
                if (m_type == null) {
                    m_type = m_resolver.AsType(m_token);
                }
                return m_type;
            }
        }
        public Int32 Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineTypeInstruction(this); }
    }
    internal class InlineSigInstruction : ILInstruction
    {
        private ITokenResolver m_resolver;
        private Int32 m_token;
        private byte[] m_signature;

        internal InlineSigInstruction(Int32 offset, OpCode opCode, Int32 token, ITokenResolver resolver)
            : base(offset, opCode) {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public byte[] Signature {
            get {
                if (m_signature == null) {
                    m_signature = m_resolver.AsSignature(m_token);
                }
                return m_signature;
            }
        }
        public Int32 Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineSigInstruction(this); }
    }
    internal class InlineTokInstruction : ILInstruction
    {
        private ITokenResolver m_resolver;
        private Int32 m_token;
        private MemberInfo m_member;

        internal InlineTokInstruction(Int32 offset, OpCode opCode, Int32 token, ITokenResolver resolver)
            : base(offset, opCode) {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public MemberInfo Member {
            get {
                if (m_member == null) {
                    m_member = m_resolver.AsMember(Token);
                }
                return m_member;
            }
        }
        public Int32 Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineTokInstruction(this); }
    }

    internal class InlineStringInstruction : ILInstruction
    {
        private ITokenResolver m_resolver;
        private Int32 m_token;
        private String m_string;

        internal InlineStringInstruction(Int32 offset, OpCode opCode, Int32 token, ITokenResolver resolver)
            : base(offset, opCode) {
            this.m_resolver = resolver;
            this.m_token = token;
        }

        public String String {
            get {
                if (m_string == null) m_string = m_resolver.AsString(Token);
                return m_string;
            }
        }
        public Int32 Token { get { return m_token; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineStringInstruction(this); }
    }

    internal class InlineVarInstruction : ILInstruction
    {
        private UInt16 m_ordinal;

        internal InlineVarInstruction(Int32 offset, OpCode opCode, UInt16 ordinal)
            : base(offset, opCode) {
            this.m_ordinal = ordinal;
        }

        public UInt16 Ordinal { get { return m_ordinal; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineVarInstruction(this); }
    }

    internal class ShortInlineVarInstruction : ILInstruction
    {
        private Byte m_ordinal;

        internal ShortInlineVarInstruction(Int32 offset, OpCode opCode, Byte ordinal)
            : base(offset, opCode) {
            this.m_ordinal = ordinal;
        }

        public Byte Ordinal { get { return m_ordinal; } }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitShortInlineVarInstruction(this); }
    }
}

#endif