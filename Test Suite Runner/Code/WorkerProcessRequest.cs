namespace Jurassic.TestSuite
{
    /// <summary>
    /// Commands a child process to perform some action.
    /// </summary>
    public class WorkerProcessRequest
    {
        /// <summary>
        /// The script to execute.
        /// </summary>
        public string Script { get; set; }
    }
}
