![Jurassic](https://raw.githubusercontent.com/wiki/paulbartrum/jurassic/logo.png)

## What is Jurassic?

Jurassic is an implementation of the ECMAScript language and runtime. It aims to provide
the best performing and most standards-compliant implementation of JavaScript for .NET.
Jurassic is not intended for end-users; instead it is intended to be integrated into .NET
programs. If you are the author of a .NET program, you can use Jurassic to compile and
execute JavaScript code.

## Features
* Supports all ECMAScript 3 and ECMAScript 5 functionality, including ES5 strict mode
* Well tested - passes over five thousand unit tests (with over thirty thousand asserts)
* Simple yet powerful API
* Compiles JavaScript into .NET bytecode (CIL); not an interpreter
* Deployed as a single .NET assembly (no native code)
* Basic support for integrated debugging within Visual Studio
* Uses light-weight code generation, so generated code is fully garbage collected
* Tested on .NET 3.5, .NET 4 and Silverlight

## How do I get it?

Install the [NuGet package](https://www.nuget.org/packages/Jurassic/).

## Usage

See the [wiki](https://github.com/paulbartrum/jurassic/wiki) for full usage details.

## ECMAScript 6 status

Support for ECMAScript 6 is in progress. See http://kangax.github.io/compat-table/es6/ for the definition of each feature.

Feature|Status
-------|------
**Optimization**|
proper tail calls (tail call optimisation)|:x:
**Syntax**|
default function parameters|:x:
rest parameters|:x:
spread (...) operator|:x:
object literal extensions|:x:
for..of loops|:x:
§octal and binary literals|:x:
template literals|:x:
RegExp "y" and "u" flags|:x:
destructuring, declarations|:x:
destructuring, assignment|:x:
destructuring, parameters|:x:
Unicode code point escapes|:x:
new.target|:x:
**Bindings**|
const|:x:
let|:x:
block-level function declaration|:x:
**Functions**|
arrow functions|:x:
class|:x:
super|:x:
generators|:x:
**Built-ins**|
typed arrays|10/46
Map|:x:
Set|:x:
WeakMap|:x:
WeakSet|:x:
Proxy|:x:
Reflect|:x:
Promise|:x:
Symbol|:x:
well-known symbols|:x:
**Built-in extensions**|
Object static methods|2/4
function "name" property|:x:
String static methods|1/2
String.prototype methods|6/8
RegExp.prototype properties|1/6
Array static methods|:x:
Array.prototype methods|1/10
Number properties|:white_check_mark: 7/7
Math methods|:white_check_mark: 17/17
**Subclassing**|
Array is subclassable|:x:
RegExp is subclassable|:x:
Function is subclassable|:x:
Promise is subclassable|:x:
miscellaneous subclassables|:x:
**Misc**|
prototype of bound functions|:x:
Proxy, internal 'get' calls|:x:
Proxy, internal 'set' calls|:x:
Proxy, internal 'defineProperty' calls|:x:
Proxy, internal 'deleteProperty' calls|:x:
Proxy, internal 'getOwnPropertyDescriptor' calls|:x:
Proxy, internal 'ownKeys' calls|:x:
Object static methods accept primitives|:white_check_mark: 10/10
own property order|:x:
miscellaneous|2/10
**Annex B**|
non-strict function semantics|:x:
\__proto\__ in object literals|:x:
Object.prototype.\__proto\__|:x:
String.prototype HTML methods|:white_check_mark: 3/3
RegExp.prototype.compile|:white_check_mark: 1/1
RegExp syntax extensions|:x:
HTML-style comments|:x:
