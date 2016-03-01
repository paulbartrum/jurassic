namespace Jurassic.Library
{
    /// <summary>
    /// Used when creating an error to specify the specific type of error.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Represents a generic error.
        /// </summary>
        Error,

        /// <summary>
        /// Indicates a value that is not in the set or range of allowable values.
        /// </summary>
        RangeError,

        /// <summary>
        /// Indicates the actual type of an operand is different than the expected type.
        /// </summary>
        TypeError,

        /// <summary>
        /// Indicates that a parsing error has occurred.
        /// </summary>
        SyntaxError,

        /// <summary>
        /// Indicates that one of the global URI handling functions was used in a way that is incompatible with its definition.
        /// </summary>
        URIError,

        /// <summary>
        /// Not used.
        /// </summary>
        EvalError,

        /// <summary>
        /// Indicate that an invalid reference value has been detected.
        /// </summary>
        ReferenceError,
    }
}