using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jurassic;
using Jurassic.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Suite_Runner
{
    public class SuiteRunner
    {
        private List<TestSuite> suites = new List<TestSuite>();
        private TimeSpan totalTime;

        /// <summary>
        /// Gets or sets a value that indicates whether to run in partial trust.
        /// </summary>
        public bool RunInPartialTrust
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the total number of tests in the test suite.  EnumerateTests() must be called
        /// before this property returns the correct result.
        /// </summary>
        public int TotalTestCount
        {
            get { return this.suites.Sum(suite => suite.TotalTestCount); }
        }

        /// <summary>
        /// Gets the overall number of failures (including skipped tests).
        /// </summary>
        public int OverallFailureCount
        {
            get { return this.Suites.Sum(suite => suite.FailedTestCount + suite.SkippedTestCount); }
        }

        /// <summary>
        /// Gets the percentage of failures.
        /// </summary>
        public double OverallFailurePercentage
        {
            get { return (double)this.OverallFailureCount / (double)this.TotalTestCount; }
        }

        /// <summary>
        /// Gets the total time taken.
        /// </summary>
        public TimeSpan TotalTime
        {
            get { return this.totalTime; }
        }

        /// <summary>
        /// Gets a list of all the suites to run.
        /// </summary>
        public IList<TestSuite> Suites
        {
            get { return this.suites; }
        }

        /// <summary>
        /// Gets the currently running test suite.
        /// </summary>
        public TestSuite CurrentTestSuite
        {
            get;
            private set;
        }

        /// <summary>
        /// Loads the list of tests.  The TotalTestCount property is populated after calling this
        /// method.
        /// </summary>
        public void EnumerateTests()
        {
            foreach (TestSuite suite in this.Suites)
                suite.EnumerateTests();
        }

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
            /// <param name="filePath"> The path of the script file to execute. </param>
            /// <returns> The exception that was thrown by the Execute method, or <c>null</c> if no
            /// exception was thrown. </returns>
            public Exception Execute(string scriptContents, string filePath)
            {
                try
                {
                    this.Engine.Execute(new StringScriptSource(scriptContents, filePath));
                    return null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }

        /// <summary>
        /// Starts a new test run.
        /// </summary>
        /// <param name="testsToRun"> The tests to run.  <c>null</c> to run all tests. </param>
        public void Run(IEnumerable<Test> testsToRun)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            foreach (TestSuite suite in this.Suites)
            {
                // Set the current test suite.
                this.CurrentTestSuite = suite;

                // Raise the TestSuiteStarted event.
                if (this.TestSuiteStarted != null)
                    this.TestSuiteStarted(this, EventArgs.Empty);

                // Determine the tests to run.
                List<Test> suiteTestsToRun;
                if (testsToRun == null)
                    suiteTestsToRun = new List<Test>(suite.Tests);
                else
                    suiteTestsToRun = new List<Test>(testsToRun.Where(test => test.Suite == suite));

                // The AppDomain is recreated every X tests.  Only needed in partial trust because
                // partial trust has a memory leak.
                AppDomain appDomain = null;
                int recreateAppDomainAfterTestCount = this.RunInPartialTrust ? 200 : int.MaxValue;
                int startTestIndex = 0;
                while (startTestIndex < suite.TotalTestCount)
                {
                    // Unload the old AppDomain.
                    if (appDomain != null)
                        AppDomain.Unload(appDomain);

                    // Create an AppDomain with internet sandbox permissions.
                    var e = new System.Security.Policy.Evidence();
                    e.AddHostEvidence(new System.Security.Policy.Zone(System.Security.SecurityZone.Internet));
                    appDomain = AppDomain.CreateDomain(
                        "Jurassic script domain",
                        null,
                        new AppDomainSetup() { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory },
                        System.Security.SecurityManager.GetStandardSandbox(e));

                    // Enumerate through the tests.
                    System.Threading.Tasks.Parallel.For(
                        startTestIndex,
                        Math.Min(startTestIndex + recreateAppDomainAfterTestCount, suite.TotalTestCount),
                        () =>
                        {
                            // Set the DeserializationEnvironment so any JavaScriptExceptions can be serialized
                            // accross the AppDomain boundary.
                            ScriptEngine.DeserializationEnvironment = new ScriptEngine();
                            return 0;
                        },
                        (index, loopState, localState) =>
                        {
                            // Get the test to run.
                            var test = suiteTestsToRun[index];

                            // Check if the test should be skipped.
                            if (suite.SkipTest(test) == true)
                            {
                                // Record the fact that the test was skipped.
                                suite.IncrementSkippedTestCount();

                                // Raise the TestSkipped event.
                                if (this.TestSkipped != null)
                                    this.TestSkipped(this, new TestEventArgs(test));
                            }
                            else
                            {

                                ScriptEngineWrapper scriptEngineProxy;
                                if (this.RunInPartialTrust == false)
                                {
                                    // Create a new script engine.
                                    var scriptEngine = new ScriptEngine();
                                    scriptEngine.CompatibilityMode = suite.CompatibilityMode;

                                    // Create a wrapper object.
                                    scriptEngineProxy = new ScriptEngineWrapper(scriptEngine);
                                }
                                else
                                {
                                    // Create a new script engine.
                                    var scriptEngine = new ScriptEngine();
                                    scriptEngine.CompatibilityMode = suite.CompatibilityMode;

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
                                    scriptEngineProxy = (ScriptEngineWrapper)engineHandle.Unwrap();
                                    if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy(scriptEngineProxy) == false)
                                        throw new InvalidOperationException("Script engine not operating within the sandbox.");

                                }

                                // Load the script file.
                                string scriptContents = File.ReadAllText(test.Path, suite.ScriptEncoding);

                                // Transform the script file.
                                scriptContents = suite.TransformTestScript(test, scriptContents);

                                // Save the script file to a temporary file.
                                string debugScriptPath = null;
                                if (scriptEngineProxy.EnableDebugging == true)
                                {
                                    debugScriptPath = Path.GetTempFileName();
                                    File.WriteAllText(debugScriptPath, scriptContents);
                                }

                                // Execute the test script in the AppDomain.
                                Exception ex = scriptEngineProxy.Execute(scriptContents, debugScriptPath);

                                if ((test.IsNegativeTest == false && ex == null) || (test.IsNegativeTest == true && ex != null))
                                {

                                    // Increment the succeeded count.
                                    suite.IncrementSuccessfulTestCount();

                                    // Raise the succeeded test event.
                                    if (this.TestSucceeded != null)
                                        this.TestSucceeded(this, new TestEventArgs(test));

                                }
                                else
                                {

                                    // Save the exception for later inspection.
                                    test.FailureException = ex ?? new InvalidOperationException("The test was expected to fail, but it didn't");

                                    // Increment the failed count.
                                    suite.IncrementFailedTestCount();

                                    // Raise the failed test event.
                                    if (this.TestFailed != null)
                                        this.TestFailed(this, new TestEventArgs(test));

                                }

                                // Delete the temporary file.
                                if (debugScriptPath != null)
                                    File.Delete(debugScriptPath);
                            }
                            return 0;
                        },
                        (localState) => { });

                    // Synchronize every X tests.
                    startTestIndex += recreateAppDomainAfterTestCount;
                }
            }

            // Record the total time it took to run the test suites.
            this.totalTime = stopWatch.Elapsed;
        }

        /// <summary>
        /// Occurs when a test suite starts running.
        /// </summary>
        public event EventHandler TestSuiteStarted;

        /// <summary>
        /// Occurs when a test is run successfully.
        /// </summary>
        public event EventHandler<TestEventArgs> TestSucceeded;

        /// <summary>
        /// Occurs when a test fails.
        /// </summary>
        public event EventHandler<TestEventArgs> TestFailed;

        /// <summary>
        /// Occurs when a test is skipped.
        /// </summary>
        public event EventHandler<TestEventArgs> TestSkipped;
    }

    /// <summary>
    /// Used to convey the test details to the event consumer.
    /// </summary>
    public class TestEventArgs : EventArgs
    {
        public TestEventArgs(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            this.Test = test;
        }

        /// <summary>
        /// Gets the test that is related to the event.
        /// </summary>
        public Test Test
        {
            get;
            private set;
        }
    }
}
