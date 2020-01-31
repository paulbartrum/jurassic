using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{
    /// <summary>
    /// The constructor function for the Request object.
    /// </summary>
    public partial class RequestConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Request constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal RequestConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "Request", 0, RequestInstance.CreatePrototype(Engine, this));
            FastSetProperties(properties);
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
        /// Called when the Request object is invoked like a function, e.g. var x = Request().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Constructor Request requires 'new'");
        }

        /// <summary>
        /// Creates a new Request object.
        /// </summary>
        /// <param name="resource"> This defines the resource that you wish to fetch. This can
        /// either be a URI string or a Request object.  </param>
        /// <param name="init"> An object containing any custom settings that you want to apply to
        /// the request. </param>
        /// <returns> A new Request object, either empty or initialised with the given values. </returns>
        [JSConstructorFunction]
        public RequestInstance Construct(object resource, ObjectInstance init = null)
        {
            return new RequestInstance(InstancePrototype, resource, init);
        }

    }
}
