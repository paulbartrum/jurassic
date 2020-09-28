using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the base class of all the javascript errors.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public partial class ErrorInstance : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

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

        /// <summary>
        /// Creates the Error prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        /// <param name="type"> The type of error, e.g. Error, RangeError, etc. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, ErrorConstructor constructor, ErrorType type)
        {
            var result = CreateRawObject(GetPrototype(engine, type));
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue("name", type.ToString(), PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue("message", string.Empty, PropertyAttributes.NonEnumerable));
            result.InitializeProperties(properties);
            return result;
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



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

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
        /// <param name="engine"> The current script environment. </param>
        /// <param name="thisObj"> The object that is being operated on. </param>
        /// <returns> A string representing the current object. </returns>
        [JSInternalFunction(Name = "toString", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static string ToString(ScriptEngine engine, object thisObj)
        {
            if (!(thisObj is ObjectInstance))
                throw new JavaScriptException(ErrorType.TypeError, "this is not an object.");

            // Get the relevant properties.
            var obj = (ObjectInstance)thisObj;
            var nameObj = obj["name"];
            var messageObj = obj["message"];

            // Convert them to strings.
            var name = TypeUtilities.IsUndefined(nameObj) ? "Error" : TypeConverter.ToString(nameObj);
            var message = TypeUtilities.IsUndefined(messageObj) ? "" : TypeConverter.ToString(messageObj);

            // Concatenate them.
            if (string.IsNullOrEmpty(name))
                return message;
            if (string.IsNullOrEmpty(message))
                return name;
            return string.Format("{0}: {1}", name, message);
        }

        /// <summary>
        /// Gets an enumerable list of every property name and value associated with this object.
        /// Does not include properties in the prototype chain.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get { return this.Message; }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayValue; }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayType
        {
            get { return "Error"; }
        }
    }
}
