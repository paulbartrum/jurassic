using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ProxyInstance : ObjectInstance
    {
        private readonly ObjectInstance target;
        private readonly ObjectInstance handler;

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new proxy instance.
        /// </summary>
        /// <param name="engine"> The next object in the prototype chain. </param>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        internal ProxyInstance(ScriptEngine engine, ObjectInstance target, ObjectInstance handler)
            : base(engine.Object.InstancePrototype)
        {
            this.target = target;
            this.handler = handler;
        }



    }
}
