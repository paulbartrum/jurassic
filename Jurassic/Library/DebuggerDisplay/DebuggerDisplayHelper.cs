using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Helper methods used from debugger visualization
    /// </summary>
    internal static class DebuggerDisplayHelper
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
            if (value is IDebuggerDisplay debuggerDisplay)
            {
                result = debuggerDisplay.DebuggerDisplayValue;
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
            if (value is IDebuggerDisplay debuggerDisplay)
            {
                result = debuggerDisplay.DebuggerDisplayShortValue;
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
            if (value is IDebuggerDisplay debuggerDisplay)
            {
                result = debuggerDisplay.DebuggerDisplayType;
            }
            else
            {
                result = value?.GetType().Name;
            }
            return result;

        }

        /// <summary>
        /// Converts map contents to string
        /// </summary>
        /// <param name="mapStore">Internal storage of a MapInstance</param>
        /// <returns>The result string</returns>
        public static string MapRepresentation(Dictionary<object, LinkedListNode<KeyValuePair<object, object>>> mapStore)
        {
            IEnumerable<string> strValues =
                mapStore.Values.Select(node =>
                    string.Format("{0} => {1}",
                        ShortStringRepresentation(node.Value.Key),
                        ShortStringRepresentation(node.Value.Value)));

            return string.Join(", ", strValues);
        }

        /// <summary>
        /// Gets the keys of a weak map using reflection
        /// </summary>
        /// <param name="weakMap">The WeakMapInstance</param>
        /// <returns>Keys</returns>
        public static IEnumerable<ObjectInstance> GetKeys(this ConditionalWeakTable<ObjectInstance, object> weakMap)
        {
            IEnumerable<ObjectInstance> keys = weakMap.GetType().InvokeMember(
                                        "Keys",
                                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                                        null,
                                        weakMap,
                                        new object[] { }) as IEnumerable<ObjectInstance>;
            return keys;
        }

        /// <summary>
        /// Converts WeakMapInstance to its string representation
        /// </summary>
        /// <param name="sb">StringBuilder to add strings</param>
        /// <param name="weakMapStore">Internal storage of a WeakMapInstance</param>
        public static void WeakMapRepresentation(StringBuilder sb,
            ConditionalWeakTable<ObjectInstance, object> weakMapStore)
        {
            bool comma = false;
            IEnumerable<ObjectInstance> keys = weakMapStore.GetKeys();
            foreach (ObjectInstance key in keys)
            {
                object value;
                if (weakMapStore.TryGetValue(key, out value))
                {
                    if (comma)
                    {
                        sb.Append(", ");
                    }
                    sb.AppendFormat("{0} => {1}",
                        DebuggerDisplayHelper.ShortStringRepresentation(key),
                        DebuggerDisplayHelper.ShortStringRepresentation(value));
                    comma = true;
                }
            }
        }
    }
}
