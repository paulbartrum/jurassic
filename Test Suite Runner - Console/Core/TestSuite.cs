using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using Jurassic;
using Jurassic.Library;

namespace Test_Suite_Runner_WP7
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
                path => Directory.GetFiles(Path.Combine(storagePath, path)),
                path => new FileStream(Path.Combine(storagePath, path), FileMode.Open, FileAccess.Read)
            )
        {
        }

        /// <summary>
        /// Creates a new TestSuite instance.
        /// </summary>
        public TestSuite(Func<string, IEnumerable<string>> enumerateFiles, Func<string, FileStream> openFile)
        {
            if (enumerateFiles == null)
                throw new ArgumentNullException("enumerateFiles");
            if (openFile == null)
                throw new ArgumentNullException("openFile");

            // Open the excludelist.xml file to generate a list of skipped file names.
            var doc = new System.Xml.XmlDocument();
            doc.Load(openFile(@"config\excludeList.xml"));
            foreach (System.Xml.XmlElement element in doc.SelectNodes("/excludeList/test"))
                this.skippedTestNames.Add(element.GetAttribute("id"));

            // Read the include files.
            var includeBuilder = new System.Text.StringBuilder();
            includeBuilder.AppendLine(ReadInclude(openFile, "cth.js"));
            includeBuilder.AppendLine(ReadInclude(openFile, "sta.js"));
            includeBuilder.AppendLine(ReadInclude(openFile, "ed.js"));
            this.includes = includeBuilder.ToString();

            this.zipStream = openFile(@"suite\2011-11-11.zip");
            this.zipFile = new ZipFile(this.zipStream);
            this.ApproximateTotalTestCount = (int)this.zipFile.Count;
        }

        /// <summary>
        /// Reads an include file from the harness directory.
        /// </summary>
        /// <param name="openFile"> The callback used to open the file. </param>
        /// <param name="name"> The name of the include file. </param>
        /// <returns> The contents of the file. </returns>
        private static string ReadInclude(Func<string, FileStream> openFile, string name)
        {
            using (var fileStream = openFile(Path.Combine("harness", name)))
            using (var reader = new StreamReader(fileStream))
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
            for (int i = 0; i < Environment.ProcessorCount; i++)
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

        /// <summary>
        /// Called to start a thread that runs tests.
        /// </summary>
        /// <param name="state">  </param>
        private void ThreadStart(object state)
        {
            var queue = (BlockingQueue<TestExecutionState>)state;

            // Loop as long as there are tests in the queue.
            while (true)
            {

                // Retrieve a test from the queue.
                TestExecutionState executionState;
                if (queue.TryDequeue(out executionState) == false)
                    break;

                // Run the test.
                RunTest(executionState.Test, strictMode: executionState.RunInStrictMode);

            }
        }

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

        /// <summary>
        /// Runs a single test.
        /// </summary>
        /// <param name="test"> The test to run. </param>
        /// <param name="strictMode"> <c>true</c> to run in strict mode; <c>false</c> otherwise. </param>
        private void RunTest(Test test, bool strictMode)
        {
            // Restore the ScriptEngine state.
            var engine = new ScriptEngine();
            foreach (var propertyNameAndValue in includeProperties)
                engine.Global[propertyNameAndValue.Key] = propertyNameAndValue.Value;

            // Set strict mode.
            engine.ForceStrictMode = strictMode;

            try
            {
                // Run the test script.
                engine.Execute(test.Script);

                if (test.IsNegativeTest)
                {
                    // The test succeeded but was expected to fail.
                    this.failedTestCount++;
                    TestFinished(this, new TestEventArgs(TestRunStatus.Failed, test, engine.ForceStrictMode,
                        new InvalidOperationException("Expected failure but the test succeeded.")));
                }
                else
                {
                    // The test succeeded and was expected to succeed.
                    this.successfulTestCount++;
                    TestFinished(this, new TestEventArgs(TestRunStatus.Success, test, engine.ForceStrictMode));
                }
            }
            catch (JavaScriptException ex)
            {
                if (test.IsNegativeTest)
                {
                    // The test was expected to fail.
                    if (test.NegativeErrorPattern == null)
                    {
                        // The test succeeded.
                        this.successfulTestCount ++;
                        TestFinished(this, new TestEventArgs(TestRunStatus.Success, test, engine.ForceStrictMode));
                    }
                    else
                    {
                        // Check if the exception had the name we expected.
                        if (ex.ErrorObject is ObjectInstance &&
                            System.Text.RegularExpressions.Regex.IsMatch(TypeConverter.ToString(((ObjectInstance)ex.ErrorObject)["name"]), test.NegativeErrorPattern))
                        {
                            // The test succeeded.
                            this.successfulTestCount ++;
                            TestFinished(this, new TestEventArgs(TestRunStatus.Success, test, engine.ForceStrictMode));
                        }
                        else
                        {
                            // The type of error was wrong.
                            this.failedTestCount++;
                            TestFinished(this, new TestEventArgs(TestRunStatus.Failed, test, engine.ForceStrictMode, ex));
                        }
                    }
                }
                else
                {
                    // The test shouldn't have thrown an exception.
                    this.failedTestCount ++;
                    TestFinished(this, new TestEventArgs(TestRunStatus.Failed, test, engine.ForceStrictMode, ex));
                }
            }
            catch (Exception ex)
            {
                // .NET exceptions are always bad.
                this.failedTestCount ++;
                TestFinished(this, new TestEventArgs(TestRunStatus.Failed, test, engine.ForceStrictMode, ex));
            }
        }

        /// <summary>
        /// Event that is triggered when a test finishes executing.  May execute on any arbitrary thread.
        /// </summary>
        public event EventHandler<TestEventArgs> TestFinished;
    }
}
