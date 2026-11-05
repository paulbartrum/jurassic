using System;

namespace Jurassic.Library
{
    /// <summary>
    /// The getter and setter for the "caller" and "arguments" properties on Function.prototype.
    /// Accessing these properties on an arrow function is an error.  For other functions the
    /// properties behave as if they did not exist.
    /// </summary>
    internal sealed class RestrictedPropertyAccessor : FunctionInstance
    {
        internal RestrictedPropertyAccessor(ObjectInstance prototype)
            : base(prototype)
        {
            this.FastSetProperty("name", "", PropertyAttributes.Configurable);
            this.FastSetProperty("length", 0, PropertyAttributes.Configurable);
        }

        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            if (thisObject is UserDefinedFunction function && function.IsArrow)
                throw new JavaScriptException(ErrorType.TypeError, "'caller' and 'arguments' cannot be accessed on arrow functions");
            return Undefined.Value;
        }
    }
}
