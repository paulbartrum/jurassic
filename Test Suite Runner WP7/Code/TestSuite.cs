using System;
using System.Collections.Generic;
using System.IO;
using Jurassic;
using Jurassic.Library;

namespace Test_Suite_Runner_WP7
{

    /// <summary>
    /// Represents the base class of all test suites.
    /// </summary>
    public abstract class TestSuite
    {
        private string zipFileName;
        private List<string> allTests = new List<string>();
        private int successfulTestCount;
        private int failedTestCount;
        private int skippedTestCount;

        /// <summary>
        /// Creates a new TestSuite instance.
        /// </summary>
        /// <param name="zipFileName"> The name of the zip file containing the test files. </param>
        public TestSuite(string zipFileName)
        {
            if (zipFileName == null)
                throw new ArgumentNullException("zipFileName");
            this.zipFileName = zipFileName;
        }

        /// <summary>
        /// Gets the name of the test suite.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets the compatibility mode the tests were designed for.
        /// </summary>
        public virtual CompatibilityMode CompatibilityMode
        {
            get { return CompatibilityMode.Latest; }
        }

        /// <summary>
        /// Gets the name of the zip file containing the test scripts.
        /// </summary>
        public string ZipFileName
        {
            get { return this.zipFileName; }
        }

        /// <summary>
        /// Gets an enumerable list of all the tests in the suite.
        /// </summary>
        public IEnumerable<Test> Tests
        {
            get
            {
                var streamInfo = System.Windows.Application.GetResourceStream(new Uri("Suites/" + this.zipFileName + ".zip", UriKind.Relative));
                using (var unzip = new SharpGIS.UnZipper(streamInfo.Stream))
                {
                    foreach (string path in unzip.FileNamesInZip)
                        yield return new Test(this, path);
                }
            }
        }

        /// <summary>
        /// Gets the total number of tests in the test suite.  EnumerateTests() must be called
        /// before this property returns the correct result.
        /// </summary>
        public int TotalTestCount
        {
            get { return this.allTests.Count; }
        }

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
        /// percentage.
        /// </summary>
        public double SuccessfulPercentage
        {
            get { return (double)this.SuccessfulTestCount / (double)this.TotalTestCount; }
        }

        /// <summary>
        /// Increments the number of successful tests by one.
        /// </summary>
        public void IncrementSuccessfulTestCount()
        {
            System.Threading.Interlocked.Increment(ref this.successfulTestCount);
        }

        /// <summary>
        /// Gets the number of tests that failed (in the current run).
        /// </summary>
        public int FailedTestCount
        {
            get { return this.failedTestCount; }
        }

        /// <summary>
        /// Gets the number of that failed (in the current run), as a percentage.
        /// </summary>
        public double FailedPercentage
        {
            get { return (double)this.FailedTestCount / (double)this.TotalTestCount; }
        }

        /// <summary>
        /// Increments the number of successful tests by one.
        /// </summary>
        public void IncrementFailedTestCount()
        {
            System.Threading.Interlocked.Increment(ref this.failedTestCount);
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
        /// Increments the number of skipped tests by one.
        /// </summary>
        public void IncrementSkippedTestCount()
        {
            System.Threading.Interlocked.Increment(ref this.skippedTestCount);
        }

        /// <summary>
        /// Gets the number of that were not executed (in the current run), as a percentage.
        /// </summary>
        public double SkippedPercentage
        {
            get { return (double)this.SkippedTestCount / (double)this.TotalTestCount; }
        }

        /// <summary>
        /// Gets the number of tests that failed or were skipped.
        /// </summary>
        public int OverallFailedTestCount
        {
            get { return this.FailedTestCount + this.SkippedTestCount; }
        }

        /// <summary>
        /// Gets the number of tests that failed or were skipped, as a percentage.
        /// </summary>
        public double OverallFailedPercentage
        {
            get { return (double)this.OverallFailedTestCount / (double)this.TotalTestCount; }
        }

        /// <summary>
        /// Loads the list of tests.  The TotalTestCount property is populated after calling this
        /// method.
        /// </summary>
        public void EnumerateTests()
        {
            this.allTests.Clear();
            EnumerateScripts(this.allTests, this.zipFileName);
        }

        /// <summary>
        /// Enumerates all the javascript files in a directory.
        /// </summary>
        /// <param name="allPaths"> The list to all test file paths to. </param>
        /// <param name="dir"> The directory to enumerate. </param>
        private void EnumerateScripts(List<string> allPaths, string dir)
        {
            var streamInfo = System.Windows.Application.GetResourceStream(new Uri("Suites/" + this.zipFileName + ".zip", UriKind.Relative));
            using (var unzip = new SharpGIS.UnZipper(streamInfo.Stream))
            {
                foreach (string path in unzip.FileNamesInZip)
                    allPaths.Add(path);
            }
        }

        /// <summary>
        /// Gets the text encoding of the script files in the test suite.
        /// </summary>
        public abstract System.Text.Encoding ScriptEncoding
        {
            get;
        }

        /// <summary>
        /// Determines whether to skip a test.
        /// </summary>
        /// <param name="test"> The test to check. </param>
        /// <returns> <c>true</c> if the test should be skipped; <c>false</c> otherwise. </returns>
        public virtual bool SkipTest(Test test)
        {
            return false;
        }

        /// <summary>
        /// Reads the contents of a script.
        /// </summary>
        /// <param name="test"> The test to read. </param>
        /// <returns> The test script source, as a string. </returns>
        public string ReadScriptContents(Test test)
        {
            var streamInfo = System.Windows.Application.GetResourceStream(new Uri("Suites/" + this.zipFileName + ".zip", UriKind.Relative));
            using (var unzip = new SharpGIS.UnZipper(streamInfo.Stream))
            using (var reader = new StreamReader(unzip.GetFileStream(test.Path), this.ScriptEncoding))
            {
                return TransformTestScript(test, reader.ReadToEnd());
            }
        }

        /// <summary>
        /// Transforms the test script before executing it.
        /// </summary>
        /// <param name="test"> Details about the test. </param>
        /// <param name="scriptContents"> The contents of the test file. </param>
        /// <returns> The new contents of the test file. </returns>
        public virtual string TransformTestScript(Test test, string scriptContents)
        {
            return scriptContents;
        }
    }
}
