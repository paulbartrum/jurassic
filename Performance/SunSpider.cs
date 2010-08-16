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
            manager.RunTest(@"3d-cube.js", 217.5);
        }

        [TestMethod]
        public void threed_morph()
        {
            manager.RunTest(@"3d-morph.js", 596.1);
        }

        [TestMethod]
        public void threed_raytrace()
        {
            manager.RunTest(@"3d-raytrace.js", 597.9);
        }

        [TestMethod]
        public void access_binary_trees()
        {
            manager.RunTest(@"access-binary-trees.js", 1152.5);
        }

        [TestMethod]
        public void access_fannkuch()
        {
            manager.RunTest(@"access-fannkuch.js", 1575.7);
        }

        [TestMethod]
        public void access_nbody()
        {
            manager.RunTest(@"access-nbody.js", 718.9);
        }

        [TestMethod]
        public void access_nsieve()
        {
            manager.RunTest(@"access-nsieve.js", 617.6);
        }

        [TestMethod]
        public void bitops_3bit_bits_in_byte()
        {
            manager.RunTest(@"bitops-3bit-bits-in-byte.js", 161.6);
        }

        [TestMethod]
        public void bitops_bits_in_byte()
        {
            manager.RunTest(@"bitops-bits-in-byte.js", 202.9);
        }

        [TestMethod]
        public void bitops_bitwise_and()
        {
            manager.RunTest(@"bitops-bitwise-and.js", 1082.8);
        }

        [TestMethod]
        public void bitops_nsieve_bits()
        {
            manager.RunTest(@"bitops-nsieve-bits.js", 617.9);
        }

        [TestMethod]
        public void controlflow_recursive()
        {
            manager.RunTest(@"controlflow-recursive.js", 162.8);
        }

        [TestMethod]
        public void crypto_aes()
        {
            manager.RunTest(@"crypto-aes.js", 967.6);
        }

        [TestMethod]
        public void crypto_md5()
        {
            manager.RunTest(@"crypto-md5.js", 784.0);
        }

        [TestMethod]
        public void crypto_sha1()
        {
            manager.RunTest(@"crypto-sha1.js", 411.2);
        }

        [TestMethod]
        public void date_format_tofte()
        {
            manager.RunTest(@"date-format-tofte.js", 660.3);
        }

        [TestMethod]
        public void date_format_xparb()
        {
            manager.RunTest(@"date-format-xparb.js", 583.3);
        }

        [TestMethod]
        public void math_cordic()
        {
            manager.RunTest(@"math-cordic.js", 530.7);
        }

        [TestMethod]
        public void math_partial_sums()
        {
            manager.RunTest(@"math-partial-sums.js", 514.6);
        }

        [TestMethod]
        public void math_spectral_norm()
        {
            manager.RunTest(@"math-spectral-norm.js", 335.2);
        }

        [TestMethod]
        public void regexp_dna()
        {
            manager.RunTest(@"regexp-dna.js", 942.6);
        }

        [TestMethod]
        public void string_base64()
        {
            manager.RunTest(@"string-base64.js", 2310.8);
        }

        [TestMethod]
        public void string_fasta()
        {
            manager.RunTest(@"string-fasta.js", 1005.8);
        }

        [TestMethod]
        public void string_tagcloud()
        {
            manager.RunTest(@"string-tagcloud.js", 2268.3);
        }

        [TestMethod]
        public void string_unpack_code()
        {
            manager.RunTest(@"string-unpack-code.js", 3479.1);
        }

        [TestMethod]
        public void string_validate_input()
        {
            manager.RunTest(@"string-validate-input.js", 5343.1);   // 4500-6000
        }

    }

}