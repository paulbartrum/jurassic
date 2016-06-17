using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a string literal.
    /// </summary>
    internal class StringLiteralToken : LiteralToken
    {
        public StringLiteralToken(string value, int escapeSequenceCount, int lineContinuationCount, bool isEndOfTemplateLiteral)
            : base(value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            this.EscapeSequenceCount = escapeSequenceCount;
            this.LineContinuationCount = lineContinuationCount;
            this.IsEndOfTemplateLiteral = isEndOfTemplateLiteral;
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
        /// Indicates whether this string is the last part of a template literal.  For example,
        /// this is true for `test` but also the "world" in `hello${1}world`.
        /// </summary>
        public bool IsEndOfTemplateLiteral
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