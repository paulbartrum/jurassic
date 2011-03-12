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
        /// Called when the RegExp object is invoked like a function e.g. RegExp('abc', 'g') or
        /// RegExp(/abc/).  If a string is passed as the first parameter it creates a new regular
        /// expression instance.  Otherwise, if a regular expression is passed it returns the given
        /// regular expression verbatim.
        /// </summary>
        /// <param name="patternOrRegExp"> A regular expression pattern or a regular expression. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        [JSCallFunction]
        public RegExpInstance Call(object patternOrRegExp, string flags = null)
        {
            if (patternOrRegExp is RegExpInstance)
            {
                // RegExp(/abc/)
                if (flags != null)
                    throw new JavaScriptException(this.Engine, "TypeError", "Cannot supply flags when constructing one RegExp from another");
                return (RegExpInstance)patternOrRegExp;
            }
            else
            {
                // RegExp('abc', 'g')
                var pattern = string.Empty;
                if (TypeUtilities.IsUndefined(patternOrRegExp) == false)
                    pattern = TypeConverter.ToString(patternOrRegExp);
                return new RegExpInstance(this.InstancePrototype, pattern, flags);
            }
        }

        /// <summary>
        /// Called when the new keyword is used on the RegExp object e.g. new RegExp(/abc/).
        /// Creates a new regular expression instance.
        /// </summary>
        /// <param name="patternOrRegExp"> The regular expression pattern, or a regular expression
        /// to clone. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        [JSConstructorFunction]
        public RegExpInstance Construct(object patternOrRegExp, string flags = null)
        {
            if (patternOrRegExp is RegExpInstance)
            {
                // new RegExp(regExp, flags)
                if (flags != null)
                    throw new JavaScriptException(this.Engine, "TypeError", "Cannot supply flags when constructing one RegExp from another");
                return new RegExpInstance(this.InstancePrototype, (RegExpInstance)patternOrRegExp);
            }
            else
            {
                // new RegExp(pattern, flags)
                var pattern = string.Empty;
                if (TypeUtilities.IsUndefined(patternOrRegExp) == false)
                    pattern = TypeConverter.ToString(patternOrRegExp);
                return new RegExpInstance(this.InstancePrototype, pattern, flags);
            }
        }
    }
}
