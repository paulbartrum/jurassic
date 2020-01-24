using System;
using System.Diagnostics;
using System.Collections.Generic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

    /// <summary>
    /// </summary>
    public abstract partial class BodyInstance : ObjectInstance
    {
        /// <summary>
        /// Creates a new FirebugConsole instance.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        public BodyInstance(ScriptEngine engine)
            : base(engine.Object.InstancePrototype)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        [JSProperty(Name = "body")]
        public ObjectInstance Body
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        [JSProperty(Name = "bodyUsed")]
        public bool BodyUsed
        {
            get { throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated."); }
        }

        [JSInternalFunction(Name = "arrayBuffer")]
        public PromiseInstance ArrayBuffer()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }

        [JSInternalFunction(Name = "blob")]
        public PromiseInstance Blob()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }

        [JSInternalFunction(Name = "formData")]
        public PromiseInstance FormData()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }

        [JSInternalFunction(Name = "json")]
        public PromiseInstance Json()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }

        [JSInternalFunction(Name = "text")]
        public PromiseInstance Text()
        {
            throw new JavaScriptException(Engine, ErrorType.TypeError, "Functionality is not implementated.");
        }
    }
}
