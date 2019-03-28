using Jurassic.Compiler;
using System.Diagnostics;
using System.Linq;


namespace Jurassic.Library
{
    /// <summary>
    /// Debugger decorator for ArrayInstance.
    /// </summary>
    public class UserDefinedFunctionDebugView
    {
        /// <summary>
        /// The watched function
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected UserDefinedFunction userDefinedFunction;

        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="userDefinedFunction">The watched function</param>
        public UserDefinedFunctionDebugView(UserDefinedFunction userDefinedFunction)
        {
            this.userDefinedFunction = userDefinedFunction;
        }

        /// <summary>
        /// Parent scope of the function
        /// </summary>
        public Scope Closure
        {
            get
            {
                return this.userDefinedFunction.ParentScope;
            }
        }

        /// <summary>
        /// Local scope of the function
        /// </summary>
        public DeclarativeScope LocalScope
        {
            get
            {
                return this.userDefinedFunction.Scope;
            }
        }

        /// <summary>
        /// The prototype reference
        /// </summary>
        public ObjectInstance __proto__
        {
            get
            {
                return this.userDefinedFunction.Prototype;
            }
        }

        /// <summary>
        /// Array properties list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Properties
        {
            get
            {
                return this.userDefinedFunction.Properties.ToArray();
            }
        }
    }
}
