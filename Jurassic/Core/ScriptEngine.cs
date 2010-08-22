using System;
using Jurassic.Library;

namespace Jurassic
{
    /// <summary>
    /// Represents the JavaScript script engine.  This is the first object that needs to be
    /// instantiated in order to execute javascript code.
    /// </summary>
    public class ScriptEngine
    {
        // The initial hidden class schema.
        private HiddenClassSchema emptySchema;

        // The built-in objects.
        private GlobalObject globalObject;
        private ArrayConstructor arrayConstructor;
        private BooleanConstructor booleanConstructor;
        private DateConstructor dateConstructor;
        private FunctionConstructor functionConstructor;
        private JSONObject jsonObject;
        private MathObject mathObject;
        private NumberConstructor numberConstructor;
        private ObjectConstructor objectConstructor;
        private RegExpConstructor regExpConstructor;
        private StringConstructor stringConstructor;

        // The built-in error objects.
        private ErrorConstructor errorConstructor;
        private ErrorConstructor rangeErrorConstructor;
        private ErrorConstructor typeErrorConstructor;
        private ErrorConstructor syntaxErrorConstructor;
        private ErrorConstructor uriErrorConstructor;
        private ErrorConstructor evalErrorConstructor;
        private ErrorConstructor referenceErrorConstructor;


