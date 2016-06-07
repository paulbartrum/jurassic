using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global ArrayBuffer object.
    /// </summary>
    [TestClass]
    public class ArrayBufferTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", EvaluateExceptionType("ArrayBuffer()"));

            // Construct
            Assert.AreEqual("[object ArrayBuffer]", Evaluate("new ArrayBuffer().toString()"));
            Assert.AreEqual("[object ArrayBuffer]", Evaluate("new ArrayBuffer(2).toString()"));

            // toString and valueOf.
            Assert.AreEqual("function ArrayBuffer() { [native code] }", Evaluate("ArrayBuffer.toString()"));
            Assert.AreEqual(true, Evaluate("ArrayBuffer.valueOf() === ArrayBuffer"));
            Assert.AreEqual("ArrayBuffer", Evaluate("ArrayBuffer.prototype[Symbol.toStringTag]"));

            // length
            Assert.AreEqual(1, Evaluate("ArrayBuffer.length"));
        }

        [TestMethod]
        public void isView()
        {
            Assert.AreEqual(false, Evaluate("ArrayBuffer.isView();"));
            Assert.AreEqual(false, Evaluate("ArrayBuffer.isView([]);"));
            Assert.AreEqual(false, Evaluate("ArrayBuffer.isView({ });"));
            Assert.AreEqual(false, Evaluate("ArrayBuffer.isView(null);"));
            Assert.AreEqual(false, Evaluate("ArrayBuffer.isView(undefined);"));
            Assert.AreEqual(false, Evaluate("ArrayBuffer.isView(new ArrayBuffer(10));"));

            Assert.AreEqual(true, Evaluate("ArrayBuffer.isView(new Uint8Array());"));
            Assert.AreEqual(true, Evaluate("ArrayBuffer.isView(new Float32Array());"));
            Assert.AreEqual(true, Evaluate("ArrayBuffer.isView(new Int8Array(10).subarray(0, 3));"));

            Assert.AreEqual(true, Evaluate("var buffer = new ArrayBuffer(2); var dv = new DataView(buffer); ArrayBuffer.isView(dv);"));
        }

        [TestMethod]
        public void byteLength()
        {
            Assert.AreEqual(0, Evaluate("new ArrayBuffer().byteLength"));
            Assert.AreEqual(8, Evaluate("new ArrayBuffer(8).byteLength"));
        }

        [TestMethod]
        public void slice()
        {
            Assert.AreEqual(2, Evaluate("new ArrayBuffer(8).slice(2, 4).byteLength"));
        }
    }
}
