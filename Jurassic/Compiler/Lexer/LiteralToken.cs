using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a string, number, boolean or null literal in the source code.
    /// </summary>
    internal class LiteralToken : Token
    {
        private object value;

        /// <summary>
        /// Creates a new LiteralToken instance with the given value.
        /// </summary>
        /// <param name="value"></param>
        public LiteralToken(object value)
        {
            this.value = value;
        }

        // Literal keywords.
        public readonly static LiteralToken True = new LiteralToken(true);
        public readonly static LiteralToken False = new LiteralToken(false);
        public readonly static LiteralToken Null = new LiteralToken(Jurassic.Null.Value);

        /// <summary>
        /// The value of the literal.
        /// </summary>
        public object Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets a string that represents the token in a parseable form.
        /// </summary>
        public override string Text
        {
            get
            {
                if (this.Value is string)
                    return string.Format("\"{0}\"", ((string)this.Value).Replace("\"", "\\\""));
                if (this.Value is bool)
                    return (bool)this.Value ? "true" : "false";
                return this.Value.ToString();
            }
        }
    }

    /// <summary>
    /// Represents a multi-line literal (i.e. a string literal with line continuations).
    /// </summary>
    internal class MultiLineLiteralToken : LiteralToken
    {
        public MultiLineLiteralToken(object value, int lineTerminatorCount)
            : base(value)
        {
            this.LineTerminatorCount = lineTerminatorCount;
        }

        /// <summary>
        /// Gets the number of line terminators encounted while parsing the string literal.  This
        /// is not the same as the number of line terminators within the literal itself.
        /// </summary>
        public int LineTerminatorCount
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Represents the value of a regular expression literal token.
    /// </summary>
    internal class RegularExpressionLiteralValue
    {
        /// <summary>
        /// Creates a new RegularExpressionLiteralValue instance.
        /// </summary>
        /// <param name="body"> The regular expression text. </param>
        /// <param name="flags"> One or more flag characters concatenated together to form a
        /// string. </param>
        public RegularExpressionLiteralValue(string body, string flags)
        {
            if (body == null)
                throw new ArgumentNullException("body");
            if (flags == null)
                throw new ArgumentNullException("flags");
            this.Body = body;
            this.Flags = flags;
        }

        /// <summary>
        /// Gets the regular expression text.
        /// </summary>
        public string Body
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets one or more flag characters concatenated together to form a string.
        /// </summary>
        public string Flags
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            return string.Format("/{0}/{1}", this.Body, this.Flags);
        }
    }

}