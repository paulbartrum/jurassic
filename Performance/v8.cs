using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Library;

namespace Performance
{

    /// <summary>
    /// Run the v8 performance tests.
    /// </summary>
    [TestClass]
    public class v8
    {
        private static ScriptEngineManager manager;

        [ClassInitialize]
        public static void WarmUp(TestContext context)
        {
            manager = new ScriptEngineManager(@"..\..\..\Performance\Files\v8\", (engine) =>
            {
                // Replace the default random number generator with a deterministic one.
                engine.Math["random"] = engine.Evaluate(@"
                    (function() {
                      var seed = 49734321;
                      return function() {
                        // Robert Jenkins' 32 bit integer hash function.
                        seed = ((seed + 0x7ed55d16) + (seed << 12))  & 0xffffffff;
                        seed = ((seed ^ 0xc761c23c) ^ (seed >>> 19)) & 0xffffffff;
                        seed = ((seed + 0x165667b1) + (seed << 5))   & 0xffffffff;
                        seed = ((seed + 0xd3a2646c) ^ (seed << 9))   & 0xffffffff;
                        seed = ((seed + 0xfd7046c5) + (seed << 3))   & 0xffffffff;
                        seed = ((seed ^ 0xb55a4f09) ^ (seed >>> 16)) & 0xffffffff;
                        return (seed & 0xfffffff) / 0x10000000;
                      };
                    })();");

                // Some of the tests call alert if there is an error.  Since alert is not supported,
                // translate this into throwing an exception.
                engine.Global["alert"] = engine.Evaluate("(function alert(str) { throw new Error(str) })");

                // For debugging purposes, define the console object.
                engine.Global["console"] = new Jurassic.Library.FirebugConsole(engine);

            });
        }

        [TestMethod]
        public void crypto()
        {
            manager.RunTest("crypto.js", 27308.3);
        }

        [TestMethod]
        public void deltablue()
        {
            manager.RunTest("deltablue.js", 352.3);
        }

        [TestMethod]
        public void earley_boyer()
        {
            // Test uses octal escape sequences.
            manager.ScriptEngine.CompatibilityMode = Jurassic.CompatibilityMode.ECMAScript3;
            try
            {
                manager.RunTest("earley-boyer.js", 27067);
            }
            finally
            {
                manager.ScriptEngine.CompatibilityMode = Jurassic.CompatibilityMode.Latest;
            }
        }

        [TestMethod]
        public void raytrace()
        {
            manager.RunTest("raytrace.js", 2345);
        }

        [TestMethod]
        public void regexp()
        {
            manager.RunTest("regexp.js", 3294);
        }

        [TestMethod]
        public void richards()
        {
            manager.RunTest("richards.js", 298.7);
        }

        [TestMethod]
        public void splay()
        {
            manager.RunTest(@"splay.js", 6873);
        }

    }

}