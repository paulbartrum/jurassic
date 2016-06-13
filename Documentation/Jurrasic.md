#Jurassic JavaScript .Net Documentation

##Basic Hosting Scenarios
###<a name="eval></a>Evaluating an expression

The following code will evaluate a javascript expression:
```C#
var engine = new Jurassic.ScriptEngine();
Console.WriteLine(engine.Evaluate("5 * 10 + 2"));
```

This code will output "52" to the console.

You can include multiple statements in the code string; the result of the last valid expression will be returned.

The above call to Evaluate returns an object. If you want to return a value of a specific type, you can use the coercing version of Evaluate. For example:

```C#
var engine = new Jurassic.ScriptEngine();
Console.WriteLine(engine.Evaluate<int>("1.5 + 2.4"));
```

This code outputs "3" to the console. (If you are wondering why it returns 3, rather than 4, the conversion to an integer uses the javascript type conversion rules, which specify that numbers are rounded towards zero). Type coercian is only supported for certain types.

###Executing a script
Executing a script runs a script from start to finish. For example, to execute the script at "c:\test.js" use the following code:

```C#
var engine = new Jurassic.ScriptEngine();
engine.ExecuteFile(@"c:\test.js");
```

If you have the code to run in a string, use the Execute() method:

```C#
var engine = new Jurassic.ScriptEngine();
engine.Execute("console.log('testing')");
```

Note: executing a script is different from evaluating it in the following ways:
Evaluate() returns a value and Execute() does not.
Execute() runs slightly faster.
Variables declared inside Evaluate() can be deleted whereas variables declared inside Execute() cannot.
The specification uses the terms "global code" and "eval code" to describe these differences.

###Accessing and modifying global variables]
In order to interact with a script, it is useful to be able to modify and retrieve the values of global variables.

For example, say we have the following javascript file (c:\test.js):

```C#
interop = interop + 5
```

The following c# code initializes, then outputs the value of the "interop" variable.

```C#
var engine = new Jurassic.ScriptEngine();
engine.SetGlobalValue("interop", 15);
engine.ExecuteFile(@"c:\test.js");
Console.WriteLine(engine.GetGlobalValue<int>("interop"));
```
This code outputs "20" to the console.

Note that you can only set the value of a variable to a supported type.

###Using the console API
The Firebug console API allows javascript programs to write to the console (along with a few other tricks). It is available for use, but is disabled by default. To enable it, use the following code:

var engine = new Jurassic.ScriptEngine();
engine.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(engine));

Here's an example of using the console object within JavaScript:

var x = 15, y = 'test';
console.log('X is %i, Y is %s', x, y);
This prints "X is 15, Y is test" to the console.

Only a subset of the functionality of the Firebug console API is available at this time.
The available functions are:
1. log
2. debug
3. info
4. warn
5. error
6. assert
7. group
8. groupEnd
9. time
10. timeEnd

In addition, only the following format string specifiers are supported:

|format|type|
|------|--------|
|%i	|integer|
|%s	|string|
|%d	|integer|
|%f	|floating point number|

##Advanced Hosting Scenarios

###Calling a single .NET method from JavaScript

You can call any .NET from JavaScript using the SetGlobalFunction API. For example:

```C#
var engine = new Jurassic.ScriptEngine();
engine.SetGlobalFunction("test", new Func<int, int, int>((a, b) => a + b));
Console.WriteLine(engine.Evaluate<int>("test(5, 6)"));
```

This code will output "11" to the console.

Note the following:
* The delegate parameter types and return type must all be supported.
* An exception thrown within the provided delegate cannot be caught within JavaScript (unless it is a JavaScriptException).
* The above example will not work in Silverlight because of security restrictions. To fix it, pass a delegate to a public method.

###Calling a JavaScript function from .NET
You can also call a JavaScript function from .NET.

```C#
var engine = new Jurassic.ScriptEngine();
engine.Evaluate("function test(a, b) { return a + b }");
Console.WriteLine(engine.CallGlobalFunction<int>("test", 5, 6));
```

This code outputs "11" to the console.

###Exposing a .NET class to JavaScript

####Building a property bag class

To start with, let's create a class that contains a couple of properties.

```C#
using Jurassic;
using Jurassic.Library;

public class AppInfo : ObjectInstance
{
    public AppInfo(ScriptEngine engine)
        : base(engine)
    {
        // Read-write property (name).
        this["name"] = "Test Application";

        // Read-only property (version).
        this.DefineProperty("version", new PropertyDescriptor(5, PropertyAttributes.Sealed), true);
    }
}
```

In this example there are a few things to note:
* Classes that are exposed to JavaScript are required to inherit from Jurassic.Library.ObjectInstance.
* Passing a ScriptEngine to the base class constructor means that the object will have no prototype. Therefore the usual functions (hasOwnProperty, toString) will not be available.
* The class indexer can be used to create read-write properties.
* **DefineProperty** can be used to create read-only properties.

