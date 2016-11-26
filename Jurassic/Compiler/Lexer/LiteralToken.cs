using System.Globalization;

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
        /// Gets a value that indicates whether the literal is a keyword.  Literal keywords are
        /// <c>false</c>, <c>true</c> and <c>null</c>.
        /// </summary>
        public bool IsKeyword
        {
            get { return this.value is bool || this.value is Null; }
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
                if (this.Value is double)
                    return ((double)this.Value).ToString(CultureInfo.InvariantCulture);
                return this.Value.ToString();
            }
        }
    }

}