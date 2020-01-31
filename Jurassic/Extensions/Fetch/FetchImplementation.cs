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
        internal static void Add(ScriptEngine engine)
        {
            engine.Global.FastSetProperties(GetDeclarativeProperties(engine));
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
        [JSInternalFunction(Name = "fetch", Flags = JSFunctionFlags.HasEngineParameter)]
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

        internal static Uri GetBaseUri(ScriptEngine engine)
        {
            return null;
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
