using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Compiler;

namespace UnitTests
{
    /// <summary>
    /// Real-world tests.
    /// </summary>
    [TestClass]
    public class RealWorldTests
    {
        [TestMethod]
        public void Showdown()
        {
            // See http://attacklab.net/showdown/
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\showdown.js");
            engine.Execute("var converter = new Showdown.converter()");
            engine.SetGlobalValue("text", @"
Showdown Demo
-------------

You can try out Showdown on this page:

  - Type some [Markdown] text on the left side.
  - See the corresponding HTML on the right.

For a Markdown cheat-sheet, switch the right-hand window from *Preview* to *Syntax Guide*.");
            Assert.AreEqual("<h2>Showdown Demo</h2>\n\n<p>You can try out Showdown on this page:</p>\n\n" +
                "<ul>\n<li>Type some [Markdown] text on the left side.</li>\n<li>See the corresponding HTML on the right.</li>\n" +
                "</ul>\n\n<p>For a Markdown cheat-sheet, switch the right-hand window from <em>Preview</em> to <em>Syntax Guide</em>.</p>",
                engine.Evaluate(@"converter.makeHtml(text);"));
        }

        [TestMethod]
        public void ColorConversion()
        {
            // http://www.webtoolkit.info/javascript-color-conversion.html
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\color-conversion.js");
            engine.Execute("var result = ColorConverter.toRGB(new HSV(10, 20, 30))");
            Assert.AreEqual(77, engine.Evaluate("result.r"));
            Assert.AreEqual(64, engine.Evaluate("result.g"));
            Assert.AreEqual(61, engine.Evaluate("result.b"));
        }

        [TestMethod]
        public void sprintf()
        {
            // From http://phpjs.org/functions/sprintf
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\sprintf.js");
            Assert.AreEqual("123.10", engine.Evaluate("sprintf('%01.2f', 123.1)"));
            Assert.AreEqual("[    monkey]", engine.Evaluate("sprintf('[%10s]', 'monkey')"));
            Assert.AreEqual("[####monkey]", engine.Evaluate("sprintf(\"[%'#10s]\", 'monkey')"));
        }

        [TestMethod]
        public void MD5()
        {
            // From http://www.webtoolkit.info/javascript-md5.html
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\md5.js");
            Assert.AreEqual("ae2b1fca515949e5d54fb22b8ed95575", engine.Evaluate(@"MD5('testing')"));
            Assert.AreEqual("023c0c18f0c6e89076d668146fcb81c2", engine.Evaluate(@"MD5('Mary had a little lamb, it\'s fleece was white as snow!')"));
            Assert.AreEqual("cbbcd86416057ca304141fc9b3b418d5", engine.Evaluate(@"MD5('\u2020')"));
        }

        [TestMethod]
        public void SHA1()
        {
            // From http://www.webtoolkit.info/javascript-sha1.html
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\sha1.js");
            Assert.AreEqual("dc724af18fbdd4e59189f5fe768a5f8311527050", engine.Evaluate(@"SHA1('testing')"));
            Assert.AreEqual("5fb03a9e2c9d14894b51a2bd0e521177e083af3f", engine.Evaluate(@"SHA1('Mary had a little lamb, it\'s fleece was white as snow!')"));
            Assert.AreEqual("5b7c3f4be781869083966e4b5eac6bd2900d9340", engine.Evaluate(@"SHA1('\u2020')"));
        }

        [TestMethod]
        public void SHA256()
        {
            // From http://www.webtoolkit.info/javascript-sha256.html
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\sha256.js");
            Assert.AreEqual("cf80cd8aed482d5d1527d7dc72fceff84e6326592848447d2dc0b0e87dfc9a90", engine.Evaluate(@"SHA256('testing')"));
            Assert.AreEqual("68aa952fc1ee38fd07c7d58e693b5e6bebaf183f1b47a1fc9f41cd42bbb427d2", engine.Evaluate(@"SHA256('Mary had a little lamb, it\'s fleece was white as snow!')"));
            Assert.AreEqual("8efeb7661b801b1f4f1286b262b89ec8550bfbb4b438fe9fb31be551e747a547", engine.Evaluate(@"SHA256('\u2020')"));
        }
    }
}
