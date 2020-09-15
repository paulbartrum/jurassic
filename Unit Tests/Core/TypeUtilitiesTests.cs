using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the type utility routines.
    /// </summary>
    [TestClass]
    public class TypeUtilitiesTests : TestBase
    {
        [TestMethod]
        public void CreateListFromArrayLike()
        {
            // Basic array.
            var result = TypeUtilities.CreateListFromArrayLike((ObjectInstance)Evaluate("[1, 2, 3]"));
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);

            // Array with holes.
            result = TypeUtilities.CreateListFromArrayLike((ObjectInstance)Evaluate("[3, , 5]"));
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(3, result[0]);
            Assert.AreEqual(Undefined.Value, result[1]);
            Assert.AreEqual(5, result[2]);

            // Array-like.
            result = TypeUtilities.CreateListFromArrayLike((ObjectInstance)Evaluate("({ })"));
            Assert.AreEqual(0, result.Length);

            // Array-like.
            result = TypeUtilities.CreateListFromArrayLike((ObjectInstance)Evaluate("({ length: 3, 0: 4, 1: 2, 2: 0 })"));
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(4, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(0, result[2]);

            // Array-like with holes.
            result = TypeUtilities.CreateListFromArrayLike((ObjectInstance)Evaluate("({ length: 3, 0: 2, 2: 3 })"));
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(Undefined.Value, result[1]);
            Assert.AreEqual(3, result[2]);
        }
    }
}
