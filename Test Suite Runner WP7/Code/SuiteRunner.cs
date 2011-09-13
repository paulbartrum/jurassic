using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jurassic;
using Jurassic.Library;
using System.Threading;

namespace Test_Suite_Runner_WP7
{
    public class SuiteRunner
    {
        private List<TestSuite> suites = new List<TestSuite>();
        private TimeSpan totalTime;

        /// <summary>
        /// Gets the total number of tests in the test suite.  EnumerateTests() must be called
        /// before this property returns the correct result.
        /// </summary>
        public int TotalTestCount
        {
            get { return this.suites.Sum(suite => suite.TotalTestCount); }
        }

        /// <summary>
        /// Gets the overall number of successes.
        /// </summary>
        public int OverallSuccessCount
        {
            get { return this.Suites.Sum(suite => suite.SuccessfulTestCount); }
        }

        /// <summary>
        /// Gets the overall number of skipped tests.
        /// </summary>
        public int OverallSkipCount
        {
            get { return this.Suites.Sum(suite => suite.SkippedTestCount); }
        }

        /// <summary>
        /// Gets the overall number of failures.
        /// </summary>
        public int OverallFailureCount
        {
            get { return this.Suites.Sum(suite => suite.FailedTestCount); }
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
        /// Starts a new test run.
        /// </summary>
        /// <param name="testsToRun"> The tests to run.  <c>null</c> to run all tests. </param>
        public void Run(IEnumerable<Test> testsToRun)
        {
            var thread = new Thread(new ThreadStart(() => RunCore(testsToRun)));
            thread.Start();
        }

        /// <summary>
        /// Starts a new test run.
        /// </summary>
        /// <param name="testsToRun"> The tests to run.  <c>null</c> to run all tests. </param>
        private void RunCore(IEnumerable<Test> testsToRun)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            foreach (TestSuite suite in this.Suites)
            {
                // Set the current test suite.
                this.CurrentTestSuite = suite;

                // Raise the TestSuiteStarted event.
                if (this.TestSuiteStarted != null)
                    this.TestSuiteStarted(this, EventArgs.Empty);

                // Create a new script engine.
                var scriptEngine = new ScriptEngine();
                scriptEngine.CompatibilityMode = suite.CompatibilityMode;

                // Determine the tests to run.
                List<Test> suiteTestsToRun;
                if (testsToRun == null)
                    suiteTestsToRun = new List<Test>(suite.Tests);
                else
                    suiteTestsToRun = new List<Test>(testsToRun.Where(test => test.Suite == suite));

                foreach (var test in suiteTestsToRun)
                {
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
                        // Load the script file.
                        string scriptContents = suite.ReadScriptContents(test);

                        // Execute the test script.
                        Exception scriptException = null;
                        try
                        {
                            scriptEngine.Execute(new StringScriptSource(scriptContents, test.Path));
                        }
                        catch (Exception ex)
                        {
                            scriptException = ex;
                        }

                        if ((test.IsNegativeTest == false && scriptException == null) || (test.IsNegativeTest == true && scriptException != null))
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
                            test.FailureException = scriptException ?? new InvalidOperationException("The test was expected to fail, but it didn't");

                            // Increment the failed count.
                            suite.IncrementFailedTestCount();

                            // Raise the failed test event.
                            if (this.TestFailed != null)
                                this.TestFailed(this, new TestEventArgs(test));

                        }
                    }
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
