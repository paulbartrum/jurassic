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
            engine.Global.SetPropertyValue("console", new FirebugConsole(engine), throwOnError: true);
        }

        /// <summary>
        /// Adds 'fetch' and related classes to the global namespace.
        /// </summary>
        /// <param name="engine"> The script engine to modify. </param>
        public static void AddFetch(this ScriptEngine engine)
        {
            Fetch.FetchImplementation.Add(engine);
            engine.Global.SetPropertyValue("Headers", new Fetch.HeadersConstructor(engine.Function.InstancePrototype), throwOnError: true);
            engine.Global.SetPropertyValue("Request", new Fetch.RequestConstructor(engine.Function.InstancePrototype), throwOnError: true);
            engine.Global.SetPropertyValue("Response", new Fetch.ResponseConstructor(engine.Function.InstancePrototype), throwOnError: true);
        }
    }
}