        public ScriptEngine()
        {
            // Create the initial hidden class schema.  This must be done first.
            this.emptySchema = HiddenClassSchema.CreateEmptySchema();

            // Create the base of the prototype chain.
            var baseObject = ObjectInstance.CreateRootObject(this);

            // Create the global object.
            this.globalObject = new GlobalObject(baseObject);

            // Create the function object that second to last in the prototype chain.
            var baseFunction = UserDefinedFunction.CreateEmptyFunction(baseObject);

            // Object must be created first, then function.
            this.objectConstructor = new ObjectConstructor(baseFunction, baseObject);
            this.functionConstructor = new FunctionConstructor(baseFunction, baseFunction);

            // Create all the built-in objects.
            this.mathObject = new MathObject(baseObject);
            this.jsonObject = new JSONObject(baseObject);

            // Create all the built-in functions.
            this.arrayConstructor = new ArrayConstructor(baseFunction);
            this.booleanConstructor = new BooleanConstructor(baseFunction);
            this.dateConstructor = new DateConstructor(baseFunction);
            this.numberConstructor = new NumberConstructor(baseFunction);
            this.regExpConstructor = new RegExpConstructor(baseFunction);
            this.stringConstructor = new StringConstructor(baseFunction);

            // Create the error functions.
            this.errorConstructor = new ErrorConstructor(baseFunction, "Error");
            this.rangeErrorConstructor = new ErrorConstructor(baseFunction, "RangeError");
            this.typeErrorConstructor = new ErrorConstructor(baseFunction, "TypeError");
            this.syntaxErrorConstructor = new ErrorConstructor(baseFunction, "SyntaxError");
            this.uriErrorConstructor = new ErrorConstructor(baseFunction, "URIError");
            this.evalErrorConstructor = new ErrorConstructor(baseFunction, "EvalError");
            this.referenceErrorConstructor = new ErrorConstructor(baseFunction, "ReferenceError");

            // Populate the instance prototypes (TODO: optimize this, currently takes about 15ms).
            this.globalObject.PopulateFunctions();
            this.objectConstructor.PopulateFunctions();
            this.objectConstructor.InstancePrototype.PopulateFunctions();
            this.functionConstructor.InstancePrototype.PopulateFunctions(typeof(FunctionInstance));
            this.mathObject.PopulateFunctions();
            this.mathObject.PopulateFields();
            this.jsonObject.PopulateFunctions();
            this.arrayConstructor.PopulateFunctions();
            this.arrayConstructor.InstancePrototype.PopulateFunctions();
            this.booleanConstructor.InstancePrototype.PopulateFunctions();
            this.dateConstructor.PopulateFunctions();
            this.dateConstructor.InstancePrototype.PopulateFunctions();
            this.numberConstructor.InstancePrototype.PopulateFunctions();
            this.numberConstructor.PopulateFields();
            this.regExpConstructor.InstancePrototype.PopulateFunctions();
            this.stringConstructor.PopulateFunctions();
            this.stringConstructor.InstancePrototype.PopulateFunctions();
            this.errorConstructor.InstancePrototype.PopulateFunctions();

            // Add them as JavaScript-accessible properties of the global instance.
            this.globalObject.FastSetProperty("Array", this.arrayConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("Boolean", this.booleanConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("Date", this.dateConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("Function", this.functionConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("JSON", this.jsonObject, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("Math", this.mathObject, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("Number", this.numberConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("Object", this.objectConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("RegExp", this.regExpConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("String", this.stringConstructor, PropertyAttributes.NonEnumerable);

            // And the errors.
            this.globalObject.FastSetProperty("Error", this.errorConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("RangeError", this.rangeErrorConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("TypeError", this.typeErrorConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("SyntaxError", this.syntaxErrorConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("URIError", this.uriErrorConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("EvalError", this.evalErrorConstructor, PropertyAttributes.NonEnumerable);
            this.globalObject.FastSetProperty("ReferenceError", this.referenceErrorConstructor, PropertyAttributes.NonEnumerable);
        }


        //     PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets or sets a value that indicates whether to force ECMAScript 5 strict mode, even if
        /// the code does not contain a strict mode directive ("use strict").  The default is
        /// <c>false</c>.
        /// </summary>
        public bool ForceStrictMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the script engine should run in
        /// compatibility mode.
        /// </summary>
        public CompatibilityMode CompatibilityMode
        {
            get;
            set;
        }



        //     GLOBAL BUILT-IN OBJECTS
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the built-in global object.  This object is implicitly accessed when creating
        /// global variables and functions.
        /// </summary>
        public GlobalObject Global
        {
            get { return this.globalObject; }
        }

        /// <summary>
        /// Gets the built-in Array object.
        /// </summary>
        public ArrayConstructor Array
        {
            get { return this.arrayConstructor; }
        }

        /// <summary>
        /// Gets the built-in Boolean object.
        /// </summary>
        public BooleanConstructor Boolean
        {
            get { return this.booleanConstructor; }
        }

        /// <summary>
        /// Gets the built-in Date object.
        /// </summary>
        public DateConstructor Date
        {
            get { return this.dateConstructor; }
        }

        /// <summary>
        /// Gets the built-in Function object.
        /// </summary>
        public FunctionConstructor Function
        {
            get { return this.functionConstructor; }
        }

        /// <summary>
        /// Gets the built-in Math object.
        /// </summary>
        public MathObject Math
        {
            get { return this.mathObject; }
        }

        /// <summary>
        /// Gets the built-in Number object.
        /// </summary>
        public NumberConstructor Number
        {
            get { return this.numberConstructor; }
        }

        /// <summary>
        /// Gets the built-in Object object.
        /// </summary>
        public ObjectConstructor Object
        {
            get { return this.objectConstructor; }
        }

        /// <summary>
        /// Gets the built-in RegExp object.
        /// </summary>
        public RegExpConstructor RegExp
        {
            get { return this.regExpConstructor; }
        }

        /// <summary>
        /// Gets the built-in String object.
        /// </summary>
        public StringConstructor String
        {
            get { return this.stringConstructor; }
        }


        /// <summary>
        /// Gets the built-in Error object.
        /// </summary>
        public ErrorConstructor Error
        {
            get { return this.errorConstructor; }
        }

        /// <summary>
        /// Gets the built-in RangeError object.
        /// </summary>
        public ErrorConstructor RangeError
        {
            get { return this.rangeErrorConstructor; }
        }

        /// <summary>
        /// Gets the built-in TypeError object.
        /// </summary>
        public ErrorConstructor TypeError
        {
            get { return this.typeErrorConstructor; }
        }

        /// <summary>
        /// Gets the built-in SyntaxError object.
        /// </summary>
        public ErrorConstructor SyntaxError
        {
            get { return this.syntaxErrorConstructor; }
        }

        /// <summary>
        /// Gets the built-in URIError object.
        /// </summary>
        public ErrorConstructor URIError
        {
            get { return this.uriErrorConstructor; }
        }

        /// <summary>
        /// Gets the built-in EvalError object.
        /// </summary>
        public ErrorConstructor EvalError
        {
            get { return this.evalErrorConstructor; }
        }

        /// <summary>
        /// Gets the built-in ReferenceError object.
        /// </summary>
        public ErrorConstructor ReferenceError
        {
            get { return this.referenceErrorConstructor; }
        }



        //     DEBUGGING SUPPORT
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets or sets a value which indicates whether debug information should be generated.  If
        /// this is set to <c>true</c> performance and memory usage are negatively impacted.
        /// </summary>
        public bool EnableDebugging
        {
            get;
            set;
        }

        internal class ReflectionEmitModuleInfo
        {
            public System.Reflection.Emit.AssemblyBuilder AssemblyBuilder;
            public System.Reflection.Emit.ModuleBuilder ModuleBuilder;
            public int TypeCount;
        }

        /// <summary>
        /// Gets or sets information needed by Reflection.Emit.
        /// </summary>
        internal ReflectionEmitModuleInfo ReflectionEmitInfo
        {
            get;
            set;
        }



        //     EXECUTION
        //_________________________________________________________________________________________

        /// <summary>
        /// Executes the given source code.  Execution is bound to the global scope.
        /// </summary>
        /// <param name="code"> The javascript source code to execute. </param>
        /// <returns> The result of executing the source code. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="code"/> is a <c>null</c> reference. </exception>
        public object Evaluate(string code)
        {
            return Evaluate(new StringScriptSource(code));
        }

        /// <summary>
        /// Executes the given source code.  Execution is bound to the global scope.
        /// </summary>
        /// <typeparam name="T"> The type to convert the result to. </typeparam>
        /// <param name="code"> The javascript source code to execute. </param>
        /// <returns> The result of executing the source code. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="code"/> is a <c>null</c> reference. </exception>
        public T Evaluate<T>(string code)
        {
            return TypeConverter.ConvertTo<T>(this, Evaluate(code));
        }

        /// <summary>
        /// Executes the given source code.  Execution is bound to the global scope.
        /// </summary>
        /// <param name="source"> The javascript source code to execute. </param>
        /// <returns> The result of executing the source code. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="code"/> is a <c>null</c> reference. </exception>
        public object Evaluate(ScriptSource source)
        {
            var methodGen = new Jurassic.Compiler.EvalMethodGenerator(
                this,                               // The script engine
                this.CreateGlobalScope(),           // The variable scope.
                source,                             // The source code.
                CreateOptions(),                    // The compiler options.
                this.Global);                       // The value of the "this" keyword.
            
            // Parse
            if (this.ParsingStarted != null)
                this.ParsingStarted(this, EventArgs.Empty);
            methodGen.Parse();

            // Optimize
            if (this.OptimizationStarted != null)
                this.OptimizationStarted(this, EventArgs.Empty);
            methodGen.Optimize();

            // Generate code
            if (this.CodeGenerationStarted != null)
                this.CodeGenerationStarted(this, EventArgs.Empty);
            methodGen.GenerateCode();
            VerifyGeneratedCode();

            // Execute
            if (this.ExecutionStarted != null)
                this.ExecutionStarted(this, EventArgs.Empty);
            return methodGen.Execute();
        }

        /// <summary>
        /// Executes the given source code.  Execution is bound to the global scope.
        /// </summary>
        /// <typeparam name="T"> The type to convert the result to. </typeparam>
        /// <param name="source"> The javascript source code to execute. </param>
        /// <returns> The result of executing the source code. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="code"/> is a <c>null</c> reference. </exception>
        public T Evaluate<T>(ScriptSource source)
        {
            return TypeConverter.ConvertTo<T>(this, Evaluate(source));
        }

        /// <summary>
        /// Executes the given source code.  Execution is bound to the global scope.
        /// </summary>
        /// <param name="code"> The javascript source code to execute. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="path"/> is a <c>null</c> reference. </exception>
        public void Execute(string code)
        {
            Execute(new StringScriptSource(code));
        }

        /// <summary>
        /// Executes the given file.  Execution is bound to the global scope.
        /// </summary>
        /// <param name="path"> The path to a javascript file.  This can be a local file path or a
        /// UNC path. </param>
        /// <param name="encoding"> The character encoding to use if the file lacks a byte order
        /// mark (BOM).  If this parameter is omitted, the file is assumed to be UTF8. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="path"/> is a <c>null</c> reference. </exception>
        public void ExecuteFile(string path, System.Text.Encoding encoding = null)
        {
            Execute(new FileScriptSource(path, encoding));
        }

        /// <summary>
        /// Executes the given source code.  Execution is bound to the global scope.
        /// </summary>
        /// <param name="source"> The javascript source code to execute. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="code"/> is a <c>null</c> reference. </exception>
        public void Execute(ScriptSource source)
        {
            var methodGen = new Jurassic.Compiler.GlobalMethodGenerator(
                this,                               // The script engine
                source,                             // The source code.
                CreateOptions());                   // The compiler options.

            // Parse
            if (this.ParsingStarted != null)
                this.ParsingStarted(this, EventArgs.Empty);
            methodGen.Parse();

            // Optimize
            if (this.OptimizationStarted != null)
                this.OptimizationStarted(this, EventArgs.Empty);
            methodGen.Optimize();

            // Generate code
            if (this.CodeGenerationStarted != null)
                this.CodeGenerationStarted(this, EventArgs.Empty);
            methodGen.GenerateCode();
            VerifyGeneratedCode();

            // Execute
            if (this.ExecutionStarted != null)
                this.ExecutionStarted(this, EventArgs.Empty);
            methodGen.Execute();
        }

        /// <summary>
        /// Verifies the generated byte code.
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        private void VerifyGeneratedCode()
        {
#if false
            if (this.EnableDebugging == false)
                return;

            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "JurassicDebug.dll");

            // set the entry point for the application and save it
            this.ReflectionEmitInfo.AssemblyBuilder.Save(System.IO.Path.GetFileName(filePath));

            // Copy this DLL there as well.
            var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            System.IO.File.Copy(assemblyPath, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), System.IO.Path.GetFileName(assemblyPath)), true);

            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\x64\PEVerify.exe";
            startInfo.Arguments = string.Format("\"{0}\" /nologo /verbose /unique", filePath);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            var verifyProcess = System.Diagnostics.Process.Start(startInfo);
            string output = verifyProcess.StandardOutput.ReadToEnd();

            if (verifyProcess.ExitCode != 0)
            {
                throw new InvalidOperationException(output);
            }
            //else
            //{
            //    System.Diagnostics.Process.Start(@"C:\Program Files\Reflector\Reflector.exe", string.Format("\"{0}\" /select:JavaScriptClass", filePath));
            //    Environment.Exit(0);
            //}

            // The assembly can no longer be modified - so don't use it again.
            this.ReflectionEmitInfo = null;
#endif
        }

        /// <summary>
        /// Creates a CompilerOptions instance using the script engine properties.
        /// </summary>
        /// <returns> A populated CompilerOptions instance. </returns>
        private Compiler.CompilerOptions CreateOptions()
        {
            return new Compiler.CompilerOptions() { ForceStrictMode = this.ForceStrictMode, EnableDebugging = this.EnableDebugging };
        }



        //     GLOBAL HELPERS
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets a value that indicates whether the global variable with the given name is defined.
        /// </summary>
        /// <param name="variableName"> The name of the variable to check. </param>
        /// <returns> <c>true</c> if the given variable has a value; <c>false</c> otherwise. </returns>
        /// <remarks> Note that a variable that has been set to <c>undefined</c> is still
        /// considered to have a value. </remarks>
        public bool HasGlobalValue(string variableName)
        {
            if (variableName == null)
                throw new ArgumentNullException("variableName");
            return this.Global.HasProperty(variableName);
        }

        /// <summary>
        /// Gets the value of the global variable with the given name.
        /// </summary>
        /// <param name="variableName"> The name of the variable to retrieve the value for. </param>
        /// <returns> The value of the global variable, or <c>null</c> otherwise. </returns>
        public object GetGlobalValue(string variableName)
        {
            if (variableName == null)
                throw new ArgumentNullException("variableName");
            return this.Global.GetPropertyValue(variableName);
        }

        /// <summary>
        /// Gets the value of the global variable with the given name and coerces it to the given
        /// type.
        /// </summary>
        /// <typeparam name="T"> The type to coerce the value to. </typeparam>
        /// <param name="variableName"> The name of the variable to retrieve the value for. </param>
        /// <returns> The value of the global variable, or <c>null</c> otherwise. </returns>
        /// <remarks> Note that <c>null</c> is coerced to the following values: <c>false</c> (if
        /// <typeparamref name="T"/> is <c>bool</c>), 0 (if <typeparamref name="T"/> is <c>int</c>
        /// or <c>double</c>), string.Empty (if <typeparamref name="T"/> is <c>string</c>). </remarks>
        public T GetGlobalValue<T>(string variableName)
        {
            if (variableName == null)
                throw new ArgumentNullException("variableName");
            return TypeConverter.ConvertTo<T>(this, this.Global.GetPropertyValue(variableName));
        }

        /// <summary>
        /// Sets the value of the global variable with the given name.  If the property does not
        /// exist, it will be created.
        /// </summary>
        /// <param name="variableName"> The name of the variable to set. </param>
        /// <param name="value"> The desired value of the variable.  This must be of a supported
        /// type (bool, int, double, string, Null, Undefined or a ObjectInstance-derived type). </param>
        /// <exception cref="JavaScriptException"> The property is read-only or the property does
        /// not exist and the object is not extensible. </exception>
        public void SetGlobalValue(string variableName, object value)
        {
            if (variableName == null)
                throw new ArgumentNullException("variableName");
            if (value == null)
                throw new ArgumentNullException("value");
            this.Global.SetPropertyValue(variableName, value, true);
        }

        /// <summary>
        /// Calls a global function and returns the result.
        /// </summary>
        /// <param name="functionName"> The name of the function to call. </param>
        /// <param name="argumentValues"> The argument values to pass to the function. </param>
        /// <returns> The return value from the function. </returns>
        public object CallGlobalFunction(string functionName, params object[] argumentValues)
        {
            if (functionName == null)
                throw new ArgumentNullException("functionName");
            if (argumentValues == null)
                throw new ArgumentNullException("argumentValues");
            var value = this.Global.GetPropertyValue(functionName);
            if ((value is FunctionInstance) == false)
                throw new InvalidOperationException(string.Format("'{0}' is not a function.", functionName));
            return ((FunctionInstance)value).CallLateBound(null, argumentValues);
        }

        /// <summary>
        /// Calls a global function and returns the result.
        /// </summary>
        /// <typeparam name="T"> The type to coerce the value to. </typeparam>
        /// <param name="functionName"> The name of the function to call. </param>
        /// <param name="argumentValues"> The argument values to pass to the function. </param>
        /// <returns> The return value from the function, coerced to the given type. </returns>
        public T CallGlobalFunction<T>(string functionName, params object[] argumentValues)
        {
            if (functionName == null)
                throw new ArgumentNullException("functionName");
            if (argumentValues == null)
                throw new ArgumentNullException("argumentValues");
            return TypeConverter.ConvertTo<T>(this, CallGlobalFunction(functionName, argumentValues));
        }

        /// <summary>
        /// Sets the global variable with the given name to a function implemented by the provided
        /// delegate.
        /// </summary>
        /// <param name="functionName"> The name of the global variable to set. </param>
        /// <param name="functionDelegate"> The delegate that will implement the function. </param>
        public void SetGlobalFunction(string functionName, Delegate functionDelegate)
        {
            if (functionName == null)
                throw new ArgumentNullException("functionName");
            if (functionDelegate == null)
                throw new ArgumentNullException("functionDelegate");
            SetGlobalValue(functionName, new ClrFunction(this.Function.InstancePrototype, functionDelegate, functionName));
        }




        //     TIMING EVENTS
        //_________________________________________________________________________________________

        /// <summary>
        /// Fires when the compiler starts parsing javascript source code.
        /// </summary>
        public event EventHandler ParsingStarted;

        /// <summary>
        /// Fires when the compiler starts optimizing.
        /// </summary>
        public event EventHandler OptimizationStarted;

        /// <summary>
        /// Fires when the compiler starts generating byte code.
        /// </summary>
        public event EventHandler CodeGenerationStarted;

        /// <summary>
        /// Fires when the compiler starts running javascript code.
        /// </summary>
        public event EventHandler ExecutionStarted;



        //     SCOPE
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new global scope.
        /// </summary>
        /// <returns> A new global scope, with no declared variables. </returns>
        internal Compiler.ObjectScope CreateGlobalScope()
        {
            return Compiler.ObjectScope.CreateGlobalScope(this.Global);
        }



        //     HIDDEN CLASS INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets an empty schema.
        /// </summary>
        internal HiddenClassSchema EmptySchema
        {
            get { return this.emptySchema; }
        }
    }
}
