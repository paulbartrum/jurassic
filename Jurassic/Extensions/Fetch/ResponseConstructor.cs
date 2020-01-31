using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{
    /// <summary>
    /// The constructor function for the Response object.
    /// </summary>
    public partial class ResponseConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Response constructor.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal ResponseConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "Response", 0, ResponseInstance.CreatePrototype(Engine, this));
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
        /// Called when the Response object is invoked like a function, e.g. var x = Response().
        /// Throws an error.
        /// </summary>
        [JSCallFunction]
        public object Call()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Constructor Response requires 'new'");
        }

        /// <summary>
        /// Creates a new Response object.
        /// </summary>
        /// <returns> A new Response object, either empty or initialised with the given values. </returns>
        [JSConstructorFunction]
        public ResponseInstance Construct(object resource, ObjectInstance init = null)
        {
            return new ResponseInstance(InstancePrototype);
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________


        /// <summary>
        /// Returns a new Response object associated with a network error.
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        [JSInternalFunction(Name = "error", Flags = JSFunctionFlags.HasEngineParameter)]
        public static ResponseInstance Error(ScriptEngine engine)
        {
            return ResponseInstance.Error(engine);
        }

        /// <summary>
        /// 
        /// Creates a new response with a different URL.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="url"> The URL that the new response is to originate from. </param>
        /// <param name="status"> An optional status code for the response (e.g., 302.) </param>
        /// <returns> A Response object. </returns>
        [JSInternalFunction(Name = "redirect", Flags = JSFunctionFlags.HasEngineParameter)]
        public static ResponseInstance Redirect(ScriptEngine engine, string url, int status = 302)
        {
            return ResponseInstance.Redirect(engine, url, status);
        }
    }
}
