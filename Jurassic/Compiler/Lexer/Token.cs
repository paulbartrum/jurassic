using System;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents the base class of all tokens.
    /// </summary>
    internal abstract class Token
    {
        /// <summary>
        /// Gets a string that represents the token in a parseable form.
        /// </summary>
        public abstract string Text
        {
            get;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.Text, this.GetType().Name);
        }
    }

    /// <summary>
    /// Represents whitespace or a line terminator.
    /// </summary>
    internal class WhiteSpaceToken : Token
    {
        public WhiteSpaceToken(int count)
        {
            this.LineTerminatorCount = count;
        }

        /// <summary>
        /// Gets a count of the number of line terminators.
        /// </summary>
        public int LineTerminatorCount
        {
            get;
            private set;
        }

        public override string Text
        {
            get { return Environment.NewLine; }
        }
    }

}