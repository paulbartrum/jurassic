using System;
using System.Diagnostics;
using System.Collections.Generic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

    /// <summary>
    /// </summary>
    public partial class ResponseInstance : ObjectInstance
    {
        /// <summary>
        /// Creates a new FirebugConsole instance.
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        public ResponseInstance(ScriptEngine engine)
            : base(engine.Object.InstancePrototype)
        {
            //FastSetProperties(GetDeclarativeProperties(engine));
        }
    }
}
