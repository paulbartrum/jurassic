using System;
using System.Collections.Generic;
using Jurassic.Library;
using Jurassic.Compiler;
using System.ComponentModel;

namespace Jurassic
{
    /// <summary>
    /// Represents the JavaScript script engine.  This is the first object that needs to be
    /// instantiated in order to execute javascript code.
    /// </summary>
    public sealed class ScriptEngine
    {
        // Compatibility mode.
        private CompatibilityMode compatibilityMode;

        // The initial hidden class schema.
        private HiddenClassSchema emptySchema;

        // The built-in objects.
        private GlobalObject globalObject;
        private ArrayConstructor arrayConstructor;
        private BooleanConstructor booleanConstructor;
        private DateConstructor dateConstructor;
        private FunctionConstructor functionConstructor;
        private JSONObject jsonObject;
        private MapConstructor mapConstructor;
        private MathObject mathObject;
        private NumberConstructor numberConstructor;
        private ObjectConstructor objectConstructor;
        private PromiseConstructor promiseConstructor;
        private RegExpConstructor regExpConstructor;
        private SetConstructor setConstructor;
        private StringConstructor stringConstructor;
        private SymbolConstructor symbolConstructor;
        private WeakMapConstructor weakMapConstructor;
        private WeakSetConstructor weakSetConstructor;

        // The built-in error objects.
        private ErrorConstructor errorConstructor;
        private ErrorConstructor rangeErrorConstructor;
        private ErrorConstructor typeErrorConstructor;
        private ErrorConstructor syntaxErrorConstructor;
        private ErrorConstructor uriErrorConstructor;
        private ErrorConstructor evalErrorConstructor;
        private ErrorConstructor referenceErrorConstructor;

        // The built-in typed array objects.
        private ArrayBufferConstructor arrayBufferConstructor;
        private DataViewConstructor dataViewConstructor;
        private TypedArrayConstructor int8ArrayConstructor;
        private TypedArrayConstructor uint8ArrayConstructor;
        private TypedArrayConstructor uint8ClampedArrayConstructor;
        private TypedArrayConstructor int16ArrayConstructor;
        private TypedArrayConstructor uint16ArrayConstructor;
        private TypedArrayConstructor int32ArrayConstructor;
        private TypedArrayConstructor uint32ArrayConstructor;
        private TypedArrayConstructor float32ArrayConstructor;
        private TypedArrayConstructor float64ArrayConstructor;

        // Prototypes
        private ObjectInstance baseIteratorPrototype;
        private ObjectInstance stringIteratorPrototype;
        private ObjectInstance mapIteratorPrototype;
        private ObjectInstance setIteratorPrototype;
        private ObjectInstance arrayIteratorPrototype;


