using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="engine"> The script engine used to create the error object. </param>
        /// <param name="name"> The name of the error, e.g "RangeError". </param>
        /// <param name="message"> A description of the error. </param>
        public JavaScriptException(ScriptEngine engine, string name, string message)
            : base(string.Format("{0}: {1}", name, message))
        {
            this.ErrorObject = CreateError(engine, name, message);
            this.LineNumber = -1;
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="name"> The name of the error, e.g "RangeError". </param>
        /// <param name="message"> A description of the error. </param>
        /// <param name="innerException"> The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified. </param>
        public JavaScriptException(ScriptEngine engine, string name, string message, Exception innerException)
            : base(string.Format("{0}: {1}", name, message), innerException)
        {
            this.ErrorObject = CreateError(engine, name, message);
            this.LineNumber = -1;
        }

        /// <summary>
        /// Creates a new JavaScriptException instance.
        /// </summary>
        /// <param name="name"> The name of the error, e.g "RangeError". </param>
        /// <param name="message"> A description of the error. </param>
        /// <param name="lineNumber"> The line number in the source file the error occurred on. </param>
        /// <param name="sourcePath"> The path or URL of the source file.  Can be <c>null</c>. </param>
        public JavaScriptException(ScriptEngine engine, string name, string message, int lineNumber, string sourcePath)
            : base(string.Format("{0}: {1}", name, message))
        {
            this.ErrorObject = CreateError(engine, name, message);
            this.LineNumber = lineNumber;
            this.SourcePath = sourcePath;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets a reference to the JavaScript Error object.
        /// </summary>
        public object ErrorObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of error, e.g. "TypeError" or "SyntaxError".
        /// </summary>
        public string Name
        {
            get
            {
                if (this.ErrorObject is Library.ErrorInstance)
                    return ((Library.ErrorInstance)this.ErrorObject).Name;
                return null;
            }
        }

        /// <summary>
        /// Gets the line number in the source file the error occurred on.
        /// </summary>
        public int LineNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the path or URL of the source file.  Can be <c>null</c>.
        /// </summary>
        public string SourcePath
        {
            get;
            private set;
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates an error object with the given message.
        /// </summary>
        /// <param name="engine"> The script engine used to create the error object. </param>
        /// <param name="name"> The name of the error, e.g "RangeError". </param>
        /// <param name="message"> A description of the error. </param>
        /// <returns> A new Error instance. </returns>
        private static Library.ErrorInstance CreateError(ScriptEngine engine, string name, string message)
        {
            if (engine == null)
                throw new ArgumentNullException("engine");
            var errorPropertyInfo = typeof(ScriptEngine).GetProperty(name);
            if (errorPropertyInfo == null)
                throw new ArgumentException(string.Format("No error named '{0}' could be found.", name), "name");
            var errorConstructor = (Library.FunctionInstance)errorPropertyInfo.GetValue(engine, null);
            return (Library.ErrorInstance)errorConstructor.ConstructLateBound(message);
        }
    }
}
