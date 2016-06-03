using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global TypedArray object.
    /// </summary>
    [TestClass]
    public class TypedArrayTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", EvaluateExceptionType("Int8Array(2)"));

            // Construct

            // new %TypedArray%();
            Assert.AreEqual(0, Evaluate("new Int8Array().length"));
            Assert.AreEqual("", Evaluate("new Int8Array().toString()"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Int8Array()[0]"));

            // new %TypedArray%(length);
            Assert.AreEqual(2, Evaluate("new Int8Array(2).length"));
            Assert.AreEqual("0,0", Evaluate("new Int8Array(2).toString()"));
            Assert.AreEqual(0, Evaluate("new Int8Array(2)[0]"));
            Assert.AreEqual(0, Evaluate("new Int8Array(2)[1]"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Int8Array(2)[2]"));

            // new %TypedArray%(object);
            Assert.AreEqual("1,2,3,4", Evaluate("new Int16Array([1,2,3,4]).toString()"));
            Assert.AreEqual("1,0,3,4", Evaluate("new Int16Array([1,,3,4]).toString()"));
            Assert.AreEqual("1,2", Evaluate("x = [1, 2, 3, 4]; x.length = 2; new Int16Array(x).toString()"));

            // new %TypedArray%(typedArray);
            Assert.AreEqual("-24", Evaluate("new Int8Array(new Int16Array([1000])).toString()"));

            // new %TypedArray%(buffer[, byteOffset[, length]]);
            Assert.AreEqual("0,0,0", Evaluate("new Int8Array(new ArrayBuffer(3)).toString()"));
            Assert.AreEqual("-24,3,-46,4", Evaluate("new Int8Array(new Int16Array([1000, 1234]).buffer).toString()"));
            Assert.AreEqual("-46,4", Evaluate("new Int8Array(new Int16Array([1000, 1234]).buffer, 2).toString()"));
            Assert.AreEqual("3,-46", Evaluate("new Int8Array(new Int16Array([1000, 1234]).buffer, 1, 2).toString()"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Int16Array(new ArrayBuffer(3)).toString()"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Int16Array(new ArrayBuffer(2), 4).toString()"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Int16Array(new ArrayBuffer(2), 2, 1).toString()"));

            // Array indexer
            Assert.AreEqual(2, Evaluate("Object.getOwnPropertyDescriptor(new Int8Array([1, 2]), '1').value"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(new Int8Array([1, 2]), '1').writable"));
            Assert.AreEqual(true, Evaluate("Object.getOwnPropertyDescriptor(new Int8Array([1, 2]), '1').enumerable"));
            Assert.AreEqual(false, Evaluate("Object.getOwnPropertyDescriptor(new Int8Array([1, 2]), '1').configurable"));

            // delete
            Assert.AreEqual(false, Evaluate("delete new Int8Array([1, 2])[1]"));
            Assert.AreEqual("1,2", Evaluate("x = new Int8Array([1, 2]); delete x[1]; x.toString()"));

            // toString and valueOf.
            Assert.AreEqual("function Int8Array() { [native code] }", Evaluate("Int8Array.toString()"));
            Assert.AreEqual(true, Evaluate("Int8Array.valueOf() === Int8Array"));
            Assert.AreEqual(false, Evaluate("Int16Array.valueOf() === Int8Array"));

            // length
            Assert.AreEqual(3, Evaluate("Int8Array.length"));
            Assert.AreEqual(3, Evaluate("Uint8Array.length"));
            Assert.AreEqual(3, Evaluate("Uint8ClampedArray.length"));
            Assert.AreEqual(3, Evaluate("Int16Array.length"));
            Assert.AreEqual(3, Evaluate("Uint16Array.length"));
            Assert.AreEqual(3, Evaluate("Int32Array.length"));
            Assert.AreEqual(3, Evaluate("Uint32Array.length"));
            Assert.AreEqual(3, Evaluate("Float32Array.length"));
            Assert.AreEqual(3, Evaluate("Float64Array.length"));
        }

        [TestMethod]
        public void Properties()
        {
            // BYTES_PER_ELEMENT
            Assert.AreEqual(1, Evaluate("Int8Array.BYTES_PER_ELEMENT"));
            Assert.AreEqual(1, Evaluate("Uint8Array.BYTES_PER_ELEMENT"));
            Assert.AreEqual(1, Evaluate("Uint8ClampedArray.BYTES_PER_ELEMENT"));
            Assert.AreEqual(2, Evaluate("Int16Array.BYTES_PER_ELEMENT"));
            Assert.AreEqual(2, Evaluate("Uint16Array.BYTES_PER_ELEMENT"));
            Assert.AreEqual(4, Evaluate("Int32Array.BYTES_PER_ELEMENT"));
            Assert.AreEqual(4, Evaluate("Uint32Array.BYTES_PER_ELEMENT"));
            Assert.AreEqual(4, Evaluate("Float32Array.BYTES_PER_ELEMENT"));
            Assert.AreEqual(8, Evaluate("Float64Array.BYTES_PER_ELEMENT"));
        }

        [TestMethod]
        public void length()
        {
            Execute("var buffer = new ArrayBuffer(8);");

            // Matches the length of the buffer
            Assert.AreEqual(8, Evaluate("var uint8 = new Uint8Array(buffer); uint8.length"));

            // As specified when constructing the Uint8Array
            Assert.AreEqual(5, Evaluate("var uint8 = new Uint8Array(buffer, 1, 5); uint8.length"));

            // Due to the offset of the constructed Uint8Array
            Assert.AreEqual(6, Evaluate("var uint8 = new Uint8Array(buffer, 2); uint8.length"));

            // Property is read-only.
            Assert.AreEqual(6, Evaluate("uint8.length = 2; uint8.length"));
        }

        [TestMethod]
        public void from()
        {
            Assert.Fail("TODO");
        }

        [TestMethod]
        public void of()
        {
            Assert.Fail("TODO");
        }

        [TestMethod]
        public void copyWithin()
        {
            // Check basic copies.
            Assert.AreEqual("1,2,3,1,2,3", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).copyWithin(3, 0, 3).toString()"));
            Assert.AreEqual("1,2,3,1,2,3", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).copyWithin(3, 0).toString()"));
            Assert.AreEqual("4,5,6,4,5,6", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).copyWithin(0, 3).toString()"));
            Assert.AreEqual("3,4,5,6,5,6", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).copyWithin(0, 2).toString()"));
            Assert.AreEqual("1,2,3,4,1,2", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).copyWithin(4, 0).toString()"));
            Assert.AreEqual("1,2,3,2,3,4", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).copyWithin(3, 1, 4).toString()"));

            // Check bounds.
            Assert.AreEqual("1,2,5,4,5,6", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).copyWithin(-4, -2, -1).toString()"));
            Assert.AreEqual("1,2,3,4,5,6", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).copyWithin(3, 20, 21).toString()"));
            Assert.AreEqual("1,2,3,1,2,3", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).copyWithin(3, -20).toString()"));

            // length
            Assert.AreEqual(2, Evaluate("Int8Array.prototype.copyWithin.length"));
        }

        [TestMethod]
        public void entries()
        {
            Execute("var i = new Int8Array([11, 7]).entries()");

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
        public void every()
        {
            Assert.AreEqual(true, Evaluate("new Int8Array([1, 2, 3, 4]).every(function(value, index, array) { return value > 0 })"));
            Assert.AreEqual(false, Evaluate("new Int8Array([1, 2, 3, 4]).every(function(value, index, array) { return value > 1 })"));
            Assert.AreEqual(false, Evaluate("new Int8Array([1, 2, 3, 4]).every(function(value, index, array) { return value > 4 })"));
            Assert.AreEqual(true, Evaluate("new Int8Array([1, 2, 3, 4]).every(function(value, index, array) { return this == 0 }, 0)"));
            Assert.AreEqual(false, Evaluate("new Int8Array([1, 2, 3, 4]).every(function(value, index, array) { return this == 1 }, 0)"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).every(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).every(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).every({})"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.every.length"));
        }

        [TestMethod]
        public void fill()
        {
            Assert.AreEqual("0,0,0,0,0,0", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).fill(0).toString()"));
            Assert.AreEqual("1,0,0,0,0,0", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).fill(0, 1).toString()"));
            Assert.AreEqual("1,3,3,3,5,6", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).fill(3, 1, 4).toString()"));
            Assert.AreEqual("1,0,0,0,5,6", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).fill('test', 1, 4).toString()"));
            Assert.AreEqual("1,2,3,0,0,6", Evaluate("new Int8Array([1, 2, 3, 4, 5, 6]).fill(0, -3, -1).toString()"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.fill.length"));
        }

        [TestMethod]
        public void filter()
        {
            Assert.AreEqual("2,3", Evaluate("new Int8Array([1, 2, 3]).filter(function(value, index, array) { return value > 1; }).toString()"));
            Assert.AreEqual("3,1,4,1", Evaluate("new Int8Array([3, 1, 4, 1, 5, 9]).filter(function(value, index, array) { return value < 5; }).toString()"));
            Assert.AreEqual(0, Evaluate("new Int8Array([3, 1, 4, 1, 5, 9]).filter(function(value, index, array) { return value > 9; }).length"));

            Assert.AreEqual("0,1,2", Evaluate("var output = []; new Int8Array([1, 2, 3]).filter(function(value, index, array) { output.push(index); return false; }); output.toString()"));
            Assert.AreEqual("1,2,3,1,2,3,1,2,3", Evaluate("var output = []; new Int8Array([1, 2, 3]).filter(function(value, index, array) { output.push(array); return false; }); output.toString()"));

            Assert.AreEqual("ho", Evaluate("var output = 'hi'; new Int8Array([1, 2, 3]).filter(function(value, index, array) { output = this; return false; }, 'ho'); output.toString()"));
            Assert.AreEqual(Evaluate("this"), Evaluate("var output = 5; new Int8Array([1, 2, 3]).filter(function(value, index, array) { output = this; return false; }); output"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).filter(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).filter(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).filter({})"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.filter.length"));
        }

        [TestMethod]
        public void find()
        {
            Assert.AreEqual("2", Evaluate("new Int8Array([1, 2, 3]).find(function(value, index, array) { return value > 1; }).toString()"));
            Assert.AreEqual("3", Evaluate("new Int8Array([3, 1, 4, 1, 5, 9]).find(function(value, index, array) { return value < 5; }).toString()"));
            Assert.AreEqual(Undefined.Value, Evaluate("new Int8Array([3, 1, 4, 1, 5, 9]).find(function(value, index, array) { return value > 9; })"));

            Assert.AreEqual("0,1,2", Evaluate("var output = []; new Int8Array([1, 2, 3]).find(function(value, index, array) { output.push(index); return false; }); output.toString()"));
            Assert.AreEqual("1,2,3,1,2,3,1,2,3", Evaluate("var output = []; new Int8Array([1, 2, 3]).find(function(value, index, array) { output.push(array); return false; }); output.toString()"));

            Assert.AreEqual("ho", Evaluate("var output = 'hi'; new Int8Array([1, 2, 3]).find(function(value, index, array) { output = this; return false; }, 'ho'); output.toString()"));
            Assert.AreEqual(Evaluate("this"), Evaluate("var output = 5; new Int8Array([1, 2, 3]).find(function(value, index, array) { output = this; return false; }); output"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).find(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).find(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).find({})"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.find.length"));
        }

        [TestMethod]
        public void findIndex()
        {
            Assert.AreEqual(1, Evaluate("new Int8Array([1, 2, 3]).findIndex(function(value, index, array) { return value > 1; })"));
            Assert.AreEqual(0, Evaluate("new Int8Array([3, 1, 4, 1, 5, 9]).findIndex(function(value, index, array) { return value < 5; })"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 1, 4, 1, 5, 9]).findIndex(function(value, index, array) { return value > 9; })"));

            Assert.AreEqual("0,1,2", Evaluate("var output = []; new Int8Array([1, 2, 3]).findIndex(function(value, index, array) { output.push(index); return false; }); output.toString()"));
            Assert.AreEqual("1,2,3,1,2,3,1,2,3", Evaluate("var output = []; new Int8Array([1, 2, 3]).findIndex(function(value, index, array) { output.push(array); return false; }); output.toString()"));

            Assert.AreEqual("ho", Evaluate("var output = 'hi'; new Int8Array([1, 2, 3]).findIndex(function(value, index, array) { output = this; return false; }, 'ho'); output.toString()"));
            Assert.AreEqual(Evaluate("this"), Evaluate("var output = 5; new Int8Array([1, 2, 3]).findIndex(function(value, index, array) { output = this; return false; }); output"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).findIndex(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).findIndex(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).findIndex({})"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.findIndex.length"));
        }

        [TestMethod]
        public void forEach()
        {
            Assert.AreEqual("2,3,4", Evaluate("array = new Int8Array([1, 2, 3]); array.forEach(function(value, index, array) { array[index] = value + 1 }); array.toString()"));
            Assert.AreEqual("2,3,4", Evaluate("array = new Int8Array([1, 2, 3]); array.forEach(function(value, index, array) { this[index] = value + 1 }, array); array.toString()"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).forEach(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).forEach(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).forEach({})"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.forEach.length"));
        }

        [TestMethod]
        public void indexOf()
        {
            // indexOf(searchElement)
            Assert.AreEqual(2, Evaluate("new Int8Array([3, 2, 1]).indexOf(1)"));
            Assert.AreEqual(1, Evaluate("new Int8Array([3, 2, 1]).indexOf(2)"));
            Assert.AreEqual(0, Evaluate("new Int8Array([3, 2, 1]).indexOf(3)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).indexOf(4)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1, 0]).indexOf(true)"));
            Assert.AreEqual(1, Evaluate("new Int8Array([3, 1, 2, 1]).indexOf(1)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3,0,1]).indexOf(undefined)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3,undefined,1]).indexOf(undefined)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3,undefined,1]).indexOf(null)"));

            // indexOf(searchElement, fromIndex)
            Assert.AreEqual(2, Evaluate("new Int8Array([3, 2, 1]).indexOf(1, 1)"));
            Assert.AreEqual(1, Evaluate("new Int8Array([3, 2, 1]).indexOf(2, 1)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).indexOf(3, 1)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).indexOf(4, 1)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).indexOf(2, -1)"));
            Assert.AreEqual(1, Evaluate("new Int8Array([3, 2, 1]).indexOf(2, -2)"));
            Assert.AreEqual(2, Evaluate("new Int8Array([3, 2, 1]).indexOf(1, undefined)"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.indexOf.length"));
        }

        [TestMethod]
        public void join()
        {
            Assert.AreEqual("", Evaluate("new Int8Array([]).join()"));
            Assert.AreEqual("1", Evaluate("new Int8Array([1]).join()"));
            Assert.AreEqual("1,2", Evaluate("new Int8Array([1, 2]).join()"));
            Assert.AreEqual("1,0,2", Evaluate("new Int8Array([1, null, 2]).join()"));
            Assert.AreEqual("1,0,2", Evaluate("new Int8Array([1, undefined, 2]).join()"));
            Assert.AreEqual("1.2", Evaluate("new Int8Array([1, 2]).join('.')"));
            Assert.AreEqual("1,2", Evaluate("new Int8Array([1, 2]).join(undefined)"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.join.length"));
        }

        [TestMethod]
        public void keys()
        {
            Execute("var i = new Int8Array([11, 7]).keys()");

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
        public void lastIndexOf()
        {
            // lastIndexOf(searchElement)
            Assert.AreEqual(2, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(1)"));
            Assert.AreEqual(1, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(2)"));
            Assert.AreEqual(0, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(3)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(4)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(true)"));
            Assert.AreEqual(3, Evaluate("new Int8Array([3, 1, 2, 1]).lastIndexOf(1)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3,,1]).lastIndexOf(undefined)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3,undefined,1]).lastIndexOf(undefined)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3,undefined,1]).lastIndexOf(null)"));

            // lastIndexOf(searchElement, fromIndex)
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(1, 1)"));
            Assert.AreEqual(1, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(2, 1)"));
            Assert.AreEqual(0, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(3, 1)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(4, 1)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(1, undefined)"));
            Assert.AreEqual(-1, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(2, undefined)"));
            Assert.AreEqual(0, Evaluate("new Int8Array([3, 2, 1]).lastIndexOf(3, undefined)"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.lastIndexOf.length"));
        }

        [TestMethod]
        public void map()
        {
            Assert.AreEqual("2,3,4", Evaluate("new Int8Array([1, 2, 3]).map(function(value, index, array) { return value + 1; }).toString()"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).map(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).map(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).map({})"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.map.length"));
        }

        [TestMethod]
        public void reduce()
        {
            Assert.AreEqual(6, Evaluate("new Int8Array([1, 2, 3]).reduce(function(accum, value, index, array) { return accum + value; })"));
            Assert.AreEqual(4, Evaluate("new Int8Array([1, 2, 3]).reduce(function(accum, value, index, array) { return accum + index; })"));
            Assert.AreEqual(6, Evaluate("new Int8Array([1, 2, 3]).reduce(function(accum, value, index, array) { return accum + array[index]; })"));
            Assert.AreEqual("1,2", Evaluate("indices = []; new Int8Array([1, 2, 3]).reduce(function(accum, value, index, array) { indices.push(index); }); indices.toString()"));

            // this should be undefined in the callback function.
            Assert.AreEqual(Undefined.Value, Evaluate("t = 0; new Int8Array([1, 2, 3]).reduce(function(a, b, c, d) { 'use strict'; t = this; }); t"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).reduce(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).reduce(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).reduce({})"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.reduce.length"));
        }

        [TestMethod]
        public void reduceRight()
        {
            Assert.AreEqual(6, Evaluate("new Int8Array([1, 2, 3]).reduceRight(function(accum, value, index, array) { return accum + value; })"));
            Assert.AreEqual(4, Evaluate("new Int8Array([1, 2, 3]).reduceRight(function(accum, value, index, array) { return accum + index; })"));
            Assert.AreEqual(6, Evaluate("new Int8Array([1, 2, 3]).reduceRight(function(accum, value, index, array) { return accum + array[index]; })"));
            Assert.AreEqual("1,0", Evaluate("indices = []; new Int8Array([1, 2, 3]).reduceRight(function(accum, value, index, array) { indices.push(index); }); indices.toString()"));

            // this should be undefined in the callback function.
            Assert.AreEqual(Undefined.Value, Evaluate("t = 0; new Int8Array([1, 2, 3]).reduceRight(function(a, b, c, d) { 'use strict'; t = this; }); t"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).reduceRight(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).reduceRight(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).reduceRight({})"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.reduceRight.length"));
        }

        [TestMethod]
        public void reverse()
        {
            Evaluate("var x = new Int8Array([]).reverse()");
            Assert.AreEqual("undefined", Evaluate("typeof(x[0])"));
            Assert.AreEqual(0, Evaluate("x.length"));

            Evaluate("var x = new Int8Array([1]).reverse()");
            Assert.AreEqual(1, Evaluate("x[0]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[1])"));
            Assert.AreEqual(1, Evaluate("x.length"));

            Evaluate("var x = new Int8Array([1, 2]).reverse()");
            Assert.AreEqual(2, Evaluate("x[0]"));
            Assert.AreEqual(1, Evaluate("x[1]"));
            Assert.AreEqual("undefined", Evaluate("typeof(x[2])"));
            Assert.AreEqual(2, Evaluate("x.length"));
            
            // length
            Assert.AreEqual(0, Evaluate("Int8Array.prototype.reverse.length"));
        }

        [TestMethod]
        public void set()
        {
            Assert.Fail("TODO");
        }

        [TestMethod]
        public void slice()
        {
            Evaluate("var y = new Int8Array([1, 2, 3, 4]).slice(0, 2)");
            Assert.AreEqual(1, Evaluate("y[0]"));
            Assert.AreEqual(2, Evaluate("y[1]"));
            Assert.AreEqual(2, Evaluate("y.length"));

            Evaluate("var y = new Int8Array([1, 2, 3, 4]).slice(1)");
            Assert.AreEqual(2, Evaluate("y[0]"));
            Assert.AreEqual(3, Evaluate("y[1]"));
            Assert.AreEqual(4, Evaluate("y[2]"));
            Assert.AreEqual(3, Evaluate("y.length"));

            Evaluate("var y = new Int8Array([1, 2, 3, 4]).slice(-2, 10)");
            Assert.AreEqual(3, Evaluate("y[0]"));
            Assert.AreEqual(4, Evaluate("y[1]"));
            Assert.AreEqual(2, Evaluate("y.length"));

            Evaluate("var y = new Int8Array([1, 2, 3, 4]).slice(0, -2)");
            Assert.AreEqual(1, Evaluate("y[0]"));
            Assert.AreEqual(2, Evaluate("y[1]"));
            Assert.AreEqual(2, Evaluate("y.length"));

            // Should return a copy of the array.
            Evaluate("var x = new Int8Array([1, 2, 3, 4]); var y = x.slice(2, 3)");
            Evaluate("y[0] = 5");
            Assert.AreEqual(1, Evaluate("x[0]"));
            Assert.AreEqual(2, Evaluate("x[1]"));
            Assert.AreEqual(3, Evaluate("x[2]"));
            Assert.AreEqual(4, Evaluate("x[3]"));
            Assert.AreEqual(4, Evaluate("x.length"));

            // length
            Assert.AreEqual(2, Evaluate("Int8Array.prototype.slice.length"));
        }

        [TestMethod]
        public void some()
        {
            Evaluate("var array = new Int8Array([1, 2, 3, 4])");
            Assert.AreEqual(true, Evaluate("array.some(function(value, index, array) { return value > 0 })"));
            Assert.AreEqual(true, Evaluate("array.some(function(value, index, array) { return value > 1 })"));
            Assert.AreEqual(false, Evaluate("array.some(function(value, index, array) { return value > 4 })"));
            Assert.AreEqual(true, Evaluate("array.some(function(value, index, array) { return this == 0 }, 0)"));
            Assert.AreEqual(false, Evaluate("array.some(function(value, index, array) { return this == 1 }, 0)"));

            // TypeError should be thrown if the callback is not a function.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).some(true)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).some(1)"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Int8Array([1, 2, 3]).some({})"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.some.length"));
        }

        [TestMethod]
        public void sort()
        {
            Assert.Fail("TODO");
        }

        [TestMethod]
        public void subarray()
        {
            Assert.Fail("TODO");
        }

        [TestMethod]
        public void toLocaleString()
        {
            var originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {
                // Culture is en-us.
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");
                Assert.AreEqual("", Evaluate("new Int8Array([]).toLocaleString()"));
                Assert.AreEqual("1", Evaluate("new Int8Array([1]).toLocaleString()"));
                Assert.AreEqual("1,2", Evaluate("new Int8Array([1, 2]).toLocaleString()"));

                // Culture is de-de (german).
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-de");
                Assert.AreEqual("", Evaluate("new Int8Array([]).toLocaleString()"));
                Assert.AreEqual("1", Evaluate("new Int8Array([1]).toLocaleString()"));
                Assert.AreEqual("1;2", Evaluate("new Int8Array([1, 2]).toLocaleString()"));
            }
            finally
            {
                // Revert the culture back to the original value.
                System.Threading.Thread.CurrentThread.CurrentCulture = originalCulture;
            }

            // length
            Assert.AreEqual(0, Evaluate("Int8Array.prototype.toLocaleString.length"));
        }

        [TestMethod]
        public void values()
        {
            Execute("var i = new Int8Array([11, 7]).values()");

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
            Assert.AreEqual(true, Evaluate("new Int8Array([11, 7])[Symbol.iterator] === new Int8Array([11, 7]).values"));
        }

        [TestMethod]
        public void Symbol_toStringTag()
        {
            //get %TypedArray%.prototype [ @@toStringTag ]
            Assert.AreEqual("Int16Array", Evaluate("new Int16Array()[Symbol.toStringTag]"));
        }
    }
}
