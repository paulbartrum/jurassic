using System;
using System.Collections.Generic;
using Jurassic.Compiler;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a JavaScript function implemented in javascript.
    /// </summary>
    public class UserDefinedFunction : FunctionInstance
    {
        private Func<LexicalScope, object> body;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new instance of a user-defined function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="argumentNames"> The names of the arguments. </param>
        /// <param name="body"> The source code for the body of the function. </param>
        /// <param name="hasNoPrototype"> <c>true</c> if the function should have a prototype;
        /// <c>false</c> otherwise.  </param>
        internal UserDefinedFunction(ObjectInstance prototype, string name, IList<string> argumentNames, string body, bool hasPrototype = true)
            : base(prototype)
        {
            if (argumentNames == null)
                throw new ArgumentNullException("argumentNames");
            if (body == null)
                throw new ArgumentNullException("body");
            this.ArgumentNames = new System.Collections.ObjectModel.ReadOnlyCollection<string>(argumentNames);
            this.Body = body;

            // Add function properties.
            this.SetProperty("name", name);
            this.SetProperty("length", argumentNames.Count);
            if (hasPrototype == true)
            {
                this.SetProperty("prototype", hasPrototype ? GlobalObject.Object.Construct() : null, PropertyAttributes.Writable);
                this.InstancePrototype.SetProperty("constructor", this, PropertyAttributes.NonEnumerable);
            }
            else
            {
                // The empty function doesn't have a prototype.
                // i.e. Object.getPrototypeOf(Function).prototype === null
                this.SetProperty("prototype", Null.Value, PropertyAttributes.Sealed);
            }

            // The parent scope is the global scope.
            this.ParentScope = LexicalScope.Global;

            // Compile the body of the function.
            var parser = new Parser(new Lexer(new System.IO.StringReader(body), name), ScriptContext.Function);
            this.body = parser.Compile();
        }

        /// <summary>
        /// Creates a new instance of a user-defined function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="argumentNames"> The names of the arguments. </param>
        /// <param name="parentScope"> The parent variable scope. </param>
        /// <param name="body"> A delegate which represents the body of the function. </param>
        internal UserDefinedFunction(ObjectInstance prototype, string name, IList<string> argumentNames, LexicalScope parentScope, Func<LexicalScope, object> body)
            : base(prototype)
        {
            if (argumentNames == null)
                throw new ArgumentNullException("argumentNames");
            if (body == null)
                throw new ArgumentNullException("body");
            if (parentScope == null)
                throw new ArgumentNullException("parentScope");
            this.ArgumentNames = new System.Collections.ObjectModel.ReadOnlyCollection<string>(argumentNames);
            this.body = body;
            this.ParentScope = parentScope;

            // Add function properties.
            this.SetProperty("name", name);
            this.SetProperty("length", argumentNames.Count);
            this.SetProperty("prototype", GlobalObject.Object.Construct(), PropertyAttributes.Writable);
            this.InstancePrototype.SetProperty("constructor", this, PropertyAttributes.NonEnumerable);
        }

        /// <summary>
        /// Creates an empty function that is used as the prototype for all the built-in
        /// global function objects (Object, Function, Number, etc).
        /// </summary>
        /// <param name="prototype"> The prototype of the function. </param>
        /// <returns> An empty function that is used as the prototype for all the built-in
        /// global function objects. </returns>
        internal static UserDefinedFunction CreateEmptyFunction(ObjectInstance prototype)
        {
            return new UserDefinedFunction(prototype, "Empty", new string[0], string.Empty, false);
        }


        //     PROPERTIES
        //_________________________________________________________________________________________

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
        /// Gets the parent scope.
        /// </summary>
        internal LexicalScope ParentScope
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the source code for the body of the function.
        /// </summary>
        public string Body
        {
            get;
            private set;
        }



        //     OVERRIDES
        //_________________________________________________________________________________________

        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            // Create a new declarative scope.
            var scope = new LexicalScope(this.ParentScope);

            // In ES3 the "this" value must be an object.  See 10.4.3 in the spec.
            if (this.StrictMode == false)
            {
                if (thisObject == null || thisObject == Null.Value || thisObject == Undefined.Value)
                    thisObject = GlobalObject.Instance;
                else
                    thisObject = TypeConverter.ToObject(thisObject);
            }

            // Bind the "this" value.
            scope.CreateMutableBinding("this", false);
            scope.SetMutableBinding("this", thisObject, this.StrictMode);

            // Bind the argument names to the argument values.
            for (int i = 0; i < this.ArgumentNames.Count; i ++)
            {
                scope.CreateMutableBinding(this.ArgumentNames[i], true);
                if (i < argumentValues.Length)
                    scope.SetMutableBinding(this.ArgumentNames[i], argumentValues[i], this.StrictMode);
            }

            // Create a new arguments object and bind it to the "arguments" variable.
            if (scope.HasBinding("arguments", true) == false)
            {
                scope.CreateMutableBinding("arguments", false);
                scope.SetMutableBinding("arguments", new ArgumentsInstance(GlobalObject.Object.InstancePrototype, this, scope, argumentValues), this.StrictMode);
            }

            // Call the function.
            return this.body(scope);
        }

        public override ObjectInstance ConstructLateBound(params object[] argumentValues)
        {
            // Create a new object and set the prototype to the instance prototype of the function.
            var newObject = ObjectInstance.CreateRawObject(this.InstancePrototype);
            
            // Run the function, with the new object as the "this" keyword.
            CallLateBound(newObject, argumentValues);
            
            // Return the new object.
            return newObject;
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            return string.Format("function {0}({1}) {{\n{2}\n}}", this.Name, string.Join(", ", this.ArgumentNames), this.Body);
        }
    }
}
