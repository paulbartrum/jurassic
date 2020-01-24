using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the fetch() API.
    /// </summary>
    [TestClass]
    public class FetchTests : TestBase
    {
        [TestMethod]
        public void Headers()
        {
            
            
        }

        [TestMethod]
        public void Headers_get()
        {
            // Retrieve a set value.
            Assert.AreEqual("text/xml", Evaluate("new Headers({'Content-Type': 'text/xml'}).get('Content-Type')"));

            // Case-insensitive.
            Assert.AreEqual("text/xml", Evaluate("new Headers({'Content-Type': 'text/xml'}).get('CONTENT-type')"));
        }
    }
}
