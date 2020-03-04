using Jurassic.Library;

namespace Jurassic.Extensions
{
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
            engine.Global.SetPropertyValue("console", new FirebugConsole.FirebugConsole(engine), throwOnError: true);
        }

        /// <summary>
        /// Adds 'fetch' and related classes to the global namespace.
        /// </summary>
        /// <param name="engine"> The script engine to modify. </param>
        /// <param name="options"> Custom </param>
        public static void AddFetch(this ScriptEngine engine, Fetch.FetchOptions options = null)
        {
            Fetch.FetchImplementation.Add(engine, options);
            engine.Global.DefineProperty("Headers", new PropertyDescriptor(new Fetch.HeadersConstructor(engine.Function.InstancePrototype), PropertyAttributes.NonEnumerable), throwOnError: true);
            engine.Global.DefineProperty("Request", new PropertyDescriptor(new Fetch.RequestConstructor(engine.Function.InstancePrototype), PropertyAttributes.NonEnumerable), throwOnError: true);
            engine.Global.DefineProperty("Response", new PropertyDescriptor(new Fetch.ResponseConstructor(engine.Function.InstancePrototype), PropertyAttributes.NonEnumerable), throwOnError: true);
        }

        /// <summary>
        /// Adds the 'setTimeout', 'setInterval', 'clearTimeout' and 'clearInterval' functions to
        /// the global object.
        /// </summary>
        /// <param name="engine"> The script engine to modify. </param>
        public static void AddSimpleTiming(this ScriptEngine engine)
        {
            SimpleTiming.SimpleTimingImplementation.Add(engine);
        }
    }
}
