using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the text content of a template literal.
    /// 
    /// Example 1: `plain text`
    /// 1. TemplateLiteralToken (Value = "plain text")
    /// 
    /// Example 2: `${count}`
    /// 1. TemplateLiteralToken (Value = "")
    /// 2. IdentifierToken (Name = "count")
    /// 3. PunctuatorToken (Text = "}")
    /// 4. TemplateLiteralToken (Value = "")
    /// 
    /// Example 3: `Bought ${count} items from ${person}!`
    /// 1. TemplateLiteralToken (Value = "Bought ")
    /// 2. IdentifierToken (Name = "count")
    /// 3. PunctuatorToken (Text = "}")
    /// 4. TemplateLiteralToken (Value = " items from ")
    /// 5. IdentifierToken (Name = "person")
    /// 6. PunctuatorToken (Text = "}")
    /// 7. TemplateLiteralToken (Value = "!")
    /// </summary>
    internal class TemplateLiteralToken : LiteralToken
    {
        /// <summary>
        /// Creates a new TemplateLiteralToken instance.
        /// </summary>
        /// <param name="value">The literal text.</param>
        /// <param name="rawText">The raw text.</param>
        /// <param name="substitutionFollows">Indicates whether a substitution follows this
        /// string.</param>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public TemplateLiteralToken(string value, string rawText, bool substitutionFollows)
            : base(value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            this.RawText = rawText;
            this.SubstitutionFollows = substitutionFollows;
        }

        /// <summary>
        /// Indicates whether a substitution follows this string.  For example, this is true for
        /// the "hello" in `hello${1}world`, but not the "world".
        /// </summary>
        public bool SubstitutionFollows { get; private set; }

        /// <summary>
        /// The raw text, prior to performing any escape sequence processing.
        /// </summary>
        public string RawText { get; private set; }

        /// <summary>
        /// Gets the contents of the template string literal.
        /// </summary>
        public new string Value
        {
            get { return (string)base.Value; }
        }
    }

}