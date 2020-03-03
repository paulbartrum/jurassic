using Jurassic.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jurassic.Extensions.Fetch
{

    /// <summary>
    /// The Headers interface of the Fetch API allows you to perform various actions on HTTP
    /// request and response headers. These actions include retrieving, setting, adding to, and
    /// removing. A Headers object has an associated header list, which is initially empty and
    /// consists of zero or more name and value pairs.  You can add to this using methods like
    /// append(). In all methods of this interface, header names are case-insensitive. 
    /// </summary>
    public partial class HeadersInstance : ObjectInstance, IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> headers = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new FirebugConsole instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. Cannot be <c>null</c>. </param>
        public HeadersInstance(ObjectInstance prototype)
            : base(prototype)
        {
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

            // The initial value of the @@iterator property is the same function object as the
            // initial value of the Headers.prototype.entries property.
            PropertyNameAndValue entriesProperty = properties.Find(p => "entries".Equals(p.Key));
            if (entriesProperty == null)
                throw new InvalidOperationException("Expected entries property.");
            properties.Add(new PropertyNameAndValue(engine.Symbol.Iterator, entriesProperty.Value, PropertyAttributes.NonEnumerable));

            result.FastSetProperties(properties);
            return result;
        }

        /// <summary>
        /// Appends a new value onto an existing header inside a Headers object, or adds the header if it does not already exist.
        /// </summary>
        [JSInternalFunction(Name = "append")]
        public void Append(string name, string value)
        {
            name = NormalizeName(name);
            if (this.headers.TryGetValue(name, out string existingValue))
                this.headers[name] = existingValue + ", " + NormalizeValue(value);
            else
                this.headers.Add(name, NormalizeValue(value));
        }

        /// <summary>
        /// Deletes a header from a Headers object.
        /// </summary>
        [JSInternalFunction(Name = "delete")]
        public void Delete(string name)
        {
            this.headers.Remove(NormalizeName(name));
        }

        /// <summary>
        /// Returns an iterator allowing to go through all key/value pairs contained in this object.
        /// </summary>
        [JSInternalFunction(Name = "entries")]
        public ObjectInstance Entries()
        {
            return new Iterator(Engine,
                this.headers.Select(keyValuePair => Engine.Array.New(new[] { keyValuePair.Key, keyValuePair.Value })));
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
        public object Get(string name)
        {
            if (this.headers.TryGetValue(NormalizeName(name), out string value))
                return value;
            return Null.Value;
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
            return this.headers.ContainsKey(NormalizeName(name));
        }

        /// <summary>
        /// Returns an iterator allowing you to go through all keys of the key/value pairs contained in this object.
        /// </summary>
        [JSInternalFunction(Name = "keys")]
        public ObjectInstance Keys()
        {
            return new Iterator(Engine, this.headers.Keys);
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
            this.headers[NormalizeName(name)] = NormalizeValue(value);
        }

        /// <summary>
        /// Returns an iterator allowing you to go through all values of the key/value pairs
        /// contained in this object.
        /// </summary>
        /// <returns> An iterator. </returns>
        [JSInternalFunction(Name = "values")]
        public ObjectInstance Values()
        {
            return new Iterator(Engine, this.headers.Values);
        }



        //     INTERNAL/PRIVATE METHODS
        //_________________________________________________________________________________________

        private string NormalizeName(string name)
        {
            return name.ToLowerInvariant();
        }

        private string NormalizeValue(string value)
        {
            return value.Trim(' ', '\t', '\r', '\n');
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)headers).GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)headers).GetEnumerator();
        }
    }
}
