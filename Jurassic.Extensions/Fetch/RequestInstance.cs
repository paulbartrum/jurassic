using System;
using System.Collections.Generic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{
    /// <summary>
    /// Represents a resource request.
    /// </summary>
    public partial class RequestInstance : BodyInstance
    {
        private static HashSet<string> normalizedMethods = new HashSet<string>(new[] { "DELETE", "GET", "HEAD", "OPTIONS", "POST", "PUT" }, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates a new Request instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="resource"> This defines the resource that you wish to fetch. This can
        /// either be a URI string or a Request object.  </param>
        /// <param name="init"> An object containing any custom settings that you want to apply to
        /// the request. </param>
        public RequestInstance(ObjectInstance prototype, object resource, ObjectInstance init)
            : base(prototype)
        {
            // Defaults.
            Cache = "default";
            Credentials = "omit";
            Destination = "";
            Integrity = "";
            Method = "GET";
            Mode = "no-cors";
            Redirect = "follow";
            Referrer = "about:client";
            ReferrerPolicy = "";

            // resource can be a URL (string) or a Request object.
            if (resource is string url)
            {
                var parsedUri = FetchImplementation.ConvertToAbsoluteUri(Engine, url);
                if (!string.IsNullOrEmpty(parsedUri.UserInfo))
                    throw new JavaScriptException(Engine, ErrorType.TypeError, "Request cannot be constructed from a URL that includes credentials.");
                Url = parsedUri.ToString();
                Mode = "cors";
                Credentials = "same-origin";
                Headers = FetchImplementation.GetHeadersConstructor(Engine).Construct();
            }
            else
            {
                var request = TypeConverter.ToObject<RequestInstance>(Engine, resource);
                //Body = request.Body;
                Cache = request.Cache;
                Credentials = request.Credentials;
                Destination = request.Destination;
                Headers = FetchImplementation.GetHeadersConstructor(Engine).Construct(request.Headers);
                Integrity = request.Integrity;
                Method = request.Method;
                Mode = request.Mode;
                Redirect = request.Redirect;
                Referrer = request.Referrer;
                ReferrerPolicy = request.ReferrerPolicy;
                Url = request.Url;
            }

            if (init != null)
            {
                // Referrer
                if (init.TryGetPropertyValue("referrer", out var referrer))
                {
                    var referrerStr = TypeConverter.ToString(referrer);
                    if (referrerStr == "")
                        Referrer = "";
                    else
                        Referrer = FetchImplementation.ConvertToAbsoluteUri(Engine, referrerStr).ToString();
                }

                // Referrer policy.
                if (init.TryGetPropertyValue("referrerPolicy", out var referrerPolicy))
                    ReferrerPolicy = TypeConverter.ToString(referrerPolicy);

                // Mode.
                if (init.TryGetPropertyValue("mode", out var mode) && mode != Null.Value)
                {
                    Mode = TypeConverter.ToString(mode);

                    // Make sure the mode is valid.
                    if (Array.IndexOf(new[] { "navigate", "same-origin", "no-cors", "cors" }, Mode) < 0)
                        throw new JavaScriptException(Engine, ErrorType.TypeError, $"The provided value '{Mode}' is not a valid enum value of type RequestMode.");

                    // If mode is "navigate", then throw a TypeError.
                    if (Mode == "navigate")
                        throw new JavaScriptException(Engine, ErrorType.TypeError, "Cannot construct a Request with a mode set to 'navigate'.");
                }

                // Credentials.
                if (init.TryGetPropertyValue("credentials", out var credentials) && credentials != Null.Value)
                {
                    Credentials = TypeConverter.ToString(credentials);

                    // Make sure the credentials value is valid.
                    if (Array.IndexOf(new[] { "omit", "same-origin", "include" }, Credentials) < 0)
                        throw new JavaScriptException(Engine, ErrorType.TypeError, $"The provided value '{Credentials}' is not a valid enum value of type RequestCredentials.");
                }

                // Cache.
                if (init.TryGetPropertyValue("cache", out var cache))
                {
                    Cache = TypeConverter.ToString(cache);

                    // Make sure the cache value is valid.
                    if (Array.IndexOf(new[] { "default", "no-store", "reload", "no-cache", "force-cache", "only-if-cached" }, Cache) < 0)
                        throw new JavaScriptException(Engine, ErrorType.TypeError, $"The provided value '{Cache}' is not a valid enum value of type RequestCache.");

                    // If request’s cache mode is "only-if-cached" and request’s mode is not "same-origin", then throw a TypeError.
                    if (Cache == "only-if-cached" && Mode != "same-origin")
                        throw new JavaScriptException(Engine, ErrorType.TypeError, "Cannot construct a Request with a mode set to 'navigate'.");
                }

                // Redirect.
                if (init.TryGetPropertyValue("redirect", out var redirect))
                {
                    Redirect = TypeConverter.ToString(redirect);

                    // Make sure the cache value is valid.
                    if (Array.IndexOf(new[] { "follow", "error", "manual" }, Redirect) < 0)
                        throw new JavaScriptException(Engine, ErrorType.TypeError, $"The provided value '{Redirect}' is not a valid enum value of type RequestRedirect.");
                }

                // Integrity.
                if (init.TryGetPropertyValue("integrity", out var integrity))
                    Integrity = TypeConverter.ToString(integrity);

                // keepalive is not supported.

                // Method.
                if (init.TryGetPropertyValue("method", out var method))
                {
                    Method = TypeConverter.ToString(method);

                    // Normalize by converting 'get' to 'GET', etc.
                    if (normalizedMethods.Contains(Method))
                        Method = Method.ToUpperInvariant();
                }

                // signal is not supported.

                // Headers
                // If r’s request’s mode is "no-cors", then:
                // If r’s request’s method is not a CORS - safelisted method, then throw a TypeError.
                // Set r’s headers’s guard to "request-no-cors".
                if (init.TryGetPropertyValue("headers", out var headers))
                    Headers = FetchImplementation.GetHeadersConstructor(Engine).Construct(headers);

                //if (init.TryGetPropertyValue("signal", out var referrer))
                //    request.Signal = TypeConverter.ToString(referrer);
                //if (init.TryGetPropertyValue("body", out var body))
                //    Body = TypeConverter.ToString(body);


                // Let inputBody be input’s request’s body if input is a Request object, and null otherwise.
                // If either init["body"] exists and is non - null or inputBody is non - null, and request’s method is `GET` or `HEAD`, then throw a TypeError.
                // Let body be inputBody.
                // If init["body"] exists and is non - null, then:
                //                 Let Content-Type be null.
                // If init["keepalive"] exists and is true, then set body and Content - Type to the result of extracting init["body"], with the keepalive flag set.
                // Otherwise, set body and Content-Type to the result of extracting init["body"].
                // If Content-Type is non - null and r’s headers’s header list does not contain `Content - Type`, then append `Content - Type`/ Content - Type to r’s headers.
                // If body is non - null and body’s source is null, then:
                //                 If r’s request’s mode is neither "same-origin" nor "cors", then throw a TypeError.
                // Set r’s request’s use-CORS - preflight flag.
                // If inputBody is body and input is disturbed or locked, then throw a TypeError.
                // If inputBody is body and inputBody is non - null, then:
                //                 Let ws and rs be the writable side and readable side of an identity transform stream, respectively.
                // Let promise be the result of calling ReadableStreamPipeTo(inputBody, ws, false, false, false, undefined).
                // This makes inputBody’s stream locked and disturbed immediately.
                // Set promise.[[PromiseIsHandled]] to true.
                // Set body to a new body whose stream is rs, whose source is inputBody’s source, and whose total bytes is inputBody’s total bytes.

            }
        }

        /// <summary>
        /// Creates the Request prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, RequestConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(engine.Symbol.ToStringTag, "Request", PropertyAttributes.Configurable));
            result.InitializeProperties(properties);
            return result;
        }

        /// <summary>
        /// Contains the cache mode of the request(e.g., default, reload, no-cache).
        /// </summary>
        [JSProperty(Name = "cache")]
        public string Cache { get; private set; }

        /// <summary>
        /// Contains the credentials of the request(e.g., "omit", "same-origin", "include"). The default is "same-origin".
        /// </summary>
        [JSProperty(Name = "credentials")]
        public string Credentials { get; private set; }

        /// <summary>
        /// Returns a string from the RequestDestination enum describing the request's destination. This is a string indicating the type of content being requested.
        /// </summary>
        [JSProperty(Name = "destination")]
        public string Destination { get; private set; }

        /// <summary>
        /// Contains the associated Headers object of the request.
        /// </summary>
        [JSProperty(Name = "headers")]
        public HeadersInstance Headers { get; private set; }

        /// <summary>
        /// Contains the subresource integrity value of the request (e.g., sha256-BpfBw7ivV8q2jLiT13fxDYAe2tJllusRSZ273h2nFSE=).
        /// </summary>
        [JSProperty(Name = "integrity")]
        public string Integrity { get; private set; }

        /// <summary>
        /// Contains the request's method (GET, POST, etc.)
        /// </summary>
        [JSProperty(Name = "method")]
        public string Method { get; private set; }

        /// <summary>
        /// Contains the mode of the request(e.g., cors, no-cors, same-origin, navigate.)
        /// </summary>
        [JSProperty(Name = "mode")]
        public string Mode { get; private set; }

        /// <summary>
        /// Contains the mode for how redirects are handled. It may be one of follow, error, or manual.
        /// </summary>
        [JSProperty(Name = "redirect")]
        public string Redirect { get; private set; }

        /// <summary>
        /// Contains the referrer of the request (e.g., client).
        /// </summary>
        [JSProperty(Name = "referrer")]
        public string Referrer { get; private set; }

        /// <summary>
        /// Contains the referrer policy of the request (e.g., no-referrer).
        /// </summary>
        [JSProperty(Name = "referrerPolicy")]
        public string ReferrerPolicy { get; private set; }

        /// <summary>
        /// Contains the URL of the request.
        /// </summary>
        [JSProperty(Name = "url")]
        public string Url { get; private set; }

        /// <summary>
        /// Creates a copy of the current Request object.
        /// </summary>
        /// <returns>  </returns>
        [JSInternalFunction(Name = "clone")]
        public RequestInstance Clone()
        {
            return new RequestInstance(Prototype, this, null);
        }
    }
}