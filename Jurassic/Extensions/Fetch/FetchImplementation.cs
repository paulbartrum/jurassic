using System;
using System.Net.Http;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class FetchImplementation
    {
        private const string BaseUriKey = "__fetch_baseURI";
        private const string UserAgentKey = "__fetch_userAgent";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="options"></param>
        internal static void Add(ScriptEngine engine, FetchOptions options)
        {
            foreach (var property in GetDeclarativeProperties(engine))
                engine.Global.DefineProperty(property.Key, new PropertyDescriptor(property.Value, property.Attributes), throwOnError: true);

            options = options ?? new FetchOptions();
            if (options.BaseUri == null)
                engine.Global.Delete(BaseUriKey, throwOnError: true);
            else
                engine.Global.DefineProperty(BaseUriKey, new PropertyDescriptor(options.BaseUri.ToString(), PropertyAttributes.NonEnumerable), throwOnError: true);
            if (options.UserAgent == null)
                engine.Global.Delete(UserAgentKey, throwOnError: true);
            else
                engine.Global.DefineProperty(UserAgentKey, new PropertyDescriptor(options.UserAgent, PropertyAttributes.NonEnumerable), throwOnError: true);
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
        public static PromiseInstance Fetch(ScriptEngine engine, object resource, ObjectInstance init)
        {
            var request = new RequestInstance(GetRequestConstructor(engine).InstancePrototype, resource, init);

            /*var request = new HttpRequestMessage(HttpMethod.Post, $"https://{Authority}/3/device/{pushNotification.DeviceToken}");
            request.Version = HttpVersion.Version20;

            var client = new HttpClient();
            var task = client.SendAsync(request);
            task = task.ContinueWith(t => 5);
            return engine.Promise.Construct(task.GetAwaiter());*/
            return null;
        }

        internal static Uri ConvertToAbsoluteUri(ScriptEngine engine, string uri)
        {
            var baseUri = engine.Global.GetPropertyValue(BaseUriKey) as string;
            if (baseUri != null)
            {
                // A base URI exists, relative URIs are allowed.
                if (Uri.TryCreate(new Uri(baseUri, UriKind.Absolute), uri, out Uri result))
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
