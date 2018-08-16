using System;
using System.Globalization;


namespace Jurassic.Library
{
    /// <summary>
    /// Helper methods used from debugger visualization
    /// </summary>
    public static class DebuggerDisplayHelper
    {
        /// <summary>
        /// Converts any object to string, to be displayed in debugger watch window.
        /// This is the long version, used when the object is directly displayed (not in parent).
        /// </summary>
        /// <param name="value">The object to be converted</param>
        /// <returns>The result string</returns>
        public static string StringRepresentation(object value)
        {
            string result;
            if (value is ObjectInstance)
            {
                result = (value as ObjectInstance).DebuggerDisplayValue;
            }
            else if (value is string)
            {
                result = string.Format("\"{0}\"", value);
            }
            else
            {
                IFormattable formattable = value as IFormattable;
                if (formattable == null)
                {
                    result = value.ToString();
                }
                else
                {
                    result = formattable.ToString(null, CultureInfo.InvariantCulture);
                }
            }
            return result;
        }


        /// <summary>
        /// Converts any object to string, to be displayed in debugger watch window.
        /// This is the short version, used when the object is displayed as a child.
        /// </summary>
        /// <param name="value">The object to be converted</param>
        /// <returns>The result string</returns>
        public static string ShortStringRepresentation(object value)
        {
            string result;
            if (value is ObjectInstance)
            {
                result = (value as ObjectInstance).DebuggerDisplayShortValue;
            }
            else if (value is string)
            {
                result = string.Format("\"{0}\"", value);
            }
            else
            {
                IFormattable formattable = value as IFormattable;
                if (formattable == null)
                {
                    result = value.ToString();
                }
                else
                {
                    result = formattable.ToString(null, CultureInfo.InvariantCulture);
                }
            }
            return result;
        }

        /// <summary>
        /// Return decorated or actual Clr Type
        /// </summary>
        /// <param name="value">The object</param>
        /// <returns>The name of the type</returns>
        public static string TypeRepresentation(object value)
        {
            string result;
            if (value is ObjectInstance)
            {
                result = (value as ObjectInstance).DebuggerDisplayType;
            }
            else
            {
                result = value?.GetType().Name;
            }
            return result;

        }
    }
}
