using System;
using System.Collections.Generic;
using System.Linq;
using Jurassic.Compiler;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a JavaScript function implemented in javascript.
    /// </summary>
    public class UserDefinedFunction : FunctionInstance
    {
        [ThreadStatic]
        private static int currentRecursionDepth;

        [NonSerialized]
        private GeneratedMethod generatedMethod;

        [NonSerialized]
        private FunctionDelegate body;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new instance of a user-defined function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="argumentsText"> A comma-separated list of arguments. </param>
        /// <param name="bodyText"> The source code for the body of the function. </param>
        /// <remarks> This is used by <c>new Function()</c>. </remarks>
        internal UserDefinedFunction(ObjectInstance prototype, string name, string argumentsText, string bodyText)
            : base(prototype)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (argumentsText == null)
                throw new ArgumentNullException(nameof(argumentsText));
            if (bodyText == null)
                throw new ArgumentNullException(nameof(bodyText));

            // Set up a new function scope.
            var scope = DeclarativeScope.CreateFunctionScope(this.Engine.CreateGlobalScope(), name, null);

            // Compile the code.
            var context = new FunctionMethodGenerator(this.Engine, scope, name, argumentsText, bodyText, new CompilerOptions());
            context.GenerateCode();

            this.ArgumentsText = argumentsText;
            this.ArgumentNames = context.Arguments.Select(a => a.Name).ToList();
            this.BodyText = bodyText;
            this.generatedMethod = context.GeneratedMethod;
            this.body = (FunctionDelegate)this.generatedMethod.GeneratedDelegate;
            this.ParentScope = this.Engine.CreateGlobalScope();
            this.StrictMode = context.StrictMode;
            InitProperties(name, context.Arguments.Count);
        }

        /// <summary>
        /// Creates a new instance of a user-defined function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="argumentNames"> The names of the arguments. </param>
        /// <param name="parentScope"> The scope at the point the function is declared. </param>
        /// <param name="bodyText"> The source code for the function body. </param>
        /// <param name="body"> A delegate which represents the body of the function. </param>
        /// <param name="strictMode"> <c>true</c> if the function body is strict mode; <c>false</c> otherwise. </param>
        /// <remarks> This is used by <c>arguments</c>. </remarks>
        internal UserDefinedFunction(ObjectInstance prototype, string name, IList<string> argumentNames, Scope parentScope, string bodyText, FunctionDelegate body, bool strictMode)
            : base(prototype)
        {
            this.ArgumentsText = string.Join(", ", argumentNames);
            this.ArgumentNames = argumentNames;
            this.BodyText = bodyText;
            this.generatedMethod = new GeneratedMethod(body, null);
            this.body = body;
            this.ParentScope = parentScope;
            this.StrictMode = strictMode;
            InitProperties(name, argumentNames.Count);
        }

        /// <summary>
        /// Creates a new instance of a user-defined function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="argumentNames"> The names of the arguments. </param>
        /// <param name="parentScope"> The scope at the point the function is declared. </param>
        /// <param name="bodyText"> The source code for the function body. </param>
        /// <param name="generatedMethod"> A delegate which represents the body of the function plus any dependencies. </param>
        /// <param name="strictMode"> <c>true</c> if the function body is strict mode; <c>false</c> otherwise. </param>
        /// <remarks> This is used by functions declared in JavaScript code (including getters and setters). </remarks>
        public UserDefinedFunction(ObjectInstance prototype, string name, IList<string> argumentNames, Scope parentScope, string bodyText, GeneratedMethod generatedMethod, bool strictMode)
            : base(prototype)
        {
            this.ArgumentsText = string.Join(", ", argumentNames);
            this.ArgumentNames = argumentNames;
            this.BodyText = bodyText;
            this.generatedMethod = generatedMethod;
            this.body = (FunctionDelegate)this.generatedMethod.GeneratedDelegate;
            this.ParentScope = parentScope;
            this.StrictMode = strictMode;
            InitProperties(name, argumentNames.Count);
        }

        /// <summary>
        /// Initializes the object properties.
        /// </summary>
        /// <param name="name"> The name of the function. </param>
        /// <param name="length"> The expected number of arguments. </param>
        private void InitProperties(string name, int length)
        {
            // Create an object to serve as the instance prototype.
            var instancePrototype = this.Engine.Object.Construct();
            instancePrototype.FastSetProperties(new List<PropertyNameAndValue>
            {
                new PropertyNameAndValue("constructor", this, PropertyAttributes.NonEnumerable)
            });

            // Now add properties to this object.
            FastSetProperties(new List<PropertyNameAndValue>()
            {
                new PropertyNameAndValue("name", name, PropertyAttributes.Configurable),
                new PropertyNameAndValue("length", length, PropertyAttributes.Configurable),
                new PropertyNameAndValue("prototype", instancePrototype, PropertyAttributes.Writable),
            });
        }



        //     PROPERTIES
        //_________________________________________________________________________________________


        /// <summary>
        /// A comma-separated list of arguments.
        /// </summary>
        public string ArgumentsText
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list containing the names of the function arguments, in order of definition.
        /// This list can contain duplicate names.
        /// </summary>
        public IList<string> ArgumentNames
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value that indicates whether the function was declared within strict mode code
        /// -or- the function contains a strict mode directive within the function body.
        /// </summary>
        public bool StrictMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the scope at the point the function was declared.
        /// </summary>
        internal Scope ParentScope
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the source code for the body of the function.
        /// </summary>
        public string BodyText
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the body of the method in the form of disassembled IL code.  Will be <c>null</c>
        /// unless ScriptEngine.EnableILAnalysis has been set to <c>true</c>.
        /// </summary>
        public string DisassembledIL
        {
            get { return this.generatedMethod.DisassembledIL; }
        }



        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values to pass to the function. </param>
        /// <returns> The value that was returned from the function. </returns>
        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            // Check the allowed recursion depth.
            if (this.Engine.RecursionDepthLimit > 0 && currentRecursionDepth >= this.Engine.RecursionDepthLimit)
                throw new StackOverflowException("The allowed recursion depth of the script engine has been exceeded.");

            currentRecursionDepth++;
            try
            {
                // Call the function.
                return this.body(this.Engine, this.ParentScope, thisObject, this, argumentValues);
            }
            finally
            {
                currentRecursionDepth--;
            }
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            return string.Format("function {0}({1}) {{\n{2}\n}}", this.Name, this.ArgumentsText, this.BodyText);
        }
    }
}
