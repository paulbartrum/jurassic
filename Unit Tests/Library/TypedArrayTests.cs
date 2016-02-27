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


            // Construct


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
    }
}
