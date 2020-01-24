using Jurassic.Library;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Jurassic.Extensions.Fetch
{

    /// <summary>
    /// The Headers interface of the Fetch API allows you to perform various actions on HTTP
    /// request and response headers. These actions include retrieving, setting, adding to, and
    /// removing. A Headers object has an associated header list, which is initially empty and
    /// consists of zero or more name and value pairs.  You can add to this using methods like
    /// append() (see Examples.) In all methods of this interface, header names are matched by
    /// case-insensitive byte sequence. 
    /// </summary>
    public partial class HeadersInstance : ObjectInstance
    {
        private HttpHeaders headers;

        /// <summary>
        /// Creates a new FirebugConsole instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. Cannot be <c>null</c>. </param>
        /// <param name="headers"> The headers object that this class wraps. </param>
        public HeadersInstance(ObjectInstance prototype, HttpHeaders headers)
            : base(prototype)
        {
            this.headers = headers;
        }

        /// <summary>
        /// Creates the Headers prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, HeadersConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            properties.Add(new PropertyNameAndValue(engine.Symbol.ToStringTag, "Headers", PropertyAttributes.Configurable));
            result.FastSetProperties(properties);
            return result;
        }

        /// <summary>
        /// Appends a new value onto an existing header inside a Headers object, or adds the header if it does not already exist.
        /// </summary>
        [JSInternalFunction(Name = "append")]
        public void Append(string name, string value)
        {
            this.headers.TryAddWithoutValidation(name, NormalizeValue(value));
        }

        /// <summary>
        /// Deletes a header from a Headers object.
        /// </summary>
        [JSInternalFunction(Name = "delete")]
        public void Delete(string name)
        {
            this.headers.Remove(name);
        }

        /// <summary>
        /// Returns an iterator allowing to go through all key/value pairs contained in this object.
        /// </summary>
        [JSInternalFunction(Name = "entries")]
        public ObjectInstance Entries()
        {
            return new Iterator(Engine.BaseIteratorPrototype,
                this.headers.Select(keyValuePair => Engine.Array.New(new[] { keyValuePair.Key, CombineValues(keyValuePair.Value) })));
        }

        /// <summary>
        /// Returns a ByteString sequence of all the values of a header within a Headers object
        /// with a given name.
        /// </summary>
        /// <param name="name"> The name of the HTTP header whose values you want to retrieve from
        /// the Headers object. If the given name is not the name of an HTTP header, this method
        /// throws a TypeError. The name is case-insensitive. </param>
        /// <returns> A ByteString sequence representing the values of the retrieved header or
        /// null if this header is not set. </returns>
        [JSInternalFunction(Name = "get")]
        public string Get(string name)
        {
            IEnumerable<string> values;
            if (this.headers.TryGetValues("", out values))
                return CombineValues(values);
            return null;
        }

        /// <summary>
        /// Returns a boolean stating whether a Headers object contains a certain header.
        /// </summary>
        /// <param name="name"> The name of the HTTP header you want to test for. If the given name
        /// is not the name of an HTTP header, this method throws a TypeError. </param>
        /// <returns> A boolean indicating whether the header exists. </returns>
        [JSInternalFunction(Name = "has")]
        public bool Has(string name)
        {
            return this.headers.Contains(name);
        }

        /// <summary>
        /// Returns an iterator allowing you to go through all keys of the key/value pairs contained in this object.
        /// </summary>
        [JSInternalFunction(Name = "keys")]
        public ObjectInstance Keys()
        {
            return new Iterator(Engine.BaseIteratorPrototype,
                this.headers.Select(keyValuePair => keyValuePair.Key));
        }

        /// <summary>
        /// Sets a new value for an existing header inside a Headers object, or adds the header if
        /// it does not already exist.
        /// </summary>
        /// <param name="name"> The name of the HTTP header you want to set to a new value. If the
        /// given name is not the name of an HTTP header, this method throws a TypeError. </param>
        /// <param name="value"> The new value you want to set. </param>
        [JSInternalFunction(Name = "set")]
        public void Set(string name, string value)
        {
            this.headers.Remove(name);
            this.headers.TryAddWithoutValidation(name, NormalizeValue(value));
        }

        /// <summary>
        /// Returns an iterator allowing you to go through all values of the key/value pairs
        /// contained in this object.
        /// </summary>
        /// <returns> An iterator. </returns>
        [JSInternalFunction(Name = "values")]
        public ObjectInstance Values()
        {
            return new Iterator(Engine.BaseIteratorPrototype,
                this.headers.Select(keyValuePair => CombineValues(keyValuePair.Value)));
        }


        private string NormalizeValue(string value)
        {
            return value.Trim(' ', '\t', '\r', '\n');
        }

        private string CombineValues(IEnumerable<string> values)
        {
            return string.Join(",", values);
        }
    }
}
