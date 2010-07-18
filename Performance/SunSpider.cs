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
        [ClassInitialize]
        public static void WarmUp(TestContext context)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            GlobalObject.Eval("qwerew = 1");
            context.WriteLine("Warm up time: {0:n2}ms", timer.Elapsed.TotalMilliseconds);
        }

        [TestMethod]
        public void threed_cube()
        {
            RunTest(@"3d-cube.js", 217.5);
        }

        [TestMethod]
        public void threed_morph()
        {
            RunTest(@"3d-morph.js", 596.1);
        }

        [TestMethod]
        public void threed_raytrace()
        {
            RunTest(@"3d-raytrace.js", 597.9);
        }

        [TestMethod]
        public void access_binary_trees()
        {
            RunTest(@"access-binary-trees.js", 1152.5);
        }

        [TestMethod]
        public void access_fannkuch()
        {
            RunTest(@"access-fannkuch.js", 1575.7);
        }

        [TestMethod]
        public void access_nbody()
        {
            RunTest(@"access-nbody.js", 718.9);
        }

        [TestMethod]
        public void access_nsieve()
        {
            RunTest(@"access-nsieve.js", 617.6);
        }

        [TestMethod]
        public void bitops_3bit_bits_in_byte()
        {
            RunTest(@"bitops-3bit-bits-in-byte.js", 161.6);
        }

        [TestMethod]
        public void bitops_bits_in_byte()
        {
            RunTest(@"bitops-bits-in-byte.js", 202.9);
        }

        [TestMethod]
        public void bitops_bitwise_and()
        {
            RunTest(@"bitops-bitwise-and.js", 1082.8);
        }

        [TestMethod]
        public void bitops_nsieve_bits()
        {
            RunTest(@"bitops-nsieve-bits.js", 617.9);
        }

        [TestMethod]
        public void controlflow_recursive()
        {
            RunTest(@"controlflow-recursive.js", 162.8);
        }

        [TestMethod]
        public void crypto_aes()
        {
            RunTest(@"crypto-aes.js", 967.6);
        }

        [TestMethod]
        public void crypto_md5()
        {
            RunTest(@"crypto-md5.js", 784.0);
        }

        [TestMethod]
        public void crypto_sha1()
        {
            RunTest(@"crypto-sha1.js", 411.2);
        }

        [TestMethod]
        public void date_format_tofte()
        {
            RunTest(@"date-format-tofte.js", 660.3);
        }

        [TestMethod]
        public void date_format_xparb()
        {
            RunTest(@"date-format-xparb.js", 583.3);
        }

        [TestMethod]
        public void math_cordic()
        {
            RunTest(@"math-cordic.js", 530.7);
        }

        [TestMethod]
        public void math_partial_sums()
        {
            RunTest(@"math-partial-sums.js", 514.6);
        }

        [TestMethod]
        public void math_spectral_norm()
        {
            RunTest(@"math-spectral-norm.js", 335.2);
        }

        [TestMethod]
        public void regexp_dna()
        {
            RunTest(@"regexp-dna.js", 942.6);
        }

        [TestMethod]
        public void string_base64()
        {
            RunTest(@"string-base64.js", 2310.8);
        }

        [TestMethod]
        public void string_fasta()
        {
            RunTest(@"string-fasta.js", 1005.8);
        }

        [TestMethod]
        public void string_tagcloud()
        {
            RunTest(@"string-tagcloud.js", 2268.3);
        }

        [TestMethod]
        public void string_unpack_code()
        {
            RunTest(@"string-unpack-code.js", 3479.1);
        }

        [TestMethod]
        public void string_validate_input()
        {
            RunTest(@"string-validate-input.js", 5343.1);   // 4500-6000
        }

        [TestMethod]
        public void RunAllTests()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            foreach (string path in Directory.EnumerateFiles(@"..\..\..\Performance\Files\sunspider-0.9.1"))
                RunTest(path, 0, false);
            Assert.Inconclusive(string.Format("{0}ms, was 40355ms", timer.ElapsedMilliseconds));
        }

        private void RunTest(string scriptPath, double previous, bool assertResults = true)
        {
            scriptPath = Path.Combine(@"..\..\..\Performance\Files\sunspider-0.9.1\", scriptPath);
            var script = File.ReadAllText(scriptPath);
            var timer = System.Diagnostics.Stopwatch.StartNew();

            // Parse the javascript code.
            var context = new Jurassic.Compiler.GlobalContext(new System.IO.StringReader(script), scriptPath);
            context.Parse();
            double parseTime = timer.Elapsed.TotalMilliseconds;

            // Optimize the code.
            timer.Restart();
            context.Optimize();
            double optimizationTime = timer.Elapsed.TotalMilliseconds;

            // Compile the code.
            timer.Restart();
            context.GenerateCode();
            double compilationTime = timer.Elapsed.TotalMilliseconds;

            // Run the javascript code.
            timer.Restart();
            context.Execute();
            double runTime = timer.Elapsed.TotalMilliseconds;

            string infoString = string.Format("{0:n1}ms (parse: {1:n1}ms, compile: {2:n1}ms, optimize: {3:n1}ms, runtime: {4:n1}ms)",
                parseTime + compilationTime + runTime,
                parseTime,
                compilationTime,
                optimizationTime,
                runTime);
            if (previous > 0)
                infoString += string.Format(", was {0:n1}ms", previous);
            //Console.WriteLine("{0}: {1}", Path.GetFileNameWithoutExtension(scriptPath), infoString);
            Console.WriteLine("{0}\t{1:n1}\t{2:n1}\t{3:n1}\t{4:n1}\t{5:n1}",
                Path.GetFileNameWithoutExtension(scriptPath),
                parseTime,
                compilationTime,
                optimizationTime,
                runTime,
                parseTime + compilationTime + runTime);
            if (assertResults == true)
                throw new AssertInconclusiveException(infoString);
        }
    }

}