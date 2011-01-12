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

        [TestMethod]
        public void LZW()
        {
            // From http://rosettacode.org/wiki/LZW_compression#JavaScript
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\lzw.js");
            Assert.AreEqual("109,97,114,121,32,104,97,100,32,97,32,108,105,116,116,108,101,266,97,109,98",
                engine.Evaluate(@"LZW.compress('mary had a little lamb').toString()"));
            Assert.AreEqual("mary had a little lamb",
                engine.Evaluate(@"LZW.decompress([109,97,114,121,32,104,97,100,32,97,32,108,105,116,116,108,101,266,97,109,98]).toString()"));
            Assert.AreEqual("122,256,257,258,259,260,257",
                engine.Evaluate(@"LZW.compress('zzzzzzzzzzzzzzzzzzzzzzzz').toString()"));
            Assert.AreEqual("zzzzzzzzzzzzzzzzzzzzzzzz",
                engine.Evaluate(@"LZW.decompress([122,256,257,258,259,260,257]).toString()"));
        }

        [TestMethod]
        public void RSAEncrypt()
        {
            // From http://xenon.stanford.edu/~tjw/jsbn/
            var engine = new ScriptEngine();
            engine.EnableDebugging = true;
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\rsa.js");
            engine.Execute(@"
                var rsa = new RSAKey();
                rsa.setPublic('a5261939975948bb7a58dffe5ff54e65f0498f9175f5a09288810b8975871e99\n' +
                    'af3b5dd94057b0fc07535f5f97444504fa35169d461d0d30cf0192e307727c06\n' + 
                    '5168c788771c561a9400fb49175e9e6aa4e23fe11af69e9412dd23b0cb6684c4\n' +
                    'c2429bce139e848ab26d0829073351f4acd36074eafd036a5eb83359d2a698d3', '10001');
                var encrypted = rsa.encrypt('text to encrypt');
                rsa.setPrivateEx('a5261939975948bb7a58dffe5ff54e65f0498f9175f5a09288810b8975871e99\n' +
                    'af3b5dd94057b0fc07535f5f97444504fa35169d461d0d30cf0192e307727c06\n' + 
                    '5168c788771c561a9400fb49175e9e6aa4e23fe11af69e9412dd23b0cb6684c4\n' +
                    'c2429bce139e848ab26d0829073351f4acd36074eafd036a5eb83359d2a698d3',
                    '10001',
                    '8e9912f6d3645894e8d38cb58c0db81ff516cf4c7e5a14c7f1eddb1459d2cded\n4d8d293fc97aee6aefb861859c8b6a3d1dfe710463e1f9ddc72048c09751971c\n4a580aa51eb523357a3cc48d31cfad1d4a165066ed92d4748fb6571211da5cb1\n4bc11b6e2df7c1a559e6d5ac1cd5c94703a22891464fba23d0d965086277a161',
                    'd090ce58a92c75233a6486cb0a9209bf3583b64f540c76f5294bb97d285eed33\naec220bde14b2417951178ac152ceab6da7090905b478195498b352048f15e7d',
                    'cab575dc652bb66df15a0359609d51d1db184750c00c6698b90ef3465c996551\n03edbf0d54c56aec0ce3c4d22592338092a126a0cc49f65a4a30d222b411e58f',
                    '1a24bca8e273df2f0e47c199bbf678604e7df7215480c77c8db39f49b000ce2c\nf7500038acfff5433b7d582a01f1826e6f4d42e1c57f5e1fef7b12aabc59fd25',
                    '3d06982efbbe47339e1f6d36b1216b8a741d410b0c662f54f7118b27b9a4ec9d\n914337eb39841d8666f3034408cf94f5b62f11c402fc994fe15a05493150d9fd',
                    '3a3e731acd8960b7ff9eb81a7ff93bd1cfa74cbd56987db58b4594fb09c09084\ndb1734c8143f98b602b981aaa9243ca28deb69b5b280ee8dcee0fd2625e53250');");
            Assert.AreEqual("", engine.Evaluate("rsa.decrypt(encrypted)"));
        }

        [TestMethod]
        public void Levenshtein()
        {
            // From http://phpjs.org/functions/levenshtein
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\levenshtein.js");
            Assert.AreEqual(3, engine.Evaluate(@"levenshtein('Kevin van Zonneveld', 'Kevin van Sommeveld')"));
            Assert.AreEqual(5, engine.Evaluate(@"levenshtein('Phoney Malony', 'Fonee Malowney')"));
            Assert.AreEqual(7, engine.Evaluate(@"levenshtein('Phoney Malony', 'Bogey Gilooni')"));
            Assert.AreEqual(14, engine.Evaluate(@"levenshtein('Phoney Malony', 'Major Carrothead')"));
        }
    }
}
