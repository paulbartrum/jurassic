namespace Jurassic
{

    /// <summary>
    /// Used to indicate that the script engine should run in compatibility mode.
    /// </summary>
    public enum CompatibilityMode
    {
        /// <summary>
        /// Indicates that the script engine should run in the most standards compliant mode.
        /// </summary>
        Latest,

        /// <summary>
        /// Indicates that the script engine should conform to the ECMAScript 3 specification.
        /// This has the following effects:
        /// 1. "this" is converted to an object at the call site of function calls.
        /// 2. Octal literals and octal escape sequences are supported.
        /// 3. parseInt() parses octal numbers without requiring an explicit radix.
        /// 4. NaN, undefined and Infinity can be modified.
        /// 5. The list of keywords is much longer (for example, 'abstract' is a keyword).
        /// </summary>
        ECMAScript3,
    }

}
