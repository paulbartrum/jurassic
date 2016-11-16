using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the global Array object.
    /// </summary>
    [TestClass]
    public class ArrayTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // new Array([item0, [item1 [, ... ]]])
            Assert.AreEqual(0, Evaluate("new Array().length"));
            Assert.AreEqual(false, Evaluate("new Array().hasOwnProperty(0)"));
            Assert.AreEqual(1, Evaluate("new Array('test').length"));
            Assert.AreEqual("test", Evaluate("new Array('test')[0]"));
            Assert.AreEqual(2, Evaluate("new Array('test', 'test2').length"));
            Assert.AreEqual("test", Evaluate("new Array('test', 'test2')[0]"));
            Assert.AreEqual("test2", Evaluate("new Array('test', 'test2')[1]"));
            Assert.AreEqual("a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z",
                Evaluate("new Array('a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z').toString()"));

            // new Array(length)
            Assert.AreEqual(5, Evaluate("new Array(5).length"));
            Assert.AreEqual(false, Evaluate("new Array(5).hasOwnProperty(0)"));
            Assert.AreEqual(3, Evaluate("a = [1, 2, 3]; new Array(a.length).length"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Array(-1)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Array(-1.5)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Array(4294967296)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Array(1.5)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Array(NaN)"));

            // Array([item0, [item1 [, ... ]]])
            Assert.AreEqual(0, Evaluate("Array().length"));
            Assert.AreEqual(false, Evaluate("Array().hasOwnProperty(0)"));
            Assert.AreEqual(1, Evaluate("Array('test').length"));
            Assert.AreEqual("test", Evaluate("Array('test')[0]"));
            Assert.AreEqual(2, Evaluate("Array('test', 'test2').length"));
            Assert.AreEqual("test", Evaluate("Array('test', 'test2')[0]"));
            Assert.AreEqual("test2", Evaluate("Array('test', 'test2')[1]"));

            // Array(length)
            Assert.AreEqual(5, Evaluate("Array(5).length"));
            Assert.AreEqual(false, Evaluate("Array(5).hasOwnProperty(0)"));

            // species
            Assert.AreEqual(true, Evaluate("Array[Symbol.species] === Array"));

            // length
            Assert.AreEqual(1, Evaluate("Array.length"));

            // toString
            Assert.AreEqual("function Array() { [native code] }", Evaluate("Array.toString()"));

            // valueOf
            Assert.AreEqual(true, Evaluate("Array.valueOf() === Array"));

            // prototype
            Assert.AreEqual(true, Evaluate("Array.prototype === Object.getPrototypeOf(new Array())"));
            Assert.AreEqual(PropertyAttributes.Sealed, EvaluateAccessibility("Array", "prototype"));

            // constructor
            Assert.AreEqual(true, Evaluate("Array.prototype.constructor === Array"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, EvaluateAccessibility("Array.prototype", "constructor"));
        }

        [TestMethod]
        public void ArrayIndexer()
        {
            // Check basic indexer access.
            Assert.AreEqual(Undefined.Value, Evaluate("[4, 5][-1]"));
            Assert.AreEqual(4, Evaluate("[4, 5][0]"));
            Assert.AreEqual(5, Evaluate("[4, 5][1]"));
            Assert.AreEqual(Undefined.Value, Evaluate("[4, 5][2]"));

            // The array indexer can see the prototype elements.
            Evaluate("var array = [1, ,3]");
            Evaluate("Array.prototype[1] = 'two'");
            Evaluate("Array.prototype[20] = 'twenty'");
            try
            {
                Assert.AreEqual(true, Evaluate("array.hasOwnProperty(0)"));
                Assert.AreEqual(false, Evaluate("array.hasOwnProperty(1)"));
                Assert.AreEqual(true, Evaluate("array.hasOwnProperty(2)"));
                Assert.AreEqual(false, Evaluate("array.hasOwnProperty(20)"));
                Assert.AreEqual(1, Evaluate("array[0]"));
                Assert.AreEqual("two", Evaluate("array[1]"));
                Assert.AreEqual(3, Evaluate("array[2]"));
                Assert.AreEqual("twenty", Evaluate("array[20]"));
                Assert.AreEqual("twenty", Evaluate("array['20']"));
                Assert.AreEqual(Undefined.Value, Evaluate("array['020']"));
                Assert.AreEqual("1,two,3", Evaluate("array.toString()"));
            }
            finally
            {
                Evaluate("delete Array.prototype[1]");
                Evaluate("delete Array.prototype[20]");
            }

            // Array indexes should work even if the array is in the prototype.
            Evaluate("var array = Object.create(['one', 'two', 'three'])");
            Assert.AreEqual(false, Evaluate("array.hasOwnProperty(0)"));
            Assert.AreEqual(3, Evaluate("array.length"));
            Assert.AreEqual("one", Evaluate("array[0]"));
            Assert.AreEqual("one", Evaluate("array['0']"));
            Assert.AreEqual("two", Evaluate("array[1]"));
            Assert.AreEqual("three", Evaluate("array[2]"));

            // Access via a string index.
            Assert.AreEqual("three", Evaluate("var array = ['one', 'two', 'three']; var x = String.fromCharCode(50); array[x]"));
        }

        [TestMethod]
        public void LargeArrays()
        {
            Assert.AreEqual(5, Evaluate("var x = []; x[4294967295] = 5; x[4294967295]"));
            Assert.AreEqual(5, Evaluate("var x = []; x[4294967294] = 5; x[4294967294]"));
        }

        [TestMethod]
        public void length()
        {
            // Setting a new element increases the length.
            Assert.AreEqual(3, Evaluate("var x = [1, 2, 3]; x.length"));
            Assert.AreEqual(4, Evaluate("var x = [1, 2, 3]; x[3] = 4; x.length"));
            Assert.AreEqual(3, Evaluate("var x = [1, 2, 3]; x[0] = 4; x.length"));
            Assert.AreEqual(301, Evaluate("var x = [1, 2, 3]; x[300] = 4; x.length"));
            Assert.AreEqual(false, Evaluate("var x = [1, 2, 3]; x[300] = 4; x.hasOwnProperty(299)"));
            Assert.AreEqual(100000, Evaluate(@"var a = new Array(); a[99999] = ''; a.length"));
            Assert.AreEqual(100000, Evaluate(@"var a = new Array(); a[99999] = ''; a[5] = 2; a.length"));

            // Increasing the length has no effect on the elements of the array.
            Assert.AreEqual(false, Evaluate("var x = [1, 2, 3]; x.length = 10; x.hasOwnProperty(3)"));
            Assert.AreEqual("1,2,3,,,,,,,", Evaluate("var x = [1, 2, 3]; x.length = 10; x.toString()"));

            // Decreasing the length truncates elements.
            Assert.AreEqual(false, Evaluate("var x = [1, 2, 3]; x.length = 2; x.hasOwnProperty(2)"));
            Assert.AreEqual("1,2", Evaluate("var x = [1, 2, 3]; x.length = 2; x.toString()"));
            Assert.AreEqual(false, Evaluate("var x = [1, 2, 3]; x[100] = 1; x.length = 2; x.hasOwnProperty(2)"));
            Assert.AreEqual("1,2", Evaluate("var x = [1, 2, 3]; x[100] = 1; x.length = 2; x.toString()"));
            Assert.AreEqual("1,2,", Evaluate("var x = [1, 2, 3]; x.length = 2; x.length = 3; x.toString()"));

            // Check that a length > 2^31 is reported correctly.
            Assert.AreEqual(4294967295.0, Evaluate("new Array(4294967295).length"));

            // The length property is virtual, but it should behave as though it was a real property.
            Assert.AreEqual(0, Evaluate("length = 0; with (Object.create(['one', 'two', 'three'])) { length = 5 } length"));

            // Must be an integer >= 0 and <= uint.MaxValue
            Assert.AreEqual("RangeError", EvaluateExceptionType("x = []; x.length = -1"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("x = []; x.length = NaN"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("x = []; x.length = 4294967296"));
        }

        [TestMethod]
        public void isArray()
        {
            Assert.AreEqual(false, Evaluate("Array.isArray(0)"));
            Assert.AreEqual(false, Evaluate("Array.isArray({})"));
            Assert.AreEqual(true, Evaluate("Array.isArray([])"));
            Assert.AreEqual(true, Evaluate("Array.isArray(new Array())"));

            // length
            Assert.AreEqual(1, Evaluate("Array.isArray.length"));

            // isArray is generic.
            Assert.AreEqual(true, Evaluate("var $ = {}; $.isArray = Array.isArray; $.isArray([5])"));
        }

        [Ignore]
        [TestMethod]
        public void freezeSealAndPreventExtensions()
        {
            // Array
            Assert.AreEqual(true, Evaluate("var x = [56]; Object.seal(x) === x"));
            Assert.AreEqual(false, Evaluate("delete x[0]"));
            Assert.AreEqual(2, Evaluate("x[0] = 2; x[0]"));
            Assert.AreEqual(Undefined.Value, Evaluate("x[1] = 6; x[1]"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, '0').configurable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, '0').writable"));

            // Array length
            Assert.AreEqual(true, Evaluate("var x = [56]; Object.seal(x) === x"));
            Assert.AreEqual(false, Evaluate("delete x.length"));
            Assert.AreEqual(2, Evaluate("x.length = 2; x.length"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, 'length').configurable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, 'length').writable"));

            // Array
            Assert.AreEqual(true, Evaluate("var x = [56]; Object.freeze(x) === x"));
            Assert.AreEqual(false, Evaluate("delete x[0]"));
            Assert.AreEqual(56, Evaluate("x[0] = 2; x[0]"));
            Assert.AreEqual(Undefined.Value, Evaluate("x[1] = 6; x[1]"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, '0').configurable"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, '0').writable"));

            // Array length
            Assert.AreEqual(true, Evaluate("var x = [56]; Object.freeze(x) === x"));
            Assert.AreEqual(false, Evaluate("delete x.length"));
            Assert.AreEqual(1, Evaluate("x.length = 2; x.length"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, 'length').configurable"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(x, 'length').writable"));

            // Array
            Assert.AreEqual(true, Evaluate("var x = [56]; Object.preventExtensions(x) === x"));
            Assert.AreEqual(2, Evaluate("x[0] = 2; x[0]"));
            Assert.AreEqual(true, Evaluate("delete x[0]"));
            Assert.AreEqual(Undefined.Value, Evaluate("x[1] = 6; x[1]"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, '0').configurable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(x, '0').writable"));
        }



        [TestMethod]
        public void concat()
        {
            Assert.AreEqual("1,2", Evaluate("[1, 2].concat().toString()"));
            Assert.AreEqual("1,2,3", Evaluate("[1, 2].concat(3).toString()"));
            Assert.AreEqual("1,2,3,4", Evaluate("[1, 2].concat([3, 4]).toString()"));
            Assert.AreEqual("1,2,,,6", Evaluate("[1, 2].concat(undefined, null, 6).toString()"));

            // concat should return a new array.
            Evaluate("var x = [1, 2];");
            Evaluate("var y = x.concat();");
            Evaluate("y[0] = 3;");
            Assert.AreEqual(1, Evaluate("x[0]"));
            Assert.AreEqual(3, Evaluate("y[0]"));

            // Try concat with a sparse array.
            Evaluate("var x = [1, 2]; x = x.concat(new Array(2000))");
            Assert.AreEqual(2002, Evaluate("x.length"));

            var x = (ArrayInstance)Evaluate(
                @"var x = [];
                  x[1] = 5;
                  x[22] = 13
                  var y = [];
                  y[0] = 22;
                  y[25] = 34
                  y[66] = 12;
                  x.concat(y)");
            Assert.AreEqual(Undefined.Value, x[0]);
            Assert.AreEqual(5, x[1]);
            Assert.AreEqual(Undefined.Value, x[2]);
            Assert.AreEqual(Undefined.Value, x[21]);
            Assert.AreEqual(13, x[22]);
            Assert.AreEqual(22, x[23]);
            Assert.AreEqual(Undefined.Value, x[24]);
            Assert.AreEqual(Undefined.Value, x[47]);
            Assert.AreEqual(34, x[48]);
            Assert.AreEqual(Undefined.Value, x[49]);
            Assert.AreEqual(Undefined.Value, x[88]);
            Assert.AreEqual(12, x[89]);
            Assert.AreEqual(null, x[90]);

            // concat is generic.
            Evaluate("var x = new Number(5);");
            Evaluate("x.concat = Array.prototype.concat");
            Assert.AreEqual("5,6", Evaluate("x.concat(6).toString()"));

            // concat() can retrieve array items from the prototype.
            Evaluate("Array.prototype[1] = 1");
            try
            {
                Assert.AreEqual(false, Evaluate("x = [0]; x.length = 2; x.hasOwnProperty(1)"));
                Assert.AreEqual("0,1", Evaluate("x = [0]; x.length = 2; x.concat().toString()"));
                Assert.AreEqual(false, Evaluate("x = [0]; x.length = 2; x.concat(); x.hasOwnProperty(1)"));
                Assert.AreEqual(true, Evaluate("x = [0]; x.length = 2; x = x.concat(); x.hasOwnProperty(1)"));
                Assert.AreEqual(1, Evaluate("x = new Array(2000); x = x.concat(); x[1]"));
                Assert.AreEqual(true, Evaluate("x = new Array(2000); x = x.concat(); x.hasOwnProperty(1)"));
            }
            finally
            {
                Evaluate("delete Array.prototype[1]");
            }

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.concat.length"));
        }

        [TestMethod]
        public void join()
        {
            Assert.AreEqual("", Evaluate("[].join()"));
            Assert.AreEqual("1", Evaluate("[1].join()"));
            Assert.AreEqual("1,2", Evaluate("[1, 2].join()"));
            Assert.AreEqual("1,,2", Evaluate("[1, null, 2].join()"));
            Assert.AreEqual("1,,2", Evaluate("[1, undefined, 2].join()"));
            Assert.AreEqual("test", Evaluate("['test'].join()"));
            Assert.AreEqual(",,,,", Evaluate("new Array(5).join()"));
            Assert.AreEqual("1.2", Evaluate("[1,2].join('.')"));
            Assert.AreEqual("1,2", Evaluate("[1, 2].join(undefined)"));

            // join is generic.
            Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            Evaluate("x.join = Array.prototype.join;");
            Assert.AreEqual("5,6", Evaluate("x.join()"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.join.length"));
        }

        [TestMethod]
        public void pop()
        {
            Assert.AreEqual("undefined", Evaluate("typeof([].pop())"));

            Evaluate("var x = [1, 2]");
            Assert.AreEqual(2, Evaluate("x.pop()"));
            Assert.AreEqual(1, Evaluate("x[0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[1])"));
            Assert.AreEqual(1, Evaluate("x.length"));

            // pop is generic.
            Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            Evaluate("x.pop = Array.prototype.pop;");
            Assert.AreEqual(6, Evaluate("x.pop()"));
            Assert.AreEqual(1, Evaluate("x.length"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[1])"));

            // pop() can retrieve array items from the prototype.
            Evaluate("Array.prototype[1] = 1");
            try
            {
                Assert.AreEqual(1, Evaluate("x = [0]; x.length = 2; x.pop();"));
            }
            finally
            {
                Evaluate("delete Array.prototype[1]");
            }

            // length
            Assert.AreEqual(0, Evaluate("Array.prototype.pop.length"));
        }

        [TestMethod]
        public void push()
        {
            Evaluate("var array = [1, 2]");

            Assert.AreEqual(3, Evaluate("array.push(3)"));
            Assert.AreEqual(1, Evaluate("array[0]"));
            Assert.AreEqual(2, Evaluate("array[1]"));
            Assert.AreEqual(3, Evaluate("array[2]"));
            Assert.AreEqual(3, Evaluate("array.length"));

            Assert.AreEqual(5, Evaluate("array.push(4, 5)"));
            Assert.AreEqual(1, Evaluate("array[0]"));
            Assert.AreEqual(2, Evaluate("array[1]"));
            Assert.AreEqual(3, Evaluate("array[2]"));
            Assert.AreEqual(4, Evaluate("array[3]"));
            Assert.AreEqual(5, Evaluate("array[4]"));
            Assert.AreEqual(5, Evaluate("array.length"));

            // push is generic.
            Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            Evaluate("x.push = Array.prototype.push;");
            Assert.AreEqual(3, Evaluate("x.push(7)"));
            Assert.AreEqual(7, Evaluate("x[2]"));
            Assert.AreEqual(3, Evaluate("x.length"));

            // push with very large arrays.
            Assert.AreEqual(1, Evaluate("x = []; x.length = 4294967295; try { x.push(1); } catch (e) { } x[4294967295]"));
            Assert.AreEqual(1, Evaluate("x = []; x.length = 4294967294; try { x.push(1, 2, 3); } catch (e) { } x[4294967294]"));
            Assert.AreEqual(2, Evaluate("x = []; x.length = 4294967294; try { x.push(1, 2, 3); } catch (e) { } x[4294967295]"));
            Assert.AreEqual(3, Evaluate("x = []; x.length = 4294967294; try { x.push(1, 2, 3); } catch (e) { } x[4294967296]"));
            Assert.AreEqual(4294967295.0, Evaluate("x = []; x.length = 4294967294; try { x.push(1, 2, 3); } catch (e) { } x.length"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.push.length"));
        }

        [TestMethod]
        public void reverse()
        {
            Evaluate("var x = [].reverse()");
            Assert.AreEqual("undefined", Evaluate("typeof(x[0])"));
            Assert.AreEqual(0, Evaluate("x.length"));

            Evaluate("var x = [1].reverse()");
            Assert.AreEqual(1, Evaluate("x[0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[1])"));
            Assert.AreEqual(1, Evaluate("x.length"));

            Evaluate("var x = [1, 2].reverse()");
            Assert.AreEqual(2, Evaluate("x[0]"));
            Assert.AreEqual(1, Evaluate("x[1]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[2])"));
            Assert.AreEqual(2, Evaluate("x.length"));

            Evaluate("var x = [1, 2, undefined].reverse()");
            Assert.AreEqual("undefined", Evaluate("typeof(x[0])"));
            Assert.AreEqual(2, Evaluate("x[1]"));
            Assert.AreEqual(1, Evaluate("x[2]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[3])"));
            Assert.AreEqual(3, Evaluate("x.length"));
            Assert.AreEqual(true, Evaluate("x.hasOwnProperty('0')"));

            Evaluate("var x = [1, 2, 3]");
            Evaluate("delete x[2]");
            Assert.AreEqual(true, Evaluate("x.reverse() === x"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[0])"));
            Assert.AreEqual(2, Evaluate("x[1]"));
            Assert.AreEqual(1, Evaluate("x[2]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[3])"));
            Assert.AreEqual(3, Evaluate("x.length"));
            Assert.AreEqual(false, Evaluate("x.hasOwnProperty('0')"));

            // reverse is generic.
            Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            Evaluate("x.reverse = Array.prototype.reverse;");
            Assert.AreEqual(true, Evaluate("x.reverse() === x"));
            Assert.AreEqual(6, Evaluate("x[0]"));
            Assert.AreEqual(5, Evaluate("x[1]"));
            Assert.AreEqual(2, Evaluate("x.length"));

            // reverse is generic.
            Evaluate("var obj = { }"); // 0: true, 2: Infinity, 4: undefined, 5: undefined, 8: 'NaN', 9: '-1' };");
            Evaluate("obj.length = 10;");
            Evaluate("obj.reverse = Array.prototype.reverse;");
            Evaluate("obj[0] = true;");
            Evaluate("obj[2] = Infinity;");
            Evaluate("obj[4] = undefined;");
            Evaluate("obj[5] = undefined;");
            Evaluate("obj[8] = 'NaN';");
            Evaluate("obj[9] = '-1';");
            Assert.AreEqual(true, Evaluate("obj.reverse() === obj"));
            Assert.AreEqual("-1",                       Evaluate("obj[0]"));
            Assert.AreEqual("NaN",                      Evaluate("obj[1]"));
            Assert.AreEqual(Undefined.Value,            Evaluate("obj[2]"));
            Assert.AreEqual(Undefined.Value,            Evaluate("obj[3]"));
            Assert.AreEqual(Undefined.Value,            Evaluate("obj[4]"));
            Assert.AreEqual(Undefined.Value,            Evaluate("obj[5]"));
            Assert.AreEqual(Undefined.Value,            Evaluate("obj[6]"));
            Assert.AreEqual(double.PositiveInfinity,    Evaluate("obj[7]"));
            Assert.AreEqual(Undefined.Value,            Evaluate("obj[8]"));
            Assert.AreEqual(true,                       Evaluate("obj[9]"));

            // length
            Assert.AreEqual(0, Evaluate("Array.prototype.reverse.length"));
        }

        [TestMethod]
        public void shift()
        {
            Evaluate("var x = []");
            Assert.AreEqual("undefined", Evaluate("typeof(x.shift())"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[0])"));
            Assert.AreEqual(0, Evaluate("x.length"));

            Evaluate("var x = [1]");
            Assert.AreEqual(1, Evaluate("x.shift()"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[0])"));
            Assert.AreEqual(0, Evaluate("x.length"));

            Evaluate("var x = [1, 2]");
            Assert.AreEqual(1, Evaluate("x.shift()"));
            Assert.AreEqual(2, Evaluate("x[0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[1])"));
            Assert.AreEqual(1, Evaluate("x.length"));

            Evaluate("var x = [1, 2, undefined]");
            Assert.AreEqual(1, Evaluate("x.shift()"));
            Assert.AreEqual(2, Evaluate("x[0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[1])"));
            Assert.AreEqual(2, Evaluate("x.length"));
            Assert.AreEqual(true, Evaluate("x.hasOwnProperty('1')"));

            Evaluate("var x = [1, 2, 3]");
            Evaluate("delete x[2]");
            Assert.AreEqual(1, Evaluate("x.shift()"));
            Assert.AreEqual(2, Evaluate("x[0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[1])"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[2])"));
            Assert.AreEqual(2, Evaluate("x.length"));
            Assert.AreEqual(false, Evaluate("x.hasOwnProperty('1')"));

            // shift is generic.
            Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            Evaluate("x.shift = Array.prototype.shift;");
            Assert.AreEqual(5, Evaluate("x.shift()"));
            Assert.AreEqual(6, Evaluate("x[0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[1])"));
            Assert.AreEqual(1, Evaluate("x.length"));

            // length
            Assert.AreEqual(0, Evaluate("Array.prototype.shift.length"));
        }

        [TestMethod]
        public void slice()
        {
            Evaluate("var y = [1, 2, 3, 4].slice(0, 2)");
            Assert.AreEqual(1, Evaluate("y[0]"));
            Assert.AreEqual(2, Evaluate("y[1]"));
            Assert.AreEqual(2, Evaluate("y.length"));

            Evaluate("var y = [1, 2, 3, 4].slice(1)");
            Assert.AreEqual(2, Evaluate("y[0]"));
            Assert.AreEqual(3, Evaluate("y[1]"));
            Assert.AreEqual(4, Evaluate("y[2]"));
            Assert.AreEqual(3, Evaluate("y.length"));

            Evaluate("var y = [1, 2, 3, 4].slice(-2, 10)");
            Assert.AreEqual(3, Evaluate("y[0]"));
            Assert.AreEqual(4, Evaluate("y[1]"));
            Assert.AreEqual(2, Evaluate("y.length"));

            Evaluate("var y = [1, 2, 3, 4].slice(0, -2)");
            Assert.AreEqual(1, Evaluate("y[0]"));
            Assert.AreEqual(2, Evaluate("y[1]"));
            Assert.AreEqual(2, Evaluate("y.length"));

            // Should return a copy of the array.
            Evaluate("var x = [1, 2, 3, 4]; var y = x.slice(2, 3)");
            Evaluate("y[0] = 5");
            Assert.AreEqual(1, Evaluate("x[0]"));
            Assert.AreEqual(2, Evaluate("x[1]"));
            Assert.AreEqual(3, Evaluate("x[2]"));
            Assert.AreEqual(4, Evaluate("x[3]"));
            Assert.AreEqual(4, Evaluate("x.length"));

            // Check behavior with undefined.
            Evaluate("var y = [1, 2, undefined].slice(2, 3)");
            Assert.AreEqual("undefined", Evaluate("typeof(y[0])"));
            Assert.AreEqual(1, Evaluate("y.length"));
            Assert.AreEqual(true, Evaluate("y.hasOwnProperty('0')"));

            Evaluate("var x = [1, 2, 3]");
            Evaluate("delete x[2]");
            Evaluate("var y = x.slice(2, 3)");
            Assert.AreEqual("undefined", Evaluate("typeof(y[0])"));
            Assert.AreEqual(1, Evaluate("y.length"));
            Assert.AreEqual(false, Evaluate("y.hasOwnProperty('0')"));

            // slice is generic.
            Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            Evaluate("x.slice = Array.prototype.slice;");
            Evaluate("var y = x.slice(1)");
            Assert.AreEqual(6, Evaluate("y[0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(y[1])"));
            Assert.AreEqual(1, Evaluate("y.length"));

            // length
            Assert.AreEqual(2, Evaluate("Array.prototype.slice.length"));
        }

        private string[] words = new string[] {
            "erostratus", "patisserie", "persimmon", "Miasmatic", "Unyoke", "Sparer", "tempter",
            "vapor", "bath", "Inembryonate", "podesta", "yen-shee", "consulate", "general", "motor",
            "cycle", "de21", "Drilling", "brake", "light", "presoak", "zither", "explainable", "dodoes",
            "unappetizing", "durance", "struthious", "Prolepsis", "platyrrhine", "Knight-errantries",
            "tar", "magician", "43ok", "troublesome", "tensile", "Pteridophyta", "danish", "pastry",
            "Whooping", "swan", "Nobodies", "eye", "muscle", "te87", "Cruentous", "neginoth",
            "Sniffle", "unnail", "domesticate", "Herd", "Beguinage", "bender", "transpacific", "Nardine",
            "Unilocular", "sheraton", "great", "St", "John", "wort", "gilt-edged", "gammer", "Staten",
            "Island", "cense", "mistress", "stand", "by", "inveigh", "against", "coagulation", "Laffer",
            "curve", "hop", "skip", "jump", "permission", "hero", "worship", "footbridge", "battle-scarred",
            "okra", "plant", "ne0", "Spagyric", "ornamented", "selfless", "allomorphic", "poinsettia"
        };

        [TestMethod]
        public void sort()
        {
            // Build up a large array.
            var script = new StringBuilder("var array = [");
            foreach (string word in words)
                script.AppendFormat("'{0}',", word);
            script.Length = script.Length - 1;
            script.Append("]");
            Evaluate(script.ToString());

            // Sort it and check it matches the result of the .NET sort.
            Assert.AreEqual(true, Evaluate("array.sort() === array"));
            Array.Sort(words, (a, b) => string.CompareOrdinal(a, b));
            var array = Evaluate("array");
            for (int i = 0; i < words.Length; i++)
                Assert.AreEqual(words[i], Evaluate(string.Format("array[{0}]", i)));

            // Even numbers are sorted using ASCII sort.
            Evaluate("var array = [1, 21, 4, 11].sort()");
            Assert.AreEqual(1, Evaluate("array[0]"));
            Assert.AreEqual(11, Evaluate("array[1]"));
            Assert.AreEqual(21, Evaluate("array[2]"));
            Assert.AreEqual(4, Evaluate("array[3]"));

            // Unless you use a custom comparer function.
            Evaluate("var array = [1, 21, 4, 11, 5, 3].sort(function(a,b) { return a-b; })");
            Assert.AreEqual(1, Evaluate("array[0]"));
            Assert.AreEqual(3, Evaluate("array[1]"));
            Assert.AreEqual(4, Evaluate("array[2]"));
            Assert.AreEqual(5, Evaluate("array[3]"));
            Assert.AreEqual(11, Evaluate("array[4]"));
            Assert.AreEqual(21, Evaluate("array[5]"));

            // The comparison function doesn't have to return integers.
            Assert.AreEqual("0.1,0.2,0.4,0.5,0.6,0.7", Evaluate("[0.7, 0.1, 0.5, 0.4, 0.6, 0.2].sort(function(a,b) { return a-b; }).toString()"));

            // Try sorting some small arrays.
            Evaluate("var array = ['a', 'c', 'b'].sort()");
            Assert.AreEqual("a", Evaluate("array[0]"));
            Assert.AreEqual("b", Evaluate("array[1]"));
            Assert.AreEqual("c", Evaluate("array[2]"));
            Assert.AreEqual(3, Evaluate("array.length"));
            Evaluate("var array = ['a', 'b'].sort()");
            Assert.AreEqual("a", Evaluate("array[0]"));
            Assert.AreEqual("b", Evaluate("array[1]"));
            Assert.AreEqual(2, Evaluate("array.length"));
            Evaluate("var array = ['b', 'a'].sort()");
            Assert.AreEqual("a", Evaluate("array[0]"));
            Assert.AreEqual("b", Evaluate("array[1]"));
            Assert.AreEqual(2, Evaluate("array.length"));
            Evaluate("var array = ['a'].sort()");
            Assert.AreEqual("a", Evaluate("array[0]"));
            Assert.AreEqual(1, Evaluate("array.length"));
            Evaluate("var array = [].sort()");
            Assert.AreEqual(0, Evaluate("array.length"));

            // Ensure an inconsistant sort routine doesn't cause an infinite loop.
            Assert.AreEqual(0, Evaluate("array.length"));
            Evaluate(@"
                var badCompareFunction = function(x,y) {
                  if (x === undefined) return -1; 
                  if (y === undefined) return 1;
                  return 0;
                }");
            Evaluate(@"var x = new Array(2); x[1] = 1; x.sort(badCompareFunction);");
            Evaluate(@"var x = new Array(2); x[0] = 1; x.sort(badCompareFunction);");

            // The comparer function is not called for elements that are undefined.
            Evaluate(@"
                var myComparefn = function(x,y) {
                  if (x === undefined) return -1; 
                  if (y === undefined) return 1;
                  return 0;
                }
                var x = new Array(2);
                x[1] = 1; 
                x.sort(myComparefn);");
            Assert.AreEqual(2, Evaluate(@"x.length"));
            Assert.AreEqual(1, Evaluate(@"x[0]"));
            Assert.AreEqual(Undefined.Value, Evaluate(@"x[1]"));
            Evaluate(@"
                var myComparefn = function(x,y) {
                  if (x === undefined) return -1; 
                  if (y === undefined) return 1;
                  return 0;
                }
                var x = [undefined, 1];
                x.sort(myComparefn);");
            Assert.AreEqual(2, Evaluate(@"x.length"));
            Assert.AreEqual(1, Evaluate(@"x[0]"));
            Assert.AreEqual(Undefined.Value, Evaluate(@"x[1]"));

            // Passing in undefined as the sort function is okay.
            Assert.AreEqual("1,2", Evaluate(@"[2,1].sort(undefined).toString()"));

            // But passing in anything else is not.
            Assert.AreEqual("TypeError", EvaluateExceptionType(@"[2,1].sort(true)"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.sort.length"));

            // "this" should be the global object in non-strict mode.
            Assert.AreEqual(true, Evaluate(@"
                var global = this;
                var success = false;
                [2,3].sort(function (x, y) {
                    success = this === global;
                });
                success"));

            // "this" should be undefined in strict mode.
            Assert.AreEqual(true, Evaluate(@"
                'use strict';
                var success = false;
                [2,3].sort(function (x, y) {
                    success = this === undefined;
                });
                success"));
        }

        [TestMethod]
        public void splice()
        {
            Evaluate("var array = [1, 21, 4, 11]");
            Evaluate("var deletedItems = array.splice(2, 3, 'a', 'b', 'c')");
            Assert.AreEqual(5, Evaluate("array.length"));
            Assert.AreEqual(1, Evaluate("array[0]"));
            Assert.AreEqual(21, Evaluate("array[1]"));
            Assert.AreEqual("a", Evaluate("array[2]"));
            Assert.AreEqual("b", Evaluate("array[3]"));
            Assert.AreEqual("c", Evaluate("array[4]"));
            Assert.AreEqual(Undefined.Value, Evaluate("array[5]"));
            Assert.AreEqual(2, Evaluate("deletedItems.length"));
            Assert.AreEqual(4, Evaluate("deletedItems[0]"));
            Assert.AreEqual(11, Evaluate("deletedItems[1]"));

            Evaluate("var array = [1, 21, 4, 11]");
            Evaluate("var deletedItems = array.splice(2, 1)");
            Assert.AreEqual(3, Evaluate("array.length"));
            Assert.AreEqual(1, Evaluate("array[0]"));
            Assert.AreEqual(21, Evaluate("array[1]"));
            Assert.AreEqual(11, Evaluate("array[2]"));
            Assert.AreEqual(Undefined.Value, Evaluate("array[3]"));

            // Start index can be negative.
            Evaluate("var array = [1, 21, 4, 11]");
            Evaluate("var deletedItems = array.splice(-2, 1)");
            Assert.AreEqual(3, Evaluate("array.length"));
            Assert.AreEqual(1, Evaluate("array[0]"));
            Assert.AreEqual(21, Evaluate("array[1]"));
            Assert.AreEqual(11, Evaluate("array[2]"));
            Assert.AreEqual(Undefined.Value, Evaluate("array[3]"));

            // Start index can be negative.
            Evaluate("var array = [1, 21, 4, 11]");
            Evaluate("var deletedItems = array.splice(-10, 6)");
            Assert.AreEqual(0, Evaluate("array.length"));
            Assert.AreEqual(Undefined.Value, Evaluate("array[0]"));
            Assert.AreEqual(4, Evaluate("deletedItems.length"));
            Assert.AreEqual("1,21,4,11", Evaluate("deletedItems.toString()"));

            // Start index can be negative.
            Evaluate("var array = [0, 1]");
            Evaluate("var deletedItems = array.splice(-1, -1, 2, 3)");
            Assert.AreEqual(4, Evaluate("array.length"));
            Assert.AreEqual(0, Evaluate("array[0]"));
            Assert.AreEqual(2, Evaluate("array[1]"));
            Assert.AreEqual(3, Evaluate("array[2]"));
            Assert.AreEqual(1, Evaluate("array[3]"));
            Assert.AreEqual(Undefined.Value, Evaluate("array[4]"));
            Assert.AreEqual(0, Evaluate("deletedItems.length"));

            // splice is generic.
            Evaluate("var obj = {0: 0, 1: 1, 2: 2, 3: 3}");
            Evaluate("obj.length = 4;");
            Evaluate("obj.splice = Array.prototype.splice;");
            Evaluate("var deletedItems = obj.splice(0, 3, 4, 5);");
            Assert.AreEqual(3, Evaluate("obj.length"));
            Assert.AreEqual(4, Evaluate("obj[0]"));
            Assert.AreEqual(5, Evaluate("obj[1]"));
            Assert.AreEqual(3, Evaluate("obj[2]"));
            Assert.AreEqual(Undefined.Value, Evaluate("obj[3]"));
            Assert.AreEqual(3, Evaluate("deletedItems.length"));
            Assert.AreEqual(0, Evaluate("deletedItems[0]"));
            Assert.AreEqual(1, Evaluate("deletedItems[1]"));
            Assert.AreEqual(2, Evaluate("deletedItems[2]"));

            // length
            Assert.AreEqual(2, Evaluate("Array.prototype.splice.length"));
        }

        [TestMethod]
        public void unshift()
        {
            // Basic tests.
            Evaluate("var array = [1, 3, 9]");
            Assert.AreEqual(4, Evaluate("array.unshift(10)"));
            Assert.AreEqual(4, Evaluate("array.length"));
            Assert.AreEqual("10,1,3,9", Evaluate("array.toString()"));

            Evaluate("var array = [1, 3, 9]");
            Assert.AreEqual(5, Evaluate("array.unshift(10, 12)"));
            Assert.AreEqual(5, Evaluate("array.length"));
            Assert.AreEqual("10,12,1,3,9", Evaluate("array.toString()"));

            Evaluate("var array = [1, 3, 9]");
            Assert.AreEqual(3, Evaluate("array.unshift()"));
            Assert.AreEqual(3, Evaluate("array.length"));
            Assert.AreEqual("1,3,9", Evaluate("array.toString()"));

            Evaluate("var array = [1, 3, 9]");
            Assert.AreEqual(4, Evaluate("array.unshift(undefined)"));
            Assert.AreEqual(4, Evaluate("array.length"));
            Assert.AreEqual(",1,3,9", Evaluate("array.toString()"));
            Assert.AreEqual(Undefined.Value, Evaluate("array[0]"));
            Assert.AreEqual(true, Evaluate("array.hasOwnProperty(0)"));

            // Check that undefined elements are still undefined after unshift().
            Evaluate("var array = [1, , 9]; array.length = 10");
            Assert.AreEqual(false, Evaluate("array.hasOwnProperty(1)"));
            Assert.AreEqual(11, Evaluate("array.unshift(3)"));
            Assert.AreEqual(11, Evaluate("array.length"));
            Assert.AreEqual("3,1,,9,,,,,,,", Evaluate("array.toString()"));
            Assert.AreEqual(false, Evaluate("array.hasOwnProperty(2)"));

            // Check that elements from the prototype are copied into the array
            // (holes are created using Array.prototype.length).
            Evaluate("var array = [1]; array.length = 3");
            Evaluate("Array.prototype[0] = 'one'");
            Evaluate("Array.prototype[2] = 'three'");
            try
            {
                Assert.AreEqual(3, Evaluate("array.unshift()"));
                Assert.AreEqual(3, Evaluate("array.length"));
                Assert.AreEqual("1,,three", Evaluate("array.toString()"));
                Assert.AreEqual(true, Evaluate("array.hasOwnProperty(0)"));
                Assert.AreEqual(false, Evaluate("array.hasOwnProperty(1)"));
                Assert.AreEqual(true, Evaluate("array.hasOwnProperty(2)"));
            }
            finally
            {
                Evaluate("delete Array.prototype[0]");
                Evaluate("delete Array.prototype[2]");
            }

            // Check that elements from the prototype are copied into the array.
            // (holes are created using [] syntax).
            Evaluate("var array = [1,,,];");
            Evaluate("Array.prototype[0] = 'one'");
            Evaluate("Array.prototype[2] = 'three'");
            try
            {
                Assert.AreEqual(3, Evaluate("array.unshift()"));
                Assert.AreEqual(3, Evaluate("array.length"));
                Assert.AreEqual("1,,three", Evaluate("array.toString()"));
                Assert.AreEqual(true, Evaluate("array.hasOwnProperty(0)"));
                Assert.AreEqual(false, Evaluate("array.hasOwnProperty(1)"));
                Assert.AreEqual(true, Evaluate("array.hasOwnProperty(2)"));
            }
            finally
            {
                Evaluate("delete Array.prototype[0]");
                Evaluate("delete Array.prototype[2]");
            }

            // Check that elements from the prototype are copied into the array.
            // (holes are created using delete).
            Evaluate("var array = [1,2,3]; delete array[1]; delete array[2]");
            Evaluate("Array.prototype[0] = 'one'");
            Evaluate("Array.prototype[2] = 'three'");
            try
            {
                Assert.AreEqual(3, Evaluate("array.unshift()"));
                Assert.AreEqual(3, Evaluate("array.length"));
                Assert.AreEqual("1,,three", Evaluate("array.toString()"));
                Assert.AreEqual(true, Evaluate("array.hasOwnProperty(0)"));
                Assert.AreEqual(false, Evaluate("array.hasOwnProperty(1)"));
                Assert.AreEqual(true, Evaluate("array.hasOwnProperty(2)"));
            }
            finally
            {
                Evaluate("delete Array.prototype[0]");
                Evaluate("delete Array.prototype[2]");
            }

            // Check boundary conditions.
            Assert.AreEqual(4294967295.0, Evaluate("new Array(4294967293).unshift(1, 2)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Array(4294967293).unshift(1, 2, 3)"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.unshift.length"));
        }

        [TestMethod]
        public void indexOf()
        {
            // indexOf(searchElement)
            Assert.AreEqual(2, Evaluate("[3, 2, 1].indexOf(1)"));
            Assert.AreEqual(1, Evaluate("[3, 2, 1].indexOf(2)"));
            Assert.AreEqual(0, Evaluate("[3, 2, 1].indexOf(3)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].indexOf(4)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].indexOf(true)"));
            Assert.AreEqual(1, Evaluate("[3, 1, 2, 1].indexOf(1)"));
            Assert.AreEqual(-1, Evaluate("[3,,1].indexOf(undefined)"));
            Assert.AreEqual(1, Evaluate("[3,undefined,1].indexOf(undefined)"));
            Assert.AreEqual(-1, Evaluate("[3,undefined,1].indexOf(null)"));

            // indexOf(searchElement, fromIndex)
            Assert.AreEqual(2, Evaluate("[3, 2, 1].indexOf(1, 1)"));
            Assert.AreEqual(1, Evaluate("[3, 2, 1].indexOf(2, 1)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].indexOf(3, 1)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].indexOf(4, 1)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].indexOf(2, -1)"));
            Assert.AreEqual(1, Evaluate("[3, 2, 1].indexOf(2, -2)"));
            Assert.AreEqual(0, Evaluate("['a', 'b', 'c', 'a'].indexOf('a', 0)"));
            Assert.AreEqual(3, Evaluate("['a', 'b', 'c', 'a'].indexOf('a', 1)"));
            Assert.AreEqual(0, Evaluate("['a', 'b', 'c', 'a'].indexOf('a', -10)"));
            Assert.AreEqual(-1, Evaluate("['a', 'b', 'c', 'a'].indexOf('a', 10)"));
            Assert.AreEqual(2, Evaluate("[3, 2, 1].indexOf(1, undefined)"));

            Evaluate(@"
                var a = new Array();
                a[100] = 1;
                a[99999] = '';  
                a[10] = new Object();
                a[5555] = 5.5;
                a[123456] = 'str';
                a[5] = 1E+309;");
            Assert.AreEqual(100, Evaluate("a.indexOf(1)"));
            Assert.AreEqual(99999, Evaluate("a.indexOf('')"));
            Assert.AreEqual(123456, Evaluate("a.indexOf('str')"));
            Assert.AreEqual(5, Evaluate("a.indexOf(1E+309)"));
            Assert.AreEqual(5555, Evaluate("a.indexOf(5.5)"));
            Assert.AreEqual(-1, Evaluate("a.indexOf(true)"));
            Assert.AreEqual(-1, Evaluate("a.indexOf(5)"));
            Assert.AreEqual(-1, Evaluate("a.indexOf('str1')"));
            Assert.AreEqual(-1, Evaluate("a.indexOf(null)"));
            Assert.AreEqual(-1, Evaluate("a.indexOf(new Object())"));

            // indexOf(undefined)
            Assert.AreEqual(6, Evaluate("[0, false, , null, 'undefined', , undefined, 0].indexOf(undefined)"));
            Assert.AreEqual(1, Evaluate("var indexOf_nullVariable; [0, indexOf_nullVariable, 0].indexOf(undefined)"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.indexOf.length"));
        }

        [TestMethod]
        public void lastIndexOf()
        {
            // lastIndexOf(searchElement)
            Assert.AreEqual(2, Evaluate("[3, 2, 1].lastIndexOf(1)"));
            Assert.AreEqual(1, Evaluate("[3, 2, 1].lastIndexOf(2)"));
            Assert.AreEqual(0, Evaluate("[3, 2, 1].lastIndexOf(3)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].lastIndexOf(4)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].lastIndexOf(true)"));
            Assert.AreEqual(3, Evaluate("[3, 1, 2, 1].lastIndexOf(1)"));
            Assert.AreEqual(-1, Evaluate("[3,,1].lastIndexOf(undefined)"));
            Assert.AreEqual(1, Evaluate("[3,undefined,1].lastIndexOf(undefined)"));
            Assert.AreEqual(-1, Evaluate("[3,undefined,1].lastIndexOf(null)"));

            // lastIndexOf(searchElement, fromIndex)
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].lastIndexOf(1, 1)"));
            Assert.AreEqual(1, Evaluate("[3, 2, 1].lastIndexOf(2, 1)"));
            Assert.AreEqual(0, Evaluate("[3, 2, 1].lastIndexOf(3, 1)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].lastIndexOf(4, 1)"));
            Assert.AreEqual(0, Evaluate("['a', 'b', 'c', 'a'].lastIndexOf('a', 0)"));
            Assert.AreEqual(0, Evaluate("['a', 'b', 'c', 'a'].lastIndexOf('a', 1)"));
            Assert.AreEqual(-1, Evaluate("['a', 'b', 'c', 'a'].lastIndexOf('a', -10)"));
            Assert.AreEqual(3, Evaluate("['a', 'b', 'c', 'a'].lastIndexOf('a', 10)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].lastIndexOf(1, undefined)"));
            Assert.AreEqual(-1, Evaluate("[3, 2, 1].lastIndexOf(2, undefined)"));
            Assert.AreEqual(0, Evaluate("[3, 2, 1].lastIndexOf(3, undefined)"));

            // lastIndexOf(undefined)
            Assert.AreEqual(6, Evaluate("[0, false, , null, 'undefined', , undefined, 0].lastIndexOf(undefined)"));
            Assert.AreEqual(1, Evaluate("var indexOf_nullVariable; [0, indexOf_nullVariable, 0].lastIndexOf(undefined)"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.lastIndexOf.length"));
        }

        [TestMethod]
        public void every()
        {
            Evaluate("var array = [1, 2, 3, 4]");
            Assert.AreEqual(true, Evaluate("array.every(function(value, index, array) { return value > 0 })"));
            Assert.AreEqual(false, Evaluate("array.every(function(value, index, array) { return value > 1 })"));
            Assert.AreEqual(false, Evaluate("array.every(function(value, index, array) { return value > 4 })"));
            Assert.AreEqual(true, Evaluate("array.every(function(value, index, array) { return this == 0 }, 0)"));
            Assert.AreEqual(false, Evaluate("array.every(function(value, index, array) { return this == 1 }, 0)"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].every(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].every(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].every({})"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.every.length"));
        }

        [TestMethod]
        public void some()
        {
            Evaluate("var array = [1, 2, 3, 4]");
            Assert.AreEqual(true, Evaluate("array.some(function(value, index, array) { return value > 0 })"));
            Assert.AreEqual(true, Evaluate("array.some(function(value, index, array) { return value > 1 })"));
            Assert.AreEqual(false, Evaluate("array.some(function(value, index, array) { return value > 4 })"));
            Assert.AreEqual(true, Evaluate("array.some(function(value, index, array) { return this == 0 }, 0)"));
            Assert.AreEqual(false, Evaluate("array.some(function(value, index, array) { return this == 1 }, 0)"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].some(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].some(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].some({})"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.some.length"));
        }

        [TestMethod]
        public void forEach()
        {
            Assert.AreEqual("2,3,4", Evaluate("array = [1, 2, 3]; array.forEach(function(value, index, array) { array[index] = value + 1 }); array.toString()"));
            Assert.AreEqual("2,3,4", Evaluate("array = [1, 2, 3]; array.forEach(function(value, index, array) { this[index] = value + 1 }, array); array.toString()"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].forEach(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].forEach(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].forEach({})"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.forEach.length"));
        }

        [TestMethod]
        public void map()
        {
            Assert.AreEqual("2,3,4", Evaluate("[1, 2, 3].map(function(value, index, array) { return value + 1; }).toString()"));

            // What if elements are deleted by the callback?
            Assert.AreEqual("0,0,,0", Evaluate("a = [1,2,3,4]; a.map(function() { delete a[2]; return 0; }).toString()"));
            Assert.AreEqual(4, Evaluate("a = [1,2,3,4]; a.map(function() { delete a[2]; return 0; }).length"));
            Assert.AreEqual(4, Evaluate("a = [1,2,3,4]; a.map(function() { delete a[3]; return 0; }).length"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].map(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].map(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].map({})"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.map.length"));
        }

        [TestMethod]
        public void filter()
        {
            Assert.AreEqual("2,3", Evaluate("[1, 2, 3].filter(function(value, index, array) { return value > 1; }).toString()"));
            Assert.AreEqual("3,1,4,1", Evaluate("[3, 1, 4, 1, 5, 9].filter(function(value, index, array) { return value < 5; }).toString()"));
            Assert.AreEqual(0, Evaluate("[3, 1, 4, 1, 5, 9].filter(function(value, index, array) { return value > 9; }).length"));

            Assert.AreEqual("0,1,2", Evaluate("var output = []; [1, 2, 3].filter(function(value, index, array) { output.push(index); return false; }); output.toString()"));
            Assert.AreEqual("1,2,3,1,2,3,1,2,3", Evaluate("var output = []; [1, 2, 3].filter(function(value, index, array) { output.push(array); return false; }); output.toString()"));

            Assert.AreEqual("ho", Evaluate("var output = 'hi'; [1, 2, 3].filter(function(value, index, array) { output = this; return false; }, 'ho'); output.toString()"));
            Assert.AreEqual(Evaluate("this"), Evaluate("var output = 5; [1, 2, 3].filter(function(value, index, array) { output = this; return false; }); output"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].filter(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].filter(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].filter({})"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.filter.length"));
        }

        [TestMethod]
        public void find()
        {
            Assert.AreEqual("2", Evaluate("[1, 2, 3].find(function(value, index, array) { return value > 1; }).toString()"));
            Assert.AreEqual("3", Evaluate("[3, 1, 4, 1, 5, 9].find(function(value, index, array) { return value < 5; }).toString()"));
            Assert.AreEqual(Undefined.Value, Evaluate("[3, 1, 4, 1, 5, 9].find(function(value, index, array) { return value > 9; })"));

            Assert.AreEqual("0,1,2", Evaluate("var output = []; [1, 2, 3].find(function(value, index, array) { output.push(index); return false; }); output.toString()"));
            Assert.AreEqual("1,2,3,1,2,3,1,2,3", Evaluate("var output = []; [1, 2, 3].find(function(value, index, array) { output.push(array); return false; }); output.toString()"));

            Assert.AreEqual("ho", Evaluate("var output = 'hi'; [1, 2, 3].find(function(value, index, array) { output = this; return false; }, 'ho'); output.toString()"));
            Assert.AreEqual(Evaluate("this"), Evaluate("var output = 5; [1, 2, 3].find(function(value, index, array) { output = this; return false; }); output"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].find(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].find(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].find({})"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.find.length"));
        }

        [TestMethod]
        public void reduce()
        {
            Assert.AreEqual(6, Evaluate("[1, 2, 3].reduce(function(accum, value, index, array) { return accum + value; })"));
            Assert.AreEqual(4, Evaluate("[1, 2, 3].reduce(function(accum, value, index, array) { return accum + index; })"));
            Assert.AreEqual(6, Evaluate("[1, 2, 3].reduce(function(accum, value, index, array) { return accum + array[index]; })"));
            Assert.AreEqual("1,2", Evaluate("indices = []; [1, 2, 3].reduce(function(accum, value, index, array) { indices.push(index); }); indices.toString()"));
            
            // this should be undefined in the callback function.
            Assert.AreEqual(Undefined.Value, Evaluate("t = 0; [1, 2, 3].reduce(function(a, b, c, d) { 'use strict'; t = this; }); t"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].reduce(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].reduce(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].reduce({})"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.reduce.length"));
        }

        [TestMethod]
        public void reduceRight()
        {
            Assert.AreEqual(6, Evaluate("[1, 2, 3].reduceRight(function(accum, value, index, array) { return accum + value; })"));
            Assert.AreEqual(4, Evaluate("[1, 2, 3].reduceRight(function(accum, value, index, array) { return accum + index; })"));
            Assert.AreEqual(6, Evaluate("[1, 2, 3].reduceRight(function(accum, value, index, array) { return accum + array[index]; })"));
            Assert.AreEqual("1,0", Evaluate("indices = []; [1, 2, 3].reduceRight(function(accum, value, index, array) { indices.push(index); }); indices.toString()"));

            // this should be undefined in the callback function.
            Assert.AreEqual(Undefined.Value, Evaluate("t = 0; [1, 2, 3].reduceRight(function(a, b, c, d) { 'use strict'; t = this; }); t"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].reduceRight(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].reduceRight(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].reduceRight({})"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.reduceRight.length"));
        }

        [TestMethod]
        public void toLocaleString()
        {
            var originalCulture = System.Globalization.CultureInfo.CurrentCulture;
            try
            {
                // Culture is en-us.
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-us");
                Assert.AreEqual("", Evaluate("[].toLocaleString()"));
                Assert.AreEqual("1", Evaluate("[1].toLocaleString()"));
                Assert.AreEqual("1,2", Evaluate("[1, 2].toLocaleString()"));
                Assert.AreEqual("1,,2", Evaluate("[1, null, 2].toLocaleString()"));
                Assert.AreEqual("1,,2", Evaluate("[1, undefined, 2].toLocaleString()"));
                Assert.AreEqual("test,test2", Evaluate("['test', 'test2'].toLocaleString()"));

                // Culture is de-de (german).
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("de-de");
                Assert.AreEqual("", Evaluate("[].toLocaleString()"));
                Assert.AreEqual("1", Evaluate("[1].toLocaleString()"));
                Assert.AreEqual("1;2", Evaluate("[1, 2].toLocaleString()"));
                Assert.AreEqual("1;;2", Evaluate("[1, null, 2].toLocaleString()"));
                Assert.AreEqual("1;;2", Evaluate("[1, undefined, 2].toLocaleString()"));
                Assert.AreEqual("test;test2", Evaluate("['test', 'test2'].toLocaleString()"));
            }
            finally
            {
                // Revert the culture back to the original value.
                System.Globalization.CultureInfo.CurrentCulture = originalCulture;
            }

            // length
            Assert.AreEqual(0, Evaluate("Array.prototype.toLocaleString.length"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("1,2", Evaluate("[1, 2].toString()"));
            Assert.AreEqual("1,,2", Evaluate("[1, null, 2].toString()"));
            Assert.AreEqual("1,,2", Evaluate("[1, undefined, 2].toString()"));
            Assert.AreEqual("test", Evaluate("['test'].toString()"));

            // In ECMAScript 5 toString() calls join() which in turn calls toString() on each element.
            Evaluate("var x = {};");
            Evaluate("x.join = function() { return 'join overridden'; }");
            Evaluate("x.toString = Array.prototype.toString;");
            Assert.AreEqual("join overridden", Evaluate("x.toString()"));

            // length
            Assert.AreEqual(0, Evaluate("Array.prototype.toString.length"));
        }

        [TestMethod]
        public void copyWithin()
        {
            // Check basic copies.
            Assert.AreEqual("1,2,3,1,2,3", Evaluate("[1, 2, 3, 4, 5, 6].copyWithin(3, 0, 3).toString()"));
            Assert.AreEqual("1,2,3,1,2,3", Evaluate("[1, 2, 3, 4, 5, 6].copyWithin(3, 0).toString()"));
            Assert.AreEqual("4,5,6,4,5,6", Evaluate("[1, 2, 3, 4, 5, 6].copyWithin(0, 3).toString()"));
            Assert.AreEqual("3,4,5,6,5,6", Evaluate("[1, 2, 3, 4, 5, 6].copyWithin(0, 2).toString()"));
            Assert.AreEqual("1,2,3,4,1,2", Evaluate("[1, 2, 3, 4, 5, 6].copyWithin(4, 0).toString()"));
            Assert.AreEqual("1,2,3,2,3,4", Evaluate("[1, 2, 3, 4, 5, 6].copyWithin(3, 1, 4).toString()"));

            // Check bounds.
            Assert.AreEqual("1,2,5,4,5,6", Evaluate("[1, 2, 3, 4, 5, 6].copyWithin(-4, -2, -1).toString()"));
            Assert.AreEqual("1,2,3,4,5,6", Evaluate("[1, 2, 3, 4, 5, 6].copyWithin(3, 20, 21).toString()"));
            Assert.AreEqual("1,2,3,1,2,3", Evaluate("[1, 2, 3, 4, 5, 6].copyWithin(3, -20).toString()"));

            // Check heterogenous values and null, undefined and missing values.
            Assert.AreEqual(",,test,,,test", Evaluate("[null, undefined, 'test', 4, 5, 6].copyWithin(3, 0, 3).toString()"));
            Assert.AreEqual(Null.Value, Evaluate("[null, undefined, 5].copyWithin(2, 0, 1)[2]"));
            Assert.AreEqual(Undefined.Value, Evaluate("[null, undefined, 5].copyWithin(2, 1, 2)[2]"));
            Assert.AreEqual(Undefined.Value, Evaluate("[, undefined, 5].copyWithin(2, 0, 1)[2]"));
            Assert.AreEqual("1,3", Evaluate("Object.keys([, undefined, 1, 2].copyWithin(2, 0, 2)).toString()"));

            // length
            Assert.AreEqual(2, Evaluate("Array.prototype.copyWithin.length"));
        }

        [TestMethod]
        public void fill()
        {
            Assert.AreEqual("0,0,0,0,0,0", Evaluate("[1, 2, 3, 4, 5, 6].fill(0).toString()"));
            Assert.AreEqual("1,0,0,0,0,0", Evaluate("[1, 2, 3, 4, 5, 6].fill(0, 1).toString()"));
            Assert.AreEqual("1,3,3,3,5,6", Evaluate("[1, 2, 3, 4, 5, 6].fill(3, 1, 4).toString()"));
            Assert.AreEqual("1,test,test,test,5,6", Evaluate("[1, 2, 3, 4, 5, 6].fill('test', 1, 4).toString()"));
            Assert.AreEqual("1,2,3,0,0,6", Evaluate("[1, 2, 3, 4, 5, 6].fill(0, -3, -1).toString()"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.fill.length"));
        }

        [TestMethod]
        public void findIndex()
        {
            Assert.AreEqual(1, Evaluate("[1, 2, 3].findIndex(function(value, index, array) { return value > 1; })"));
            Assert.AreEqual(0, Evaluate("[3, 1, 4, 1, 5, 9].findIndex(function(value, index, array) { return value < 5; })"));
            Assert.AreEqual(-1, Evaluate("[3, 1, 4, 1, 5, 9].findIndex(function(value, index, array) { return value > 9; })"));

            Assert.AreEqual("0,1,2", Evaluate("var output = []; [1, 2, 3].findIndex(function(value, index, array) { output.push(index); return false; }); output.toString()"));
            Assert.AreEqual("1,2,3,1,2,3,1,2,3", Evaluate("var output = []; [1, 2, 3].findIndex(function(value, index, array) { output.push(array); return false; }); output.toString()"));

            Assert.AreEqual("ho", Evaluate("var output = 'hi'; [1, 2, 3].findIndex(function(value, index, array) { output = this; return false; }, 'ho'); output.toString()"));
            Assert.AreEqual(Evaluate("this"), Evaluate("var output = 5; [1, 2, 3].findIndex(function(value, index, array) { output = this; return false; }); output"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].findIndex(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].findIndex(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("[1, 2, 3].findIndex({})"));

            // length
            Assert.AreEqual(1, Evaluate("Array.prototype.findIndex.length"));
        }

        [TestMethod]
        public void entries()
        {
            Execute("var i = [11, 7].entries()");

            // Item #1
            Execute("var result = i.next();");
            Assert.AreEqual("0,11", Evaluate("result.value.toString()"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // Item #2
            Execute("var result = i.next();");
            Assert.AreEqual("1,7", Evaluate("result.value.toString()"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // No more items.
            Execute("var result = i.next();");
            Assert.AreEqual(Undefined.Value, Evaluate("result.value"));
            Assert.AreEqual(true, Evaluate("result.done"));

            // toString
            Assert.AreEqual("[object Array Iterator]", Evaluate("new Int8Array([1, 2, 3]).entries().toString()"));
        }

        [TestMethod]
        public void keys()
        {
            Execute("var i = [11, 7].keys()");

            // Item #1
            Execute("var result = i.next();");
            Assert.AreEqual(0, Evaluate("result.value"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // Item #2
            Execute("var result = i.next();");
            Assert.AreEqual(1, Evaluate("result.value"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // No more items.
            Execute("var result = i.next();");
            Assert.AreEqual(Undefined.Value, Evaluate("result.value"));
            Assert.AreEqual(true, Evaluate("result.done"));

            // toString
            Assert.AreEqual("[object Array Iterator]", Evaluate("new Int8Array([1, 2, 3]).keys().toString()"));
        }

        [TestMethod]
        public void values()
        {
            Execute("var i = [11, 7].values()");

            // Item #1
            Execute("var result = i.next();");
            Assert.AreEqual(11, Evaluate("result.value"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // Item #2
            Execute("var result = i.next();");
            Assert.AreEqual(7, Evaluate("result.value"));
            Assert.AreEqual(false, Evaluate("result.done"));

            // No more items.
            Execute("var result = i.next();");
            Assert.AreEqual(Undefined.Value, Evaluate("result.value"));
            Assert.AreEqual(true, Evaluate("result.done"));

            // toString
            Assert.AreEqual("[object Array Iterator]", Evaluate("new Int8Array([1, 2, 3]).values().toString()"));
        }

        [TestMethod]
        public void Symbol_iterator()
        {
            // The Symbol.iterator value is just equal to the values function.
            Assert.AreEqual(true, Evaluate("[][Symbol.iterator] === [].values"));
        }
    }
}
