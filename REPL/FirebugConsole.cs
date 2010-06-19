using System;
using System.Diagnostics;
using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace REPL
{
    public class FirebugConsole : ObjectInstance
    {
        private int indentationLevel = 2;
        private Dictionary<string, Stopwatch> timers;

        /// <summary>
        /// Creates a new FirebugConsole instance.
        /// </summary>
        /// <param name="initialIndent"> The initial number of spaces to indent. </param>
        public FirebugConsole(int initialIndent = 0)
            : base(GlobalObject.Object.InstancePrototype)
        {
            this.indentationLevel = initialIndent;
            this.PopulateFunctions();
        }



        //     API METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Logs a message to the console.  The objects provided will be converted to strings then
        /// joined together in a space separated line.  The first parameter can be a string
        /// containing the following patterns:
        ///  %s	 String
        ///  %d, %i	 Integer (numeric formatting is not yet supported)
        ///  %f	 Floating point number (numeric formatting is not yet supported)
        ///  %o	 Object hyperlink (not yet supported)
        /// </summary>
        /// <param name="items"> The items to format. </param>
        [JSFunction(Name = "log")]
        public void log(params object[] items)
        {
            WriteLine(Format(items));
        }

        /// <summary>
        /// Logs a message to the console.  The objects provided will be converted to strings then
        /// joined together in a space separated line.  The first parameter can be a string
        /// containing the following patterns:
        ///  %s	 String
        ///  %d, %i	 Integer (numeric formatting is not yet supported)
        ///  %f	 Floating point number (numeric formatting is not yet supported)
        ///  %o	 Object hyperlink (not yet supported)
        /// </summary>
        /// <param name="items"> The items to format. </param>
        [JSFunction(Name = "debug")]
        public void debug(params object[] items)
        {
            WriteLine(Format(items));
        }

        /// <summary>
        /// Logs a message to the console using a style suggesting informational content. The
        /// objects provided will be converted to strings then joined together in a space separated
        /// line.  The first parameter can be a string containing the following patterns:
        ///  %s	 String
        ///  %d, %i	 Integer (numeric formatting is not yet supported)
        ///  %f	 Floating point number (numeric formatting is not yet supported)
        ///  %o	 Object hyperlink (not yet supported)
        /// </summary>
        /// <param name="items"> The items to format. </param>
        [JSFunction(Name = "info")]
        public void info(params object[] items)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            WriteLine(Format(items));
            Console.ForegroundColor = original;
        }

        /// <summary>
        /// Logs a message to the console using a style suggesting a warning.  The objects provided
        /// will be converted to strings then joined together in a space separated line.  The first
        /// parameter can be a string containing the following patterns:
        ///  %s	 String
        ///  %d, %i	 Integer (numeric formatting is not yet supported)
        ///  %f	 Floating point number (numeric formatting is not yet supported)
        ///  %o	 Object hyperlink (not yet supported)
        /// </summary>
        /// <param name="items"> The items to format. </param>
        [JSFunction(Name = "warn")]
        public void warn(params object[] items)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteLine(Format(items));
            Console.ForegroundColor = original;
        }

        /// <summary>
        /// Logs a message to the console using a style suggesting an error.  The objects provided
        /// will be converted to strings then joined together in a space separated line.  The
        /// first parameter can be a string containing the following patterns:
        ///  %s	 String
        ///  %d, %i	 Integer (numeric formatting is not yet supported)
        ///  %f	 Floating point number (numeric formatting is not yet supported)
        ///  %o	 Object hyperlink (not yet supported)
        /// </summary>
        /// <param name="items"> The items to format. </param>
        [JSFunction(Name = "error")]
        public void error(params object[] items)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(Format(items));
            Console.ForegroundColor = original;
        }

        /// <summary>
        /// Tests that an expression is true. If not, it will write a message to the console and =
        /// throw an exception.
        /// </summary>
        /// <param name="expression"> The expression to test. </param>
        /// <param name="items"> The items to format. </param>
        [JSFunction(Name = "assert")]
        public void assert(bool expression, params object[] items)
        {
            if (expression == false)
            {
                if (items.Length > 0)
                    throw new JavaScriptException(string.Format("Assertion failed: {0}", Format(items)), -1, null);
                else
                    throw new JavaScriptException("Assertion failed", -1, null);
            }
        }

        /// <summary>
        /// Writes a message to the console and opens a nested block to indent all future messages
        /// sent to the console.  Call console.groupEnd() to close the block.
        /// </summary>
        /// <param name="items"> The items to format. </param>
        [JSFunction(Name = "group")]
        public void group(params object[] items)
        {
            WriteLine(Format(items));
            this.indentationLevel = Math.Min(this.indentationLevel + 4, 40);
        }

        /// <summary>
        /// Closes the most recently opened block created by a call to console.group().
        /// </summary>
        [JSFunction(Name = "groupEnd")]
        public void groupEnd()
        {
            this.indentationLevel = Math.Max(this.indentationLevel - 4, 0);
        }

        /// <summary>
        /// Creates a new timer under the given name. Call console.timeEnd(name) with the same name
        /// to stop the timer and print the time elapsed.
        /// </summary>
        /// <param name="name"> The name of the time to create. </param>
        [JSFunction(Name = "time")]
        public void time(string name = "")
        {
            if (name == null)
                name = string.Empty;
            if (this.timers == null)
                this.timers = new Dictionary<string, Stopwatch>();
            if (this.timers.ContainsKey(name) == true)
                return;
            this.timers.Add(name, Stopwatch.StartNew());
        }

        /// <summary>
        /// Stops a timer created by a call to console.time(name) and writes the time elapsed.
        /// </summary>
        /// <param name="name"> The name of the timer to stop. </param>
        [JSFunction(Name = "timeEnd")]
        public void timeEnd(string name = "")
        {
            if (name == null)
                name = string.Empty;
            if (this.timers == null || this.timers.ContainsKey(name) == false)
                return;
            var stopwatch = this.timers[name];
            if (string.IsNullOrEmpty(name))
                WriteLine(string.Format("{0}ms", stopwatch.ElapsedMilliseconds));
            else
                WriteLine(string.Format("{0}: {1}ms", name, stopwatch.ElapsedMilliseconds));
            this.timers.Remove(name);
        }



        //     PRIVATE METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Formats a message.  The objects provided will be converted to strings then
        /// joined together in a space separated line.  The first parameter can be a string
        /// containing the following patterns:
        ///  %s	 String
        ///  %d, %i	 Integer (numeric formatting is not yet supported)
        ///  %f	 Floating point number (numeric formatting is not yet supported)
        ///  %o	 Object hyperlink (not yet supported)
        /// </summary>
        /// <param name="items"> The items to format. </param>
        /// <returns> A formatted string. </returns>
        private static string Format(object[] items)
        {
            if (items.Length == 0)
                return string.Empty;
            var result = new System.Text.StringBuilder();

            // If the first item is a string, then it is assumed to be a format string.
            int itemsConsumed = 1;
            if (items[0] is string)
            {
                string formatString = (string)items[0];

                int previousPatternIndex = 0, patternIndex;
                while (items.Length > itemsConsumed)
                {
                    // Find a percent sign.
                    patternIndex = formatString.IndexOf('%', previousPatternIndex);
                    if (patternIndex == -1 || patternIndex == formatString.Length - 1)
                        break;

                    // Append the text that didn't contain a pattern to the result.
                    result.Append(formatString, previousPatternIndex, patternIndex - previousPatternIndex);

                    // Extract the pattern type.
                    char patternType = formatString[patternIndex + 1];

                    // Determine the replacement string.
                    string replacement;
                    switch (patternType)
                    {
                        case 's':
                            replacement = TypeConverter.ToString(items[itemsConsumed++]);
                            break;
                        case 'd':
                        case 'i':
                            replacement = Math.Truncate(TypeConverter.ToNumber(items[itemsConsumed++])).ToString();
                            break;
                        case 'f':
                            replacement = TypeConverter.ToNumber(items[itemsConsumed++]).ToString();
                            break;
                        case '%':
                            replacement = "%";
                            break;
                        default:
                            replacement = "%" + patternType;
                            break;
                    }

                    // Replace the pattern with the corresponding argument.
                    result.Append(replacement);

                    // Start searching just after the end of the pattern.
                    previousPatternIndex = patternIndex + 2;
                }

                // Append the text that didn't contain a pattern to the result.
                result.Append(formatString, previousPatternIndex, formatString.Length - previousPatternIndex);
            }
            else
            {
                // The first item is not a string - just append it to the result.
                result.Append(TypeConverter.ToString(items[0]));
            }

            // Append the items that weren't consumed to the end of the string, separated by spaces.
            for (int i = itemsConsumed; i < items.Length; i++)
            {
                result.Append(' ');
                result.Append(TypeConverter.ToString(items[i]));
            }
            return result.ToString();
        }

        /// <summary>
        /// Writes a string to the console.
        /// </summary>
        /// <param name="message"> The message to write to the console. </param>
        private void WriteLine(string message)
        {
            if (this.indentationLevel > 0)
                System.Console.Write(new string(' ', this.indentationLevel));
            System.Console.WriteLine(message);
        }
    }
}
