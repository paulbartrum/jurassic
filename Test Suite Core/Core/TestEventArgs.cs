using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.TestSuite
{
    /// <summary>
    /// Indicates the status of the test run, i.e. whether the test succeeded, failed or was skipped.
    /// </summary>
    public enum TestRunStatus
    {
        /// <summary>
        /// The test succeeded.
        /// </summary>
        Success,

        /// <summary>
        /// The test failed.
        /// </summary>
        Failed,

        /// <summary>
        /// The test was skipped, presumably because it is known to be bad.
        /// </summary>
        Skipped,
    }

    /// <summary>
    /// Contains information about a test run.
    /// </summary>
    public class TestEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a TestEventArgs instance.
        /// </summary>
        /// <param name="status"> A value indicating whether the test succeeded, failed or was skipped. </param>
        /// <param name="test"> The test that was excecuted. </param>
        /// <param name="compatibilityMode"> The compatibility mode the test was run under. </param>
        /// <param name="failureException"> The exception that represents the failure. </param>
        /// <returns> A TestEventArgs instance. </returns>
        public TestEventArgs(TestRunStatus status, Test test, bool strictMode, Exception failureException = null)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (status == TestRunStatus.Failed && failureException == null)
                throw new ArgumentNullException("failureException");
            if (status != TestRunStatus.Failed && failureException != null)
                throw new ArgumentException("failureException should be null", "failureException");
            this.Status = status;
            this.Test = test;
            this.StrictMode = strictMode;
            this.FailureException = failureException;
        }

        /// <summary>
        /// Gets a value indicating whether the test succeeded, failed or was skipped.
        /// </summary>
        public TestRunStatus Status { get; private set; }

        /// <summary>
        /// Gets the test that was executed.
        /// </summary>
        public Test Test { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether the test was run in strict mode.
        /// </summary>
        public bool StrictMode { get; private set; }

        /// <summary>
        /// If the test failed, contains the exception that was thrown.
        /// </summary>
        public Exception FailureException { get; private set; }
    }
}
