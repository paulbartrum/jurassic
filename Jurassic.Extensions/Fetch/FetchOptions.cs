using System;
using System.Net.Http;

namespace Jurassic.Extensions.Fetch
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

        /// <summary>
        /// 
        /// </summary>
        public Func<HttpClient> HttpClientFactory { get; set; }
    }
}
