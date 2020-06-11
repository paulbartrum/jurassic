namespace Jurassic.TestSuiteRunner
{
    /// <summary>
    /// Commands a child process to perform some action.
    /// </summary>
    public class WorkerProcessRequest
    {
        /// <summary>
        /// A collection of names of variables to return in the response.
        /// </summary>
        public string[] VariablesToReturn { get; set; }

        /// <summary>
        /// The script to execute.
        /// </summary>
        public string Script { get; set; }
    }
}
