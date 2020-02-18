using System;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{
    /// <summary>
    /// Represents the response to a request.
    /// </summary>
    public partial class ResponseInstance : BodyInstance
    {
        /// <summary>
        /// Creates a new Response instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        public ResponseInstance(ObjectInstance prototype)
            : base(prototype)
        {
            Url = "";
            StatusText = "";
        }

        /// <summary>
        /// Creates a new Response instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="body"> An object defining a body for the response. This can be null (which
        /// is the default value), or one of: Blob, BufferSource, FormData, ReadableStream,
        /// URLSearchParams, String. </param>
        /// <param name="init"> An options object containing any custom settings that you want to
        /// apply to the response, or an empty object (which is the default value). The possible
        /// options are: 'status', 'statusText', 'headers'. </param>
        public ResponseInstance(ObjectInstance prototype, ObjectInstance body = null, ObjectInstance init = null)
            : base(prototype)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates the Response prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, ResponseConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(engine.Symbol.ToStringTag, "Response", PropertyAttributes.Configurable));
            result.FastSetProperties(properties);
            return result;
        }

        /// <summary>
        /// Implements the Response.error() method.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <returns> A Response object. </returns>
        internal static ResponseInstance Error(ScriptEngine engine)
        {
            var result = new ResponseInstance(FetchImplementation.GetResponseConstructor(engine).InstancePrototype);
            result.Type = "error";
            result.Headers = new HeadersInstance(FetchImplementation.GetHeadersConstructor(engine).InstancePrototype);
            return result;
        }

        /// <summary>
        /// Implements the Response.redirect() method.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="url"> The URL that the new response is to originate from. </param>
        /// <param name="status"> An optional status code for the response (e.g., 302.) </param>
        /// <returns> A Response object. </returns>
        internal static ResponseInstance Redirect(ScriptEngine engine, string url, int status = 302)
        {
            var parsedUrl = FetchImplementation.ConvertToAbsoluteUri(engine, url);
            if (status != 301 && status != 302 && status != 303 && status != 307 && status != 308)
                throw new JavaScriptException(engine, ErrorType.RangeError, "Invalid status code.");

            var result = new ResponseInstance(FetchImplementation.GetResponseConstructor(engine).InstancePrototype);
            result.Type = "default";
            result.Status = status;
            result.Headers = new HeadersInstance(FetchImplementation.GetHeadersConstructor(engine).InstancePrototype);
            result.Headers.Append("Location", parsedUrl.ToString());
            return result;
        }

        /// <summary>
        /// The type of the response (e.g., basic, cors).
        /// </summary>
        [JSProperty(Name = "type")]
        public string Type { get; private set; }

        /// <summary>
        /// The URL of the response.
        /// </summary>
        [JSProperty(Name = "url")]
        public string Url { get; private set; }

        /// <summary>
        /// Indicates whether or not the response is the result of a redirect (that is, its URL list has more than one entry).
        /// </summary>
        [JSProperty(Name = "redirected")]
        public bool Redirected { get; private set; }

        /// <summary>
        /// The status code of the response. (This will be 200 for a success).
        /// </summary>
        [JSProperty(Name = "status")]
        public int Status { get; private set; }

        /// <summary>
        /// A boolean indicating whether the response was successful (status in the range 200–299) or not.
        /// </summary>
        [JSProperty(Name = "ok")]
        public bool Ok { get; private set; }

        /// <summary>
        /// The status message corresponding to the status code. (e.g., OK for 200).
        /// </summary>
        [JSProperty(Name = "statusText")]
        public string StatusText { get; private set; }

        /// <summary>
        /// The Headers object associated with the response.
        /// </summary>
        [JSProperty(Name = "headers")]
        public HeadersInstance Headers { get; private set; }

        /// <summary>
        /// Creates a clone of a response object.
        /// </summary>
        /// <returns> A cloned Response. </returns>
        [JSFunction(Name = "clone")]
        public ResponseInstance Clone()
        {
            var result = new ResponseInstance(Prototype);
            result.Type = Type;
            result.Url = Url;
            result.Redirected = Redirected;
            result.Status = Status;
            result.Ok = Ok;
            result.StatusText = StatusText;
            result.Headers = Headers;
            return result;
        }
    }
}
