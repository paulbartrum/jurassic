using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a string literal.
    /// </summary>
    internal class StringLiteralToken : LiteralToken
    {
        public StringLiteralToken(string value, int escapeSequenceCount, int lineContinuationCount)
            : base(value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            this.EscapeSequenceCount = escapeSequenceCount;
            this.LineContinuationCount = lineContinuationCount;
        }

        /// <summary>
        /// Gets the number of character escape sequences encounted while parsing the string
        /// literal.
        /// </summary>
        public int EscapeSequenceCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of line continuations encounted while parsing the string literal.
        /// </summary>
        public int LineContinuationCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the contents of the string literal.
        /// </summary>
        public new string Value
        {
            get { return (string)base.Value; }
        }
    }

}