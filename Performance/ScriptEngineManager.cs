using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace Performance
{

    /// <summary>
    /// Creates one script engine per thread.
    /// </summary>
    public class ScriptEngineManager
    {
        private string currentDir;
        private Action<ScriptEngine> initCallback;

        [ThreadStatic]
        private static ScriptEngine engine;

        [ThreadStatic]
        private static System.Diagnostics.Stopwatch timer;

        [ThreadStatic]
        private static double parseTime, optimizationTime, codeGenerationTime;

        /// <summary>
        /// Creates a new ScriptEngineManager instance.
        /// </summary>
        /// <param name="currentDir"> The path to start looking for script files. </param>
        /// <param name="initCallback"> Called to initialize the script engine. </param>
        public ScriptEngineManager(string currentDir, Action<ScriptEngine> initCallback = null)
        {
            if (currentDir == null)
                throw new ArgumentNullException("currentDir");
            this.currentDir = currentDir;
            this.initCallback = initCallback;
        }

        /// <summary>
        /// Gets the script engine for the current thread.
        /// </summary>
        public ScriptEngine ScriptEngine
        {
            get
            {
                if (engine == null)
                    engine = InitializeScriptEngine();
                return engine;
            }
        }

        /// <summary>
        /// Initializes the script engine (once per thread).
        /// </summary>
        /// <returns> An initialized script engine. </returns>
        private ScriptEngine InitializeScriptEngine()
        {
            // Initialize the script engine.
            var engine = new ScriptEngine();

#if DEBUG
            engine.EnableDebugging = true;
#endif

            engine.OptimizationStarted += (sender, e) => { parseTime = timer.Elapsed.TotalMilliseconds; timer.Restart(); };
            engine.CodeGenerationStarted += (sender, e) => { optimizationTime = timer.Elapsed.TotalMilliseconds; timer.Restart(); };
            engine.ExecutionStarted += (sender, e) => { codeGenerationTime = timer.Elapsed.TotalMilliseconds; timer.Restart(); };
            
            // Call the user-supplied init callback.
            if (initCallback != null)
            {
                timer = Stopwatch.StartNew();
                this.initCallback(engine);
            }

            return engine;
        }

        /// <summary>
        /// Runs a test script.
        /// </summary>
        /// <param name="scriptPath"> The path to the script. </param>
        /// <param name="previousTime"> The previous time required to run the test. </param>
        /// <param name="assertResults"> <c>true</c> to throw an AssertInconclusiveException. </param>
        public void RunTest(string scriptPath, double previousTime, bool assertResults = true)
        {
            // Calculate the full path.
            if (this.currentDir != null)
                scriptPath = Path.Combine(this.currentDir, scriptPath);

            // Read the script file.
            var script = File.ReadAllText(scriptPath);

            // Initialize the script engine.
            var scriptEngine = this.ScriptEngine;

            // Run once to warm up the cache.
            timer = System.Diagnostics.Stopwatch.StartNew();
            scriptEngine.Execute(new Jurassic.StringScriptSource(script, scriptPath));

            // Execute the javascript code.
            timer = System.Diagnostics.Stopwatch.StartNew();
            scriptEngine.Execute(new Jurassic.StringScriptSource(script, scriptPath));
            double runTime = timer.Elapsed.TotalMilliseconds;

            string infoString = string.Format("{0:n1}ms (parse: {1:n1}ms, compile: {2:n1}ms, optimize: {3:n1}ms, runtime: {4:n1}ms)",
                parseTime + optimizationTime + codeGenerationTime + runTime,
                parseTime,
                codeGenerationTime,
                optimizationTime,
                runTime);
            if (previousTime > 0)
                infoString += string.Format(", was {0:n1}ms", previousTime);
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

            //var engine = new Jint.JintEngine();
            //var timer = System.Diagnostics.Stopwatch.StartNew();
            //engine.Run(script);
            //throw new AssertInconclusiveException(string.Format("{0:n1}ms", timer.Elapsed.TotalMilliseconds));
        }

        /// <summary>
        /// Runs all the script files in the script directory (specified in the constructor).
        /// </summary>
        /// <param name="previousTime"> The previous time required to run all the tests. </param>
        public void RunAllTests(double previousTime)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            foreach (string path in Directory.EnumerateFiles(this.currentDir))
                RunTest(path, 0, false);
            Assert.Inconclusive(string.Format("{0}ms, was {1}ms", timer.ElapsedMilliseconds, previousTime));
        }
    }

}