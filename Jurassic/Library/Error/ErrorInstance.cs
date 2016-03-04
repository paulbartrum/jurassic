using System;
using System.Collections.Generic;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the base class of all the javascript errors.
    /// </summary>
    [Serializable]
    public partial class ErrorInstance : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates an empty Error instance for use as a prototype.
        /// </summary>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        internal ErrorInstance(ErrorConstructor constructor, ErrorType type)
            : base(GetPrototype(constructor.Engine, type))
        {
            // Initialize the prototype properties.
            var properties = GetDeclarativeProperties(Engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue("name", type.ToString(), PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue("message", string.Empty, PropertyAttributes.NonEnumerable));
            FastSetProperties(properties);
        }

        /// <summary>
        /// Determine the prototype for the given error type.
        /// </summary>
        /// <param name="engine"> The script engine associated with this object. </param>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        /// <returns> The prototype. </returns>
        private static ObjectInstance GetPrototype(ScriptEngine engine, ErrorType type)
        {
            if (type == ErrorType.Error)
            {
                // This constructor is for regular Error objects.
                // Prototype chain: Error instance -> Error prototype -> Object prototype
                return engine.Object.InstancePrototype;
            }
            else
            {
                // This constructor is for derived Error objects like RangeError, etc.
                // Prototype chain: XXXError instance -> XXXError prototype -> Error prototype -> Object prototype
                return engine.Error.InstancePrototype;
            }
        }

        /// <summary>
        /// Creates a new Error instance with the given message.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="message"> The initial value of the message property.  Pass <c>null</c> to
        /// avoid creating this property. </param>
        internal ErrorInstance(ObjectInstance prototype, string message)
            : base(prototype)
        {
            FastSetProperty("message", message, PropertyAttributes.NonEnumerable);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "Error"; }
        }

        /// <summary>
        /// Gets the name for the type of error.
        /// </summary>
        public string Name
        {
            get { return TypeConverter.ToString(this["name"]); }
        }

        /// <summary>
        /// Gets a human-readable description of the error.
        /// </summary>
        public string Message
        {
            get { return TypeConverter.ToString(this["message"]); }
        }

        /// <summary>
        /// Gets the stack trace.  Note that this is populated when the object is thrown, NOT when
        /// it is initialized.
        /// </summary>
        public string Stack
        {
            get { return TypeConverter.ToString(this["stack"]); }
        }

        /// <summary>
        /// Sets the stack trace information.
        /// </summary>
        /// <param name="path"> The path of the javascript source file that is currently executing. </param>
        /// <param name="function"> The name of the currently executing function. </param>
        /// <param name="line"> The line number of the statement that is currently executing. </param>
        internal void SetStackTrace(string path, string function, int line)
        {
            var stackTrace = this.Engine.FormatStackTrace(this.Name, this.Message, path, function, line);
            this.FastSetProperty("stack", stackTrace, PropertyAttributes.FullAccess);
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a string representing the current object.
        /// </summary>
        /// <returns> A string representing the current object. </returns>
        [JSInternalFunction(Name = "toString")]
        public string ToStringJS()
        {
            if (string.IsNullOrEmpty(this.Message))
                return this.Name;
            else if (string.IsNullOrEmpty(this.Name))
                return this.Message;
            else
                return string.Format("{0}: {1}", this.Name, this.Message);
        }
    }
}