        /// <summary>
        /// Initializes a new scripting environment.
        /// </summary>
        public ScriptEngine()
        {
            // Create the initial hidden class schema.  This must be done first.
            this.emptySchema = HiddenClassSchema.CreateEmptySchema();

            // Create the base of the prototype chain.
            var baseObject = ObjectInstance.CreateRootObject(this);

            // Create the function object that second to last in the prototype chain.
            var baseFunction = new ClrStubFunction(baseObject, BaseFunctionImplementation);
            FunctionInstancePrototype = baseFunction;

            // Create the symbol prototype object.
            var baseSymbol = ObjectInstance.CreateRawObject(baseObject);

            // Create the built-in Symbol function first.
            this.symbolConstructor = new SymbolConstructor(baseFunction, baseSymbol);

            // Nullify the method generation helper
            this.MethodGenerationHelper = null;

            // Create the rest of the built-ins.
            this.globalObject = new GlobalObject(baseObject);
            this.mathObject = new MathObject(baseObject);
            this.jsonObject = new JSONObject(baseObject);
            this.objectConstructor = new ObjectConstructor(baseFunction, baseObject);
            this.functionConstructor = new FunctionConstructor(baseFunction, baseFunction);
            this.arrayConstructor = new ArrayConstructor(baseFunction);
            this.booleanConstructor = new BooleanConstructor(baseFunction);
            this.dateConstructor = new DateConstructor(baseFunction);
            this.mapConstructor = new MapConstructor(baseFunction);
            this.numberConstructor = new NumberConstructor(baseFunction);
            this.promiseConstructor = new PromiseConstructor(baseFunction);
            this.regExpConstructor = new RegExpConstructor(baseFunction);
            this.setConstructor = new SetConstructor(baseFunction);
            this.stringConstructor = new StringConstructor(baseFunction);
            this.weakMapConstructor = new WeakMapConstructor(baseFunction);
            this.weakSetConstructor = new WeakSetConstructor(baseFunction);

            // Create the error functions.
            this.errorConstructor = new ErrorConstructor(baseFunction, ErrorType.Error);
            this.rangeErrorConstructor = new ErrorConstructor(baseFunction, ErrorType.RangeError);
            this.typeErrorConstructor = new ErrorConstructor(baseFunction, ErrorType.TypeError);
            this.syntaxErrorConstructor = new ErrorConstructor(baseFunction, ErrorType.SyntaxError);
            this.uriErrorConstructor = new ErrorConstructor(baseFunction, ErrorType.URIError);
            this.evalErrorConstructor = new ErrorConstructor(baseFunction, ErrorType.EvalError);
            this.referenceErrorConstructor = new ErrorConstructor(baseFunction, ErrorType.ReferenceError);

            // Create the typed array functions.
            this.arrayBufferConstructor = new ArrayBufferConstructor(baseFunction);
            this.dataViewConstructor = new DataViewConstructor(baseFunction);
            this.int8ArrayConstructor = new TypedArrayConstructor(baseFunction, TypedArrayType.Int8Array);
            this.uint8ArrayConstructor = new TypedArrayConstructor(baseFunction, TypedArrayType.Uint8Array);
            this.uint8ClampedArrayConstructor = new TypedArrayConstructor(baseFunction, TypedArrayType.Uint8ClampedArray);
            this.int16ArrayConstructor = new TypedArrayConstructor(baseFunction, TypedArrayType.Int16Array);
            this.uint16ArrayConstructor = new TypedArrayConstructor(baseFunction, TypedArrayType.Uint16Array);
            this.int32ArrayConstructor = new TypedArrayConstructor(baseFunction, TypedArrayType.Int32Array);
            this.uint32ArrayConstructor = new TypedArrayConstructor(baseFunction, TypedArrayType.Uint32Array);
            this.float32ArrayConstructor = new TypedArrayConstructor(baseFunction, TypedArrayType.Float32Array);
            this.float64ArrayConstructor = new TypedArrayConstructor(baseFunction, TypedArrayType.Float64Array);

            // Initialize the prototypes for the base of the prototype chain.
            ObjectInstance.InitializePrototypeProperties(baseObject, this.objectConstructor);
            FunctionInstance.InitializePrototypeProperties(baseFunction, this.functionConstructor);
            SymbolInstance.InitializePrototypeProperties(baseSymbol, this.symbolConstructor);

            // Add them as JavaScript-accessible properties of the global instance.
            var globalProperties = this.globalObject.GetGlobalProperties();
            globalProperties.Add(new PropertyNameAndValue("Array", this.arrayConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Boolean", this.booleanConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Date", this.dateConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Function", this.functionConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("JSON", this.jsonObject, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Map", this.mapConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Math", this.mathObject, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Number", this.numberConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Object", this.objectConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Promise", this.promiseConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("RegExp", this.regExpConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Set", this.setConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("String", this.stringConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Symbol", this.symbolConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("WeakMap", this.weakMapConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("WeakSet", this.weakSetConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Error", this.errorConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("RangeError", this.rangeErrorConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("TypeError", this.typeErrorConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("SyntaxError", this.syntaxErrorConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("URIError", this.uriErrorConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("EvalError", this.evalErrorConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("ReferenceError", this.referenceErrorConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("ArrayBuffer", this.arrayBufferConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("DataView", this.dataViewConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Int8Array", this.int8ArrayConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Uint8Array", this.uint8ArrayConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Uint8ClampedArray", this.uint8ClampedArrayConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Int16Array", this.int16ArrayConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Uint16Array", this.uint16ArrayConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Int32Array", this.int32ArrayConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Uint32Array", this.uint32ArrayConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Float32Array", this.float32ArrayConstructor, PropertyAttributes.NonEnumerable));
            globalProperties.Add(new PropertyNameAndValue("Float64Array", this.float64ArrayConstructor, PropertyAttributes.NonEnumerable));

            this.globalObject.FastSetProperties(globalProperties);
        }

        /// <summary>
        /// Implements the behaviour of the function that is the prototype of the Function object.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object BaseFunctionImplementation(ScriptEngine engine, object thisObj, object[] args)
        {
            return Undefined.Value;
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
            get { return this.compatibilityMode; }
            set
            {
                this.compatibilityMode = value;

                // Infinity, NaN and undefined are writable in ECMAScript 3 and read-only in ECMAScript 5.
                var attributes = PropertyAttributes.Sealed;
                if (this.CompatibilityMode == CompatibilityMode.ECMAScript3)
                    attributes = PropertyAttributes.Writable;
                this.Global.FastSetProperty("Infinity", double.PositiveInfinity, attributes, overwriteAttributes: true);
                this.Global.FastSetProperty("NaN", double.NaN, attributes, overwriteAttributes: true);
                this.Global.FastSetProperty("undefined", Undefined.Value, attributes, overwriteAttributes: true);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to disassemble any generated IL and store it
        /// in the associated function.
        /// </summary>
        public bool EnableILAnalysis
        {
            get;
            set;
        }

        /// <summary>
        /// Get or sets a value that indicates the maximum recursion depth of user-defined
        /// functions that is allowed by this script engine.
        /// When a user-defined function exceeds the recursion depth limit, a
        /// <see cref="StackOverflowException"/> will be thrown.
        /// The default value is <c>0</c>, which allows unlimited recursion.
        /// </summary>
        public int RecursionDepthLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Represents a method that transforms a stack frame when formatting the stack trace.
        /// </summary>
        /// <param name="context"></param>
        public delegate void StackFrameTransformDelegate(StackFrameTransformContext context);

        /// <summary>
        /// Gets or sets a delegate that transforms a stack frame when
        /// formatting the stack trace for <see cref="ErrorInstance.Stack"/>.
        /// This can be useful if you are using a source map to map generated lines
        /// to source lines and the stack trace should contain the source line numbers.
        /// </summary>
        public StackFrameTransformDelegate StackFrameTransform
        {
            get;
            set;
        }

        /// <summary>
        /// The instance prototype of the Function object (i.e. Function.InstancePrototype).
        /// </summary>
        /// <remarks>
        /// This property solves a circular reference in the initialization, plus it speeds up
        /// initialization.
        /// </remarks>
        internal FunctionInstance FunctionInstancePrototype
        {
            get;
            private set;
        }

        /// <summary>
        /// The prototype shared by all iterators.
        /// </summary>
        internal ObjectInstance BaseIteratorPrototype
        {
            get
            {
                if (this.baseIteratorPrototype == null)
                {
                    var result = Object.Construct();
                    result.FastSetProperties(new List<PropertyNameAndValue>(1)
                    {
                        new PropertyNameAndValue(Symbol.Iterator, new ClrStubFunction(FunctionInstancePrototype, "[Symbol.iterator]", 0,
                            (engine, thisObj, args) => thisObj), PropertyAttributes.NonEnumerable),
                    });
                    this.baseIteratorPrototype = result;
                }
                return this.baseIteratorPrototype;
            }
        }

        /// <summary>
        /// The prototype of all string iterators.
        /// </summary>
        internal ObjectInstance StringIteratorPrototype
        {
            get
            {
                if (this.stringIteratorPrototype == null)
                    this.stringIteratorPrototype = StringIterator.CreatePrototype(this);
                return this.stringIteratorPrototype;
            }
        }

        /// <summary>
        /// The prototype of all map iterators.
        /// </summary>
        internal ObjectInstance MapIteratorPrototype
        {
            get
            {
                if (this.mapIteratorPrototype == null)
                    this.mapIteratorPrototype = MapIterator.CreatePrototype(this);
                return this.mapIteratorPrototype;
            }
        }

        /// <summary>
        /// The prototype of all set iterators.
        /// </summary>
        internal ObjectInstance SetIteratorPrototype
        {
            get
            {
                if (this.setIteratorPrototype == null)
                    this.setIteratorPrototype = SetIterator.CreatePrototype(this);
                return this.setIteratorPrototype;
            }
        }

        /// <summary>
        /// The prototype of all array iterators.
        /// </summary>
        internal ObjectInstance ArrayIteratorPrototype
        {
            get
            {
                if (this.arrayIteratorPrototype == null)
                    this.arrayIteratorPrototype = ArrayIterator.CreatePrototype(this);
                return this.arrayIteratorPrototype;
            }
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
        /// Gets the built-in Map object.
        /// </summary>
        public MapConstructor Map
        {
            get { return this.mapConstructor; }
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
        /// Gets the built-in Promise object.
        /// </summary>
        public PromiseConstructor Promise
        {
            get { return this.promiseConstructor; }
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
        public SetConstructor Set
        {
            get { return this.setConstructor; }
        }

        /// <summary>
        /// Gets the built-in String object.
        /// </summary>
        public StringConstructor String
        {
            get { return this.stringConstructor; }
        }

        /// <summary>
        /// Gets the built-in Symbol object.
        /// </summary>
        public SymbolConstructor Symbol
        {
            get { return this.symbolConstructor; }
        }

        /// <summary>
        /// Gets the built-in WeakMap object.
        /// </summary>
        public WeakMapConstructor WeakMap
        {
            get { return this.weakMapConstructor; }
        }

        /// <summary>
        /// Gets the built-in WeakSet object.
        /// </summary>
        public WeakSetConstructor WeakSet
        {
            get { return this.weakSetConstructor; }
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

        /// <summary>
        /// Gets the built-in ArrayBuffer object.
        /// </summary>
        public ArrayBufferConstructor ArrayBuffer
        {
            get { return this.arrayBufferConstructor; }
        }

        /// <summary>
        /// Gets the built-in DataView object.
        /// </summary>
        public DataViewConstructor DataView
        {
            get { return this.dataViewConstructor; }
        }

        /// <summary>
        /// Gets the built-in Int8Array object.
        /// </summary>
        public TypedArrayConstructor Int8Array
        {
            get { return this.int8ArrayConstructor; }
        }

        /// <summary>
        /// Gets the built-in Uint8Array object.
        /// </summary>
        public TypedArrayConstructor Uint8Array
        {
            get { return this.uint8ArrayConstructor; }
        }

        /// <summary>
        /// Gets the built-in Uint8ClampedArray object.
        /// </summary>
        public TypedArrayConstructor Uint8ClampedArray
        {
            get { return this.uint8ClampedArrayConstructor; }
        }

        /// <summary>
        /// Gets the built-in Int16Array object.
        /// </summary>
        public TypedArrayConstructor Int16Array
        {
            get { return this.int16ArrayConstructor; }
        }

        /// <summary>
        /// Gets the built-in Uint16Array object.
        /// </summary>
        public TypedArrayConstructor Uint16Array
        {
            get { return this.uint16ArrayConstructor; }
        }

        /// <summary>
        /// Gets the built-in Int32Array object.
        /// </summary>
        public TypedArrayConstructor Int32Array
        {
            get { return this.int32ArrayConstructor; }
        }

        /// <summary>
        /// Gets the built-in Uint32Array object.
        /// </summary>
        public TypedArrayConstructor Uint32Array
        {
            get { return this.uint32ArrayConstructor; }
        }

        /// <summary>
        /// Gets the built-in Float32Array object.
        /// </summary>
        public TypedArrayConstructor Float32Array
        {
            get { return this.float32ArrayConstructor; }
        }

        /// <summary>
        /// Gets the built-in Float64Array object.
        /// </summary>
        public TypedArrayConstructor Float64Array
        {
            get { return this.float64ArrayConstructor; }
        }

        /// <summary>
        /// Gets or sets an auxillary method generation helper. Typically used to generate symbolic information
        /// rich methods.
        /// </summary>
        public IMethodGenerationHelper  MethodGenerationHelper
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether CLR types can be exposed directly to the script engine.  If this is set to 
        /// <c>false</c>, attempting to instantiate CLR types from script may result in exceptions being
        /// thrown in script.
        /// </summary>
        /// <remarks>
        /// <para>This property is intended to prevent script developers from accessing the entire CLR
        /// type system, for security purposes.  When this property is set to <c>false</c>, it should prevent
        /// new instances of CLR types from being exposed to the script engine, even if you have already 
        /// exposed CLR types to the script engine.</para>
        /// </remarks>
        public bool EnableExposedClrTypes
        {
            get;
            set;
        }



        //     EXECUTION
        //_________________________________________________________________________________________
        
        /// <summary>
        /// Compiles the given source code and returns it in a form that can be executed many
        /// times.
        /// </summary>
        /// <param name="source"> The javascript source code to execute. </param>
        /// <returns> A CompiledScript instance, which can be executed as many times as needed. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="source"/> is a <c>null</c> reference. </exception>
        public CompiledScript Compile(ScriptSource source)
        {
            var methodGen = new GlobalMethodGenerator(
                source,                             // The source code.
                CreateOptions());                   // The compiler options.

            // Parse
            this.ParsingStarted?.Invoke(this, EventArgs.Empty);
            methodGen.Parse();

            // Optimize
            this.OptimizationStarted?.Invoke(this, EventArgs.Empty);
            methodGen.Optimize();

            // Generate code
            this.CodeGenerationStarted?.Invoke(this, EventArgs.Empty);
            methodGen.GenerateCode();
            VerifyGeneratedCode();

            return new CompiledScript(methodGen);
        }

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
        /// <exception cref="ArgumentNullException"> <paramref name="source"/> is a <c>null</c> reference. </exception>
        public object Evaluate(ScriptSource source)
        {
            var methodGen = new EvalMethodGenerator(
                ObjectScope.CreateGlobalScope(this.Global), // The variable scope.
                source,                                     // The source code.
                CreateOptions(),                            // The compiler options.
                this.Global);                               // The value of the "this" keyword.

            try
            {
                // Parse
                this.ParsingStarted?.Invoke(this, EventArgs.Empty);
                methodGen.Parse();

                // Optimize
                this.OptimizationStarted?.Invoke(this, EventArgs.Empty);
                methodGen.Optimize();

                // Generate code
                this.CodeGenerationStarted?.Invoke(this, EventArgs.Empty);
                methodGen.GenerateCode();
                VerifyGeneratedCode();
            }
            catch (SyntaxErrorException ex)
            {
                throw new JavaScriptException(this, ErrorType.SyntaxError, ex.Message, ex.LineNumber, ex.SourcePath);
            }

            // Execute
            this.ExecutionStarted?.Invoke(this, EventArgs.Empty);
            var result = methodGen.Execute(this);

            // Normalize the result (convert null to Undefined, double to int, etc).
            return TypeUtilities.NormalizeValue(result);
        }

        /// <summary>
        /// Executes the given source code.  Execution is bound to the global scope.
        /// </summary>
        /// <typeparam name="T"> The type to convert the result to. </typeparam>
        /// <param name="source"> The javascript source code to execute. </param>
        /// <returns> The result of executing the source code. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="source"/> is a <c>null</c> reference. </exception>
        public T Evaluate<T>(ScriptSource source)
        {
            return TypeConverter.ConvertTo<T>(this, Evaluate(source));
        }

        /// <summary>
        /// Executes the given source code.  Execution is bound to the global scope.
        /// </summary>
        /// <param name="code"> The javascript source code to execute. </param>
        public void Execute(string code)
        {
            Execute(new StringScriptSource(code));
        }

        /// <summary>
        /// Executes the given file.  If the file does not have a BOM then it is assumed to be UTF8.
        /// Execution is bound to the global scope.
        /// </summary>
        /// <param name="path"> The path to a javascript file.  This can be a local file path or a
        /// UNC path. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="path"/> is a <c>null</c> reference. </exception>
        public void ExecuteFile(string path)
        {
            ExecuteFile(path, null);
        }

        /// <summary>
        /// Executes the given file.  Execution is bound to the global scope.
        /// </summary>
        /// <param name="path"> The path to a javascript file.  This can be a local file path or a
        /// UNC path. </param>
        /// <param name="encoding"> The character encoding to use if the file lacks a byte order
        /// mark (BOM). </param>
        /// <exception cref="ArgumentNullException"> <paramref name="path"/> is a <c>null</c> reference. </exception>
        public void ExecuteFile(string path, System.Text.Encoding encoding)
        {
            Execute(new FileScriptSource(path, encoding));
        }

        /// <summary>
        /// Executes the given source code.  Execution is bound to the global scope.
        /// </summary>
        /// <param name="source"> The javascript source code to execute. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="source"/> is a <c>null</c> reference. </exception>
        public void Execute(ScriptSource source)
        {
            try
            {
                // Compile the script.
                var compiledScript = Compile(source);

                // ...and execute it.
                this.ExecutionStarted?.Invoke(this, EventArgs.Empty);
                compiledScript.Execute(this);
            }
            catch (SyntaxErrorException ex)
            {
                throw new JavaScriptException(this, ErrorType.SyntaxError, ex.Message, ex.LineNumber, ex.SourcePath);
            }
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
        private CompilerOptions CreateOptions()
        {
            return new CompilerOptions()
            {
                ForceStrictMode = this.ForceStrictMode,
                CompatibilityMode = this.CompatibilityMode,
                EnableILAnalysis = this.EnableILAnalysis,
                MethodGenerationHelper = this.MethodGenerationHelper
            };
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
                throw new ArgumentNullException(nameof(variableName));
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
                throw new ArgumentNullException(nameof(variableName));
            return TypeUtilities.NormalizeValue(this.Global.GetPropertyValue(variableName));
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
                throw new ArgumentNullException(nameof(variableName));
            return TypeConverter.ConvertTo<T>(this, TypeUtilities.NormalizeValue(this.Global.GetPropertyValue(variableName)));
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
                throw new ArgumentNullException(nameof(variableName));

            if (value == null)
                value = Null.Value;
            else if (value != Undefined.Value && value != Null.Value)
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Boolean:
                        break;
                    case TypeCode.Byte:
                        value = (int)(byte)value;
                        break;
                    case TypeCode.Char:
                        value = new string((char)value, 1);
                        break;
                    case TypeCode.Decimal:
                        value = decimal.ToDouble((decimal)value);
                        break;
                    case TypeCode.Double:
                        break;
                    case TypeCode.Int16:
                        value = (int)(short)value;
                        break;
                    case TypeCode.Int32:
                        break;
                    case TypeCode.Int64:
                        value = (double)(long)value;
                        break;
                    case TypeCode.Object:
                        if (value is Type)
                            value = ClrStaticTypeWrapper.FromCache(this, (Type)value);
                        else if ((value is ObjectInstance) == false)
                            value = new ClrInstanceWrapper(this, value);
                        break;
                    case TypeCode.SByte:
                        value = (int)(sbyte)value;
                        break;
                    case TypeCode.Single:
                        value = (double)(float)value;
                        break;
                    case TypeCode.String:
                        break;
                    case TypeCode.UInt16:
                        value = (int)(ushort)value;
                        break;
                    case TypeCode.UInt32:
                        break;
                    case TypeCode.UInt64:
                        value = (double)(ulong)value;
                        break;
                    default:
                        throw new ArgumentException(string.Format("Cannot store value of type {0}.", value.GetType()), nameof(value));
                }
            }
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
                throw new ArgumentNullException(nameof(functionName));
            if (argumentValues == null)
                throw new ArgumentNullException(nameof(argumentValues));
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
                throw new ArgumentNullException(nameof(functionName));
            if (argumentValues == null)
                throw new ArgumentNullException(nameof(argumentValues));
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
                throw new ArgumentNullException(nameof(functionName));
            if (functionDelegate == null)
                throw new ArgumentNullException(nameof(functionDelegate));
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



        //     HIDDEN CLASS INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets an empty schema.
        /// </summary>
        internal HiddenClassSchema EmptySchema
        {
            get { return this.emptySchema; }
        }



        //     EVAL SUPPORT
        //_________________________________________________________________________________________

        //private class EvalCacheKey
        //{
        //    public string Code;
        //    public Scope Scope;
        //    public bool StrictMode;

        //    public override int GetHashCode()
        //    {
        //        int bitValue = 1;
        //        int hashCode = this.Code.GetHashCode();
        //        var scope = this.Scope;
        //        do
        //        {
        //            if (scope is DeclarativeScope)
        //                hashCode ^= bitValue;
        //            scope = scope.ParentScope;
        //            bitValue *= 2;
        //        } while (scope != null);
        //        if (this.StrictMode == true)
        //            hashCode ^= bitValue;
        //        return hashCode;
        //    }

        //    public override bool Equals(object obj)
        //    {
        //        if ((obj is EvalCacheKey) == false)
        //            return false;
        //        var other = (EvalCacheKey) obj;
        //        if (this.Code != other.Code ||
        //            this.StrictMode != other.StrictMode)
        //            return false;
        //        var scope1 = this.Scope;
        //        var scope2 = other.Scope;
        //        do
        //        {
        //            if (scope1.GetType() != scope2.GetType())
        //                return false;
        //            scope1 = scope1.ParentScope;
        //            scope2 = scope2.ParentScope;
        //            if (scope1 == null && scope2 != null)
        //                return false;
        //            if (scope1 != null && scope2 == null)
        //                return false;
        //        } while (scope1 != null);
        //        return true;
        //    }
        //}

        //private Dictionary<EvalCacheKey, WeakReference> evalCache =
        //    new Dictionary<EvalCacheKey, WeakReference>();

        /// <summary>
        /// Evaluates the given javascript source code and returns the result.
        /// </summary>
        /// <param name="code"> The source code to evaluate. </param>
        /// <param name="scope"> The containing scope. </param>
        /// <param name="thisObject"> The value of the "this" keyword in the containing scope. </param>
        /// <param name="strictMode"> Indicates whether the eval statement is being called from
        /// strict mode code. </param>
        /// <returns> The value of the last statement that was executed, or <c>undefined</c> if
        /// there were no executed statements. </returns>
        internal object Eval(string code, Scope scope, object thisObject, bool strictMode)
        {
            // Check if the cache contains the eval already.
            //var key = new EvalCacheKey() { Code = code, Scope = scope, StrictMode = strictMode };
            //WeakReference cachedEvalGenRef;
            //if (evalCache.TryGetValue(key, out cachedEvalGenRef) == true)
            //{
            //    var cachedEvalGen = (EvalMethodGenerator)cachedEvalGenRef.Target;
            //    if (cachedEvalGen != null)
            //    {
            //        // Replace the "this object" before running.
            //        cachedEvalGen.ThisObject = thisObject;

            //        // Execute the cached code.
            //        return ((EvalMethodGenerator)cachedEvalGen).Execute();
            //    }
            //}

            // Parse the eval string into an AST.
            var options = new CompilerOptions() { ForceStrictMode = strictMode };
            var evalGen = new EvalMethodGenerator(
                scope,                                                  // The scope to run the code in.
                new StringScriptSource(code, "eval"),                   // The source code to execute.
                options,                                                // Options.
                thisObject);                                            // The value of the "this" keyword.

            // Make sure the eval cache doesn't get too big.  TODO: add some sort of LRU strategy?
            //if (evalCache.Count > 100)
            //    evalCache.Clear();

            //// Add the eval method generator to the cache.
            //evalCache[key] = new WeakReference(evalGen);

            try
            {

                // Compile and run the eval code.
                return evalGen.Execute(this);

            }
            catch (SyntaxErrorException ex)
            {
                throw new JavaScriptException(this, ErrorType.SyntaxError, ex.Message, ex.LineNumber, ex.SourcePath);
            }
        }



        //     STACK TRACE SUPPORT
        //_________________________________________________________________________________________

        private class StackFrame
        {
            public string Path;
            public string Function;
            public int Line;
            public CallType CallType;
        }

        internal enum CallType
        {
            MethodCall,
            NewOperator,
        }

        private Stack<StackFrame> stackFrames = new Stack<StackFrame>();

        /// <summary>
        /// Creates a stack trace.
        /// </summary>
        /// <param name="errorName"> The name of the error (e.g. "ReferenceError"). </param>
        /// <param name="message"> The error message. </param>
        /// <param name="path"> The path of the javascript source file that is currently executing. </param>
        /// <param name="function"> The name of the currently executing function. </param>
        /// <param name="line"> The line number of the statement that is currently executing. </param>
        internal string FormatStackTrace(string errorName, string message, string path, string function, int line)
        {
            var result = new System.Text.StringBuilder(errorName);
            if (string.IsNullOrEmpty(message) == false)
            {
                result.Append(": ");
                result.Append(message);
            }
            StackFrame[] stackFrameArray = this.stackFrames.ToArray();
            if (path != null || function != null || line != 0)
            {
                CallType callType = stackFrameArray.Length > 0 ? stackFrameArray[0].CallType : CallType.MethodCall;
                AppendStackFrame(result, path, function, line, callType);
            }
            for (int i = 0; i < stackFrameArray.Length; i++)
            {
                var frame = stackFrameArray[i];
                CallType callType = stackFrameArray.Length > i + 1 ? stackFrameArray[i + 1].CallType : CallType.MethodCall;
                AppendStackFrame(result, frame.Path, frame.Function, frame.Line, callType);
            }
            return result.ToString();
        }

        /// <summary>
        /// Appends a stack frame to the end of the given StringBuilder instance.
        /// </summary>
        /// <param name="result"> The StringBuilder to append to. </param>
        /// <param name="path"> The path of the javascript source file. </param>
        /// <param name="function"> The name of the function. </param>
        /// <param name="line"> The line number of the statement. </param>
        /// <param name="parentCallType"> The method by which the current stack frame was created. </param>
        private void AppendStackFrame(System.Text.StringBuilder result, string path, string function, int line, CallType parentCallType)
        {
            // Create a context object which is used for the StackFrameTransform.
            StackFrameTransformContext ctx = new StackFrameTransformContext()
            {
                Line = line,
                Path = path,
                Function = function
            };
            if (StackFrameTransform != null)
                StackFrameTransform(ctx);

            // Check if we need to suppress the current stack frame.
            if (ctx.SuppressStackFrame)
                return;

            result.AppendLine();
            result.Append("    ");
            result.Append("at ");
            if (string.IsNullOrEmpty(ctx.Function) == false)
            {
                if (parentCallType == CallType.NewOperator)
                    result.Append("new ");
                result.Append(ctx.Function);
                result.Append(" (");
            }
            result.Append(ctx.Path ?? "unknown");
            if (ctx.Line > 0)
            {
                result.Append(":");
                result.Append(ctx.Line);
            }
            if (string.IsNullOrEmpty(ctx.Function) == false)
                result.Append(")");
        }

        /// <summary>
        /// Pushes a frame to the javascript stack.  This needs to be called every time there is a
        /// function call.
        /// </summary>
        /// <param name="path"> The path of the javascript source file that contains the function. </param>
        /// <param name="function"> The name of the function that is calling another function. </param>
        /// <param name="line"> The line number of the function call. </param>
        /// <param name="callType"> The type of call that is being made. </param>
        internal void PushStackFrame(string path, string function, int line, CallType callType)
        {
            this.stackFrames.Push(new StackFrame() { Path = path, Function = function, Line = line, CallType = callType });
        }

        /// <summary>
        /// Pops a frame from the javascript stack.
        /// </summary>
        internal void PopStackFrame()
        {
            this.stackFrames.Pop();
        }

        /// <summary>
        /// Checks if the given <see cref="Exception"/> is catchable by JavaScript code with a
        /// <c>catch</c> clause.
        /// Note: This property is public for technical reasons only and should not be used by user code.
        /// </summary>
        /// <param name="ex"> The exception to check. </param>
        /// <returns><c>true</c> if the <see cref="Exception"/> is catchable, <c>false otherwise</c></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CanCatchException(Exception ex)
        {
            var jsException = ex as JavaScriptException;
            if (jsException == null)
                return false;
            return jsException.Engine == this || jsException.Engine == null;
        }



        //     CLRTYPEWRAPPER CACHE
        //_________________________________________________________________________________________

        private Dictionary<Type, ClrInstanceTypeWrapper> instanceTypeWrapperCache;
        private Dictionary<Type, ClrStaticTypeWrapper> staticTypeWrapperCache;

        /// <summary>
        /// Gets a dictionary that can be used to cache ClrInstanceTypeWrapper instances.
        /// </summary>
        internal Dictionary<Type, ClrInstanceTypeWrapper> InstanceTypeWrapperCache
        {
            get
            {
                if (this.instanceTypeWrapperCache == null)
                    this.instanceTypeWrapperCache = new Dictionary<Type, ClrInstanceTypeWrapper>();
                return this.instanceTypeWrapperCache;
            }
        }

        /// <summary>
        /// Gets a dictionary that can be used to cache ClrStaticTypeWrapper instances.
        /// </summary>
        internal Dictionary<Type, ClrStaticTypeWrapper> StaticTypeWrapperCache
        {
            get
            {
                if (this.staticTypeWrapperCache == null)
                    this.staticTypeWrapperCache = new Dictionary<Type, ClrStaticTypeWrapper>();
                return this.staticTypeWrapperCache;
            }
        }
    }
}
