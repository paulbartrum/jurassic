using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{
    /// <summary>
    /// Test the global DataView object.
    /// </summary>
    [TestClass]
    public class DataViewTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", EvaluateExceptionType("DataView()"));

            // Construct
            Assert.AreEqual("TypeError", EvaluateExceptionType("new DataView().toString()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("new DataView(2).toString()"));
            Execute(@"
                buffer = new ArrayBuffer(2);
                new DataView(buffer, 0).setInt16(0, 1234)");
            Assert.AreEqual(4, Evaluate("new DataView(buffer, 0).getInt8(0)"));
            Assert.AreEqual(-46, Evaluate("new DataView(buffer, 1).getInt8(0)"));

            // toString and valueOf.
            Assert.AreEqual("function DataView() { [native code] }", Evaluate("DataView.toString()"));
            Assert.AreEqual(true, Evaluate("DataView.valueOf() === DataView"));

            // length
            Assert.AreEqual(3, Evaluate("DataView.length"));
        }

        [TestMethod]
        public void Properties()
        {
            // buffer
            Assert.AreEqual(true, Evaluate("buffer = new ArrayBuffer(1); new DataView(buffer).buffer === buffer"));

            // byteLength
            Assert.AreEqual(3, Evaluate("new DataView(new ArrayBuffer(3)).byteLength"));
            Assert.AreEqual(2, Evaluate("new DataView(new ArrayBuffer(3), 1).byteLength"));
            Assert.AreEqual(1, Evaluate("new DataView(new ArrayBuffer(3), 1, 1).byteLength"));

            // byteOffset
            Assert.AreEqual(0, Evaluate("new DataView(new ArrayBuffer(3)).byteOffset"));
            Assert.AreEqual(1, Evaluate("new DataView(new ArrayBuffer(3), 1).byteOffset"));
            Assert.AreEqual(1, Evaluate("new DataView(new ArrayBuffer(3), 1, 1).byteOffset"));
        }

        [TestMethod]
        public void getInt8()
        {
            Assert.AreEqual(0, Evaluate("new DataView(new ArrayBuffer(1)).getInt8(0)"));

            // Error conditions.
            Assert.AreEqual("TypeError", EvaluateExceptionType("new DataView(new ArrayBuffer(1)).getInt8()"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(0)).getInt8(0)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(0)).getInt8(-1)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(1)).getInt8(1234567890123)"));

            // length
            Assert.AreEqual(1, Evaluate("DataView.prototype.getInt8.length"));
        }

        [TestMethod]
        public void setInt8()
        {
            Execute("var buffer = new DataView(new ArrayBuffer(2));");
            Assert.AreEqual(2, Evaluate("buffer.setInt8(0, 2); buffer.getInt8(0)"));
            Assert.AreEqual(3, Evaluate("buffer.setInt8(1, 3); buffer.getInt8(1)"));
            Assert.AreEqual(-128, Evaluate("buffer.setInt8(1, 128); buffer.getInt8(1)"));

            // Error conditions.
            Assert.AreEqual("TypeError", EvaluateExceptionType("buffer.setInt8()"));
            Assert.AreEqual("TypeError", EvaluateExceptionType("buffer.setInt8(2)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setInt8(2, 2)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setInt8(-1, 9)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setInt8(1234567890123, 0)"));

            // length
            Assert.AreEqual(2, Evaluate("DataView.prototype.setInt8.length"));
        }

        [TestMethod]
        public void getInt16()
        {
            Assert.AreEqual(0, Evaluate("new DataView(new ArrayBuffer(2)).getInt16(0)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(1)).getInt16(0)"));

            // length
            Assert.AreEqual(2, Evaluate("DataView.prototype.getInt16.length"));
        }

        [TestMethod]
        public void setInt16()
        {
            Execute(@"
                var buffer = new DataView(new ArrayBuffer(10));
                buffer.setInt16(0, 4097);
                buffer.setInt16(3, 45678);
                buffer.setInt16(5, 4097, true);
                buffer.setInt16(8, 45678, true);");

            // Check each byte.
            Assert.AreEqual(16, Evaluate("buffer.getUint8(0)"));
            Assert.AreEqual(1, Evaluate("buffer.getUint8(1)"));
            Assert.AreEqual(0, Evaluate("buffer.getUint8(2)"));
            Assert.AreEqual(178, Evaluate("buffer.getUint8(3)"));
            Assert.AreEqual(110, Evaluate("buffer.getUint8(4)"));
            Assert.AreEqual(1, Evaluate("buffer.getUint8(5)"));
            Assert.AreEqual(16, Evaluate("buffer.getUint8(6)"));
            Assert.AreEqual(0, Evaluate("buffer.getUint8(7)"));
            Assert.AreEqual(110, Evaluate("buffer.getUint8(8)"));
            Assert.AreEqual(178, Evaluate("buffer.getUint8(9)"));

            Assert.AreEqual(4097, Evaluate("buffer.getInt16(0)"));
            Assert.AreEqual(272, Evaluate("buffer.getInt16(0, true)"));
            Assert.AreEqual(-19858, Evaluate("buffer.getInt16(3)"));
            Assert.AreEqual(28338, Evaluate("buffer.getInt16(3, true)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setInt16(9, 2)"));

            // length
            Assert.AreEqual(3, Evaluate("DataView.prototype.setInt16.length"));
        }

        [TestMethod]
        public void getInt32()
        {
            Assert.AreEqual(0, Evaluate("new DataView(new ArrayBuffer(4)).getInt32(0)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(3)).getInt32(0)"));

            // length
            Assert.AreEqual(2, Evaluate("DataView.prototype.getInt32.length"));
        }

        [TestMethod]
        public void setInt32()
        {
            Execute(@"
                var buffer = new DataView(new ArrayBuffer(18));
                buffer.setInt32(0, 123456789);
                buffer.setInt32(5, 2345678901);
                buffer.setInt32(9, 123456789, true);
                buffer.setInt32(14, 2345678901, true);");

            // Check each byte.
            Assert.AreEqual(  7, Evaluate("buffer.getUint8(0)"));
            Assert.AreEqual( 91, Evaluate("buffer.getUint8(1)"));
            Assert.AreEqual(205, Evaluate("buffer.getUint8(2)"));
            Assert.AreEqual( 21, Evaluate("buffer.getUint8(3)"));
            Assert.AreEqual(  0, Evaluate("buffer.getUint8(4)"));
            Assert.AreEqual(139, Evaluate("buffer.getUint8(5)"));
            Assert.AreEqual(208, Evaluate("buffer.getUint8(6)"));
            Assert.AreEqual( 56, Evaluate("buffer.getUint8(7)"));
            Assert.AreEqual( 53, Evaluate("buffer.getUint8(8)"));
            Assert.AreEqual( 21, Evaluate("buffer.getUint8(9)"));
            Assert.AreEqual(205, Evaluate("buffer.getUint8(10)"));
            Assert.AreEqual( 91, Evaluate("buffer.getUint8(11)"));
            Assert.AreEqual(  7, Evaluate("buffer.getUint8(12)"));
            Assert.AreEqual(  0, Evaluate("buffer.getUint8(13)"));
            Assert.AreEqual( 53, Evaluate("buffer.getUint8(14)"));
            Assert.AreEqual( 56, Evaluate("buffer.getUint8(15)"));
            Assert.AreEqual(208, Evaluate("buffer.getUint8(16)"));
            Assert.AreEqual(139, Evaluate("buffer.getUint8(17)"));

            Assert.AreEqual(123456789, Evaluate("buffer.getInt32(0)"));
            Assert.AreEqual(365779719, Evaluate("buffer.getInt32(0, true)"));
            Assert.AreEqual(-1949288395, Evaluate("buffer.getInt32(5)"));
            Assert.AreEqual(892915851, Evaluate("buffer.getInt32(5, true)"));

            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setInt32(15, 2)"));

            // length
            Assert.AreEqual(3, Evaluate("DataView.prototype.setInt32.length"));
        }

        [TestMethod]
        public void getUint8()
        {
            Assert.AreEqual(0, Evaluate("new DataView(new ArrayBuffer(1)).getUint8(0)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(0)).getUint8(0)"));

            // length
            Assert.AreEqual(1, Evaluate("DataView.prototype.getUint8.length"));
        }

        [TestMethod]
        public void setUint8()
        {
            Execute("var buffer = new DataView(new ArrayBuffer(2));");
            Assert.AreEqual(2, Evaluate("buffer.setUint8(0, 2); buffer.getUint8(0)"));
            Assert.AreEqual(3, Evaluate("buffer.setUint8(1, 3); buffer.getUint8(1)"));
            Assert.AreEqual(128, Evaluate("buffer.setUint8(1, 128); buffer.getUint8(1)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setUint8(2, 2)"));

            // length
            Assert.AreEqual(2, Evaluate("DataView.prototype.setUint8.length"));
        }

        [TestMethod]
        public void getUint16()
        {
            Assert.AreEqual(0, Evaluate("new DataView(new ArrayBuffer(2)).getUint16(0)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(1)).getUint16(0)"));

            // length
            Assert.AreEqual(2, Evaluate("DataView.prototype.getUint16.length"));
        }

        [TestMethod]
        public void setUint16()
        {
            Execute(@"
                var buffer = new DataView(new ArrayBuffer(10));
                buffer.setUint16(0, 4097);
                buffer.setUint16(3, 45678);
                buffer.setUint16(5, 4097, true);
                buffer.setUint16(8, 45678, true);");

            // Check each byte.
            Assert.AreEqual(16, Evaluate("buffer.getUint8(0)"));
            Assert.AreEqual(1, Evaluate("buffer.getUint8(1)"));
            Assert.AreEqual(0, Evaluate("buffer.getUint8(2)"));
            Assert.AreEqual(178, Evaluate("buffer.getUint8(3)"));
            Assert.AreEqual(110, Evaluate("buffer.getUint8(4)"));
            Assert.AreEqual(1, Evaluate("buffer.getUint8(5)"));
            Assert.AreEqual(16, Evaluate("buffer.getUint8(6)"));
            Assert.AreEqual(0, Evaluate("buffer.getUint8(7)"));
            Assert.AreEqual(110, Evaluate("buffer.getUint8(8)"));
            Assert.AreEqual(178, Evaluate("buffer.getUint8(9)"));

            Assert.AreEqual(4097, Evaluate("buffer.getUint16(0)"));
            Assert.AreEqual(272, Evaluate("buffer.getUint16(0, true)"));
            Assert.AreEqual(45678, Evaluate("buffer.getUint16(3)"));
            Assert.AreEqual(28338, Evaluate("buffer.getUint16(3, true)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setUint16(9, 2)"));

            // length
            Assert.AreEqual(3, Evaluate("DataView.prototype.setUint16.length"));
        }

        [TestMethod]
        public void getUint32()
        {
            Assert.AreEqual(0, Evaluate("new DataView(new ArrayBuffer(4)).getUint32(0)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(3)).getUint32(0)"));

            // length
            Assert.AreEqual(2, Evaluate("DataView.prototype.getUint32.length"));
        }

        [TestMethod]
        public void setUint32()
        {
            Execute(@"
                var buffer = new DataView(new ArrayBuffer(18));
                buffer.setUint32(0, 123456789);
                buffer.setUint32(5, 2345678901);
                buffer.setUint32(9, 123456789, true);
                buffer.setUint32(14, 2345678901, true);");

            // Check each byte.
            Assert.AreEqual(7, Evaluate("buffer.getUint8(0)"));
            Assert.AreEqual(91, Evaluate("buffer.getUint8(1)"));
            Assert.AreEqual(205, Evaluate("buffer.getUint8(2)"));
            Assert.AreEqual(21, Evaluate("buffer.getUint8(3)"));
            Assert.AreEqual(0, Evaluate("buffer.getUint8(4)"));
            Assert.AreEqual(139, Evaluate("buffer.getUint8(5)"));
            Assert.AreEqual(208, Evaluate("buffer.getUint8(6)"));
            Assert.AreEqual(56, Evaluate("buffer.getUint8(7)"));
            Assert.AreEqual(53, Evaluate("buffer.getUint8(8)"));
            Assert.AreEqual(21, Evaluate("buffer.getUint8(9)"));
            Assert.AreEqual(205, Evaluate("buffer.getUint8(10)"));
            Assert.AreEqual(91, Evaluate("buffer.getUint8(11)"));
            Assert.AreEqual(7, Evaluate("buffer.getUint8(12)"));
            Assert.AreEqual(0, Evaluate("buffer.getUint8(13)"));
            Assert.AreEqual(53, Evaluate("buffer.getUint8(14)"));
            Assert.AreEqual(56, Evaluate("buffer.getUint8(15)"));
            Assert.AreEqual(208, Evaluate("buffer.getUint8(16)"));
            Assert.AreEqual(139, Evaluate("buffer.getUint8(17)"));

            Assert.AreEqual(123456789, Evaluate("buffer.getUint32(0)"));
            Assert.AreEqual(365779719, Evaluate("buffer.getUint32(0, true)"));
            Assert.AreEqual(2345678901.0, Evaluate("buffer.getUint32(5)"));
            Assert.AreEqual(892915851, Evaluate("buffer.getUint32(5, true)"));

            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setUint32(15, 2)"));

            // length
            Assert.AreEqual(3, Evaluate("DataView.prototype.setUint32.length"));
        }

        [TestMethod]
        public void getFloat32()
        {
            Assert.AreEqual(0, Evaluate("new DataView(new ArrayBuffer(4)).getFloat32(0)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(3)).getFloat32(0)"));

            // length
            Assert.AreEqual(2, Evaluate("DataView.prototype.getFloat32.length"));
        }

        [TestMethod]
        public void setFloat32()
        {
            Execute(@"
                var buffer = new DataView(new ArrayBuffer(8));
                buffer.setFloat32(0, 1.234567890123456789);
                buffer.setFloat32(4, 1.234567890123456789, true);");

            // Check each byte.
            Assert.AreEqual(63, Evaluate("buffer.getUint8(0)"));
            Assert.AreEqual(158, Evaluate("buffer.getUint8(1)"));
            Assert.AreEqual(6, Evaluate("buffer.getUint8(2)"));
            Assert.AreEqual(82, Evaluate("buffer.getUint8(3)"));
            Assert.AreEqual(82, Evaluate("buffer.getUint8(4)"));
            Assert.AreEqual(6, Evaluate("buffer.getUint8(5)"));
            Assert.AreEqual(158, Evaluate("buffer.getUint8(6)"));
            Assert.AreEqual(63, Evaluate("buffer.getUint8(7)"));

            Assert.AreEqual(1.2345678806304932, Evaluate("buffer.getFloat32(0)"));
            Assert.AreEqual(144545136640.0, Evaluate("buffer.getFloat32(0, true)"));

            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setFloat32(5, 1.23)"));

            // length
            Assert.AreEqual(3, Evaluate("DataView.prototype.setFloat32.length"));
        }

        [TestMethod]
        public void getFloat64()
        {
            Assert.AreEqual(0, Evaluate("new DataView(new ArrayBuffer(8)).getFloat64(0)"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new DataView(new ArrayBuffer(7)).getFloat64(0)"));

            // length
            Assert.AreEqual(2, Evaluate("DataView.prototype.getFloat64.length"));
        }

        [TestMethod]
        public void setFloat64()
        {
            Execute(@"
                var buffer = new DataView(new ArrayBuffer(16));
                buffer.setFloat64(0, 1.234567890123456789);
                buffer.setFloat64(8, 1.234567890123456789, true);");

            // Check each byte.
            Assert.AreEqual( 63, Evaluate("buffer.getUint8(0)"));
            Assert.AreEqual(243, Evaluate("buffer.getUint8(1)"));
            Assert.AreEqual(192, Evaluate("buffer.getUint8(2)"));
            Assert.AreEqual(202, Evaluate("buffer.getUint8(3)"));
            Assert.AreEqual( 66, Evaluate("buffer.getUint8(4)"));
            Assert.AreEqual(140, Evaluate("buffer.getUint8(5)"));
            Assert.AreEqual( 89, Evaluate("buffer.getUint8(6)"));
            Assert.AreEqual(251, Evaluate("buffer.getUint8(7)"));
            Assert.AreEqual(251, Evaluate("buffer.getUint8(8)"));
            Assert.AreEqual( 89, Evaluate("buffer.getUint8(9)"));
            Assert.AreEqual(140, Evaluate("buffer.getUint8(10)"));
            Assert.AreEqual( 66, Evaluate("buffer.getUint8(11)"));
            Assert.AreEqual(202, Evaluate("buffer.getUint8(12)"));
            Assert.AreEqual(192, Evaluate("buffer.getUint8(13)"));
            Assert.AreEqual(243, Evaluate("buffer.getUint8(14)"));
            Assert.AreEqual( 63, Evaluate("buffer.getUint8(15)"));

            Assert.AreEqual(1.234567890123456789, Evaluate("buffer.getFloat64(0)"));
            Assert.AreEqual(-1.5196060239826275e+286, Evaluate("buffer.getFloat64(0, true)"));

            Assert.AreEqual("RangeError", EvaluateExceptionType("buffer.setFloat64(9, 1.23)"));

            // length
            Assert.AreEqual(3, Evaluate("DataView.prototype.setFloat64.length"));
        }
    }
}
