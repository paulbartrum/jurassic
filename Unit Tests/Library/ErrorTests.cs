using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the global Error objects.
    /// </summary>
    [TestClass]
    public class ErrorTests
    {
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
            Assert.AreEqual(true, TestUtils.Evaluate("new RangeError('test').hasOwnProperty('stack')"));
            Assert.AreEqual(true, TestUtils.Evaluate("new RangeError('test').constructor === RangeError"));

            // Second level of the chain, the RangeError prototype.
            Assert.AreEqual("RangeError", TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')).toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')) === RangeError.prototype"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')).hasOwnProperty('name')"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')).hasOwnProperty('message')"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')).hasOwnProperty('stack')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new RangeError('test')).constructor === RangeError"));

            // Third level of the chain, the Error prototype.
            Assert.AreEqual("Error", TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))).toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))) === Error.prototype"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))).hasOwnProperty('name')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))).hasOwnProperty('message')"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))).hasOwnProperty('stack')"));
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test'))).constructor === Error"));

            // Third level of the chain, the Object prototype.
            Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test')))) === Object.prototype"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test')))).hasOwnProperty('name')"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test')))).hasOwnProperty('message')"));
            Assert.AreEqual(false, TestUtils.Evaluate("Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(new RangeError('test')))).hasOwnProperty('stack')"));
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
            Assert.AreEqual("Error: hah\r\n    at [eval code]:1", TestUtils.Evaluate(@"new Error('hah').stack"));
            Assert.AreEqual("Error: myError\r\n" +
                "    at trace ([eval code]:4)\r\n" +
                "    at b ([eval code]:11)\r\n" +
                "    at a ([eval code]:14)\r\n" +
                "    at [eval code]:16",
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
        }
    }
}
