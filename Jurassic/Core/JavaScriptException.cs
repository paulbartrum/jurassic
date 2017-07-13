using System;
using Jurassic.Library;

namespace Jurassic
{
    /// <summary>
    /// Represents a wrapper for javascript error objects.
    /// </summary>
    public class JavaScriptException : Exception
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new JavaScriptException instance based on the given object.
        /// </summary>
        /// <param name="errorObject"> The javascript object that was thrown. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        public JavaScriptException(object errorObject, int lineNumber, string sourcePath)
            : base(TypeConverter.ToString(errorObject))
        {
            this.ErrorObject = errorObject;
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
            this.PopulateStackTrace();
        }

        /// <summary>
        /// Creates a new JavaScriptException instance based on the given object.
        /// </summary>
        /// <param name="errorObject"> The javascript object that was thrown. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        /// <param name="functionName"> The name of the function.  Can be <c>null</c>. </param>
        public JavaScriptException(object errorObject, int lineNumber, string sourcePath, string functionName)
            : base(TypeConverter.ToString(errorObject))
        {
            this.ErrorObject = errorObject;
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
            this.FunctionName = functionName;
            this.PopulateStackTrace();
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="engine"> The script engine used to create the error object. </param>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        public JavaScriptException(ScriptEngine engine, ErrorType type, string message)
            : base(string.Format("{0}: {1}", type, message))
        {
            this.ErrorObject = CreateError(engine, type, message);
            this.PopulateStackTrace();
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified. </param>
        public JavaScriptException(ScriptEngine engine, ErrorType type, string message, Exception innerException)
            : base(string.Format("{0}: {1}", type, message), innerException)
        {
            this.ErrorObject = CreateError(engine, type, message);
            this.PopulateStackTrace();
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        public JavaScriptException(ScriptEngine engine, ErrorType type, string message, int lineNumber, string sourcePath)
            : base(string.Format("{0}: {1}", type, message))
        {
            this.ErrorObject = CreateError(engine, type, message);
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
            this.PopulateStackTrace();
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        /// <param name="functionName"> The name of the function.  Can be <c>null</c>. </param>
        public JavaScriptException(ScriptEngine engine, ErrorType type, string message, int lineNumber, string sourcePath, string functionName)
            : base(string.Format("{0}: {1}", type, message))
        {
            this.ErrorObject = CreateError(engine, type, message);
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
            this.FunctionName = functionName;
            this.PopulateStackTrace();
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets a reference to the JavaScript Error object.
        /// </summary>
        public object ErrorObject { get; private set; }

        /// <summary>
        /// Gets the type of error, e.g. ErrorType.TypeError or ErrorType.SyntaxError.
        /// </summary>
        public string Name
        {
            get
            {
                if (this.ErrorObject is ErrorInstance)
                    return ((ErrorInstance)this.ErrorObject).Name;
                return null;
            }
        }

        /// <summary>
        /// Gets the line number in the source file the error occurred on.  Can be <c>0</c> if no
        /// line number information is available.
        /// </summary>
        public int LineNumber { get; internal set; }

        /// <summary>
        /// Gets the path or URL of the source file.  Can be <c>null</c> if no source information
        /// is available.
        /// </summary>
        public string SourcePath { get; internal set; }

        /// <summary>
        /// Gets the name of the function where the exception occurred.  Can be <c>null</c> if no
        /// source information is available.
        /// </summary>
        public string FunctionName { get; internal set; }

        /// <summary>
        /// Gets a reference to the script engine associated with this object.  Will be <c>null</c>
        /// for statements like "throw 2".
        /// </summary>
        public ScriptEngine Engine
        {
            get
            {
                if (this.ErrorObject is ErrorInstance)
                    return ((ErrorInstance)this.ErrorObject).Engine;
                return null;
            }
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates an error object with the given message.
        /// </summary>
        /// <param name="engine"> The script engine used to create the error object. </param>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        /// <returns> A new Error instance. </returns>
        private static ErrorInstance CreateError(ScriptEngine engine, ErrorType type, string message)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            // Get the constructor corresponding to the error name.
            ErrorConstructor constructor;
            switch (type)
            {
                case ErrorType.Error:
                    constructor = engine.Error;
                    break;
                case ErrorType.RangeError:
                    constructor = engine.RangeError;
                    break;
                case ErrorType.TypeError:
                    constructor = engine.TypeError;
                    break;
                case ErrorType.SyntaxError:
                    constructor = engine.SyntaxError;
                    break;
                case ErrorType.URIError:
                    constructor = engine.URIError;
                    break;
                case ErrorType.EvalError:
                    constructor = engine.EvalError;
                    break;
                case ErrorType.ReferenceError:
                    constructor = engine.ReferenceError;
                    break;
                default:
                    throw new ArgumentException($"Unrecognised error type {type}.", nameof(type));
            }

            // Create an error instance.
            return constructor.Construct(message);
        }

        /// <summary>
        /// Populates the error object stack trace, if the error object is an Error.
        /// </summary>
        internal void PopulateStackTrace()
        {
            // Ensure the error object is an Error or derived instance.
            var errorObject = this.ErrorObject as ErrorInstance;
            if (errorObject == null)
                return;
            errorObject.SetStackTrace(this.SourcePath, this.FunctionName, this.LineNumber);
        }
    }
}
