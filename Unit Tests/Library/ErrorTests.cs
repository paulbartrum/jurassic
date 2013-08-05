using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global Error objects.
    /// </summary>
    [TestClass]
    public class ErrorTests
    {
        [TestMethod]
        public void Constructor()
        {
            // Construct
            Assert.AreEqual("test", TestUtils.Evaluate("new Error('test').message"));
            Assert.AreEqual("", TestUtils.Evaluate("new Error().message"));
            Assert.AreEqual("", TestUtils.Evaluate("new Error(undefined).message"));

            // Call
            Assert.AreEqual("test", TestUtils.Evaluate("Error('test').message"));
            Assert.AreEqual("", TestUtils.Evaluate("Error().message"));
            Assert.AreEqual("", TestUtils.Evaluate("Error(undefined).message"));
        }

        [TestMethod]
        public void PrototypeChain()
        {
            // JScript doesn't support Error.stack or Object.getPrototypeOf.
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            // __proto__		        properties		                constructor
            // ---------                ----------                      -----------
            // new RangeError("test")	message, stack		            RangeError
            // new RangeError		    name ("RangeError")	            RangeError
            // new Error			    name ("Error"), message ("")	Error
            // new Object		        <none>			                Object
            // null

            // Top level of the chain.
            Assert.AreEqual("RangeError: test", TestUtils.Evaluate("new RangeError('test').toString()"));
            Assert.AreEqual(false, TestUtils.Evaluate("new RangeError('test').hasOwnProperty('name')"));
            Assert.AreEqual(true, TestUtils.Evaluate("new RangeError('test').hasOwnProperty('message')"));
            Assert.AreEqual(true, TestUtils.Evaluate("new RangeError('test').constructor === RangeError"));

            // Second level of the chain, the RangeError prototype.
            Assert.AreEqual("RangeError", TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')).toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')) === RangeError.prototype"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')).hasOwnProperty('name')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')).hasOwnProperty('message')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')).constructor === RangeError"));

            // Third level of the chain, the Error prototype.
            Assert.AreEqual("Error", TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))).toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))) === Error.prototype"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))).hasOwnProperty('name')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))).hasOwnProperty('message')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))).constructor === Error"));

            // Third level of the chain, the Object prototype.
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test')))) === Object.prototype"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test')))).hasOwnProperty('name')"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test')))).hasOwnProperty('message')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test')))).constructor === Object"));

            // Prototype is null at this point.
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))))) === null"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("Error: test", TestUtils.Evaluate("new Error('test').toString()"));
            Assert.AreEqual("RangeError: test", TestUtils.Evaluate("new RangeError('test').toString()"));
        }

        [TestMethod]
        public void stack()
        {
            Assert.AreEqual(Undefined.Value, TestUtils.Evaluate(@"new Error('hah').stack"));
            Assert.AreEqual("Error: myError\r\n" +
                "    at trace (unknown:4)\r\n" +
                "    at b (unknown:11)\r\n" +
                "    at a (unknown:14)\r\n" +
                "    at unknown:16",
                TestUtils.Evaluate(@"
                    function trace() {
                        try {
                            throw new Error('myError');
                        }
                        catch (e) {
                            return e.stack;
                        }
                    }
                    function b() {
                        return trace();
                    }
                    function a() {
                        return b(3, 4, '\n\n', undefined, {});
                    }
                    a('first call, firstarg');"));
            Assert.AreEqual("Error: this error is initialized at line 3, but thrown at line 5\r\n" +
                "    at trace (unknown:5)\r\n" +
                "    at unknown:11",
                TestUtils.Evaluate(@"
                    function trace() {
                        var y = new Error('this error is initialized at line 3, but thrown at line 5');
                        try {
                            throw y;
                        }
                        catch (e) {
                            return e.stack;
                        }
                    }
                    trace()"));
            Assert.AreEqual("URIError: URI malformed\r\n" +
                "    at decodeURI (native)\r\n" +
                "    at unknown:1",
                TestUtils.Evaluate(@"try { decodeURI('%z') } catch (e) { e.stack }"));
            Assert.AreEqual("Error: two\r\n" +
                "    at a (unknown:13)\r\n" +
                "    at unknown:17",
                TestUtils.Evaluate(@"
                    function trace() {
                        throw new Error('one');
                    }
                    function b() {
                        return trace();
                    }
                    function a() {
                        try {
                            return b(3, 4, '\n\n', undefined, {});
                        }
                        catch (e) {
                            throw new Error('two');
                        }
                    }
                    try {
                        a('first call, firstarg')
                    }
                    catch (e) {
                        e.stack
                    }"));
            Assert.AreEqual("Error: inside callback\r\n" +
                "    at anonymous (unknown:4)\r\n" +
                "    at sort (native)\r\n" +
                "    at unknown:3",
                TestUtils.Evaluate(@"
                    try {
                        [1, 2].sort(function () {
                            throw new Error('inside callback');
                        })
                    }
                    catch (e) {
                        e.stack
                    }"));
            Assert.AreEqual("TypeError: undefined cannot be converted to an object\r\n" +
                "    at unknown:4",
                TestUtils.Evaluate(@"
                    try {
                        var x = undefined;
                        x.sdfsf();
                    }
                    catch (e) {
                        e.stack
                    }"));
        }
    }
}
