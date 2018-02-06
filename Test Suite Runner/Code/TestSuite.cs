using System;

namespace Jurassic.TestSuiteRunner
{
    /// <summary>
    /// The base class for each test suite.
    /// </summary>
    public abstract class TestSuite
    {
        /// <summary>
        /// Creates a new test suite instance using the given ID.
        /// </summary>
        /// <param name="id"> The ID of a test suite. </param>
        /// <returns> A new test suite. </returns>
        public static TestSuite FromId(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            switch (id)
            {
                case "kangax":
                    return new KangaxTestSuite();
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// The ID of the test suite.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// The name of the test suite.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Executes the test suite.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Processes worker process requests.
        /// </summary>
        /// <param name="request"> The request to process. </param>
        public abstract string ProcessRequest(string request);

        /// <summary>
        /// Updates the stats based on the outcome of a test.
        /// </summary>
        /// <param name="testOutcome"> The outcome of the test. </param>
        protected void ReportTestOutcome(TestOutcome testOutcome)
        {
            switch (testOutcome)
            {
                case TestOutcome.Success:
                    SuccessfulTestCount++;
                    break;
                case TestOutcome.Failed:
                    FailedTestCount++;
                    break;
                case TestOutcome.Skipped:
                    SkippedTestCount++;
                    break;
                default:
                    throw new NotSupportedException();
            }
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
        public int SuccessfulTestCount { get; private set; }

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
        public int FailedTestCount { get; private set; }

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
        public int SkippedTestCount { get; private set; }

        /// <summary>
        /// Gets the number of that were not executed (in the current run), as a fraction (will be
        /// between 0 and 1).
        /// </summary>
        public double SkippedPercentage
        {
            get { return this.ExecutedTestCount == 0 ? 0.0 : (double)this.SkippedTestCount / (double)this.ExecutedTestCount; }
        }
    }

}