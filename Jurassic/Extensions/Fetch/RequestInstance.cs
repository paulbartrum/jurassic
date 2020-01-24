using System;
using System.Diagnostics;
using System.Collections.Generic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

    /// <summary>
    /// </summary>
    public partial class RequestInstance : ObjectInstance
    {
        /// <summary>
        /// Creates a new FirebugConsole instance.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        public RequestInstance(ScriptEngine engine)
            : base(engine.Object.InstancePrototype)
        {
            FastSetProperties(GetDeclarativeProperties(engine));
        }

        /// <summary>
        /// Contains the cache mode of the request(e.g., default, reload, no-cache).
        /// </summary>
        [JSProperty(Name = "cache")]
        public string Cache
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Contains the credentials of the request(e.g., "omit", "same-origin", "include"). The default is "same-origin".
        /// </summary>
        [JSProperty(Name = "credentials")]
        public string Credentials
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Returns a string from the RequestDestination enum describing the request's destination. This is a string indicating the type of content being requested.
        /// </summary>
        [JSProperty(Name = "destination")]
        public string Destination
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Contains the associated Headers object of the request.
        /// </summary>
        [JSProperty(Name = "headers")]
        public string Headers
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Contains the subresource integrity value of the request (e.g., sha256-BpfBw7ivV8q2jLiT13fxDYAe2tJllusRSZ273h2nFSE=).
        /// </summary>
        [JSProperty(Name = "integrity")]
        public string Integrity
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Contains the request's method (GET, POST, etc.)
        /// </summary>
        [JSProperty(Name = "method")]
        public string Method
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Contains the mode of the request(e.g., cors, no-cors, same-origin, navigate.)
        /// </summary>
        [JSProperty(Name = "mode")]
        public string Mode
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Contains the mode for how redirects are handled. It may be one of follow, error, or manual.
        /// </summary>
        [JSProperty(Name = "redirect")]
        public string Redirect
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Contains the referrer of the request (e.g., client).
        /// </summary>
        [JSProperty(Name = "referrer")]
        public string Referrer
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Contains the referrer policy of the request (e.g., no-referrer).
        /// </summary>
        [JSProperty(Name = "referrerPolicy")]
        public string ReferrerPolicy
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Contains the URL of the request.
        /// </summary>
        [JSProperty(Name = "url")]
        public string Url
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        /// <summary>
        /// Creates a copy of the current Request object.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "clone")]
        public RequestInstance Clone()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }
    }
}
