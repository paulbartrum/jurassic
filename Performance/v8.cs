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
    public class v8
    {
        [ThreadStatic]
        private static Jurassic.ScriptEngine engine;

        [ThreadStatic]
        private static System.Diagnostics.Stopwatch timer;

        [ThreadStatic]
        private static double parseTime, optimizationTime, codeGenerationTime;

        private static object originalRandom;

        [ClassInitialize]
        public static void WarmUp(TestContext context)
        {
            // Create a new script engine.
            var warmUpTimer = System.Diagnostics.Stopwatch.StartNew();
            engine = new Jurassic.ScriptEngine();
            context.WriteLine("Warm up time: {0:n2}ms", warmUpTimer.Elapsed.TotalMilliseconds);

            // Hook events to record statistics.
            engine.OptimizationStarted += (sender, e) => { parseTime = timer.Elapsed.TotalMilliseconds; timer.Restart(); };
            engine.CodeGenerationStarted += (sender, e) => { optimizationTime = timer.Elapsed.TotalMilliseconds; timer.Restart(); };
            engine.ExecutionStarted += (sender, e) => { codeGenerationTime = timer.Elapsed.TotalMilliseconds; timer.Restart(); };

            // Replace the default random number generator with a deterministic one.
            originalRandom = engine.Math["random"];
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
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            // Revert the random number generator.
            engine.Math["random"] = originalRandom;
        }

        [TestMethod]
        public void crypto()
        {
            // The test calls alert if there is an error.  Since alert is not supported,
            // translate this into throwing an exception.
            engine.Global["alert"] = engine.Evaluate("(function alert(str) { throw new Error(str) })");

            RunTest(@"crypto.js", 0);
        }

        [TestMethod]
        public void deltablue()
        {
            // The test calls alert if there is an error.  Since alert is not supported,
            // translate this into throwing an exception.
            engine.Global["alert"] = engine.Evaluate("(function alert(str) { throw new Error(str) })");
            
            // Run the deltablue test.
            RunTest(@"deltablue.js", 352.3);
        }

        [TestMethod]
        public void earley_boyer()
        {
            // The test calls alert if there is an error.  Since alert is not supported,
            // translate this into throwing an exception.
            engine.Global["alert"] = engine.Evaluate("(function alert(str) { throw new Error(str) })");

            RunTest(@"earley-boyer.js", 0);
        }

        [TestMethod]
        public void raytrace()
        {
            RunTest(@"raytrace.js", 2345);
        }

        [TestMethod]
        public void regexp()
        {
            RunTest(@"regexp.js", 3294);
        }

        [TestMethod]
        public void richards()
        {
            engine.Global["console"] = new Jurassic.Library.FirebugConsole(engine);
            RunTest(@"richards.js", 298.7);
        }

        [TestMethod]
        public void splay()
        {
            engine.Global["console"] = new Jurassic.Library.FirebugConsole(engine);
            RunTest(@"splay.js", 6873);
        }

        [TestMethod]
        public void RunAllTests()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            foreach (string path in Directory.EnumerateFiles(@"..\..\..\Performance\Files\v8"))
                RunTest(path, 0, false);
            Assert.Inconclusive(string.Format("{0}ms, was 24471ms", timer.ElapsedMilliseconds));
        }

        private void RunTest(string scriptPath, double previous, bool assertResults = true)
        {
            scriptPath = Path.Combine(@"..\..\..\Performance\Files\v8\", scriptPath);
            var script = File.ReadAllText(scriptPath);

            // Execute the javascript code.
            timer = System.Diagnostics.Stopwatch.StartNew();
            engine.Execute(new Jurassic.StringScriptSource(script));
            double runTime = timer.Elapsed.TotalMilliseconds;

            string infoString = string.Format("{0:n1}ms (parse: {1:n1}ms, compile: {2:n1}ms, optimize: {3:n1}ms, runtime: {4:n1}ms)",
                parseTime + optimizationTime + codeGenerationTime + runTime,
                parseTime,
                codeGenerationTime,
                optimizationTime,
                runTime);
            if (previous > 0)
                infoString += string.Format(", was {0:n1}ms", previous);
            //Console.WriteLine("{0}: {1}", Path.GetFileNameWithoutExtension(scriptPath), infoString);
            Console.WriteLine("{0}\t{1:n1}\t{2:n1}\t{3:n1}\t{4:n1}\t{5:n1}",
                Path.GetFileNameWithoutExtension(scriptPath),
                parseTime,
                codeGenerationTime,
                optimizationTime,
                runTime,
                parseTime + optimizationTime + codeGenerationTime + runTime);
            if (assertResults == true)
                throw new AssertInconclusiveException(infoString);
        }
    }

}