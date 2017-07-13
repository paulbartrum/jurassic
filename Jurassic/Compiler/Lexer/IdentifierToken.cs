using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents punctuation or an operator in the source code.
    /// </summary>
    internal class IdentifierToken : Token
    {
        /// <summary>
        /// Creates a new IdentifierToken instance.
        /// </summary>
        /// <param name="name"> The identifier name. </param>
        private IdentifierToken(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the identifier.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a string that represents the token in a parseable form.
        /// </summary>
        public override string Text
        {
            get { return this.Name; }
        }

        // Contextual keywords.
        public readonly static IdentifierToken Of = new IdentifierToken("of");

        /// <summary>
        /// Creates a new identifier token.
        /// </summary>
        /// <param name="name"> The name of the identifer. </param>
        /// <returns> A new IdentifierToken instance. </returns>
        public static IdentifierToken Create(string name)
        {
            if (name == "of")
                return Of;
            return new IdentifierToken(name);
        }
    }

}