namespace Jurassic.Library
{
    /// <summary>
    /// Interface for objects supporting decoration of the debugger information.
    /// </summary>
    internal interface IDebuggerDisplay
    {
        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        string DebuggerDisplayValue { get; }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        string DebuggerDisplayShortValue { get; }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        string DebuggerDisplayType { get; }
    }
}
