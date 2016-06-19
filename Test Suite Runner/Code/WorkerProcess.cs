using Jurassic.Library;
using System;
using System.Runtime.InteropServices;

namespace Jurassic.TestSuite
{
    /// <summary>
    /// The main worker process class.
    /// </summary>
    public class WorkerProcess
    {
        /// <summary>
        /// Starts the worker process.
        /// </summary>
        /// <param name="inPipeHandle"> The pipe handle identifier for incoming data. </param>
        /// <param name="outPipeHandle"> The pipe handle identifier for outgoing data. </param>
        public static void Start(string inPipeHandle, string outPipeHandle)
        {
            // Disable the error reporting dialog.
            NativeMethods.SetErrorMode(
                NativeMethods.SetErrorMode(0) |
                ErrorModes.SEM_NOGPFAULTERRORBOX |
                ErrorModes.SEM_FAILCRITICALERRORS |
                ErrorModes.SEM_NOOPENFILEERRORBOX);
            PipeClient<WorkerProcessRequest, WorkerProcessResponse>.Start(inPipeHandle, outPipeHandle, WorkerProcess.ProcessRequest);
        }

        /// <summary>
        /// Processes worker process requests.
        /// </summary>
        public static WorkerProcessResponse ProcessRequest(WorkerProcessRequest request)
        {
            var engine = new ScriptEngine();

            var response = new WorkerProcessResponse();
            try
            {
                // Execute the provided script.
                object result = engine.Evaluate(request.Script);
                response.JsonResult = JSONObject.Stringify(engine, result);
            }
            catch (Exception e)
            {
                // There was an error.
                response.ErrorType = e.GetType().Name;
                response.ErrorMessage = e.Message;
            }
            
            return response;
        }

        [Flags]
        private enum ErrorModes : uint
        {
            SYSTEM_DEFAULT = 0x0,
            SEM_FAILCRITICALERRORS = 0x0001,
            SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            SEM_NOOPENFILEERRORBOX = 0x8000
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            internal static extern ErrorModes SetErrorMode(ErrorModes mode);
        }
    }
}