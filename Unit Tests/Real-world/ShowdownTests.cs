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
    /// Showdown tests (see http://attacklab.net/showdown/).
    /// </summary>
    [TestClass]
    public class ShowdownTests
    {
        [TestMethod]
        public void Showdown1()
        {
            var engine = new ScriptEngine();
            engine.EnableDebugging = true;
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\showdown.js");
            engine.Execute("var converter = new Showdown.converter()");
            engine.SetGlobalValue("text", @"
Showdown Demo
-------------

You can try out Showdown on this page:

  - Type some [Markdown] text on the left side.
  - See the corresponding HTML on the right.

For a Markdown cheat-sheet, switch the right-hand window from *Preview* to *Syntax Guide*.");
            Assert.AreEqual(@"<h2>Showdown Demo</h2>

<p>You can try out Showdown on this page:</p>

<ul>
<li>Type some [Markdown] text on the left side.</li>
<li>See the corresponding HTML on the right.</li>
</ul>

<p>For a Markdown cheat-sheet, switch the right-hand window from <em>Preview</em> to <em>Syntax Guide</em>.</p>", engine.Evaluate(@"converter.makeHtml(text);"));
        }

    }
}
