using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class FetchImplementation
    {
        private static ConditionalWeakTable<ScriptEngine, FetchOptions> engineOptions = new ConditionalWeakTable<ScriptEngine, FetchOptions>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="options"></param>
        internal static void Add(ScriptEngine engine, FetchOptions options)
        {
            if (engineOptions.TryGetValue(engine, out var existingOptions))
                throw new InvalidOperationException("Please call this method only once per script engine.");
            foreach (var property in GetDeclarativeProperties(engine))
                engine.Global.DefineProperty(property.Key, new PropertyDescriptor(property.Value, property.Attributes), throwOnError: true);
            engineOptions.Add(engine, options ?? new FetchOptions());
        }

        /// <summary>
        /// Starts the process of fetching a resource from the network, returning a promise which
        /// is fulfilled once the response is available. The promise resolves to the Response
        /// object representing the response to your request. The promise does not reject on HTTP
        /// errors — it only rejects on network errors. You must use then handlers to check for
        /// HTTP errors.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="resource"> This defines the resource that you wish to fetch. This can
        /// either be a URI string or a Request object.  </param>
        /// <param name="init"> An object containing any custom settings that you want to apply to
        /// the request. </param>
        /// <returns> A Promise that resolves to a Response object. </returns>
        [JSInternalFunction(Name = "fetch", Flags = JSFunctionFlags.HasEngineParameter, IsEnumerable = true)]
        public static PromiseInstance Fetch(ScriptEngine engine, object resource, ObjectInstance init = null)
        {
            var requestInstance = new RequestInstance(GetRequestConstructor(engine).InstancePrototype, resource, init);

            var method = HttpMethod.Get;
            switch (requestInstance.Method)
            {
                case "DELETE":
                    method = HttpMethod.Delete;
                    break;
                case "HEAD":
                    method = HttpMethod.Head;
                    break;
                case "OPTIONS":
                    method = HttpMethod.Options;
                    break;
                case "POST":
                    method = HttpMethod.Post;
                    break;
                case "PUT":
                    method = HttpMethod.Put;
                    break;
                case "TRACE":
                    method = HttpMethod.Trace;
                    break;
            }

            var request = new HttpRequestMessage(method, requestInstance.Url);
            foreach (var keyValuePair in request.Headers)
                request.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);

            var client = new HttpClient();
            var task = client.SendAsync(request);
            //task = task.ContinueWith(t => 5);
            return engine.Promise.FromTask(task);
        }

        internal static Uri ConvertToAbsoluteUri(ScriptEngine engine, string uri)
        {
            var fetchOptions = GetFetchOptions(engine);
            if (fetchOptions.BaseUri != null)
            {
                // A base URI exists, relative URIs are allowed.
                if (Uri.TryCreate(fetchOptions.BaseUri, uri, out Uri result))
                    return result;
            }
            else
            {
                // No base URI exists, relative URIs are not allowed.
                if (Uri.TryCreate(uri, UriKind.Absolute, out Uri result))
                    return result;

                // Check if the problem is that the URI is relative.
                if (Uri.TryCreate(uri, UriKind.Relative, out result))
                    throw new JavaScriptException(engine, ErrorType.TypeError, $"Relative URIs are not allowed unless a BaseUri is specified.");
            }

            throw new JavaScriptException(engine, ErrorType.TypeError, $"Invalid URI '{uri}'.");
        }

        internal static FetchOptions GetFetchOptions(ScriptEngine engine)
        {
            if (engineOptions.TryGetValue(engine, out var fetchOptions))
                return fetchOptions;
            throw new InvalidOperationException("AddFetch() has not been called.");
        }

        internal static HeadersConstructor GetHeadersConstructor(ScriptEngine engine)
        {
            return (HeadersConstructor)engine.Global["Headers"];
        }

        internal static RequestConstructor GetRequestConstructor(ScriptEngine engine)
        {
            return (RequestConstructor)engine.Global["Request"];
        }

        internal static ResponseConstructor GetResponseConstructor(ScriptEngine engine)
        {
            return (ResponseConstructor)engine.Global["Response"];
        }
    }
}
