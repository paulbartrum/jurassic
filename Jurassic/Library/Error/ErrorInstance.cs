using System;
using System.Collections.Generic;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the base class of all the javascript errors.
    /// </summary>
    [Serializable]
    public class ErrorInstance : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Error instance with the given name, message and optionally a stack trace.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The initial value of the name property.  Pass <c>null</c> to avoid
        /// creating this property. </param>
        /// <param name="message"> The initial value of the message property.  Pass <c>null</c> to
        /// avoid creating this property. </param>
        /// <param name="generateStack"> Indicates whether to generate a stack trace and attach it
        /// to this object. </param>
        internal ErrorInstance(ObjectInstance prototype, string name, string message, bool generateStack)
            : base(prototype)
        {
            if (name != null)
                this.FastSetProperty("name", name, PropertyAttributes.FullAccess);
            if (message != null)
                this.FastSetProperty("message", message, PropertyAttributes.FullAccess);

#if !SILVERLIGHT            
            if (generateStack == true && ScriptEngine.LowPrivilegeEnvironment == false)
            {
                //try
                //{
                    var stackTrace = string.Concat(this.ToStringJS(), Environment.NewLine, Environment.StackTrace);
                    this.FastSetProperty("stack", stackTrace, PropertyAttributes.FullAccess);
                //}
                //catch (System.Security.SecurityException)
                //{
                //    // Note: Environment.StackTrace requires EnvironmentPermission (unrestricted).
                //    ScriptEngine.SetLowPrivilegeEnvironment();
                //}
            }
#endif
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
        /// Gets the stack trace
        /// </summary>
        public string Stack
        {
            get { return TypeConverter.ToString(this["stack"]); }
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
