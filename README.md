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
&nbsp;&nbsp;default function parameters|:x:
&nbsp;&nbsp;rest parameters|:x:
&nbsp;&nbsp;spread (...) operator|:x:
&nbsp;&nbsp;object literal extensions|:x:
&nbsp;&nbsp;for..of loops|:x:
&nbsp;&nbsp;octal and binary literals|:white_check_mark: 4/4
&nbsp;&nbsp;template literals|:x:
&nbsp;&nbsp;RegExp "y" and "u" flags|:x:
&nbsp;&nbsp;destructuring, declarations|:x:
&nbsp;&nbsp;destructuring, assignment|:x:
&nbsp;&nbsp;destructuring, parameters|:x:
&nbsp;&nbsp;Unicode code point escapes|:x:
&nbsp;&nbsp;new.target|:x:
**Bindings**|
&nbsp;&nbsp;const|:x:
&nbsp;&nbsp;let|:x:
&nbsp;&nbsp;block-level function declaration|:x:
**Functions**|
&nbsp;&nbsp;arrow functions|:x:
&nbsp;&nbsp;class|:x:
&nbsp;&nbsp;super|:x:
&nbsp;&nbsp;generators|:x:
**Built-ins**|
&nbsp;&nbsp;typed arrays|10/46
&nbsp;&nbsp;Map|:x:
&nbsp;&nbsp;Set|:x:
&nbsp;&nbsp;WeakMap|:x:
&nbsp;&nbsp;WeakSet|:x:
&nbsp;&nbsp;Proxy|:x:
&nbsp;&nbsp;Reflect|:x:
&nbsp;&nbsp;Promise|:x:
&nbsp;&nbsp;&nbsp;&nbsp;Symbol|:x:
&nbsp;&nbsp;well-known symbols|:x:
**Built-in extensions**|
&nbsp;&nbsp;Object static methods|2/4
&nbsp;&nbsp;function "name" property|:x:
&nbsp;&nbsp;String static methods|1/2
&nbsp;&nbsp;String.prototype methods|6/8
&nbsp;&nbsp;RegExp.prototype properties|1/6
&nbsp;&nbsp;Array static methods|:x:
&nbsp;&nbsp;Array.prototype methods|1/10
&nbsp;&nbsp;Number properties|:white_check_mark: 7/7
&nbsp;&nbsp;Math methods|:white_check_mark: 17/17
**Subclassing**|
&nbsp;&nbsp;Array is subclassable|:x:
&nbsp;&nbsp;RegExp is subclassable|:x:
&nbsp;&nbsp;Function is subclassable|:x:
&nbsp;&nbsp;Promise is subclassable|:x:
&nbsp;&nbsp;miscellaneous subclassables|:x:
**Misc**|
&nbsp;&nbsp;&nbsp;&nbsp;prototype of bound functions|:x:
&nbsp;&nbsp;Proxy, internal 'get' calls|:x:
&nbsp;&nbsp;Proxy, internal 'set' calls|:x:
&nbsp;&nbsp;Proxy, internal 'defineProperty' calls|:x:
&nbsp;&nbsp;Proxy, internal 'deleteProperty' calls|:x:
&nbsp;&nbsp;Proxy, internal 'getOwnPropertyDescriptor' calls|:x:
&nbsp;&nbsp;Proxy, internal 'ownKeys' calls|:x:
&nbsp;&nbsp;Object static methods accept primitives|:white_check_mark: 10/10
&nbsp;&nbsp;own property order|:x:
&nbsp;&nbsp;miscellaneous|2/10
**Annex B**|
&nbsp;&nbsp;non-strict function semantics|:x:
&nbsp;&nbsp;\__proto\__ in object literals|:x:
&nbsp;&nbsp;Object.prototype.\__proto\__|:x:
&nbsp;&nbsp;String.prototype HTML methods|:white_check_mark: 3/3
&nbsp;&nbsp;RegExp.prototype.compile|:white_check_mark: 1/1
&nbsp;&nbsp;RegExp syntax extensions|:x:
&nbsp;&nbsp;HTML-style comments|:x:
