using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Performance
{

    /// <summary>
    /// Benchmarks the parser and lexer.
    /// </summary>
    [TestClass]
    public class ParserBenchmarks
    {

        [TestMethod]
        public void CoffeeScript()
        {
            // Load coffee-script.js into a string variable.
            string script = System.IO.File.ReadAllText(@"..\..\..\Performance\Files\coffee-script.js");

            // Time the lex + parse.
            TestUtils.Benchmark(script, 0.929);
        }

    }

}