Here's an example of how to create and use the new class:

```C#
var engine = new Jurassic.ScriptEngine();
engine.SetGlobalValue("appInfo", new AppInfo(engine));
Console.WriteLine(engine.Evaluate<string>("appInfo.name + ' ' + appInfo.version"));
```

This will output "Test Application 5" to the console.

####Building a class with static functions

The next step up is to create a class with static functions, similar to how the built-in Math object works. For example, say you want to create a new Math2 object with a log10 function:

```C#
using Jurassic;
using Jurassic.Library;

public class Math2 : ObjectInstance
{
    public Math2(ScriptEngine engine)
        : base(engine)
    {
        this.PopulateFunctions();
    }

    [JSFunction(Name = "log10")]
    public static double Log10(double num)
    {
        return Math.Log10(num);
    }
}
```

Note the following:
* PopulateFunctions searches the class for JSFunction attributes and creates a function for each one it finds.
* The JSFunction attributes allows the JavaScript function name to be different from the .NET method name.
* The parameter types and the return type must be on the list of supported types.
* Please note: This feature requires the latest build from source control and is not yet in the official Downloads Decorating a property with the JSProperty attribute allows properties to be exposed to script as accessors. You can therefore code properties, complete with backers, in CLR, or make read-only properties in CLR code.

Here's an example of how to create and use the new class:

```C#
var engine = new Jurassic.ScriptEngine();
engine.SetGlobalValue("math2", new Math2(engine));
Console.WriteLine(engine.Evaluate<double>("math2.log10(1000)"));
```

This will output "3" to the console.

####Building an instance class

Objects that can be instantiated, like the built-in Number, String, Array and RegExp objects, require two .NET classes, one for the constructor and one for the instance. For example, let's make a JavaScript object that works similar to the .NET Random class (with a seed, since JavaScript doesn't support this):

```C#
using Jurassic;
using Jurassic.Library;

public class RandomConstructor : ClrFunction
{
    public RandomConstructor(ScriptEngine engine)
        : base(engine.Function.InstancePrototype, "Random", new RandomInstance(engine.Object.InstancePrototype))
    {
    }

    [JSConstructorFunction]
    public RandomInstance Construct(int seed)
    {
        return new RandomInstance(this.InstancePrototype, seed);
    }
}

public class RandomInstance : ObjectInstance
{
    private Random random;

    public RandomInstance(ObjectInstance prototype)
        : base(prototype)
    {
        this.PopulateFunctions();
        this.random = new Random(0);
    }

    public RandomInstance(ObjectInstance prototype, int seed)
        : base(prototype)
    {
        this.random = new Random(seed);
    }

    [JSFunction(Name = "nextDouble")]
    public double NextDouble()
    {
        return this.random.NextDouble();
    }
}
```

Note the following:
* You need two classes - one is the constructor (i.e. the function object that you call new on) and one is for the instance object.
* The ClrFunction base class requires three parameters: the prototype for the function object, the name of the function and the prototype for any instances created using the function.
* The JSConstructorFunction attribute marks the method that is called when the new operator is used. You can also use JSCallFunction to mark the method that is called when the function is called directly.
* The RandomInstance class has two constructors - one is used to initialize the prototype, one is used to initialize all other instances. PopulateFunctions should only be called from the prototype object.

Here's an example of how to create and use the new class:

```C#
var engine = new Jurassic.ScriptEngine();
engine.SetGlobalValue("Random", new RandomConstructor(engine));
Console.WriteLine(engine.Evaluate<double>("var rand = new Random(1000); rand.nextDouble()"));
```

This will output "0.151557459100875" to the console (since we are using a seed, the first call to nextDouble() will be the same every time).

This example is much more involved, but it supports all of the advanced JavaScript concepts:
* rand supports the built-in Object functions (hasOwnProperty, toString, etc).
* rand utilizes prototypical inheritance. In this case you have the following prototype chain: random instance (with no properties) -> random prototype (with nextDouble defined) -> object prototype -> null.


###Loading a script from a custom source
If the script you want to execute is not in a file and you do not want to load it into a string, you can create a custom ScriptSource. The following example shows how it is done:

```C#
/// <summary>
/// Represents a string containing script code.
/// </summary>
public class StringScriptSource : ScriptSource
{
    private string code;

    /// <summary>
    /// Creates a new StringScriptSource instance.
    /// </summary>
    /// <param name="code"> The script code. </param>
    public StringScriptSource(string code)
    {
        if (code == null)
            throw new ArgumentNullException("code");
        this.code = code;
    }

    /// <summary>
    /// Gets the path of the source file (either a path on the file system or a URL).  This
    /// can be <c>null</c> if no path is available.
    /// </summary>
    public override string Path
    {
        get { return null; }
    }

    /// <summary>
    /// Returns a reader that can be used to read the source code for the script.
    /// </summary>
    /// <returns> A reader that can be used to read the source code for the script, positioned
    /// at the start of the source code. </returns>
    /// <remarks> If this method is called multiple times then each reader must return the
    /// same source code. </remarks>
    public override TextReader GetReader()
    {
        return new StringReader(this.code);
    }
}
```

