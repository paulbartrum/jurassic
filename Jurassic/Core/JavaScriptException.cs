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
        public JavaScriptException(object errorObject)
            : this(errorObject, 0, null, null)
        {
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

            if (this.ErrorObject is ErrorInstance error)
            {
                if (Enum.TryParse(error.Name, out ErrorType errorType))
                    this.ErrorType = errorType;
                this.ErrorMessage = error.Message;
                error.SetStackTrace(this.SourcePath, this.FunctionName, this.LineNumber);
            }
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        public JavaScriptException(ErrorType type, string message)
            : base(string.Format("{0}: {1}", type, message))
        {
            this.ErrorType = type;
            this.ErrorMessage = message;
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        public JavaScriptException(ErrorType type, string message, int lineNumber, string sourcePath)
            : base(string.Format("{0}: {1}", type, message))
        {
            this.ErrorType = type;
            this.ErrorMessage = message;
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified. </param>
        public JavaScriptException(ErrorType type, string message, int lineNumber, string sourcePath, Exception innerException)
            : base(string.Format("{0}: {1}", type, message), innerException)
        {
            this.ErrorType = type;
            this.ErrorMessage = message;
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <param name="message"> A description of the error. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        /// <param name="functionName"> The name of the function.  Can be <c>null</c>. </param>
        public JavaScriptException(ErrorType type, string message, int lineNumber, string sourcePath, string functionName)
            : base(string.Format("{0}: {1}", type, message))
        {
            this.ErrorType = type;
            this.ErrorMessage = message;
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
            this.FunctionName = functionName;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets a reference to the JavaScript Error object.
        /// </summary>
        private object ErrorObject { get; set; }

        /// <summary>
        /// Gets the type of error, e.g. ErrorType.TypeError or ErrorType.SyntaxError.
        /// </summary>
        public ErrorType? ErrorType { get; private set; }

        /// <summary>
        /// The error message, excluding the error type.
        /// </summary>
        public string ErrorMessage { get; private set; }

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
        internal ScriptEngine Engine
        {
            get
            {
                if (this.ErrorObject is ObjectInstance objectInstance)
                    return objectInstance.Engine;
                return null;
            }
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns the error instance associated with this exception.
        /// </summary>
        /// <param name="engine"> The script engine used to create the error object. </param>
        /// <returns> A new Error instance. </returns>
        public object GetErrorObject(ScriptEngine engine)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));
            if (ErrorObject == null)
            {
                // Get the constructor corresponding to the error name.
                ErrorConstructor constructor;
                switch (ErrorType.Value)
                {
                    case Library.ErrorType.Error:
                        constructor = engine.Error;
                        break;
                    case Library.ErrorType.RangeError:
                        constructor = engine.RangeError;
                        break;
                    case Library.ErrorType.TypeError:
                        constructor = engine.TypeError;
                        break;
                    case Library.ErrorType.SyntaxError:
                        constructor = engine.SyntaxError;
                        break;
                    case Library.ErrorType.URIError:
                        constructor = engine.URIError;
                        break;
                    case Library.ErrorType.EvalError:
                        constructor = engine.EvalError;
                        break;
                    case Library.ErrorType.ReferenceError:
                        constructor = engine.ReferenceError;
                        break;
                    default:
                        throw new NotSupportedException($"Unrecognised error type {ErrorType.Value}.");
                }

                // Create an error instance.
                var result = constructor.Construct(ErrorMessage);
                result.SetStackTrace(this.SourcePath, this.FunctionName, this.LineNumber);
                ErrorObject = result;
            }
            return ErrorObject;
        }
    }
}
