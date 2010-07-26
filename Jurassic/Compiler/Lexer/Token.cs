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
        /// <summary>
        /// Creates a new WhiteSpaceToken instance.
        /// </summary>
        /// <param name="lineTerminatorCount"> The number of line terminators encountered while
        /// reading the whitespace. </param>
        public WhiteSpaceToken(int lineTerminatorCount)
        {
            this.LineTerminatorCount = lineTerminatorCount;
        }

        /// <summary>
        /// Gets a count of the number of line terminators.
        /// </summary>
        public int LineTerminatorCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a string that represents the token in a parseable form.
        /// </summary>
        public override string Text
        {
            get { return Environment.NewLine; }
        }
    }

}