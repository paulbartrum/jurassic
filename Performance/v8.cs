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
        private static object originalRandom;

        [ClassInitialize]
        public static void WarmUp(TestContext context)
        {
            // Replace the default random number generator with a deterministic one.
            originalRandom = GlobalObject.Math["random"];
            GlobalObject.Math["random"] = GlobalObject.Eval(@"
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
            GlobalObject.Math["random"] = originalRandom;
        }

        [TestMethod]
        public void crypto()
        {
            // The test calls alert if there is an error.  Since alert is not supported,
            // translate this into throwing an exception.
            GlobalObject.Instance["alert"] = GlobalObject.Eval("(function alert(str) { throw new Error(str) })");

            RunTest(@"crypto.js", 0);
        }

        [TestMethod]
        public void deltablue()
        {
            // The test calls alert if there is an error.  Since alert is not supported,
            // translate this into throwing an exception.
            GlobalObject.Instance["alert"] = GlobalObject.Eval("(function alert(str) { throw new Error(str) })");
            
            // Run the deltablue test.
            RunTest(@"deltablue.js", 352.3);
        }

        [TestMethod]
        public void earley_boyer()
        {
            // The test calls alert if there is an error.  Since alert is not supported,
            // translate this into throwing an exception.
            GlobalObject.Instance["alert"] = GlobalObject.Eval("(function alert(str) { throw new Error(str) })");

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
            GlobalObject.Instance["console"] = new Jurassic.Library.FirebugConsole();
            RunTest(@"richards.js", 298.7);
        }

        [TestMethod]
        public void splay()
        {
            GlobalObject.Instance["console"] = new Jurassic.Library.FirebugConsole();
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
            scriptPath = Path.GetFullPath(Path.Combine(@"..\..\..\Performance\Files\v8\", scriptPath));
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