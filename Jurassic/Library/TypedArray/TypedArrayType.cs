namespace Jurassic.Library
{
    /// <summary>
    /// Defines the element type and behaviour of typed array.
    /// </summary>
    public enum TypedArrayType
    {
        /// <summary>
        /// An array of signed 8-bit elements.
        /// </summary>
        Int8Array,

        /// <summary>
        /// An array of unsigned 8-bit elements.
        /// </summary>
        Uint8Array,

        /// <summary>
        /// An array of unsigned 8-bit elements, clamped to 0-255.
        /// </summary>
        Uint8ClampedArray,

        /// <summary>
        /// An array of signed 16-bit elements.
        /// </summary>
        Int16Array,

        /// <summary>
        /// An array of unsigned 16-bit elements.
        /// </summary>
        Uint16Array,

        /// <summary>
        /// An array of signed 32-bit elements.
        /// </summary>
        Int32Array,

        /// <summary>
        /// An array of unsigned 32-bit elements.
        /// </summary>
        Uint32Array,

        /// <summary>
        /// An array of 32-bit floating point elements.
        /// </summary>
        Float32Array,

        /// <summary>
        /// An array of 64-bit floating point elements.
        /// </summary>
        Float64Array,
    }
}
