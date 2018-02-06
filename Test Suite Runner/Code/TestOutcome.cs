namespace Jurassic.TestSuiteRunner
{
    /// <summary>
    /// Indicates the status of the test run, i.e. whether the test succeeded, failed or was skipped.
    /// </summary>
    public enum TestOutcome
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
}
