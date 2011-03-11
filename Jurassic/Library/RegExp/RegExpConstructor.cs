using System;
using System.Collections.Generic;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript RegExp object.
    /// </summary>
    public class RegExpConstructor : ClrFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new RegExp object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal RegExpConstructor(ObjectInstance prototype)
            : base(prototype, "RegExp", new RegExpInstance(prototype.Engine.Object.InstancePrototype, string.Empty))
        {
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the RegExp object is invoked like a function e.g. RegExp('abc', 'g').
        /// Creates a new regular expression instance.
        /// </summary>
        /// <param name="pattern"> The regular expression pattern. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        [JSCallFunction(Flags = FunctionBinderFlags.Preferred)]
        public RegExpInstance Call(string pattern, string flags = null)
        {
            return new RegExpInstance(this.InstancePrototype, pattern, flags);
        }

        /// <summary>
        /// Called when the RegExp object is invoked like a function e.g. RegExp(/abc/).
        /// Returns the given regular expression verbatim.
        /// </summary>
        /// <param name="regExp"> The regular expression. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        [JSCallFunction]
        public RegExpInstance Call(RegExpInstance regExp, string flags = null)
        {
            if (flags != null)
                throw new JavaScriptException(this.Engine, "TypeError", "Cannot supply flags when constructing one RegExp from another");
            return regExp;
        }

        /// <summary>
        /// Called when the new keyword is used on the RegExp object e.g. new RegExp(/abc/).
        /// Creates a new regular expression instance.
        /// </summary>
        /// <param name="pattern"> The regular expression pattern. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        [JSConstructorFunction(Flags = FunctionBinderFlags.Preferred)]
        public RegExpInstance Construct(string pattern, string flags = null)
        {
            return new RegExpInstance(this.InstancePrototype, pattern, flags);
        }

        /// <summary>
        /// Called when the new keyword is used on the RegExp object e.g. new RegExp(/abc/).
        /// Returns a new regular expression with the same pattern and flags as the given regular
        /// expression.
        /// </summary>
        /// <param name="regExp"> The regular expression to clone. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        [JSConstructorFunction]
        public RegExpInstance Construct(RegExpInstance regExp, string flags = null)
        {
            if (flags != null)
                throw new JavaScriptException(this.Engine, "TypeError", "Cannot supply flags when constructing one RegExp from another");
            return new RegExpInstance(this.InstancePrototype, regExp);
        }
    }
}
