namespace Jurassic.TestSuite
{
    /// <summary>
    /// The reply message from a worker process.
    /// </summary>
    public class WorkerProcessResponse
    {
        /// <summary>
        /// The result of executing the script (JSON encoded).
        /// </summary>
        public string JsonResult { get; set; }

        /// <summary>
        /// If there was an error, contains the type of error.
        /// </summary>
        public string ErrorType { get; set; }

        /// <summary>
        /// If there was an error, contains the error message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
