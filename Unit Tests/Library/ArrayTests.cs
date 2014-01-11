using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the global Array object.
    /// </summary>
    [TestClass]
    public class ArrayTests
    {
        [TestMethod]
        public void Constructor()
        {
            // new Array([item0, [item1 [, ... ]]])
            Assert.AreEqual(0, TestUtils.Evaluate("new Array().length"));
            Assert.AreEqual(false, TestUtils.Evaluate("new Array().hasOwnProperty(0)"));
            Assert.AreEqual(1, TestUtils.Evaluate("new Array('test').length"));
            Assert.AreEqual("test", TestUtils.Evaluate("new Array('test')[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("new Array('test', 'test2').length"));
            Assert.AreEqual("test", TestUtils.Evaluate("new Array('test', 'test2')[0]"));
            Assert.AreEqual("test2", TestUtils.Evaluate("new Array('test', 'test2')[1]"));
            Assert.AreEqual("a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z",
                TestUtils.Evaluate("new Array('a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z').toString()"));

            // new Array(length)
            Assert.AreEqual(5, TestUtils.Evaluate("new Array(5).length"));
            Assert.AreEqual(false, TestUtils.Evaluate("new Array(5).hasOwnProperty(0)"));
            Assert.AreEqual(3, TestUtils.Evaluate("a = [1, 2, 3]; new Array(a.length).length"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("new Array(-1)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("new Array(-1.5)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("new Array(4294967296)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("new Array(1.5)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("new Array(NaN)"));

            // Array([item0, [item1 [, ... ]]])
            Assert.AreEqual(0, TestUtils.Evaluate("Array().length"));
            Assert.AreEqual(false, TestUtils.Evaluate("Array().hasOwnProperty(0)"));
            Assert.AreEqual(1, TestUtils.Evaluate("Array('test').length"));
            Assert.AreEqual("test", TestUtils.Evaluate("Array('test')[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("Array('test', 'test2').length"));
            Assert.AreEqual("test", TestUtils.Evaluate("Array('test', 'test2')[0]"));
            Assert.AreEqual("test2", TestUtils.Evaluate("Array('test', 'test2')[1]"));

            // Array(length)
            Assert.AreEqual(5, TestUtils.Evaluate("Array(5).length"));
            Assert.AreEqual(false, TestUtils.Evaluate("Array(5).hasOwnProperty(0)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.length"));

            // toString
            Assert.AreEqual("function Array() { [native code] }", TestUtils.Evaluate("Array.toString()"));

            // valueOf
            Assert.AreEqual(true, TestUtils.Evaluate("Array.valueOf() === Array"));

            // prototype
            Assert.AreEqual(true, TestUtils.Evaluate("Array.prototype === Object.getPrototypeOf(new Array())"));
            Assert.AreEqual(PropertyAttributes.Sealed, TestUtils.EvaluateAccessibility("Array", "prototype"));

            // constructor
            Assert.AreEqual(true, TestUtils.Evaluate("Array.prototype.constructor === Array"));
            Assert.AreEqual(PropertyAttributes.NonEnumerable, TestUtils.EvaluateAccessibility("Array.prototype", "constructor"));
        }

        [TestMethod]
        public void ArrayIndexer()
        {
            // Check basic indexer access.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("[4, 5][-1]"));
            Assert.AreEqual(4, TestUtils.Evaluate("[4, 5][0]"));
            Assert.AreEqual(5, TestUtils.Evaluate("[4, 5][1]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("[4, 5][2]"));

            // The array indexer can see the prototype elements.
            TestUtils.Evaluate("var array = [1, ,3]");
            TestUtils.Evaluate("Array.prototype[1] = 'two'");
            TestUtils.Evaluate("Array.prototype[20] = 'twenty'");
            try
            {
                Assert.AreEqual(true, TestUtils.Evaluate("array.hasOwnProperty(0)"));
                Assert.AreEqual(false, TestUtils.Evaluate("array.hasOwnProperty(1)"));
                Assert.AreEqual(true, TestUtils.Evaluate("array.hasOwnProperty(2)"));
                Assert.AreEqual(false, TestUtils.Evaluate("array.hasOwnProperty(20)"));
                Assert.AreEqual(1, TestUtils.Evaluate("array[0]"));
                Assert.AreEqual("two", TestUtils.Evaluate("array[1]"));
                Assert.AreEqual(3, TestUtils.Evaluate("array[2]"));
                Assert.AreEqual("twenty", TestUtils.Evaluate("array[20]"));
                Assert.AreEqual("twenty", TestUtils.Evaluate("array['20']"));
                Assert.AreEqual("1,two,3", TestUtils.Evaluate("array.toString()"));
            }
            finally
            {
                TestUtils.Evaluate("delete Array.prototype[1]");
                TestUtils.Evaluate("delete Array.prototype[20]");
            }

            // Array indexes should work even if the array is in the prototype.
            TestUtils.Evaluate("var array = Object.create(['one', 'two', 'three'])");
            Assert.AreEqual(false, TestUtils.Evaluate("array.hasOwnProperty(0)"));
            Assert.AreEqual(3, TestUtils.Evaluate("array.length"));
            Assert.AreEqual("one", TestUtils.Evaluate("array[0]"));
            Assert.AreEqual("one", TestUtils.Evaluate("array['0']"));
            Assert.AreEqual("two", TestUtils.Evaluate("array[1]"));
            Assert.AreEqual("three", TestUtils.Evaluate("array[2]"));

            // Access via a string index.
            Assert.AreEqual("three", TestUtils.Evaluate("var array = ['one', 'two', 'three']; var x = String.fromCharCode(50); array[x]"));
        }

        [TestMethod]
        public void LargeArrays()
        {
            Assert.AreEqual(5, TestUtils.Evaluate("var x = []; x[4294967295] = 5; x[4294967295]"));
            Assert.AreEqual(5, TestUtils.Evaluate("var x = []; x[4294967294] = 5; x[4294967294]"));
        }

        [TestMethod]
        public void length()
        {
            // Setting a new element increases the length.
            Assert.AreEqual(3, TestUtils.Evaluate("var x = [1, 2, 3]; x.length"));
            Assert.AreEqual(4, TestUtils.Evaluate("var x = [1, 2, 3]; x[3] = 4; x.length"));
            Assert.AreEqual(3, TestUtils.Evaluate("var x = [1, 2, 3]; x[0] = 4; x.length"));
            Assert.AreEqual(301, TestUtils.Evaluate("var x = [1, 2, 3]; x[300] = 4; x.length"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = [1, 2, 3]; x[300] = 4; x.hasOwnProperty(299)"));
            Assert.AreEqual(100000, TestUtils.Evaluate(@"var a = new Array(); a[99999] = ''; a.length"));
            Assert.AreEqual(100000, TestUtils.Evaluate(@"var a = new Array(); a[99999] = ''; a[5] = 2; a.length"));

            // Increasing the length has no effect on the elements of the array.
            Assert.AreEqual(false, TestUtils.Evaluate("var x = [1, 2, 3]; x.length = 10; x.hasOwnProperty(3)"));
            Assert.AreEqual("1,2,3,,,,,,,", TestUtils.Evaluate("var x = [1, 2, 3]; x.length = 10; x.toString()"));

            // Decreasing the length truncates elements.
            Assert.AreEqual(false, TestUtils.Evaluate("var x = [1, 2, 3]; x.length = 2; x.hasOwnProperty(2)"));
            Assert.AreEqual("1,2", TestUtils.Evaluate("var x = [1, 2, 3]; x.length = 2; x.toString()"));
            Assert.AreEqual(false, TestUtils.Evaluate("var x = [1, 2, 3]; x[100] = 1; x.length = 2; x.hasOwnProperty(2)"));
            Assert.AreEqual("1,2", TestUtils.Evaluate("var x = [1, 2, 3]; x[100] = 1; x.length = 2; x.toString()"));
            Assert.AreEqual("1,2,", TestUtils.Evaluate("var x = [1, 2, 3]; x.length = 2; x.length = 3; x.toString()"));

            // Check that a length > 2^31 is reported correctly.
            Assert.AreEqual(4294967295.0, TestUtils.Evaluate("new Array(4294967295).length"));

            // The length property is virtual, but it should behave as though it was a real property.
            Assert.AreEqual(0, TestUtils.Evaluate("length = 0; with (Object.create(['one', 'two', 'three'])) { length = 5 } length"));

            // Must be an integer >= 0 and <= uint.MaxValue
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("x = []; x.length = -1"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("x = []; x.length = NaN"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("x = []; x.length = 4294967296"));
        }

        [TestMethod]
        public void isArray()
        {
            Assert.AreEqual(false, TestUtils.Evaluate("Array.isArray(0)"));
            Assert.AreEqual(false, TestUtils.Evaluate("Array.isArray({})"));
            Assert.AreEqual(true, TestUtils.Evaluate("Array.isArray([])"));
            Assert.AreEqual(true, TestUtils.Evaluate("Array.isArray(new Array())"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.isArray.length"));

            // isArray is generic.
            Assert.AreEqual(true, TestUtils.Evaluate("var $ = {}; $.isArray = Array.isArray; $.isArray([5])"));
        }

        [Ignore]
        [TestMethod]
        public void freezeSealAndPreventExtensions()
        {
            // Array
            Assert.AreEqual(true, TestUtils.Evaluate("var x = [56]; Object.seal(x) === x"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete x[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("x[0] = 2; x[0]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x[1] = 6; x[1]"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, '0').configurable"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, '0').writable"));

            // Array length
            Assert.AreEqual(true, TestUtils.Evaluate("var x = [56]; Object.seal(x) === x"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete x.length"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.length = 2; x.length"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'length').configurable"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'length').writable"));

            // Array
            Assert.AreEqual(true, TestUtils.Evaluate("var x = [56]; Object.freeze(x) === x"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete x[0]"));
            Assert.AreEqual(56, TestUtils.Evaluate("x[0] = 2; x[0]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x[1] = 6; x[1]"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, '0').configurable"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, '0').writable"));

            // Array length
            Assert.AreEqual(true, TestUtils.Evaluate("var x = [56]; Object.freeze(x) === x"));
            Assert.AreEqual(false, TestUtils.Evaluate("delete x.length"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.length = 2; x.length"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'length').configurable"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, 'length').writable"));

            // Array
            Assert.AreEqual(true, TestUtils.Evaluate("var x = [56]; Object.preventExtensions(x) === x"));
            Assert.AreEqual(2, TestUtils.Evaluate("x[0] = 2; x[0]"));
            Assert.AreEqual(true, TestUtils.Evaluate("delete x[0]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("x[1] = 6; x[1]"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, '0').configurable"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getOwnPropertyDescriptor(x, '0').writable"));
        }



        [TestMethod]
        public void concat()
        {
            Assert.AreEqual("1,2", TestUtils.Evaluate("[1, 2].concat().toString()"));
            Assert.AreEqual("1,2,3", TestUtils.Evaluate("[1, 2].concat(3).toString()"));
            Assert.AreEqual("1,2,3,4", TestUtils.Evaluate("[1, 2].concat([3, 4]).toString()"));
            Assert.AreEqual("1,2,,,6", TestUtils.Evaluate("[1, 2].concat(undefined, null, 6).toString()"));

            // concat should return a new array.
            TestUtils.Evaluate("var x = [1, 2];");
            TestUtils.Evaluate("var y = x.concat();");
            TestUtils.Evaluate("y[0] = 3;");
            Assert.AreEqual(1, TestUtils.Evaluate("x[0]"));
            Assert.AreEqual(3, TestUtils.Evaluate("y[0]"));

            // Try concat with a sparse array.
            TestUtils.Evaluate("var x = [1, 2]; x = x.concat(new Array(2000))");
            Assert.AreEqual(2002, TestUtils.Evaluate("x.length"));

            var x = (ArrayInstance)TestUtils.Evaluate(
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
            TestUtils.Evaluate("var x = new Number(5);");
            TestUtils.Evaluate("x.concat = Array.prototype.concat");
            Assert.AreEqual("5,6", TestUtils.Evaluate("x.concat(6).toString()"));

            // concat() can retrieve array items from the prototype.
            TestUtils.Evaluate("Array.prototype[1] = 1");
            try
            {
                Assert.AreEqual(false, TestUtils.Evaluate("x = [0]; x.length = 2; x.hasOwnProperty(1)"));
                Assert.AreEqual("0,1", TestUtils.Evaluate("x = [0]; x.length = 2; x.concat().toString()"));
                Assert.AreEqual(false, TestUtils.Evaluate("x = [0]; x.length = 2; x.concat(); x.hasOwnProperty(1)"));
                Assert.AreEqual(true, TestUtils.Evaluate("x = [0]; x.length = 2; x = x.concat(); x.hasOwnProperty(1)"));
                Assert.AreEqual(1, TestUtils.Evaluate("x = new Array(2000); x = x.concat(); x[1]"));
                Assert.AreEqual(true, TestUtils.Evaluate("x = new Array(2000); x = x.concat(); x.hasOwnProperty(1)"));
            }
            finally
            {
                TestUtils.Evaluate("delete Array.prototype[1]");
            }

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.concat.length"));
        }

        [TestMethod]
        public void join()
        {
            Assert.AreEqual("", TestUtils.Evaluate("[].join()"));
            Assert.AreEqual("1", TestUtils.Evaluate("[1].join()"));
            Assert.AreEqual("1,2", TestUtils.Evaluate("[1, 2].join()"));
            Assert.AreEqual("1,,2", TestUtils.Evaluate("[1, null, 2].join()"));
            Assert.AreEqual("1,,2", TestUtils.Evaluate("[1, undefined, 2].join()"));
            Assert.AreEqual("test", TestUtils.Evaluate("['test'].join()"));
            Assert.AreEqual(",,,,", TestUtils.Evaluate("new Array(5).join()"));
            Assert.AreEqual("1.2", TestUtils.Evaluate("[1,2].join('.')"));
            Assert.AreEqual("1,2", TestUtils.Evaluate("[1, 2].join(undefined)"));

            // join is generic.
            TestUtils.Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            TestUtils.Evaluate("x.join = Array.prototype.join;");
            Assert.AreEqual("5,6", TestUtils.Evaluate("x.join()"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.join.length"));
        }

        [TestMethod]
        public void pop()
        {
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof([].pop())"));

            TestUtils.Evaluate("var x = [1, 2]");
            Assert.AreEqual(2, TestUtils.Evaluate("x.pop()"));
            Assert.AreEqual(1, TestUtils.Evaluate("x[0]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[1])"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.length"));

            // pop is generic.
            TestUtils.Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            TestUtils.Evaluate("x.pop = Array.prototype.pop;");
            Assert.AreEqual(6, TestUtils.Evaluate("x.pop()"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.length"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[1])"));

            // pop() can retrieve array items from the prototype.
            TestUtils.Evaluate("Array.prototype[1] = 1");
            try
            {
                Assert.AreEqual(1, TestUtils.Evaluate("x = [0]; x.length = 2; x.pop();"));
            }
            finally
            {
                TestUtils.Evaluate("delete Array.prototype[1]");
            }

            // length
            Assert.AreEqual(0, TestUtils.Evaluate("Array.prototype.pop.length"));
        }

        [TestMethod]
        public void push()
        {
            TestUtils.Evaluate("var array = [1, 2]");

            Assert.AreEqual(3, TestUtils.Evaluate("array.push(3)"));
            Assert.AreEqual(1, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("array[1]"));
            Assert.AreEqual(3, TestUtils.Evaluate("array[2]"));
            Assert.AreEqual(3, TestUtils.Evaluate("array.length"));

            Assert.AreEqual(5, TestUtils.Evaluate("array.push(4, 5)"));
            Assert.AreEqual(1, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("array[1]"));
            Assert.AreEqual(3, TestUtils.Evaluate("array[2]"));
            Assert.AreEqual(4, TestUtils.Evaluate("array[3]"));
            Assert.AreEqual(5, TestUtils.Evaluate("array[4]"));
            Assert.AreEqual(5, TestUtils.Evaluate("array.length"));

            // push is generic.
            TestUtils.Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            TestUtils.Evaluate("x.push = Array.prototype.push;");
            Assert.AreEqual(3, TestUtils.Evaluate("x.push(7)"));
            Assert.AreEqual(7, TestUtils.Evaluate("x[2]"));
            Assert.AreEqual(3, TestUtils.Evaluate("x.length"));

            // push with very large arrays.
            Assert.AreEqual(1, TestUtils.Evaluate("x = []; x.length = 4294967295; try { x.push(1); } catch (e) { } x[4294967295]"));
            Assert.AreEqual(1, TestUtils.Evaluate("x = []; x.length = 4294967294; try { x.push(1, 2, 3); } catch (e) { } x[4294967294]"));
            Assert.AreEqual(2, TestUtils.Evaluate("x = []; x.length = 4294967294; try { x.push(1, 2, 3); } catch (e) { } x[4294967295]"));
            Assert.AreEqual(3, TestUtils.Evaluate("x = []; x.length = 4294967294; try { x.push(1, 2, 3); } catch (e) { } x[4294967296]"));
            Assert.AreEqual(4294967295.0, TestUtils.Evaluate("x = []; x.length = 4294967294; try { x.push(1, 2, 3); } catch (e) { } x.length"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.push.length"));
        }

        [TestMethod]
        public void reverse()
        {
            TestUtils.Evaluate("var x = [].reverse()");
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[0])"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.length"));

            TestUtils.Evaluate("var x = [1].reverse()");
            Assert.AreEqual(1, TestUtils.Evaluate("x[0]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[1])"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.length"));

            TestUtils.Evaluate("var x = [1, 2].reverse()");
            Assert.AreEqual(2, TestUtils.Evaluate("x[0]"));
            Assert.AreEqual(1, TestUtils.Evaluate("x[1]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[2])"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.length"));

            TestUtils.Evaluate("var x = [1, 2, undefined].reverse()");
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[0])"));
            Assert.AreEqual(2, TestUtils.Evaluate("x[1]"));
            Assert.AreEqual(1, TestUtils.Evaluate("x[2]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[3])"));
            Assert.AreEqual(3, TestUtils.Evaluate("x.length"));
            if (TestUtils.Engine != JSEngine.JScript)   // JScript bug.
                Assert.AreEqual(true, TestUtils.Evaluate("x.hasOwnProperty('0')"));

            TestUtils.Evaluate("var x = [1, 2, 3]");
            TestUtils.Evaluate("delete x[2]");
            Assert.AreEqual(true, TestUtils.Evaluate("x.reverse() === x"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[0])"));
            Assert.AreEqual(2, TestUtils.Evaluate("x[1]"));
            Assert.AreEqual(1, TestUtils.Evaluate("x[2]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[3])"));
            Assert.AreEqual(3, TestUtils.Evaluate("x.length"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.hasOwnProperty('0')"));

            // reverse is generic.
            TestUtils.Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            TestUtils.Evaluate("x.reverse = Array.prototype.reverse;");
            Assert.AreEqual(true, TestUtils.Evaluate("x.reverse() === x"));
            Assert.AreEqual(6, TestUtils.Evaluate("x[0]"));
            Assert.AreEqual(5, TestUtils.Evaluate("x[1]"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.length"));

            // reverse is generic.
            TestUtils.Evaluate("var obj = { }"); // 0: true, 2: Infinity, 4: undefined, 5: undefined, 8: 'NaN', 9: '-1' };");
            TestUtils.Evaluate("obj.length = 10;");
            TestUtils.Evaluate("obj.reverse = Array.prototype.reverse;");
            TestUtils.Evaluate("obj[0] = true;");
            TestUtils.Evaluate("obj[2] = Infinity;");
            TestUtils.Evaluate("obj[4] = undefined;");
            TestUtils.Evaluate("obj[5] = undefined;");
            TestUtils.Evaluate("obj[8] = 'NaN';");
            TestUtils.Evaluate("obj[9] = '-1';");
            Assert.AreEqual(true, TestUtils.Evaluate("obj.reverse() === obj"));
            Assert.AreEqual("-1",                       TestUtils.Evaluate("obj[0]"));
            Assert.AreEqual("NaN",                      TestUtils.Evaluate("obj[1]"));
            Assert.AreEqual(Undefined.Value,            TestUtils.Evaluate("obj[2]"));
            Assert.AreEqual(Undefined.Value,            TestUtils.Evaluate("obj[3]"));
            Assert.AreEqual(Undefined.Value,            TestUtils.Evaluate("obj[4]"));
            Assert.AreEqual(Undefined.Value,            TestUtils.Evaluate("obj[5]"));
            Assert.AreEqual(Undefined.Value,            TestUtils.Evaluate("obj[6]"));
            Assert.AreEqual(double.PositiveInfinity,    TestUtils.Evaluate("obj[7]"));
            Assert.AreEqual(Undefined.Value,            TestUtils.Evaluate("obj[8]"));
            Assert.AreEqual(true,                       TestUtils.Evaluate("obj[9]"));

            // length
            Assert.AreEqual(0, TestUtils.Evaluate("Array.prototype.reverse.length"));
        }

        [TestMethod]
        public void shift()
        {
            TestUtils.Evaluate("var x = []");
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x.shift())"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[0])"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.length"));

            TestUtils.Evaluate("var x = [1]");
            Assert.AreEqual(1, TestUtils.Evaluate("x.shift()"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[0])"));
            Assert.AreEqual(0, TestUtils.Evaluate("x.length"));

            TestUtils.Evaluate("var x = [1, 2]");
            Assert.AreEqual(1, TestUtils.Evaluate("x.shift()"));
            Assert.AreEqual(2, TestUtils.Evaluate("x[0]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[1])"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.length"));

            TestUtils.Evaluate("var x = [1, 2, undefined]");
            Assert.AreEqual(1, TestUtils.Evaluate("x.shift()"));
            Assert.AreEqual(2, TestUtils.Evaluate("x[0]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[1])"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.length"));
            if (TestUtils.Engine != JSEngine.JScript)   // JScript bug.
                Assert.AreEqual(true, TestUtils.Evaluate("x.hasOwnProperty('1')"));

            TestUtils.Evaluate("var x = [1, 2, 3]");
            TestUtils.Evaluate("delete x[2]");
            Assert.AreEqual(1, TestUtils.Evaluate("x.shift()"));
            Assert.AreEqual(2, TestUtils.Evaluate("x[0]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[1])"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[2])"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.length"));
            Assert.AreEqual(false, TestUtils.Evaluate("x.hasOwnProperty('1')"));

            // shift is generic.
            TestUtils.Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            TestUtils.Evaluate("x.shift = Array.prototype.shift;");
            Assert.AreEqual(5, TestUtils.Evaluate("x.shift()"));
            Assert.AreEqual(6, TestUtils.Evaluate("x[0]"));
            if (TestUtils.Engine != JSEngine.JScript)   // JScript bug.
                Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(x[1])"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.length"));

            // length
            Assert.AreEqual(0, TestUtils.Evaluate("Array.prototype.shift.length"));
        }

        [TestMethod]
        public void slice()
        {
            TestUtils.Evaluate("var y = [1, 2, 3, 4].slice(0, 2)");
            Assert.AreEqual(1, TestUtils.Evaluate("y[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("y[1]"));
            Assert.AreEqual(2, TestUtils.Evaluate("y.length"));

            TestUtils.Evaluate("var y = [1, 2, 3, 4].slice(1)");
            Assert.AreEqual(2, TestUtils.Evaluate("y[0]"));
            Assert.AreEqual(3, TestUtils.Evaluate("y[1]"));
            Assert.AreEqual(4, TestUtils.Evaluate("y[2]"));
            Assert.AreEqual(3, TestUtils.Evaluate("y.length"));

            TestUtils.Evaluate("var y = [1, 2, 3, 4].slice(-2, 10)");
            Assert.AreEqual(3, TestUtils.Evaluate("y[0]"));
            Assert.AreEqual(4, TestUtils.Evaluate("y[1]"));
            Assert.AreEqual(2, TestUtils.Evaluate("y.length"));

            TestUtils.Evaluate("var y = [1, 2, 3, 4].slice(0, -2)");
            Assert.AreEqual(1, TestUtils.Evaluate("y[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("y[1]"));
            Assert.AreEqual(2, TestUtils.Evaluate("y.length"));

            // Should return a copy of the array.
            TestUtils.Evaluate("var x = [1, 2, 3, 4]; var y = x.slice(2, 3)");
            TestUtils.Evaluate("y[0] = 5");
            Assert.AreEqual(1, TestUtils.Evaluate("x[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("x[1]"));
            Assert.AreEqual(3, TestUtils.Evaluate("x[2]"));
            Assert.AreEqual(4, TestUtils.Evaluate("x[3]"));
            Assert.AreEqual(4, TestUtils.Evaluate("x.length"));

            // Check behavior with undefined.
            TestUtils.Evaluate("var y = [1, 2, undefined].slice(2, 3)");
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(y[0])"));
            Assert.AreEqual(1, TestUtils.Evaluate("y.length"));
            if (TestUtils.Engine != JSEngine.JScript)   // JScript bug.
                Assert.AreEqual(true, TestUtils.Evaluate("y.hasOwnProperty('0')"));

            TestUtils.Evaluate("var x = [1, 2, 3]");
            TestUtils.Evaluate("delete x[2]");
            TestUtils.Evaluate("var y = x.slice(2, 3)");
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(y[0])"));
            Assert.AreEqual(1, TestUtils.Evaluate("y.length"));
            Assert.AreEqual(false, TestUtils.Evaluate("y.hasOwnProperty('0')"));

            // slice is generic.
            TestUtils.Evaluate("var x = { '0': 5, '1': 6, length: 2};");
            TestUtils.Evaluate("x.slice = Array.prototype.slice;");
            TestUtils.Evaluate("var y = x.slice(1)");
            Assert.AreEqual(6, TestUtils.Evaluate("y[0]"));
            Assert.AreEqual("undefined", TestUtils.Evaluate("typeof(y[1])"));
            Assert.AreEqual(1, TestUtils.Evaluate("y.length"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("Array.prototype.slice.length"));
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
            TestUtils.Evaluate(script.ToString());

            // Sort it and check it matches the result of the .NET sort.
            Assert.AreEqual(true, TestUtils.Evaluate("array.sort() === array"));
            Array.Sort(words, (a, b) => string.CompareOrdinal(a, b));
            var array = TestUtils.Evaluate("array");
            for (int i = 0; i < words.Length; i++)
                Assert.AreEqual(words[i], TestUtils.Evaluate(string.Format("array[{0}]", i)));

            // Even numbers are sorted using ASCII sort.
            TestUtils.Evaluate("var array = [1, 21, 4, 11].sort()");
            Assert.AreEqual(1, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(11, TestUtils.Evaluate("array[1]"));
            Assert.AreEqual(21, TestUtils.Evaluate("array[2]"));
            Assert.AreEqual(4, TestUtils.Evaluate("array[3]"));

            // Unless you use a custom comparer function.
            TestUtils.Evaluate("var array = [1, 21, 4, 11, 5, 3].sort(function(a,b) { return a-b; })");
            Assert.AreEqual(1, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(3, TestUtils.Evaluate("array[1]"));
            Assert.AreEqual(4, TestUtils.Evaluate("array[2]"));
            Assert.AreEqual(5, TestUtils.Evaluate("array[3]"));
            Assert.AreEqual(11, TestUtils.Evaluate("array[4]"));
            Assert.AreEqual(21, TestUtils.Evaluate("array[5]"));

            // The comparison function doesn't have to return integers.
            Assert.AreEqual("0.1,0.2,0.4,0.5,0.6,0.7", TestUtils.Evaluate("[0.7, 0.1, 0.5, 0.4, 0.6, 0.2].sort(function(a,b) { return a-b; }).toString()"));

            // Try sorting some small arrays.
            TestUtils.Evaluate("var array = ['a', 'c', 'b'].sort()");
            Assert.AreEqual("a", TestUtils.Evaluate("array[0]"));
            Assert.AreEqual("b", TestUtils.Evaluate("array[1]"));
            Assert.AreEqual("c", TestUtils.Evaluate("array[2]"));
            Assert.AreEqual(3, TestUtils.Evaluate("array.length"));
            TestUtils.Evaluate("var array = ['a', 'b'].sort()");
            Assert.AreEqual("a", TestUtils.Evaluate("array[0]"));
            Assert.AreEqual("b", TestUtils.Evaluate("array[1]"));
            Assert.AreEqual(2, TestUtils.Evaluate("array.length"));
            TestUtils.Evaluate("var array = ['b', 'a'].sort()");
            Assert.AreEqual("a", TestUtils.Evaluate("array[0]"));
            Assert.AreEqual("b", TestUtils.Evaluate("array[1]"));
            Assert.AreEqual(2, TestUtils.Evaluate("array.length"));
            TestUtils.Evaluate("var array = ['a'].sort()");
            Assert.AreEqual("a", TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(1, TestUtils.Evaluate("array.length"));
            TestUtils.Evaluate("var array = [].sort()");
            Assert.AreEqual(0, TestUtils.Evaluate("array.length"));

            // Ensure an inconsistant sort routine doesn't cause an infinite loop.
            Assert.AreEqual(0, TestUtils.Evaluate("array.length"));
            TestUtils.Evaluate(@"
                var badCompareFunction = function(x,y) {
                  if (x === undefined) return -1; 
                  if (y === undefined) return 1;
                  return 0;
                }");
            TestUtils.Evaluate(@"var x = new Array(2); x[1] = 1; x.sort(badCompareFunction);");
            TestUtils.Evaluate(@"var x = new Array(2); x[0] = 1; x.sort(badCompareFunction);");

            // The comparer function is not called for elements that are undefined.
            TestUtils.Evaluate(@"
                var myComparefn = function(x,y) {
                  if (x === undefined) return -1; 
                  if (y === undefined) return 1;
                  return 0;
                }
                var x = new Array(2);
                x[1] = 1; 
                x.sort(myComparefn);");
            Assert.AreEqual(2, TestUtils.Evaluate(@"x.length"));
            Assert.AreEqual(1, TestUtils.Evaluate(@"x[0]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate(@"x[1]"));
            TestUtils.Evaluate(@"
                var myComparefn = function(x,y) {
                  if (x === undefined) return -1; 
                  if (y === undefined) return 1;
                  return 0;
                }
                var x = [undefined, 1];
                x.sort(myComparefn);");
            Assert.AreEqual(2, TestUtils.Evaluate(@"x.length"));
            Assert.AreEqual(1, TestUtils.Evaluate(@"x[0]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate(@"x[1]"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.sort.length"));

            // "this" should be the global object in non-strict mode.
            Assert.AreEqual(true, TestUtils.Evaluate(@"
                var global = this;
                var success = false;
                [2,3].sort(function (x, y) {
                    success = this === global;
                });
                success"));

            // "this" should be undefined in strict mode.
            Assert.AreEqual(true, TestUtils.Evaluate(@"
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
            TestUtils.Evaluate("var array = [1, 21, 4, 11]");
            TestUtils.Evaluate("var deletedItems = array.splice(2, 3, 'a', 'b', 'c')");
            Assert.AreEqual(5, TestUtils.Evaluate("array.length"));
            Assert.AreEqual(1, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(21, TestUtils.Evaluate("array[1]"));
            Assert.AreEqual("a", TestUtils.Evaluate("array[2]"));
            Assert.AreEqual("b", TestUtils.Evaluate("array[3]"));
            Assert.AreEqual("c", TestUtils.Evaluate("array[4]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("array[5]"));
            Assert.AreEqual(2, TestUtils.Evaluate("deletedItems.length"));
            Assert.AreEqual(4, TestUtils.Evaluate("deletedItems[0]"));
            Assert.AreEqual(11, TestUtils.Evaluate("deletedItems[1]"));

            TestUtils.Evaluate("var array = [1, 21, 4, 11]");
            TestUtils.Evaluate("var deletedItems = array.splice(2, 1)");
            Assert.AreEqual(3, TestUtils.Evaluate("array.length"));
            Assert.AreEqual(1, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(21, TestUtils.Evaluate("array[1]"));
            Assert.AreEqual(11, TestUtils.Evaluate("array[2]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("array[3]"));

            // Start index can be negative.
            TestUtils.Evaluate("var array = [1, 21, 4, 11]");
            TestUtils.Evaluate("var deletedItems = array.splice(-2, 1)");
            Assert.AreEqual(3, TestUtils.Evaluate("array.length"));
            Assert.AreEqual(1, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(21, TestUtils.Evaluate("array[1]"));
            Assert.AreEqual(11, TestUtils.Evaluate("array[2]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("array[3]"));

            // Start index can be negative.
            TestUtils.Evaluate("var array = [1, 21, 4, 11]");
            TestUtils.Evaluate("var deletedItems = array.splice(-10, 6)");
            Assert.AreEqual(0, TestUtils.Evaluate("array.length"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(4, TestUtils.Evaluate("deletedItems.length"));
            Assert.AreEqual("1,21,4,11", TestUtils.Evaluate("deletedItems.toString()"));

            // Start index can be negative.
            TestUtils.Evaluate("var array = [0, 1]");
            TestUtils.Evaluate("var deletedItems = array.splice(-1, -1, 2, 3)");
            Assert.AreEqual(4, TestUtils.Evaluate("array.length"));
            Assert.AreEqual(0, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(2, TestUtils.Evaluate("array[1]"));
            Assert.AreEqual(3, TestUtils.Evaluate("array[2]"));
            Assert.AreEqual(1, TestUtils.Evaluate("array[3]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("array[4]"));
            Assert.AreEqual(0, TestUtils.Evaluate("deletedItems.length"));

            // splice is generic.
            TestUtils.Evaluate("var obj = {0: 0, 1: 1, 2: 2, 3: 3}");
            TestUtils.Evaluate("obj.length = 4;");
            TestUtils.Evaluate("obj.splice = Array.prototype.splice;");
            TestUtils.Evaluate("var deletedItems = obj.splice(0, 3, 4, 5);");
            Assert.AreEqual(3, TestUtils.Evaluate("obj.length"));
            Assert.AreEqual(4, TestUtils.Evaluate("obj[0]"));
            Assert.AreEqual(5, TestUtils.Evaluate("obj[1]"));
            Assert.AreEqual(3, TestUtils.Evaluate("obj[2]"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("obj[3]"));
            Assert.AreEqual(3, TestUtils.Evaluate("deletedItems.length"));
            Assert.AreEqual(0, TestUtils.Evaluate("deletedItems[0]"));
            Assert.AreEqual(1, TestUtils.Evaluate("deletedItems[1]"));
            Assert.AreEqual(2, TestUtils.Evaluate("deletedItems[2]"));

            // length
            Assert.AreEqual(2, TestUtils.Evaluate("Array.prototype.splice.length"));
        }

        [TestMethod]
        public void unshift()
        {
            // Basic tests.
            TestUtils.Evaluate("var array = [1, 3, 9]");
            Assert.AreEqual(4, TestUtils.Evaluate("array.unshift(10)"));
            Assert.AreEqual(4, TestUtils.Evaluate("array.length"));
            Assert.AreEqual("10,1,3,9", TestUtils.Evaluate("array.toString()"));

            TestUtils.Evaluate("var array = [1, 3, 9]");
            Assert.AreEqual(5, TestUtils.Evaluate("array.unshift(10, 12)"));
            Assert.AreEqual(5, TestUtils.Evaluate("array.length"));
            Assert.AreEqual("10,12,1,3,9", TestUtils.Evaluate("array.toString()"));

            TestUtils.Evaluate("var array = [1, 3, 9]");
            Assert.AreEqual(3, TestUtils.Evaluate("array.unshift()"));
            Assert.AreEqual(3, TestUtils.Evaluate("array.length"));
            Assert.AreEqual("1,3,9", TestUtils.Evaluate("array.toString()"));

            TestUtils.Evaluate("var array = [1, 3, 9]");
            Assert.AreEqual(4, TestUtils.Evaluate("array.unshift(undefined)"));
            Assert.AreEqual(4, TestUtils.Evaluate("array.length"));
            Assert.AreEqual(",1,3,9", TestUtils.Evaluate("array.toString()"));
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("array[0]"));
            Assert.AreEqual(true, TestUtils.Evaluate("array.hasOwnProperty(0)"));

            // Check that undefined elements are still undefined after unshift().
            TestUtils.Evaluate("var array = [1, , 9]; array.length = 10");
            Assert.AreEqual(false, TestUtils.Evaluate("array.hasOwnProperty(1)"));
            Assert.AreEqual(11, TestUtils.Evaluate("array.unshift(3)"));
            Assert.AreEqual(11, TestUtils.Evaluate("array.length"));
            Assert.AreEqual("3,1,,9,,,,,,,", TestUtils.Evaluate("array.toString()"));
            Assert.AreEqual(false, TestUtils.Evaluate("array.hasOwnProperty(2)"));

            // Check that elements from the prototype are copied into the array
            // (holes are created using Array.prototype.length).
            TestUtils.Evaluate("var array = [1]; array.length = 3");
            TestUtils.Evaluate("Array.prototype[0] = 'one'");
            TestUtils.Evaluate("Array.prototype[2] = 'three'");
            try
            {
                Assert.AreEqual(3, TestUtils.Evaluate("array.unshift()"));
                Assert.AreEqual(3, TestUtils.Evaluate("array.length"));
                Assert.AreEqual("1,,three", TestUtils.Evaluate("array.toString()"));
                Assert.AreEqual(true, TestUtils.Evaluate("array.hasOwnProperty(0)"));
                Assert.AreEqual(false, TestUtils.Evaluate("array.hasOwnProperty(1)"));
                Assert.AreEqual(true, TestUtils.Evaluate("array.hasOwnProperty(2)"));
            }
            finally
            {
                TestUtils.Evaluate("delete Array.prototype[0]");
                TestUtils.Evaluate("delete Array.prototype[2]");
            }

            // Check that elements from the prototype are copied into the array.
            // (holes are created using [] syntax).
            TestUtils.Evaluate("var array = [1,,,];");
            TestUtils.Evaluate("Array.prototype[0] = 'one'");
            TestUtils.Evaluate("Array.prototype[2] = 'three'");
            try
            {
                Assert.AreEqual(3, TestUtils.Evaluate("array.unshift()"));
                Assert.AreEqual(3, TestUtils.Evaluate("array.length"));
                Assert.AreEqual("1,,three", TestUtils.Evaluate("array.toString()"));
                Assert.AreEqual(true, TestUtils.Evaluate("array.hasOwnProperty(0)"));
                Assert.AreEqual(false, TestUtils.Evaluate("array.hasOwnProperty(1)"));
                Assert.AreEqual(true, TestUtils.Evaluate("array.hasOwnProperty(2)"));
            }
            finally
            {
                TestUtils.Evaluate("delete Array.prototype[0]");
                TestUtils.Evaluate("delete Array.prototype[2]");
            }

            // Check that elements from the prototype are copied into the array.
            // (holes are created using delete).
            TestUtils.Evaluate("var array = [1,2,3]; delete array[1]; delete array[2]");
            TestUtils.Evaluate("Array.prototype[0] = 'one'");
            TestUtils.Evaluate("Array.prototype[2] = 'three'");
            try
            {
                Assert.AreEqual(3, TestUtils.Evaluate("array.unshift()"));
                Assert.AreEqual(3, TestUtils.Evaluate("array.length"));
                Assert.AreEqual("1,,three", TestUtils.Evaluate("array.toString()"));
                Assert.AreEqual(true, TestUtils.Evaluate("array.hasOwnProperty(0)"));
                Assert.AreEqual(false, TestUtils.Evaluate("array.hasOwnProperty(1)"));
                Assert.AreEqual(true, TestUtils.Evaluate("array.hasOwnProperty(2)"));
            }
            finally
            {
                TestUtils.Evaluate("delete Array.prototype[0]");
                TestUtils.Evaluate("delete Array.prototype[2]");
            }

            // Check boundary conditions.
            Assert.AreEqual(4294967295.0, TestUtils.Evaluate("new Array(4294967293).unshift(1, 2)"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("new Array(4294967293).unshift(1, 2, 3)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.unshift.length"));
        }

        [TestMethod]
        public void indexOf()
        {
            // JScript doesn't support Array.indexOf.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            // indexOf(searchElement)
            Assert.AreEqual(2, TestUtils.Evaluate("[3, 2, 1].indexOf(1)"));
            Assert.AreEqual(1, TestUtils.Evaluate("[3, 2, 1].indexOf(2)"));
            Assert.AreEqual(0, TestUtils.Evaluate("[3, 2, 1].indexOf(3)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].indexOf(4)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].indexOf(true)"));
            Assert.AreEqual(1, TestUtils.Evaluate("[3, 1, 2, 1].indexOf(1)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3,,1].indexOf(undefined)"));
            Assert.AreEqual(1, TestUtils.Evaluate("[3,undefined,1].indexOf(undefined)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3,undefined,1].indexOf(null)"));

            // indexOf(searchElement, fromIndex)
            Assert.AreEqual(2, TestUtils.Evaluate("[3, 2, 1].indexOf(1, 1)"));
            Assert.AreEqual(1, TestUtils.Evaluate("[3, 2, 1].indexOf(2, 1)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].indexOf(3, 1)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].indexOf(4, 1)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].indexOf(2, -1)"));
            Assert.AreEqual(1, TestUtils.Evaluate("[3, 2, 1].indexOf(2, -2)"));
            Assert.AreEqual(0, TestUtils.Evaluate("['a', 'b', 'c', 'a'].indexOf('a', 0)"));
            Assert.AreEqual(3, TestUtils.Evaluate("['a', 'b', 'c', 'a'].indexOf('a', 1)"));
            Assert.AreEqual(0, TestUtils.Evaluate("['a', 'b', 'c', 'a'].indexOf('a', -10)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("['a', 'b', 'c', 'a'].indexOf('a', 10)"));
            Assert.AreEqual(2, TestUtils.Evaluate("[3, 2, 1].indexOf(1, undefined)"));

            TestUtils.Evaluate(@"
                var a = new Array();
                a[100] = 1;
                a[99999] = '';  
                a[10] = new Object();
                a[5555] = 5.5;
                a[123456] = 'str';
                a[5] = 1E+309;");
            Assert.AreEqual(100, TestUtils.Evaluate("a.indexOf(1)"));
            Assert.AreEqual(99999, TestUtils.Evaluate("a.indexOf('')"));
            Assert.AreEqual(123456, TestUtils.Evaluate("a.indexOf('str')"));
            Assert.AreEqual(5, TestUtils.Evaluate("a.indexOf(1E+309)"));
            Assert.AreEqual(5555, TestUtils.Evaluate("a.indexOf(5.5)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("a.indexOf(true)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("a.indexOf(5)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("a.indexOf('str1')"));
            Assert.AreEqual(-1, TestUtils.Evaluate("a.indexOf(null)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("a.indexOf(new Object())"));

            // indexOf(undefined)
            Assert.AreEqual(6, TestUtils.Evaluate("[0, false, , null, 'undefined', , undefined, 0].indexOf(undefined)"));
            Assert.AreEqual(1, TestUtils.Evaluate("var indexOf_nullVariable; [0, indexOf_nullVariable, 0].indexOf(undefined)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.indexOf.length"));
        }

        [TestMethod]
        public void lastIndexOf()
        {
            // JScript doesn't support Array.lastIndexOf.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            // lastIndexOf(searchElement)
            Assert.AreEqual(2, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(1)"));
            Assert.AreEqual(1, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(2)"));
            Assert.AreEqual(0, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(3)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(4)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(true)"));
            Assert.AreEqual(3, TestUtils.Evaluate("[3, 1, 2, 1].lastIndexOf(1)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3,,1].lastIndexOf(undefined)"));
            Assert.AreEqual(1, TestUtils.Evaluate("[3,undefined,1].lastIndexOf(undefined)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3,undefined,1].lastIndexOf(null)"));

            // lastIndexOf(searchElement, fromIndex)
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(1, 1)"));
            Assert.AreEqual(1, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(2, 1)"));
            Assert.AreEqual(0, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(3, 1)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(4, 1)"));
            Assert.AreEqual(0, TestUtils.Evaluate("['a', 'b', 'c', 'a'].lastIndexOf('a', 0)"));
            Assert.AreEqual(0, TestUtils.Evaluate("['a', 'b', 'c', 'a'].lastIndexOf('a', 1)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("['a', 'b', 'c', 'a'].lastIndexOf('a', -10)"));
            Assert.AreEqual(3, TestUtils.Evaluate("['a', 'b', 'c', 'a'].lastIndexOf('a', 10)"));
            Assert.AreEqual(-1, TestUtils.Evaluate("[3, 2, 1].lastIndexOf(1, undefined)"));

            // lastIndexOf(undefined)
            Assert.AreEqual(6, TestUtils.Evaluate("[0, false, , null, 'undefined', , undefined, 0].lastIndexOf(undefined)"));
            Assert.AreEqual(1, TestUtils.Evaluate("var indexOf_nullVariable; [0, indexOf_nullVariable, 0].lastIndexOf(undefined)"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.lastIndexOf.length"));
        }

        [TestMethod]
        public void every()
        {
            // JScript doesn't support Array.every.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            TestUtils.Evaluate("var array = [1, 2, 3, 4]");
            Assert.AreEqual(true, TestUtils.Evaluate("array.every(function(value, index, array) { return value > 0 })"));
            Assert.AreEqual(false, TestUtils.Evaluate("array.every(function(value, index, array) { return value > 1 })"));
            Assert.AreEqual(false, TestUtils.Evaluate("array.every(function(value, index, array) { return value > 4 })"));
            Assert.AreEqual(true, TestUtils.Evaluate("array.every(function(value, index, array) { return this == 0 }, 0)"));
            Assert.AreEqual(false, TestUtils.Evaluate("array.every(function(value, index, array) { return this == 1 }, 0)"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].every(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].every(1)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].every({})"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.every.length"));
        }

        [TestMethod]
        public void some()
        {
            // JScript doesn't support Array.some.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            TestUtils.Evaluate("var array = [1, 2, 3, 4]");
            Assert.AreEqual(true, TestUtils.Evaluate("array.some(function(value, index, array) { return value > 0 })"));
            Assert.AreEqual(true, TestUtils.Evaluate("array.some(function(value, index, array) { return value > 1 })"));
            Assert.AreEqual(false, TestUtils.Evaluate("array.some(function(value, index, array) { return value > 4 })"));
            Assert.AreEqual(true, TestUtils.Evaluate("array.some(function(value, index, array) { return this == 0 }, 0)"));
            Assert.AreEqual(false, TestUtils.Evaluate("array.some(function(value, index, array) { return this == 1 }, 0)"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].some(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].some(1)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].some({})"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.some.length"));
        }

        [TestMethod]
        public void forEach()
        {
            // JScript doesn't support Array.forEach.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            Assert.AreEqual("2,3,4", TestUtils.Evaluate("array = [1, 2, 3]; array.forEach(function(value, index, array) { array[index] = value + 1 }); array.toString()"));
            Assert.AreEqual("2,3,4", TestUtils.Evaluate("array = [1, 2, 3]; array.forEach(function(value, index, array) { this[index] = value + 1 }, array); array.toString()"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].forEach(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].forEach(1)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].forEach({})"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.forEach.length"));
        }

        [TestMethod]
        public void map()
        {
            // JScript doesn't support Array.map.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            Assert.AreEqual("2,3,4", TestUtils.Evaluate("[1, 2, 3].map(function(value, index, array) { return value + 1; }).toString()"));

            // What if elements are deleted by the callback?
            Assert.AreEqual("0,0,,0", TestUtils.Evaluate("a = [1,2,3,4]; a.map(function() { delete a[2]; return 0; }).toString()"));
            Assert.AreEqual(4, TestUtils.Evaluate("a = [1,2,3,4]; a.map(function() { delete a[2]; return 0; }).length"));
            Assert.AreEqual(4, TestUtils.Evaluate("a = [1,2,3,4]; a.map(function() { delete a[3]; return 0; }).length"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].map(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].map(1)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].map({})"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.map.length"));
        }

        [TestMethod]
        public void filter()
        {
            // JScript doesn't support Array.filter.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            Assert.AreEqual("2,3", TestUtils.Evaluate("[1, 2, 3].filter(function(value, index, array) { return value > 1; }).toString()"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].filter(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].filter(1)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].filter({})"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.filter.length"));
        }

        [TestMethod]
        public void reduce()
        {
            // JScript doesn't support Array.reduce.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            Assert.AreEqual(6, TestUtils.Evaluate("[1, 2, 3].reduce(function(accum, value, index, array) { return accum + value; })"));
            Assert.AreEqual(4, TestUtils.Evaluate("[1, 2, 3].reduce(function(accum, value, index, array) { return accum + index; })"));
            Assert.AreEqual(6, TestUtils.Evaluate("[1, 2, 3].reduce(function(accum, value, index, array) { return accum + array[index]; })"));
            Assert.AreEqual("1,2", TestUtils.Evaluate("indices = []; [1, 2, 3].reduce(function(accum, value, index, array) { indices.push(index); }); indices.toString()"));
            
            // this should be undefined in the callback function.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("t = 0; [1, 2, 3].reduce(function(a, b, c, d) { 'use strict'; t = this; }); t"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].reduce(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].reduce(1)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].reduce({})"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.reduce.length"));
        }

        [TestMethod]
        public void reduceRight()
        {
            // JScript doesn't support Array.reduceRight.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            Assert.AreEqual(6, TestUtils.Evaluate("[1, 2, 3].reduceRight(function(accum, value, index, array) { return accum + value; })"));
            Assert.AreEqual(4, TestUtils.Evaluate("[1, 2, 3].reduceRight(function(accum, value, index, array) { return accum + index; })"));
            Assert.AreEqual(6, TestUtils.Evaluate("[1, 2, 3].reduceRight(function(accum, value, index, array) { return accum + array[index]; })"));
            Assert.AreEqual("1,0", TestUtils.Evaluate("indices = []; [1, 2, 3].reduceRight(function(accum, value, index, array) { indices.push(index); }); indices.toString()"));

            // this should be undefined in the callback function.
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate("t = 0; [1, 2, 3].reduceRight(function(a, b, c, d) { 'use strict'; t = this; }); t"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].reduceRight(true)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].reduceRight(1)"));
            Assert.AreEqual("TypeError", TestUtils.EvaluateExceptionType("[1, 2, 3].reduceRight({})"));

            // length
            Assert.AreEqual(1, TestUtils.Evaluate("Array.prototype.reduceRight.length"));
        }

        [TestMethod]
        public void toLocaleString()
        {
            // JScript puts extra zeros on the end [1].toLocaleString() == '1.00'.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            var originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {
                // Culture is en-us.
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");
                Assert.AreEqual("", TestUtils.Evaluate("[].toLocaleString()"));
                Assert.AreEqual("1", TestUtils.Evaluate("[1].toLocaleString()"));
                Assert.AreEqual("1,2", TestUtils.Evaluate("[1, 2].toLocaleString()"));
                Assert.AreEqual("1,,2", TestUtils.Evaluate("[1, null, 2].toLocaleString()"));
                Assert.AreEqual("1,,2", TestUtils.Evaluate("[1, undefined, 2].toLocaleString()"));
                Assert.AreEqual("test,test2", TestUtils.Evaluate("['test', 'test2'].toLocaleString()"));

                // Culture is de-de (german).
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-de");
                Assert.AreEqual("", TestUtils.Evaluate("[].toLocaleString()"));
                Assert.AreEqual("1", TestUtils.Evaluate("[1].toLocaleString()"));
                Assert.AreEqual("1;2", TestUtils.Evaluate("[1, 2].toLocaleString()"));
                Assert.AreEqual("1;;2", TestUtils.Evaluate("[1, null, 2].toLocaleString()"));
                Assert.AreEqual("1;;2", TestUtils.Evaluate("[1, undefined, 2].toLocaleString()"));
                Assert.AreEqual("test;test2", TestUtils.Evaluate("['test', 'test2'].toLocaleString()"));
            }
            finally
            {
                // Revert the culture back to the original value.
                System.Threading.Thread.CurrentThread.CurrentCulture = originalCulture;
            }

            // length
            Assert.AreEqual(0, TestUtils.Evaluate("Array.prototype.toLocaleString.length"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("1,2", TestUtils.Evaluate("[1, 2].toString()"));
            Assert.AreEqual("1,,2", TestUtils.Evaluate("[1, null, 2].toString()"));
            Assert.AreEqual("1,,2", TestUtils.Evaluate("[1, undefined, 2].toString()"));
            Assert.AreEqual("test", TestUtils.Evaluate("['test'].toString()"));

            // In ECMAScript 5 toString() calls join() which in turn calls toString() on each element.
            if (TestUtils.Engine == JSEngine.JScript)
                return;
            TestUtils.Evaluate("var x = {};");
            TestUtils.Evaluate("x.join = function() { return 'join overridden'; }");
            TestUtils.Evaluate("x.toString = Array.prototype.toString;");
            Assert.AreEqual("join overridden", TestUtils.Evaluate("x.toString()"));

            // length
            Assert.AreEqual(0, TestUtils.Evaluate("Array.prototype.toString.length"));
        }
    }
}
