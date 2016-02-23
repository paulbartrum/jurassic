using System;

namespace Jurassic.Library
{
    /// <summary>
    /// Indicates the level of severity.
    /// </summary>
    public enum FirebugConsoleMessageStyle
    {
        /// <summary>
        /// Log text, without any indication of the severity.
        /// </summary>
        Regular,

        /// <summary>
        /// Log informational text.
        /// </summary>
        Information,

        /// <summary>
        /// Log warnings.
        /// </summary>
        Warning,

        /// <summary>
        /// Log errors.
        /// </summary>
        Error,
    }

    /// <summary>
    /// Represents the target of any Firebug console commands.
    /// </summary>
    public interface IFirebugConsoleOutput
    {
        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="style"> The style of the message (this determines the icon and text
        /// color). </param>
        /// <param name="objects"> The objects to output to the console. These can be strings or
        /// ObjectInstances. </param>
        void Log(FirebugConsoleMessageStyle style, object[] objects);

        /// <summary>
        /// Clears the console.
        /// </summary>
        void Clear();

        /// <summary>
        /// Starts grouping messages together.
        /// </summary>
        /// <param name="title"> The title for the group. </param>
        /// <param name="initiallyCollapsed"> <c>true</c> if subsequent messages should be hidden by default. </param>
        void StartGroup(string title, bool initiallyCollapsed);

        /// <summary>
        /// Ends the most recently started group.
        /// </summary>
        void EndGroup();
    }
}
