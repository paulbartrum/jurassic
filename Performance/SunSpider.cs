using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Library;

namespace Performance
{

    /// <summary>
    /// Run the SunSpider performance tests.
    /// </summary>
    [TestClass]
    public class SunSpider
    {
        private static ScriptEngineManager manager;

        [ClassInitialize]
        public static void WarmUp(TestContext context)
        {
            manager = new ScriptEngineManager(@"..\..\..\Performance\Files\sunspider-0.9.1\");
        }

        [TestMethod]
        public void threed_cube()
        {
            manager.RunTest(@"3d-cube.js", 408);
        }

        [TestMethod]
        public void threed_morph()
        {
            manager.RunTest(@"3d-morph.js", 205);
        }

        [TestMethod]
        public void threed_raytrace()
        {
            manager.RunTest(@"3d-raytrace.js", 443);
        }

        [TestMethod]
        public void access_binary_trees()
        {
            manager.RunTest(@"access-binary-trees.js", 503);
        }

        [TestMethod]
        public void access_fannkuch()
        {
            manager.RunTest(@"access-fannkuch.js", 543);
        }

        [TestMethod]
        public void access_nbody()
        {
            manager.RunTest(@"access-nbody.js", 218);
        }

        [TestMethod]
        public void access_nsieve()
        {
            manager.RunTest(@"access-nsieve.js", 357);
        }

        [TestMethod]
        public void bitops_3bit_bits_in_byte()
        {
            manager.RunTest(@"bitops-3bit-bits-in-byte.js", 27);
        }

        [TestMethod]
        public void bitops_bits_in_byte()
        {
            manager.RunTest(@"bitops-bits-in-byte.js", 27);
        }

        [TestMethod]
        public void bitops_bitwise_and()
        {
            manager.RunTest(@"bitops-bitwise-and.js", 51);
        }

        [TestMethod]
        public void bitops_nsieve_bits()
        {
            manager.RunTest(@"bitops-nsieve-bits.js", 263);
        }

        [TestMethod]
        public void controlflow_recursive()
        {
            manager.RunTest(@"controlflow-recursive.js", 41);
        }

        [TestMethod]
        public void crypto_aes()
        {
            manager.RunTest(@"crypto-aes.js", 434);
        }

        [TestMethod]
        public void crypto_md5()
        {
            manager.RunTest(@"crypto-md5.js", 311);
        }

        [TestMethod]
        public void crypto_sha1()
        {
            manager.RunTest(@"crypto-sha1.js", 176);
        }

        [TestMethod]
        public void date_format_tofte()
        {
            manager.RunTest(@"date-format-tofte.js", 650);
        }

        [TestMethod]
        public void date_format_xparb()
        {
            manager.RunTest(@"date-format-xparb.js", 230);
        }

        [TestMethod]
        public void math_cordic()
        {
            manager.RunTest(@"math-cordic.js", 156);
        }

        [TestMethod]
        public void math_partial_sums()
        {
            manager.RunTest(@"math-partial-sums.js", 80);
        }

        [TestMethod]
        public void math_spectral_norm()
        {
            manager.RunTest(@"math-spectral-norm.js", 134);
        }

        [TestMethod]
        public void regexp_dna()
        {
            manager.RunTest(@"regexp-dna.js", 730);
        }

        [TestMethod]
        public void string_base64()
        {
            manager.RunTest(@"string-base64.js", 737);
        }

        [TestMethod]
        public void string_fasta()
        {
            manager.RunTest(@"string-fasta.js", 354);
        }

        [TestMethod]
        public void string_tagcloud()
        {
            manager.RunTest(@"string-tagcloud.js", 581);
        }

        [TestMethod]
        public void string_unpack_code()
        {
            manager.RunTest(@"string-unpack-code.js", 2995);
        }

        [TestMethod]
        public void string_validate_input()
        {
            manager.RunTest(@"string-validate-input.js", 377);
        }

        //[TestMethod]
        //public void RunAll()
        //{
        //    manager.RunAllTests(74683);
        //}
    }

}