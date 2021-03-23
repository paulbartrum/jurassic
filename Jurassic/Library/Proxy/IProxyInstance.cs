namespace Jurassic.Library
{
    /// <summary>
    /// Represents a proxy instance.
    /// </summary>
    interface IProxyInstance
    {
        /// <summary>
        /// The proxy target.
        /// </summary>
        ObjectInstance Target { get; }
    }
}
