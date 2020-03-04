using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

    /// <summary>
    /// </summary>
    public abstract partial class BodyInstance : ObjectInstance
    {
        /// <summary>
        /// Creates a new Body instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        public BodyInstance(ObjectInstance prototype)
            : base(prototype)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        [JSProperty(Name = "body")]
        public ObjectInstance Body { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [JSProperty(Name = "bodyUsed")]
        public bool BodyUsed { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "arrayBuffer")]
        public PromiseInstance ArrayBuffer()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "blob")]
        public PromiseInstance Blob()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "formData")]
        public PromiseInstance FormData()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "json")]
        public PromiseInstance Json()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "text")]
        public PromiseInstance Text()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }
    }
}
