using Jurassic.Library;
using System;

namespace Jurassic.Extensions
{
    /// <summary>
    /// Options that control various aspects of the fetch() API.
    /// </summary>
    public class FetchOptions
    {
        /// <summary>
        /// The URI to use as a base URI if any relative URIs are passed to the fetch API (or
        /// related classes). This affects request URIs as well as referrers. The default is
        /// <c>null</c>, which prohibits relative URIs.
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// The User-Agent header value to use when sending requests. The default is <c>null</c>,
        /// which does not send a User-Agent header.
        /// </summary>
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// Extension methods that add non-standard functionality to a ScriptEngine.
    /// </summary>
    public static class ScriptEngineExtensions
    {
        /// <summary>
        /// Adds 'console' to the global namespace.
        /// </summary>
        /// <param name="engine"> The script engine to modify. </param>
        public static void AddFirebugConsole(this ScriptEngine engine)
        {
            engine.Global.SetPropertyValue("console", new FirebugConsole(engine), throwOnError: true);
        }

        /// <summary>
        /// Adds 'fetch' and related classes to the global namespace.
        /// </summary>
        /// <param name="engine"> The script engine to modify. </param>
        /// <param name="options"> Custom </param>
        public static void AddFetch(this ScriptEngine engine, FetchOptions options = null)
        {
            Fetch.FetchImplementation.Add(engine, options);
            engine.Global.DefineProperty("Headers", new PropertyDescriptor(new Fetch.HeadersConstructor(engine.Function.InstancePrototype), PropertyAttributes.NonEnumerable), throwOnError: true);
            engine.Global.DefineProperty("Request", new PropertyDescriptor(new Fetch.RequestConstructor(engine.Function.InstancePrototype), PropertyAttributes.NonEnumerable), throwOnError: true);
            engine.Global.DefineProperty("Response", new PropertyDescriptor(new Fetch.ResponseConstructor(engine.Function.InstancePrototype), PropertyAttributes.NonEnumerable), throwOnError: true);
        }
    }
}
