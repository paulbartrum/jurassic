![Jurassic](https://raw.githubusercontent.com/wiki/paulbartrum/jurassic/logo.png)

[![Build status](https://ci.appveyor.com/api/projects/status/rx2xy5srhmv3kbkd/branch/master?svg=true)](https://ci.appveyor.com/project/paulbartrum/jurassic/branch/master)

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
**Optimisation**|
&nbsp;&nbsp;proper tail calls (tail call optimisation)|:x:
**Syntax**|
&nbsp;&nbsp;default function parameters|:x:
&nbsp;&nbsp;rest parameters|:x:
&nbsp;&nbsp;spread (...) operator|:x:
&nbsp;&nbsp;object literal extensions|:x:
&nbsp;&nbsp;for..of loops|4/9
&nbsp;&nbsp;octal and binary literals|:white_check_mark: 4/4
&nbsp;&nbsp;template literals|2/5
&nbsp;&nbsp;RegExp "y" and "u" flags|:x:
&nbsp;&nbsp;destructuring, declarations|:x:
&nbsp;&nbsp;destructuring, assignment|:x:
&nbsp;&nbsp;destructuring, parameters|:x:
&nbsp;&nbsp;Unicode code point escapes|:white_check_mark: 2/2
&nbsp;&nbsp;new.target|:x:
**Bindings**|
&nbsp;&nbsp;const|2/16
&nbsp;&nbsp;let|1/12
&nbsp;&nbsp;block-level function declaration[13]|:x:
**Functions**|
&nbsp;&nbsp;arrow functions|:x:
&nbsp;&nbsp;class|:x:
&nbsp;&nbsp;super|:x:
&nbsp;&nbsp;generators|:x:
**Built-ins**|
&nbsp;&nbsp;typed arrays|44/46
&nbsp;&nbsp;Map|17/19
&nbsp;&nbsp;Set|17/19
&nbsp;&nbsp;WeakMap|10/12
&nbsp;&nbsp;WeakSet|9/11
&nbsp;&nbsp;Proxy[18]|:x:
&nbsp;&nbsp;Reflect[20]|:x:
&nbsp;&nbsp;Promise|:white_check_mark:
&nbsp;&nbsp;Symbol|5/11
&nbsp;&nbsp;well-known symbols[21]|5/26
**Built-in extensions**|
&nbsp;&nbsp;Object static methods|3/4
&nbsp;&nbsp;function "name" property|5/17
&nbsp;&nbsp;String static methods|:white_check_mark: 2/2
&nbsp;&nbsp;String.prototype methods|:white_check_mark: 10/10
&nbsp;&nbsp;RegExp.prototype properties|2/6
&nbsp;&nbsp;Array static methods|1/11
&nbsp;&nbsp;Array.prototype methods|9/10
&nbsp;&nbsp;Number properties|:white_check_mark: 7/7
&nbsp;&nbsp;Math methods|:white_check_mark: 17/17
&nbsp;&nbsp;Date.prototype[Symbol.toPrimitive]|:white_check_mark: 1/1
**Subclassing**|
&nbsp;&nbsp;Array is subclassable|:x:
&nbsp;&nbsp;RegExp is subclassable|:x:
&nbsp;&nbsp;Function is subclassable|:x:
&nbsp;&nbsp;Promise is subclassable|:x:
&nbsp;&nbsp;miscellaneous subclassables|:x:
**Misc**|
&nbsp;&nbsp;prototype of bound functions|:x:
&nbsp;&nbsp;Proxy, internal 'get' calls|:x:
&nbsp;&nbsp;Proxy, internal 'set' calls|:x:
&nbsp;&nbsp;Proxy, internal 'defineProperty' calls|:x:
&nbsp;&nbsp;Proxy, internal 'deleteProperty' calls|:x:
&nbsp;&nbsp;Proxy, internal 'getOwnPropertyDescriptor' calls|:x:
&nbsp;&nbsp;Proxy, internal 'ownKeys' calls|:x:
&nbsp;&nbsp;Object static methods accept primitives|7/10
&nbsp;&nbsp;own property order|2/7
&nbsp;&nbsp;miscellaneous|6/10
**Annex b**|
&nbsp;&nbsp;non-strict function semantics[29]|2/3
&nbsp;&nbsp;\_\_proto\_\_ in object literals[30]|:x:
&nbsp;&nbsp;Object.prototype.\_\_proto\_\_|1/6
&nbsp;&nbsp;String.prototype HTML methods|:white_check_mark: 3/3
&nbsp;&nbsp;RegExp.prototype.compile|:white_check_mark: 1/1
&nbsp;&nbsp;RegExp syntax extensions|4/8
&nbsp;&nbsp;HTML-style comments|:x:
