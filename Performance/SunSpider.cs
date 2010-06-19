using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Library;

namespace Performance
{

    /// <summary>
    /// Test the Parser object.
    /// </summary>
    [TestClass]
    public class SunSpider
    {
        [TestMethod]
        public void threed_cube()
        {
            RunTest(@"3d-cube.js", 1488);
        }

        [TestMethod]
        public void threed_morph()
        {
            RunTest(@"3d-morph.js", 967);
        }

        [TestMethod]
        public void threed_raytrace()
        {
            RunTest(@"3d-raytrace.js", 1495);
        }

        [TestMethod]
        public void access_binary_trees()
        {
            RunTest(@"access-binary-trees.js", 1253);
        }

        [TestMethod]
        public void access_fannkuch()
        {
            RunTest(@"access-fannkuch.js", 3291);
        }

        [TestMethod]
        public void access_nbody()
        {
            RunTest(@"access-nbody.js", 1142);
        }

        [TestMethod]
        public void access_nsieve()
        {
            RunTest(@"access-nsieve.js", 1377);
        }

        [TestMethod]
        public void bitops_3bit_bits_in_byte()
        {
            RunTest(@"bitops-3bit-bits-in-byte.js", 1027);
        }

        [TestMethod]
        public void bitops_bits_in_byte()
        {
            RunTest(@"bitops-bits-in-byte.js", 1482);
        }

        [TestMethod]
        public void bitops_bitwise_and()
        {
            RunTest(@"bitops-bitwise-and.js", 906);
        }

        [TestMethod]
        public void bitops_nsieve_bits()
        {
            RunTest(@"bitops-nsieve-bits.js", 1888);
        }

        [TestMethod]
        public void controlflow_recursive()
        {
            RunTest(@"controlflow-recursive.js", 1245);
        }

        [TestMethod]
        public void crypto_aes()
        {
            RunTest(@"crypto-aes.js", 1353);
        }

        [TestMethod]
        public void crypto_md5()
        {
            RunTest(@"crypto-md5.js", 1302);
        }

        [TestMethod]
        public void crypto_sha1()
        {
            RunTest(@"crypto-sha1.js", 1096);
        }

        [TestMethod]
        public void date_format_tofte()
        {
            RunTest(@"date-format-tofte.js", 627);
        }

        [TestMethod]
        public void date_format_xparb()
        {
            RunTest(@"date-format-xparb.js", 725);
        }

        [TestMethod]
        public void math_cordic()
        {
            RunTest(@"math-cordic.js", 1762);
        }

        [TestMethod]
        public void math_partial_sums()
        {
            RunTest(@"math-partial-sums.js", 699);
        }

        [TestMethod]
        public void math_spectral_norm()
        {
            RunTest(@"math-spectral-norm.js", 693);
        }

        [TestMethod]
        public void regexp_dna()
        {
            RunTest(@"regexp-dna.js", 1546);
        }

        [TestMethod]
        public void string_base64()
        {
            RunTest(@"string-base64.js", 1165);
        }

        [TestMethod]
        public void string_fasta()
        {
            RunTest(@"string-fasta.js", 1036);
        }

        [TestMethod]
        public void string_tagcloud()
        {
            RunTest(@"string-tagcloud.js", 4175);
        }

        [TestMethod]
        public void string_unpack_code()
        {
            RunTest(@"string-unpack-code.js", 246);
        }

        [TestMethod]
        public void string_validate_input()
        {
            RunTest(@"string-validate-input.js", 7408);
        }

        //[TestMethod]
        //public void RunAllTests()
        //{
        //    var timer = System.Diagnostics.Stopwatch.StartNew();
        //    foreach (string path in Directory.EnumerateFiles(@"..\..\..\Performance\Files\sunspider-0.9.1"))
        //        RunTest(path, 0, false);
        //    Assert.Inconclusive(string.Format("{0}ms, was 40355ms", timer.ElapsedMilliseconds));
        //}

        private void RunTest(string scriptPath, double previous, bool assertResults = true)
        {
            scriptPath = Path.Combine(@"..\..\..\Performance\Files\sunspider-0.9.1\", scriptPath);
            var script = File.ReadAllText(scriptPath);
            var timer = System.Diagnostics.Stopwatch.StartNew();

            // Parse the javascript code.
            var parser = new Jurassic.Compiler.Parser(new Jurassic.Compiler.Lexer(new StreamReader(scriptPath), scriptPath), Jurassic.Compiler.ScriptContext.Global);
            parser.EnableDebugging = true;
            var expressionTree = parser.Parse();
            double parseTime = timer.Elapsed.TotalMilliseconds;

            // Compile the code.
            var func = parser.Compile(expressionTree);
            var scope = new Jurassic.Compiler.LexicalScope(null, GlobalObject.Instance, false);
            double compilationTime = timer.Elapsed.TotalMilliseconds - parseTime;

            // Run the javascript code.
            func(scope);

            double runTime = timer.Elapsed.TotalMilliseconds - compilationTime;
            string infoString = string.Format("{1:n1}ms (parse: {2:n1}ms, compile: {3:n1}ms, runtime: {4:n1}ms)",
                Path.GetFileNameWithoutExtension(scriptPath),
                parseTime + compilationTime + runTime,
                parseTime,
                compilationTime,
                runTime);
            if (previous > 0)
                infoString += string.Format(", was {0:n1}ms", previous);
            Console.WriteLine("{0}: {1}", Path.GetFileNameWithoutExtension(scriptPath), infoString);
            if (assertResults == true)
                throw new AssertInconclusiveException(infoString);
        }
    }

}