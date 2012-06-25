using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.TestSuite
{

    /// <summary>
    /// Represents a test suite.
    /// </summary>
    public class TestSuite : IDisposable
    {
        private Stream zipStream;
        private ZipFile zipFile;
        private List<string> skippedTestNames = new List<string>();
        private string includes;
        private static Dictionary<string, object> includeProperties;
        private int successfulTestCount;
        private int failedTestCount;
        private int skippedTestCount;

        /// <summary>
        /// Creates a new TestSuite instance.
        /// </summary>
        /// <param name="storagePath"> The path of the directory that contains the config, harness and suite directories. </param>
        public TestSuite(string storagePath)
            : this(
                path => new FileStream(Path.Combine(storagePath, path), FileMode.Open, FileAccess.Read)
            )
        {
        }

        /// <summary>
        /// Creates a new TestSuite instance.
        /// </summary>
        /// <param name="openFile"> A callback that can open a file for reading. </param>
        public TestSuite(Func<string, Stream> openFile)
        {
            if (openFile == null)
                throw new ArgumentNullException("openFile");

            // Init collection.
            this.IncludedTests = new List<string>();

            // Open the excludelist.xml file to generate a list of skipped file names.
            var reader = System.Xml.XmlReader.Create(openFile(@"config\excludeList.xml"));
            reader.ReadStartElement("excludeList");
            do
            {
                if (reader.Depth == 1 && reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name == "test")
                    this.skippedTestNames.Add(reader.GetAttribute("id"));
            } while (reader.Read());

            // Read the include files.
            var includeBuilder = new System.Text.StringBuilder();
            includeBuilder.AppendLine(ReadInclude(openFile, "cth.js"));
            includeBuilder.AppendLine(ReadInclude(openFile, "sta.js"));
            includeBuilder.AppendLine(ReadInclude(openFile, "ed.js"));
            this.includes = includeBuilder.ToString();

            this.zipStream = openFile(@"suite\2012-05-18.zip");
            this.zipFile = new ZipFile(this.zipStream);
            this.ApproximateTotalTestCount = (int)this.zipFile.Count;
        }

        /// <summary>
        /// Reads an include file from the harness directory.
        /// </summary>
        /// <param name="openFile"> The callback used to open the file. </param>
        /// <param name="name"> The name of the include file. </param>
        /// <returns> The contents of the file. </returns>
        private static string ReadInclude(Func<string, Stream> openFile, string name)
        {
            using (var stream = openFile(Path.Combine("harness", name)))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Disposes of any resources used by this class.
        /// </summary>
        public void Dispose()
        {
            this.zipFile.Close();
            this.zipStream.Dispose();
        }

#if !SILVERLIGHT

        /// <summary>
        /// A flag that indicates whether to run tests inside a partial trust appdomain.
        /// </summary>
        public bool RunInSandbox { get; set; }

#endif

        /// <summary>
        /// Gets a list of test names to include in the test run.
        /// </summary>
        public IList<string> IncludedTests { get; private set; }

        /// <summary>
        /// Gets the total number of tests in the test suite.
        /// </summary>
        public int ApproximateTotalTestCount { get; private set; }

        /// <summary>
        /// Gets the number of tests that have been executed.
        /// </summary>
        public int ExecutedTestCount
        {
            get { return this.SuccessfulTestCount + this.FailedTestCount + this.SkippedTestCount; }
        }

        /// <summary>
        /// Gets the number of tests that have been executed successfully (in the current run).
        /// </summary>
        public int SuccessfulTestCount
        {
            get { return this.successfulTestCount; }
        }

        /// <summary>
        /// Gets the number of that have been executed successfully (in the current run), as a
        /// fraction (will be between 0 and 1).
        /// </summary>
        public double SuccessfulPercentage
        {
            get { return this.ExecutedTestCount == 0 ? 0.0 : (double)this.SuccessfulTestCount / (double)this.ExecutedTestCount; }
        }

        /// <summary>
        /// Gets the number of tests that failed (in the current run).
        /// </summary>
        public int FailedTestCount
        {
            get { return this.failedTestCount; }
        }

        /// <summary>
        /// Gets the number of that failed (in the current run), as a fraction (will be between 0
        /// and 1).
        /// </summary>
        public double FailedPercentage
        {
            get { return this.ExecutedTestCount == 0 ? 0.0 : (double)this.FailedTestCount / (double)this.ExecutedTestCount; }
        }

        /// <summary>
        /// Gets the number of tests that were not executed (in the current run).  This might be
        /// because the test is known to be buggy or there are no plans to fix the bug the test
        /// reveals.
        /// </summary>
        public int SkippedTestCount
        {
            get { return this.skippedTestCount; }
        }

        /// <summary>
        /// Gets the number of that were not executed (in the current run), as a fraction (will be
        /// between 0 and 1).
        /// </summary>
        public double SkippedPercentage
        {
            get { return this.ExecutedTestCount == 0 ? 0.0 : (double)this.SkippedTestCount / (double)this.ExecutedTestCount; }
        }

        /// <summary>
        /// Represents information needed to run a test.
        /// </summary>
        private class TestExecutionState
        {
            /// <summary>
            /// Initializes a TestExecutionState instance.
            /// </summary>
            /// <param name="test"> The test to run. </param>
            /// <param name="runInStrictMode"> Whether or not to run in strict mode. </param>
            public TestExecutionState(Test test, bool runInStrictMode)
            {
                this.Test = test;
                this.RunInStrictMode = runInStrictMode;
            }

            /// <summary>
            /// The test to run.
            /// </summary>
            public Test Test { get; private set; }

            /// <summary>
            /// Whether or not to run in strict mode.
            /// </summary>
            public bool RunInStrictMode { get; private set; }
        }

        /// <summary>
        /// Starts executing the test suite.
        /// </summary>
        public void Start()
        {
            // Create a ScriptEngine and freeze its state.
            SaveScriptEngineSnapshot();

            // Create a queue to hold the tests.
            var queue = new BlockingQueue<TestExecutionState>(100);

            // Create a thread per processor.
            var threads = new List<Thread>();
            for (int i = 0; i < GetThreadCount(); i++)
            {
                var thread = new Thread(ThreadStart);
                thread.Start(queue);
                threads.Add(thread);
            }

            for (int i = 0; i < this.zipFile.Count; i++)
            {
                var zipEntry = this.zipFile[i];
                if (zipEntry.IsFile && zipEntry.Name.EndsWith(".js"))
                {
                    // This is a test file.

                    // Read out the contents (assume UTF-8).
                    string fileContents;
                    using (var entryStream = this.zipFile.GetInputStream(zipEntry))
                    using (var reader = new StreamReader(entryStream))
                    {
                        fileContents = reader.ReadToEnd();
                    }

                    // Parse out the test metadata.
                    var test = new Test(this, zipEntry.Name, fileContents);

                    // Check if the test should be skipped.
                    if (this.skippedTestNames.Contains(Path.GetFileNameWithoutExtension(zipEntry.Name)))
                    {
                        this.skippedTestCount++;
                        TestFinished(this, new TestEventArgs(TestRunStatus.Skipped, test, false));
                        continue;
                    }
                    if (this.IncludedTests.Count > 0 && this.IncludedTests.Contains(Path.GetFileNameWithoutExtension(zipEntry.Name)) == false)
                    {
                        this.skippedTestCount++;
                        TestFinished(this, new TestEventArgs(TestRunStatus.Skipped, test, false));
                        continue;
                    }
                    

                    // Queue the test.
                    if (test.RunInNonStrictMode)
                        queue.Enqueue(new TestExecutionState(test, runInStrictMode: false));
                    if (test.RunInStrictMode)
                        queue.Enqueue(new TestExecutionState(test, runInStrictMode: true));
                }
            }

            // Signal the threads that no more tests will be provided.
            queue.Close();

            // Wait for all threads to exit.
            foreach (var thread in threads)
                thread.Join();
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Gets the number of threads to start processing tests on.
        /// </summary>
        /// <returns></returns>
        private int GetThreadCount()
        {
            return 1;
        }
#else
        /// <summary>
        /// Gets the number of threads to start processing tests on.
        /// </summary>
        /// <returns></returns>
        private int GetThreadCount()
        {
            return Environment.ProcessorCount;
        }
#endif

        /// <summary>
        /// Creates a script engine, runs the includes, then saves any new state.
        /// </summary>
        private void SaveScriptEngineSnapshot()
        {
            // Create a new script engine.
            var engine = new ScriptEngine();

            // Record all the global properties.
            var standardGlobals = new HashSet<string>();
            foreach (var property in engine.Global.Properties)
                standardGlobals.Add(property.Name);

            // Execute the includes file.
            engine.Execute(this.includes);

            // Record all new properties.
            var additionalProperties = new Dictionary<string, object>();
            foreach (var property in engine.Global.Properties)
                if (standardGlobals.Contains(property.Name) == false)
                    additionalProperties.Add(property.Name, property.Value);
            includeProperties = additionalProperties;
        }

#if !SILVERLIGHT

        /// <summary>
        /// A marshal-by-reference version of the ScriptEngine object.
        /// </summary>
        private class ScriptEngineWrapper : MarshalByRefObject
        {
            public ScriptEngineWrapper(ScriptEngine engine)
            {
                if (engine == null)
                    throw new ArgumentNullException("engine");
                this.Engine = engine;
            }

            /// <summary>
            /// Gets the underlying script engine.
            /// </summary>
            public ScriptEngine Engine
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets or sets a value that indicates whether debugging is enabled.
            /// </summary>
            public bool EnableDebugging
            {
                get { return this.Engine.EnableDebugging; }
                set { this.Engine.EnableDebugging = value; }
            }

            /// <summary>
            /// Executes the script file with the given path.
            /// </summary>
            /// <param name="scriptContents"> The contents of the script file to execute. </param>
            /// <returns> The exception that was thrown by the Execute method, or <c>null</c> if no
            /// exception was thrown. </returns>
            public Exception Execute(string scriptContents)
            {
                try
                {
                    this.Engine.Execute(scriptContents);
                    return null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }

#endif

        /// <summary>
        /// Called to start a thread that runs tests.
        /// </summary>
        /// <param name="state"> The queue to retrieve tests from. </param>
        private void ThreadStart(object state)
        {
            var queue = (BlockingQueue<TestExecutionState>)state;
#if !SILVERLIGHT
            if (this.RunInSandbox)
                RunTestsInSandbox(queue);
            else
                RunTests(queue);
#else
            RunTests(queue);
#endif
        }

        /// <summary>
        /// Pulls tests from a queue and runs them.
        /// </summary>
        /// <param name="queue"> The queue to retrieve tests from. </param>
        private void RunTests(BlockingQueue<TestExecutionState> queue)
        {
            // Loop as long as there are tests in the queue.
            while (true)
            {
                // Retrieve a test from the queue.
                TestExecutionState executionState;
                if (queue.TryDequeue(out executionState) == false)
                    break;

                // Restore the ScriptEngine state.
                var scriptEngine = new ScriptEngine();
                foreach (var propertyNameAndValue in includeProperties)
                    scriptEngine.Global[propertyNameAndValue.Key] = propertyNameAndValue.Value;

                // Set strict mode.
                scriptEngine.ForceStrictMode = executionState.RunInStrictMode;

                // Run the test.
                RunTest(script =>
                {
                    try
                    {
                        scriptEngine.Execute(script);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                }, executionState);

            }
        }

#if !SILVERLIGHT

        /// <summary>
        /// Pulls tests from a queue and runs them inside a sandbox.
        /// </summary>
        /// <param name="queue"> The queue to retrieve tests from. </param>
        private void RunTestsInSandbox(BlockingQueue<TestExecutionState> queue)
        {
            // Set the DeserializationEnvironment so any JavaScriptExceptions can be serialized
            // accross the AppDomain boundary.
            ScriptEngine.DeserializationEnvironment = new ScriptEngine();

            int testCounter = 0;
            AppDomain appDomain = null;

            // Loop as long as there are tests in the queue.
            while (true)
            {
                if (testCounter == 0)
                {
                    // Unload the old AppDomain.
                    if (appDomain != null)
                        AppDomain.Unload(appDomain);

                    // Create an AppDomain with internet sandbox permissions.
                    var e = new System.Security.Policy.Evidence();
                    e.AddHostEvidence(new System.Security.Policy.Zone(System.Security.SecurityZone.Internet));
                    appDomain = AppDomain.CreateDomain(
                        "Jurassic sandbox",
                        null,
                        new AppDomainSetup() { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory },
                        System.Security.SecurityManager.GetStandardSandbox(e));
                }

                // Retrieve a test from the queue.
                TestExecutionState executionState;
                if (queue.TryDequeue(out executionState) == false)
                    break;

                // Restore the ScriptEngine state.
                var scriptEngine = new ScriptEngine();
                foreach (var propertyNameAndValue in includeProperties)
                    scriptEngine.Global[propertyNameAndValue.Key] = propertyNameAndValue.Value;

                // Set strict mode.
                scriptEngine.ForceStrictMode = executionState.RunInStrictMode;

                // Create a new ScriptEngine instance inside the AppDomain.
                var engineHandle = Activator.CreateInstanceFrom(
                    appDomain,                                          // The AppDomain to create the type within.
                    typeof(ScriptEngineWrapper).Assembly.CodeBase,      // The file name of the assembly containing the type.
                    typeof(ScriptEngineWrapper).FullName,               // The name of the type to create.
                    false,                                              // Ignore case: no.
                    (System.Reflection.BindingFlags)0,                  // Binding flags.
                    null,                                               // Binder.
                    new object[] { scriptEngine },                      // Parameters passed to the constructor.
                    null,                                               // Culture.
                    null);                                              // Activation attributes.
                var scriptEngineProxy = (ScriptEngineWrapper)engineHandle.Unwrap();
                if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy(scriptEngineProxy) == false)
                    throw new InvalidOperationException("Script engine not operating within the sandbox.");

                // Run the test.
                RunTest(scriptEngineProxy.Execute, executionState);

                // Reset the test count every 200 tests so the AppDomain gets recreated.
                // This saves us from an unavoidable memory leak in low privilege mode.
                testCounter++;
                if (testCounter == 200)
                    testCounter = 0;

            }
        }

#endif

        /// <summary>
        /// Runs a single test.
        /// </summary>
        /// <param name="executeCallback"> A callback which can execute a script and
        /// return <c>null</c> if it succeeded or an exception otherwise. </param>
        /// <param name="executionState"> Information about how the test is executed. </param>
        private void RunTest(Func<string, Exception> executeCallback, TestExecutionState executionState)
        {
            // Run the test script.
            Exception ex = executeCallback(executionState.Test.Script);

            if (ex == null)
            {
                if (executionState.Test.IsNegativeTest)
                {
                    // The test succeeded but was expected to fail.
                    Interlocked.Increment(ref this.failedTestCount);
                    TestFinished(this, new TestEventArgs(TestRunStatus.Failed, executionState.Test, executionState.RunInStrictMode,
                        new InvalidOperationException("Expected failure but the test succeeded.")));
                }
                else
                {
                    // The test succeeded and was expected to succeed.
                    Interlocked.Increment(ref this.successfulTestCount);
                    TestFinished(this, new TestEventArgs(TestRunStatus.Success, executionState.Test, executionState.RunInStrictMode));
                }
            }
            else if (ex is JavaScriptException)
            {

                if (executionState.Test.IsNegativeTest)
                {
                    // The test was expected to fail.
                    if (executionState.Test.NegativeErrorPattern == null)
                    {
                        // The test succeeded.
                        Interlocked.Increment(ref this.successfulTestCount);
                        TestFinished(this, new TestEventArgs(TestRunStatus.Success, executionState.Test, executionState.RunInStrictMode));
                    }
                    else
                    {
                        // Check if the exception had the name we expected.
                        var errorObject = ((JavaScriptException)ex).ErrorObject;
                        if (errorObject is ObjectInstance &&
                            System.Text.RegularExpressions.Regex.IsMatch(TypeConverter.ToString(((ObjectInstance)errorObject)["name"]), executionState.Test.NegativeErrorPattern))
                        {
                            // The test succeeded.
                            Interlocked.Increment(ref this.successfulTestCount);
                            TestFinished(this, new TestEventArgs(TestRunStatus.Success, executionState.Test, executionState.RunInStrictMode));
                        }
                        else
                        {
                            // The type of error was wrong.
                            Interlocked.Increment(ref this.failedTestCount);
                            TestFinished(this, new TestEventArgs(TestRunStatus.Failed, executionState.Test, executionState.RunInStrictMode, ex));
                        }
                    }
                }
                else
                {
                    // The test shouldn't have thrown an exception.
                    Interlocked.Increment(ref this.failedTestCount);
                    TestFinished(this, new TestEventArgs(TestRunStatus.Failed, executionState.Test, executionState.RunInStrictMode, ex));
                }
            }
            else
            {
                // .NET exceptions are always bad.
                Interlocked.Increment(ref this.failedTestCount);
                TestFinished(this, new TestEventArgs(TestRunStatus.Failed, executionState.Test, executionState.RunInStrictMode, ex));
            }
        }

        /// <summary>
        /// Event that is triggered when a test finishes executing.  May execute on any arbitrary thread.
        /// </summary>
        public event EventHandler<TestEventArgs> TestFinished;
    }
}
