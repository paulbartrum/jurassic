using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript RegExp object.
    /// </summary>
    [Serializable]
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
            // Set the deprecated properties to their default values.
            for (int i = 0; i < 9; i++)
                this.FastSetProperty("$" + (char)('1' + i), string.Empty, PropertyAttributes.Enumerable);
            this.FastSetProperty("input",           string.Empty, PropertyAttributes.Enumerable);
            this.FastSetProperty("$_",              string.Empty, PropertyAttributes.Sealed);
            this.FastSetProperty("lastMatch",       string.Empty, PropertyAttributes.Enumerable);
            this.FastSetProperty("$&",              string.Empty, PropertyAttributes.Sealed);
            this.FastSetProperty("lastParen",       string.Empty, PropertyAttributes.Enumerable);
            this.FastSetProperty("$+",              string.Empty, PropertyAttributes.Sealed);
            this.FastSetProperty("leftContext",     string.Empty, PropertyAttributes.Enumerable);
            this.FastSetProperty("$`",              string.Empty, PropertyAttributes.Sealed);
            this.FastSetProperty("rightContext",    string.Empty, PropertyAttributes.Enumerable);
            this.FastSetProperty("$'",              string.Empty, PropertyAttributes.Sealed);
        }

        // Deprecated properties:
        // $1, ..., $9          Parenthesized substring matches, if any.
        // input ($_)           The string against which a regular expression is matched.
        // lastMatch ($&)       The last matched characters.
        // lastParen ($+)       The last parenthesized substring match, if any.
        // leftContext ($`)     The substring preceding the most recent match.
        // rightContext ($')    The substring following the most recent match.

        /// <summary>
        /// Sets the deprecated RegExp properties.
        /// </summary>
        /// <param name="input"> The string against which a regular expression is matched. </param>
        /// <param name="match"> The regular expression match to base the properties on. </param>
        internal void SetDeprecatedProperties(string input, System.Text.RegularExpressions.Match match)
        {
            // Set the input property.
            this.FastSetProperty("input", input);
            this.FastSetProperty("$_", input);

            // Set $1 to $9.
            for (int i = 1; i <= 9; i++)
            {
                if (i < match.Groups.Count)
                    this.FastSetProperty("$" + (char)('0' + i), match.Groups[i].Value);
                else
                    this.FastSetProperty("$" + (char)('0' + i), string.Empty);
            }

            // Set the lastMatch property.
            this.FastSetProperty("lastMatch", match.Value);
            this.FastSetProperty("$&", match.Value);

            // Set the lastParen property.
            var lastParen = match.Groups.Count > 1 ? match.Groups[match.Groups.Count - 1].Value : string.Empty;
            this.FastSetProperty("lastParen", lastParen);
            this.FastSetProperty("$+", lastParen);

            // Set the leftContext property.
            var leftContext = input.Substring(0, match.Index);
            this.FastSetProperty("leftContext", leftContext);
            this.FastSetProperty("$`", leftContext);

            // Set the rightContext property.
            var rightContext = input.Substring(match.Index + match.Length);
            this.FastSetProperty("rightContext", rightContext);
            this.FastSetProperty("$'", rightContext);
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