###Threading and concurrency
Jurassic is not thread-safe. However, it also does not use any thread-local or global state. Therefore, as long as you ensure that no Jurassic object is called from multiple threads at once, it will work. The easiest way to ensure this is to use multiple script engines (one per thread is ideal). Since non-primitive objects are tied to a single script engine, each script engine is fully isolated from one another. This does make sharing data between script engines hard. One way to share data is to serialize it into a string and then pass it between threads (the JSON object is available to serialize JavaScript objects).

##Other Topics

###Debugging
Jurassic supports the integrated debugging that Visual Studio affords .NET programs.

![debugging example][debugex]

However, the following features are not supported:
* Locals window
* Watch window
* Friendly names in the Call Stack window

To enable debugging, set the EnableDebugging property of the ScriptEngine to true. This will allow debugging within Visual Studio, but it also has the following negative consequences:
* Code generated with EnableDebugging turned on can not be garbage collected. The more scripts you run, the more memory your program will use.
* Generated code will be somewhat slower.

Therefore, do not enable this option for production code.

*Tips:*
* to stop at a breakpoint programmatically (from within JavaScript), use the debugger statement (see MSDN for more details).
* to get the value of a global variable, call scope.GetValue("name of global") (where "scope" is the second parameter of the generated method).

###Supported types

Because of the dynamic nature of JavaScript Jurassic exposes many APIs with method parameters of type System.Object. However, Jurassic supports only a limited subset of the .NET type system. Attempting to use an unsupported type in a Jurassic API may have unintended effects.

The supported types are as follows:

|C# type name|	.NET type name|	JavaScript type name|
|-----------------|---------------------|--------------------|
|bool	|System.Boolean	boolean|
|int	|System.Int32	|number|
|double	|System.Double	|number|
|string	|System.String	|string|
|Jurassic.Null	|Jurassic.Null	|null|
|Jurassic.Undefined	|Jurassic.Undefined	|undefined|
|Jurassic.Library.ObjectInstance (or a derived type)	|Jurassic.Library.ObjectInstance (or a derived type)	|object

###Compatibility mode

The ScriptEngine class has a CompatibilityMode flag which can be used to revert some behaviours to that of ECMAScript 3.

```C#
var engine = new Jurassic.ScriptEngine();
engine.CompatibilityMode = Jurassic.CompatibilityMode.ECMAScript3;
```

Setting this flag has the following effects:
1. Octal literals and octal escape sequences are supported.
2. parseInt() parses octal numbers without requiring an explicit radix.
3. NaN, undefined and Infinity can be modified.
4. The list of reserved keywords becomes much longer (for example, 'abstract' becomes a keyword).
5. "this" is converted to an object at the call site of function calls.

###Performance tips
* Use local variables whenever possible - they are much faster than globals. This means you should always use var to declare variables.
* Avoid the following language features (they tend to disable optimizations):
..* eval
..* arguments
..* with
* Use strict mode - it is slightly faster.

###Non-standard and deprecated functions
A number of functions are either non-standard or were officially deprecated by the Ecma standardization group. These functions are supported for compatibility reasons but they should not be used in new code. Support for these functions may be removed at a future date.

The following functions are deprecated:
* Date.prototype.getYear (use getFullYear instead)
* Date.prototype.setYear (use setFullYear instead)
* Date.prototype.toGMTString (use toUTCString instead)
* Date.prototype.escape (use encodeURI or encodeURIComponent instead)
* Date.prototype.unescape (use decodeURI or decodeURIComponent instead)
* RegExp.prototype.compile (do not use)
* String.prototype.substr (use slice or substring instead)

The following functions are non-standard:
* String.prototype.trimLeft
* String.prototype.trimRight
* String.prototype.anchor
* String.prototype.big
* String.prototype.blink
* String.prototype.bold
* String.prototype.fixed
* String.prototype.fontcolor
* String.prototype.fontsize
* String.prototype.italics
* String.prototype.link
* String.prototype.quote (new in v2)
* String.prototype.small
* String.prototype.strike
* String.prototype.sub
String.prototype.sup

The following properties are non-standard:
* Function.prototype.name
* Function.prototype.displayName
* RegExp.$1 - RegExp.$9 (new in v2.1)
* RegExp.input, RegExp.$_ (new in v2.1)
* RegExp.lastMatch (new in v2.1)
* RegExp.lastParen (new in v2.1)
* RegExp.leftContext (new in v2.1)
* RegExp.rightContext (new in v2.1)

[debugex]: https://github.com/Nanonid/jurassic/raw/master/Documentation/debugging.png
