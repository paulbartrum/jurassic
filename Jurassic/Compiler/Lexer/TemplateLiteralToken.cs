using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the text content of a template literal that is prior to a substitution.  In
    /// other words, it indicates to the parser that a substitution immediately follows.
    /// 
    /// Example 1: `plain text`
    /// 1. StringLiteralToken (Value = "plain text")
    /// 
    /// Example 2: `${count}`
    /// 1. TemplateLiteralToken (Value = "")
    /// 2. IdentifierToken (Name = "count")
    /// 3. PunctuatorToken (Text = "}")
    /// 4. StringLiteralToken (Value = "")
    /// 
    /// Example 3: `Bought ${count} items from ${person}!`
    /// 1. TemplateLiteralToken (Value = "Bought ")
    /// 2. IdentifierToken (Name = "count")
    /// 3. PunctuatorToken (Text = "}")
    /// 4. TemplateLiteralToken (Value = " items from ")
    /// 5. IdentifierToken (Name = "person")
    /// 6. PunctuatorToken (Text = "}")
    /// 7. StringLiteralToken (Value = "!")
    /// </summary>
    internal class TemplateLiteralToken : LiteralToken
    {
        /// <summary>
        /// Creates a new TemplateLiteralToken instance.
        /// </summary>
        /// <param name="value"> The literal text. </param>
        public TemplateLiteralToken(string value)
            : base(value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
        }

        /// <summary>
        /// Gets the contents of the template string literal.
        /// </summary>
        public new string Value
        {
            get { return (string)base.Value; }
        }
    }

}