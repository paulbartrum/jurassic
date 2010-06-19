using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the RegExp object.
    /// </summary>
    public class RegExpInstance : ObjectInstance
    {
        private Regex value;
        private bool globalSearch;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new regular expression instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="pattern"> The regular expression pattern. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        public RegExpInstance(ObjectInstance prototype, string pattern, string flags = null)
            : base(prototype)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");
            this.value = new Regex(pattern, ParseFlags(flags));

            // Add the javascript properties.
            this.SetProperty("source", pattern);
            this.SetProperty("global", this.Global);
            this.SetProperty("multiline", this.Multiline);
            this.SetProperty("ignoreCase", this.IgnoreCase);
            this.SetProperty("lastIndex", 0.0, PropertyAttributes.Writable);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "RegExp"; }
        }

        /// <summary>
        /// Gets the primitive value of this object.
        /// </summary>
        public Regex Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets a string that contains the flags.
        /// </summary>
        public string Flags
        {
            get
            {
                var result = new System.Text.StringBuilder(3);
                if (this.Global)
                    result.Append("g");
                if (this.IgnoreCase)
                    result.Append("i");
                if (this.Multiline)
                    result.Append("m");
                return result.ToString();
            }
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the regular expression pattern.
        /// </summary>
        public string Source
        {
            get { return this.value.ToString(); }
        }

        /// <summary>
        /// Gets a value that indicates whether the global flag is set.  If this flag is set it
        /// indicates that a search should find all occurrences of the pattern within the searched
        /// string, not just the first one.
        /// </summary>
        public bool Global
        {
            get { return this.globalSearch; }
        }

        /// <summary>
        /// Gets a value that indicates whether the multiline flag is set.  If this flag is set it
        /// indicates that the ^ and $ tokens should match the start and end of lines and not just
        /// the start and end of the string.
        /// </summary>
        public bool Multiline
        {
            get { return (this.value.Options & RegexOptions.Multiline) != 0;}
        }

        /// <summary>
        /// Gets a value that indicates whether the ignoreCase flag is set.  If this flag is set it
        /// indicates that a search should ignore differences in case between the pattern and the
        /// matched string.
        /// </summary>
        public bool IgnoreCase
        {
            get { return (this.value.Options & RegexOptions.IgnoreCase) != 0; }
        }

        /// <summary>
        /// Gets the character position to start searching when the global flag is set.
        /// </summary>
        public int LastIndex
        {
            get { return TypeConverter.ToInteger(this["lastIndex"]); }
            set { this["lastIndex"] = value; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Compiles the regular expression for faster execution.
        /// </summary>
        /// <param name="pattern"> The regular expression pattern. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        [JSFunction(Deprecated = true, Name = "compile")]
        public void Compile(string pattern, string flags = null)
        {
            this.value = new Regex(pattern, ParseFlags(flags) | RegexOptions.Compiled);

            // Update the javascript properties.
            this.SetProperty("source", pattern);
            this.SetProperty("global", this.Global);
            this.SetProperty("multiline", this.Multiline);
            this.SetProperty("ignoreCase", this.IgnoreCase);
            this.LastIndex = 0;
        }

        /// <summary>
        /// Returns a boolean value that indicates whether or not a pattern exists in a searched string.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <returns> <c>true</c> if the regular expression has at least one match in the given
        /// string; <c>false</c> otherwise. </returns>
        [JSFunction(Name = "test")]
        public bool Test(string input)
        {
            // Check if there is a match.
            var match = this.value.Match(input, CalculateStartPosition(input));
            
            // If the regex is global, update the lastIndex property.
            if (this.Global == true)
                this.LastIndex = match.Success == true ? match.Index + match.Length : 0;

            return match.Success;
        }

        /// <summary>
        /// Executes a search on a string using a regular expression pattern, and returns an array
        /// containing the results of that search.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <returns> Returns an array containing the match and submatch details, or <c>null</c> if
        /// no match was found.  The array returned by the exec method has three properties, input,
        /// index and lastIndex. The input property contains the entire searched string. The index
        /// property contains the position of the matched substring within the complete searched
        /// string. The lastIndex property contains the position following the last character in
        /// the match. </returns>
        [JSFunction(Name = "exec")]
        public object Exec(string input)
        {
            // Perform the regular expression matching.
            var match = this.value.Match(input, CalculateStartPosition(input));

            // Return null if no match was found.
            if (match.Success == false)
            {
                // Reset the lastIndex property.
                if (this.Global == true)
                    this.LastIndex = 0;
                return Null.Value;
            }

            // If the global flag is set, update the lastIndex property.
            if (this.Global == true)
                this.LastIndex = match.Index + match.Length;

            // Otherwise, return an array.
            object[] array = new object[match.Groups.Count];
            for (int i = 0; i < match.Groups.Count; i++)
            {
                array[i] = match.Groups[i].Value;
                if (match.Groups[i].Value == string.Empty)
                    array[i] = Undefined.Value;
            }
            var result = GlobalObject.Array.New(array);
            result["index"] = match.Index;
            result["input"] = input;
            return result;
        }

        /// <summary>
        /// Calculates the position to start searching.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <returns> The character position to start searching. </returns>
        private int CalculateStartPosition(string input)
        {
            if (this.Global == false)
                return 0;
            return Math.Min(Math.Max(this.LastIndex, 0), input.Length);
        }

        /// <summary>
        /// Finds all regular expression matches within the given string.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <returns> An array containing the matched strings. </returns>
        public object Match(string input)
        {
            // If the global flag is not set, returns a single match.
            if (this.Global == false)
                return Exec(input);

            // Otherwise, find all matches.
            var matches = this.value.Matches(input);
            if (matches.Count == 0)
                return Null.Value;

            // Construct the array to return.
            object[] matchValues = new object[matches.Count];
            for (int i = 0; i < matches.Count; i++)
                matchValues[i] = matches[i].Value;
            return GlobalObject.Array.New(matchValues);
        }

        /// <summary>
        /// Returns a copy of the given string with text replaced using a regular expression.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <param name="replaceText"> A string containing the text to replace for every successful match. </param>
        /// <returns> A copy of the given string with text replaced using a regular expression. </returns>
        public string Replace(string input, string replaceText)
        {
            if (this.Global == true)
                return this.value.Replace(input, replaceText);
            else
                return this.value.Replace(input, replaceText, 1);
        }

        /// <summary>
        /// Returns a copy of the given string with text replaced using a regular expression.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <param name="replaceFunction"> A function that is called to produce the text to replace
        /// for every successful match. </param>
        /// <returns> A copy of the given string with text replaced using a regular expression. </returns>
        public string Replace(string input, FunctionInstance replaceFunction)
        {
            return this.value.Replace(input, match =>
            {
                object[] parameters = new object[match.Groups.Count + 2];
                for (int i = 0; i < match.Groups.Count; i++)
                {
                    if (string.IsNullOrEmpty(match.Groups[i].Value) == true)
                        parameters[i] = Undefined.Value;
                    else
                        parameters[i] = match.Groups[i].Value;
                }
                parameters[match.Groups.Count] = match.Index;
                parameters[match.Groups.Count + 1] = input;
                return replaceFunction.CallLateBound(GlobalObject.Instance, parameters).ToString();
            });
        }

        /// <summary>
        /// Returns the position of the first substring match in a regular expression search.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <returns> The character position of the first match, or -1 if no match was found. </returns>
        public int Search(string input)
        {
            // Perform the regular expression matching.
            var match = this.value.Match(input);

            // Return -1 if no match was found.
            if (match.Success == false)
                return -1;

            // Otherwise, return the position of the match.
            return match.Index;
        }

        /// <summary>
        /// Splits the given string into an array of strings by separating the string into substrings.
        /// </summary>
        /// <param name="input"> The string to split. </param>
        /// <param name="limit"> The maximum number of array items to return.  Defaults to unlimited. </param>
        /// <returns> An array containing the split strings. </returns>
        public ArrayInstance Split(string input, int limit = int.MaxValue)
        {
            // Constrain limit to a positive number.
            limit = Math.Max(0, limit);
            if (limit == 0)
                return GlobalObject.Array.New(new object[0]);

            // Find the first match.
            Match match = this.value.Match(input, 0);

            var results = new List<object>();
            int startIndex = 0;
            while (match.Success == true)
            {
                // Do not match the an empty substring at the start or end of the string or at the
                // end of the previous match.
                if (match.Length == 0 && (match.Index == 0 || match.Index == input.Length || match.Index == startIndex))
                {
                    // Find the next match.
                    match = match.NextMatch();
                    continue;
                }

                // Add the match results to the array.
                results.Add(input.Substring(startIndex, match.Index - startIndex));
                startIndex = match.Index + match.Length;
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    var group = match.Groups[i];
                    if (group.Captures.Count == 0)
                        results.Add(Undefined.Value);       // Non-capturing groups return "undefined".
                    else
                        results.Add(match.Groups[i].Value);
                    if (results.Count >= limit)
                        return GlobalObject.Array.New(results.ToArray());
                }

                // Find the next match.
                match = match.NextMatch();
            }
            results.Add(input.Substring(startIndex, input.Length - startIndex));
            return GlobalObject.Array.New(results.ToArray());
        }

        /// <summary>
        /// Returns a string representing the current object.
        /// </summary>
        /// <returns> A string representing the current object. </returns>
        [JSFunction(Name = "toString")]
        public new string ToString()
        {
            return string.Format("/{0}/{1}", this.Source, this.Flags);
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Parses the flags parameter into an enum.
        /// </summary>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        /// <returns> RegexOptions flags that correspond to the given flags. </returns>
        private RegexOptions ParseFlags(string flags)
        {
            var options = RegexOptions.ECMAScript;
            this.globalSearch = false;
            if (string.IsNullOrEmpty(flags) == false)
            {
                if (flags.Contains('g'))
                    this.globalSearch = true;
                if (flags.Contains('i'))
                    options |= RegexOptions.IgnoreCase;
                if (flags.Contains('m'))
                    options |= RegexOptions.Multiline;
            }
            return options;
        }
    }
}
