namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents different types of scopes.
    /// </summary>
    public enum ScopeType
    {
        /// <summary>
        /// The top-level global scope.
        /// </summary>
        Global,

        /// <summary>
        /// A scope associated with a 'with' statement.
        /// </summary>
        With,

        /// <summary>
        /// The top-level scope in a function.
        /// </summary>
        TopLevelFunction,

        /// <summary>
        /// 
        /// </summary>
        Block,

        /// <summary>
        /// The scope for a non-strict mode eval() function call.
        /// </summary>
        Eval,

        /// <summary>
        /// The scope for a strict mode eval() function call.
        /// </summary>
        EvalStrict,
    }
}