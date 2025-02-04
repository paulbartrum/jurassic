using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests {
  /// <summary>
  /// Test with() statement.
  /// </summary>
  [TestClass]
  public class WithTests : TestBase {
    [TestMethod]
    public void InFunction3() {
      Assert.AreEqual(
        1,
        Evaluate(
          "(function(obj) { var c = 0; (function() { c += 1; })(); return c;})();"
        )
      );
    }

    [TestMethod]
    public void InFunction() {
      Assert.AreEqual(
        1,
        Evaluate(
          "var a = null; (function(obj) { let a = 0; with (obj) { a = 1; }; return a; })({ b: 1 });"
        )
      );
    }

    [TestMethod]
    public void InFunction2() {
      Assert.AreEqual(
        1,
        Evaluate(
          "(function(obj) { var a = 0; with (obj) { a += b; }; return a; })({ b: 1 });"
        )
      );
    }

    [TestMethod]
    public void Alone() {
      Assert.AreEqual(
        1,
        Evaluate(
          "var a = 0; obj = { b: 1 }; with (obj) { a = b; }; a;"
        )
      );
    }
  }
}