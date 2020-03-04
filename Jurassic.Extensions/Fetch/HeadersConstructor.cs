using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{
    /// <summary>
    /// The constructor function for the Headers object.
    /// </summary>
    public partial class HeadersConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Headers constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal HeadersConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "Headers", 0, HeadersInstance.CreatePrototype(Engine, this));
            InitializeProperties(properties);
        }



        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// A reference to the constructor function that is used to create derived objects.
        /// </summary>
        [JSProperty(Name = "@@species")]
        public FunctionInstance Species
        {
            get { return this; }
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Headers object is invoked like a function, e.g. var x = Headers().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Constructor Headers requires 'new'");
        }

        /// <summary>
        /// Creates a new Headers object.
        /// </summary>
        /// <param name="obj"> A list of key-value pairs, or an object with properties that can be
        /// interpreted as header name/value pairs. </param>
        /// <returns> A new Headers object, either empty or initialised with the given values. </returns>
        [JSConstructorFunction]
        public HeadersInstance Construct(object obj = null)
        {
            // Create a new headers instance.
            var result = new HeadersInstance(this.InstancePrototype);
            if (TypeUtilities.IsUndefined(obj))
                return result;

            // Fill the headers instance.
            var objectInstance = TypeConverter.ToObject(Engine, obj);
            var iterator = TypeUtilities.GetIterator(Engine, objectInstance);
            if (iterator != null)
            {
                // e.g. [ [ "Content-Type", "text/xml" ], [ "User-Agent", "Test" ] ]
                foreach (var rawItem in TypeUtilities.Iterate(Engine, iterator))
                {
                    var item = TypeConverter.ToObject(Engine, rawItem);
                    result.Append(TypeConverter.ToString(item[0]), TypeConverter.ToString(item[1]));
                }
            }
            else
            {
                // e.g. { "Content-Type": "text/xml", "User-Agent": "Test" }
                foreach (var property in objectInstance.Properties)
                    if (property.IsEnumerable && property.Key is string)
                        result.Append((string)property.Key, TypeConverter.ToString(property.Value));
            }

            return result;
        }

    }
}
