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
        [ClassInitialize]
        public static void WarmUp(TestContext context)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            GlobalObject.Eval("qwerew = 1");
            context.WriteLine("Warm up time: {0:n2}ms", timer.Elapsed.TotalMilliseconds);
        }

        [TestMethod]
        public void crypto()
        {
            RunTest(@"v8-crypto.js", 0);
        }

        [TestMethod]
        public void deltablue()
        {
            RunTest(@"v8-deltablue.js", 0);
        }

        [TestMethod]
        public void earley_boyer()
        {
            RunTest(@"v8-earley-boyer.js", 0);
        }

        [TestMethod]
        public void raytrace()
        {
            RunTest(@"v8-raytrace.js", 13180);
        }

        [TestMethod]
        public void regexp()
        {
            RunTest(@"v8-regexp.js", 7367);
        }

        [TestMethod]
        public void richards()
        {
            RunTest(@"v8-richards.js", 0);
        }

        [TestMethod]
        public void splay()
        {
            RunTest(@"v8-splay.js", 0);
        }

        [TestMethod]
        public void RunAllTests()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            foreach (string path in Directory.EnumerateFiles(@"..\..\..\Performance\Files\v8-v4"))
                RunTest(path, 0, false);
            Assert.Inconclusive(string.Format("{0}ms, was 24471ms", timer.ElapsedMilliseconds));
        }

        private void RunTest(string scriptPath, double previous, bool assertResults = true)
        {
            scriptPath = Path.Combine(@"..\..\..\Performance\Files\v8-v4\", scriptPath);
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