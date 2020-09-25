namespace Jurassic.Library
{
    /// <summary>
    /// Represents a symbol primitive.
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Creates a new symbol primitive.
        /// </summary>
        /// <param name="description"> An optional description of the symbol. </param>
        public Symbol(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// An optional description of the symbol.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Converts the value to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Symbol({Description})";
        }
    }
}
