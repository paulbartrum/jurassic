namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a single property of an object literal or a member of a class body.
    /// </summary>
    internal sealed class PropertyDeclaration
    {
        /// <summary>
        /// Creates a new PropertyDeclaration instance.
        /// </summary>
        /// <param name="name"> The property name. </param>
        /// <param name="value"> The property value. </param>
        public PropertyDeclaration(PropertyName name, Expression value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// The member name. Can be static or computed e.g. ['fu' + 'nc']() { }.
        /// </summary>
        public PropertyName Name { get; private set; }

        /// <summary>
        /// The value of the member.
        /// </summary>
        public Expression Value { get; private set; }
    }
}
