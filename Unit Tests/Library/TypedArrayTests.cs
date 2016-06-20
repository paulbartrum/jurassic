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

            // species
            Assert.AreEqual(true, Evaluate("Int8Array[Symbol.species] === Int8Array"));
            Assert.AreEqual(true, Evaluate("Uint8Array[Symbol.species] === Uint8Array"));
            Assert.AreEqual(true, Evaluate("Uint8ClampedArray[Symbol.species] === Uint8ClampedArray"));
            Assert.AreEqual(true, Evaluate("Int16Array[Symbol.species] === Int16Array"));
            Assert.AreEqual(true, Evaluate("Uint16Array[Symbol.species] === Uint16Array"));
            Assert.AreEqual(true, Evaluate("Int32Array[Symbol.species] === Int32Array"));
            Assert.AreEqual(true, Evaluate("Uint32Array[Symbol.species] === Uint32Array"));
            Assert.AreEqual(true, Evaluate("Float32Array[Symbol.species] === Float32Array"));
            Assert.AreEqual(true, Evaluate("Float64Array[Symbol.species] === Float64Array"));

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
            Assert.AreEqual(1, Evaluate("Int16Array.from([1, 2])[0]"));
            Assert.AreEqual(2, Evaluate("Int16Array.from([1, 2])[1]"));
            Assert.AreEqual(2, Evaluate("Int16Array.from([1, 2]).length"));
            Assert.AreEqual(4, Evaluate("Int16Array.from([1, 2]).byteLength"));
            Assert.AreEqual("9,2", Evaluate("var x = [9, 2, 6]; x.length = 2; Int16Array.from(x).toString()"));
            Assert.AreEqual("6,2,9", Evaluate(@"
                var x = [9, 2, 6];
                x[Symbol.iterator] = function() {
                    var i = 3;
	                return {
		                next: function() {
                            i --;
			                return i < 0 ? { done: true } : { value: x[i] };
		                }
	                };
                };
                Int16Array.from(x).toString()"));

            // mapFn
            Assert.AreEqual("14,503", Evaluate("Int16Array.from([11, 500], function(val, index) { return val + 3; }).toString()"));
            Assert.AreEqual("27,516", Evaluate("Int16Array.from([11, 500], function(val, index) { return val + this; }, 16).toString()"));

            // length
            Assert.AreEqual(1, Evaluate("Int16Array.from.length"));
        }

        [TestMethod]
        public void of()
        {
            Assert.AreEqual(1, Evaluate("Int16Array.of(1, 2)[0]"));
            Assert.AreEqual(2, Evaluate("Int16Array.of(1, 2)[1]"));
            Assert.AreEqual(2, Evaluate("Int16Array.of(1, 2).length"));
            Assert.AreEqual(4, Evaluate("Int16Array.of(1, 2).byteLength"));

            // length
            Assert.AreEqual(0, Evaluate("Int16Array.of.length"));
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
            // set(array)
            Assert.AreEqual("5,6,49,4,11", Evaluate("var a = new Int16Array([1, 21, 49, 4, 11]); a.set([5, 6]); a.toString()"));
            Assert.AreEqual("5,6,4,11", Evaluate("var a = new Int16Array([1, 21, 49, 4, 11]).subarray(1); a.set([5, 6]); a.toString()"));

            // set(array, offset)
            Assert.AreEqual("1,21,5,6,11", Evaluate("var a = new Int16Array([1, 21, 49, 4, 11]); a.set([5, 6], 2); a.toString()"));
            Assert.AreEqual("21,49,5,6", Evaluate("var a = new Int16Array([1, 21, 49, 4, 11]).subarray(1); a.set([5, 6], 2); a.toString()"));

            // set(typedArray)
            Assert.AreEqual("5,6,49,4,11", Evaluate("var a = new Int16Array([1, 21, 49, 4, 11]); a.set(new Int8Array([5, 6])); a.toString()"));
            Assert.AreEqual("5,6,4,11", Evaluate("var a = new Int16Array([1, 21, 49, 4, 11]).subarray(1); a.set(new Int8Array([5, 6])); a.toString()"));

            // set(typedArray, offset)
            Assert.AreEqual("1,21,5,6,11", Evaluate("var a = new Int16Array([1, 21, 49, 4, 11]); a.set(new Int8Array([5, 6]), 2); a.toString()"));
            Assert.AreEqual("21,49,5,6", Evaluate("var a = new Int16Array([1, 21, 49, 4, 11]).subarray(1); a.set(new Int8Array([5, 6]), 2); a.toString()"));
            Assert.AreEqual("2100,49,49,4,11", Evaluate("var a = new Int16Array([1, 2100, 49, 4, 11]); a.set(a.subarray(1, 3), 0); a.toString()"));
            Assert.AreEqual("1,2100,2100,49,11", Evaluate("var a = new Int16Array([1, 2100, 49, 4, 11]); a.set(a.subarray(1, 3), 2); a.toString()"));

            Assert.AreEqual("RangeError", EvaluateExceptionType("new Int16Array([1, 21, 49, 4, 11]).set([1, 2], -1)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Int16Array([1, 21, 49, 4, 11]).set([1, 2], 4)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Int16Array([1, 21, 49, 4, 11]).set([1, 2], 7)"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.set.length"));
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
            // By default, array elements are sorted using a numeric sort.
            Assert.AreEqual("1,4,11,21,49", Evaluate("new Int8Array([1, 21, 49, 4, 11]).sort().toString()"));

            // NaN is sorted to the end.
            Assert.AreEqual("-5,1,4,7,NaN", Evaluate("new Float64Array([1, NaN, 4, 7, -5]).sort().toString()"));

            // Positive zero is sorted higher than negative zero.
            // We invert the results so as to more easily show the results are correct.
            Assert.AreEqual("-Infinity,-Infinity,-Infinity,Infinity,Infinity",
                Evaluate("new Float64Array([-0, 0, -0, 0, -0]).sort().map(function (val) { return 1/val; }).toString()"));

            // Comparison function.
            Assert.AreEqual("1,4,11,21,49", Evaluate("new Int8Array([1, 21, 49, 4, 11]).sort(function (a, b) { return a - b; }).toString()"));
            Assert.AreEqual("49,21,11,4,1", Evaluate("new Int8Array([1, 21, 49, 4, 11]).sort(function (a, b) { return b - a; }).toString()"));
            Assert.AreEqual("1,11,21,4,49", Evaluate("new Int8Array([1, 21, 49, 4, 11]).sort(function (a, b) { return a.toString() > b.toString() ? 1 : -1; }).toString()"));

            // length
            Assert.AreEqual(1, Evaluate("Int8Array.prototype.sort.length"));
        }

        [TestMethod]
        public void subarray()
        {
            Assert.AreEqual("4,11", Evaluate("new Int16Array([1, 21, 49, 4, 11]).subarray(3).toString()"));
            Assert.AreEqual("49,4", Evaluate("new Int16Array([1, 21, 49, 4, 11]).subarray(2, 4).toString()"));
            Assert.AreEqual("49,15", Evaluate("new Int16Array([1, 21, 49, 15, 4, 11]).subarray(1, 5).subarray(1, 3).toString()"));

            // Subarrays share the same buffer.
            Assert.AreEqual("1,21,99,4,11", Evaluate("var a1 = new Int16Array([1, 21, 49, 4, 11]); var a2 = a1.subarray(2); a2[0] = 99; a1.toString()"));
            Assert.AreEqual("49,99,11", Evaluate("var a1 = new Int16Array([1, 21, 49, 4, 11]); var a2 = a1.subarray(2); a1[3] = 99; a2.toString()"));

            // length
            Assert.AreEqual(2, Evaluate("Int8Array.prototype.subarray.length"));
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
