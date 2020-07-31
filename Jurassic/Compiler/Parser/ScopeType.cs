namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents different types of scopes.
    /// </summary>
    public enum ScopeType
    {
        Global,
        With,
        TopLevelFunction,
        Block,
        Eval,
        EvalStrict,
    }
}