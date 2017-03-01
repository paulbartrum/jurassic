using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a non-runtime compiler error.
    /// </summary>
    public class SyntaxErrorException : Exception
    {
        /// <summary>
        /// Creates a new syntax error exception.
        /// </summary>
        /// <param name="message"> A description of the error. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        public SyntaxErrorException(string message, int lineNumber, string sourcePath)
            : base(message)
        {
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
        }

        /// <summary>
        /// Creates a new syntax error exception.
        /// </summary>
        /// <param name="message"> A description of the error. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        /// <param name="functionName"> The name of the function.  Can be <c>null</c>. </param>
        public SyntaxErrorException(string message, int lineNumber, string sourcePath, string functionName)
            : base(message)
        {
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
            this.FunctionName = functionName;
        }

        /// <summary>
        /// Gets the line number in the source file the error occurred on.  Can be <c>0</c> if no
        /// line number information is available.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the path or URL of the source file.  Can be <c>null</c> if no source information
        /// is available.
        /// </summary>
        public string SourcePath { get; private set; }

        /// <summary>
        /// Gets the name of the function where the exception occurred.  Can be <c>null</c> if no
        /// source information is available.
        /// </summary>
        public string FunctionName { get; internal set; }
    }
}
