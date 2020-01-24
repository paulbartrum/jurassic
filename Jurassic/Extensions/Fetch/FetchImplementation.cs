using System.Net.Http;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class FetchImplementation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="resource"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        [JSInternalFunction(Name = "fetch", Flags = JSFunctionFlags.HasEngineParameter)]
        public static PromiseInstance Fetch(ScriptEngine engine, string resource, ObjectInstance init)
        {
            /*var request = new HttpRequestMessage(HttpMethod.Post, $"https://{Authority}/3/device/{pushNotification.DeviceToken}");
            request.Version = HttpVersion.Version20;

            var client = new HttpClient();
            var task = client.SendAsync(request);
            task = task.ContinueWith(t => 5);
            return engine.Promise.Construct(task.GetAwaiter());*/
            return null;
        }
    }
}
